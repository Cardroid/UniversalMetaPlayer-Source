﻿using System;
using System.Collections.Generic;
using System.Text;

using UMP.Core.Model.Media;
using UMP.Core.Model.ViewModel;
using UMP.Core.Player;

namespace UMP.Controller.Function.Lyrics.ViewModel
{
  public class LyricsControlViewModel : ViewModelBase
  {
    public LyricsControlViewModel()
    {
      if (MainMediaPlayer.MediaLoadedCheck)
        Lyrics = MainMediaPlayer.MediaInformation.Tags[MediaInfoType.Lyrics];
      
      MainMediaPlayer.PropertyChanged += (_, e) =>
      {
        if (MainMediaPlayer.MediaLoadedCheck && e.PropertyName == "MainPlayerInitialized")
        {
          Lyrics = MainMediaPlayer.MediaInformation.Tags[MediaInfoType.Lyrics];
          OnPropertyChanged("Lyrics");
        }
      };
    }

    public string Lyrics { get; private set; }
  }
}
