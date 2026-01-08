using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;
using TqkLibrary.CapcutAuto.ResourceGenerate.Interfaces;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public abstract class CapcutMaterialAudio : CapcutMaterial, ICapcutPath
    {
        protected CapcutMaterialAudio(MaterialType materialType, JObject jObject) : base(jObject)
        {
            this.Type = materialType;
        }

        [JsonProperty("path")]
        [JsonConverter(typeof(CapcutPathConverter))]
        public required string Path { get; init; }

        [JsonProperty("name")]
        public required string Name { get; init; }

        [JsonProperty("duration")]
        public required TimeSpan Duration { get; init; }
    }
}
