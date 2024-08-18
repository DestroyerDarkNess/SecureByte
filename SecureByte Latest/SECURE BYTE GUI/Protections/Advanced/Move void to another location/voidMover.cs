using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;

namespace Protections.Advanced
{
    public static class voidMover
    {
        public static void Execute(Context ctx, MethodDef method)
        {
            ModuleDefMD module = ctx.Module;
            MethodDef entryPointMethod = module.EntryPoint;
            if (entryPointMethod != null)
            {
                MethodDef newMethod = ICore.Utils.CreateMethod(module);
                newMethod.DeclaringType = null;
                int entryPointPosition = module.EntryPoint.DeclaringType.Methods.IndexOf(entryPointMethod);
                entryPointMethod.DeclaringType.Methods.Insert(entryPointPosition, newMethod);
                MethodDef classConstructor = module.GlobalType.FindStaticConstructor();
                MethodDef targetMethod = null;
                foreach (var t in module.GetTypes())
                {
                    foreach (var m in t.Methods)
                    {
                        if (m == method)
                        {
                            targetMethod = m;
                        }
                    }
                }
                newMethod.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(targetMethod));
                Instruction callInstructionToRemove = null;
                for (int i = 0; i < classConstructor.Body.Instructions.Count; i++)
                {
                    Instruction instruction = classConstructor.Body.Instructions[i];
                    if (instruction.OpCode == OpCodes.Call && instruction.Operand is IMethod methodOperand)
                    {
                        if (methodOperand == method)
                        {
                            callInstructionToRemove = instruction;
                            break;
                        }
                    }
                }
                if (callInstructionToRemove != null)
                {
                    classConstructor.Body.Instructions.Remove(callInstructionToRemove);
                }
                classConstructor.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(newMethod));
                targetMethod.Name = ICore.Utils.MethodsRenamig();
            }
        }
    }
}
