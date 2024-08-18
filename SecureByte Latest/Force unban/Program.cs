using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Force_unban
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                string keyPath64Bit = "SOFTWARE\\SBAC";
                RegistryKey cu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                RegistryKey key64Bit = cu.OpenSubKey(keyPath64Bit, true);
                if (key64Bit != null)
                {
                    var namesArray = key64Bit.GetValueNames();
                    foreach (string valueName in namesArray)
                    {
                        key64Bit.DeleteValue(valueName);
                    }
                }
            }
            else
            {
                string keyPath32Bit = "SOFTWARE\\SBAC";
                RegistryKey cuu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
                RegistryKey key32Bit = cuu.OpenSubKey(keyPath32Bit, true);
                if (key32Bit != null)
                {
                    var namesArray = key32Bit.GetValueNames();
                    foreach (string valueName in namesArray)
                    {
                        key32Bit.DeleteValue(valueName);
                    }
                }
            }
            var exepath = Assembly.GetEntryAssembly().Location;
            var info = new ProcessStartInfo("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del \"" + exepath + "\"")
            {
                WindowStyle = ProcessWindowStyle.Hidden
            };
            Process.Start(info).Dispose();
            Environment.Exit(0);
        }
    }
}
