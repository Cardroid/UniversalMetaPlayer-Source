﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UMP.Core;

namespace UMP.Controller.Option.OptionControl
{
  public partial class ThemeOption : UserControl
  {
    public ThemeOption()
    {
      InitializeComponent();

      OptionSync();
      GlobalProperty.PropertyChanged += (s) => 
      {
        OptionSync();
      };

      this.IsAverageColorTheme.Click += (_, e) =>
      {
        if (this.IsAverageColorTheme.IsChecked.HasValue && this.IsAverageColorTheme.IsChecked.Value)
          GlobalProperty.IsAverageColorTheme = true;
        else
          GlobalProperty.IsAverageColorTheme = false;
      };
    }

    /// <summary>
    /// 옵션 - UI 동기화
    /// </summary>
    private void OptionSync()
    {
      this.IsAverageColorTheme.IsChecked = GlobalProperty.IsAverageColorTheme;
    }
  }
}