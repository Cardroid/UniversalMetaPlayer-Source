﻿//////////////////////////////////////////////////////////////////////////////
///
/// C# wrapper to access SoundTouch APIs from an external SoundTouch.dll library
///
/// Author        : Copyright (c) Olli Parviainen
/// Author e-mail : oparviai 'at' iki.fi
/// SoundTouch WWW: http://www.surina.net/soundtouch
///
/// The C# wrapper improved by Mario Di Vece
///
////////////////////////////////////////////////////////////////////////////////
//
// License :
//
//  SoundTouch audio processing library
//  Copyright (c) Olli Parviainen
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General internal
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//  Lesser General internal License for more details.
//
//  You should have received a copy of the GNU Lesser General internal
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;

namespace UMP.Lib.Utility.SoundTouch
{
  internal sealed class SoundTouch : IDisposable
  {
    #region Internal Members
#if BIT64
    internal const string SoundTouchLibrary = "SoundTouch_x64.dll";
#endif

#if BIT32 || BITANY
    internal const string SoundTouchLibrary = "SoundTouch.dll";
#endif
    #endregion

    #region Private 멤버

    private readonly object SyncRoot = new object();
    private bool IsDisposed = false;
    private IntPtr handle;

    #endregion

    #region 생성자

    /// <summary>
    /// <see cref="SoundTouch"/> 클래스의 새 인스턴스 초기화
    /// </summary>
    internal SoundTouch()
    {
      handle = NativeMethods.CreateInstance();
    }

    /// <summary>
    /// <see cref="SoundTouch"/> 클래스 인스턴스의 리소스 해제
    /// </summary>
    ~SoundTouch()
    {
      // 이 코드를 변경하지 마십시오. 위의 삭제(bool delete)에 정리 코드를 입력하십시오.
      Dispose(false);
    }

    /// <summary>
    /// SoundTouch.h에 정의된 설정
    /// </summary>
    internal enum Setting
    {
      /// <summary>
      /// 피치 트랜스포저에서 안티앨리어스 필터 활성화/비활성화 (0 = 비활성화)
      /// </summary>
      UseAntiAliasFilter = 0,

      /// <summary>
      /// 피치 트랜스포저 안티앨리어스 필터 길이 (8 ..128 탭, 기본값 = 32)
      /// </summary>
      AntiAliasFilterLength = 1,

      /// <summary>
      /// 템포 체인저 루틴에서 빠른 검색 알고리즘 활성화/비활성화<br/>
      /// (빠른 검색을 사용하면 CPU 활용도는 낮지만 음질은 약간 저하됨)
      /// </summary>
      UseQuickSeek = 2,

      /// <summary>
      /// Time-stretch algorithm single processing sequence length in milliseconds. This determines 
      /// to how long sequences the original sound is chopped in the time-stretch algorithm. 
      /// See "STTypes.h" or README for more information.
      /// </summary>
      SequenceMilliseconds = 3,

      /// <summary>
      /// Time-stretch algorithm seeking window length in milliseconds for algorithm that finds the 
      /// best possible overlapping location. This determines from how wide window the algorithm 
      /// may look for an optimal joining location when mixing the sound sequences back together. 
      /// See "STTypes.h" or README for more information.
      /// </summary>
      SeekWindowMilliseconds = 4,

      /// <summary>
      /// Time-stretch algorithm overlap length in milliseconds. When the chopped sound sequences 
      /// are mixed back together, to form a continuous sound stream, this parameter defines over 
      /// how long period the two consecutive sequences are let to overlap each other. 
      /// See "STTypes.h" or README for more information.
      /// </summary>
      OverlapMilliseconds = 5,

      /// <summary>
      /// Call "getSetting" with this ID to query processing sequence size in samples. 
      /// This value gives approximate value of how many input samples you'll need to 
      /// feed into SoundTouch after initial buffering to get out a new batch of
      /// output samples. 
      ///
      /// This value does not include initial buffering at beginning of a new processing 
      /// stream, use SETTING_INITIAL_LATENCY to get the initial buffering size.
      ///
      /// Notices: 
      /// - This is read-only parameter, i.e. setSetting ignores this parameter
      /// - This parameter value is not constant but change depending on 
      ///   tempo/pitch/rate/samplerate settings.
      /// </summary>
      NominalInputSequence = 6,

      /// <summary>
      /// Call "getSetting" with this ID to query nominal average processing output 
      /// size in samples. This value tells approcimate value how many output samples 
      /// SoundTouch outputs once it does DSP processing run for a batch of input samples.
      ///
      /// Notices: 
      /// - This is read-only parameter, i.e. setSetting ignores this parameter
      /// - This parameter value is not constant but change depending on 
      ///   tempo/pitch/rate/samplerate settings.
      /// </summary>
      NominalOutputSequence = 7,

      /// <summary>
      /// Call "getSetting" with this ID to query initial processing latency, i.e.
      /// approx. how many samples you'll need to enter to SoundTouch pipeline before 
      /// you can expect to get first batch of ready output samples out. 
      ///
      /// After the first output batch, you can then expect to get approx. 
      /// SETTING_NOMINAL_OUTPUT_SEQUENCE ready samples out for every
      /// SETTING_NOMINAL_INPUT_SEQUENCE samples that you enter into SoundTouch.
      ///
      /// Example:
      ///     processing with parameter -tempo=5
      ///     => initial latency = 5509 samples
      ///        input sequence  = 4167 samples
      ///        output sequence = 3969 samples
      ///
      /// Accordingly, you can expect to feed in approx. 5509 samples at beginning of 
      /// the stream, and then you'll get out the first 3969 samples. After that, for 
      /// every approx. 4167 samples that you'll put in, you'll receive again approx. 
      /// 3969 samples out.
      ///
      /// This also means that average latency during stream processing is 
      /// INITIAL_LATENCY-OUTPUT_SEQUENCE/2, in the above example case 5509-3969/2 
      /// = 3524 samples
      /// 
      /// Notices: 
      /// - This is read-only parameter, i.e. setSetting ignores this parameter
      /// - This parameter value is not constant but change depending on 
      ///   tempo/pitch/rate/samplerate settings.
      /// </summary>
      InitialLatency = 8,
    }

    #endregion

    #region Properties

    /// <summary>
    /// Get SoundTouch version string
    /// </summary>
    internal static string Version =>
        // convert "char *" data to c# string
        Marshal.PtrToStringAnsi(NativeMethods.GetVersionString());

    /// <summary>
    /// Gets a value indicating whether the SoundTouch Library (dll) is available
    /// </summary>
    internal static bool IsAvailable
    {
      get
      {
        try
        {
          var versionId = NativeMethods.GetVersionId();
          return versionId != 0;
        }
        catch
        {
          return false;
        }
      }
    }

    /// <summary>
    /// Returns number of processed samples currently available in SoundTouch for immediate output.
    /// </summary>
    internal uint AvailableSampleCount
    {
      get { lock (SyncRoot) { return NativeMethods.NumSamples(handle); } }
    }

    /// <summary>
    /// Returns number of samples currently unprocessed in SoundTouch internal buffer
    /// </summary>
    /// <returns>Number of sample frames</returns>
    internal uint UnprocessedSampleCount
    {
      get { lock (SyncRoot) { return NativeMethods.NumUnprocessedSamples(handle); } }
    }

    /// <summary>
    /// Check if there aren't any samples available for outputting.
    /// </summary>
    /// <returns>nonzero if there aren't any samples available for outputting</returns>
    internal int IsEmpty
    {
      get { lock (SyncRoot) { return NativeMethods.IsEmpty(handle); } }
    }

    /// <summary>
    /// Sets the number of channels
    /// 
    /// Value: 1 = mono, 2 = stereo, n = multichannel
    /// </summary>
    internal uint Channels
    {
      set { lock (SyncRoot) { NativeMethods.SetChannels(handle, value); } }
    }

    /// <summary>
    /// Sets sample rate.
    /// Value: Sample rate, e.g. 44100
    /// </summary>
    internal uint SampleRate
    {
      set { lock (SyncRoot) { NativeMethods.SetSampleRate(handle, value); } }
    }

    /// <summary>
    /// Sets new tempo control value. 
    /// 
    /// Value: Tempo setting. Normal tempo = 1.0, smaller values
    /// represent slower tempo, larger faster tempo.
    /// </summary>
    internal float Tempo
    {
      set { lock (SyncRoot) { NativeMethods.SetTempo(handle, value); } }
    }

    /// <summary>
    /// Sets new tempo control value as a difference in percents compared
    /// to the original tempo (-50 .. +100 %);
    /// </summary>
    internal float TempoChange
    {
      set { lock (SyncRoot) { NativeMethods.SetTempoChange(handle, value); } }
    }

    /// <summary>
    /// Sets new rate control value. 
    /// Rate setting. Normal rate = 1.0, smaller values
    /// represent slower rate, larger faster rate.
    /// </summary>
    internal float Rate
    {
      set { lock (SyncRoot) { NativeMethods.SetRate(handle, value); } }
    }

    /// <summary>
    /// Sets new rate control value as a difference in percents compared
    /// to the original rate (-50 .. +100 %);
    /// 
    /// Value: Rate setting is in %
    /// </summary>
    internal float RateChange
    {
      set { lock (SyncRoot) { NativeMethods.SetRateChange(handle, value); } }
    }

    /// <summary>
    /// Sets new pitch control value. 
    /// 
    /// Value: Pitch setting. Original pitch = 1.0, smaller values
    /// represent lower pitches, larger values higher pitch.
    /// </summary>
    internal float Pitch
    {
      set { lock (SyncRoot) { NativeMethods.SetPitch(handle, value); } }
    }

    /// <summary>
    /// Sets pitch change in octaves compared to the original pitch  
    /// (-1.00 .. +1.00 for +- one octave);
    /// 
    /// Value: Pitch setting in octaves
    /// </summary>
    internal float PitchOctaves
    {
      set { lock (SyncRoot) { NativeMethods.SetPitchOctaves(handle, value); } }
    }

    /// <summary>
    /// Sets pitch change in semi-tones compared to the original pitch
    /// (-12 .. +12 for +- one octave);
    /// 
    /// Value: Pitch setting in semitones
    /// </summary>
    internal float PitchSemiTones
    {
      set { lock (SyncRoot) { NativeMethods.SetPitchSemiTones(handle, value); } }
    }

    /// <summary>
    /// Changes or gets a setting controlling the processing system behaviour. See the
    /// 'SETTING_...' defines for available setting ID's.
    /// </summary>
    /// <value>
    /// The <see cref="System.Int32"/>.
    /// </value>
    /// <param name="settingId">The setting identifier.</param>
    /// <returns>The value of the setting</returns>
    internal int this[Setting settingId]
    {
      get { lock (SyncRoot) { return NativeMethods.GetSetting(handle, (int)settingId); } }
      set { lock (SyncRoot) { NativeMethods.SetSetting(handle, (int)settingId, value); } }
    }

    #endregion

    #region Sample Stream Methods

    /// <summary>
    /// Flushes the last samples from the processing pipeline to the output.
    /// Clears also the internal processing buffers.
    /// 
    /// Note: This function is meant for extracting the last samples of a sound
    /// stream. This function may introduce additional blank samples in the end
    /// of the sound stream, and thus it's not recommended to call this function
    /// in the middle of a sound stream.
    /// </summary>
    internal void Flush()
    {
      lock (SyncRoot) { NativeMethods.Flush(handle); }
    }

    /// <summary>
    /// Clears all the samples in the object's output and internal processing
    /// buffers.
    /// </summary>
    internal void Clear()
    {
      lock (SyncRoot) { NativeMethods.Clear(handle); }
    }

    /// <summary>
    /// Adds 'numSamples' pcs of samples from the 'samples' memory position into
    /// the input of the object. Notice that sample rate _has_to_ be set before
    /// calling this function, otherwise throws a runtime_error exception.
    /// </summary>
    /// <param name="samples">Sample buffer to input</param>
    /// <param name="numSamples">Number of sample frames in buffer. Notice
    /// that in case of multi-channel sound a single sample frame contains 
    /// data for all channels</param>
    internal void PutSamples(float[] samples, uint numSamples)
    {
      lock (SyncRoot) { NativeMethods.PutSamples(handle, samples, numSamples); }
    }

    /// <summary>
    /// int16 version of putSamples(): This accept int16 (short) sample data
    /// and internally converts it to float format before processing
    /// </summary>
    /// <param name="samples">Sample input buffer.</param>
    /// <param name="numSamples">Number of sample frames in buffer. Notice
    /// that in case of multi-channel sound a single 
    /// sample frame contains data for all channels.</param>
    internal void PutSamplesI16(short[] samples, uint numSamples)
    {
      lock (SyncRoot) { NativeMethods.PutSamples_i16(handle, samples, numSamples); }
    }

    /// <summary>
    /// Receive processed samples from the processor.
    /// </summary>
    /// <param name="outBuffer">Buffer where to copy output samples</param>
    /// <param name="maxSamples">Max number of sample frames to receive</param>
    /// <returns>The number of samples received</returns>
    internal uint ReceiveSamples(float[] outBuffer, uint maxSamples)
    {
      lock (SyncRoot) { return NativeMethods.ReceiveSamples(handle, outBuffer, maxSamples); }
    }

    /// <summary>
    /// int16 version of receiveSamples(): This converts internal float samples
    /// into int16 (short) return data type
    /// </summary>
    /// <param name="outBuffer">Buffer where to copy output samples.</param>
    /// <param name="maxSamples">How many samples to receive at max.</param>
    /// <returns>Number of received sample frames</returns>
    internal uint ReceiveSamplesI16(short[] outBuffer, uint maxSamples)
    {
      lock (SyncRoot) { return NativeMethods.ReceiveSamples_i16(handle, outBuffer, maxSamples); }
    }

    #endregion

    #region IDisposable Support

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="alsoManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    private void Dispose(bool alsoManaged)
    {
      if (!IsDisposed)
      {
        if (alsoManaged)
        {
          // NOTE: Placeholder, dispose managed state (managed objects).
          // At this point, nothing managed to dispose
        }

        NativeMethods.DestroyInstance(handle);
        handle = IntPtr.Zero;

        IsDisposed = true;
      }
    }

    #endregion

    #region Native Methods

    /// <summary>
    /// Provides direct access to mapped DLL methods
    /// </summary>
    private static class NativeMethods
    {
      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_getVersionId")]
      internal static extern int GetVersionId();

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_createInstance")]
      internal static extern IntPtr CreateInstance();

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_destroyInstance")]
      internal static extern void DestroyInstance(IntPtr h);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_getVersionString")]
      internal static extern IntPtr GetVersionString();

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setRate")]
      internal static extern void SetRate(IntPtr h, float newRate);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setTempo")]
      internal static extern void SetTempo(IntPtr h, float newTempo);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setRateChange")]
      internal static extern void SetRateChange(IntPtr h, float newRate);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setTempoChange")]
      internal static extern void SetTempoChange(IntPtr h, float newTempo);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setPitch")]
      internal static extern void SetPitch(IntPtr h, float newPitch);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setPitchOctaves")]
      internal static extern void SetPitchOctaves(IntPtr h, float newPitch);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setPitchSemiTones")]
      internal static extern void SetPitchSemiTones(IntPtr h, float newPitch);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setChannels")]
      internal static extern void SetChannels(IntPtr h, uint numChannels);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setSampleRate")]
      internal static extern void SetSampleRate(IntPtr h, uint srate);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_flush")]
      internal static extern void Flush(IntPtr h);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_putSamples")]
      internal static extern void PutSamples(IntPtr h, float[] samples, uint numSamples);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_putSamples_i16")]
      internal static extern void PutSamples_i16(IntPtr h, short[] samples, uint numSamples);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_clear")]
      internal static extern void Clear(IntPtr h);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_setSetting")]
      internal static extern int SetSetting(IntPtr h, int settingId, int value);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_getSetting")]
      internal static extern int GetSetting(IntPtr h, int settingId);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_numUnprocessedSamples")]
      internal static extern uint NumUnprocessedSamples(IntPtr h);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_receiveSamples")]
      internal static extern uint ReceiveSamples(IntPtr h, float[] outBuffer, uint maxSamples);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_receiveSamples_i16")]
      internal static extern uint ReceiveSamples_i16(IntPtr h, short[] outBuffer, uint maxSamples);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_numSamples")]
      internal static extern uint NumSamples(IntPtr h);

      [DllImport(SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "soundtouch_isEmpty")]
      internal static extern int IsEmpty(IntPtr h);
    }

    #endregion
  }

  internal sealed class BPMDetect : IDisposable
  {
    #region Private Members

    private readonly object SyncRoot = new object();
    private bool IsDisposed = false;
    private IntPtr handle;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="BPMDetect"/> class.
    /// </summary>
    internal BPMDetect(int numChannels, int sampleRate)
    {
      handle = NativeMethods.BpmCreateInstance(numChannels, sampleRate);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="BPMDetect"/> class.
    /// </summary>
    ~BPMDetect()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(false);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns the analysed BPM rate.
    /// </summary>
    internal float Bpm
    {
      get { lock (SyncRoot) { return NativeMethods.BpmGet(handle); } }
    }

    #endregion

    #region Sample Stream Methods

    /// <summary>
    /// Feed 'numSamples' sample into the BPM detector
    /// </summary>
    /// <param name="samples">Sample buffer to input</param>
    /// <param name="numSamples">Number of sample frames in buffer. Notice
    /// that in case of multi-channel sound a single sample frame contains 
    /// data for all channels</param>
    internal void PutSamples(float[] samples, uint numSamples)
    {
      lock (SyncRoot) { NativeMethods.BpmPutSamples(handle, samples, numSamples); }
    }

    /// <summary>
    /// int16 version of putSamples(): This accept int16 (short) sample data
    /// and internally converts it to float format before processing
    /// </summary>
    /// <param name="samples">Sample input buffer.</param>
    /// <param name="numSamples">Number of sample frames in buffer. Notice
    /// that in case of multi-channel sound a single 
    /// sample frame contains data for all channels.</param>
    internal void PutSamplesI16(short[] samples, uint numSamples)
    {
      lock (SyncRoot) { NativeMethods.BpmPutSamples_i16(handle, samples, numSamples); }
    }

    #endregion

    #region IDisposable Support

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="alsoManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    private void Dispose(bool alsoManaged)
    {
      if (!IsDisposed)
      {
        if (alsoManaged)
        {
          // NOTE: Placeholder, dispose managed state (managed objects).
          // At this point, nothing managed to dispose
        }

        NativeMethods.BpmDestroyInstance(handle);
        handle = IntPtr.Zero;

        IsDisposed = true;
      }
    }

    #endregion

    #region Native Methods

    /// <summary>
    /// Provides direct access to mapped DLL methods
    /// </summary>
    private static class NativeMethods
    {
      [DllImport(SoundTouch.SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "bpm_createInstance")]
      internal static extern IntPtr BpmCreateInstance(int numChannels, int sampleRate);

      [DllImport(SoundTouch.SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "bpm_destroyInstance")]
      internal static extern void BpmDestroyInstance(IntPtr h);

      [DllImport(SoundTouch.SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "bpm_putSamples")]
      internal static extern void BpmPutSamples(IntPtr h, float[] samples, uint numSamples);

      [DllImport(SoundTouch.SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "bpm_putSamples_i16")]
      internal static extern void BpmPutSamples_i16(IntPtr h, short[] samples, uint numSamples);

      [DllImport(SoundTouch.SoundTouchLibrary, CallingConvention = CallingConvention.Cdecl, EntryPoint = "bpm_getBpm")]
      internal static extern float BpmGet(IntPtr h);
    }

    #endregion
  }

}