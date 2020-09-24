using System.Collections.Generic;

namespace UMP.Lib.Media.Model
{
    public class MediaInfoTags
    {
        public MediaInfoTags()
        {
            InfoDictionary = new Dictionary<MediaInfoType, string>();
        }

        public MediaInfoTags(MediaInfoTags info)
        {
            InfoDictionary = info.InfoDictionary;
        }

        private Dictionary<MediaInfoType, string> InfoDictionary { get; }

        public bool IsLock
        {
            get => _IsLock;
            set
            {
                if (!_IsLock)
                    _IsLock = value;
            }
        }
        private bool _IsLock = false;

        public string this[MediaInfoType typeKey]
        {
            get => InfoDictionary.TryGetValue(typeKey, out string value) ? value : string.Empty;
            set
            {
                if (!IsLock)
                    InfoDictionary[typeKey] = value;
            }
        }
    }

    public enum MediaInfoType
    {
        AlbumTitle,
        AlbumArtist,
        Lyrics
    }
}
