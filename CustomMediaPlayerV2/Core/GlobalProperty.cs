using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MaterialDesignThemes.Wpf;

namespace CMP2.Core
{
  public static class GlobalProperty
  {
    static GlobalProperty()
    {
      MainFontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Font/#NanumGothic");
      LogoImage = new BitmapImage(new Uri("pack://application:,,,/CustomMediaPlayer;component/Resources/IconCustomMusicPlayer.png", UriKind.RelativeOrAbsolute));
      LogoNoteImage = new BitmapImage(new Uri("pack://application:,,,/CustomMediaPlayer;component/Resources/IconnoteCustomMusicPlayer.png", UriKind.RelativeOrAbsolute));
    }
    /// <summary>
    /// 폰트
    /// </summary>
    public static FontFamily MainFontFamily { get; }
    /// <summary>
    /// 테마
    /// </summary>
    public static ITheme Theme
    {
      get => _Theme;
      set
      {
        _Theme = value;
        _paletteHelper.SetTheme(_Theme);
      }
    }
    public static bool IsDark 
    {
      get => _IsDark;
      set
      {
        if (_IsDark != value)
          ToggleBaseColour(_IsDark);
      }
    }
    private static void ToggleBaseColour(bool isDark)
    {
      ITheme theme = _paletteHelper.GetTheme();
      IBaseTheme baseTheme = isDark ? new MaterialDesignDarkTheme() : (IBaseTheme)new MaterialDesignLightTheme();
      _IsDark = isDark;
      theme.SetBaseTheme(baseTheme);
      _paletteHelper.SetTheme(theme);
    }
    /// <summary>
    /// 로고 이미지
    /// </summary>
    public static ImageSource LogoImage { get; }
    /// <summary>
    /// 로고 음표 이미지
    /// </summary>
    public static ImageSource LogoNoteImage { get; }

    private static readonly PaletteHelper _paletteHelper = new PaletteHelper();
    private static ITheme _Theme;
    private static bool _IsDark;
  }
}
