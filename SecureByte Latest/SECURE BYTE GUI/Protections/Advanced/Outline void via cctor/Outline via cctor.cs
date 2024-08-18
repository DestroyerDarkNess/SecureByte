using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;

namespace Protections.Advanced
{
    public static class Outline_via_cctor
    {
        public static void Execute(Context ctx)
        {
            var classConstructor = ctx.Module.GlobalType.FindStaticConstructor();
            if (!classConstructor.HasBody || !classConstructor.Body.HasInstructions) return;
            var instructions = classConstructor.Body.Instructions;
            for (var i = instructions.Count - 1; i >= 0; i--)
            {
                if (instructions[i].OpCode.Code != Code.Call) continue;
                if (!(instructions[i].Operand is MethodDef tm)) continue;
                if (!tm.IsStatic || tm.DeclaringType != ctx.Module.GlobalType) continue;
                classConstructor.Body.MergeCall(instructions[i]);
                tm.DeclaringType.Methods.Remove(tm);
            }
        }
    }
}
