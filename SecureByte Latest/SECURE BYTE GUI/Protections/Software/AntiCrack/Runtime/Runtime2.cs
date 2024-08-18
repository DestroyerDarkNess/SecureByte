using Inx.Ui;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Runtime
{
    internal static class AntiCracking
    {
        internal static void Init()
        {
            new Thread(DoWork)
            {
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            }.Start();
        }            
        public static void DoWork()
        {
            string x = MutationClass.Key<string>(8);
            string msg = MutationClass.Key<string>(9);
            string content = MutationClass.Key<string>(10);
            Random random = new Random();
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int length = new Random().Next(2, 25);
            while (true)
            {
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey(@"Software\SBAC");
                try
                {
                    if (rkey != null && rkey.GetValue("data").ToString() == "ban = true" || rkey.GetValue("val").ToString() != "")
                    {
                        File.WriteAllText(Directory.GetCurrentDirectory() + "\\You are banned.txt", rkey.GetValue("val").ToString());
                        Process.GetCurrentProcess().Kill();
                    }
                }
                catch { }
                string[] processName = new string[]
{
    "JetBrains",
    "dotPeek",
    "JetBrains dotPeek",
    "De4dot",
    "nopedengine-i386",
    "nopedengine-x86_64",
    "nopedengine-x86_64-SSE4-AVX2",
    "Noped Engine",
    "Game Fuq'R",
    "gamefuqr-x86_64-SSE4-AVX2",
    "gamerfuqr-i386",
    "gamerfuqr-x86_64",
                "CosMos",
                "SimpleAssemblyExplorer",
                "StringDecryptor",
                "CodeCracker",
                "x32dbg",
                "x64dbg",
                "ollydbg",
                "simpleassembly",
                "httpanalyzer",
                "httpdebug",
                "fiddler",
                "processhacker",
                "scylla_x86",
                "scylla_x64",
                "scylla",
                "IMMUNITYDEBUGGER",
                "MegaDumper",
                "reshacker",
                "cheat engine",
                "solarwinds",
                "HTTPDebuggerSvc",
                "netcheat",
                "megadumper",
                "ilspy",
                "reflector",
                "exeinfope",
                "DetectItEasy",
                "Exeinfo PE",
                "Process Hacker",
                "HTTP Debugger",
                "dnSpy",
                "Fiddler Everywhere",
                "ExtremeDumper",
                "KsDumper",
                "ollydbg",
                "HxD",
                "dumper",
                "Progress Telerik Fiddler Web Debugger",
                "dnSpy-x86",
                "cheat engine",
                "Cheat Engine",
                "cheatengine",
                "cheatengine-x86_64",
                "HTTPDebuggerUI",
                "ProcessHacker",
                "x32dbg",
                "x64dbg",
                "DotNetDataCollector32",
                "DotNetDataCollector64",
                "CFF Explorer",
                "M*3*G*4**D*u*m*p*3*R*",
                "ĘẍtŗęḿęĎựḿҏęŗ",
                "solarwinds",
                "HTTPDebuggerSvc",
                "HTTPDebuggerUI",
                "Everything",
                "FileActivityWatch",
                "netcheat"
};
                string[] eList = new string[]
{
                                "easyanticheat",
                                "eac",
                                MutationClass.Key<string>(11)
            };
                Process[] processlist = Process.GetProcesses();
                foreach (Process theprocess in processlist)
                {
                    foreach (string s in processName)
                    {
                        if (theprocess.ProcessName.ToLower().Contains(s.ToLower()))
                        {
                            foreach (string e in eList)
                            {
                                if (theprocess.ProcessName.ToLower().Contains(e.ToLower())) return;
                            }
                            int pid = theprocess.Id;
                            var proc = Process.GetProcessById(pid);
                            try
                            {
                                if (x == "0")
                                {
                                    if (msg == "1")
                                    {
                                        MessageBox.Show(content, "Warning");
                                        if (!proc.HasExited)
                                            proc.Kill();
                                        Environment.Exit(0);
                                    }
                                    else
                                    {
                                        if (!proc.HasExited)
                                            proc.Kill();
                                        Environment.Exit(0);
                                    }
                                }
                                else if (x == "1")
                                {
                                    RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\SBAC");
                                    key.SetValue("data", "ban = true");
                                    string value = new string(Enumerable.Repeat(alphabet, length)
                                           .Select(ss => ss[random.Next(ss.Length)])
                                           .ToArray());
                                    string result = new string("ISB".Select((c, i) => (char)(c ^ value[i % value.Length])).ToArray());
                                    key.SetValue("val", result);
                                    key.Close();
                                    if (msg == "1")
                                    {
                                        MessageBox.Show(content, "Warning");
                                        if (!proc.HasExited)
                                            proc.Kill();
                                        Environment.Exit(0);
                                    }
                                    else
                                    {
                                        if (!proc.HasExited)
                                            proc.Kill();
                                        Environment.Exit(0);
                                    }
                                }
                            }
                            catch
                            {
                                Environment.Exit(0);
                            }
                        }
                    }
                }
                Thread.Sleep(3000);
            }
        }
    }
}
