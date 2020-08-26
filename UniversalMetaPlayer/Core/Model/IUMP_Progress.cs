using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.Core.Model
{
  public delegate void ProgressChangedEventHandler(double percentage, string userMessage);

  public interface IUMP_Progress<T> : IProgress<T>
  {
    public void Report(T percentage, string userMessage);
  }
}
