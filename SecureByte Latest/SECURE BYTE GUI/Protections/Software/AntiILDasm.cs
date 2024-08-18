using dnlib.DotNet;
using ICore;

namespace Protections.Software
{
    class AntiILDasm
    {
        public static void Execute(Context context)
        {
            var ManifestModule = context.Module.Assembly.ManifestModule;
            TypeRef supressref = ManifestModule.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "SuppressIldasmAttribute");
            var ctorRef = new MemberRefUser(ManifestModule, ".ctor", MethodSig.CreateInstance(ManifestModule.CorLibTypes.Void), supressref);
            var supressattribute = new CustomAttribute(ctorRef);
            ManifestModule.CustomAttributes.Add(supressattribute);
        }
    }
}
