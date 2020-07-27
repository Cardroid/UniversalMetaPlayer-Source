using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

using UMP.Controller.Option.OptionControl;
using UMP.Core.Model;

namespace UMP.Controller.Option
{
  public class MainOptionControlViewModel : ViewModelBase
  {
    public MainOptionControlViewModel()
    {
      OptionPanel = new BasicOption();
    }

    public UserControl OptionPanel
    {
      get => _OptionPanel.IsAlive ? (UserControl)_OptionPanel.Target : null;
      set
      {
        _OptionPanel = new WeakReference(value);
        OnPropertyChanged("OptionPanel");
      }
    }
    public WeakReference _OptionPanel;
  }
}
