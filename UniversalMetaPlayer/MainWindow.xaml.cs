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

using UMP.Core;
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
      WindowManager.CloseAll();
      GlobalProperty.Load();

      InitializeComponent();
      ViewModel = (MainWindowViewModel)this.DataContext;

      this.PreviewKeyDown += (_, e) =>
      {
        if (GlobalKeyDownEvent.IsEnabled && GlobalProperty.Options.HotKey.IsEnabled && GlobalProperty.Options.HotKey.ContainsKey(e.Key))
        {
          e.Handled = true;
          GlobalKeyDownEvent.Invoke(e);
        }
      };

      this.Loaded += MainWindow_Loaded;
      this.Closing += MainWindow_Closing;

      GlobalMessageEvent.MessageCloseEvent += () =>
       {
         Dispatcher.Invoke(new Action(() =>
         {
           this.GlobalMessageBar.IsActive = false;
         }));
       };

      GlobalMessageEvent.MessageEvent += (msg) =>
      {
        Dispatcher.Invoke(new Action(() =>
        {
          this.GlobalMessageBar.IsActive = true;
          this.GlobalMessage.Content = msg;
        }));
      };

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
        if (e.PropertyName == "IsControllable")
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

      this.Activated += (_, e) => GlobalProperty.State.IsFocusActive = true;
      this.Deactivated += (_, e) => GlobalProperty.State.IsFocusActive = false;

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

    private void MainWindow_WindowDrag(object sender, MouseButtonEventArgs e) { e.Handled = true; this.DragMove(); }

    private bool IsPlayListSaveCancel = false;

    /// <summary>
    /// 메인 윈도우 종료 이벤트처리
    /// </summary>
    private async void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(!IsPlayListSaveCancel && MainMediaPlayer.PlayList.NeedSave)
      {
        GlobalMessageEvent.Invoke("플래이 리스트에 변경사항이 있습니다 (저장 필요)\n(무시하고 닫으려면 다시 시도하세요)", true);
        IsPlayListSaveCancel = true;
        e.Cancel = true;
        await Task.Delay(3000);
        IsPlayListSaveCancel = false;
        return;
      }
      Hook.Dispose();
      MainMediaPlayer.Dispose();
      GlobalProperty.Save();
      App.MainLog.Info("############### Exit application ###############");
      Application.Current.Shutdown();
    }
  }
}
