using Newtonsoft.Json;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks.Segments
{
    public sealed class CapcutSegmentVideo : CapcutSegment
    {
        [JsonIgnore]
        public required CapcutMaterialVideo MaterialVideo { get; set; }
        protected override CapcutId GetMaterial()
        {
            return MaterialVideo;
        }


        public required override CapcutTimeRange SourceTimerange { get; set; }//video, audio ; sticker, text is null


        [JsonProperty("clip")]
        public SegmentClip Clip { get; init; } = new();


        [JsonIgnore]
        public CapcutMaterialPlaceHolderInfo MaterialPlaceHolderInfo { get; set; } = new();

        /// <summary>
        /// Transition to next video
        /// </summary>
        [JsonIgnore]
        public CapcutMaterialTransition? MaterialTransition { get; set; }

        [JsonIgnore]
        public CapcutMaterialCanvasColor MaterialCanvasColor { get; set; } = new();

        [JsonIgnore]
        public CapcutMaterialAnimationVideo MaterialAnimation { get; set; } = new();

        [JsonIgnore]
        public CapcutMaterialSoundChannelMapping MaterialSoundChannelMapping { get; set; } = new();

        [JsonIgnore]
        public CapcutMaterialColor MaterialColor { get; set; } = new();

        [JsonIgnore]
        public CapcutMaterialVocalSeparation MaterialVocalSeparation { get; set; } = new();


        protected override IEnumerable<CapcutId> GetExtraMaterialRefs()
        {
            CapcutId?[] capcutIds = new CapcutId?[]
            {
                 MaterialSpeed,//speed
                 MaterialPlaceHolderInfo,//placeholder_infos
                 MaterialTransition,//transitions
                 MaterialCanvasColor,//canvas_color
                 MaterialAnimation,//material_animations
                 MaterialSoundChannelMapping,//sound_channel_mappings
                 MaterialColor,//material_colors
                 MaterialVocalSeparation//vocal_separations
            };
            foreach (CapcutId capcutId in capcutIds.Where(x => x is not null)!)
            {
                yield return capcutId;
            }
        }
    }
}
