using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using UMP.Utility;

//using NYoutubeDL;

//using UMP.Core;
//using UMP.Core.Function;
//using UMP.Core.Model;
//using UMP.Utility;

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
        "youtube.com/watch?v=hTPjKrkN7XA",
        "www.youtube.com/watch?v=hTPjKrkN7XA",
        "https://youtube.com/watch?v=hTPjKrkN7XA",
        "https://www.youtube.com:55/watch?v=hTPjKrkN7XA",
        "https://www.youtube.com/watch?v=hTPjKrkN7XA:44",
        "https://www.youtube.com/watch?v=hTPjKrkN7XA",
        "https://www.google.com/watch?v=hTPjKrkN7XA",
        "https://www.naver.com/watch?v=hTPjKrkN7XA",
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

    //[TestMethod]
    //public void CheckForInternetConnection()
    //{
    //  try
    //  {
    //    Checker.CheckForInternetConnection();
    //    Assert.IsTrue(true);
    //  }
    //  catch
    //  {
    //    Assert.Fail();
    //  }
    //}

    //[TestMethod]
    //public void GetID()
    //{
    //  var result = new YTDLMediaLoader("https://www.youtube.com/watch?v=kJQP7kiw5Fk").GetID().GetAwaiter().GetResult();
    //  Assert.IsTrue(result.Success);
    //}

    //[TestMethod]
    //public void YTDL_GetInfo_Test()
    //{
    //  string ytdl_path = Path.Combine(GlobalProperty.LIBRARY_PATH, "YTDL", "youtube-dl.exe");
    //  YoutubeDL ytdl = new YoutubeDL(ytdl_path);

    //  ytdl.VideoUrl = "https://www.youtube.com/watch?v=kJQP7kiw5Fk";

    //  ytdl.Options.VerbositySimulationOptions.Simulate = true;
    //  ytdl.Options.VerbositySimulationOptions.SkipDownload = true;
    //  ytdl.Options.VerbositySimulationOptions.GetId = true;
    //  ytdl.Options.VerbositySimulationOptions.GetDescription = true;
    //  ytdl.Options.VerbositySimulationOptions.GetDuration = true;
    //  ytdl.Options.VerbositySimulationOptions.GetFilename = true;
    //  ytdl.Options.VerbositySimulationOptions.GetFormat = true;
    //  ytdl.Options.VerbositySimulationOptions.GetThumbnail = true;
    //  ytdl.Options.VerbositySimulationOptions.GetTitle = true;
    //  ytdl.Options.VerbositySimulationOptions.GetUrl = true;
    //  ytdl.Options.VerbositySimulationOptions.DumpSingleJson = true;

    //  string result = string.Empty;
    //  string error = string.Empty;

    //  ytdl.StandardErrorEvent += (s, e) => { error = $"{e}\n"; };
    //  ytdl.StandardOutputEvent += (s, e) => { result = e; };

    //  ytdl.DownloadAsync().GetAwaiter().GetResult();

    //  File.WriteAllText(Path.Combine("Core", "MediaInfo.txt"), result, Encoding.UTF8);
    //  Trace.Write($"========OUTPUT========\n{result}\n======================\n");
    //  Trace.WriteLine($"========ERROR========\n{error}\n=====================");
    //  Assert.IsTrue(!string.IsNullOrWhiteSpace(result) && string.IsNullOrWhiteSpace(error));
    //}
  }
}
