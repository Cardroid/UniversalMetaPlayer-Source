using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace UMP.Core.Model.Func
{
  public class FunctionControlForm : UserControl
  {
    public FunctionControlForm(string FunctionName)
    {
      this.FunctionName = FunctionName;
    }
    public string FunctionName { get; }
  }
}
