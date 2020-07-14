using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using YoutubeExplode;

using CMP2.Utility;
using YoutubeExplode.Videos.Streams;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CMP2.Core.Model
{
  #region 미디어 정보 인터페이스

  /// <summary>
  /// 미디어 정보 인터페이스
  /// </summary>
  public interface IMediaInfo
  {
    /// <summary>
    /// 플래이리스트에서의 고유숫자
    /// </summary>
    public int ID { get; set; }
    /// <summary>
    /// 미디어 타입
    /// </summary>
    public MediaType MediaType { get; set; }
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
  public class MediaInfo : IMediaInfo
  {
    private Log Log { get; }
    public MediaInfo(MediaType mediaType, string medialocation)
    {
      if (string.IsNullOrWhiteSpace(medialocation))
        return;
      MediaLocation = medialocation;
      MediaType = mediaType;
      if (MediaType == MediaType.Local)
        Title = Path.GetFileNameWithoutExtension(MediaLocation);
      else if (MediaType == MediaType.Youtube)
      {
        Title = $"\"{GetYouTubeID()}\" Form YouTube";
        AlbumTitle = $"{MediaLocation} Form YouTube";
      }
      Log = new Log($"MediaInfo - [{Title}]");
      LoadedCheck = LoadState.NotTryed;
    }

    #region YouTube
    public static string CacheDirectoryPath = Path.Combine("Cache","Youtube");

    /// <summary>
    /// YouTube 미디어 정보 로드시도
    /// </summary>
    public async Task TryYouTubeInfomationLoadAsync()
    {
      Log.Debug("미디어 정보 로드 시도.");
      // 타입 검사
      if (MediaType != MediaType.Youtube)
      {
        Log.Error("미디어 타입이 YouTube가 아닙니다.");
        return;
      }

      // 캐쉬폴더가 존재하지 않을시 생성
      DirectoryCheck(CacheDirectoryPath);

      string videoInfoPath = Path.Combine(CacheDirectoryPath, $"{GetYouTubeID()}_Info.json");
      
      // 임시저장된 정보가 있는지 체크
      if (File.Exists(videoInfoPath))
      {
        // 임시저장된 정보를 로드
        try
        {
          string Jsonstring = File.ReadAllText(videoInfoPath);
          JObject jObject = JObject.Parse(Jsonstring);

          Title = jObject.Value<string>("Title");
          ArtistName = jObject.Value<string>("ArtistName");
          Duration = TimeSpan.FromMilliseconds(jObject.Value<double>("Duration"));

          LoadedCheck = LoadState.PartialLoaded;
          Log.Info("캐쉬에서 미디어 정보 로드 성공.");
          return;
        }
        catch (Exception e)
        {
          Log.Error("임시저장된 미디어 정보 로드 실패.\n온라인에서 정보로드를 시도합니다.", e);
        }
      }
      LoadedCheck = await TryInfoOnlineDownloadAsync(videoInfoPath);
      if (LoadedCheck == LoadState.AllLoaded || LoadedCheck == LoadState.PartialLoaded)
        Log.Info("온라인에서 미디어 정보 로드 성공.");
    }

    /// <summary>
    /// YouTube 미디어 정보 온라인 다운로드시도
    /// </summary>
    /// <param name="videoinfopath">정보 캐쉬저장 경로</param>
    /// <returns>다운로드 결과</returns>
    private async Task<LoadState> TryInfoOnlineDownloadAsync(string videoinfopath)
    {
      if (Checker.CheckForInternetConnection())
      {
        // 정보 다운로드 시작.
        var youtube = new YoutubeClient();
        try
        {
          var video = await youtube.Videos.GetAsync(MediaLocation);

          Title = video.Title;
          ArtistName = video.Author;
          Duration = video.Duration;
        }
        catch (Exception e)
        {
          Log.Error("미디어 정보 다운로드 실패.", e);
          return LoadState.Fail;
        }

        // 다운로드된 정보를 임시저장합니다.
        try
        {
          JObject Jobj = new JObject
            {
                new JProperty("Title", Title),
                new JProperty("ArtistName", ArtistName),
                new JProperty("Duration", Duration.TotalMilliseconds.ToString()),
                new JProperty("MediaLocation", MediaLocation)
            };
          File.WriteAllText(videoinfopath, Jobj.ToString());
        }
        catch (Exception e)
        {
          Log.Error("미디어 정보 저장 실패.", e);
          return LoadState.Fail;
        }
      }
      else
      {
        Log.Error("네트워크를 사용할 수 없습니다.");
        return LoadState.Fail;
      }
      return LoadState.PartialLoaded;
    }

    /// <summary>
    /// YouTube 미디어 스트림 다운로드시도
    /// </summary>
    /// <returns>스트림 캐쉬 저장 경로</returns>
    public async Task<string> TryYouTubeStreamDownloadAsync()
    {
      Log.Debug("미디어 스트림 로드 시도.");
      // 타입 검사
      if (MediaType != MediaType.Youtube)
      {
        Log.Error("미디어 타입이 YouTube가 아닙니다.");
        return string.Empty;
      }

      // 캐쉬폴더가 존재하지 않을시 생성
      DirectoryCheck(CacheDirectoryPath);

      // 미디어 스트림 캐쉬가 있을 경우 경로를 return
      string[] files = Directory.GetFiles(CacheDirectoryPath, $"{GetYouTubeID()}.*", SearchOption.AllDirectories);
      if (files.Length > 0)
      {
        // 검색 결과에 여러 파일들이 나올 경우
        //string cachedVideoPath = string.Empty;
        //for (int i = 0; i < files.Length; i++)
        //{
        //  if (files[i].ToLower().EndsWith(".json"))
        //    continue;
        //  cachedVideoPath = files[i];
        //}
        //Log.Info("캐쉬에서 미디어 스트림 로드 성공.");
        //return cachedVideoPath;

        // 단일 파일 처리
        Log.Info("캐쉬에서 미디어 스트림 로드 성공.");
        return files[0];
      }

      if (Checker.CheckForInternetConnection())
      {
        // 미디어 스트림 다운로드
        var youtube = new YoutubeClient();
        try
        {
          var streamManifest = await youtube.Videos.Streams.GetManifestAsync(MediaLocation);

          var streamInfo = streamManifest.GetAudioOnly().WithHighestBitrate();

          if (streamInfo == null)
            return string.Empty;

          string savepath = Path.Combine(CacheDirectoryPath, $"{GetYouTubeID()}.{streamInfo.Container}");

          // Download the stream to file
          await youtube.Videos.Streams.DownloadAsync(streamInfo, savepath);

          Log.Info("온라인에서 미디어 스트림 로드 성공.");
          return savepath;
        }
        catch (Exception e)
        {
          Log.Error("미디어 스트림 다운로드 or 저장 실패", e);
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
    /// YouTube Video ID를 파싱합니다
    /// </summary>
    /// <returns>YouTube Video ID</returns>
    public string GetYouTubeID()
    {
      if (MediaType != MediaType.Youtube)
        return string.Empty;

      if (!string.IsNullOrWhiteSpace(_VideoIDCache))
        return _VideoIDCache;

      string videoinfopath = MediaLocation;
      int index;

      string[] minusurls = { "https://", "http://", "www.", "youtu.be/", "youtube.com/watch?v=" };

      for(int i = 0; i < minusurls.Length; i++)
        videoinfopath = videoinfopath.Replace(minusurls[i], "");

      index = videoinfopath.IndexOf('?');
      if (index >= 0)
        videoinfopath = videoinfopath.Substring(0, index);
      
      index = videoinfopath.IndexOf('/');
      if (index >= 0)
        videoinfopath = videoinfopath.Substring(0, index);
      
      index = videoinfopath.IndexOf('=');
      if (index >= 0)
        videoinfopath = videoinfopath.Substring(0, index);

      index = videoinfopath.IndexOf('&');
      if (index >= 0)
        videoinfopath = videoinfopath.Substring(0, index);

      _VideoIDCache = videoinfopath;
      return videoinfopath;
    }

    private string _VideoIDCache = string.Empty;
    #endregion

    #region Local
    /// <summary>
    /// 로컬 미디어 정보 로드
    /// </summary>
    public void TryLocalInfomationLoad(bool fullload = false)
    {
      Log.Debug("미디어 정보 로드 시도.");
      if (File.Exists(MediaLocation))
      {
        using (var Fileinfo = TagLib.File.Create(MediaLocation))
        {
          // 미디어 정보를 정보 클래스에 저장
          Title = Fileinfo.Tag.Title ?? Path.GetFileNameWithoutExtension(MediaLocation);
          Duration = Fileinfo.Properties.Duration;
          LoadedCheck = LoadState.PartialLoaded;

          // 모든 정보 로드
          if (fullload)
          {
            try
            {
              TagLib.IPicture pic = Fileinfo.Tag.Pictures[0];  //pic contains data for image.
              MemoryStream stream = new MemoryStream(pic.Data.Data);  // create an in memory stream
              AlbumImage = BitmapFrame.Create(stream);
            }
            catch { AlbumImage = null; }
            AlbumTitle = !string.IsNullOrWhiteSpace(Fileinfo.Tag.Album) ? Fileinfo.Tag.Album : string.Empty;
            ArtistName = !string.IsNullOrWhiteSpace(Fileinfo.Tag.FirstAlbumArtist) ? Fileinfo.Tag.FirstAlbumArtist : string.Empty;
            Lyrics = !string.IsNullOrWhiteSpace(Fileinfo.Tag.Lyrics) ? Fileinfo.Tag.Lyrics : string.Empty;
            LoadedCheck = LoadState.AllLoaded;
          }
          Log.Info("미디어 정보 로드 성공.");
        }
      }
      else
      {
        Log.Error("미디어 파일이 없습니다.");
        LoadedCheck = LoadState.Fail;
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
      if (!Title.ToLower().StartsWith(MEDIA_INFO_NULL))
      {
        Title = $"{MEDIA_INFO_NULL} {Title}";
      }
    }

    /// <summary>
    /// 디랙터리를 채크합니다. 존재하지 않으면 생성합니다.
    /// </summary>
    /// <param name="path"></param>
    private void DirectoryCheck(string path)
    {
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
    }
    #endregion

    /// <summary>
    /// 정보의 로드가 완료되었는지 여부
    /// </summary>
    public LoadState LoadedCheck { get; private set; }

    #region 프로퍼티 정의 (인터페이스 상속)
    public int ID { get; set; } = -1;
    public MediaType MediaType { get; set; }
    public string MediaLocation { get; set; }
    public string Title { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public ImageSource AlbumImage { get; set; } = null;
    public string AlbumTitle { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;
    public string Lyrics { get; set; } = string.Empty;
    #endregion
  }
  public enum LoadState
  {
    NotTryed,
    Fail,
    PartialLoaded,
    AllLoaded
  }
  public enum MediaType
  {
    Local,
    Youtube
  }
  #endregion
}