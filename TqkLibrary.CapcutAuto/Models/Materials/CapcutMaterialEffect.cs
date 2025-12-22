using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.Enums;

namespace TqkLibrary.CapcutAuto.Models.Materials
{
    public class CapcutMaterialEffect : CapcutMaterial
    {
        [JsonConstructor]
        private CapcutMaterialEffect()//parse from json
        {
            Type = MaterialType.text_effect;
        }
    }
}
