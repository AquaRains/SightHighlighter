using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SightHighlighter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool hookState = false;
        private DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            TimerSetup();
            DataContext = new MainWindowViewModel();
        }

        private void TimerSetup()
        {
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10); // d, h, m, s, ms
            dispatcherTimer.IsEnabled= false;
            

            hookStateLabel.Content = "off";
        }

        private void dispatcherTimer_Tick(object? sender, EventArgs e)
        {
            //if (capturedImage1.Source != null)
            //{
            //    capturedImage1.Source.Dispose();
            //}
            //Bitmap img = ImageProcessor.CaptureScreen();
            //capturedImage1.Source = ImageProcessor.ImageSourceForBitmap(img);
            ////capturedImage1.UpdateLayout();
            capturedImage1.Source = ImageProcessor.FindImage();
        }

        private void fileLoadButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "MyImage.png";
            dialog.DefaultExt = ".png";
            dialog.Filter = "PNG|*.png|JPEG|*.jpg;*.jpeg;*.jpe,*jiff|Bitmap|*.bmp;*.dib|GIF|*.gif|TIFF|*.tif;*.tiff|ICO|*.ico|HEIC|*.heic;*.hif|WEBP|*.webp|All Files|*.*";

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                string fileName = dialog.FileName;
                templateImage1.Source = new BitmapImage(new Uri(fileName));
                ImageProcessor.imgtemp = new OpenCvSharp.Mat(fileName);
            }
        }

        private void hookStateToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (hookState)
            {
                HookUnsubscribe();
            }
            else
            {
                HookSubscribe();
            }
        }

        private void HookSubscribe()
        {
            hookState = true;
            hookStateLabel.Content = "on";
            dispatcherTimer.Start();
        }

        private void HookUnsubscribe()
        {
            hookState = false;
            hookStateLabel.Content = "off";
            dispatcherTimer.Stop();
        }


        // Todo: 너무 많이찾으면 찾다가 말기
        // Todo2: threshold 값 변경하는법
        // Todo3: 파일 읽을때까지 비활성화
        // Todo4: 파일 언로드하면서 hook 비활성화하는 기능
    }
}
