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
                    while (!capture.InitWindow(windowHelper.WindowHandle))
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
                        using Bitmap? bitmap = capture.Capture();
                        Rectangle? rectangle = null;
                        if (bitmap is not null)
                        {
                            using Image<Gray, byte> imageGray = bitmap.ToImage<Gray, byte>();
                            rectangle = imageGray.FindWhiteCover();
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



        #region Render
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

            var exportWindows = _rootProcess.AllWindows.Where(x =>
                 x.Title.StartsWith("Export", StringComparison.OrdinalIgnoreCase)
                 && "Qt622QWindowIcon".Equals(x.ClassName, StringComparison.OrdinalIgnoreCase)
                 );

            using (var capture = new WinrtGraphicCapture())
            {
                capture.MaxFps = 6;
                if (WinrtGraphicCapture.IsCaptureCursorToggleSupported)
                    capture.IsShowCursor = false;
                using (CancellationTokenSource timeout = new CancellationTokenSource(SetupCaptureTimeout))
                {
                    while (!capture.InitWindow(windowHelper.WindowHandle))
                    {
                        if (timeout.IsCancellationRequested)
                            throw new CapcutAutoTimeoutException($"Init capture window failed");
                        await Task.Delay(500);
                    }
                }

                //capture & click
                using (CancellationTokenSource timeout = new CancellationTokenSource(CheckImageAndWaitProjectTimeout))
                {
                    Hsv lowerBlue = new Hsv(79, 111, 109);
                    Hsv upperBlue = new Hsv(96, 255, 255);
                    List<Tuple<Hsv, Hsv>> tuples = new List<Tuple<Hsv, Hsv>>()
                    {
                        new Tuple<Hsv, Hsv>(lowerBlue, upperBlue)
                    };

                    while (!exportWindows.Any())
                    {
                        await Task.Delay(500, cancellationToken);
                        using Bitmap? bitmap = capture.Capture();
                        Rectangle? rectangle = null;
                        if (bitmap is not null)
                        {
                            using Image<Bgra, byte> imageBGRA = bitmap.ToImage<Bgra, byte>();
                            Rectangle crop = new Rectangle(imageBGRA.Width - 450, 0, 450, 80);//450 x 80 top right

                            (rectangle, string? name) = imageBGRA.FindFilledButtonWithText(tuples, crop, "Export", 900);
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

            using (var capture = new WinrtGraphicCapture())
            {
                capture.MaxFps = 6;
                if (WinrtGraphicCapture.IsCaptureCursorToggleSupported)
                    capture.IsShowCursor = false;
                bool setupResult = false;
                {
                    IntPtr? hmonitor = MonitorHelper.Monitors.FirstOrDefault();
                    if (!hmonitor.HasValue) throw new CapcutAutoException($"Can't get monitor handle");

                    setupResult = capture.InitMonitor(hmonitor.Value);
                    if (!setupResult) throw new CapcutAutoException($"Init capture monitor failed");
                }
                await Task.Delay(500, cancellationToken);

                List<Tuple<Hsv, Hsv>> cyanBlue = new List<Tuple<Hsv, Hsv>>()
                {
                    new Tuple<Hsv, Hsv>(new Hsv(79, 111, 109), new Hsv(96, 255, 255))
                };


                //render & chờ nút share
                using (CancellationTokenSource timeoutRender = new CancellationTokenSource(WaitRenderTimeout))
                {
                    using CancellationTokenSource timeoutClickExport = new CancellationTokenSource(CheckImageTimeout);
                    bool isClickedExport = false;
                    while (true)
                    {

                        if (timeoutRender.IsCancellationRequested)
                        {
                            throw new CapcutAutoTimeoutException($"Check render timeout");
                        }
                        await Task.Delay(1000, cancellationToken);
                        WindowHelper? exportWindowHelper = exportWindows.FirstOrDefault();
                        if (exportWindowHelper is null)
                            continue;

                        Rectangle? windowArea = exportWindowHelper.GetArea();
                        if (!windowArea.HasValue) continue;

                        using Bitmap? bitmap = capture.Capture();
                        if (bitmap is null) continue;
                        using Image<Bgra, byte> screenBGRA = bitmap.ToImage<Bgra, byte>();

                        Rectangle bottomWindow = new Rectangle(//bottom right
                            windowArea.Value.X + windowArea.Value.Width - 400,
                            windowArea.Value.Y + windowArea.Value.Height - 66,
                            400,
                            66
                            );
                        (Rectangle? rectButton, string? name) = screenBGRA.FindFilledButtonWithText(cyanBlue, bottomWindow, "ExportShare", 1000);//miss click
                        if (rectButton.HasValue)
                        {
                            if ("Export".Equals(name, StringComparison.OrdinalIgnoreCase))
                            {
                                Point center = rectButton.Value.GetCenter();
                                await exportWindowHelper.MouseClickAsync(center);//click export
                                isClickedExport = true;
                                continue;
                            }
                            else if ("Share".Equals(name, StringComparison.OrdinalIgnoreCase))
                            {
                                return;
                            }
                        }
                        if (!isClickedExport && timeoutClickExport.IsCancellationRequested)
                        {
                            throw new CapcutAutoTimeoutException("Waitting click 'Export' timeout");
                        }
                    }
                }
            }
        }
        #endregion



        #region Autocaption
        public async Task AutocaptionAsync(CancellationToken cancellationToken = default)
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
                while (!capture.InitWindow(windowHelper.WindowHandle))
                {
                    if (timeout.IsCancellationRequested)
                        throw new CapcutAutoTimeoutException($"Init capture window failed");
                    await Task.Delay(500);
                }
            }

            Hsv lowerBlue = new Hsv(79, 190, 0);
            Hsv upperBlue = new Hsv(100, 255, 255);
            List<Tuple<Hsv, Hsv>> cyanColor = new List<Tuple<Hsv, Hsv>>()
            {
                new Tuple<Hsv, Hsv>(lowerBlue, upperBlue)
            };

            bool isClickedText = false;
            bool isClickedAutocaption = false;
            using (CancellationTokenSource timeout = new CancellationTokenSource(CheckImageAndWaitProjectTimeout))
            {
                string windowTitles0 = string.Empty;
                while (true)
                {
                    await Task.Delay(500, cancellationToken);
                    using Bitmap? bitmap = capture.Capture();
                    Rectangle? rectangle = null;
                    if (bitmap is not null)
                    {
                        using Image<Bgra, byte> imageBgra = bitmap.ToImage<Bgra, byte>();

                        //text -> autocaption -> generate
                        if (!isClickedText)
                        {
                            Rectangle cropText = new Rectangle(0, 0, 200, 78);

                            List<Tuple<Hsv, Hsv>> whiteColor = new List<Tuple<Hsv, Hsv>>()
                            {
                                new Tuple<Hsv, Hsv>(new Hsv(0, 0, 150), new Hsv(180, 150, 255))
                            };

                            (Rectangle? rect, string? text) = imageBgra.FindTextButton(
                                whiteColor,
                                cropText,
                                "Text",
                                "Text",
                                PageIteratorLevel.Word
                                );
                            if (rect.HasValue)
                            {
                                Point center = rect.Value.GetCenter();
                                await windowHelper.MouseClickAsync(center);
                                continue;
                            }

                            (rect, text) = imageBgra.FindTextButton(
                                cyanColor,
                                cropText,
                                "Text",
                                "Text",
                                PageIteratorLevel.Word
                                );
                            if (rect.HasValue)
                            {
                                isClickedText = true;
                            }
                        }

                        if (isClickedText && !isClickedAutocaption)
                        {
                            Rectangle cropAutoCaption = new Rectangle(0, 0, 128, 300);
                            List<Tuple<Hsv, Hsv>> whiteColor = new List<Tuple<Hsv, Hsv>>()
                            {
                                new Tuple<Hsv, Hsv>(new Hsv(0, 0, 166), new Hsv(180, 170, 255))
                            };
                            (Rectangle? rect, string? text) = imageBgra.FindTextButton(
                                whiteColor,
                                cropAutoCaption,
                                "Auto captions",
                                "Auto captions",
                                PageIteratorLevel.TextLine
                                );
                            if (rect.HasValue)
                            {
                                Point center = rect.Value.GetCenter();
                                await windowHelper.MouseClickAsync(center);
                                continue;
                            }

                            (rect, text) = imageBgra.FindTextButton(
                                cyanColor,
                                cropAutoCaption,
                                "Auto captions",
                                "Auto captions",
                                PageIteratorLevel.TextLine
                                );
                            if (rect.HasValue)
                            {
                                isClickedAutocaption = true;
                            }
                        }

                        if (isClickedAutocaption)
                        {
                            int x = 350;
                            int y = 300;
                            Rectangle crop = new Rectangle(x, y, imageBgra.Width / 2 - x, (int)(imageBgra.Height * 2.0 / 3) - y);
                            (rectangle, string? name) = imageBgra.FindFilledButtonWithText(cyanColor, crop, "Generate", 1200, false);
                            if (rectangle.HasValue)
                            {
                                Point center = rectangle.Value.GetCenter();
                                windowTitles0 = string.Join("\r\n", _rootProcess.AllWindows);
                                await windowHelper.MouseClickAsync(center);
                                break;
                            }
                        }
                    }

                    if (timeout.IsCancellationRequested)
                    {
                        if (bitmap is null)
                            throw new CapcutAutoTimeoutException($"Can't capture image");
                        if (!isClickedText)
                            throw new CapcutAutoTimeoutException($"{nameof(isClickedText)} failed");
                        if (!isClickedAutocaption)
                            throw new CapcutAutoTimeoutException($"{nameof(isClickedAutocaption)} failed");
                        throw new CapcutAutoTimeoutException($"Check image timeout");
                    }
                }

                //wait popup closed ??
                string windowTitles1 = string.Empty;
                while (true)
                {
                    string windowTitles = string.Join("\r\n", _rootProcess.AllWindows);
                    if (!windowTitles0.Equals(windowTitles))
                    {
                        //popup showed
                        windowTitles1 = windowTitles;
                        break;
                    }
                    if (timeout.IsCancellationRequested)
                    {
                        throw new CapcutAutoTimeoutException($"Wait auto captions popup open timeout");
                    }
                    await Task.Delay(10, cancellationToken);
                }

                while (true)
                {
                    string windowTitles = string.Join("\r\n", _rootProcess.AllWindows);
                    if (!windowTitles1.Equals(windowTitles))
                    {
                        //popup closed
                        windowTitles1 = windowTitles;
                        break;
                    }
                    if (timeout.IsCancellationRequested)
                    {
                        throw new CapcutAutoTimeoutException($"Wait auto captions popup close timeout");
                    }
                    await Task.Delay(10, cancellationToken);
                }
            }
        }
        #endregion


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
    }
}
