using dnlib.DotNet.Emit;
using System.Collections.Generic;
using System;
using dnlib.DotNet;

namespace Shuffler.Instructions
{
    internal static class Shuffler
    {
        private static Random rr = new Random();
        private static readonly OpCode[] opCodes = { OpCodes.Add, OpCodes.Sub, OpCodes.Xor, OpCodes.Shr, OpCodes.Shl };
        private static void confuse(List<Instruction> instructions)
        {
            int randomIndex = rr.Next(0, opCodes.Length);
            instructions.Add(Instruction.CreateLdcI4(0));
            instructions.Add(Instruction.Create(opCodes[randomIndex]));
        }
        private static void confuse2(List<Instruction> instructions)
        {
            int randomIndex = rr.Next(0, opCodes.Length);
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
            int numExtraInstructions = 1;
            for (int i = 0; i < numExtraInstructions; i++)
            {
                int secondRandomIndex = rr.Next(0, opCodes.Length);
                OpCode opCode = opCodes[secondRandomIndex];
                if (opCode == OpCodes.Shr || opCode == OpCodes.Shl)
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldc_I4_1));
                }
                else
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                }
                instructions.Add(Instruction.Create(opCode));
            }
            instructions.Add(Instruction.Create(opCodes[randomIndex]));
            confuse(instructions);
        }
        private static void confuse3(List<Instruction> instructions)
        {
            int randomIndex = rr.Next(0, opCodes.Length);
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
            int numExtraInstructions = 1;
            for (int i = 0; i < numExtraInstructions; i++)
            {
                int secondRandomIndex = rr.Next(0, opCodes.Length);
                OpCode opCode = opCodes[secondRandomIndex];
                if (opCode == OpCodes.Shr || opCode == OpCodes.Shl)
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldc_I4_1));
                }
                else
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                }
                instructions.Add(Instruction.Create(opCode));
            }
            OpCode finalOpCode = opCodes[randomIndex];
            switch (finalOpCode.Code)
            {
                case Code.Add:
                    instructions.Add(Instruction.Create(OpCodes.Sub));
                    break;
                case Code.Sub:
                    instructions.Add(Instruction.Create(OpCodes.Add));
                    break;
                case Code.Xor:
                    instructions.Add(Instruction.Create(OpCodes.Xor));
                    break;
                case Code.Shr:
                    instructions.Add(Instruction.Create(OpCodes.Shl));
                    break;
                case Code.Shl:
                    instructions.Add(Instruction.Create(OpCodes.Shr));
                    break;
                default:
                    throw new ArgumentException("Unsupported OpCode");
            }
        }
        private static void confuse4(List<Instruction> instructions)
        {
            int randomIndex = rr.Next(0, opCodes.Length);
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
            int numExtraInstructions = 1;
            for (int i = 0; i < numExtraInstructions; i++)
            {
                int secondRandomIndex = rr.Next(0, opCodes.Length);
                OpCode opCode = opCodes[secondRandomIndex];
                if (opCode == OpCodes.Shr || opCode == OpCodes.Shl)
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldc_I4_1));
                }
                else
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                }
                instructions.Add(Instruction.Create(opCode));
            }
            OpCode differentOpCode;
            do
            {
                differentOpCode = opCodes[rr.Next(0, opCodes.Length)];
            } while (differentOpCode == opCodes[randomIndex]);
            instructions.Add(Instruction.Create(differentOpCode));
        }
        private static void confuse5(List<Instruction> instructions)
        {
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
            int numExtraInstructions = 1;
            for (int ii = 0; ii < numExtraInstructions; ii++)
            {
                int secondRandomIndex = rr.Next(0, opCodes.Length);
                OpCode opCode = opCodes[secondRandomIndex];
                if (opCode == OpCodes.Shr || opCode == OpCodes.Shl)
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldc_I4_1));
                }
                else
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                }
                instructions.Add(Instruction.Create(opCode));
            }
            instructions.Add(Instruction.Create(OpCodes.Sub));
            confuse(instructions);
        }
        private static void confuse(MethodDef Method, ref int i)
        {
            int randomIndex = rr.Next(0, opCodes.Length);
            Method.Body.Instructions.Insert(++i, OpCodes.Ldc_I4.ToInstruction(0));
            Method.Body.Instructions.Insert(++i, opCodes[randomIndex].ToInstruction());
        }
        private static void confuse2(MethodDef Method, ref int i)
        {
            int randomIndex = rr.Next(0, opCodes.Length);
            Method.Body.Instructions.Insert(++i, OpCodes.Ldc_I4_0.ToInstruction());
            int numExtraInstructions = 1;
            for (int it = 0; it < numExtraInstructions; it++)
            {
                int secondRandomIndex = rr.Next(0, opCodes.Length);
                OpCode opCode = opCodes[secondRandomIndex];
                if (opCode == OpCodes.Shr || opCode == OpCodes.Shl)
                {
                    Method.Body.Instructions.Insert(++i, OpCodes.Ldc_I4_1.ToInstruction());
                }
                else
                {
                    Method.Body.Instructions.Insert(++i, OpCodes.Ldc_I4_0.ToInstruction());
                }
                Method.Body.Instructions.Insert(++i, opCode.ToInstruction());
            }
            Method.Body.Instructions.Insert(++i, opCodes[randomIndex].ToInstruction());
        }
        private static void confuse3(MethodDef Method, ref int i)
        {
            int randomIndex = rr.Next(0, opCodes.Length);
            Method.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Ldc_I4_0));
            int numExtraInstructions = 1;
            for (int it = 0; it < numExtraInstructions; it++)
            {
                int secondRandomIndex = rr.Next(0, opCodes.Length);
                OpCode opCode = opCodes[secondRandomIndex];
                if (opCode == OpCodes.Shr || opCode == OpCodes.Shl)
                {
                    Method.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Ldc_I4_1));
                }
                else
                {
                    Method.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Ldc_I4_0));
                }
                Method.Body.Instructions.Insert(++i, Instruction.Create(opCode));
            }
            OpCode differentOpCode;
            do
            {
                differentOpCode = opCodes[rr.Next(0, opCodes.Length)];
            } while (differentOpCode == opCodes[randomIndex]);
            Method.Body.Instructions.Insert(++i, Instruction.Create(differentOpCode));
        }
        private static void confuse4(MethodDef Method, ref int i)
        {
            Method.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Ldc_I4_0));
            int numExtraInstructions = 1;
            for (int ii = 0; ii < numExtraInstructions; ii++)
            {
                int secondRandomIndex = rr.Next(0, opCodes.Length);
                OpCode opCode = opCodes[secondRandomIndex];
                if (opCode == OpCodes.Shr || opCode == OpCodes.Shl)
                {
                    Method.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Ldc_I4_1));
                }
                else
                {
                    Method.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Ldc_I4_0));
                }
                Method.Body.Instructions.Insert(++i, Instruction.Create(opCode));
            }
            Method.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Sub));
            confuse(Method, ref i);
        }
        public static void Execute(List<Instruction> instructions)
        {
            switch(new Random().Next(0, 6))
            {
                case 0:
                    confuse(instructions);
                    break;
                case 1:
                    confuse2(instructions);
                    break;
                case 2:
                    confuse3(instructions);
                    break;
                case 3:
                    confuse4(instructions);
                    break;
                case 4:
                    confuse5(instructions);
                    break;
            }
        }
        public static void Execute(MethodDef Method, ref int i)
        {
            switch (new Random().Next(0, 4))
            {
                case 0:
                    confuse(Method, ref i);
                    break;
                case 1:
                    confuse2(Method, ref i);
                    break;
                case 2:
                    confuse3(Method, ref i);
                    break;
                case 3:
                    confuse4(Method, ref i);
                    break;
            }
        }
    }
}
