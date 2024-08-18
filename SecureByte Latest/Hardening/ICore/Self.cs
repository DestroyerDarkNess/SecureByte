using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    public static class Self
    {
        static Random rnd = new Random();
        public static void Calc(MethodDef method)
        {
            for (int i = 0; i < method.Body.Instructions.Count; i++)
            {
                if (method.Body.Instructions[i].IsLdcI4())
                {
                    int op = method.Body.Instructions[i].GetLdcI4Value();
                    int newvalue = rnd.Next(-100, 10000);
                    switch (rnd.Next(1, 4))
                    {
                        case 1:
                            method.Body.Instructions[i].Operand = op - newvalue;
                            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(newvalue));
                            method.Body.Instructions.Insert(i + 2, OpCodes.Add.ToInstruction());
                            i += 2;
                            break;
                        case 2:
                            method.Body.Instructions[i].Operand = op + newvalue;
                            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(newvalue));
                            method.Body.Instructions.Insert(i + 2, OpCodes.Sub.ToInstruction());
                            i += 2;
                            break;
                        case 3:
                            method.Body.Instructions[i].Operand = op ^ newvalue;
                            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(newvalue));
                            method.Body.Instructions.Insert(i + 2, OpCodes.Xor.ToInstruction());
                            i += 2;
                            break;
                        case 4:
                            int operand = method.Body.Instructions[i].GetLdcI4Value();
                            method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                            method.Body.Instructions[i].Operand = operand - 1;
                            int valor = rnd.Next(100, 500);
                            int valor2 = rnd.Next(1000, 5000);
                            method.Body.Instructions.Insert(i + 1, Instruction.CreateLdcI4(valor));
                            method.Body.Instructions.Insert(i + 2, Instruction.CreateLdcI4(valor2));
                            method.Body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Clt));
                            method.Body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Conv_I4));
                            method.Body.Instructions.Insert(i + 5, Instruction.Create(OpCodes.Add));
                            i += 5;
                            break;
                    }
                }
            }
        }
        public static void SimpleMethodsHiding(MethodDef method)
        {
            if (method.HasBody)
            {
                method.Body.Instructions.Insert(0, new Instruction(OpCodes.Box, method.Module.Import(typeof(Math))));
            }
        }
    }

