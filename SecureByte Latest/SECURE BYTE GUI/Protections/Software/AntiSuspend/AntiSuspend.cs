using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;
using Protections.Runtime;
using Runtime;
using System.Collections.Generic;
using System.Linq;

namespace Protections.Software
{
    internal class AntiSuspend
    {
        public static void Execute(Context context)
        {
            //ModuleDefMD moduleDefMD = ModuleDefMD.Load(typeof(AntiSusRT).Module);
            //TypeDef typeDef = moduleDefMD.ResolveTypeDef(MDToken.ToRID(typeof(AntiSusRT).MetadataToken));
            //IEnumerable<IDnlibDef> source = Helpers.Injection.InjectHelper.Inject(typeDef, context.Module.EntryPoint.DeclaringType, context.Module);
            //MethodDef Init = (MethodDef)source.Single((IDnlibDef method) => method.Name == "Init");
            //MethodDef entryPoint = context.Module.EntryPoint;
            //entryPoint.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(Init));
            //Utils.MethodRenamig(Init);
        }
    }
}
