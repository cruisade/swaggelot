using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swaggelot.Extensions;
using Swaggelot.Models;
using Swaggelot.OpenApiCollector;

namespace Swaggelot
{
    public class SwaggerTransformer : ISwaggerTransformer
    {
        private readonly IOpenApiCollector _collector;
        private readonly IEnumerable<ReRouteOptions> _reRoutes;
        private readonly string _authUrl;
        private Dictionary<SwaggerDescriptor, OpenApiDocument> _innerDocs;
        private OpenApiDocument _document;

        public SwaggerTransformer(
            IOpenApiCollector collector,
            IOptions<List<ReRouteOptions>> reRoutes,
            IOptions<SwaggerSettings> swaggerSettings)
        {
            _collector = collector;
            _authUrl = swaggerSettings.Value.Auth?.TokenUrl;
            _reRoutes = reRoutes.Value;
        }

        public async Task<string> Transform()
        {
            _innerDocs = await _collector.CollectDownstreamSwaggersAsync();
            _document = OpenApiBuilder
                .Create()
                .WithOAuth(_authUrl)
                .WithSchemes(GetSchemes().GroupBy(s => s.Key).Select(g => g.First()).ToArray())
                .Build();

            foreach (var route in _reRoutes)
            {
                ProcessReroute(route);
            }

            return _document.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json);
        }

        private IEnumerable<(string Key, OpenApiSchema Value)> GetSchemes()
        {
            foreach (var innerSwaggersValue in _innerDocs.Values)
            {
                if (innerSwaggersValue==null)
                    continue;

                foreach (var openApiSchema in innerSwaggersValue.Components.Schemas)
                {
                    yield return (openApiSchema.Key, openApiSchema.Value);
                }
            }
        }

        private void ProcessWithInnerSwagger(
            ReRouteOptions route,
            SwaggerDescriptor descriptor,
            OpenApiDocument innerDocument)
        {
            var version = descriptor.Version.ToString();
            var upstreamTemplate = route.UpstreamPathTemplate.Replace(VersionConstants.VersionAnchor, version);
            var downstreamTemplate = route.DownstreamPathTemplate.Replace(VersionConstants.VersionAnchor, version);

            if (!innerDocument.Paths
                .Select(s => s.Key.Replace(VersionConstants.VersionAnchor, version).ToLower())
                .Contains(downstreamTemplate.ToLower()))
                return;

            var path = innerDocument.Paths.FirstOrDefault(p =>
                string.Equals(
                    p.Key.Replace(VersionConstants.VersionAnchor, version),
                    downstreamTemplate,
                    StringComparison.CurrentCultureIgnoreCase));


            var operations = GetOperationsForPath(route, path.Value);
            
            if (operations != null)
            {
                AddAuth(route, operations);
                AddOperations(upstreamTemplate, operations);
            }
        }

        private OpenApiPathItem GetOperationsForPath(ReRouteOptions route, OpenApiPathItem path)
        {
            var operations = new OpenApiPathItem();
            foreach (var verb in route.UpstreamHttpMethod)
            {
                Enum.TryParse<OperationType>(verb, out var typedVerb);

                if (!path.Operations.ContainsKey(typedVerb))
                {
                    //todo log
                    Console.WriteLine($"{typedVerb} not found for {route.DownstreamPath}");
                    return null;
                }

                var operation = path.Operations[typedVerb];

                var predefinedParams =
                    route.ChangeDownstreamPathTemplate.Keys.Select(s => s.ToLower()).ToList();
                predefinedParams.Add(VersionConstants.VersionVariableName);

                var filteredParameters = operation.Parameters
                    .Where(p => !predefinedParams.Contains(p.Name.ToLower()))
                    .Where(p => !route.AddQueriesToRequest.Keys.Select(k => k.ToLower()).Contains(p.Name.ToLower()));

                operation.Parameters = filteredParameters.ToList();

                operations.AddOperation(typedVerb, operation);
            }

            return operations;
        }

        private void AddOperations(string upstreamTemplate, OpenApiPathItem operations)
        {
            if (!_document.Paths.ContainsKey(upstreamTemplate))
                _document.Paths.Add(upstreamTemplate, operations);
            else
            {
                operations.Operations.ForEach(operation =>
                    _document.Paths[upstreamTemplate].AddOperation(operation.Key, operation.Value));
            }
        }

        private void AddAuth(ReRouteOptions route, OpenApiPathItem operations)
        {
            var req = new OpenApiSecurityRequirement();
            var sch = new OpenApiSecurityScheme();
            sch.Reference = new OpenApiReference()
            {
                Id = "oauth2",
                Type = ReferenceType.SecurityScheme
            };
            req.TryAdd(sch, new List<string>());
            if (route.AuthenticationOptions?.AuthenticationProviderKey == "Bearer")
            {
                foreach (var operation in operations.Operations)
                {
                    operation.Value.Security = new List<OpenApiSecurityRequirement>();
                    operation.Value.Security.Add(req);

                    operation.Value.Responses.Add("401", new OpenApiResponse()
                    {
                        Description = "Unauthorized"
                    });
                }
            }
        }

        private void ProcessReroute(ReRouteOptions route)
        {
            if (string.IsNullOrEmpty(route.SwaggerKey)) return;

            var keysForInnerDoc = _innerDocs.Keys.Where(x => x.Name == route.SwaggerKey);

            _innerDocs
                .Where(kv => keysForInnerDoc.Contains(kv.Key))
                .ForEach(tuple => ProcessWithInnerSwagger(route, tuple.Key, tuple.Value));
        }
    }
}