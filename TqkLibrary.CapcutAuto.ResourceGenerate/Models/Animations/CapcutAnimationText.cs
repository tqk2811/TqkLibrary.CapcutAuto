using Newtonsoft.Json;
using System.Threading.Tasks;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Animations
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
        public static async Task<IReadOnlyList<CapcutAnimationText>> FromDirAsync(string dir)
        {
            List<CapcutAnimationText> capcutAnimations = new List<CapcutAnimationText>();
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
