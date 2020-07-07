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

using CMP2.Core;

namespace CMP2.Controller
{
  public partial class MediaControllerControl : UserControl
  {
    private MediaControllerControlViewModel ViewModel { get; set; }

    public MediaControllerControl()
    {
      this.Loaded += (s, e) =>
      {
        InitializeComponent();
        ViewModel = (MediaControllerControlViewModel)this.DataContext;

        this.ProgressSlider.ValueChanged += ProgressSlider_ValueChanged;

        // 마우스 휠을 사용한 볼륨조절
        ((MainWindow)Application.Current.MainWindow).MouseWheel += (s, e) =>
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

        this.PlayPauseButton.Tag = ControlButton.PlayPauseButton;
        this.StopButton.Tag = ControlButton.StopButton;
        this.NextButton.Tag = ControlButton.NextButton;
        this.PreviousButton.Tag = ControlButton.PreviousButton;
        this.RepeatButton.Tag = ControlButton.RepeatButton;
        this.ShuffleButton.Tag = ControlButton.ShuffleButton;
        this.SettingButton.Tag = ControlButton.SettingButton;

        this.PlayPauseButton.Click += ControllerButton_ClickHandler;
        this.StopButton.Click += ControllerButton_ClickHandler;
        this.NextButton.Click += ControllerButton_ClickHandler;
        this.PreviousButton.Click += ControllerButton_ClickHandler;
        this.RepeatButton.Click += ControllerButton_ClickHandler;
        this.ShuffleButton.Click += ControllerButton_ClickHandler;
        this.SettingButton.Click += ControllerButton_ClickHandler;

        // 키보드 컨트롤
        //((MainWindow)Application.Current.MainWindow).KeyDown += (s, e) =>
        //{
        //  switch (e.Key)
        //  {
        //    case Key.Play:
        //    case Key.MediaPlayPause:
        //      break;
        //  }
        //};

        // 볼륨 뮤트 버튼
        this.VolumeMuteButton.Click += (s, e) =>
        {
          if (ViewModel.Volume > 0)
            ViewModel.Volume = 0;
          else
            ViewModel.Volume = ViewModel.BeforeVolume;
        };

        // (미디어 길이 <-> 남은 미디어 시간) 전환
        this.DurationLabel.MouseDown += (s, e) => 
        {
          MainMediaPlayer.Option.DurationViewStatus = !MainMediaPlayer.Option.DurationViewStatus;
          ViewModel.ApplyUI();
        };
      };
    }

    /// <summary>
    /// 컨트롤 버튼을 구분용 (버튼 테그)
    /// </summary>
    private enum ControlButton
    {
      PlayPauseButton,
      StopButton,
      NextButton,
      PreviousButton,
      RepeatButton,
      ShuffleButton,
      SettingButton
    }

    private void ControllerButton_ClickHandler(object sender, RoutedEventArgs e)
    {
      switch ((ControlButton)((Button)sender).Tag)
      {
        case ControlButton.PlayPauseButton:
          if (MainMediaPlayer.MediaLoadedCheck)
            if (MainMediaPlayer.PlaybackState == NAudio.Wave.PlaybackState.Playing)
              MainMediaPlayer.Pause();
            else
              MainMediaPlayer.Play();
          break;
        case ControlButton.StopButton:
          if (MainMediaPlayer.MediaLoadedCheck)
              MainMediaPlayer.Stop();
          break;
        case ControlButton.NextButton:
          break;
        case ControlButton.PreviousButton:
          break;
        case ControlButton.RepeatButton:
          ++MainMediaPlayer.Option.RepeatPlayOption;
          break;
        case ControlButton.ShuffleButton:
          break;
        case ControlButton.SettingButton:
          break;
      }
    }

    private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (MainMediaPlayer.MediaLoadedCheck)
      {
        if (this.ProgressSlider.IsMouseOver && Mouse.LeftButton == MouseButtonState.Pressed)
        {
          // 진행 슬라이더를 클릭 또는 드래그 하여 미디어 재생 위치 변경
          MainMediaPlayer.AudioFile.CurrentTime = TimeSpan.FromMilliseconds(e.NewValue);
        }
      }
    }
  }
}
