using System.Collections.Generic;
using System.Linq;

using NAudio.Wave;

using UMP.Core.Player.Plugin.Effect;

namespace UMP.Core.Player
{
  public class SampleAggregatorChain : ISampleProvider
  {
    public SampleAggregatorChain(ISampleProvider source)
    {
      this._SourceStream = source;
    }

    private readonly ISampleProvider _SourceStream;
    private readonly Dictionary<PluginName, ISamplePlugin> _Chain = new Dictionary<PluginName, ISamplePlugin>();

    public ISampleProvider Head => _Chain.LastOrDefault().Value ?? _SourceStream;

    public WaveFormat WaveFormat => Head.WaveFormat;

    public void AddPlugin(ISamplePlugin effect) => _Chain.Add(effect.Name, effect);

    public int Read(float[] buffer, int offset, int count) => Head.Read(buffer, offset, count);

    public T Call<T>(PluginName name) where T : class => _Chain.TryGetValue(name, out ISamplePlugin plugin) && plugin is T result ? result : null;
  }
}
