using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace authguard
{
    public class authHelpers
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        private static extern int SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        private static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        private static extern int CloseHandle(IntPtr hObject);
        [Flags]
        private enum ThreadAccess : int
        {
            SUSPEND_RESUME = 0x0002
        }
        public static void SuspendProcess(int processId)
        {
            try
            {
                Process process = Process.GetProcessById(processId);
                foreach (ProcessThread thread in process.Threads)
                {
                    IntPtr hThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                    if (hThread != IntPtr.Zero)
                    {
                        SuspendThread(hThread);
                        CloseHandle(hThread);
                    }
                }
            }
            catch { }
        }
    }
}
