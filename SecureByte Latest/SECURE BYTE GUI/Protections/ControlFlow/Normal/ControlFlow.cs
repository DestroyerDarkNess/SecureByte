using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Pdb;
using ICore;
using System.Linq;
using static Protections.NormalCFlow.BlockParser;

namespace Protections.NormalCFlow
{
    public class ControlFlow
    {
        public static void Execute(Context context)
        {
            var cctor = context.Module.GlobalType.FindOrCreateStaticConstructor();
            foreach (TypeDef type in context.Module.GetTypes())
            {
                if (type.Namespace == "Costura")
                    continue;
                foreach (MethodDef method in type.Methods)
                {
                    if (!method.HasBody || !method.Body.HasInstructions) continue;
                    if (method.ReturnType != null)
                    {
                        if (cctor == method)
                            continue;
                        PhaseControlFlow(method, context);
                    }
                }
            }
        }
        public static void PhaseControlFlow(MethodDef method, Context context)
        {
            var body = method.Body;
            body.SimplifyBranches();
            ScopeBlock root = ParseBody(body);
            new SwitchMangler().Mangle(body, root, context, method, method.ReturnType);
            body.Instructions.Clear();
            root.ToBody(body);
            if (body.PdbMethod != null)
            {
                body.PdbMethod = new PdbMethod()
                {
                    Scope = new PdbScope()
                    {
                        Start = body.Instructions.First(),
                        End = body.Instructions.Last()
                    }
                };
            }
            method.CustomDebugInfos.RemoveWhere(cdi => cdi is PdbStateMachineHoistedLocalScopesCustomDebugInfo);
            foreach (ExceptionHandler eh in body.ExceptionHandlers)
            {
                var index = body.Instructions.IndexOf(eh.TryEnd) + 1;
                eh.TryEnd = index < body.Instructions.Count ? body.Instructions[index] : null;
                index = body.Instructions.IndexOf(eh.HandlerEnd) + 1;
                eh.HandlerEnd = index < body.Instructions.Count ? body.Instructions[index] : null;
            }
        }
    }
}