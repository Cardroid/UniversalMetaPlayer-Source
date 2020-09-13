using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using UMP.Lib.Model;

namespace UMP.Lib.Media.Model
{
  public interface IMediaInfoLoader
  {
    /// <summary>
    /// 미디어 정보를 가져옵니다.
    /// </summary>
    /// <param name="fullLoad">모든 정보 로드 여부</param>
    /// <returns>로드된 <see cref="MediaInformation"/></returns>
    public Task<GenericResult<MediaInformation>> GetInformationAsync(bool fullLoad);

    /// <summary>
    /// 온라인 미디어의 ID 가져오기
    /// </summary>
    /// <returns><see cref="GenericResult&lt;T&gt;"/>성공시 true, ID</returns>
    public Task<GenericResult<string>> GetID();

    public event MessageProgressChangedEventHandler<StreamProgressKind> ProgressChanged;
  }
}
