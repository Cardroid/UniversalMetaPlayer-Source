using System;
using System.Collections.Generic;
using System.Text;

namespace CMP2.Utility
{
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
      if (value > TimeSpan.FromDays(1))
        return value.ToString(@"d\:hh\:mm\:ss");
      else if (value > TimeSpan.FromHours(1))
        return value.ToString(@"h\:mm\:ss");
      else if (value > TimeSpan.FromSeconds(1))
        return value.ToString(@"mm\:ss");
      //return value.ToString(@"s\:FFF");
      return "00:00";
    }
  }
}
