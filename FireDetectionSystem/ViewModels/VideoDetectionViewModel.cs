using Compunet.YoloSharp.Plotting;
using OpenCvSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FireDetectionSystem.ViewModels
{
    class VideoDetectionViewModel : BindableBase,IDialogAware
    {
        private string vediePath;

        public string VideoPath
        {
            get { return vediePath; }
            set { vediePath = value;RaisePropertyChanged(); }
        }

        public MediaElement MediaElement;

        private ImageSource detetionSoure;
       

        public ImageSource DetetionSoure
        {
            get { return detetionSoure; }
            set { detetionSoure = value;RaisePropertyChanged(); }
        }

        private ImageSource origneImage;

        public ImageSource OrigneImage
        {
            get { return origneImage; }
            set { origneImage = value; RaisePropertyChanged(); }
        }


        private CancellationTokenSource _cts;

        public DelegateCommand SelectVideoPathCommnad { get; set; }

        public DelegateCommand DetectionVideoCommand { get; set; }


        public VideoDetectionViewModel()
        {
            SelectVideoPathCommnad = new DelegateCommand(SelectVideoPath);

            DetectionVideoCommand = new DelegateCommand(DetectionVideo);
        }
        // 视频检测方法
        private async  void DetectionVideo()
        {
            if(string.IsNullOrEmpty(VideoPath) || !File.Exists(VideoPath))
            {
                return;
            }
            if(_cts != null)
                _cts.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

          

            App.Current.Dispatcher.Invoke(() =>
            {
                if (MediaElement!=null)
                {
                   
                    MediaElement.Play();
                }
            });



            await Task.Run(() =>
            {
                var capture = new VideoCapture(VideoPath);
                Mat frame = new Mat();
                if(capture is null || !capture.IsOpened())
                {
                    return;
                }

                while(!token.IsCancellationRequested)
                {
                    capture.Read(frame);
                    if (frame.Empty())
                    {                    
                        break;
                    }
                    var imageSharpImage = MatToImageSharp(frame);
                    var image = ImageSharpToBitmapImage(imageSharpImage);
                    var result = FireDetectionModule.Detect(imageSharpImage);
                    using var plotted = result.PlotImage(imageSharpImage) as SixLabors.ImageSharp.Image<Rgba32>;
                    if (plotted != null)
                    {
                        var bitmapImage = ImageSharpToBitmapImage(plotted);
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            OrigneImage = image;//显示原始图像
                            DetetionSoure = bitmapImage;//将处理后的图像显示在界面上
                        });
                    }
                    // 控制帧率
                    Thread.Sleep(10); 
                }
                capture.Release();
            },token);
        }
        // ImageSharp 图像转换为 BitmapImage
        private ImageSource ImageSharpToBitmapImage(SixLabors.ImageSharp.Image<Rgba32> image)
        {
            using var ms = new MemoryStream();
            image.SaveAsPng(ms);
            ms.Seek(0, SeekOrigin.Begin);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }
        // 选择视频文件路径
        private void SelectVideoPath()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "视频文件|*.mp4;*.avi;*.mov;*.mkv|所有文件|*.*"
            };
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                VideoPath = openFileDialog.FileName;
            }      
           
        }
        // OpenCV Mat 转换为 ImageSharp 图像
        private SixLabors.ImageSharp.Image<Rgba32> MatToImageSharp(Mat mat)
        {
            using var ms = new MemoryStream();
            mat.WriteToStream(ms, ".jpg");
            ms.Seek(0, SeekOrigin.Begin);
            return SixLabors.ImageSharp.Image.Load<Rgba32>(ms);
        }



        #region
        public DialogCloseListener RequestClose => throw new NotImplementedException();

        public bool CanCloseDialog()
        {
            throw new NotImplementedException();
        }

        public void OnDialogClosed()
        {
            throw new NotImplementedException();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
