using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TypeAttributes = dnlib.DotNet.TypeAttributes;

namespace Helpers.Injection
{
    public class newInjector
    {
        private List<IDnlibDef> Members { get; set; }
        private Type RuntimeType { get; set; }
        public IDnlibDef FindMember(string name)
        {
            foreach (var member in Members)
                if (member.Name == name)
                    return member;
            throw new Exception("Error to find member.");
        }
        public newInjector(ModuleDefMD module, Type type, bool injectType = true)
        {
            RuntimeType = type;
            Members = new List<IDnlibDef>();
            if (injectType)
                InjectType(module);
        }
        public void InjectType(ModuleDefMD module)
        {
            var typeModule = ModuleDefMD.Load(RuntimeType.Module);
            var typeDefs = typeModule.ResolveTypeDef(MDToken.ToRID(RuntimeType.MetadataToken));
            Members.AddRange(InjectHelper.Inject(typeDefs, module.GlobalType, module).ToList());
        }
        public void injectMethod(string Namespace, string Name, ModuleDefMD module, MethodDef method)
        {
            TypeDef newClass = new TypeDefUser(Namespace, Name, module.CorLibTypes.Object.TypeDefOrRef)
            {
                Attributes = TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.Class | TypeAttributes.AnsiClass
            };
            module.Types.Add(newClass);
            method.DeclaringType = null;
            newClass.Methods.Add(method);
        }
        public void injectMethods(string Namespace, string Name, ModuleDefMD module, MethodDef[] methods)
        {
            TypeDef newClass = new TypeDefUser(Namespace, Name, module.CorLibTypes.Object.TypeDefOrRef)
            {
                Attributes = TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.Class | TypeAttributes.AnsiClass
            };
            module.Types.Add(newClass);
            foreach (var m in methods)
            {
                m.DeclaringType = null;
                newClass.Methods.Add(m);
            }
        }
        public void Rename()
        {
            foreach (var mem in Members)
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
