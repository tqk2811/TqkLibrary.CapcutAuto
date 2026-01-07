using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Automation.Images;
using TqkLibrary.WinApi;
using TqkLibrary.WinApi.Helpers;
using TqkLibrary.WinApi.WmiHelpers;
using TqkLibrary.WindowCapture;
using TqkLibrary.WindowCapture.Captures;

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


        ProcessHelper? _rootProcess;
        public async Task OpenCapcutAsync(CancellationToken cancellationToken = default)
        {
#if DEBUG
            var processes = Process.GetProcessesByName("Capcut");
            foreach (var item in processes)
            {
                ProcessHelper pHelper = new ProcessHelper(item.Id);
                Win32_Process? win32_Process = pHelper.Query_Win32_Process();
                if (win32_Process?.CommandLine?.EndsWith("--src1") == true)
                {
                    _rootProcess = pHelper;
                    return;
                }
            }
#endif

            if (this._rootProcess is not null)
            {
                await CloseWindowAsync(cancellationToken);
                this._rootProcess = null;
            }

            ProcessStartInfo processStartInfo = new(CapcutExePath.FullName, "--src1")
            {
                WorkingDirectory = CapcutExePath.Directory!.FullName,//Apps\CapCut.exe
                UseShellExecute = false,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
            };
            using var rootProcess = Process.Start(processStartInfo)!;
            ProcessHelper processHelper = new ProcessHelper(rootProcess.Id);
            ProcessHelper? child = null;
            using CancellationTokenSource timeout = new CancellationTokenSource(10000);
            while (true)
            {
                child = processHelper.ChildrensProcess.FirstOrDefault();//Apps\7.7.0.3143\CapCut.exe
                if (child is not null)
                    break;
                await Task.Delay(10, cancellationToken);
                if (timeout.IsCancellationRequested)
                {
                    throw new TimeoutException("Waitting child process timeout");
                }
            }
            _rootProcess = child;
        }

        public async Task<bool> ClickProjectWhiteCoverAsync(CancellationToken cancellationToken = default)
        {
            if (_rootProcess is null) throw new InvalidOperationException($"Run {nameof(OpenCapcutAsync)} first");

            var windows = _rootProcess.WindowsTree.Where(x =>
                x.IsAltTabWindow
                && "CapCut".Equals(x.Title, StringComparison.OrdinalIgnoreCase)
                && "Qt622QWindowIcon".Equals(x.ClassName, StringComparison.OrdinalIgnoreCase)
                );
            WindowHelper? windowHelper = null;
            using CancellationTokenSource timeout = new CancellationTokenSource(10000);
            while (windowHelper is null)
            {
                if (timeout.IsCancellationRequested)
                {
                    throw new TimeoutException("Waitting window timeout");
                }
                await Task.Delay(100, cancellationToken);
                windowHelper = windows.FirstOrDefault();
            }
            await Task.Delay(1000, cancellationToken);
            using var capture = new WinrtGraphicCapture();
            capture.MaxFps = 6;

            var setupResult = await capture.InitWindowAsync(windowHelper.WindowHandle);
            await Task.Delay(500);
            using Bitmap? bitmap = await capture.CaptureImageAsync();
            if (bitmap is null) throw new Exception($"Can't capture image");

            using Image<Gray, byte> imageGray = bitmap.ToImage<Gray, byte>();
            Rectangle? rectangle = FindWhiteCover(imageGray);
            if (rectangle.HasValue)
            {
                await windowHelper.WindowHandle.ControlLClickAsync(rectangle.Value.GetCenter());
            }
            return rectangle.HasValue;
        }

        public async Task<bool> ClickExportAsync(CancellationToken cancellationToken = default)
        {
            if (_rootProcess is null) throw new InvalidOperationException($"Run {nameof(OpenCapcutAsync)} first");

            var windows = _rootProcess.WindowsTree.Where(x =>
                x.IsAltTabWindow
                && "CapCut".Equals(x.Title, StringComparison.OrdinalIgnoreCase)
                && "Qt622QWindowIcon".Equals(x.ClassName, StringComparison.OrdinalIgnoreCase)
                );
            WindowHelper? windowHelper = null;
            using CancellationTokenSource timeout = new CancellationTokenSource(10000);
            while (windowHelper is null)
            {
                if (timeout.IsCancellationRequested)
                {
                    throw new TimeoutException("Waitting window timeout");
                }
                await Task.Delay(100, cancellationToken);
                windowHelper = windows.FirstOrDefault();
            }
            await Task.Delay(1000, cancellationToken);
            using var capture = new WinrtGraphicCapture();
            capture.MaxFps = 6;
            if (WinrtGraphicCapture.IsCaptureCursorToggleSupported)
                capture.IsShowCursor = false;

            var setupResult = await capture.InitWindowAsync(windowHelper.WindowHandle);
            if (!setupResult) throw new Exception($"Init capture window failed");
            await Task.Delay(500, cancellationToken);
            using Bitmap? bitmap = await capture.CaptureImageAsync();
            if (bitmap is null) throw new Exception($"Can't capture image");

            using Image<Hsv, byte> imageHsv = bitmap.ToImage<Hsv, byte>();

            Rectangle? rect = FindBlueButton(imageHsv);
            if (rect.HasValue)
            {
                await windowHelper.WindowHandle.ControlLClickAsync(rect.Value.GetCenter());
            }
            return rect.HasValue;
        }


        public async Task<bool> ExportAsync(CancellationToken cancellationToken = default)
        {
            if (_rootProcess is null) throw new InvalidOperationException($"Run {nameof(OpenCapcutAsync)} first");

            WindowHelper? capcutWindowHelper = null;
            using (CancellationTokenSource timeout = new CancellationTokenSource(10000))
            {
                var capcutWindows = _rootProcess.WindowsTree.Where(x =>
                    x.IsAltTabWindow
                    && "CapCut".Equals(x.Title, StringComparison.OrdinalIgnoreCase)
                    && "Qt622QWindowIcon".Equals(x.ClassName, StringComparison.OrdinalIgnoreCase)
                    );
                while (capcutWindowHelper is null)
                {
                    if (timeout.IsCancellationRequested)
                    {
                        throw new TimeoutException("Waitting window timeout");
                    }
                    await Task.Delay(100, cancellationToken);
                    capcutWindowHelper = capcutWindows.FirstOrDefault();
                }
            }

            WindowHelper? exportWindowHelper = null;
            using (CancellationTokenSource timeout = new CancellationTokenSource(10000))
            {
                var exportWindows = _rootProcess.AllWindows.Where(x =>
                    "Export".Equals(x.Title, StringComparison.OrdinalIgnoreCase)
                    && "Qt622QWindowIcon".Equals(x.ClassName, StringComparison.OrdinalIgnoreCase)
                    );
                while (exportWindowHelper is null)
                {
                    if (timeout.IsCancellationRequested)
                    {
                        throw new TimeoutException("Waitting window timeout");
                    }
                    await Task.Delay(100, cancellationToken);
                    exportWindowHelper = exportWindows.FirstOrDefault();
                }
            }


            using var capture = new WinrtGraphicCapture();//new HdcCapture();
            capture.MaxFps = 6;
            if (WinrtGraphicCapture.IsCaptureCursorToggleSupported)
                capture.IsShowCursor = false;

            //bool isMonitorCapture = false;
            bool setupResult = false;
            //setupResult = await capture.InitWindowAsync(exportWindowHelper.WindowHandle);
            //if (!setupResult)
            //{
            //    setupResult = await capture.InitWindowAsync(capcutWindowHelper.WindowHandle);
            //}
            //if (!setupResult)
            {
                IntPtr? hmonitor = MonitorHelper.Monitors.FirstOrDefault();
                if (!hmonitor.HasValue) throw new Exception($"Can't get monitor handle");

                setupResult = await capture.InitMonitorAsync(hmonitor.Value);
                if (!setupResult) throw new Exception($"Init capture window failed");
                //isMonitorCapture = true;
            }


            await Task.Delay(500, cancellationToken);
            using Bitmap? bitmap = await capture.CaptureImageAsync();
            if (bitmap is null) throw new Exception($"Can't capture image");

            //bitmap.Save("C:\\export.png");

            using Image<Hsv, byte> screenHsv = bitmap.ToImage<Hsv, byte>();
            Rectangle? windowArea = exportWindowHelper.GetArea();
            if (!windowArea.HasValue) throw new Exception($"failed to get window area");
            using Image<Hsv, byte> screenHsv_crop = screenHsv.Copy(windowArea.Value);

            Rectangle? rectButton = FindBlueButton(screenHsv_crop);
            if (rectButton.HasValue)
            {
                Point center = rectButton.Value.GetCenter();
                Point point = new Point(windowArea.Value.X + center.X, windowArea.Value.Y + center.Y);
                await exportWindowHelper.MouseClickAsync(point);//click export

                await Task.Delay(5000, cancellationToken);
                //render & chờ nút share
                using (CancellationTokenSource timeout = new CancellationTokenSource(2 * 60000))
                {
                    while (true)
                    {
                        await Task.Delay(1000, cancellationToken);
                        using Bitmap? bitmap2 = await capture.CaptureImageAsync();
                        if (bitmap2 is null) throw new Exception($"Can't capture image");
                        using Image<Hsv, byte> screenHsv2 = bitmap2.ToImage<Hsv, byte>();
                        using Image<Hsv, byte> screenHsv2_crop = screenHsv2.Copy(windowArea.Value);
                        Rectangle? rectButton2 = FindBlueButton(screenHsv2_crop);//share button
                        if (rectButton2.HasValue)
                        {
                            return true;
                        }
                        if (timeout.IsCancellationRequested)
                        {
                            throw new TimeoutException($"Render timeout");
                        }
                    }
                }
            }
            return false;
        }






        static Rectangle? FindWhiteCover(Image<Gray, byte> imageGray, double areaSize = 500)
        {
            using Image<Gray, byte> imgThreshold = imageGray.ThresholdBinary(new Gray(230), new Gray(255));

            double maxArea = 0;
            Rectangle? largestSquare = null;
            using (Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(imgThreshold, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                for (int i = 0; i < contours.Size; i++)
                {
                    using (Emgu.CV.Util.VectorOfPoint contour = contours[i])
                    {
                        double area = CvInvoke.ContourArea(contour);
                        if (area > areaSize)
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

        static Rectangle? FindBlueButton(Image<Hsv, byte> imageHsv, double areaSize = 500)
        {
            using Image<Gray, byte> mask = imageHsv.InRange(new Hsv(79, 111, 109), new Hsv(96, 255, 255));
            CvInvoke.MedianBlur(mask, mask, 5);

            using VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            using Mat hierarchy = new Mat();
            CvInvoke.FindContours(mask, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            for (int i = 0; i < contours.Size; i++)
            {
                double area = CvInvoke.ContourArea(contours[i]);
                if (area > areaSize)
                {
                    return CvInvoke.BoundingRectangle(contours[i]);
                }
            }

            return null;
        }
    }
}
