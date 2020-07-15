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

using CMP2.Core;
using CMP2.Core.Model;
using CMP2.Test;

using MaterialDesignThemes.Wpf;

namespace CMP2
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      MainLogger.Info("### Start application ###");
      this.Loaded += (s, e) => { InitializeComponent(); };
      this.Loaded += MainWindow_Loaded;
      this.Closing += MainWindow_Closing;
    }
    private Log MainLogger { get; } = new Log(typeof(App));

    /// <summary>
    /// 메인원도우 로드 후 이벤트 처리
    /// </summary>
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      // 마우스로 배경을 클릭 했을 경우 컨트롤 숨기기 or 보이기
      this.MouseEnter += (_, e) =>
      {
        this.MainControllerControl.Visibility = Visibility.Visible;
        this.MainInfoControl.InfoPanel.VerticalAlignment = VerticalAlignment.Center;
      };
      this.MouseLeave += async (_, e) =>
      {
        await Task.Delay(5000);
        this.MainControllerControl.Visibility = Visibility.Collapsed;
        this.MainInfoControl.InfoPanel.VerticalAlignment = VerticalAlignment.Bottom;
      };

      // 창 드레그 움직임
      this.MouseLeftButtonDown += MainWindow_WindowDrag;

      // 종료 버튼
      this.ExitButteon.Click += (_, e) => { this.Close(); };

      // 디버그 전용 코드
      // 오류시 제거 요망
#if DEBUG
      PlayTest test = new PlayTest();
      test.StartPlayTest();
#endif
    }
    private void MainWindow_WindowDrag(object sender, MouseButtonEventArgs e) { this.DragMove(); e.Handled = true; }

    /// <summary>
    /// 메인 윈도우 종료 이벤트처리
    /// </summary>
    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      MainLogger.Info("### Exit application ###\n");
      MainMediaPlayer.Dispose();
      Application.Current.Shutdown();
    }
  }
}
