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

namespace CMP2.Controller
{
  public partial class PlaylistControl : UserControl
  {
    private PlaylistControlViewModel ViewModel { get; set; }
    public PlaylistControl()
    {
      InitializeComponent();
      ViewModel = (PlaylistControlViewModel)this.DataContext;
    }
  }
}
