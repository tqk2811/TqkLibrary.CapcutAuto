using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Diagnostics;
using System.Drawing;
using Tesseract;
using TqkLibrary.Automation.Images;
using TqkLibrary.CapcutAuto.Exceptions;
using TqkLibrary.WinApi;
using TqkLibrary.WinApi.Helpers;
using TqkLibrary.WinApi.WmiHelpers;
using TqkLibrary.WindowCapture.Captures;
using Windows.Win32;

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

        public static TimeSpan WaitCloseProcessTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public static TimeSpan DelayBeforeWindowShow { get; set; } = TimeSpan.FromSeconds(3);
        public static TimeSpan WaitWindowTimeout { get; set; } = TimeSpan.FromSeconds(20);
        public static TimeSpan CheckImageTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public static TimeSpan CheckImageAndWaitProjectTimeout { get; set; } = TimeSpan.FromMinutes(1);
        public static TimeSpan SetupCaptureTimeout { get; set; } = TimeSpan.FromSeconds(10);
        public static TimeSpan WaitRenderTimeout { get; set; } = TimeSpan.FromMinutes(5);

        public static async Task CloseWindowAsync(CancellationToken cancellationToken = default)
        {
            using CancellationTokenSource timeout = new CancellationTokenSource(WaitCloseProcessTimeout);
            while (true)
            {
                var processes = Process.GetProcessesByName("Capcut").Concat(Process.GetProcessesByName("VEDetector")).ToArray();
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


        protected ProcessHelper? _rootProcess;
        public virtual async Task OpenCapcutAsync(CancellationToken cancellationToken = default)
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
            using CancellationTokenSource timeout = new CancellationTokenSource(WaitWindowTimeout);
            while (true)
            {
                foreach (var item in processHelper.ChildrensProcess)//Apps\7.7.0.3143\CapCut.exe or VEDetector.exe
                {
                    if (item.Name.StartsWith("VEDetector"))
                    {
                        foreach (var window in item.WindowsTree)
                        {
                            window.SendMessage(PInvoke.WM_CLOSE, UIntPtr.Zero, IntPtr.Zero);
                        }
                    }
                    else if (item.Name.StartsWith("CapCut"))
                    {
                        child = item;
                        break;
                    }
                }
                if (child is not null)
                    break;
                await Task.Delay(10, cancellationToken);
                if (timeout.IsCancellationRequested)
                {
                    throw new CapcutAutoTimeoutException("Waitting child process timeout");
                }
            }
            _rootProcess = child;
        }

        public virtual async Task ClickProjectWhiteCoverAsync(CancellationToken cancellationToken = default)
        {
            if (_rootProcess is null) throw new InvalidOperationException($"Run {nameof(OpenCapcutAsync)} first");

            var windows = _rootProcess.WindowsTree.Where(x =>
                x.IsAltTabWindow
                && "CapCut".Equals(x.Title, StringComparison.OrdinalIgnoreCase)
                && "Qt622QWindowIcon".Equals(x.ClassName, StringComparison.OrdinalIgnoreCase)
                );
            WindowHelper? windowHelper = null;
            using (CancellationTokenSource timeout = new CancellationTokenSource(WaitWindowTimeout))
            {
                while (windowHelper is null)
                {
                    if (timeout.IsCancellationRequested)
                    {
                        throw new CapcutAutoTimeoutException("Waitting window timeout");
                    }
                    await Task.Delay(100, cancellationToken);
                    windowHelper = windows.FirstOrDefault();
                }
            }
            await Task.Delay(DelayBeforeWindowShow, cancellationToken);

            using CancellationTokenSource timeout_CloseAnotherPopupWindow = new();
            try
            {
                using var register = cancellationToken.Register(() => timeout_CloseAnotherPopupWindow.Cancel());
                _ = CloseAnotherPopupWindowAsync(
                    new List<string>()
                    {
                    "CapCut"
                    },
                    timeout_CloseAnotherPopupWindow.Token
                    );

                //init capture
                using var capture = new WinrtGraphicCapture();
                capture.MaxFps = 6;
                if (WinrtGraphicCapture.IsCaptureCursorToggleSupported)
                    capture.IsShowCursor = false;
                using (CancellationTokenSource timeout = new CancellationTokenSource(SetupCaptureTimeout))
                {
                    while (!await capture.InitWindowAsync(windowHelper.WindowHandle))
                    {
                        if (timeout.IsCancellationRequested)
                            throw new CapcutAutoTimeoutException($"Init capture window failed");
                        await Task.Delay(500);
                    }
                }

                //capture & click
                using (CancellationTokenSource timeout = new CancellationTokenSource(CheckImageTimeout))
                {
                    while (true)
                    {
                        await Task.Delay(500, cancellationToken);
                        using Bitmap? bitmap = await capture.CaptureImageAsync();
                        Rectangle? rectangle = null;
                        if (bitmap is not null)
                        {
                            using Image<Gray, byte> imageGray = bitmap.ToImage<Gray, byte>();
                            rectangle = FindWhiteCover(imageGray);
                            if (rectangle.HasValue)
                            {
                                await windowHelper.WindowHandle.ControlLClickAsync(rectangle.Value.GetCenter());
                                return;
                            }
                        }
                        if (timeout.IsCancellationRequested)
                        {
                            if (bitmap is null)
                                throw new CapcutAutoTimeoutException($"Can't capture image");
                            if (!rectangle.HasValue)
                                throw new CapcutAutoTimeoutException($"WhiteCover not found");
                            throw new CapcutAutoTimeoutException($"Check image timeout");
                        }
                    }
                }
            }
            finally
            {
                timeout_CloseAnotherPopupWindow.Cancel();
            }
        }

        public virtual async Task ClickExportAsync(CancellationToken cancellationToken = default)
        {
            if (_rootProcess is null) throw new InvalidOperationException($"Run {nameof(OpenCapcutAsync)} first");

            var windows = _rootProcess.WindowsTree.Where(x =>
                x.IsAltTabWindow
                && "CapCut".Equals(x.Title, StringComparison.OrdinalIgnoreCase)
                && "Qt622QWindowIcon".Equals(x.ClassName, StringComparison.OrdinalIgnoreCase)
                );
            WindowHelper? windowHelper = null;
            using (CancellationTokenSource timeout = new CancellationTokenSource(WaitWindowTimeout))
            {
                while (windowHelper is null)
                {
                    if (timeout.IsCancellationRequested)
                        throw new CapcutAutoTimeoutException("Waitting window timeout");
                    await Task.Delay(100, cancellationToken);
                    windowHelper = windows.FirstOrDefault();
                }
            }

            await Task.Delay(DelayBeforeWindowShow, cancellationToken);



            using var capture = new WinrtGraphicCapture();
            capture.MaxFps = 6;
            if (WinrtGraphicCapture.IsCaptureCursorToggleSupported)
                capture.IsShowCursor = false;
            using (CancellationTokenSource timeout = new CancellationTokenSource(SetupCaptureTimeout))
            {
                while (!await capture.InitWindowAsync(windowHelper.WindowHandle))
                {
                    if (timeout.IsCancellationRequested)
                        throw new CapcutAutoTimeoutException($"Init capture window failed");
                    await Task.Delay(500);
                }
            }

            //capture & click
            using (CancellationTokenSource timeout = new CancellationTokenSource(CheckImageAndWaitProjectTimeout))
            {
                var exportWindows = _rootProcess.AllWindows.Where(x =>
                     x.Title.StartsWith("Export", StringComparison.OrdinalIgnoreCase)
                     && "Qt622QWindowIcon".Equals(x.ClassName, StringComparison.OrdinalIgnoreCase)
                     );
                while (!exportWindows.Any())
                {
                    await Task.Delay(500, cancellationToken);
                    using Bitmap? bitmap = await capture.CaptureImageAsync();
                    Rectangle? rectangle = null;
                    if (bitmap is not null)
                    {
                        using Image<Hsv, byte> imageHsv = bitmap.ToImage<Hsv, byte>();
                        Rectangle crop = new Rectangle(imageHsv.Width - 450, 0, 450, 80);//450 x 80 top left

                        (rectangle, string? name) = FindBlueButton(imageHsv, crop, "Export", 900);
                        if (rectangle.HasValue && "Export".Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            await windowHelper.WindowHandle.ControlLClickAsync(rectangle.Value.GetCenter());
                        }
                    }

                    if (timeout.IsCancellationRequested)
                    {
                        if (bitmap is null)
                            throw new CapcutAutoTimeoutException($"Can't capture image");
                        if (!rectangle.HasValue)
                            throw new CapcutAutoTimeoutException($"FindBlueButton not found");
                        throw new CapcutAutoTimeoutException($"Check image timeout");
                    }
                }
            }
        }


        public virtual async Task ExportRenderAsync(CancellationToken cancellationToken = default)
        {
            if (_rootProcess is null) throw new InvalidOperationException($"Run {nameof(OpenCapcutAsync)} first");

            WindowHelper? capcutWindowHelper = null;
            using (CancellationTokenSource timeout = new CancellationTokenSource(WaitWindowTimeout))
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
                        throw new CapcutAutoTimeoutException("Waitting window timeout");
                    }
                    await Task.Delay(100, cancellationToken);
                    capcutWindowHelper = capcutWindows.FirstOrDefault();
                }
            }

            WindowHelper? exportWindowHelper = null;
            using (CancellationTokenSource timeout = new CancellationTokenSource(WaitWindowTimeout))
            {
                var exportWindows = _rootProcess.AllWindows.Where(x =>
                    x.Title.StartsWith("Export", StringComparison.OrdinalIgnoreCase)
                    && "Qt622QWindowIcon".Equals(x.ClassName, StringComparison.OrdinalIgnoreCase)
                    );
                while (exportWindowHelper is null)
                {
                    if (timeout.IsCancellationRequested)
                    {
                        throw new CapcutAutoTimeoutException("Waitting window timeout");
                    }
                    await Task.Delay(100, cancellationToken);
                    exportWindowHelper = exportWindows.FirstOrDefault();
                }
            }


            using var capture = new WinrtGraphicCapture();//new HdcCapture();
            capture.MaxFps = 6;
            if (WinrtGraphicCapture.IsCaptureCursorToggleSupported)
                capture.IsShowCursor = false;
            bool setupResult = false;
            {
                IntPtr? hmonitor = MonitorHelper.Monitors.FirstOrDefault();
                if (!hmonitor.HasValue) throw new CapcutAutoException($"Can't get monitor handle");

                setupResult = await capture.InitMonitorAsync(hmonitor.Value);
                if (!setupResult) throw new CapcutAutoException($"Init capture window failed");
            }

            await Task.Delay(500, cancellationToken);

            //render & chờ nút share
            using (CancellationTokenSource timeout = new CancellationTokenSource(WaitRenderTimeout))
            {
                while (true)
                {
                    if (timeout.IsCancellationRequested)
                    {
                        throw new CapcutAutoTimeoutException($"Check render timeout");
                    }
                    await Task.Delay(1000, cancellationToken);

                    Rectangle? windowArea = exportWindowHelper.GetArea();
                    if (!windowArea.HasValue) continue;

                    using Bitmap? bitmap = await capture.CaptureImageAsync();
                    if (bitmap is null) continue;
                    using Image<Hsv, byte> screenHsv = bitmap.ToImage<Hsv, byte>();

                    Rectangle bottomWindow = new Rectangle(//bottom right
                        windowArea.Value.X + windowArea.Value.Width - 400,
                        windowArea.Value.Y + windowArea.Value.Height - 66,
                        400,
                        66
                        );
                    (Rectangle? rectButton, string? name) = FindBlueButton(screenHsv, bottomWindow, "ExportShare", 1000);//miss click
                    if (rectButton.HasValue)
                    {
                        if ("Export".Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            Point center = rectButton.Value.GetCenter();
                            await exportWindowHelper.MouseClickAsync(center);//click export
                            continue;
                        }
                        else if ("Share".Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }
                    }
                }
            }
        }


        protected virtual async Task CloseAnotherPopupWindowAsync(IEnumerable<string> exceptTitles, CancellationToken cancellationToken = default)
        {
            if (_rootProcess is null) return;
            using CancellationTokenSource timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            while (!timeout.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
                var altTabWindows = _rootProcess.AltTabWindows.Where(x => !exceptTitles.Any(y => y.Equals(x.Title, StringComparison.OrdinalIgnoreCase)));
                foreach (var altTabWindow in altTabWindows)
                {
                    altTabWindow.SendMessage(PInvoke.WM_CLOSE, UIntPtr.Zero, IntPtr.Zero);
                }
            }
        }




        protected static Rectangle? FindWhiteCover(Image<Gray, byte> imageGray, double areaSize = 4500)
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

        protected static(Rectangle?, string?) FindBlueButton(Image<Hsv, byte> imageHsv, Rectangle crop, string whiteList, double areaSize)
        {
            using var imageHsvCrop = imageHsv.Copy(crop);
            using Image<Gray, byte> mask = imageHsvCrop.InRange(new Hsv(79, 111, 109), new Hsv(96, 255, 255));
            //mask.Save("C:\\BlueButtonMark.png");
            using Image<Gray, byte> maskBlur = new(mask.Size);
            CvInvoke.MedianBlur(mask, maskBlur, 5);

            using VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            using Mat hierarchy = new Mat();
            CvInvoke.FindContours(maskBlur, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            for (int i = 0; i < contours.Size; i++)
            {
                double area = CvInvoke.ContourArea(contours[i]);
                if (area > areaSize)
                {
                    Rectangle rectangle = CvInvoke.BoundingRectangle(contours[i]);

                    using var grayCropButton = mask.Copy(rectangle);
                    using var grayCropButtonScale = grayCropButton.Resize(2.0, Inter.Cubic);
#if DEBUG
                    grayCropButtonScale.Save("C:\\BlueButtonMark.png");
#endif

                    using var tessEngine = new TesseractEngine(Path.Combine(AppContext.BaseDirectory, "TessDatas"), "eng", EngineMode.Default);
                    tessEngine.SetVariable("tessedit_char_whitelist", new string(whiteList.Distinct().ToArray()));

                    using Bitmap preTess = grayCropButtonScale.ToBitmap();
                    using var img = PixConverter.ToPix(preTess);
                    using var page = tessEngine.Process(img, PageSegMode.SingleLine);
                    string text = page.GetText();
                    text = text.Trim().Replace(" ", string.Empty);
                    return (new Rectangle(crop.X + rectangle.X, crop.Y + rectangle.Y, rectangle.Width, rectangle.Height), text);
                }
            }

            return (null, null);
        }
    }
}
