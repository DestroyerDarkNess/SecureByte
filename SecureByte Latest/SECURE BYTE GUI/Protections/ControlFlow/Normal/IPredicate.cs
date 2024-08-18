using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;
using System;
using System.Collections.Generic;

namespace Protections.NormalCFlow
{
    // so you can create predicate encode mode
    internal interface IPredicate
    {
        void Inititalize(CilBody body);
        void EmitSwitchLoad(IList<Instruction> instrs);
        int GetSwitchKey(int key);
    }
}