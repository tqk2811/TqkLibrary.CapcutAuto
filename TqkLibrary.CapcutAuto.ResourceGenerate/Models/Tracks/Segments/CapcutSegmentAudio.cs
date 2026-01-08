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

        public required override CapcutTimeRange SourceTimerange { get; set; }//video, audio ; sticker, text is null

        [JsonIgnore]
        public CapcutMaterialPlaceHolderInfo MaterialPlaceHolderInfo { get; set; } = new();

        [JsonIgnore]
        public CapcutMaterialBeat MaterialBeat { get; set; } = new();

        [JsonIgnore]
        public CapcutMaterialSoundChannelMapping MaterialSoundChannelMapping { get; set; } = new();

        [JsonIgnore]
        public CapcutMaterialVocalSeparation MaterialVocalSeparation { get; set; } = new();


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
