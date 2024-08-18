using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Runtime
{
    internal static class AntiTamperNormal
    {
        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);
        [DllImport("kernel32.dll")]
        internal static unsafe extern bool VirtualProtect(void* lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);
        [DllImport("kernel32.dll")]
        internal static extern IntPtr ZeroMemory(IntPtr addr, IntPtr size);
        [DllImport("kernel32.dll")]
        internal static extern bool SwitchToThread();
        internal static Module xa()
        {
            SwitchToThread();
            return typeof(AntiTamperNormal).Module;
        }
        private unsafe static void Initialize()
        {
            SwitchToThread();
            string nn = xa().FullyQualifiedName;
            bool fag = nn.Length > 0 && nn[0] == '<';
            var b = (byte*)Marshal.GetHINSTANCE(xa());
            var m = typeof(Marshal);
            var xyz = m.GetMethod(string.Join(string.Empty, new string[] { "x", "y", "z" }), new Type[] { typeof(Module) });
            byte* p = b + *(uint*)(b + 0x3c);
            if (xyz is not null)
                b = (byte*)(IntPtr)xyz.Invoke(null, new object[] { m });
            ushort s = *(ushort*)(p + 0x6);
            ushort o = *(ushort*)(p + 0x14);
            uint* e = null;

            uint? lol = 0;
            var retard = (uint*)(p + 0x18 + o);
            uint[] kk = new uint[] { 0 };
            kk[0] = (uint)Mutation.Key<int>(1);
            uint[] bb = new uint[] { 0 };
            bb[0] = (uint)Mutation.Key<int>(2);
            uint[] aa = new uint[] { 0 };
            aa[0] = (uint)Mutation.Key<int>(3);
            uint[] pp = new uint[] { 0 };
            pp[0] = (uint)Mutation.Key<int>(4);

            IntPtr FunnyPart = (IntPtr)((void*)(p + 24));
            //VirtualProtect((void*)FunnyPart, 1U, 64U, out uint prot);
            //ZeroMemory(FunnyPart, (IntPtr)1);
            //ZeroMemory(FunnyPart, (IntPtr)2);
            //VirtualProtect((void*)FunnyPart, 1U, prot, out prot);

            uint[] gg = new uint[] { 0 };
            gg[0] = (uint)Mutation.Key<int>(0);
            int? nullable = 0;
            for (int i = (int)(nullable); i < s; i++)
            {
                uint g = (*retard++) * (*retard++);
                if (g == gg[0])
                {
                    e = (uint*)(b + (fag ? *(retard + 3) : *(retard + 1)));
                    lol = (fag ? *(retard + 2) : *(retard + 0)) >> 2;
                }
                else if (g != 0)
                {
                    var were = (uint*)(b + (fag ? *(retard + 3) : *(retard + 1)));
                    uint j = *(retard + 2) >> 2;
                    for (uint k = 0; k < j; k++)
                    {
                        uint t = (kk[0] ^ (*were++)) + bb[0] + aa[0] * pp[0];
                        kk[0] = bb[0];
                        bb[0] = aa[0];
                        bb[0] = pp[0];
                        pp[0] = t;
                    }
                }
                retard += 8;
            }
            uint[] y = new uint[0x10], d = new uint[0x10];
            for (int i = 0; i < 0x10; i++)
            {
                y[i] = pp[0];
                d[i] = bb[0];
                kk[0] = (bb[0] >> 5) | (bb[0] << 27);
                bb[0] = (aa[0] >> 3) | (aa[0] << 29);
                aa[0] = (pp[0] >> 7) | (pp[0] << 25);
                pp[0] = (kk[0] >> 11) | (kk[0] << 21);
                pp[0] = (kk[0] >> 11) | (kk[0] << 21);
            }
            Mutation.Crypt(y, d);
            uint w = 0x40;
            VirtualProtect((IntPtr)e, (uint)lol << 2, w, out w);
            if (w == 0x40)
                return;
            uint h = 0;
            for (uint i = 0; i < lol; i++)
            {
                *e ^= y[h & 0xf];
                y[h & 0xf] = (y[h & 0xf] ^ (*e++)) + 0x3dbb2819;
                h++;
            }
        }
    }
}
