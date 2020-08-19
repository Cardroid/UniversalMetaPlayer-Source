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

using UMP.Controller.Function.OptionControl.ViewModel;
using UMP.Core;
using UMP.Utility;

namespace UMP.Controller.Function.OptionControl
{
  public partial class ThemeOption : FunctionModelControl
  {
    public ThemeOption() : base("옵션 - 테마")
    {
      InitializeComponent();

      GlobalProperty.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == "AverageColorProcessingOffset")
          this.AverageColorProcessingOffsetTextBox.Text = GlobalProperty.Options.AverageColorProcessingOffset.ToString();
      };

      this.PreviewMouseDown += (_, e) => { Keyboard.ClearFocus(); };

      this.AverageColorProcessingOffsetTextBox.GotKeyboardFocus += (_, e) => { this.AverageColorProcessingOffsetTextBox.Text = ""; };
      this.AverageColorProcessingOffsetTextBox.PreviewKeyDown += (_, e) => { if (e.Key == Key.Enter) Keyboard.ClearFocus(); };
      this.AverageColorProcessingOffsetTextBox.LostKeyboardFocus += (_, e) => { AverageColorProcessingOffsetTextBox_Apply(); };
      this.AverageColorProcessingOffsetTextBox.Text = GlobalProperty.Options.AverageColorProcessingOffset.ToString();
    }

    private void AverageColorProcessingOffsetTextBox_Apply()
    {
      if (int.TryParse(this.AverageColorProcessingOffsetTextBox.Text, out int result))
        result = Math.Clamp(result, 1, 501);
      else
        result = GlobalProperty.Options.DefaultValue.AverageColorProcessingOffset;
      GlobalProperty.Options.AverageColorProcessingOffset = result;
    }
  }
}
