using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public sealed class CapcutMaterialSoundChannelMapping : CapcutMaterial
    {
        public CapcutMaterialSoundChannelMapping()
        {
            Type = MaterialType.none;
        }

        [JsonProperty("audio_channel_mapping")]
        public int AudioChannelMapping { get; set; } = 0;

        [JsonProperty("is_config_open")]
        public bool IsConfigOpen { get; set; } = false;
    }
}
