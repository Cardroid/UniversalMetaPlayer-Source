using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using UMP.Core.Model;

namespace UMP.Utility
{
  public static class DialogHelper
  {
    /// <summary>
    /// FileDialog 창 열기
    /// </summary>
    /// <param name="title">창의 이름</param>
    /// <param name="filter">필터</param>
    /// <param name="multiselect">다중 선택 허용 여부</param>
    /// <param name="defaultPath">기본 경로</param>
    /// <returns>선택한 파일의 경로 문자열 배열</returns>
    public static GenericResult<string[]> OpenFileDialog(string title, string filter, bool multiselect = false, string defaultPath = "")
    {
      OpenFileDialog openFile = new OpenFileDialog
      {
        Title = title,
        Multiselect = multiselect,
        Filter = filter,
        CheckPathExists = true,
        CheckFileExists = true
      };

      if (!string.IsNullOrWhiteSpace(defaultPath))
        openFile.InitialDirectory = defaultPath;

      var result = openFile.ShowDialog();
      if (result.HasValue && result.Value)
      {
        return new GenericResult<string[]>(true, openFile.FileNames);
      }
      else
        return new GenericResult<string[]>(false);
    }

    /// <summary>
    /// DirectoryDialog 창 열기
    /// </summary>
    /// <param name="title">창의 이름</param>
    /// <param name="defaultPath">기본 경로</param>
    /// <returns>선택한 폴더의 경로 문자열</returns>
    public static GenericResult<string> OpenDirectoryDialog(string title, string defaultPath = "")
    {
      CommonOpenFileDialog folderDialog = new CommonOpenFileDialog()
      {
        Title = title,
        DefaultDirectory = defaultPath,
        IsFolderPicker = true 
      };

      if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok && Directory.Exists(folderDialog.FileName))
        return new GenericResult<string>(true, folderDialog.FileName);
      else
        return new GenericResult<string>(false);
    }
  }
}
