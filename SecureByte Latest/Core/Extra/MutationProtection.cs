//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using dnlib.DotNet;
//using dnlib.DotNet.Emit;


//namespace Protector.Protections
//{
// class MutationProtection
// {

//  private ModuleDef CurrentModule;
//  public void StartMutate(MethodDef method, ModuleDef module, int repeats)
//  {
//   CurrentModule = module;

//   Mutate1(method); //DisIntegration
//   Mutate3(method); //Arithmetic
//   Mutate2(method); //TimeSpan
//   Mutate4(method); //ArrayMutation
//   Mutate5(method); //IntMath
//   Mutate6(method); //ZeroReplacer
//   Mutate7(method); //Number to string


//   //Mutate1(method); //DisIntegration
//   //Mutate3(method); //Arithmetic
//   //Mutate2(method); //TimeSpan
//   //Mutate4(method); //ArrayMutation
//   //Mutate5(method); //IntMath
//   //Mutate6(method); //ZeroReplacer
//   //Mutate7(method); //Number to string

//  }

//  public void Mutate1(MethodDef method)
//  {
//   CilBody body = method.Body;
//   body.SimplifyBranches();
//   Random rnd = new Random();
//   int x = 0;
//   while (x < body.Instructions.Count)
//   {
//    if (body.Instructions[x].IsLdcI4())
//    {
//     int original = body.Instructions[x].GetLdcI4Value();
//     int multiplier = rnd.Next(5, 40);
//     body.Instructions[x].OpCode = OpCodes.Ldc_I4;
//     body.Instructions[x].Operand = multiplier * original;
//     body.Instructions.Insert(x + 1, Instruction.Create(OpCodes.Ldc_I4, multiplier));
//     body.Instructions.Insert(x + 2, Instruction.Create(OpCodes.Div));
//     x += 3;
//    }
//    else
//     x++;
//   }
//   Random R = new Random();
//   int num = 0;
//   ITypeDefOrRef type = null;
//   for (int i = 0; i < method.Body.Instructions.Count; i++)
//   {
//    Instruction instruction = method.Body.Instructions[i];
//    if (instruction.IsLdcI4())
//    {
//     switch (R.Next(1, 16))
//     {
//      case 1:
//       type = method.Module.Import(typeof(int));
//       num = 4;
//       break;
//      case 2:
//       type = method.Module.Import(typeof(sbyte));
//       num = 1;
//       break;
//      case 3:
//       type = method.Module.Import(typeof(byte));
//       num = 1;
//       break;
//      case 4:
//       type = method.Module.Import(typeof(bool));
//       num = 1;
//       break;
//      case 5:
//       type = method.Module.Import(typeof(decimal));
//       num = 16;
//       break;
//      case 6:
//       type = method.Module.Import(typeof(short));
//       num = 2;
//       break;
//      case 7:
//       type = method.Module.Import(typeof(long));
//       num = 8;
//       break;
//      case 8:
//       type = method.Module.Import(typeof(uint));
//       num = 4;
//       break;
//      case 9:
//       type = method.Module.Import(typeof(float));
//       num = 4;
//       break;
//      case 10:
//       type = method.Module.Import(typeof(char));
//       num = 2;
//       break;
//      case 11:
//       type = method.Module.Import(typeof(ushort));
//       num = 2;
//       break;
//      case 12:
//       type = method.Module.Import(typeof(double));
//       num = 8;
//       break;
//      case 13:
//       type = method.Module.Import(typeof(DateTime));
//       num = 8;
//       break;
//      case 14:
//       type = method.Module.Import(typeof(ConsoleKeyInfo));
//       num = 12;
//       break;
//      case 15:
//       type = method.Module.Import(typeof(Guid));
//       num = 16;
//       break;
//     }
//     int num2 = R.Next(1, 1000);
//     bool flag = Convert.ToBoolean(R.Next(0, 2));
//     switch ((num != 0) ? ((Convert.ToInt32(instruction.Operand) % num == 0) ? R.Next(1, 5) : R.Next(1, 4)) : R.Next(1, 4))
//     {
//      case 1:
//       method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, type));
//       method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Add));
//       instruction.Operand = Convert.ToInt32(instruction.Operand) - num + (flag ? (-num2) : num2);
//       method.Body.Instructions.Insert(i + 3, Instruction.CreateLdcI4(num2));
//       method.Body.Instructions.Insert(i + 4, Instruction.Create(flag ? OpCodes.Add : OpCodes.Sub));
//       i += 4;
//       break;
//      case 2:
//       method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, type));
//       method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Sub));
//       instruction.Operand = Convert.ToInt32(instruction.Operand) + num + (flag ? (-num2) : num2);
//       method.Body.Instructions.Insert(i + 3, Instruction.CreateLdcI4(num2));
//       method.Body.Instructions.Insert(i + 4, Instruction.Create(flag ? OpCodes.Add : OpCodes.Sub));
//       i += 4;
//       break;
//      case 3:
//       method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, type));
//       method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Add));
//       instruction.Operand = Convert.ToInt32(instruction.Operand) - num + (flag ? (-num2) : num2);
//       method.Body.Instructions.Insert(i + 3, Instruction.CreateLdcI4(num2));
//       method.Body.Instructions.Insert(i + 4, Instruction.Create(flag ? OpCodes.Add : OpCodes.Sub));
//       i += 4;
//       break;
//      case 4:
//       method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, type));
//       method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Mul));
//       instruction.Operand = Convert.ToInt32(instruction.Operand) / num;
//       i += 2;
//       break;
//      default:
//       method.Body.Instructions.Insert(i + 3, Instruction.CreateLdcI4(num2));
//       method.Body.Instructions.Insert(i + 4, Instruction.Create(flag ? OpCodes.Add : OpCodes.Sub));
//       i += 4;
//       break;
//     }
//    }
//   }
//  }

//  public void Mutate2(MethodDef method)
//  {
//   Importer importer = new Importer(method.Module);
//   for (int i = 0; i < method.Body.Instructions.Count; i++)
//   {
//    if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
//    {
//     if (!DnLibHelper.CanMutateLDCI4(method.Body.Instructions, i)) continue;
//     int operand = method.Body.Instructions[i].GetLdcI4Value();
//     if (operand < 1 || operand > 10099999) continue;

//     TypeRef timespanRef = new TypeRefUser(method.Module, "System", "TimeSpan", method.Module.CorLibTypes.AssemblyRef);
//     int hours = new Random().Next(0, 10);
//     int minutes = new Random().Next(0, 10);
//     int days = method.Body.Instructions[i].GetLdcI4Value() - hours - minutes;
//     Local lcl = new Local(importer.Import(timespanRef.ToTypeSig()));
//     method.Body.Variables.Add(lcl);
//     method.Body.Instructions[i] = Instruction.CreateLdcI4(days);

//     method.Body.Instructions.Insert(i + 1, Instruction.CreateLdcI4(hours * 24));
//     method.Body.Instructions.Insert(i + 2, Instruction.CreateLdcI4(minutes * 1440));
//     method.Body.Instructions.Insert(i + 3, Instruction.CreateLdcI4(new Random().Next(0, 59)));
//     method.Body.Instructions.Insert(i + 4, OpCodes.Newobj.ToInstruction(importer.Import(typeof(TimeSpan).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }))));
//     method.Body.Instructions.Insert(i + 5, OpCodes.Stloc.ToInstruction(lcl));
//     method.Body.Instructions.Insert(i + 6, OpCodes.Ldloca.ToInstruction(lcl));
//     method.Body.Instructions.Insert(i + 7, OpCodes.Call.ToInstruction(importer.Import(typeof(TimeSpan).GetMethod("get_TotalDays"))));
//     method.Body.Instructions.Insert(i + 8, OpCodes.Conv_I4.ToInstruction());

//     i += 8;
//    }
//   }
//  }

//  public void Mutate3(MethodDef mDef)
//  {
//   for (int i = 0; i < mDef.Body.Instructions.Count; i++)
//   {
//    if (ArithmeticUtils.CheckArithmetic(mDef.Body.Instructions[i]))
//    {
//     if (mDef.Body.Instructions[i].GetLdcI4Value() < 0)
//     {
//      iFunction iFunction = Tasks[new Random().Next(5)];
//      List<Instruction> lstInstr = ArithmeticUtils.GenerateBody(iFunction.Arithmetic(mDef.Body.Instructions[i], mDef.Module), mDef.Module);
//      if (lstInstr == null) continue;
//      mDef.Body.Instructions[i].OpCode = OpCodes.Nop;
//      foreach (Instruction instr in lstInstr)
//      {
//       mDef.Body.Instructions.Insert(i + 1, instr);
//       i++;
//      }
//     }
//     else
//     {
//      iFunction iFunction = Tasks[new Random().Next(Tasks.Count)];
//      List<Instruction> lstInstr = ArithmeticUtils.GenerateBody(iFunction.Arithmetic(mDef.Body.Instructions[i], mDef.Module), mDef.Module);
//      if (lstInstr == null) continue;
//      mDef.Body.Instructions[i].OpCode = OpCodes.Nop;
//      foreach (Instruction instr in lstInstr)
//      {
//       mDef.Body.Instructions.Insert(i + 1, instr);
//       i++;
//      }
//     }
//    }
//   }
//  }

//  List<iFunction> Tasks = new List<iFunction>()
//            {
//                new Arithmetic.Functions.Add(),
//                new Arithmetic.Functions.Sub(),
//                new Arithmetic.Functions.Div(),
//                new Arithmetic.Functions.Mul(),
//                new Arithmetic.Functions.Xor(),
//                new Arithmetic.Functions.Maths.Abs(),
//                new Arithmetic.Functions.Maths.Log(),
//                new Arithmetic.Functions.Maths.Log10(),
//                new Arithmetic.Functions.Maths.Sin(),
//                new Arithmetic.Functions.Maths.Cos(),
//                new Arithmetic.Functions.Maths.Floor(),
//                new Arithmetic.Functions.Maths.Round(),
//                new Arithmetic.Functions.Maths.Tan(),
//                new Arithmetic.Functions.Maths.Tanh(),
//                new Arithmetic.Functions.Maths.Sqrt(),
//                new Arithmetic.Functions.Maths.Ceiling(),
//                new Arithmetic.Functions.Maths.Truncate()
//            };

//  public void Mutate4(MethodDef method)
//  {
//   Index = 0;
//   Integers_Position = new List<int>();
//   Integers = new List<int>();
//   instr = new List<Instruction>();

//   //STEP=1
//   instr.Add(Instruction.Create(OpCodes.Ldc_I4, Index));
//   instr.Add(Instruction.Create(OpCodes.Newarr, method.Module.Import(typeof(int))));
//   instr.Add(OpCodes.Dup.ToInstruction());
//   Local local = new Local(method.Module.CorLibTypes.Int32);
//   method.Body.Variables.Add(local);
//   for (int i = 0; i < method.Body.Instructions.Count; i++)
//   {
//    if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
//    {
//     if (!DnLibHelper.CanMutateLDCI4(method.Body.Instructions, i)) continue;
//     int xorMe = rnd.Next(50, 500);
//     Integers.Add(method.Body.Instructions[i].GetLdcI4Value() ^ xorMe);
//     Integers.Add(xorMe);
//     Integers_Position.Add(i);
//     method.Body.Instructions[i] = new Instruction(OpCodes.Ldstr, "66636");
//     method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Call, method.Module.Import(typeof(int).GetMethod("Parse", new Type[] { typeof(string) }))));
//     instr.Add(OpCodes.Ldc_I4.ToInstruction(Index));
//     instr.Add(new Instruction(OpCodes.Ldc_I4, Integers[Index]));
//     instr.Add(OpCodes.Stelem_I4.ToInstruction());
//     instr.Add(OpCodes.Dup.ToInstruction());

//     instr.Add(OpCodes.Ldc_I4.ToInstruction(Index + 1));
//     instr.Add(new Instruction(OpCodes.Ldc_I4, Integers[Index + 1]));
//     instr.Add(OpCodes.Stelem_I4.ToInstruction());
//     instr.Add(OpCodes.Dup.ToInstruction());
//     Index += 2;
//    }
//   }

//   if (instr[instr.Count - 1].OpCode == OpCodes.Dup && instr.Count > 4)
//    instr[instr.Count - 1] = new Instruction(OpCodes.Nop);
//   if (instr.Count > 4)
//    instr[0].Operand = Index;
//   instr.Add(Instruction.Create(OpCodes.Stloc_S, local));
//   //STEP=2

//   int z = 0;
//   int y = 0;
//   for (int i = 0; i < method.Body.Instructions.Count; i++)
//   {
//    var abc = method.Module.Import(typeof(int).GetMethod("Parse", new Type[] { typeof(string) }));
//    var abc2 = "66636";
//    if (method.Body.Instructions[i].Operand == null) continue;
//    if (method.Body.Instructions[i].Operand.ToString() == abc.ToString())
//    {
//     if (method.Body.Instructions[i - 1].Operand.ToString() == abc2)
//     {
//      if (z == 0)
//      {
//       int x = 0;
//       foreach (Instruction instruction in instr)
//       {
//        method.Body.Instructions.Insert(x, instruction);
//        x++;
//        i++;
//        z++;
//       }
//      }

//      method.Body.Instructions[i] = new Instruction(OpCodes.Ldloc_S, local);
//      method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(y));
//      method.Body.Instructions.Insert(i + 2, OpCodes.Ldelem_I4.ToInstruction());
//      method.Body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Ldloc_S, local));
//      method.Body.Instructions.Insert(i + 4, OpCodes.Ldc_I4.ToInstruction(y + 1));
//      method.Body.Instructions.Insert(i + 5, OpCodes.Ldelem_I4.ToInstruction());
//      method.Body.Instructions.Insert(i + 6, OpCodes.Xor.ToInstruction());
//      y += 2;
//      method.Body.Instructions.RemoveAt(i - 1);
//     }
//    }
//   }
//  }
//  public static List<Instruction> instr = new List<Instruction>();
//  static List<int> Integers = new List<int>();
//  static List<int> Integers_Position = new List<int>();
//  static int Index = 0;
//  static Random rnd = new Random();

//  public void Mutate5(MethodDef method)
//  {
//   for (int i = 0; i < method.Body.Instructions.Count; i++)
//   {
//    if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
//    {
//     List<Instruction> instructions = Calc(Convert.ToInt32(method.Body.Instructions[i].Operand));
//     method.Body.Instructions[i].OpCode = OpCodes.Nop;
//     foreach (Instruction instr in instructions)
//     {
//      method.Body.Instructions.Insert(i + 1, instr);
//      i++;
//     }
//    }
//   }

//   for (int i = 0; i < method.Body.Instructions.Count; i++)
//   {
//    if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
//    {
//     int operand = method.Body.Instructions[i].GetLdcI4Value();
//     if (operand <= 1) continue;
//     var two = NextInt(1, 10);
//     method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
//     method.Body.Instructions[i].Operand = operand * two;
//     method.Body.Instructions.Insert(++i, new Instruction(OpCodes.Ldc_I4, two));
//     method.Body.Instructions.Insert(++i, new Instruction(OpCodes.Div));
//    }
//   }

//   for (int i = 0; i < method.Body.Instructions.Count; i++)
//   {
//    if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
//    {
//     int operand = method.Body.Instructions[i].GetLdcI4Value();
//     if (operand <= 1) continue;
//     var two = NextInt(1, (int)((double)operand / 1.5));
//     var one = operand / two;
//     while (two * one != operand)
//     {
//      two = NextInt(1, (int)((double)operand / 1.5));
//      one = operand / two;
//     }
//     method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
//     method.Body.Instructions[i].Operand = one;
//     method.Body.Instructions.Insert(++i, new Instruction(OpCodes.Ldc_I4, two));
//     method.Body.Instructions.Insert(++i, new Instruction(OpCodes.Mul));
//    }
//   }
//  }

//  private List<Instruction> Calc(int value)
//  {
//   List<Instruction> instructions = new List<Instruction>();
//   int num = new Random(Guid.NewGuid().GetHashCode()).Next(0, 100000);
//   bool once = Convert.ToBoolean(new Random(Guid.NewGuid().GetHashCode()).Next(0, 2));
//   int num1 = new Random(Guid.NewGuid().GetHashCode()).Next(0, 100000);
//   instructions.Add(Instruction.Create(OpCodes.Ldc_I4, value - num + (once ? (0 - num1) : num1)));
//   instructions.Add(Instruction.Create(OpCodes.Ldc_I4, num));
//   instructions.Add(Instruction.Create(OpCodes.Add));
//   instructions.Add(Instruction.Create(OpCodes.Ldc_I4, num1));
//   instructions.Add(Instruction.Create(once ? OpCodes.Add : OpCodes.Sub));
//   return instructions;
//  }

//  public int NextInt(int minValue, int maxValue)
//  {
//   byte[] b = new byte[4];
//   new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
//   return (int)Math.Floor((double)(BitConverter.ToUInt32(b, 0) / uint.MaxValue) * (maxValue - minValue)) + minValue;
//  }

//  public void Mutate6(MethodDef method)
//  {
//   for (int i = 0; i < method.Body.Instructions.Count; i++)
//   {
//    if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
//    {
//     if (method.Body.Instructions[i].GetLdcI4Value() == 0)
//     {
//      switch (new Random().Next(0, 2))
//      {
//       case 0:
//        method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Add));
//        break;
//       case 1:
//        method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sub));
//        break;
//      }
//      method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldsfld, method.Module.Import(typeof(Type).GetField("EmptyTypes"))));
//      method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Ldlen));
//      i += 2;
//     }
//    }
//   }
//  }

//  public void Mutate7(MethodDef method)
//  {
//   for (int i = 0; i < method.Body.Instructions.Count; i++)
//   {
//    if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
//    {
//     string value = method.Body.Instructions[i].Operand.ToString();
//     method.Body.Instructions[i].OpCode = OpCodes.Nop;
//     method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldstr, value));
//     method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Call, method.Module.Import(typeof(int).GetMethod("Parse", new Type[] { typeof(string) }))));
//     i += 2;
//    }
//    else if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4_S)
//    {
//     string value = method.Body.Instructions[i].Operand.ToString();
//     method.Body.Instructions[i].OpCode = OpCodes.Nop;
//     method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldstr, value));
//     method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Call, method.Module.Import(typeof(short).GetMethod("Parse", new Type[] { typeof(string) }))));
//     i += 2;
//    }
//    else if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I8)
//    {
//     string value = method.Body.Instructions[i].Operand.ToString();
//     method.Body.Instructions[i].OpCode = OpCodes.Nop;
//     method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldstr, value));
//     method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Call, method.Module.Import(typeof(long).GetMethod("Parse", new Type[] { typeof(string) }))));
//     i += 2;
//    }
//    else if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_R4)
//    {
//     string value = method.Body.Instructions[i].Operand.ToString();
//     method.Body.Instructions[i].OpCode = OpCodes.Nop;
//     method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldstr, value));
//     method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Call, method.Module.Import(typeof(float).GetMethod("Parse", new Type[] { typeof(string) }))));
//     i += 2;
//    }
//   }
//  }





// }
//}
