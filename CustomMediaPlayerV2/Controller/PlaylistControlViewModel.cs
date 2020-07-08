using System;
using System.Collections.Generic;
using System.Text;

using CMP2.Core;
using CMP2.Core.Playlist;

namespace CMP2.Controller
{
  public class PlaylistControlViewModel : ViewModel
  {
    public PlaylistControlViewModel()
    {
      Playlist = new PlayList();
    }
    private PlayList _Playlist;
    public PlayList Playlist
    {
      get => _Playlist;
      set
      {
        _Playlist = value;
        OnPropertyChanged("Playlist");
      }
    }
  }
}
