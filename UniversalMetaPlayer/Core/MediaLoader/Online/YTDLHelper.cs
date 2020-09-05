using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NYoutubeDL;
using NYoutubeDL.Helpers;
using NYoutubeDL.Options;
using UMP.Core.Global;
using UMP.Core.Model;
using UMP.Utility;

namespace UMP.Core.MediaLoader.Online
{
  public class YTDLHelper
  {
    public event EventHandler<string> ErrorEvent;
    public event EventHandler<string> OutPutEvent;
    public event PropertyChangedEventHandler InfoChangedEvent;

    public async Task<GenericResult<string>> DownloadAudioAsync(string uri, string savePath, int quality = 5)
    {
      Log log = new Log("AudioDownloader");
      log.Debug("오디오 다운로드 시도", $"Uri : [{uri}]\nPath : [{savePath}]");

      if (Checker.CheckForInternetConnection())
      {
        YoutubeDL ytdl = new YoutubeDL(GlobalProperty.Predefine.YTDL_PATH) { VideoUrl = uri };
        string error = string.Empty;

        string id;
        var idResult = await GetIDAsync(uri);
        if (!idResult.Success)
        {
          log.Fatal("오디오 ID 파싱 오류 발생", $"Uri : [{uri}]\nPath : [{savePath}]");
          return new GenericResult<string>(false);
        }
        else
        {
          id = idResult.Result;
        }

        quality = Math.Clamp(quality, 0, 10);

        // 변환 실패 오류 패치
        //ytdl.Options.PostProcessingOptions.EmbedThumbnail = true;

        ytdl.Options.PostProcessingOptions.FfmpegLocation = GlobalProperty.Predefine.FFMPEG_PATH;
        ytdl.Options.PostProcessingOptions.AudioQuality = quality.ToString();
        ytdl.Options.PostProcessingOptions.ExtractAudio = true;
        ytdl.Options.PostProcessingOptions.AudioFormat = NYoutubeDL.Helpers.Enums.AudioFormat.mp3;
        ytdl.Options.PostProcessingOptions.ConvertSubs = NYoutubeDL.Helpers.Enums.SubtitleFormat.srt;
        ytdl.Options.PostProcessingOptions.EmbedSubs = true;
        ytdl.Options.PostProcessingOptions.AddMetadata = true;

        try
        {
          savePath = Path.GetFullPath(savePath);
        }
        catch (Exception e)
        {
          log.Fatal("저장 경로 파싱 오류 발생", e, $"Uri : [{uri}]\nPath : [{savePath}]");
          return new GenericResult<string>(false);
        }

        Checker.DirectoryCheck(Path.GetDirectoryName(GlobalProperty.Predefine.DownloadCachePath));
        string saveStreamPath = Path.Combine(GlobalProperty.Predefine.DownloadCachePath, $"{id}.%(ext)s");
        ytdl.Options.FilesystemOptions.Output = saveStreamPath;

        ytdl.StandardErrorEvent += (s, e) => { error = $"{e}\n"; ErrorEvent?.Invoke(s, e); };
        ytdl.StandardOutputEvent += (s, e) => OutPutEvent?.Invoke(s, e);
        ytdl.InfoChangedEvent += (s, e) => InfoChangedEvent?.Invoke(s, e);

        await ytdl.DownloadAsync();

        string result = string.Empty;
        string[] resultPath = Directory.GetFiles(GlobalProperty.Predefine.DownloadCachePath, $"{id}.mp3", SearchOption.TopDirectoryOnly);
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
            log.Fatal("파일 이동 오류", e, $"Result Path : [{resultPath[0]}]");
          }
        }

        if (string.IsNullOrWhiteSpace(error))
        {
          log.Info("오디오 다운로드 완료", $"Uri : [{uri}]\nPath : [{savePath}]");
          return new GenericResult<string>(true, result);
        }
        else
        {
          log.Fatal($"오디오 다운로드 오류 발생\n{error[0..^1]}", $"Uri : [{uri}]\nPath : [{savePath}]");
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
        YoutubeDL ytdl = new YoutubeDL(GlobalProperty.Predefine.YTDL_PATH) { VideoUrl = uri };

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
          log.Fatal($"ID 파싱 실패\n{error[0..^1]}", $"Uri : [{uri}]");
          return new GenericResult<string>(false);
        }
      }
      else
      {
        log.Error(PredefineMessage.UnableNetwork, $"Uri : [{uri}]");
        return new GenericResult<string>(false);
      }
    }

    public async Task<GenericResult<JObject>> GetJsonInfoAsync(string uri)
    {
      Log log = new Log("IDParser");

      if (Checker.CheckForInternetConnection())
      {
        YoutubeDL ytdl = new YoutubeDL(GlobalProperty.Predefine.YTDL_PATH) { VideoUrl = uri };

        ytdl.Options.VerbositySimulationOptions.Simulate = true;
        ytdl.Options.VerbositySimulationOptions.SkipDownload = true;
        ytdl.Options.VerbositySimulationOptions.GetId = true;
        ytdl.Options.VerbositySimulationOptions.GetDescription = true;
        ytdl.Options.VerbositySimulationOptions.GetDuration = true;
        ytdl.Options.VerbositySimulationOptions.GetFilename = true;
        ytdl.Options.VerbositySimulationOptions.GetFormat = true;
        ytdl.Options.VerbositySimulationOptions.GetThumbnail = true;
        ytdl.Options.VerbositySimulationOptions.GetTitle = true;
        ytdl.Options.VerbositySimulationOptions.GetUrl = true;
        ytdl.Options.VerbositySimulationOptions.DumpSingleJson = true;

        string result = string.Empty;
        string error = string.Empty;

        ytdl.StandardErrorEvent += (s, e) => { error = $"{e}\n"; ErrorEvent?.Invoke(s, e); };
        ytdl.StandardOutputEvent += (s, e) => { result = e; OutPutEvent?.Invoke(s, e); };

        await ytdl.DownloadAsync();

        if (!string.IsNullOrWhiteSpace(result))
        {
          if (!string.IsNullOrWhiteSpace(error))
            log.Warn($"JsonInfo 파싱 경고\n{error[0..^1]}", $"Uri : [{uri}]");
          return new GenericResult<JObject>(true, JsonConvert.DeserializeObject<JObject>(result));
        }
        else
        {
          log.Fatal($"JsonInfo 파싱 실패\n{error[0..^1]}", $"Uri : [{uri}]");
          return new GenericResult<JObject>(false);
        }
      }
      else
      {
        log.Error(PredefineMessage.UnableNetwork, $"Uri : [{uri}]");
        return new GenericResult<JObject>(false);
      }
    }
  }
}