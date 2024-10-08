﻿using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;
using System.Collections.Generic;
using static Protections.NormalCFlow.BlockParser;

namespace Protections.NormalCFlow
{
    internal abstract class ManglerBase
    {
        protected static IEnumerable<InstrBlock> GetAllBlocks(ScopeBlock scope)
        {
            foreach (BlockBase child in scope.Children)
            {
                if (child is InstrBlock)
                    yield return (InstrBlock)child;
                else
                {
                    foreach (InstrBlock block in GetAllBlocks((ScopeBlock)child))
                        yield return block;
                }
            }
        }
        public abstract void Mangle(CilBody body, ScopeBlock root, Context ctx, MethodDef method, TypeSig retType);
    }
}
