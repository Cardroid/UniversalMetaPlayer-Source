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
using System.Windows.Navigation;
using System.Windows.Shapes;

using CMP2.Controller.ViewModel;
using CMP2.Core;

namespace CMP2.Controller
{
  public partial class PlayListControl : UserControl
  {
    private PlayListControlViewModel ViewModel { get; set; }
    public PlayListControl()
    {
      InitializeComponent();
      ViewModel = (PlayListControlViewModel)this.DataContext;

      this.Loaded += (s, e) =>
      {
        // 헤더 설정
        this.ID.Header = "No.";
        this.Title.Header = "제목";
        this.Duration.Header = "길이";
      };

      this.Loaded += (s, e) =>
      {
        Log log = new Log(typeof(MediaInfoControl));
        log.Debug("초기화 성공");
      };
    }
  }
}