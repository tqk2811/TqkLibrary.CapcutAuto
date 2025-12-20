using TqkLibrary.CapcutAuto.Enums;
using TqkLibrary.CapcutAuto.Models.Tracks.Segments;

namespace TqkLibrary.CapcutAuto.Models.Tracks
{
    public sealed class CapcutTrackText : CapcutTrack
    {
        public CapcutTrackText()
        {
            this.Type = TrackType.text;
        }
        public void Add(CapcutSegmentText capcutSegmentText) => _segments.Add(capcutSegmentText);
    }
}
