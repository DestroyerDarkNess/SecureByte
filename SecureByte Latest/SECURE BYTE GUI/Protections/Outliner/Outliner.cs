using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;
using System.Collections.Generic;

namespace Protections.Outliner
{
    class Outliner
    {
        public static void ExecuteStrings(Context context)
        {
            var module = context.Module;
            foreach (var type in module.GetTypes())
            {
                if (type.IsGlobalModuleType)
                    continue;
                if (type.Namespace == "Costura")
                    continue;
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody || !method.Body.HasInstructions)
                        continue;
                    StringOutliner(method);
                }
            }
        }
        public static void ExecuteIntegers(Context context)
        {
            var module = context.Module;
            foreach (var type in module.GetTypes())
            {
                if (type.IsGlobalModuleType)
                    continue;
                if (type.Namespace == "Costura")
                    continue;
                foreach (var method in type.Methods)
                {
                    if (Ints.Contains(method)) continue;
                    if (!method.HasBody || !method.Body.HasInstructions)
                        continue;
                    IntegerOutliner(method);
                }
            }
        }
        private static void StringOutliner(MethodDef methodDef)
        {
            bool hasBody = methodDef.HasBody;
            if (hasBody)
            {
                foreach (Instruction instruction in methodDef.Body.Instructions)
                {
                    bool flag = instruction.OpCode != OpCodes.Ldstr;
                    if (!flag)
                    {
                        MethodDef methodDef2 = new MethodDefUser(Utils.MethodsRenamig(), MethodSig.CreateStatic(methodDef.DeclaringType.Module.CorLibTypes.String),
                            MethodImplAttributes.IL | MethodImplAttributes.Managed,
                            MethodAttributes.Public | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig)
                        {
                            Body = new CilBody()
                        };
                        methodDef2.Body.Instructions.Add(new Instruction(OpCodes.Ldstr, instruction.Operand.ToString()));
                        methodDef2.Body.Instructions.Add(new Instruction(OpCodes.Ret));
                        methodDef2.Module.GlobalType.Methods.Add(methodDef2);
                        instruction.OpCode = OpCodes.Call;
                        instruction.Operand = methodDef2;
                        DnlibUtils.EnsureNoInlining(methodDef2);
                        DnlibUtils.HideMethods(methodDef2);
                    }
                }
            }
        }
        static readonly List<MethodDef> Ints = new List<MethodDef>();
        private static void IntegerOutliner(MethodDef methodDef)
        {
            bool hasBody = methodDef.HasBody;
            if (hasBody)
            {
                foreach (Instruction instruction in methodDef.Body.Instructions)
                {
                    bool flag = instruction.OpCode != OpCodes.Ldc_I4;
                    if (!flag)
                    {
                        MethodDef methodDef2 = new MethodDefUser(Utils.MethodsRenamig(), MethodSig.CreateStatic(methodDef.DeclaringType.Module.CorLibTypes.UInt32),
                            MethodImplAttributes.IL | MethodImplAttributes.Managed,
                            MethodAttributes.Public | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig)
                        {
                            Body = new CilBody()
                        };
                        methodDef2.Body.Instructions.Add(new Instruction(OpCodes.Ldc_I4, instruction.GetLdcI4Value()));
                        methodDef2.Body.Instructions.Add(new Instruction(OpCodes.Ret));
                        methodDef.Module.GlobalType.Methods.Add(methodDef2);
                        Ints.Add(methodDef2);
                        instruction.OpCode = OpCodes.Call;
                        instruction.Operand = methodDef2;
                        DnlibUtils.EnsureNoInlining(methodDef2);
                        DnlibUtils.HideMethods(methodDef2);
                    }
                }
            }
        }
    }
}
