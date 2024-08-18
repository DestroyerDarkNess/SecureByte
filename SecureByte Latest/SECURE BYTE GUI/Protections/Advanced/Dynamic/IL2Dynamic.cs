﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Reflection;
using System.Reflection.Emit;
using OpCode = dnlib.DotNet.Emit.OpCode;
using ReflOpCode = System.Reflection.Emit.OpCode;
using OpCodes = dnlib.DotNet.Emit.OpCodes;
using ReflOpCodes = System.Reflection.Emit.OpCodes;
using ROpCode = System.Reflection.Emit.OpCode;
using ROpCodes = System.Reflection.Emit.OpCodes;
using OperandType = dnlib.DotNet.Emit.OperandType;
using ICore;

namespace IlDyn
{
    class IL2Dynamic
    {
        public void Execute(ModuleDefMD Module)
        {
            foreach(var t in Module.Types)
            {
                foreach(var m in t.Methods)
                {
                    if (m == Module.GlobalType.FindOrCreateStaticConstructor())
                    {
                        ConvertToDynamic(m, Module);
                    }
                }
            }
        }
        public void ConvertToDynamic(MethodDef method, ModuleDef module)
        {
            try
            {
                AssemblyDef ctx = module.Assembly;
                Utils.LoadOpCodes();
                Utils2.LoadOpCodes();

                TypeDef type = method.DeclaringType;


                Instruction[] oldInstructions = method.Body.Instructions.ToArray();
                Instruction[] instructions = null;
                Local local = new Local(ctx.ManifestModule.Import(typeof(List<Type>)).ToTypeSig());
                Local local2 = new Local(ctx.ManifestModule.Import(typeof(DynamicMethod)).ToTypeSig());
                Local local3 = new Local(ctx.ManifestModule.Import(typeof(ILGenerator)).ToTypeSig());
                Local local4 = new Local(ctx.ManifestModule.Import(typeof(Label[])).ToTypeSig());
                TypeSig ReturnType = method.ReturnType;
                Local[] oldLocals = method.Body.Variables.ToArray();
                List<Local> outLocals = new List<Local>();
                if (method.Name != ".ctor")
                    if (method.HasParamDefs)
                        instructions = BuildInstruction(method.Body.Instructions.ToArray(), type, method, method.ParamDefs[0].DeclaringMethod.MethodSig.Params, method.ReturnType.ToTypeDefOrRef(), method.Parameters.ToArray(), type, local, local2, local3, local4, oldLocals, oldInstructions, ctx, false, out outLocals, ReturnType);
                    else
                        instructions = BuildInstruction(method.Body.Instructions.ToArray(), type, method, null, method.ReturnType.ToTypeDefOrRef(), method.Parameters.ToArray(), type, local, local2, local3, local4, oldLocals, oldInstructions, ctx, false, out outLocals, ReturnType);
                else
                    if (method.HasParamDefs)
                    instructions = BuildInstruction(method.Body.Instructions.ToArray(), type, method, method.ParamDefs[0].DeclaringMethod.MethodSig.Params, method.ReturnType.ToTypeDefOrRef(), method.Parameters.ToArray(), type, local, local2, local3, local4, oldLocals, oldInstructions, ctx, true, out outLocals, ReturnType);
                else
                    instructions = BuildInstruction(method.Body.Instructions.ToArray(), type, method, null, method.ReturnType.ToTypeDefOrRef(), method.Parameters.ToArray(), type, local, local2, local3, local4, oldLocals, oldInstructions, ctx, true, out outLocals, ReturnType);
                method.Body.Instructions.Clear();
                method.Body.Variables.Add(local);
                method.Body.Variables.Add(local2);
                method.Body.Variables.Add(local3);
                method.Body.Variables.Add(local4);
                foreach (Local locals in outLocals)
                    method.Body.Variables.Add(locals);
                foreach (Instruction inst in instructions)
                {
                    method.Body.Instructions.Add(inst);
                }

            }
            catch { }
            
        }
        static Dictionary<int, int> counterList = new Dictionary<int, int>();
        public static Instruction[] BuildInstruction(Instruction[] toBuild, TypeDef typeDef, MethodDef method, IList<TypeSig> Param, ITypeDefOrRef type, IList<Parameter> pp, TypeDef typeM, Local local, Local local2, Local local3, Local local4, Local[] oldLocals, Instruction[] oldInstructions, AssemblyDef ctx, bool ISConstructorMethod, out List<Local> outLocals, TypeSig returnType)
        {
            List<Instruction> lista = new List<Instruction>();
            List<Local> variables = new List<Local>();
            lista.Add(OpCodes.Nop.ToInstruction());
            lista.Add(OpCodes.Ldc_I4.ToInstruction(9999));
            lista.Add(OpCodes.Newarr.ToInstruction(ctx.ManifestModule.Import(typeof(Label))));
            lista.Add(OpCodes.Stloc_S.ToInstruction(local4));

            lista.Add(OpCodes.Newobj.ToInstruction(ctx.ManifestModule.Import(typeof(List<Type>).GetConstructor(new Type[0]))));
            lista.Add(OpCodes.Stloc_S.ToInstruction(local));
            if (pp.ToArray().Count() != 0)
                if (pp[0] != null)
                {
                    lista.Add(OpCodes.Ldloc_S.ToInstruction(local));
                    lista.Add(OpCodes.Ldtoken.ToInstruction(pp[0].Type.ToTypeDefOrRef()));
                    lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(List<Type>).GetMethod("Add", new Type[] { typeof(Type) }))));
                }
            if (Param != null)
            {
                foreach (TypeSig p in Param)
                {
                    lista.Add(OpCodes.Ldloc_S.ToInstruction(local));
                    lista.Add(OpCodes.Ldtoken.ToInstruction(p.ToTypeDefOrRef()));
                    lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(List<Type>).GetMethod("Add", new Type[] { typeof(Type) }))));
                }
            }
            lista.Add(OpCodes.Ldstr.ToInstruction(ICore.Utils.MethodsRenamig()));
            lista.Add(OpCodes.Ldtoken.ToInstruction(type));
            lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
            lista.Add(OpCodes.Ldloc_S.ToInstruction(local));
            lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(List<Type>).GetMethod("ToArray", new Type[0]))));
            lista.Add(OpCodes.Ldtoken.ToInstruction(typeM));
            lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
            lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("get_Module"))));
            lista.Add(OpCodes.Ldc_I4_1.ToInstruction());
            lista.Add(OpCodes.Newobj.ToInstruction(ctx.ManifestModule.Import(typeof(DynamicMethod).GetConstructor(new Type[] { typeof(string), typeof(Type), typeof(Type[]), typeof(Module), typeof(bool) }))));
            lista.Add(OpCodes.Stloc_S.ToInstruction(local2));
            lista.Add(OpCodes.Ldloc_S.ToInstruction(local2));
            lista.Add(Instruction.Create(OpCodes.Callvirt, ctx.ManifestModule.Import(typeof(DynamicMethod).GetMethod("GetILGenerator", new Type[0]))));
            lista.Add(OpCodes.Stloc_S.ToInstruction(local3));
            if (ISConstructorMethod)
            {
                addLocal(new Local(ctx.ManifestModule.Import(typeDef).ToTypeSig()), local3, ref lista, ctx, ref variables);
            }
            if (oldLocals.Count() != 0)
            {
                foreach (Local nlocal in oldLocals)
                    addLocal(nlocal, local3, ref lista, ctx, ref variables);
                //lista.RemoveAt(lista.Count - 1);
            }
            List<Instruction> brTargets = new List<Instruction>();
            foreach (Instruction instruct in oldInstructions)
            {
                if (instruct.OpCode.OperandType == OperandType.InlineBrTarget || instruct.OpCode.OperandType == OperandType.ShortInlineBrTarget)
                {
                    brTargets.Add(instruct);
                    lista.Add(OpCodes.Ldloc_S.ToInstruction(local4));
                    lista.Add(OpCodes.Ldc_I4.ToInstruction((int)((Instruction)instruct.Operand).Offset));
                    lista.Add(OpCodes.Ldloc_S.ToInstruction(local3));
                    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("DefineLabel", new Type[0]))));
                    lista.Add(OpCodes.Stelem.ToInstruction(ctx.ManifestModule.Import(typeof(Label))));
                }
            }
            LocalsCount = 0;
            foreach (Instruction instruct in oldInstructions)
            {

                if (instruct.Operand != null)
                    ConvertInstructionWithOperand(instruct, local3, ref lista, variables, brTargets, ctx);
                else
                    ConvertInstruction(instruct, local3, ref lista, ctx);
            }
            lista.UpdateInstructionOffsets();
            var xD = new List<Instruction>();
            var xD2 = new List<Instruction>();
            var xD3 = new List<int>();
            var xD4 = new List<int>();
            foreach (Instruction instruct in lista)
                if (instruct.OpCode == OpCodes.Ldsfld)
                    xD.Add(instruct);

            foreach (Instruction instruct in oldInstructions)
                if (instruct.OpCode.OperandType == OperandType.InlineBrTarget || instruct.OpCode.OperandType == OperandType.ShortInlineBrTarget)
                {
                    xD2.Add(instruct);
                    Instruction tmp = ((Instruction)(((Instruction)instruct).Operand));
                    int eee = 0;
                    for (int i = 0; i < oldInstructions.Count(); i++)
                    {
                        if (oldInstructions[i].OpCode == tmp.OpCode)
                        {
                            eee++;
                            if (oldInstructions[i] == tmp)
                            {

                                xD3.Add(eee); break;
                            }
                        }
                    }
                    tmp = instruct;
                    int eeex = 0;
                    for (int i = 0; i < oldInstructions.Count(); i++)
                    {
                        if (oldInstructions[i].OpCode == tmp.OpCode)
                        {
                            eeex++;
                            if (oldInstructions[i] == tmp)
                            {

                                xD4.Add(eeex); break;
                            }
                        }
                    }

                }
            int v = 0;
            int chave = 0;
            int subChave = 0;
            string preventLocalIntCharge = "";
            int localInt = 0;

            for (int e = 0; e < xD2.Count; e++)
            {
                for (int iJ = 0; iJ < lista.Count; iJ++)
                {
                    if (lista[iJ].OpCode != OpCodes.Ldsfld) continue;
                    {
                        if (chave != 0)
                        {
                            chave--;
                            continue;
                        }
                        var tmp = ((Instruction)(((Instruction)xD2[e]).Operand)).ToString().Substring(9).ToLower();
                        var tmp2 = lista[iJ].ToString().Replace("System.Reflection.Emit.OpCode System.Reflection.Emit.OpCodes::", "").ToLower().Replace("_", ".").Substring(16);
                        if (tmp == tmp2)
                        {
                            if (preventLocalIntCharge != tmp)
                                localInt = 0;
                            localInt++;
                            preventLocalIntCharge = tmp;
                            //if (method.Name == "Dispose" && tmp2 == "nop" && localInt == 1) continue;
                            if (localInt == xD3[v])
                            {
                                localInt = 0;
                                subChave++;
                                chave = subChave;
                                v++;
                                int getIndex = iJ;

                                lista.Insert(getIndex - 1, OpCodes.Ldloc_S.ToInstruction(local3));
                                //+2
                                lista.Insert(getIndex, OpCodes.Ldloc_S.ToInstruction(local4));
                                //+2

                                //xD.Add(xxx++, (int)((Instruction)oldInstructions[e].Operand).Offset);
                                lista.Insert(getIndex + 1, OpCodes.Ldc_I4.ToInstruction(((int)((Instruction)xD2[e].Operand).Offset)));
                                //oldInstructions = oldInstructions.RemoveAt(e);
                                //oldInstructions = oldInstructions.RemoveAt(e);
                                //+2
                                lista.Insert(getIndex + 2, OpCodes.Ldelem.ToInstruction(ctx.ManifestModule.Import(typeof(Label))));
                                //+2
                                lista.Insert(getIndex + 3, OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("MarkLabel", new Type[] { typeof(Label) }))));
                                iJ += 3;

                                goto il_finale;
                            }
                        }
                        else
                        {
                            chave = subChave;
                        }

                    }
                il_finale:;
                }


            }
            v = 0;
            chave = 0;
            subChave = 0;
            preventLocalIntCharge = "";
            localInt = 0;
            for (int e = 0; e < xD2.Count; e++)
            {
                for (int iJ = 0; iJ < lista.Count; iJ++)
                {
                    if (lista[iJ].OpCode != OpCodes.Ldsfld) continue;
                    {
                        if (chave != 0)
                        {
                            chave--;
                            continue;
                        }
                        var tmp = xD2[e].OpCode.ToString().ToLower();
                        var tmp2 = lista[iJ].ToString().Replace("System.Reflection.Emit.OpCode System.Reflection.Emit.OpCodes::", "").ToLower().Replace("_", ".").Substring(16);
                        if (tmp == tmp2)
                        {
                            if (preventLocalIntCharge != tmp)
                                localInt = 0;
                            localInt++;
                            preventLocalIntCharge = tmp;
                            //if (method.Name == "Dispose" && tmp2 == "nop" && localInt == 1) continue;
                            if (localInt == xD4[v])
                            {
                                localInt = 0;
                                subChave++;
                                chave = subChave;
                                v++;
                                int getIndex = iJ;

                                lista.Insert(getIndex + 1, OpCodes.Ldloc_S.ToInstruction(local4));
                                lista.Insert(getIndex + 2, OpCodes.Ldc_I4.ToInstruction(((int)((Instruction)xD2[e].Operand).Offset)));
                                lista.Insert(getIndex + 3, OpCodes.Ldelem.ToInstruction(ctx.ManifestModule.Import(typeof(Label))));
                                lista.Insert(getIndex + 4, OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(Label) }))));
                                iJ += 3;

                                goto il_finale;
                            }
                        }
                        else
                        {
                            chave = subChave;
                        }

                    }
                il_finale:;
                }


            }

            lista.Add(OpCodes.Ldloc_S.ToInstruction(local2));
            lista.Add(OpCodes.Ldnull.ToInstruction());
            if (Param != null)
                lista.Add(OpCodes.Ldc_I4.ToInstruction(Param.Count + 1));
            else if (pp.ToArray().Count() != 0)
                lista.Add(OpCodes.Ldc_I4.ToInstruction(pp.ToArray().Count()));
            else
                lista.Add(OpCodes.Ldc_I4.ToInstruction(0));
            lista.Add(OpCodes.Newarr.ToInstruction(ctx.ManifestModule.Import(typeof(object))));
            if (Param != null)
            {
                int x = 0;
                lista.Add(OpCodes.Dup.ToInstruction());
                foreach (Parameter p in pp)
                {
                    lista.Add(OpCodes.Ldc_I4.ToInstruction(x));
                    lista.Add(OpCodes.Ldarg_S.ToInstruction(p));
                    lista.Add(OpCodes.Stelem_Ref.ToInstruction());
                    lista.Add(OpCodes.Dup.ToInstruction());
                    x++;
                }
                lista.RemoveAt(lista.Count - 1);
            }
            else if (pp.ToArray().Count() != 0)
            {
                int x = 0;
                lista.Add(OpCodes.Dup.ToInstruction());
                foreach (Parameter p in pp)
                {
                    lista.Add(OpCodes.Ldc_I4.ToInstruction(x));
                    lista.Add(OpCodes.Ldarg_S.ToInstruction(p));
                    lista.Add(OpCodes.Stelem_Ref.ToInstruction());
                    lista.Add(OpCodes.Dup.ToInstruction());
                    x++;
                }
                lista.RemoveAt(lista.Count - 1);
            }
            lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(MethodBase).GetMethod("Invoke", new Type[] { typeof(object), typeof(object[]) }))));
            if (returnType.TypeName != "Void")
                lista.Add(OpCodes.Unbox_Any.ToInstruction(returnType.ToTypeDefOrRef()));
            else
                lista.Add(OpCodes.Pop.ToInstruction());
            lista.Add(OpCodes.Ret.ToInstruction());
            outLocals = variables;
            return lista.ToArray();
        }
        static int LocalsCount = 0;
        public static void ConvertInstructionWithOperand(Instruction instruct, Local push, ref List<Instruction> lista, List<Local> variables, List<Instruction> brTargets, AssemblyDef ctx)
        {
            lista.Add(OpCodes.Ldloc_S.ToInstruction(push));
            char[] Opcode = Utils.ConvertOpCode(instruct.OpCode).Name.ToCharArray();
            Opcode[0] = Convert.ToChar(Opcode[0].ToString().Replace(Opcode[0].ToString(), Opcode[0].ToString().ToUpper()));
            string f = new string(Opcode);
            string a = "";
            if (f.Contains("."))
            {
                a = f.Substring(f.IndexOf('.')).ToUpper();
                f = f.Replace(a.ToLower(), a);
            }
            var final = typeof(ROpCodes).GetField(f.Replace(".", "_"), BindingFlags.Public | BindingFlags.Static);
            lista.Add(OpCodes.Ldsfld.ToInstruction(ctx.ManifestModule.Import(final)));

            var obj = instruct.Operand;
            if (obj is ConstructorInfo)
            {
                //il.Emit(Utils.ConvertOpCode(instr.OpCode), (ConstructorInfo)obj);
            }
            else if (obj is MethodDef)
            {
                if (obj.ToString().Contains(".ctor"))
                {
                    lista.Add(OpCodes.Ldtoken.ToInstruction(((MethodDef)obj).DeclaringType.ToTypeSig().ToTypeDefOrRef()));
                    lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                    lista.Add(OpCodes.Ldc_I4.ToInstruction(0));
                    lista.Add(OpCodes.Newarr.ToInstruction(ctx.ManifestModule.Import(typeof(Type))));
                    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetConstructor", new Type[] { typeof(Type[]) }))));
                    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(ConstructorInfo) }))));
                    return;
                }
                if (instruct.OpCode == OpCodes.Ldftn)
                {
                    lista.Add(OpCodes.Ldtoken.ToInstruction(((MethodDef)obj).DeclaringType.ToTypeSig().ToTypeDefOrRef()));
                    lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                    lista.Add(OpCodes.Ldstr.ToInstruction((((MethodDef)obj).Name)));
                    lista.Add(OpCodes.Ldc_I4.ToInstruction(0x107FF7F));
                    lista.Add(OpCodes.Ldnull.ToInstruction());
                    int xx = 0;
                    int yy = 0;

                    foreach (TypeSig sig in ((MethodBaseSig)((MethodDef)obj).Signature).Params)
                    {
                        if (xx == 0)
                        {
                            lista.Add(OpCodes.Ldc_I4.ToInstruction(((MethodBaseSig)((MethodDef)obj).Signature).Params.Count));
                            lista.Add(OpCodes.Newarr.ToInstruction(ctx.ManifestModule.Import(typeof(Type))));
                            lista.Add(OpCodes.Dup.ToInstruction());
                            xx++;
                        }
                        lista.Add(OpCodes.Ldc_I4.ToInstruction(yy));
                        lista.Add(OpCodes.Ldtoken.ToInstruction(sig.ToTypeDefOrRef()));
                        lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                        lista.Add(OpCodes.Stelem_Ref.ToInstruction());
                        lista.Add(OpCodes.Dup.ToInstruction());
                        yy++;
                    }
                    lista.RemoveAt(lista.Count - 1);
                    lista.Add(OpCodes.Ldnull.ToInstruction());
                    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string), typeof(BindingFlags), typeof(Binder), typeof(Type[]), typeof(ParameterModifier[]) }))));
                    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(MethodInfo) }))));
                    return;
                }
                lista.Add(OpCodes.Ldtoken.ToInstruction(((MethodDef)obj).DeclaringType.ToTypeSig().ToTypeDefOrRef()));
                lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                lista.Add(OpCodes.Ldstr.ToInstruction((((MethodDef)obj).Name)));
                lista.Add(OpCodes.Ldc_I4.ToInstruction(0x107FF7F));
                lista.Add(OpCodes.Ldnull.ToInstruction());
                int x = 0;
                int y = 0;
                if (((MethodBaseSig)((MethodDef)obj).Signature).Params.Count >= 1)
                {
                    foreach (TypeSig sig in ((MethodBaseSig)((MethodDef)obj).Signature).Params)
                    {

                        if (x == 0)
                        {
                            lista.Add(OpCodes.Ldc_I4.ToInstruction(((MethodBaseSig)((MethodDef)obj).Signature).Params.Count));
                            lista.Add(OpCodes.Newarr.ToInstruction(ctx.ManifestModule.Import(typeof(Type))));
                            lista.Add(OpCodes.Dup.ToInstruction());
                            x++;
                        }
                        lista.Add(OpCodes.Ldc_I4.ToInstruction(y));
                        lista.Add(OpCodes.Ldtoken.ToInstruction(sig.ToTypeDefOrRef()));
                        lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                        lista.Add(OpCodes.Stelem_Ref.ToInstruction());
                        lista.Add(OpCodes.Dup.ToInstruction());
                        y++;
                    }
                    lista.RemoveAt(lista.Count - 1);
                    lista.Add(OpCodes.Ldnull.ToInstruction());
                    lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string), typeof(BindingFlags), typeof(Binder), typeof(Type[]), typeof(ParameterModifier[]) }))));
                    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(MethodInfo) }))));
                }
                else
                {
                    //lista.Add(OpCodes.Ldc_I4.ToInstruction(0x107FF7F));
                    //lista.Add(OpCodes.Ldnull.ToInstruction());
                    lista.Add(OpCodes.Ldc_I4_0.ToInstruction());
                    lista.Add(OpCodes.Newarr.ToInstruction(ctx.ManifestModule.Import(typeof(Type))));
                    lista.Add(OpCodes.Ldnull.ToInstruction());
                    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string), typeof(BindingFlags), typeof(Binder), typeof(Type[]), typeof(ParameterModifier[]) }))));
                    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(MethodInfo) }))));
                    //il.Emit(Utils.ConvertOpCode(instr.OpCode), (MethodInfo)obj);
                }
                return;
            }
            else if (obj is string)
            {
                lista.Add(OpCodes.Ldstr.ToInstruction(obj.ToString()));
                lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(string) }))));
                return;
            }
            else if (obj is TypeDef)
            {
                return;
            }
            else if (obj is ConstructorInfo)
            {
                //lista.Add(OpCodes.Ldtoken.ToInstruction(((MethodDef)obj).DeclaringType.ToTypeSig().ToTypeDefOrRef()));
                //lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                //int x = 0;
                //int y = 0;
                //if (((MethodBaseSig)((MethodDef)obj).Signature).Params.Count >= 1)
                //{
                //    foreach (TypeSig sig in ((MethodBaseSig)((MethodDef)obj).Signature).Params)
                //    {

                //        if (x == 0)
                //        {
                //            lista.Add(OpCodes.Ldc_I4.ToInstruction(((MethodBaseSig)((MethodDef)obj).Signature).Params.Count));
                //            lista.Add(OpCodes.Newarr.ToInstruction(ctx.ManifestModule.Import(typeof(Type))));
                //            lista.Add(OpCodes.Dup.ToInstruction());
                //            x++;
                //        }
                //        lista.Add(OpCodes.Ldc_I4.ToInstruction(y));
                //        lista.Add(OpCodes.Ldtoken.ToInstruction(sig.ToTypeDefOrRef()));
                //        lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                //        lista.Add(OpCodes.Stelem_Ref.ToInstruction());
                //        lista.Add(OpCodes.Dup.ToInstruction());
                //        y++;
                //    }
                //    lista.RemoveAt(lista.Count - 1);
                //    lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetConstructor", new Type[] { typeof(Type[]) }))));
                //    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(ConstructorInfo) }))));
                //}
                //else
                //{
                //    lista.Add(OpCodes.Ldc_I4.ToInstruction(0x107FF7F));
                //    lista.Add(OpCodes.Ldnull.ToInstruction());
                //    lista.Add(OpCodes.Ldc_I4_0.ToInstruction());
                //    lista.Add(OpCodes.Newarr.ToInstruction(ctx.ManifestModule.Import(typeof(Type))));
                //    lista.Add(OpCodes.Ldnull.ToInstruction());
                //    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string), typeof(BindingFlags), typeof(Binder), typeof(Type[]), typeof(ParameterModifier[]) }))));
                //    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(MethodInfo) }))));
                //}
            }
            else if (obj is int)
            {
                lista.Add(OpCodes.Ldc_I4.ToInstruction(int.Parse(obj.ToString())));
                lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(int) }))));
                return;
            }
            else if (instruct.OpCode == OpCodes.Ldc_I4_S)
            {
                lista.Add(OpCodes.Ldc_I4_S.ToInstruction(sbyte.Parse(obj.ToString())));
                lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(sbyte) }))));
                return;
            }
            else if (obj is double)
            {
                lista.Add(OpCodes.Ldc_R8.ToInstruction(double.Parse(obj.ToString().Replace(".", ","))));
                lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(double) }))));
                return;
            }
            else if (obj is float)
            {
                lista.Add(OpCodes.Ldc_R4.ToInstruction(float.Parse(obj.ToString().Replace(".", ","))));
                lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(float) }))));
                return;
            }
            else if (obj is dnlib.DotNet.MemberRef)
            {
                if (instruct.OpCode == OpCodes.Ldftn)
                    return;
                if (obj.ToString().Contains(".ctor"))
                {
                    lista.Add(OpCodes.Ldtoken.ToInstruction(((MemberRef)obj).DeclaringType.ToTypeSig().ToTypeDefOrRef()));
                    lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));

                    int xx = 0;
                    int yy = 0;
                    if (((MethodBaseSig)((MemberRef)obj).Signature).Params.Count >= 1)
                    {
                        foreach (TypeSig sig in ((MethodBaseSig)((MemberRef)obj).Signature).Params)
                        {

                            if (xx == 0)
                            {
                                lista.Add(OpCodes.Ldc_I4.ToInstruction(((MethodBaseSig)((MemberRef)obj).Signature).Params.Count));
                                lista.Add(OpCodes.Newarr.ToInstruction(ctx.ManifestModule.Import(typeof(Type))));
                                lista.Add(OpCodes.Dup.ToInstruction());
                                xx++;
                            }
                            lista.Add(OpCodes.Ldc_I4.ToInstruction(yy));
                            lista.Add(OpCodes.Ldtoken.ToInstruction(sig.ToTypeDefOrRef()));
                            lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                            lista.Add(OpCodes.Stelem_Ref.ToInstruction());
                            lista.Add(OpCodes.Dup.ToInstruction());
                            yy++;
                        }
                        lista.RemoveAt(lista.Count - 1);
                        lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetConstructor", new Type[] { typeof(Type[]) }))));
                        lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(ConstructorInfo) }))));
                    }
                    else
                    {
                        lista.Add(OpCodes.Ldc_I4.ToInstruction(0));
                        lista.Add(OpCodes.Newarr.ToInstruction(ctx.ManifestModule.Import(typeof(Type))));
                        lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetConstructor", new Type[] { typeof(Type[]) }))));
                        lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(ConstructorInfo) }))));
                    }
                    return;
                }
                lista.Add(OpCodes.Ldtoken.ToInstruction(((MemberRef)obj).DeclaringType.ToTypeSig().ToTypeDefOrRef()));
                lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                lista.Add(OpCodes.Ldstr.ToInstruction((((MemberRef)obj).Name)));
                int x = 0;
                int y = 0;
                if (((MethodBaseSig)((MemberRef)obj).Signature).Params.Count >= 1)
                {
                    foreach (TypeSig sig in ((MethodBaseSig)((MemberRef)obj).Signature).Params)
                    {

                        if (x == 0)
                        {
                            lista.Add(OpCodes.Ldc_I4.ToInstruction(((MethodBaseSig)((MemberRef)obj).Signature).Params.Count));
                            lista.Add(OpCodes.Newarr.ToInstruction(ctx.ManifestModule.Import(typeof(Type))));
                            lista.Add(OpCodes.Dup.ToInstruction());
                            x++;
                        }
                        lista.Add(OpCodes.Ldc_I4.ToInstruction(y));
                        lista.Add(OpCodes.Ldtoken.ToInstruction(sig.ToTypeDefOrRef()));
                        lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                        lista.Add(OpCodes.Stelem_Ref.ToInstruction());
                        lista.Add(OpCodes.Dup.ToInstruction());
                        y++;
                    }
                    lista.RemoveAt(lista.Count - 1);
                    lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string), typeof(Type[]) }))));
                    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(MethodInfo) }))));
                }
                else
                {
                    lista.Add(OpCodes.Ldc_I4.ToInstruction(0x107FF7F));
                    lista.Add(OpCodes.Ldnull.ToInstruction());
                    lista.Add(OpCodes.Ldc_I4_0.ToInstruction());
                    lista.Add(OpCodes.Newarr.ToInstruction(ctx.ManifestModule.Import(typeof(Type))));
                    lista.Add(OpCodes.Ldnull.ToInstruction());
                    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string), typeof(BindingFlags), typeof(Binder), typeof(Type[]), typeof(ParameterModifier[]) }))));
                    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(MethodInfo) }))));
                    //il.Emit(Utils.ConvertOpCode(instr.OpCode), (MethodInfo)obj);
                }
                return;
            }
            else if (obj is FieldDef)
            {
                lista.Add(OpCodes.Ldtoken.ToInstruction(((FieldDef)obj).DeclaringType.ToTypeSig().ToTypeDefOrRef()));
                lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                lista.Add(OpCodes.Ldstr.ToInstruction((((FieldDef)obj).Name)));
                lista.Add(OpCodes.Ldc_I4.ToInstruction(0x107FF7F));
                lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetField", new Type[] { typeof(string), typeof(BindingFlags) }))));
                lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(FieldInfo) }))));
                return;
            }
            else if (obj is TypeRef)
            {
                //if (obj.ToString().Contains(".ctor"))
                //{
                //    lista.Add(OpCodes.Ldtoken.ToInstruction(((TypeRef)obj).DeclaringType.ToTypeSig().ToTypeDefOrRef()));
                //    lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                //    lista.Add(OpCodes.Ldc_I4.ToInstruction(0));
                //    lista.Add(OpCodes.Newarr.ToInstruction(ctx.ManifestModule.Import(typeof(Type))));
                //    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetConstructor", new Type[] { typeof(Type[]) }))));
                //    lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(ConstructorInfo) }))));
                //    return;
                //}
                lista.Add(OpCodes.Ldtoken.ToInstruction(((TypeRef)obj).ToTypeSig().ToTypeDefOrRef()));
                lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
                lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(Type) }))));
                return;
            }
            else if (obj is Local)
            {
                //if (instruct.OpCode == OpCodes.Stloc_S)
                {
                    try
                    {
                        var res = variables.ToDictionary(x => x, x => x);
                        Local outL;
                        res.TryGetValue(variables[int.Parse((string)((Local)obj).ToString().Replace("V_", ""))], out outL);
                        lista.Add(OpCodes.Ldloc_S.ToInstruction(outL));
                        lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode), typeof(LocalBuilder) }))));
                        return;
                    }
                    catch (Exception ex) { Console.WriteLine(string.Format("{0}::{1} msg: {2}", instruct.OpCode, obj, ex.Message)); }
                }
            }

            else if (instruct.OpCode.OperandType == OperandType.InlineBrTarget || instruct.OpCode.OperandType == OperandType.ShortInlineBrTarget)
                return;

            else if (instruct.OpCode == OpCodes.Nop)
            {
                return;
            }

            lista.RemoveAt(lista.Count - 1);
            lista.RemoveAt(lista.Count - 1);

        }
        public static void ConvertInstruction(Instruction instruct, Local push, ref List<Instruction> lista, AssemblyDef ctx)
        {
            lista.Add(OpCodes.Ldloc_S.ToInstruction(push));
            char[] Opcode = Utils.ConvertOpCode(instruct.OpCode).Name.ToCharArray();
            Opcode[0] = Convert.ToChar(Opcode[0].ToString().Replace(Opcode[0].ToString(), Opcode[0].ToString().ToUpper()));
            string f = new string(Opcode);
            string a = "";
            if (f.Contains("."))
            {
                a = f.Substring(f.IndexOf('.')).ToUpper();
                f = f.Replace(a.ToLower(), a);
            }
            var final = typeof(ROpCodes).GetField(f.Replace(".", "_"), BindingFlags.Public | BindingFlags.Static);
            if (final == null)
                Console.WriteLine(f);
            lista.Add(OpCodes.Ldsfld.ToInstruction(ctx.ManifestModule.Import(final)));
            lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("Emit", new Type[] { typeof(ROpCode) }))));
        }
        public static void addLocal(Local local, Local push, ref List<Instruction> lista, AssemblyDef ctx, ref List<Local> list)
        {
            lista.Add(OpCodes.Ldloc_S.ToInstruction(push));
            lista.Add(OpCodes.Ldtoken.ToInstruction(local.Type.ToTypeDefOrRef()));
            lista.Add(OpCodes.Call.ToInstruction(ctx.ManifestModule.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(System.RuntimeTypeHandle) }))));
            lista.Add(OpCodes.Callvirt.ToInstruction(ctx.ManifestModule.Import(typeof(ILGenerator).GetMethod("DeclareLocal", new Type[] { typeof(Type) }))));
            list.Add(new Local(ctx.ManifestModule.Import(typeof(LocalBuilder)).ToTypeSig()));
            lista.Add(OpCodes.Stloc_S.ToInstruction(list[list.Count - 1]));
        }
        public static int Emulate(Instruction[] code, AssemblyDef ctx)
        {
            DynamicMethod emulatore = new DynamicMethod(ICore.Utils.MethodsRenamig(), typeof(void), null);
            ILGenerator il = emulatore.GetILGenerator();
            foreach (Instruction instr in code)
            {
                if (instr.Operand != null)
                {
                    if (instr.OpCode == OpCodes.Ldc_I4)
                        il.Emit(Utils.ConvertOpCode(instr.OpCode), Convert.ToInt32(instr.Operand));
                    else if (instr.OpCode == OpCodes.Ldstr)
                        il.Emit(Utils.ConvertOpCode(OpCodes.Ldstr), Convert.ToString(instr.Operand));
                    else if (instr.OpCode.OperandType == OperandType.InlineTok || instr.OpCode.OperandType == OperandType.InlineType || instr.OpCode.OperandType == OperandType.InlineMethod || instr.OpCode.OperandType == OperandType.InlineField)
                    {
                        Type Resolver = Assembly.LoadWithPartialName("AssemblyData").GetType("AssemblyData.methodsrewriter.Resolver");
                        var Method = Resolver.GetMethod("GetRtObject", new Type[] { typeof(ITokenOperand) });
                        var obj = Method.Invoke("", new object[] { (ITokenOperand)instr.Operand });
                        if (obj is ConstructorInfo)
                            il.Emit(Utils.ConvertOpCode(instr.OpCode), (ConstructorInfo)obj);
                        else if (obj is MethodInfo)
                            il.Emit(Utils.ConvertOpCode(instr.OpCode), (MethodInfo)obj);
                        else if (obj is FieldInfo)
                            il.Emit(Utils.ConvertOpCode(instr.OpCode), (FieldInfo)obj);
                        else if (obj is Type)
                            il.Emit(Utils.ConvertOpCode(instr.OpCode), (Type)obj);


                    }
                }
                else
                    il.Emit(Utils.ConvertOpCode(instr.OpCode));

            }
            emulatore.Invoke(null, new object[0]);
            //Result xdd = (Result)emulatore.CreateDelegate(typeof(Result));
            //int abcc = xdd.Invoke();
            return 0;
        }

    }
    //public static class Ext
    //{
    //    public static T[] RemoveAt<T>(this T[] source, int index)
    //    {
    //        T[] dest = new T[source.Length - 1];
    //        if (index > 0)
    //            Array.Copy(source, 0, dest, 0, index);

    //        if (index < source.Length - 1)
    //            Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

    //        return dest;
    //    }
    //}

    class Utils
    {
        static Dictionary<OpCode, ROpCode> dnlibToReflection = new Dictionary<OpCode, ROpCode>();
        static ROpCode ropcode;
        public static ROpCode ConvertOpCode(OpCode opcode)
        {

            if (dnlibToReflection.TryGetValue(opcode, out ropcode))
                return ropcode;
            return ROpCodes.Nop;
        }
        public static void LoadOpCodes()
        {
            var refDict = new Dictionary<short, ROpCode>(0x100);
            foreach (var f in typeof(ROpCodes).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (f.FieldType != typeof(ROpCode))
                    continue;
                var ropcode = (ROpCode)f.GetValue(null);
                refDict[ropcode.Value] = ropcode;
            }

            foreach (var f in typeof(OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (f.FieldType != typeof(OpCode))
                    continue;
                var opcode = (OpCode)f.GetValue(null);
                if (!refDict.TryGetValue(opcode.Value, out ropcode))
                    continue;
                dnlibToReflection[opcode] = ropcode;
            }
        }

    }
    class Utils2
    {
        static Dictionary<ROpCode, OpCode> reflectionToDnlib = new Dictionary<ROpCode, OpCode>();
        static OpCode Opcode;
        public static OpCode ConvertOpCode(ROpCode ropcode)
        {

            if (reflectionToDnlib.TryGetValue(ropcode, out Opcode))
                return Opcode;
            return OpCodes.Nop;
        }
        public static void LoadOpCodes()
        {
            var refDict = new Dictionary<short, OpCode>(0x100);
            foreach (var f in typeof(OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (f.FieldType != typeof(OpCode))
                    continue;
                var opcode = (OpCode)f.GetValue(null);
                refDict[opcode.Value] = opcode;
            }

            foreach (var f in typeof(ROpCodes).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (f.FieldType != typeof(ROpCode))
                    continue;
                var ropcode = (ROpCode)f.GetValue(null);
                if (!refDict.TryGetValue(ropcode.Value, out Opcode))
                    continue;
                reflectionToDnlib[ropcode] = Opcode;
            }
        }


    }


}

