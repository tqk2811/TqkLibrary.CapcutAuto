using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Interfaces;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public class CapcutMaterialVideoBase : CapcutMaterial, ICapcutPath
    {
        protected CapcutMaterialVideoBase(JObject jObject) : base(jObject)
        {

        }

        [JsonProperty("path")]
        [JsonConverter(typeof(CapcutPathConverter))]
        public required string Path { get; init; }

        [JsonProperty("material_name")]
        public required string MaterialName { get; init; }

        [JsonProperty("duration")]
        public required TimeSpan Duration { get; init; }

        [JsonProperty("has_audio")]
        public required bool HasAudio { get; init; }

        [JsonProperty("width")]
        public required int Width { get; init; }

        [JsonProperty("height")]
        public required int Height { get; init; }

        [JsonProperty("local_material_id")]
        public required Guid LocalMaterialId { get; init; }
    }
}
