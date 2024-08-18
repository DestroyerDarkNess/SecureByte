using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;
using Protections.Runtime;

namespace Protections.Software
{
	public class AntiDump 
	{
        public static void Execute(Context context)
        {
            ModuleDefMD moduleDefMD = ModuleDefMD.Load(typeof(AntiDumpRuntime).Module);
            TypeDef typeDef = moduleDefMD.ResolveTypeDef(MDToken.ToRID(typeof(AntiDumpRuntime).MetadataToken));
            IEnumerable<IDnlibDef> defs = Helpers.Injection.InjectHelper.Inject(typeDef, context.Module.EntryPoint.DeclaringType, context.Module);
            MethodDef method2 = (MethodDef)defs.Single((IDnlibDef method) => method.Name == "Initialize");
            MethodDef entryPoint = context.Module.EntryPoint;
            entryPoint.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(method2));
            foreach (var mem in defs)
            {
                if (mem is MethodDef method)
                {
                    if (method.HasImplMap)
                        continue;
                    if (method.DeclaringType.IsDelegate)
                        continue;
                }
                Utils.MethodsRenamig(mem);
            }
        }
    }
}
