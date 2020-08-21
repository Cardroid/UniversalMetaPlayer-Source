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

using UMP.Core;
using UMP.Utility;

namespace UMP.Controller.WindowButton
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

      this.ButtonPanelBorder.BorderBrush = new SolidColorBrush(ThemeHelper.PrimaryColor);
      ThemeHelper.ThemeChangedEvent += (e) =>
      {
        this.ButtonPanelBorder.BorderBrush = new SolidColorBrush(e.PrimaryColor);
      };
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
      this.ResizeButton.Click += (_, e) =>
      {
        Window parentWindow = Window.GetWindow(Parent);
        parentWindow.Width = 800;
        parentWindow.Height = 450;
        parentWindow.Left = SystemParameters.WorkArea.Width / 2 - parentWindow.Width / 2;
        parentWindow.Top = SystemParameters.WorkArea.Height / 2 - parentWindow.Height / 2;
      };
      this.MaximizeButton.Click += (s, e) =>
      {
        Window parentWindow = Window.GetWindow(Parent);
        if (parentWindow.WindowState == WindowState.Normal)
        {
          parentWindow.WindowState = WindowState.Maximized;
          MaximizeButton.ToolTip = "이전 크기로 복원";
        }
        else
        {
          parentWindow.WindowState = WindowState.Normal;
          MaximizeButton.ToolTip = "최대화";
        }
      };
      this.CloseButton.Click += (s, e) =>
      {
        Window parentWindow = Window.GetWindow(Parent);
        parentWindow.Close();
      };
    }
  }
}
