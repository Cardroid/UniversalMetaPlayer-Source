using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

      this.KeyEventDelayOffsetTextBox.PreviewKeyDown += KeyEventDelayOffsetTextBox_PreviewKeyDown;
      this.KeyEventDelayOffsetTextBox.Text = GlobalProperty.Options.KeyEventDelay.ToString();
    }

    private async void KeyEventDelayOffsetTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        this.KeyEventDelayOffsetTextBox.Focusable = false;
        this.KeyEventDelayOffsetTextBox.IsReadOnlyCaretVisible = false;
        this.KeyEventDelayOffsetTextBox.IsReadOnly = true;
        if (int.TryParse(this.KeyEventDelayOffsetTextBox.Text, out int result))
          result = Math.Clamp(result, 10, 201);
        else
          result = GlobalProperty.Options.DefaultValue.DefaultKeyEventDelay;
        GlobalProperty.Options.KeyEventDelay = result;
        this.KeyEventDelayOffsetTextBox.Text = result.ToString();
        await Task.Delay(300);
        this.KeyEventDelayOffsetTextBox.IsReadOnlyCaretVisible = true;
        this.KeyEventDelayOffsetTextBox.IsReadOnly = false;
        this.KeyEventDelayOffsetTextBox.Focusable = true;
      }
    }
  }
}
