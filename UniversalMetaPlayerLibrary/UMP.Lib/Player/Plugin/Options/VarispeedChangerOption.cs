using System;
using System.Collections.Generic;
using System.Text;

using UMP.Lib.Player.Model;
using UMP.Lib.Utility.SoundTouch;

namespace UMP.Lib.Player.Plugin.Options
{
  public class VarispeedChangerOption : PlayerPluginOptionHelper
  {
    public VarispeedChangerOption(SoundTouchProfile soundTouchProfile, VarispeedChangerParameter parameter = null, int readDurationMilliseconds = 100)
    {
      this._SoundTouchProfile = soundTouchProfile;
      this._Parameter = parameter ?? new VarispeedChangerParameter();
      this.ReadDurationMilliseconds = Math.Max(readDurationMilliseconds, 1);
    }

    public SoundTouchProfile SoundTouchProfile
    {
      get => _SoundTouchProfile;
      set
      {
        _SoundTouchProfile = value;
        OnPropertyChanged("SoundTouchProfile");
      }
    }
    private SoundTouchProfile _SoundTouchProfile;
    
    public VarispeedChangerParameter Parameter
    {
      get => _Parameter;
      set
      {
        _Parameter = value;
        OnPropertyChanged("Parameter");
      }
    }
    private VarispeedChangerParameter _Parameter;
    
    public int ReadDurationMilliseconds { get; }
  }

  public class VarispeedChangerParameter
  {
    public VarispeedChangerParameter(float tempo = 1.0f, float rate = 1.0f, float pitch = 1.0f)
    {
      this.Tempo = tempo;
      this.Rate = rate;
      this.Pitch = pitch;
    }

    public float Tempo { get; }
    public float Rate { get; }
    public float Pitch { get; }
  }
}
