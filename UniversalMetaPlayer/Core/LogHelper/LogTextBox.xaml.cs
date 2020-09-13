using System;
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
using UMP.Core.Model;

namespace UMP.Core.LogHelper
{
  public partial class LogTextBox : UserControl
  {
    public LogTextBox()
    {
      InitializeComponent();
      MessageQueue = new ConcurrentQueue<LogMessageStruct>();
      WatcherThread = new ThreadHelper(WatchQueue);
      WatcherThread.Start();
      QueueItemChanged += LogTextBox_QueueItemChanged;
      this.Unloaded += LogTextBox_Unloaded;
    }

    private void LogTextBox_Unloaded(object sender, RoutedEventArgs e) => WatcherThread.Dispose();

    private void LogTextBox_QueueItemChanged()
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
    }

    private ConcurrentQueue<LogMessageStruct> MessageQueue { get; }

    private ThreadHelper WatcherThread { get; }
    private event UMP_VoidEventHandler QueueItemChanged;

    private void WatchQueue()
    {
      if (!MessageQueue.IsEmpty)
        QueueItemChanged?.Invoke();
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
