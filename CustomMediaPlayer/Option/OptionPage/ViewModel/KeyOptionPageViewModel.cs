using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMediaPlayer.Option.OptionPage.ViewModel
{
    public class KeyOptionPageViewModel
    {
        public bool KeyHookOption
        {
            get { return MainWindow.Optioncore.KeyHookOption; }
            set { MainWindow.Optioncore.KeyHookOption = value; }
        }
    }
}
