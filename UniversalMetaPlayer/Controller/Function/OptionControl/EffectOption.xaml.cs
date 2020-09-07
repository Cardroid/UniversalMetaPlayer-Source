using System;
using System.Collections.Generic;
using System.IO;
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

using UMP.Core.Global;
using UMP.Core.Model.Control;

namespace UMP.Controller.Function.OptionControl
{
  public partial class EffectOption : FunctionControlForm
  {
    public EffectOption() : base("옵션 - 효과")
    {
      InitializeComponent();

      GlobalProperty.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == "FadeEffectDelay")
          this.FadeEffectDelayTextBox.Text = GlobalProperty.Options.Getter<int>(Enums.ValueName.FadeEffectDelay).ToString();
      };

      this.FadeEffectDelayTextBox.GotKeyboardFocus += (_, e) => { this.FadeEffectDelayTextBox.Text = ""; };
      this.FadeEffectDelayTextBox.PreviewKeyDown += (_, e) => { if (e.Key == Key.Enter) Keyboard.ClearFocus(); };
      this.FadeEffectDelayTextBox.LostKeyboardFocus += (_, e) => { FadeEffectDelayTextBox_Apply(); };
      this.FadeEffectDelayTextBox.Text = GlobalProperty.Options.Getter<int>(Enums.ValueName.FadeEffectDelay).ToString();
    }

    private void FadeEffectDelayTextBox_Apply()
    {
      if (int.TryParse(this.FadeEffectDelayTextBox.Text, out int result))
        result = Math.Clamp(result, 1, 3001);
      else
        result = GlobalProperty.DefaultValue.GetDefaultValue<int>(Enums.ValueName.FadeEffectDelay);
      GlobalProperty.Options.Setter(Enums.ValueName.FadeEffectDelay, result.ToString());
    }
  }
}
