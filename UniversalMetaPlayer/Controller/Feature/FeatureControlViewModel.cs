using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

using UMP.Controller.Feature.OptionControl;
using UMP.Core.Model;

namespace UMP.Controller.Feature
{
  public class FeatureControlViewModel : ViewModelBase
  {
    public FeatureControlViewModel()
    {
      FeaturePanel = new BasicOption();
    }

    public string Header => FeaturePanel != null ? FeaturePanel.FeatureName : "기능 패널";

    public FeatureModelControl FeaturePanel
    {
      get => _FeaturePanel.IsAlive ? (FeatureModelControl)_FeaturePanel.Target : null;
      set
      {
        _FeaturePanel = new WeakReference(value);
        OnPropertyChanged("FeaturePanel");
        OnPropertyChanged("Header");
      }
    }
    private WeakReference _FeaturePanel;
  }
}
