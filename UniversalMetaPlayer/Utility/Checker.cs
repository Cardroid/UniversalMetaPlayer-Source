using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using UMP.Core.Model;
using YoutubeExplode.Videos;

namespace UMP.Utility
{
  public static class Checker
  {
    /// <summary>
    /// 인터넷에 연결되었는지 체크합니다
    /// </summary>
    /// <returns>true = 인터넷 사용가능, false = 인터넷 사용불가능</returns>
    public static bool CheckForInternetConnection()
    {
      try
      {
        using (var client = new WebClient())
        using (client.OpenRead("http://clients3.google.com/generate_204"))
        { return true; }
      }
      catch { return false; }
    }

    /// <summary>
    /// 디랙터리를 채크합니다. 존재하지 않으면 생성합니다.
    /// </summary>
    /// <param name="path"></param>
    public static void DirectoryCheck(string path)
    {
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
        App.MainLog.Info("폴더가 생성되었습니다.", $"Path : [{path}]");
      }
    }

    /// <summary>
    /// 미디어 타입이 판별기
    /// </summary>
    /// <param name="path">체크할 미디어의 위치</param>
    /// <returns>없을 시 Null</returns>
    public static MediaType MediaTypeChecker(string path)
    {
      if (File.Exists(path))
      {
        string ext = Path.GetExtension(path).ToLower();
        string[] extArray = { ".mp3", ".flac" };

        for(int i = 0; i < extArray.Length; i++)
        {
          if (extArray[i].Equals(ext))
            break;
          if (i == extArray.Length - 1)
            return MediaType.NotSupport;
        }

        return MediaType.Local;
      }
      else if (VideoId.TryParse(path).HasValue)
        return MediaType.Youtube;
      else
        return MediaType.NotSupport;
    }
  }
}
