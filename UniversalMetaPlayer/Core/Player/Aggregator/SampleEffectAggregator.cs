using System.Collections.Generic;

using NAudio.Wave;

using UMP.Core.Global;
using UMP.Core.Model.Func;
using UMP.Core.Player.Effect;

namespace UMP.Core.Player.Aggregator
{
  public class SampleEffectAggregator : ISamplePlugin
  {
    public SampleEffectAggregator(ISampleProvider source)
    {
      this.Source = source;
    }

    private ISampleProvider Source { get; }

    public WaveFormat WaveFormat => Source.WaveFormat;

    private Dictionary<EffectPluginName, IEffectPlugin> Plugins = new Dictionary<EffectPluginName, IEffectPlugin>();

    private delegate int SampleEffectAggregatorReadHandler(int samplesRead, float[] buffer, int offset, int count);
    private event SampleEffectAggregatorReadHandler ReadEvent;
    public bool IsEnabled { get; set; } = true;

    public int Read(int samplesRead, float[] buffer, int offset, int count)
    {
      if (IsEnabled)
      {
        var result = ReadEvent?.Invoke(samplesRead, buffer, offset, count);
        if (result != null)
          samplesRead = result.Value;
      }
      return samplesRead;
    }

    public void AddPlugin(IEffectPlugin plugin, bool pluginUse = true)
    {
      if (plugin != null && !Plugins.ContainsKey(plugin.Name))
      {
        Plugins.Add(plugin.Name, plugin);
        plugin.IsEnabled = pluginUse;
        ReadEvent += plugin.Read;
      }
    }

    public void RemovePlugin(EffectPluginName pluginName)
    {
      if (Plugins.TryGetValue(pluginName, out IEffectPlugin plugin))
      {
        plugin.IsEnabled = false;
        ReadEvent -= plugin.Read;
        plugin = null;
        Plugins.Remove(plugin.Name);
      }
    }

    public void Call(EffectPluginName name, bool isActive)
    {
      if (Plugins.TryGetValue(name, out IEffectPlugin plugin))
        plugin.Call(isActive);
    }

    public void Reset()
    {
      if (Plugins.Count > 0)
        foreach (var key in Plugins.Keys)
          RemovePlugin(key);
    }
  }
}
