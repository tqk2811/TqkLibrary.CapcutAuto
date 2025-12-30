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
        public static async Task<IReadOnlyList<CapcutAnimationSticker>> FromDirAsync(string dir)
        {
            List<CapcutAnimationSticker> capcutAnimations = new List<CapcutAnimationSticker>();
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
