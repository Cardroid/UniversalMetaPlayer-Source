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
using System.Windows.Threading;

using UMP.Controller.ViewModel;
using UMP.Core;
using UMP.Core.Model;
using UMP.Utility;

using NeatInput.Windows.Events;
using NeatInput.Windows.Processing.Keyboard.Enums;

namespace UMP.Controller
{
  public partial class MediaControllerControl : UserControl
  {
    private MediaControllerControlViewModel ViewModel { get; }

    public MediaControllerControl()
    {
      InitializeComponent();
      ViewModel = (MediaControllerControlViewModel)this.DataContext;
      Hook.KeyboardEvent += Hook_KeyboardEvent;

      this.Loaded += (_, e) =>
      {
        // 재생 진행바 이벤트 연결
        this.ProgressSlider.ValueChanged += ProgressSlider_ValueChanged;

        // 버튼 테그 추가
        this.PlayPauseButton.Tag = ControlType.PlayPause;
        this.StopButton.Tag = ControlType.Stop;
        this.NextButton.Tag = ControlType.Next;
        this.PreviousButton.Tag = ControlType.Previous;
        this.RepeatButton.Tag = ControlType.Repeat;
        this.ShuffleButton.Tag = ControlType.Shuffle;
        this.PlayListCheckBox.Tag = ControlType.PlayList;
        this.SettingCheckBox.Tag = ControlType.Setting;

        // 버튼 이벤트 연결
        this.PlayPauseButton.Click += ControllerButton_ClickHandler;
        this.StopButton.Click += ControllerButton_ClickHandler;
        this.NextButton.Click += ControllerButton_ClickHandler;
        this.PreviousButton.Click += ControllerButton_ClickHandler;
        this.RepeatButton.Click += ControllerButton_ClickHandler;
        this.ShuffleButton.Click += ControllerButton_ClickHandler;

        Window parentWindow = Window.GetWindow(Parent);

        GlobalEvent.KeyDownEvent += GlobalEvent_KeyDownEvent;

        // 마우스 휠을 사용한 볼륨조절
        parentWindow.MouseWheel += (s, e) =>
        {
          if (e.Delta > 0)
          {
            if (Math.Abs(e.Delta) > 120)
              ViewModel.Volume += 4;
            ViewModel.Volume += 1;
          }
          else if (e.Delta < 0)
          {
            if (Math.Abs(e.Delta) > 120)
              ViewModel.Volume -= 4;
            ViewModel.Volume -= 1;
          }
        };

        // (미디어 길이 <-> 남은 미디어 시간) 전환
        this.TotalTimeLabel.MouseDown += (s, e) =>
        {
          MainMediaPlayer.Option.DurationViewStatus = !MainMediaPlayer.Option.DurationViewStatus;
          ViewModel.ApplyUI();
        };

        this.SettingCheckBox.Click += (_, e) =>
        {
          SettingControlOpen(this.SettingCheckBox.IsChecked.GetValueOrDefault());
        };
        this.PlayListCheckBox.Click += (_, e) =>
        {
          PlayListControlOpen(this.PlayListCheckBox.IsChecked.GetValueOrDefault());
        };

        GlobalProperty.PropertyChanged += (e) =>
        {
          if (e == "IsControllable")
          {
            ControlPanel.IsEnabled = GlobalProperty.IsControllable;
          }
        };
      };

      this.Loaded += (s, e) =>
      {
        Log log = new Log(typeof(MediaControllerControl));
        log.Debug("초기화 완료");
      };
    }

    private void SettingControlOpen(bool isOpen)
    {
      var parentWindow = (MainWindow)Window.GetWindow(Parent);
      if (isOpen)
      {
        parentWindow.MainOptionControl.Visibility = Visibility.Visible;
        parentWindow.Width += parentWindow.MainOptionControl.Width;
        parentWindow.MinWidth += parentWindow.MainOptionControl.Width;
        this.SettingCheckBox.IsChecked = true;
      }
      else
      {
        parentWindow.MainOptionControl.Visibility = Visibility.Collapsed;
        parentWindow.MinWidth -= parentWindow.MainOptionControl.Width;
        parentWindow.Width -= parentWindow.MainOptionControl.Width;
        this.SettingCheckBox.IsChecked = false;
      }
    }

    private void PlayListControlOpen(bool isOpen)
    {
      var parentWindow = (MainWindow)Window.GetWindow(Parent);
      if (isOpen)
      {
        parentWindow.MainPlayListControl.Visibility = Visibility.Visible;
        parentWindow.Height += parentWindow.MainPlayListControl.Height;
        parentWindow.MinHeight += parentWindow.MainPlayListControl.Height;
        this.PlayListCheckBox.IsChecked = true;
      }
      else
      {
        parentWindow.MainPlayListControl.Visibility = Visibility.Collapsed;
        parentWindow.MinHeight -= parentWindow.MainPlayListControl.Height;
        parentWindow.Height -= parentWindow.MainPlayListControl.Height;
        this.PlayListCheckBox.IsChecked = false;
      }
    }

    /// <summary>
    /// 컨트롤 키보드 이벤트 처리 (내부 이벤트)
    /// </summary>
    private void GlobalEvent_KeyDownEvent(KeyEventArgs e)
    {
      if (GlobalProperty.Options.HotKey && GlobalProperty.IsControllable)
      {
        switch (e.Key)
        {
          // Play/Pause
          case Key.Space:
          case Key.Pause:
          case Key.P:
            if (MainMediaPlayer.PlaybackState == NAudio.Wave.PlaybackState.Playing)
              MainMediaPlayer.Pause();
            else
              MainMediaPlayer.Play();
            break;
          // Stop
          case Key.O:
            MainMediaPlayer.Stop();
            break;
          // Repeat
          case Key.I:
            ++MainMediaPlayer.Option.RepeatPlayOption;
            break;
          // Shuffle
          case Key.U:
            break;
          // PlayList
          case Key.L:
            PlayListControlOpen(((MainWindow)Window.GetWindow(Parent)).MainPlayListControl.Visibility != Visibility.Visible);
            break;
          // Setting
          case Key.S:
            SettingControlOpen(((MainWindow)Window.GetWindow(Parent)).MainOptionControl.Visibility != Visibility.Visible);
            break;
          // Mute
          case Key.M:
            if (ViewModel.Volume > 0)
              ViewModel.Volume = 0;
            else
              ViewModel.Volume = ViewModel.BeforeVolume;
            break;

          case Key.Left:
            MediaPositionChanger(-TimeSpan.FromSeconds(5));
            break;
          case Key.Right:
            MediaPositionChanger(TimeSpan.FromSeconds(5));
            break;
          case Key.Up:
            ViewModel.Volume += 5;
            break;
          case Key.Down:
            ViewModel.Volume -= 5;
            break;
        }
      }
    }

    /// <summary>
    /// 컨트롤 버튼 구분용 (버튼 테그)
    /// </summary>
    private enum ControlType
    {
      PlayPause,
      Stop,
      Next,
      Previous,
      Repeat,
      Shuffle,
      PlayList,
      Setting
    }

    /// <summary>
    /// 컨트롤 키보드 이벤트 처리 (전역 이벤트)
    /// </summary>
    private void Hook_KeyboardEvent(KeyboardEvent e)
    {
      if (e.State == NeatInput.Windows.Processing.Keyboard.Enums.KeyStates.Down && GlobalProperty.IsControllable)
      {
        switch (e.Key)
        {
          // Play/Pause
          case Keys.Play:
          case Keys.MediaPlayPause:
            if (MainMediaPlayer.PlaybackState == NAudio.Wave.PlaybackState.Playing)
              MainMediaPlayer.Pause();
            else
              MainMediaPlayer.Play();
            break;
          // Stop
          case Keys.MediaStop:
            MainMediaPlayer.Stop();
            break;
          // Next
          case Keys.MediaNextTrack:
            MainMediaPlayer.Next();
            break;
          // Previous
          case Keys.MediaPreviousTrack:
            MainMediaPlayer.Previous();
            break;
        }
      }
    }

    private void MediaPositionChanger(TimeSpan appendtime)
    {
      if (!MainMediaPlayer.MediaLoadedCheck || appendtime == TimeSpan.Zero)
        return;

      if (appendtime > TimeSpan.Zero)
      {
        // 양수
        if (MainMediaPlayer.AudioTotalTime > (MainMediaPlayer.AudioCurrentTime + appendtime))
          MainMediaPlayer.AudioCurrentTime += appendtime;
        else
          MainMediaPlayer.AudioCurrentTime = MainMediaPlayer.AudioTotalTime;
      }
      else
      {
        // 음수
        if (MainMediaPlayer.AudioCurrentTime + appendtime > TimeSpan.Zero)
          MainMediaPlayer.AudioCurrentTime += appendtime;
        else
          MainMediaPlayer.AudioCurrentTime = TimeSpan.Zero;
      }
      ViewModel.ApplyUI(false);
    }

    /// <summary>
    /// 컨트롤 버튼 클릭 이벤트 처리
    /// </summary>
    private void ControllerButton_ClickHandler(object sender, RoutedEventArgs e)
    {
      if (((Button)sender).Tag is ControlType type && GlobalProperty.IsControllable)
      {
        switch (type)
        {
          case ControlType.PlayPause:
            if (MainMediaPlayer.MediaLoadedCheck)
              if (MainMediaPlayer.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                MainMediaPlayer.Pause();
              else
                MainMediaPlayer.Play();
            break;
          case ControlType.Stop:
            if (MainMediaPlayer.MediaLoadedCheck)
              MainMediaPlayer.Stop();
            break;
          case ControlType.Next:
            MainMediaPlayer.Next();
            break;
          case ControlType.Previous:
            MainMediaPlayer.Previous();
            break;
          case ControlType.Repeat:
            MainMediaPlayer.Option.RepeatPlayOption++;
            break;
          case ControlType.Shuffle:
            MainMediaPlayer.Option.Shuffle = !MainMediaPlayer.Option.Shuffle;
            break;
          case ControlType.Setting:
            break;
        }
      }
    }

    /// <summary>
    /// 진행 슬라이더 이벤트 처리
    /// </summary>
    private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (MainMediaPlayer.MediaLoadedCheck && GlobalProperty.IsControllable)
      {
        if (this.ProgressSlider.IsMouseOver && Mouse.LeftButton == MouseButtonState.Pressed)
        {
          // 진행 슬라이더를 클릭 또는 드래그 하여 미디어 재생 위치 변경
          MainMediaPlayer.AudioCurrentTime = TimeSpan.FromMilliseconds(e.NewValue);
        }
      }
    }
  }
}
