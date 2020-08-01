using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using UMP.Core.Model;

namespace UMP.Core.Function
{
  public class LocalMediaLoader
  {
    /// <summary>
    /// 로컬 미디어 로드 시도
    /// </summary>
    /// <param name="info">Location이 포함된 <see cref="MediaInfomation"/></param>
    /// <param name="fullLoad">모두 로드 할지 여부 (메모리 소모량 많아 짐)</param>
    /// <param name="log">로거 <see cref="Log"/></param>
    /// <returns>성공시 true</returns>
    public static async Task<GenericResult<MediaInfomation>> TryLoadInfoAsync(MediaInfomation info, bool fullLoad, Log log = null)
    {
      var path = info.MediaStreamPath;

      if (File.Exists(path))
      {
        await Task.Run(() =>
        {
          using var Fileinfo = TagLib.File.Create(path);
          // 미디어 정보를 정보 클래스에 저장
          info.Title = !string.IsNullOrWhiteSpace(Fileinfo.Tag.Title) 
          ? Fileinfo.Tag.Title 
          : $"{info.MediaStreamPath} form Online";
          info.Duration = Fileinfo.Properties.Duration;

          // 모든 정보 로드
          if (fullLoad)
          {
            try { info.AlbumImage = BitmapFrame.Create(new MemoryStream(Fileinfo.Tag.Pictures[0].Data.Data)); }
            catch { info.AlbumImage = null; }
            info.Tags[MediaInfoType.AlbumTitle] = !string.IsNullOrWhiteSpace(Fileinfo.Tag.Album) 
            ? Fileinfo.Tag.Album 
            : $"{info.MediaStreamPath} form Online";
            info.Tags[MediaInfoType.AlbumArtist] = Fileinfo.Tag.FirstAlbumArtist;
            info.Tags[MediaInfoType.Lyrics] = Fileinfo.Tag.Lyrics;
          }
          info.LoadState = true;
        });
        return new GenericResult<MediaInfomation>(true, info);
      }
      else
      {
        if (log != null)
          log.Error("미디어 파일이 없습니다", new FileNotFoundException(), $"MediaLocation : [{path}]");
        return new GenericResult<MediaInfomation>(false);
      }
    }

    /// <summary>
    /// 캐시에서 미디어 스트림 파일 찾기 시도
    /// </summary>
    /// <param name="id">비디오 ID</param>
    /// <returns>성공시 true, 미디어 스트림 파일 경로</returns>
    public static GenericResult<string> TryGetOnlineMediaCacheAsync(string id)
    {
      if (!Directory.Exists(GlobalProperty.OnlineMediaCachePath) || string.IsNullOrWhiteSpace(id))
        return new GenericResult<string>(false);

      string[] searchFiles = Directory.GetFiles(GlobalProperty.OnlineMediaCachePath, $"{id}.mp3", SearchOption.AllDirectories);
      if (searchFiles.Length > 0 && File.Exists(searchFiles[0]))
        return new GenericResult<string>(true, searchFiles[0]);
      else
        return new GenericResult<string>(false);
    }
  }
}
