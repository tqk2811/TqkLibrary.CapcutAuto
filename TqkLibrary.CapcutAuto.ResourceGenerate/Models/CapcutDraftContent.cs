using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models
{
    public class CapcutDraftContent
    {
        readonly JObject _jobject;
        readonly JsonSerializer _jsonSerializer;
        internal CapcutDraftContent(JObject jobject)
        {
            this._jobject = jobject ?? throw new ArgumentNullException(nameof(jobject));
            this._jsonSerializer = JsonSerializer.Create(Singleton.JsonSerializerSettings);
        }

        public Size CanvasSize
        {
            get
            {
                var config = _jobject["canvas_config"];
                if (config == null) throw new InvalidOperationException("canvas_config not found");

                int width = config["width"]!.Value<int>();
                int height = config["height"]!.Value<int>();

                return new Size(width, height);
            }
            set
            {
                if (_jobject["canvas_config"] is JObject config)
                {
                    config["width"] = value.Width;
                    config["height"] = value.Height;
                }
            }
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
