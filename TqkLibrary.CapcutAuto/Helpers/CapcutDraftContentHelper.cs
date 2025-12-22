using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Reflection;
using TqkLibrary.CapcutAuto.JsonConverters;
using TqkLibrary.CapcutAuto.Models;
using TqkLibrary.CapcutAuto.Models.Tracks;
using TqkLibrary.CapcutAuto.Models.Tracks.Segments;

namespace TqkLibrary.CapcutAuto.Helpers
{
    public class CapcutDraftContentHelper
    {
        readonly JObject _jobject;
        readonly JsonSerializer _jsonSerializer;
        public CapcutDraftContentHelper(CapcutTrackCollection capcutTracks)
        {
            this.CapcutTracks = capcutTracks ?? throw new ArgumentNullException(nameof(capcutTracks));
            _jsonSerializer = JsonSerializer.Create(Singleton.JsonSerializerSettings);

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"{assembly.GetName().Name}.BaseDraftContent.json";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
            {
                using StreamReader streamReader = new StreamReader(stream);
                string json = streamReader.ReadToEnd();
                _jobject = JObject.Parse(json);
            }
            this.DraftContent = new CapcutDraftContent(_jobject, _jsonSerializer);
        }


        public CapcutDraftContent DraftContent { get; }
        public CapcutTrackCollection CapcutTracks { get; }


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
            var materials_animations = textSegments.Select(x => x.MaterialAnimation).Where(x => x is not null).ToArray();
            _jobject["materials"]!["texts"] = JArray.FromObject(materials_texts, _jsonSerializer);
            _jobject["materials"]!["material_animations"] = JArray.FromObject(materials_animations);//text animation


            var videoSegments = CapcutTracks
                .OfType<CapcutTrackVideo>()
                .SelectMany(x => x.Segments)
                .OfType<CapcutSegmentVideo>();
            var materials_videos = videoSegments.Select(x => x.MaterialVideo).Where(x => x is not null).ToArray();
            var materials_transitions = videoSegments.Select(x => x.MaterialTransition).Where(x => x is not null).ToArray();
            _jobject["materials"]!["videos"] = JArray.FromObject(materials_videos, _jsonSerializer);
            _jobject["materials"]!["transitions"] = JArray.FromObject(materials_transitions, _jsonSerializer);//video transitions


            var speeds = videoSegments
                .Select(x => x.MaterialSpeed)
                ;
            _jobject["materials"]!["speeds"] = JArray.FromObject(speeds.Where(x => x is not null).ToArray(), _jsonSerializer);



            _jobject["tracks"] = JArray.FromObject(CapcutTracks, _jsonSerializer);
            return JsonConvert.SerializeObject(_jobject, Formatting.Indented);
        }
    }
}
