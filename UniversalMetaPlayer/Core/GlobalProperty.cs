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

using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NYoutubeDL.Options;

using UMP.Utility;

using static UMP.Core.GlobalProperty.Options.Enums;

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
      Log.Info("메인 설정 초기화 완료");
    }

    /// <summary>
    /// 기본 설정으로 되돌립니다.
    /// </summary>
    public static void SetDefault()
    {
      Options.Clear();
      OnPropertyChanged("SetDefault");
    }

    /// <summary>
    /// 설정을 저장 합니다.
    /// </summary>
    public static void Save()
    {
      try
      {
        JObject jObj = new JObject();

        jObj.Add("GlobalProperty", JObject.FromObject(Options.SettingsConvertToJson()));
        jObj.Add("MainMediaPlayerOption", JObject.FromObject(MainMediaPlayer.Option));

        string jText = $"# 이 설정 파일을 임의로 조작하지 마세요!\n# 임의로 설정파일을 조작하면, 프로그램에 오류가 발생할 수 있습니다!\n\n{jObj}";
        File.WriteAllText("UMP_Options.json", jText, Encoding.UTF8);

        Log.Info("메인 설정 저장 완료");
        GlobalEvent.GlobalMessageEventInvoke("메인 설정 저장 완료", true);
      }
      catch (Exception e)
      {
        GlobalEvent.GlobalMessageEventInvoke("메인 설정 저장 실패", true);
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

      if (File.Exists("UMP_Options.json"))
      {
        JObject jObj = null;

        // Json 파싱
        try
        {
          string[] jStringArray = File.ReadAllLines("UMP_Options.json", Encoding.UTF8);
          string jstring = string.Empty;

          for (int i = 0; i < jStringArray.Length; i++)
          {
            if (!jStringArray[i].StartsWith('#'))
              jstring += jStringArray[i];
          }

          jObj = JObject.Parse(jstring);
        }
        catch (Exception e)
        {
          Log.Fatal("설정 파일 불러오기 실패", e);
          SetDefault();
        }

        // 메인 설정 불러오기
        try
        {
          var loadedOptions = JsonConvert.DeserializeObject<Dictionary<string, string>>(jObj["GlobalProperty"].ToString());

          foreach (var pair in loadedOptions)
            Options.SetSetting(pair.Key, pair.Value);

          Options.GlobalKeyboardHook = loadedOptions.TryGetValue("GlobalKeyboardHook", out string value)
            ? (bool.TryParse(value, out bool result)
            ? result
            : Options.DefaultValue.DefaultGlobalKeyboardHook)
            : Options.DefaultValue.DefaultGlobalKeyboardHook;

          Log.Info("메인 설정 불러오기 완료");
        }
        catch (Exception e)
        {
          Log.Fatal("메인 설정 불러오기 실패", e);
          SetDefault();
        }

        // 플레이어 설정 불러오기
        try
        {
          var playerOption = JsonConvert.DeserializeObject<PlayerOption>(jObj["MainMediaPlayerOption"].ToString());

          MainMediaPlayer.Option = playerOption;

          Log.Info("플레이어 설정 불러오기 완료");
        }
        catch (Exception e)
        {
          Log.Fatal("플레이어 설정 불러오기 실패", e);
        }

        // 테마 적용하기
        try
        {
          ThemeHelper.IsDarkMode = bool.Parse(Options.TryGetSetting("IsDarkMode", out string value) ? value : bool.TrueString);
          ThemeHelper.ChangePrimaryColor((Color)ColorConverter.ConvertFromString(Options.TryGetSetting("PrimaryColor", out string primaryColor) ? value : Colors.Green.Lighten(3).ToString()));
          ThemeHelper.ChangeSecondaryColor((Color)ColorConverter.ConvertFromString(Options.TryGetSetting("SecondaryColor", out string secondaryColor) ? value : Colors.Green.Darken(3).ToString()));
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

    public static class Options
    {
      static Options()
      {
        ThemeHelper.ThemeChangedEvent += ThemeHelper_ThemeChangedEvent;
      }

      public static JObject SettingsConvertToJson() => JObject.FromObject(Settings);

      private static Dictionary<string, string> Settings { get; } = new Dictionary<string, string>();
      public static void SetSetting(string key, string value) => Settings[key] = value;
      public static bool TryGetSetting(string key, out string value) => Settings.TryGetValue(key, out value);
      public static void Clear() => Settings.Clear();

      #region 일반
      /// <summary>
      /// 캐시를 재외한 파일 저장 폴더 경로
      /// </summary>
      public static string FileSavePath
      {
        get => TryGetSetting("FileSavePath", out string value)
          ? (!string.IsNullOrWhiteSpace(value)
          ? value
          : DefaultValue.DefaultFileSavePath)
          : DefaultValue.DefaultFileSavePath;
        set
        {
          SetSetting("FileSavePath", value);
          OnPropertyChanged("FileSavePath");
        }
      }

      /// <summary>
      /// 개인정보(곡 정보, 경로 등) 로깅 여부
      /// </summary>
      public static bool PrivateLogging
      {
        get => TryGetSetting("PrivateLogging", out string value)
          ? (bool.TryParse(value, out bool result)
          ? result
          : DefaultValue.DefaultPrivateLogging)
          : DefaultValue.DefaultPrivateLogging;
        set
        {
          SetSetting("PrivateLogging", value.ToString());
          OnPropertyChanged("PrivateLogging");
        }
      }
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
      /// 평균색 테마 적용 여부
      /// </summary>
      public static bool IsAverageColorTheme
      {
        get => TryGetSetting("IsAverageColorTheme", out string value)
          ? (bool.TryParse(value, out bool result)
          ? result
          : DefaultValue.DefaultIsAverageColorTheme)
          : DefaultValue.DefaultIsAverageColorTheme;
        set
        {
          SetSetting("IsAverageColorTheme", value.ToString());
          if (value)
            ThemeHelper.SetAverageColorTheme();
          OnPropertyChanged("IsAverageColorTheme");
        }
      }

      /// <summary>
      /// 대표(평균)색 추출 오프셋 (0 이면 Off)
      /// </summary>
      public static int AverageColorProcessingOffset
      {
        get => TryGetSetting("AverageColorProcessingOffset", out string value)
          ? (int.TryParse(value, out int result)
          ? result
          : DefaultValue.DefaultAverageColorProcessingOffset)
          : DefaultValue.DefaultAverageColorProcessingOffset;
        set
        {
          SetSetting("AverageColorProcessingOffset", value.ToString());
          OnPropertyChanged("AverageColorProcessingOffset");
        }
      }
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
          : DefaultValue.DefaultHotKey)
          : DefaultValue.DefaultHotKey;
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

      /// <summary>
      /// 전역 키보드 후킹 여부
      /// </summary>
      public static bool GlobalKeyboardHook
      {
        get => TryGetSetting("GlobalKeyboardHook", out string value)
          ? (bool.TryParse(value, out bool result)
          ? result
          : DefaultValue.DefaultGlobalKeyboardHook)
          : DefaultValue.DefaultGlobalKeyboardHook;
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

      /// <summary>
      /// 키보드 입력 딜레이
      /// </summary>
      public static int KeyEventDelay
      {
        get => TryGetSetting("KeyEventDelay", out string value)
          ? (int.TryParse(value, out int result)
          ? result
          : DefaultValue.DefaultKeyEventDelay)
          : DefaultValue.DefaultKeyEventDelay;
        set
        {
          SetSetting("KeyEventDelay", value.ToString());
          OnPropertyChanged("KeyEventDelay");
        }
      }
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
          : DefaultValue.DefaultMediaLoadEngineType)
          : DefaultValue.DefaultMediaLoadEngineType;
        set
        {
          SetSetting("MediaLoadEngine", value.ToString());
          OnPropertyChanged("MediaLoadEngine");
        }
      }
      #endregion

      public static class DefaultValue
      {
        public const string DefaultFileSavePath = "Save";
        public const bool DefaultPrivateLogging = true;

        public const bool DefaultIsAverageColorTheme = true;
        public const int DefaultAverageColorProcessingOffset = 30;

        public const bool DefaultHotKey = false;
        public const bool DefaultGlobalKeyboardHook = false;
        public const int DefaultKeyEventDelay = 20;

        public const MediaLoadEngineType DefaultMediaLoadEngineType = MediaLoadEngineType.Native;
      }

      public class Enums
      {
        public enum MediaLoadEngineType
        {
          Native, YoutubeDL
        }
      }
    }

    public static class StaticValues
    {
      #region Not Save
      /// <summary>
      /// 폰트
      /// </summary>
      public static FontFamily MainFontFamily { get; } = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Font/#NanumGothic");
      /// <summary>
      /// 로고 이미지
      /// </summary>
      public static ImageSource LogoImage { get; } = new BitmapImage(new Uri("pack://application:,,,/UniversalMetaPlayer;component/Resources/MainImage.png", UriKind.RelativeOrAbsolute));
      /// <summary>
      /// 로고 음표 이미지
      /// </summary>
      public static ImageSource LogoNoteImage { get; } = new BitmapImage(new Uri("pack://application:,,,/UniversalMetaPlayer;component/Resources/NoteImage.png", UriKind.RelativeOrAbsolute));

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
}
