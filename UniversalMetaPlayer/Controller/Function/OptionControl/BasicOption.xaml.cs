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

using MaterialDesignThemes.Wpf;

using UMP.Core;
using UMP.Core.Global;
using UMP.Core.Model.Func;
using UMP.Utility;

namespace UMP.Controller.Function.OptionControl
{
  public partial class BasicOption : FunctionControlForm
  {
    public BasicOption() : base("옵션 - 일반")
    {
      InitializeComponent();

      this.PreviewMouseDown += (_, e) => { Keyboard.ClearFocus(); };

      this.SetDefault.Click += SetDefault_Click;
      SetDefaultForeground = this.SetDefault.Foreground;
    }

    #region SetDefaultButton

    private readonly Brush SetDefaultForeground;

    private bool IsReset
    {
      get => _IsReset;
      set
      {
        _IsReset = value;
        if (value)
        {
          this.SetDefault.Foreground = Brushes.Red;
          IsResetLock();
        }
        else
          this.SetDefault.Foreground = SetDefaultForeground;
      }
    }
    private bool _IsReset = false;

    private async void IsResetLock()
    {
      await Task.Delay(3000);
      IsReset = false;
    }

    private void SetDefault_Click(object sender, RoutedEventArgs e)
    {
      if (!IsReset)
      {
        IsReset = true;
      }
      else
      {
        GlobalProperty.SetDefault();
        IsReset = false;
      }
    }
    #endregion
  }
}
