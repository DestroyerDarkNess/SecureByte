using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace Protections.Runtime
{
    [SecuritySafeCritical]
    internal static class AntiHook
    {
        #region Functions
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
        [DllImport("kernel32.dll")]
        public static extern bool IsDebuggerPresent();
        static bool IsHooked_IsDebuggerPresent()
        {
            IntPtr kernel32 = LoadLibrary("kernel32.dll");
            IntPtr IsDebuggerPresentAddr = GetProcAddress(kernel32, "IsDebuggerPresent");

            byte[] data = new byte[2];
            Marshal.Copy(IsDebuggerPresentAddr, data, 0, 2);

            if (Environment.Is64BitProcess)
            {
                if ((data[0] != 0x48 || data[1] != 0xFF))
                    return true;
            }
            else
            {
                if ((data[0] != 0xFF || data[1] != 0x25))
                    return true;
            }

            return false;
        }
        #endregion
        public static void Initialize()
        {
            bool isDebuggerPresentHooked = IsHooked_IsDebuggerPresent();
            if (isDebuggerPresentHooked)
                Process.GetCurrentProcess().Kill();
        }
    }
}