using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.Utils.SoundTouch
{
  public class SoundTouchProfile
  {
    public bool UseAntiAliasing { get; set; }
    public bool UseQuickSeek { get; set; }

    public SoundTouchProfile(bool useAntiAliasing, bool useQuickSeek)
    {
      this.UseAntiAliasing = useAntiAliasing;
      this.UseQuickSeek = useQuickSeek;
    }
  }
}