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
    private MediaInformation Information;
    public bool Online { get; private set; }
    public string CachePath { get; set; }

    public event UMP_ProgressChangedEventHandler ProgressChanged;
    private void OnProgressChanged(ProgressKind progressKind, int percentage, string userMessage = "") => ProgressChanged?.Invoke(this, new UMP_ProgressChangedEventArgs(progressKind, percentage, userMessage));

    private NativeMediaLoader()
    {
      this.Log = new Log($"{typeof(NativeMediaLoader)}");
      this.CachePath = GlobalProperty.Predefine.OnlineMediaCachePath;
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

    public async Task<GenericResult<MediaInformation>> GetInformationAsync(bool fullLoad)
    {
      OnProgressChanged(ProgressKind.Info, 0, "초기화 중...");
      OnProgressChanged(ProgressKind.Info, 5, "미디어 타입 확인 중...");
      if (Online)
      {
          OnProgressChanged(ProgressKind.Info, 10, "미디어 파일 로드 중...");
        var result = await GetYouTubeMediaAsync(true); if (result)
          Information.MediaStreamPath = result.Result;
        else
        {
          OnProgressChanged(ProgressKind.Info, -1, $"Fatal : {result.Result}");
          return new GenericResult<MediaInformation>(false, Information);
        }
      }

      OnProgressChanged(ProgressKind.Info, 20, "미디어 정보 로드 중...");
      MediaLoaderProgress progress = new MediaLoaderProgress();
      progress.ProgressChanged += (percentage, msg) => OnProgressChanged(ProgressKind.InfoLoad, (int)percentage, msg);
      var resultInfo = await LocalMediaLoader.TryLoadInfoAsync(Information, fullLoad, Log, progress);
      if (resultInfo)
      {
        OnProgressChanged(ProgressKind.Info, 100, "완료");
        Information = resultInfo.Result;
        return new GenericResult<MediaInformation>(true, Information);
      }
      else
      {
        LoadFailProcess();
        OnProgressChanged(ProgressKind.Info, -1, "미디어 정보 로드 실패");
        return new GenericResult<MediaInformation>(false, Information);
      }
    }

    public async Task<GenericResult<string>> GetStreamPathAsync(bool useCache)
    {
      OnProgressChanged(ProgressKind.Stream, 0, "초기화 중...");
      if (Online)
      {
        var result = await GetYouTubeMediaAsync(useCache);
        if (result)
        {
          Information.MediaStreamPath = result.Result;
          OnProgressChanged(ProgressKind.Stream, 100, "완료");
        }
        else
          OnProgressChanged(ProgressKind.StreamDownload, -1, $"Fatal : {result.Result}");
        return result;
      }
      else
      {
        if (File.Exists(Information.MediaStreamPath))
        {
          OnProgressChanged(ProgressKind.Stream, 100, "완료");
          return new GenericResult<string>(true, Information.MediaStreamPath);
        }
        else
        {
          OnProgressChanged(ProgressKind.StreamDownload, -1, $"Fatal : 파일이 없습니다");
          return new GenericResult<string>(false);
        }
      }
    }

    #region Stream
    /// <summary>
    ///  YouTube 미디어 스트림 & 정보 다운로드시도
    /// </summary>
    /// <param name="useCache">캐시된 미디어 사용 여부</param>
    /// <returns>스트림 캐시 저장 경로</returns>
    private async Task<GenericResult<string>> GetYouTubeMediaAsync(bool useCache)
    {
      OnProgressChanged(ProgressKind.StreamLoad, 0, "초기화 중...");
      if (!Online)
      {
        Log.Error(PredefineMessage.IsNotOnlineMedia);
        OnProgressChanged(ProgressKind.StreamLoad, -1, $"Error : {PredefineMessage.IsNotOnlineMedia}");
        return new GenericResult<string>(false);
      }

      // 캐시된 정보를 로드
      if (useCache)
      {
        OnProgressChanged(ProgressKind.StreamLoad, 5, "캐시 검색 중...");
        var streamCachePathResult = LocalMediaLoader.TryGetOnlineMediaCacheAsync((await GetID()).Result, CachePath);
        if (streamCachePathResult)
        {
          Log.Debug("캐시된 미디어가 확인됨");
          OnProgressChanged(ProgressKind.StreamLoad, 100, "캐시 로드 완료");
          return new GenericResult<string>(true, streamCachePathResult.Result);
        }
        else
        {
          Log.Warn("캐시 된 미디어 로드 실패. 온라인에서 다운로드 시도");
          OnProgressChanged(ProgressKind.StreamLoad, 10, "캐시 로드 실패...");
        }
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
          OnProgressChanged(ProgressKind.StreamLoad, 15, "다운로드 중...");
          var streamPathResult = await TryDownloadYouTubeStreamAsync(CachePath);
          if (streamPathResult)
            streamPath = streamPathResult.Result;
          else
            throw new WebException("미디어 스트림 다운로드 실패");


          // Mp3로 변환
          OnProgressChanged(ProgressKind.StreamLoad, 40, "변환 중...");
          MediaLoaderProgress progress = new MediaLoaderProgress();
          progress.ProgressChanged += (percentage, msg) => OnProgressChanged(ProgressKind.StreamConvert, (int)percentage, msg);
          if (!await Converter.ConvertToMP3Async(streamPath, mp3FilePath, progress))
            throw new FileNotFoundException($"File is Null\nSourceFile : [{streamPath}]\nTargetFile : [{mp3FilePath}]");
          File.Delete(streamPath);
          if (File.Exists(mp3FilePath))
            Information.MediaStreamPath = mp3FilePath;
          else
            throw new FileNotFoundException("미디어 스트림 변환 실패 (변환을 완료 했지만, 파일을 찾을 수 없습니다)");
          Log.Info("미디어 스트림 변환 완료");


          // 메타 데이터 저장
          OnProgressChanged(ProgressKind.StreamLoad, 75, "메타데이터 저장 중...");
          if (await TryYouTubeMetaDataSave())
          {
            Log.Info("미디어 메타데이터 다운로드 & 병합 완료");
          OnProgressChanged(ProgressKind.StreamLoad, 100, "완료");
          }
          else
            throw new InvalidCastException("미디어 메타데이터 다운로드 & 병합 실패");
        }
        catch (Exception e)
        {
          Log.Fatal("미디어 스트림 처리 중 오류 발생", e, $"MediaLocation : [{Information.MediaLocation}]\nMp3Path : [{mp3FilePath}]\nStreamPath : [{streamPath}]");
          OnProgressChanged(ProgressKind.StreamLoad, -1, $"Fatal : {e.Message}");
          return new GenericResult<string>(false);
        }
        return new GenericResult<string>(true, mp3FilePath);
      }
      else
      {
        Log.Error(PredefineMessage.UnableNetwork);
        OnProgressChanged(ProgressKind.StreamLoad, -1, $"Error : {PredefineMessage.UnableNetwork}");
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
      OnProgressChanged(ProgressKind.StreamDownload, 0, "초기화 중...");
      Checker.DirectoryCheck(path);

      if (Checker.CheckForInternetConnection())
      {
        // 미디어 스트림 다운로드
        var youtube = new YoutubeClient();
        try
        {
          OnProgressChanged(ProgressKind.StreamDownload, 5, "매니페스트 가져오는 중...");
          var streamManifest = await youtube.Videos.Streams.GetManifestAsync(Information.MediaLocation);
          var streamInfo = streamManifest.GetAudioOnly().WithHighestBitrate();

          if (streamInfo == null)
            throw new NullReferenceException("스트림 정보가 Null입니다");

          string savepath = Path.Combine(path, $"{(await GetID()).Result}.{streamInfo.Container}");

          // Download the stream to file
          OnProgressChanged(ProgressKind.StreamDownload, 10, "스트림 다운로드 시작 중...");
          MediaLoaderProgress progress = new MediaLoaderProgress();
          progress.ProgressChanged += (percentage, msg) => OnProgressChanged(ProgressKind.StreamDownload, (int)(percentage * 90) + 10, msg);
          await youtube.Videos.Streams.DownloadAsync(streamInfo, savepath, progress);

          if (string.IsNullOrWhiteSpace(savepath))
            throw new FileNotFoundException("스트림 파일을 찾을 수 없습니다");

          Log.Info("온라인에서 미디어 스트림 로드 완료");
          OnProgressChanged(ProgressKind.StreamDownload, 100, "완료");
          return new GenericResult<string>(true, savepath);
        }
        catch (Exception e)
        {
          Log.Fatal("미디어 스트림 다운로드 실패", e);
          return new GenericResult<string>(false, e.Message);
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

      OnProgressChanged(ProgressKind.InfoSave, 0, "초기화 중...");
      OnProgressChanged(ProgressKind.InfoSave, 5, "파일 로드 중...");
      if (string.IsNullOrWhiteSpace(Information.MediaStreamPath) && !File.Exists(Information.MediaStreamPath))
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

          OnProgressChanged(ProgressKind.InfoSave, 10, "메타데이터 다운로드 중...");
          try
          {
            OnProgressChanged(ProgressKind.InfoDownload, 0, "초기화 중...");
            OnProgressChanged(ProgressKind.InfoDownload, 5, "정보 다운로드 중...");
            videoinfo = await youtube.Videos.GetAsync(Information.MediaLocation);
            OnProgressChanged(ProgressKind.InfoDownload, 100, "완료");
          }
          catch (Exception e)
          {
            Log.Fatal("네트워크에서 정보 로드 실패", e);
            OnProgressChanged(ProgressKind.InfoDownload, -1, $"Fatal : {e.Message}");
            OnProgressChanged(ProgressKind.InfoSave, -1, "Fatal : 정보 다운로드 실패");
            success = false;
            return;
          }

          OnProgressChanged(ProgressKind.InfoSave, 20, "파일 정보 생성 중...");
          TagLib.File Fileinfo = null;
          try { Fileinfo = TagLib.File.Create(Information.MediaStreamPath); }
          catch (Exception e)
          {
            Log.Fatal("Mp3 파일 열기 또는 메타정보 로드 실패", e);
            Fileinfo?.Dispose();
            success = false;
            OnProgressChanged(ProgressKind.InfoSave, -1, $"Fatal : {e.Message}");
            return;
          }

          // 기본정보 처리
          OnProgressChanged(ProgressKind.InfoSave, 35, "기본 정보 처리 중...");
          Fileinfo.Tag.Title = videoinfo.Title;
          Fileinfo.Tag.Album = $"\"{videoinfo.Url}\" form Online";
          Fileinfo.Tag.AlbumArtists = new string[] { videoinfo.Author };
          Fileinfo.Tag.Description = $"YouTubeID : \"{videoinfo.Id}\"";

          // 가사(자막) 처리
          OnProgressChanged(ProgressKind.InfoSave, 45, "가사(자막) 처리 중...");
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
            OnProgressChanged(ProgressKind.InfoSave, 50, "Warn : 가사(자막) 로드 밎 파싱 실패");
          }

          // 섬네일 처리
          OnProgressChanged(ProgressKind.InfoSave, 55, "섬네일 다운로드 중...");
          byte[] imagedata = null;
          using (WebClient webClient = new WebClient())
          {
            try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.MaxResUrl); }
            catch
            {
              OnProgressChanged(ProgressKind.InfoSave, 56, "MaxRes 화질 다운로드 실패, HighRes 화질 시도");
              try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.HighResUrl); }
              catch
              {
                OnProgressChanged(ProgressKind.InfoSave, 57, "HighRes 화질 다운로드 실패, StandardRes 화질 시도");
                try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.StandardResUrl); }
                catch
                {
                  OnProgressChanged(ProgressKind.InfoSave, 58, "StandardRes 화질 다운로드 실패, MediumRes 화질 시도");
                  try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.MediumResUrl); }
                  catch
                  {
                    OnProgressChanged(ProgressKind.InfoSave, 59, "MediumRes 화질 다운로드 실패, LowRes 화질 시도");
                    try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.LowResUrl); }
                    catch (Exception e)
                    {
                      Log.Warn("Thumbnail 다운로드 중 오류가 발생했습니다", e);
                      OnProgressChanged(ProgressKind.InfoSave, 60, $"Warn : {e.Message}");
                      imagedata = null;
                    }
                  }
                }
              }
            }
          }

          OnProgressChanged(ProgressKind.InfoSave, 70, "섬네일 처리 중...");
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
              Log.Warn("메타데이터에 섬네일 정보등록을 실패했습니다", e);
              OnProgressChanged(ProgressKind.InfoSave, 75, $"Warn : {e.Message}");
            }
          }
          else
          {
            Log.Warn("섬네일 추출 실패", new NullReferenceException("Image data is Null"));
            OnProgressChanged(ProgressKind.InfoSave, 75, "Warn : 섬네일 추출 실패 (Image data is Null)");
          }

          OnProgressChanged(ProgressKind.InfoSave, 85, "파일에 정보 쓰는 중...");
          try
          {
            Fileinfo.Save();
            Log.Info("YouTube에서 Mp3 메타 데이터 저장 완료");
            success = true;
            OnProgressChanged(ProgressKind.InfoSave, 100, "완료");
          }
          catch (Exception e)
          {
            Log.Fatal("메타데이터에 저장에 실패했습니다", e);
            success = false;
            OnProgressChanged(ProgressKind.InfoSave, -1, $"Fatal : {e.Message}");
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
        OnProgressChanged(ProgressKind.InfoSave, -1, $"Error : {PredefineMessage.UnableNetwork}");
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
