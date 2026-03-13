using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models
{
    public class TextCurve
    {
        [JsonProperty("angle")]
        public double Angle { get; set; } = 72.0;

        [JsonProperty("enable")]
        public bool Enable { get; set; } = false;
    }
}
