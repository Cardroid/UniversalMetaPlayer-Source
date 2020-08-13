using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UMP.UpdateClient
{
  public static class Utility
  {
    /// <summary>
    /// ZIP 파일 추출하기
    /// </summary>
    /// <param name="zipFilePath">ZIP 파일 경로</param>
    /// <param name="targetFilePath">백업 폴더</param>
    public static async Task<bool> ExtractZIPFileAsync(string zipFilePath, string targetFilePath)
    {
      bool result = false;
      await Task.Run(() =>
      {
        if (File.Exists(zipFilePath))
        {
          try
          {
            using (ZipArchive zipArchive = ZipFile.OpenRead(zipFilePath))
            {
              // 압축 풀 폴더에 이미 존재하면 삭제
              if (Directory.Exists(Path.Combine(targetFilePath, zipArchive.Entries[0].FullName)))
                Directory.Delete(Path.Combine(targetFilePath, zipArchive.Entries[0].FullName), true);

              foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
              {
                string folderPath = Path.GetDirectoryName(Path.Combine(targetFilePath, zipArchiveEntry.FullName));

                if (!Directory.Exists(folderPath))
                  Directory.CreateDirectory(folderPath);

                // 경로명은 제외
                if (!string.IsNullOrWhiteSpace(zipArchiveEntry.FullName) && !zipArchiveEntry.FullName.EndsWith("\\") && !zipArchiveEntry.FullName.EndsWith("/"))
                  zipArchiveEntry.ExtractToFile(Path.Combine(targetFilePath, zipArchiveEntry.FullName));
              }
            }
            result = true;
          }
          catch
          {
            result = false;
          }
        }
        else
          result = false;
      });
      return result;
    }

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
