using System;

namespace UMP.Lib.Model
{
  public interface IMessageProgress<T> : IProgress<T>
  {
    public void Report(T percentage, string userMessage);
  }
}
