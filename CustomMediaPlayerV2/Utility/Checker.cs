using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CMP2.Utility
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
  }
}
