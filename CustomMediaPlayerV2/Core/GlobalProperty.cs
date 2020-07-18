using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using MaterialDesignThemes.Wpf;

using Newtonsoft.Json;

namespace CMP2.Core
{
  /// <summary>
  /// 프로그램 전체 설정
  /// </summary>
  [JsonObject]
  public static class GlobalProperty
  {
    static GlobalProperty()
    {
      MainFontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Font/#NanumGothic");
      LogoImage = new BitmapImage(new Uri("pack://application:,,,/CustomMediaPlayer;component/Resources/IconCustomMusicPlayer.png", UriKind.RelativeOrAbsolute));
      LogoNoteImage = new BitmapImage(new Uri("pack://application:,,,/CustomMediaPlayer;component/Resources/IconnoteCustomMusicPlayer.png", UriKind.RelativeOrAbsolute));
    }

    /// <summary>
    /// 기본 설정으로 되돌립니다.
    /// </summary>
    public static void SetDefault()
    {
      // 테마 기본 설정
      var theme = new CustomColorTheme();
      theme.BaseTheme = BaseTheme.Dark;
      theme.PrimaryColor = Colors.LightGreen;
      theme.SecondaryColor = Colors.DarkSeaGreen;
      Theme = theme.GetTheme();
    }

    #region 테마
    /// <summary>
    /// 테마
    /// </summary>
    [JsonProperty]
    public static ITheme Theme
    {
      get => _paletteHelper.GetTheme();
      set => _paletteHelper.SetTheme(value);
    }
    #endregion

    #region Not Save
    /// <summary>
    /// 폰트
    /// </summary>
    [JsonIgnore]
    public static FontFamily MainFontFamily { get; }
    /// <summary>
    /// 로고 이미지
    /// </summary>
    [JsonIgnore]
    public static ImageSource LogoImage { get; }
    /// <summary>
    /// 로고 음표 이미지
    /// </summary>
    [JsonIgnore]
    public static ImageSource LogoNoteImage { get; }
    #endregion

    private static readonly PaletteHelper _paletteHelper = new PaletteHelper();
  }
}
