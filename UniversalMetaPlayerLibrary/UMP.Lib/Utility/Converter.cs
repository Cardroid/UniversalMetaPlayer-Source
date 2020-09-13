using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using UMP.Lib.Utility.Model;

namespace UMP.Lib.Utility
{
  public class Converter
  {
    private Converter()
    {
    }

    private static Converter Instence = null;

    public static Converter GetConverter()
    {
      if (Instence == null)
        Instence = new Converter();

      return Instence;
    }

    /// <summary>
    /// 시간을 문자열로 변환합니다
    /// </summary>
    /// <param name="value">시간으로 바꿀 TimeSpan</param>
    /// <returns>문자열</returns>
    public string TimeSpanStringConverter(TimeSpan value)
    {
      if (value == TimeSpan.Zero)
        return "00:00";
      if (value.TotalDays > 1)
        return value.ToString(@"d\:hh\:mm\:ss");
      else if (value.TotalHours > 1)
        return value.ToString(@"h\:mm\:ss");
      else if (value.TotalSeconds > 1)
        return value.ToString(@"mm\:ss");
      else
        //return value.ToString(@"s\:FFF");
        return "00:00";
    }

    /// <summary>
    /// ZIP 파일 추출합니다
    /// </summary>
    /// <param name="zipFilePath">ZIP 파일 경로</param>
    /// <param name="targetFilePath">백업 폴더</param>
    public async Task<bool> ExtractZIPFileAsync(string zipFilePath, string targetFilePath)
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
    /// 오디오 파일을 Mp3파일로 변환합니다.
    /// </summary>
    /// <param name="sourceFilename">소스 파일 경로</param>
    /// <param name="targetFilename">출력 파일 경로</param>
    /// <param name="progress">진행 정보</param>
    /// <param name="minProgressInvokeTime">갱신 빈도</param>
    /// <returns>성공시 true</returns>
    public async Task<bool> ConvertToMP3Async(string sourceFilename, string targetFilename, IUMP_Progress<double> progress, int minProgressInvokeTime = 100)
    {
      if (!string.IsNullOrWhiteSpace(sourceFilename) && !string.IsNullOrWhiteSpace(targetFilename) && File.Exists(sourceFilename))
      {
        if (!Regex.IsMatch(targetFilename, @"(\.[Mm][Pp]3)$"))
          targetFilename += ".mp3";

        try
        {
          using var reader = new NAudio.Wave.AudioFileReader(sourceFilename);
          using var writer = new NAudio.Lame.LameMP3FileWriter(targetFilename, reader.WaveFormat, NAudio.Lame.LAMEPreset.STANDARD) { MinProgressTime = minProgressInvokeTime };
          var length = reader.Length;

          progress.Report(0, "초기화 중...");
          writer.OnProgress += (_, i, o, fin) =>
          {
            if (fin)
              progress.Report(100, "완료");
            else
              progress.Report(
                i * 100 / length,
                string.Format("출력: {0:#,0} bytes, 비율: 1:{1:0.0}", o, ((double)i) / Math.Max(1, o)));
          };

          await reader.CopyToAsync(writer);
          return true;
        }
        catch (Exception e)
        {
          progress.Report(-1, $"Fatal : {e.Message}");
          return false;
        }
      }
      else
        return false;
    }

    /// <summary>
    /// 오디오 파일을 Mp3파일로 변환합니다.
    /// </summary>
    /// <param name="sourceFilename">소스 파일 경로</param>
    /// <param name="targetFilename">출력 파일 경로</param>
    /// <returns>성공시 true</returns>
    public async Task<bool> ConvertToMP3Async(string sourceFilename, string targetFilename)
    {
      if (File.Exists(sourceFilename))
      {
        try
        {
          using var reader = new NAudio.Wave.AudioFileReader(sourceFilename);
          using var writer = new NAudio.Lame.LameMP3FileWriter(targetFilename, reader.WaveFormat, NAudio.Lame.LAMEPreset.STANDARD);
          await reader.CopyToAsync(writer);
          return true;
        }
        catch
        {
          return false;
        }
      }
      else
        return false;
    }

    /// <summary>
    /// SHA256 알고리즘으로 HASH값을 구합니다
    /// </summary>
    /// <param name="data">원본 데이터</param>
    /// <returns>HASH 값</returns>
    public string SHA256Hash(string data)
    {
      SHA256 sha = new SHA256Managed();
      byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(data));
      StringBuilder stringBuilder = new StringBuilder();

      foreach (byte b in hash)
        stringBuilder.AppendFormat("{0:x2}", b);

      return stringBuilder.ToString();
    }
  }
}
