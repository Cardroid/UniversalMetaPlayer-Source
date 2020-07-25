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
    public static Log MainLog { get; } = new Log("System");
  }
}
