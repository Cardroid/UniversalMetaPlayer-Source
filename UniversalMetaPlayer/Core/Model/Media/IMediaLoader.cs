using System.IO;
using System.Threading.Tasks;

namespace UMP.Core.Model.Media
{
  public interface IMediaLoader
  {
    /// <summary>
    /// 미디어 정보를 가져옵니다.
    /// </summary>
    /// <param name="fullLoad">모든 정보 로드 여부</param>
    /// <returns>로드된 <see cref="MediaInformation"/></returns>
    public abstract Task<MediaInformation> GetInformationAsync(bool fullLoad);
    /// <summary>
    /// 미디어 스트림 경로를 가져옵니다.
    /// </summary>
    /// <returns>성공시 true, 미디어 스트림 경로</returns>
    public abstract Task<GenericResult<string>> GetStreamPathAsync(bool useCache);
    /// <summary>
    /// 온라인 미디어의 ID 가져오기
    /// </summary>
    /// <returns><see cref="GenericResult&lt;T&gt;"/>성공시 true, ID</returns>
    public abstract Task<GenericResult<string>> GetID();
    /// <summary>
    /// true = 온라인 미디어, false = 로컬 미디어
    /// </summary>
    public bool Online { get; }
  }
}
