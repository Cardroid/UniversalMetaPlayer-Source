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

using UMP.Controller.Dialog;
using UMP.Controller.ViewModel;
using UMP.Core;
using UMP.Core.Model;
using UMP.Utility;

using MaterialDesignThemes.Wpf;
using GongSolutions.Wpf.DragDrop.Utilities;
using UMP.Core.Function;

namespace UMP.Controller
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
      this.PlayListSaveButton.Tag = PlayListControlType.Save;
      this.PlayListLoadButton.Tag = PlayListControlType.Load;
      this.PlayListReloadButton.Tag = PlayListControlType.Reload;
      this.PlayListResetButton.Tag = PlayListControlType.Reset;

      this.PlayListAddButton.Click += PlayListControlButton_Click;
      this.PlayListSaveButton.Click += PlayListControlButton_Click;
      this.PlayListLoadButton.Click += PlayListControlButton_Click;
      this.PlayListReloadButton.Click += PlayListControlButton_Click;
      this.PlayListResetButton.Click += PlayListControlButton_Click;

      // 플레이 리스트 이름 변경
      this.PlayListNameEditButton.Click += (_, e) =>
      {
        if (this.PlayListName.IsReadOnly)
        {
          this.PlayListName.Focusable = true;
          this.PlayListName.IsReadOnly = false;
          this.PlayListName.IsReadOnlyCaretVisible = true;
          GlobalEvent.KeyDownEventHandled = true;
          this.PlayListName.Focus();
          this.PlayListName.CaretIndex = this.PlayListName.Text.Length;
          this.PlayListNameEditButton.Content = new PackIcon() { Kind = PackIconKind.EditOff, Width = 20, Height = 20 };
        }
        else
        {
          this.PlayListName.Focusable = false;
          this.PlayListName.IsReadOnly = true;
          this.PlayListName.IsReadOnlyCaretVisible = false;
          GlobalEvent.KeyDownEventHandled = false;
          this.PlayListNameEditButton.Content = new PackIcon() { Kind = PackIconKind.Edit, Width = 20, Height = 20 };
        }
      };
      this.PlayListName.KeyDown += (_, e) =>
       {
         if (!this.PlayListName.IsReadOnly && e.Key == Key.Enter)
         {
           this.PlayListName.Focusable = false;
           this.PlayListName.IsReadOnly = true;
           this.PlayListName.IsReadOnlyCaretVisible = false;
           GlobalEvent.KeyDownEventHandled = false;
           this.PlayListNameEditButton.Content = new PackIcon() { Kind = PackIconKind.Edit, Width = 20, Height = 20 };
         }
       };

      this.PlayList.MouseDoubleClick += PlayList_MouseDoubleClick;
      this.PlayListGroupBox.PreviewMouseDown += PlayList_MouseDownUnSelect;
      this.PlayList.KeyDown += PlayListControl_KeyDown;
      this.PlayListPopupBox.MouseEnter += (_, e) => { UnselectActive = false; };
      this.PlayListPopupBox.MouseLeave += (_, e) => { UnselectActive = true; };

      // 로그 설정
      Log.Debug("초기화 완료");
    }

    private void PlayListControl_KeyDown(object sender, KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.Delete:
          if (this.PlayList.SelectedIndex >= 0)
          {
            var deleteItemList = this.PlayList.SelectedItems;
            for (int i = deleteItemList.Count - 1; i >= 0; i--)
              ViewModel.PlayList.Remove((MediaInformation)deleteItemList[i]);
          }
          break;
      }
    }

    /// <summary>
    /// 플레이 리스트 버튼들 처리
    /// </summary>
    private async void PlayListControlButton_Click(object sender, RoutedEventArgs e)
    {
      if (((Button)sender).Tag is PlayListControlType type)
      {
        switch (type)
        {
          case PlayListControlType.Add:
            EnableControl(false);
            var addView = new PlayListAddDialog();
            addView.Close += () => { this.PlayListDialog.IsOpen = false; };
            await this.PlayListDialog.ShowDialog(addView);
            EnableControl(true);
            break;
          case PlayListControlType.Save:
            await ViewModel.PlayList.Save();
            break;
          case PlayListControlType.Load:
            EnableControl(false);
            var loadView = new PlayListLoadDialog();

            var loadResultObj = await this.PlayListDialog.ShowDialog(loadView);
            if (loadResultObj is bool loadResult && loadResult)
            {
              var result = loadView.GetResult();
              GlobalProperty.IsControllable = false;

              if (result.TryGetValue("SaveCurrentPlayList", out string saveCurrentPlayList) && saveCurrentPlayList.ToLower() == bool.TrueString)
                await ViewModel.PlayList.Save();

              bool loadContinue = result.TryGetValue("LoadContinue", out string _loadContinue) && _loadContinue.ToLower() == bool.TrueString;
              if (!loadContinue)
                ViewModel.PlayList.Clear();

              if (result.TryGetValue("PlayListFilePath", out string loadValue))
                await ViewModel.PlayList.Load(loadValue, !loadContinue);
              GlobalProperty.IsControllable = true;
            }
            EnableControl(true);
            break;
          case PlayListControlType.Reload:
            EnableControl(false);
            var selectedList = this.PlayList.SelectedItems;
            if (selectedList != null)
            {
              for (int i = selectedList.Count - 1; i >= 0; i--)
                await ViewModel.PlayList.ReloadAsync((MediaInformation)selectedList[i]);
            }
            else
              await ViewModel.PlayList.ReloadAllAsync();
            EnableControl(true);
            break;
          case PlayListControlType.Reset:
            EnableControl(false);
            ViewModel.PlayList.Clear();
            EnableControl(true);
            break;
        }
      }
    }

    private void EnableControl(bool isEnable)
    {
      if (isEnable)
      {
        GlobalEvent.KeyDownEventHandled = false;
        this.PlayListGroupBox.IsEnabled = true;
      }
      else
      {
        GlobalEvent.KeyDownEventHandled = true;
        this.PlayListGroupBox.IsEnabled = false;
      }
    }

    /// <summary>
    /// 컨트롤 버튼 구분용 (버튼 테그)
    /// </summary>
    private enum PlayListControlType
    {
      Add, Save, Load, Reload, Reset
    }

    /// <summary>
    /// 플레이 리스트 빈 곳 클릭시 선택해제 처리
    /// </summary>
    private void PlayList_MouseDownUnSelect(object sender, MouseButtonEventArgs e)
    {
      if (UnselectActive)
      {
        HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
        if (r != null)
          if (r.VisualHit.GetType() == null || r.VisualHit.GetType() != typeof(ListBoxItem))
          {
            PlayList.UnselectAll();
            ViewModel.PlayListSelectIndex = -1;
          }
      }
    }
    private bool UnselectActive = true;

    /// <summary>
    /// 플레이 리스트 더블 클릭시 재생 처리
    /// </summary>
    private async void PlayList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (0 <= ViewModel.PlayListSelectIndex && ViewModel.PlayList.Count > ViewModel.PlayListSelectIndex)
      {
        if (ViewModel.PlayList[ViewModel.PlayListSelectIndex].LoadState)
        {
          MainMediaPlayer.PlayListPlayMediaIndex = ViewModel.PlayListSelectIndex;
          if (await MainMediaPlayer.Init(new MediaLoader(ViewModel.PlayList[ViewModel.PlayListSelectIndex])))
            MainMediaPlayer.PlayListEigenValue = ViewModel.PlayList.EigenValue;
        }
        else
        {
          var info = ViewModel.PlayList[ViewModel.PlayListSelectIndex];
          Log.Fatal("미디어 정보가 로드되지 않았거나 로드에 실패 했습니다", $"MediaLocation : [{info.MediaLocation}]\nTitle : [{info.Title}]");
        }
      }
    }
  }
}