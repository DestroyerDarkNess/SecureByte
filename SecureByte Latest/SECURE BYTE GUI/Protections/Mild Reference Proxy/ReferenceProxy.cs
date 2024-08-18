using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Generic;
using System.Linq;
using ICore;
using Core;
using System.Diagnostics;

namespace Protections.RefProxy
{
    public class Helper
    {
        public static bool CanObfuscate(MethodDef methodDef)
        {
            if (methodDef.DeclaringType.IsGlobalModuleType)
                return false;
            if (FixedReferenceProxy.ProxyMethods.Contains(methodDef))
                return false;
            if (methodDef.Name.Contains("Dispose")) 
                return false;
            if (!methodDef.HasBody)
                return false;
            if (!methodDef.Body.HasInstructions)
                return false;
            return true;
        }
        public static string[] DontObf = new string[]
{
            "ToArray",
            "set_foregroundcolor",
            "get_conte",
            "GetTypeFromHandle",
            "TypeFromHandle",
            "GetFunctionPointer",
            "get_value",
            "GetIndex",
            "set_IgnoreProtocal",
            "Split",
            "WithAuthor",
            "Match",
            "ClearAllHeaders",
            "Post",
            "set_IgnoreProtocal",
            "GetChannel",
            "op_Implicit",
            "invoke",
            "get_Task",
            "get_ContentType",
            "ADD",
            "op_Equality",
            "op_Inequality",
            "Contains",
            "FreeHGlobal",
            "get_Module",
            "ResolveMethod",
            ".ctor",
            "ReadLine",
            "Dispose",
            "Next",
            "Async",
            "GetAwaiter",
            "SetException",
            "Exception",
            "Enter",
            "ReadLines",
            "UnaryOperation",
            "BinaryOperation",
            "Close",
            "WithTitle",
            "Format",
            "get_Memeber",
            "set_IgnoreProtocallErrors",
            "MoveNext",
            "Getinstances",
            "Build",
            "Serialize",
            "Exists",
            "UseCommandsNext",
            "Delay"
};
        public static MethodDef GenerateMethod(Context ctx, TypeDef declaringType, object targetMethod, bool hasThis = false, bool isVoid = false)
        {
            MemberRef methodReference = (MemberRef)targetMethod;
            MethodDef methodDefinition = new MethodDefUser(ICore.Utils.MethodsRenamig(), MethodSig.CreateStatic((methodReference).ReturnType), MethodImplAttributes.IL | MethodImplAttributes.Managed, MethodAttributes.Public | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig)
            {
                Body = new CilBody()
            };
            if (hasThis)
                methodDefinition.MethodSig.Params.Add(declaringType.Module.Import(declaringType.ToTypeSig(true)));
            foreach (TypeSig current in methodReference.MethodSig.Params)
                methodDefinition.MethodSig.Params.Add(current);
            methodDefinition.Parameters.UpdateParameterTypes();
            foreach (var current in methodDefinition.Parameters)
                methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, current));
            methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Call, methodReference));
            methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            IMethod DebugAssert = ctx.Module.Import(typeof(Debug).GetMethod("Assert", new[] { typeof(bool) }));
            methodDefinition.Body.Instructions.Insert(0, new Instruction(OpCodes.Call, DebugAssert));
            methodDefinition.Body.Instructions.Insert(0, new Instruction(OpCodes.Ldc_I4_1));
            DnlibUtils.EnsureNoInlining(methodDefinition);
            return methodDefinition;
        }
        public static MethodDef GenerateMethod(Context ctx, IMethod targetMethod, MethodDef md)
        {
            MethodDef methodDef = new MethodDefUser(Utils.MethodsRenamig(), MethodSig.CreateStatic(md.Module.Import(targetMethod.DeclaringType.ToTypeSig(true))))
            {
                ImplAttributes = MethodImplAttributes.Managed | MethodImplAttributes.IL,
                Attributes = MethodAttributes.Public | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig,
                IsHideBySig = true,
                Body = new CilBody()
            };
            for (int x = 0; x < targetMethod.MethodSig.Params.Count; x++)
            {
                methodDef.ParamDefs.Add(new ParamDefUser(Utils.MethodsRenamig(), (ushort)(x + 1)));
                methodDef.MethodSig.Params.Add(targetMethod.MethodSig.Params[x]);
            }
            methodDef.Parameters.UpdateParameterTypes();
            for (int x = 0; x < methodDef.Parameters.Count; x++)
            {
                Parameter parameter = methodDef.Parameters[x];
                methodDef.Body.Instructions.Add(new Instruction(OpCodes.Ldarg, parameter));
            }
            methodDef.Body.Instructions.Add(new Instruction(OpCodes.Newobj, targetMethod));
            methodDef.Body.Instructions.Add(new Instruction(OpCodes.Ret));
            IMethod DebugAssert = ctx.Module.Import(typeof(Debug).GetMethod("Assert", new[] { typeof(bool) }));
            methodDef.Body.Instructions.Insert(0, new Instruction(OpCodes.Call, DebugAssert));
            methodDef.Body.Instructions.Insert(0, new Instruction(OpCodes.Ldc_I4_1));
            DnlibUtils.EnsureNoInlining(methodDef);
            return methodDef;
        }
        public static MethodDef GenerateMethod(Context ctx, FieldDef targetField, MethodDef md)
        {
            MethodDef methodDefinition = new MethodDefUser(Utils.MethodsRenamig(), MethodSig.CreateStatic(md.Module.Import(targetField.FieldType)), MethodImplAttributes.IL | MethodImplAttributes.Managed, MethodAttributes.Public | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig)
            {
                Body = new CilBody()
            };
            TypeDef declaringType = md.DeclaringType;
            methodDefinition.MethodSig.Params.Add(md.Module.Import(declaringType).ToTypeSig());
            methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ldfld, targetField));
            methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            IMethod DebugAssert = ctx.Module.Import(typeof(Debug).GetMethod("Assert", new[] { typeof(bool) }));
            methodDefinition.Body.Instructions.Insert(0, new Instruction(OpCodes.Call, DebugAssert));
            methodDefinition.Body.Instructions.Insert(0, new Instruction(OpCodes.Ldc_I4_1));
            md.DeclaringType.Methods.Add(methodDefinition);
            DnlibUtils.EnsureNoInlining(methodDefinition);
            return methodDefinition;
        }
    }
    internal class FixedReferenceProxy 
    {
        public static List<MethodDef> ProxyMethods = new List<MethodDef>();
        public static void Execute(Context ctx)
        {
            foreach (TypeDef type in ctx.Module.Types.ToArray())
            {
                foreach (MethodDef method in type.Methods.ToArray())
                {                 
                    if (Helper.CanObfuscate(method))
                    {
                        foreach (Instruction instruction in method.Body.Instructions.ToArray())
                        {
                            if (instruction.OpCode == OpCodes.Newobj)
                            {
                                IMethodDefOrRef methodDefOrRef = instruction.Operand as IMethodDefOrRef;
                                if (methodDefOrRef.IsMethodSpec) continue;
                                if (methodDefOrRef == null) continue;
                                bool flag3 = instruction.OpCode.Code != Code.Newobj && method.Name == ".ctor";
                                if (!flag3)
                                {
                                    bool flag5 = methodDefOrRef is MethodSpec;
                                    if (!flag5)
                                    {
                                        bool flag6 = methodDefOrRef.DeclaringType is TypeSpec;
                                        if (!flag6)
                                        {
                                            bool flag7 = methodDefOrRef.MethodSig.ParamsAfterSentinel != null && methodDefOrRef.MethodSig.ParamsAfterSentinel.Count > 0;
                                            if (!flag7)
                                            {
                                                bool flag9 = type.IsValueType && methodDefOrRef.MethodSig.HasThis;
                                                if (!flag9)
                                                {
                                                    MethodDef methodDef = Helper.GenerateMethod(ctx, methodDefOrRef, method);
                                                    if (methodDef == null) continue;
                                                    method.DeclaringType.Methods.Add(methodDef);
                                                    ProxyMethods.Add(methodDef);
                                                    Protector.usedMethods.Add(methodDef);
                                                    instruction.Operand = methodDef;
                                                    instruction.OpCode = OpCodes.Call;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            #region need fixes
                            //if (instruction.OpCode == OpCodes.Stfld)
                            //{
                            //    FieldDef fieldDef = instruction.Operand as FieldDef;
                            //    bool flag7 = fieldDef == null;
                            //    if (!flag7)
                            //    {
                            //        CilBody cilBody = new CilBody();
                            //        cilBody.Instructions.Add(OpCodes.Nop.ToInstruction());
                            //        cilBody.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
                            //        cilBody.Instructions.Add(OpCodes.Ldarg_1.ToInstruction());
                            //        cilBody.Instructions.Add(OpCodes.Stfld.ToInstruction(fieldDef));
                            //        cilBody.Instructions.Add(OpCodes.Ret.ToInstruction());
                            //        MethodSig methodSig = MethodSig.CreateInstance(ctx.Module.CorLibTypes.Void, fieldDef.FieldSig.GetFieldType());
                            //        methodSig.HasThis = true;
                            //        MethodDefUser methodDefUser = new MethodDefUser(Utils.MethodsRenamig(), methodSig)
                            //        {
                            //            Body = cilBody,
                            //            IsHideBySig = true
                            //        };
                            //        ProxyMethods.Add(methodDefUser);
                            //        method.DeclaringType.Methods.Add(methodDefUser);
                            //        instruction.Operand = methodDefUser;
                            //        instruction.OpCode = OpCodes.Call;
                            //    }
                            //}
                            //if (instruction.OpCode == OpCodes.Ldfld)
                            //{
                            //    if (!(instruction.Operand is FieldDef targetField)) continue;
                            //    MethodDef newmethod = Helper.GenerateMethod(ctx, targetField, method);
                            //    //instruction.OpCode = OpCodes.Call;
                            //    //instruction.Operand = newmethod;
                            //    ProxyMethods.Add(newmethod);
                            //    Protector.usedMethods.Add(newmethod);
                            //}
                            #endregion
                            else if (instruction.OpCode == OpCodes.Call)
                            {
                                if (instruction.Operand is MemberRef methodReference)
                                {
                                    bool flag3 = instruction.OpCode.Code != Code.Newobj && method.Name == ".ctor";
                                    if (!flag3)
                                    {
                                        bool flag6 = methodReference.DeclaringType is TypeSpec;
                                        if (!flag6)
                                        {
                                            bool flag7 = methodReference.MethodSig.ParamsAfterSentinel != null && methodReference.MethodSig.ParamsAfterSentinel.Count > 0;
                                            if (!flag7)
                                            {
                                                bool flag9 = type.IsValueType && methodReference.MethodSig.HasThis;
                                                if (!flag9)
                                                {
                                                    if (!Helper.DontObf.Any((string x) => x.ToLower().Contains(methodReference.Name.ToLower())) && !Helper.DontObf.Any((string xx) => xx.ToLower().Contains(methodReference.FullName.ToLower())) && !methodReference.FullName.Contains("bool") && !methodReference.FullName.Contains("Collections.Generic") && !methodReference.Name.Contains("ToString") && !methodReference.FullName.Contains("Thread::Start") && !methodReference.Name.Contains("Properties.Settings") && !methodReference.FullName.Contains("System.Boolean") && !methodReference.FullName.Contains("ctor"))
                                                    {
                                                        MethodDef methodDef = Helper.GenerateMethod(ctx, type, methodReference, methodReference.HasThis, methodReference.FullName.StartsWith("System.Void"));
                                                        if (methodDef != null)
                                                        {
                                                            ProxyMethods.Add(methodDef);
                                                            Protector.usedMethods.Add(methodDef);
                                                            type.Methods.Add(methodDef);
                                                            instruction.Operand = methodDef;
                                                            methodDef.Body.Instructions.Add(new Instruction(OpCodes.Ret));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}