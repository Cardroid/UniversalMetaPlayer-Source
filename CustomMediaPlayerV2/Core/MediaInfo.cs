﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CMP2.Core
{
  #region 미디어 정보 인터페이스
  /// <summary>
  /// 미디어 정보 인터페이스
  /// </summary>
  public interface IMediaInfo
  {
    /// <summary>
    /// 플래이리스트에서의 고유숫자
    /// </summary>
    public int ID { get; set; }
    /// <summary>
    /// 파일의 위치
    /// </summary>
    public string FileFullName { get; set; }
    /// <summary>
    /// 타이틀
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// 미디어의 총 재생시간
    /// </summary>
    public TimeSpan Duration { get; set; }
    /// <summary>
    /// 존재하면 엘범이미지, 존재하지 않으면 로고 이미지가 반환됩니다.
    /// </summary>
    public ImageSource AlbumImage { get; set; }
    /// <summary>
    /// 엘범 타이틀
    /// </summary>
    public string AlbumTitle { get; set; }
    /// <summary>
    /// 아티스트 이름
    /// </summary>
    public string ArtistName { get; set; }
    /// <summary>
    /// 가사
    /// </summary>
    public string Lyrics { get; set; }
  }
  #endregion

  #region 미디어 정보 클레스
  /// <summary>
  /// 미디어 정보 클레스
  /// </summary>
  public class MediaInfo : IMediaInfo
  {
    public MediaInfo(string filepath)
    {
      if (string.IsNullOrWhiteSpace(filepath))
        return;
      FileFullName = filepath;
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
      if (loadinfo)
        InfomationLoader();
=======
      InfomationLoader();
>>>>>>> parent of 708ea27... Add Uno Project
=======
      InfomationLoader();
>>>>>>> parent of 708ea27... Add Uno Project
    }

    /// <summary>
    /// 미디어 정보 로드
    /// </summary>
    public void InfomationLoader()
    {
      using (var Fileinfo = TagLib.File.Create(FileFullName))
      {
<<<<<<< HEAD
<<<<<<< HEAD
        using (var Fileinfo = TagLib.File.Create(FileFullName))
=======

      using (var Fileinfo = TagLib.File.Create(FileFullName))
      {
        // 미디어 정보를 정보 클래스에 저장
        Title = Fileinfo.Tag.Title ?? Path.GetFileNameWithoutExtension(filepath);
        Duration = Fileinfo.Properties.Duration;
        try
>>>>>>> parent of 4baef91... Add MainPlayer Option Changed Event & Fixed App namespace
=======
        Title = Fileinfo.Tag.Title ?? Path.GetFileNameWithoutExtension(FileFullName);
        Duration = Fileinfo.Properties.Duration;
        try
>>>>>>> parent of 708ea27... Add Uno Project
=======
        Title = Fileinfo.Tag.Title ?? Path.GetFileNameWithoutExtension(FileFullName);
        Duration = Fileinfo.Properties.Duration;
        try
>>>>>>> parent of 708ea27... Add Uno Project
        {
          TagLib.IPicture pic = Fileinfo.Tag.Pictures[0];  //pic contains data for image.
          MemoryStream stream = new MemoryStream(pic.Data.Data);  // create an in memory stream
          AlbumImage = BitmapFrame.Create(stream);
        }
        catch { AlbumImage = null; }
        AlbumTitle = !string.IsNullOrWhiteSpace(Fileinfo.Tag.Album) ? Fileinfo.Tag.Album : string.Empty;
        ArtistName = !string.IsNullOrWhiteSpace(Fileinfo.Tag.FirstAlbumArtist) ? Fileinfo.Tag.FirstAlbumArtist : string.Empty;
        Lyrics = !string.IsNullOrWhiteSpace(Fileinfo.Tag.Lyrics) ? Fileinfo.Tag.Lyrics : string.Empty;
      }
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
      else
        LoadedCheck = LoadState.Fail;
      return Task.FromResult(0);
=======
      LoadedCheck = true;
>>>>>>> parent of 708ea27... Add Uno Project
=======
      LoadedCheck = true;
>>>>>>> parent of 708ea27... Add Uno Project
    }
    /// <summary>
    /// 정보의 로드가 완료되었는지 여부
    /// </summary>
<<<<<<< HEAD
<<<<<<< HEAD
    public LoadState LoadedCheck { get; private set; } = LoadState.NotTryed;
    #region 프로퍼티 정의 (인터페이스 상속)
=======
    }
>>>>>>> parent of 4baef91... Add MainPlayer Option Changed Event & Fixed App namespace
=======
    public bool LoadedCheck { get; private set; } = false;
>>>>>>> parent of 708ea27... Add Uno Project
=======
    public bool LoadedCheck { get; private set; } = false;
>>>>>>> parent of 708ea27... Add Uno Project
    public int ID { get; set; } = -1;
    public string FileFullName { get; set; }
    public string Title { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public ImageSource AlbumImage { get; set; } = null;
    public string AlbumTitle { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;
    public string Lyrics { get; set; } = string.Empty;
  }
  #endregion
}
