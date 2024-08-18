using dnlib.DotNet;
using EmbedLibrary.Core.Library;
using System;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using CallingConvention = System.Runtime.InteropServices.CallingConvention;

namespace CrtAsmOfEmbeded_x86
{
    static partial class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtect(byte[] lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallWindowProc(byte[] lpPrevWndFunc, int hWnd, int Msg, int wParam, int lParam);

        private const int NULL = 0;
        private const int PAGE_EXECUTE_READWRITE = 64;
    }
    static partial class Program
    {
        private static readonly byte[] buf_asm = { 85, 139, 236, 129, 236, 192, 0, 0, 0, 83, 86, 87, 141, 189, 64, 255, 255, 255, 185, 48, 0, 0, 0, 184, 204, 204, 204, 204, 243, 171, 184, 0, 0, 0, 0, 51, 210, 15, 162, 137, 85, 252, 137, 69, 248, 184, 1, 0, 0, 0, 51, 201, 51, 210, 15, 162, 137, 85, 244, 137, 69, 240, 139, 69, 252, 137, 69, 236, 139, 69, 248, 137, 69, 232, 139, 69, 244, 137, 69, 228, 139, 69, 240, 137, 69, 224, 141, 69, 236, 95, 94, 91, 139, 229, 93, 195 };
    }
    static partial class Program
    {
        private static void VirtualProtect(byte[] address)
        {
            uint lpflOldProtect;
            VirtualProtect(address, address.Length, PAGE_EXECUTE_READWRITE, out lpflOldProtect);
        }
    }
    static partial class Program
    {
        public static string GetCPUID()
        {
            VirtualProtect(buf_asm);
            IntPtr ptr = CallWindowProc(buf_asm, NULL, NULL, NULL, NULL);
            int s1 = Marshal.ReadInt32(ptr);
            int s2 = Marshal.ReadInt32(ptr, 4);
            int s3 = Marshal.ReadInt32(ptr, 8);
            int s4 = Marshal.ReadInt32(ptr, 12);
            return s1.ToString("X") + s2.ToString("X") + s3.ToString("X") + s4.ToString("X");
        }
    }
}