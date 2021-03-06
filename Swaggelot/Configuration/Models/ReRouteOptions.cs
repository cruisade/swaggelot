using System;
using System.Collections.Generic;

namespace Swaggelot.Models
{
    public class ReRouteOptions
    {
        /// <summary>
        /// Swagger key. This key is used for generating swagger documentation for downstream services.
        /// The same key have to be in <see cref="SwaggerEndPointOptions"/> collection.
        /// </summary>
        public string SwaggerKey { get; set; }

        /// <summary>
        /// Gets or sets the downstream path template.
        /// </summary>
        public string DownstreamPathTemplate { get; set; }

        /// <summary>
        /// Gets or sets the upstream path template.
        /// </summary>
        public string UpstreamPathTemplate { get; set; }

        /// <summary>
        /// Gets or sets the upstream HTTP method.
        /// </summary>
        public IEnumerable<string> UpstreamHttpMethod { get; set; }

        /// <summary>
        /// Gets or sets the virtual directory, where is host service.
        /// </summary>
        /// <remarks>Default value is <see langword="null"/>.</remarks>
        public string VirtualDirectory { get; set; }

        /// <summary>
        /// Gets the downstream path.
        /// </summary>
        public string DownstreamPath
        {
            get
            {
                var ret = DownstreamPathTemplate;
                if (!string.IsNullOrWhiteSpace(VirtualDirectory)
                    && ret.StartsWith(VirtualDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    ret = ret.Substring(VirtualDirectory.Length);
                }

                return ret;
            }
        }

        public Dictionary<string, string> ChangeDownstreamPathTemplate { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> AddQueriesToRequest { get; set; } = new Dictionary<string, string>();
        public AuthenticationOptions AuthenticationOptions { get; set; }
    }

    public class AuthenticationOptions
    {
        public string AuthenticationProviderKey { get; set; }
    }
}