using dnlib.DotNet.Emit;
using System.Collections.Generic;
using ICore;
using System;
using dnlib.DotNet;
using System.Linq;

namespace Protections.Mutation
{
    internal static class charRuntime
    {
        public static int Add(int i)
        {
            return i;
        }
    }
    public class charMutation 
    {
        public static MethodDef Converter { get; set; }
        public static void Inject(Context ctx)
        {
            ModuleDefMD moduleDefMD = ModuleDefMD.Load(typeof(charRuntime).Module);
            TypeDef typeDef = moduleDefMD.ResolveTypeDef(MDToken.ToRID(typeof(charRuntime).MetadataToken));
            IEnumerable<IDnlibDef> defs = xHelpers.injection.InjectHelper.Inject(typeDef, ctx.Module.GlobalType, ctx.Module);
            Converter = (MethodDef)defs.Single((IDnlibDef method) => method.Name == "Add");
            Converter.Name = ICore.Utils.MethodsRenamig();
        }
        public static void Process(MethodDef Method, ref int i)
        {
            Method.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Ldc_I4, 0));
            Method.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Call, Converter));
            Method.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Add));
        }
    }
    public class IntsToMath
    {
        private MethodDef Method { get; set; }
        private static readonly Random rnd = new Random();
        public IntsToMath(MethodDef method)
        {
            Method = method;
        }
        public void Execute(MethodDef method, ref int i)
        {
            switch (rnd.Next(0, 10))
            {
                case 0:
                    Neg(ref i);
                    //Shuffler.Instructions.Shuffler.Execute(method, ref i);
                    break;
                case 1:
                    Not(ref i);
                    //Shuffler.Instructions.Shuffler.Execute(method, ref i);
                    break;
                case 2:
                    Shr(ref i);
                    //Shuffler.Instructions.Shuffler.Execute(method, ref i);
                    break;
                case 3:
                    Shl(ref i);
                    //Shuffler.Instructions.Shuffler.Execute(method, ref i);
                    break;
                case 4: 
                    Or(ref i);
                    //Shuffler.Instructions.Shuffler.Execute(method, ref i);
                    break;
                case 5: 
                    Rem(ref i);
                    //Shuffler.Instructions.Shuffler.Execute(method, ref i);
                    break;
                case 6: 
                    ConditionalMath(ref i);
                    //Shuffler.Instructions.Shuffler.Execute(method, ref i);
                    break;
                case 7: 
                    Add(ref i);
                    //Shuffler.Instructions.Shuffler.Execute(method, ref i);
                    break;
                case 8: 
                    Sub(ref i);
                    //Shuffler.Instructions.Shuffler.Execute(method, ref i);
                    break;
                case 9:
                    Xor(ref i);
                    //Shuffler.Instructions.Shuffler.Execute(method, ref i);
                    break;
            }
        }
        private void Sub(ref int i)
        {
            int value = Method.Body.Instructions[i].GetLdcI4Value();
            int random = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            int total = value - random;
            Method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
            Method.Body.Instructions[i].Operand = total;
            Method.Body.Instructions.Insert(++i, OpCodes.Ldc_I4.ToInstruction(random));
            Method.Body.Instructions.Insert(++i, OpCodes.Add.ToInstruction());
        }
        private void Xor(ref int i)
        {
            int value = Method.Body.Instructions[i].GetLdcI4Value();
            int random = ICore.Utils.RandomTinyInt32();
            int total = value ^ random;
            Method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
            Method.Body.Instructions[i].Operand = total;
            Method.Body.Instructions.Insert(++i, OpCodes.Ldc_I4.ToInstruction(random));
            Method.Body.Instructions.Insert(++i, OpCodes.Xor.ToInstruction());
        }
        private void Add(ref int i)
        {
            int value = Method.Body.Instructions[i].GetLdcI4Value();
            int random = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            int total = value + random;
            Method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
            Method.Body.Instructions[i].Operand = total;
            Method.Body.Instructions.Insert(++i, OpCodes.Ldc_I4.ToInstruction(random));
            Method.Body.Instructions.Insert(++i, OpCodes.Sub.ToInstruction());
        }
        private void Neg(ref int i)
        {
            int value = Method.Body.Instructions[i].GetLdcI4Value();
            int random = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            int nr = -random;
            Calculator calculator = new Calculator(value, nr);
            Method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
            Method.Body.Instructions[i].Operand = calculator.getResult();
            Method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(random));
            Method.Body.Instructions.Insert(i + 2, OpCodes.Neg.ToInstruction());
            Method.Body.Instructions.Insert(i + 3, calculator.getOpCode().ToInstruction());
            i += 3;
        }
        private void Rem(ref int i)
        {
            int value = Method.Body.Instructions[i].GetLdcI4Value();
            int random = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            int random2 = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            int nr = random2 % random;
            Calculator calculator = new Calculator(value, nr);
            Method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
            Method.Body.Instructions[i].Operand = calculator.getResult();
            Method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(random2));
            Method.Body.Instructions.Insert(i + 2, OpCodes.Ldc_I4.ToInstruction(random));
            Method.Body.Instructions.Insert(i + 3, OpCodes.Rem.ToInstruction());
            Method.Body.Instructions.Insert(i + 4, calculator.getOpCode().ToInstruction());
            i += 4;
        }
        private void Not(ref int i)
        {
            int value = Method.Body.Instructions[i].GetLdcI4Value();
            int random = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            int nr = ~random;
            Calculator calculator = new Calculator(value, nr);
            Method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
            Method.Body.Instructions[i].Operand = calculator.getResult();
            Method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(random));
            Method.Body.Instructions.Insert(i + 2, OpCodes.Not.ToInstruction());
            Method.Body.Instructions.Insert(i + 3, calculator.getOpCode().ToInstruction());
            i += 3;
        }
        private void Shl(ref int i)
        {
            int value = Method.Body.Instructions[i].GetLdcI4Value();
            int random = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            int random2 = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            int nr = random2 << random;
            Calculator calculator = new Calculator(value, nr);
            Method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
            Method.Body.Instructions[i].Operand = calculator.getResult();
            Method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(random2));
            Method.Body.Instructions.Insert(i + 2, OpCodes.Ldc_I4.ToInstruction(random));
            Method.Body.Instructions.Insert(i + 3, OpCodes.Shl.ToInstruction());
            Method.Body.Instructions.Insert(i + 4, calculator.getOpCode().ToInstruction());
            i += 4;
        }
        private void Or(ref int i)
        {
            int value = Method.Body.Instructions[i].GetLdcI4Value();
            int random = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            int random2 = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            int nr = random2 | random;
            Calculator calculator = new Calculator(value, nr);
            Method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
            Method.Body.Instructions[i].Operand = calculator.getResult();
            Method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(random2));
            Method.Body.Instructions.Insert(i + 2, OpCodes.Ldc_I4.ToInstruction(random));
            Method.Body.Instructions.Insert(i + 3, OpCodes.Or.ToInstruction());
            Method.Body.Instructions.Insert(i + 4, calculator.getOpCode().ToInstruction());
            i += 4;
        }
        private void Shr(ref int i)
        {
            int value = Method.Body.Instructions[i].GetLdcI4Value();
            int random = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            int random2 = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            int nr = random2 >> random;
            Calculator calculator = new Calculator(value, nr);
            Method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
            Method.Body.Instructions[i].Operand = calculator.getResult();
            Method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(random2));
            Method.Body.Instructions.Insert(i + 2, OpCodes.Ldc_I4.ToInstruction(random));
            Method.Body.Instructions.Insert(i + 3, OpCodes.Shr.ToInstruction());
            Method.Body.Instructions.Insert(i + 4, calculator.getOpCode().ToInstruction());
            i += 4;
        }
        private void ConditionalMath(ref int i)
        {
            Instruction instr = Method.Body.Instructions[i];
            Local int_lcl = new Local(Method.Module.ImportAsTypeSig(typeof(int)));
            int Real_Number = instr.GetLdcI4Value();
            int randomvalue1 = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            int randomvalue2 = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            int l2;
            int l;
            if (randomvalue1 > randomvalue2)
            {
                l = Real_Number;
                l2 = Real_Number + Real_Number / 3;
            }
            else
            {
                l2 = Real_Number;
                l = Real_Number + Real_Number / 3;
            }
            Method.Body.Variables.Add(int_lcl);
            instr.OpCode = OpCodes.Ldc_I4;
            instr.Operand = randomvalue2;
            Method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldc_I4, randomvalue1));
            Method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Nop));//BGT.S
            Method.Body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Ldc_I4, l));
            Method.Body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Nop));//BR.S
            Method.Body.Instructions.Insert(i + 5, Instruction.Create(OpCodes.Ldc_I4, l2));
            Method.Body.Instructions.Insert(i + 6, Instruction.Create(OpCodes.Stloc, int_lcl));
            Method.Body.Instructions.Insert(i + 7, Instruction.Create(OpCodes.Ldloc, int_lcl));
            Method.Body.Instructions[i + 2].OpCode = OpCodes.Bgt_S;
            Method.Body.Instructions[i + 2].Operand = Method.Body.Instructions[i + 5];
            Method.Body.Instructions[i + 4].OpCode = OpCodes.Br_S;
            Method.Body.Instructions[i + 4].Operand = Method.Body.Instructions[i + 6];
            i += 7;
        }
    }
    public class Calculator
    {
        public static Random rnd = new Random();
        private OpCode cOpCode = null;
        private int result = 0;
        public Calculator(int value, int value2)
        {
            result = Calculate(value, value2);
        }
        public int getResult()
        {
            return result;
        }
        public OpCode getOpCode()
        {
            return cOpCode;
        }
        private int Calculate(int num, int num2)
        {
            int cresult = 0;
            int r = rnd.Next(0, 3);

            switch (r)
            {
                case 0:
                    cresult = num + num2;
                    cOpCode = OpCodes.Sub;
                    break;
                case 1:
                    cresult = num ^ num2;
                    cOpCode = OpCodes.Xor;
                    break;
                case 2:
                    cresult = num - num2;
                    cOpCode = OpCodes.Add;
                    break;
            }
            return cresult;
        }
    }
    public class Helpers
    {       
        public static bool CanObfuscate(IList<Instruction> instructions, int i)
        {
            try
            {
                if (instructions[i + 1].GetOperand() != null)
                    if (instructions[i + 1].Operand.ToString().Contains("bool"))
                        return false;
                if (instructions[i + 1].GetOpCode() == OpCodes.Newobj) return false;
                if (instructions[i].GetLdcI4Value() == 0 || instructions[i].GetLdcI4Value() == 1)
                    return false;
            }
            catch { }
            return true;
        }
    }
    public class MutationConfusion
    {
        public static void executeFor(MethodDef method)
        {
            if (method.HasBody && method.Body.HasInstructions)
            {
                var instrs = method.Body.Instructions;
                for (int i = 0; i < instrs.Count; i++)
                {
                    if (method.Body.Instructions[i].Operand != null
                         && method.Body.Instructions[i].IsLdcI4()
                            && Helpers.CanObfuscate(method.Body.Instructions, i)
                            && method.Body.Instructions[i].Operand != null
                            && method.Body.Instructions[i].GetLdcI4Value() < int.MaxValue)
                    {
                        Shuffler.Instructions.Shuffler.Execute(method, ref i);
                    }
                }
            }
        }
        public static void ExecuteNormal(Context context)
        {
            var module = context.Module;
            if (charMutation.Converter == null)
            {
                charMutation.Inject(context);
            }
            foreach (var type in module.GetTypes())
            {
                if (type.IsGlobalModuleType)
                    continue;
                if (type.Namespace == "Costura")
                    continue;
                foreach (var method in type.Methods)
                {
                    if (method.HasBody && method.Body.HasInstructions)
                    {
                        DnlibUtils.Simplify(method);
                        var maths = new IntsToMath(method);
                        var instrs = method.Body.Instructions;
                        for (int i = 0; i < instrs.Count; i++)
                        {
                            if (method.Body.Instructions[i].Operand != null
                                 && method.Body.Instructions[i].IsLdcI4()
                                    && Helpers.CanObfuscate(method.Body.Instructions, i)
                                    && method.Body.Instructions[i].Operand != null
                                    && method.Body.Instructions[i].GetLdcI4Value() < int.MaxValue)
                            {
                                maths.Execute(method, ref i);
                                charMutation.Process(method, ref i);
                                Shuffler.Instructions.Shuffler.Execute(method, ref i);
                            }
                        }
                        DnlibUtils.Optimize(method);
                    }
                }
            }
            charMutation.Converter = null;
        }
    }
}
