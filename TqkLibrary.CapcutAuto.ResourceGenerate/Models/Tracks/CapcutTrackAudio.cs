using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks.Segments;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks
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
