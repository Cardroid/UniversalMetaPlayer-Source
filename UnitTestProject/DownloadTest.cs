using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NYoutubeDL;

using UMP.Core.MediaLoader;
using UMP.Core.Global;
using UMP.Core.Model.Media;
using UMP.Utils;

namespace UnitTestProject
{
  [TestClass]
  public class DownloadTest
  {
    [TestMethod]
    public void GetUrl()
    {
      string[] urls = new string[]
      {
        "",
        "ht",
        "http",
        "https",
        "https://",
        "youtube.com",
        "www.youtube.com",
        "https://youtube",
        "https://youtube.com",
        "youtube.com/watch?v=EnDXGQmCz3U",
        "www.youtube.com/watch?v=EnDXGQmCz3U",
        "https://youtube.com/watch?v=EnDXGQmCz3U",
        "https://www.youtube.com:55/watch?v=EnDXGQmCz3U",
        "https://www.youtube.com/watch?v=EnDXGQmCz3U:44",
        "https://www.youtube.com/watch?v=EnDXGQmCz3U",
        "https://www.google.com/watch?v=EnDXGQmCz3U",
        "https://www.naver.com/watch?v=EnDXGQmCz3U",
      };

      for (int j = 0; j < urls.Length; j++)
      {
        var urlinfo = Parser.GetUrlInfo(urls[j]);

        Trace.WriteLine($"Url : {urlinfo.Url}");
        Trace.WriteLine($"Success : {urlinfo.Success}");
        Trace.WriteLine($"Protocol : {urlinfo.Protocol}");
        Trace.WriteLine($"Domain : {urlinfo.Domain}");
        Trace.WriteLine($"Port : {urlinfo.Port}");
        Trace.WriteLine("");
      }
    }

    [TestMethod]
    public void DownloadLog()
    {
      MediaInformation mediaInfo = new MediaInformation("https://www.youtube.com/watch?v=EnDXGQmCz3U");
      MediaLoader mediaLoader = new MediaLoader(mediaInfo);

      mediaLoader.CachePath = "TestCache";
      mediaLoader.ProgressChanged += (_, e) =>
      {
        Trace.WriteLine($"{e.ProgressKind}".PadRight(15) + $"{e.Percentage}%".PadLeft(4) + $"   {e.UserMessage}");
      };
      var result = mediaLoader.GetStreamPathAsync(false).GetAwaiter().GetResult();

      Trace.WriteLine("### Result ###");
      Trace.WriteLine(result.Success);
      if (result)
        Trace.WriteLine(result.Result);
      Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public void CheckForInternetConnection()
    {
      try
      {
        Checker.CheckForInternetConnection();
        Assert.IsTrue(true);
      }
      catch
      {
        Assert.Fail();
      }
    }

    [TestMethod]
    public void YTDL_GetID()
    {
      var result = new YTDLMediaLoader("https://www.youtube.com/watch?v=EnDXGQmCz3U").GetID().GetAwaiter().GetResult();
      Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public void YTDL_GetInfo()
    {
      string ytdl_path = Path.Combine(GlobalProperty.Predefine.LIBRARY_PATH, "YTDL", "youtube-dl.exe");
      YoutubeDL ytdl = new YoutubeDL(ytdl_path);

      ytdl.VideoUrl = "https://www.youtube.com/watch?v=EnDXGQmCz3U";

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

      ytdl.StandardErrorEvent += (s, e) => { error = $"{e}\n"; };
      ytdl.StandardOutputEvent += (s, e) => { result = e; };

      ytdl.DownloadAsync().GetAwaiter().GetResult();

      File.WriteAllText(Path.Combine("Core", "MediaInfo.txt"), result, Encoding.UTF8);
      Trace.Write($"========OUTPUT========\n{result}\n======================\n");
      Trace.WriteLine($"========ERROR========\n{error}\n=====================");
      Assert.IsTrue(!string.IsNullOrWhiteSpace(result) && string.IsNullOrWhiteSpace(error));
    }
  }
}
