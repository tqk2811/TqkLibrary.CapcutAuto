using FFMpegCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TqkLibrary.CapcutAuto.Enums;
using TqkLibrary.CapcutAuto.JsonConverters;

namespace TqkLibrary.CapcutAuto.Models.Materials
{
    public class CapcutMaterialVideo : CapcutMaterial
    {
        private CapcutMaterialVideo(JObject jObject) : base(jObject)
        {
            this.Type = MaterialType.video;
        }

        [JsonProperty("path")]
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


        public static async Task<CapcutMaterialVideo> CreateAsync(string videoFilePath, CancellationToken cancellationToken = default)
        {
            FileInfo fileInfo = new(videoFilePath);
            if (!fileInfo.Exists)
                throw new FileNotFoundException(videoFilePath);
            IMediaAnalysis mediaAnalysis = await FFProbe.AnalyseAsync(videoFilePath, cancellationToken: cancellationToken);
            if (mediaAnalysis.PrimaryVideoStream is null)
                throw new InvalidOperationException($"File had no VideoStream");

            string json = Extensions.GetEmbeddedResource("Materials.Video.json");
            JObject jObject = JObject.Parse(json);
            return new CapcutMaterialVideo(jObject)
            {
                Path = fileInfo.FullName.Replace('\\', '/'),
                Type = MaterialType.video,
                MaterialName = fileInfo.Name,
                Duration = mediaAnalysis.Duration,
                HasAudio = mediaAnalysis.PrimaryAudioStream is not null,
                Width = mediaAnalysis.PrimaryVideoStream!.Width,
                Height = mediaAnalysis.PrimaryVideoStream!.Height,
            };
        }
    }
}
