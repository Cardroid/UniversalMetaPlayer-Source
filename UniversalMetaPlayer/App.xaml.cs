using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using UMP.Core;
using UMP.Core.Global;
using UMP.Utility;

namespace UMP
{
  public partial class App : Application
  {
    public static Log MainLog { get; } = new Log("System");
    public App()
    {
#if DEBUG
      Log.LogViewerAppender.IsEnable = true;
#endif

      //#if DEBUG
      //      GlobalEventLogger.IsEnabled = true;
      //#else
      GlobalEventLogger.IsEnabled = false;
      //#endif

      MainLog.Info("############### Start application ###############\n" +
        $"Current Version : [{GlobalProperty.Predefine.FileVersion}]\nBit : [{GlobalProperty.Predefine.BitVersion}]",
        $"Start Path : [{AppDomain.CurrentDomain.BaseDirectory}]\nTask Path : [{Environment.CurrentDirectory}]");
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
      Application.Current.DispatcherUnhandledException += DispatcherOnUnhandledException;
      TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
    }

    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
    {
      if (unhandledExceptionEventArgs.IsTerminating)
        MainLog.Fatal("[Unhandled Exception] Current Domain On Unhandled Exception", (Exception)unhandledExceptionEventArgs.ExceptionObject);
    }

    private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
    {
      if(!dispatcherUnhandledExceptionEventArgs.Handled)
        MainLog.Fatal("[Unhandled Exception] Dispatcher On Unhandled Exception", dispatcherUnhandledExceptionEventArgs.Exception);
    }

    private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
    {
      if (!unobservedTaskExceptionEventArgs.Observed)
        MainLog.Fatal("[Unhandled Exception] Task Scheduler On Unobserved Task Exception", unobservedTaskExceptionEventArgs.Exception);
    }
  }
}
