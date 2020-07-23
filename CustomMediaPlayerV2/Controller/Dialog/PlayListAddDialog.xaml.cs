﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using CMP2.Core.Model;
using CMP2.Utility;

namespace CMP2.Controller.Dialog
{
  /// <summary>
  /// PlayListAddDialog.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class PlayListAddDialog : UserControl
  {
    public PlayListAddDialog()
    {
      InitializeComponent();

      this.AcceptButton.IsEnabled = false;
      this.UserTextBox.TextChanged += UserTextBox_TextChanged;
      this.MouseDown += (s, e) => { this.UserTextBox.Focus(); };
    }

    private bool IsWorkDelay = false;

    private async void UserTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(this.UserTextBox.Text))
      {
        this.MessageLabel.Content = "미디어의 위치";
        return;
      }

      if (IsWorkDelay)
        return;
      IsWorkDelay = true;
      this.ProgressRing.Visibility = Visibility.Visible;

      await Task.Delay(500);

      var result = Checker.MediaTypeChecker(this.UserTextBox.Text);
      if (result.HasValue)
      {
        this.AcceptButton.IsEnabled = true;
        this.MessageLabel.Content = $"미디어 타입 : {result.Value}";
      }
      else
      {
        this.AcceptButton.IsEnabled = false;
        this.MessageLabel.Content = "타입을 알 수 없습니다.";
      }
      
      this.ProgressRing.Visibility = Visibility.Collapsed;
      IsWorkDelay = false;
    }

    public Dictionary<string, string> GetResult()
    {
      var result = new Dictionary<string, string>();

      result.Add("MediaLocation", this.UserTextBox.Text);
      return result;
    }
  }
}