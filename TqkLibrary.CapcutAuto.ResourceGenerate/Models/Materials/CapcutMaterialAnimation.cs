using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Animations;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials
{
    public abstract class CapcutMaterialAnimation : CapcutMaterial
    {

    }
    public abstract class CapcutMaterialAnimation<T> : CapcutMaterialAnimation
        where T : CapcutAnimation
    {
        readonly List<T> _animations = new();

        [JsonProperty("animations")]
        public IReadOnlyList<T> Animations => _animations;

        [JsonProperty("multi_language_current")]
        public string MultiLanguageCurrent { get; set; } = "none";

        public CapcutMaterialAnimation()
        {
            Type = MaterialType.sticker_animation;
        }



        [JsonIgnore]
        public T? In
        {
            get { return _animations.FirstOrDefault(x => x.Type == AnimationType.@in); }
            set
            {
                if (value is not null && value.Type != AnimationType.@in)
                    throw new InvalidOperationException($"Animation type must be '{nameof(AnimationType.@in)}'");
                var @in = In;
                if (@in is not null)
                    _animations.Remove(@in);
                if (value is not null)
                    _animations.Add(value);
            }
        }

        [JsonIgnore]
        public T? Out
        {
            get { return _animations.FirstOrDefault(x => x.Type == AnimationType.@out); }
            set
            {
                if (value is not null && value.Type != AnimationType.@out)
                    throw new InvalidOperationException($"Animation type must be '{nameof(AnimationType.@out)}'");
                var @out = Out;
                if (@out is not null)
                    _animations.Remove(@out);
                if (value is not null)
                    _animations.Add(value);
            }
        }


        [JsonIgnore]
        public T? Loop
        {
            get { return _animations.FirstOrDefault(x => x.Type == AnimationType.loop); }
            set
            {
                if (value is not null && value.Type != AnimationType.loop)
                    throw new InvalidOperationException($"Animation type must be '{nameof(AnimationType.loop)}'");
                var @out = Out;
                if (@out is not null)
                    _animations.Remove(@out);
                if (value is not null)
                    _animations.Add(value);
            }
        }
    }
}
