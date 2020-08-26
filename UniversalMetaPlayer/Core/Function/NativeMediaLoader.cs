using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using UMP.Core.Global;
using UMP.Core.Model;
using UMP.Core.Model.Media;
using UMP.Utility;

using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace UMP.Core.Function
{
  public class NativeMediaLoader : IMediaLoader
  {
    Log Log { get; }
    /// <summary>
    /// 정보 구조체
    /// </summary>
    private MediaInformation Information;
    public bool Online { get; private set; }
    public string CachePath { get; set; }

    public event UMP_ProgressChangedEventHandler ProgressChanged;
    private void OnProgressChanged(ProgressKind progressKind, int percentage, string userMessage = "") => ProgressChanged?.Invoke(this, new UMP_ProgressChangedEventArgs(progressKind, percentage, userMessage));

    private NativeMediaLoader()
    {
      this.Log = new Log($"{typeof(NativeMediaLoader)}");
      this.CachePath = GlobalProperty.Predefine.CACHE_PATH;
    }

    public NativeMediaLoader(string mediaLocation) : this()
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

    public NativeMediaLoader(MediaInformation mediaInfo) : this(mediaInfo.MediaLocation) { }

    public async Task<GenericResult<string>> GetID()
    {
      if (!Online)
      {
        Log.Error(PredefineMessage.IsNotOnlineMedia);
        return new GenericResult<string>(false);
      }

      VideoId? id = null;
      await Task.Run(() => { id = VideoId.TryParse(Information.MediaLocation); });

      if (id.HasValue)
        return new GenericResult<string>(true, id.Value.ToString());
      else
        return new GenericResult<string>(false);
    }

    public async Task<MediaInformation> GetInformationAsync(bool fullLoad)
    {
      if (Online)
      {
        var result = await GetYouTubeMediaAsync(true);
        if (result)
          Information.MediaStreamPath = result.Result;
      }

      var resultInfo = await LocalMediaLoader.TryLoadInfoAsync(Information, fullLoad, Log);
      if (resultInfo)
        Information = resultInfo.Result;
      else
        LoadFailProcess();

      return Information;
    }

    public async Task<GenericResult<string>> GetStreamPathAsync(bool useCache)
    {
      if (Online)
      {
        var result = await GetYouTubeMediaAsync(useCache);
        if (result)
          Information.MediaStreamPath = result.Result;
        return result;
      }
      else
      {
        if (File.Exists(Information.MediaStreamPath))
          return new GenericResult<string>(true, Information.MediaStreamPath);
        else
          return new GenericResult<string>(false);
      }
    }

    #region Stream
    /// <summary>
    /// YouTube 미디어 스트림 & 정보 다운로드시도
    /// </summary>
    /// <returns>스트림 캐시 저장 경로</returns>
    private async Task<GenericResult<string>> GetYouTubeMediaAsync(bool useCache)
    {
      if (!Online)
      {
        Log.Error(PredefineMessage.IsNotOnlineMedia);
        return new GenericResult<string>(false);
      }

      OnProgressChanged(ProgressKind.Stream, 0, "Initializing...");

      // 임시저장된 정보를 로드
      if (useCache)
      {
        var streamCachePathResult = LocalMediaLoader.TryGetOnlineMediaCacheAsync((await GetID()).Result, CachePath);
        if (streamCachePathResult)
        {
          Log.Debug("캐시에서 미디어가 확인됨");
          return new GenericResult<string>(true, streamCachePathResult.Result);
        }
        else
          Log.Warn("캐시 된 미디어 로드 실패. 온라인에서 다운로드를 시도합니다");
      }

      if (Checker.CheckForInternetConnection())
      {
        // 캐시폴더가 존재하지 않을시 생성
        Checker.DirectoryCheck(CachePath);

        string mp3FilePath = Path.Combine(CachePath, $"{(await GetID()).Result}.mp3");
        string streamPath = string.Empty;
        try
        {
          // 유튜브 스트림 다운로드
          OnProgressChanged(ProgressKind.Stream, 5, "Downloading Stream from Network...");
          var streamPathResult = await TryDownloadYouTubeStreamAsync(CachePath);
          if (streamPathResult)
            streamPath = streamPathResult.Result;
          else
            throw new Exception("Failed to download YouTube stream");


          // Mp3로 변환
          OnProgressChanged(ProgressKind.Stream, 40, "Converting to Mp3...");
          MediaLoaderProgress progress = new MediaLoaderProgress();
          progress.ProgressChanged += (percentage, msg) => OnProgressChanged(ProgressKind.StreamConvert, (int)percentage, msg);
          if (!await Converter.ConvertToMP3Async(streamPath, mp3FilePath, progress))
            throw new FileNotFoundException($"File is Null\nSourceFile : [{streamPath}]\nTargetFile : [{mp3FilePath}]");
          File.Delete(streamPath);
          if (File.Exists(mp3FilePath))
            Information.MediaStreamPath = mp3FilePath;
          else
            throw new FileNotFoundException("Media Stream Mp3 Conversion Error (변환을 완료 했지만, 파일을 찾을 수 없습니다)");
          Log.Info("미디어 스트림 Mp3 변환 완료");


          // 메타 데이터 저장
          OnProgressChanged(ProgressKind.Stream, 75, "MetaData Saving...");
          if (await TryYouTubeMetaDataSave())
          {
            Log.Info("미디어 메타데이터 다운로드 & 병합 완료");
            OnProgressChanged(ProgressKind.Stream, 100, "Finished");
          }
          else
            throw new InvalidCastException("Error merging Media Metadata");
        }
        catch (Exception e)
        {
          Log.Fatal("미디어 스트림 다운로드 & 변환 실패", e, $"MediaLocation : [{Information.MediaLocation}]\nMp3Path : [{mp3FilePath}]\nStreamPath : [{streamPath}]");
          OnProgressChanged(ProgressKind.Stream, -1, $"Fatal : {e.Message}");
          return new GenericResult<string>(false);
        }
        return new GenericResult<string>(true, mp3FilePath);
      }
      else
      {
        Log.Error(PredefineMessage.UnableNetwork);
          OnProgressChanged(ProgressKind.Stream, -1, $"Error : {PredefineMessage.UnableNetwork}");
        return new GenericResult<string>(false);
      }
    }

    /// <summary>
    /// 온라인에서 유튜브 스트림 다운로드를 시도합니다.
    /// </summary>
    /// <param name="path">저장할 폴더 경로</param>
    /// <returns>다운로드 성공시 스트림 저장 폴더 경로</returns>
    private async Task<GenericResult<string>> TryDownloadYouTubeStreamAsync(string path)
    {
      Checker.DirectoryCheck(path);
      OnProgressChanged(ProgressKind.StreamDownload, 0, "Initializing...");

      if (Checker.CheckForInternetConnection())
      {
        // 미디어 스트림 다운로드
        var youtube = new YoutubeClient();
        try
        {
          var streamManifest = await youtube.Videos.Streams.GetManifestAsync(Information.MediaLocation);
          var streamInfo = streamManifest.GetAudioOnly().WithHighestBitrate();

          if (streamInfo == null)
            throw new NullReferenceException("Stream Info is Null");

          string savepath = Path.Combine(path, $"{(await GetID()).Result}.{streamInfo.Container}");

          // Download the stream to file
          MediaLoaderProgress progress = new MediaLoaderProgress();
          progress.ProgressChanged += (percentage, msg) => OnProgressChanged(ProgressKind.StreamDownload, (int)(percentage * 100), msg);
          await youtube.Videos.Streams.DownloadAsync(streamInfo, savepath, progress);

          if (string.IsNullOrWhiteSpace(savepath))
            throw new FileNotFoundException("Stream File Not Found");

          Log.Info("온라인에서 미디어 스트림 로드 완료");
          OnProgressChanged(ProgressKind.StreamDownload, 100, "Finished");
          return new GenericResult<string>(true, savepath);
        }
        catch (Exception e)
        {
          Log.Fatal("미디어 스트림 다운로드 실패", e);
          OnProgressChanged(ProgressKind.StreamDownload, -1, $"Fatal : {e.Message}");
          return new GenericResult<string>(false);
        }
      }
      else
      {
        Log.Error(PredefineMessage.UnableNetwork);
        OnProgressChanged(ProgressKind.StreamDownload, -1, $"Error : {PredefineMessage.UnableNetwork}");
        return new GenericResult<string>(false);
      }
    }
    #endregion

    #region Info
    /// <summary>
    /// 유튜브에서 정보를 다운로드하여 다운로드된 스트림에 저장합니다<br/>
    /// (Mp3로 변환후 저장 권장)
    /// </summary>
    private async Task<bool> TryYouTubeMetaDataSave()
    {
      if (!Online)
      {
        Log.Error(PredefineMessage.IsNotOnlineMedia);
        return false;
      }

      OnProgressChanged(ProgressKind.MetaDataSave, 0, "Initializing...");
      OnProgressChanged(ProgressKind.MetaDataSave, 5, "File Loading...");
      if (!File.Exists(Information.MediaStreamPath))
      {
        Log.Fatal("파일이 없습니다", new FileNotFoundException("File Not Found"), $"Path : [{Information.MediaStreamPath}]");
        return false;
      }

      if (Checker.CheckForInternetConnection())
      {
        bool success = false;
        await Task.Run(async () =>
        {
          YoutubeClient youtube = new YoutubeClient();
          Video videoinfo = null;

          OnProgressChanged(ProgressKind.MetaDataSave, 10, "Loading Information from Network...");
          try { videoinfo = await youtube.Videos.GetAsync(Information.MediaLocation); }
          catch (Exception e)
          {
            Log.Fatal("네트워크에서 정보 로드 실패", e);
            OnProgressChanged(ProgressKind.MetaDataSave, -1, $"Fatal : {e.Message}");
            success = false;
            return;
          }

          OnProgressChanged(ProgressKind.MetaDataSave, 20, "Creating File Information...");
          TagLib.File Fileinfo = null;
          try { Fileinfo = TagLib.File.Create(Information.MediaStreamPath); }
          catch (Exception e)
          {
            Log.Fatal("Mp3 파일 열기 또는 메타정보 로드 실패", e);
            Fileinfo?.Dispose();
            success = false;
            OnProgressChanged(ProgressKind.MetaDataSave, -1, $"Fatal : {e.Message}");
            return;
          }

          // 기본정보 처리
          OnProgressChanged(ProgressKind.MetaDataSave, 35, "Processing Basic Information...");
          Fileinfo.Tag.Title = videoinfo.Title;
          Fileinfo.Tag.Album = $"\"{videoinfo.Url}\" form Online";
          Fileinfo.Tag.AlbumArtists = new string[] { videoinfo.Author };
          Fileinfo.Tag.Description = $"YouTubeID : \"{videoinfo.Id}\"";

          // 자막 처리
          OnProgressChanged(ProgressKind.MetaDataSave, 45, "Processing Subtitle...");
          try
          {
            var trackManifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(Information.MediaLocation);

            var trackInfo = trackManifest.TryGetByLanguage("ko");

            if (trackInfo != null)
            {
              var track = await youtube.Videos.ClosedCaptions.GetAsync(trackInfo);

              string caption = string.Empty;

              for (int i = 0; i < track.Captions.Count; i++)
                caption += $"{track.Captions[i].Text}\n";

              Fileinfo.Tag.Lyrics = caption[0..^1];
            }
          }
          catch (Exception e) 
          {
            Log.Warn("자막 로드 및 파싱 실패", e);
          OnProgressChanged(ProgressKind.MetaDataSave, 50, "Warn : Subtitle Loading and Parsing Failure");
          }

          // Thumbnail 처리
          OnProgressChanged(ProgressKind.MetaDataSave, 55, "Downloading Thumbnails from the Network...");
          byte[] imagedata = null;
          using (WebClient webClient = new WebClient())
          {
            try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.MaxResUrl); }
            catch
            {
              OnProgressChanged(ProgressKind.MetaDataSave, 56, "MaxRes Quality Download Failed, Try HighRes Quality");
              try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.HighResUrl); }
              catch
              {
                OnProgressChanged(ProgressKind.MetaDataSave, 57, "HighRes Quality Download Failed, Try StandardRes Quality");
                try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.StandardResUrl); }
                catch
                {
                  OnProgressChanged(ProgressKind.MetaDataSave, 58, "StandardRes Quality Download Failed, Try MediumRes Quality");
                  try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.MediumResUrl); }
                  catch
                  {
                    OnProgressChanged(ProgressKind.MetaDataSave, 59, "MediumRes Quality Download Failed, Try LowRes Quality");
                    try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.LowResUrl); }
                    catch (Exception e)
                    {
                      Log.Error("Thumbnail 다운로드 중 오류가 발생했습니다", e);
                      OnProgressChanged(ProgressKind.MetaDataSave, 60, $"Error : {e.Message}");
                      imagedata = null;
                    }
                  }
                }
              }
            }
          }

          OnProgressChanged(ProgressKind.MetaDataSave, 70, "Processing Thumbnail...");
          if (imagedata != null)
          {
            try
            {
              Fileinfo.Tag.Pictures = new TagLib.IPicture[]
              {
                new TagLib.Picture(new TagLib.ByteVector(imagedata))
                {
                  Type = TagLib.PictureType.FrontCover,
                  Description = "Cover"
                }
              };
            }
            catch (Exception e)
            {
              Log.Error("메타데이터에 Thumbnail 정보등록을 실패했습니다", e);
              OnProgressChanged(ProgressKind.MetaDataSave, 75, $"Error : {e.Message}");
            }
          }
          else
          {
            Log.Error("Thumbnail 추출 실패", new NullReferenceException("Image data is Null"));
            OnProgressChanged(ProgressKind.MetaDataSave, 75, "Error : Thumbnail extraction failed (Image data is Null)");
          }

          OnProgressChanged(ProgressKind.MetaDataSave, 85, "Saving File Information...");
          try
          {
            Fileinfo.Save();
            Log.Info("YouTube에서 Mp3 메타 데이터 저장 완료");
            success = true;
          OnProgressChanged(ProgressKind.MetaDataSave, 100, "Finished");
          }
          catch (Exception e)
          {
            Log.Fatal("메타데이터에 작성 & 저장에 실패했습니다", e);
            success = false;
            OnProgressChanged(ProgressKind.MetaDataSave, -1, $"Fatal : {e.Message}");
            return;
          }
          finally { Fileinfo.Dispose(); }
          return;
        });
        return success;
      }
      else
      {
        Log.Error(PredefineMessage.UnableNetwork);
        OnProgressChanged(ProgressKind.MetaDataSave, -1, $"Error : {PredefineMessage.UnableNetwork}");
        return false;
      }
    }
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
    #endregion
  }
}
