using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace UMP.Core.Model.Media
{
  public delegate void UMP_ProgressChangedEventHandler(object o, UMP_ProgressChangedEventArgs e);

  public interface IMediaLoader
  {
    /// <summary>
    /// 미디어 정보를 가져옵니다.
    /// </summary>
    /// <param name="fullLoad">모든 정보 로드 여부</param>
    /// <returns>로드된 <see cref="MediaInformation"/></returns>
    public Task<MediaInformation> GetInformationAsync(bool fullLoad);
    /// <summary>
    /// 미디어 스트림 경로를 가져옵니다.
    /// </summary>
    /// <returns>성공시 true, 미디어 스트림 경로</returns>
    public Task<GenericResult<string>> GetStreamPathAsync(bool useCache);
    /// <summary>
    /// 온라인 미디어의 ID 가져오기
    /// </summary>
    /// <returns><see cref="GenericResult&lt;T&gt;"/>성공시 true, ID</returns>
    public Task<GenericResult<string>> GetID();
    /// <summary>
    /// true = 온라인 미디어, false = 로컬 미디어
    /// </summary>
    public bool Online { get; }
    /// <summary>
    /// 캐쉬를 저장하는 디렉터리 경로
    /// </summary>
    public string CachePath { get; set; }

    public event UMP_ProgressChangedEventHandler ProgressChanged;
  }

  public class UMP_ProgressChangedEventArgs
  {
    public UMP_ProgressChangedEventArgs(ProgressKind progressKind, int percentage, string userMessage = "")
    {
      this.ProgressKind = progressKind;
      this.Percentage = percentage;
      this.UserMessage = userMessage;
    }
    public ProgressKind ProgressKind { get; }
    public int Percentage { get; }
    public string UserMessage { get; }
  }

  public class MediaLoaderProgress : IUMP_Progress<double>
  {
    public event ProgressChangedEventHandler ProgressChanged;
    public void Report(double percentage, string userMessage = "") => ProgressChanged?.Invoke(percentage, userMessage);
    public void Report(double percentage) => ProgressChanged?.Invoke(percentage, "");
  }

  [Flags]
  public enum ProgressKind
  {
    Info,
    InfoDownload,
    InfoLoad,
    InfoSave,
    Stream,
    StreamDownload,
    StreamConvert,
    MetaDataSave,
  }
}
