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
using System.Threading.Tasks;

using MaterialDesignThemes.Wpf;

using UMP.Core;
using UMP.Core.Player;
using UMP.Core.Model.Media;
using UMP.Controller.Dialog;
using UMP.Core.Global;
using UMP.Controller.ViewModel;
using System.Timers;

namespace UMP.Controller
{
  public partial class PlayListControl : UserControl
  {
    private Log Log { get; }
    public PlayListControl()
    {
      InitializeComponent();
      Log = new Log(typeof(PlayListControl));

      this.Loaded += PlayListControl_Loaded;
    }

    private void PlayListControl_Loaded(object sender, RoutedEventArgs e)
    {
      this.PlayListAddButton.Click += PlayListControlButton_Click;
      this.PlayListSaveButton.Click += PlayListControlButton_Click;
      this.PlayListLoadButton.Click += PlayListControlButton_Click;
      this.PlayListReloadButton.Click += PlayListControlButton_Click;
      this.PlayListResetButton.Click += PlayListControlButton_Click;

      // 플레이리스트 이름 변경
      this.PlayListName.MouseDown += (_, e) =>
      {
        this.PlayListName.Focus();
        this.PlayListName.SelectAll();
      };
      this.MouseDown += (_, e) => Keyboard.ClearFocus();
      this.PlayListName.KeyDown += (_, e) =>
       {
         if (this.PlayListName.IsKeyboardFocused && e.Key == Key.Enter)
           Keyboard.ClearFocus();
       };
      this.PlayListName.GotKeyboardFocus += (_, e) => GlobalKeyDownEvent.IsEnabled = false;
      this.PlayListName.LostKeyboardFocus += (_, e) =>
      {
        ((PlayListControlViewModel)this.DataContext).PlayListName = this.PlayListName.Text;
        GlobalKeyDownEvent.IsEnabled = true;
      };

      // 플레이리스트 더블 클릭시 재생 처리
      this.PlayList.MouseDoubleClick += async (_, e) => { await PlaySelectItem(); };

      this.PlayListGroupBox.PreviewMouseDown += PlayList_MouseDownUnSelect;
      this.PreviewKeyDown += PlayListControl_PreviewKeyDown;
      this.PlayListPopupBox.MouseEnter += (_, e) => { UnselectActive = false; };
      this.PlayListPopupBox.MouseLeave += (_, e) => { UnselectActive = true; };

      Window parentWindow = Window.GetWindow(this.Parent);
      parentWindow.Closing += ParentWindow_Closing;

      // 로그 설정
      Log.Debug("초기화 완료");
    }

    /// <summary>
    /// 플레이리스트 버튼들 처리
    /// </summary>
    private async void PlayListControlButton_Click(object sender, RoutedEventArgs e)
    {
      if (GlobalProperty.State.IsControllable && sender is Button button)
      {
        switch (button.Name)
        {
          case "PlayListAddButton":
            EnableControl(false);
            var addView = new PlayListAddDialog();
            addView.CloseEvent += () => { this.PlayListDialog.IsOpen = false; };
            await this.PlayListDialog.ShowDialog(addView);
            EnableControl(true);
            break;
          case "PlayListSaveButton":
            await MainMediaPlayer.PlayList.Save();
            break;
          case "PlayListLoadButton":
            EnableControl(false);
            var loadView = new PlayListLoadDialog();
            loadView.CloseEvent += () => { this.PlayListDialog.IsOpen = false; };
            await this.PlayListDialog.ShowDialog(loadView);
            EnableControl(true);
            break;
          case "PlayListReloadButton":
            EnableControl(false);
            var selectedList = this.PlayList.SelectedItems;
            if (selectedList != null)
            {
              for (int i = selectedList.Count - 1; i >= 0; i--)
                await MainMediaPlayer.PlayList.ReloadAsync((MediaInformation)selectedList[i]);
            }
            else
              await MainMediaPlayer.PlayList.ReloadAllAsync();
            EnableControl(true);
            break;
          case "PlayListResetButton":
            EnableControl(false);
            MainMediaPlayer.PlayList.Clear();
            EnableControl(true);
            break;
        }
      }
    }

    private void EnableControl(bool isEnable)
    {
      GlobalKeyDownEvent.IsEnabled = isEnable;
      this.PlayListGroupBox.IsEnabled = isEnable;
      GlobalProperty.State.IsControllable = isEnable;
    }

    /// <summary>
    /// 플레이리스트 빈 곳 클릭시 선택해제 처리
    /// </summary>
    private void PlayList_MouseDownUnSelect(object sender, MouseButtonEventArgs e)
    {
      if (UnselectActive)
      {
        HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
        if (r != null)
          if (r.VisualHit.GetType() == null || r.VisualHit.GetType() != typeof(ListBoxItem))
            PlayList.UnselectAll();;
      }
    }
    private bool UnselectActive = true;

    private void PlayListControl_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (GlobalProperty.State.IsControllable)
      {
        switch (e.Key)
        {
          case Key.Delete:
            DeleteSelectItme();
            break;
        }
      }
    }

    /// <summary>
    /// 선택된 아이템 재생 처리
    /// </summary>
    private async Task PlaySelectItem()
    {
      if (0 <= this.PlayList.SelectedIndex && this.PlayList.SelectedIndex < MainMediaPlayer.PlayList.Count)
      {
        if (MainMediaPlayer.PlayList[this.PlayList.SelectedIndex].LoadState)
        {
          if (await MainMediaPlayer.Init(MainMediaPlayer.PlayList[this.PlayList.SelectedIndex]))
          {
            MainMediaPlayer.PlayListEigenValue = MainMediaPlayer.PlayList.EigenValue;
            MainMediaPlayer.ReserveCommand(NAudio.Wave.PlaybackState.Playing);
          }
          else
            GlobalMessageEvent.Invoke("재생 실패! [로그를 확인해주세요]");
        }
        else
        {
          var info = MainMediaPlayer.PlayList[this.PlayList.SelectedIndex];
          Log.Fatal("미디어 정보가 로드되지 않았거나 로드에 실패했습니다", $"MediaLocation : [{info.MediaLocation}]\nTitle : [{info.Title}]");
          GlobalMessageEvent.Invoke("재생 실패! 미디어가 로드 되지 않음! [로그를 확인해 주세요]");
        }
      }
    }

    /// <summary>
    /// 선택된 아이템 삭제
    /// </summary>
    private void DeleteSelectItme()
    {
      if (this.PlayList.SelectedIndex >= 0)
      {
        var deleteItemList = this.PlayList.SelectedItems;
        for (int i = deleteItemList.Count - 1; i >= 0; i--)
          MainMediaPlayer.PlayList.Remove((MediaInformation)deleteItemList[i]);
      }
    }

    private bool CloseCount = false;

    private async void ParentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (!CloseCount && ((PlayListControlViewModel)this.DataContext).PlayList.NeedSave)
      {
        GlobalMessageEvent.Invoke("플래이 리스트에 변경사항이 있습니다 (저장 필요)\n(무시하고 닫으려면 다시 시도하세요)", true);
        CloseCount = true;
        e.Cancel = true;
        await Task.Delay(3000);
        CloseCount = false;
      }
    }
  }
}