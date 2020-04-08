using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomMediaPlayer.Controllers.Playlist
{
    public partial class PlaylistCore : UserControl
    {
        public PlaylistCore()
        {
            InitializeComponent();

            // 배경색 동기화
            this.Background = ((MainWindow)Application.Current.MainWindow).viewModel.BackgroundBrush;
            ((MainWindow)Application.Current.MainWindow).viewModel.BackgroundColorChanged += (b) => { this.Background = b; };
        }
    }
}
