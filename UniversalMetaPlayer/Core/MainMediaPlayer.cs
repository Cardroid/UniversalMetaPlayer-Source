using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

using UMP.Core.Model;
using UMP.Utility;

using NAudio.Wave;

using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using UMP.Core.Function;
using System.IO;

namespace UMP.Core
{
  public static class MainMediaPlayer
  {
    static MainMediaPlayer()
    {
      Option = new PlayerOption();
      OptionDefault();
      PlayList = new PlayList();
      MediaInformation = new MediaInformation()
      {
        Title = string.Empty,
        Duration = TimeSpan.Zero,
        AlbumImage = null
      };

      PlayListPlayMediaIndex = -1;
      GlobalProperty.PropertyChanged += (e) => { if (e == "Loaded") Volume = Option.Volume; };
      // 오토 플레이 옵션
      PropertyChangedEvent += async (e) =>
      {
        if (e == "PlaybackStopped")
        {
          if (WavePlayer.PlaybackState == PlaybackState.Stopped)
          {
            AudioFile.CurrentTime = TimeSpan.Zero;
            if (StopButtonActive)
              StopButtonActive = false;
            else
            {
              if (Option.RepeatPlayOption == 1)
                RevertStatus(StateSave);
              else if (Option.RepeatPlayOption == 2)
              {
                if (PlayListPlayMediaIndex == -1)
                  RevertStatus(StateSave);
                else
                  await Next();
              }
            }
          }
        }          
      };

      Log.Debug("초기화 완료");
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
      Option.Volume = 0.3f;
      Option.Shuffle = false;
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
    public static string PlayListEigenValue { get; set; }

    /// <summary>
    /// 현재 플레이리스트에서 재생중인 미디어의 Index
    /// </summary>
    public static int PlayListPlayMediaIndex { get; set; }

    private static DispatcherTimer Tick { get; } = new DispatcherTimer { Interval = TimeSpan.FromTicks(1000) /* 1 ms = 10000 ticks */ };
    public static event EventHandler TickEvent
    {
      add => Tick.Tick += value;
      remove => Tick.Tick -= value;
    }

    private static bool StopButtonActive { get; set; } = false;
    private static bool IsWork { get; set; } = false;

    public delegate void PlayStateChangedEventHandler(PlaybackState state);
    public static event PlayStateChangedEventHandler PlayStateChangedEvent;
    public static event UMP_PropertyChangedEventHandler PropertyChangedEvent;
    private static void OnPropertyChanged(string name) => PropertyChangedEvent?.Invoke(name);

    public static bool MediaLoadedCheck => AudioFile != null;

    /// <summary>
    /// 미디어 파일 정보
    /// </summary>
    public static MediaInformation MediaInformation
    {
      get => _MediaInformation;
      set
      {
        _MediaInformation = value;
        OnPropertyChanged("MediaInformation");
      }
    }
    private static MediaInformation _MediaInformation;

    /// <summary>
    /// 오디오 파일 (약한참조)
    /// </summary>
    private static WaveStream AudioFile
    {
      get
      {
        if (_AudioFile != null && _AudioFile.TryGetTarget(out WaveStream ws))
          return ws;
        else
          return null;
      }
      set
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
    /// 오디오 현재 재생위치
    /// </summary>
    public static TimeSpan AudioCurrentTime
    {
      get => AudioFile.CurrentTime;
      set
      {
        AudioFile.CurrentTime = value;
        PlayStateChangedEvent?.Invoke(PlaybackState);
      }
    }

    /// <summary>
    /// 오디오 현재 재생위치
    /// </summary>
    public static TimeSpan AudioTotalTime => AudioFile.TotalTime;

    /// <summary>
    /// 재생이 끝났을 경우 이벤트 처리
    /// </summary>
    private static void MediaPlayer_PlaybackStopped(object sender, StoppedEventArgs e)
    {
      if (e.Exception != null)
        Log.Fatal("메인 플레이어 [PlaybackStopped] 이벤트 처리오류", e.Exception);
      PropertyChangedEvent?.Invoke("PlaybackStopped");
      PlayStateChangedEvent?.Invoke(WavePlayer.PlaybackState);
    }

    /// <summary>
    /// 볼륨
    /// </summary>
    public static float Volume
    {
      get => WavePlayer != null ? WavePlayer.Volume : Option.Volume;
      set
      {
        if (WavePlayer != null)
          WavePlayer.Volume = value;
        Option.Volume = value;
        OnPropertyChanged("Volume");
      }
    }

    /// <summary>
    /// 현재 재생 상태
    /// </summary>
    public static PlaybackState PlaybackState => WavePlayer != null ? WavePlayer.PlaybackState : PlaybackState.Stopped;
   private static PlaybackState StateSave { get; set; }

    /// <summary>
    /// 미디어로 초기화하고 재생을 준비합니다
    /// </summary>
    /// <param name="media">재생할 미디어</param>
    public static async Task<bool> Init(IMediaLoader mediaLoader)
    {
      // 모든 정보로드
      var info = await mediaLoader.GetInformationAsync(true);
      if (!info.LoadState)
        Log.Warn("미디어 정보에 오류가 있습니다", new NullReferenceException("Null Processed Media"), $"Title : [{info.Title}]\nLocation : [{info.MediaLocation}]");

      GenericResult<string> streamResult = await mediaLoader.GetStreamPathAsync(true);
      if (!streamResult)
      {
        Log.Fatal("미디어 스트림 로드에 실패했습니다", new FileLoadException("Media Stream Path is Null"), $"Title : [{info.Title}]\nLocation : [{info.MediaLocation}]");
        return false;
      }

      string path = streamResult.Result;

      // 재생중인 미디어 정지
      if (MediaLoadedCheck && PlaybackState != PlaybackState.Stopped)
      {
        StopButtonActive = true;
        WavePlayer.Stop();
        Tick.Stop();
      }

      // 이전 오디오 스트림 해제
      if (AudioFile != null)
      {
        await AudioFile.DisposeAsync();
        AudioFile = null;
      }

      // 오디오 스트림 로드 시도
      try
      {
        AudioFile = new MediaFoundationReader(path);
      }
      catch (Exception e)
      {
        Log.Fatal("미디어 파일 로드중 오류가 발생했습니다", e);
        return false;
      }
      MediaInformation = info;

      StopButtonActive = false;

      // 플래이어 초기화
      WavePlayer?.Dispose();
      WavePlayer = new WaveOut();
      WavePlayer.PlaybackStopped += MediaPlayer_PlaybackStopped;
      WavePlayer.Volume = Volume;
      WavePlayer.Init(AudioFile);

      PropertyChangedEvent?.Invoke("MainPlayerInitialized");
      return true;
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
        StateSave = PlaybackState.Playing;
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
        StateSave = PlaybackState.Stopped;
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
        StateSave = PlaybackState.Paused;
        PlayStateChangedEvent?.Invoke(WavePlayer.PlaybackState);
      }
    }

    /// <summary>
    /// 다음 미디어
    /// </summary>
    public static async Task Next()
    {
      if (IsWork)
        return;
      if (PlayList.Count <= 0)
        return;

      IsWork = true;
      if (PlayListPlayMediaIndex >= 0)
      {
        if (PlayListEigenValue != PlayList.EigenValue)
          PlayListPlayMediaIndex = 0;

        if (Option.Shuffle)
          PlayListPlayMediaIndex = RandomFunc.RandomInt(PlayListPlayMediaIndex, PlayListPlayMediaIndex, 0, PlayList.Count);

        int index = PlayListPlayMediaIndex + 1;
        bool InitComplete = false;
        do
        {
          if (index >= PlayList.Count)
            index = 0;

          if (PlayList[index].LoadState)
          {
            if (await Init(new MediaLoader(PlayList[index])))
            {
              PlayListEigenValue = PlayList.EigenValue;
              PlayListPlayMediaIndex = index;
              InitComplete = true;
            }
          }
          else if (PlayListPlayMediaIndex == index)
            break;

          index++;
        }
        while (!InitComplete);
        RevertStatus(StateSave);
      }
      IsWork = false;
    }

    /// <summary>
    /// 이전 미디어
    /// </summary>
    public static async Task Previous()
    {
      if (IsWork)
        return;
      if (PlayList.Count <= 0)
        return;

      IsWork = true;
      if (AudioFile.CurrentTime > TimeSpan.FromSeconds(5))
      {
        AudioFile.CurrentTime = TimeSpan.Zero;
        PlayStateChangedEvent?.Invoke(PlaybackState);
      }
      else if (PlayListPlayMediaIndex >= 0)
      {
        if (PlayListEigenValue != PlayList.EigenValue)
          PlayListPlayMediaIndex = 0;

        if (Option.Shuffle)
          PlayListPlayMediaIndex = RandomFunc.RandomInt(PlayListPlayMediaIndex, PlayListPlayMediaIndex, 0, PlayList.Count);

        int index = PlayListPlayMediaIndex - 1;
        bool InitComplete = false;
        do
        {
          if (index < 0)
            index = PlayList.Count - 1;

          if (PlayList[index].LoadState)
          {
            if (await Init(new MediaLoader(PlayList[index])))
            {
              PlayListEigenValue = PlayList.EigenValue;
              PlayListPlayMediaIndex = index;
              InitComplete = true;
            }
          }
          else if (PlayListPlayMediaIndex == index)
            break;

          index--;
        }
        while (!InitComplete);
        RevertStatus(StateSave);
      }
      IsWork = false;
    }

    /// <summary>
    /// 재생상태로 되돌립니다
    /// </summary>
    /// <param name="state">재생상태</param>
    private static void RevertStatus(PlaybackState state)
    {
      if (MediaLoadedCheck)
      {
        switch (state)
        {
          default:
          case PlaybackState.Stopped:
            Stop();
            break;
          case PlaybackState.Playing:
            Play();
            break;
          case PlaybackState.Paused:
            Pause();
            break;
        }
      }
    }

    /// <summary>
    /// 모든 리소스 해제
    /// </summary>
    public static void Dispose()
    {
      AudioFile?.Dispose();
      WavePlayer?.Dispose();
      Log.Debug("리소스 해제 완료");
    }
  }

  /// <summary>
  /// 플레이어 옵션
  /// </summary>
  public struct PlayerOption
  {
    public event PropertyChangedEventHandler PropertyChangedEvent;
    private void OnPropertyChanged(string propertyName) => PropertyChangedEvent?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool _Shuffle;
    private int _RepeatPlayOption;
    private bool _DurationViewStatus;
    private bool _LastSongSaveOption;

    /// <summary>
    /// 세이브 전용 볼륨값
    /// </summary>
    public float Volume;

    /// <summary>
    /// 셔플 옵션
    /// </summary>
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
