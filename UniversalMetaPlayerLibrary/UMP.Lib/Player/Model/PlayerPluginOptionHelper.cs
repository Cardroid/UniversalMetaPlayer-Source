using System.ComponentModel;

namespace UMP.Lib.Player.Model
{
  public class PlayerPluginOptionHelper : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    protected internal void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}
