using System.Collections.Generic;

namespace Swaggelot.Models
{
    public class SwaggerEndPointOptions
    {
        /// <summary>
        /// Swagger Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The swagger endpoint config collection
        /// </summary>
        public List<SwaggerEndPointConfig> Versions { get; set; }
    }
}