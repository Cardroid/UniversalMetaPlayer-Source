using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;

using MaterialDesignThemes.Wpf;

namespace CMP2.Core.Model
{
  public class ViewModelBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    public FontFamily FontFamily => GlobalProperty.MainFontFamily;
    public ITheme Theme => GlobalProperty.Theme;
  }
}
