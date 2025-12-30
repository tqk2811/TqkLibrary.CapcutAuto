using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Reflection;
using TqkLibrary.CapcutAuto.ResourceGenerate.JsonConverters;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Materials;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks.Segments;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Helpers
{
    public class CapcutDraftContentHelper
    {
        readonly JObject _jobject;
        readonly JsonSerializer _jsonSerializer;
        internal CapcutDraftContentHelper()
        {
            _jsonSerializer = JsonSerializer.Create(Singleton.JsonSerializerSettings);

            _jobject = JObject.Parse(Extensions.GetEmbeddedResourceString("draft_content.json"));
            this.DraftContent = new CapcutDraftContent(_jobject);
        }


        public CapcutDraftContent DraftContent { get; }
        public CapcutTrackCollection CapcutTracks { get; } = new();


        public string BuildJson()
        {
            //pre calc
            DraftContent.Id = Guid.NewGuid();




            foreach (JProperty material in _jobject["materials"]!)
            {
                if (material?.Value.Type == JTokenType.Array)
                {
                    _jobject["materials"]![material.Name] = JArray.FromObject(new object[0]);
                }
            }

            var textSegments = CapcutTracks
                .OfType<CapcutTrackText>()
                .SelectMany(x => x.Segments)
                .OfType<CapcutSegmentText>();
            var materials_texts = textSegments.Select(x => x.MaterialText).Where(x => x is not null).ToArray();
            var materials_effects = textSegments.SelectMany(x => x.CapcutMaterialEffects).Where(x => x is not null).ToArray();
            _jobject["materials"]!["texts"] = JArray.FromObject(materials_texts, _jsonSerializer);
            _jobject["materials"]!["effects"] = JArray.FromObject(materials_effects);

            var videoSegments = CapcutTracks
                .OfType<CapcutTrackVideo>()
                .SelectMany(x => x.Segments)
                .OfType<CapcutSegmentVideo>();
            var materials_videos = videoSegments.Select(x => x.MaterialVideo).Where(x => x is not null).ToArray();
            var materials_v_transitions = videoSegments.Select(x => x.MaterialTransition).Where(x => x is not null).ToArray();
            var materials_v_canvases = videoSegments.Select(x => x.MaterialCanvasColor).Where(x => x is not null).ToArray();
            var materials_v_colors = videoSegments.Select(x => x.MaterialColor).Where(x => x is not null).ToArray();
            _jobject["materials"]!["videos"] = JArray.FromObject(materials_videos, _jsonSerializer);
            _jobject["materials"]!["transitions"] = JArray.FromObject(materials_v_transitions, _jsonSerializer);
            _jobject["materials"]!["canvases"] = JArray.FromObject(materials_v_canvases, _jsonSerializer);
            _jobject["materials"]!["material_colors"] = JArray.FromObject(materials_v_colors, _jsonSerializer);

            var audioSegments = CapcutTracks
                .OfType<CapcutTrackAudio>()
                .SelectMany(x => x.Segments)
                .OfType<CapcutSegmentAudio>();
            var materials_audios = audioSegments.Select(x => x.MaterialAudio).Where(x => x is not null).ToArray();
            var materials_a_beats = audioSegments.Select(x => x.MaterialBeat).Where(x => x is not null).ToArray();
            _jobject["materials"]!["audios"] = JArray.FromObject(materials_audios, _jsonSerializer);
            _jobject["materials"]!["beats"] = JArray.FromObject(materials_a_beats, _jsonSerializer);


            var materials_v_speeds = videoSegments.Select(x => x.MaterialSpeed).Where(x => x is not null).ToArray();
            var materials_a_speeds = audioSegments.Select(x => x.MaterialSpeed).Where(x => x is not null).ToArray();
            var speeds = materials_v_speeds.Concat(materials_a_speeds);
            _jobject["materials"]!["speeds"] = JArray.FromObject(speeds.ToArray(), _jsonSerializer);


            CapcutMaterialAnimation[] materials_t_animations = textSegments.Select(x => x.MaterialAnimation).Where(x => x is not null).ToArray();
            var materials_v_animations = videoSegments.Select(x => x.MaterialAnimation).Where(x => x is not null).ToArray();
            var material_animations = materials_t_animations.Concat(materials_v_animations).ToArray();
            _jobject["materials"]!["material_animations"] = JArray.FromObject(material_animations, _jsonSerializer);//text animation

            var materials_v_placeholderinfos = videoSegments.Select(x => x.MaterialPlaceHolderInfo).Where(x => x is not null).ToArray();
            var materials_a_placeholderinfos = audioSegments.Select(x => x.MaterialPlaceHolderInfo).Where(x => x is not null).ToArray();
            var placeholderinfos = materials_v_placeholderinfos.Concat(materials_a_placeholderinfos);
            _jobject["materials"]!["placeholder_infos"] = JArray.FromObject(placeholderinfos.ToArray(), _jsonSerializer);


            var materials_v_soundchannelmappings = videoSegments.Select(x => x.MaterialSoundChannelMapping).Where(x => x is not null).ToArray();
            var materials_a_soundchannelmappings = audioSegments.Select(x => x.MaterialSoundChannelMapping).Where(x => x is not null).ToArray();
            var soundchannelmappings = materials_v_soundchannelmappings.Concat(materials_a_soundchannelmappings);
            _jobject["materials"]!["sound_channel_mappings"] = JArray.FromObject(soundchannelmappings.ToArray(), _jsonSerializer);


            var materials_v_vocals = videoSegments.Select(x => x.MaterialVocalSeparation).Where(x => x is not null).ToArray();
            var materials_a_vocals = audioSegments.Select(x => x.MaterialVocalSeparation).Where(x => x is not null).ToArray();
            var vocals = materials_v_vocals.Concat(materials_a_vocals);
            _jobject["materials"]!["vocal_separations"] = JArray.FromObject(vocals.ToArray(), _jsonSerializer);

            JArray tracks = JArray.FromObject(CapcutTracks, _jsonSerializer);
#if DEBUG
            string json = JsonConvert.SerializeObject(tracks, Formatting.Indented);
#endif
            _jobject["tracks"] = tracks;
            return JsonConvert.SerializeObject(_jobject, Formatting.Indented);
        }
    }
}
