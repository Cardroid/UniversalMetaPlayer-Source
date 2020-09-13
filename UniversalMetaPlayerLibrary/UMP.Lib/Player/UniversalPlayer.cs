using System;
using System.Collections.Generic;
using System.Text;

using NAudio.Wave;

using UMP.Lib.Player.Model;

namespace UMP.Lib.Player
{
  public class UniversalPlayer
  {
    private UniversalPlayer()
    {
      Options = new PlayerOption();
    }

    private static UniversalPlayer Player =  null;

    public static UniversalPlayer GetUniversalPlayer()
    {
      if (Player == null)
        Player = new UniversalPlayer();

      return Player;
    }

    /// <summary>
    /// 메인 플레이어
    /// </summary>
    private IWavePlayer WavePlayer { get; set; }

    public ISampleAggregatorChain Aggregator => _Aggregator;
    private SampleAggregatorChain _Aggregator = null;

    public event PlayerStatusChangedEventHandler PlayerStatusChanged;

    public PlayerOption Options { get; } = null;

    public void Init()
    {
      WavePlayer.Init(_Aggregator.Head);
    }
  }
}
