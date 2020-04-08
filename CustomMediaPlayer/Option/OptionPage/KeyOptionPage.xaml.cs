using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using CustomMediaPlayer.Option.OptionPage.ViewModel;

namespace CustomMediaPlayer.Option.OptionPage
{
    public partial class KeyOptionPage : UserControl
    {
        public KeyOptionPageViewModel ViewModel = new KeyOptionPageViewModel();
        // 키보드 옵션 클래스 입니다
        public KeyOptionPage()
        {
            InitializeComponent();

            this.DataContext = ViewModel;

            #region 초기 설정
            // 배경색 동기화
            this.Background = ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundBrush;
            ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundColorChanged += (b) => { this.Background = b; };

            // 옵션 내용 설정
            KeyHookOptionCheckbox.Content = "원격 키보드 단축키 설정";
            KeyHookOptionCheckbox.ToolTip = "창이 활성화 되어있지 않아도, 미디어 컨트롤 버튼을 통해 컨트롤 할 수 있습니다." + "\n\n" +
                                            "관리자 권한이 없으면 일부 윈도우에서 키 입력을 감지하지 못할 수 있습니다.";
            #endregion
        }
    }
}
