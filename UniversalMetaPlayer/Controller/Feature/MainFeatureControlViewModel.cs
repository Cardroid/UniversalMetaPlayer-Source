using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

using UMP.Controller.Feature.OptionControl;
using UMP.Core.Model;

namespace UMP.Controller.Feature
{
  public class MainFeatureControlViewModel : ViewModelBase
  {
    public MainFeatureControlViewModel()
    {
      FeaturePanel = new BasicOption();
    }

    public UserControl FeaturePanel
    {
      get => _FeaturePanel.IsAlive ? (UserControl)_FeaturePanel.Target : null;
      set
      {
        _FeaturePanel = new WeakReference(value);
        OnPropertyChanged("FeaturePanel");
      }
    }
    private WeakReference _FeaturePanel;
  }
}
