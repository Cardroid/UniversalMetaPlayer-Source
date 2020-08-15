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

using UMP.Controller.Feature.OptionControl.ViewModel;
using UMP.Core;
using UMP.Utility;

namespace UMP.Controller.Feature.OptionControl
{
  public partial class ThemeOption : UserControl
  {
    public ThemeOption()
    {
      InitializeComponent();

      this.PreviewMouseDown += (_, e) => { AverageColorProcessingOffsetTextBox_Apply(); };

      this.AverageColorProcessingOffsetTextBox.PreviewKeyDown += (_, e) => { if (e.Key == Key.Enter) AverageColorProcessingOffsetTextBox_Apply(); };
      this.AverageColorProcessingOffsetTextBox.Text = GlobalProperty.Options.AverageColorProcessingOffset.ToString();
    }

    private void AverageColorProcessingOffsetTextBox_Apply()
    {
        if (int.TryParse(this.AverageColorProcessingOffsetTextBox.Text, out int result))
          result = Math.Clamp(result, 1, 501);
        else
          result = GlobalProperty.Options.DefaultValue.DefaultAverageColorProcessingOffset;
        GlobalProperty.Options.AverageColorProcessingOffset = result;
        this.AverageColorProcessingOffsetTextBox.Text = result.ToString();
        Keyboard.ClearFocus();
    }
  }
}
