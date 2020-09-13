using System;

namespace UMP.Lib.Media.Model
{
  public class MessageProgressChangedEventArgs<T>
  {
    public MessageProgressChangedEventArgs(T progressKind, int percentage, string userMessage = "")
    {
      this.ProgressKind = progressKind;
      this.Percentage = percentage;
      this.UserMessage = userMessage;
    }
    public T ProgressKind { get; }
    public int Percentage { get; }
    public string UserMessage { get; }
  }

  public delegate void MessageProgressChangedEventHandler<T>(object o, MessageProgressChangedEventArgs<T> e);

  [Flags]
  public enum InfoProgressKind
  {
    Info,
    InfoDownload,
    InfoLoad,
    InfoSave,
  }

  [Flags]
  public enum StreamProgressKind
  {
    Stream,
    StreamLoad,
    StreamDownload,
    StreamConvert,
  }
}
