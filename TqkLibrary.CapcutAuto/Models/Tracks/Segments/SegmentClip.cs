using Newtonsoft.Json;

namespace TqkLibrary.CapcutAuto.Models.Tracks.Segments
{
    public sealed class SegmentClip
    {
        [JsonProperty("alpha")]
        public double Alpha { get; set; } = 1.0;

        [JsonProperty("flip")]
        public _Flip Flip { get; init; } = new();

        [JsonProperty("rotation")]
        public double Rotation { get; set; } = 0.0;

        [JsonProperty("scale")]
        public _Scale Scale { get; init; } = new();

        [JsonProperty("transform")]
        public _Transform Transform { get; init; } = new();


        public class _Flip
        {
            [JsonProperty("horizontal")]
            public bool Horizontal { get; set; } = false;

            [JsonProperty("vertical")]
            public bool Vertical { get; set; } = false;
        }

        public class _Scale
        {
            [JsonProperty("x")]
            public double X { get; set; } = 1.0;

            [JsonProperty("y")]
            public double Y { get; set; } = 1.0;
        }
        /// <summary>
        /// gốc toạ độ là giữa khung hình<br/>
        /// trục x tăng qua phải, giới hạn -1 tới 1<br/>
        /// trục y tăng lên trên, giới hạn -1 tới 1<br/>
        /// gốc nằm giữa vật thể<br/>
        /// dùng cho <see cref="CapcutSegmentText"/> và <see cref="CapcutSegmentVideo"/><br/>
        /// </summary>
        public class _Transform
        {
            [JsonProperty("x")]
            public double X { get; set; } = 0.0;

            [JsonProperty("y")]
            public double Y { get; set; } = 0.0;
        }
    }
}
