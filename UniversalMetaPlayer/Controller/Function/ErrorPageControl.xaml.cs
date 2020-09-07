using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UMP.Core.Model.Control;

namespace UMP.Controller.Function
{
  public partial class ErrorPageControl : FunctionControlForm
  {
    public ErrorPageControl() : base("오류페이지")
    {
      InitializeComponent();
    }
  }
}
