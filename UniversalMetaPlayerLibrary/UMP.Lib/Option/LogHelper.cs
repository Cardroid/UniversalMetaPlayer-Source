using System;

using Serilog;

namespace UMP.Lib.Option
{
  public static class LogHelper
  {
    static LogHelper()
    {
      Logger = new LoggerConfiguration()
        .WriteTo.Console()
#if DEBUG
        .MinimumLevel.Verbose()
#else
        .MinimumLevel.Information()
#endif
        .CreateLogger();
    }

    public static ILogger Logger { get; set; }

    internal static ILogger WithType(this ILogger logger, Type type) =>
      logger.ForContext("Type", type);
  }
}
