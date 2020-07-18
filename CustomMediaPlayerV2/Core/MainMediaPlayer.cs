using System;
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
      Volume = 0.8f;
      Option.AutoPlayOption = true;
      Option.RepeatPlayOption = 0;
      Option.DurationViewStatus = true;
      // 오토 플래이 옵션
      PropertyChangedEvent += (e) =>
      {
        if (e == "MainPlayerInitialized")
          if (MediaLoadedCheck && Option.AutoPlayOption)
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

    private static DispatcherTimer Tick { get; } = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
    public static event EventHandler TickEvent
    {
      add => Tick.Tick += value;
      remove => Tick.Tick -= value;
    }

    private static bool StopButtonActive = false;

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
      await ReadToPlay(mediaInfo);

      WavePlayer?.Dispose();
      WavePlayer = new WaveOut();
      WavePlayer.PlaybackStopped += MediaPlayer_PlaybackStopped;
      WavePlayer.Volume = _Volume;
      WavePlayer.Init(AudioFile);

      Log.Debug($"[{(string.IsNullOrWhiteSpace(mediaInfo.GetYouTubeID()) ? System.IO.Path.GetFileNameWithoutExtension(MediaInfo.MediaLocation) : $"\"{mediaInfo.GetYouTubeID()}\" {MediaInfo.Title}")}] 메인 미디어 플레이어에 로드 성공.");

      PropertyChangedEvent?.Invoke("MainPlayerInitialized");
    }

    private static async Task ReadToPlay(MediaInfo mediaInfo)
    {
      if (mediaInfo == null || string.IsNullOrWhiteSpace(mediaInfo.MediaLocation))
      {
        Log.Error("미디어 위치정보가 누락 되었습니다. (잘못된 매개변수)");
        return;
      }

      string path = await mediaInfo.GetPathAsync();

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
      if (MediaLoadedCheck)
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
      if (MediaLoadedCheck)
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
      if (MediaLoadedCheck)
      {
        WavePlayer.Pause();
        Tick.Stop();
        PlayStateChangedEvent?.Invoke(WavePlayer.PlaybackState);
      }
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

    private int _RepeatPlayOption;
    private bool _AutoPlayOption;
    private bool _DurationViewStatus;
    private bool _LastSongSaveOption;

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
