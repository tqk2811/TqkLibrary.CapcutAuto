using TqkLibrary.CapcutAuto.Enums;
using TqkLibrary.CapcutAuto.Models.Tracks.Segments;

namespace TqkLibrary.CapcutAuto.Models.Tracks
{
    public sealed class CapcutTrackAudio: CapcutTrack
    {
        public CapcutTrackAudio()
        {
            this.Type = TrackType.audio;
        }

        public void Add(CapcutSegmentAudio capcutSegmentAudio) => _segments.Add(capcutSegmentAudio);
    }
}
