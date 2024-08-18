using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace dnlib.Inject
{
    /// <summary>
    ///     Mapper of the injection process.
    /// </summary>
    internal class InjectMapper : ImportMapper
    {
        /// <summary>
        ///     The mapping of origin definitions to injected definitions.
        /// </summary>
        public Dictionary<IDnlibDef, IDnlibDef> DnlibDefMaps { get; private set; }

        /// <summary>
        ///     The mapping of origin definitions to injected definitions.
        /// </summary>
        public Dictionary<ITypeDefOrRef, ITypeDefOrRef> TypeDefOrRefMaps { get; private set; }

        /// <summary>
        ///     The mapping of origin definitions to injected definitions.
        /// </summary>
        public Dictionary<MemberRef, MemberRef> MemberRefMaps { get; private set; }

        /// <summary>
        ///     The mapping of origin definitions to injected definitions.
        /// </summary>
        public Dictionary<Type, TypeRef> TypeRefMaps { get; private set; }


        /// <summary>
        ///     The module which source type is being injected to.
        /// </summary>
        public ModuleDef TargetModule { get; private set; }

        /// <summary>
        ///     Gets the importer.
        /// </summary>
        /// <value>The importer.</value>
        public Importer Importer { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InjectMapper" /> class.
        /// </summary>
        /// <param name="target">The target module.</param>
        public InjectMapper(ModuleDef target)
        {
            DnlibDefMaps = new Dictionary<IDnlibDef, IDnlibDef>();
            TypeDefOrRefMaps = new Dictionary<ITypeDefOrRef, ITypeDefOrRef>();
            MemberRefMaps = new Dictionary<MemberRef, MemberRef>();
            TypeRefMaps = new Dictionary<Type, TypeRef>();
            TargetModule = target;
            Importer = new Importer(target, ImporterOptions.TryToUseTypeDefs, default(GenericParamContext), this);
        }

        public override ITypeDefOrRef Map(ITypeDefOrRef source)
        {
            if (TypeDefOrRefMaps.ContainsKey(source))
                return TypeDefOrRefMaps[source];
            return null;
        }

        public TypeDef Map(TypeDef typeDef)
        {
            if (DnlibDefMaps.ContainsKey(typeDef))
                return (TypeDef)DnlibDefMaps[typeDef];
            return null;
        }

        public override IField Map(FieldDef source)
        {
            if (DnlibDefMaps.ContainsKey(source))
                return (FieldDef)DnlibDefMaps[source];
            return null;
        }

        public override IMethod Map(MethodDef source)
        {
            if (DnlibDefMaps.ContainsKey(source))
                return (MethodDef)DnlibDefMaps[source];
            return null;
        }

        public PropertyDef Map(PropertyDef propertyDef)
        {
            if (DnlibDefMaps.ContainsKey(propertyDef))
                return (PropertyDef)DnlibDefMaps[propertyDef];
            return null;
        }

        public override MemberRef Map(MemberRef source)
        {
            if (MemberRefMaps.ContainsKey(source))
                return MemberRefMaps[source];
            return null;
        }
    }
}
