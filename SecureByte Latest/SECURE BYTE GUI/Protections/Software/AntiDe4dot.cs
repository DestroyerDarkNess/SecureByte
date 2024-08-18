using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;
using System.Linq;

namespace Protections.Software
{
	internal class AntiDe4dot
	{
        private static ModuleDef publicmodule;
		private static void SecureByte()
		{
            var attrRef = publicmodule.CorLibTypes.GetTypeRef("System", "Attribute");
            var attrType = publicmodule.FindNormal("SecureByteAttribute");
            if (attrType == null)
            {
                attrType = new TypeDefUser(string.Empty, "SecureByteAttribute", attrRef);
                publicmodule.Types.Add(attrType);
            }
            var ctor = attrType.FindInstanceConstructors()
                .FirstOrDefault(m => m.Parameters.Count == 1 && m.Parameters[0].Type == publicmodule.CorLibTypes.String);
            if (ctor == null)
            {
                ctor = new MethodDefUser(
                    ".ctor",
                    MethodSig.CreateInstance(publicmodule.CorLibTypes.Void, publicmodule.CorLibTypes.String),
                    MethodImplAttributes.Managed,
                    MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName)
                {
                    Body = new CilBody { MaxStack = 1 }
                };
                ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, new MemberRefUser(publicmodule, ".ctor",
                    MethodSig.CreateInstance(publicmodule.CorLibTypes.Void), attrRef)));
                ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                attrType.Methods.Add(ctor);
            }
            var attr = new CustomAttribute(ctor);
            attr.ConstructorArguments.Add(new CAArgument(publicmodule.CorLibTypes.String, "SecureByte 1.0.0 (DeepRET Version)"));
            publicmodule.CustomAttributes.Add(attr);
        }
        public static void Execute(Context context)
		{
			publicmodule = context.Module;
            SecureByte();
        }
	}
}
