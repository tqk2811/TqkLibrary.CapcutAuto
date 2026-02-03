using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public sealed class CapcutMaterialAudioExtractMusic : CapcutMaterialAudio
    {
        [JsonConstructor]
        private CapcutMaterialAudioExtractMusic(JObject jObject) : base(MaterialType.extract_music, jObject)
        {
        }


        [JsonProperty("local_material_id")]
        public required Guid LocalMaterialId { get; init; }

        internal static CapcutMaterialAudioExtractMusic Create(DraftMetaInfo.DraftMaterialValueAudio draftMaterialValueAudio)
        {
            string json = Extensions.GetEmbeddedResourceString("Materials.Audio.json");
            JObject jObject = JObject.Parse(json);
            return new CapcutMaterialAudioExtractMusic(jObject)
            {
                LocalMaterialId = draftMaterialValueAudio.Id,
                Duration = draftMaterialValueAudio.Duration,
                Name = draftMaterialValueAudio.ExtraInfo,
                Path = draftMaterialValueAudio.FilePath,
            };
        }
    }
}
