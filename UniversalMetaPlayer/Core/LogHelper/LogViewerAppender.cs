﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using log4net.Appender;
using log4net.Core;
using log4net.Layout;

using UMP.Controller.WindowHelper;

namespace UMP.Core.LogHelper
{
  public class LogViewerAppender : IAppender, INotifyPropertyChanged
  {
    public LogViewerAppender(PatternLayout patternLayout, LogTextBox logTextBox)
    {
      Name = "LogViewerAppender";
      Layout = patternLayout;
      this.LogTextBox = logTextBox;
    }

    public string Name { get; set; }
    public bool IsEnable
    {
      get => _IsEnable;
      set
      {
        if(_IsEnable != value)
        {
          _IsEnable = value;
          if (_IsEnable)
          {
            if (LogViewerWindow == null)
            {
              LogViewerWindow = new UserWindow(LogTextBox, "UMP - Log");
              LogViewerWindow.Closed += (_, e) => { IsEnable = false; };
            }
            LogViewerWindow.Show();
          }
          else
          {
            if (LogViewerWindow != null)
            {
              LogViewerWindow.Close();
              LogViewerWindow = null;
            }
          }
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnable"));
        }
      }
    }
    private bool _IsEnable;
    private LogTextBox LogTextBox { get; }
    private UserWindow LogViewerWindow
    {
      get => _LogViewerWindow != null && _LogViewerWindow.IsAlive ? _LogViewerWindow.Target as UserWindow : null;
      set
      {
        if (value == null)
          _LogViewerWindow = null;
        else
          _LogViewerWindow = new WeakReference(value);
      }
    }
    private WeakReference _LogViewerWindow;

    public event PropertyChangedEventHandler PropertyChanged;

    private PatternLayout Layout { get; }

    public void Close()
    {
    }

    public void DoAppend(LoggingEvent loggingEvent)
    {
      if (IsEnable)
      {
        Paragraph paragraph = new Paragraph();

        Brush foreground;
        Brush background;

        switch (loggingEvent.Level.Name.ToUpper())
        {
          case "FATAL":
            foreground = Brushes.Red;
            background = Brushes.White;
            break;
          case "ERROR":
            foreground = Brushes.Red;
            background = Brushes.Transparent;
            break;
          case "WARN":
            foreground = Brushes.Yellow;
            background = Brushes.Transparent;
            break;
          case "INFO":
            foreground = Brushes.White;
            background = Brushes.Transparent;
            break;
          case "DEBUG":
            foreground = Brushes.LightGreen;
            background = Brushes.Transparent;
            break;
          default:
            foreground = Brushes.White;
            background = Brushes.Transparent;
            break;
        }

        paragraph.Inlines.Add(new Run(Layout.Format(loggingEvent)) { Foreground = foreground, Background = background });
        LogTextBox.TextBox.Document.Blocks.Add(paragraph);
        LogTextBox.ScrollViewer.ScrollToEnd();
      }
    }
  }
}