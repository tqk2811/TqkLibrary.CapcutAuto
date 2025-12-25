using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks.Segments
{
    public sealed class CapcutSegmentAudio : CapcutSegment
    {
        [JsonIgnore]
        public required CapcutMaterialAudio MaterialAudio { get; set; }
        protected override CapcutId GetMaterial()
        {
            return MaterialAudio;
        }




        [JsonIgnore]
        public CapcutMaterialSpeed MaterialSpeed { get; set; } = new();

        [JsonIgnore]
        public CapcutMaterialPlaceHolderInfo MaterialPlaceHolderInfo { get; set; } = new();

        [JsonIgnore]
        public CapcutMaterialBeat MaterialBeat { get; set; } = new();

        [JsonIgnore]
        public CapcutMaterialSoundChannelMapping MaterialSoundChannelMapping { get; set; } = new();

        [JsonIgnore]
        public CapcutMaterialVocalSeparation MaterialVocalSeparation { get; set; } = new();


        [JsonProperty("speed")]
        public override double Speed
        {
            get => MaterialSpeed.Speed;
            set => MaterialSpeed.Speed = value;
        }


        protected override IEnumerable<CapcutId> GetExtraMaterialRefs()
        {
            CapcutId[] capcutIds = new CapcutId[]
            {
                 MaterialSpeed,//speed
                 MaterialPlaceHolderInfo,//placeholder_infos
                 MaterialBeat,//beats
                 MaterialSoundChannelMapping,//sound_channel_mappings
                 MaterialVocalSeparation,//vocal_separations
            };
            foreach (CapcutId capcutId in capcutIds.Where(x => x is not null))
            {
                yield return capcutId;
            }
        }
    }
}
