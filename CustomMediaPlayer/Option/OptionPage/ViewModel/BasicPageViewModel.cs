using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMediaPlayer.Option.OptionPage.ViewModel
{
  public class BasicPageViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private void Notify(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); 

    public bool MediaOpeningPlayOption { get { return MainWindow.Optioncore.MediaOpeningPlayOption; } set { MainWindow.Optioncore.MediaOpeningPlayOption = value; Notify("MediaOpeningPlayOption"); } }
    public bool LastSongSaveOption { get { return MainWindow.Optioncore.LastSongSaveOption; } set { MainWindow.Optioncore.LastSongSaveOption = value; Notify("LastSongSaveOption"); } }

  }
}
