using Compunet.YoloSharp;
using Compunet.YoloSharp.Plotting;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FireDetectionSystem.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using var predictor = new YoloPredictor("D:\\下载\\ultralytics-main\\runs\\detect\\train4\\weights\\best.onnx");

            // 等待异步检测结果
            var result = await Task.Run(
                () => predictor.Detect("C:\\Users\\Seeney\\Desktop\\fire.jpg")
            );

            image1.Source = new BitmapImage(new System.Uri("C:\\Users\\Seeney\\Desktop\\fire.jpg"));

            // 修正：使用 PlottingExtensions.PlotImage 并传入检测结果
            using var img = SixLabors.ImageSharp.Image.Load("C:\\Users\\Seeney\\Desktop\\fire.jpg");
            using var plotted = result.PlotImage(img);
            using var ms = new MemoryStream();
            plotted.Save(ms, new PngEncoder());
            ms.Seek(0, SeekOrigin.Begin);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            bitmap.Freeze();
            image2.Source = bitmap;
        }
    }
}
