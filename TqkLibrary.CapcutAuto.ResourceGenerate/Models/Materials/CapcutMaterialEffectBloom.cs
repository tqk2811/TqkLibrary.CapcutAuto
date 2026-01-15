using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public sealed class CapcutMaterialEffectBloom : CapcutMaterialEffect
    {
        [JsonConstructor]
        private CapcutMaterialEffectBloom(JObject jObject) : base(MaterialType.bloom, jObject)//parse from json
        {

        }

        public static CapcutMaterialEffectBloom Parse(string json)
        {
            return JsonConvert.DeserializeObject<CapcutMaterialEffectBloom>(json, Singleton.JsonSerializerSettings)!;
        }

    }
}
