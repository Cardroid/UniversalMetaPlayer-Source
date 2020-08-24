using UMP.Core.Player;

namespace UMP.Core.Global
{
  public static class GlobalEventLogger
  {
    private static Log Log { get; }

    static GlobalEventLogger()
    {
      Log = new Log(typeof(GlobalEventLogger));
      GlobalProperty.PropertyChanged += (_, e) =>
      {
        if (IsEnabled)
          Log.Info($"Event Handled GlobalProperty.PropertyChanged {e.PropertyName}");
      };
      MainMediaPlayer.PlayList.Field_PropertyChanged += (_, e) =>
      {
        if (IsEnabled)
          Log.Info($"Event Handled MainMediaPlayer.PlayList.Field_PropertyChanged {e.PropertyName}");
      };
      MainMediaPlayer.PropertyChanged += (_, e) =>
      {
        if (IsEnabled)
          Log.Info($"Event Handled MainMediaPlayer.PropertyChanged {e.PropertyName}");
        if (e.PropertyName == "PlayList")
          MainMediaPlayer.PlayList.Field_PropertyChanged += (_, e) =>
          {
            if (IsEnabled)
              Log.Info($"Event Handled MainMediaPlayer.PlayList.Field_PropertyChanged {e.PropertyName}");
          };
      };
    }

    public static bool IsEnabled { get; set; }
  }
}
