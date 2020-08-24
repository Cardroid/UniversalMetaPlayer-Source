using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace UMP.Utility
{
  public static class Parser
  {
    public static UrlInfo GetUrlInfo(string url, RegexOptions options = RegexOptions.None, int matchTimeOutMillisecond = 60000)
    {
      var r = new Regex(@"^((?<proto>\w+)://)?([wW]+\.)?(?<domain>\w+)(\.\w+)(:(?<port>\d+))?", options, TimeSpan.FromMilliseconds(matchTimeOutMillisecond));
      var m = r.Match(url);

      string protocol = string.Empty;
      string domain = string.Empty;
      int? port = null;

      if (m.Success)
      {
        domain = m.Result("${domain}");
        domain = $"{domain.First().ToString().ToUpper()}{domain.Substring(1)}";

        if (int.TryParse(m.Result("${port}"), out int result))
          port = result;
        return new UrlInfo(url, m.Success, m.Result("${proto}"), domain, port);
      }

      return new UrlInfo(url, false);
    }

    /// <summary>
    /// 타입변환기
    /// </summary>
    /// <typeparam name="BeforeT">변환 전 타입</typeparam>
    /// <typeparam name="AfterT">변환 후 타입</typeparam>
    /// <param name="value">변환할 값</param>
    /// <returns>변환된 값</returns>
    public static AfterT ChangeType<AfterT, BeforeT>(BeforeT value)
    {
      if (typeof(AfterT) == typeof(Color))
        return (AfterT)ColorConverter.ConvertFromString(value.ToString());
      else if (typeof(AfterT).IsEnum)
        return (AfterT)Enum.Parse(typeof(AfterT), value.ToString());
      return (AfterT)Convert.ChangeType(value, typeof(AfterT));
    }
  }

  /// <summary>
  /// Url의 정보 구조체
  /// </summary>
  public struct UrlInfo
  {
    public UrlInfo(string url, bool success, string protocol, string domain, int? port)
    {
      if (string.IsNullOrWhiteSpace(url))
        this.Success = false;
      else
        this.Success = success;

      this.Url = url;
      this.Protocol = protocol;
      this.Domain = domain;
      this.Port = port;
    }

    public UrlInfo(string url, bool success = false)
    {
      if (string.IsNullOrWhiteSpace(url))
        this.Success = false;
      else
        this.Success = success;
      this.Url = url;
      this.Protocol = string.Empty;
      this.Domain = string.Empty;
      this.Port = null;
    }

    /// <summary>
    /// 성공여부 (Url 형식이 아니면 false)
    /// </summary>
    public bool Success { get; private set; }
    /// <summary>
    /// 검사한 Url
    /// </summary>
    public string Url { get; private set; }
    /// <summary>
    /// Url의 프로토콜
    /// </summary>
    public string Protocol { get; private set; }
    /// <summary>
    /// Url의 도메인
    /// </summary>
    public string Domain { get; private set; }
    /// <summary>
    /// Url의 포트번호
    /// </summary>
    public int? Port { get; private set; }
  }
}
