using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.Core.Model
{
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
