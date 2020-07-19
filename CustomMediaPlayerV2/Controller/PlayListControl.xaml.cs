using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CMP2.Controller.ViewModel;
using CMP2.Core;

namespace CMP2.Controller
{
  public partial class PlayListControl : UserControl
  {
    private PlayListControlViewModel ViewModel { get; set; }
    private Log Log { get; }
    public PlayListControl()
    {
      InitializeComponent();
      ViewModel = (PlayListControlViewModel)this.DataContext;
      Log = new Log(typeof(PlayListControl));

      this.Loaded += PlayListControl_Loaded;
    }

    private void PlayListControl_Loaded(object sender, RoutedEventArgs e)
    {
      // 헤더 설정
      this.Title.Header = "제목";
      this.MediaType.Header = "타입";
      this.Duration.Header = "길이";

      // 로그 설정
      Log.Debug("초기화 성공");

      this.PlayList.MouseDoubleClick += PlayList_MouseDoubleClick;
      this.PlayList.PreviewMouseDown += PlayList_MouseDownUnSelect;
      this.PlayListGroupBox.PreviewMouseDown += PlayList_MouseDownUnSelect;
    }

    private void PlayList_MouseDownUnSelect(object sender, MouseButtonEventArgs e)
    {
      HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
      if (r.VisualHit.GetType() != typeof(ListBoxItem))
        PlayList.UnselectAll();
    }

    private void PlayList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (0 <= ViewModel.PlayListSelectIndex && ViewModel.PlayList.Count > ViewModel.PlayListSelectIndex)
      {
        if (MainMediaPlayer.PlayList[ViewModel.PlayListSelectIndex].LoadedCheck == Core.Model.LoadState.Loaded)
        {
          MainMediaPlayer.PlayListPlayMediaIndex = ViewModel.PlayListSelectIndex;
          MainMediaPlayer.Init(new Core.Model.Media(MainMediaPlayer.PlayList[ViewModel.PlayListSelectIndex].MediaType, MainMediaPlayer.PlayList[ViewModel.PlayListSelectIndex].MediaLocation));
        }
        else
        {
          var info = MainMediaPlayer.PlayList[ViewModel.PlayListSelectIndex];
          Log.Error($"<{info.MediaType}>[{info.Title}] 미디어 정보가 로드되지 않았거나 로드에 실패 했습니다.");
        }
      }
    }
  }
}