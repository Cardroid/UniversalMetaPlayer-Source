using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;

using UMP.Core.Model;

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

    /// <summary>
    /// SI 단위 접두사 파싱
    /// </summary>
    /// <param name="d">값</param>
    /// <param name="format">문자열 포멧</param>
    /// <returns>단위 접두사가 붙은 문자열 값</returns>
    public static string ToSI(double d, string format = null, bool wordSpacing = true)
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
