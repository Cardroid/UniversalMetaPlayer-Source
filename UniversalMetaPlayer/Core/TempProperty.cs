using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.Core
{
  public static class TempProperty
  {
    static TempProperty()
    {
      SetDefault();
    }

    public static void SetDefault()
    {
      SaveCurrentPlayList = true;
      LoadContinue = false;
    }

    #region PlayListLoadDialog
    public static bool SaveCurrentPlayList { get; set; }
    public static bool LoadContinue { get; set; }
    #endregion
  }
}
