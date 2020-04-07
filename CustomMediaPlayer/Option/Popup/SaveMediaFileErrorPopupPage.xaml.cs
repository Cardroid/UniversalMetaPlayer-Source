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

namespace CustomMediaPlayer.Option.Popup
{
    public partial class SaveMediaFileErrorPopupPage : UserControl
    {
        public SaveMediaFileErrorPopupPage()
        {
            InitializeComponent();

            // 배이스 컬러 동기화
            this.Background = MainWindow.viewModel.BackgroundBrush;
            MainWindow.viewModel.BackgroundColorChanged += (b) => { this.Background = b; };

            ContentsLabel.Content = "저장된 미디어 정보로 미디어 파일을 불러올 수 없습니다.";
        }
    }
}
