using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;
using SECURE_BYTE_GUI.Global_for_Obfuscation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ICore
{
    public class Context
    {
        #region vars
        public ModuleWriterOptions ModOpts { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string OutPutPath { get; set; }
        public string DirPath { get; set; }
        public static bool preAll = false;
        public ModuleDefMD Module { get; set; }
        public Context ctx;
        #endregion
        #region Preparing
        private void resolveModule()
        {
            foreach (TypeDef type in Module.GetTypes())
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (method.Body != null)
                        Resolver(method);
                }
            }
        }
        private void simplifyModule()
        {
            foreach (TypeDef type in Module.GetTypes())
            {
                foreach (MethodDef method in type.Methods)
                {
                    method.Body?.Instructions.SimplifyMacros(method.Body.Variables, method.Parameters);
                }
            }
        }
        private void optimizeModule()
        {
            foreach (TypeDef type in Module.GetTypes())
            {
                foreach (MethodDef method in type.Methods)
                {
                    method.Body?.Instructions.OptimizeMacros();
                }
            }
        }
        #endregion
        #region Anti Tamper Exe
        void ATNWriterEvent(object sender, ModuleWriterEventArgs e)
        {
            switch (e.Event)
            {
                case ModuleWriterEvent.MDEndCreateTables:
                    new ExAntiTamper.AntiTamperNormal().CreateSections(e.Writer);
                    break;
                case ModuleWriterEvent.BeginStrongNameSign:
                    new ExAntiTamper.AntiTamperNormal().EncryptSection(e.Writer);
                    break;
            }
        }
        #endregion
        #region Invalid Metadata
        private RandomGenerator random;
        private void Randomize<T>(MDTable<T> table) where T : struct
        {
            random.Shuffle<T>(table);
        }
        void IMDWriterEvent(object sender, ModuleWriterEventArgs e)
        {
            random = new RandomGenerator(32);
            var writer = (ModuleWriterBase)sender;
            if (e.Event == ModuleWriterEvent.MDEndCreateTables)
            {
                PESection pESection = new PESection(".????", 1073741888u);
                writer.AddSection(pESection);
                pESection.Add(new ByteArrayChunk(new byte[123]), 4u);
                pESection.Add(new ByteArrayChunk(new byte[10]), 4u);
                writer.Metadata.TablesHeap.ModuleTable.Add(new RawModuleRow(0, 0x7fff7fff, 0, 0, 0));
                writer.Metadata.TablesHeap.AssemblyTable.Add(new RawAssemblyRow(0, 0, 0, 0, 0, 0, 0, 0x7fff7fff, 0));
                int r = random.NextInt32(8, 16);
                for (int i = 0; i < r; i++)
                    writer.Metadata.TablesHeap.ENCLogTable.Add(new RawENCLogRow(random.NextUInt32(), random.NextUInt32()));
                r = random.NextInt32(8, 16);
                for (int i = 0; i < r; i++)
                    writer.Metadata.TablesHeap.ENCMapTable.Add(new RawENCMapRow(random.NextUInt32()));
                Randomize(writer.Metadata.TablesHeap.ManifestResourceTable);
                writer.TheOptions.MetadataOptions.TablesHeapOptions.ExtraData = random.NextUInt32();
                writer.TheOptions.MetadataOptions.TablesHeapOptions.UseENC = false;
                writer.TheOptions.MetadataOptions.MetadataHeaderOptions.VersionString += "\0\0\0\0";
                writer.TheOptions.MetadataOptions.CustomHeaps.Add(new RawHeap("#GUID", Guid.NewGuid().ToByteArray()));
                writer.TheOptions.MetadataOptions.CustomHeaps.Add(new RawHeap("#Strings", new byte[1]));
                writer.TheOptions.MetadataOptions.CustomHeaps.Add(new RawHeap("#Blob", new byte[1]));
                writer.TheOptions.MetadataOptions.CustomHeaps.Add(new RawHeap("#Schema", new byte[1]));
                writer.TheOptions.MetadataOptions.CustomHeaps.Add(new RawHeap("#GUID", Guid.NewGuid().ToByteArray()));
                writer.TheOptions.MetadataOptions.CustomHeaps.Add(new RawHeap("#<Module>", new byte[1]));
                writer.TheOptions.MetadataOptions.CustomHeaps.Add(new RawHeap("#" + Safe.GenerateRandomLetters(4), new byte[1]));
                writer.TheOptions.MetadataOptions.CustomHeaps.Add(new RawHeap("#<Module>", new byte[1]));
                //writer.TheOptions.MetadataOptions.CustomHeaps.Add(new RawHeap("#<Module>", new byte[5]));
                //writer.TheOptions.MetadataOptions.CustomHeaps.Add(new RawHeap("#" + Safe.GenerateRandomLetters(4), new byte[21]));
                //writer.TheOptions.MetadataOptions.CustomHeaps.Add(new RawHeap("#<Module>", new byte[82]));
                string text = ".????";
                string s = null;
                for (int i = 0; i < 10; i++)
                {
                    text += GetRandomString();
                }
                for (int j = 0; j < 10; j++)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(text);
                    s = EncodeString(bytes, asciiCharset);
                }
                byte[] bytes2 = Encoding.ASCII.GetBytes(s);
                writer.TheOptions.MetadataOptions.CustomHeaps.Add(new RawHeap("#null", bytes2));
                pESection.Add(new ByteArrayChunk(bytes2), 4u);
                uint signature = (uint)(writer.Metadata.TablesHeap.TypeSpecTable.Rows + 1);
                writer.Metadata.TablesHeap.TypeSpecTable.Add(new RawTypeSpecRow(signature));
            }
            else if (e.Event == ModuleWriterEvent.MDOnAllTablesSorted)
            {
                writer.Metadata.TablesHeap.DeclSecurityTable.Add(new RawDeclSecurityRow(unchecked(0x7fff), 0xffff7fff, 0xffff7fff));
            }
        }
        void IMDWriterEvent2(object sender, ModuleWriterEventArgs e)
        {
            switch (e.Event)
            {
                case ModuleWriterEvent.PESectionsCreated:
                    var sect1 = new PESection(Safe.GenerateRandomLetters(4) + Safe.GenerateRandomLetters(4), 0xE0000040);
                    e.Writer.AddSection(sect1);
                    sect1.Add(new ByteArrayChunk(new byte[10]), 4);
                    sect1.Add(new ByteArrayChunk(new byte[10]), 4);
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region Fake Native
        private static readonly System.Random R = new System.Random();
        public static string GetRandomString()
        {
            string randomFileName = System.IO.Path.GetRandomFileName();
            return randomFileName.Replace(".", "");
        }
        private static readonly char[] asciiCharset = (from ord in Enumerable.Range(32, 95)
                                                       select (char)ord).Except(new char[]
        {
                '.'
        }).ToArray<char>();
        public static string EncodeString(byte[] buff, char[] charset)
        {
            int current = buff[0];
            var ret = new StringBuilder();
            for (int i = 1; i < buff.Length; i++)
            {
                current = (current << 8) + buff[i];
                while (current >= charset.Length)
                {
                    ret.Append(charset[current % charset.Length]);
                    current /= charset.Length;
                }
            }
            if (current != 0)
                ret.Append(charset[current % charset.Length]);
            return ret.ToString();
        }
        #endregion       
        #region Resolve
        private void Resolver(MethodDef method)
        {
            IList<Instruction> instr = method.Body.Instructions;
            for (int i = 0; i < instr.Count; i++)
            {
                object operand = instr[i].Operand;
                if (operand != null && instr[i].OpCode == OpCodes.Call && operand.ToString().Contains("System.Convert::ToInt32") && instr[i - 1].OpCode == OpCodes.Ldc_R4)
                {
                    int num = Convert.ToInt32(instr[i - 1].Operand);
                    instr[i - 1].OpCode = OpCodes.Nop;
                    instr[i].OpCode = OpCodes.Ldc_I4;
                    instr[i].Operand = num;
                }
            }
        }
        private void LoadAssemblies()
        {
            var asmResolver = new AssemblyResolver { EnableTypeDefCache = true };
            var modCtx = new ModuleContext(asmResolver);
            asmResolver.DefaultModuleContext = modCtx;
            var asmRefs = Module.GetAssemblyRefs().ToList();
            Module.Context = modCtx;
            foreach (var asmRef in asmRefs)
            {
                try
                {
                    if (asmRef == null)
                        continue;
                    var asm = asmResolver.Resolve(asmRef.FullName, Module);
                    if (asm == null)
                        continue;
                    Module.Context.AssemblyResolver.Resolve(asm, Module);
                }
                catch { }
            }
        }
        #endregion
        public Context(string path)
        {
            Path = path;
            Module = ModuleDefMD.Load(path);
            ModOpts = new ModuleWriterOptions(Module);
            resolveModule();
            LoadAssemblies();
            simplifyModule();
        }
        public void SaveFile()
        {
            ModOpts.MetadataLogger = DummyLogger.NoThrowInstance;
            //Module.Cor20HeaderFlags = ComImageFlags.ILOnly;
            //ModOpts.Cor20HeaderOptions.Flags |= ComImageFlags.ILOnly;
            if (preAll)
            {
                ModOpts.MetadataOptions.Flags = MetadataFlags.PreserveAll;
            }
            else
            {
                ModOpts.MetadataOptions.Flags = MetadataFlags.AlwaysCreateGuidHeap | MetadataFlags.AlwaysCreateStringsHeap | MetadataFlags.AlwaysCreateUSHeap | MetadataFlags.AlwaysCreateBlobHeap | MetadataFlags.PreserveAllMethodRids;
            }
            if (oGlobals.InvalidMD)
            {
                ModOpts.WriterEvent += IMDWriterEvent;
                ModOpts.WriterEvent += IMDWriterEvent2;
                ModOpts.MetadataOptions.TablesHeapOptions.ExtraData = 0x12345678;
                ModOpts.MetadataOptions.CustomHeaps.Add(new RawHeap("#" + Safe.GenerateRandomLetters(4), new byte[1]));
                ModOpts.MetadataOptions.MetadataHeapsAdded += (s, e) =>
                {
                    ModOpts.MetadataOptions.CustomHeaps.Add(new RawHeap("#" + Safe.GenerateRandomLetters(4), new byte[1]));
                };
            }
            if (oGlobals.Atamper)
            {
                ModOpts.WriterEvent += ATNWriterEvent;
            }
            optimizeModule();
            Directory.CreateDirectory(DirPath + @"\");
            if (Directory.Exists(DirPath))
                Module.Write(OutPutPath, ModOpts);
            Module.Dispose();
            Clear();
        }
        public void Clear()
        {
            Path = null; Module = null; ctx = null;
        }
    }
}
