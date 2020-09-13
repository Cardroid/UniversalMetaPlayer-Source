using NAudio.Wave;

using UMP.Lib.Player.Model;
using UMP.Lib.Player.Plugin.Options;

namespace UMP.Lib.Player.Plugin
{
  public class FadeOutEffect : ISamplePlugin
  {
    public string Name { get; } = "FadeOutEffect";
    public bool IsEnabled { get; set; } = false;
    public bool IsActive
    {
      get => IsEnabled && _IsActive;
      private set
      {
        _IsActive = value;
      }
    }
    private bool _IsActive = false;
    public T GetOption<T>() where T : class => Option is T ? Option as T : null;
    private FadeOutEffectOption Option { get; }

    public FadeOutEffect(ISampleProvider source, bool initiallySilent = false)
    {
      this.Source = source;
      
      fadeState = initiallySilent ? FadeState.Silence : FadeState.FullVolume;
    }

    enum FadeState
    {
      Silence,
      FadingIn,
      FullVolume,
      FadingOut,
    }

    private ISampleProvider Source { get; }
    private readonly object lockObject = new object();
    private int fadeSamplePosition;
    private int fadeSampleCount;
    private FadeState fadeState;

    public WaveFormat WaveFormat => Source.WaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
      int samplesRead = Source.Read(buffer, offset, count);

      if (IsActive)
      {
        lock (lockObject)
        {
          switch (fadeState)
          {
            case FadeState.Silence:
              ClearBuffer(buffer, offset, count);
              break;
            case FadeState.FadingIn:
              FadeIn(buffer, offset, samplesRead);
              break;
            case FadeState.FadingOut:
              FadeOut(buffer, offset, samplesRead);
              break;
          }
        }
      }
      return samplesRead;
    }

    /// <summary>
    /// 페이드 인 시작 요청 (다음 Read 호출 시 시작됨)
    /// </summary>
    /// <param name="fadeDurationInMilliseconds">Duration of fade in milliseconds</param>
    public void BeginFadeIn(int fadeDurationInMilliseconds)
    {
      lock (lockObject)
      {
        fadeSamplePosition = 0;
        fadeSampleCount = fadeDurationInMilliseconds * Source.WaveFormat.SampleRate / 1000;
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
        fadeSampleCount = fadeDurationInMilliseconds * Source.WaveFormat.SampleRate / 1000;
        fadeState = FadeState.FadingOut;
      }
    }

    private static void ClearBuffer(float[] buffer, int offset, int count)
    {
      for (int n = 0; n < count; n++)
        buffer[n + offset] = 0;
    }

    private void FadeOut(float[] buffer, int offset, int sourceSamplesRead)
    {
      int sample = 0;
      while (sample < sourceSamplesRead)
      {
        float multiplier = 1.0f - (fadeSamplePosition / (float)fadeSampleCount);
        for (int ch = 0; ch < Source.WaveFormat.Channels; ch++)
          buffer[offset + sample++] *= multiplier;

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
        for (int ch = 0; ch < Source.WaveFormat.Channels; ch++)
          buffer[offset + sample++] *= multiplier;

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
