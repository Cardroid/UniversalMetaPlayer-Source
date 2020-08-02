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

using UMP.Core;

namespace UMP.Controller.Option.OptionControl
{
  /// <summary>
  /// Information.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class Information : UserControl
  {
    public Information()
    {
      InitializeComponent();

      this.Mainlogo.Source = GlobalProperty.LogoImage;

      this.CoreVersionLabel.Content = GlobalProperty.CoreVersion;
      this.FileVersionLabel.Content = GlobalProperty.FileVersion;
    }
  }
}
