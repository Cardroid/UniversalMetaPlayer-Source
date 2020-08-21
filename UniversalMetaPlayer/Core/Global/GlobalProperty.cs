using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UMP.Utility;
using UMP.Core.Player;
using MaterialDesignColors.ColorManipulation;

namespace UMP.Core.Global
{
  /// <summary>
  /// 프로그램 전체 설정
  /// </summary>
  public static class GlobalProperty
  {
    private static readonly Log Log;
    public static event PropertyChangedEventHandler PropertyChanged;
    private static void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));

    static GlobalProperty()
    {
      Log = new Log(typeof(GlobalProperty));
      ThemeHelper.ThemeChangedEvent += ThemeHelper_ThemeChangedEvent;
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
        JObject jObj = new JObject
        {
          { "GlobalProperty", JObject.FromObject(Options.SettingsConvertToJson()) },
          { "MainMediaPlayerOption", JObject.FromObject(MainMediaPlayer.Option) }
        };

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
          {
            if (Enum.TryParse(pair.Key, out Enums.ValueName key))
              Options.Setter(key, pair.Value);
          }

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
          ThemeHelper.ThemeChangedEvent -= ThemeHelper_ThemeChangedEvent;
          ThemeHelper.IsDarkMode = Options.Getter<bool>(Enums.ValueName.IsDarkMode);
          ThemeHelper.ChangePrimaryColor(Options.Getter<Color>(Enums.ValueName.PrimaryColor));
          ThemeHelper.ChangeSecondaryColor(Options.Getter<Color>(Enums.ValueName.SecondaryColor));
          ThemeHelper.ThemeChangedEvent += ThemeHelper_ThemeChangedEvent;
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
        Log.Warn("저장된 메인 설정 파일이 없습니다");
    }

    /// <summary>
    /// 테마 변경되면 자동저장
    /// </summary>
    /// <param name="e">변경 후 테마</param>
    private static void ThemeHelper_ThemeChangedEvent(ThemeHelper.ThemeProperty e)
    {
      Options.Setter(Enums.ValueName.IsDarkMode, e.IsDarkMode.ToString());
      Options.Setter(Enums.ValueName.PrimaryColor, e.PrimaryColor.ToString());
      Options.Setter(Enums.ValueName.SecondaryColor, e.SecondaryColor.ToString());
      OnPropertyChanged("Theme");
    }

    public static class State
    {
      /// <summary>
      /// 컨트롤 가능 여부
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
    }

    public static class Options
    {
      public static JObject SettingsConvertToJson() => JObject.FromObject(Settings);
      private static Dictionary<string, string> Settings { get; } = new Dictionary<string, string>();

      public static void Setter(Enums.ValueName key, string value)
      {
        Settings[key.ToString()] = value;

        if (key == Enums.ValueName.GlobalKeyboardHook)
        {
          if (bool.TryParse(value, out bool result) && result)
            Hook.Start();
          else
            Hook.Dispose();
        }

        OnPropertyChanged(key.ToString());
      }

      public static T Getter<T>(Enums.ValueName key)
      {
        if (Settings.TryGetValue(key.ToString(), out string result))
          return Converter.ChangeType<string, T>(result);
        else
          return DefaultValue.GetDefaultValue<T>(key);
      }

      public static void Clear() => Settings.Clear();
    }

    /// <summary>
    /// 기본값이 정의 되어 있는 클래스
    /// </summary>
    public static class DefaultValue
    {
      public static T GetDefaultValue<T>(Enums.ValueName valueName)
      {
        return valueName switch
        {
          // 일반
          Enums.ValueName.FileSavePath => Converter.ChangeType<string, T>("Save"),
          Enums.ValueName.PrivateLogging => Converter.ChangeType<bool, T>(true),
          Enums.ValueName.MediaLoadEngine => Converter.ChangeType<Enums.MediaLoadEngineType, T>(Enums.MediaLoadEngineType.Native),
          Enums.ValueName.LyricsWindowActive => Converter.ChangeType<Enums.LyricsSettingsType, T>(Enums.LyricsSettingsType.Off),
          // 테마
          Enums.ValueName.IsDarkMode => Converter.ChangeType<bool, T>(true),
          Enums.ValueName.PrimaryColor => Converter.ChangeType<Color, T>(Colors.Green.Lighten(3)),
          Enums.ValueName.SecondaryColor => Converter.ChangeType<Color, T>(Colors.Green.Darken(3)),
          Enums.ValueName.IsAverageColorTheme => Converter.ChangeType<bool, T>(true),
          Enums.ValueName.AverageColorProcessingOffset => Converter.ChangeType<int, T>(30),
          // 키보드
          Enums.ValueName.HotKey => Converter.ChangeType<bool, T>(false),
          Enums.ValueName.GlobalKeyboardHook => Converter.ChangeType<bool, T>(true),
          Enums.ValueName.KeyEventDelay => Converter.ChangeType<int, T>(20),
          // 효과
          Enums.ValueName.IsUseFadeEffect => Converter.ChangeType<bool, T>(true),
          Enums.ValueName.FadeEffectDelay => Converter.ChangeType<int, T>(200),

          _ => default,
        };
      }
    }

    public static class Predefine
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
      public static string MEDIA_INFO_NULL { get; } = "(null)";

      /// <summary>
      /// 라이브러리 폴더
      /// </summary>
      public static string LIBRARY_PATH { get; } = @"Core\Library";
      /// <summary>
      /// 캐시 폴더
      /// </summary>
      public static string CACHE_PATH { get; } = @"Core\Cache";
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
      public static string YTDL_PATH => Path.Combine(LIBRARY_PATH, "YTDL");
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
      /// <summary>
      /// 현재 프로세스의 비트버전
      /// </summary>
      public static string BitVersion => Environment.Is64BitProcess ? "x64" : "x86";
      #endregion
    }
  }

  public class Enums
  {
    public enum ValueName
    {
      // 일반
      FileSavePath, PrivateLogging, MediaLoadEngine, LyricsWindowActive,
      // 테마
      IsDarkMode, PrimaryColor, SecondaryColor,
      IsAverageColorTheme, AverageColorProcessingOffset,
      // 키보드
      HotKey, GlobalKeyboardHook, KeyEventDelay,
      // 효과
      IsUseFadeEffect, FadeEffectDelay
    }

    /// <summary>
    /// 미디어 로더 타입
    /// </summary>
    public enum MediaLoadEngineType
    {
      Native, YoutubeDL
    }

    /// <summary>
    /// 가사창 설정 타입<br/>
    /// Auto => <c>Lyrics != null ? Open : Close</c>
    /// </summary>
    public enum LyricsSettingsType
    {
      Off, Auto, On
    }
  }
}
