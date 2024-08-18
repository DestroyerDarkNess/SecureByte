using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;
using System.Linq;

namespace Protections.Ints
{
    public class LocalToField
    {
        public static void ToField(Context context)
        {
            var module = context.Module;
            var cctor = module.GlobalType.FindOrCreateStaticConstructor();
            var body = cctor.Body.Instructions;
            foreach (var type in module.GetTypes())
            {
                if (type.IsGlobalModuleType)
                    continue;
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody || !method.Body.HasInstructions)
                        continue;
                    var instrs = method.Body.Instructions;
                    if (!instrs.Any(x => x.IsLdcI4()))
                        continue;
                    var first = instrs.First(x => x.IsLdcI4());
                    var value = first.GetLdcI4Value();
                    var field = Utils.CreateField(new FieldSig(module.CorLibTypes.Int32));
                    module.GlobalType.Fields.Add(field);
                    body.Insert(0, OpCodes.Ldc_I4.ToInstruction(value));
                    body.Insert(1, OpCodes.Stsfld.ToInstruction(field));
                    first.OpCode = OpCodes.Ldsfld;
                    first.Operand = field;
                }
            }
        }
        public static void ToMethod(Context context)
        {
            var module = context.Module;
            var cctor = module.GlobalType.FindOrCreateStaticConstructor();
            var body = cctor.Body.Instructions;
            foreach (var type in module.GetTypes())
            {
                if (type.IsGlobalModuleType)
                    continue;
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody || !method.Body.HasInstructions)
                        continue;
                    var instrs = method.Body.Instructions;
                    if (!instrs.Any(x => x.IsLdcI4()))
                        continue;
                    var first = instrs.First(x => x.IsLdcI4());
                    var value = first.GetLdcI4Value();
                    var field = Utils.CreateField(new FieldSig(module.CorLibTypes.Int32));
                    MethodDef mdefuser = new MethodDefUser(Utils.MethodsRenamig(), MethodSig.CreateStatic(module.CorLibTypes.Int32),
                    MethodImplAttributes.IL | MethodImplAttributes.Managed,
                    MethodAttributes.Public | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig)
                    {
                        Body = new CilBody()
                    };
                    module.GlobalType.Fields.Add(field);
                    mdefuser.Body.Instructions.Add(Instruction.Create(OpCodes.Ldsfld, field));
                    mdefuser.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
                    module.GlobalType.Methods.Add(mdefuser);
                    body.Insert(0, OpCodes.Ldc_I4.ToInstruction(value));
                    body.Insert(1, OpCodes.Stsfld.ToInstruction(field));
                    first.OpCode = OpCodes.Call;
                    first.Operand = mdefuser;
                }
            }
        }
    }
}