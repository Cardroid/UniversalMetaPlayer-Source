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

namespace UMP.Controller.Feature.OptionControl
{
  public partial class KeyboardOption : FeatureModelControl
  {
    public KeyboardOption() : base("옵션 - 키보드")
    {
      InitializeComponent();

      this.PreviewMouseDown += (_, e) => { KeyEventDelayOffsetTextBox_Apply(); };

      this.KeyEventDelayOffsetTextBox.PreviewKeyDown += (_, e) => { if (e.Key == Key.Enter) KeyEventDelayOffsetTextBox_Apply(); };
      this.KeyEventDelayOffsetTextBox.Text = GlobalProperty.Options.KeyEventDelay.ToString();
    }

    private void KeyEventDelayOffsetTextBox_Apply()
    {
      if (int.TryParse(this.KeyEventDelayOffsetTextBox.Text, out int result))
        result = Math.Clamp(result, 10, 201);
      else
        result = GlobalProperty.Options.DefaultValue.DefaultKeyEventDelay;
      GlobalProperty.Options.KeyEventDelay = result;
      this.KeyEventDelayOffsetTextBox.Text = result.ToString();
      Keyboard.ClearFocus();
    }
  }
}
