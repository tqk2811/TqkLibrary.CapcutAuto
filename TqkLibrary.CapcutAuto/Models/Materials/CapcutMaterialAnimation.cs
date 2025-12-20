using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.Enums;

namespace TqkLibrary.CapcutAuto.Models.Materials
{
    public class CapcutMaterialAnimation : CapcutMaterial
    {
        readonly List<CapcutAnimation> _animations = new();

        [JsonProperty("animations")]
        public IReadOnlyList<CapcutAnimation> Animations => _animations;

        [JsonProperty("multi_language_current")]
        public string MultiLanguageCurrent { get; set; } = "none";


        public CapcutMaterialAnimation()
        {
            Type = MaterialType.sticker_animation;
        }

        public void Add(CapcutAnimation capcutAnimation)
        {
            if (capcutAnimation is null) throw new ArgumentNullException(nameof(capcutAnimation));
            if (_animations.Any(x => x.Type == capcutAnimation.Type))
                throw new InvalidOperationException($"Only two animation in and out");
            _animations.Add(capcutAnimation);
        }
    }
}
