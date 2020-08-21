using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

using UMP.Core.Global;
using UMP.Core.Player;
using UMP.Utility;

namespace UMP
{
  public partial class MainWindow : Window
  {
    private MainWindowViewModel ViewModel { get; }
    public MainWindow()
    {
      GlobalProperty.Load();

      InitializeComponent();
      ViewModel = (MainWindowViewModel)this.DataContext;

      this.KeyDown += (_, e) => GlobalEvent.KeyDownEventInvoke(e);
      this.Loaded += MainWindow_Loaded;
      this.Closing += MainWindow_Closing;

      // 전역 메시지 닫힘 타이머 정의
      GlobalMessageCloseTimer = new System.Timers.Timer(3000) { AutoReset = true };
      GlobalMessageCloseTimer.Elapsed += (_, e) =>
      {
        Dispatcher.Invoke(new Action(() => {
          this.GlobalMessageBar.IsActive = false;
        }));
      };

      GlobalEvent.GlobalMessageEvent += GlobalEvent_GlobalMessageEvent;
      this.GlobalMessageBar.MouseLeftButtonDown += (_, e) =>
      {
        if (this.GlobalMessageBar.IsActive)
          this.GlobalMessageBar.IsActive = false;
      };
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

      GlobalProperty.PropertyChanged += (_, e) => 
      {
        if(e.PropertyName == "IsControllable")
        {
          if (GlobalProperty.State.IsControllable)
            this.MainControllerControl.IsEnabled = true;
          else
            this.MainControllerControl.IsEnabled = false;
        }
      };

      ViewModel.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == "PlayListControl")
        {
          if (MainPlayListControl != null)
            this.MainPlayListControl.Visibility = Visibility.Visible;
          else
            this.MainPlayListControl.Visibility = Visibility.Collapsed;
        }

        if (e.PropertyName == "FunctionControl")
        {
          if (FunctionControl != null)
            this.FunctionControl.Visibility = Visibility.Visible;
          else
            this.FunctionControl.Visibility = Visibility.Collapsed;
        }
      };

      this.GlobalMessageBar.IsActiveChanged += (_, e) => { if (!e.NewValue) this.GlobalMessage.Content = null; };

      #region 실행 시 정보 띄우기
      this.GlobalMessageBar.IsActive = true;
      this.Loaded += async (_, e) => { await Task.Delay(100000); this.GlobalMessageBar.IsActive = false; };
#if DEBUG
      this.Title = "UniversalMetaPlayer - V2 [Test Version]";
      this.GlobalMessage.Content =
        $"현재 버전은 [테스트 버전] [v{GlobalProperty.Predefine.FileVersion}] [{GlobalProperty.Predefine.BitVersion}] 입니다\n" +
        $"오류가 발생하면 로그파일과 함께 신고해주세요!\n";
#else
      this.Title = "UniversalMetaPlayer - V2";
      this.GlobalMessage.Content =
        $"현재 버전은 [v{GlobalProperty.Predefine.FileVersion}] [{GlobalProperty.Predefine.BitVersion}] 입니다\n" +
        $"오류가 발생하면 로그파일과 함께 신고해주세요!\n";
#endif
      #endregion
    }

    private System.Timers.Timer GlobalMessageCloseTimer { get; }

    private void MainWindow_WindowDrag(object sender, MouseButtonEventArgs e) { e.Handled = true; this.DragMove(); }

    private void GlobalEvent_GlobalMessageEvent(string message, bool autoClose)
    {
      Dispatcher.Invoke(new Action(() =>
      {
        GlobalMessageCloseTimer.Stop();
        this.GlobalMessageBar.IsActive = true;
        this.GlobalMessage.Content = message;
      }));
      if (autoClose)
        GlobalMessageCloseTimer.Start();
    }

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
