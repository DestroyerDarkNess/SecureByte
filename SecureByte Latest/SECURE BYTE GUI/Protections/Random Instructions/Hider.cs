using dnlib.DotNet.Emit;
using dnlib.DotNet;

namespace Shuffler.Instructions
{
    internal static class Hider
    {
        public static void addInstructions(MethodDef Method)
        {
            Method.Body.Instructions.Insert(0, new Instruction(OpCodes.Nop));
            Method.Body.Instructions.Insert(1, new Instruction(OpCodes.Br_S, Method.Body.Instructions[1]));
            Method.Body.Instructions.Insert(2, new Instruction(OpCodes.Unaligned, (byte)0));
        }
    }
}