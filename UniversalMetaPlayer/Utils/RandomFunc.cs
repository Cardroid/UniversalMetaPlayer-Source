using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.Utils
{
  public class RandomFunc
  {
    /// <summary>
    /// 랜덤 문자열을 생성합니다.
    /// </summary>
    /// <param name="_nLength">생성할 문자열 길이</param>
    /// <returns>랜덤 문자열</returns>
    public string RandomString(int _nLength = 12)
    {
      const string strPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";  //문자 생성 풀
      char[] chRandom = new char[_nLength];

      for (int i = 0; i < _nLength; i++)
      {
        Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF); //랜덤 시드값
        chRandom[i] = strPool[random.Next(strPool.Length)];
      }
      string strRet = new string(chRandom);   // char to string
      return strRet;
    }

    /// <summary>
    /// 중복 없는 랜덤 값
    /// </summary>
    /// <param name="seed">시드</param>
    /// <param name="originalValue">원본 값</param>
    /// <param name="minValue">최소 값</param>
    /// <param name="maxValue">최대 값</param>
    /// <returns>범위내 랜덤 값</returns>
    public int RandomInt(int seed, int originalValue, int minValue, int maxValue)
    {
      int beforeindex = originalValue;
      do
      {
        originalValue = new Random(++seed + (int)DateTime.Now.Ticks & 0x0000FFFF).Next(minValue, maxValue);
      } while (originalValue == beforeindex);
      return originalValue;
    }
  }
}
