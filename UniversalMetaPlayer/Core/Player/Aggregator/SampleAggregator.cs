using System;

using NAudio.Wave;

using UMP.Core.Model.Func;
using UMP.Core.Player.Plugin.Effect;

namespace UMP.Core.Player.Aggregator
{
  public interface IPluginSampleProvider : ISampleProvider
  {
    public void Reset();

    public SampleEffectAggregator EffectAggregator { get; }
    public SampleAnalysisAggregator AnalysisAggregator { get; }
    public void AddPlugin(IEffectPlugin plugin, bool pluginUse = true);
    public void RemovePlugin(EffectPluginName pluginName);
    public void Call(EffectPluginName name, bool isActive);

    public bool PerformFFT { get; set; }
    public int NotificationCount { get; set; }
    public event EventHandler<MaxSampleEventArgs> MaximumCalculated;
    public event EventHandler<FftEventArgs> FftCalculated;
  }

  public class SampleAggregator : IPluginSampleProvider
  {
    public SampleAggregator(ISampleProvider source, int fftLength = 1024)
    {
      this.Source = source;
      EffectAggregator = new SampleEffectAggregator(Source);
      ConvertorAggregator = new SampleConvertorAggregator(Source);
      AnalysisAggregator = new SampleAnalysisAggregator(Source, fftLength);
    }

    private ISampleProvider Source { get; }
    public SampleEffectAggregator EffectAggregator { get; }
    public SampleConvertorAggregator ConvertorAggregator { get; }
    public SampleAnalysisAggregator AnalysisAggregator { get; }

    public WaveFormat WaveFormat => Source.WaveFormat;

    public bool PerformFFT
    {
      get => AnalysisAggregator.PerformFFT;
      set => AnalysisAggregator.PerformFFT = value;
    }
    public int NotificationCount
    {
      get => AnalysisAggregator.NotificationCount;
      set => AnalysisAggregator.NotificationCount = value;
    }

    public event EventHandler<MaxSampleEventArgs> MaximumCalculated
    {
      add => AnalysisAggregator.MaximumCalculated += value;
      remove => AnalysisAggregator.MaximumCalculated -= value;
    }
    public event EventHandler<FftEventArgs> FftCalculated
    {
      add => AnalysisAggregator.FftCalculated += value;
      remove => AnalysisAggregator.FftCalculated -= value;
    }

    public int Read(float[] buffer, int offset, int count)
    {
      int samplesRead = Source.Read(buffer, offset, count);

      samplesRead = EffectAggregator.Read(samplesRead, buffer, offset, count);
      samplesRead = ConvertorAggregator.Read(samplesRead, buffer, offset, count);
      samplesRead = AnalysisAggregator.Read(samplesRead, buffer, offset, count);

      return samplesRead;
    }
    public void AddPlugin(IEffectPlugin plugin, bool pluginUse = true) => EffectAggregator.AddPlugin(plugin, pluginUse);
    public void RemovePlugin(EffectPluginName pluginName) => EffectAggregator.RemovePlugin(pluginName);

    public void Call(EffectPluginName name, bool isActive) => EffectAggregator.Call(name, isActive);

    public void Reset()
    {
      EffectAggregator.Reset();
      AnalysisAggregator.Reset();
    }
  }
}
