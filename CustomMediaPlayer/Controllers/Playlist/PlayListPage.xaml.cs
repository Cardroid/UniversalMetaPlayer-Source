using System;
using System.Collections.Generic;
using System.IO;
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
using CustomMediaPlayer.Core;
using MahApps.Metro;
using Microsoft.Win32;

namespace CustomMediaPlayer.Controllers.PlayList
{
    public partial class PlayListPage : UserControl
    {
        private MainWindow MainWindow => (MainWindow)Application.Current.MainWindow;
        public PlayListViewModel ViewModel = new PlayListViewModel();

        public PlayListPage()
        {
            InitializeComponent();

            this.DataContext = ViewModel;

            // 배경색 동기화
            this.Background = ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundBrush;
            ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundColorChanged += (b) => { this.Background = b; };
            PlayList.Background = ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundBrush;
            ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundColorChanged += (b) => { PlayList.Background = b; };
            ThemeManager.IsThemeChanged += (s, e) => { ViewModel.Notify("BorderBrush"); PlayList.Items.Refresh(); };

            // 플레이리스트 설정
            PlayList.MouseLeftButtonDown += (s, e) => { // 빈 부분을 클릭하면 선택 해제
                if (e.GetType() != typeof(ListBoxItem)) PlayList.SelectedItems.Clear();
            };
            PlayList.KeyDown += (s, e) => 
            {
                switch (e.Key)
                {
                    case Key.Delete:
                        RemoveMediafromPlayList();
                        break;
                }
            };

            // 플래이리스트 헤더 추가
            Title.Header = "제목";

            // 우클릭 매뉴
            PlayListRightClickMenu.Child = new PlayListRightClickMenu();
            this.MouseRightButtonUp += (s, e) => { PlayListRightClickMenu.IsOpen = true; };
            ((PlayListRightClickMenu)PlayListRightClickMenu.Child).AddButton.Click += (s, e) => { AddMediaformFiles(); }; // 음악 추가
            ((PlayListRightClickMenu)PlayListRightClickMenu.Child).RemoveButton.Click += (s, e) => { RemoveMediafromPlayList(); }; // 음악 삭제
        }

        /// <summary>
        /// 파일 다이얼로그를 열어 파일을 플래이리스트에 추가합니다.
        /// </summary>
        private void AddMediaformFiles()
        {
            var utility = new Utility.Utility();
            var filenames = utility.OpenDialog();
            var CurrentPlayList = MainWindow.PlayList.Playlist;
            if (filenames != null)
            {
                foreach (string filename in filenames)
                {
                    CurrentPlayList.Add(new MediaInfo(new FileInfo(filename)));
                }
                PlayList.ItemsSource = CurrentPlayList;
                PlayList.Items.Refresh();
            }
        }

        private void RemoveMediafromPlayList()
        {
            // 선택된 항목이 없을때
            if (PlayList.SelectedItems.Count == 0)
                return;

            var CurrentPlayList = MainWindow.PlayList.Playlist;
            foreach(MediaInfo file in PlayList.SelectedItems)
            {
                CurrentPlayList.Remove(file);
            }
            PlayList.ItemsSource = CurrentPlayList;
            PlayList.Items.Refresh();
        }
    }
}
