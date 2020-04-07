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

namespace CustomMediaPlayer.Option.OptionPage
{
    public partial class ThemeOptionPage : UserControl
    {
        public ThemeOptionPage()
        {
            InitializeComponent();

            // 배이스 컬러 동기화
            this.Background = MainWindow.viewModel.BackgroundBrush;
            MainWindow.viewModel.BackgroundColorChanged += (b) => { this.Background = b; };

            // 옵션 내용 설정
            AccentColor.Header = "전경색 설정";
            BaseColor.Header = "배경색 설정";
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            if (button.Tag.ToString().StartsWith("Base"))
            { OptionSaveLoad.optionValue.BaseColor = button.Tag.ToString();

                string colorstring = "Black";
                if (button.Tag.ToString() == "BaseDark")
                    colorstring = @"#FF000000";
                else if (button.Tag.ToString() == "BaseLight")
                    colorstring = @"#FFFFFFFF";
                // 메인 윈도우 배이스 컬러 변경
                MainWindow.viewModel.BackgroundBrush = (Brush)(new BrushConverter().ConvertFromString(colorstring));
                //MainWindow.viewModel.BackgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorstring));
            }
            else
            { OptionSaveLoad.optionValue.AccentColor = button.Tag.ToString(); }

            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent(OptionSaveLoad.optionValue.AccentColor),
                ThemeManager.GetAppTheme(OptionSaveLoad.optionValue.BaseColor));
        }
    }
}
