using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Gma.System.MouseKeyHook;
using System.Windows;

namespace CustomMediaPlayer
{
    /// <summary>
    /// 전역 마우스 키보드 후킹을 통한 원격 조종 설정
    /// </summary>
    public class Hooking
    {
        private static IKeyboardMouseEvents keyboardMouseEvents;
        private MainWindow MainWindow => (MainWindow)Application.Current.MainWindow;

        // 키보드 누를떄 후킹
        private void KeyboardMouseEvents_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case System.Windows.Forms.Keys.MediaPlayPause:
                    MainWindow.MediaController_Click(MainWindow.PlayPauseButton, new RoutedEventArgs());
                    break;
                case System.Windows.Forms.Keys.MediaStop:
                    MainWindow.MediaController_Click(MainWindow.StopButton, new RoutedEventArgs());
                    break;
            }
            ((MainWindow)Application.Current.MainWindow).MediaPlayer_PlayStateChange();
        }

        // 중복 시작이 관계없는 메서드
        public void Start() // 후킹 시작
        {
            if (keyboardMouseEvents == null)
            {
                keyboardMouseEvents = Hook.GlobalEvents();
                keyboardMouseEvents.KeyDown += KeyboardMouseEvents_KeyDown;
            }
        }

        // 중복 중지가 관계없는 메서드
        public void Stop() // 후킹 중지
        {
            if (keyboardMouseEvents != null)
            {
                keyboardMouseEvents.KeyDown -= KeyboardMouseEvents_KeyDown;
                keyboardMouseEvents.Dispose();
                keyboardMouseEvents = null;
            }
        }

        public void Restart()
        {
            Stop();
            Start();
        }
    }
}
