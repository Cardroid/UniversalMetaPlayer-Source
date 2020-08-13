using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using Octokit;

namespace UMP.UpdateClient
{
  class Program
  {
    static bool exitSystem = false;

    #region Trap application termination
    [DllImport("Kernel32")]
    private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

    private delegate bool EventHandler(CtrlType sig);
    static EventHandler _handler;

    enum CtrlType
    {
      CTRL_C_EVENT = 0,
      CTRL_BREAK_EVENT = 1,
      CTRL_CLOSE_EVENT = 2,
      CTRL_LOGOFF_EVENT = 5,
      CTRL_SHUTDOWN_EVENT = 6
    }

    private static bool Handler(CtrlType sig)
    {
      Console.WriteLine("[업데이트 취소 요청 입력됨]");
      Thread.Sleep(1000);

      //allow main to run off
      exitSystem = true;

      //shutdown right away so there are no lingering threads
      Environment.Exit(-1);

      return true;
    }
    #endregion

    static void Main()
    {
      Console.Write("초기화 중...");
      Console.Title = "UniversalMetaPlayer Auto Update Client";

      // Some biolerplate to react to close window event, CTRL-C, kill, etc
      _handler += new EventHandler(Handler);
      SetConsoleCtrlHandler(_handler, true);

      //start your multi threaded program here
      Program p = new Program();
      Console.WriteLine("완료");
      p.Start();

      //hold the console so it doesn’t run off the end
      while (!exitSystem)
      {
        Thread.Sleep(500);
      }

      Console.WriteLine("프로그램 종료 중...");
#if DEBUG
      Console.WriteLine();
      Thread.Sleep(1000);
      Console.Write("5...");
      Thread.Sleep(1000);
      Console.Write("4...");
      Thread.Sleep(1000);
      Console.Write("3...");
      Thread.Sleep(1000);
      Console.Write("2...");
      Thread.Sleep(1000);
      Console.Write("1...");
      Thread.Sleep(1000);
#endif
      Console.Write("End");
    }

    public async void Start()
    {
      Console.WriteLine("업데이트를 시작합니다");

      if (Utility.CheckForInternetConnection())
      {
        Console.WriteLine("현재 버전 정보 가져오는 중...");
        GetCurrentVersion();
        Console.WriteLine("완료");
        Console.WriteLine("새 버전 정보 가져오는 중...");
        await GetNewVersion();
        Console.WriteLine("완료");
        if(TargetVerstion != null && TargetVerstion.CompareTo(ProgramVerstion) > 0)
        {
          Console.WriteLine("업데이트가 필요합니다");
          // TODO 업데이트 메소드 작성
          Console.WriteLine("업데이트 시작");
        }
        else
        {
          Console.WriteLine("업데이트가 필요하지 않습니다");
        }
      }
      else
      {
        Console.WriteLine("네트워크를 사용할 수 없습니다");
        Thread.Sleep(3000);
      }
      exitSystem = true;
    }

    private Dictionary<string, string> Assets { get; set; }
    private string OS_Bit => Environment.Is64BitOperatingSystem ? "x64" : "x86";
    private Version TargetVerstion { get; set; } = null;
    private Version ProgramVerstion { get; set; } = null;

    private bool GetCurrentVersion()
    {
      string file = string.Empty;
      string[] files;

      try { files = Directory.GetFiles(Environment.CurrentDirectory, "*.exe"); }
      catch { return false; }

      if (files == null || files.Length <= 0)
        return false;

      for (int i = 0; i < files.Length; i++)
      {
        if (FileVersionInfo.GetVersionInfo(files[i]).ProductName == "Universal Meta Player")
          file = files[i];
      }

      if (string.IsNullOrWhiteSpace(file))
        return false;

      ProgramVerstion = new Version(FileVersionInfo.GetVersionInfo(file).FileVersion);

      if (ProgramVerstion != null)
        Console.WriteLine($"Current Version : [{ProgramVerstion}]");
      else
        Console.WriteLine("Current Version : [정보 없음]");

      return true;
    }

    private async Task<bool> GetNewVersion()
    {
      Assets = new Dictionary<string, string>();
      var client = new GitHubClient(new ProductHeaderValue("UniversalMetaPlayer"));

      try
      {
        var releases = await client.Repository.Release.GetLatest("Cardroid", "UniversalMetaPlayer");

        if (releases == null)
          throw new ArgumentNullException();

        TargetVerstion = new Version(releases.TagName);

        for (int i = 0; i < releases.Assets.Count; i++)
          Assets.Add(releases.Assets[i].Name, releases.Assets[i].BrowserDownloadUrl);
      }
      catch
      {
        return false;
      }

      Console.WriteLine($"Target Version : [{TargetVerstion}]");
#if DEBUG
      foreach (var item in Assets.Keys)
      {
        Console.WriteLine(item);
        Console.WriteLine(Assets.TryGetValue(item, out string value) ? value : "Null");
      }
#endif
      return true;
    }
  }
}
