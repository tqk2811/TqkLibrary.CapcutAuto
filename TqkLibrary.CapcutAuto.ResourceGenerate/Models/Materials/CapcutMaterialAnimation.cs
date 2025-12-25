using Newtonsoft.Json;
using System.Collections;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Resources;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
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

        [JsonIgnore]
        public CapcutAnimation? In
        {
            get { return _animations.FirstOrDefault(x => x.Type == AnimationType.@in); }
            set
            {
                if (value is not null && value.Type != AnimationType.@in)
                    throw new InvalidOperationException($"Animation type must be 'in'");
                var @in = In;
                if (@in is not null)
                    _animations.Remove(@in);
                if (value is not null)
                    _animations.Add(value);
            }
        }

        [JsonIgnore]
        public CapcutAnimation? Out
        {
            get { return _animations.FirstOrDefault(x => x.Type == AnimationType.@out); }
            set
            {
                if (value is not null && value.Type != AnimationType.@out)
                    throw new InvalidOperationException($"Animation type must be 'out'");
                var @out = Out;
                if (@out is not null)
                    _animations.Remove(@out);
                if (value is not null)
                    _animations.Add(value);
            }
        }
    }
}
