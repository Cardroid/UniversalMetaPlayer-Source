using System;
using System.Collections.Generic;
using System.Text;

namespace CMP2.Utility
{
  public static class RandomFunc
  {
    private static readonly Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF); //랜덤 시드값

    public static string RandomString(int _nLength = 12)
    {
      const string strPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";  //문자 생성 풀
      char[] chRandom = new char[_nLength];

      for (int i = 0; i < _nLength; i++)
      {
        chRandom[i] = strPool[random.Next(strPool.Length)];
      }
      string strRet = new string(chRandom);   // char to string
      return strRet;
    }
  }
}
