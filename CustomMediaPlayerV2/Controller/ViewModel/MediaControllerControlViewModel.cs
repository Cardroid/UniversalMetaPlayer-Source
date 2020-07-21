using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Media;

using CMP2.Core;
using CMP2.Core.Model;
using CMP2.Utility;

using MaterialDesignThemes.Wpf;

using NAudio.Wave;

namespace CMP2.Controller.ViewModel
{
  public class MediaControllerControlViewModel : ViewModelBase
  {
    public MediaControllerControlViewModel()
    {
      MainMediaPlayer.PlayStateChangedEvent += MainMediaPlayer_PlayStateChangedEvent;
      MainMediaPlayer.PropertyChangedEvent += MainMediaPlayer_PropertyChangedEvent;
      MainMediaPlayer.TickEvent += MainMediaPlayer_TickEvent;
      MainMediaPlayer.Option.PropertyChangedEvent += Option_PropertyChangedEvent;
      MainMediaPlayer.PlayList.PropertyChangedEvent += PlayList_PropertyChangedEvent;

      PlayListWindow = new PlayListWindow { Visibility = Visibility.Collapsed };
      PlayListWindow.Closing += PlayListWindow_Closing;
    }

    #region 볼륨
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
        VolumeMuteButtonIcon = VolumeMuteButtonIconChanger();
        OnPropertyChanged("Volume");
        OnPropertyChanged("VolumeString");
      }
    }
    // 볼륨 뮤트 버튼 아이콘 관련
    private PackIcon _VolumeMuteButtonIcon = null;
    public PackIcon VolumeMuteButtonIcon
    {
      get
      {
        if (_VolumeMuteButtonIcon == null)
          _VolumeMuteButtonIcon = VolumeMuteButtonIconChanger();
        return _VolumeMuteButtonIcon;
      }
      set
      {
        _VolumeMuteButtonIcon = value;
        OnPropertyChanged("VolumeMuteButtonIcon");
      }
    }
    public PackIcon VolumeMuteButtonIconChanger()
    {
      var Icon = new PackIcon() { Width = 25, Height = 25 };
      if (Volume > 70)
        Icon.Kind = PackIconKind.VolumeHigh;
      else if (Volume > 25)
        Icon.Kind = PackIconKind.VolumeMedium;
      else if (Volume > 0)
        Icon.Kind = PackIconKind.VolumeLow;
      else if (Volume == 0)
        Icon.Kind = PackIconKind.VolumeOff;
      return Icon;
    }
    #endregion

    #region 재생 / 일시정지
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

    #region 반복재생
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

    #region 플레이 시간 UI

    // 재생길이 (단일 파일)
    public TimeSpan DurationTime => 
      MainMediaPlayer.MediaLoadedCheck
      ? MainMediaPlayer.MediaInfomation.Duration 
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

    #region PlayList 창 관련
    private static PlayListWindow PlayListWindow { get; set; }

    private void PlayListWindow_Closing(object sender, CancelEventArgs e) => IsPlayListWindowOpen = false;

    /// <summary>
    /// 플레이리스트 창이 열렸는지 여부
    /// </summary>
    public bool IsPlayListWindowOpen 
    {
      get => _IsPlayListWindowOpen;
      set
      {
        PlayListWindowManager(value);
        OnPropertyChanged("IsPlayListWindowOpen");
      }
    }
    private bool _IsPlayListWindowOpen = false;

    public void PlayListWindowManager(bool isopen)
    {
      if (isopen)
      {
        PlayListWindow.Visibility = Visibility.Visible;
        PlayListWindow.Focus();
      }
      else
        PlayListWindow.Visibility = Visibility.Collapsed;
      _IsPlayListWindowOpen = isopen;
    }
    #endregion

    #region 동기화 메소드
    private void Option_PropertyChangedEvent(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "RepeatPlayOption")
        OnPropertyChanged("RepeatPlayOptionIcon");
      if (e.PropertyName == "Shuffle")
        OnPropertyChanged("ShuffleIcon");
    }
    private void MainMediaPlayer_PlayStateChangedEvent(PlaybackState state) => ApplyUI();
    private void MainMediaPlayer_TickEvent(object sender, EventArgs e) => ApplyUI(false);
    private void MainMediaPlayer_PropertyChangedEvent(string propertyname)
    {
      if (propertyname == "MediaInfo")
        ApplyUI();
    }
    private void PlayList_PropertyChangedEvent(string propertyname)
    {
      if (propertyname == "TotalDuration")
        OnPropertyChanged("TotalDuration");      // 플레이리스트 재생길이 총합 적용
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
