using FFMpegCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.Enums;

namespace TqkLibrary.CapcutAuto.Models.Materials
{
    public class CapcutMaterialAudio : CapcutMaterial
    {
        private CapcutMaterialAudio(JObject jObject) : base(jObject)
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

        //public static async Task<CapcutMaterialAudio> CreateAsync(string audioFilePath, CancellationToken cancellationToken = default)
        //{
        //    FileInfo fileInfo = new(audioFilePath);
        //    if (!fileInfo.Exists)
        //        throw new FileNotFoundException(audioFilePath);
        //    IMediaAnalysis mediaAnalysis = await FFProbe.AnalyseAsync(audioFilePath, cancellationToken: cancellationToken);
        //    if (mediaAnalysis.PrimaryAudioStream is null)
        //        throw new InvalidOperationException($"File had no AudioStream");

        //    string json = Extensions.GetEmbeddedResource("Materials.Audio.json");
        //    JObject jObject = JObject.Parse(json);
        //    return new CapcutMaterialAudio(jObject)
        //    {
        //        Duration = mediaAnalysis.Duration,
        //        Name = fileInfo.Name,
        //        Path = fileInfo.FullName.Replace('\\', '/'),
        //    };
        //}

        internal static CapcutMaterialAudio Create(DraftMetaInfo.DraftMaterialValueAudio draftMaterialValueAudio)
        {
            string json = Extensions.GetEmbeddedResourceString("Materials.Audio.json");
            JObject jObject = JObject.Parse(json);
            return new CapcutMaterialAudio(jObject)
            {
                Duration = draftMaterialValueAudio.Duration,
                Name = draftMaterialValueAudio.ExtraInfo,
                Path = draftMaterialValueAudio.FilePath,
                LocalMaterialId = draftMaterialValueAudio.Id,
            };
        }
    }
}
