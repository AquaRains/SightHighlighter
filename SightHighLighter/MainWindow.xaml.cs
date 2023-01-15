using System;
using System.Windows;
using System.Windows.Media;
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
            if (capturedImage1.Source != null)
            {
                capturedImage1.Source = null;
            }
            //Bitmap img = ImageProcessor.CaptureScreen();
            //capturedImage1.Source = ImageProcessor.ImageSourceForBitmap(img);
            ////capturedImage1.UpdateLayout();
            (int matchCount, ImageSource markedImage) = ImageProcessor.FindImage();
            if (matchCount >= ImageProcessor.matchCountThreshold)
            {
                matchedCountsLabel.Content = string.Format("{0} (Too many, please set larger threshold)",matchCount);
            }
            else
            {
                matchedCountsLabel.Content = string.Format("{0}", matchCount);
            }
            capturedImage1.Source = markedImage;
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
                ImageProcessor.SetImageTemplate(fileName);

                //templateImageBGR.Source = ImageProcessor.bgrImg;
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
            if (templateImage1.Source == null)
            {
                MessageBox.Show("Image not set");
            }
            else
            {
                hookState = true;
                hookStateLabel.Content = "on";
                dispatcherTimer.Start();
            }
        }

        private void HookUnsubscribe()
        {
            hookState = false;
            hookStateLabel.Content = "off";
            matchedCountsLabel.Content = "unknown";
            dispatcherTimer.Stop();
        }





        // Todo: 너무 많이찾으면 찾다가 말기 - done 2023.01.03
        // Todo2: threshold 값 변경하는법 - done 2023.01.03
        // Todo3: 파일 읽을때까지 비활성화 - done 2023.01.03
        // Todo4: 파일 언로드하면서 hook 비활성화하는 기능
        // Todo5: 흑백으로 프로세싱
        // Todo6: parallel for in Imageprocessor + concurrentbag
        //        ref:https://math-development-geometry.tistory.com/38
    }
}
