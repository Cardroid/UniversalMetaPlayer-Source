using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UMP.Lib.Utility
{
  public static class CacheManager
  {
    /// <summary>
    /// 이전 버전의 호환성을 유지하기 위한 메서드 입니다.<br/>
    /// Core 폴더 내부의 캐시폴더를 밖으로 이동시킵니다.
    /// </summary>
    public static void MoveCacheDirectory()
    {
      string oldCacheLocation = @"Core\Cache";

      if (Directory.Exists(oldCacheLocation))
        Directory.Move(oldCacheLocation, @"Cache");
    }
  }
}
