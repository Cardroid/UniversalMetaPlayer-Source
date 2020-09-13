using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

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
      IsEnable = false;
      LogTextBox.Unloaded += (_, e) => IsEnable = false;
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
            WindowManager.Controller.Open(Model.Control.WindowKind.Log);
          else
            WindowManager.Controller.Close(Model.Control.WindowKind.Log);

          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnable"));
        }
      }
    }
    private bool _IsEnable = false;

    public LogTextBox LogTextBox { get; }

    public event PropertyChangedEventHandler PropertyChanged;

    private PatternLayout Layout { get; }

    public void Close()
    {
      IsEnable = false;
    }

    public void DoAppend(LoggingEvent loggingEvent)
    {
      if (IsEnable)
      {
        Color foreground;
        Color background;

        switch (loggingEvent.Level.Name.ToUpper())
        {
          case "FATAL":
            foreground = Colors.Red;
            background = Colors.White;
            break;
          case "ERROR":
            foreground = Colors.Red;
            background = Colors.Transparent;
            break;
          case "WARN":
            foreground = Colors.Yellow;
            background = Colors.Transparent;
            break;
          case "INFO":
            foreground = Colors.White;
            background = Colors.Transparent;
            break;
          case "DEBUG":
            foreground = Colors.LightGreen;
            background = Colors.Transparent;
            break;
          default:
            foreground = Colors.White;
            background = Colors.Transparent;
            break;
        }

        LogTextBox.AddMessage(new LogMessageStruct(Layout.Format(loggingEvent), foreground, background));
      }
    }
  }
}
