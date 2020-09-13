using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace UMP.Core.Model
{
  public class ThreadHelper : IDisposable
  {
    private bool disposedValue;

    public ThreadHelper(ThreadStart threadStart, int threadSleepInterval = 10)
    {
      IsRunning = false;

      this.ThreadSleepInterval = threadSleepInterval;
      this.ThreadStart = threadStart;
      this.Thread = new Thread(ThreadMethod);
    }

    private Thread Thread { get; }
    private ThreadStart ThreadStart { get; }
    public ThreadState ThreadState => Thread.ThreadState;
    public bool IsRunning { get; private set; }
    public int ThreadSleepInterval { get; }

    private void ThreadMethod()
    {
      while (IsRunning)
      {
        ThreadStart.Invoke();
        Thread.Sleep(ThreadSleepInterval);
      }
    }

    public void Start()
    {
      IsRunning = true;
      Thread.Start();
    }
    
    public void Stop()
    {
      IsRunning = false;
      Thread.Join();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
        }

        if (IsRunning)
        {
          Stop();
          if (Thread.IsAlive)
            try
            {
              Thread.Abort();
              Thread.Join();
              IsRunning = false;
            }
            catch { }
        }

        disposedValue = true;
      }
    }

    ~ThreadHelper()
    {
      Dispose(disposing: false);
    }

    public void Dispose()
    {
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
  }
}
