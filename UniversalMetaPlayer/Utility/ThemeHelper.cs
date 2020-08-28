using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;

using MaterialDesignThemes.Wpf;

using UMP.Core;
using UMP.Core.Global;
using UMP.Core.Player;

namespace UMP.Utility
{
  public static class ThemeHelper
  {
    public delegate void UMP_ThemeEventHandler(ThemeProperty e);
    public static event UMP_ThemeEventHandler ThemeChangedEvent;

    static ThemeHelper()
    {
      MainMediaPlayer.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == "MainPlayerInitialized")
          SetAverageColorThemeAsync();
      };
    }

    public static ITheme Theme => new PaletteHelper().GetTheme();
    public static Color PrimaryColor { get; private set; }
    public static Color SecondaryColor { get; private set; }
    private static void ThemeChangedInvoke() => ThemeChangedEvent?.Invoke(new ThemeProperty(PrimaryColor, SecondaryColor, IsDarkMode));

    public static bool IsDarkMode
    {
      get => _IsDarkMode;
      set
      {
        _IsDarkMode = value;
        ChangeDarkMode(_IsDarkMode);
      }
    }
    private static bool _IsDarkMode;

    /// <summary>
    /// 테마 기본 설정
    /// </summary>
    public static void SetDefaultTheme()
    {
      var customTheme = new CustomColorTheme();

      Color primaryColor = GlobalProperty.DefaultValue.GetDefaultValue<Color>(Enums.ValueName.PrimaryColor);
      Color secondaryColor = GlobalProperty.DefaultValue.GetDefaultValue<Color>(Enums.ValueName.SecondaryColor);

      customTheme.PrimaryColor = primaryColor;
      customTheme.SecondaryColor = secondaryColor;
      customTheme.BaseTheme = BaseTheme.Dark;

      var theme = customTheme.GetTheme();

      new PaletteHelper().SetTheme(theme);

      PrimaryColor = primaryColor;
      SecondaryColor = secondaryColor;
      _IsDarkMode = true;

      ThemeChangedInvoke();
    }

    public static void ChangePrimaryColor(Color color)
    {
      var paletteHelper = new PaletteHelper();
      ITheme theme = paletteHelper.GetTheme();

      theme.PrimaryLight = new ColorPair(color.Lighten(), theme.PrimaryLight.ForegroundColor);
      theme.PrimaryMid = new ColorPair(color, theme.PrimaryMid.ForegroundColor);
      theme.PrimaryDark = new ColorPair(color.Darken(), theme.PrimaryDark.ForegroundColor);

      PrimaryColor = color;

      paletteHelper.SetTheme(theme);

      ThemeChangedInvoke();
    }

    public static void ChangeSecondaryColor(Color color)
    {
      var paletteHelper = new PaletteHelper();
      ITheme theme = paletteHelper.GetTheme();

      theme.SecondaryLight = new ColorPair(color.Lighten(), theme.SecondaryLight.ForegroundColor);
      theme.SecondaryMid = new ColorPair(color, theme.SecondaryMid.ForegroundColor);
      theme.SecondaryDark = new ColorPair(color.Darken(), theme.SecondaryDark.ForegroundColor);

      SecondaryColor = color;
      
      paletteHelper.SetTheme(theme);
      
      ThemeChangedInvoke();
    }

    private static void ChangeDarkMode(bool isDark)
    {
      var paletteHelper = new PaletteHelper();
      ITheme theme = paletteHelper.GetTheme();

      IBaseTheme baseTheme = isDark ? (IBaseTheme)new MaterialDesignDarkTheme() : new MaterialDesignLightTheme();
      theme.SetBaseTheme(baseTheme);

      paletteHelper.SetTheme(theme);
      ThemeChangedInvoke();
    }

    /// <summary>
    /// 대표색 추출
    /// </summary>
    public static async void SetAverageColorThemeAsync()
    {
      if (MainMediaPlayer.MediaLoadedCheck)
      {
        if (GlobalProperty.Options.Getter<bool>(Enums.ValueName.IsAverageColorTheme) && MainMediaPlayer.MediaInformation.AlbumImage != null)
        {
          Log log = new Log(typeof(ThemeHelper));
          try
          {
            Color color;
            await Task.Run(() =>
            {
              color = ImageProcessing.GetAverageColor(MainMediaPlayer.MediaInformation.AlbumImage as BitmapSource, GlobalProperty.Options.Getter<int>(Enums.ValueName.AverageColorProcessingOffset));
            });
            Application.Current.MainWindow.BorderBrush = new SolidColorBrush(color);
            ChangePrimaryColor(color.Lighten(2));
            ChangeSecondaryColor(color.Darken(2));
          }
          catch (Exception e)
          {
            log.Error("이미지 평균색 추출에 실패 했습니다.", e);
          }
        }
      }
      else
        SetDefaultTheme();
    }

    public struct ThemeProperty
    {
      public ThemeProperty(Color primaryColor, Color secondaryColor, bool isDarkMode)
      {
        this.PrimaryColor = primaryColor;
        this.SecondaryColor = secondaryColor;
        this.IsDarkMode = isDarkMode;
      }
      public bool IsDarkMode { get; }
      public Color PrimaryColor { get; }
      public Color SecondaryColor { get; }
    }
  }
}
