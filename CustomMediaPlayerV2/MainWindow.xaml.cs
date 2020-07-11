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

using ControlzEx.Theming;
using MahApps.Metro.Controls;

using CMP2.Core;
using CMP2.Core.Model;

namespace CMP2
{
  public partial class MainWindow : MetroWindow
  {
    public MainWindow()
    {
      this.Loaded += MainWindow_Loaded;
      this.Closing += MainWindow_Closing;

      // 초기 메인 테마 설정
      ThemeManager.Current.ChangeTheme(this, IGlobalProperty.MainTheme);
    }

    /// <summary>
    /// 메인원도우 로드 후 이벤트 처리
    /// </summary>
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      InitializeComponent();

      MainMediaPlayer.Init(new MediaInfo(@"D:\Lab\Project\C#\CustomMediaPlayer\TestMusic\093 황인욱 - 포장마차.mp3"), true);
    }

    /// <summary>
    /// 메인 윈도우 종료 이벤트처리
    /// </summary>
    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      MainMediaPlayer.Dispose();
      Application.Current.Shutdown();
    }
  }
}
