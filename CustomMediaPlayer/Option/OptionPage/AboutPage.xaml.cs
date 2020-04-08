﻿using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CustomMediaPlayer.Option.OptionPage.ViewModel;

namespace CustomMediaPlayer.Option.OptionPage
{
    public partial class AboutPage : UserControl
    {
        public AboutPageViewModel ViewModel = new AboutPageViewModel();
        public AboutPage()
        {
            InitializeComponent();

            this.DataContext = ViewModel;

            // 배경색 동기화
            this.Background = ((MainWindow)Application.Current.MainWindow).viewModel.BackgroundBrush;
            ((MainWindow)Application.Current.MainWindow).viewModel.BackgroundColorChanged += (b) => { this.Background = b; };

            Mainlogo.Stretch = System.Windows.Media.Stretch.Fill;

            // 현재 버전 표시
            VersionLabel.Content = "Version : " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
