using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace Swaggelot.OpenApiCollector
{
    public interface IOpenApiCollector
    {
        Task<Dictionary<SwaggerDescriptor, OpenApiDocument>> CollectDownstreamSwaggersAsync();
    }

    public struct SwaggerDescriptor
    {
        public SwaggerDescriptor(string name, int version)
        {
            Name = name;
            Version = version;
        }
        
        public string Name { get; set; }
        public int Version { get; set; }
    }
}