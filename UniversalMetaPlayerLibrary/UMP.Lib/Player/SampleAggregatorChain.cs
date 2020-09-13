using System;
using System.Collections.Generic;
using System.Linq;

using NAudio.Wave;

using UMP.Lib.Player.Model;

namespace UMP.Lib.Player
{
  internal class SampleAggregatorChain : ISampleAggregatorChain, ISampleProvider
  {
    public SampleAggregatorChain(ISampleProvider source)
    {
      this._Source = source;
    }

    private readonly ISampleProvider _Source;
    public WaveFormat WaveFormat => Head.WaveFormat;
    public int Read(float[] buffer, int offset, int count) => Head.Read(buffer, offset, count);

    private readonly Dictionary<string, ISamplePlugin> _PluginChain = new Dictionary<string, ISamplePlugin>();
    public ISampleProvider Head => _PluginChain.LastOrDefault().Value ?? _Source;

    public void AddPlugin(ISamplePlugin plugin, bool isEnabled = false)
    {
      if (plugin != null)
      {
        _PluginChain.Add(plugin.Name, plugin);
        plugin.IsEnabled = isEnabled;
      }
      else
        throw new NullReferenceException("plugin is Null");
    }

    public bool RemovePlugin(string name)
    {
      if (_PluginChain.TryGetValue(name, out ISamplePlugin plugin))
      {
        plugin.IsEnabled = false;
        _PluginChain.Remove(name);
        return true;
      }
      else
        return false;
    }

    public int PluginCount => _PluginChain.Count;

    public Dictionary<string, ISamplePlugin>.KeyCollection Keys => _PluginChain.Keys;
    public Dictionary<string, ISamplePlugin>.ValueCollection Values => _PluginChain.Values;

    public bool ContainsKey(string name) => _PluginChain.ContainsKey(name);
    public bool ContainsValue(ISamplePlugin plugin) => _PluginChain.ContainsValue(plugin);

    public T GetPlugin<T>(string name) where T : class => _PluginChain.TryGetValue(name, out ISamplePlugin plugin) && plugin is T result ? result : null;

    public override string ToString()
    {
      string pluginNames = string.Empty;
      
      foreach (var key in _PluginChain.Keys)
        pluginNames += $"{key}, ";

      if (!string.IsNullOrEmpty(pluginNames))
        pluginNames.Remove(pluginNames.Length - 2, 2);

      return $"Plugin Count : {_PluginChain.Count}\nPlugin Names : {pluginNames}";
    }
  }
}
