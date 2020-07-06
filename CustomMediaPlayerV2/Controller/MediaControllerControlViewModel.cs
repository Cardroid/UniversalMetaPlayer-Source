using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;

using CMP2.Core;
using CMP2.Utility;

using MahApps.Metro.IconPacks;

using NAudio.Wave;

namespace CMP2.Controller
{
  public class MediaControllerControlViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    public FontFamily FontFamily => IGlobalProperty.MainFontFamily;

    public MediaControllerControlViewModel()
    {
      MainMediaPlayer.PlaybackStopped += MainMediaPlayer_PlaybackStopped;
      MainMediaPlayer.PlaybackStopped += MainMediaPlayer_Stopped;
      MainMediaPlayer.AudioFileOpenEvent += MainMediaPlayer_AudioFileOpenEvent;
      MainMediaPlayer.TickEvent += MainMediaPlayer_TickEvent;
    }

    #region 볼륨
    public float BeforeVolume { get; private set; } = 100;
    public string VolumeString { get => ((int)(Volume)).ToString() + "%"; }
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
    private PackIconBase _VolumeMuteButtonIcon = null;
    public PackIconBase VolumeMuteButtonIcon
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
    public PackIconBase VolumeMuteButtonIconChanger()
    {
      var Icon = new PackIconControl() { Width = 15, Height = 15 };
      if (Volume > 75)
        Icon.Kind = PackIconMaterialKind.VolumeHigh;
      else if (Volume > 35)
        Icon.Kind = PackIconMaterialKind.VolumeMedium;
      else if (Volume > 0)
        Icon.Kind = PackIconMaterialKind.VolumeLow;
      else if (Volume == 0)
        Icon.Kind = PackIconMaterialKind.VolumeOff;
      return Icon;
    }
    #endregion

    #region 재생 / 일시정지
    public PackIconBase PlayPauseStateIcon { get; private set; } = new PackIconControl { Width = 30, Height = 30, Kind = PackIconMaterialKind.Play };
    private void MainMediaPlayer_PlaybackStopped(object sender, StoppedEventArgs e)
    {
      if (MainMediaPlayer.PlaybackState == PlaybackState.Playing)
        PlayPauseStateIcon = new PackIconControl { Width = 30, Height = 30, Kind = PackIconMaterialKind.Pause };
      else
        PlayPauseStateIcon = new PackIconControl { Width = 30, Height = 30, Kind = PackIconMaterialKind.Play };
      OnPropertyChanged("PlayPauseStateIcon");
    }
    #endregion

    #region 반복재생
    public PackIconBase RepeatPlayOptionIcon { get; private set; } = new PackIconControl { Width = 20, Height = 20, Kind = PackIconMaterialKind.RepeatOff };
    public int RepeatPlayOption
    {
      get => MainMediaPlayer.Option.RepeatPlayOption;
      set
      {
        if (value > 3)
          MainMediaPlayer.Option.RepeatPlayOption = 0;
        else if (value < 0)
          MainMediaPlayer.Option.RepeatPlayOption = 2;
        else
          MainMediaPlayer.Option.RepeatPlayOption = value;
        // 반복 아이콘 설정
        var RepeatIcon = new PackIconControl() { Width = 20, Height = 20 };
        if (MainMediaPlayer.Option.RepeatPlayOption == 0) // 반복 안함
        { RepeatIcon.Kind = PackIconMaterialKind.RepeatOff; }
        else if (MainMediaPlayer.Option.RepeatPlayOption == 1) // 한 곡 반복
        { RepeatIcon.Kind = PackIconMaterialKind.RepeatOnce; }
        else if (MainMediaPlayer.Option.RepeatPlayOption == 2) // 전채 반복
        { RepeatIcon.Kind = PackIconMaterialKind.Repeat; }
        RepeatPlayOptionIcon = RepeatIcon;
        OnPropertyChanged("RepeatPlayOptionIcon");
      }
    }
    #endregion

    #region 플레이 시간 UI

    // 재생길이 (단일 파일)
    public TimeSpan DurationTime => MainMediaPlayer.MediaInfo != null && MainMediaPlayer.AudioFile != null ? MainMediaPlayer.MediaInfo.Duration : TimeSpan.Zero;
    #region 아직 구현/사용 안함
    // 전채 재생길이 (복수 파일)
    private double _TotalDuration = 0;
    public double TotalDuration
    {
      set
      {
        _TotalDuration = value;
        OnPropertyChanged("TotalDuration");
      }
      get => _TotalDuration;
    }
    #endregion
    public string DurationTimestring
    {
      get
      {
        if (DurationTime == TimeSpan.Zero) // 오류 방지용
          return "00:00";
        if (MainMediaPlayer.Option.DurationViewStatus)
          return Converter.TimeSpanStringConverter(DurationTime); // 전채 시간
        else
          return "-" + Converter.TimeSpanStringConverter(DurationTime - CurrentPostion); // 남은 시간
      }
    }

    // 현재 재생위치
    public TimeSpan CurrentPostion => MainMediaPlayer.MediaInfo != null && MainMediaPlayer.AudioFile != null ? MainMediaPlayer.AudioFile.CurrentTime : TimeSpan.Zero;
    public double CurrentPostiondouble
    {
      get => CurrentPostion.TotalMilliseconds;
      set => MainMediaPlayer.AudioFile.CurrentTime = TimeSpan.FromMilliseconds(value);
    }
    public string CurrentPostionstring => Converter.TimeSpanStringConverter(CurrentPostion);
    #endregion

    #region 동기화 메소드
    private void MainMediaPlayer_AudioFileOpenEvent(IMediaInfo mediaInfo)
    {
      ApplyUI();
    }
    private void MainMediaPlayer_Stopped(object sender, StoppedEventArgs e)
    {
      ApplyUI();
    }
    private void MainMediaPlayer_TickEvent(object sender, EventArgs e)
    {
      ApplyUI();
    }

    private void ApplyUI()
    {
      OnPropertyChanged("DurationTime");
      OnPropertyChanged("DurationTimestring");
      OnPropertyChanged("CurrentPostion");
      OnPropertyChanged("CurrentPostiondouble");
      OnPropertyChanged("CurrentPostionstring");
    }
    #endregion
  }
}
