using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using IL_Emulator_Dynamic;
using Runtime;
using VMExample.Instructions;

namespace ConversionBack
{
    public class Class
    {
        public static OpCode[] oneByteOpCodes;
        public static OpCode[] twoByteOpCodes;
        public static StackTrace stackTrace;
        public static System.Reflection.Module callingModule;
        public static byte[] byteArrayResource;
        public static byte[] byteArrayResource2;
        public static a bc;
        #region Decompress
        public const int QLZ_VERSION_MAJOR = 1;
        public const int QLZ_VERSION_MINOR = 5;
        public const int QLZ_VERSION_REVISION = 0;
        public const int QLZ_STREAMING_BUFFER = 0;
        public const int QLZ_MEMORY_SAFE = 0;
        private const int HASH_VALUES = 4096;
        private const int UNCONDITIONAL_MATCHLEN = 6;
        private const int UNCOMPRESSED_END = 4;
        private const int CWORD_LEN = 4;
        private static int HeaderLen(byte[] source)
        {
            return ((source[0] & 2) == 2) ? 9 : 3;
        }
        private static int SizeDecompressed(byte[] source)
        {
            if (HeaderLen(source) == 9)
                return source[5] | (source[6] << 8) | (source[7] << 16) | (source[8] << 24);
            else
                return source[2];
        }
        private static byte[] Decompress(byte[] source)
        {
            int level;
            int size = SizeDecompressed(source);
            int src = HeaderLen(source);
            int dst = 0;
            uint cword_val = 1;
            byte[] destination = new byte[size];
            int[] hashtable = new int[4096];
            byte[] hash_counter = new byte[4096];
            int last_matchstart = size - UNCONDITIONAL_MATCHLEN - UNCOMPRESSED_END - 1;
            int last_hashed = -1;
            int hash;
            uint fetch = 0;
            level = (source[0] >> 2) & 0x3;
            if (level != 1 && level != 3)
                throw new ArgumentException("C# version only supports level 1 and 3");
            if ((source[0] & 1) != 1)
            {
                byte[] d2 = new byte[size];
                Array.Copy(source, HeaderLen(source), d2, 0, size);
                return d2;
            }
            for (; ; )
            {
                if (cword_val == 1)
                {
                    cword_val = (uint)(source[src] | (source[src + 1] << 8) | (source[src + 2] << 16) | (source[src + 3] << 24));
                    src += 4;
                    if (dst <= last_matchstart)
                    {
                        if (level == 1)
                            fetch = (uint)(source[src] | (source[src + 1] << 8) | (source[src + 2] << 16));
                        else
                            fetch = (uint)(source[src] | (source[src + 1] << 8) | (source[src + 2] << 16) | (source[src + 3] << 24));
                    }
                }
                if ((cword_val & 1) == 1)
                {
                    uint matchlen;
                    uint offset2;
                    cword_val = cword_val >> 1;
                    if (level == 1)
                    {
                        hash = ((int)fetch >> 4) & 0xfff;
                        offset2 = (uint)hashtable[hash];
                        if ((fetch & 0xf) != 0)
                        {
                            matchlen = (fetch & 0xf) + 2;
                            src += 2;
                        }
                        else
                        {
                            matchlen = source[src + 2];
                            src += 3;
                        }
                    }
                    else
                    {
                        uint offset;
                        if ((fetch & 3) == 0)
                        {
                            offset = (fetch & 0xff) >> 2;
                            matchlen = 3;
                            src++;
                        }
                        else if ((fetch & 2) == 0)
                        {
                            offset = (fetch & 0xffff) >> 2;
                            matchlen = 3;
                            src += 2;
                        }
                        else if ((fetch & 1) == 0)
                        {
                            offset = (fetch & 0xffff) >> 6;
                            matchlen = ((fetch >> 2) & 15) + 3;
                            src += 2;
                        }
                        else if ((fetch & 127) != 3)
                        {
                            offset = (fetch >> 7) & 0x1ffff;
                            matchlen = ((fetch >> 2) & 0x1f) + 2;
                            src += 3;
                        }
                        else
                        {
                            offset = (fetch >> 15);
                            matchlen = ((fetch >> 7) & 255) + 3;
                            src += 4;
                        }
                        offset2 = (uint)(dst - offset);
                    }
                    destination[dst + 0] = destination[offset2 + 0];
                    destination[dst + 1] = destination[offset2 + 1];
                    destination[dst + 2] = destination[offset2 + 2];
                    for (int i = 3; i < matchlen; i += 1)
                    {
                        destination[dst + i] = destination[offset2 + i];
                    }
                    dst += (int)matchlen;
                    if (level == 1)
                    {
                        fetch = (uint)(destination[last_hashed + 1] | (destination[last_hashed + 2] << 8) | (destination[last_hashed + 3] << 16));
                        while (last_hashed < dst - matchlen)
                        {
                            last_hashed++;
                            hash = (int)(((fetch >> 12) ^ fetch) & (HASH_VALUES - 1));
                            hashtable[hash] = last_hashed;
                            hash_counter[hash] = 1;
                            fetch = (uint)(fetch >> 8 & 0xffff | destination[last_hashed + 3] << 16);
                        }
                        fetch = (uint)(source[src] | (source[src + 1] << 8) | (source[src + 2] << 16));
                    }
                    else
                    {
                        fetch = (uint)(source[src] | (source[src + 1] << 8) | (source[src + 2] << 16) | (source[src + 3] << 24));
                    }
                    last_hashed = dst - 1;
                }
                else
                {
                    if (dst <= last_matchstart)
                    {
                        destination[dst] = source[src];
                        dst += 1;
                        src += 1;
                        cword_val = cword_val >> 1;
                        if (level == 1)
                        {
                            while (last_hashed < dst - 3)
                            {
                                last_hashed++;
                                int fetch2 = destination[last_hashed] | (destination[last_hashed + 1] << 8) | (destination[last_hashed + 2] << 16);
                                hash = ((fetch2 >> 12) ^ fetch2) & (HASH_VALUES - 1);
                                hashtable[hash] = last_hashed;
                                hash_counter[hash] = 1;
                            }
                            fetch = (uint)(fetch >> 8 & 0xffff | source[src + 2] << 16);
                        }
                        else
                        {
                            fetch = (uint)(fetch >> 8 & 0xffff | source[src + 2] << 16 | source[src + 3] << 24);
                        }
                    }
                    else
                    {
                        while (dst <= size - 1)
                        {
                            if (cword_val == 1)
                            {
                                src += CWORD_LEN;
                                cword_val = 0x80000000;
                            }
                            destination[dst] = source[src];
                            dst++;
                            src++;
                            cword_val = cword_val >> 1;
                        }
                        return destination;
                    }
                }
            }
        }
        #endregion
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress", ExactSpelling = true)]
        private static extern IntPtr e(IntPtr intptr, string str);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "GetModuleHandle")]
        private static extern IntPtr ab(string str);
        public delegate void a(byte[] bytes, int len, byte[] key, int keylen);
        public static void Init(string resName)
        {
            callingModule = new StackTrace().GetFrame(1).GetMethod().Module;
            byteArrayResource = extractResource(resName);
            byte[] tester = extractResource(Xor.Xoring("A"));
            byte[] dTester = Decompress(tester);
            VMExample.Instructions.All.binr = new BinaryReader(new MemoryStream(dTester));
            VMExample.Instructions.All.val = new ValueStack();
            VMExample.Instructions.All.val.parameters = new object[1];
            All.val.parameters[0] = byteArrayResource;
            All.val.locals = new object[10];
            VMExample.Instructions.All.run();
            IntPtr abb;
            IntPtr def;
            if (IntPtr.Size == 4)
            {
                byte[] tester2 = extractResource(Xor.Xoring("B"));
                byte[] dtester2 = Decompress(tester2);
                EmbeddedDllClass.ExtractEmbeddedDlls("sByte.dll", dtester2);
                abb = EmbeddedDllClass.LoadDll("sByte.dll");
                def = e(abb, "_a@16");
            }
            else
            {
                byte[] tester2 = extractResource(Xor.Xoring("C"));
                byte[] dtester2 = Decompress(tester2);
                EmbeddedDllClass.ExtractEmbeddedDlls("sByte.dll", dtester2);
                abb = EmbeddedDllClass.LoadDll("sByte.dll");
                def = e(abb, "a");
            }



            //a(x,x,x,x) 0000000010001070 1

            bc = (a)Marshal.GetDelegateForFunctionPointer(def, typeof(a));
            byteArrayResource = (byte[])All.val.locals[1];
            //process all opcodes into fields so that they relate to the way i process them in the conversion to method
            var array = new OpCode[256];
            var array2 = new OpCode[256];
            oneByteOpCodes = array;
            twoByteOpCodes = array2;
            var typeFromHandle = typeof(OpCode);
            var typeFromHandle2 = typeof(OpCodes);
            foreach (var fieldInfo in typeFromHandle2.GetFields())
                if (fieldInfo.FieldType == typeFromHandle)
                {
                    var opCode = (OpCode)fieldInfo.GetValue(null);
                    var num = (ushort)opCode.Value;
                    if (opCode.Size == 1)
                    {
                        var b = (byte)num;
                        oneByteOpCodes[b] = opCode;
                    }
                    else
                    {
                        var b2 = (byte)(num | 65024);
                        twoByteOpCodes[b2] = opCode;
                    }
                }
        }


        private static byte[] extractResource(string resourceName)
        {
            using (Stream stream = callingModule.Assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                byte[] array = new byte[stream.Length];
                stream.Read(array, 0, array.Length);
                return array;
            }
        }
    }
}
