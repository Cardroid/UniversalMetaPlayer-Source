using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.Core.Model
{
  public class MediaInfoDictionary
  {
    private readonly Dictionary<MediaInfoType, string> InfoDictionary;
    public MediaInfoDictionary()
    {
      InfoDictionary = new Dictionary<MediaInfoType, string>();
    }
    public MediaInfoDictionary(MediaInfoDictionary info)
    {
      InfoDictionary = info.InfoDictionary;
    }

    public string this[MediaInfoType typeKey]
    {
      get => InfoDictionary.TryGetValue(typeKey, out string value) ? value : string.Empty;
      set => InfoDictionary[typeKey] = value;
    }
  }
  public enum MediaInfoType
  {
    AlbumTitle,
    AlbumArtist,
    Lyrics
  }
}
