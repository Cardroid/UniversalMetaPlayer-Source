﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using MaterialDesignThemes.Wpf;
using MaterialDesignColors.ColorManipulation;

using NAudio.Wave;

using UMP.Core.Player;
using UMP.Core.Model.ViewModel;
using UMP.Core.Global;
using UMP.Controller.Function;
using UMP.Controller.WindowHelper;
using UMP.Utility;

namespace UMP.Controller.ViewModel
{
  public class MediaControllerControlViewModel : ViewModelBase
  {
    public MediaControllerControlViewModel()
    {
      MainMediaPlayer.PlayStateChanged += MainMediaPlayer_PlayStateChanged;
      MainMediaPlayer.PropertyChanged += MainMediaPlayer_PropertyChanged;
      MainMediaPlayer.TickEvent += MainMediaPlayer_TickEvent;
      MainMediaPlayer.Option.PropertyChanged += Option_PropertyChanged;
      MainMediaPlayer.PlayList.Field_PropertyChanged += PlayList_Field_PropertyChanged;
      ThemeHelper.ThemeChangedEvent += ThemeHelper_ThemeChangedEvent;

      GlobalKeyDownEvent.KeyDownEvent += GlobalKeyDownEvent_KeyDownEvent;

      PlayPauseCommand = new RelayCommand((o) => PlayPause());
      StopCommand = new RelayCommand((o) => Stop());
      NextCommand = new RelayCommand((o) => Next());
      PreviousCommand = new RelayCommand((o) => Previous());
      RepeatCommand = new RelayCommand((o) => Repeat());
      ShuffleCommand = new RelayCommand((o) => Shuffle());
    }

    #region Volume
    private float _BeforeVolume = 80;
    public float BeforeVolume
    {
      get => _BeforeVolume;
      private set => _BeforeVolume = Math.Clamp(value, 10, 100);
    }
    public float Volume
    {
      get => MainMediaPlayer.Volume * 100;
      set
      {
        MainMediaPlayer.Volume = Math.Clamp(value / 100f, 0f, 1f); // 오류 방지용
        if (MainMediaPlayer.Volume != 0)
          BeforeVolume = Volume;
        OnPropertyChanged("Volume");
        OnPropertyChanged("VolumeString");
        OnPropertyChanged("VolumeMuteButtonIcon");
      }
    }

    // 볼륨 버튼 관련
    public PackIconKind VolumeMuteButtonIcon => VolumeMuteButtonIconChanger();
    public PackIconKind VolumeMuteButtonIconChanger()
    {
      if (Volume > 70)
        return PackIconKind.VolumeHigh;
      else if (Volume > 25)
        return PackIconKind.VolumeMedium;
      else if (Volume > 0)
        return PackIconKind.VolumeLow;
      else
        return PackIconKind.VolumeOff;
    }
    #endregion

    #region Play / Pause
    public RelayCommand PlayPauseCommand { get; }

    private void PlayPause()
    {
      if (MainMediaPlayer.MediaLoadedCheck)
      {
        if (MainMediaPlayer.PlaybackState == PlaybackState.Playing)
          MainMediaPlayer.ReserveCommand(PlaybackState.Paused);
        else
          MainMediaPlayer.ReserveCommand(PlaybackState.Playing);
      }
    }

    public PackIcon PlayPauseStateIcon
    {
      get
      {
        if (MainMediaPlayer.PlaybackState == PlaybackState.Playing)
          return new PackIcon { Width = 50, Height = 50, Kind = PackIconKind.Pause };
        else
          return new PackIcon { Width = 50, Height = 50, Kind = PackIconKind.Play };
      }
    }
    #endregion

    #region Stop
    public RelayCommand StopCommand { get; }
    private void Stop() => MainMediaPlayer.ReserveCommand(PlaybackState.Stopped);
    #endregion

    #region Next
    public RelayCommand NextCommand { get; }
    private void Next() => _ = MainMediaPlayer.Next();
    #endregion

    #region Previous
    public RelayCommand PreviousCommand { get; }
    private void Previous() => _ = MainMediaPlayer.Previous();
    #endregion

    #region Repeat
    public RelayCommand RepeatCommand { get; }
    private void Repeat() => MainMediaPlayer.Option.RepeatPlayOption++;
    public PackIcon RepeatPlayOptionIcon
    {
      get
      {
        var RepeatIcon = new PackIcon() { Width = 30, Height = 30 };
        if (MainMediaPlayer.Option.RepeatPlayOption == 0) // 반복 안함
        { RepeatIcon.Kind = PackIconKind.RepeatOff; }
        else if (MainMediaPlayer.Option.RepeatPlayOption == 1) // 한 곡 반복
        { RepeatIcon.Kind = PackIconKind.RepeatOnce; }
        else if (MainMediaPlayer.Option.RepeatPlayOption == 2) // 전채 반복
        { RepeatIcon.Kind = PackIconKind.Repeat; }
        return RepeatIcon;
      }
    }
    #endregion

    #region Shuffle
    public RelayCommand ShuffleCommand { get; }
    private void Shuffle() => MainMediaPlayer.Option.Shuffle = !MainMediaPlayer.Option.Shuffle;
    public PackIcon ShuffleIcon
    {
      get
      {
        if (MainMediaPlayer.Option.Shuffle)
          return new PackIcon { Width = 30, Height = 30, Kind = PackIconKind.ShuffleVariant };
        else
          return new PackIcon { Width = 40, Height = 40, Kind = PackIconKind.ShuffleDisabled };
      }
    }
    #endregion

    #region FunctionControl
    public bool IsCheckedFunctionToggleButton
    {
      get => _IsCheckedFunctionToggleButton;
      set
      {
        FunctionWindowClose();
        _IsCheckedFunctionToggleButton = value;
        if (_IsCheckedFunctionToggleButton)
          FunctionWindowOpen();
        OnPropertyChanged("IsCheckedFunctionToggleButton");
      }
    }
    private bool _IsCheckedFunctionToggleButton = false;

    private void FunctionWindowOpen()
    {
      FunctionWindow = new UserWindow(new FunctionControl(), "UMP - Function") { WindowStartupLocation = WindowStartupLocation.CenterOwner };
      FunctionWindow.Show();
      FunctionWindow.Closed += (_, e) =>
      {
        FunctionWindowClose();
        _IsCheckedFunctionToggleButton = false;
        OnPropertyChanged("IsCheckedFunctionToggleButton");
      };
    }

    private void FunctionWindowClose()
    {
      if (FunctionWindow != null)
        FunctionWindow.Close();
      FunctionWindow = null;
    }

    private UserWindow FunctionWindow
    {
      get => _FunctionWindow.IsAlive ? (UserWindow)_FunctionWindow.Target : null;
      set => _FunctionWindow = new WeakReference(value);
    }
    private WeakReference _FunctionWindow = new WeakReference(null);
    #endregion

    #region PlayListControl
    public bool IsCheckedPlayListToggleButton
    {
      get => _IsCheckedPlayListToggleButton;
      set
      {
        PlayListWindowClose();
        _IsCheckedPlayListToggleButton = value;
        if (_IsCheckedPlayListToggleButton)
          PlayListWindowOpen();
        OnPropertyChanged("IsCheckedPlayListToggleButton");
      }
    }
    private bool _IsCheckedPlayListToggleButton = false;

    private void PlayListWindowOpen()
    {
      PlayListWindow = new UserWindow(new PlayListControl(), "UMP - PlayList") { WindowStartupLocation = WindowStartupLocation.CenterOwner };
      PlayListWindow.Show();
      PlayListWindow.Closed += (_, e) =>
      {
        PlayListWindowClose();
        _IsCheckedPlayListToggleButton = false;
        OnPropertyChanged("IsCheckedPlayListToggleButton");
      };
    }
    private void PlayListWindowClose()
    {
      if (PlayListWindow != null)
        PlayListWindow.Close();
      PlayListWindow = null;
    }

    private UserWindow PlayListWindow
    {
      get => _PlayListWindow.IsAlive ? (UserWindow)_PlayListWindow.Target : null;
      set => _PlayListWindow = new WeakReference(value);
    }
    private WeakReference _PlayListWindow = new WeakReference(null);
    #endregion

    #region 플레이 시간 UI

    // 재생길이 (단일 파일)
    public TimeSpan DurationTime =>
      MainMediaPlayer.MediaLoadedCheck
      ? MainMediaPlayer.MediaInformation.Duration
      : TimeSpan.Zero;
    // 전채 재생길이 (복수 파일)
    public TimeSpan TotalDuration =>
      MainMediaPlayer.PlayList != null
      ? MainMediaPlayer.PlayList.TotalDuration
      : TimeSpan.Zero;
    public string DurationTimestring
    {
      get
      {
        if (MainMediaPlayer.Option.DurationViewStatus)
          return Converter.TimeSpanStringConverter(DurationTime); // 전채 시간
        else
          return "-" + Converter.TimeSpanStringConverter(DurationTime - CurrentPostion); // 남은 시간
      }
    }

    // 현재 재생위치
    public TimeSpan CurrentPostion =>
      MainMediaPlayer.MediaLoadedCheck
      ? MainMediaPlayer.AudioCurrentTime
      : TimeSpan.Zero;
    public string CurrentPostionstring => Converter.TimeSpanStringConverter(CurrentPostion);
    #endregion

    #region ToggleButton
    public Brush ToggleButtonBackground => new SolidColorBrush(ThemeHelper.PrimaryColor.Darken(3));
    #endregion

    #region HotKey
    /// <summary>
    /// 컨트롤 키보드 이벤트 처리 (내부 이벤트)
    /// </summary>
    private void GlobalKeyDownEvent_KeyDownEvent(KeyEventArgs e)
    {
      if (GlobalProperty.State.IsControllable)
      {
        switch (GlobalProperty.Options.HotKey.Getter(e.Key))
        {
          case GlobalProperty.Options.HotKey.ControlTarget.PlayPause:
            if (PlayPauseCommand.CanExecute(null))
              PlayPauseCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Stop:
            if (StopCommand.CanExecute(null))
              StopCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Previous:
            if (PreviousCommand.CanExecute(null))
              PreviousCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Next:
            if (NextCommand.CanExecute(null))
              NextCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Mute:
            if (Volume > 0)
              Volume = 0;
            else
              Volume = BeforeVolume;
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Repeat:
            if (RepeatCommand.CanExecute(null))
              RepeatCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Shuffle:
            if (ShuffleCommand.CanExecute(null))
              ShuffleCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.MediaPositionForward:
            MediaPositionChanger(TimeSpan.FromSeconds(5));
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.MediaPositionBack:
            MediaPositionChanger(-TimeSpan.FromSeconds(5));
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.VolumeUp:
            Volume += 5;
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.VolumeDown:
            Volume -= 5;
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.PlayListWindow:
            IsCheckedPlayListToggleButton = !IsCheckedPlayListToggleButton;
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.FunctionWindow:
            IsCheckedFunctionToggleButton = !IsCheckedFunctionToggleButton;
            break;
          default:
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
      ApplyUI(false);
    }
    #endregion

    #region 동기화 메소드
    private void Option_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "RepeatPlayOption")
        OnPropertyChanged("RepeatPlayOptionIcon");
      if (e.PropertyName == "Shuffle")
        OnPropertyChanged("ShuffleIcon");
    }
    private void MainMediaPlayer_PlayStateChanged(PlaybackState state) => ApplyUI();
    private void MainMediaPlayer_TickEvent(object sender, EventArgs e) => ApplyUI(false);
    private void MainMediaPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "MainPlayerInitialized" || e.PropertyName == "MediaInfo")
        ApplyUI();
    }
    private void PlayList_Field_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "TotalDuration")
        OnPropertyChanged("TotalDuration");      // 플레이리스트 재생길이 총합 적용
    }
    private void ThemeHelper_ThemeChangedEvent(ThemeHelper.ThemeProperty e)
    {
      OnPropertyChanged("ToggleButtonBackground"); // 테마 색 변경 적용
    }

    /// <summary>
    /// 현재 플레이어 상태를 UI에 적용합니다 (UI 갱신)
    /// </summary>
    /// <param name="fullapply">모든 정보를 적용할지의 여부</param>
    public void ApplyUI(bool fullapply = true)
    {
      if (!MainMediaPlayer.Option.DurationViewStatus || fullapply)
        OnPropertyChanged("DurationTimestring"); // 라벨 총 제생시간 적용
      if (fullapply)
      {
        OnPropertyChanged("DurationTime");       // 슬라이드 바 최대 길이 적용
        OnPropertyChanged("PlayPauseStateIcon"); // Play/Pause 버튼 아이콘 새로고침 적용
      }
      OnPropertyChanged("CurrentPostion");
      OnPropertyChanged("CurrentPostiondouble"); // 슬라이드 바 현재 재생위치 적용
      OnPropertyChanged("CurrentPostionstring"); // 라벨 현재 재생위치 적용
    }
    #endregion
  }
}
