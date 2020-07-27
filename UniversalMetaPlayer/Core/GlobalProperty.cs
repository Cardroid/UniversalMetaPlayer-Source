using System;
using System.Collections.Generic;
using System.IO;
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
    public static UMP_PropertyChangedEventHandler PropertyChanged;
    private static void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(propertyName);
    static GlobalProperty()
    {
      Log = new Log(typeof(GlobalProperty));
      Log.Debug("메인 설정 초기화 시작.");
      ThemeHelper.ThemeChangedEvent += ThemeHelper_ThemeChangedEvent;
      MainFontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Font/#NanumGothic");
      LogoImage = new BitmapImage(new Uri("pack://application:,,,/UniversalMetaPlayer;component/Resources/MainImage.png", UriKind.RelativeOrAbsolute));
      LogoNoteImage = new BitmapImage(new Uri("pack://application:,,,/UniversalMetaPlayer;component/Resources/NoteImage.png", UriKind.RelativeOrAbsolute));
      Log.Info("메인 설정 초기화 완료.");
    }

    /// <summary>
    /// 기본 설정으로 되돌립니다.
    /// </summary>
    public static void SetDefault()
    {
      CachePath = "Cache";
      FileSavePath = "Save";
      GlobalKeyboardHook = false;
      AverageColorProcessingOffset = 30;
      IsAverageColorTheme = true;

      KeyEventDelay = 20;
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
        Log.Info("메인 설정 저장 완료.");
      }
      catch (Exception e)
      {
        Log.Error("메인 설정 저장 실패.", e);
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
            Settings[pair.Key] = pair.Value;

          GlobalKeyboardHook = bool.TryParse(loadedSettings["GlobalKeyboardHook"], out bool globalKeyboardHookOption) && globalKeyboardHookOption;

          Log.Info("메인 설정 불러오기 성공.");
        }
        catch (Exception e)
        {
          Log.Error("메인 설정 불러오기 실패.", e);
          SetDefault();
        }

        // 테마 적용하기
        try
        {
          ThemeHelper.IsDarkMode = bool.Parse(Settings["IsDarkMode"]);
          ThemeHelper.ChangePrimaryColor((Color)ColorConverter.ConvertFromString(Settings["PrimaryColor"]));
          ThemeHelper.ChangeSecondaryColor((Color)ColorConverter.ConvertFromString(Settings["SecondaryColor"]));
          Log.Info("메인 테마 불러오기 성공.");
        }
        catch (Exception e)
        {
          Log.Error("메인 테마 불러오기 실패.", e);
          ThemeHelper.SetDefaultTheme();
        }
        OnPropertyChanged("Loaded");
      }
      else
      {
        Log.Error("저장된 메인 설정 파일이 없습니다.");
      }
      NowLoading = false;
    }

    private static bool NowLoading = false;

    private static readonly Dictionary<string, string> Settings = new Dictionary<string, string>();

    private static void ThemeHelper_ThemeChangedEvent(ThemeHelper.ThemeProperty e)
    {
      if (!NowLoading)
      {
        Settings["IsDarkMode"] = e.IsDarkMode.ToString();
        Settings["PrimaryColor"] = e.PrimaryColor.ToString();
        Settings["SecondaryColor"] = e.SecondaryColor.ToString();
        OnPropertyChanged("Theme");
      }
    }

    /// <summary>
    /// 캐쉬저장 폴더 경로
    /// </summary>
    public static string CachePath
    {
      get => Settings["CachePath"];
      set { Settings["CachePath"] = value;
        OnPropertyChanged("CachePath");
      }
    }

    /// <summary>
    /// 캐쉬를 재외한 파일 저장 폴더 경로
    /// </summary>
    public static string FileSavePath
    {
      get => Settings["FileSavePath"];
      set
      {
        Settings["FileSavePath"] = value;
        OnPropertyChanged("FileSavePath");
      }
    }

    /// <summary>
    /// 전역 키보드 후킹 여부
    /// </summary>
    public static bool GlobalKeyboardHook
    {
      get => bool.Parse(Settings["GlobalKeyboardHook"]);
      set
      {
        Settings["GlobalKeyboardHook"] = value.ToString();
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
      get => int.Parse(Settings["KeyEventDelay"]);
      set
      {
        Settings["KeyEventDelay"] = value.ToString();
        OnPropertyChanged("KeyEventDelay");
      }
    }

    /// <summary>
    /// 평균색 추출 오프셋 (0 이면 Off)
    /// </summary>
    public static int AverageColorProcessingOffset
    {
      get => int.Parse(Settings["AverageColorProcessingOffset"]);
      set
      {
        Settings["AverageColorProcessingOffset"] = value.ToString();
        OnPropertyChanged("AverageColorProcessingOffset");
      }
    }

    /// <summary>
    /// 평균색 테마 적용 여부
    /// </summary>
    public static bool IsAverageColorTheme
    {
      get => bool.Parse(Settings["IsAverageColorTheme"]);
      set
      {
        Settings["IsAverageColorTheme"] = value.ToString();
        if (value)
          MainMediaPlayer.GetAverageColor();
        OnPropertyChanged("IsAverageColorTheme");
      }
    }

    #region Not Save
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
    #endregion
  }
}
