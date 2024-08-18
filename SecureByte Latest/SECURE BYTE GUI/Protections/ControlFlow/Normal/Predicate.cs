using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;
using System;
using System.Collections.Generic;

namespace Protections.NormalCFlow
{
    internal class Predicate : IPredicate
    {
        readonly Context ctx;
        bool inited;
        int xorKey;

        public Predicate(Context ctx)
        {
            this.ctx = ctx;
        }

        public void Inititalize(CilBody body)
        {
            if (inited)
                return;

            xorKey = new Random().Next(); // 1905184866
            inited = true;
        }

        public int GetSwitchKey(int key)
        {
            return key ^ xorKey; // here is encode switch keys "num = 1145692050;"
        }

        public void EmitSwitchLoad(IList<Instruction> instrs) // here is decode.
        {
            // switch (num "^ 1905184866")
            instrs.Add(Instruction.Create(OpCodes.Ldc_I4, xorKey));
            instrs.Add(Instruction.Create(OpCodes.Xor));
        }
    }
}
