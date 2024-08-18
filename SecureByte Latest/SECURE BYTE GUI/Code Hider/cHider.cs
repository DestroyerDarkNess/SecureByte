using dnlib.DotNet;
using System;
using System.Linq;
using ICore;
namespace Code_Hider
{
    internal static class cHider
    {
        public static void Hide(ModuleDefMD module)
        {
            string text = null;
            foreach (TypeDef typeDef in module.Types.Where(t => !t.IsGlobalModuleType && !t.Namespace.Contains("My") && t.Interfaces.Count == 0 && !t.IsSpecialName && !t.IsRuntimeSpecialName))
            {
                if (typeDef.Namespace != $"{module.Assembly.Name}.Properties")
                {
                    if (typeDef.IsPublic)
                        text = typeDef.Name;
                    if (!typeDef.Name.Contains("PrivateImplementationDetails"))
                        typeDef.Name = $"<{typeDef.Name}>";
                    foreach (MethodDef methodDef in typeDef.Methods.Where(m => !m.DeclaringType.IsForwarder && !m.IsFamily && !m.IsRuntimeSpecialName && !m.DeclaringType.IsForwarder))
                    {
                        methodDef.CustomAttributes.Add(new CustomAttribute(new MemberRefUser(module, "<" + Utils.MethodsRenamig() + ">", MethodSig.CreateInstance(module.Import(typeof(void)).ToTypeSig(true)), module.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "CompilerGeneratedAttribute"))));
                        methodDef.Name = "<" + Utils.MethodsRenamig() + ">";
                        foreach (Parameter parameter in methodDef.Parameters)
                            parameter.Name = "<" + Utils.MethodsRenamig() + ">";
                        if (typeDef.Name.Contains(methodDef.Name))
                            methodDef.Name = typeDef.Name;
                    }
                    foreach (FieldDef fieldDef in typeDef.Fields.Where(f => !f.DeclaringType.IsEnum && !f.DeclaringType.IsForwarder && !f.IsRuntimeSpecialName && !f.DeclaringType.IsEnum))
                        fieldDef.Name = "<" + Utils.MethodsRenamig() + ">";
                    foreach (EventDef eventDef in typeDef.Events.Where(e => !e.DeclaringType.IsForwarder && !e.IsRuntimeSpecialName))
                        eventDef.Name = "<" + Utils.MethodsRenamig() + ">";
                    if (typeDef.IsPublic)
                        foreach (Resource resource in module.Resources.Where(r => r.Name.Contains(text)))
                            resource.Name = resource.Name.Replace(text, typeDef.Name);
                }
            }
        }
    }
}
