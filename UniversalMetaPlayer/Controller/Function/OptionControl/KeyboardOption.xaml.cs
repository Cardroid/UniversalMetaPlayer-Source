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

namespace UMP.Controller.Function.OptionControl
{
  public partial class KeyboardOption : FunctionModelControl
  {
    public KeyboardOption() : base("옵션 - 키보드")
    {
      InitializeComponent();

      GlobalProperty.PropertyChanged += (e) =>
      {
        if (e == "KeyEventDelay")
          this.KeyEventDelayOffsetTextBox.Text = GlobalProperty.Options.KeyEventDelay.ToString();
      };

      this.PreviewMouseDown += (_, e) => { KeyEventDelayOffsetTextBox_Apply(); };

      this.KeyEventDelayOffsetTextBox.GotKeyboardFocus += (_, e) => { this.KeyEventDelayOffsetTextBox.Text = ""; };
      this.KeyEventDelayOffsetTextBox.PreviewKeyDown += (_, e) => { if (e.Key == Key.Enter) Keyboard.ClearFocus(); };
      this.KeyEventDelayOffsetTextBox.LostKeyboardFocus += (_, e) => { KeyEventDelayOffsetTextBox_Apply(); };
      this.KeyEventDelayOffsetTextBox.Text = GlobalProperty.Options.KeyEventDelay.ToString();
    }

    private void KeyEventDelayOffsetTextBox_Apply()
    {
      if (int.TryParse(this.KeyEventDelayOffsetTextBox.Text, out int result))
        result = Math.Clamp(result, 10, 201);
      else
        result = GlobalProperty.Options.DefaultValue.DefaultKeyEventDelay;
      GlobalProperty.Options.KeyEventDelay = result;
    }
  }
}
