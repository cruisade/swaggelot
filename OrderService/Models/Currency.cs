using System.Text.Json.Serialization;

namespace OrderService.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Currency
    {
        Ru = 1,
        Eu = 2,
        Usd = 3
    }
}