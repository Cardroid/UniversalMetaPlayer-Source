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
      MainLog.Info("############### Start application ###############", $"Start Path : [{AppDomain.CurrentDomain.BaseDirectory}]\nTask Path : [{Environment.CurrentDirectory}]");
    }
    public static Log MainLog { get; } = new Log("System");
  }
}
