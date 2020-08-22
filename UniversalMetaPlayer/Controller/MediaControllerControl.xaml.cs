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
using UMP.Controller.Function;
using UMP.Core.Player;
using NAudio.Wave;
using UMP.Core.Global;

namespace UMP.Controller
{
  public partial class MediaControllerControl : UserControl
  {
    private MediaControllerControlViewModel ViewModel { get; }

    public MediaControllerControl()
    {
      Log log = new Log(typeof(MediaControllerControl));
      InitializeComponent();
      ViewModel = (MediaControllerControlViewModel)this.DataContext;
      Hook.KeyboardEvent += Hook_KeyboardEvent;

      this.Loaded += (_, e) =>
      {
        // 재생 진행바 이벤트 연결
        this.ProgressSlider.ValueChanged += ProgressSlider_ValueChanged;

        Window parentWindow = Window.GetWindow(Parent);

        GlobalKeyDownEvent.KeyDownEvent += GlobalKeyDownEvent_KeyDownEvent;

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

        GlobalProperty.PropertyChanged += (_, e) =>
        {
          if (e.PropertyName == "IsControllable")
            ControlPanel.IsEnabled = GlobalProperty.State.IsControllable;
        };

        log.Debug("초기화 완료");
      };
    }

    /// <summary>
    /// 컨트롤 키보드 이벤트 처리 (내부 이벤트)
    /// </summary>
    private void GlobalKeyDownEvent_KeyDownEvent(KeyEventArgs e)
    {
      if (GlobalProperty.Options.Getter<bool>(Enums.ValueName.HotKey) && GlobalProperty.State.IsControllable)
      {
        var viewModel = ((MediaControllerControlViewModel)this.DataContext);
        switch (GlobalProperty.Options.HotKey.Getter(e.Key))
        {
          case GlobalProperty.Options.HotKey.ControlTarget.PlayPause:
            if (viewModel.PlayPauseCommand.CanExecute(null))
              viewModel.PlayPauseCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Stop:
            if (viewModel.StopCommand.CanExecute(null))
              viewModel.StopCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Previous:
            if (viewModel.PreviousCommand.CanExecute(null))
              viewModel.PreviousCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Next:
            if (viewModel.NextCommand.CanExecute(null))
              viewModel.NextCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Mute:
            if (ViewModel.Volume > 0)
              ViewModel.Volume = 0;
            else
              ViewModel.Volume = ViewModel.BeforeVolume;
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Repeat:
            if (viewModel.RepeatCommand.CanExecute(null))
              viewModel.RepeatCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Shuffle:
            if (viewModel.ShuffleCommand.CanExecute(null))
              viewModel.ShuffleCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.MediaPositionForward:
            MediaPositionChanger(TimeSpan.FromSeconds(5));
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.MediaPositionBack:
            MediaPositionChanger(-TimeSpan.FromSeconds(5));
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.VolumeUp:
            ViewModel.Volume += 5;
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.VolumeDown:
            ViewModel.Volume -= 5;
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.PlayListWindow:
            viewModel.IsCheckedPlayListToggleButton = !viewModel.IsCheckedPlayListToggleButton;
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.FunctionWindow:
            viewModel.IsCheckedFunctionToggleButton = !viewModel.IsCheckedFunctionToggleButton;
            break;
          default:
            break;
        }
      }
    }

    /// <summary>
    /// 컨트롤 키보드 이벤트 처리 (전역 이벤트)
    /// </summary>
    private void Hook_KeyboardEvent(KeyboardEvent e)
    {
      if (e.State == NeatInput.Windows.Processing.Keyboard.Enums.KeyStates.Down && GlobalProperty.State.IsControllable)
      {
        switch (e.Key)
        {
          // Play/Pause
          case Keys.Play:
          case Keys.MediaPlayPause:
            if (MainMediaPlayer.PlaybackState == NAudio.Wave.PlaybackState.Playing)
            {
              MainMediaPlayer.ReserveCommand(PlaybackState.Paused);
              GlobalMessageEvent.Invoke($"외부키 활성 : Pause", true);
            }
            else
            {
              MainMediaPlayer.ReserveCommand(PlaybackState.Playing);
              GlobalMessageEvent.Invoke($"외부키 활성 : Play", true);
            }
            break;
          // Stop
          case Keys.MediaStop:
              MainMediaPlayer.ReserveCommand(PlaybackState.Stopped);
            GlobalMessageEvent.Invoke($"외부키 활성 : Stop", true);
            break;
          // Next
          case Keys.MediaNextTrack:
            _ = MainMediaPlayer.Next();
            GlobalMessageEvent.Invoke($"외부키 활성 : Next", true);
            break;
          // Previous
          case Keys.MediaPreviousTrack:
            _ = MainMediaPlayer.Previous();
            GlobalMessageEvent.Invoke($"외부키 활성 : Previous", true);
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
    /// 진행 슬라이더 이벤트 처리
    /// </summary>
    private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (MainMediaPlayer.MediaLoadedCheck && GlobalProperty.State.IsControllable)
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
