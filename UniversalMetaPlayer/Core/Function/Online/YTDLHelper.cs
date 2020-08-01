using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NYoutubeDL;
using NYoutubeDL.Helpers;
using NYoutubeDL.Options;
using UMP.Core.Model;
using UMP.Utility;

namespace UMP.Core.Function.Online
{
  public class YTDLHelper
  {
    private readonly string YTDL_PATH = Path.Combine(GlobalProperty.LIBRARY_PATH, "YTDL", "youtube-dl.exe");
    private readonly string FFMPEG_PATH = Path.Combine(GlobalProperty.LIBRARY_PATH, "FFmpeg");

    private readonly string CachePath = Path.Combine(GlobalProperty.CACHE_PATH, "DownloadCache");

    public event EventHandler<string> ErrorEvent;
    public event EventHandler<string> OutPutEvent;
    public event PropertyChangedEventHandler InfoChangedEvent;

    public async Task<GenericResult<string>> DownloadAudioAsync(string uri, string savePath, int quality = 5)
    {
      Log log = new Log("AudioDownloader");
      log.Debug("오디오 다운로드 시도", $"Uri : [{uri}]\nPath : [{savePath}]");

      if (Checker.CheckForInternetConnection())
      {
        YoutubeDL ytdl = new YoutubeDL(YTDL_PATH) { VideoUrl = uri };
        string error = string.Empty;

        string id;
        var idResult = await GetIDAsync(uri);
        if (!idResult.Success)
        {
          log.Error("오디오 ID 파싱 오류 발생", $"Uri : [{uri}]\nPath : [{savePath}]");
          return new GenericResult<string>(false);
        }
        else
        {
          id = idResult.Result;
        }

        quality = Math.Clamp(quality, 0, 10);

        // 변환 실패 오류 패치
        //ytdl.Options.PostProcessingOptions.EmbedThumbnail = true;

        ytdl.Options.PostProcessingOptions.FfmpegLocation = FFMPEG_PATH;
        ytdl.Options.PostProcessingOptions.AudioQuality = quality.ToString();
        ytdl.Options.PostProcessingOptions.ExtractAudio = true;
        ytdl.Options.PostProcessingOptions.AudioFormat = Enums.AudioFormat.mp3;
        ytdl.Options.PostProcessingOptions.ConvertSubs = Enums.SubtitleFormat.srt;
        ytdl.Options.PostProcessingOptions.EmbedSubs = true;
        ytdl.Options.PostProcessingOptions.AddMetadata = true;

        try
        {
          savePath = Path.GetFullPath(savePath);
        }
        catch (Exception e)
        {
          log.Error("저장 경로 파싱 오류 발생", e, $"Uri : [{uri}]\nPath : [{savePath}]");
          return new GenericResult<string>(false);
        }

        Checker.DirectoryCheck(Path.GetDirectoryName(CachePath));
        string saveStreamPath = Path.Combine(CachePath, $"{id}.%(ext)s");
        ytdl.Options.FilesystemOptions.Output = saveStreamPath;

        ytdl.StandardErrorEvent += (s, e) => { error = $"{e}\n"; ErrorEvent?.Invoke(s, e); };
        ytdl.StandardOutputEvent += (s, e) => OutPutEvent?.Invoke(s, e);
        ytdl.InfoChangedEvent += (s, e) => InfoChangedEvent?.Invoke(s, e);

        await ytdl.DownloadAsync();

        string result = string.Empty;
        string[] resultPath = Directory.GetFiles(CachePath, $"{id}.mp3", SearchOption.TopDirectoryOnly);
        if (resultPath.Length <= 0)
          error += "Download Fail (Download File is Not Exists.)\n";
        else
        {
          result = Path.Combine(savePath, Path.GetFileName(resultPath[0]));
          Checker.DirectoryCheck(savePath);
          try
          {
            File.Move(Path.GetFullPath(resultPath[0]), result, true);
          }
          catch (Exception e)
          {
            error += $"{e}\n";
            log.Error("파일 이동 오류", e, $"Result Path : [{resultPath[0]}]");
          }
        }

        if (string.IsNullOrWhiteSpace(error))
        {
          log.Info("오디오 다운로드 완료", $"Uri : [{uri}]\nPath : [{savePath}]");
          return new GenericResult<string>(true, result);
        }
        else
        {
          log.Error($"오디오 다운로드 오류 발생.\n{error[0..^1]}", $"Uri : [{uri}]\nPath : [{savePath}]");
          return new GenericResult<string>(false);
        }
      }
      else
      {
        log.Error(PredefineMessage.UnableNetwork, $"Uri : [{uri}]\nPath : [{savePath}]");
        return new GenericResult<string>(false);
      }
    }

    public async Task<GenericResult<string>> GetIDAsync(string uri)
    {
      Log log = new Log("IDParser");

      if (Checker.CheckForInternetConnection())
      {
        YoutubeDL ytdl = new YoutubeDL(YTDL_PATH) { VideoUrl = uri };

        ytdl.Options.VerbositySimulationOptions.Simulate = true;
        ytdl.Options.VerbositySimulationOptions.SkipDownload = true;
        ytdl.Options.VerbositySimulationOptions.GetId = true;

        string result = string.Empty;
        string error = string.Empty;

        ytdl.StandardErrorEvent += (s, e) => { error = $"{e}\n"; ErrorEvent?.Invoke(s, e); };
        ytdl.StandardOutputEvent += (s, e) => { result = e; OutPutEvent?.Invoke(s, e); };

        await ytdl.DownloadAsync();

        if (!string.IsNullOrWhiteSpace(result))
        {
          if (!string.IsNullOrWhiteSpace(error))
            log.Warn($"ID 파싱 경고\n{error[0..^1]}", $"Uri : [{uri}]");
          return new GenericResult<string>(true, result);
        }
        else
        {
          log.Error($"ID 파싱 실패\n{error[0..^1]}", $"Uri : [{uri}]");
          return new GenericResult<string>(false);
        }
      }
      else
      {
        log.Error(PredefineMessage.UnableNetwork, $"Uri : [{uri}]");
        return new GenericResult<string>(false);
      }
    }
  }
}