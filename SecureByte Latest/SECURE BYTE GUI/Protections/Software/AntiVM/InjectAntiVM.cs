using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;
using Protections.Runtime;

namespace Protections.Software
{
    class InjectAntiVM
    {
		public static void Inject(Context context)
		{
			ModuleDef moduleDef = context.Module;
			ModuleDefMD moduleDefMD = ModuleDefMD.Load(typeof(AntiVM).Module);
			TypeDef typeDef = moduleDefMD.ResolveTypeDef(MDToken.ToRID(typeof(AntiVM).MetadataToken));
            IEnumerable<IDnlibDef> source = Helpers.Injection.InjectHelper.Inject(typeDef, moduleDef.EntryPoint.DeclaringType, moduleDef);
            MethodDef method2 = (MethodDef)source.Single((IDnlibDef method) => method.Name == "Initialize");
            MethodDef entryPoint = moduleDef.EntryPoint;
            entryPoint.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(method2));
            foreach (var mem in source)
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
