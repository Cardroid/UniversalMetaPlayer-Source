using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace CustomMediaPlayer.Option
{
    public class OptionCore : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // first value is selected menu item, second value is selected option item
            if (values != null && values.Length > 1)
            {
                return values[0] ?? values[1];
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => Binding.DoNothing).ToArray();
        }
    }
}
