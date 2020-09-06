using System;
using System.Collections.Generic;
using System.Text;

using UMP.Core.Model.ViewModel;
using UMP.Core.Player.Plugin.Effect;

namespace UMP.Core.Player.Plugin.Control
{
  public class EqualizerParameterSliderViewModel : ViewModelBase
  {
    public EqualizerParameterSliderViewModel(EqualizerBand band)
    {
      this.Band = band;
      ResetCommand = new RelayCommand((o) => Reset());
    }

    public RelayCommand ResetCommand { get; }
    public void Reset() => Gain = 0f;

    private EqualizerBand Band { get; }

    public static float MaxGain => 20f;
    public static float MinGain => -20f;

    public string FrequencyString => $"{UMP.Utils.Parser.ToSI(Band.Frequency, "F1")}Hz";
    public string BandWidthString => $"{Band.BandWidth:F1}";

    public float Gain
    {
      get => Band.Gain;
      set
      {
        Band.Gain = Math.Clamp(value, MinGain, MaxGain);
        MainMediaPlayer.Call<Equalizer>(PluginName.Equalizer)?.Update();
        OnPropertyChanged("Gain");
        OnPropertyChanged("GainString");
      }
    }
    public string GainString => $"{Gain:F1}dB";
  }
}
