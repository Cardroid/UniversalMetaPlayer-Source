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
    private MediaInfomation Infomation;
    public bool Online { get; private set; }
    /// <summary>
    /// 온라인에서 파싱된 Json 오브젝트
    /// </summary>
    private JObject OnlineObject { get; set; }

    public YTDLMediaLoader(string medialocation)
    {
      Log = new Log($"{typeof(YTDLMediaLoader)}");

      Infomation = new MediaInfomation()
      {
        Title = string.Empty,
        Duration = TimeSpan.Zero,
        AlbumImage = null,
        Tags = new MediaInfoDictionary()
      };

      if (!string.IsNullOrWhiteSpace(medialocation))
      {
        Infomation.MediaLocation = medialocation;
        if (Checker.IsLocalPath(medialocation) && File.Exists(medialocation))
        {
          Online = false;
          Infomation.MediaStreamPath = Infomation.MediaLocation; 
        }
        else
          Online = true;
      }
    }
    public YTDLMediaLoader(MediaInfomation mediainfo)
    {
      Log = new Log($"{typeof(YTDLMediaLoader)}");

      Infomation = mediainfo;

      if (!string.IsNullOrWhiteSpace(mediainfo.MediaLocation))
      {
        Infomation.MediaLocation = mediainfo.MediaLocation;
        if (Checker.IsLocalPath(mediainfo.MediaLocation) && File.Exists(mediainfo.MediaLocation))
        {
          Online = false;
          Infomation.MediaStreamPath = Infomation.MediaLocation; 
        }
        else
          Online = true;
      }
    }

    public async Task<MediaInfomation> GetInfomationAsync(bool fullLoad)
    {
      if (Online)
      {
        var result = await TryGetMediaStreamAsync(true);
        if (result)
          Infomation.MediaStreamPath = result.Result;
        else
          Infomation.MediaStreamPath = string.Empty;
      }
      Infomation.LoadState = await TryLoadInfoAsync(fullLoad);

      if (!Infomation.LoadState)
        LoadFailProcess();

      return Infomation;
    }

    public async Task<GenericResult<string>> GetStreamPathAsync(bool useCache)
    {
      if(File.Exists(Infomation.MediaStreamPath))
        return new GenericResult<string>(true, Infomation.MediaStreamPath);

      if (Online)
      {
        var result = await TryGetMediaStreamAsync(useCache);
        if (result)
        {
          Infomation.MediaStreamPath = result.Result;
          return result;
        }
        else
          return new GenericResult<string>(false);
      }
      else
      {
        Infomation.MediaStreamPath = Infomation.MediaLocation;
        if (File.Exists(Infomation.MediaStreamPath))
          return new GenericResult<string>(true, Infomation.MediaStreamPath);
        else
        {
          LoadFailProcess();
          return new GenericResult<string>(false);
        }
      }
    }

    #region Core

    #region Infomation
    /// <summary>
    /// 미디어 정보 로드 시도
    /// </summary>
    /// <param name="fullload">모든 정보 로드 여부</param>
    /// <returns>로드 성공 여부</returns>
    private async Task<bool> TryLoadInfoAsync(bool fullLoad)
    {
      var path = Infomation.MediaStreamPath;

      if (File.Exists(path))
      {
        await Task.Run(() =>
        {
          using var Fileinfo = TagLib.File.Create(path);
          // 미디어 정보를 정보 클래스에 저장
          Infomation.Title = !string.IsNullOrWhiteSpace(Fileinfo.Tag.Title) ? Fileinfo.Tag.Title : Infomation.Title;
          Infomation.Duration = Fileinfo.Properties.Duration;

          // 모든 정보 로드
          if (fullLoad)
          {
            try { Infomation.AlbumImage = BitmapFrame.Create(new MemoryStream(Fileinfo.Tag.Pictures[0].Data.Data)); }
            catch { Infomation.AlbumImage = null; }
            if (Online && !string.IsNullOrWhiteSpace(Fileinfo.Tag.Album))
              Infomation.Tags[MediaInfoType.AlbumTitle] = Fileinfo.Tag.Album;
            else
              Infomation.Tags[MediaInfoType.AlbumTitle] = Fileinfo.Tag.Album;
            Infomation.Tags[MediaInfoType.AlbumArtist] = Fileinfo.Tag.FirstAlbumArtist;
            Infomation.Tags[MediaInfoType.Lyrics] = Fileinfo.Tag.Lyrics;
          }
          Infomation.LoadState = true;
        });
        return true;
      }
      else
      {
        Log.Error("미디어 파일이 없습니다", $"MediaLocation : [{path}]");
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
        return await ytdlHelper.DownloadAudioAsync(Infomation.MediaLocation, GlobalProperty.OnlineMediaCachePath);
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
      if (!Infomation.Title.ToLower().StartsWith(GlobalProperty.MEDIA_INFO_NULL.ToLower()))
      {
        if (string.IsNullOrWhiteSpace(Infomation.Title))
          Infomation.Title = $"{GlobalProperty.MEDIA_INFO_NULL} {Path.GetFileNameWithoutExtension(Infomation.MediaLocation)}";
        else
          Infomation.Title = $"{GlobalProperty.MEDIA_INFO_NULL} {Infomation.Title}";
      }
      Infomation.MediaStreamPath = string.Empty;
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
        var result = await new YTDLHelper().GetIDAsync(Infomation.MediaLocation);
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
