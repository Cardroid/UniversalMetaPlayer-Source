using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UMP.Core;

namespace UMP
{
  /// <summary>
  /// PlayListWindow.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class PlayListWindow : Window
  {
    public PlayListWindow()
    {
      InitializeComponent();

      this.KeyDown += (_, e) => GlobalEvent.KeyDownEventInvoke(e);

      // 창 드레그 움직임
      this.MainPlayListWindowHelperControl.PlayListColorZone.MouseLeftButtonDown += PlayListWindow_WindowDrag;

      this.MaxWidth = SystemParameters.WorkArea.Width * (2.0 / 3.0);
      this.MaxHeight = SystemParameters.WorkArea.Height * (2.0 / 3.0);

      this.Closing += PlayListWindow_Closing;
    }

    private void PlayListWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      e.Cancel = true;
    }

    private void PlayListWindow_WindowDrag(object sender, MouseButtonEventArgs e) { this.DragMove(); e.Handled = true; }
  }
}
