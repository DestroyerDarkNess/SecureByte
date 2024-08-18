using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using Helpers.Injection;
using Helpers.Mutations;
using ICore;
using JIT.Utils;
using Protections.Software.Global;
using SECURE_BYTE_GUI.Global_for_Obfuscation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MethodImplAttributes = dnlib.DotNet.MethodImplAttributes;

namespace JIT.Protection
{
    public class Protection
    {
        private readonly ModuleDefMD module;
        static List<EncryptedMethod> encryptedMethods;
        public static string x = "";
        public static string xx = "";
        public static string mname = "";
        public Protection(ModuleDefMD Module)
        {
            module = Module;
            encryptedMethods = new List<EncryptedMethod>();
        }
        public byte[] Protect()
        {
            AddToResources(module);
            InjectRuntime();
            if (oGlobals.dynCctor)
                new IlDyn.IL2Dynamic().Execute(module);
            SearchMethods();
            return EncryptModule();
        }
        private static newInjector inj = null;
        void InjectRuntime()
        {
            x = ICore.Utils.MethodsRenamig();
            xx = ICore.Safe.GenerateRandomLetters(5);
            var cctor = module.GlobalType.FindOrCreateStaticConstructor();
            inj = new newInjector(module, typeof(Runtime.JITRuntime));
            var initMethod = inj.FindMember("Initialize") as MethodDef;
            foreach (Instruction Instruction in initMethod.Body.Instructions.Where((Instruction I) => I.OpCode == OpCodes.Ldstr))
            {
                if (Instruction.Operand.ToString() == "Key")
                    Instruction.Operand = Convert.ToString(new Random().Next(1, 9));
            }
            var strdecode = inj.FindMember("decodeStr") as MethodDef;
            var converter = inj.FindMember("StreamToByteArray") as MethodDef;
            var DecompressBytes = inj.FindMember("DecompressBytes") as MethodDef;
            var HeaderLen = inj.FindMember("HeaderLen") as MethodDef;
            var SizeDecompressed = inj.FindMember("SizeDecompressed") as MethodDef;
            string nSapce = ICore.Utils.MethodsRenamig();
            MethodDef[] Utils = new MethodDef[]
           {
               converter,
               strdecode,
                DecompressBytes,
                HeaderLen,
                SizeDecompressed
           };
            cctor.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(initMethod));
            var mutation = new MutationHelper("MutationClass");
            mutation.InjectKey<string>(initMethod, 12, toBase64(x));
            mutation.InjectKey<string>(initMethod, 13, oGlobals.x86RT);
            mutation.InjectKey<string>(initMethod, 14, oGlobals.x64RT);
            mutation.InjectKey<string>(initMethod, 15, toBase64("ISByte"));
            //Helpers.Mutations.MutationHelper.InjectString(initMethod, "j", toBase64(x));
            //Helpers.Mutations.MutationHelper.InjectString(initMethod, "n", oGlobals.x86RT);
            //Helpers.Mutations.MutationHelper.InjectString(initMethod, "o", oGlobals.x64RT);
            //Helpers.Mutations.MutationHelper.InjectString(initMethod, "u", toBase64("ISByte"));
            foreach (var m in Utils)
            {
                m.Name = ICore.Utils.MethodsRenamig();
            }
            initMethod.Name = ICore.Utils.MethodsRenamig();
        }
        static string toBase64(string str)
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytesToEncode);
        }
        void AddToResources(ModuleDefMD Module)
        {
            var x86 = SECURE_BYTE_GUI.Properties.Resources.JLX86;
            var cX86 = ICore.Compression.QuickLZ.CompressBytes(x86);
            var x64 = SECURE_BYTE_GUI.Properties.Resources.JLX64;
            var cX64 = ICore.Compression.QuickLZ.CompressBytes(x64);
            Module.Resources.Add(new EmbeddedResource(oGlobals.x86RT, cX86, ManifestResourceAttributes.Private));
            Module.Resources.Add(new EmbeddedResource(oGlobals.x64RT, cX64, ManifestResourceAttributes.Private));
        }
        void SearchMethods()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            foreach (TypeDef typeDef in module.Types.ToArray())
            {
                if (typeDef.IsGlobalModuleType)
                    continue;
                foreach (MethodDef methodDef in typeDef.Methods)
                {
                    if (methodDef.HasGenericParameters || (methodDef.HasReturnType && methodDef.ReturnType.IsGenericParameter)) continue;
                    bool isConstructor = methodDef.IsStaticConstructor;
                    if (!isConstructor)
                    {
                        bool hasBody = methodDef.HasBody;
                        if (hasBody && methodDef.Body.HasInstructions)
                        {
                            methodDef.ImplAttributes |= MethodImplAttributes.NoInlining;
                            var exceptionRef = module.CorLibTypes.GetTypeRef("System", "Exception");
                            var objectCtor = new MemberRefUser(module, ".ctor",
                            MethodSig.CreateInstance(module.CorLibTypes.Void, module.CorLibTypes.String), exceptionRef);
                            methodDef.Body.Instructions.Add(OpCodes.Ldstr.ToInstruction("Failed to load Runtime!"));
                            methodDef.Body.Instructions.Add(OpCodes.Newobj.ToInstruction(objectCtor));
                            methodDef.Body.Instructions.Add(OpCodes.Throw.ToInstruction());
                            encryptedMethods.Add(new EncryptedMethod
                            {
                                Method = methodDef,
                                OriginalBytes = module.GetOriginalRawILBytes(methodDef),
                                IsEncrypted = false
                            });
                        }
                    }
                }
            }
            writer.Write(encryptedMethods.Count);
            foreach (var method in encryptedMethods)
            {
                writer.Write(method.Method.MDToken.ToInt32());
                writer.Write(Convert.ToBase64String(method.OriginalBytes));
            }
            byte[] streamArr = stream.ToArray();
            byte[] compressedStream = ICore.Compression.QuickLZ.CompressBytes(streamArr);
            var res = new EmbeddedResource(x, compressedStream, ManifestResourceAttributes.Private);
            module.Resources.Add(res);          
        }
        byte[] EncryptModule()
        {
            var writerOptions = new ModuleWriterOptions(module)
            {
                Logger = DummyLogger.NoThrowInstance,
            };
            writerOptions.MetadataOptions.Flags = MetadataFlags.PreserveAll;
            writerOptions.WriterEvent += OnWriterEvent;
            var result = new MemoryStream();
            module.Write(result, writerOptions);
            var bytes = result.ToArray();
            foreach (var method in encryptedMethods)
            {
                if (!method.IsEncrypted)
                {
                    var end = (int)method.FileOffset + method.CodeSize;
                    for (int i = (int)method.FileOffset; i < end; i++)
                    {
                        bytes[i] = 0x0;
                    }
                }
            }
            return bytes;
        }
        void OnWriterEvent(object sender, ModuleWriterEventArgs e)
        {
            if (e.Event == ModuleWriterEvent.EndWriteChunks)
            {
                foreach (var method in encryptedMethods)
                {
                    var methodBody = e.Writer.Metadata.GetMethodBody(method.Method);
                    var index = Utils.Utils.Locate(methodBody.Code, method.OriginalBytes);
                    method.FileOffset = (uint)(((uint)methodBody.FileOffset) + index);
                    method.CodeSize = method.OriginalBytes.Length;
                }
            }
        }
        public class EncryptedMethod
        {
            public MethodDef Method;
            public uint FileOffset;
            public int CodeSize;
            public byte[] OriginalBytes;
            public bool IsEncrypted;
        }
    }
}
