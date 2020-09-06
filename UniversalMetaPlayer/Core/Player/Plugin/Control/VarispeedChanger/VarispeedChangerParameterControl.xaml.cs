using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UMP.Core.Player.Plugin.Control
{
  public partial class VarispeedChangerParameterControl : UserControl
  {
    public VarispeedChangerParameterControl()
    {
      InitializeComponent();
      ((VarispeedChangerParameterControlViewModel)this.DataContext).SetDispatcher(this.Dispatcher);

      this.PreviewMouseDown += (_, e) => Keyboard.ClearFocus();
      this.PreviewKeyDown += (_, e) => { if (e.Key == Key.Enter) Keyboard.ClearFocus(); };

      this.RateTextBox.LostKeyboardFocus += TextBox_LostKeyboardFocus;
      this.TempoTextBox.LostKeyboardFocus += TextBox_LostKeyboardFocus;
      this.PitchTextBox.LostKeyboardFocus += TextBox_LostKeyboardFocus;
    }

    private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      if (sender is TextBox textBox)
      {
        var text = textBox.Text;
        if (text.StartsWith("x"))
          text = text.Substring(1);

        var viewModel = (VarispeedChangerParameterControlViewModel)this.DataContext;

        if (float.TryParse(text, out float result))
        {
          switch (textBox.Name)
          {
            case "RateTextBox":
              viewModel.Rate = result;
              break;
            case "TempoTextBox":
              viewModel.Tempo = result;
              break;
            case "PitchTextBox":
              viewModel.Pitch = result;
              break;
          }
        }
        else
        {
          switch (textBox.Name)
          {
            case "RateTextBox":
              viewModel.Rate = viewModel.Rate;
              break;
            case "TempoTextBox":
              viewModel.Tempo = viewModel.Tempo;
              break;
            case "PitchTextBox":
              viewModel.Pitch = viewModel.Pitch;
              break;
          }
        }
      }
    }
  }
}
