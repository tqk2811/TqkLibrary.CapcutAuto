using FFMpegCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public class CapcutMaterialVideo : CapcutMaterial
    {
        private CapcutMaterialVideo(JObject jObject) : base(jObject)
        {
            this.Type = MaterialType.video;
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

        //public static async Task<CapcutMaterialVideo> CreateAsync(string videoFilePath, CancellationToken cancellationToken = default)
        //{
        //    FileInfo fileInfo = new(videoFilePath);
        //    if (!fileInfo.Exists)
        //        throw new FileNotFoundException(videoFilePath);
        //    IMediaAnalysis mediaAnalysis = await FFProbe.AnalyseAsync(videoFilePath, cancellationToken: cancellationToken);
        //    if (mediaAnalysis.PrimaryVideoStream is null)
        //        throw new InvalidOperationException($"File had no VideoStream");

        //    string json = Extensions.GetEmbeddedResource("Materials.Video.json");
        //    JObject jObject = JObject.Parse(json);
        //    return new CapcutMaterialVideo(jObject)
        //    {
        //        Path = fileInfo.FullName,
        //        Type = MaterialType.video,
        //        MaterialName = fileInfo.Name,
        //        Duration = mediaAnalysis.Duration,
        //        HasAudio = mediaAnalysis.PrimaryAudioStream is not null,
        //        Width = mediaAnalysis.PrimaryVideoStream!.Width,
        //        Height = mediaAnalysis.PrimaryVideoStream!.Height,
        //    };
        //}

        internal static CapcutMaterialVideo Create(DraftMetaInfo.DraftMaterialValueVideo draftMaterialValueVideo)
        {
            string json = Extensions.GetEmbeddedResourceString("Materials.Video.json");
            JObject jObject = JObject.Parse(json);
            return new CapcutMaterialVideo(jObject)
            {
                Duration = draftMaterialValueVideo.Duration,
                HasAudio = draftMaterialValueVideo.HasAudio,
                Height = draftMaterialValueVideo.Height,
                Width = draftMaterialValueVideo.Width,
                MaterialName = draftMaterialValueVideo.ExtraInfo,
                Path = draftMaterialValueVideo.FilePath,
                LocalMaterialId = draftMaterialValueVideo.Id,
            };
        }
    }
}
