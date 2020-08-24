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
using System.Windows.Shapes;
using UMP.Core.Global;
using UMP.Utility;

namespace UMP.Controller.WindowHelper
{
  /// <summary>
  /// UserWindow.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class UserWindow : Window
  {
    private UserWindowViewModel ViewModel { get; }
    public UserWindow(UserControl userControl, string title)
    {
      InitializeComponent();
      ViewModel = (UserWindowViewModel)this.DataContext;
      this.Title = title;
      ViewModel.UserControl = userControl;

      this.BorderBrush = new SolidColorBrush(ThemeHelper.PrimaryColor);
      ThemeHelper.ThemeChangedEvent += (e) => this.BorderBrush = new SolidColorBrush(e.PrimaryColor);

      this.PreviewKeyDown += (_, e) =>
      {
        if (GlobalKeyDownEvent.IsEnabled && GlobalProperty.Options.HotKey.IsEnabled && GlobalProperty.Options.HotKey.ContainsKey(e.Key))
        {
          e.Handled = true;
          GlobalKeyDownEvent.Invoke(e);
        }
      };

      GlobalMessageEvent.MessageCloseEvent += () =>
      {
        Dispatcher.Invoke(new Action(() =>
        {
          this.GlobalMessageBar.IsActive = false;
        }));
      };

      GlobalMessageEvent.MessageEvent += (msg) =>
      {
        Dispatcher.Invoke(new Action(() =>
        {
          this.GlobalMessageBar.IsActive = true;
          this.GlobalMessage.Content = msg;
        }));
      };

      this.GlobalMessageBar.MouseLeftButtonDown += (_, e) =>
      {
        if (this.GlobalMessageBar.IsActive)
          this.GlobalMessageBar.IsActive = false;
      };
    }
  }
}
