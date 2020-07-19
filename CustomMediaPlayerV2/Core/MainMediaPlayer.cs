﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

using CMP2.Core.Model;

using NAudio.Wave;

using Newtonsoft.Json;

namespace CMP2.Core
{
  public static class MainMediaPlayer
  {
    static MainMediaPlayer()
    {
      Option = new PlayerOption();
      PlayList = new PlayList();
      PlayListPlayMediaIndex = -1;
      Volume = 0.8f;
      OptionDefault();
      // 오토 플래이 옵션
      PropertyChangedEvent += (e) =>
      {
        if (e == "MainPlayerInitialized")
          if (MediaLoadedCheck && Option.AutoPlayOption && !NotAutoPlay)
            Play();
      };
      Log.Debug("초기화 성공");
    }
    private static Log Log { get; } = new Log(typeof(MainMediaPlayer));

    /// <summary>
    /// 메인 플레이어
    /// </summary>
    private static IWavePlayer WavePlayer { get; set; }
    /// <summary>
    /// 플레이어 옵션
    /// </summary>
    public static PlayerOption Option;

    public static void OptionDefault()
    {
      Option.Shuffle = false;
      Option.AutoPlayOption = true;
      Option.RepeatPlayOption = 0;
      Option.DurationViewStatus = true;
    }
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
    private static PlayList _PlayList;

    /// <summary>
    /// 현재 플레이리스트에서 재생중인 미디어의 Index
    /// </summary>
    public static int PlayListPlayMediaIndex { get; private set; }

    private static DispatcherTimer Tick { get; } = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1) };
    public static event EventHandler TickEvent
    {
      add => Tick.Tick += value;
      remove => Tick.Tick -= value;
    }

    private static bool StopButtonActive { get; set; } = false;
    private static bool NotAutoPlay { get; set; } = false;

    public delegate void PlayStateChangedEventHandler(PlaybackState state);
    public static event PlayStateChangedEventHandler PlayStateChangedEvent;
    public static event CMP_PropertyChangedEventHandler PropertyChangedEvent;
    private static void OnPropertyChanged(string name) => PropertyChangedEvent?.Invoke(name);

    public static bool MediaLoadedCheck => AudioFile != null && MediaInfo != null;

    /// <summary>
    /// 미디어 파일 정보
    /// </summary>
    public static IMediaInfo MediaInfo
    {
      get => _MediaInfo;
      set
      {
        _MediaInfo = value;
        OnPropertyChanged("MediaInfo");
      }
    }
    private static IMediaInfo _MediaInfo = null;

    /// <summary>
    /// 오디오 파일 (약한참조)
    /// </summary>
    public static WaveStream AudioFile
    {
      get
      {
        if (_AudioFile != null && _AudioFile.TryGetTarget(out WaveStream ws))
          return ws;
        else
          return null;
      }
      private set
      {
        if (_AudioFile != null)
          _AudioFile.SetTarget(value);
        else
          _AudioFile = new WeakReference<WaveStream>(value);
        OnPropertyChanged("AudioFile");
      }
    }
    private static WeakReference<WaveStream> _AudioFile = null;

    /// <summary>
    /// 재생이 끝났을 경우 이벤트 처리
    /// </summary>
    private static void MediaPlayer_PlaybackStopped(object sender, StoppedEventArgs e)
    {
      if (e.Exception != null)
        Log.Error("메인 플레이어 [PlaybackStopped]이벤트 처리오류", e.Exception);
      if (WavePlayer.PlaybackState == PlaybackState.Stopped)
      {
        AudioFile.CurrentTime = TimeSpan.Zero;
        if (StopButtonActive)
          StopButtonActive = false;
        else
        {
          // 한 곡 반복
          if (Option.RepeatPlayOption == 1)
            Play();
          // 전체 반복
          else if (Option.RepeatPlayOption == 2)
          {
            if (PlayListPlayMediaIndex == -1)
              Play();
            else
            {
              Next();
              Play();
            }
          }
        }
      }
      PlayStateChangedEvent?.Invoke(WavePlayer.PlaybackState);
    }

    /// <summary>
    /// 볼륨
    /// </summary>
    public static float Volume
    {
      get => WavePlayer != null ? WavePlayer.Volume : _Volume;
      set
      {
        if (WavePlayer != null)
          WavePlayer.Volume = value;
        _Volume = value;
        OnPropertyChanged("Volume");
      }
    }
    public static float _Volume;

    /// <summary>
    /// 현재 재생 상태
    /// </summary>
    public static PlaybackState PlaybackState => WavePlayer != null ? WavePlayer.PlaybackState : PlaybackState.Stopped;

    /// <summary>
    /// 미디어로 초기화하고 재생을 준비합니다
    /// </summary>
    /// <param name="mediaInfo">재생할 미디어</param>
    /// <param name="autoplay">자동 재생 여부</param>
    public static async void Init(MediaInfo mediaInfo)
    {
      PlayListPlayMediaIndex = PlayList.IndexOf(mediaInfo);

      await ReadToPlay(mediaInfo);
      StopButtonActive = false;

      WavePlayer?.Dispose();
      WavePlayer = new WaveOut();
      WavePlayer.PlaybackStopped += MediaPlayer_PlaybackStopped;
      WavePlayer.Volume = _Volume;
      WavePlayer.Init(AudioFile);

      Log.Debug($"[{(string.IsNullOrWhiteSpace(mediaInfo.GetYouTubeVideoID()) ? System.IO.Path.GetFileNameWithoutExtension(MediaInfo.MediaLocation) : $"\"{mediaInfo.GetYouTubeVideoID()}\" {MediaInfo.Title}")}] 메인 미디어 플레이어에 로드 성공.");

      PropertyChangedEvent?.Invoke("MainPlayerInitialized");
    }

    /// <summary>
    /// 재생준비
    /// </summary>
    private static async Task ReadToPlay(MediaInfo mediaInfo)
    {
      if (mediaInfo == null || string.IsNullOrWhiteSpace(mediaInfo.MediaLocation))
      {
        Log.Error("미디어 위치정보가 누락 되었습니다. (잘못된 매개변수)");
        return;
      }

      string path = await mediaInfo.GetStreamPath();

      if (string.IsNullOrWhiteSpace(path))
      {
        Log.Error("미디어 위치정보가 누락 되었습니다. (path is Null)");
        return;
      }

      Stop();
      MediaInfo = null;
      if (AudioFile != null)
      {
        AudioFile.Dispose();
        AudioFile = null;
      }

      // 모든 정보로드
      await mediaInfo.TryInfoAllLoadAsync();

      AudioFile = new MediaFoundationReader(path);
      if (mediaInfo.Duration <= TimeSpan.Zero)
        mediaInfo.Duration = AudioFile.TotalTime;
      MediaInfo = mediaInfo;
    }

    /// <summary>
    /// 재생
    /// </summary>
    public static void Play()
    {
      if (MediaLoadedCheck && PlaybackState != PlaybackState.Playing)
      {
        Tick.Start();
        WavePlayer.Play();
        PlayStateChangedEvent?.Invoke(WavePlayer.PlaybackState);
      }
    }

    /// <summary>
    /// 정지
    /// </summary>
    public static void Stop()
    {
      if (MediaLoadedCheck && PlaybackState != PlaybackState.Stopped)
      {
        StopButtonActive = true;
        WavePlayer.Stop();
        Tick.Stop();
        PlayStateChangedEvent?.Invoke(WavePlayer.PlaybackState);
      }
    }

    /// <summary>
    /// 일시정지
    /// </summary>
    public static void Pause()
    {
      if (MediaLoadedCheck && PlaybackState != PlaybackState.Paused)
      {
        WavePlayer.Pause();
        Tick.Stop();
        PlayStateChangedEvent?.Invoke(WavePlayer.PlaybackState);
      }
    }

    /// <summary>
    /// 다음 미디어
    /// </summary>
    public static void Next()
    {
      if (PlayListPlayMediaIndex < 0)
        return;

      if (Option.Shuffle && PlayListPlayMediaIndex >= 0)
      {
        PlayListPlayMediaIndex = new Random().Next(0, PlayList.Count);
      }

      int index = PlayListPlayMediaIndex + 1;
      var beforeState = PlaybackState;
      bool InitComplete = false;
      NotAutoPlay = true;
      do
      {
        if (index == PlayList.Count)
          index = 0;

        if ((int)PlayList[index].LoadedCheck >= 2)
        {
          Init(PlayList[index]);
          InitComplete = true;
        }
        else if (PlayListPlayMediaIndex == index)
          return;

        index++;
      }
      while (!InitComplete);
      if (beforeState == PlaybackState.Playing)
        Play();
      NotAutoPlay = false;
    }

    /// <summary>
    /// 이전 미디어
    /// </summary>
    public static void Previous()
    {
      if (PlayListPlayMediaIndex < 0)
        return;

      if (Option.Shuffle && PlayListPlayMediaIndex >= 0)
      {
        PlayListPlayMediaIndex = new Random().Next(0, PlayList.Count);
      }

      int index = PlayListPlayMediaIndex - 1;
      var beforeState = PlaybackState;
      bool InitComplete = false;
      NotAutoPlay = true;
      do
      {
        if (index == 0)
          index = PlayList.Count;

        if ((int)PlayList[index].LoadedCheck >= 2)
        {
          Init(PlayList[index]);
          InitComplete = true;
        }
        else if (PlayListPlayMediaIndex == index)
          return;

        index--;
      }
      while (!InitComplete);
      if (beforeState == PlaybackState.Playing)
        Play();
      NotAutoPlay = false;
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
  [JsonObject]
  public struct PlayerOption
  {
    public event PropertyChangedEventHandler PropertyChangedEvent;
    private void OnPropertyChanged(string propertyName) => PropertyChangedEvent?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool _Shuffle;
    private int _RepeatPlayOption;
    private bool _AutoPlayOption;
    private bool _DurationViewStatus;
    private bool _LastSongSaveOption;

    /// <summary>
    /// 셔플 옵션
    /// </summary>
    [JsonProperty]
    public bool Shuffle
    {
      get => _Shuffle;
      set
      {
        _Shuffle = value;
        OnPropertyChanged("Shuffle");
      }
    }

    /// <summary>
    /// 반복 옵션 (0 = OFF, 1 = Once, 2 = All)
    /// </summary>
    [JsonProperty]
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
    [JsonProperty]
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
    [JsonProperty]
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
    [JsonProperty]
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
