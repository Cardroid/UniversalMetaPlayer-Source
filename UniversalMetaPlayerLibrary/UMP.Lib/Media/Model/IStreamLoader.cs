using System;
using System.ComponentModel;
using System.Threading.Tasks;

using UMP.Lib.Model;

namespace UMP.Lib.Media.Model
{
    public interface IStreamLoader
    {
        public event MessageProgressChangedEventHandler<StreamProgressKind> StreamProgressChanged;

        /// <summary>
        /// 미디어 스트림 경로를 가져옵니다.
        /// </summary>
        /// <returns>성공시 true, 미디어 스트림 경로</returns>
        public Task<GenericResult<string>> GetStreamPathAsync(MediaInformation mediaInfo, bool useCache, IMediaIDParer mediaIDParer);
    }
}
