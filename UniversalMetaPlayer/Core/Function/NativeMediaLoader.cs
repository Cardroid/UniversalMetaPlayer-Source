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

    public NativeMediaLoader(string mediaLocation)
    {
      Log = new Log($"{typeof(NativeMediaLoader)}");

      Information = new MediaInformation()
      {
        Title = string.Empty,
        Duration = TimeSpan.Zero,
        AlbumImage = null,
        Tags = new MediaInfoDictionary()
      };

      if (!string.IsNullOrWhiteSpace(mediaLocation))
      {
        Information.MediaLocation = mediaLocation;
        if (Checker.IsLocalPath(mediaLocation) && File.Exists(mediaLocation))
        {
          Online = false;
          Information.MediaStreamPath = Information.MediaLocation;
        }
        else
          Online = true;
      }
    }

    public NativeMediaLoader(MediaInformation mediainfo)
    {
      Log = new Log($"{typeof(NativeMediaLoader)}");

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

      // 임시저장된 정보를 로드
      if (useCache)
      {
        var streamCachePathResult = LocalMediaLoader.TryGetOnlineMediaCacheAsync((await GetID()).Result);
        if (streamCachePathResult)
        {
          Log.Debug("캐시에서 미디어가 확인됨");
          return new GenericResult<string>(true, streamCachePathResult.Result);
        }
        else
          Log.Warn("캐시된 미디어 로드 실패. 온라인에서 다운로드를 시도합니다");
      }

      if (Checker.CheckForInternetConnection())
      {
        // 캐시폴더가 존재하지 않을시 생성
        Checker.DirectoryCheck(GlobalProperty.Predefine.OnlineMediaCachePath);

        string mp3FilePath = Path.Combine(GlobalProperty.Predefine.OnlineMediaCachePath, $"{(await GetID()).Result}.mp3");
        try
        {
          // 유튜브 스트림 다운로드
          string streampath;
          var streamPathResult = await TryDownloadYouTubeStreamAsync(GlobalProperty.Predefine.OnlineMediaCachePath);
          if (streamPathResult)
            streampath = streamPathResult.Result;
          else
            throw new Exception("Failed to download YouTube stream");

          // Mp3로 변환
          if (!await Converter.ConvertToMP3Async(streampath, mp3FilePath))
            throw new FileNotFoundException($"File is Null\nSourceFile : [{streampath}]\nTargetFile : [{mp3FilePath}]");
          File.Delete(streampath);
          if (File.Exists(mp3FilePath))
            Information.MediaStreamPath = mp3FilePath;
          else
          {
            Log.Fatal("미디어 스트림 Mp3 변환 오류",new FileNotFoundException("변환을 완료 했지만, 파일을 찾을 수 없습니다."), $"Mp3Path : [{mp3FilePath}]\nStreamPath : [{streampath}]\nMediaLocation : [{Information.MediaLocation}]");
            return new GenericResult<string>(false);
          }
          Log.Info("미디어 스트림 Mp3 변환 완료");

          // 메타 데이터 저장
          if (await TryYouTubeMetaDataSave())
            Log.Info("미디어 메타데이터 다운로드 & 병합 완료");
          else
            Log.Fatal("미디어 메타데이터 병합 중 오류 발생");
        }
        catch (Exception e)
        {
          Log.Fatal("미디어 스트림 다운로드 & 변환 실패", e, $"MediaLocation : [{Information.MediaLocation}]");
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
          var streamManifest = await youtube.Videos.Streams.GetManifestAsync(Information.MediaLocation);
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
          Log.Fatal("미디어 스트림 다운로드 실패", e);
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
          try
          {
            videoinfo = await youtube.Videos.GetAsync(Information.MediaLocation);
          }
          catch (Exception e)
          {
            Log.Fatal("온라인 정보 로드 실패", e);
            success = false;
            return;
          }
          TagLib.File Fileinfo = null;
          try
          {
            Fileinfo = TagLib.File.Create(Information.MediaStreamPath);
          }
          catch (Exception e)
          {
            Log.Fatal("Mp3 파일 열기 또는 메타정보 로드 실패", e);
            Fileinfo?.Dispose();
            success = false;
            return;
          }

          // 기본정보 처리
          Fileinfo.Tag.Title = videoinfo.Title;
          Fileinfo.Tag.Album = $"\"{videoinfo.Url}\" form Online";
          Fileinfo.Tag.AlbumArtists = new string[] { videoinfo.Author };
          Fileinfo.Tag.Description = $"YouTubeID : \"{videoinfo.Id}\"";
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
            Log.Warn("자막 저장 오류", e);
          }

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
                Log.Fatal("일반 화질 썸네일 추출 중 오류가 발생했습니다", ex);
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
            Log.Fatal("메타데이터에 작성 & 저장에 실패했습니다", e);
            success = false;
            return;
          }
          finally
          {
            Fileinfo.Dispose();
          }

          Log.Info("YouTube에서 Mp3 메타 데이터 저장 완료");
          success = true;
          return;
        });
        return success;
      }
      else
      {
        Log.Error(PredefineMessage.UnableNetwork);
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
