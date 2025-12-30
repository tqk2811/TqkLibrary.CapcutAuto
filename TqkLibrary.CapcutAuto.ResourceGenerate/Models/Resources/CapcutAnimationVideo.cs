using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Resources
{
    public sealed class CapcutAnimationVideo : CapcutAnimation
    {
        [JsonConstructor]
        private CapcutAnimationVideo()
        {

        }
        public static CapcutAnimationVideo Parse(string json_text)
        {
            return JsonConvert.DeserializeObject<CapcutAnimationVideo>(json_text, Singleton.JsonSerializerSettings)!;
        }
    }
}
