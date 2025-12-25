using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public class CapcutMaterialSpeed : CapcutMaterial
    {
        public CapcutMaterialSpeed()
        {
            this.Type = MaterialType.speed;
        }

        [JsonProperty("mode")]
        public int Mode { get; set; } = 0;

        [JsonProperty("speed")]
        public double Speed { get; set; } = 1.0;
    }
}
