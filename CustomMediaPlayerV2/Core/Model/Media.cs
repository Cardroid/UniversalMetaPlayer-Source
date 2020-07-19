using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using YoutubeExplode;

using CMP2.Utility;
using YoutubeExplode.Videos.Streams;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using YoutubeExplode.Videos;

namespace CMP2.Core.Model
{
  #region 미디어 정보 구조체

  /// <summary>
  /// 미디어 정보 인터페이스
  /// </summary>
  public struct MediaInfomation
  {
    /// <summary>
    /// 미디어 타입
    /// </summary>
    public MediaType MediaType { get; set; }
    /// <summary>
    /// 정보가 로드 되었는지 여부
    /// </summary>
    public LoadState LoadedCheck { get; set; }
    /// <summary>
    /// 파일의 위치
    /// </summary>
    public string MediaLocation { get; set; }
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
    public ImageSource AlbumImage { get; set; }
    /// <summary>
    /// 엘범 타이틀
    /// </summary>
    public string AlbumTitle { get; set; }
    /// <summary>
    /// 아티스트 이름
    /// </summary>
    public string ArtistName { get; set; }
    /// <summary>
    /// 가사
    /// </summary>
    public string Lyrics { get; set; }
  }
  #endregion

  #region 미디어 정보 클레스
  /// <summary>
  /// 미디어 정보 클레스
  /// </summary>
  public class Media
  {
    private Log Log { get; }
    public Media(MediaType mediaType, string medialocation)
    {
      if (string.IsNullOrWhiteSpace(medialocation))
        throw new NullReferenceException("미디어 위치정보는 비어있을 수 없습니다.");

      Infomation = new MediaInfomation()
      {
        LoadedCheck = LoadState.NotTryed,
        Title = string.Empty,
        Duration = TimeSpan.Zero,
        AlbumImage = null,
        AlbumTitle = string.Empty,
        ArtistName = string.Empty,
        Lyrics = string.Empty
      };

      Infomation.MediaLocation = medialocation;
      Infomation.MediaType = mediaType;

      if (Infomation.MediaType == MediaType.Local)
        Infomation.Title = Path.GetFileNameWithoutExtension(Infomation.MediaLocation);
      else if (Infomation.MediaType == MediaType.Youtube)
      {
        if (!string.IsNullOrWhiteSpace(GetYouTubeVideoID()))
          Infomation.Title = $"\"{GetYouTubeVideoID()}\" Form YouTube";
        else
          Infomation.Title = $"\"{Infomation.MediaLocation}\" Form YouTube";
        Infomation.AlbumTitle = $"{Infomation.MediaLocation} Form YouTube";
      }

      Log = new Log($"{typeof(Media)} - <{Infomation.MediaType}>[{Infomation.Title}]");
      LoadedCheck = LoadState.NotTryed;
    }

    /// <summary>
    /// 일부 정보 로드 시도
    /// </summary>
    /// <returns>성공시 true 반환</returns>
    public async Task<bool> TryInfoPartialLoadAsync()
    {
      // 로컬 파일
      if (Infomation.MediaType == MediaType.Local)
        return TryFileInfomationLoad(Infomation.MediaLocation, false);
      // YouTube
      else if (Infomation.MediaType == MediaType.Youtube)
        return await TryYouTubeInfomationLoadAsync(false);
      return false;
    }

    /// <summary>
    /// 모든 정보 로드 시도
    /// </summary>
    /// <returns>성공시 true 반환</returns>
    public async Task<bool> TryInfoAllLoadAsync()
    {
      // 로컬 파일
      if (Infomation.MediaType == MediaType.Local)
        return TryFileInfomationLoad(Infomation.MediaLocation, true);
      // YouTube
      else if (Infomation.MediaType == MediaType.Youtube)
        return await TryYouTubeInfomationLoadAsync(true);
      return false;
    }

    /// <summary>
    /// 미디어 스트림의 저장 경로
    /// </summary>
    public async Task<string> GetStreamPath()
    {
      // 로컬 파일
      if (Infomation.MediaType == MediaType.Local)
        return Infomation.MediaLocation;
      // YouTube
      else if (Infomation.MediaType == MediaType.Youtube)
        return await GetYouTubeMediaAsync(true);
      return string.Empty;
    }

    #region YouTube
    public static string CacheYouTubeDirectoryPath = Path.Combine("Cache", "Youtube", "Stream");

    /// <summary>
    /// YouTube 미디어 정보 로드시도
    /// </summary>
    /// <param name="fullload">모든 정보 로드 여부</param>
    /// <returns>성공시 true를 반환</returns>
    private async Task<bool> TryYouTubeInfomationLoadAsync(bool fullload)
    {
      string streamCachePath = TrySearchCachedMedia();
      if (!string.IsNullOrWhiteSpace(streamCachePath))
      {
        if (TryFileInfomationLoad(streamCachePath, fullload))
        {
          Log.Info($"캐쉬에서 미디어 정보 로드 성공. Full : {fullload}");
          return true;
        }
        else
          Log.Warn($"캐쉬된 미디어 정보 로드 실패. 온라인에서 다운로드를 시도합니다. Full : {fullload}");
      }
      else
        Log.Warn($"캐쉬된 미디어 정보 로드 실패. 온라인에서 다운로드를 시도합니다. Full : {fullload}");

      string cachepath = await GetYouTubeMediaAsync(false);

      if (string.IsNullOrWhiteSpace(cachepath))
      {
        LoadedCheck = LoadState.Fail;
        Log.Info($"미디어 정보 로드 실패. Full : {fullload}");
        return false;
      }

      // 임시저장된 정보를 로드
      if (TryFileInfomationLoad(cachepath, fullload))
      {
        Log.Info($"캐쉬에서 미디어 정보 로드 성공. Full : {fullload}");
        LoadedCheck = LoadState.Loaded;
        return true;
      }
      else
        LoadedCheck = LoadState.Fail;

      if (LoadedCheck == LoadState.Loaded)
      {
        Log.Info($"온라인에서 미디어 정보 로드 성공. Full : {fullload}");
        return true;
      }
      else
      {
        Log.Info($"미디어 정보 로드 실패. Full : {fullload}");
        return false;
      }
    }

    /// <summary>
    /// YouTube 미디어 스트림 & 정보 다운로드시도
    /// </summary>
    /// <returns>스트림 캐쉬 저장 경로</returns>
    private async Task<string> GetYouTubeMediaAsync(bool useCache)
    {
      // 캐쉬폴더가 존재하지 않을시 생성
      Checker.DirectoryCheck(CacheYouTubeDirectoryPath);

      // 임시저장된 정보를 로드
      if (useCache)
      {
        string streamCachePath = TrySearchCachedMedia();
        if (!string.IsNullOrWhiteSpace(streamCachePath))
        {
          Log.Info("캐쉬에서 미디어 로드 성공.");
          return streamCachePath;
        }
        else
          Log.Warn("캐쉬된 미디어 로드 실패. 온라인에서 다운로드를 시도합니다.");
      }

      if (Checker.CheckForInternetConnection())
      {
        string mp3FilePath = Path.Combine(CacheYouTubeDirectoryPath, $"{GetYouTubeVideoID()}.mp3");
        try
        {
          // 유튜브 스트림 다운로드
          string streampath = await TryDownloadYouTubeStreamAsync(CacheYouTubeDirectoryPath);
          if (string.IsNullOrWhiteSpace(streampath))
            throw new Exception("Failed to download YouTube stream.");

          // Mp3로 변환
          Converter.ConvertToMP3(streampath, mp3FilePath);
          File.Delete(streampath);
          Log.Info("미디어 스트림 Mp3 변환 성공.");

          // 메타 데이터 저장
          await TryYouTubeMetaDataSave(Infomation.MediaLocation, mp3FilePath, Log);

          Log.Info("미디어 스트림 다운로드 & 변환 성공.");
        }
        catch (Exception e)
        {
          Log.Error("미디어 스트림 다운로드 & 변환 실패.", e);
        }
        return mp3FilePath;
      }
      else
      {
        Log.Error("네트워크를 사용할 수 없습니다.");
        return string.Empty;
      }
    }

    /// <summary>
    /// 온라인에서 유튜브 스트림 다운로드를 시도합니다.
    /// </summary>
    /// <param name="path">저장할 폴더 경로</param>
    /// <returns>다운로드 성공시 스트림 저장 폴더 경로</returns>
    private async Task<string> TryDownloadYouTubeStreamAsync(string path)
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
            return string.Empty;

          string savepath = Path.Combine(path, $"{GetYouTubeVideoID()}.{streamInfo.Container}");

          // Download the stream to file
          await youtube.Videos.Streams.DownloadAsync(streamInfo, savepath);

          Log.Info("온라인에서 미디어 스트림 로드 성공.");
          return savepath;
        }
        catch (Exception e)
        {
          Log.Error("미디어 스트림 다운로드 실패", e);
          return string.Empty;
        }
      }
      else
      {
        Log.Error("네트워크를 사용할 수 없습니다.");
        return string.Empty;
      }
    }

    /// <summary>
    /// 온라인에서 정보를 다운로드하여 다운로드된 스트림에 저장합니다
    /// (Mp3로 변환후 저장 권장)
    /// </summary>
    /// <param name="url">다운로드할 YouTube주소</param>
    /// <param name="mp3filepath">Mp3 파일 경로</param>
    private static async Task TryYouTubeMetaDataSave(string url, string mp3filepath, Log log)
    {
      if (Checker.CheckForInternetConnection())
      {
        var youtube = new YoutubeClient();
        try
        {
          var videoinfo = await youtube.Videos.GetAsync(url);

          using (var Fileinfo = TagLib.File.Create(mp3filepath))
          {
            Fileinfo.Tag.Title = videoinfo.Title;
            Fileinfo.Tag.AlbumArtists = new string[] { videoinfo.Author };

            using (WebClient webClient = new WebClient())
            {
              var imagedata = webClient.DownloadData(videoinfo.Thumbnails.MaxResUrl);
              Fileinfo.Tag.Pictures = new TagLib.IPicture[]
              {
                new TagLib.Picture(new TagLib.ByteVector(imagedata))
                {
                  Type = TagLib.PictureType.FrontCover,
                  Description = "Cover"
                }
              };
            }

            Fileinfo.Tag.Description = $"\"{url}\" form YouTube";

            Fileinfo.Save();
          }
          log.Info("YouTube에서 Mp3 메타 데이터 저장 성공.");
        }
        catch (Exception e)
        {
          log.Error("온라인 정보 로드 실패.", e);
        }
      }
      else
      {
        log.Error("네트워크를 사용할 수 없습니다.");
      }
    }

    /// <summary>
    /// 미디어 스트림 캐쉬가 있을 경우 경로를 return
    /// </summary>
    /// <returns>미디어 스트림 캐쉬 경로</returns>
    private string TrySearchCachedMedia()
    {
      if (Directory.Exists(CacheYouTubeDirectoryPath))
      {
        string[] files = Directory.GetFiles(CacheYouTubeDirectoryPath, $"{GetYouTubeVideoID()}.mp3", SearchOption.AllDirectories);
        if (files.Length > 0)
        {
          // 단일 파일 처리
          return files[0];
        }
      }
      return string.Empty;
    }

    /// <summary>
    /// YouTube Video ID를 파싱합니다
    /// </summary>
    /// <returns>YouTube Video ID</returns>
    public string GetYouTubeVideoID()
    {
      if (Infomation.MediaType != MediaType.Youtube)
        return string.Empty;

      var id = VideoId.TryParse(Infomation.MediaLocation);
      if (id.HasValue)
        return id.Value;
      else
        return string.Empty;
    }
    #endregion

    #region Core
    /// <summary>
    /// 파일의 미디어 정보를 가져옵니다
    /// </summary>
    /// <param name="path">파일의 위치</param>
    /// <param name="fullload">모든 정보의 로드여부</param>
    /// <returns>성공 하면 true를 반환합니다.</returns>
    private bool TryFileInfomationLoad(string path, bool fullload)
    {
      if (File.Exists(path))
      {
        using (var Fileinfo = TagLib.File.Create(path))
        {
          // 미디어 정보를 정보 클래스에 저장
          Infomation.Title = !string.IsNullOrWhiteSpace(Fileinfo.Tag.Title) ? Fileinfo.Tag.Title : Infomation.Title;
          Infomation.Duration = Fileinfo.Properties.Duration;

          // 모든 정보 로드
          if (fullload)
          {
            try { Infomation.AlbumImage = BitmapFrame.Create(new MemoryStream(Fileinfo.Tag.Pictures[0].Data.Data)); }
            catch { Infomation.AlbumImage = null; }
            Infomation.AlbumTitle = Fileinfo.Tag.Album;
            Infomation.ArtistName = Fileinfo.Tag.FirstAlbumArtist;
            Infomation.Lyrics = Fileinfo.Tag.Lyrics;
          }
          LoadedCheck = LoadState.Loaded;
          return true;
        }
      }
      else
      {
        Log.Error($"미디어 파일이 없습니다.\nMediaLocation : {path}");
        LoadedCheck = LoadState.Fail;
        return false;
      }
    }
    #endregion

    #region Other Function
    private const string MEDIA_INFO_NULL = "(null)";
    /// <summary>
    /// 미디어 로드에 실패했을 경우 호출
    /// </summary>
    public void LoadFailProcess()
    {
      if (!Infomation.Title.ToLower().StartsWith(MEDIA_INFO_NULL))
        Infomation.Title = $"{MEDIA_INFO_NULL} {Infomation.Title}";
    }
    #endregion

    /// <summary>
    /// 정보의 로드가 완료되었는지 여부
    /// </summary>
    public LoadState LoadedCheck
    {
      get => Infomation.LoadedCheck;
      private set
      {
        Infomation.LoadedCheck = value;
        if (Infomation.LoadedCheck == LoadState.Fail)
          LoadFailProcess();
      }
    }

    #region 프로퍼티 정의 (인터페이스 상속)
    /// <summary>
    /// 정보 구조체
    /// </summary>
    private MediaInfomation Infomation;
    /// <summary>
    /// 정보 구조체 Getter
    /// </summary>
    public MediaInfomation GetInfomation() => Infomation;
    #endregion
  }
  public enum LoadState
  {
    NotTryed,
    Fail,
    Loaded
  }
  public enum MediaType
  {
    Local,
    Youtube
  }
  #endregion
}