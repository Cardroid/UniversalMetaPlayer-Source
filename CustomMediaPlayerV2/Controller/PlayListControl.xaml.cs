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
using CMP2.Controller.Dialog;
using CMP2.Controller.ViewModel;
using CMP2.Core;
using MaterialDesignThemes.Wpf;

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
      this.PlayListAddButton.Tag = PlayListControlType.Add;
      this.PlayListEditButton.Tag = PlayListControlType.Edit;
      this.PlayListSaveButton.Tag = PlayListControlType.Save;
      this.PlayListLoadButton.Tag = PlayListControlType.Load;
      this.PlayListReloadButton.Tag = PlayListControlType.Reload;
      this.PlayListResetButton.Tag = PlayListControlType.Reset;

      this.PlayListAddButton   .Click += PlayListControlButton_Click;
      this.PlayListEditButton  .Click += PlayListControlButton_Click;
      this.PlayListSaveButton  .Click += PlayListControlButton_Click;
      this.PlayListLoadButton  .Click += PlayListControlButton_Click;
      this.PlayListReloadButton.Click += PlayListControlButton_Click;
      this.PlayListResetButton .Click += PlayListControlButton_Click;

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

    private async void PlayListControlButton_Click(object sender, RoutedEventArgs e)
    {
      if (((Button)sender).Tag is PlayListControlType type)
      {
        switch (type)
        {
          case PlayListControlType.Add:
            break;
          case PlayListControlType.Edit:
            break;
          case PlayListControlType.Save:
            break;
          case PlayListControlType.Load:
            break;
          case PlayListControlType.Reload:
            await this.PlayListDialog.ShowDialog(new PlayListAddDialog());
            break;
          case PlayListControlType.Reset:
            break;
        }
      }
    }

    /// <summary>
    /// 컨트롤 버튼 구분용 (버튼 테그)
    /// </summary>
    private enum PlayListControlType
    {
      Add, Edit, Save, Load, Reload, Reset
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