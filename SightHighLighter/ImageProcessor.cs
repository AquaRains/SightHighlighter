using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using OpenCvSharp;

namespace SightHighlighter
{
    public static class ImageProcessor
    {
        private static readonly int primaryScreenLeft = 0;
        private static readonly int primaryScreenTop = 0;
        public static Mat imgtemp = new Mat();
        private static double _threshold = 0.99;
        public static double threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }
        public static readonly int matchCountThreshold = 10;


        private static System.Drawing.Pen redPen = new System.Drawing.Pen(System.Drawing.Brushes.Red, 5);
        public static Bitmap CaptureScreen() // ref: https://stackoverflow.com/questions/4978157/how-to-search-for-an-image-on-screen-in-c
        {
            var image = new System.Drawing.Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(image))
            {   // http://m.csharpstudy.com/Tip/View?aspx=Tip-screen-copy.aspx&title=C%23%EC%97%90%EC%84%9C%20%EC%8A%A4%ED%81%AC%EB%A6%B0%20%EC%BA%A1%EC%B3%90
                graphics.CopyFromScreen(primaryScreenLeft, primaryScreenTop, 0, 0, image.Size, CopyPixelOperation.SourceCopy);
            }
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

        public static System.Windows.Media.Imaging.BitmapSource BitmapSourceFromBitmap(Bitmap bitmap)
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

        public static ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                ImageSource newSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(handle);
                return newSource;
            }
            catch (Exception ex)
            {
                DeleteObject(handle);
                return null;
            }
        }

        public static (int,ImageSource) FindImage()
        {
            int matchCount = 0;

            Bitmap img = CaptureScreen();
            Graphics graphics = Graphics.FromImage(img); // disposing this is important (memory leak)

            // img to mat using opencvsharp
            // ref: https://kanais2.tistory.com/307
            Mat mimg = OpenCvSharp.Extensions.BitmapConverter.ToMat(img);

            // convert matrix without memory violation
            // ref: https://answers.opencv.org/question/4811/convertto-from-cv_32f-to-cv_8u/
            // Todo: pull out conversion process from event loop and reduce duplication
            Mat imgtemp_8UC3 = new Mat();
            imgtemp.ConvertTo(imgtemp_8UC3, MatType.CV_8UC3);
            Mat imgtemp_8UC4 = new Mat();
            Cv2.CvtColor(imgtemp_8UC3, imgtemp_8UC4, ColorConversionCodes.BGR2BGRA);
            imgtemp_8UC3.Dispose();

            // template matching to result
            Mat result = new Mat();
            Cv2.MatchTemplate(mimg, imgtemp_8UC4, result, TemplateMatchModes.CCorrNormed);
            mimg.Dispose();
            imgtemp_8UC4.Dispose();

            // loop to match result
            // Type specific Mat method
            // ref: https://github.com/shimat/opencvsharp/wiki/Accessing-Pixel
            var mat3 = new Mat<float>(result);
            var indexer = mat3.GetIndexer();
            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    if (matchCount >= matchCountThreshold)
                    {
                        goto PixelLoopEnd;
                    }
                    double value = indexer[y, x];
                    if (value >= _threshold)
                    {
                        Rectangle rect = new Rectangle(x, y, imgtemp.Width, imgtemp.Height);
                        graphics.DrawRectangle(redPen, rect);
                        matchCount++;
                        //boxpoints.Add((x,y));
                    }
                }
            }
            PixelLoopEnd:
            mat3.Dispose();
            result.Dispose();

            graphics.Dispose();

            return (matchCount, ImageSourceForBitmap(img));
        }
    }
}
