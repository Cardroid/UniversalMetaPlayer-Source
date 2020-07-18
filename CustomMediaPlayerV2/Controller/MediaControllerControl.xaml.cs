﻿using System;
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
using CMP2.Controller.ViewModel;
using CMP2.Core;
using CMP2.Core.Model;
using CMP2.Utility;

using NeatInput.Windows.Events;
using NeatInput.Windows.Processing.Keyboard.Enums;

namespace CMP2.Controller
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
        this.SettingButton.Tag = ControlType.Setting;

        // 버튼 이벤트 연결
        this.PlayPauseButton.Click += ControllerButton_ClickHandler;
        this.StopButton.Click += ControllerButton_ClickHandler;
        this.NextButton.Click += ControllerButton_ClickHandler;
        this.PreviousButton.Click += ControllerButton_ClickHandler;
        this.RepeatButton.Click += ControllerButton_ClickHandler;
        this.ShuffleButton.Click += ControllerButton_ClickHandler;
        this.SettingButton.Click += ControllerButton_ClickHandler;

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

        // 볼륨 뮤트 버튼
        VolumeMuteButton.Click += (s, e) =>
        {
          if (ViewModel.Volume > 0)
            ViewModel.Volume = 0;
          else
            ViewModel.Volume = ViewModel.BeforeVolume;
        };

        // (미디어 길이 <-> 남은 미디어 시간) 전환
        this.TotalTimeLabel.MouseDown += (s, e) =>
        {
          MainMediaPlayer.Option.DurationViewStatus = !MainMediaPlayer.Option.DurationViewStatus;
          ViewModel.ApplyUI();
        };
      };

      this.Loaded += (s, e) =>
      {
        Log log = new Log(typeof(MediaInfoControl));
        log.Debug("초기화 성공");
      };
    }

    /// <summary>
    /// 컨트롤 키보드 이벤트 처리 (내부 이벤트)
    /// </summary>
    private void GlobalEvent_KeyDownEvent(KeyEventArgs e)
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
          ViewModel.IsPlayListWindowOpen = !ViewModel.IsPlayListWindowOpen;
          break;
        // Setting
        case Key.S:
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

    /// <summary>
    /// 컨트롤 버튼을 구분용 (버튼 테그)
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
      if (e.State == NeatInput.Windows.Processing.Keyboard.Enums.KeyStates.Down)
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
            break;
          // Previous
          case Keys.MediaPreviousTrack:
            break;
        }
      }
    }

    private void MediaPositionChanger(TimeSpan appendtime)
    {
      if (!MainMediaPlayer.MediaLoadedCheck || appendtime == TimeSpan.Zero)
        return;

      if(appendtime > TimeSpan.Zero)
      {
        // 양수
        if (MainMediaPlayer.AudioFile.TotalTime > (MainMediaPlayer.AudioFile.CurrentTime + appendtime))
          MainMediaPlayer.AudioFile.CurrentTime += appendtime;
        else
          MainMediaPlayer.AudioFile.CurrentTime = MainMediaPlayer.AudioFile.TotalTime;
      }
      else
      {
        // 음수
        if (MainMediaPlayer.AudioFile.CurrentTime + appendtime > TimeSpan.Zero)
          MainMediaPlayer.AudioFile.CurrentTime += appendtime;
        else
          MainMediaPlayer.AudioFile.CurrentTime = TimeSpan.Zero;
      }
      ViewModel.ApplyUI(false);
    }

    /// <summary>
    /// 컨트롤 버튼 클릭 이벤트 처리
    /// </summary>
    private void ControllerButton_ClickHandler(object sender, RoutedEventArgs e)
    {
      switch ((ControlType)((Button)sender).Tag)
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
          break;
        case ControlType.Previous:
          break;
        case ControlType.Repeat:
          ++MainMediaPlayer.Option.RepeatPlayOption;
          break;
        case ControlType.Shuffle:
          break;
        case ControlType.Setting:
          break;
      }
    }

    /// <summary>
    /// 진행 슬라이더 이벤트 처리
    /// </summary>
    private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (MainMediaPlayer.MediaLoadedCheck)
        if (this.ProgressSlider.IsMouseOver && Mouse.LeftButton == MouseButtonState.Pressed)
        {
          // 진행 슬라이더를 클릭 또는 드래그 하여 미디어 재생 위치 변경
          MainMediaPlayer.AudioFile.CurrentTime = TimeSpan.FromMilliseconds(e.NewValue);
        }
    }
  }
}
