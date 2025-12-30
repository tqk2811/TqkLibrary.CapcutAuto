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
        public static async Task<IReadOnlyList<CapcutAnimationVideo>> FromDirAsync(string dir)
        {
            List<CapcutAnimationVideo> capcutAnimations = new List<CapcutAnimationVideo>();
            if (Directory.Exists(dir))
            {
                foreach (var file in Directory.GetFiles(dir, "*.json"))
                {
                    string json_text = await File.ReadAllTextAsync(file);
                    capcutAnimations.Add(Parse(json_text));
                }
            }
            return capcutAnimations;
        }
    }
}
