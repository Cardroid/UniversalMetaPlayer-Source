﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Newtonsoft.Json;

namespace CustomMediaPlayer.Core
{
  /// <summary>
  /// 미디어의 기본 정보
  /// </summary>
  public class MediaInfo
  {
    public MediaInfo(string filefullpath)
    {
      if (string.IsNullOrWhiteSpace(filefullpath))
        return;
      FileFullName = filefullpath;

      using (var Fileinfo = TagLib.File.Create(FileFullName))
      {
        Title = Fileinfo.Tag.Title ?? Path.GetFileNameWithoutExtension(filefullpath);
        Duration = Fileinfo.Properties.Duration;
      }
    }

    #region 프로퍼티 정의
    /// <summary>
    /// 플레이리스트에서의 고유숫자
    /// </summary>
    public int ID { get; set; }
    /// <summary>
    /// 파일의 위치
    /// </summary>
    public string FileFullName { get; set; } = null;
    /// <summary>
    /// 타이틀
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// 미디어의 총 재생시간
    /// </summary>
    public TimeSpan Duration { get; private set; } = TimeSpan.Zero;
    #endregion

    public static string[] Serialize(MediaInfo mediaInfo)
    {
      string[] Properties = { mediaInfo.Title, mediaInfo.FileFullName };
      return Properties;
    }
    public static bool Deserialize(string mediafullpath, out MediaInfo mediaInfo)
    {
      if (!string.IsNullOrWhiteSpace(mediafullpath))
        try
        {
          mediaInfo = new MediaInfo(mediafullpath);
          return true;
        }
        catch
        {
          mediaInfo = new MediaInfo(null);
          return false;
        }
      else
      {
        mediaInfo = new MediaInfo(null);
        return false;
      }
    }
  }

  /// <summary>
  /// 미디어의 자세한 정보
  /// </summary>
  public class MediaFullInfo : MediaInfo
  {

    public MediaFullInfo(MediaInfo media) : base(media.FileFullName)
    {
      base.ID = media.ID;
      using (var Fileinfo = TagLib.File.Create(FileFullName))
      {
        // 미디어 정보를 정보 클래스에 저장
        try
        {
          TagLib.IPicture pic = Fileinfo.Tag.Pictures[0];  //pic contains data for image.
          MemoryStream stream = new MemoryStream(pic.Data.Data);  // create an in memory stream
          AlbumImage = BitmapFrame.Create(stream);
        }
        catch { AlbumImage = Utility.Utility.LogoNoteImage; }
        AlbumTitle = string.IsNullOrWhiteSpace(Fileinfo.Tag.Album) ? Fileinfo.Tag.Album : null;
        ArtistName = string.IsNullOrWhiteSpace(Fileinfo.Tag.FirstAlbumArtist) ? Fileinfo.Tag.FirstAlbumArtist : null;
      }
    }

    #region 프로퍼티 정의
    /// <summary>
    /// 존재하면 엘범이미지, 존재하지 않으면 로고 이미지가 반환됩니다.
    /// </summary>
    public ImageSource AlbumImage { get; private set; } = Utility.Utility.LogoNoteImage;
    /// <summary>
    /// 엘범 타이틀
    /// </summary>
    public string AlbumTitle { get; private set; } = null;
    /// <summary>
    /// 아티스트 이름
    /// </summary>
    public string ArtistName { get; private set; } = null;
    #endregion
  }
}
