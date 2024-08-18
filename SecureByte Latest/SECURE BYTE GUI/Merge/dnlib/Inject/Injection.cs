using System.Linq;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace dnlib.Inject
{
    public class Injection
    {
        private readonly InjectMapper _mapper;

        public Injection(ModuleDef newModule)
        {
            _mapper = new InjectMapper(newModule);
        }

        public void Inject(TypeDef typeDef)
        {
            var newTypeDef = Init(typeDef);
            _mapper.TargetModule.Types.Add(newTypeDef);
        }

        public void Inject(TypeDef typeDef, out TypeDef newTypeDef)
        {
            newTypeDef = Init(typeDef);
        }

        private TypeDef Init(TypeDef typeDef)
        {
            TypeDef newTypeDef;
            IDnlibDef existing;
            if (!_mapper.DnlibDefMaps.TryGetValue(typeDef, out existing))
            {
                newTypeDef = Clone(typeDef);
                _mapper.DnlibDefMaps[typeDef] = newTypeDef;

                foreach (TypeDef nestedType in typeDef.NestedTypes)
                {
                    newTypeDef.NestedTypes.Add(Init(nestedType));
                }

                foreach (MethodDef method in typeDef.Methods)
                {
                    var newMethodDef = Clone(method);
                    newTypeDef.Methods.Add(newMethodDef);

                    _mapper.DnlibDefMaps[method] = newMethodDef;
                }

                foreach (FieldDef field in typeDef.Fields)
                {
                    var newFieldDef = Clone(field);
                    newTypeDef.Fields.Add(newFieldDef);

                    _mapper.DnlibDefMaps[field] = newFieldDef;
                }

                foreach (var property in typeDef.Properties)
                {
                    var newPropertyDef = Clone(property);
                    newTypeDef.Properties.Add(newPropertyDef);

                    _mapper.DnlibDefMaps[property] = newPropertyDef;
                }

                Copy(typeDef, true);
            }
            else
                newTypeDef = (TypeDef)existing;

            return newTypeDef;
        }

        #region Clone

        /// <summary>
        ///     Clones the specified origin TypeDef.
        /// </summary>
        /// <param name="origin">The origin TypeDef.</param>
        /// <returns>The cloned TypeDef.</returns>
        private TypeDefUser Clone(TypeDef origin)
        {
            var typeDef = new TypeDefUser(origin.Namespace, origin.Name, _mapper.Importer.Import(origin.BaseType));
            typeDef.Attributes = origin.Attributes;

            if (origin.ClassLayout != null)
                typeDef.ClassLayout = new ClassLayoutUser(origin.ClassLayout.PackingSize, origin.ClassSize);

            foreach (GenericParam genericParam in origin.GenericParameters)
                typeDef.GenericParameters.Add(new GenericParamUser(genericParam.Number, genericParam.Flags, "-"));

            return typeDef;
        }

        /// <summary>
        ///     Clones the specified origin MethodDef.
        /// </summary>
        /// <param name="origin">The origin MethodDef.</param>
        /// <returns>The cloned MethodDef.</returns>
        private MethodDefUser Clone(MethodDef origin)
        {
            var methodDef = new MethodDefUser(origin.Name, _mapper.Importer.Import(origin.MethodSig), origin.ImplAttributes, origin.Attributes);

            foreach (GenericParam genericParam in origin.GenericParameters)
                methodDef.GenericParameters.Add(new GenericParamUser(genericParam.Number, genericParam.Flags, "-"));

            return methodDef;
        }

        /// <summary>
        ///     Clones the specified origin MethodDef.
        /// </summary>
        /// <param name="origin">The origin MethodDef.</param>
        /// <returns>The cloned MethodDef.</returns>
        public PropertyDefUser Clone(PropertyDef origin)
        {
            return new PropertyDefUser(origin.Name, _mapper.Importer.Import(origin.PropertySig), origin.Attributes);
        }

        /// <summary>
        ///     Clones the specified origin FieldDef.
        /// </summary>
        /// <param name="origin">The origin FieldDef.</param>
        /// <returns>The cloned FieldDef.</returns>
        private FieldDefUser Clone(FieldDef origin)
        {
            return new FieldDefUser(origin.Name, _mapper.Importer.Import(origin.FieldSig), origin.Attributes);
        }

        #endregion

        #region Copy

        /// <summary>
        ///     Copies the information from the origin type to injected type.
        /// </summary>
        /// <param name="typeDef">The origin TypeDef.</param>
        private void CopyTypeDef(TypeDef typeDef)
        {
            var newTypeDef = (TypeDef)_mapper.DnlibDefMaps[typeDef];

            newTypeDef.BaseType = _mapper.Importer.Import(typeDef.BaseType);

            foreach (InterfaceImpl iface in typeDef.Interfaces)
                newTypeDef.Interfaces.Add(new InterfaceImplUser(_mapper.Importer.Import(iface.Interface)));
        }

        /// <summary>
        ///     Copies the information from the origin method to injected method.
        /// </summary>
        /// <param name="methodDef">The origin MethodDef.</param>
        private void CopyMethodDef(MethodDef methodDef)
        {
            var newMethodDef = (MethodDef)_mapper.DnlibDefMaps[methodDef];

            newMethodDef.Signature = _mapper.Importer.Import(methodDef.Signature);
            newMethodDef.Parameters.UpdateParameterTypes();

            if (methodDef.ImplMap != null)
                newMethodDef.ImplMap = new ImplMapUser(new ModuleRefUser(_mapper.TargetModule, methodDef.ImplMap.Module.Name), methodDef.ImplMap.Name, methodDef.ImplMap.Attributes);

            foreach (CustomAttribute ca in methodDef.CustomAttributes)
                newMethodDef.CustomAttributes.Add(new CustomAttribute((ICustomAttributeType)_mapper.Importer.Import(ca.Constructor)));

            if (methodDef.HasBody)
            {
                newMethodDef.Body = new CilBody(methodDef.Body.InitLocals, new List<Instruction>(), new List<ExceptionHandler>(), new List<Local>());
                newMethodDef.Body.MaxStack = methodDef.Body.MaxStack;

                var bodyMap = new Dictionary<object, object>();

                foreach (Local local in methodDef.Body.Variables)
                {
                    var newLocal = new Local(_mapper.Importer.Import(local.Type));
                    newMethodDef.Body.Variables.Add(newLocal);
                    bodyMap[local] = newLocal;
                }

                foreach (Instruction instr in methodDef.Body.Instructions)
                {
                    var newInstr = new Instruction(instr.OpCode, instr.Operand);
                    if (newInstr.Operand is IType)
                        newInstr.Operand = _mapper.Importer.Import((IType)newInstr.Operand);

                    else if (newInstr.Operand is IMethod)
                        newInstr.Operand = _mapper.Importer.Import((IMethod)newInstr.Operand);

                    else if (newInstr.Operand is IField)
                        newInstr.Operand = _mapper.Importer.Import((IField)newInstr.Operand);

                    newMethodDef.Body.Instructions.Add(newInstr);
                    bodyMap[instr] = newInstr;
                }

                foreach (Instruction instr in newMethodDef.Body.Instructions)
                {
                    if (instr.Operand != null && bodyMap.ContainsKey(instr.Operand))
                        instr.Operand = bodyMap[instr.Operand];

                    else if (instr.Operand is Instruction[])
                        instr.Operand = ((Instruction[])instr.Operand).Select(target => (Instruction)bodyMap[target]).ToArray();
                }

                foreach (ExceptionHandler eh in methodDef.Body.ExceptionHandlers)
                    newMethodDef.Body.ExceptionHandlers.Add(new ExceptionHandler(eh.HandlerType)
                    {
                        CatchType = eh.CatchType == null ? null : _mapper.Importer.Import(eh.CatchType),
                        TryStart = (Instruction)bodyMap[eh.TryStart],
                        TryEnd = (Instruction)bodyMap[eh.TryEnd],
                        HandlerStart = (Instruction)bodyMap[eh.HandlerStart],
                        HandlerEnd = (Instruction)bodyMap[eh.HandlerEnd],
                        FilterStart = eh.FilterStart == null ? null : (Instruction)bodyMap[eh.FilterStart]
                    });

                newMethodDef.Body.SimplifyMacros(newMethodDef.Parameters);
            }
        }

        /// <summary>
        ///     Copies the information from the origin field to injected field.
        /// </summary>
        /// <param name="fieldDef">The origin FieldDef.</param>
        private void CopyFieldDef(FieldDef fieldDef)
        {
            var newFieldDef = (FieldDef)_mapper.DnlibDefMaps[fieldDef];
            newFieldDef.Signature = _mapper.Importer.Import(fieldDef.Signature);
        }

        /// <summary>
        ///     Copies the information from the origin property to injected property.
        /// </summary>
        /// <param name="propertyDef">The origin PropertyDef.</param>
        private void CopyPropertyDef(PropertyDef propertyDef)
        {
            var newPropertyDef = (PropertyDef)_mapper.DnlibDefMaps[propertyDef];

            if (propertyDef.SetMethod.HasBody)
                newPropertyDef.SetMethod = newPropertyDef.DeclaringType.Methods.Where(m => m.Name == $"set_{propertyDef.Name}").First();
            if (propertyDef.GetMethod.HasBody)
                newPropertyDef.GetMethod = newPropertyDef.DeclaringType.Methods.Where(m => m.Name == $"get_{propertyDef.Name}").First();
        }

        /// <summary>
        ///     Copies the information to the injected definitions.
        /// </summary>
        /// <param name="typeDef">The origin TypeDef.</param>
        /// <param name="copySelf">if set to <c>true</c>, copy information of <paramref name="typeDef" />.</param>
        private void Copy(TypeDef typeDef, bool copySelf)
        {
            if (copySelf)
                CopyTypeDef(typeDef);

            foreach (TypeDef nestedType in typeDef.NestedTypes)
                Copy(nestedType, true);

            foreach (MethodDef method in typeDef.Methods)
                CopyMethodDef(method);

            foreach (FieldDef field in typeDef.Fields)
                CopyFieldDef(field);

            foreach (PropertyDef propertyDef in typeDef.Properties)
                CopyPropertyDef(propertyDef);
        }

        #endregion
    }
}
