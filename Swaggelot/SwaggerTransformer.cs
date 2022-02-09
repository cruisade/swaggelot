using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
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
        private readonly ILogger _logger;

        private Dictionary<SwaggerDescriptor, OpenApiDocument> _innerDocs;
        private Dictionary<SwaggerDescriptor, JToken> _jsonDocs;
        private OpenApiDocument _document;

        public SwaggerTransformer(
            IOpenApiCollector collector,
            IOptions<List<ReRouteOptions>> reRoutes,
            IOptions<SwaggerSettings> swaggerSettings,
            ILogger<SwaggerTransformer> logger)
        {
            _collector = collector;
            _logger = logger;
            _authUrl = swaggerSettings.Value.Auth?.TokenUrl;
            _reRoutes = reRoutes.Value;
        }

        public async Task<string> Transform()
        {
            var docs = await _collector.CollectDownstreamSwaggersAsync();
            _jsonDocs = docs.ToDictionary(
                x => x.Key,
                x => x.Value.Item2 == null ? null : JToken.Parse(x.Value.Item2));
            _innerDocs = docs.ToDictionary(x => x.Key, x => x.Value.Item1);
            _document = OpenApiBuilder
                .Create()
                .WithOAuth(_authUrl)
                .WithSchemes(GetSchemes().GroupBy(s => s.Key).Select(g => g.First()).ToArray())
                .WithTags(GetTags())
                .Build();

            foreach (var route in _reRoutes)
            {
                ProcessReroute(route);
            }

            var json =  _document.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json);
            return FixExamples(json);
        }

        private string FixExamples(string json)
        {
            var parsed = JToken.Parse(json);
            var tokensToFix = parsed.SelectTokens("$..requestBody..content..examples");
            var sourceExample = _jsonDocs.Values
                .Where(x => x != null)
                .SelectMany(x => x.SelectTokens($"$..requestBody..content..examples"))
                .GroupBy(x=>((JProperty) x.First).Name)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());
            foreach (var token in tokensToFix)
            {
                var obj = (JObject) token;
                var exampleName = ((JProperty) obj.First).Name;
                if (sourceExample.ContainsKey(exampleName))
                    token[exampleName] = sourceExample[exampleName][exampleName];
            }

            return parsed.ToString();
        }

        private IEnumerable<(string Key, OpenApiSchema Value)> GetSchemes()
        {
            foreach (var innerSwaggersValue in _innerDocs.Values)
            {
                if (innerSwaggersValue == null)
                    continue;

                foreach (var openApiSchema in innerSwaggersValue.Components.Schemas)
                {
                    yield return (openApiSchema.Key, openApiSchema.Value);
                }
            }
        }
        
        private IEnumerable<OpenApiTag> GetTags()
        {
            foreach (var innerSwaggersValue in _innerDocs.Values)
            {
                if (innerSwaggersValue == null)
                    continue;

                foreach (var tag in innerSwaggersValue.Tags)
                {
                    yield return tag;
                }
            }
        }

        private void ProcessWithInnerSwagger(
            ReRouteOptions route,
            SwaggerDescriptor descriptor,
            OpenApiDocument innerDocument)
        {
            if (!route.DownstreamPathTemplate.Contains(Constants.EverythingAnchor))
            {
                ProcessWithInnerSwaggerForSingleDestinationDownstream(route, descriptor, innerDocument);
                return;
            }

            if (!route.UpstreamPathTemplate.Contains(Constants.EverythingAnchor))
            {
                throw new InvalidOperationException(
                    $"Can't map upstream ({route.UpstreamPathTemplate}) to downstream ({route.DownstreamPathTemplate})");
            }

            ProcessWithInnerSwaggerForManyDestinationDownstream(route, descriptor, innerDocument);
        }


        private void ProcessWithInnerSwaggerForSingleDestinationDownstream(
            ReRouteOptions route,
            SwaggerDescriptor descriptor,
            OpenApiDocument innerDocument)
        {
            var version = descriptor.Version.ToString();
            var upstreamTemplate = ReplaceVersion(route.UpstreamPathTemplate, version);
            var downstreamTemplate = ReplaceVersion(route.DownstreamPathTemplate, version);

            if (!innerDocument.Paths
                .Select(s => ReplaceVersion(s.Key, version).ToLower())
                .Contains(downstreamTemplate.ToLower()))
                return;

            var path = innerDocument.Paths.FirstOrDefault(p =>
                string.Equals(
                    ReplaceVersion(p.Key, version),
                    downstreamTemplate,
                    StringComparison.CurrentCultureIgnoreCase));


            var operations = GetOperationsForPath(route, path.Value);

            if (operations != null)
            {
                AddAuth(route, operations);
                AddOperations(upstreamTemplate, operations);
            }
        }

        private void ProcessWithInnerSwaggerForManyDestinationDownstream(
            ReRouteOptions route,
            SwaggerDescriptor descriptor,
            OpenApiDocument innerDocument)
        {
            var version = descriptor.Version.ToString();
            var upstreamTemplate = RemoveEverything(ReplaceVersion(route.UpstreamPathTemplate, version));
            var downstreamTemplate = RemoveEverything(ReplaceVersion(route.DownstreamPathTemplate, version));

            var reRoutedPaths = innerDocument.Paths
                .Select(p => new
                {
                    path = ReplaceVersion(p.Key, version),
                    openApiPathItem = p.Value
                })
                .Where(p => p.path.StartsWith(downstreamTemplate, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(p => p.path.Replace(downstreamTemplate, upstreamTemplate), p => p.openApiPathItem);

            foreach (var path in reRoutedPaths)
            {
                var operations = GetOperationsForPath(route, path.Value);
                if (operations != null)
                {
                    AddAuth(route, operations);
                    AddOperations(path.Key, operations);
                }
            }
        }

        private string ReplaceVersion(string value, string version) => value.Replace(Constants.VersionAnchor, version,
            StringComparison.OrdinalIgnoreCase);

        private string RemoveEverything(string value) =>
            value.Replace("{everything}", string.Empty, StringComparison.OrdinalIgnoreCase);


        private OpenApiPathItem GetOperationsForPath(ReRouteOptions route, OpenApiPathItem path)
        {
            IDictionary<OperationType, OpenApiOperation> operationsByTypes = route.UpstreamHttpMethod == null
                ? path.Operations
                : route.UpstreamHttpMethod
                    .Select(x => Enum.TryParse<OperationType>(x, out var result) ? result : (OperationType?) null)
                    .OfType<OperationType>()
                    .ToDictionary(
                        x => x,
                        x => path.Operations.TryGetValue(x, out var result) ? result : null);


            var operations = new OpenApiPathItem();
            foreach (var operationItem in operationsByTypes)
            {
                var typedVerb = operationItem.Key;
                var operation = operationItem.Value;
                if (operation == null)
                {
                    _logger.LogWarning($"{typedVerb} not found for {route.DownstreamPath}");
                    return null;
                }

                var predefinedParams =
                    route.ChangeDownstreamPathTemplate.Keys.Select(s => s.ToLower()).ToList();
                predefinedParams.Add(Constants.VersionVariableName);

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
            if (!string.IsNullOrEmpty(route.AuthenticationOptions?.AuthenticationProviderKey))
            {
                foreach (var operation in operations.Operations)
                {
                    operation.Value.Security = new List<OpenApiSecurityRequirement>();
                    operation.Value.Security.Add(req);

                    operation.Value.Responses.TryAdd("401", new OpenApiResponse()
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
                .Where(kv => kv.Value != null)
                .ForEach(tuple => ProcessWithInnerSwagger(route, tuple.Key, tuple.Value));
        }
    }
}