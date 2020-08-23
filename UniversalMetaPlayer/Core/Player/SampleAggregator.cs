using System;
using System.Diagnostics;

using NAudio.Dsp;
using NAudio.Wave;

using UMP.Core.Global;

namespace UMP.Core.Player
{
  public partial class SampleAggregator : ISampleProvider
  {
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
    private readonly ISampleProvider source;

    public SampleAggregator(ISampleProvider source, int fftLength = 1024, bool initiallySilent = false)
    {
      if (!IsPowerOfTwo(fftLength))
        throw new ArgumentException("FFT Length must be a power of two");

      m = (int)Math.Log(fftLength, 2.0);
      this.fftLength = fftLength;
      fftBuffer = new Complex[fftLength];
      fftArgs = new FftEventArgs(fftBuffer);
      this.source = source;

      fadeState = initiallySilent ? FadeState.Silence : FadeState.FullVolume;
    }

    static bool IsPowerOfTwo(int x)
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

    public WaveFormat WaveFormat => source.WaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
      var samplesRead = source.Read(buffer, offset, count);

      if (GlobalProperty.Options.Getter<bool>(Enums.ValueName.IsUseFadeEffect))
      {
        lock (lockObject)
        {
          if (fadeState == FadeState.FadingIn)
          {
            FadeIn(buffer, offset, samplesRead);
          }
          else if (fadeState == FadeState.FadingOut)
          {
            FadeOut(buffer, offset, samplesRead);
          }
          else if (fadeState == FadeState.Silence)
          {
            ClearBuffer(buffer, offset, count);
          }
        }
      }

      // n = 0 left
      for (int n = 0; n < samplesRead; n += source.WaveFormat.Channels)
        Add(buffer[n + offset]);
      if (source.WaveFormat.Channels >= 2)
        for (int n = 1; n < samplesRead; n += source.WaveFormat.Channels)
          Add(buffer[n + offset], 1);
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
