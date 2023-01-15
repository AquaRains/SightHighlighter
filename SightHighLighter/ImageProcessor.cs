using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using OpenCvSharp;
using System.Threading.Tasks;
using OpenCvSharp.Extensions;

namespace SightHighlighter
{
    public static class ImageProcessor
    {
        private static int _screenHeight = (int)SystemParameters.PrimaryScreenHeight;
        private static int _screenWidth = (int)SystemParameters.PrimaryScreenWidth;

        private const int PrimaryScreenLeft = 0;
        private const int PrimaryScreenTop = 0;
        public static Mat ImgTemp = new();
        public static double Threshold { get; set; } = 0.99;

        public static readonly int MatchCountThreshold = 10;
        private static readonly System.Drawing.Pen _redPen = new(System.Drawing.Brushes.Red, 5);


        public static ImageSource Mat2ImageSource(Mat src, ImageSource dst)
        {
            return BitmapSourceFromBitmap(BitmapConverter.ToBitmap(src));
        }

        public static Bitmap CaptureScreen() // ref: https://stackoverflow.com/questions/4978157/how-to-search-for-an-image-on-screen-in-c
        {
            var image = new System.Drawing.Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using Graphics graphics = Graphics.FromImage(image);
            graphics.CopyFromScreen(PrimaryScreenLeft, PrimaryScreenTop, 0, 0, image.Size, CopyPixelOperation.SourceCopy);

            return image;
        }

        public static Bitmap CaptureMouseRegion(int xpos, int ypos)
        {
            var width = 960;
            var height = 540;
            var image = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.CopyFromScreen(xpos - width / 2, ypos - height / 2, 0, 0, image.Size, CopyPixelOperation.SourceCopy);
            }

            return image;
        }

        public static BitmapSource BitmapSourceFromBitmap(Bitmap bitmap)
        {
            // Conversion without interop
            // ref: https://stackoverflow.com/questions/26260654/wpf-converting-bitmap-to-imagesource/26261562#26261562
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return System.Windows.Media.Imaging.BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    System.Windows.Media.PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static ImageSource? ImageSourceForBitmap(Bitmap bmp)

        {
            var handle = bmp.GetHbitmap();
            try
            {
                ImageSource newSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                return newSource;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                DeleteObject(handle);
            }
        }

        public static unsafe (int, ImageSource) FindImage()
        {
            int matchCount = 0;

            Bitmap img = CaptureScreen();
            using Graphics graphics = Graphics.FromImage(img); // disposing this is important (memory leak)

            Mat mimg = OpenCvSharp.Extensions.BitmapConverter.ToMat(img);

            using Mat imgtemp_8UC3 = new Mat();
            ImgTemp.ConvertTo(imgtemp_8UC3, MatType.CV_8UC3);

            using Mat imgtemp_8UC4 = new Mat();
            Cv2.CvtColor(imgtemp_8UC3, imgtemp_8UC4, ColorConversionCodes.BGR2BGRA);
            imgtemp_8UC3.Dispose();

            using Mat result = new Mat();
            Cv2.MatchTemplate(mimg, imgtemp_8UC4, result, TemplateMatchModes.CCorrNormed);

            using var mat3 = new Mat<float>(result);
            var p = (nint)mat3.DataPointer;
            var spant = new Span<float>(mat3.DataPointer, mat3.Width * mat3.Height);

            for (int y = 0; y < mat3.Height; y++)
            {
                for (int x = 0; x < mat3.Width; x++)
                {
                    if (matchCount >= MatchCountThreshold)
                    {
                        // goto PixelLoopEnd;
                        continue;
                    }

                    float value = Indexer(p, x, y, mat3.Width);
                    //float value = spant[mat3.Width * y + x];
                    if (value >= Threshold)
                    {
                        var rect = new Rectangle(x, y, ImgTemp.Width, ImgTemp.Height);
                        graphics.DrawRectangle(_redPen, rect);
                        matchCount++;
                    }
                }
            }


            PixelLoopEnd:

            return (matchCount, ImageSourceFromBitmap(img));
        }

        private static unsafe float Indexer(nint p, int x, int y, int width)
        {
            return *(float*)(p + width * sizeof(float) * (long)y + sizeof(float) * (long)x);
        }

    }
}
