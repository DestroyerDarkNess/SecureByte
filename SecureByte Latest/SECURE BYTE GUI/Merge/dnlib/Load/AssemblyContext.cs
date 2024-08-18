using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Resources;
using dnlib.Inject;

namespace dnlib.Load
{
	public class AssemblyContext
    {
        private ResourceManagement _resources = null;

        public AssemblyDef Assembly { get; private set; }
        public ModuleDef Module { get; private set; }
        public ModuleDefMD ModuleMD { get; private set; }
        public Importer Importer { get; private set; }
        public TypeDef EntryPointType { get; private set; }
        public MethodDef EntryPointMethod { get; private set; }
        public TypeDef GlobalType { get; private set; }
        public MethodDef GlobalTypeStaticConstructor { get; private set; }
        public ResourceManagement Resources { get { return _resources; } }

        public AssemblyContext(ModuleDefMD module)
		{
            ModuleMD = module;

            Assembly = module.Assembly;

            Module = Assembly.ManifestModule;

            Importer = new Importer(Module);

            EntryPointMethod = Module.EntryPoint;
            if(EntryPointMethod != null)
                EntryPointType = Module.EntryPoint.DeclaringType;

            GlobalType = Module.GlobalType;
            GlobalTypeStaticConstructor = GlobalType.FindOrCreateStaticConstructor();

            _resources = new ResourceManagement(this);
		}

        public TypeDef GetTypeDef(Type type)
        {
            var typeModule = ModuleDefMD.Load(type.Module);
            var typeDef = typeModule.ResolveTypeDef(MDToken.ToRID(type.MetadataToken));
            return typeDef;
        }

        public AssemblyRefUser GetAssemblyRefUser(string path)
        {
            var assembly = System.Reflection.Assembly.LoadFile(path);
            var assemblyName = assembly.GetName();
            var assemblyRefUser = new AssemblyRefUser(assemblyName);
            return assemblyRefUser;
        }

        public IEnumerable<AssemblyRef> GetAssemblyRefs()
        {
            return Module.GetAssemblyRefs();
        }

        public void Merge(Type type)
        {
            var typeDef = GetTypeDef(type);
            Injection injection = new Injection(Module);
            injection.Inject(typeDef);
        }

        public void Merge(TypeDef typeDef)
        {
            Injection injection = new Injection(Module);
            injection.Inject(typeDef);
        }

        public TypeDef CopyTypeDef(TypeDef typeDef)
        {
            TypeDef newTypeDef;
            Injection injection = new Injection(Module);
            injection.Inject(typeDef, out newTypeDef);
            return newTypeDef;
        }

        public TypeDef CopyTypeDef(TypeDef typeDef, string name)
        {
            TypeDef newTypeDef;
            Injection injection = new Injection(Module);
            injection.Inject(typeDef, out newTypeDef);

            newTypeDef.Name = name;

            return newTypeDef;
        }

        public TypeDef CopyTypeDef(TypeDef typeDef, string name, string nameSpace)
        {
            TypeDef newTypeDef;
            Injection injection = new Injection(Module);
            injection.Inject(typeDef, out newTypeDef);

            newTypeDef.Name = name;
            newTypeDef.Namespace = nameSpace;

            return newTypeDef;
        }
    }
}
