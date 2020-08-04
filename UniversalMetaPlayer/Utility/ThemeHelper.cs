using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;

using MaterialDesignThemes.Wpf;

using UMP.Core;

namespace UMP.Utility
{
  public static class ThemeHelper
  {
    private static readonly PaletteHelper paletteHelper = new PaletteHelper();
    public static event UMP_ThemeEventHandler ThemeChangedEvent;

    static ThemeHelper()
    {
      MainMediaPlayer.PropertyChangedEvent += (e) =>
      {
        if (e == "MediaInformation")
          SetAverageColorTheme();
      };
    }

    public static ITheme Theme => paletteHelper.GetTheme();
    public static Color PrimaryColor { get; private set; }
    public static Color SecondaryColor { get; private set; }
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
      var theme = new CustomColorTheme();

      Color primaryColor = Colors.LightGreen;
      Color secondaryColor = Colors.DarkSeaGreen;

      theme.PrimaryColor = primaryColor;
      theme.SecondaryColor = secondaryColor;
      theme.BaseTheme = BaseTheme.Dark;

      paletteHelper.SetTheme(theme.GetTheme());

      PrimaryColor = primaryColor;
      SecondaryColor = secondaryColor;
      _IsDarkMode = true;

      ThemeChangedEvent?.Invoke(new ThemeProperty(PrimaryColor, SecondaryColor, IsDarkMode));
    }

    public static void ChangePrimaryColor(Color color)
    {
      ITheme theme = paletteHelper.GetTheme();

      theme.PrimaryLight = new ColorPair(color.Lighten(), theme.PrimaryLight.ForegroundColor);
      theme.PrimaryMid = new ColorPair(color, theme.PrimaryMid.ForegroundColor);
      theme.PrimaryDark = new ColorPair(color.Darken(), theme.PrimaryDark.ForegroundColor);

      paletteHelper.SetTheme(theme);
      PrimaryColor = color;
      ThemeChangedEvent?.Invoke(new ThemeProperty(PrimaryColor, SecondaryColor, IsDarkMode));
    }

    public static void ChangeSecondaryColor(Color color)
    {
      ITheme theme = paletteHelper.GetTheme();

      theme.SecondaryLight = new ColorPair(color.Lighten(), theme.SecondaryLight.ForegroundColor);
      theme.SecondaryMid = new ColorPair(color, theme.SecondaryMid.ForegroundColor);
      theme.SecondaryDark = new ColorPair(color.Darken(), theme.SecondaryDark.ForegroundColor);

      paletteHelper.SetTheme(theme);
      SecondaryColor = color;
      ThemeChangedEvent?.Invoke(new ThemeProperty(PrimaryColor, SecondaryColor, IsDarkMode));
    }

    private static void ChangeDarkMode(bool isDark)
    {
      ITheme theme = paletteHelper.GetTheme();

      IBaseTheme baseTheme = isDark ? new MaterialDesignDarkTheme() : (IBaseTheme)new MaterialDesignLightTheme();
      theme.SetBaseTheme(baseTheme);

      paletteHelper.SetTheme(theme);
      ThemeChangedEvent?.Invoke(new ThemeProperty(PrimaryColor, SecondaryColor, IsDarkMode));
    }


    /// <summary>
    /// 대표색 추출
    /// </summary>
    public static void SetAverageColorTheme()
    {
      if (MainMediaPlayer.MediaLoadedCheck)
      {
        if (GlobalProperty.IsAverageColorTheme && MainMediaPlayer.MediaInformation.AlbumImage != null)
        {
          var color = ImageProcessing.GetAverageColor(MainMediaPlayer.MediaInformation.AlbumImage as BitmapSource, GlobalProperty.AverageColorProcessingOffset);
          Application.Current.MainWindow.BorderBrush = new SolidColorBrush(color);
          ChangePrimaryColor(color.Lighten(3));
          ChangeSecondaryColor(color.Darken(3));
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
      public Color PrimaryColor { get; }
      public Color SecondaryColor { get; }
      public bool IsDarkMode { get; }
    }
  }
}
