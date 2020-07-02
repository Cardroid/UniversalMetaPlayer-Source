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
    public MainWindowPopupPage(PopupContents popupContents, string errormessage = "")
    {
      InitializeComponent();

      // 배경색 동기화
      this.Background = ((MainWindow)System.Windows.Application.Current.MainWindow).ViewModel.BackgroundBrush;
      ((MainWindow)System.Windows.Application.Current.MainWindow).ViewModel.BackgroundColorChanged += (b) => { this.Background = b; };

      // 전경색 동기화
      this.BorderBrush = ThemeManager.DetectAppStyle().Item2.Resources["AccentColorBrush"] as SolidColorBrush;
      this.MessageBorder.BorderBrush = this.BorderBrush;

      switch (popupContents)
      {
#if DEBUG
        case PopupContents.DebugMode:
          ContentsLabel.Content = "본 프로그램은 테스트를 위해 제작되었습니다.\n사용에 문제가 발생할 수 있습니다.";
          errormessage = "Debug Mode";
          break;
#endif
        case PopupContents.StreamIsNull:
          ContentsLabel.Content = "재생 중인 미디어가 없습니다.";
          break;
        case PopupContents.FileError:
          ContentsLabel.Content = "파일을 열 수 없거나 파일이 존재하지 않습니다.";
          break;
        case PopupContents.LoadError:
          ContentsLabel.Content = "불러오는 중 문제가 발생했습니다.";
          break;
        case PopupContents.SaveError:
          ContentsLabel.Content = "저장 중 문제가 발생했습니다.";
          break;
        case PopupContents.FormatError:
          ContentsLabel.Content = "형식에 문제가 있습니다.";
          break;
        case PopupContents.Error:
        default:
          ContentsLabel.Content = "오류가 발생했습니다.";
          break;
      }
      if (string.IsNullOrWhiteSpace(errormessage))
        this.MessageBorder.Visibility = Visibility.Collapsed;
      else
        this.ErrorMessageLabel.Content = errormessage;

      ((MainWindow)System.Windows.Application.Current.MainWindow).MainPopup.IsOpen = true;
    }
  }

  public enum PopupContents
  {
#if DEBUG
    /// <summary>
    /// Debug모드 경고
    /// </summary>
    DebugMode,
#endif
    /// <summary>
    /// 오류가 발생했습니다.
    /// </summary>
    Error,
    /// <summary>
    /// 재생 중인 미디어가 없습니다.
    /// </summary>
    StreamIsNull,
    /// <summary>
    /// 파일을 열 수 없거나 파일이 존재하지 않습니다.
    /// </summary>
    FileError,
    /// <summary>
    /// 불러오는 중 문제가 발생했습니다.
    /// </summary>
    LoadError,
    /// <summary>
    /// 저장 중 문제가 발생했습니다.
    /// </summary>
    SaveError,
    /// <summary>
    /// 형식에 문제가 있습니다.
    /// </summary>
    FormatError
  }
}
