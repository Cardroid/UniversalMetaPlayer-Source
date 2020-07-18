using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using CMP2.Core;
using CMP2.Core.Model;

namespace CMP2.Controller.ViewModel
{
  public class PlayListControlViewModel : ViewModelBase
  {
    public PlayListControlViewModel()
    {
      MainMediaPlayer.PlayList.PropertyChangedEvent += MainMediaPlayer_PropertyChangedEvent;
      MainMediaPlayer.PropertyChangedEvent += MainMediaPlayer_PropertyChangedEvent;
    }

    public PlayList PlayList => MainMediaPlayer.PlayList;

    public int PlayListSelectIndex { get; set; }

    #region UI갱신
    private void MainMediaPlayer_PropertyChangedEvent(string propertyname)
    {
      if(propertyname == "PlayList") 
      {
        ApplyUI();
      }
    }

    public void ApplyUI()
    {
      OnPropertyChanged("Playlist");
    }
    #endregion
  }
}