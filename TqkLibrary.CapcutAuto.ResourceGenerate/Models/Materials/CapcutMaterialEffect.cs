using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public class CapcutMaterialEffect : CapcutMaterial
    {
        [JsonConstructor]
        private CapcutMaterialEffect(JObject jObject) : base(jObject)//parse from json
        {
            Type = MaterialType.text_effect;
        }


        [JsonProperty("path")]
        [JsonConverter(typeof(CapcutPathConverter))]
        public required string Path { get; init; }

        public static CapcutMaterialEffect Parse(string json)
        {
            return JsonConvert.DeserializeObject<CapcutMaterialEffect>(json, Singleton.JsonSerializerSettings)!;
        }
    }
}
