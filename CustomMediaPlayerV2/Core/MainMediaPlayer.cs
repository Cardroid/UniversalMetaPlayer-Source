using System;
using System.Windows.Threading;
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
<<<<<<< HEAD
      Option.AutoPlayOption = true;
      Option.RepeatPlayOption = 0;
      Option.DurationViewStatus = true;
<<<<<<< HEAD
      _MediaPlayer.PlaybackStopped += MediaPlayer_PlaybackStopped;
=======
>>>>>>> parent of 4baef91... Add MainPlayer Option Changed Event & Fixed App namespace
=======
      PlaybackStopped += MainMediaPlayer_PlaybackStopped;
>>>>>>> parent of 708ea27... Add Uno Project
    }

    // 메인 플레이어
    private static IWavePlayer _MediaPlayer = new WaveOut();
    public static PlayerOption Option = new PlayerOption
    { AutoPlayOption = true, RepeatPlayOption = 0, DurationViewStatus = true };

    private static DispatcherTimer Tick { get; } = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1) };
    public static event EventHandler TickEvent
    {
      add => Tick.Tick += value;
      remove => Tick.Tick -= value;
    }

    public delegate void AudioFileEventHandler(IMediaInfo mediaInfo);
    public static event AudioFileEventHandler AudioFileOpenEvent;

    /// <summary>
    /// 미디어 파일 정보
    /// </summary>
    public static IMediaInfo MediaInfo { get; private set; }
    /// <summary>
    /// 오디오 파일
    /// </summary>
    public static AudioFileReader AudioFile { get; private set; }

    /// <summary>
<<<<<<< HEAD
<<<<<<< HEAD
    /// 재생이 끝났을 경우 이벤트 처리
    /// </summary>
    private static void MediaPlayer_PlaybackStopped(object sender, StoppedEventArgs e) =>
      PlayStateChangedEvent?.Invoke(_MediaPlayer.PlaybackState);
=======
    /// 미디어 재생 상태 변화시 호출
    /// </summary>
    public static event EventHandler<StoppedEventArgs> PlaybackStopped
    {
      add => _MediaPlayer.PlaybackStopped += value;
      remove => _MediaPlayer.PlaybackStopped -= value;
    }
>>>>>>> parent of 4baef91... Add MainPlayer Option Changed Event & Fixed App namespace
=======
    /// 미디어 재생 상태 변화시 호출
    /// </summary>
    public static event EventHandler<StoppedEventArgs> PlaybackStopped
    {
      add => _MediaPlayer.PlaybackStopped += value;
      remove => _MediaPlayer.PlaybackStopped -= value;
    }
    /// <summary>
    /// 재생이 끝났을 경우 이벤트 처리
    /// </summary>
    private static void MainMediaPlayer_PlaybackStopped(object sender, StoppedEventArgs e)
    {
      AudioFile.CurrentTime = TimeSpan.Zero;
    }
>>>>>>> parent of 708ea27... Add Uno Project
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
      AudioFileOpenEvent?.Invoke(MediaInfo);
    }
    /// <summary>
    /// 재생
    /// </summary>
    public static void Play()
    {
      Tick.Start();
      _MediaPlayer.Play();
    }
    /// <summary>
    /// 정지
    /// </summary>
    public static void Stop()
    {
      PlaybackStopped -= MainMediaPlayer_PlaybackStopped;
      _MediaPlayer.Stop();
<<<<<<< HEAD
<<<<<<< HEAD
      AudioFile.CurrentTime = TimeSpan.Zero;
=======
>>>>>>> parent of 4baef91... Add MainPlayer Option Changed Event & Fixed App namespace
=======
      PlaybackStopped += MainMediaPlayer_PlaybackStopped;
>>>>>>> parent of 708ea27... Add Uno Project
      Tick.Stop();
    }
    /// <summary>
    /// 일시정지
    /// </summary>
    public static void Pause()
    {
      _MediaPlayer.Pause();
      Tick.Stop();
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
    /// <summary>
    /// 반복 옵션
    /// </summary>
    public int RepeatPlayOption { get; set; }
    /// <summary>
    /// 자동 재생 옵션
    /// </summary>
    public bool AutoPlayOption { get; set; }
    /// <summary>
    /// 남은시간 <=> 전체시간 전환
    /// </summary>
    public bool DurationViewStatus { get; set; }
    /// <summary>
    /// 마지막 곡 저장
    /// </summary>
    public bool LastSongSaveOption { get; set; }
  }
}
