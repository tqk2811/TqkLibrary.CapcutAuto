using TqkLibrary.CapcutAuto.Enums;
using TqkLibrary.CapcutAuto.Models.Tracks.Segments;

namespace TqkLibrary.CapcutAuto.Models.Tracks
{
    public sealed class CapcutTrackVideo : CapcutTrack
    {
        public CapcutTrackVideo()
        {
            this.Type = TrackType.video;
        }

        public void Add(CapcutSegmentVideo capcutSegmentVideo) => _segments.Add(capcutSegmentVideo);
    }
}
