using dnlib.DotNet.Emit;

namespace Helpers.Emulator.Instructions {
    internal class Ldloc : EmuInstruction {
        internal override OpCode OpCode => OpCodes.Ldloc;

        internal override void Emulate(EmuContext context, Instruction instr) {
            context.Stack.Push(context.GetLocalValue((Local)instr.Operand));
        }
    }
}
