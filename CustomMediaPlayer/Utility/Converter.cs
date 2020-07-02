using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CustomMediaPlayer.Utility
{
  public class TimeSpanStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is TimeSpan)
      { return Utility.TimeSpanStringConverter((TimeSpan)value); }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}
