using FFMpegCore;
using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.Enums;

namespace TqkLibrary.CapcutAuto.Models.Materials
{
    public class CapcutMaterialAudio : CapcutMaterial
    {
        private CapcutMaterialAudio()
        {
            this.Type = MaterialType.extract_music;
        }

        [JsonProperty("path")]
        public required string Path { get; init; }

        [JsonProperty("name")]
        public required string Name { get; init; }

        [JsonProperty("duration")]
        public required TimeSpan Duration { get; init; }

        public static async Task<CapcutMaterialAudio> CreateAsync(string audioFilePath, CancellationToken cancellationToken = default)
        {
            FileInfo fileInfo = new(audioFilePath);
            if (!fileInfo.Exists)
                throw new FileNotFoundException(audioFilePath);
            IMediaAnalysis mediaAnalysis = await FFProbe.AnalyseAsync(audioFilePath, cancellationToken: cancellationToken);
            if (mediaAnalysis.PrimaryAudioStream is null)
                throw new InvalidOperationException($"File had no AudioStream");

            return new CapcutMaterialAudio()
            {
                Duration = mediaAnalysis.Duration,
                Name = fileInfo.Name,
                Path = fileInfo.FullName.Replace('\\', '/'),
            };
        }

    }
}
