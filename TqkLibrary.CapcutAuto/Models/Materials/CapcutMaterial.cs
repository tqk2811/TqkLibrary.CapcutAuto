using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.Enums;

namespace TqkLibrary.CapcutAuto.Models.Materials
{
    public abstract class CapcutMaterial : CapcutId
    {
        protected CapcutMaterial()
        {

        }
        protected CapcutMaterial(JObject jObject) : base(jObject)
        {

        }

        [JsonProperty("type", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public MaterialType? Type { get; protected set; }
    }
}
