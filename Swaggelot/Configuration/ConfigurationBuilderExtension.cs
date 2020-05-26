using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Swaggelot.Configuration
{
    public static class ConfigurationBuilderExtension
    {
        public static IConfigurationBuilder AddOcelotConfig(
            this IConfigurationBuilder builder,
            string folder)
        {
            var fileList = new DirectoryInfo(folder)
                .EnumerateFiles()
                .Where(fi => fi.Extension== ".json")
                .ToList();

            var result = new JObject();
            
            fileList.ForEach(x =>
            {
                var content = File.ReadAllText(x.FullName);
                var jObject = JObject.Parse(content);
                result.Merge(jObject, new JsonMergeSettings()
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
                
            });
            
            File.WriteAllText("ocelot.json", result.ToString());
            builder.AddJsonFile("ocelot.json", false, false);
            return builder;
        }
    }
}