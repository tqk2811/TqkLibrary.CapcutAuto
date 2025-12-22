using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.Models.Materials
{
    public sealed class CapcutMaterialColor : CapcutId
    {
        public CapcutMaterialColor()
        {

        }

        [JsonProperty("gradient_angle")]
        public double GradientAngle { get; set; } = 90.0;

        [JsonProperty("gradient_colors")]
        public List<object> GradientColors { get; set; } = new();

        [JsonProperty("gradient_percents")]
        public List<object> GradientPercents { get; set; } = new();

        [JsonProperty("height")]
        public double Height { get; set; } = 0.0;

        [JsonProperty("is_color_clip")]
        public bool IsColorClip { get; set; } = false;

        [JsonProperty("is_gradient")]
        public bool IsGradient { get; set; } = false;

        [JsonProperty("solid_color")]
        public string SolidColor { get; set; } = string.Empty;

        [JsonProperty("width")]
        public double Width { get; set; } = 0.0;
    }
}
