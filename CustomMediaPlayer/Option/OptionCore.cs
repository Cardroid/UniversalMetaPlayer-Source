using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace CustomMediaPlayer.Option
{
  public class OptionCore
  {
    private bool keyhookoption;
    public bool KeyHookOption
    {
      get { return keyhookoption; }
      set
      {
        keyhookoption = value;
        if (keyhookoption)
          OptionWindow.hooking.Start();
        else
          OptionWindow.hooking.Stop();
      }
    }
    public bool MediaOpeningPlayOption;
    public bool DurationViewStatus;
    public bool LastSongSaveOption;
  }
}
