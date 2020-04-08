using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMediaPlayer.Option.OptionPage.ViewModel
{
    public class BasicPageViewModel
    {
        public bool MediaOpeningPlayOption { get { return MainWindow.Optioncore.MediaOpeningPlayOption; } set { MainWindow.Optioncore.MediaOpeningPlayOption = value; } }
        public bool LastSongSaveOption { get { return MainWindow.Optioncore.LastSongSaveOption; } set { MainWindow.Optioncore.LastSongSaveOption = value; } }
    }
}
