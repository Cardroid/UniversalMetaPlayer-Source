using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
      Console.WriteLine("[취소 요청 입력됨]");

      //allow main to run off
      exitSystem = true;

      //shutdown right away so there are no lingering threads
      //Environment.Exit(-1);

      return true;
    }
    #endregion

    static void Main()
    {
      Console.Write("초기화 중...");
      AppDomain.CurrentDomain.UnhandledException += (_, e) =>
      {
        string message = "처리되지 않은 오류가 있습니다.";
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        if (e.ExceptionObject != null)
        {
          if (!Directory.Exists("Log"))
            Directory.CreateDirectory("Log");
          File.AppendAllText(@"Log\UpdateCrash_Report.log", $"{DateTime.Now} [{message}]\n{e.ExceptionObject as Exception}\n\n");
        }
      };
      Console.SetError(ErrorWriter);
      Console.Title = "UniversalMetaPlayer Auto Update Client";

      // Some biolerplate to react to close window event, CTRL-C, kill, etc
      _handler += new EventHandler(Handler);
      SetConsoleCtrlHandler(_handler, true);

      //start your multi threaded program here
      Program p = new Program();
      Console.WriteLine("완료");
      p.Start();

      //hold the console so it doesn’t run off the end
      while (!exitSystem) { Thread.Sleep(500); }

      Console.WriteLine("프로그램 종료 중...");
      Clean();

      if (IsErrorExist)
      {
        Console.WriteLine("오류가 존재합니다\n30초 후 자동으로 종료됩니다");
        Thread.Sleep(30000);
      }

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


      if (!Utility.CheckForInternetConnection())
      {
        WriteError("네트워크를 사용할 수 없습니다");
        Thread.Sleep(3000);
        exitSystem = true;
        return;
      }


      Console.WriteLine("현재 버전 정보 가져오는 중...");
      var result = GetCurrentVersion();
      if (result.HasValue == false)
      {
        exitSystem = true;
        return;
      }

      Console.WriteLine("새 버전 정보 가져오는 중...");
      await GetNewVersion();


      if (TargetVerstion == null)
      {
        WriteError("새 버전 정보를 가져오지 못했습니다");
        Thread.Sleep(2000);
        exitSystem = true;
        return;
      }


      if (TargetVerstion.CompareTo(ProgramVerstion) <= 0)
      {
        Console.WriteLine("업데이트가 필요하지 않습니다");
        Thread.Sleep(2000);
        exitSystem = true;
        return;
      }
      else
        Console.WriteLine("업데이트가 필요합니다");


      Console.WriteLine("업데이트 파일 다운로드 중...");
      // TODO 업데이트 메소드 작성
      if (!await DownloadUpdateAsync())
      {
        WriteError("다운로드 실패");
        Thread.Sleep(3000);
        exitSystem = true;
        return;
      }


      Console.WriteLine("UniversalMetaPlayer의 좀료를 기다리는 중...");
      while (Utility.IsProcessRun("UniversalMetaPlayer"))
      {
        Thread.Sleep(500);
      }
      Console.WriteLine("종료 확인");


      if (await UnZip())
      {
        Console.WriteLine("업데이트 완료");
      }
      else
      {
        WriteError("압축 해제 실패");
        Thread.Sleep(3000);
        exitSystem = true;
        return;
      }


      Console.Write("UMP 실행...");
      try
      {
        Process.Start("UniversalMetaPlayer.exe");
        Console.WriteLine("완료");
      }
      catch (Exception e)
      {
        WriteError("실패", e);
        Thread.Sleep(3000);
        exitSystem = true;
        return;
      }
    }

    private static TextWriter ErrorWriter { get; } = TextWriter.Null;
    private static bool IsErrorExist = false;
    private Dictionary<string, string> Assets { get; set; }
    private static string OS_Bit => Environment.Is64BitOperatingSystem ? "x64" : "x86";
    private Version TargetVerstion { get; set; } = null;
    private Version ProgramVerstion { get; set; } = null;
    private int DownloadCursorPostion { get; set; }
    private bool DownloadComplate { get; set; } = false;
    private string DownloadFilePath { get; set; }
    private const string DownloadPath = @"Cache\UpdateCache";

    private bool? GetCurrentVersion()
    {
      string file = string.Empty;
      string[] files;

      try { files = Directory.GetFiles(Environment.CurrentDirectory, "*.exe"); }
      catch (Exception e)
      {
        WriteError("파일 목록 로드 중 오류발생", e);
        return false;
      }

      if (files == null || files.Length <= 0)
        return false;

      for (int i = 0; i < files.Length; i++)
      {
        if (FileVersionInfo.GetVersionInfo(files[i]).ProductName == "Universal Meta Player")
          file = files[i];
      }

      if (string.IsNullOrWhiteSpace(file))
      {
        Console.WriteLine("업데이트 대상 파일(Universal Meta Player)을 찾지 못했습니다");
        Console.WriteLine("최신버전을 설치합니다");

        int fileCount = Directory.GetFiles(Environment.CurrentDirectory).Length + Directory.GetDirectories(Environment.CurrentDirectory).Length;
        if (fileCount > 1)
        {
          Console.WriteLine();
          var beforeColor = Console.ForegroundColor;
          Console.ForegroundColor = ConsoleColor.Yellow;
          Console.WriteLine($"현재 폴더에 {fileCount}개의 파일 및 폴더가 있습니다");
          Console.WriteLine($"설치를 계속할 경우 본 프로그램의 구성요소가 다른 파일 및 폴더와 혼합될 수 있습니다");
          Console.WriteLine($"설치를 계속하시곘습니까? [(Y/N) 입력 후 엔터]");
          Console.ForegroundColor = beforeColor;

          string input = Console.ReadLine();
          if (Regex.IsMatch(input, "[Yy]"))
            return true;
          else
          {
            Console.WriteLine($"작업이 취소 되었습니다");
            return null;
          }
        }
        return false;
      }

      ProgramVerstion = new Version(FileVersionInfo.GetVersionInfo(file).FileVersion);

      if (ProgramVerstion != null)
        Console.WriteLine($"Current Version : [{ProgramVerstion}]");
      else
        Console.WriteLine("Current Version : [정보 없음]");

      Console.WriteLine("완료");
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
      catch (Exception e)
      {
        WriteError("원격지의 버전을 확인할 수 없습니다", e);
        TargetVerstion = null;
        return false;
      }

      Console.WriteLine($"Target Version : [{TargetVerstion}]");
      Console.WriteLine("완료");
      return true;
    }

    #region DownloadUpdateFiles
    private static WebClient webClient { get; set; }

    private async Task<bool> DownloadUpdateAsync()
    {
      if (Assets == null)
        return false;

      if (!Directory.Exists(DownloadPath))
        Directory.CreateDirectory(DownloadPath);
      DownloadFilePath = Path.Combine(DownloadPath, "update.zip");

      if (webClient == null)
        webClient = new WebClient();
      webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
      DownloadComplate = false;

      Console.WriteLine($"[{OS_Bit}]Bit OS 감지됨");
      Console.WriteLine("다운로드 시작...\n");
      DownloadCursorPostion = Console.CursorTop;

      try
      {
        await webClient.DownloadFileTaskAsync(new Uri(Assets[OS_Bit + ".zip"]), DownloadFilePath);
      }
      catch (Exception e)
      {
        WriteError("파일 다운로드 중 오류 발생", e);
        return false;
      }
      finally
      {
        webClient.Dispose();
        webClient = null;
      }

      DownloadComplate = true;
      Console.WriteLine("완료");
      return true;
    }

    private bool IsDisplayWork = false;

    private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
      if (IsDisplayWork)
        return;
      IsDisplayWork = true;
      string result = $"[{e.ProgressPercentage}%]  {e.BytesReceived}/{e.TotalBytesToReceive} Byte";

      for (int i = result.Length; i < Console.BufferWidth; i++)
        result += ' ';

      Console.Write(result);
      Console.SetCursorPosition(0, DownloadCursorPostion);
      IsDisplayWork = false;
    }
    #endregion

    private async Task<bool> UnZip()
    {
      if (DownloadComplate && !string.IsNullOrWhiteSpace(DownloadFilePath))
      {
        Console.WriteLine("압축 푸는 중...");
        try
        {
          await Utility.ExtractZIPFileAsync(DownloadFilePath, Environment.CurrentDirectory, true);
        }
        catch (Exception e)
        {
          WriteError("압축 해제 중 오류 발생", e);
          return false;
        }
        if (!Directory.Exists(OS_Bit))
          return false;
        Console.WriteLine("완료");

        Console.WriteLine("파일 이동 중...");
        try
        {
          var files = Directory.GetFiles(OS_Bit, "*.*");
          for (int i = 0; i < files.Length; i++)
            File.Move(files[i], Path.Combine(Environment.CurrentDirectory, Path.GetFileName(files[i])), true);
        }
        catch (Exception e)
        {
          WriteError("파일 이동 중 오류 발생", e);
          return false;
        }
        Console.WriteLine("완료");
        return true;
      }
      else
      {
        WriteError("다운로드가 정상적으로 완료되지 않았습니다");
        return false;
      }
    }

    private static void Clean()
    {
      Console.WriteLine("정리 중...");

      if (webClient != null)
        webClient.Dispose();

      if (Directory.Exists(DownloadPath))
      {
        var cacheFiles = Directory.GetFiles(DownloadPath, "*.*");

        if (cacheFiles != null && cacheFiles.Length > 0)
        {
          for (int i = 0; i < cacheFiles.Length; i++)
          {
            try
            {
              File.Delete(cacheFiles[i]);
            }
            catch { }
          }
        }

        try
        {
          Directory.Delete(DownloadPath, true);
        }
        catch { }
      }

      if (Directory.Exists(OS_Bit))
        Directory.Delete(OS_Bit, true);

      Console.WriteLine("완료");
    }

    private static void WriteError(string message) => WriteError(message, null);
    private static void WriteError(string message, Exception exception)
    {
      IsErrorExist = true;
      var beforeFgColor = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"{DateTime.Now} [{message}]\n{exception}\n\n");
      if (exception != null)
      {
        if (!Directory.Exists("Log"))
          Directory.CreateDirectory("Log");
        File.AppendAllText(@"Log\UpdateCrash_Report.log", $"{DateTime.Now} [{message}]\n{exception}\n\n");
      }
      ErrorWriter.WriteLine(message);
      Console.ForegroundColor = beforeFgColor;
    }
  }
}