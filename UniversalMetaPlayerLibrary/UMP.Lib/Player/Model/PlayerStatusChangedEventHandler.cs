using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.Lib.Player.Model
{
  public delegate void PlayerStatusChangedEventHandler(object o, PlayerStatusChangedEventArgs args);

  public class PlayerStatusChangedEventArgs : EventArgs
  {
    public PlayerStatusChangedEventArgs(PlayerStatusName statusName)
    {
      this.StatusName = statusName;
    }

    public PlayerStatusName StatusName { get; }
  }

  public enum PlayerStatusName
  {
    Init, 
    Unload, Loaded,
    Play, Pause, Stop,
    PluginAdded, PluginRemoved,
  }
}
