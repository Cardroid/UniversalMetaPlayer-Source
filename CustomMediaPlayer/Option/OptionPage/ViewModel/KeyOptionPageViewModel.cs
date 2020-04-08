using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMediaPlayer.Option.OptionPage.ViewModel
{
    public class KeyOptionPageViewModel
    {
        private bool keyhookoption;
        public bool KeyHookOption
        {
            get { return keyhookoption; }
            set 
            {
                keyhookoption = value;
                if (keyhookoption)
                    OptionWindow.hooking.Start();
                else
                    OptionWindow.hooking.Stop();
            }
        }
    }
}
