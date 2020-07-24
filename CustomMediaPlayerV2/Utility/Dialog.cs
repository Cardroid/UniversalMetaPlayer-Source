using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Win32;

namespace CMP2.Utility
{
  public static class DialogHelper
  {
    public static string OpenFileDialog(string title, string filter, string defaultPath = "")
    {
      OpenFileDialog openFile = new OpenFileDialog
      {
        Title = title,
        Multiselect = false,
        Filter = filter,
        CheckPathExists = true,
        CheckFileExists = true
      };

      if (!string.IsNullOrWhiteSpace(defaultPath))
        openFile.InitialDirectory = defaultPath;

      var result = openFile.ShowDialog();
      if (result.HasValue && result.Value)
      {
        return openFile.FileName;
      }
      else
        return string.Empty;
    }
  }
}
