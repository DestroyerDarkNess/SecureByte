using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using dnlib.PE;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Security.Cryptography;
using System.Text;

namespace Core.Injection
{
    class InjectInitialise
    {
        public static MemberRef conversionInit;
        public static MemberRef convertBack;
        internal static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        internal static int GetNextInt32(int maxValue)
        {
            var buffer = new byte[4];
            int bits, val;
            if ((maxValue & -maxValue) == maxValue)
            {
                rng.GetBytes(buffer);
                bits = BitConverter.ToInt32(buffer, 0);
                return bits & (maxValue - 1);
            }
            do
            {
                rng.GetBytes(buffer);
                bits = BitConverter.ToInt32(buffer, 0) & 0x7FFFFFFF;
                val = bits % maxValue;
            } while (bits - val + (maxValue - 1) < 0);
            return val;
        }
        public static void initaliseMethod()
        {
         
            byte[] conversionPlain = System.IO.File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Runtime.dll"));
            conversionAssembly = Assembly.Load(conversionPlain).ManifestModule;
            conversionDef = ModuleDefMD.Load(conversionPlain);
          
        }
        public static void injectIntoCctor()
        {
           //("injectIntoCctor0");
            foreach (TypeDef t in conversionDef.Types)
            {
                foreach (MethodDef m in t.Methods)
                {
                    if (m.Name == "Init")
                    {
                        conversionInit = (MemberRef)Protector.moduleDefMD.Import(m);
                    }
                    if (m.Name == "Run")
                    {
                        convertBack = (MemberRef)Protector.moduleDefMD.Import(m);
                    }
                }
            }
            //("injectIntoCctor1");
            //conversionInit = Protector.moduleDefMD.Import(conversionTypes[33].Methods[2]);
            var a = typeof(Resource);
            var asm = ModuleDefMD.Load(typeof(Resource).Assembly.Location);
            var tester2 = asm.GetTypes();
            var abc = InjectHelper.Inject(tester2.ToArray()[13], Protector.moduleDefMD.GlobalType, Protector.moduleDefMD);

            foreach (var md in Protector.moduleDefMD.GlobalType.Methods)
            {
                if (md.Name != ".ctor") continue;
                Protector.moduleDefMD.GlobalType.Remove(md);
                break;
            }

            //("injectIntoCctor3");
            if (Protector.moduleDefMD.GlobalType.FindOrCreateStaticConstructor().Body == null)
            {
               //("injectIntoCctor4");
                var cil = new CilBody();


                cil.Instructions.Add(new Instruction(OpCodes.Call, Protector.moduleDefMD.Types[0].Methods[0]));

                cil.Instructions.Add(new Instruction(OpCodes.Ret));
                Protector.moduleDefMD.GlobalType.FindOrCreateStaticConstructor().Body = cil;
            }
            else
            {

                var typeModule = ModuleDefMD.Load(typeof(Resource).Module);
                var cctor = Protector.moduleDefMD.GlobalType.FindOrCreateStaticConstructor();
                var typeDef = typeModule.ResolveTypeDef(MDToken.ToRID(typeof(Resource).MetadataToken));
                var members = InjectHelper.Inject(typeDef, Protector.moduleDefMD.GlobalType, Protector.moduleDefMD);
                var init = (MethodDef)members.Single(method => method.Name == "setup");
                cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, init));
                foreach (var md in Protector.moduleDefMD.GlobalType.Methods)
                {
                    if (md.Name != ".ctor") continue;
                    Protector.moduleDefMD.GlobalType.Remove(md);
                    break;
                }

                var vody = Protector.moduleDefMD.GlobalType.FindOrCreateStaticConstructor().Body;
                foreach(var t in Protector.moduleDefMD.Types)
                {
                    foreach(var m in t.Methods)
                    {
                        if (m.Name == "setup")
                        {
                            Protector.setupMDT = m;
                        }
                    }
                }
             
                if (Protector.setupMDT != null)
                {
                   // MethodDef Setup = Protector.moduleDefMD.Types[0].Methods.Where(i => i.Name == "setup").FirstOrDefault();
                   // vody.Instructions.Insert(0, new Instruction(OpCodes.Call, Setup));
                }
            
                if ((Protector.moduleDefMD.Characteristics & Characteristics.Dll) != 0)
                {

                    vody.Instructions.Insert(1, new Instruction(OpCodes.Ldstr, "TestResc"));

                    vody.Instructions.Insert(2, new Instruction(OpCodes.Call, InjectInitialise.conversionInit));


                }
             
            }
        }

        public static string GenerateRandomLetters(int length)
        {
            Random random = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                int letterCode = random.Next(65, 91); // ASCII codes for uppercase letters (A-Z)
                char letter = (char)letterCode;
                sb.Append(letter);
            }
            return sb.ToString();
        }
        public static void InjectMethod(MethodDef meth, int pos, int id, int size)
        {
           //("     -> Started");
            var containsOut = false;
            meth.Body.Instructions.Clear();
            var rrr = meth.Parameters.Where(i => i.Type.FullName.EndsWith("&"));
            if (rrr.Count() != 0)
                containsOut = true;
            var rrg = new Local(Protector.moduleDefMD.CorLibTypes.Object.ToSZArraySig());
            var loc = new Local(Protector.moduleDefMD.CorLibTypes.Object);
            var loc2 = new Local(new SZArraySig(Protector.moduleDefMD.CorLibTypes.Object));
            var cli = new CilBody();
            foreach (var bodyVariable in meth.Body.Variables)
            {
                cli.Variables.Add(bodyVariable);
                meth.Body.SimplifyBranches();
                meth.Body.SimplifyMacros(meth.Parameters);
            }
            cli.Variables.Add(rrg);
            cli.Variables.Add(loc);
            cli.Variables.Add(loc2);
            var outParams = new List<Local>();
            var testerDictionary = new Dictionary<Parameter, Local>();
            if (containsOut)
                foreach (var parameter in rrr)
                {
                    var locf = new Local(parameter.Type.Next);
                    testerDictionary.Add(parameter, locf);
                    cli.Variables.Add(locf);
                }
            //
            #region Tne
            //cli.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
            //var except = ModuleDefMD.Load(typeof(Exception).Module).Find("System.Exception", true);
            //foreach (var except_ctor in except.FindMethods(".ctor"))
            //    if (except_ctor.Parameters.Count == 2)
            //        cli.Instructions.Add(Instruction.Create(OpCodes.Newobj,
            //         meth.Module.Import(except_ctor)));
            //cli.Instructions.Add(Instruction.Create(OpCodes.Pop));
            #endregion
            //
            cli.Instructions.Add(new Instruction(OpCodes.Ldstr, ByteEncryption.XorClass.xIT(pos + " " + id + " " + size)));
            cli.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, meth.Parameters.Count));
            cli.Instructions.Add(Instruction.Create(OpCodes.Newarr, meth.Module.CorLibTypes.Object.ToTypeDefOrRef()));
            //
            for (var i = 0; i < meth.Parameters.Count; i++)
            {
                var par = meth.Parameters[i];
                cli.Instructions.Add(new Instruction(OpCodes.Dup));
                cli.Instructions.Add(new Instruction(OpCodes.Ldc_I4, i));
                if (containsOut)
                {
                    if (rrr.Contains(meth.Parameters[i]))
                    {
                        cli.Instructions.Add(new Instruction(OpCodes.Ldloc, testerDictionary[meth.Parameters[i]]));
                    }
                    else
                    {
                        cli.Instructions.Add(new Instruction(OpCodes.Ldarg, meth.Parameters[i]));
                    }
                }
                else
                {
                    cli.Instructions.Add(new Instruction(OpCodes.Ldarg, meth.Parameters[i]));
                }
                if (true)
                {
                    cli.Instructions.Add(par.Type.FullName.EndsWith("&")
                        ? new Instruction(OpCodes.Box, par.Type.Next.ToTypeDefOrRef())
                        : new Instruction(OpCodes.Box, par.Type.ToTypeDefOrRef()));
                    cli.Instructions.Add(new Instruction(OpCodes.Stelem_Ref));
                }
            }
            #region Ex
            //for (var i = 0; i < meth.Parameters.Count; i++)
            //{
            //    foreach (var param in meth.Parameters)
            //    {
            //        var par = meth.Parameters[i];
            //        cli.Instructions.Add(new Instruction(OpCodes.Dup));
            //        cli.Instructions.Add(new Instruction(OpCodes.Ldc_I4, i));
            //        if (containsOut)
            //            if (rrr.Contains(meth.Parameters[i]))
            //            {
            //                cli.Instructions.Add(new Instruction(OpCodes.Ldloc, testerDictionary[meth.Parameters[i]]));
            //            }
            //            else
            //            {
            //                cli.Instructions.Add(new Instruction(OpCodes.Ldarg, meth.Parameters[i]));
            //            }
            //        else
            //            cli.Instructions.Add(new Instruction(OpCodes.Ldarg, meth.Parameters[i]));

            //        if (true)
            //        {
            //            cli.Instructions.Add(par.Type.FullName.EndsWith("&")
            //                ? new Instruction(OpCodes.Box, par.Type.Next.ToTypeDefOrRef())
            //                : new Instruction(OpCodes.Box, par.Type.ToTypeDefOrRef()));
            //            cli.Instructions.Add(new Instruction(OpCodes.Stelem_Ref));
            //        }
            //        //
            //        //cli.Instructions.Add(Instruction.Create(OpCodes.Dup));
            //        //cli.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, param.Index));
            //        //cli.Instructions.Add(Instruction.Create(OpCodes.Ldarg, param));
            //        ////
            //        //if (param.Type.IsPointer)
            //        //{
            //        //    cli.Instructions.Add(Instruction.Create(OpCodes.Ldtoken, param.Type.ToTypeDefOrRef()));
            //        //    cli.Instructions.Add(Instruction.Create(OpCodes.Call,
            //        //   meth.Module.Import(typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static))));
            //        //    //
            //        //    cli.Instructions.Add(Instruction.Create(OpCodes.Call,
            //        //        meth.Module.Import(typeof(Pointer).GetMethod("Box", BindingFlags.Public | BindingFlags.Static))));
            //        //}
            //        //else if (param.Type.IsValueType)
            //        //    cli.Instructions.Add(Instruction.Create(OpCodes.Box, param.Type.ToTypeDefOrRef()));
            //        ////
            //        //cli.Instructions.Add(Instruction.Create(OpCodes.Stelem_Ref));
            //    }
            //}
            #endregion
            //———//
            cli.Instructions.Add(new Instruction(OpCodes.Call, convertBack));
            //———//
            if (meth.HasReturnType)
                cli.Instructions.Add(new Instruction(OpCodes.Unbox_Any, meth.ReturnType.ToTypeDefOrRef()));
            else
                cli.Instructions.Add(new Instruction(OpCodes.Stloc, loc));
            //
            if (containsOut)
            {
                foreach (var parameter in rrr)
                {
                    cli.Instructions.Add(new Instruction(OpCodes.Ldarg, parameter));
                    cli.Instructions.Add(new Instruction(OpCodes.Ldloc, loc));
                    cli.Instructions.Add(new Instruction(OpCodes.Ldloc, loc2));
                    cli.Instructions.Add(new Instruction(OpCodes.Ldc_I4, meth.Parameters.IndexOf(parameter)));
                    cli.Instructions.Add(new Instruction(OpCodes.Ldelem_Ref));
                    cli.Instructions.Add(new Instruction(OpCodes.Unbox_Any, parameter.Type.Next.ToTypeDefOrRef()));
                    cli.Instructions.Add(new Instruction(OpCodes.Stind_Ref));
                }
                cli.Instructions.Add(new Instruction(OpCodes.Ret));
            }
            else
            {
                cli.Instructions.Add(new Instruction(OpCodes.Ret));
            }
            //
            //foreach (var type in Protector.moduleDefMD.GetTypes())
            //{
            //    if (type.Name != "<Module>") continue;
            //    foreach (var method in type.Methods)
            //    {
            //        if (method.IsRuntimeSpecialName || method.IsSpecialName || method.Name == "Invoke") continue;
            //        method.Name = ICore.Utils.GenerateString();
            //        cli.Instructions.Insert(1, new Instruction(OpCodes.Br_S, cli.Instructions[1]));
            //        cli.Instructions.Insert(2, new Instruction(OpCodes.Unaligned, 0));
            //        method.Body.Instructions.Insert(1, new Instruction(OpCodes.Br_S, method.Body.Instructions[1]));
            //        method.Body.Instructions.Insert(2, new Instruction(OpCodes.Unaligned, 0));
            //        for (int i = 0; i < 5; i++)
            //        {
            //            cli.Instructions.Insert(0, OpCodes.UNKNOWN1.ToInstruction());
            //            cli.Instructions.Insert(1, OpCodes.Nop.ToInstruction());
            //            cli.Instructions.Insert(2, OpCodes.UNKNOWN2.ToInstruction());
            //            method.Body.Instructions.Insert(0,  OpCodes.UNKNOWN1.ToInstruction());
            //            method.Body.Instructions.Insert(1, OpCodes.Nop.ToInstruction());
            //            method.Body.Instructions.Insert(2, OpCodes.UNKNOWN2.ToInstruction());
            //        }
            //    }
            //}
            meth.Body = cli;
            meth.Body.UpdateInstructionOffsets();
            meth.Body.MaxStack += 10;          
            ICore.Utils.EnsureNoInlining(meth);
            DnlibUtils.Optimize(meth);
        }
        public static Module conversionAssembly { get; set; }
        public static ModuleDefMD conversionDef { get; set; }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
