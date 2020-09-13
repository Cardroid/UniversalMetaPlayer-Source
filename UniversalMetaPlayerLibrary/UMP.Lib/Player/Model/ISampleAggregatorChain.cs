using System.Collections.Generic;

namespace UMP.Lib.Player.Model
{
  public interface ISampleAggregatorChain
  {
    public void AddPlugin(ISamplePlugin plugin, bool isEnabled = false);
    public bool RemovePlugin(string name);
    public T GetPlugin<T>(string name) where T : class;
    public int PluginCount { get; }
    public Dictionary<string, ISamplePlugin>.KeyCollection Keys { get; }
    public Dictionary<string, ISamplePlugin>.ValueCollection Values { get; }
    public bool ContainsKey(string name);
    public bool ContainsValue(ISamplePlugin plugin);

  }
}
