using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows.Media;
using System.Windows.Threading;

using UMP.Core.Global;
using UMP.Core.Model.ViewModel;
using UMP.Core.Player;
using UMP.Utils;

namespace UMP.Controller.Function.OptionControl.ViewModel
{
  public class PlayerOptionViewModel : ViewModelBase
  {
    public PlayerOptionViewModel()
    {
      //GlobalProperty.PropertyChanged += (_, e) =>
      //{
      //};

      SetDefaultMediaPlayerCommand = new RelayCommand((o) => SetDefault_Click());
    }

    #region 플레이어 설정 값 초기화
    public void SetDispatcher(Dispatcher dispatcher) => ViewDispatcher = dispatcher;
    private Dispatcher ViewDispatcher { get; set; }
    public RelayCommand SetDefaultMediaPlayerCommand { get; }
    public Brush SetDefaultMediaPlayerButtenForeground { get; set; } = ThemeHelper.IsDarkMode ? Brushes.White : Brushes.Black;

    private bool IsReset
    {
      get => _IsReset;
      set
      {
        _IsReset = value;

        if (_IsReset)
          SetDefaultMediaPlayerButtenForeground = Brushes.Red;
        else
          SetDefaultMediaPlayerButtenForeground = ThemeHelper.IsDarkMode ? Brushes.White : Brushes.Black;

        OnPropertyChanged("SetDefaultMediaPlayerButtenForeground");
      }
    }
    private bool _IsReset = false;

    private Timer IsResetLockTimer;

    private void SetDefault_Click()
    {
      if (IsResetLockTimer == null)
      {
        IsResetLockTimer = new Timer(3000) { AutoReset = true };
        IsResetLockTimer.Elapsed += (_, e) =>
        {
          IsResetLockTimer.Stop();
          ViewDispatcher.Invoke(() => { IsReset = false; });
        };
      }

      if (IsReset)
      {
        IsReset = false;
        MainMediaPlayer.OptionSetDefault();
      }
      else
      {
        IsReset = true;
        IsResetLockTimer.Start();
      }
    }
    #endregion
  }
}
