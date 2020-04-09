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
using CustomMediaPlayer.Option.OptionPage;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace CustomMediaPlayer
{
    public partial class OptionWindow : MetroWindow
    {
        public static Hooking hooking = new Hooking(); // 후킹 설정
        public MainWindow mainWindow => (MainWindow)Application.Current.MainWindow;
        public string CurrentPage { get; private set; }

        public OptionWindow()
        {
            InitializeComponent();

            // 배이스 컬러 동기화
            this.Background = ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundBrush;
            ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundColorChanged += (b) => { this.Background = b; };

            // 포커스 설정
            this.Focusable = false;

            // 옵션 타이틀 설정
            AboutOptionPageButton.ToolTip = "프로그램 정보";
            BasicOptionPageButton.ToolTip = "기본 설정";
            KeyOptionPageButton.ToolTip = "키보드 설정";
            ThemeOptionPageButton.ToolTip = "테마 설정";

            AboutOptionPageButton.Tag = ButtonDistinction.About;
            BasicOptionPageButton.Tag = ButtonDistinction.BasicOption;
            KeyOptionPageButton.Tag = ButtonDistinction.KeyOption;
            ThemeOptionPageButton.Tag = ButtonDistinction.ThemeOption;

            // 초기 페이지 설정
            Contents.Header = "기본 설정";
            Contents.Content = new BasicPage();
            CurrentPage = AboutOptionPageButton.ToolTip.ToString();

            // 창 위치 설정
            this.Loaded += (s, e) =>
            {
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

        private void OptionPageChangeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            switch (button.Tag)
            {
                case ButtonDistinction.About:
                    Contents.Header = button.ToolTip;
                    Contents.Content = new AboutPage();
                    break;
                case ButtonDistinction.BasicOption:
                    Contents.Header = button.ToolTip;
                    Contents.Content = new BasicPage();
                    break;
                case ButtonDistinction.KeyOption:
                    Contents.Header = button.ToolTip;
                    Contents.Content = new KeyOptionPage();
                    break;
                case ButtonDistinction.ThemeOption:
                    Contents.Header = button.ToolTip;
                    Contents.Content = new ThemeOptionPage();
                    break;
            }
        }

        private enum ButtonDistinction
        {
            About,
            BasicOption,
            KeyOption,
            ThemeOption
        }
    }
}