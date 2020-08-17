using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UMP.Controller;
using UMP.Controller.Function;
using UMP.Core.Model;

namespace UMP
{
  public class MainWindowViewModel : ViewModelBase
  {
    public MainWindowViewModel()
    {
      PlayListControl = new PlayListControl();
      FunctionControl = new FunctionControl();
      PlayListControlColumnDefinition = new GridLength(1, GridUnitType.Auto);
      FunctionControlRowDefinition = new GridLength(1, GridUnitType.Auto);
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

    public UserControl FunctionControl
    {
      get => _FunctionControl.IsAlive ? (UserControl)_FunctionControl.Target : null;
      set
      {
        _FunctionControl = new WeakReference(value);

        if (value != null)
          FunctionControlRowDefinition = new GridLength(1, GridUnitType.Star);
        else
          FunctionControlRowDefinition = new GridLength(1, GridUnitType.Auto);

        OnPropertyChanged("FunctionControl");
        OnPropertyChanged("FunctionControlRowDefinition");
      }
    }
    private WeakReference _FunctionControl;
    public GridLength FunctionControlRowDefinition { get; set; }
  }
}
