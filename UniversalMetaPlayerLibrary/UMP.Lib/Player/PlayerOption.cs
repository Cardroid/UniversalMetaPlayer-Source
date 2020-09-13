using System.ComponentModel;

namespace UMP.Lib.Player
{
  public class PlayerOption
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private float _Volume = 0.2f;
    private bool _Shuffle = false;
    private RepeatOption _Repeat;

    /// <summary>
    /// 볼륨
    /// </summary>
    public float Volume
    {
      get => _Volume;
      set
      {
        _Volume = value;
        OnPropertyChanged("Volume");
      }
    }

    /// <summary>
    /// 셔플
    /// </summary>
    public bool Shuffle
    {
      get => _Shuffle;
      set
      {
        _Shuffle = value;
        OnPropertyChanged("Shuffle");
      }
    }

    /// <summary>
    /// 반복 (0 = OFF, 1 = Once, 2 = All)
    /// </summary>
    public RepeatOption Repeat
    {
      get => _Repeat;
      set
      {
        _Repeat = value;
        OnPropertyChanged("Repeat");
      }
    }

    public enum RepeatOption { Off, Once, All }
  }
}
