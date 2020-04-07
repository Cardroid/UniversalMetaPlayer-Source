using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CustomMediaPlayer.Option.OptionPage
{
    public partial class AboutPage : UserControl
    {
        public AboutPage()
        {
            InitializeComponent();

            // 배이스 컬러 동기화
            this.Background = MainWindow.viewModel.BackgroundBrush;
            MainWindow.viewModel.BackgroundColorChanged += (b) => { this.Background = b; };

            // 좌측 상단에 로고 표시
            Mainlogo.Source = MainWindow.LogoImage;
            Mainlogo.Stretch = System.Windows.Media.Stretch.Fill;

            // 현재 버전 표시
            VersionLabel.Content = "Version : " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
