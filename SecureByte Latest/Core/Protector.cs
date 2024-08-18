﻿using System;
using System.Linq;
using System.Reflection;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using Core.Properties;
using System.Collections.Generic;

namespace Core
{
    public class Protector
    {
        #region Compress
        public const int QLZ_VERSION_MAJOR = 1;
        public const int QLZ_VERSION_MINOR = 5;
        public const int QLZ_VERSION_REVISION = 0;
        public const int QLZ_STREAMING_BUFFER = 0;
        public const int QLZ_MEMORY_SAFE = 0;
        private const int HASH_VALUES = 4096;
        private const int MINOFFSET = 2;
        private const int UNCONDITIONAL_MATCHLEN = 6;
        private const int UNCOMPRESSED_END = 4;
        private const int CWORD_LEN = 4;
        private const int DEFAULT_HEADERLEN = 9;
        private const int QLZ_POINTERS_1 = 1;
        private const int QLZ_POINTERS_3 = 16;
        private static int HeaderLen(byte[] source)
        {
            return ((source[0] & 2) == 2) ? 9 : 3;
        }
        public static int SizeCompressed(byte[] source)
        {
            if (HeaderLen(source) == 9)
                return source[1] | (source[2] << 8) | (source[3] << 16) | (source[4] << 24);
            else
                return source[1];
        }
        private static void Write_header(byte[] dst, int level, bool compressible, int size_compressed, int size_decompressed)
        {
            dst[0] = (byte)(2 | (compressible ? 1 : 0));
            dst[0] |= (byte)(level << 2);
            dst[0] |= (1 << 6);
            dst[0] |= (0 << 4);
            Fast_write(dst, 1, size_decompressed, 4);
            Fast_write(dst, 5, size_compressed, 4);
        }
        public static byte[] Compress(byte[] source)
        {
            int level = 3;
            int src = 0;
            int dst = DEFAULT_HEADERLEN + CWORD_LEN;
            uint cword_val = 0x80000000;
            int cword_ptr = DEFAULT_HEADERLEN;
            byte[] destination = new byte[source.Length + 400];
            int[,] hashtable;
            int[] cachetable = new int[HASH_VALUES];
            byte[] hash_counter = new byte[HASH_VALUES];
            byte[] d2;
            int fetch = 0;
            int last_matchstart = (source.Length - UNCONDITIONAL_MATCHLEN - UNCOMPRESSED_END - 1);
            int lits = 0;

            if (level != 1 && level != 3)
                throw new ArgumentException("C# version only supports level 1 and 3");

            if (level == 1)
                hashtable = new int[HASH_VALUES, QLZ_POINTERS_1];
            else
                hashtable = new int[HASH_VALUES, QLZ_POINTERS_3];

            if (source.Length == 0)
                return new byte[0];

            if (src <= last_matchstart)
                fetch = source[src] | (source[src + 1] << 8) | (source[src + 2] << 16);

            while (src <= last_matchstart)
            {
                if ((cword_val & 1) == 1)
                {
                    if (src > source.Length >> 1 && dst > src - (src >> 5))
                    {
                        d2 = new byte[source.Length + DEFAULT_HEADERLEN];
                        Write_header(d2, level, false, source.Length, source.Length + DEFAULT_HEADERLEN);
                        System.Array.Copy(source, 0, d2, DEFAULT_HEADERLEN, source.Length);
                        return d2;
                    }

                    Fast_write(destination, cword_ptr, (int)((cword_val >> 1) | 0x80000000), 4);
                    cword_ptr = dst;
                    dst += CWORD_LEN;
                    cword_val = 0x80000000;
                }

                if (level == 1)
                {
                    int hash = ((fetch >> 12) ^ fetch) & (HASH_VALUES - 1);
                    int o = hashtable[hash, 0];
                    int cache = cachetable[hash] ^ fetch;
                    cachetable[hash] = fetch;
                    hashtable[hash, 0] = src;

                    if (cache == 0 && hash_counter[hash] != 0 && (src - o > MINOFFSET || (src == o + 1 && lits >= 3 && src > 3 && source[src] == source[src - 3] && source[src] == source[src - 2] && source[src] == source[src - 1] && source[src] == source[src + 1] && source[src] == source[src + 2])))
                    {
                        cword_val = ((cword_val >> 1) | 0x80000000);
                        if (source[o + 3] != source[src + 3])
                        {
                            int f = 3 - 2 | (hash << 4);
                            destination[dst + 0] = (byte)(f >> 0 * 8);
                            destination[dst + 1] = (byte)(f >> 1 * 8);
                            src += 3;
                            dst += 2;
                        }
                        else
                        {
                            int old_src = src;
                            int remaining = ((source.Length - UNCOMPRESSED_END - src + 1 - 1) > 255 ? 255 : (source.Length - UNCOMPRESSED_END - src + 1 - 1));

                            src += 4;
                            if (source[o + src - old_src] == source[src])
                            {
                                src++;
                                if (source[o + src - old_src] == source[src])
                                {
                                    src++;
                                    while (source[o + (src - old_src)] == source[src] && (src - old_src) < remaining)
                                        src++;
                                }
                            }

                            int matchlen = src - old_src;

                            hash <<= 4;
                            if (matchlen < 18)
                            {
                                int f = (hash | (matchlen - 2));
                                destination[dst + 0] = (byte)(f >> 0 * 8);
                                destination[dst + 1] = (byte)(f >> 1 * 8);
                                dst += 2;
                            }
                            else
                            {
                                Fast_write(destination, dst, hash | (matchlen << 16), 3);
                                dst += 3;
                            }
                        }
                        fetch = source[src] | (source[src + 1] << 8) | (source[src + 2] << 16);
                        lits = 0;
                    }
                    else
                    {
                        lits++;
                        hash_counter[hash] = 1;
                        destination[dst] = source[src];
                        cword_val = (cword_val >> 1);
                        src++;
                        dst++;
                        fetch = ((fetch >> 8) & 0xffff) | (source[src + 2] << 16);
                    }

                }
                else
                {
                    fetch = source[src] | (source[src + 1] << 8) | (source[src + 2] << 16);

                    int o, offset2;
                    int matchlen, k, m, best_k = 0;
                    byte c;
                    int remaining = ((source.Length - UNCOMPRESSED_END - src + 1 - 1) > 255 ? 255 : (source.Length - UNCOMPRESSED_END - src + 1 - 1));
                    int hash = ((fetch >> 12) ^ fetch) & (HASH_VALUES - 1);

                    c = hash_counter[hash];
                    matchlen = 0;
                    offset2 = 0;
                    for (k = 0; k < QLZ_POINTERS_3 && c > k; k++)
                    {
                        o = hashtable[hash, k];
                        if ((byte)fetch == source[o] && (byte)(fetch >> 8) == source[o + 1] && (byte)(fetch >> 16) == source[o + 2] && o < src - MINOFFSET)
                        {
                            m = 3;
                            while (source[o + m] == source[src + m] && m < remaining)
                                m++;
                            if ((m > matchlen) || (m == matchlen && o > offset2))
                            {
                                offset2 = o;
                                matchlen = m;
                                best_k = k;
                            }
                        }
                    }
                    o = offset2;
                    hashtable[hash, c & (QLZ_POINTERS_3 - 1)] = src;
                    c++;
                    hash_counter[hash] = c;

                    if (matchlen >= 3 && src - o < 131071)
                    {
                        int offset = src - o;

                        for (int u = 1; u < matchlen; u++)
                        {
                            fetch = source[src + u] | (source[src + u + 1] << 8) | (source[src + u + 2] << 16);
                            hash = ((fetch >> 12) ^ fetch) & (HASH_VALUES - 1);
                            c = hash_counter[hash]++;
                            hashtable[hash, c & (QLZ_POINTERS_3 - 1)] = src + u;
                        }

                        src += matchlen;
                        cword_val = ((cword_val >> 1) | 0x80000000);

                        if (matchlen == 3 && offset <= 63)
                        {
                            Fast_write(destination, dst, offset << 2, 1);
                            dst++;
                        }
                        else if (matchlen == 3 && offset <= 16383)
                        {
                            Fast_write(destination, dst, (offset << 2) | 1, 2);
                            dst += 2;
                        }
                        else if (matchlen <= 18 && offset <= 1023)
                        {
                            Fast_write(destination, dst, ((matchlen - 3) << 2) | (offset << 6) | 2, 2);
                            dst += 2;
                        }
                        else if (matchlen <= 33)
                        {
                            Fast_write(destination, dst, ((matchlen - 2) << 2) | (offset << 7) | 3, 3);
                            dst += 3;
                        }
                        else
                        {
                            Fast_write(destination, dst, ((matchlen - 3) << 7) | (offset << 15) | 3, 4);
                            dst += 4;
                        }
                        lits = 0;
                    }
                    else
                    {
                        destination[dst] = source[src];
                        cword_val = (cword_val >> 1);
                        src++;
                        dst++;
                    }
                }
            }
            while (src <= source.Length - 1)
            {
                if ((cword_val & 1) == 1)
                {
                    Fast_write(destination, cword_ptr, (int)((cword_val >> 1) | 0x80000000), 4);
                    cword_ptr = dst;
                    dst += CWORD_LEN;
                    cword_val = 0x80000000;
                }

                destination[dst] = source[src];
                src++;
                dst++;
                cword_val = (cword_val >> 1);
            }
            while ((cword_val & 1) != 1)
            {
                cword_val = (cword_val >> 1);
            }
            Fast_write(destination, cword_ptr, (int)((cword_val >> 1) | 0x80000000), CWORD_LEN);
            Write_header(destination, level, true, source.Length, dst);
            d2 = new byte[dst];
            System.Array.Copy(destination, d2, dst);
            return d2;
        }
        private static void Fast_write(byte[] a, int i, int value, int numbytes)
        {
            for (int j = 0; j < numbytes; j++)
                a[i + j] = (byte)(value >> (j * 8));
        }
        #endregion
        public static MethodDef setupMDT = null;
        public static List<MethodDef> usedMethods = new List<MethodDef>();
        public static List<string> usedMethodsFullNames = new List<string>();
        public static string path2;
        public static string resName = "";
        public static ModuleDefMD moduleDefMD { get; private set; }
        public static string name { get; private set; }

     


        public static MemoryStream Protect(ModuleDefMD assemblyData)
        {
            foreach (var item in usedMethods)
            {
                usedMethodsFullNames.Add(item.FullName);
            }
            //("test1");
            name =  ByteEncryption.XorClass.xIT("HYDRA");
            Protection.MethodProccesor.AllMethods.Clear();
            moduleDefMD = assemblyData; // ModuleDefMD.Load(assemblyData);
            asmRefAdder(); //this will resolve references (dlls) such as mscorlib and any dlls the unprotected binary may use. this will be to make sure resolving methods/types/fields in another assembly can be correctly identified
            //("test2");
            Protection.MethodProccesor.ModuleProcessor(); //this will process the module
            //("test3");
            //
            var nativePath = Resources.NativeEncoderx86;
            byte[] nativePathcompressed = Compress(nativePath);
            EmbeddedResource emv = new EmbeddedResource(ByteEncryption.XorClass.xIT("B"), (nativePathcompressed));
            moduleDefMD.Resources.Add(emv);
            //("test4");
            //
            var NativeEncoderx64 = Resources.NativeEncoderx64;
            byte[] NativeEncoderx64Compressed = Compress(NativeEncoderx64);
            EmbeddedResource emv64 = new EmbeddedResource(ByteEncryption.XorClass.xIT("C"), (NativeEncoderx64Compressed));
            moduleDefMD.Resources.Add(emv64);
            //
            //("test5");
            byte[] cleanConversion = File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Runtime.dll"));
            byte[] compressedcleanConversion = Compress(cleanConversion);
            EmbeddedResource embc = new EmbeddedResource(" ", compressedcleanConversion); //Full
            moduleDefMD.Resources.Add(embc);
            //("test6");
            //
            var XorMethod = Resources.XorMethod;
            byte[] compressedXorMethod = Compress(XorMethod);
            EmbeddedResource emb = new EmbeddedResource(ByteEncryption.XorClass.xIT("A"), compressedXorMethod); //XorMethod
            moduleDefMD.Resources.Add(emb);
            //("test7");
            //
            voidMover.Execute(moduleDefMD, setupMDT);
            //("test8");
            //
            /* Writing */
            ModuleWriterOptions modOpts = new ModuleWriterOptions(moduleDefMD);
            modOpts.MetadataOptions.Flags = MetadataFlags.PreserveAll;
            modOpts.Cor20HeaderOptions.Flags = dnlib.DotNet.MD.ComImageFlags.ILOnly;
            modOpts.MetadataLogger = DummyLogger.NoThrowInstance;
            MemoryStream mem = new MemoryStream();
            moduleDefMD.Write(mem, modOpts);
            //("test9");
            return mem;
        }
        private static void asmRefAdder()
        {
            var asmResolver = new AssemblyResolver { EnableTypeDefCache = true };
            var modCtx = new ModuleContext(asmResolver);
            asmResolver.DefaultModuleContext = modCtx;
            var asmRefs = moduleDefMD.GetAssemblyRefs().ToList();
            moduleDefMD.Context = modCtx;
            foreach (var asmRef in asmRefs)
            {
                try
                {
                    if (asmRef == null)
                        continue;
                    var asm = asmResolver.Resolve(asmRef.FullName, moduleDefMD);
                    if (asm == null)
                        continue;
                    moduleDefMD.Context.AssemblyResolver.Resolve(asm, moduleDefMD);
                }
                catch { }
            }
        }
    }
}