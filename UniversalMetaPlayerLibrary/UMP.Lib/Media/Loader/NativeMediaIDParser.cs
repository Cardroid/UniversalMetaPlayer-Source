using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using UMP.Lib.Media.Model;
using UMP.Lib.Model;

namespace UMP.Lib.Media.Loader
{
    public class NativeMediaIDParser : IMediaIDParer
    {
        public async Task<GenericResult<string>> GetID(MediaInformation mediaInfo)
        {
            if (string.IsNullOrWhiteSpace(mediaInfo.MediaLocation))
                return new GenericResult<string>(false);

            if (mediaInfo.IsOnlineMedia)
            {
                mediaInfo.
            }
            else
            {

            }
        }
    }
}
