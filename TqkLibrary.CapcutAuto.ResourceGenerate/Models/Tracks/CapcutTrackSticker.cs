using TqkLibrary.CapcutAuto.ResourceGenerate.Enums;
using TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks.Segments;

namespace TqkLibrary.CapcutAuto.ResourceGenerate.Models.Tracks
{
    public sealed class CapcutTrackSticker : CapcutTrack
    {
        public CapcutTrackSticker()
        {
            this.Type = TrackType.sticker;
        }
        public void Add(CapcutSegmentSticker capcutSegmentSticker) => _segments.Add(capcutSegmentSticker);
    }
}
