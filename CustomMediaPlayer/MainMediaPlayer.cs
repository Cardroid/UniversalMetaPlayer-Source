using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CustomMediaPlayer.Core;
using CustomMediaPlayer.Option;

using NAudio.Wave;

namespace CustomMediaPlayer
{
  public static class MainMediaPlayer
  {
    //메인 플레이어
    private static IWavePlayer _MediaPlayer = new WaveOut();
    /// <summary>
    /// 미디어 재생 상태 변화시 호출
    /// </summary>
    public static event EventHandler<StoppedEventArgs> PlaybackStopped
    {
      add => _MediaPlayer.PlaybackStopped += value;
      remove => _MediaPlayer.PlaybackStopped -= value;
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

    #region 현재 열린 파일 관련
    public delegate void AudioFileEventHandler(MediaFullInfo audioFile);
    public static event AudioFileEventHandler AudioFileOpen;
    public delegate void NowPlayMediaCursorChangeHandler(int id);
    public static event NowPlayMediaCursorChangeHandler NowPlayMediaCursorChange;

    public static AudioFileReader NowPlayAudioStream { get; private set; }
    public static MediaFullInfo NowPlayMedia { get; private set; }
    #endregion

    /// <summary>
    /// 미디어로 초기화하고 재생을 준비합니다 (UI 초기화 포함)
    /// </summary>
    /// <param name="mediaInfo">재생할 미디어</param>
    public static void Init(MediaInfo mediaInfo)
    {
      _MediaPlayer.Stop();
      NowPlayAudioStream = null;
      NowPlayMedia = new MediaFullInfo(mediaInfo);
      NowPlayAudioStream = new AudioFileReader(NowPlayMedia.FileFullName);
      // 미디어 길이 표시 라벨 너비 초기화
      ((MainWindow)System.Windows.Application.Current.MainWindow).TotalTimeLabel.Width = (Utility.Utility.MeasureString(Utility.Utility.TimeSpanStringConverter(NowPlayMedia.Duration))).Width;
      _MediaPlayer.Init(NowPlayAudioStream);
      if (MainWindow.Optioncore.MediaOpeningPlayOption)
        _MediaPlayer.Play();
      ((MainWindow)System.Windows.Application.Current.MainWindow).MediaPlayer_PlayStateChange();
      AudioFileOpen?.Invoke(NowPlayMedia);
      NowPlayMediaCursorChange?.Invoke(NowPlayMedia.ID);
    }
    /// <summary>
    /// 재생
    /// </summary>
    public static void Play() => _MediaPlayer.Play();
    /// <summary>
    /// 정지
    /// </summary>
    public static void Stop() => _MediaPlayer.Stop();
    /// <summary>
    /// 일시정지
    /// </summary>
    public static void Pause() => _MediaPlayer.Pause();
    /// <summary>
    /// 모든 리소스 해제
    /// </summary>
    public static void Dispose() => _MediaPlayer.Dispose();
  }
}
