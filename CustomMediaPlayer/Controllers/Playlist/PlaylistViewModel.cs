using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using CustomMediaPlayer.Core;

namespace CustomMediaPlayer.Controllers.PlayList
{
    public class PlayListViewModel : INotifyPropertyChanged
    {
        private MainWindow MainWindow => (MainWindow)Application.Current.MainWindow;
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propName)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); }

        // 플래이리스트
        public List<MediaInfo> PlayList
        {
            get { return MainWindow.PlayList.Playlist; }
            set { MainWindow.PlayList.Playlist = value; }
        }

        // 테마
        public Brush BorderBrush { get { return ((MainWindow)Application.Current.MainWindow).BorderBrush; } }
    }
}
