using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using CMP2.Core;

namespace CMP2.Utility
{
  public class TimeSpanStringConverter : IValueConverter
  {
    /// <summary>
    /// 값 변환기 메소드 (인터페이스 구현용)
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Converter.TimeSpanStringConverter((TimeSpan)value);
    }

    /// <summary>
    /// 값 변환기 메소드 (인터페이스 구현용)
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  public class StarWidthConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      ListView listview = value as ListView;
      double width = listview.Width;
      GridView gv = listview.View as GridView;
      for (int i = 0; i < gv.Columns.Count; i++)
      {
        if (!double.IsNaN(gv.Columns[i].Width))
          width -= gv.Columns[i].Width;
      }
      return width - 5;// this is to take care of margin/padding
    }

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
