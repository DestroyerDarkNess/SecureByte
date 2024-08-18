using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using ICore;

namespace Protections.Entrypoint
{
    public class MoveEntryPoint
    {
        public static void MoveMethod(MethodDef method)
        {
            if (method == null)
                return;
            if (method.FullName.Contains("My."))
                return;
            if (!method.HasBody)
                return;
            if (!method.Body.HasInstructions)
                return;
            List<Instruction> instructions = new List<Instruction>();
            instructions.AddRange(method.Body.Instructions);
            MethodDef newMethod = new MethodDefUser(Utils.MethodsRenamig(), method.MethodSig, method.Attributes) { Body = new CilBody(method.Body.InitLocals, new List<Instruction>(), new List<ExceptionHandler>(), new List<Local>()) { MaxStack = method.Body.MaxStack } };
            foreach (ParamDef paramDef in method.ParamDefs)
            {
                newMethod.ParamDefs.Add(new ParamDefUser(paramDef.Name, paramDef.Sequence, paramDef.Attributes));
            }
            newMethod.Parameters.UpdateParameterTypes();
            int index = 0;
            if (method.HasParamDefs && method.Parameters != null && method.Parameters.Count > 0)
            {
                newMethod.Parameters[index].CreateParamDef();
                newMethod.Parameters[index].Name = method.Parameters[index].Name;
                newMethod.Parameters[index].Type = method.Parameters[index].Type;
                index++;
            }
            newMethod.Parameters.UpdateParameterTypes();
            foreach (CustomAttribute ca in method.CustomAttributes)
            {
                newMethod.CustomAttributes.Add(new CustomAttribute((ICustomAttributeType)ca.Constructor));
            }
            if (method.ImplMap != null)
            {
                newMethod.ImplMap = new ImplMapUser(new ModuleRefUser(method.Module, method.ImplMap.Module.Name), method.ImplMap.Name, method.ImplMap.Attributes);
            }
            Dictionary<object, object> bodyMap = new Dictionary<object, object>();
            foreach (Local local in method.Body.Variables)
            {
                Local newLocal = new Local(local.Type);
                newMethod.Body.Variables.Add(newLocal);
                newLocal.Name = local.Name;
                bodyMap[local] = newLocal;
            }
            foreach (Instruction instr in method.Body.Instructions)
            {
                Instruction newInstr = new Instruction(instr.OpCode, instr.Operand)
                {
                    SequencePoint = instr.SequencePoint
                };
                switch (newInstr.Operand)
                {
                    case IType type:
                        newInstr.Operand = type;
                        break;
                    case IMethod theMethod:
                        newInstr.Operand = theMethod;
                        break;
                    case IField field:
                        newInstr.Operand = field;
                        break;
                }
                newMethod.Body.Instructions.Add(newInstr);
                bodyMap[instr] = newInstr;
            }
            foreach (Instruction instr in newMethod.Body.Instructions)
            {
                if (instr.Operand != null && bodyMap.ContainsKey(instr.Operand))
                {
                    instr.Operand = bodyMap[instr.Operand];
                }
                else if (instr.Operand is Instruction[] theInstructions)
                {
                    instr.Operand = theInstructions.Select(target => (Instruction)bodyMap[target]).ToArray();
                }
            }
            foreach (ExceptionHandler eh in method.Body.ExceptionHandlers)
            {
                newMethod.Body.ExceptionHandlers.Add(new ExceptionHandler(eh.HandlerType)
                {
                    CatchType = eh.CatchType == null ? null : eh.CatchType,
                    TryStart = (Instruction)bodyMap[eh.TryStart],
                    TryEnd = (Instruction)bodyMap[eh.TryEnd],
                    HandlerStart = (Instruction)bodyMap[eh.HandlerStart],
                    HandlerEnd = (Instruction)bodyMap[eh.HandlerEnd],
                    FilterStart = eh.FilterStart == null ? null : (Instruction)bodyMap[eh.FilterStart]
                });
            }
            newMethod.Body.SimplifyMacros(newMethod.Parameters);
            method.DeclaringType.Methods.Add(newMethod);
            method.Body.Instructions.Clear();
            if (method.HasParamDefs && method.Parameters != null && method.Parameters.Count > 0)
            {
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Nop));
                int current = 0;
                foreach (Parameter parameter in method.Parameters)
                {
                    if (parameter.Name != null && parameter.Name != "")
                    {
                        if (current == 0)
                        {
                            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                        }
                        else if (current == 1)
                        {
                            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
                        }
                        else if (current == 2)
                        {
                            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
                        }
                        else if (current == 3)
                        {
                            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_3));
                        }
                        else
                        {
                            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_S, current));
                        }
                        current++;
                    }
                }
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, newMethod));
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            }
            else
            {
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Nop));
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, newMethod));
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            }
        }
        public static void Execute(Context ctx)
        {
            MoveMethod(ctx.Module.EntryPoint);
        }
    }
}