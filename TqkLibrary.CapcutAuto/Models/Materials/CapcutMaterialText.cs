using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.Enums;

namespace TqkLibrary.CapcutAuto.Models.Materials
{
    public class CapcutMaterialText : CapcutMaterial
    {
        [JsonConstructor]
        private CapcutMaterialText()
        {
            Type = MaterialType.text;
        }
    }
}
