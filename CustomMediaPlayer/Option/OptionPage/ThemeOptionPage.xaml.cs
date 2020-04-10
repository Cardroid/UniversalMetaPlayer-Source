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
        public ThemeOptionPage()
        {
            InitializeComponent();

            // 배경색 동기화
            this.Background = ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundBrush;
            ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundColorChanged += (b) => { this.Background = b; };

            // 모든 전경색을 지원
            foreach (var color in ThemeManager.Accents)
            {
                Button button = new ThemeButtonTemplate(color);
                button.Click += ColorButton_Click;
                AccentColorGroup.Children.Add(button);
            }

            // 옵션 내용 설정
            AccentColor.Header = "전경색 설정";
            BaseColor.Header = "배경색 설정";
        }

        private class ThemeButtonTemplate : Button
        {
            public ThemeButtonTemplate(Accent color)
            {
                this.HorizontalAlignment = HorizontalAlignment.Center;
                this.VerticalAlignment = VerticalAlignment.Center;
                this.Style = Application.Current.FindResource("SquareButtonStyle") as Style;
                this.Content = "        ";
                this.Tag = color.Name;
                this.ToolTip = color.Name;
                this.Background = new SolidColorBrush((Color)color.Resources["AccentColor"]);
                this.Margin = new Thickness(2.5);
            }
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var CurrentTheme = ThemeManager.DetectAppStyle();

            Accent accentColorstring = CurrentTheme.Item2;
            AppTheme baseColorstring = CurrentTheme.Item1;
            string backgroundcolorstring = @"#FF000000";

            if (button.Tag.ToString().StartsWith("Base"))
            {
                if (button.Tag.ToString() == "BaseDark")
                    backgroundcolorstring = @"#FF000000";
                else if (button.Tag.ToString() == "BaseLight")
                    backgroundcolorstring = @"#FFFFFFFF";
                baseColorstring = ThemeManager.GetAppTheme(button.Tag.ToString());

                // 메인 윈도우 배경색 변경
                ((MainWindow)Application.Current.MainWindow).ViewModel.BackgroundBrush = new BrushConverter().ConvertFromString(backgroundcolorstring) as Brush;
            }
            else { accentColorstring = ThemeManager.GetAccent(button.Tag.ToString()); }

            ThemeManager.ChangeAppStyle(Application.Current, accentColorstring, baseColorstring);
        }
    }
}
