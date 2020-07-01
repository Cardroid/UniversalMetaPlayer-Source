using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace CustomMediaPlayer.Controllers.PlayList
{
    public partial class PlayListPage : UserControl
    {
        public PlayListPageViewModel ViewModel;

        public PlayListPage()
        {
            InitializeComponent();

            ViewModel = (PlayListPageViewModel)this.DataContext;

            // 배경색 동기화
            this.Background = ((MainWindow)System.Windows.Application.Current.MainWindow).ViewModel.BackgroundBrush;
            ((MainWindow)System.Windows.Application.Current.MainWindow).ViewModel.BackgroundColorChanged += (b) => { this.Background = b; };
            PlayList.Background = ((MainWindow)System.Windows.Application.Current.MainWindow).ViewModel.BackgroundBrush;
            ((MainWindow)System.Windows.Application.Current.MainWindow).ViewModel.BackgroundColorChanged += (b) => { PlayList.Background = b; };
            ThemeManager.IsThemeChanged += (s, e) => { ViewModel.Notify("BorderBrush"); PlayList.Items.Refresh(); };

            #region 플레이리스트 설정
            this.PlayList.MouseLeftButtonDown += (s, e) =>
            { // 빈 부분을 클릭하면 선택 해제
                if (e.GetType() != typeof(ListBoxItem)) PlayList.SelectedItems.Clear();
            };
            this.PlayList.KeyDown += (s, e) => // 플레이리스트 단축키
            {
                switch (e.Key)
                {
                    case Key.Delete:
                        RemoveMediafromPlayList();
                        break;
                }
            };

            // 헤더 설정
            this.ID.Header = "No.";
            this.Title.Header = "제목";
            this.Duration.Header = "길이";

            // 화면 사이즈 조절 시 헤더 가로길이 자동 조절
            this.SizeChanged += (s, e) => { ListViewHeaderWidthRefresh(); };

            // 플레이리스트의 변경사항 발생시 ID갱신, 헤더 가로길이 자동 조절, 리스트 뷰 새로고침
            ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "PlayList")
                {
                    MainWindow.PlayList.Playlist.IDRefresh();
                    this.PlayList.Items.Refresh();
                    ListViewHeaderWidthRefresh();
                }
            };

            // 이름 바꾸기 + 커서 설정
            this.PlayListName.MouseDown += async (s, e) =>
            {
                ViewModel.PlayListName = await DialogManager.ShowInputAsync((MetroWindow)Window.GetWindow(this),
                                                                            "플레이리스트 이름 변경",
                                                                            "변경할 이름을 입력하세요.") ?? ViewModel.PlayListName;
            };
            this.PlayListName.MouseEnter += (s, e) => { this.Cursor = Cursors.IBeam; };
            this.PlayListName.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            #endregion

            // 우클릭 매뉴
            this.PlayListRightClickMenu.Child = new PlayListRightClickMenu();
            this.MouseRightButtonUp += (s, e) => { PlayListRightClickMenu.IsOpen = true; };
            ((PlayListRightClickMenu)PlayListRightClickMenu.Child).AddButton.Click += (s, e) => { ViewModel.AddMediaformFileDialog(); }; // 음악 추가
            ((PlayListRightClickMenu)PlayListRightClickMenu.Child).RemoveButton.Click += (s, e) => { RemoveMediafromPlayList(); }; // 음악 삭제

            this.PlayList.MouseDoubleClick += (s, e) =>
            {
                if (this.PlayList.SelectedItem != null)
                {
                    MainMediaPlayer.mediaPlayer.Stop();
                    if (MainMediaPlayer.NowPlayAudioStream != null)
                        MainMediaPlayer.NowPlayAudioStream.Dispose();
                    try
                    {
                        MainMediaPlayer.NowPlayMedia = new MediaFullInfo((MediaInfo)this.PlayList.SelectedItem);
                    }
                    catch
                    {
                        ((MainWindow)Application.Current.MainWindow).MainPopup.Child = new Option.Popup.MainWindowPopupPage(Option.Popup.PopupContents.FileOpenError);
                        ((MainWindow)Application.Current.MainWindow).MainPopup.IsOpen = true;
                    }
                }
            };
        }

        /// <summary>
        /// 호출되었을 때 선택되어있는 미디어를 리스트에서 제거합니다.
        /// </summary>
        private void RemoveMediafromPlayList()
        {
            // 선택된 항목이 없을때
            if (PlayList.SelectedItems.Count == 0)
                return;

            var CurrentPlayList = MainWindow.PlayList.Playlist;
            for (int i = PlayList.SelectedItems.Count; i > 0; i--)
            {
                CurrentPlayList.RemoveAt(((MediaInfo)PlayList.SelectedItems[i - 1]).ID);
            }
            ViewModel.PlayList = CurrentPlayList;
            ViewModel.Notify("PlayList"); 
            this.PlayList.Items.Refresh();
            ListViewHeaderWidthRefresh();
        }

        /// <summary>
        /// 리스트 뷰 헤더 가로길이 자동수정
        /// </summary>
        public void ListViewHeaderWidthRefresh()
        {
            if (MainWindow.PlayList.Playlist.Count > 0)
            {
                string lastid = ((MediaInfo)this.PlayList.Items[this.PlayList.Items.Count - 1]).ID.ToString();
                string lastduration = Utility.Utility.TimeSpanStringConverter(((MediaInfo)this.PlayList.Items[this.PlayList.Items.Count - 1]).Duration);
#if DEBUG
                Debug.WriteLine(this.Width);
#endif
                this.ID.Width = Utility.Utility.MeasureString(lastid).Width + 10;
                this.Duration.Width = Utility.Utility.MeasureString(lastduration).Width + 14;
                this.Title.Width = this.Width - this.ID.Width - this.Duration.Width - 14;
            }
        }
    }
}
