using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Resources
{
    public sealed class CapcutAnimationText : CapcutAnimation
    {
        [JsonConstructor]
        private CapcutAnimationText()
        {

        }
        public static CapcutAnimationText Parse(string json_text)
        {
            return JsonConvert.DeserializeObject<CapcutAnimationText>(json_text, Singleton.JsonSerializerSettings)!;
        }
    }
}
