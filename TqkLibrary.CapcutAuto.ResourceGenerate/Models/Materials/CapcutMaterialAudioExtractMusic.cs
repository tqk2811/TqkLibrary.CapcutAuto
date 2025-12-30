using FFMpegCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public class CapcutMaterialAudioExtractMusic : CapcutMaterial
    {
        private CapcutMaterialAudioExtractMusic(JObject jObject) : base(jObject)
        {
            this.Type = MaterialType.extract_music;
        }

        [JsonProperty("path")]
        public required string Path { get; init; }

        [JsonProperty("name")]
        public required string Name { get; init; }

        [JsonProperty("duration")]
        public required TimeSpan Duration { get; init; }

        [JsonProperty("local_material_id")]
        public required Guid LocalMaterialId { get; init; }

        internal static CapcutMaterialAudioExtractMusic Create(DraftMetaInfo.DraftMaterialValueAudio draftMaterialValueAudio)
        {
            string json = Extensions.GetEmbeddedResourceString("Materials.Audio.json");
            JObject jObject = JObject.Parse(json);
            return new CapcutMaterialAudioExtractMusic(jObject)
            {
                Duration = draftMaterialValueAudio.Duration,
                Name = draftMaterialValueAudio.ExtraInfo,
                Path = draftMaterialValueAudio.FilePath,
                LocalMaterialId = draftMaterialValueAudio.Id,
            };
        }
    }
}
