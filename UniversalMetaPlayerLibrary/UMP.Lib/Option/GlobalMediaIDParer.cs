using System;
using System.Collections.Generic;
using System.Text;

using UMP.Lib.Media.Model;

namespace UMP.Lib.Option
{
    public static class GlobalMediaIDParer
    {
        public static bool SetMediaIDParser(IMediaIDParer mediaID)
        {
            GetIDParer = mediaID;
            return true;
        }

        public static IMediaIDParer GetIDParer { get; private set; }
    }
}
