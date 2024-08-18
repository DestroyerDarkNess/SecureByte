using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Linq;
using OpCode = dnlib.DotNet.Emit.OpCode;
using ReflOpCode = System.Reflection.Emit.OpCode;
using OpCodes = dnlib.DotNet.Emit.OpCodes;
using ReflOpCodes = System.Reflection.Emit.OpCodes;

namespace ICore
{
	internal static class DnlibUtils
	{
        public static byte[] GetILAsByteArray(this CilBody body)
        {
            List<byte> list = new List<byte>();
            foreach (Instruction instruction in body.Instructions)
            {
                byte[] bytes = BitConverter.GetBytes(instruction.GetOpCode().Value);
                byte[] array = bytes;
                foreach (byte item in array)
                {
                    list.Add(item);
                }
            }
            return list.ToArray();
        }
        public static void EnsureNoInlining(MethodDef method)
        {
            method.ImplAttributes &= ~MethodImplAttributes.AggressiveInlining;
            method.ImplAttributes |= MethodImplAttributes.NoInlining;
        }
        public static void HideMethods(MethodDef methodDef)
        {
            methodDef.Body.Instructions.Insert(1, new Instruction(OpCodes.Br_S, methodDef.Body.Instructions[1]));
            methodDef.Body.Instructions.Insert(2, new Instruction(OpCodes.Unaligned, 0));
        }
        public static void InsertInstructions(IList<Instruction> instructions, Dictionary<Instruction, int> keyValuePairs)
        {
            foreach (KeyValuePair<Instruction, int> kv in keyValuePairs)
            {
                Instruction instruction = kv.Key;
                int index = kv.Value;
                instructions.Insert(index, instruction);
            }
        }
        public static MethodDef ResolveThrow(this IMethod method)
        {
            var def = method as MethodDef;
            if (def != null)
                return def;

            var spec = method as MethodSpec;
            if (spec != null)
                return spec.Method.ResolveThrow();

            return ((MemberRef)method).ResolveMethodThrow();
        }
        public static bool hasExceptionHandlers(MethodDef methodDef)
        {
            if (methodDef.Body.HasExceptionHandlers)
                return true;
            return false;
        }
        public static bool HasUnsafeInstructions(MethodDef methodDef)
        {
            bool hasBody = methodDef.HasBody;
            if (hasBody)
            {
                bool hasVariables = methodDef.Body.HasVariables;
                if (hasVariables)
                {
                    return methodDef.Body.Variables.Any((Local x) => x.Type.IsPointer);
                }
            }
            return false;
        }
    
        public static List<Instruction> Calc(int value)
        {
            List<Instruction> list = new List<Instruction>();
            System.Random random = new System.Random(Guid.NewGuid().GetHashCode());
            int num = random.Next(0, 100000);
            int num2 = random.Next(0, 100000);
            bool flag = Convert.ToBoolean(random.Next(0, 2));
            list.Add(Instruction.Create(OpCodes.Ldc_I4, value - num + (flag ? (0 - num2) : num2)));
            list.Add(Instruction.Create(OpCodes.Ldc_I4, num));
            list.Add(Instruction.Create(OpCodes.Add));
            list.Add(Instruction.Create(OpCodes.Ldc_I4, num2));
            list.Add(Instruction.Create(flag ? OpCodes.Add : OpCodes.Sub));
            return list;
        }
        public static bool Simplify(MethodDef methodDef)
        {
            if (methodDef.Parameters == null)
                return false;
            methodDef.Body.SimplifyMacros(methodDef.Parameters);
            methodDef.Body.SimplifyBranches();
            return true;
        }
        public static bool Optimize(MethodDef methodDef)
        {
            if (methodDef.Body == null)
                return false;
            methodDef.Body.OptimizeMacros();
            methodDef.Body.OptimizeBranches();
            //methodDef.Body.SimplifyBranches();
            return true;
        }
        public static bool InGlobalModuleType(this MethodDef method)
        {
            return method.DeclaringType.IsGlobalModuleType || method.DeclaringType2.IsGlobalModuleType || method.FullName.Contains("My.");
        }
        public static bool InGlobalModuleType(this TypeDef type)
        {
            return type.IsGlobalModuleType || (type.DeclaringType != null && type.DeclaringType.IsGlobalModuleType) || (type.DeclaringType2 != null && type.DeclaringType2.IsGlobalModuleType);
        }
        public static bool IsDelegate(this TypeDef type)
        {
            if (type.BaseType == null)
            {
                return false;
            }
            string fullName = type.BaseType.FullName;
            return fullName == "System.Delegate" || fullName == "System.MulticastDelegate";
        }
        public static void MergeCall(this CilBody targetBody, Instruction callInstruction)
        {
            if (!(callInstruction.Operand is MethodDef methodToMerge))
                throw new ArgumentException("Call instruction has invalid operand");
            if (!methodToMerge.HasBody)
                throw new Exception("Method to merge has no body!");

            var localParams = methodToMerge.Parameters.ToDictionary(param => param.Index, param => new Local(param.Type));
            var localMap = methodToMerge.Body.Variables.ToDictionary(local => local, local => new Local(local.Type));
            foreach (var local in localParams)
                targetBody.Variables.Add(local.Value);
            foreach (var local in localMap)
                targetBody.Variables.Add(local.Value);

            // Nop the call
            int index = targetBody.Instructions.IndexOf(callInstruction) + 1;
            callInstruction.OpCode = OpCodes.Nop;
            callInstruction.Operand = null;
            var afterIndex = targetBody.Instructions[index];

            // Find Exception handler index
            int exIndex = 0;
            foreach (var ex in targetBody.ExceptionHandlers)
            {
                if (targetBody.Instructions.IndexOf(ex.TryStart) < index)
                    exIndex = targetBody.ExceptionHandlers.IndexOf(ex);
            }

            // setup parameter locals
            foreach (var paramLocal in localParams.Reverse())
            {
                targetBody.Instructions.Insert(index++, new Instruction(OpCodes.Stloc, paramLocal.Value));
            }

            var instrMap = new Dictionary<Instruction, Instruction>();
            var newInstrs = new List<Instruction>();

            // Transfer instructions to list
            foreach (var instr in methodToMerge.Body.Instructions)
            {
                Instruction newInstr;
                if (instr.OpCode == OpCodes.Ret)
                {
                    newInstr = new Instruction(OpCodes.Br, afterIndex);
                }
                else if (instr.IsLdarg())
                {
                    localParams.TryGetValue(instr.GetParameterIndex(), out var lc);
                    newInstr = new Instruction(OpCodes.Ldloc, lc);
                }
                else if (instr.IsStarg())
                {
                    localParams.TryGetValue(instr.GetParameterIndex(), out var lc);
                    newInstr = new Instruction(OpCodes.Stloc, lc);
                }
                else if (instr.IsLdloc())
                {
                    localMap.TryGetValue(instr.GetLocal(methodToMerge.Body.Variables), out var lc);
                    newInstr = new Instruction(OpCodes.Ldloc, lc);
                }
                else if (instr.IsStloc())
                {
                    localMap.TryGetValue(instr.GetLocal(methodToMerge.Body.Variables), out var lc);
                    newInstr = new Instruction(OpCodes.Stloc, lc);
                }
                else
                {
                    newInstr = new Instruction(instr.OpCode, instr.Operand);
                }

                newInstrs.Add(newInstr);
                instrMap[instr] = newInstr;
            }

            // Fix branch targets & add instructions
            foreach (var instr in newInstrs)
            {
                if (instr.Operand != null && instr.Operand is Instruction instrOp && instrMap.ContainsKey(instrOp))
                    instr.Operand = instrMap[instrOp];
                else if (instr.Operand is Instruction[] instructionArrayOp)
                    instr.Operand = instructionArrayOp.Select(target => instrMap[target]).ToArray();

                targetBody.Instructions.Insert(index++, instr);
            }

            // Add Exception Handlers
            foreach (var eh in methodToMerge.Body.ExceptionHandlers)
            {
                targetBody.ExceptionHandlers.Insert(++exIndex, new ExceptionHandler(eh.HandlerType)
                {
                    CatchType = eh.CatchType,
                    TryStart = instrMap[eh.TryStart],
                    TryEnd = instrMap[eh.TryEnd],
                    HandlerStart = instrMap[eh.HandlerStart],
                    HandlerEnd = instrMap[eh.HandlerEnd],
                    FilterStart = eh.FilterStart == null ? null : instrMap[eh.FilterStart]
                });
            }
        }
        public static System.Reflection.Emit.OpCode ToReflectionOp(this dnlib.DotNet.Emit.OpCode op)
        {
            Code code = op.Code;
            if (code <= Code.Ldc_I4)
            {
                if (code == Code.Ldarg_0)
                {
                    return System.Reflection.Emit.OpCodes.Ldarg_0;
                }
                if (code == Code.Ldc_I4)
                {
                    return System.Reflection.Emit.OpCodes.Ldc_I4;
                }
            }
            else
            {
                if (code == Code.Ret)
                {
                    return System.Reflection.Emit.OpCodes.Ret;
                }
                switch (code)
                {
                    case Code.Add:
                        return System.Reflection.Emit.OpCodes.Add;
                    case Code.Sub:
                        return System.Reflection.Emit.OpCodes.Sub;
                    case Code.Mul:
                        return System.Reflection.Emit.OpCodes.Mul;
                    case Code.And:
                        return System.Reflection.Emit.OpCodes.And;
                    case Code.Or:
                        return System.Reflection.Emit.OpCodes.Or;
                    case Code.Xor:
                        return System.Reflection.Emit.OpCodes.Xor;
                }
            }
            throw new NotImplementedException();
        }
        public static IEnumerable<IDnlibDef> FindDefinitions(this ModuleDef module)
		{
			yield return module;
			foreach (TypeDef type in module.GetTypes())
			{
				yield return type;
				foreach (MethodDef method in type.Methods)
				{
					yield return method;
				}
				IEnumerator<MethodDef> enumerator2 = null;
				foreach (FieldDef field in type.Fields)
				{
					yield return field;
				}
				IEnumerator<FieldDef> enumerator3 = null;
				foreach (PropertyDef prop in type.Properties)
				{
					yield return prop;
				}
				IEnumerator<PropertyDef> enumerator4 = null;
				foreach (EventDef evt in type.Events)
				{
					yield return evt;
				}
				IEnumerator<EventDef> enumerator5 = null;
				//type = null;
			}
			IEnumerator<TypeDef> enumerator = null;
			yield break;
			yield break;
		}
		public static IEnumerable<IDnlibDef> FindDefinitions(this TypeDef typeDef)
		{
			yield return typeDef;
			foreach (TypeDef nestedType in typeDef.NestedTypes)
			{
				yield return nestedType;
			}
			IEnumerator<TypeDef> enumerator = null;
			foreach (MethodDef method in typeDef.Methods)
			{
				yield return method;
			}
			IEnumerator<MethodDef> enumerator2 = null;
			foreach (FieldDef field in typeDef.Fields)
			{
				yield return field;
			}
			IEnumerator<FieldDef> enumerator3 = null;
			foreach (PropertyDef prop in typeDef.Properties)
			{
				yield return prop;
			}
			IEnumerator<PropertyDef> enumerator4 = null;
			foreach (EventDef evt in typeDef.Events)
			{
				yield return evt;
			}
			IEnumerator<EventDef> enumerator5 = null;
			yield break;
			yield break;
		}
        public static bool IsTypePublic(this TypeDef type)
        {
            while (type.IsPublic || type.IsNestedFamily || type.IsNestedFamilyAndAssembly || type.IsNestedFamilyOrAssembly || type.IsNestedPublic || type.IsPublic)
            {
                type = type.DeclaringType;
                if (type == null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
