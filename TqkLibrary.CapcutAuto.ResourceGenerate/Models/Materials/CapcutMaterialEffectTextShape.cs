using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public sealed class CapcutMaterialEffectTextShape : CapcutMaterialEffect
    {
        [JsonConstructor]
        private CapcutMaterialEffectTextShape(JObject jObject) : base(MaterialType.text_shape, jObject)//parse from json
        {

        }

        public static CapcutMaterialEffectTextShape Parse(string json)
        {
            return JsonConvert.DeserializeObject<CapcutMaterialEffectTextShape>(json, Singleton.JsonSerializerSettings)!;
        }
    }
}
