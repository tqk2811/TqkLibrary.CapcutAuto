using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Resources
{
    public sealed class CapcutAnimationSticker : CapcutAnimation
    {
        [JsonConstructor]
        private CapcutAnimationSticker()
        {

        }
        public static CapcutAnimationSticker Parse(string json_text)
        {
            return JsonConvert.DeserializeObject<CapcutAnimationSticker>(json_text, Singleton.JsonSerializerSettings)!;
        }
    }
}
