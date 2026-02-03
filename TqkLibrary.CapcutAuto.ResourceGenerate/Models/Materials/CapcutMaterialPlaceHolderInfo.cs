using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public sealed class CapcutMaterialPlaceHolderInfo : CapcutMaterial
    {
        public CapcutMaterialPlaceHolderInfo()
        {
            Type = MaterialType.placeholder_info;
        }

        [JsonProperty("error_path")]
        public string ErrorPath { get; set; } = string.Empty;

        [JsonProperty("error_text")]
        public string ErrorText { get; set; } = string.Empty;

        [JsonProperty("meta_type")]
        public string MetaType { get; set; } = "none";

        [JsonProperty("res_path")]
        public string ResPath { get; set; } = string.Empty;

        [JsonProperty("res_text")]
        public string ResText { get; set; } = string.Empty;
    }
}
