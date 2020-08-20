using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;

using MaterialDesignThemes.Wpf;

using UMP.Core.Global;
using UMP.Utility;

namespace UMP.Core.Model.ViewModel
{
  public class ViewModelBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    public FontFamily FontFamily => GlobalObj.Property.Predefine.MainFontFamily;
    public ITheme Theme => ThemeHelper.Theme;
  }
}
