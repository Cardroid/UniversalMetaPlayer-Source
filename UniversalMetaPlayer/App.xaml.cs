using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using UMP.Core;

namespace UMP
{
  public partial class App : Application
  {
    public App()
    {
      MainLog.Info("############### Start application ###############\n" +
        $"Current Version : [{GlobalProperty.StaticValues.FileVersion}]\nBit : [{GlobalProperty.StaticValues.BitVersion}]",
        $"Start Path : [{AppDomain.CurrentDomain.BaseDirectory}]\nTask Path : [{Environment.CurrentDirectory}]");

      // Application의 모든 에러 이벤트를 로그로 남깁니다
      //#if DEBUG
      //      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
      //      DispatcherUnhandledException += App_DispatcherUnhandledException;
      //      AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
      //#endif
    }

    public static Log MainLog { get; } = new Log("System");

    // Application의 모든 에러 이벤트를 로그로 남깁니다
    //#if DEBUG
    //    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) =>
    //      MainLog.Error($"\n\n[CurrentDomain Exception] Terminating : [{e.IsTerminating}]", e.ExceptionObject as Exception);

    //    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) =>
    //      MainLog.Error($"\n\n[Dispatcher Exception] Handled : [{e.Handled}] Dispatcher Thread : [{e.Dispatcher.Thread}]", e.Exception);

    //    private void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e) =>
    //      MainLog.Error($"\n\n[CurrentDomain FirstChanceException]", e.Exception);
    //#endif
  }
}
