using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public abstract class CapcutMaterialEffect : CapcutMaterial
    {
        protected CapcutMaterialEffect(MaterialType materialType, JObject jObject) : base(jObject)//parse from json
        {
            Type = materialType;
        }


        [JsonProperty("path")]
        [JsonConverter(typeof(CapcutPathConverter))]
        public required string Path { get; init; }

    }
}
