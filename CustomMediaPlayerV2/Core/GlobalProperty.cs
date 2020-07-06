using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ControlzEx.Theming;

namespace CMP2.Core
{
  public interface IGlobalProperty
  {
    static IGlobalProperty()
    {
      MainFontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Font/#NanumGothic");
      MainTheme = ThemeManager.Current.GetTheme("Dark", "Green");
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
    public static Theme MainTheme { get; set; }
    /// <summary>
    /// 로고 이미지
    /// </summary>
    public static ImageSource LogoImage { get; }
    /// <summary>
    /// 로고 음표 이미지
    /// </summary>
    public static ImageSource LogoNoteImage { get; }
  }
}
