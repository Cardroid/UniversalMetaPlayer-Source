using NAudio.Wave;

namespace UMP.Lib.Player.Model
{
  public interface ISamplePlugin : ISampleProvider
  {
    /// <summary>
    /// 플러그인의 이름
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 플러그인의 사용 여부
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 현재 플러그인의 활성화 여부
    /// </summary>
    public bool IsActive { get; }

    /// <summary>
    /// 옵션 가져오기
    /// </summary>
    /// <typeparam name="T">옵션타입</typeparam>
    /// <returns>참조형식의 옵션 (실패시 null)</returns>
    public T GetOption<T>() where T : class;
  }
}
