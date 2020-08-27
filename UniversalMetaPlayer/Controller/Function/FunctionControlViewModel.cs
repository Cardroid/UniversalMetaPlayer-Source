using System;
using System.Windows.Media;
using UMP.Controller.Function.OptionControl;
using UMP.Core.Model.Func;
using UMP.Core.Model.ViewModel;
using UMP.Utility;

namespace UMP.Controller.Function
{
  public class FunctionControlViewModel : ViewModelBase
  {
    public FunctionControlViewModel()
    {
      FunctionPanel = new BasicOption();

      ThemeHelper.ThemeChangedEvent += (e) => this.ControlBorderBrush = new SolidColorBrush(e.PrimaryColor);
    }

    public string Header => FunctionPanel != null ? FunctionPanel.FunctionName : "기능 패널";

    public FunctionControlForm FunctionPanel
    {
      get => _FunctionPanel.IsAlive ? (FunctionControlForm)_FunctionPanel.Target : null;
      set
      {
        if (value == null)
          _FunctionPanel = new WeakReference(new ErrorPageControl());
        else
          _FunctionPanel = new WeakReference(value);

        OnPropertyChanged("FunctionPanel");
        OnPropertyChanged("Header");
      }
    }
    private WeakReference _FunctionPanel;

    public Brush ControlBorderBrush
    {
      get => _ControlBorderBrush;
      set
      {
        _ControlBorderBrush = value;
        OnPropertyChanged("ControlBorderBrush");
      }
    }
    private Brush _ControlBorderBrush = new SolidColorBrush(ThemeHelper.PrimaryColor);
  }
}
