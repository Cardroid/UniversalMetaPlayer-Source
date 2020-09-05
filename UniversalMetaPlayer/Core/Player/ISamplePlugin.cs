using NAudio.Wave;

using UMP.Core.Player.Plugin.Effect;

namespace UMP.Core.Player
{
  public interface ISamplePlugin : ISampleProvider
  {
    /// <summary>
    /// 플러그인의 이름
    /// </summary>
    public PluginName Name { get; }
    /// <summary>
    /// 플러그인의 사용 여부
    /// </summary>
    public bool IsEnabled { get; set; }
    /// <summary>
    /// 현재 플러그인의 활성화 여부
    /// </summary>
    public bool IsActive { get; }
  }
}
