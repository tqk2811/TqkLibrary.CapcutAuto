using System.Drawing;
using TqkLibrary.WinApi.Helpers;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace TqkLibrary.CapcutAuto
{
    public static class Extensions
    {
        public static async Task MouseClickAsync(this WindowHelper windowHelper, Point point, CancellationToken cancellationToken = default)
        {
            Rectangle? area = windowHelper.GetArea();
            if (!area.HasValue) throw new Exception();

            PInvoke.SetCursorPos(point.X, point.Y);
            PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            await Task.Delay(10, cancellationToken);
            PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
    }
}
