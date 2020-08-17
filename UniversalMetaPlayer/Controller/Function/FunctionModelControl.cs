using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace UMP.Controller.Function
{
  public class FunctionModelControl : UserControl
  {
    public FunctionModelControl(string FunctionName)
    {
      this.FunctionName = FunctionName;
    }
    public string FunctionName { get; }
  }
}
