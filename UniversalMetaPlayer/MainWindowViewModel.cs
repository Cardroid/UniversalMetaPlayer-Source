using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UMP.Controller;
using UMP.Controller.Feature;
using UMP.Core.Model;

namespace UMP
{
  public class MainWindowViewModel : ViewModelBase
  {
    public MainWindowViewModel()
    {
      PlayListControl = new PlayListControl();
      FeatureControl = new MainFeatureControl();
    }

    public UserControl PlayListControl
    {
      get => _PlayListControl.IsAlive ? (UserControl)_PlayListControl.Target : null;
      set
      {
        _PlayListControl = new WeakReference(value);
        OnPropertyChanged("PlayListControl");
      }
    }
    private WeakReference _PlayListControl;

    public UserControl FeatureControl
    {
      get => _FeatureControl.IsAlive ? (UserControl)_FeatureControl.Target : null;
      set
      {
        _FeatureControl = new WeakReference(value);
        OnPropertyChanged("FeatureControl");
      }
    }
    private WeakReference _FeatureControl;
  }
}
