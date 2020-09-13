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

    public string this[MediaInfoType typeKey] => InfoDictionary.TryGetValue(typeKey, out string value) ? value : string.Empty;
  }

  public enum MediaInfoType
  {
    AlbumTitle,
    AlbumArtist,
    Lyrics
  }
}
