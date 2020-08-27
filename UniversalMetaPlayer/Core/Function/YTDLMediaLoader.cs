using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using Newtonsoft.Json.Linq;

using UMP.Core.Function.Online;
using UMP.Core.Global;
using UMP.Core.Model;
using UMP.Core.Model.Media;
using UMP.Utility;

namespace UMP.Core.Function
{
  public class YTDLMediaLoader : IMediaLoader
  {
    private Log Log { get; set; }
    private MediaInformation Information;
    public bool Online { get; private set; }
    public string CachePath { get; set; }

    public event UMP_ProgressChangedEventHandler ProgressChanged;
    private void OnProgressChanged(ProgressKind progressKind, int percentage, string userMessage = "") => ProgressChanged?.Invoke(this, new UMP_ProgressChangedEventArgs(progressKind, percentage, userMessage));

    /// <summary>
    /// 온라인에서 파싱된 Json 오브젝트
    /// </summary>
    private JObject OnlineObject { get; set; }

    private YTDLMediaLoader()
    {
      this.Log = new Log($"{typeof(YTDLMediaLoader)}");
      this.CachePath = GlobalProperty.Predefine.CACHE_PATH;
    }

    public YTDLMediaLoader(string mediaLocation) : this()
    {
      if (!string.IsNullOrWhiteSpace(mediaLocation))
      {
        this.Information = new MediaInformation(mediaLocation);
        if (Checker.IsLocalPath(mediaLocation) && File.Exists(mediaLocation))
        {
          this.Online = false;
          this.Information.MediaStreamPath = Information.MediaLocation;
        }
        else
          this.Online = true;
      }
      else
        this.Information = new MediaInformation();
    }

    public YTDLMediaLoader(MediaInformation mediainfo) : this(mediainfo.MediaLocation) { }

    public async Task<GenericResult<MediaInformation>> GetInformationAsync(bool fullLoad)
    {
      if (Online)
      {
        var result = await TryGetMediaStreamAsync(true);
        if (result)
          Information.MediaStreamPath = result.Result;
        else
          Information.MediaStreamPath = string.Empty;
      }
      var Loadresult = await LocalMediaLoader.TryLoadInfoAsync(Information, fullLoad, Log);

      if (Loadresult)
      {
        Information = Loadresult.Result;
        return new GenericResult<MediaInformation>(true, Information);
      }
      else
      {
        LoadFailProcess();
        return new GenericResult<MediaInformation>(false, Information);
      }
    }

    public async Task<GenericResult<string>> GetStreamPathAsync(bool useCache)
    {
      if (File.Exists(Information.MediaStreamPath))
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
        return await ytdlHelper.DownloadAudioAsync(Information.MediaLocation, CachePath);
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
        return LocalMediaLoader.TryGetOnlineMediaCacheAsync((await GetID()).Result, CachePath);
    }
    #endregion

    #endregion

    #region Other Function
    /// <summary>
    /// 미디어 로드에 실패했을 경우 호출
    /// </summary>
    private void LoadFailProcess()
    {
      if (!Information.Title.ToLower().StartsWith(GlobalProperty.Predefine.MEDIA_INFO_NULL.ToLower()))
      {
        if (string.IsNullOrWhiteSpace(Information.Title))
          Information.Title = $"{GlobalProperty.Predefine.MEDIA_INFO_NULL} {Path.GetFileNameWithoutExtension(Information.MediaLocation)}";
        else
          Information.Title = $"{GlobalProperty.Predefine.MEDIA_INFO_NULL} {Information.Title}";
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
