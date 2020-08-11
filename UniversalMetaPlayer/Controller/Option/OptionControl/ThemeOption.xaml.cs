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

using UMP.Controller.Option.OptionControl.ViewModel;
using UMP.Core;
using UMP.Utility;

namespace UMP.Controller.Option.OptionControl
{
  public partial class ThemeOption : UserControl
  {
    public ThemeOption()
    {
      InitializeComponent();

      this.AverageColorProcessingOffsetTextBox.PreviewKeyDown += AverageColorProcessingOffsetTextBox_PreviewKeyDown;
      this.AverageColorProcessingOffsetTextBox.Text = GlobalProperty.Options.AverageColorProcessingOffset.ToString();
    }

    private async void AverageColorProcessingOffsetTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        this.AverageColorProcessingOffsetTextBox.Focusable = false;
        this.AverageColorProcessingOffsetTextBox.IsReadOnlyCaretVisible = false;
        this.AverageColorProcessingOffsetTextBox.IsReadOnly = true;
        if (int.TryParse(this.AverageColorProcessingOffsetTextBox.Text, out int result))
          result = Math.Clamp(result, 1, 501);
        else
          result = GlobalProperty.Options.DefaultValue.DefaultAverageColorProcessingOffset;
        GlobalProperty.Options.AverageColorProcessingOffset = result;
        this.AverageColorProcessingOffsetTextBox.Text = result.ToString();
        await Task.Delay(300);
        this.AverageColorProcessingOffsetTextBox.IsReadOnlyCaretVisible = true;
        this.AverageColorProcessingOffsetTextBox.IsReadOnly = false;
        this.AverageColorProcessingOffsetTextBox.Focusable = true;
      }
    }
  }
}
