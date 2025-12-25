using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public sealed class CapcutMaterialVocalSeparation : CapcutMaterial
    {
        public CapcutMaterialVocalSeparation()
        {
            Type = MaterialType.vocal_separation;
        }

        [JsonProperty("choice")]
        public int Choice { get; set; } = 0;

        [JsonProperty("enter_from")]
        public string EnterFrom { get; set; } = string.Empty;

        [JsonProperty("final_algorithm")]
        public string FinalAlgorithm { get; set; } = string.Empty;

        [JsonProperty("production_path")]
        public string ProductionPath { get; set; } = string.Empty;

        [JsonProperty("removed_sounds")]
        public List<object> RemovedSounds { get; set; } = new();

        [JsonProperty("time_range")]
        public object? TimeRange { get; set; }
    }
}
