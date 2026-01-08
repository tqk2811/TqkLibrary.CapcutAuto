using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Interfaces;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public sealed class CapcutMaterialSticker : CapcutMaterial, ICapcutPath
    {
        [JsonConstructor]
        private CapcutMaterialSticker(JObject jObject) : base(jObject)
        {

        }

        [JsonProperty("path")]
        [JsonConverter(typeof(CapcutPathConverter))]
        public required string Path { get; init; }


        public static CapcutMaterialSticker Parse(string json)
        {
            return JsonConvert.DeserializeObject<CapcutMaterialSticker>(json, Singleton.JsonSerializerSettings)!;
        }
    }
}
