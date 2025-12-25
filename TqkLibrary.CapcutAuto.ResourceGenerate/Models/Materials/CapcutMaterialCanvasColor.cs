using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public sealed class CapcutMaterialCanvasColor : CapcutMaterial
    {
        public CapcutMaterialCanvasColor()
        {
            Type = MaterialType.canvas_color;
        }


        [JsonProperty("album_image")]
        public string AlbumImage { get; set; } = string.Empty;

        [JsonProperty("blur")]
        public double Blur { get; set; } = 0.0;

        [JsonProperty("color")]
        public string Color { get; set; } = string.Empty;

        [JsonProperty("image")]
        public string Image { get; set; } = string.Empty;

        [JsonProperty("image_id")]
        public string ImageId { get; set; } = string.Empty;

        [JsonProperty("image_name")]
        public string ImageName { get; set; } = string.Empty;

        [JsonProperty("source_platform")]
        public int SourcePlatform { get; set; } = 0;

        [JsonProperty("team_id")]
        public string TeamId { get; set; } = string.Empty;
    }
}
