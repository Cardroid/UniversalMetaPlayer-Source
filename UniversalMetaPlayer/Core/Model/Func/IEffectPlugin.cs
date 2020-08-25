using NAudio.Wave;

using UMP.Core.Player.Effect;

namespace UMP.Core.Model.Func
{
  public interface IEffectPlugin
  {
    /// <summary>
    /// 플러그인의 이름
    /// </summary>
    public EffectPluginName Name { get; }
    /// <summary>
    /// 플러그인의 사용 여부
    /// </summary>
    public bool IsEnabled { get; set; }
    /// <summary>
    /// 현재 플러그인의 활성화 여부
    /// </summary>
    public bool IsActive { get; }
    /// <summary>
    /// 액티브 적용 효과일 경우 작성
    /// </summary>
    /// <param name="isActive">효과의 적용 여부</param>
    public void Call(bool isActive);
    public int Read(int samplesRead, float[] buffer, int offset, int count);
  }
}
