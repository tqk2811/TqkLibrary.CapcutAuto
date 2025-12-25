using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks.Segments;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks
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
