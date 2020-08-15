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
      PlayListControlColumnDefinition = new GridLength(1, GridUnitType.Auto);
      FeatureControlRowDefinition = new GridLength(1, GridUnitType.Auto);
    }

    public UserControl PlayListControl
    {
      get => _PlayListControl.IsAlive ? (UserControl)_PlayListControl.Target : null;
      set
      {
        _PlayListControl = new WeakReference(value);
        
        if (value != null)
          PlayListControlColumnDefinition = new GridLength(1, GridUnitType.Star);
        else
          PlayListControlColumnDefinition = new GridLength(1, GridUnitType.Auto);

        OnPropertyChanged("PlayListControl");
        OnPropertyChanged("PlayListControlColumnDefinition");
      }
    }
    private WeakReference _PlayListControl;
    public GridLength PlayListControlColumnDefinition { get; set; }

    public UserControl FeatureControl
    {
      get => _FeatureControl.IsAlive ? (UserControl)_FeatureControl.Target : null;
      set
      {
        _FeatureControl = new WeakReference(value);

        if (value != null)
          FeatureControlRowDefinition = new GridLength(1, GridUnitType.Star);
        else
          FeatureControlRowDefinition = new GridLength(1, GridUnitType.Auto);

        OnPropertyChanged("FeatureControl");
        OnPropertyChanged("FeatureControlRowDefinition");
      }
    }
    private WeakReference _FeatureControl;
    public GridLength FeatureControlRowDefinition { get; set; }
  }
}
