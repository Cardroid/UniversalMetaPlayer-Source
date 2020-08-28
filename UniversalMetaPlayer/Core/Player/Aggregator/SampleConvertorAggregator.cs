using System;
using System.Collections.Generic;
using System.Text;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using UMP.Core.Model.Func;

namespace UMP.Core.Player.Aggregator
{
  public class SampleConvertorAggregator : VolumeSampleProvider, ISamplePlugin
  {
    public SampleConvertorAggregator(ISampleProvider source) : base(source)
    {
      this.Source = source;
    }

    private ISampleProvider Source { get; }
    public bool IsEnabled { get; set; }

    public int Read(int samplesRead, float[] buffer, int offset, int count)
    {

      return samplesRead;
    }
  }
}
