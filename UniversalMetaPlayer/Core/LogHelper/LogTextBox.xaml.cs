﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UMP.Core.Global;

namespace UMP.Core.LogHelper
{
  public partial class LogTextBox : UserControl
  {
    public LogTextBox()
    {
      InitializeComponent();
      MessageQueue = new ConcurrentQueue<LogMessageStruct>();
      WatcherThread = new Thread(WatchQueue);
      WatcherThread.Start();
      QueueItemChanged += () =>
      {
        if (MessageQueue.TryDequeue(out LogMessageStruct result))
        {
          this.Dispatcher.Invoke(() =>
          {
            this.TextBox.Document.Blocks.Add(new Paragraph()
            {
              Inlines =
              {
                  new Run(result.Message)
                  {
                    Foreground = new SolidColorBrush(result.Foreground),
                    Background = new SolidColorBrush(result.Background)
                  }
              }
            });
            this.ScrollViewer.ScrollToEnd();
          });
        }
      };
    }

    private ConcurrentQueue<LogMessageStruct> MessageQueue { get; }

    private Thread WatcherThread { get; }
    private event UMP_VoidEventHandler QueueItemChanged;

    private void WatchQueue()
    {
      while (true)
      {
        if (!MessageQueue.IsEmpty)
          QueueItemChanged?.Invoke();
        Thread.Sleep(1);

      }
    }

    public void AddMessage(LogMessageStruct run) => MessageQueue.Enqueue(run);
  }

  public struct LogMessageStruct
  {
    public LogMessageStruct(string message, Color foreground,Color background)
    {
      this.Message = message;
      this.Foreground = foreground;
      this.Background = background;
    }
    public string Message { get; }
    public Color Foreground { get; }
    public Color Background { get; }
  }
}
