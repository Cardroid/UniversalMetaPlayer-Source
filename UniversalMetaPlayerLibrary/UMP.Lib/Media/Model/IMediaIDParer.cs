using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using UMP.Lib.Model;

namespace UMP.Lib.Media.Model
{
    public interface IMediaIDParer
    {
        /// <summary>
        /// 미디어의 ID 가져오기
        /// </summary>
        /// <returns><see cref="GenericResult&lt;string&gt;"/>성공시 true, ID</returns>
        public Task<GenericResult<string>> GetID(MediaInformation mediaInfo);
    }
}
