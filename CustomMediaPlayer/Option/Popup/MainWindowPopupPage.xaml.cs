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
    public partial class MainWindowPopupPage : UserControl
    {
        public MainWindowPopupPage(PopupContents popupContents)
        {
            InitializeComponent();

            // 배경색 동기화
            this.Background = ((MainWindow)System.Windows.Application.Current.MainWindow).ViewModel.BackgroundBrush;
            ((MainWindow)System.Windows.Application.Current.MainWindow).ViewModel.BackgroundColorChanged += (b) => { this.Background = b; };

            // 전경색 동기화
            this.BorderBrush = ThemeManager.DetectAppStyle().Item2.Resources["AccentColorBrush"] as SolidColorBrush;

            switch (popupContents)
            {
#if DEBUG
                case PopupContents.DebugMode:
                    ContentsLabel.Content = "본 프로그램은 테스트를 위해 제작되었습니다.\n사용에 문제가 발생할 수 있습니다.";
                    break;
#endif
                case PopupContents.NowMediaFileNotToExist:
                    ContentsLabel.Content = "재생 중인 미디어가 없습니다.";
                    break;
                case PopupContents.SaveMediaFileLoadError:
                    ContentsLabel.Content = "저장된 미디어 정보로 미디어 파일을 불러올 수 없습니다.";
                    break;
                case PopupContents.FileOpenError:
                    ContentsLabel.Content = "파일을 열 수 없습니다.";
                    break;
            }
        }
    }

    public enum PopupContents
    {
#if DEBUG
        DebugMode,
#endif
        NowMediaFileNotToExist,
        SaveMediaFileLoadError,
        FileOpenError
    }
}
