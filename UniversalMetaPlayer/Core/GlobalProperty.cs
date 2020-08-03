using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using MaterialDesignThemes.Wpf;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UMP.Utility;

namespace UMP.Core
{
  /// <summary>
  /// 프로그램 전체 설정
  /// </summary>
  public static class GlobalProperty
  {
    private static readonly Log Log;
    public static event UMP_PropertyChangedEventHandler PropertyChanged;
    private static void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(propertyName);
    static GlobalProperty()
    {
      Log = new Log(typeof(GlobalProperty));
      Log.Debug("메인 설정 초기화 시작");
      ThemeHelper.ThemeChangedEvent += ThemeHelper_ThemeChangedEvent;
      MainFontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Font/#NanumGothic");
      LogoImage = new BitmapImage(new Uri("pack://application:,,,/UniversalMetaPlayer;component/Resources/MainImage.png", UriKind.RelativeOrAbsolute));
      LogoNoteImage = new BitmapImage(new Uri("pack://application:,,,/UniversalMetaPlayer;component/Resources/NoteImage.png", UriKind.RelativeOrAbsolute));
      Log.Info("메인 설정 초기화 완료");
    }

    /// <summary>
    /// 기본 설정으로 되돌립니다.
    /// </summary>
    public static void SetDefault()
    {
      Settings.Clear();

      //FileSavePath = "Save";
      //PrivateLogging = true;

      //GlobalKeyboardHook = false;
      //KeyEventDelay = 20;

      //IsAverageColorTheme = true;
      //AverageColorProcessingOffset = 30;
    }

    /// <summary>
    /// 설정을 저장 합니다.
    /// </summary>
    public static void Save()
    {
      try
      {
        var json = JsonConvert.SerializeObject(Settings);
        File.WriteAllText("UMP_Settings.json", json);
        Log.Info("메인 설정 저장 완료");
      }
      catch (Exception e)
      {
        Log.Fatal("메인 설정 저장 실패", e);
      }
    }

    /// <summary>
    /// 설정을 불러옵니다.
    /// </summary>
    /// <returns>성공시 true 반환</returns>
    public static void Load()
    {
      if (NowLoading)
        return;
      NowLoading = true;

      SetDefault();
      ThemeHelper.SetDefaultTheme();

      if (File.Exists("UMP_Settings.json"))
      {
        // 설정 불러오기
        try
        {
          string josn = File.ReadAllText("UMP_Settings.json", Encoding.UTF8);
          var loadedSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(josn);

          foreach (var pair in loadedSettings)
            SetSetting(pair.Key, pair.Value);

          GlobalKeyboardHook = loadedSettings.TryGetValue("GlobalKeyboardHook", out string value)
            ? (bool.TryParse(value, out bool result)
            ? result
            : DefaultGlobalKeyboardHook)
            : DefaultGlobalKeyboardHook;

          Log.Info("메인 설정 불러오기 완료");
        }
        catch (Exception e)
        {
          Log.Fatal("메인 설정 불러오기 실패", e);
          SetDefault();
        }

        // 테마 적용하기
        try
        {
          ThemeHelper.IsDarkMode = bool.Parse(Settings["IsDarkMode"]);
          ThemeHelper.ChangePrimaryColor((Color)ColorConverter.ConvertFromString(Settings["PrimaryColor"]));
          ThemeHelper.ChangeSecondaryColor((Color)ColorConverter.ConvertFromString(Settings["SecondaryColor"]));
          Log.Info("메인 테마 불러오기 완료");
        }
        catch (Exception e)
        {
          Log.Fatal("메인 테마 불러오기 실패", e);
          ThemeHelper.SetDefaultTheme();
        }
        OnPropertyChanged("Loaded");
      }
      else
      {
        Log.Warn("저장된 메인 설정 파일이 없습니다");
      }
      NowLoading = false;
    }

    private static bool NowLoading = false;

    private static readonly Dictionary<string, string> Settings = new Dictionary<string, string>();
    private static void SetSetting(string key, string value) => Settings[key] = value;
    private static bool TryGetSetting(string key, out string value) => Settings.TryGetValue(key, out value);

    #region 일반
    /// <summary>
    /// 캐시를 재외한 파일 저장 폴더 경로
    /// </summary>
    public static string FileSavePath
    {
      get => TryGetSetting("FileSavePath", out string value)
        ? (!string.IsNullOrWhiteSpace(value)
        ? value
        : DefaultFileSavePath)
        : DefaultFileSavePath;
      set
      {
        SetSetting("FileSavePath", value);
        OnPropertyChanged("FileSavePath");
      }
    }
    private const string DefaultFileSavePath = "Save";

    /// <summary>
    /// 개인정보(곡 정보, 경로 등) 로깅 여부
    /// </summary>
    public static bool PrivateLogging
    {
      get => TryGetSetting("PrivateLogging", out string value)
        ? (bool.TryParse(value, out bool result)
        ? result
        : DefaultPrivateLogging)
        : DefaultPrivateLogging;
      set
      {
        SetSetting("PrivateLogging", value.ToString());
        OnPropertyChanged("PrivateLogging");
      }
    }
    private const bool DefaultPrivateLogging = true;
    #endregion

    #region 테마
    /// <summary>
    /// 테마 변경되면 자동저장
    /// </summary>
    /// <param name="e">변경 후 테마</param>
    private static void ThemeHelper_ThemeChangedEvent(ThemeHelper.ThemeProperty e)
    {
      if (!NowLoading)
      {
        SetSetting("IsDarkMode", e.IsDarkMode.ToString());
        SetSetting("PrimaryColor", e.PrimaryColor.ToString());
        SetSetting("SecondaryColor", e.SecondaryColor.ToString());
        OnPropertyChanged("Theme");
      }
    }

    /// <summary>
    /// 대표(평균)색 테마 적용 여부
    /// </summary>
    public static bool IsAverageColorTheme
    {
      get => TryGetSetting("IsAverageColorTheme", out string value)
        ? (bool.TryParse(value, out bool result)
        ? result
        : DefaultIsAverageColorTheme)
        : DefaultIsAverageColorTheme;
      set
      {
        SetSetting("IsAverageColorTheme", value.ToString());
        if (value)
          MainMediaPlayer.GetAverageColor();
        OnPropertyChanged("IsAverageColorTheme");
      }
    }
    private const bool DefaultIsAverageColorTheme = true;

    /// <summary>
    /// 대표(평균)색 추출 오프셋 (0 이면 Off)
    /// </summary>
    public static int AverageColorProcessingOffset
    {
      get => TryGetSetting("AverageColorProcessingOffset", out string value)
        ? (int.TryParse(value, out int result)
        ? result
        : DefaultAverageColorProcessingOffset)
        : DefaultAverageColorProcessingOffset;
      set
      {
        SetSetting("AverageColorProcessingOffset", value.ToString());
        OnPropertyChanged("AverageColorProcessingOffset");
      }
    }
    private const int DefaultAverageColorProcessingOffset = 30;
    #endregion

    #region 키보드
    /// <summary>
    /// 전역 키보드 후킹 여부
    /// </summary>
    public static bool HotKey
    {
      get => TryGetSetting("HotKey", out string value)
        ? (bool.TryParse(value, out bool result)
        ? result
        : DefaultHotKey)
        : DefaultHotKey;
      set
      {
        SetSetting("HotKey", value.ToString());
        if (value)
          Hook.Start();
        else
          Hook.Dispose();
        OnPropertyChanged("HotKey");
      }
    }
    private const bool DefaultHotKey = false;

    /// <summary>
    /// 전역 키보드 후킹 여부
    /// </summary>
    public static bool GlobalKeyboardHook
    {
      get => TryGetSetting("GlobalKeyboardHook", out string value)
        ? (bool.TryParse(value, out bool result)
        ? result
        : DefaultGlobalKeyboardHook)
        : DefaultGlobalKeyboardHook;
      set
      {
        SetSetting("GlobalKeyboardHook", value.ToString());
        if (value)
          Hook.Start();
        else
          Hook.Dispose();
        OnPropertyChanged("GlobalKeyboardHook");
      }
    }
    private const bool DefaultGlobalKeyboardHook = false;

    /// <summary>
    /// 키보드 입력 딜레이
    /// </summary>
    public static int KeyEventDelay
    {
      get => TryGetSetting("KeyEventDelay", out string value)
        ? (int.TryParse(value, out int result)
        ? result
        : DefaultKeyEventDelay)
        : DefaultKeyEventDelay;
      set
      {
        SetSetting("KeyEventDelay", value.ToString());
        OnPropertyChanged("KeyEventDelay");
      }
    }
    private const int DefaultKeyEventDelay = 20;
    #endregion

    #region 엔진
    /// <summary>
    /// 사용할 미디어 로드 엔진
    /// </summary>
    public static MediaLoadEngineType MediaLoadEngine
    {
      get => TryGetSetting("MediaLoadEngine", out string value)
        ? (Enum.TryParse(value, out MediaLoadEngineType result)
        ? result
        : DefaultMediaLoadEngineType)
        : DefaultMediaLoadEngineType;
      set
      {
        SetSetting("MediaLoadEngine", value.ToString());
        OnPropertyChanged("MediaLoadEngine");
      }
    }
    private const MediaLoadEngineType DefaultMediaLoadEngineType = MediaLoadEngineType.Native;
    public enum MediaLoadEngineType
    {
      Native, YoutubeDL
    }

    #endregion

    #region Not Save
    /// <summary>
    /// 컨트롤 가능 여부 (로딩 중 조작 금지용)
    /// </summary>
    public static bool IsControllable
    {
      get => _IsControllable;
      set
      {
        _IsControllable = value;
        OnPropertyChanged("IsControllable");
      }
    }
    private static bool _IsControllable = true;
    /// <summary>
    /// 폰트
    /// </summary>
    public static FontFamily MainFontFamily { get; private set; }
    /// <summary>
    /// 로고 이미지
    /// </summary>
    public static ImageSource LogoImage { get; private set; }
    /// <summary>
    /// 로고 음표 이미지
    /// </summary>
    public static ImageSource LogoNoteImage { get; private set; }

    /// <summary>
    /// 미디어 Null Process 때 접두어
    /// </summary>
    public const string MEDIA_INFO_NULL = "(null)";

    /// <summary>
    /// 라이브러리 폴더
    /// </summary>
    public const string LIBRARY_PATH = @"Core\Library";
    /// <summary>
    /// 캐시 폴더
    /// </summary>
    public const string CACHE_PATH = @"Core\Cache";
    /// <summary>
    /// 다운로드 캐시
    /// </summary>
    public static string DownloadCachePath => Path.Combine(CACHE_PATH, "DownloadCache");
    /// <summary>
    /// 온라인에서 다운로드한 미디어 저장 캐시
    /// </summary>
    public static string OnlineMediaCachePath => Path.Combine(CACHE_PATH, "OnlineMedia");
    /// <summary>
    /// YouTube-dl 라이브러리 저장 폴더
    /// </summary>
    public static string YTDL_PATH => Path.Combine(LIBRARY_PATH, "YTDL", "youtube-dl.exe");
    /// <summary>
    /// FFmpeg 라이브러리 저장 폴더
    /// </summary>
    public static string FFMPEG_PATH => Path.Combine(LIBRARY_PATH, "FFmpeg");
    #endregion

    #region Version
    /// <summary>
    /// 코어 버전
    /// </summary>
    public static string CoreVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    /// <summary>
    /// 파일 버전
    /// </summary>
    public static string FileVersion => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
    #endregion
  }
}
