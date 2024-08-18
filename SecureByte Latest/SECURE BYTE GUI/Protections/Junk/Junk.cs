using dnlib.DotNet;
using ICore;

namespace Protections.Junk
{
    public class Junk
    {
        public static void Execute(Context context, int length)
        {
            int num = context.Module.Assembly.Modules.Count - 1;
            for (int i = 0; i <= num; i++)
            {
                for (int j = 0; j <= length; j++)
                {
                    new TypeDefUser(Utils.MethodsRenamig(), context.Module.CorLibTypes.Object.TypeDefOrRef).Attributes = TypeAttributes.Public;
                    TypeDef item = new TypeDefUser(Utils.MethodsRenamig(), Utils.MethodsRenamig(), context.Module.CorLibTypes.Object.TypeDefOrRef)
                    {
                        Attributes = TypeAttributes.Public
                    };
                    TypeDef item2 = new TypeDefUser(Utils.MethodsRenamig(), Utils.MethodsRenamig(), context.Module.CorLibTypes.Object.TypeDefOrRef)
                    {
                        Attributes = TypeAttributes.Public
                    };
                    context.Module.Types.Add(item);
                    context.Module.Types.Add(item2);
                }
            }
        }
    }
}
