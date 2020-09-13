using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.Lib.Utility.SoundTouch
{
  public struct SoundTouchProfile
  {
    public SoundTouchProfile(bool useAntiAliasing, bool useQuickSeek)
    {
      this.UseAntiAliasing = useAntiAliasing;
      this.UseQuickSeek = useQuickSeek;
    }

    public bool UseAntiAliasing { get; }
    public bool UseQuickSeek { get; }
  }
}