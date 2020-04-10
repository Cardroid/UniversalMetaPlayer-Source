using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CustomMediaPlayer.Option.OptionPage.ViewModel
{
    public class AboutPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propName)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); }

        public ImageSource LogoImage { get { return MainWindow.LogoImage; } }

        // 버전 표시
        public string CoreVersion { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
        public string FileVersion { get { return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString(); } }
    }
}
