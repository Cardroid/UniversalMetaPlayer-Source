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

using UMP.Core.Global;
using UMP.Core.Model.Control;

namespace UMP.Controller.Function.OptionControl
{
  public partial class KeyboardOption : FunctionControlForm
  {
    public KeyboardOption() : base("옵션 - 키보드")
    {
      InitializeComponent();
    }
  }
}
