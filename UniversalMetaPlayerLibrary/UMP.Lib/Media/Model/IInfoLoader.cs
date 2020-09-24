using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using UMP.Lib.Model;

namespace UMP.Lib.Media.Model
{
    public interface IInfoLoader
    {
        public event MessageProgressChangedEventHandler<InfoProgressKind> InfoProgressChanged;

        /// <summary>
        /// 미디어 정보를 가져옵니다.
        /// </summary>
        /// <param name="fullLoad">모든 정보 로드 여부</param>
        /// <returns>로드된 <see cref="MediaInformation"/></returns>
        public Task<GenericResult<MediaInformation>> GetInformationAsync(MediaInformation mediaInfo, bool fullLoad, IMediaIDParer mediaIDParer);
    }
}
