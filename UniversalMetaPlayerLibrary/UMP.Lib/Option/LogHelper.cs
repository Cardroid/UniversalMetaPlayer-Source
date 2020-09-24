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

        public static ILogger Logger
        {
            get => _Logger;
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Logger is null");
                else
                    _Logger = value;
            }
        }
        private static ILogger _Logger;

        internal static ILogger WithType(this ILogger logger, Type type) =>
          logger.ForContext("Type", type);
    }
}
