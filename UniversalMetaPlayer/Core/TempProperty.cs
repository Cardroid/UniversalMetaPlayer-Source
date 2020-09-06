using System.Collections.Generic;

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
      VarispeedChangerParameter = new VarispeedChangerParameter(1.0f, 1.0f, 1.0f);
      EqualizerBandParameter = new EqualizerBand[]
      {
        new EqualizerBand(0.8f, 63),
        new EqualizerBand(0.8f, 125),
        new EqualizerBand(0.8f, 250),
        new EqualizerBand(0.8f, 500),
        new EqualizerBand(0.8f, 1000),
        new EqualizerBand(0.8f, 2000),
        new EqualizerBand(0.8f, 4000),
        new EqualizerBand(0.8f, 8000),
        new EqualizerBand(0.8f, 12500),
        new EqualizerBand(0.8f, 16000),
      };
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

    #region Equalizer
    public static bool IsUseEqualizer { get; set; }
    public static EqualizerBand[] EqualizerBandParameter { get; set; }
    #endregion
  }
}
