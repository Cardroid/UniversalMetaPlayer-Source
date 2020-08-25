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

namespace UMP
{
  public partial class App : Application
  {
    public static Log MainLog { get; } = new Log("System");
    public App()
    {
#if DEBUG
      GlobalEventLogger.IsEnabled = true;
#else
      GlobalEventLogger.IsEnabled = false;
#endif
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
        MainLog.Fatal("\n\nCurrentDomainOnUnhandledException 예외 발생", (Exception)unhandledExceptionEventArgs.ExceptionObject);
    }

    private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
    {
      if(!dispatcherUnhandledExceptionEventArgs.Handled)
        MainLog.Fatal("\n\nDispatcherOnUnhandledException 예외 발생", dispatcherUnhandledExceptionEventArgs.Exception);
    }

    private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
    {
      if (!unobservedTaskExceptionEventArgs.Observed)
        MainLog.Fatal("\n\nTaskSchedulerOnUnobservedTaskException 예외 발생", unobservedTaskExceptionEventArgs.Exception);
    }
  }
}
