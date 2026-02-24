using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using Tesseract;
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
            try
            {
                PInvoke.SetCursorPos(point.X, point.Y);
                PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                await Task.Delay(10, cancellationToken);
            }
            finally
            {
                PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
        }

        public static Image<Gray, byte> GetMarks(this Image<Hsv, byte> imageHsv, params Tuple<Hsv, Hsv>[] minMaxs)
            => GetMasks(imageHsv, minMaxs.AsEnumerable());
        public static Image<Gray, byte> GetMasks(this Image<Hsv, byte> imageHsv, IEnumerable<Tuple<Hsv, Hsv>> minMaxs)
        {
            if (imageHsv == null) throw new ArgumentNullException(nameof(imageHsv));
            if (minMaxs == null || !minMaxs.Any()) throw new ArgumentNullException(nameof(minMaxs));

            Image<Gray, byte> combinedMask = new Image<Gray, byte>(imageHsv.Size);
            foreach (var range in minMaxs)
            {
                using Image<Gray, byte> tempMask = imageHsv.InRange(range.Item1, range.Item2);
                combinedMask = combinedMask.Or(tempMask);
            }
            return combinedMask;
        }



        public static Rectangle? FindWhiteCover(
            this Image<Gray, byte> imageGray,
            double areaSize = 4500
            )
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

        public static (Rectangle?, string?) FindFilledButtonWithText(
            this Image<Bgra, byte> imageBGRA,
            IEnumerable<Tuple<Hsv, Hsv>> minMaxs,
            Rectangle? crop,
            string whiteList,
            double areaSize,
            bool isOcrGray = true
            )
        {
            using var imageBGRACrop = imageBGRA.Copy(crop ?? imageBGRA.ROI);
            using var imageHSVCrop = imageBGRACrop.Convert<Hsv, byte>();
            using Image<Gray, byte> mask = imageHSVCrop.GetMasks(minMaxs);
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

                    Pix img;
                    if (isOcrGray)
                    {
                        using var grayCropButton = mask.Copy(rectangle);
                        using var grayCropButtonScale = grayCropButton.Resize(2.0, Inter.Cubic);
#if DEBUG
                        grayCropButtonScale.Save("C:\\BlueButtonMark.png");
#endif
                        using Bitmap preTess = grayCropButtonScale.ToBitmap();
                        img = PixConverter.ToPix(preTess);
                    }
                    else
                    {
                        using var bgraCropButton = imageBGRACrop.Copy(rectangle);
                        using var bgraCropButtonScale = bgraCropButton.Resize(2.0, Inter.Cubic);
#if DEBUG
                        bgraCropButtonScale.Save("C:\\BlueButtonMark.png");
#endif
                        using Bitmap preTess = bgraCropButtonScale.ToBitmap();
                        img = PixConverter.ToPix(preTess);
                    }
                    using var tessEngine = new TesseractEngine(Path.Combine(AppContext.BaseDirectory, "TessDatas"), "eng", EngineMode.Default);
                    tessEngine.SetVariable("tessedit_char_whitelist", new string(whiteList.Distinct().ToArray()));
                    try
                    {
                        using var page = tessEngine.Process(img, PageSegMode.SingleLine);
                        string text = page.GetText();
                        text = text.Trim().Replace(" ", string.Empty);
                        return (new Rectangle((crop?.X ?? 0) + rectangle.X, (crop?.Y ?? 0) + rectangle.Y, rectangle.Width, rectangle.Height), text);
                    }
                    finally
                    {
                        img.Dispose();
                    }
                }
            }

            return (null, null);
        }

        public static (Rectangle?, string?) FindTextButton(
            this Image<Bgra, byte> imageBGRA,
            IEnumerable<Tuple<Hsv, Hsv>> minMaxs,
            Rectangle crop,
            string whiteList,
            string textToFind,
            PageIteratorLevel pageIteratorLevel,
            bool isOcrGray = true
            )
        {
            double zoomLevel = 2.0;
            using var imageBGRACrop = imageBGRA.Copy(crop);
            using var imageHSVCrop = imageBGRACrop.Convert<Hsv, byte>();
            using Image<Gray, byte> mask = imageHSVCrop.GetMasks(minMaxs);

            using var imageBGRACropWithMask = new Image<Bgra, byte>(imageBGRACrop.Size);
            CvInvoke.BitwiseAnd(imageBGRACrop, imageBGRACrop, imageBGRACropWithMask, mask);

            using var zoomedMask = imageBGRACropWithMask.Resize(zoomLevel, Inter.Cubic);
            //CvInvoke.Threshold(zoomedMask, zoomedMask, 128, 255, ThresholdType.Binary);

            using var bitmap = zoomedMask.ToBitmap();
#if DEBUG
            bitmap.Save("C:\\FindTextButton.png", System.Drawing.Imaging.ImageFormat.Png);
#endif
            using var pix = PixConverter.ToPix(bitmap);

            using var tessEngine = new TesseractEngine(Path.Combine(AppContext.BaseDirectory, "TessDatas"), "eng", EngineMode.Default);
            if (!string.IsNullOrWhiteSpace(whiteList))
                tessEngine.SetVariable("tessedit_char_whitelist", new string(whiteList.Distinct().ToArray()));
            using var page = tessEngine.Process(pix);
            using var iter = page.GetIterator();
            iter.Begin();
            do
            {
                string currentText = iter.GetText(pageIteratorLevel).Trim();

                if (!string.IsNullOrEmpty(currentText) &&
                    currentText.Contains(textToFind, StringComparison.OrdinalIgnoreCase))
                {
                    if (iter.TryGetBoundingBox(pageIteratorLevel, out Rect bounds))
                    {
                        // 4. Chuyển đổi tọa độ từ vùng Crop về tọa độ ảnh gốc
                        Rectangle globalRect = new Rectangle(
                            crop.X + (int)(bounds.X1 / zoomLevel),
                            crop.Y + (int)(bounds.Y1 / zoomLevel),
                            (int)(bounds.Width / zoomLevel),
                            (int)(bounds.Height / zoomLevel)
                        );

                        return (globalRect, currentText);
                    }
                }
            } while (iter.Next(pageIteratorLevel));

            return (null, null);
        }
    }
}
