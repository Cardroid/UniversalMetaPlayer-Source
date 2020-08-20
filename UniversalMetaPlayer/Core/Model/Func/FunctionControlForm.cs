using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

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
