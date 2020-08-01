using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using UMP.Utility;

namespace UMP.Core.Function.Online
{
  public class LibraryDownloader
  {
    public event AsyncCompletedEventHandler Completed;
    public event DownloadProgressChangedEventHandler ProgressChanged;

    public async Task<bool> DownloadFFmpegAsync()
    {
      Log log = new Log($"{typeof(LibraryDownloader)} <FFmpeg>");
      var libraryCachePath = Path.Combine(GlobalProperty.CACHE_PATH, "ffmpeg");
      WebClient webClient = new WebClient();

      webClient.DownloadFileCompleted += (s, e) => { Completed?.Invoke(s, e); };
      webClient.DownloadProgressChanged += (s, e) => { ProgressChanged?.Invoke(s, e); };

      Checker.DirectoryCheck(libraryCachePath);
      try
      {
        await webClient.DownloadFileTaskAsync(new Uri("https://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-latest-win64-static.zip"), Path.Combine(libraryCachePath, "ffmpeg.zip"));
      }
      catch (Exception e)
      {
        log.Error("라이브러리 다운로드 오류", e);
        return false;
      }
      finally
      {
        webClient.Dispose();
      }

      if (File.Exists(Path.Combine(libraryCachePath, "ffmpeg.zip")))
      {
        // 암축푸는 코드
        if (await Converter.ExtractZIPFileAsync(Path.Combine(libraryCachePath, "ffmpeg.zip"), libraryCachePath))
        {
          var libraryPath = Path.Combine(GlobalProperty.LIBRARY_PATH, "FFmpeg");
          if (Directory.Exists(libraryPath))
            Directory.Delete(libraryPath, true);
          Directory.CreateDirectory(libraryPath);

          var exeFiles = Directory.GetFiles(libraryCachePath, "*.exe", SearchOption.AllDirectories);
          for (int i = 0; i < exeFiles.Length; i++)
            File.Move(exeFiles[i], Path.Combine(libraryPath, Path.GetFileName(exeFiles[i])));

          Directory.Delete(libraryCachePath, true);

          if (!Checker.CheckFFmpeg())
          {
            log.Error("라이브러리 다운로드 중 알 수 없는 오류 발생", $"Cache Path : [{Path.GetFullPath(libraryCachePath)}]\nLibrary Path : [{Path.GetFullPath(libraryPath)}]");
            Directory.Delete(libraryCachePath, true);
            return false;
          }

          log.Info("라이브러리 다운로드 완료");
          return true;
        }
        else
        {
          log.Error("라이브러리 압축 파일 추출 오류 발생");
          return false;
        }
      }
      else
      {
        log.Error("라이브러리 다운로드 오류", new FileNotFoundException("Not Found \"ffmpeg file\" in Cache"));
        return false;
      }
    }

    public async Task<bool> DownloadYtdlAsync()
    {
      Log log = new Log($"{typeof(LibraryDownloader)} <youtube-dl>");
      var libraryCachePath = Path.Combine(GlobalProperty.CACHE_PATH, "ytdl");
      var ytdlFileName = "youtube-dl.exe";
      WebClient webClient = new WebClient();

      webClient.DownloadFileCompleted += (s, e) => { Completed?.Invoke(s, e); };
      webClient.DownloadProgressChanged += (s, e) => { ProgressChanged?.Invoke(s, e); };

      Checker.DirectoryCheck(libraryCachePath);
      try
      {
        await webClient.DownloadFileTaskAsync(new Uri("https://github.com/ytdl-org/youtube-dl/releases/latest/download/youtube-dl.exe"), Path.Combine(libraryCachePath, ytdlFileName));
      }
      catch (Exception e)
      {
        log.Error("라이브러리 다운로드 오류", e);
        return false;
      }
      finally
      {
        webClient.Dispose();
      }

      if (File.Exists(Path.Combine(libraryCachePath, ytdlFileName)))
      {
        var libraryPath = Path.Combine(GlobalProperty.LIBRARY_PATH, "YTDL");
        Checker.DirectoryCheck(libraryPath);
        File.Move(Path.Combine(libraryCachePath, ytdlFileName), Path.Combine(libraryPath, ytdlFileName));
        Directory.Delete(libraryCachePath, true);

        if (!Checker.CheckYTDL())
        {
          log.Error("라이브러리 다운로드 중 알 수 없는 오류 발생", $"Cache Path : [{Path.GetFullPath(libraryCachePath)}]\nLibrary Path : [{Path.GetFullPath(libraryPath)}]");
          Directory.Delete(libraryCachePath, true);
          return false;
        }

        log.Info("라이브러리 다운로드 완료");
        return true;
      }
      else
      {
        log.Error("라이브러리 다운로드 오류", new FileNotFoundException($"Not Found \"{ytdlFileName}\" in Cache"));
        return false;
      }
    }
  }
}
