//using dnlib.DotNet;
//using dnlib.DotNet.Emit;
//using ICore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Confuser.Protections
//{
//    public class CalliProtection
//    {
//        protected override void Execute(Context context)
//        {
//            var rt = context.Registry.GetService<IRuntimeService>();
//            var marker = context.Registry.GetService<IMarkerService>();
//            var name = context.Registry.GetService<INameService>();

//            foreach (ModuleDef module in parameters.Targets.OfType<ModuleDef>().WithProgress(context.Logger))
//            {
//                CalliMode mode = parameters.GetParameter(context, module, "mode", CalliMode.Ldftn);

//                switch (mode)
//                {
//                    case CalliMode.Normal:
//                        {
//                            TypeDef typeDef = rt.GetRuntimeType("Confuser.Runtime.Calli");
//                            IEnumerable<IDnlibDef> members = InjectHelper.Inject(typeDef, module.GlobalType, module);
//                            var init = (MethodDef)members.Single(methodddd => methodddd.Name == "ResolveToken");
//                            foreach (IDnlibDef member in members)
//                                name.MarkHelper(member, marker, (Protection)Parent);
//                            foreach (TypeDef type in module.Types.ToArray())
//                            {
//                                if (type.IsGlobalModuleType) continue;
//                                foreach (MethodDef method in type.Methods.ToArray())
//                                {
//                                    if (!method.Equals(init) && !method.Equals(module.EntryPoint) && method.HasBody && method.Body.HasInstructions)
//                                    {
//                                        for (int i = 0; i < method.Body.Instructions.Count - 1; i++)
//                                        {
//                                            try
//                                            {
//                                                if (method.Body.Instructions[i].OpCode == OpCodes.Call || method.Body.Instructions[i].OpCode == OpCodes.Callvirt || method.Body.Instructions[i].OpCode == OpCodes.Ldloc_S)
//                                                {
//                                                    if (!method.Body.Instructions[i].Operand.ToString().Contains("System.Type") && !method.Body.Instructions[i].Operand.ToString().Contains("MessageBoxButtons"))
//                                                    {
//                                                        try
//                                                        {
//                                                            MemberRef membertocalli = (MemberRef)method.Body.Instructions[i].Operand;
//                                                            tokentocalli = membertocalli.MDToken.ToInt32();
//                                                            if (CanObfuscate(membertocalli, method.Body.Instructions[i]))
//                                                            {
//                                                                listmember.Add(membertocalli);
//                                                                listtoken.Add(tokentocalli);
//                                                                if (!membertocalli.ToString().Contains("ResolveToken") && !membertocalli.HasThis)
//                                                                {
//                                                                    if (listmember.Contains(membertocalli))
//                                                                    {
//                                                                        method.Body.Instructions[i].OpCode = OpCodes.Calli;
//                                                                        method.Body.Instructions[i].Operand = listmember[listmember.IndexOf(membertocalli)].MethodSig;
//                                                                        method.Body.Instructions.Insert(i, Instruction.Create(OpCodes.Call, init));
//                                                                        method.Body.Instructions.Insert(i, Instruction.Create(OpCodes.Ldc_I4, (listtoken[listmember.IndexOf(membertocalli)])));
//                                                                    }
//                                                                    else
//                                                                    {
//                                                                        MethodSig MethodSign = membertocalli.MethodSig;
//                                                                        method.Body.Instructions[i].OpCode = OpCodes.Calli;
//                                                                        method.Body.Instructions[i].Operand = MethodSign;
//                                                                        method.Body.Instructions.Insert(i, Instruction.Create(OpCodes.Call, init));
//                                                                        method.Body.Instructions.Insert(i, Instruction.CreateLdcI4(tokentocalli));
//                                                                    }
//                                                                }
//                                                            }
//                                                        }
//                                                        catch { }
//                                                    }
//                                                }
//                                            }
//                                            catch { }
//                                        }
//                                    }
//                                }
//                                foreach (MethodDef md in module.GlobalType.Methods)
//                                {
//                                    if (md.Name == ".ctor")
//                                    {
//                                        module.GlobalType.Remove(md);
//                                        break;
//                                    }
//                                }
//                            }
//                        }
//                        break;
//                    case CalliMode.Ldftn:
//                        {
//                            foreach (TypeDef type in module.Types)
//                            {
//                                foreach (MethodDef method in type.Methods)
//                                {
//                                    if (!method.HasBody) continue;
//                                    if (!method.Body.HasInstructions) continue;
//                                    for (int i = 0; i < method.Body.Instructions.Count - 1; i++)
//                                    {
//                                        if (method.Body.Instructions[i].OpCode == OpCodes.Call || method.Body.Instructions[i].OpCode == OpCodes.Callvirt)
//                                        {
//                                            try
//                                            {
//                                                MemberRef membertocalli = (MemberRef)method.Body.Instructions[i].Operand;
//                                                method.Body.Instructions[i].OpCode = OpCodes.Calli;
//                                                method.Body.Instructions[i].Operand = membertocalli.MethodSig;
//                                                method.Body.Instructions.Insert(i, Instruction.Create(OpCodes.Ldftn, membertocalli));
//                                            }
//                                            catch { }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                        break;
//                }
//            }
//        }
//        public static int tokentocalli = 0;
//        public static List<MemberRef> listmember = new List<MemberRef>();
//        public static List<int> listtoken = new List<int>();

//        static bool CanObfuscate(MemberRef mRef, Instruction instruction)
//        {
//            if (mRef.ResolveMethodDef().ParamDefs.Any(x => x.IsOut)) return false;
//            if (mRef.ResolveMethodDef().IsVirtual) return false;
//            if (mRef.ResolveMethodDef().ReturnType.FullName.ToLower().Contains("bool")) return false;
//            if (mRef.ResolveMethodDef().ReturnType.FullName.ToLower().Contains("read")) return false;
//            return true;
//        }

//        enum CalliMode
//        {
//            Normal,
//            Ldftn
//        }
//    }
//}
