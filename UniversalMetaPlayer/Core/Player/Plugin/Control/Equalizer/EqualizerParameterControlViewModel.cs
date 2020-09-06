using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using UMP.Core.Model.ViewModel;
using UMP.Core.Player.Plugin.Effect;

namespace UMP.Core.Player.Plugin.Control
{
  public class EqualizerParameterControlViewModel : ViewModelBase
  {
    public EqualizerParameterControlViewModel()
    {
      ParameterSliders = new EqualizerParameterSliderList();

      Init();
      MainMediaPlayer.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == "MainPlayerInitialized")
          Init();
      };

      ResetCommand = new RelayCommand((o) => Reset());

#if DEBUG
      new Log(typeof(EqualizerParameterControlViewModel)).Debug("EQ 초기화 됨");
#endif
    }

    private void Init()
    {
      ParameterSliders.Clear();
      var equalizer = MainMediaPlayer.Call<Equalizer>(PluginName.Equalizer);
      if (equalizer != null)
        for (int i = 0; i < equalizer.Bands.Length; i++)
          ParameterSliders.Add(new EqualizerParameterSlider(equalizer.Bands[i]));
    }

    private void Reset()
    {
      for (int i = 0; i < ParameterSliders.Count; i++)
        ((EqualizerParameterSliderViewModel)ParameterSliders[i].DataContext).Reset();
    }

    public RelayCommand ResetCommand { get; }

    // ListBox의 선택 기능 미사용
    public int SelectedIndex
    {
      get => -1;
      set => OnPropertyChanged("SelectedIndex");
    }

    public EqualizerParameterSliderList ParameterSliders { get; }

    public bool EqualizerIsChecked
    {
      get => TempProperty.IsUseEqualizer;
      set
      {
        TempProperty.IsUseEqualizer = value;

        var plgEQ = MainMediaPlayer.Call<Equalizer>(PluginName.Equalizer);
        if (plgEQ != null)
          plgEQ.IsEnabled = TempProperty.IsUseEqualizer;

        OnPropertyChanged("EqualizerIsChecked");
      }
    }
  }

  public class EqualizerParameterSliderList : ObservableCollection<EqualizerParameterSlider>
  {
    public new void Add(EqualizerParameterSlider band)
    {
      if (base.Count == 0)
      {
        base.Add(band);
        return;
      }

      for (int i = 0; i < base.Count; i++)
      {
        if (base[i].Frequency >= band.Frequency)
        {
          base.Insert(i, band);
          break;
        }

        if (i == base.Count - 1)
        {
          base.Add(band);
          break;
        }
      }
    }
  }
}
