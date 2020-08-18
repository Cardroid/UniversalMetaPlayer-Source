using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.IO;

using NAudio.Wave;

using UMP.Core.Function;
using UMP.Core.Model;
using UMP.Utility;
using System.Collections.Generic;

namespace UMP.Core.Player
{
  public static class MainMediaPlayer
  {
    static MainMediaPlayer()
    {
      Log = new Log(typeof(MainMediaPlayer));
      Option = new PlayerOption();
      OptionDefault();
      PlayList = new PlayList();
      MediaInformation = new MediaInformation()
      {
        Title = string.Empty,
        Duration = TimeSpan.Zero,
        AlbumImage = null
      };

      GlobalProperty.PropertyChanged += (e) => { if (e == "Loaded") Volume = Option.Volume; };

      // 미디어 재생시간 오류 처리
      TickEvent += (_, e) =>
      {
        if (MediaLoadedCheck && MediaInformation.Duration < AudioFile.CurrentTime)
        {
          WavePlayer.Stop();
          Tick.Stop();
          Log.Warn("미디어 재생시간 오류가 감지되어 자동으로 이벤트 처리됨", $"MediaLocation : [{MediaInformation.MediaLocation}]");
          PropertyChangedEvent?.Invoke("PlaybackStopped");
          PlayStateChangedEvent?.Invoke(PlaybackState);
        }
      };

      // 재생 종료후 이벤트
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
                ReserveCommand(PlaybackState.Playing);
              else if (Option.RepeatPlayOption == 2)
                await Next();
            }
          }
        }
      };

      Log.Debug("초기화 완료");
    }

    private static Log Log { get; }

    /// <summary>
    /// 메인 플레이어
    /// </summary>
    private static IWavePlayer WavePlayer { get; set; }

    private static SampleAggregator Aggregator { get; set; }

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

    private static DispatcherTimer Tick { get; } = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(3) /* 1 ms = 10000 ticks */ };
    public static event EventHandler TickEvent
    {
      add => Tick.Tick += value;
      remove => Tick.Tick -= value;
    }

    private static bool StopButtonActive { get; set; } = false;

    public delegate void PlayStateChangedEventHandler(PlaybackState state);
    public static event PlayStateChangedEventHandler PlayStateChangedEvent;
    public static event UMP_PropertyChangedEventHandler PropertyChangedEvent;
    private static void OnPropertyChanged(string name) => PropertyChangedEvent?.Invoke(name);

    // 오디오 분석용 코드

    public static event EventHandler<FftEventArgs> FftCalculated;

    public static event EventHandler<MaxSampleEventArgs> MaximumCalculated;


    public static bool MediaLoadedCheck => AudioFile != null && MediaInformation.Duration > TimeSpan.Zero;

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
    public static MediaInformation NotChangedMediaInformation { get; private set; }

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

    public static WaveFormat WaveFormat => Aggregator.WaveFormat;

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
    /// 오디오 전체 길이
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
      PlayStateChangedEvent?.Invoke(PlaybackState);
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
    public static PlaybackState PlaybackState
    {
      get
      {
        if (WavePlayer != null)
        {
          //if (WavePlayer.PlaybackState == PlaybackState.Stopped)
          //  return PlaybackState.Stopped;
          //if (!GlobalProperty.Options.FadeEffect)
          return WavePlayer.PlaybackState;
          //else
          //  return StateSave;
        }
        else
          return PlaybackState.Stopped;
      }
    }
    private static PlaybackState StateSave { get; set; }

    /// <summary>
    /// 미디어로 초기화하고 재생을 준비합니다
    /// </summary>
    /// <param name="media">재생할 미디어</param>
    public static async Task<bool> Init(MediaInformation mediainfo)
    {
      MediaLoader mediaLoader = new MediaLoader(mediainfo);
      MediaInformation info = new MediaInformation();
      try
      {
        // 모든 정보로드
        info = await mediaLoader.GetInformationAsync(true);
      }
      catch (Exception e)
      {
        Log.Fatal("미디어 정보 로드 실패", e, $"Title : [{info.Title}]\nLocation : [{info.MediaLocation}]");
        return false;
      }
      if (!info.LoadState)
        Log.Warn("미디어 정보에 오류가 있습니다", new NullReferenceException("Null Processed Media"), $"Title : [{info.Title}]\nLocation : [{info.MediaLocation}]");

      GenericResult<string> streamResult = null;
      try
      {
        // 미디어 스트림 로드
        streamResult = await mediaLoader.GetStreamPathAsync(true);
      }
      catch (Exception e)
      {
        Log.Fatal("미디어 스트림 로드에 오류가 발생했습니다", e, $"Title : [{info.Title}]\nLocation : [{info.MediaLocation}]");
        return false;
      }
      if (streamResult != null && !streamResult)
      {
        Log.Fatal("미디어 스트림 로드에 실패했습니다", new FileLoadException("Media Stream Path is Null"), $"Title : [{info.Title}]\nLocation : [{info.MediaLocation}]");
        return false;
      }

      string path = streamResult.Result;

      // 이전 오디오 스트림 해제
      if (AudioFile != null)
      {
        await AudioFile.DisposeAsync();
        AudioFile = null;
      }

      // 오디오 스트림 로드 시도
      AudioFileReader audioFile;
      try
      {
        audioFile = new AudioFileReader(path);
        AudioFile = audioFile;
      }
      catch (Exception e)
      {
        Log.Fatal("미디어 파일 로드중 오류가 발생했습니다", e);
        return false;
      }
      MediaInformation = info;
      NotChangedMediaInformation = mediainfo;

      StopButtonActive = false;


      // 오디오 분석용 코드
      Aggregator = new SampleAggregator(audioFile);
      Aggregator.NotificationCount = AudioFile.WaveFormat.SampleRate / 100;
      Aggregator.PerformFFT = true;
      Aggregator.FftCalculated += (s, a) => FftCalculated?.Invoke(s, a);
      Aggregator.MaximumCalculated += (s, a) => MaximumCalculated?.Invoke(s, a);


      // 플래이어 초기화
      try
      {
        WavePlayer?.Dispose();
        WavePlayer = new WaveOut();
        WavePlayer.PlaybackStopped += MediaPlayer_PlaybackStopped;
        WavePlayer.Volume = Volume;
        WavePlayer.Init(Aggregator);
      }
      catch (Exception e)
      {
        WavePlayer?.Dispose();
        Log.Fatal("플레이어 초기화 실패", e, $"Title : [{info.Title}]\nLocation : [{info.MediaLocation}]");
        return false;
      }

      PropertyChangedEvent?.Invoke("MainPlayerInitialized");
      return true;
    }

    private static bool IsPlayStateChangeWork
    {
      get
      {
        return !IsWork && _IsPlayStateChangeWork;
      }
      set => _IsPlayStateChangeWork = value;
    }
    private static bool _IsPlayStateChangeWork = false;

    private static Queue<PlaybackState> PlayBackCommand { get; } = new Queue<PlaybackState>();
    private static bool IsExecuteWork = false;

    public static void ReserveCommand(PlaybackState state)
    {
      PlayBackCommand.Enqueue(state);
      if (!IsExecuteWork)
      {
        IsExecuteWork = true;
        ExecuteCommand();
        IsExecuteWork = false;
      }
    }

    private static async void ExecuteCommand()
    {
      if (PlayBackCommand.Count > 0)
      {
        await RevertStatusAsync(PlayBackCommand.Dequeue(), true);
        ExecuteCommand();
      }
    }

    /// <summary>
    /// 재생
    /// </summary>
    private static void Play(bool stateSave = true)
    {
      if (!IsPlayStateChangeWork && MediaLoadedCheck && WavePlayer.PlaybackState != PlaybackState.Playing)
      {
        IsPlayStateChangeWork = true;
        if (stateSave)
          StateSave = PlaybackState.Playing;
        Tick.Start();
        WavePlayer.Play();

        if (GlobalProperty.Options.FadeEffect)
          Aggregator.BeginFadeIn(GlobalProperty.Options.FadeEffectDelay);

        PlayStateChangedEvent?.Invoke(PlaybackState);
        IsPlayStateChangeWork = false;
      }
    }

    /// <summary>
    /// 정지
    /// </summary>
    private static async Task Stop(bool stateSave = true)
    {
      if (!IsPlayStateChangeWork && MediaLoadedCheck && WavePlayer.PlaybackState != PlaybackState.Stopped)
      {
        IsPlayStateChangeWork = true;
        if (stateSave)
          StateSave = PlaybackState.Stopped;
        StopButtonActive = true;
        PlayStateChangedEvent?.Invoke(PlaybackState);

        if (GlobalProperty.Options.FadeEffect)
        {
          Aggregator.BeginFadeOut(GlobalProperty.Options.FadeEffectDelay);
          await Task.Delay(GlobalProperty.Options.FadeEffectDelay + 200);
        }

        WavePlayer.Stop();
        Tick.Stop();
        IsPlayStateChangeWork = false;
        PlayStateChangedEvent?.Invoke(PlaybackState);
      }
    }

    /// <summary>
    /// 일시정지
    /// </summary>
    private static async Task Pause(bool stateSave = true)
    {
      if (!IsPlayStateChangeWork && MediaLoadedCheck && WavePlayer.PlaybackState != PlaybackState.Paused)
      {
        IsPlayStateChangeWork = true;
        if (stateSave)
          StateSave = PlaybackState.Paused;
        PlayStateChangedEvent?.Invoke(PlaybackState);

        if (GlobalProperty.Options.FadeEffect)
        {
          Aggregator.BeginFadeOut(GlobalProperty.Options.FadeEffectDelay);
          await Task.Delay(GlobalProperty.Options.FadeEffectDelay + 200);
        }

        WavePlayer.Pause();
        Tick.Stop();
        IsPlayStateChangeWork = false;
        PlayStateChangedEvent?.Invoke(PlaybackState);
      }
    }

    private static bool IsWork { get; set; } = false;

    /// <summary>
    /// 다음 미디어
    /// </summary>
    public static async Task Next()
    {
      if (IsWork)
        return;
      if (PlayList.Count <= 0)
      {
        IsWork = false;
        return;
      }

      IsWork = true;
      int playListMediaIndex = PlayList.IndexOf(NotChangedMediaInformation);

      if (playListMediaIndex >= 0)
      {
        if (PlayListEigenValue != PlayList.EigenValue)
        {
          if (PlayList.Count > 0)
            playListMediaIndex = 0;
          else
          {
            ReserveCommand(StateSave);
            IsWork = false;
            return;
          }
        }
        else
        {
          if (Option.Shuffle)
          {
            playListMediaIndex = new RandomFunc().RandomInt(playListMediaIndex, playListMediaIndex, 0, PlayList.Count);
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"셔플 인텍스 : {playListMediaIndex}");
#endif
          }
          else
            playListMediaIndex++;
        }

        int index = playListMediaIndex;
        bool InitComplete = false;
        do
        {
          if (index >= PlayList.Count)
            index = 0;

          if (PlayList[index].LoadState)
          {
            if (await Init(PlayList[index]))
            {
              PlayListEigenValue = PlayList.EigenValue;
              playListMediaIndex = index;
              InitComplete = true;
            }
          }
          else if (playListMediaIndex == index)
            break;

          index++;
        }
        while (!InitComplete);
      }
      ReserveCommand(StateSave);
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
      {
        IsWork = false;
        return;
      }

      IsWork = true;

      if (AudioFile.CurrentTime > TimeSpan.FromSeconds(5))
      {
        AudioFile.CurrentTime = TimeSpan.Zero;
        PlayStateChangedEvent?.Invoke(PlaybackState);
        IsWork = false;
        return;
      }

      int playListMediaIndex = PlayList.IndexOf(NotChangedMediaInformation);

      if (playListMediaIndex >= 0)
      {
        if (PlayListEigenValue != PlayList.EigenValue)
        {
          if (PlayList.Count > 0)
            playListMediaIndex = 0;
          else
          {
            ReserveCommand(StateSave);
            IsWork = false;
            return;
          }
        }
        else
        {
          if (Option.Shuffle)
            playListMediaIndex = new RandomFunc().RandomInt(playListMediaIndex, playListMediaIndex, 0, PlayList.Count);
          else
            playListMediaIndex--;
        }

        int index = playListMediaIndex;
        bool InitComplete = false;
        do
        {
          if (index < 0)
            index = PlayList.Count - 1;

          if (PlayList[index].LoadState)
          {
            if (await Init(PlayList[index]))
            {
              PlayListEigenValue = PlayList.EigenValue;
              playListMediaIndex = index;
              InitComplete = true;
            }
          }
          else if (playListMediaIndex == index)
            break;

          index--;
        }
        while (!InitComplete);
      }
      ReserveCommand(StateSave);
      IsWork = false;
    }

    /// <summary>
    /// 재생상태로 되돌립니다
    /// </summary>
    /// <param name="state">재생상태</param>
    private static async Task RevertStatusAsync(PlaybackState state, bool stateSave = false)
    {
      if (MediaLoadedCheck)
      {
        switch (state)
        {
          default:
          case PlaybackState.Stopped:
            await Stop(stateSave);
            break;
          case PlaybackState.Playing:
            Play(stateSave);
            break;
          case PlaybackState.Paused:
            await Pause(stateSave);
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
