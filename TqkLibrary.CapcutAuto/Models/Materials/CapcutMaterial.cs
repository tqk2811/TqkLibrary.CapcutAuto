using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.Enums;

namespace TqkLibrary.CapcutAuto.Models.Materials
{
    public abstract class CapcutMaterial : CapcutId
    {
        [JsonProperty("type")]
        public MaterialType Type { get; protected set; }
    }
}
