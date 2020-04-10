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
using MahApps.Metro;

namespace CustomMediaPlayer.Controllers.PlayList
{
    public partial class PlayListRightClickMenu : UserControl
    {
        private PlayListPage PlayListPage => (PlayListPage)((MainWindow)Application.Current.MainWindow).PlayListwindow.FrameContent.Content;

        public PlayListRightClickMenu()
        {
            InitializeComponent();

            // 배경색 동기화
            this.Background = ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundBrush;
            ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundColorChanged += (b) => { this.Background = b; };
            
            // 전경색 동기화
            this.BorderBrush = ThemeManager.DetectAppStyle().Item2.Resources["AccentColorBrush"] as SolidColorBrush;

            AddButton.Content = "추가";
            AddButton.ToolTip = "플래이리스트에 미디어를 추가 합니다.";
            RemoveButton.Content = "삭제";
            RemoveButton.ToolTip = "플래이리스트에서 선택된 미디어를 삭제 합니다.";

            // 선택한 아이템(음악)이 없으면 삭제 버튼 비활성화
            this.Loaded += (s, e) => { RemoveButton.IsEnabled = PlayListPage.PlayList.SelectedItems.Count == 0 ? false : true; };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PlayListPage.PlayListRightClickMenu.IsOpen = false;
        }
    }
}
