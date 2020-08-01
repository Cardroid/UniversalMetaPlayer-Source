using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using UMP.Core.Function.Online;
using UMP.Core.Model;
using UMP.Utility;

namespace UMP.Core.Function
{
  public class MediaLoader : IMediaLoader
  {
    private readonly string OnlineMediaCachePath = Path.Combine(GlobalProperty.CACHE_PATH, "OnlineMedia");
    private readonly string FFmpegPath = Path.Combine(GlobalProperty.LIBRARY_PATH, "FFmpeg");
    private Log Log { get; set; }
    public MediaLoader(string medialocation)
    {
      Log = new Log($"{typeof(MediaLoader)}");

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
    public MediaLoader(MediaInfomation mediainfo)
    {
      Log = new Log($"{typeof(MediaLoader)}");

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
        var result = await TryGetMediaStreamAsync();
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

    public async Task<GenericResult<string>> GetStreamPathAsync()
    {
      if(File.Exists(Infomation.MediaStreamPath))
        return new GenericResult<string>(true, Infomation.MediaStreamPath);

      if (Online)
      {
        var result = await TryGetMediaStreamAsync();
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
    /// <returns>성공시 스트림 파일 경로</returns>
    private async Task<GenericResult<string>> TryGetMediaStreamAsync()
    {
      if (!Online)
        return new GenericResult<string>(false);

      var cache = await TryGetMediaCacheAsync();
      if (cache)
        return cache;

      if (Checker.CheckForInternetConnection())
      {
        YTDLHelper ytdlHelper = new YTDLHelper();
        return await ytdlHelper.DownloadAudioAsync(Infomation.MediaLocation, OnlineMediaCachePath);
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
        return new GenericResult<string>(false);
      if(!Directory.Exists(OnlineMediaCachePath))
        return new GenericResult<string>(false);

      var id = await GetID();
      if (!id)
        return new GenericResult<string>(false);

      string[] searchFiles = Directory.GetFiles(OnlineMediaCachePath, $"{id.Result}.mp3", SearchOption.AllDirectories);
      if (searchFiles.Length > 0 && File.Exists(searchFiles[0]))
        return new GenericResult<string>(true, searchFiles[0]);
      else
        return new GenericResult<string>(false);
    }
    #endregion

    #endregion

    #region Other Function
    private const string MEDIA_INFO_NULL = "(null)";
    /// <summary>
    /// 미디어 로드에 실패했을 경우 호출
    /// </summary>
    private void LoadFailProcess()
    {
      if (!Infomation.Title.ToLower().StartsWith(MEDIA_INFO_NULL.ToLower()))
      {
        if (string.IsNullOrWhiteSpace(Infomation.Title))
          Infomation.Title = $"{MEDIA_INFO_NULL} {Path.GetFileNameWithoutExtension(Infomation.MediaLocation)}";
        else
          Infomation.Title = $"{MEDIA_INFO_NULL} {Infomation.Title}";
      }
      Infomation.MediaStreamPath = string.Empty;
    }

    public async Task<GenericResult<string>> GetID()
    {
      if (!Online)
        return new GenericResult<string>(false);

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

    /// <summary>
    /// 정보 구조체
    /// </summary>
    private MediaInfomation Infomation;
    /// <summary>
    /// true = 온라인 미디어, false = 로컬 미디어
    /// </summary>
    public bool Online { get; private set; }
  }
}
