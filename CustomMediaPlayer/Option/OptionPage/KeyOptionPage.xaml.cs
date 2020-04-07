using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace CustomMediaPlayer.Option.OptionPage
{
    public partial class KeyOptionPage : UserControl
    {
        // 키보드 옵션 클래스 입니다
        public KeyOptionPage()
        {
            InitializeComponent();

            #region 초기 설정
            // 배이스 컬러 동기화
            this.Background = MainWindow.viewModel.BackgroundBrush;
            MainWindow.viewModel.BackgroundColorChanged += (b) => { this.Background = b; };

            //초기화
            KeyHookOptionCheckbox.IsChecked = false;

            // 저장된 정보 로드
            KeyHookOptionCheckbox.IsChecked = OptionSaveLoad.optionValue.KeyHooking.Value;

            // 옵션 내용 설정
            KeyHookOptionCheckbox.Content = "원격 키보드 단축키 설정";
            KeyHookOptionCheckbox.ToolTip = "창이 활성화 되어있지 않아도, 미디어 버튼 컨트롤 버튼을 통해 컨트롤 할 수 있습니다." + "\n\n" +
                                            "관리자권한이 없으면 일부 윈도우에서 키 입력을 감지하지 못할 수 있습니다.";
            #endregion

            #region 이벤트 연결
            KeyHookOptionCheckbox.Click += (s,e)=>
            {
                OptionSaveLoad.optionValue.KeyHooking = KeyHookOptionCheckbox.IsChecked.Value;
                if (OptionSaveLoad.optionValue.KeyHooking.Value)
                    OptionWindow.hooking.Start();
                else
                    OptionWindow.hooking.Stop();
            };
            #endregion
        }
    }
}
