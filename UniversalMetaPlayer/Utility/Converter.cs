using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using UMP.Core;

namespace UMP.Utility
{
  public class TimeSpanStringConverter : IValueConverter
  {
    /// <summary>
    /// 값 변환기 메소드 (인터페이스 구현용)
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Converter.TimeSpanStringConverter((TimeSpan)value);
    }

    /// <summary>
    /// 값 변환기 메소드 (인터페이스 구현용)
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  public class StarWidthConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      ListView listview = value as ListView;
      double width = listview.Width;
      GridView gv = listview.View as GridView;
      for (int i = 0; i < gv.Columns.Count; i++)
      {
        if (!double.IsNaN(gv.Columns[i].Width))
          width -= gv.Columns[i].Width;
      }
      return width - 5;// this is to take care of margin/padding
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  public static class Converter
  {
    /// <summary>
    /// 시간을 문자열로 변환합니다,
    /// </summary>
    /// <param name="value">시간으로 바꿀 TimeSpan</param>
    /// <returns>문자열</returns>
    public static string TimeSpanStringConverter(TimeSpan value)
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
    /// ZIP 파일 추출하기
    /// </summary>
    /// <param name="zipFilePath">ZIP 파일 경로</param>
    /// <param name="targetFilePath">백업 폴더</param>
    public static async Task<bool> ExtractZIPFileAsync(string zipFilePath, string targetFilePath)
    {
      bool result = false;
      Log log = new Log($"{typeof(Converter)} <ZIP_Extractor>");
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
          catch (Exception e)
          {
            log.Fatal("압축 풀기 오류 발생", e, $"Zip Path : [{zipFilePath}]\nTarget Path : [{targetFilePath}]");
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
    /// <param name="sourceFilename">소스가 될 파일 경로</param>
    /// <param name="targetFilename">타겟파일 경로</param>
    /// <returns>성공시 true</returns>
    public static async Task<bool> ConvertToMP3Async(string sourceFilename, string targetFilename)
    {
      if (File.Exists(sourceFilename))
      {
        using var reader = new NAudio.Wave.AudioFileReader(sourceFilename);
        using var writer = new NAudio.Lame.LameMP3FileWriter(targetFilename, reader.WaveFormat, NAudio.Lame.LAMEPreset.STANDARD);
        await reader.CopyToAsync(writer);
        return true;
      }
      else
        return false;
    }
  }
}
