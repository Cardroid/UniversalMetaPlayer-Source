using System.Collections.Generic;
using NAudio.Dsp;
using NAudio.Wave;

namespace UMP.Core.Player.Plugin.Effect
{
  public class Equalizer : ISamplePlugin
  {
    private readonly ISampleProvider Source;
    private readonly BiQuadFilter[,] Filters;
    private bool UpdateRequest;

    public Equalizer(ISampleProvider sourceProvider, EqualizerBand[] bands)
    {
      this.Source = sourceProvider;
      this.Bands = bands;

      IsEnabled = TempProperty.IsUseEqualizer;

      Filters = new BiQuadFilter[Source.WaveFormat.Channels, bands.Length];
      CreateFilters();
    }

    private void CreateFilters()
    {
      for (int bandIndex = 0; bandIndex < Bands.Length; bandIndex++)
      {
        var band = Bands[bandIndex];
        for (int n = 0; n < Source.WaveFormat.Channels; n++)
        {
          if (Filters[n, bandIndex] == null)
            Filters[n, bandIndex] = BiQuadFilter.PeakingEQ(Source.WaveFormat.SampleRate, band.Frequency, band.BandWidth, band.Gain);
          else
            Filters[n, bandIndex].SetPeakingEq(Source.WaveFormat.SampleRate, band.Frequency, band.BandWidth, band.Gain);
        }
      }
    }

    public EqualizerBand[] Bands { get; }

    public void Update()
    {
      UpdateRequest = true;
      CreateFilters();
    }

    public WaveFormat WaveFormat => Source.WaveFormat;

    public PluginName Name => PluginName.Equalizer;

    public bool IsEnabled { get; set; }

    public bool IsActive => IsEnabled;

    public int Read(float[] buffer, int offset, int count)
    {
      int samplesRead = Source.Read(buffer, offset, count);

      if (IsActive)
      {
        if (UpdateRequest)
        {
          CreateFilters();
          UpdateRequest = false;
        }

        for (int n = 0; n < samplesRead; n++)
        {
          int ch = n % WaveFormat.Channels;

          for (int band = 0; band < Bands.Length; band++)
            buffer[offset + n] = Filters[ch, band].Transform(buffer[offset + n]);
        }
      }
      return samplesRead;
    }
  }

  public class EqualizerBand
  {
    public EqualizerBand(float bandWidth, float frequency, float gain = 0f)
    {
      this.Frequency = frequency;
      this.BandWidth = bandWidth;
      this.Gain = gain;
    }
    public float Frequency { get; set; }
    public float BandWidth { get; set; }
    public float Gain { get; set; }
  }
}
