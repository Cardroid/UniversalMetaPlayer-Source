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
using UMP.Utils;

using NeatInput.Windows.Events;
using NeatInput.Windows.Processing.Keyboard.Enums;
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

    /// <summary>
    /// 진행 슬라이더 이벤트 처리
    /// </summary>
    private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (MainMediaPlayer.MediaLoadedCheck && GlobalProperty.State.IsControllable)
      {
        if (this.ProgressSlider.IsMouseOver && Mouse.LeftButton == MouseButtonState.Pressed)
          // 진행 슬라이더를 클릭 또는 드래그 하여 미디어 재생 위치 변경
          MainMediaPlayer.AudioCurrentTime = TimeSpan.FromMilliseconds(e.NewValue);
      }
    }
  }
}
