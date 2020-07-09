using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace CMP2.Utility
{
  public class ValueConverter : IValueConverter
  {
    /// <summary>
    /// 값 변환기 메소드 (인터페이스 구현용)
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is TimeSpan)
      { return Converter.TimeSpanStringConverter((TimeSpan)value); }
      return null;
    }

    /// <summary>
    /// 값 변환기 메소드 (인터페이스 구현용)
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  public static class Converter
  {
    /// <summary>
    /// 시간을 문자열로 변환합니다,
    /// </summary>
    /// <param name="value">시간으로 바꿀 TimeSpan</param>
    /// <returns>문자열</returns>
    public static string TimeSpanStringConverter(TimeSpan value)
    {
      if (value == TimeSpan.Zero)
        return "00:00";
      if (value.TotalDays > 1)
        return value.ToString(@"d\:hh\:mm\:ss");
      else if (value.TotalHours > 1)
        return value.ToString(@"h\:mm\:ss");
      else if (value.TotalSeconds > 1)
        return value.ToString(@"mm\:ss");
      else
        //return value.ToString(@"s\:FFF");
        return "00:00";
    }
  }
}
