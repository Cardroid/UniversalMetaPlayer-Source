using UMP.Core.Player.Plugin.Effect;

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
      IsUseMediaInfoLibrary = false;
      VarispeedChangerParameter = new VarispeedChangerParameter();
    }

    #region PlayListLoadDialog
    public static bool SaveCurrentPlayList { get; set; }
    public static bool LoadContinue { get; set; }
    #endregion

    #region MediaAnalysis
    public static bool IsUseMediaInfoLibrary { get; set; }
    #endregion

    #region VarispeedChanger
    public static bool IsUseVarispeedChanger { get; set; }
    public static VarispeedChangerParameter VarispeedChangerParameter { get; set; }
    #endregion
  }
}
