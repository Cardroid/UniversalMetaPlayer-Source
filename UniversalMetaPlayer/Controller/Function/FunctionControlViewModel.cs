using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

using UMP.Controller.Function.OptionControl;
using UMP.Core.Model;

namespace UMP.Controller.Function
{
  public class FunctionControlViewModel : ViewModelBase
  {
    public FunctionControlViewModel()
    {
      FunctionPanel = new BasicOption();
    }

    public string Header => FunctionPanel != null ? FunctionPanel.FunctionName : "기능 패널";

    public FunctionModelControl FunctionPanel
    {
      get => _FunctionPanel.IsAlive ? (FunctionModelControl)_FunctionPanel.Target : null;
      set
      {
        _FunctionPanel = new WeakReference(value);
        OnPropertyChanged("FunctionPanel");
        OnPropertyChanged("Header");
      }
    }
    private WeakReference _FunctionPanel;
  }
}
