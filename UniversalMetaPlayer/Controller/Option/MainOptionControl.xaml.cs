using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UMP.Controller.Option.OptionControl;

namespace UMP.Controller.Option
{
  /// <summary>
  /// MainOptionControl.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class MainOptionControl : UserControl
  {
    private MainOptionControlViewModel ViewModel { get; }

    public MainOptionControl()
    {
      InitializeComponent();

      ViewModel = (MainOptionControlViewModel)this.DataContext;

      this.Basic.PreviewMouseLeftButtonDown += MainOptionControl_MouseDown;
      this.Keyboard.PreviewMouseLeftButtonDown += MainOptionControl_MouseDown;
      this.Theme.PreviewMouseLeftButtonDown += MainOptionControl_MouseDown;
    }

    private void MainOptionControl_MouseDown(object sender, MouseButtonEventArgs e)
    {
      switch (((TreeViewItem)sender).Name)
      {
        case "Basic":
          ViewModel.OptionPanel = new BasicOption();
          break;
        case "Keyboard":
          ViewModel.OptionPanel = new KeyboardOption();
          break;
        case "Theme":
          ViewModel.OptionPanel = new ThemeOption();
          break;
      }
    }
  }
}
