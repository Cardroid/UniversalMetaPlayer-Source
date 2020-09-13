using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

using UMP.Lib.Model;

namespace UMP.Lib.Utility
{
  public class Parser
  {
    private Parser()
    {
    }

    private static Parser Instence = null;

    public static Parser GetParser()
    {
      if (Instence == null)
        Instence = new Parser();

      return Instence;
    }

    public UrlInfo GetUrlInfo(string url, RegexOptions options = RegexOptions.None, int matchTimeOutMillisecond = 60000)
    {
      var r = new Regex(@"^((?<protocol>\w+)://)?([wW]+\.)?(?<domain>\w+)(\.\w+)(:(?<port>\d+))?", options, TimeSpan.FromMilliseconds(matchTimeOutMillisecond));
      var m = r.Match(url);

      if (m.Success)
      {
        string protocol;
        try { protocol = m.Result("${protocol}"); }
        catch { protocol = string.Empty; }

        string domain;
        try
        {
          domain = m.Result("${domain}");
          domain = $"{domain.First().ToString().ToUpper()}{domain.Substring(1).ToLower()}";
        }
        catch { domain = string.Empty; }

        int? port = null;
        try
        {
          if (int.TryParse(m.Result("${port}"), out int result))
            port = result;
        }
        catch { port = null; }

        return new UrlInfo(url, m.Success, protocol, domain, port);
      }
      else
        return new UrlInfo(url);
    }

    /// <summary>
    /// 타입변환기
    /// </summary>
    /// <typeparam name="BeforeT">변환 전 타입</typeparam>
    /// <typeparam name="AfterT">변환 후 타입</typeparam>
    /// <param name="value">변환할 값</param>
    /// <returns>변환된 값</returns>
    public AfterT ChangeType<AfterT, BeforeT>(BeforeT value)
    {
      if (typeof(AfterT) == typeof(Color))
        return (AfterT)new ColorConverter().ConvertFromString(value.ToString());
      else if (typeof(AfterT).IsEnum)
        return (AfterT)Enum.Parse(typeof(AfterT), value.ToString());
      return (AfterT)Convert.ChangeType(value, typeof(AfterT));
    }

    /// <summary>
    /// SI 단위 접두사 파싱
    /// </summary>
    /// <param name="d">값</param>
    /// <param name="format">문자열 포멧</param>
    /// <returns>단위 접두사가 붙은 문자열 값</returns>
    public string ToSI(double d, string format = null, bool wordSpacing = true)
    {
      char[] incPrefixes = new[] { 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y' };
      char[] decPrefixes = new[] { 'm', '\u03bc', 'n', 'p', 'f', 'a', 'z', 'y' };

      int degree = (int)Math.Floor(Math.Log10(Math.Abs(d)) / 3);
      double scaled = d * Math.Pow(1000, -degree);

      char? prefix = null;
      switch (Math.Sign(degree))
      {
        case 1: prefix = incPrefixes[degree - 1]; break;
        case -1: prefix = decPrefixes[-degree - 1]; break;
      }

      return $"{scaled.ToString(format)}{(wordSpacing ? " " : "")}{prefix}";
    }
  }
}
