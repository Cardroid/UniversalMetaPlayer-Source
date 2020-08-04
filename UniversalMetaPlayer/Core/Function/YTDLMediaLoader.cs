using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using Newtonsoft.Json.Linq;

using UMP.Core.Function.Online;
using UMP.Core.Model;
using UMP.Utility;

namespace UMP.Core.Function
{
  public class YTDLMediaLoader : IMediaLoader
  {
    private Log Log { get; set; }

    /// <summary>
    /// 정보 구조체
    /// </summary>
    private MediaInformation Information;
    public bool Online { get; private set; }
    /// <summary>
    /// 온라인에서 파싱된 Json 오브젝트
    /// </summary>
    private JObject OnlineObject { get; set; }

    public YTDLMediaLoader(string medialocation)
    {
      Log = new Log($"{typeof(YTDLMediaLoader)}");

      Information = new MediaInformation()
      {
        Title = string.Empty,
        Duration = TimeSpan.Zero,
        AlbumImage = null,
        Tags = new MediaInfoDictionary()
      };

      if (!string.IsNullOrWhiteSpace(medialocation))
      {
        Information.MediaLocation = medialocation;
        if (Checker.IsLocalPath(medialocation) && File.Exists(medialocation))
        {
          Online = false;
          Information.MediaStreamPath = Information.MediaLocation; 
        }
        else
          Online = true;
      }
    }
    public YTDLMediaLoader(MediaInformation mediainfo)
    {
      Log = new Log($"{typeof(YTDLMediaLoader)}");

      Information = mediainfo;

      if (!string.IsNullOrWhiteSpace(mediainfo.MediaLocation))
      {
        Information.MediaLocation = mediainfo.MediaLocation;
        if (Checker.IsLocalPath(mediainfo.MediaLocation) && File.Exists(mediainfo.MediaLocation))
        {
          Online = false;
          Information.MediaStreamPath = Information.MediaLocation; 
        }
        else
          Online = true;
      }
    }

    public async Task<MediaInformation> GetInformationAsync(bool fullLoad)
    {
      if (Online)
      {
        var result = await TryGetMediaStreamAsync(true);
        if (result)
          Information.MediaStreamPath = result.Result;
        else
          Information.MediaStreamPath = string.Empty;
      }
      Information.LoadState = await TryLoadInfoAsync(fullLoad);

      if (!Information.LoadState)
        LoadFailProcess();

      return Information;
    }

    public async Task<GenericResult<string>> GetStreamPathAsync(bool useCache)
    {
      if(File.Exists(Information.MediaStreamPath))
        return new GenericResult<string>(true, Information.MediaStreamPath);

      if (Online)
      {
        var result = await TryGetMediaStreamAsync(useCache);
        if (result)
        {
          Information.MediaStreamPath = result.Result;
          return result;
        }
        else
          return new GenericResult<string>(false);
      }
      else
      {
        Information.MediaStreamPath = Information.MediaLocation;
        if (File.Exists(Information.MediaStreamPath))
          return new GenericResult<string>(true, Information.MediaStreamPath);
        else
        {
          LoadFailProcess();
          return new GenericResult<string>(false);
        }
      }
    }

    #region Core

    #region Information
    /// <summary>
    /// 미디어 정보 로드 시도
    /// </summary>
    /// <param name="fullload">모든 정보 로드 여부</param>
    /// <returns>로드 성공 여부</returns>
    private async Task<bool> TryLoadInfoAsync(bool fullLoad)
    {
      var path = Information.MediaStreamPath;

      if (File.Exists(path))
      {
        await Task.Run(() =>
        {
          using var Fileinfo = TagLib.File.Create(path);
          // 미디어 정보를 정보 클래스에 저장
          Information.Title = !string.IsNullOrWhiteSpace(Fileinfo.Tag.Title) ? Fileinfo.Tag.Title : Information.Title;
          Information.Duration = Fileinfo.Properties.Duration;

          // 모든 정보 로드
          if (fullLoad)
          {
            try { Information.AlbumImage = BitmapFrame.Create(new MemoryStream(Fileinfo.Tag.Pictures[0].Data.Data)); }
            catch { Information.AlbumImage = null; }
            if (Online && !string.IsNullOrWhiteSpace(Fileinfo.Tag.Album))
              Information.Tags[MediaInfoType.AlbumTitle] = Fileinfo.Tag.Album;
            else
              Information.Tags[MediaInfoType.AlbumTitle] = Fileinfo.Tag.Album;
            Information.Tags[MediaInfoType.AlbumArtist] = Fileinfo.Tag.FirstAlbumArtist;
            Information.Tags[MediaInfoType.Lyrics] = Fileinfo.Tag.Lyrics;
          }
          Information.LoadState = true;
        });
        return true;
      }
      else
      {
        Log.Fatal("미디어 파일이 없습니다", $"MediaLocation : [{path}]");
        return false;
      }
    }
    #endregion

    #region Stream
    /// <summary>
    /// 미디어 스트림 가져오기 시도
    /// </summary>
    /// <returns>성공시 true, 스트림 파일 경로</returns>
    private async Task<GenericResult<string>> TryGetMediaStreamAsync(bool useCache)
    {
      if (!Online)
      {
        Log.Error(PredefineMessage.IsNotOnlineMedia);
        return new GenericResult<string>(false);
      }

      if (useCache)
      {
        var cache = await TryGetMediaCacheAsync();
        if (cache)
          return cache;
      }

      if (Checker.CheckForInternetConnection())
      {
        YTDLHelper ytdlHelper = new YTDLHelper();
        return await ytdlHelper.DownloadAudioAsync(Information.MediaLocation, GlobalProperty.StaticValues.OnlineMediaCachePath);
      }
      else
        return new GenericResult<string>(false);
    }

    /// <summary>
    /// 캐시에서 미디어 스트림 파일 찾기 시도
    /// </summary>
    /// <returns>성공시 true, 미디어 스트림 파일 경로</returns>
    private async Task<GenericResult<string>> TryGetMediaCacheAsync()
    {
      if (!Online)
      {
        Log.Error(PredefineMessage.IsNotOnlineMedia);
        return new GenericResult<string>(false);
      }
      else
        return LocalMediaLoader.TryGetOnlineMediaCacheAsync((await GetID()).Result);
    }
    #endregion

    #endregion

    #region Other Function
    /// <summary>
    /// 미디어 로드에 실패했을 경우 호출
    /// </summary>
    private void LoadFailProcess()
    {
      if (!Information.Title.ToLower().StartsWith(GlobalProperty.StaticValues.MEDIA_INFO_NULL.ToLower()))
      {
        if (string.IsNullOrWhiteSpace(Information.Title))
          Information.Title = $"{GlobalProperty.StaticValues.MEDIA_INFO_NULL} {Path.GetFileNameWithoutExtension(Information.MediaLocation)}";
        else
          Information.Title = $"{GlobalProperty.StaticValues.MEDIA_INFO_NULL} {Information.Title}";
      }
      Information.MediaStreamPath = string.Empty;
    }

    public async Task<GenericResult<string>> GetID()
    {
      if (!Online)
      {
        Log.Error(PredefineMessage.IsNotOnlineMedia);
        return new GenericResult<string>(false);
      }

      if (!string.IsNullOrWhiteSpace(ID))
        return new GenericResult<string>(true, ID);
      else
      {
        var result = await new YTDLHelper().GetIDAsync(Information.MediaLocation);
        if (result)
        {
          ID = result.Result;
          return result;
        }
        else
          return new GenericResult<string>(false);
      }
    }
    private string ID { get; set; }
    #endregion
  }
}
