/*  Copyright (c) MediaArea.net SARL. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license that can
 *  be found in the License.html file in the root of the source tree.
 */

//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//
// Microsoft Visual C# wrapper for MediaInfo Library
// See MediaInfo.h for help
//
// To make it working, you must put MediaInfo.Dll
// in the executable folder
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using System.Runtime.InteropServices;

namespace UMP.Lib.Utility.MediaInfoLib.Reference
{
  internal enum StreamKind
  {
    General,
    Video,
    Audio,
    Text,
    Other,
    Image,
    Menu,
  }

  internal enum InfoKind
  {
    Name,
    Text,
    Measure,
    Options,
    NameText,
    MeasureText,
    Info,
    HowTo
  }

  internal enum InfoOptions
  {
    ShowInInform,
    Support,
    ShowInSupported,
    TypeOfValue
  }

  internal enum InfoFileOptions
  {
    FileOption_Nothing = 0x00,
    FileOption_NoRecursive = 0x01,
    FileOption_CloseAll = 0x02,
    FileOption_Max = 0x04
  };

  internal enum Status
  {
    None = 0x00,
    Accepted = 0x01,
    Filled = 0x02,
    Updated = 0x04,
    Finalized = 0x08,
  }

  internal class Lib_MediaInfo : IDisposable
  {
    #region LoadDLL
#if BIT64
    private const string DLLName = "MediaInfo.64.dll";
#endif

#if BIT32 || BITANY
    private const string DLLName = "MediaInfo.32.dll";
#endif

    //Import of DLL functions. DO NOT USE until you know what you do (MediaInfo DLL do NOT use CoTaskMemAlloc to allocate memory)
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfo_New();
    [DllImport(DLLName)]
    private static extern void MediaInfo_Delete(IntPtr Handle);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfo_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoA_Open(IntPtr Handle, IntPtr FileName);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfo_Open_Buffer_Init(IntPtr Handle, long File_Size, long File_Offset);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoA_Open(IntPtr Handle, long File_Size, long File_Offset);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfo_Open_Buffer_Continue(IntPtr Handle, IntPtr Buffer, IntPtr Buffer_Size);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoA_Open_Buffer_Continue(IntPtr Handle, long File_Size, byte[] Buffer, IntPtr Buffer_Size);
    [DllImport(DLLName)]
    private static extern long MediaInfo_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);
    [DllImport(DLLName)]
    private static extern long MediaInfoA_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfo_Open_Buffer_Finalize(IntPtr Handle);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoA_Open_Buffer_Finalize(IntPtr Handle);
    [DllImport(DLLName)]
    private static extern void MediaInfo_Close(IntPtr Handle);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfo_Inform(IntPtr Handle, IntPtr Reserved);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoA_Inform(IntPtr Handle, IntPtr Reserved);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfo_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoA_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfo_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoA_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfo_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoA_Option(IntPtr Handle, IntPtr Option, IntPtr Value);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfo_State_Get(IntPtr Handle);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfo_Count_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber);
    #endregion

    internal Lib_MediaInfo()
    {
      try { Handle = MediaInfo_New(); }
      catch { Handle = IntPtr.Zero; }

      if (Environment.OSVersion.ToString().IndexOf("Windows") == -1)
        MustUseAnsi = true;
      else
        MustUseAnsi = false;
    }

    private const string LIB_NOT_LOADED = "Unable to load MediaInfo library";
    private readonly IntPtr Handle;
    private readonly bool MustUseAnsi;
    private bool disposedValue;

    internal int Open(string FileName)
    {
      if (!IsLibLoaded())
          return int.MinValue;

      if (MustUseAnsi)
      {
        IntPtr FileName_Ptr = Marshal.StringToHGlobalAnsi(FileName);
        int ToReturn = (int)MediaInfoA_Open(Handle, FileName_Ptr);
        Marshal.FreeHGlobal(FileName_Ptr);
        return ToReturn;
      }
      else
        return (int)MediaInfo_Open(Handle, FileName);
    }
    internal int Open_Buffer_Init(long File_Size, long File_Offset)
    {
      if (!IsLibLoaded())
          return int.MinValue;

      return (int)MediaInfo_Open_Buffer_Init(Handle, File_Size, File_Offset);
    }
    internal int Open_Buffer_Continue(IntPtr Buffer, IntPtr Buffer_Size)
    {
      if (!IsLibLoaded())
          return int.MinValue;

      return (int)MediaInfo_Open_Buffer_Continue(Handle, Buffer, Buffer_Size);
    }
    internal long Open_Buffer_Continue_GoTo_Get()
    {
      if (!IsLibLoaded())
        return int.MinValue;

      return MediaInfo_Open_Buffer_Continue_GoTo_Get(Handle);
    }
    internal int Open_Buffer_Finalize()
    {
      if (!IsLibLoaded())
        return int.MinValue;

      return (int)MediaInfo_Open_Buffer_Finalize(Handle);
    }

    internal void Close()
    {
      if (IsLibLoaded())
        MediaInfo_Close(Handle);
    }

    internal string Inform()
    {
      if (!IsLibLoaded())
        return LIB_NOT_LOADED;

      if (MustUseAnsi)
        return Marshal.PtrToStringAnsi(MediaInfoA_Inform(Handle, (IntPtr)0));
      else
        return Marshal.PtrToStringUni(MediaInfo_Inform(Handle, (IntPtr)0));
    }

    internal string Get(StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch)
    {
      if (!IsLibLoaded())
        return LIB_NOT_LOADED;

      if (MustUseAnsi)
      {
        IntPtr Parameter_Ptr = Marshal.StringToHGlobalAnsi(Parameter);
        string ToReturn = Marshal.PtrToStringAnsi(MediaInfoA_Get(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, Parameter_Ptr, (IntPtr)KindOfInfo, (IntPtr)KindOfSearch));
        Marshal.FreeHGlobal(Parameter_Ptr);
        return ToReturn;
      }
      else
        return Marshal.PtrToStringUni(MediaInfo_Get(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, Parameter, (IntPtr)KindOfInfo, (IntPtr)KindOfSearch));
    }

    internal string Get(StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo)
    {
      if (!IsLibLoaded())
        return LIB_NOT_LOADED;

      if (MustUseAnsi)
        return Marshal.PtrToStringAnsi(MediaInfoA_GetI(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, (IntPtr)Parameter, (IntPtr)KindOfInfo));
      else
        return Marshal.PtrToStringUni(MediaInfo_GetI(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, (IntPtr)Parameter, (IntPtr)KindOfInfo));
    }

    internal string Option(string Option, string Value)
    {
      if (!IsLibLoaded())
        return LIB_NOT_LOADED;

      if (MustUseAnsi)
      {
        IntPtr Option_Ptr = Marshal.StringToHGlobalAnsi(Option);
        IntPtr Value_Ptr = Marshal.StringToHGlobalAnsi(Value);
        string ToReturn = Marshal.PtrToStringAnsi(MediaInfoA_Option(Handle, Option_Ptr, Value_Ptr));
        Marshal.FreeHGlobal(Option_Ptr);
        Marshal.FreeHGlobal(Value_Ptr);
        return ToReturn;
      }
      else
        return Marshal.PtrToStringUni(MediaInfo_Option(Handle, Option, Value));
    }

    internal int State_Get()
    {
      if (!IsLibLoaded())
          return int.MinValue;

      return (int)MediaInfo_State_Get(Handle);
    }

    internal int Count_Get(StreamKind StreamKind, int StreamNumber)
    {
      if (!IsLibLoaded())
          return int.MinValue;

      return (int)MediaInfo_Count_Get(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber);
    }

    // �⺻ �� ����
    internal string Get(StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo) =>
      Get(StreamKind, StreamNumber, Parameter, KindOfInfo, InfoKind.Name);

    internal string Get(StreamKind StreamKind, int StreamNumber, string Parameter) =>
      Get(StreamKind, StreamNumber, Parameter, InfoKind.Text, InfoKind.Name);

    internal string Get(StreamKind StreamKind, int StreamNumber, int Parameter) =>
      Get(StreamKind, StreamNumber, Parameter, InfoKind.Text);

    internal string Option(string Option_) =>
      Option(Option_, "");

    internal int Count_Get(StreamKind StreamKind) =>
      Count_Get(StreamKind, -1);

    internal bool IsLibLoaded() => Handle != IntPtr.Zero;

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
        }

        if (IsLibLoaded())
          MediaInfo_Delete(Handle);

        disposedValue = true;
      }
    }

    ~Lib_MediaInfo()
    {
      Dispose(disposing: false);
    }

    public void Dispose()
    {
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
  }

  internal class Lib_MediaInfoList : IDisposable
  {
    #region LoadDLL
#if BIT64
    private const string DLLName = "MediaInfo.64.dll";
#endif

#if BIT32 || BITANY
    private const string DLLName = "MediaInfo.32.dll";
#endif

    //Import of DLL functions. DO NOT USE until you know what you do (MediaInfo DLL do NOT use CoTaskMemAlloc to allocate memory)
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoList_New();
    [DllImport(DLLName)]
    private static extern void MediaInfoList_Delete(IntPtr Handle);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoList_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName, IntPtr Options);
    [DllImport(DLLName)]
    private static extern void MediaInfoList_Close(IntPtr Handle, IntPtr FilePos);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoList_Inform(IntPtr Handle, IntPtr FilePos, IntPtr Reserved);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoList_GetI(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoList_Get(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoList_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoList_State_Get(IntPtr Handle);
    [DllImport(DLLName)]
    private static extern IntPtr MediaInfoList_Count_Get(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber);
    #endregion

    // MediaInfo class
    internal Lib_MediaInfoList()
    {
      Handle = MediaInfoList_New();
    }

    internal int Open(string FileName, InfoFileOptions Options) =>
      (int)MediaInfoList_Open(Handle, FileName, (IntPtr)Options);

    internal void Close(int FilePos) =>
      MediaInfoList_Close(Handle, (IntPtr)FilePos);

    internal string Inform(int FilePos) =>
      Marshal.PtrToStringUni(MediaInfoList_Inform(Handle, (IntPtr)FilePos, (IntPtr)0));

    internal string Get(int FilePos, StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch) => Marshal.PtrToStringUni(MediaInfoList_Get(Handle, (IntPtr)FilePos, (IntPtr)StreamKind, (IntPtr)StreamNumber, Parameter, (IntPtr)KindOfInfo, (IntPtr)KindOfSearch));

    internal string Get(int FilePos, StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo) =>
      Marshal.PtrToStringUni(MediaInfoList_GetI(Handle, (IntPtr)FilePos, (IntPtr)StreamKind, (IntPtr)StreamNumber, (IntPtr)Parameter, (IntPtr)KindOfInfo));

    internal string Option(string Option, string Value) =>
      Marshal.PtrToStringUni(MediaInfoList_Option(Handle, Option, Value));

    internal int State_Get() =>
      (int)MediaInfoList_State_Get(Handle);

    internal int Count_Get(int FilePos, StreamKind StreamKind, int StreamNumber) =>
      (int)MediaInfoList_Count_Get(Handle, (IntPtr)FilePos, (IntPtr)StreamKind, (IntPtr)StreamNumber);

    private readonly IntPtr Handle;
    private bool disposedValue;

    // �⺻ �� ����
    internal void Open(string FileName) =>
      Open(FileName, 0);

    internal void Close() =>
      Close(-1);

    internal string Get(int FilePos, StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo) =>
      Get(FilePos, StreamKind, StreamNumber, Parameter, KindOfInfo, InfoKind.Name);

    internal string Get(int FilePos, StreamKind StreamKind, int StreamNumber, string Parameter) =>
      Get(FilePos, StreamKind, StreamNumber, Parameter, InfoKind.Text, InfoKind.Name);

    internal string Get(int FilePos, StreamKind StreamKind, int StreamNumber, int Parameter) =>
      Get(FilePos, StreamKind, StreamNumber, Parameter, InfoKind.Text);

    internal string Option(string Option_) =>
      Option(Option_, "");

    internal int Count_Get(int FilePos, StreamKind StreamKind) =>
      Count_Get(FilePos, StreamKind, -1);

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
        }
        MediaInfoList_Delete(Handle);
        disposedValue = true;
      }
    }

    ~Lib_MediaInfoList()
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
