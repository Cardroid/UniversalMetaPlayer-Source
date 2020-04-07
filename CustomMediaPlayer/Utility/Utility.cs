using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using MahApps.Metro.Controls;

namespace CustomMediaPlayer.Utility
{
    public class Utility
    {
        // / 관리자 권한 여부 확인
        public bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (null != identity)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }

        public string TimeSpanStringConverter(TimeSpan value)
        {
            if (value == TimeSpan.Zero)
                return "00:00";
            if (value > TimeSpan.FromDays(1))
                return value.ToString(@"d\:hh\:mm\:ss");
            else if (value > TimeSpan.FromHours(1))
                return value.ToString(@"h\:mm\:ss");
            else if (value > TimeSpan.FromSeconds(1))
                return value.ToString(@"mm\:ss");
            return "00:00";
        }

        public Size MeasureString(string text)
        {
            var formattedText = new FormattedText(text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(((MainWindow)Application.Current.MainWindow).FontFamily, ((MainWindow)Application.Current.MainWindow).FontStyle, ((MainWindow)Application.Current.MainWindow).FontWeight, ((MainWindow)Application.Current.MainWindow).FontStretch),
                ((MainWindow)Application.Current.MainWindow).FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                1);

            return new Size(formattedText.Width + 14, formattedText.Height + 14);
        }

        public string VolumeConverter(int value)
        {
            return value.ToString() +"%";
        }
    }
}
