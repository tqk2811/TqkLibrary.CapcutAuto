using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public sealed class CapcutMaterialEffectTextEffect : CapcutMaterialEffect
    {
        [JsonConstructor]
        private CapcutMaterialEffectTextEffect(JObject jObject) : base(MaterialType.text_effect, jObject)//parse from json
        {

        }

        public static CapcutMaterialEffectTextEffect Parse(string json)
        {
            return JsonConvert.DeserializeObject<CapcutMaterialEffectTextEffect>(json, Singleton.JsonSerializerSettings)!;
        }
    }
}
