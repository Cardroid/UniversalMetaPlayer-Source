using System;
using System.ComponentModel;
using System.Windows.Media;

using UMP.Utility;

namespace UMP.Core.Model.ViewModel
{
  public class ViewModelBase : INotifyPropertyChanged
  {
    public ViewModelBase()
    {
      ThemeHelper.ThemeChangedEvent += (e) => this.ControlBorderBrush = new SolidColorBrush(e.PrimaryColor);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public Brush ControlBorderBrush
    {
      get => _ControlBorderBrush;
      set
      {
        _ControlBorderBrush = value;
        OnPropertyChanged("ControlBorderBrush");
      }
    }
    private Brush _ControlBorderBrush = new SolidColorBrush(ThemeHelper.PrimaryColor);
  }
}
