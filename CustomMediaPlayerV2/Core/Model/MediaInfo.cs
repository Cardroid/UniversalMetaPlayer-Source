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
    private Log Log = new Log(typeof(MediaInfo));
    public MediaInfo(MediaType mediaType, string medialocation)
    {
      if (string.IsNullOrWhiteSpace(medialocation))
        return;
      MediaLocation = medialocation;
      MediaType = mediaType;
      if (MediaType == MediaType.Local)
      {
        Title = Path.GetFileNameWithoutExtension(MediaLocation);
      }
      LoadedCheck = LoadState.NotTryed;
    }

    #region YouTube
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
      if (!Directory.Exists("Cache"))
        Directory.CreateDirectory("Cache");

      // 유튜브 Url을 Video_ID_Info.json 으로 파싱
      string videoinfopath = MediaLocation.Replace("https://www.youtube.com/watch?v=", "");
      if (videoinfopath.IndexOf('&') != -1)
        videoinfopath = videoinfopath.Substring(0, videoinfopath.IndexOf('&') - 1);
      videoinfopath = $"Cache\\{videoinfopath}_Info.json";

      // 임시저장된 정보가 있는지 체크
      if (File.Exists(videoinfopath))
      {
        // 임시저장된 정보를 로드
        try
        {
          string Jsonstring = File.ReadAllText(videoinfopath);
          JObject jObject = JObject.Parse(Jsonstring);

          Title = jObject.Value<string>("Title");
          ArtistName = jObject.Value<string>("ArtistName");
          Duration = TimeSpan.FromMilliseconds(jObject.Value<double>("Duration"));

          LoadedCheck = LoadState.PartialLoaded;
        }
        catch (Exception e)
        {
          Log.Error("임시저장된 미디어 정보 로드 실패.\n자동으로 온라인 다운로드를 시도합니다.", e);
          LoadedCheck = await TryInfoOnlineDownloadAsync(videoinfopath);
        }
      }
      else
      {
        LoadedCheck = await TryInfoOnlineDownloadAsync(videoinfopath);
      }
      if(LoadedCheck != LoadState.Fail || LoadedCheck != LoadState.NotTryed)
        Log.Info("미디어 정보 로드 성공.");
    }

    /// <summary>
    /// YouTube 미디어 정보 온라인 다운로드시도
    /// </summary>
    /// <param name="videoinfopath">정보 캐쉬저장 경로</param>
    /// <returns>다운로드 결과</returns>
    private async Task<LoadState> TryInfoOnlineDownloadAsync(string videoinfopath)
    {
      Log.Debug("온라인에서 미디어 정보 다운로드 시도.");
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
                new JProperty("Duration", Duration.TotalMilliseconds.ToString())
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
      if (!Directory.Exists("Cache"))
        Directory.CreateDirectory("Cache");

      // 유튜브 Url을 Video_ID로 파싱
      string videoid = MediaLocation.Replace("https://www.youtube.com/watch?v=", "");
      if (videoid.IndexOf('&') != -1)
        videoid = videoid.Substring(0, videoid.IndexOf('&') - 1);

      // 미디어 스트림 캐쉬가 있을 경우 경로를 return
      if (File.Exists($"Cache\\{videoid}.{Container.WebM.Name}"))
      {
        Log.Info("캐쉬에서 미디어 스트림 로드 성공.");
        return $"Cache\\{videoid}.{Container.WebM.Name}";
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

          string savepath = $"Cache\\{videoid}.{streamInfo.Container}";

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
    #endregion

    #region Local
    /// <summary>
    /// 로컬 미디어 정보 로드
    /// </summary>
    public void TryLocalInfomationLoad(bool fullload = false)
    {
      if (File.Exists(MediaLocation))
      {
        using (var Fileinfo = TagLib.File.Create(MediaLocation))
        {
          // 미디어 정보를 정보 클래스에 저장
          Title = Fileinfo.Tag.Title ?? Path.GetFileNameWithoutExtension(MediaLocation);
          Duration = Fileinfo.Properties.Duration;
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
          }
        }
        if (fullload)
          LoadedCheck = LoadState.AllLoaded;
        else
          LoadedCheck = LoadState.PartialLoaded;
      }
      else
        LoadedCheck = LoadState.Fail;
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