using System;
using System.Collections.Generic;
using System.Text;

using CMP2.Core;
using CMP2.Core.Playlist;

namespace CMP2.Controller
{
  public class PlayListControlViewModel : ViewModelBase
  {
    public PlayList PlayList
    {
      get => MainMediaPlayer.PlayList;
      set
      {
        MainMediaPlayer.PlayList = value;
        OnPropertyChanged("Playlist");
      }
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