using System.Windows;
using System.Windows.Controls;

namespace CustomMediaPlayer.Option.OptionPage
{
    public partial class BasicPage : UserControl
    {
        public BasicPage()
        {
            InitializeComponent();

            // 배이스 컬러 동기화
            this.Background = MainWindow.viewModel.BackgroundBrush;
            MainWindow.viewModel.BackgroundColorChanged += (b) => { this.Background = b; };

            #region 옵션 내용 및 툴팁 초기화
            MediaOpeningPlayCheckbox.Content = "미디어 자동 재생";
            MediaOpeningPlayCheckbox.ToolTip = "미디어 파일이 열리면,\n자동으로 재생합니다.";

            LastSongSaveCheckbox.Content = "마지막 미디어 이어서 재생";
            LastSongSaveCheckbox.ToolTip = "프로그램이 종료될 때 로드되어 있는 미디어의 위치를 저장하고,\n" +
                                           "다시 열릴 때 미디어를 이전 재생 위치로 로드합니다.";
            #endregion

            #region 초기화 후 설정 불러오기
            MediaOpeningPlayCheckbox.IsChecked = false;
            LastSongSaveCheckbox.IsChecked = false;

            MediaOpeningPlayCheckbox.IsChecked = OptionSaveLoad.optionValue.MediaOpeningPlay.Value;
            LastSongSaveCheckbox.IsChecked = OptionSaveLoad.optionValue.LastMediaSave.Value;
            #endregion

            #region 기능 구현
            MediaOpeningPlayCheckbox.Click += (s, e) => { OptionSaveLoad.optionValue.MediaOpeningPlay = MediaOpeningPlayCheckbox.IsChecked.Value; };
            LastSongSaveCheckbox.Click += (s, e) => { OptionSaveLoad.optionValue.LastMediaSave = LastSongSaveCheckbox.IsChecked.Value; };
            #endregion
        }
    }
}
