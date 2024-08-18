using Guna.UI2.WinForms;
using System;

namespace SecureByte_Unban_tool
{
    internal class Logger
    {
        public static void AppendToLog(string format, Guna2TextBox guna2TextBox)
        {
            guna2TextBox.AppendText(string.Format(format) + Environment.NewLine);
        }
    }
}
