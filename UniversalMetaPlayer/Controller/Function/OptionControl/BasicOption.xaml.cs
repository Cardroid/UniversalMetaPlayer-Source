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

using UMP.Controller.Function.OptionControl.ViewModel;
using UMP.Core.Model.Func;

namespace UMP.Controller.Function.OptionControl
{
  public partial class BasicOption : FunctionControlForm
  {
    public BasicOption() : base("옵션 - 일반")
    {
      InitializeComponent();
      ((BasicOptionViewModel)this.DataContext).SetDispatcher(this.Dispatcher);
    }
  }
}
