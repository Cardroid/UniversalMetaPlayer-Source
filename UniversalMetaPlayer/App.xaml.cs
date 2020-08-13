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

      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
      DispatcherUnhandledException += App_DispatcherUnhandledException;
      AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
    }

    public static Log MainLog { get; } = new Log("System");

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      MainLog.Error($"[CurrentDomain Exception] Terminating : [{e.IsTerminating}]", e.ExceptionObject as Exception);
    }

    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
      MainLog.Error($"[Dispatcher Exception] Handled : [{e.Handled}] Dispatcher Thread : [{e.Dispatcher.Thread}]", e.Exception);
    }

    private void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
      MainLog.Error($"[CurrentDomain FirstChanceException]", e.Exception);
    }
  }
}
