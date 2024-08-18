using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;
using Protections.Runtime;

namespace Protections.Software
{
    class AntiHookInject
    {
        public static void Execute(Context context)
        {
            ModuleDefMD moduleDefMD = ModuleDefMD.Load(typeof(AntiHook).Module);
            TypeDef typeDef = moduleDefMD.ResolveTypeDef(MDToken.ToRID(typeof(AntiHook).MetadataToken));
            IEnumerable<IDnlibDef> source = Helpers.Injection.InjectHelper.Inject(typeDef, context.Module.EntryPoint.DeclaringType, context.Module);
            MethodDef method2 = (MethodDef)source.Single((IDnlibDef method) => method.Name == "Initialize");
            MethodDef entryPoint = context.Module.EntryPoint;
            entryPoint.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(method2));
            method2.Name = Utils.MethodsRenamig();
        }
    }
}
