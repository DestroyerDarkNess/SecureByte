using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;
using Protections.Runtime;

namespace Protections.Software
{
    class InjectAntiHttp
    {
		public static void Inject(Context context)
		{
			ModuleDef moduleDef = context.Module;
			ModuleDefMD moduleDefMD = ModuleDefMD.Load(typeof(AntiHttpRuntime).Module);
			TypeDef typeDef = moduleDefMD.ResolveTypeDef(MDToken.ToRID(typeof(AntiHttpRuntime).MetadataToken));
			IEnumerable<IDnlibDef> source = Helpers.Injection.InjectHelper.Inject(typeDef, moduleDef.GlobalType, moduleDef);
			MethodDef method2 = (MethodDef)source.Single((IDnlibDef method) => method.Name == "Initialize");
            MethodDef entryPoint = moduleDef.GlobalType.FindOrCreateStaticConstructor();
			entryPoint.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(method2));
            method2.Name = Utils.MethodsRenamig();
		}
    }
}
