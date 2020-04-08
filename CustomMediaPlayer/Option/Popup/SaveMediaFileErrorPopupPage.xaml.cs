using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro;

namespace CustomMediaPlayer.Option.Popup
{
    public partial class SaveMediaFileErrorPopupPage : UserControl
    {
        public SaveMediaFileErrorPopupPage()
        {
            InitializeComponent();

            // 배경색 동기화
            this.Background = ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundBrush;
            ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundColorChanged += (b) => { this.Background = b; };

            // 전경색 동기화
            this.BorderBrush = ThemeManager.DetectAppStyle().Item2.Resources["AccentColorBrush"] as SolidColorBrush;

            ContentsLabel.Content = "저장된 미디어 정보로 미디어 파일을 불러올 수 없습니다.";
        }
    }
}
