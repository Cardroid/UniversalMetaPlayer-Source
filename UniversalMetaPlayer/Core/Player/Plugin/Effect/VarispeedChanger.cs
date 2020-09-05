using System;
using System.Collections.Generic;
using System.Text;

using NAudio.Wave;

using UMP.Utility.SoundTouch;

namespace UMP.Core.Player.Plugin.Effect
{
  public class VarispeedChanger : ISamplePlugin
  {
    private readonly ISampleProvider Source;
    private readonly SoundTouch soundTouch;
    private readonly float[] sourceReadBuffer;
    private readonly float[] soundTouchReadBuffer;
    private bool repositionRequested;

    public VarispeedChanger(ISampleProvider source, SoundTouchProfile soundTouchProfile,VarispeedChangerParameter parameter = null, int readDurationMilliseconds = 100)
    {
      soundTouch = new SoundTouch();
      // explore what the default values are before we change them:
      //Debug.WriteLine(String.Format("SoundTouch Version {0}", soundTouch.VersionString));
      //Debug.WriteLine("Use QuickSeek: {0}", soundTouch.GetUseQuickSeek());
      //Debug.WriteLine("Use AntiAliasing: {0}", soundTouch.GetUseAntiAliasing());

      IsEnabled = TempProperty.IsUseVarispeedChanger;

      SetSoundTouchProfile(soundTouchProfile);
      this.Source = source;
      soundTouch.SampleRate = (uint)WaveFormat.SampleRate;
      soundTouch.Channels = (uint)Source.WaveFormat.Channels;
      sourceReadBuffer = new float[WaveFormat.SampleRate * Source.WaveFormat.Channels * readDurationMilliseconds / 1000];
      soundTouchReadBuffer = new float[sourceReadBuffer.Length * 10]; // support down to 0.1 speed

      if (parameter != null)
      {
        Tempo = parameter.Tempo;
        Rate = parameter.Rate;
        Pitch = parameter.Pitch;
      }
    }

    ~VarispeedChanger()
    {
      soundTouch.Dispose();
    }

    public int Read(float[] buffer, int offset, int count)
    {
      if (IsActive)
      {
        if (repositionRequested)
        {
          soundTouch.Clear();
          repositionRequested = false;
        }

        int samplesRead = 0;
        bool reachedEndOfSource = false;
        while (samplesRead < count)
        {
          if (soundTouch.AvailableSampleCount == 0)
          {
            var readFromSource = Source.Read(sourceReadBuffer, 0, sourceReadBuffer.Length);
            if (readFromSource > 0)
              soundTouch.PutSamples(sourceReadBuffer, (uint)(readFromSource / Source.WaveFormat.Channels));
            else
            {
              reachedEndOfSource = true;
              // we've reached the end, tell SoundTouch we're done
              soundTouch.Flush();
            }
          }
          var desiredSampleFrames = (count - samplesRead) / Source.WaveFormat.Channels;

          var received = soundTouch.ReceiveSamples(soundTouchReadBuffer, (uint)desiredSampleFrames) * Source.WaveFormat.Channels;
          // use loop instead of Array.Copy due to WaveBuffer
          for (int n = 0; n < received; n++)
            buffer[offset + samplesRead++] = soundTouchReadBuffer[n];

          if (received == 0 && reachedEndOfSource)
            break;
        }
        return samplesRead;
      }
      else
        return Source.Read(buffer, offset, count);
    }

    public WaveFormat WaveFormat => Source.WaveFormat;

    public PluginName Name => PluginName.VarispeedChanger;

    public bool IsEnabled { get; set; }

    public bool IsActive => IsEnabled && (Tempo != 1.0f || Rate != 1.0f || Pitch != 1.0f);

    public float Tempo
    {
      get => _Tempo;
      private set
      {
        if (value > 0)
        {
          _Tempo = value;
          soundTouch.Tempo = _Tempo;
        }
      }
    }
    private float _Tempo = 1.0f;

    public float Rate
    {
      get => _Rate;
      private set
      {
        if (value > 0)
        {
          _Rate = value;
          soundTouch.Rate = _Rate;
        }
      }
    }
    private float _Rate = 1.0f;

    public float Pitch
    {
      get => _Pitch;
      private set
      {
        if (value > 0)
        {
          _Pitch = value;
          soundTouch.Pitch = _Pitch;
        }
      }
    }
    private float _Pitch = 1.0f;

    public void SetSoundTouchProfile(SoundTouchProfile soundTouchProfile)
    {
      soundTouch[SoundTouch.Setting.UseAntiAliasFilter] = soundTouchProfile.UseAntiAliasing ? 1 : 0;
      soundTouch[SoundTouch.Setting.UseQuickSeek] = soundTouchProfile.UseQuickSeek ? 1 : 0;
    }

    public void Reposition()
    {
      repositionRequested = true;
    }

    public void SetParameter(VarispeedChangerParameter parameter)
    {
      Tempo = parameter.Tempo;
      Rate = parameter.Rate;
      Pitch = parameter.Pitch;
      Reposition();
      TempProperty.VarispeedChangerParameter = parameter;
    }
  }

  public class VarispeedChangerParameter
  {
    public VarispeedChangerParameter(float tempo = 1.0f, float rate = 1.0f, float pitch = 1.0f)
    {
      this.Tempo = tempo;
      this.Rate = rate;
      this.Pitch = pitch;
    }

    public float Tempo { get; }
    public float Rate { get; }
    public float Pitch { get; }
  }
}
