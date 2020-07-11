﻿using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Threading;

using CMP2.Core.Model;

using NAudio.Wave;

namespace CMP2.Core
{
  public static class MainMediaPlayer
  {
    static MainMediaPlayer()
    {
      WavePlayer = new WaveOut();
      Option = new PlayerOption();
      PlayList = new PlayList();
      Volume = 0.8f;
      Option.AutoPlayOption = true;
      Option.RepeatPlayOption = 0;
      Option.DurationViewStatus = true;
      WavePlayer.PlaybackStopped += MediaPlayer_PlaybackStopped;
      Log.Debug("Initialized");
    }
    private static Log Log { get; } = new Log(typeof(MainMediaPlayer));

    /// <summary>
    /// 메인 플레이어
    /// </summary>
    private static IWavePlayer WavePlayer { get; }
    /// <summary>
    /// 플레이어 옵션
    /// </summary>
    public static PlayerOption Option;
    /// <summary>
    /// 플레이리스트
    /// </summary>
    public static PlayList PlayList
    {
      get => _PlayList;
      set
      {
        _PlayList = value;
        OnPropertyChanged("PlayList");
      }
    }
    private static PlayList _PlayList = null;

    private static DispatcherTimer Tick { get; } = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
    public static event EventHandler TickEvent
    {
      add => Tick.Tick += value;
      remove => Tick.Tick -= value;
    }

    private static bool StopButtonActive = false;

    public delegate void PlayStateChangedEventHandler(PlaybackState state);
    public static event PlayStateChangedEventHandler PlayStateChangedEvent;
    public delegate void PropertyChangedEventHandler(string propertyname);
    public static event PropertyChangedEventHandler PropertyChangedEvent;
    private static void OnPropertyChanged(string name) => PropertyChangedEvent?.Invoke(name);

    public static bool MediaLoadedCheck => AudioFile != null && MediaInfo != null;

    /// <summary>
    /// 미디어 파일 정보
    /// </summary>
    public static IMediaInfo MediaInfo { get; private set; }

    /// <summary>
    /// 오디오 파일
    /// </summary>
    public static AudioFileReader AudioFile
    {
      get => _AudioFile;
      private set
      {
        _AudioFile = value;
        OnPropertyChanged("AudioFile");
      }
    }
    private static AudioFileReader _AudioFile = null;

    /// <summary>
    /// 재생이 끝났을 경우 이벤트 처리
    /// </summary>
    private static void MediaPlayer_PlaybackStopped(object sender, StoppedEventArgs e)
    {
      if(e.Exception != null)
        Log.Error("PlaybackStopped", e.Exception);
      if (WavePlayer.PlaybackState == PlaybackState.Stopped)
      {
        AudioFile.CurrentTime = TimeSpan.Zero;
        if (StopButtonActive)
          StopButtonActive = false;
        else
        {
          if (Option.RepeatPlayOption == 1)
            Play();
        }
      }
      PlayStateChangedEvent?.Invoke(WavePlayer.PlaybackState);
    }
    /// <summary>
    /// 볼륨
    /// </summary>
    public static float Volume
    {
      get => WavePlayer.Volume;
      set
      {
        WavePlayer.Volume = value;
        OnPropertyChanged("Volume");
      }
    }

    /// <summary>
    /// 현재 재생 상태
    /// </summary>
    public static PlaybackState PlaybackState => WavePlayer.PlaybackState;

    /// <summary>
    /// 미디어로 초기화하고 재생을 준비합니다
    /// </summary>
    /// <param name="mediaInfo">재생할 미디어</param>
    /// <param name="autoplay">자동 재생 여부</param>
    public static void Init(MediaInfo mediaInfo, bool autoplay = false)
    {
      if (mediaInfo == null || string.IsNullOrWhiteSpace(mediaInfo.FileFullName))
      {
        Log.Error("Missing media absolute path.");
        return;
      }
      if (PlaybackState != PlaybackState.Stopped)
        Stop();
      AudioFile?.Dispose();
      if (mediaInfo.LoadedCheck != LoadState.AllLoaded)
      {
        mediaInfo.TryInfomationLoad(true);
        switch (mediaInfo.LoadedCheck)
        {
          case LoadState.Fail:
            Log.Error("Failed to load information.");
            break;
          case LoadState.PartialLoaded:
            Log.Warn("Some of the information on the media is missing.");
            break;
          case LoadState.AllLoaded:
            Log.Info("Media Load Successfully.");
            break;
        }
      }
      MediaInfo = mediaInfo;
      AudioFile = new AudioFileReader(mediaInfo.FileFullName);
      WavePlayer.Init(AudioFile);
      if (Option.AutoPlayOption || autoplay)
        Play();
    }

    /// <summary>
    /// 재생
    /// </summary>
    public static void Play()
    {
      Tick.Start();
      WavePlayer.Play();
      PlayStateChangedEvent?.Invoke(WavePlayer.PlaybackState);
    }

    /// <summary>
    /// 정지
    /// </summary>
    public static void Stop()
    {
      StopButtonActive = true;
      WavePlayer.Stop();
      Tick.Stop();
      PlayStateChangedEvent?.Invoke(WavePlayer.PlaybackState);
    }

    /// <summary>
    /// 일시정지
    /// </summary>
    public static void Pause()
    {
      WavePlayer.Pause();
      Tick.Stop();
      PlayStateChangedEvent?.Invoke(WavePlayer.PlaybackState);
    }

    /// <summary>
    /// 모든 리소스 해제
    /// </summary>
    public static void Dispose()
    {
      AudioFile?.Dispose();
      WavePlayer?.Dispose();
    }
  }

  /// <summary>
  /// 플레이어 옵션
  /// </summary>
  public struct PlayerOption
  {
    public event PropertyChangedEventHandler PropertyChangedEvent;
    private void OnPropertyChanged(string propertyName) => PropertyChangedEvent?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private int _RepeatPlayOption;
    private bool _AutoPlayOption;
    private bool _DurationViewStatus;
    private bool _LastSongSaveOption;

    /// <summary>
    /// 반복 옵션 (0 = OFF, 1 = Once, 2 = All)
    /// </summary>
    public int RepeatPlayOption
    {
      get => _RepeatPlayOption;
      set
      {
        if (value > 2)
          _RepeatPlayOption = 0;
        else if (value < 0)
          _RepeatPlayOption = 2;
        else
          _RepeatPlayOption = value;
        OnPropertyChanged("RepeatPlayOption");
      }
    }
    /// <summary>
    /// 자동 재생 옵션
    /// </summary>
    public bool AutoPlayOption
    {
      get => _AutoPlayOption;
      set
      {
        _AutoPlayOption = value;
        OnPropertyChanged("AutoPlayOption");
      }
    }

    /// <summary>
    /// 남은시간 - 전체시간 전환 (ture = 전체시간, false = 남은시간)
    /// </summary>
    public bool DurationViewStatus
    {
      get => _DurationViewStatus;
      set
      {
        _DurationViewStatus = value;
        OnPropertyChanged("DurationViewStatus");
      }
    }
    /// <summary>
    /// 마지막 곡 저장
    /// </summary>
    public bool LastSongSaveOption
    {
      get => _LastSongSaveOption;
      set
      {
        _LastSongSaveOption = value;
        OnPropertyChanged("LastSongSaveOption");
      }
    }
  }
}
