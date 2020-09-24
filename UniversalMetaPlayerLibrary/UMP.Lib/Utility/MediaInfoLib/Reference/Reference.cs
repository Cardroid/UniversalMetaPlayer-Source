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
        internal const string DLLName64 = "MediaInfo.64.dll";

        internal const string DLLName32 = "MediaInfo.32.dll";

        internal static bool Is64Bit => Environment.Is64BitProcess;


        //Import of DLL functions. DO NOT USE until you know what you do (MediaInfo DLL do NOT use CoTaskMemAlloc to allocate memory)
        private static IntPtr MediaInfo_New() => Is64Bit ? NativeMethod64.MediaInfo_New() : NativeMethod32.MediaInfo_New();
        private static void MediaInfo_Delete(IntPtr Handle)
        {
            if (Is64Bit)
                NativeMethod64.MediaInfo_Delete(Handle);
            else
                NativeMethod32.MediaInfo_Delete(Handle);
        }
        private static IntPtr MediaInfo_Open(IntPtr Handle, string FileName) => Is64Bit ? NativeMethod64.MediaInfo_Open(Handle, FileName) : NativeMethod32.MediaInfo_Open(Handle, FileName);
        private static IntPtr MediaInfoA_Open(IntPtr Handle, IntPtr FileName) => Is64Bit ? NativeMethod64.MediaInfoA_Open(Handle, FileName) : NativeMethod32.MediaInfoA_Open(Handle, FileName);
        private static IntPtr MediaInfo_Open_Buffer_Init(IntPtr Handle, long File_Size, long File_Offset) => Is64Bit ? NativeMethod64.MediaInfo_Open_Buffer_Init(Handle, File_Size, File_Offset) : NativeMethod32.MediaInfo_Open_Buffer_Init(Handle, File_Size, File_Offset);
        private static IntPtr MediaInfoA_Open(IntPtr Handle, long File_Size, long File_Offset) => Is64Bit ? NativeMethod64.MediaInfoA_Open(Handle, File_Size, File_Offset) : NativeMethod32.MediaInfoA_Open(Handle, File_Size, File_Offset);
        private static IntPtr MediaInfo_Open_Buffer_Continue(IntPtr Handle, IntPtr Buffer, IntPtr Buffer_Size) => Is64Bit ? NativeMethod64.MediaInfo_Open_Buffer_Continue(Handle, Buffer, Buffer_Size) : NativeMethod32.MediaInfo_Open_Buffer_Continue(Handle, Buffer, Buffer_Size);
        private static IntPtr MediaInfoA_Open_Buffer_Continue(IntPtr Handle, long File_Size, byte[] Buffer, IntPtr Buffer_Size) => Is64Bit ? NativeMethod64.MediaInfoA_Open_Buffer_Continue(Handle, File_Size, Buffer, Buffer_Size) : NativeMethod32.MediaInfoA_Open_Buffer_Continue(Handle, File_Size, Buffer, Buffer_Size);
        private static long MediaInfo_Open_Buffer_Continue_GoTo_Get(IntPtr Handle) => Is64Bit ? NativeMethod64.MediaInfo_Open_Buffer_Continue_GoTo_Get(Handle) : NativeMethod32.MediaInfo_Open_Buffer_Continue_GoTo_Get(Handle);
        private static long MediaInfoA_Open_Buffer_Continue_GoTo_Get(IntPtr Handle) => Is64Bit ? NativeMethod64.MediaInfoA_Open_Buffer_Continue_GoTo_Get(Handle) : NativeMethod32.MediaInfoA_Open_Buffer_Continue_GoTo_Get(Handle);
        private static IntPtr MediaInfo_Open_Buffer_Finalize(IntPtr Handle) => Is64Bit ? NativeMethod64.MediaInfo_Open_Buffer_Finalize(Handle) : NativeMethod32.MediaInfo_Open_Buffer_Finalize(Handle);
        private static IntPtr MediaInfoA_Open_Buffer_Finalize(IntPtr Handle) => Is64Bit ? NativeMethod64.MediaInfoA_Open_Buffer_Finalize(Handle) : NativeMethod32.MediaInfoA_Open_Buffer_Finalize(Handle);
        private static void MediaInfo_Close(IntPtr Handle)
        {
            if (Is64Bit)
                NativeMethod64.MediaInfo_Close(Handle);
            else
                NativeMethod32.MediaInfo_Close(Handle);
        }
        private static IntPtr MediaInfo_Inform(IntPtr Handle, IntPtr Reserved) => Is64Bit ? NativeMethod64.MediaInfo_Inform(Handle, Reserved) : NativeMethod32.MediaInfo_Inform(Handle, Reserved);
        private static IntPtr MediaInfoA_Inform(IntPtr Handle, IntPtr Reserved) => Is64Bit ? NativeMethod64.MediaInfoA_Inform(Handle, Reserved) : NativeMethod32.MediaInfoA_Inform(Handle, Reserved);
        private static IntPtr MediaInfo_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo) => Is64Bit ? NativeMethod64.MediaInfo_GetI(Handle, StreamKind, StreamNumber, Parameter, KindOfInfo) : NativeMethod32.MediaInfo_GetI(Handle, StreamKind, StreamNumber, Parameter, KindOfInfo);
        private static IntPtr MediaInfoA_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo) => Is64Bit ? NativeMethod64.MediaInfoA_GetI(Handle, StreamKind, StreamNumber, Parameter, KindOfInfo) : NativeMethod32.MediaInfoA_GetI(Handle, StreamKind, StreamNumber, Parameter, KindOfInfo);
        private static IntPtr MediaInfo_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch) => Is64Bit ? NativeMethod64.MediaInfo_Get(Handle, StreamKind, StreamNumber, Parameter, KindOfInfo, KindOfSearch) : NativeMethod32.MediaInfo_Get(Handle, StreamKind, StreamNumber, Parameter, KindOfInfo, KindOfSearch);
        private static IntPtr MediaInfoA_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch) => Is64Bit ? NativeMethod64.MediaInfoA_Get(Handle, StreamKind, StreamNumber, Parameter, KindOfInfo, KindOfSearch) : NativeMethod32.MediaInfoA_Get(Handle, StreamKind, StreamNumber, Parameter, KindOfInfo, KindOfSearch);
        private static IntPtr MediaInfo_Option(IntPtr Handle, string Option, string Value) => Is64Bit ? NativeMethod64.MediaInfo_Option(Handle, Option, Value) : NativeMethod32.MediaInfo_Option(Handle, Option, Value);
        private static IntPtr MediaInfoA_Option(IntPtr Handle, IntPtr Option, IntPtr Value) => Is64Bit ? NativeMethod64.MediaInfoA_Option(Handle, Option, Value) : NativeMethod32.MediaInfoA_Option(Handle, Option, Value);
        private static IntPtr MediaInfo_State_Get(IntPtr Handle) => Is64Bit ? NativeMethod64.MediaInfo_State_Get(Handle) : NativeMethod32.MediaInfo_State_Get(Handle);
        private static IntPtr MediaInfo_Count_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber) => Is64Bit ? NativeMethod64.MediaInfo_Count_Get(Handle, StreamKind, StreamNumber) : NativeMethod32.MediaInfo_Count_Get(Handle, StreamKind, StreamNumber);

        #region Native Methods
        private static class NativeMethod64
        {
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfo_New();
            [DllImport(DLLName64)]
            internal static extern void MediaInfo_Delete(IntPtr Handle);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfo_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfoA_Open(IntPtr Handle, IntPtr FileName);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfo_Open_Buffer_Init(IntPtr Handle, long File_Size, long File_Offset);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfoA_Open(IntPtr Handle, long File_Size, long File_Offset);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfo_Open_Buffer_Continue(IntPtr Handle, IntPtr Buffer, IntPtr Buffer_Size);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfoA_Open_Buffer_Continue(IntPtr Handle, long File_Size, byte[] Buffer, IntPtr Buffer_Size);
            [DllImport(DLLName64)]
            internal static extern long MediaInfo_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);
            [DllImport(DLLName64)]
            internal static extern long MediaInfoA_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfo_Open_Buffer_Finalize(IntPtr Handle);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfoA_Open_Buffer_Finalize(IntPtr Handle);
            [DllImport(DLLName64)]
            internal static extern void MediaInfo_Close(IntPtr Handle);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfo_Inform(IntPtr Handle, IntPtr Reserved);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfoA_Inform(IntPtr Handle, IntPtr Reserved);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfo_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfoA_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfo_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfoA_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfo_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfoA_Option(IntPtr Handle, IntPtr Option, IntPtr Value);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfo_State_Get(IntPtr Handle);
            [DllImport(DLLName64)]
            internal static extern IntPtr MediaInfo_Count_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber);
        }

        private static class NativeMethod32
        {
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfo_New();
            [DllImport(DLLName32)]
            internal static extern void MediaInfo_Delete(IntPtr Handle);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfo_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfoA_Open(IntPtr Handle, IntPtr FileName);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfo_Open_Buffer_Init(IntPtr Handle, long File_Size, long File_Offset);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfoA_Open(IntPtr Handle, long File_Size, long File_Offset);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfo_Open_Buffer_Continue(IntPtr Handle, IntPtr Buffer, IntPtr Buffer_Size);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfoA_Open_Buffer_Continue(IntPtr Handle, long File_Size, byte[] Buffer, IntPtr Buffer_Size);
            [DllImport(DLLName32)]
            internal static extern long MediaInfo_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);
            [DllImport(DLLName32)]
            internal static extern long MediaInfoA_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfo_Open_Buffer_Finalize(IntPtr Handle);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfoA_Open_Buffer_Finalize(IntPtr Handle);
            [DllImport(DLLName32)]
            internal static extern void MediaInfo_Close(IntPtr Handle);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfo_Inform(IntPtr Handle, IntPtr Reserved);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfoA_Inform(IntPtr Handle, IntPtr Reserved);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfo_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfoA_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfo_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfoA_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfo_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfoA_Option(IntPtr Handle, IntPtr Option, IntPtr Value);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfo_State_Get(IntPtr Handle);
            [DllImport(DLLName32)]
            internal static extern IntPtr MediaInfo_Count_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber);
        }
        #endregion
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

        //Import of DLL functions. DO NOT USE until you know what you do (MediaInfo DLL do NOT use CoTaskMemAlloc to allocate memory)
        internal static IntPtr MediaInfoList_New() => Lib_MediaInfo.Is64Bit ? NativeMethod64.MediaInfoList_New() : NativeMethod32.MediaInfoList_New();
        internal static void MediaInfoList_Delete(IntPtr Handle)
        {
            if (Lib_MediaInfo.Is64Bit)
                NativeMethod64.MediaInfoList_Delete(Handle);
            else
                NativeMethod32.MediaInfoList_Delete(Handle);
        }
        internal static IntPtr MediaInfoList_Open(IntPtr Handle, string FileName, IntPtr Options) => Lib_MediaInfo.Is64Bit ? NativeMethod64.MediaInfoList_Open(Handle, FileName, Options) : NativeMethod32.MediaInfoList_Open(Handle, FileName, Options);
        internal static void MediaInfoList_Close(IntPtr Handle, IntPtr FilePos)
        {
            if (Lib_MediaInfo.Is64Bit)
                NativeMethod64.MediaInfoList_Close(Handle, FilePos);
            else
                NativeMethod32.MediaInfoList_Close(Handle, FilePos);
        }
        internal static IntPtr MediaInfoList_Inform(IntPtr Handle, IntPtr FilePos, IntPtr Reserved) => Lib_MediaInfo.Is64Bit ? NativeMethod64.MediaInfoList_Inform(Handle, FilePos, Reserved) : NativeMethod32.MediaInfoList_Inform(Handle, FilePos, Reserved);
        internal static IntPtr MediaInfoList_GetI(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo) => Lib_MediaInfo.Is64Bit ? NativeMethod64.MediaInfoList_GetI(Handle, FilePos, StreamKind, StreamNumber, Parameter, KindOfInfo) : NativeMethod32.MediaInfoList_GetI(Handle, FilePos, StreamKind, StreamNumber, Parameter, KindOfInfo);
        internal static IntPtr MediaInfoList_Get(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber, string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch) => Lib_MediaInfo.Is64Bit ? NativeMethod64.MediaInfoList_Get(Handle, FilePos, StreamKind, StreamNumber, Parameter, KindOfInfo, KindOfSearch) : NativeMethod32.MediaInfoList_Get(Handle, FilePos, StreamKind, StreamNumber, Parameter, KindOfInfo, KindOfSearch);
        internal static IntPtr MediaInfoList_Option(IntPtr Handle, string Option,  string Value) => Lib_MediaInfo.Is64Bit ? NativeMethod64.MediaInfoList_Option(Handle, Option, Value) : NativeMethod32.MediaInfoList_Option(Handle, Option, Value);
        internal static IntPtr MediaInfoList_State_Get(IntPtr Handle) => Lib_MediaInfo.Is64Bit ? NativeMethod64.MediaInfoList_State_Get(Handle) : NativeMethod32.MediaInfoList_State_Get(Handle);
        internal static IntPtr MediaInfoList_Count_Get(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber) => Lib_MediaInfo.Is64Bit ? NativeMethod64.MediaInfoList_Count_Get(Handle, FilePos, StreamKind, StreamNumber) : NativeMethod32.MediaInfoList_Count_Get(Handle, FilePos, StreamKind, StreamNumber);

        #region Native Methods
        private static class NativeMethod32
        {
            //Import of DLL functions. DO NOT USE until you know what you do (MediaInfo DLL do NOT use CoTaskMemAlloc to allocate memory)
            [DllImport(Lib_MediaInfo.DLLName32)]
            internal static extern IntPtr MediaInfoList_New();
            [DllImport(Lib_MediaInfo.DLLName32)]
            internal static extern void MediaInfoList_Delete(IntPtr Handle);
            [DllImport(Lib_MediaInfo.DLLName32)]
            internal static extern IntPtr MediaInfoList_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName, IntPtr Options);
            [DllImport(Lib_MediaInfo.DLLName32)]
            internal static extern void MediaInfoList_Close(IntPtr Handle, IntPtr FilePos);
            [DllImport(Lib_MediaInfo.DLLName32)]
            internal static extern IntPtr MediaInfoList_Inform(IntPtr Handle, IntPtr FilePos, IntPtr Reserved);
            [DllImport(Lib_MediaInfo.DLLName32)]
            internal static extern IntPtr MediaInfoList_GetI(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
            [DllImport(Lib_MediaInfo.DLLName32)]
            internal static extern IntPtr MediaInfoList_Get(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
            [DllImport(Lib_MediaInfo.DLLName32)]
            internal static extern IntPtr MediaInfoList_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);
            [DllImport(Lib_MediaInfo.DLLName32)]
            internal static extern IntPtr MediaInfoList_State_Get(IntPtr Handle);
            [DllImport(Lib_MediaInfo.DLLName32)]
            internal static extern IntPtr MediaInfoList_Count_Get(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber);
        }

        private static class NativeMethod64
        {
            //Import of DLL functions. DO NOT USE until you know what you do (MediaInfo DLL do NOT use CoTaskMemAlloc to allocate memory)
            [DllImport(Lib_MediaInfo.DLLName64)]
            internal static extern IntPtr MediaInfoList_New();
            [DllImport(Lib_MediaInfo.DLLName64)]
            internal static extern void MediaInfoList_Delete(IntPtr Handle);
            [DllImport(Lib_MediaInfo.DLLName64)]
            internal static extern IntPtr MediaInfoList_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName, IntPtr Options);
            [DllImport(Lib_MediaInfo.DLLName64)]
            internal static extern void MediaInfoList_Close(IntPtr Handle, IntPtr FilePos);
            [DllImport(Lib_MediaInfo.DLLName64)]
            internal static extern IntPtr MediaInfoList_Inform(IntPtr Handle, IntPtr FilePos, IntPtr Reserved);
            [DllImport(Lib_MediaInfo.DLLName64)]
            internal static extern IntPtr MediaInfoList_GetI(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
            [DllImport(Lib_MediaInfo.DLLName64)]
            internal static extern IntPtr MediaInfoList_Get(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
            [DllImport(Lib_MediaInfo.DLLName64)]
            internal static extern IntPtr MediaInfoList_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);
            [DllImport(Lib_MediaInfo.DLLName64)]
            internal static extern IntPtr MediaInfoList_State_Get(IntPtr Handle);
            [DllImport(Lib_MediaInfo.DLLName64)]
            internal static extern IntPtr MediaInfoList_Count_Get(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber);
        }
        #endregion
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
