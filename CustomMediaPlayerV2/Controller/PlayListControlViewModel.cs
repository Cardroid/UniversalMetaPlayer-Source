using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using CMP2.Core;
using CMP2.Core.Model;

namespace CMP2.Controller
{
  public class PlayListControlViewModel : ViewModelBase
  {
    public PlayListControlViewModel()
    {
      MainMediaPlayer.PropertyChangedEvent += MainMediaPlayer_PropertyChangedEvent;
    }
    private bool PlayListIsNull => MainMediaPlayer.PlayList == null;
    public PlayList PlayList
    {
      get => MainMediaPlayer.PlayList;
      set
      {
        MainMediaPlayer.PlayList = value;
        OnPropertyChanged("Playlist");
      }
    }

    private void MainMediaPlayer_PropertyChangedEvent(string propertyname)
    {
      if(propertyname == "PlayList") { }
    }

    // 플레이리스트 선택 항목
    private MediaInfo _SelectItem = null;
    public MediaInfo SelectItem
    {
      get => _SelectItem;
      set
      {
        _SelectItem = value;
        OnPropertyChanged("SelectItem");
      }
    }
  }
}