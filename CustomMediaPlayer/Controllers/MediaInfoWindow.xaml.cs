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
using System.Windows.Shapes;
using CustomMediaPlayer.Utility;
using MahApps.Metro.Controls;
using static CustomMediaPlayer.MainMediaPlayer;

namespace CustomMediaPlayer.Controllers
{
    public partial class MediaInfoWindow : MetroWindow
    {
        public MediaInfoWindow()
        {
            InitializeComponent();

            //Reset();

            // 배이스 컬러 동기화
            this.Background = MainWindow.viewModel.BackgroundBrush;
            MainWindow.viewModel.BackgroundColorChanged += (b) => { this.Background = b; };

            // 헤더 및 설명 초기화
            ExplanationGroup.Header = "기본정보";

            // 커서 설정
            MediaImage.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            MediaImage.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };

            // 이미지 클릭시 자세한 정보
            MediaImage.ToolTip = "Click for more information\n자세한 정보를 보려면 클릭";

            // 자세한 정보 보기 = 인터넷 검색
            MediaImage.MouseDown += (s, e) => 
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    Process.Start(@"https://www.google.com/search?q=" + nowPlayInfo.Title);
            };
        }

        public void Reset()
        {
            #region 정보 라벨 초기화

            MediaImage.Source = nowPlayInfo.AlbumImage;

            SongTitleLabel.Content  = "곡 제목 : "       + nowPlayInfo.Title;
            AlbumTitleLabel.Content = "엘범 제목 : "     + nowPlayInfo.AlbumTitle;
            ArtistNameLabel.Content = "아티스트 이름 : " + nowPlayInfo.ArtistName;
            #endregion
        }
    }
}
