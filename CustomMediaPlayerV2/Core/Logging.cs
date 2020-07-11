﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace CMP2.Core
{
  public class Log
  {
    static Log()
    {
      // 로그 매니져 세팅
      Hierarchy hierarchy = (Hierarchy)LogManager.CreateRepository("MainLoggerRepository");

      string logSavePath = Path.Combine(Environment.CurrentDirectory, "Log");
      if (!Directory.Exists(logSavePath))
        Directory.CreateDirectory(logSavePath);

      Layout = new PatternLayout();
      Layout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
      Layout.ActivateOptions();

      // 파일 로그 패턴 설정
      RollingAppender = new RollingFileAppender
      {
        Name = "LogFileAppender",
        // 시스템이 기동되면 파일을 추가해서 할 것인가? 새로 작성할 것인가?
        AppendToFile = true,
        File = logSavePath + "\\Log.log",
        MaxSizeRollBackups = 5,
        MaximumFileSize = "300MB",
        StaticLogFileName = true,
        // 파일 단위는 날짜 단위인 것인가, 파일 사이즈인가?
        RollingStyle = RollingFileAppender.RollingMode.Size,
        // 로그 패턴
        Layout = Layout
      };
      RollingAppender.ActivateOptions();

      hierarchy.Root.AddAppender(RollingAppender);
      // 로그 출력 설정 All 이면 모든 설정이 되고 Info 이면 최하 레벨 Info 위가 설정됩니다.
      hierarchy.Root.Level = log4net.Core.Level.All;
      hierarchy.Configured = true;
    }

    public Log(string loggername = "MainLog") => Logger = LogManager.GetLogger("MainLoggerRepository", loggername);

    private static readonly RollingFileAppender RollingAppender;
    private static readonly PatternLayout Layout;
    private ILog Logger { get; }

    public void Fatal(object message) => Logger.Fatal(message);
    public void Error(object message) => Logger.Error(message);
    public void Warn(object message) => Logger.Warn(message);
    public void Info(object message) => Logger.Info(message);
    public void Debug(object message) => Logger.Debug(message);
    public void Fatal(object message, Exception exception) => Logger.Fatal(message, exception);
    public void Error(object message, Exception exception) => Logger.Fatal(message, exception);
    public void Warn(object message, Exception exception) => Logger.Fatal(message, exception);
    public void Info(object message, Exception exception) => Logger.Fatal(message, exception);
    public void Debug(object message, Exception exception) => Logger.Fatal(message, exception);
  }
}