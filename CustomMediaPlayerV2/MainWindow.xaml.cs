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
using MaterialDesignThemes.Wpf;

namespace CMP2
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      this.Loaded += (s, e) => { InitializeComponent(); };
      MainLogger.Info("### Start application ###");
      this.Loaded += MainWindow_Loaded;
      this.Closing += MainWindow_Closing;
    }
    private Log MainLogger { get; } = new Log(typeof(App));

    /// <summary>
    /// 메인원도우 로드 후 이벤트 처리
    /// </summary>
    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      await Task.Delay(3000);

      //MainMediaPlayer.Init(new MediaInfo(MediaType.Youtube, @"https://www.youtube.com/watch?v=jv543Nk5s18"), true);
      //MainMediaPlayer.Init(new MediaInfo(MediaType.Youtube, @"https://www.youtube.com/watch?v=3vhA8njtoQg"), true);

      MainMediaPlayer.Init(new MediaInfo(MediaType.Local, @"D:\Lab\Project\C#\CustomMediaPlayer\TestMusic\093 황인욱 - 포장마차.mp3"), true);

      await MainMediaPlayer.PlayList.Add(new MediaInfo(MediaType.Local, @"D:\Lab\Project\C#\CustomMediaPlayer\TestMusic\093 황인욱 - 포장마차.mp3"));
      await MainMediaPlayer.PlayList.Add(new MediaInfo(MediaType.Local, @"D:\Lab\Project\C#\CustomMediaPlayer\TestMusic\073 폴킴 - 안녕.mp3"));
      await MainMediaPlayer.PlayList.Add(new MediaInfo(MediaType.Local, @"D:\Lab\Project\C#\CustomMediaPlayer\TestMusic\090 창모 (CHANGMO) - 빌었어.mp3"));
    }

    /// <summary>
    /// 메인 윈도우 종료 이벤트처리
    /// </summary>
    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      MainMediaPlayer.Dispose();
      MainLogger.Info("### Exit application ###\n");
      Application.Current.Shutdown();
    }
  }
}
