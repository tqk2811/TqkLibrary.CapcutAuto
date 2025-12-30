using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Automation.Images;
using TqkLibrary.WinApi;
using TqkLibrary.WinApi.FindWindowHelper;
using TqkLibrary.WindowCapture;
using TqkLibrary.WindowCapture.Captures;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace TqkLibrary.CapcutAuto
{
    public class CapcutAutoClickHelper
    {
        public static DirectoryInfo CapcutDir { get; set; } = new DirectoryInfo(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "AppData\\Local\\CapCut"
            ));

        public static FileInfo CapcutExePath { get; set; } = new FileInfo(Path.Combine(
            CapcutDir.FullName,
            "Apps\\CapCut.exe"
            ));

        public static async Task CloseWindowAsync(CancellationToken cancellationToken = default)
        {
            using CancellationTokenSource timeout = new CancellationTokenSource(10000);
            while (true)
            {
                var processes = Process.GetProcessesByName("Capcut");
                if (!processes.Any())
                    break;
                foreach (var process in processes)
                {
                    if (timeout.IsCancellationRequested)
                    {
                        process.Kill();
                    }
                    else
                    {
                        process.CloseMainWindow();
                    }
                }
                await Task.Delay(500, cancellationToken);
            }
        }

        public static async Task OpenProjectWhiteCover(CancellationToken cancellationToken = default)
        {
            ProcessStartInfo processStartInfo = new(CapcutExePath.FullName, "--src1")
            {
                WorkingDirectory = CapcutExePath.Directory!.FullName,
                UseShellExecute = false,
            };
            using Process process = Process.Start(processStartInfo)!;
            ProcessHelper processHelper = new ProcessHelper(process.Id);
            ProcessHelper? child = null;
            while (true)
            {
                child = processHelper.ChildrensProcess.FirstOrDefault();
                if (child is not null)
                    break;
                await Task.Delay(10, cancellationToken);
            }
            var windows = child.WindowsTree.Where(x =>
                x.IsAltTabWindow
                && "CapCut".Equals(x.Title, StringComparison.OrdinalIgnoreCase)
                && "Qt622QWindowIcon".Equals(x.ClassName, StringComparison.OrdinalIgnoreCase)
                );
            while (!windows.Any())
            {
                await Task.Delay(500, cancellationToken);
            }
            await Task.Delay(1000, cancellationToken);
            var capture = new WinrtGraphicCapture();
            capture.MaxFps = 6;

            windows = windows.ToArray();
            var window = windows.First();
            var setupResult = await capture.InitAsync(window.WindowHandle);
            using Bitmap? bitmap = await capture.CaptureImageAsync();
            if (bitmap is null) throw new Exception();

            //bitmap.Save("C:\\test.png");
            using Image<Gray, byte> imageGray = bitmap.ToImage<Gray, byte>();
            Rectangle? rectangle = FindWhiteCover(imageGray);
            if (!rectangle.HasValue) throw new Exception();

            await window.WindowHandle.ControlLClickAsync(rectangle.Value.GetCenter());

            await CloseWindowAsync(cancellationToken);
        }


        static Rectangle? FindWhiteCover(Image<Gray, byte> imageGray)
        {
            using Image<Gray, byte> imgThreshold = imageGray.ThresholdBinary(new Gray(230), new Gray(255));

            double maxArea = 0;
            double minAreaThreshold = 500;
            Rectangle? largestSquare = null;
            using (Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(imgThreshold, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                for (int i = 0; i < contours.Size; i++)
                {
                    using (Emgu.CV.Util.VectorOfPoint contour = contours[i])
                    {
                        double area = CvInvoke.ContourArea(contour);
                        if (area > minAreaThreshold)
                        {
                            // Xấp xỉ đa giác để kiểm tra hình dạng
                            double peri = CvInvoke.ArcLength(contour, true);
                            using (Emgu.CV.Util.VectorOfPoint approx = new Emgu.CV.Util.VectorOfPoint())
                            {
                                CvInvoke.ApproxPolyDP(contour, approx, 0.04 * peri, true);

                                // Kiểm tra nếu là hình có 4 cạnh (gần giống hình chữ nhật/vuông)
                                if (approx.Size == 4)
                                {
                                    Rectangle rect = CvInvoke.BoundingRectangle(approx);

                                    // KIỂM TRA TỈ LỆ CẠNH: Để đảm bảo là hình VUÔNG (hoặc gần vuông)
                                    double ratio = (double)rect.Width / rect.Height;
                                    if (ratio >= 0.8 && ratio <= 1.2) // Chênh lệch tối đa 20%
                                    {
                                        if (area > maxArea)
                                        {
                                            maxArea = area;
                                            largestSquare = rect;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return largestSquare;
        }
    }


    public static class Extensions
    {
        public static async Task MouseClickAsync(this WindowHelper windowHelper, Point point, CancellationToken cancellationToken = default)
        {
            Rectangle? area = windowHelper.Area;
            if (!area.HasValue) throw new Exception();

            PInvoke.SetCursorPos(point.X, point.Y);
            PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            await Task.Delay(10, cancellationToken);
            PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
    }
}
