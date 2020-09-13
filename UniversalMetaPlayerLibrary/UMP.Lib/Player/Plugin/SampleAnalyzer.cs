using System;
using System.Diagnostics;

using NAudio.Dsp;
using NAudio.Wave;

using UMP.Lib.Player.Model;
using UMP.Lib.Player.Plugin.Options;

namespace UMP.Lib.Player.Plugin
{
  public class SampleAnalyzer : ISamplePlugin
  {
    public string Name => "SampleAnalyzer";
    public bool IsEnabled { get; set; } = false;
    public bool IsActive => IsEnabled;
    public T GetOption<T>() where T : class => Option is T ? Option as T : null;
    private SampleAnalyzerOption Option { get; }

    // volume
    public event EventHandler<MaxSampleEventArgs> MaximumCalculated;
    private float maxValue;
    private float minValue;
    public int NotificationCount { get; set; }
    int count;

    // FFT
    public event EventHandler<FftEventArgs> FftCalculated;
    public bool PerformFFT { get; set; }
    private readonly Complex[] fftBuffer;
    private readonly FftEventArgs fftArgs;
    private int fftPos;
    private readonly int fftLength;
    private readonly int m;

    private readonly ISampleProvider Source;

    public SampleAnalyzer(ISampleProvider source, int fftLength = 1024)
    {
      if (!IsPowerOfTwo(fftLength))
        throw new ArgumentException("FFT 길이는 2의 거듭제곱이어야 합니다");

      m = (int)Math.Log(fftLength, 2.0);
      this.fftLength = fftLength;
      fftBuffer = new Complex[fftLength];
      fftArgs = new FftEventArgs(fftBuffer);
      this.Source = source;
    }

    private static bool IsPowerOfTwo(int x)
    {
      return (x & (x - 1)) == 0;
    }

    public void Reset()
    {
      count = 0;
      maxValue = minValue = 0;
    }

    private void Add(float value, int channel = 0)
    {
      if (PerformFFT && FftCalculated != null)
      {
        fftBuffer[fftPos].X = (float)(value * FastFourierTransform.HammingWindow(fftPos, fftLength));
        fftBuffer[fftPos].Y = 0;
        fftPos++;
        if (fftPos >= fftBuffer.Length)
        {
          fftPos = 0;
          // 1024 = 2^10
          FastFourierTransform.FFT(true, m, fftBuffer);
          FftCalculated(this, fftArgs);
        }
      }

      maxValue = Math.Max(maxValue, value);
      minValue = Math.Min(minValue, value);
      count++;
      if (count >= NotificationCount && NotificationCount > 0)
      {
        MaximumCalculated?.Invoke(this, new MaxSampleEventArgs(minValue, maxValue, channel));
        Reset();
      }
    }

    public WaveFormat WaveFormat => Source.WaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
      int samplesRead = Source.Read(buffer, offset, count);

      if (IsActive)
      {
        // n = 0 left
        for (int n = 0; n < samplesRead; n += Source.WaveFormat.Channels)
          Add(buffer[n + offset]);
        if (Source.WaveFormat.Channels >= 2)
          for (int n = 1; n < samplesRead; n += Source.WaveFormat.Channels)
            Add(buffer[n + offset], 1);
      }
      return samplesRead;
    }
  }

  public class MaxSampleEventArgs : EventArgs
  {
    [DebuggerStepThrough]
    public MaxSampleEventArgs(float minValue, float maxValue, int channel)
    {
      MaxSample = maxValue;
      MinSample = minValue;
      Channel = channel;
    }
    public float MaxSample { get; private set; }
    public float MinSample { get; private set; }
    public int Channel { get; private set; }
  }

  public class FftEventArgs : EventArgs
  {
    [DebuggerStepThrough]
    public FftEventArgs(Complex[] result)
    {
      Result = result;
    }
    public Complex[] Result { get; private set; }
  }
}
