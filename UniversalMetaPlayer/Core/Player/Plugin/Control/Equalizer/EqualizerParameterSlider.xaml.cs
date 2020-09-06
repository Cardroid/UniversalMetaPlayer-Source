using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UMP.Core.Player.Plugin.Effect;

namespace UMP.Core.Player.Plugin.Control
{
  public partial class EqualizerParameterSlider : UserControl
  {
    public EqualizerParameterSlider(float bandWidth, float frequency, float gain = 0f)
    {
      InitializeComponent();
      this.DataContext = new EqualizerParameterSliderViewModel(new EqualizerBand(bandWidth, frequency, gain));
      this.Frequency = frequency;
    }

    public EqualizerParameterSlider(EqualizerBand band)
    {
      InitializeComponent();
      this.DataContext = new EqualizerParameterSliderViewModel(band);
      this.Frequency = band.Frequency;
    }

    public float Frequency { get; }
  }
}
