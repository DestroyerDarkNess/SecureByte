using System.Collections.Generic;
using System.Linq;
using ICore;
using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System.Diagnostics;

namespace Protections.Junk
{
    internal static class ModuleFlood
    {
        private static void Initialize()
        {
            if (Debugger.IsAttached || Debugger.IsLogging())
            {
                Process.GetCurrentProcess().Kill();
            }
        }
    }
    public static class Flood
    {
        public static void Execute(Context ctx)
        {
            ModuleDefMD moduleDefMD = ModuleDefMD.Load(typeof(ModuleFlood).Module);
            TypeDef typeDef = moduleDefMD.ResolveTypeDef(MDToken.ToRID(typeof(ModuleFlood).MetadataToken));
            var module = ctx.Module;
            for (int i = 0; i < 256; i++)
            {
                IEnumerable<IDnlibDef> source = InjectHelper.Inject(typeDef, ctx.Module.GlobalType, ctx.Module);
                MethodDef cctor = module.GlobalType.FindStaticConstructor();
                MethodDef init = (MethodDef)source.Single((IDnlibDef method) => method.Name == "Initialize");
                cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
            }
        }
    }
}
