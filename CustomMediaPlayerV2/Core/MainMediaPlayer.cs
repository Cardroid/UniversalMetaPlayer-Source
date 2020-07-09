using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Threading;

using CMP2.Core.Playlist;

using NAudio.Wave;

namespace CMP2.Core
{
  public static class MainMediaPlayer
  {
    /// <summary>
    /// 초기화 설정
    /// </summary>
    static MainMediaPlayer()
    {
      Volume = 0.8f;
      Option.AutoPlayOption = true;
      Option.RepeatPlayOption = 0;
      Option.DurationViewStatus = true;
      _MediaPlayer.PlaybackStopped += MediaPlayer_PlaybackStopped;
    }

    // 메인 플레이어
    private static readonly IWavePlayer _MediaPlayer = new WaveOut();
    public static PlayerOption Option = new PlayerOption();
    public static PlayList PlayList { get; set; } = new PlayList();

    private static DispatcherTimer Tick { get; } = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
    public static event EventHandler TickEvent
    {
      add => Tick.Tick += value;
      remove => Tick.Tick -= value;
    }

    private static bool StopButtonActive = false;

    public delegate void PlayStateChangedEventHandler(PlaybackState state);
    public static event PlayStateChangedEventHandler PlayStateChangedEvent;
    public delegate void AudioFileEventHandler(IMediaInfo mediaInfo);
    public static event AudioFileEventHandler AudioFileOpenEvent;

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
        AudioFileOpenEvent?.Invoke(MediaInfo);
      }
    }
    private static AudioFileReader _AudioFile = null;

    /// <summary>
    /// 재생이 끝났을 경우 이벤트 처리
    /// </summary>
    private static void MediaPlayer_PlaybackStopped(object sender, StoppedEventArgs e)
    {
      if (_MediaPlayer.PlaybackState == PlaybackState.Stopped)
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
      PlayStateChangedEvent?.Invoke(_MediaPlayer.PlaybackState);
    }
    /// <summary>
    /// 볼륨
    /// </summary>
    public static float Volume
    {
      get => _MediaPlayer.Volume;
      set => _MediaPlayer.Volume = value;
    }

    /// <summary>
    /// 현재 재생 상태
    /// </summary>
    public static PlaybackState PlaybackState => _MediaPlayer.PlaybackState;

    /// <summary>
    /// 미디어로 초기화하고 재생을 준비합니다
    /// </summary>
    /// <param name="mediaInfo">재생할 미디어</param>
    /// <param name="autoplay">자동 재생 여부</param>
    public static void Init(IMediaInfo mediaInfo, bool autoplay = false)
    {
      if (mediaInfo == null && string.IsNullOrWhiteSpace(mediaInfo.FileFullName))
        return;
      if (PlaybackState != PlaybackState.Stopped)
        Stop();
      MediaInfo = mediaInfo;
      AudioFile?.Dispose();
      AudioFile = new AudioFileReader(mediaInfo.FileFullName);
      _MediaPlayer.Init(AudioFile);
      if (Option.AutoPlayOption || autoplay)
        Play();
    }

    /// <summary>
    /// 재생
    /// </summary>
    public static void Play()
    {
      Tick.Start();
      _MediaPlayer.Play();
      PlayStateChangedEvent?.Invoke(_MediaPlayer.PlaybackState);
    }

    /// <summary>
    /// 정지
    /// </summary>
    public static void Stop()
    {
      StopButtonActive = true;
      _MediaPlayer.Stop();
      Tick.Stop();
      PlayStateChangedEvent?.Invoke(_MediaPlayer.PlaybackState);
    }

    /// <summary>
    /// 일시정지
    /// </summary>
    public static void Pause()
    {
      _MediaPlayer.Pause();
      Tick.Stop();
      PlayStateChangedEvent?.Invoke(_MediaPlayer.PlaybackState);
    }

    /// <summary>
    /// 모든 리소스 해제
    /// </summary>
    public static void Dispose()
    {
      AudioFile?.Dispose();
      _MediaPlayer?.Dispose();
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
