using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.Core.Player
{
  public partial class SampleAggregator
  {
    enum FadeState
    {
      Silence,
      FadingIn,
      FullVolume,
      FadingOut,
    }

    private readonly object lockObject = new object();
    private int fadeSamplePosition;
    private int fadeSampleCount;
    private FadeState fadeState;

    /// <summary>
    /// 페이드 인 시작 요청 (다음 Read 호출 시 시작됨)
    /// </summary>
    /// <param name="fadeDurationInMilliseconds">Duration of fade in milliseconds</param>
    public void BeginFadeIn(int fadeDurationInMilliseconds)
    {
      lock (lockObject)
      {
        fadeSamplePosition = 0;
        fadeSampleCount = fadeDurationInMilliseconds * source.WaveFormat.SampleRate / 1000;
        fadeState = FadeState.FadingIn;
      }
    }

    /// <summary>
    /// 페이드 아웃 시작 요청 (다음 Read 호출 시 시작됨)
    /// </summary>
    /// <param name="fadeDurationInMilliseconds">Duration of fade in milliseconds</param>
    public void BeginFadeOut(int fadeDurationInMilliseconds)
    {
      lock (lockObject)
      {
        fadeSamplePosition = 0;
        fadeSampleCount = fadeDurationInMilliseconds * source.WaveFormat.SampleRate / 1000;
        fadeState = FadeState.FadingOut;
      }
    }

    private static void ClearBuffer(float[] buffer, int offset, int count)
    {
      for (int n = 0; n < count; n++)
      {
        buffer[n + offset] = 0;
      }
    }

    private void FadeOut(float[] buffer, int offset, int sourceSamplesRead)
    {
      int sample = 0;
      while (sample < sourceSamplesRead)
      {
        float multiplier = 1.0f - (fadeSamplePosition / (float)fadeSampleCount);
        for (int ch = 0; ch < source.WaveFormat.Channels; ch++)
        {
          buffer[offset + sample++] *= multiplier;
        }
        fadeSamplePosition++;
        if (fadeSamplePosition > fadeSampleCount)
        {
          fadeState = FadeState.Silence;
          // clear out the end
          ClearBuffer(buffer, sample + offset, sourceSamplesRead - sample);
          break;
        }
      }
    }

    private void FadeIn(float[] buffer, int offset, int sourceSamplesRead)
    {
      int sample = 0;
      while (sample < sourceSamplesRead)
      {
        float multiplier = (fadeSamplePosition / (float)fadeSampleCount);
        for (int ch = 0; ch < source.WaveFormat.Channels; ch++)
        {
          buffer[offset + sample++] *= multiplier;
        }
        fadeSamplePosition++;
        if (fadeSamplePosition > fadeSampleCount)
        {
          fadeState = FadeState.FullVolume;
          // no need to multiply any more
          break;
        }
      }
    }
  }
}
