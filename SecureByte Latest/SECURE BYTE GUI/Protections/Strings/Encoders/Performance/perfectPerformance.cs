using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Helpers.Injection;
using ICore;
using ICore.CryptoRandom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Protections.Strings
{
    public static class sUtils
    {
        public static Dictionary<string, string> hTable;
        public static string XorCipher(string cipherText, int key, bool t)
        {
            string Text = string.Empty;
            if (hTable == null)
            {
                hTable = new Dictionary<string, string>();
            }
            Module module = typeof(sUtils).Module;
            Assembly assembly = module.Assembly;
            if (assembly != null && t)
            {
                Dictionary<string, string> hashtable = hTable;
                var stringBuilder = new StringBuilder();
                lock (hashtable)
                {
                    if (hTable.ContainsKey(cipherText))
                        Text = hTable[cipherText];
                    else
                    {
                        if (t)
                        {
                            foreach (var symbol in cipherText)
                            {
                                stringBuilder.Append((char)(symbol ^ key));
                                Text = stringBuilder.ToString();
                            }
                        }
                        hTable[cipherText] = Text;
                    }
                }
                return Text;
            }
            return null;
        }
    }
    public static class stillWorkingOn2
    {
        private static string xor(Tuple<string, int> values)
        {
            var stringBuilder = new StringBuilder();
            int key = values.Item2;
            foreach (var symbol in values.Item1)
                stringBuilder.Append((char)(symbol ^ key));
            return stringBuilder.ToString();
        }
        private static newInjector inj = null;
        static MethodDef Call = null;
        private static void Inject(ModuleDefMD module)
        {
            inj = new newInjector(module, typeof(sUtils));
            Call = inj.FindMember("XorCipher") as MethodDef;
            inj.injectMethod(Utils.MethodsRenamig(), Utils.MethodsRenamig(), module, Call);
        }
        public static void Encode(Context context)
        {
            Inject(context.Module);
            var cryptoRandom = new CryptoRandom();
            foreach (var typeDef in context.Module.GetTypes().Where(x => x.HasMethods && !x.IsGlobalModuleType && x.Name != "Costura"))
            {
                foreach (var m in typeDef.Methods.Where(x => x.HasBody))
                {
                    m.Body.SimplifyMacros(m.Parameters);
                    m.Body.SimplifyBranches();
                    IList<Instruction> instr = m.Body.Instructions;
                    for (int j = 0; j < instr.Count; j++)
                    {
                        if (m.Body.Instructions[j].OpCode == OpCodes.Ldstr)
                        {
                            var key = m.Name.Length + cryptoRandom.Next();
                            var encrypted = xor(new Tuple<string, int>(instr[j].Operand.ToString(), key));
                            m.Body.Instructions[j].OpCode = OpCodes.Ldstr;
                            m.Body.Instructions[j].Operand = encrypted;
                            m.Body.Instructions.Insert(j + 1, new Instruction(OpCodes.Ldc_I4, key));
                            m.Body.Instructions.Insert(j + 2, new Instruction(OpCodes.Ldc_I4_1, true));
                            m.Body.Instructions.Insert(j + 3, new Instruction(OpCodes.Call, Call));
                            j += 3;
                        }
                    }
                    m.Body.OptimizeMacros();
                }
            }
            inj.Rename();
        }
        public static void EncodeFor(Context context, MethodDef[] methods)
        {
            Inject(context.Module);
            var cryptoRandom = new CryptoRandom();
            foreach (var typeDef in context.Module.GetTypes().Where(x => x.HasMethods && !x.IsGlobalModuleType && x.Name != "Costura"))
            {
                foreach (var m in typeDef.Methods.Where(x => x.HasBody))
                {
                    foreach (var method in methods)
                    {
                        if (m == method)
                        {
                            m.Body.SimplifyMacros(m.Parameters);
                            m.Body.SimplifyBranches();
                            IList<Instruction> instr = m.Body.Instructions;
                            for (int j = 0; j < instr.Count; j++)
                            {
                                if (m.Body.Instructions[j].OpCode == OpCodes.Ldstr)
                                {
                                    var key = m.Name.Length + cryptoRandom.Next();
                                    var encrypted = xor(new Tuple<string, int>(instr[j].Operand.ToString(), key));
                                    m.Body.Instructions[j].OpCode = OpCodes.Ldstr;
                                    m.Body.Instructions[j].Operand = encrypted;
                                    m.Body.Instructions.Insert(j + 1, new Instruction(OpCodes.Ldc_I4, key));
                                    m.Body.Instructions.Insert(j + 2, new Instruction(OpCodes.Ldc_I4_1, true));
                                    m.Body.Instructions.Insert(j + 3, new Instruction(OpCodes.Call, Call));
                                    j += 3;
                                }
                            }
                            m.Body.OptimizeMacros();
                        }
                    }
                }
            }
            inj.Rename();
        }
    }
}