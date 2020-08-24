using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    public void CheckUrl()
    {
      List<string> result = new List<string>();
      string[] urls=  new string[] 
      {
        "",
        "ht",
        "http",
        "https",
        "https://",
        "https://youtube",
        "https://youtube.com",
        "https://youtube.com/watch?v=hTPjKrkN7XA",
        "https://www.youtube.com/watch?v=hTPjKrkN7XA",
        "https://www.google.com/watch?v=hTPjKrkN7XA",
        "https://www.naver.com/watch?v=hTPjKrkN7XA",
      };

      for (int i = 0; i < urls.Length; i++)
      {
        var item = Regex.Match(urls[i], @"(http(s)?://)?(w{0,3}\.)?([a-zA-Z0-9]+)\.");
        result.Add(item.Groups[item.Groups.Count - 1].Value);
      }

      for (int i = 0; i < result.Count; i++)
        Trace.WriteLine(result[i]);

      Assert.IsTrue(result.Count > 0);
    }

    [TestMethod]
    public void tests()
    {
      Trace.WriteLine(Path.GetInvalidFileNameChars().Length);
      Trace.WriteLine(Path.GetInvalidPathChars().Length);
      Assert.IsTrue(true);
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
