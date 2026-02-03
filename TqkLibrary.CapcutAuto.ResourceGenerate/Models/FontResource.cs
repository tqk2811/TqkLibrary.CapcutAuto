using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models
{
    public class FontResource : BaseCapcut
    {
        [JsonConstructor]
        private FontResource(JObject jObject) : base(jObject)
        {

        }

        [JsonProperty("path")]
        [JsonConverter(typeof(CapcutPathConverter))]
        public required string Path { get; set; }

        [JsonProperty("resource_id")]
        public required string ResourceId { get; init; }

        public static FontResource Parse(string json)
        {
            return JsonConvert.DeserializeObject<FontResource>(json, Singleton.JsonSerializerSettings)!;
        }
    }
}
