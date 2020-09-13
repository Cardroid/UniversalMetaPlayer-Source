using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace UMP.Lib.Utility
{
  public class Checker
  {
    private Checker()
    {
    }

    private static Checker Instence = null;

    public static Checker GetChecker()
    {
      if (Instence == null)
        Instence = new Checker();

      return Instence;
    }

    /// <summary>
    /// 인터넷에 연결되었는지 체크합니다
    /// </summary>
    /// <returns>true = 인터넷 사용가능, false = 인터넷 사용불가능</returns>
    public bool CheckForInternetConnection()
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
    /// <param name="path">검사할 경로</param>
    public void DirectoryCheck(string path)
    {
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
    }

    /// <summary>
    /// 로컬 경로인지 체크합니다.
    /// </summary>
    /// <param name="path">체크할 경로</param>
    /// <returns>로컬 경로일 경우 true</returns>
    public bool IsLocalPath(string path)
    {
      try
      {
        return new Uri(path).IsFile;
      }
      catch
      {
        return false;
      }
    }
  }
}
