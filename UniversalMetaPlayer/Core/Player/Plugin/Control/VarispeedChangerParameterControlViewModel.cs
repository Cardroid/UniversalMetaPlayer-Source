using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows.Threading;
using UMP.Core.Model.ViewModel;
using UMP.Core.Player.Plugin.Effect;

namespace UMP.Core.Player.Plugin.Control
{
  public class VarispeedChangerParameterControlViewModel : ViewModelBase
  {
    public VarispeedChangerParameterControlViewModel()
    {
      ApplyTimer = new Timer(1500) { AutoReset = true };
      ApplyTimer.Elapsed += (_, e) => ViewDispatcher.Invoke(() => { ApplyTimer.Stop(); Apply(); });
      ResetCommand = new RelayCommand((o) => Reset());
    }

    public void SetDispatcher(Dispatcher dispatcher) => ViewDispatcher = dispatcher;
    private Dispatcher ViewDispatcher { get; set; }

    public bool VarispeedChangerIsChecked
    {
      get => TempProperty.IsUseVarispeedChanger;
      set
      {
        TempProperty.IsUseVarispeedChanger = value;
        ApplyTimer.Stop();
        Apply();
      }
    }

    private Timer ApplyTimer { get; }

    private void Apply()
    {
      if (MainMediaPlayer.MediaLoadedCheck)
      {
        var varispeedChanger = MainMediaPlayer.Call<VarispeedChanger>(PluginName.VarispeedChanger);
        if(varispeedChanger != null)
        {
          varispeedChanger.IsEnabled = TempProperty.IsUseVarispeedChanger;
          varispeedChanger.SetParameter(new VarispeedChangerParameter(Tempo, Rate, Pitch));
        }
#if DEBUG
        App.MainLog.Debug($"오디오 효과 적용됨 Active : {VarispeedChangerIsChecked} Value : x{Tempo} x{Rate} x{Pitch}");
#endif
      }
    }

    #region Tempo
    public float Tempo
    {
      get => _Tempo;
      set
      {
        _Tempo = (float)Math.Round(Math.Clamp(value, MinTempo, MaxTempo), 2);
        OnPropertyChanged("Tempo");
        OnPropertyChanged("TempoString");
        ApplyTimer.Stop();
        ApplyTimer.Start();
      }
    }
    private float _Tempo = TempProperty.VarispeedChangerParameter.Tempo;
    public string TempoString => $"x{Tempo:F2}";
    public float MinTempo => 0.01f;
    public float MaxTempo => 10f;
    #endregion

    #region Rate
    public float Rate
    {
      get => _Rate;
      set
      {
        _Rate = (float)Math.Round(Math.Clamp(value, MinRate, MaxRate), 2);
        OnPropertyChanged("Rate");
        OnPropertyChanged("RateString");
        ApplyTimer.Stop();
        ApplyTimer.Start();
      }
    }
    private float _Rate = TempProperty.VarispeedChangerParameter.Rate;
    public string RateString => $"x{Rate:F2}";
    public float MinRate => 0.01f;
    public float MaxRate => 10f;
    #endregion

    #region Pitch
    public float Pitch
    {
      get => _Pitch;
      set
      {
        _Pitch = (float)Math.Round(Math.Clamp(value, MinPitch, MaxPitch), 2);
        OnPropertyChanged("Pitch");
        OnPropertyChanged("PitchString");
        ApplyTimer.Stop();
        ApplyTimer.Start();
      }
    }
    private float _Pitch = TempProperty.VarispeedChangerParameter.Pitch;
    public string PitchString => $"x{Pitch:F2}";
    public float MinPitch => 0.01f;
    public float MaxPitch => 10f;
    #endregion

    public RelayCommand ResetCommand { get; }

    private void Reset()
    {
      _Tempo = 1f;
      _Rate = 1f;
      _Pitch = 1f;
      Apply();
      OnPropertyChanged("Tempo");
      OnPropertyChanged("TempoString");
      OnPropertyChanged("Rate");
      OnPropertyChanged("RateString");
      OnPropertyChanged("Pitch");
      OnPropertyChanged("PitchString");
    }
  }
}
