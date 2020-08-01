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
      PlayList = new PlayList();
      MediaInfomation = new MediaInfomation()
      {
        Title = string.Empty,
        Duration = TimeSpan.Zero,
        AlbumImage = null
      };

      PlayListPlayMediaIndex = -1;
      Volume = 0.3f;
      OptionDefault();
      // 오토 플레이 옵션
      PropertyChangedEvent += (e) =>
      {
        if (e == "MainPlayerInitialized")
          if (MediaLoadedCheck && Option.AutoPlayOption && !NotAutoPlay)
            Play();
      };

      // 스트림 끝에 오류가 있는 미디어 예방
      TickEvent += (s, e) =>
      {
        if (AudioCurrentTime >= AudioTotalTime)
          WavePlayer.Stop();
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
    public static string PlayListEigenValue { get; set; }

    /// <summary>
    /// 현재 플레이리스트에서 재생중인 미디어의 Index
    /// </summary>
    public static int PlayListPlayMediaIndex { get; set; }

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
    public static event UMP_PropertyChangedEventHandler PropertyChangedEvent;
    private static void OnPropertyChanged(string name) => PropertyChangedEvent?.Invoke(name);

    public static bool MediaLoadedCheck => AudioFile != null;

    /// <summary>
    /// 미디어 파일 정보
    /// </summary>
    public static MediaInfomation MediaInfomation
    {
      get => _MediaInfomation;
      set
      {
        _MediaInfomation = value;
        GetAverageColor();
        OnPropertyChanged("MediaInfomation");
      }
    }
    private static MediaInfomation _MediaInfomation;

    /// <summary>
    /// 대표색 추출
    /// </summary>
    public static void GetAverageColor()
    {
      if (GlobalProperty.AverageColorProcessingOffset > 0 && _MediaInfomation.AlbumImage != null)
        AverageColor = ImageProcessing.GetAverageColor(_MediaInfomation.AlbumImage as BitmapSource, GlobalProperty.AverageColorProcessingOffset);
      else
        AverageColor = ThemeHelper.PrimaryColor;
    }

    /// <summary>
    /// 미디어 이미지 대표색
    /// </summary>
    public static Color AverageColor
    {
      get => _AverageColor;
      private set
      {
        _AverageColor = value;
        OnPropertyChanged("AverageColor");
      }
    }
    private static Color _AverageColor;

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
    private static float _Volume;

    /// <summary>
    /// 현재 재생 상태
    /// </summary>
    public static PlaybackState PlaybackState => WavePlayer != null ? WavePlayer.PlaybackState : PlaybackState.Stopped;

    /// <summary>
    /// 미디어로 초기화하고 재생을 준비합니다
    /// </summary>
    /// <param name="media">재생할 미디어</param>
    public static async Task<bool> Init(IMediaLoader mediaLoader)
    {
      // 모든 정보로드
      var info = await mediaLoader.GetInfomationAsync(true);
      if (!info.LoadState)
        Log.Warn("미디어 정보에 오류가 있습니다", new NullReferenceException("Null Processed Media"), $"Title : [{info.Title}]\nLocation : [{info.MediaLocation}]");

      GenericResult<string> streamResult = await mediaLoader.GetStreamPathAsync(true);
      if (!streamResult)
      {
        Log.Error("미디어 스트림 로드에 실패했습니다", new FileLoadException("Media Stream Path is Null"), $"Title : [{info.Title}]\nLocation : [{info.MediaLocation}]");
        return false;
      }

      string path = streamResult.Result;

      Stop();
      if (AudioFile != null)
      {
        await AudioFile.DisposeAsync();
        AudioFile = null;
      }

      try
      {
        AudioFile = new MediaFoundationReader(path);
      }
      catch (Exception e)
      {
        Log.Error("미디어 파일 로드중 오류가 발생했습니다", e);
        return false;
      }
      MediaInfomation = info;

      StopButtonActive = false;

      WavePlayer?.Dispose();
      WavePlayer = new WaveOut();
      WavePlayer.PlaybackStopped += MediaPlayer_PlaybackStopped;
      WavePlayer.Volume = _Volume;
      WavePlayer.Init(AudioFile);

      Log.Debug("미디어 재생 준비 완료", $"Info : [{(mediaLoader.Online ? $"ID : [{mediaLoader.GetID()}]" : $"FileName : [{Path.GetFileName(info.MediaLocation)}]")}]\nTitle : [{info.Title}]");

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
    public static async void Next()
    {
      if (!MediaLoadedCheck || PlayList.Count <= 0)
        return;

      if (PlayListPlayMediaIndex >= 0)
      {
        if (PlayListEigenValue != PlayList.EigenValue)
          PlayListPlayMediaIndex = 0;

        if (Option.Shuffle && PlayListPlayMediaIndex >= 0)
          PlayListPlayMediaIndex = RandomFunc.RandomInt(PlayListPlayMediaIndex, PlayListPlayMediaIndex, 0, PlayList.Count);

        int index = PlayListPlayMediaIndex + 1;
        var beforeState = PlaybackState;
        bool InitComplete = false;
        NotAutoPlay = true;
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
            return;

          index++;
        }
        while (!InitComplete);
        if (beforeState == PlaybackState.Playing)
          Play();
      }
      NotAutoPlay = false;
    }

    /// <summary>
    /// 이전 미디어
    /// </summary>
    public static async void Previous()
    {
      if (!MediaLoadedCheck || PlayList.Count <= 0)
        return;

      if (AudioFile.CurrentTime > TimeSpan.FromSeconds(5))
      {
        AudioFile.CurrentTime = TimeSpan.Zero;
        PlayStateChangedEvent?.Invoke(PlaybackState);
      }
      else if (PlayListPlayMediaIndex >= 0)
      {
        if (PlayListEigenValue != PlayList.EigenValue)
          PlayListPlayMediaIndex = 0;

        if (Option.Shuffle && PlayListPlayMediaIndex >= 0)
          PlayListPlayMediaIndex = RandomFunc.RandomInt(PlayListPlayMediaIndex, PlayListPlayMediaIndex, 0, PlayList.Count);

        int index = PlayListPlayMediaIndex - 1;
        var beforeState = PlaybackState;
        bool InitComplete = false;
        NotAutoPlay = true;
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
            return;

          index--;
        }
        while (!InitComplete);
        if (beforeState == PlaybackState.Playing)
          Play();
      }
      NotAutoPlay = false;
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
