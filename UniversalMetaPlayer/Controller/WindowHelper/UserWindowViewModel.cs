using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

using UMP.Core.Model.ViewModel;

namespace UMP.Controller.WindowHelper
{
  public class UserWindowViewModel : ViewModelBase
  {
    public UserControl UserControl
    {
      get => _UserControl;
      set
      {
        _UserControl = value;
        OnPropertyChanged("UserControl");
      }
    }
    private UserControl _UserControl;
  }
}
