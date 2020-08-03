using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UMP.Core;

namespace UMP.Controller.Option.OptionControl
{
  public partial class KeyboardOption : UserControl
  {
    public KeyboardOption()
    {
      InitializeComponent();

      OptionSync();
      GlobalProperty.PropertyChanged += (s) =>
      {
        OptionSync();
      };

      this.GlobalKeyboardHook.Click += ToggleButton_Click;
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e)
    {

      if (sender is ToggleButton button)
      {
        switch (button.Name)
        {
          case "GlobalKeyboardHook":
            GlobalProperty.GlobalKeyboardHook = !GlobalProperty.GlobalKeyboardHook;
            break;
        }
      }
    }

    /// <summary>
    /// 옵션 - UI 동기화
    /// </summary>
    private void OptionSync()
    {
      this.GlobalKeyboardHook.IsChecked = GlobalProperty.GlobalKeyboardHook;
    }
  }
}
