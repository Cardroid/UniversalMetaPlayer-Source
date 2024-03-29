﻿using System;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

using UMP.Core.Global;

namespace UMP.Core
{
  public class Log
  {
    static Log()
    {
      // 로그 매니져 세팅
      Hierarchy = (Hierarchy)LogManager.CreateRepository("MainLoggerRepository");

      string logSavePath = Path.Combine(Environment.CurrentDirectory, "Log");
      if (!Directory.Exists(logSavePath))
        Directory.CreateDirectory(logSavePath);

      //Layout = new PatternLayout("%date [%thread] %-5level %logger - %message%newline");
      Layout = new PatternLayout("%date %-5level %logger - %message%newline");
      Layout.ActivateOptions();

      // 파일 로그 패턴 설정
      RollingAppender = new RollingFileAppender
      {
        Name = "LogFileAppender",
        // 시스템이 기동되면 파일을 추가해서 할 것인가? 새로 작성할 것인가?
        AppendToFile = true,
        File = logSavePath + "\\UMP_Log_",
        DatePattern = "yyyy-MM-dd'.log'",
        MaxSizeRollBackups = 10,
        MaximumFileSize = "100MB",
        StaticLogFileName = false,
        // 파일 단위는 날짜 단위인 것인가, 파일 사이즈인가?
        RollingStyle = RollingFileAppender.RollingMode.Composite,
        // 로그 패턴
        Layout = Layout
      };
      RollingAppender.ActivateOptions();

      Hierarchy.Root.AddAppender(RollingAppender);
      // 로그 출력 설정 All 이면 모든 설정이 되고 Info 이면 최하 레벨 Info 위가 설정됩니다.
#if DEBUG
      Hierarchy.Root.Level = Level.All;
#else
      Hierarchy.Root.Level = Level.Info;
#endif
      Hierarchy.Configured = true;
    }

    public Log(Type type) => Logger = LogManager.GetLogger("MainLoggerRepository", type);
    public Log(string loggername) => Logger = LogManager.GetLogger("MainLoggerRepository", loggername);

    public static Hierarchy Hierarchy { get; }
    public static RollingFileAppender RollingAppender { get; }
    public static PatternLayout Layout { get; }
    private ILog Logger { get; }

    public void Fatal(string message, string privateData = "") =>
      Logger.Fatal(GlobalProperty.Options.Getter<bool>(Enums.ValueName.PrivateLogging)
        && !string.IsNullOrWhiteSpace(privateData) ? $"{message}\n===Private===\n{privateData}" : message);
    public void Error(string message, string privateData = "") =>
      Logger.Error(GlobalProperty.Options.Getter<bool>(Enums.ValueName.PrivateLogging)
        && !string.IsNullOrWhiteSpace(privateData) ? $"{message}\n===Private===\n{privateData}" : message);
    public void Warn(string message, string privateData = "") =>
      Logger.Warn(GlobalProperty.Options.Getter<bool>(Enums.ValueName.PrivateLogging)
         && !string.IsNullOrWhiteSpace(privateData) ? $"{message}\n===Private===\n{privateData}" : message);
    public void Info(string message, string privateData = "") => 
      Logger.Info(GlobalProperty.Options.Getter<bool>(Enums.ValueName.PrivateLogging)
        && !string.IsNullOrWhiteSpace(privateData) ? $"{message}\n===Private===\n{privateData}" : message);
    public void Debug(string message, string privateData = "") =>
      Logger.Debug(GlobalProperty.Options.Getter<bool>(Enums.ValueName.PrivateLogging)
        && !string.IsNullOrWhiteSpace(privateData) ? $"{message}\n===Private===\n{privateData}" : message);
    public void Fatal(string message, Exception exception, string privateData = "") =>
      Logger.Fatal(GlobalProperty.Options.Getter<bool>(Enums.ValueName.PrivateLogging)
        && !string.IsNullOrWhiteSpace(privateData) ? $"{message}\n===Private===\n{privateData}" : message, exception);
    public void Error(string message, Exception exception, string privateData = "") =>
      Logger.Error(GlobalProperty.Options.Getter<bool>(Enums.ValueName.PrivateLogging)
         && !string.IsNullOrWhiteSpace(privateData) ? $"{message}\n===Private===\n{privateData}" : message, exception);
    public void Warn(string message, Exception exception, string privateData = "") =>
      Logger.Warn(GlobalProperty.Options.Getter<bool>(Enums.ValueName.PrivateLogging)
        && !string.IsNullOrWhiteSpace(privateData) ? $"{message}\n===Private===\n{privateData}" : message, exception);
    public void Info(string message, Exception exception, string privateData = "") =>
      Logger.Info(GlobalProperty.Options.Getter<bool>(Enums.ValueName.PrivateLogging)
        && !string.IsNullOrWhiteSpace(privateData) ? $"{message}\n===Private===\n{privateData}" : message, exception);
    public void Debug(string message, Exception exception, string privateData = "") =>
      Logger.Debug(GlobalProperty.Options.Getter<bool>(Enums.ValueName.PrivateLogging)
        && !string.IsNullOrWhiteSpace(privateData) ? $"{message}\n===Private===\n{privateData}" : message, exception);
  }
}