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
using CustomMediaPlayer.Option.OptionPage.ViewModel;

namespace CustomMediaPlayer.Option.OptionPage
{
    public partial class ThemeOptionPage : UserControl
    {
        public ThemeOptionPageViewModel ViewModel = new ThemeOptionPageViewModel();
        public ThemeOptionPage()
        {
            InitializeComponent();

            this.DataContext = ViewModel;

            // 배경색 동기화
            this.Background = ((MainWindow)Application.Current.MainWindow).viewModel.BackgroundBrush;
            ((MainWindow)Application.Current.MainWindow).viewModel.BackgroundColorChanged += (b) => { this.Background = b; };

            // 옵션 내용 설정
            AccentColor.Header = "전경색 설정";
            BaseColor.Header = "배경색 설정";
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            string accentColorstring = ThemeManager.Accents.ToString();
            string baseColorstring = ThemeManager.AppThemes.ToString();
            string backgroundcolorstring = @"#FF000000";

            if (button.Tag.ToString().StartsWith("Base"))
            {
                if (button.Tag.ToString() == "BaseDark")
                    backgroundcolorstring = @"#FF000000";
                else if (button.Tag.ToString() == "BaseLight")
                    backgroundcolorstring = @"#FFFFFFFF";
                baseColorstring = button.Tag.ToString();

                // 메인 윈도우 배경색 변경
                ((MainWindow)Application.Current.MainWindow).viewModel.BackgroundBrush = (Brush)(new BrushConverter().ConvertFromString(backgroundcolorstring));
            }
            else { accentColorstring = button.Tag.ToString(); }

            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent(accentColorstring),
                ThemeManager.GetAppTheme(baseColorstring));
        }
    }
}
