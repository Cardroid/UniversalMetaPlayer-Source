using System;
using System.IO;

using UMP.Lib.Utility;

namespace UMP.Lib.Media.Model
{
  public class MediaInformation : IMediaInformation
  {
    private MediaInformation()
    {
    }

    private static MediaInformation Create()
    {
      var instence = new MediaInformation
      {
        LoadedStatus = MediaInfoLoadedStatus.Not,
        IsOnlineMedia = false,
        MediaLocation = string.Empty,
        MediaStreamPath = string.Empty,
        Title = string.Empty,
        Duration = TimeSpan.Zero,
        AlbumImage = null,
        Tags = new MediaInfoTags()
      };

      return instence;
    }

    public static MediaInformation Create(string mediaLocation)
    {
      if (string.IsNullOrWhiteSpace(mediaLocation))
        throw new NullReferenceException("MediaLocation is Null");

      bool isOnline = false;
      if (Checker.GetChecker().IsLocalPath(mediaLocation))
      {
        if (!File.Exists(mediaLocation))
          throw new FileNotFoundException("MediaLocation is Local path, but no file found");
      }
      else
        isOnline = Parser.GetParser().GetUrlInfo(mediaLocation).Success;

      var instence = Create();
      instence.MediaLocation = mediaLocation;
      instence.LoadedStatus = MediaInfoLoadedStatus.Not;
      instence.IsOnlineMedia = isOnline;

      return instence;
    }

    public void InformationLoad(IMediaInfoLoader loader)
    {
      loader.CachePath
    }
    
    public void StreamLoad(IMediaStreamLoader loader)
    {
      
    }

    /// <summary>
    /// 정보 로드 상태
    /// </summary>
    public MediaInfoLoadedStatus LoadedStatus { get; set; }

    /// <summary>
    /// 온라인 미디어 여부
    /// </summary>
    public bool IsOnlineMedia { get; set; }

    /// <summary>
    /// 파일의 위치 (온라인, 로컬)
    /// </summary>
    public string MediaLocation { get; set; }

    /// <summary>
    /// 실제 스트림 파일의 위치 (무조건 로컬)
    /// </summary>
    public string MediaStreamPath { get; set; }

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
    public byte[][] AlbumImage { get; set; }

    /// <summary>
    /// 기타 테그들
    /// </summary>
    public MediaInfoTags Tags { get; set; }
  }
}