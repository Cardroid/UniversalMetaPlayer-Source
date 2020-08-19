using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Media;

using UMP.Core;
using UMP.Core.Model;
using UMP.Core.Player;
using UMP.Utility;

namespace UMP.Controller.ViewModel
{
  public class PlayListControlViewModel : ViewModelBase
  {
    public PlayListControlViewModel()
    {
      MainMediaPlayer.PlayList.Field_PropertyChanged += PlayList_Field_PropertyChanged; ;
      MainMediaPlayer.PropertyChanged += MainMediaPlayer_PropertyChanged;
    }

    public PlayList PlayList => MainMediaPlayer.PlayList;
    public string PlayListName
    {
      get => PlayList.PlayListName;
      set
      {
        PlayList.PlayListName = value;
        OnPropertyChanged("PlayListName");
      }
    }

    #region UI갱신
    private void PlayList_Field_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "PlayListName")
        OnPropertyChanged("PlayListName");
    }
    private void MainMediaPlayer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "PlayList")
        OnPropertyChanged("PlayList");
    }
    #endregion
  }
}