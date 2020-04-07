using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace CustomMediaPlayer.Utility
{
    class RegistryEditer
    {
        public static void FileConnect()
        {
            RegistryKey registry = Registry.ClassesRoot.CreateSubKey(@"Applications\CustomMediaPlayer.exe\shell\open\command");
            registry.SetValue("", "\"" + Assembly.GetEntryAssembly().Location + "\"" + " \"%1\"", RegistryValueKind.String);
        }

        public static void FileDisconnect()
        {
            Registry.ClassesRoot.DeleteSubKeyTree(@"Applications\CustomMediaPlayer.exe");
        }
    }
}
