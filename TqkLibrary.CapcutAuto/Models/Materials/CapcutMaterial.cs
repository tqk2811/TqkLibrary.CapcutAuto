using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.Enums;

namespace TqkLibrary.CapcutAuto.Models.Materials
{
    public abstract class CapcutMaterial : CapcutId
    {
        [JsonProperty("type", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public MaterialType? Type { get; protected set; }
    }
}
