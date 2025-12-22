using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.Enums;
using TqkLibrary.CapcutAuto.JsonConverters;

namespace TqkLibrary.CapcutAuto.Models.Materials
{
    [JsonConverter(typeof(CapcutIdConverter<CapcutMaterialEffect>))]
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
