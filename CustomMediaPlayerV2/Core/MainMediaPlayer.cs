using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

using CMP2.Core.Model;
using CMP2.Utility;

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
      MediaInfomation = new MediaInfomation()
      {
        LoadedCheck = LoadState.NotTryed,
        Title = string.Empty,
        Duration = TimeSpan.Zero,
        AlbumImage = null,
        AlbumTitle = string.Empty,
        AlbumArtist = string.Empty,
        Lyrics = string.Empty
      };

      PlayListPlayMediaIndex = -1;
      Volume = 0.8f;
      OptionDefault();
      // 오토 플레이 옵션
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
    public static event CMP_PropertyChangedEventHandler PropertyChangedEvent;
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
        OnPropertyChanged("MediaInfomation");
      }
    }
    private static MediaInfomation _MediaInfomation;

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
    public static float _Volume;

    /// <summary>
    /// 현재 재생 상태
    /// </summary>
    public static PlaybackState PlaybackState => WavePlayer != null ? WavePlayer.PlaybackState : PlaybackState.Stopped;

    /// <summary>
    /// 미디어로 초기화하고 재생을 준비합니다
    /// </summary>
    /// <param name="media">재생할 미디어</param>
    public static async void Init(Media media)
    {
      string path = await media.GetStreamPath();

      if (string.IsNullOrWhiteSpace(path))
      {
        Log.Error("미디어 위치정보가 누락 되었습니다. (path is Null)");
        return;
      }

      // 모든 정보로드
      await media.TryInfoAllLoadAsync();
      var info = media.GetInfomation();

      Stop();
      if (AudioFile != null)
      {
        await AudioFile.DisposeAsync();
        AudioFile = null;
      }

      AudioFile = new MediaFoundationReader(path);
      MediaInfomation = info;

      StopButtonActive = false;

      WavePlayer?.Dispose();
      WavePlayer = new WaveOut();
      WavePlayer.PlaybackStopped += MediaPlayer_PlaybackStopped;
      WavePlayer.Volume = _Volume;
      WavePlayer.Init(AudioFile);

      Log.Debug($"[{(string.IsNullOrWhiteSpace(media.GetYouTubeVideoID()) ? System.IO.Path.GetFileNameWithoutExtension(info.MediaLocation) : $"\"{media.GetYouTubeVideoID()}\" {info.Title}")}] 메인 미디어 플레이어에 로드 성공.");

      PropertyChangedEvent?.Invoke("MainPlayerInitialized");
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
      if (!MediaLoadedCheck)
        return;

      if (PlayListPlayMediaIndex >= 0)
      {
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

          if ((int)PlayList[index].LoadedCheck >= 2)
          {
            Init(new Media(PlayList[index].MediaType, PlayList[index].MediaLocation));
            PlayListPlayMediaIndex = index;
            InitComplete = true;
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
    public static void Previous()
    {
      if (!MediaLoadedCheck)
        return;

      if (AudioFile.CurrentTime > TimeSpan.FromSeconds(5))
      {
        AudioFile.CurrentTime = TimeSpan.Zero;
        PlayStateChangedEvent?.Invoke(PlaybackState);
      }
      else if (PlayListPlayMediaIndex >= 0)
      {
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

          if ((int)PlayList[index].LoadedCheck >= 2)
          {
            Init(new Media(PlayList[index].MediaType, PlayList[index].MediaLocation));
            PlayListPlayMediaIndex = index;
            InitComplete = true;
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
