using System;
using System.Collections.Generic;
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
  public class LogViewerAppender : IAppender
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
        _IsEnable = value;
        if (_IsEnable)
        {
          if (LogViewerWindow == null)
          {
            LogViewerWindow = new UserWindow(LogTextBox, "UMP - Log");
            LogViewerWindow.Closed += (_, e) =>
            {
              _IsEnable = false;
              LogViewerWindow = null;
            };
          }
          LogViewerWindow.Show();
        }
        else
        {
          if(LogViewerWindow!= null)
          {
            LogViewerWindow.Close();
            LogViewerWindow = null;
          }
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

    private PatternLayout Layout { get; }

    public void Close()
    {
    }

    public void DoAppend(LoggingEvent loggingEvent)
    {
      if (IsEnable)
      {
        //Paragraph paragraph = LogTextBox.TextBox.Document.ContentEnd.Paragraph;
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
            foreground = Brushes.Green;
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
