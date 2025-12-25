using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public sealed class CapcutMaterialBeat : CapcutMaterial
    {
        public CapcutMaterialBeat()
        {
            Type = MaterialType.beats;
        }

        [JsonProperty("ai_beats")]
        public _AiBeats AiBeats { get; set; } = new();

        [JsonProperty("enable_ai_beats")]
        public bool EnableAiBeats { get; set; } = false;

        [JsonProperty("gear")]
        public int Gear { get; set; } = 404;

        [JsonProperty("gear_count")]
        public int GearCount { get; set; } = 0;

        [JsonProperty("mode")]
        public int Mode { get; set; } = 404;

        [JsonProperty("user_beats")]
        public List<object> UserBeats { get; set; } = new();

        [JsonProperty("user_delete_ai_beats")]
        public object? UserDeleteAiBeats { get; set; } = null;


        public class _AiBeats
        {
            [JsonProperty("beat_speed_infos")]
            public List<object> BeatSpeedInfos { get; set; } = new();

            [JsonProperty("beats_path")]
            public string BeatsPath { get; set; } = string.Empty;

            [JsonProperty("beats_url")]
            public string BeatsUrl { get; set; } = string.Empty;

            [JsonProperty("melody_path")]
            public string MelodyPath { get; set; } = string.Empty;

            [JsonProperty("melody_percents")]
            public List<double> MelodyPercents { get; set; } = new() { 0.0 };

            [JsonProperty("melody_url")]
            public string MelodyUrl { get; set; } = string.Empty;
        }
    }
}
