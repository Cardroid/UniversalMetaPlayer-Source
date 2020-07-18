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

namespace CMP2.Controller.WindowButton
{
  /// <summary>
  /// WindowStyleButtonControl.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class WindowStyleButtonControl : UserControl
  {
    public WindowStyleButtonControl()
    {
      InitializeComponent();
      this.Loaded += WindowStyleButtonControl_Loaded;
    }

    private void WindowStyleButtonControl_Loaded(object sender, RoutedEventArgs e)
    {
      this.MinimizeButton.Click += (s, e) =>
      {
        Window parentWindow = Window.GetWindow(Parent);
        if (parentWindow.WindowState == WindowState.Normal)
          parentWindow.WindowState = WindowState.Minimized;
        else
          parentWindow.WindowState = WindowState.Normal;
      };
      this.MaximizeButton.Click += (s, e) =>
      {
        Window parentWindow = Window.GetWindow(Parent);
        if (parentWindow.WindowState == WindowState.Normal)
          parentWindow.WindowState = WindowState.Maximized;
        else
          parentWindow.WindowState = WindowState.Normal;
      };
      this.CloseButton.Click += (s, e) =>
      {
        Window parentWindow = Window.GetWindow(Parent);
        parentWindow.Close();
      };
    }
  }
}
