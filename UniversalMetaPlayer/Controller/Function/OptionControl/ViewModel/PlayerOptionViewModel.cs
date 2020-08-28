using System;
using System.Collections.Generic;
using System.Text;

using UMP.Core;
using UMP.Core.Global;
using UMP.Core.Model.ViewModel;

namespace UMP.Controller.Function.OptionControl.ViewModel
{
  public class PlayerOptionViewModel : ViewModelBase
  {
    public PlayerOptionViewModel()
    {
      GlobalProperty.PropertyChanged += (_, e) =>
      {
      };
    }
  }
}
