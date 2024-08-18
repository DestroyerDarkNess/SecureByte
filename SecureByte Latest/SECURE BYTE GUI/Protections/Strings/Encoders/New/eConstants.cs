using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Helpers.Injection;
using ICore.Compression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Protections.newStrings
{
    public static class eConstants
    {
        private static byte[] convertListToByteArray(List<string> list)
        {
            string concatenatedString = string.Join(Environment.NewLine, list);
            return Encoding.UTF8.GetBytes(concatenatedString);
        }
        private static void Inject(ModuleDefMD Module)
        {
            inj = new newInjector(Module, typeof(Utils));
            streamToByteArray = inj.FindMember("Read") as MethodDef;
            extractResources = inj.FindMember("Extract") as MethodDef;
            foreach (Instruction Instruction in extractResources.Body.Instructions.Where((Instruction I) => I.OpCode == OpCodes.Ldstr))
            {
                if (Instruction.Operand.ToString() == "CallMeInx")
                    Instruction.Operand = resName;
            }
            decryptStrings = inj.FindMember("Call") as MethodDef;
            stringsListFld = inj.FindMember("stringsList") as FieldDef;
            foreach (Instruction Instruction in decryptStrings.Body.Instructions.Where((Instruction I) => I.OpCode == OpCodes.Ldstr))
            {
                if (Instruction.Operand.ToString() == "Key")
                    Instruction.Operand = Key;
                if (Instruction.Operand.ToString() == "IV")
                    Instruction.Operand = IV;
            }
            DecompressBytes = inj.FindMember("DecompressBytes") as MethodDef;
            HeaderLen = inj.FindMember("HeaderLen") as MethodDef;
            SizeDecompressed = inj.FindMember("SizeDecompressed") as MethodDef;
            MethodDef[] extraction = new MethodDef[]
            {
                streamToByteArray,
                extractResources
            };
            MethodDef[] decryption = new MethodDef[]
            {
                decryptStrings
            };
            string nSpace = ICore.Utils.MethodsRenamig();
            MethodDef[] decompression = new MethodDef[]
            {
                DecompressBytes,
                HeaderLen,
                SizeDecompressed
            };
            inj.injectMethods(nSpace, ICore.Utils.MethodsRenamig(), Module, extraction);
            inj.injectMethods(nSpace, ICore.Utils.MethodsRenamig(), Module, decompression);
            inj.injectMethods(nSpace, ICore.Utils.MethodsRenamig(), Module, decryption);
        }
        private static string Encrypt(string plainText)
        {
            Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(Key);
            aesAlg.IV = Encoding.UTF8.GetBytes(IV);
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            byte[] encrypted;
            using (var msEncrypt = new System.IO.MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                encrypted = msEncrypt.ToArray();
            }
            return Convert.ToBase64String(encrypted);
        }
        public static void Encode(ModuleDefMD Module)
        {
            Key = ICore.Safe.GenerateRandomLetters(32);
            //
            IV = ICore.Safe.GenerateRandomLetters(16);
            //
            resName = ICore.Utils.MethodsRenamig();
            //
            //if (decryptStrings == null)
                Inject(Module);
            //
            foreach (TypeDef typeDef in Module.GetTypes().Where(x => x.HasMethods && !x.IsGlobalModuleType && x.Namespace != "Costura"))
            {
                foreach (MethodDef i in typeDef.Methods.Where(x => x.HasBody && x.Body.HasInstructions))
                {
                    if (!ToIgnore.Contains(i.Name.ToString()))
                    {
                        i.Body.SimplifyMacros(i.Parameters);
                        i.Body.SimplifyBranches();
                        IList<Instruction> instr = i.Body.Instructions;
                        for (int j = 0; j < instr.Count; j++)
                        {
                            if (instr[j].OpCode == OpCodes.Ldstr)
                            {
                                string encrypted = Encrypt(i.Body.Instructions[j].Operand.ToString());
                                stringsList.Add(encrypted);
                                i.Body.Instructions[j].OpCode = OpCodes.Ldsfld;
                                i.Body.Instructions[j].Operand = stringsListFld;
                                i.Body.Instructions.Insert(j + 1, new Instruction(OpCodes.Ldc_I4, stringsList.LastIndexOf(encrypted)));
                                i.Body.Instructions.Insert(j + 2, new Instruction(OpCodes.Call, decryptStrings));
                                j += 2;
                            }
                        }
                        i.Body.OptimizeMacros();
                    }
                }
            }
            //
            byte[] compressedBytes = QuickLZ.CompressBytes(convertListToByteArray(stringsList));
            EmbeddedResource embeddedResource = new EmbeddedResource(resName, compressedBytes, ManifestResourceAttributes.Private);
            Module.Resources.Add(embeddedResource);
            MethodDef global = Module.GlobalType.FindOrCreateStaticConstructor();
            global.Body.Instructions.Insert(global.Body.Instructions.Count - 1, OpCodes.Call.ToInstruction(extractResources));
            stringsList.Clear();
            inj.Rename();
        }
        private static readonly List<string> stringsList = new List<string>();
        private static string resName = "";
        private static string Key = "";
        private static string IV = "";
        private static MethodDef streamToByteArray = null;
        private static MethodDef extractResources = null;
        private static MethodDef DecompressBytes = null;
        private static MethodDef HeaderLen = null;
        private static MethodDef SizeDecompressed = null;
        private static MethodDef decryptStrings = null;
        private static FieldDef stringsListFld = null;
        private static newInjector inj = null;
        public static string[] ToIgnore = new string[] { "Call", "Extract", "DecompressBytes" };
    }
}