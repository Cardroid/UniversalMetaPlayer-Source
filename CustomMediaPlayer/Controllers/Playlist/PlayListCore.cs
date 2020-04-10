using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomMediaPlayer.Core;

namespace CustomMediaPlayer.Controllers.PlayList
{
    public class PlayListCore
    {
        private List<MediaInfo> playlist = new List<MediaInfo>();
        public List<MediaInfo> Playlist
        {
            get { return playlist; }
            set { playlist = value; }
        }
    }
}
