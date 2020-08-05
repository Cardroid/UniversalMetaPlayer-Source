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

using UMP.Core;
using UMP.Utility;
using UMP.Utility.Effect;

namespace UMP
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      GlobalProperty.Load();
      InitializeComponent();
      this.KeyDown += (_, e) => GlobalEvent.KeyDownEventInvoke(e);
      this.Loaded += MainWindow_Loaded;
      this.Closing += MainWindow_Closing;
    }

    /// <summary>
    /// 메인원도우 로드 후 이벤트 처리
    /// </summary>
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      #region 마우스 우클릭 컨트롤 숨기기 or 보이기
      this.MainWindowButton.Visibility = Visibility.Collapsed;
      this.MainControllerControl.Visibility = Visibility.Collapsed;
      this.MainInfoControl.InfoPanel.VerticalAlignment = VerticalAlignment.Bottom;

      this.MouseRightButtonDown += (_, e) =>
      {
        if (this.MainWindowButton.Visibility != Visibility.Visible)
        {
          this.MainWindowButton.Visibility = Visibility.Visible;
          this.MainControllerControl.Visibility = Visibility.Visible;
          this.MainInfoControl.InfoPanel.VerticalAlignment = VerticalAlignment.Center;
        }
        else
        {
          this.MainWindowButton.Visibility = Visibility.Collapsed;
          this.MainControllerControl.Visibility = Visibility.Collapsed;
          this.MainInfoControl.InfoPanel.VerticalAlignment = VerticalAlignment.Bottom;
        }
      };
      #endregion

      // 창 드레그 움직임
      this.MainInfoControl.MouseLeftButtonDown += MainWindow_WindowDrag;

      this.BorderBrush = new SolidColorBrush(ThemeHelper.PrimaryColor);
    }

    private void MainWindow_WindowDrag(object sender, MouseButtonEventArgs e) { this.DragMove(); e.Handled = true; }

    /// <summary>
    /// 메인 윈도우 종료 이벤트처리
    /// </summary>
    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Hook.Dispose();
      MainMediaPlayer.Dispose();
      GlobalProperty.Save();
      App.MainLog.Info("############### Exit application ###############");
      Application.Current.Shutdown();
    }
  }
}
