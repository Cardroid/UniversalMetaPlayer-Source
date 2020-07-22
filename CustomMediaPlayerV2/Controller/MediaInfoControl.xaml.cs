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

using CMP2.Controller.ViewModel;
using CMP2.Core;
using CMP2.Utility.Effect;

namespace CMP2.Controller
{
  public partial class MediaInfoControl : UserControl
  {
    private MediaInfoControlViewModel ViewModel { get; }
    public MediaInfoControl()
    {
      InitializeComponent();
      ViewModel = (MediaInfoControlViewModel)this.DataContext;

      this.Loaded += (s, e) =>
      {
        Log log = new Log(typeof(MediaInfoControl));
        log.Debug("초기화 성공");
      };
    }
  }
}
