using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using UMP.Core.Model;
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
    private MediaInfomation Infomation;
    public bool Online { get; private set; }

    public NativeMediaLoader(string mediaLocation)
    {
      Log = new Log($"{typeof(NativeMediaLoader)}");

      Infomation = new MediaInfomation()
      {
        Title = string.Empty,
        Duration = TimeSpan.Zero,
        AlbumImage = null,
        Tags = new MediaInfoDictionary()
      };

      if (!string.IsNullOrWhiteSpace(mediaLocation))
      {
        Infomation.MediaLocation = mediaLocation;
        if (Checker.IsLocalPath(mediaLocation) && File.Exists(mediaLocation))
        {
          Online = false;
          Infomation.MediaStreamPath = Infomation.MediaLocation;
        }
        else
          Online = true;
      }
    }
    public NativeMediaLoader(MediaInfomation mediainfo)
    {
      Log = new Log($"{typeof(NativeMediaLoader)}");

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

    public async Task<GenericResult<string>> GetID()
    {
      if (!Online)
      {
        Log.Error(PredefineMessage.IsNotOnlineMedia);
        return new GenericResult<string>(false);
      }

      VideoId? id = null;
      await Task.Run(() => { id = VideoId.TryParse(Infomation.MediaLocation); });

      if (id.HasValue)
        return new GenericResult<string>(true, id.Value.ToString());
      else
        return new GenericResult<string>(false);
    }

    public async Task<MediaInfomation> GetInfomationAsync(bool fullLoad)
    {
      if (Online)
      {
        var result = await GetYouTubeMediaAsync(true);
        if (result)
          Infomation.MediaStreamPath = result.Result;
      }

      var resultInfo = await LocalMediaLoader.TryLoadInfoAsync(Infomation, fullLoad, Log);
      if (resultInfo)
        Infomation = resultInfo.Result;
      else
        LoadFailProcess();

      return Infomation;
    }

    public async Task<GenericResult<string>> GetStreamPathAsync(bool useCache)
    {
      if (Online)
      {
        var result = await GetYouTubeMediaAsync(useCache);
        if (result)
          Infomation.MediaStreamPath = result.Result;
        return result;
      }
      else
      {
        if (File.Exists(Infomation.MediaStreamPath))
          return new GenericResult<string>(true, Infomation.MediaStreamPath);
        else
          return new GenericResult<string>(false);
      }
    }

    #region Stream
    /// <summary>
    /// YouTube 미디어 스트림 & 정보 다운로드시도
    /// </summary>
    /// <returns>스트림 캐쉬 저장 경로</returns>
    private async Task<GenericResult<string>> GetYouTubeMediaAsync(bool useCache)
    {
      if (!Online)
      {
        Log.Error(PredefineMessage.IsNotOnlineMedia);
        return new GenericResult<string>(false);
      }

      // 임시저장된 정보를 로드
      if (useCache)
      {
        var streamCachePathResult = LocalMediaLoader.TryGetOnlineMediaCacheAsync((await GetID()).Result);
        if (streamCachePathResult)
        {
          Log.Debug("캐쉬에서 미디어가 확인됨");
          return new GenericResult<string>(true, streamCachePathResult.Result);
        }
        else
          Log.Warn("캐쉬된 미디어 로드 실패. 온라인에서 다운로드를 시도합니다");
      }

      if (Checker.CheckForInternetConnection())
      {
        // 캐쉬폴더가 존재하지 않을시 생성
        Checker.DirectoryCheck(GlobalProperty.OnlineMediaCachePath);

        string mp3FilePath = Path.Combine(GlobalProperty.OnlineMediaCachePath, $"{(await GetID()).Result}.mp3");
        try
        {
          // 유튜브 스트림 다운로드
          string streampath;
          var streamPathResult = await TryDownloadYouTubeStreamAsync(GlobalProperty.OnlineMediaCachePath);
          if (streamPathResult)
            streampath = streamPathResult.Result;
          else
            throw new Exception("Failed to download YouTube stream");

          // Mp3로 변환
          if (!await Converter.ConvertToMP3Async(streampath, mp3FilePath))
            throw new FileNotFoundException($"File is Null\nSourceFile : [{streampath}]\nTargetFile : [{mp3FilePath}]");
          File.Delete(streampath);
          if (File.Exists(mp3FilePath))
            Infomation.MediaStreamPath = mp3FilePath;
          else
          {
            Log.Error("미디어 스트림 Mp3 변환 오류",new FileNotFoundException("변환을 완료 했지만, 파일을 찾을 수 없습니다."), $"Mp3Path : [{mp3FilePath}]\nStreamPath : [{streampath}]\nMediaLocation : [{Infomation.MediaLocation}]");
            return new GenericResult<string>(false);
          }
          Log.Info("미디어 스트림 Mp3 변환 완료");

          // 메타 데이터 저장
          if (await TryYouTubeMetaDataSave())
            Log.Info("미디어 메타데이터 다운로드 & 병합 완료");
          else
            Log.Error("미디어 메타데이터 병합 중 오류 발생");
        }
        catch (Exception e)
        {
          Log.Error("미디어 스트림 다운로드 & 변환 실패", e, $"MediaLocation : [{Infomation.MediaLocation}]");
        }

        return new GenericResult<string>(true, mp3FilePath);
      }
      else
      {
        Log.Error(PredefineMessage.UnableNetwork);
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
      if (Checker.CheckForInternetConnection())
      {
        // 미디어 스트림 다운로드
        var youtube = new YoutubeClient();
        try
        {
          var streamManifest = await youtube.Videos.Streams.GetManifestAsync(Infomation.MediaLocation);
          var streamInfo = streamManifest.GetAudioOnly().WithHighestBitrate();
          if (streamInfo == null)
            return new GenericResult<string>(false);
          string savepath = Path.Combine(path, $"{(await GetID()).Result}.{streamInfo.Container}");
          // Download the stream to file
          await youtube.Videos.Streams.DownloadAsync(streamInfo, savepath);
          Log.Info("온라인에서 미디어 스트림 로드 완료");
          return new GenericResult<string>(true, savepath);
        }
        catch (Exception e)
        {
          Log.Error("미디어 스트림 다운로드 실패", e);
          return new GenericResult<string>(false);
        }
      }
      else
      {
        Log.Error(PredefineMessage.UnableNetwork);
        return new GenericResult<string>(false);
      }
    }
    #endregion

    #region Info
    /// <summary>
    /// 유튜브에서 정보를 다운로드하여 다운로드된 스트림에 저장합니다
    /// (Mp3로 변환후 저장 권장)
    /// </summary>
    private async Task<bool> TryYouTubeMetaDataSave()
    {
      if (!Online)
      {
        Log.Error(PredefineMessage.IsNotOnlineMedia);
        return false;
      }

      if (!File.Exists(Infomation.MediaStreamPath))
      {
        Log.Error("파일이 없습니다", new FileNotFoundException("File Not Found"), $"Path : [{Infomation.MediaStreamPath}]");
        return false;
      }

      if (Checker.CheckForInternetConnection())
      {
        await Task.Run(async () =>
        {
          var youtube = new YoutubeClient();
          Video videoinfo;
          try
          {
            videoinfo = await youtube.Videos.GetAsync(Infomation.MediaLocation);
          }
          catch (Exception e)
          {
            Log.Error("온라인 정보 로드 실패", e);
            return false;
          }
          TagLib.File Fileinfo = null;
          try
          {
            Fileinfo = TagLib.File.Create(Infomation.MediaStreamPath);
          }
          catch (Exception e)
          {
            Log.Error("Mp3 파일 열기 또는 메타정보 로드 실패", e);
            Fileinfo?.Dispose();
            return false;
          }

          // 기본정보 처리
          Fileinfo.Tag.Title = videoinfo.Title;
          Fileinfo.Tag.Album = $"\"{videoinfo.Url}\" form Online";
          Fileinfo.Tag.AlbumArtists = new string[] { videoinfo.Author };
          Fileinfo.Tag.Description = $"YouTubeID : \"{videoinfo.Id}\"";

          // 썸네일 처리
          byte[] imagedata = null;
          using (WebClient webClient = new WebClient())
          {
            try
            {
              imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.MaxResUrl);
            }
            catch (Exception e)
            {
              Log.Error("썸네일 추출 중 오류가 발생했습니다. 일반 화질로 다시시도 합니다", e);
              try
              {
                imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.StandardResUrl);
              }
              catch (Exception ex)
              {
                Log.Error("일반 화질 썸네일 추출 중 오류가 발생했습니다", ex);
              }
            }
          }

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
              Log.Error("메타데이터에 썸네일 정보등록을 실패했습니다", e);
            }
          }
          else
            Log.Error("썸네일 정보가 Null 입니다", new NullReferenceException("Image data is Null"));

          try
          {
            Fileinfo.Save();
          }
          catch (Exception e)
          {
            Log.Error("메타데이터에 작성 & 저장에 실패했습니다", e);
            return false;
          }
          finally
          {
            Fileinfo.Dispose();
          }
          Log.Info("YouTube에서 Mp3 메타 데이터 저장 완료");
          return true;
        });
        Log.Error("YouTube에서 Mp3 메타 데이터 저장 중 알 수 없는 오류 발생", $"StreamPath : [{Path.GetFullPath(Infomation.MediaStreamPath)}]\nPath : [{Infomation.MediaLocation}]");
        return false;
      }
      else
      {
        Log.Error("네트워크를 사용할 수 없습니다");
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
      if (!Infomation.Title.ToLower().StartsWith(GlobalProperty.MEDIA_INFO_NULL.ToLower()))
      {
        if (string.IsNullOrWhiteSpace(Infomation.Title))
          Infomation.Title = $"{GlobalProperty.MEDIA_INFO_NULL} {Path.GetFileNameWithoutExtension(Infomation.MediaLocation)}";
        else
          Infomation.Title = $"{GlobalProperty.MEDIA_INFO_NULL} {Infomation.Title}";
      }
      Infomation.MediaStreamPath = string.Empty;
    }
    #endregion
  }
}
