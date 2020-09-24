using System;

namespace UMP.Lib.Media.Model
{
    public class MessageProgressChangedEventArgs<T>
    {
        public MessageProgressChangedEventArgs(T progressKind, double percentage, string userMessage = "")
        {
            this.ProgressKind = progressKind;
            this.Percentage = percentage;
            this.UserMessage = userMessage;
        }
        public T ProgressKind { get; }
        public double Percentage { get; }
        public string UserMessage { get; }
    }

    public delegate void MessageProgressChangedEventHandler<T>(object o, MessageProgressChangedEventArgs<T> e);

    [Flags]
    public enum MediaProgressKind
    {
        Info = InfoProgressKind.Info,
        InfoDownload = InfoProgressKind.InfoDownload,
        InfoLoad = InfoProgressKind.InfoLoad,
        InfoSave = InfoProgressKind.InfoSave,
        Stream = StreamProgressKind.Stream,
        StreamLoad = StreamProgressKind.StreamLoad,
        StreamDownload = StreamProgressKind.StreamDownload,
        StreamConvert = StreamProgressKind.StreamConvert,
    }

    [Flags]
    public enum InfoProgressKind
    {
        Info = 0,
        InfoDownload = 1,
        InfoLoad = 2,
        InfoSave = 4,
    }

    [Flags]
    public enum StreamProgressKind
    {
        Stream = 8,
        StreamLoad = 16,
        StreamDownload = 32,
        StreamConvert = 64,
    }
}
