using FFMpegCore;
using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.Enums;

namespace TqkLibrary.CapcutAuto.Models.Materials
{
    public class CapcutMaterialVideo : CapcutMaterial
    {
        private CapcutMaterialVideo()
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
                throw new InvalidOperationException($"Video had no VideoStream");

            return new CapcutMaterialVideo()
            {
                Path = fileInfo.FullName,
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
