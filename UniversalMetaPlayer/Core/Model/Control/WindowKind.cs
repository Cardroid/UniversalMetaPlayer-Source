using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;

using UMP.Controller;
using UMP.Controller.Function;
using UMP.Controller.Function.Lyrics;
using UMP.Controller.WindowHelper;
using UMP.Core.Player.Plugin.Control;

namespace UMP.Core.Model.Control
{
  public enum WindowKind
  {
    PlayList, Function,
    Lyrics,
    Log,
    AudioEffect,
    EQ
  }

  public static class WindowKindHelper
  {
    public static UserWindow CreateUserWindow(WindowKind windowKind) => windowKind switch
    {
      WindowKind.PlayList => new UserWindow(new PlayListControl(), "UMP - PlayList")
      { WindowStartupLocation = WindowStartupLocation.CenterOwner },
      WindowKind.Function => new UserWindow(new FunctionControl(), "UMP - Function")
      { WindowStartupLocation = WindowStartupLocation.CenterOwner },
      WindowKind.Lyrics => new UserWindow(new LyricsControl(), "UMP - Lyrics")
      { WindowStartupLocation = WindowStartupLocation.CenterScreen },
      WindowKind.Log => new UserWindow(Log.LogViewerAppender.LogTextBox, "UMP - Log", false, false),
      WindowKind.AudioEffect => new UserWindow(new VarispeedChangerParameterControl(), "UMP - AudioEffect")
      {
        SizeToContent = SizeToContent.WidthAndHeight,
        WindowStartupLocation = WindowStartupLocation.CenterScreen
      },
      WindowKind.EQ => new UserWindow(new EqualizerParameterControl(), "UMP - EQ")
      { WindowStartupLocation = WindowStartupLocation.CenterScreen },

      _ => null,
	};
  }
}
