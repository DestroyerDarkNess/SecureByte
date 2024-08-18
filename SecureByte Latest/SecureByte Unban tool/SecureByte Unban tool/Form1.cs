using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SecureByte_Unban_tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Opacity = 0.98;
            guna2ShadowForm1.SetShadowForm(this);
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Inx.Ui.Movement.Move(this, e);
        }
        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            guna2TextBox1.Clear();
            string keyPath = "SOFTWARE\\SBAC";
            if (Environment.Is64BitOperatingSystem)
            {
                RegistryKey cu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                RegistryKey key64Bit = cu.OpenSubKey(keyPath, true);
                if (key64Bit != null)
                {
                    RegistryKey rkey = Registry.CurrentUser.OpenSubKey(@"Software\SBAC");
                    if (rkey.ValueCount == 0)
                    {
                        Logger.AppendToLog("  You're not banned !", guna2TextBox1);
                        return;
                    }
                    foreach (string valueName in rkey.GetValueNames())
                    {                      
                        if (valueName == "val")
                        {
                            string value = rkey.GetValue(valueName).ToString();
                            var decodedValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(new string("ISB".Select((c, i) => (char)(c ^ value[i % value.Length])).ToArray())));
                            if (string.IsNullOrEmpty(guna2TextBox2.Text) || decodedValue != guna2TextBox2.Text)
                            {
                                Logger.AppendToLog("  Wrong value ;)", guna2TextBox1);
                                return;
                            }
                            else
                                timer1.Start();
                        }
                    }
                }
            }         
            else
            {
                RegistryKey cu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
                RegistryKey key32Bit = cu.OpenSubKey(keyPath, true);
                if (key32Bit != null)
                {
                    RegistryKey rkey = Registry.CurrentUser.OpenSubKey(@"Software\SBAC");
                    if (rkey.ValueCount == 0)
                    {
                        Logger.AppendToLog("  You're not banned !", guna2TextBox1);
                        return;
                    }
                    foreach (string valueName in rkey.GetValueNames())
                    {
                        if (valueName == "val")
                        {
                            string value = rkey.GetValue(valueName).ToString();
                            var decodedValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(new string("ISB".Select((c, i) => (char)(c ^ value[i % value.Length])).ToArray())));
                            if (string.IsNullOrEmpty(guna2TextBox2.Text) || decodedValue != guna2TextBox2.Text)
                            {
                                Logger.AppendToLog("  Wrong value ;)", guna2TextBox1);
                                return;
                            }
                            else
                                timer1.Start();
                        }
                    }
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            string keyPath = "SOFTWARE\\SBAC";
            RegistryKey cuu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
            RegistryKey key32Bit = cuu.OpenSubKey(keyPath, true);
            RegistryKey cu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            RegistryKey key64Bit = cu.OpenSubKey(keyPath, true);
            guna2ProgressBar1.Increment(5);
            switch(guna2ProgressBar1.Value)
            {
                case 5:
                    Logger.AppendToLog("  Processing ....", guna2TextBox1);
                    break;
                case 25:                   
                    if (key32Bit != null)
                    {
                        var namesArray = key32Bit.GetValueNames();
                        foreach (string valueName in namesArray)
                        {
                            if (valueName == "data")
                                key32Bit.DeleteValue(valueName);
                            Logger.AppendToLog("  Step 1 done !", guna2TextBox1);
                        }
                    }               
                    if (key64Bit != null)
                    {
                        var namesArray = key64Bit.GetValueNames();
                        foreach (string valueName in namesArray)
                        {
                            if (valueName == "data")
                                key64Bit.DeleteValue(valueName);
                            Logger.AppendToLog("  Step 1 done !", guna2TextBox1);
                        }
                    }
                    break;
                case 75:
                    if (key32Bit != null)
                    {
                        var namesArray = key32Bit.GetValueNames();
                        foreach (string valueName in namesArray)
                        {
                            if (valueName == "val")
                                key32Bit.DeleteValue(valueName);
                            Logger.AppendToLog("  Step 2 done !", guna2TextBox1);
                        }
                    }
                    if (key64Bit != null)
                    {
                        var namesArray = key64Bit.GetValueNames();
                        foreach (string valueName in namesArray)
                        {
                            if (valueName == "val")
                                key64Bit.DeleteValue(valueName);
                            Logger.AppendToLog("  Step 2 done !", guna2TextBox1);
                        }
                    }
                    break;
                case 100:
                    Logger.AppendToLog("  Finalizing....", guna2TextBox1);
                    timer1.Stop();
                    timer2.Start();
                    break;
            }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            Application.Exit();
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start("cmd.exe", "/c ping 127.0.0.1 -n 0 > nul & del \"" + exePath + "\"");
            Environment.Exit(0);
        }
    }
}
