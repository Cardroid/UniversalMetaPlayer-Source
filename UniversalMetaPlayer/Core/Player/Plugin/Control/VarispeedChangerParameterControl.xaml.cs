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

namespace UMP.Core.Player.Plugin.Control
{
  public partial class VarispeedChangerParameterControl : UserControl
  {
    public VarispeedChangerParameterControl()
    {
      InitializeComponent();
      ((VarispeedChangerParameterControlViewModel)this.DataContext).SetDispatcher(this.Dispatcher);
    }
  }
}
