using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Swaggelot
{
    public class OpenApiBuilder
    {
        private readonly OpenApiDocument _document;

        public OpenApiDocument Build()
        {
            return _document;
        }

        private OpenApiBuilder()
        {
            _document = InitDocument();
        }

        public static OpenApiBuilder Create()
        {
            return new OpenApiBuilder();
        }

        public OpenApiBuilder WithOAuth(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                _document.Components.SecuritySchemes.Add("oauth2", GetOAuthPasswordFlowScheme(url));
            }

            return this;
        }

        public OpenApiBuilder WithSchemes(params (string, OpenApiSchema)[] schemes)
        {
            foreach (var tuple in schemes)
            {
                _document.Components.Schemas.Add(tuple.Item1, tuple.Item2);
            }

            return this;
        }
        
        public OpenApiBuilder WithTags(IEnumerable<OpenApiTag> tags)
        {
            foreach (var tag in tags)
            {
                _document.Tags.Add(tag);
            }

            return this;
        }

        private static OpenApiDocument InitDocument()
        {
            return new OpenApiDocument
            {
                Info = new OpenApiInfo()
                {
                    Version = "1.0.0",
                    Title = "Swagger",
                },
                Paths = new OpenApiPaths(),
                Components = new OpenApiComponents()
                {
                    Schemas = new Dictionary<string, OpenApiSchema>(),
                    SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>()
                },
            };
        }

        private static OpenApiSecurityScheme GetOAuthPasswordFlowScheme(string url)
        {
            return new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Password = new OpenApiOAuthFlow()
                    {
                        TokenUrl = new Uri(url),
                    },
                    ClientCredentials = new OpenApiOAuthFlow()
                    {
                        TokenUrl = new Uri(url)
                    }
                },
                In = ParameterLocation.Header,
            };
        }
    }
}