using Guna.UI2.WinForms;
using System;

namespace SECURE_BYTE_GUI.Logger
{
    public static class Logger
    {
        static DateTime begin;
        public static void AppendToLog(string format, Guna2TextBox guna2TextBox)
        {
            guna2TextBox.BeginInvoke((Action)(() =>
            {
                guna2TextBox.AppendText(string.Format(format) + Environment.NewLine);
            }));
          
        }
        public static void AppendToLogF(string format, Guna2TextBox guna2TextBox)
        {
            guna2TextBox.BeginInvoke((Action)(() =>
            {
                guna2TextBox.AppendText(string.Format(format));
            }));
          
        }
        public static void Starting(Guna2TextBox guna2TextBox)
        {
            guna2TextBox.Clear();
            begin = DateTime.Now;
            AppendToLog("[ Info ] started obfuscation process at : " + begin, guna2TextBox);
        }
        public static void Finish(Guna2TextBox guna2TextBox)
        {
            DateTime now = DateTime.Now;
            string timeString = string.Format(
                "at {0}, {1}:{2:d2} elapsed.",
                now.ToShortTimeString(),
                (int)now.Subtract(begin).TotalMinutes,
                now.Subtract(begin).Seconds);
            AppendToLogF("[Info] Obfuscation Completed ! : " + timeString, guna2TextBox);
        }
    }
}
