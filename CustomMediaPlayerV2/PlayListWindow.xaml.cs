using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CMP2.Core;

namespace CMP2
{
  /// <summary>
  /// PlayListWindow.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class PlayListWindow : Window
  {
    public PlayListWindow()
    {
      InitializeComponent();

      // 창 드레그 움직임
      this.MainPlayListControl.PlayListName.MouseLeftButtonDown += PlayListWindow_WindowDrag;
      this.MaxWidth = SystemParameters.WorkArea.Width * (2.0 / 3.0);
    }

    private void PlayListWindow_WindowDrag(object sender, MouseButtonEventArgs e) { this.DragMove(); e.Handled = true; }
  }
}
