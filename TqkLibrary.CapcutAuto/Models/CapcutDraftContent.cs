using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TqkLibrary.CapcutAuto.Models
{
    public class CapcutDraftContent
    {
        readonly JObject _jobject;
        readonly JsonSerializer _jsonSerializer;
        public CapcutDraftContent(JObject jobject, JsonSerializer jsonSerializer)
        {
            this._jobject = jobject ?? throw new ArgumentNullException(nameof(jobject));
            this._jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public TimeSpan Duration
        {
            get { return _jobject["duration"]!.ToObject<TimeSpan>(_jsonSerializer)!; }
            internal set { _jobject["duration"] = JToken.FromObject(value, _jsonSerializer); }
        }

        public double Fps
        {
            get { return _jobject["fps"]!.ToObject<double>(_jsonSerializer)!; }
            set { _jobject["fps"] = JToken.FromObject(value, _jsonSerializer); }
        }

        public Guid Id
        {
            get { return _jobject["id"]!.ToObject<Guid>(_jsonSerializer)!; }
            internal set { _jobject["id"] = JToken.FromObject(value, _jsonSerializer); }
        }
    }
}
