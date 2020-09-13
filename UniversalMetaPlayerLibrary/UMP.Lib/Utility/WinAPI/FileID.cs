using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace UMP.Lib.Utility.WinAPI
{
  internal class WinAPI
  {
    [DllImport("ntdll.dll", SetLastError = true)]
    internal static extern IntPtr NtQueryInformationFile(IntPtr fileHandle, ref IO_STATUS_BLOCK IoStatusBlock, IntPtr pInfoBlock, uint length, FILE_INFORMATION_CLASS fileInformation);

    internal struct IO_STATUS_BLOCK
    {
      uint status;
      ulong information;
    }

    internal struct _FILE_INTERNAL_INFORMATION
    {
      internal ulong IndexNumber;
    }

    // Abbreviated, there are more values than shown
    internal enum FILE_INFORMATION_CLASS
    {
      FileDirectoryInformation = 1, // 1
      FileFullDirectoryInformation, // 2
      FileBothDirectoryInformation, // 3
      FileBasicInformation,         // 4
      FileStandardInformation,      // 5
      FileInternalInformation       // 6
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool GetFileInformationByHandle(IntPtr hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

    internal struct BY_HANDLE_FILE_INFORMATION
    {
      public uint FileAttributes;
      public FILETIME CreationTime;
      public FILETIME LastAccessTime;
      public FILETIME LastWriteTime;
      public uint VolumeSerialNumber;
      public uint FileSizeHigh;
      public uint FileSizeLow;
      public uint NumberOfLinks;
      public uint FileIndexHigh;
      public uint FileIndexLow;
    }
  }

  public static class FileID
  {
    public static ulong GetFileID(this FileInfo file)
    {
      FileStream fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

      WinAPI.GetFileInformationByHandle(fs.SafeFileHandle.DangerousGetHandle(), out WinAPI.BY_HANDLE_FILE_INFORMATION objectFileInfo);

      fs.Close();

      ulong fileIndex = ((ulong)objectFileInfo.FileIndexHigh << 32) + objectFileInfo.FileIndexLow;

      return fileIndex;
    }
  }
}
