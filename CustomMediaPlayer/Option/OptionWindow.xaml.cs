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
using System.Windows.Shapes;
using CustomMediaPlayer.Option;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace CustomMediaPlayer
{
    public partial class OptionWindow : MetroWindow
    {
        public static Hooking hooking = new Hooking(); // 후킹 설정
        public MainWindow mainWindow => (MainWindow)Application.Current.MainWindow;

        public OptionWindow()
        {
            InitializeComponent();

            // 메뉴 선택시 닫힘 설정
            HamburgerMenuControl.ItemClick += (s, e) => { HamburgerMenuControl.IsPaneOpen = false; };
            HamburgerMenuControl.OptionsItemClick += (s, e) => { HamburgerMenuControl.IsPaneOpen = false; };

            // 초기 화면 설정 (열리면 기본 설정화면)
            HamburgerMenuControl.SelectedItem = BasicOptionPanel;

            // 옵션 타이틀 설정
            BasicOptionPanel.Label = "기본 설정";
            KeyOptionPanel.Label = "키보드 설정";
            ThemePanel.Label = "테마 설정";
            AboutPagePanel.Label = "정보";

            // 창 위치 설정
            this.Loaded += (s, e) => {
                
                // 메인 윈도우의 위치, 크기 가져오기
                double MainLeft = mainWindow.Left;
                double MainWidth = mainWindow.Width;
                double MainTop = mainWindow.Top;
                //double MainHeight = ((MainWindow)Application.Current.MainWindow).Height;

                double PosOver = SystemParameters.WorkArea.Width - (MainLeft + MainWidth + this.Width);
                if (PosOver < 0) // 오른쪽 자리가 부족한 경우
                    this.Left = MainLeft - this.Width;
                else // 아닌 경우
                    this.Left = MainLeft + MainWidth;
                if (this.Left < 0)// 왼쪽 자리가 부족한 경우
                    this.Left = MainLeft + MainWidth / 2 - this.Width / 2;
                this.Top = MainTop;
            };
        }
    }
}