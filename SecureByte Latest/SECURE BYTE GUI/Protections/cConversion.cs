using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallConversion
{
    class cConversion
    {
        public static void Execute(ModuleDefMD Module)
        {
            foreach (var t in Module.GetTypes())
            {
                foreach (var methodDef in t.Methods)
                {
                    bool hasBody = methodDef.HasBody;
                    if (hasBody)
                    {
                        bool hasInstructions = methodDef.Body.HasInstructions;
                        if (hasInstructions)
                        {
                            bool flag = methodDef.FullName.Contains("My.");
                            if (!flag)
                            {
                                bool flag2 = methodDef.FullName.Contains(".My");
                                if (!flag2)
                                {
                                    bool flag3 = methodDef.FullName.Contains("Costura");
                                    if (!flag3)
                                    {
                                        bool isConstructor = methodDef.IsConstructor;
                                        if (!isConstructor)
                                        {
                                            bool isGlobalModuleType = methodDef.DeclaringType.IsGlobalModuleType;
                                            if (!isGlobalModuleType)
                                            {
                                                int k = 0;
                                                while (k < methodDef.Body.Instructions.Count - 1)
                                                {
                                                    try
                                                    {
                                                        bool flag4 = methodDef.Body.Instructions[k].ToString().Contains("ISupportInitialize");
                                                        if (!flag4)
                                                        {
                                                            bool flag5 = methodDef.Body.Instructions[k].OpCode == OpCodes.Call || methodDef.Body.Instructions[k].OpCode == OpCodes.Callvirt;
                                                            if (flag5)
                                                            {
                                                                try
                                                                {
                                                                    MemberRef memberRef = (MemberRef)methodDef.Body.Instructions[k].Operand;
                                                                    methodDef.Body.Instructions[k].OpCode = OpCodes.Calli;
                                                                    methodDef.Body.Instructions[k].Operand = memberRef.MethodSig;
                                                                    methodDef.Body.Instructions.Insert(k, Instruction.Create(OpCodes.Ldftn, memberRef));
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    string message = ex.Message;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    catch
                                                    {
                                                    }
                                                    k++;
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void forMethod(MethodDef methodDef)
        {
            bool hasBody = methodDef.HasBody;
            if (hasBody)
            {
                bool hasInstructions = methodDef.Body.HasInstructions;
                if (hasInstructions)
                {
                    bool flag = methodDef.FullName.Contains("My.");
                    if (!flag)
                    {
                        bool flag2 = methodDef.FullName.Contains(".My");
                        if (!flag2)
                        {
                            bool flag3 = methodDef.FullName.Contains("Costura");
                            if (!flag3)
                            {
                                bool isConstructor = methodDef.IsConstructor;
                                if (!isConstructor)
                                {
                                    bool isGlobalModuleType = methodDef.DeclaringType.IsGlobalModuleType;
                                    if (!isGlobalModuleType)
                                    {
                                        int k = 0;
                                        while (k < methodDef.Body.Instructions.Count - 1)
                                        {
                                            try
                                            {
                                                bool flag4 = methodDef.Body.Instructions[k].ToString().Contains("ISupportInitialize");
                                                if (!flag4)
                                                {
                                                    bool flag5 = methodDef.Body.Instructions[k].OpCode == OpCodes.Call || methodDef.Body.Instructions[k].OpCode == OpCodes.Callvirt;
                                                    if (flag5)
                                                    {
                                                        try
                                                        {
                                                            MemberRef memberRef = (MemberRef)methodDef.Body.Instructions[k].Operand;
                                                            methodDef.Body.Instructions[k].OpCode = OpCodes.Calli;
                                                            methodDef.Body.Instructions[k].Operand = memberRef.MethodSig;
                                                            methodDef.Body.Instructions.Insert(k, Instruction.Create(OpCodes.Ldftn, memberRef));
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            string message = ex.Message;
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                            }
                                            k++;
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
