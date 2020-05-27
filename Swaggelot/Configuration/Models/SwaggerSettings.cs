using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Swaggelot.Models
{
    public class SwaggerSettings
    {
        public const string ConfigurationSectionName = "SwaggerSettings";
        
        public AuthSettings Auth { get; set; }
        public List<SwaggerEndPointOptions> Endpoints { get; set; }
    }

    public class AuthSettings
    {
        public string TokenUrl { get; set; }
    }
}