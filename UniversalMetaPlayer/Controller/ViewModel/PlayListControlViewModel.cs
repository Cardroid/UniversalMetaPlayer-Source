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
      MainMediaPlayer.PlayList.PropertyChangedEvent += MainMediaPlayer_PropertyChangedEvent;
      MainMediaPlayer.PropertyChangedEvent += MainMediaPlayer_PropertyChangedEvent;
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

    public int PlayListSelectIndex { get; set; }

    #region UI갱신
    private void MainMediaPlayer_PropertyChangedEvent(string propertyname)
    {
      switch (propertyname)
      {
        case "PlayList":
          OnPropertyChanged("PlayList");
          break;
        case "PlayListName":
          OnPropertyChanged("PlayListName");
          break;
      }
    }
    #endregion
  }
}