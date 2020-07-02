using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMediaPlayer.Option.OptionPage.ViewModel
{
  public class KeyOptionPageViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private void Notify(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); 

    public bool KeyHookOption
    {
      get { return MainWindow.Optioncore.KeyHookOption; }
      set { MainWindow.Optioncore.KeyHookOption = value; Notify("KeyHookOption"); }
    }
  }
}
