using dnlib.DotNet.Emit;

namespace Helpers.Emulator.Instructions {
    internal class Stloc : EmuInstruction {
        internal override OpCode OpCode => OpCodes.Stloc;

        internal override void Emulate(EmuContext context, Instruction instr) {
            var val = context.Stack.Pop();
            context.SetLocalValue((Local)instr.Operand, val);
        }
    }
}
