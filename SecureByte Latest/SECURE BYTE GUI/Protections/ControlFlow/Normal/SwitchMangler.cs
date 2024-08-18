using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;
using Protections.Software.Global;
using Protections.Strings;
using Shuffler.Instructions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static JIT.Runtime.JITRuntime;
using static Protections.NormalCFlow.BlockParser;

namespace Protections.NormalCFlow
{
    internal class SwitchMangler
    {
        public ModuleDefMD ctx { get; set; }
        static ExAntiTamper.Stuffs.RandomGenerator random;

        struct Trace
        {
            public Dictionary<uint, int> RefCount;
            public Dictionary<uint, List<Instruction>> BrRefs;
            public Dictionary<uint, int> BeforeStack;
            public Dictionary<uint, int> AfterStack;

            static void Increment(Dictionary<uint, int> counts, uint key)
            {
                int value;
                if (!counts.TryGetValue(key, out value))
                    value = 0;
                counts[key] = value + 1;
            }

            public Trace(CilBody body, bool hasReturnValue)
            {
                RefCount = new Dictionary<uint, int>();
                BrRefs = new Dictionary<uint, List<Instruction>>();
                BeforeStack = new Dictionary<uint, int>();
                AfterStack = new Dictionary<uint, int>();

                body.UpdateInstructionOffsets();

                foreach (ExceptionHandler eh in body.ExceptionHandlers)
                {
                    BeforeStack[eh.TryStart.Offset] = 0;
                    BeforeStack[eh.HandlerStart.Offset] = (eh.HandlerType != ExceptionHandlerType.Finally ? 1 : 0);
                    if (eh.FilterStart != null)
                        BeforeStack[eh.FilterStart.Offset] = 1;
                }

                int currentStack = 0;

                for (int i = 0; i < body.Instructions.Count; i++)
                {
                    var instr = body.Instructions[i];

                    if (BeforeStack.ContainsKey(instr.Offset))
                        currentStack = BeforeStack[instr.Offset];

                    BeforeStack[instr.Offset] = currentStack;
                    instr.UpdateStack(ref currentStack, hasReturnValue);
                    AfterStack[instr.Offset] = currentStack;

                    uint offset;

                    switch (instr.OpCode.FlowControl)
                    {
                        case FlowControl.Branch:
                            offset = ((Instruction)instr.Operand).Offset;
                            if (!BeforeStack.ContainsKey(offset))
                                BeforeStack[offset] = currentStack;

                            Increment(RefCount, offset);
                            BrRefs.AddListEntry(offset, instr);

                            currentStack = 0;
                            continue;
                        case FlowControl.Call:
                            if (instr.OpCode.Code == Code.Jmp)
                                currentStack = 0;
                            break;
                        case FlowControl.Cond_Branch:
                            if (instr.OpCode.Code == Code.Switch)
                            {
                                foreach (Instruction target in (Instruction[])instr.Operand)
                                {
                                    if (!BeforeStack.ContainsKey(target.Offset))
                                        BeforeStack[target.Offset] = currentStack;

                                    Increment(RefCount, target.Offset);
                                    BrRefs.AddListEntry(target.Offset, instr);
                                }
                            }
                            else
                            {
                                offset = ((Instruction)instr.Operand).Offset;
                                if (!BeforeStack.ContainsKey(offset))
                                    BeforeStack[offset] = currentStack;

                                Increment(RefCount, offset);
                                BrRefs.AddListEntry(offset, instr);
                            }
                            break;
                        case FlowControl.Meta:
                        case FlowControl.Next:
                        case FlowControl.Break:
                            break;
                        case FlowControl.Return:
                        case FlowControl.Throw:
                            continue;
                        default:
                            throw new Exception();
                    }

                    if (i + 1 < body.Instructions.Count)
                    {
                        offset = body.Instructions[i + 1].Offset;
                        Increment(RefCount, offset);
                    }
                }
            }

            public bool IsBranchTarget(uint offset)
            {
                List<Instruction> src;
                if (BrRefs.TryGetValue(offset, out src))
                    return src.Count > 0;
                return false;
            }

            public bool HasMultipleSources(uint offset)
            {
                int src;
                if (RefCount.TryGetValue(offset, out src))
                    return src > 1;
                return false;
            }
        }

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
        public void Mangle(CilBody body, ScopeBlock root, Context ctx, MethodDef Method, TypeSig retType)
        {
            random = new ExAntiTamper.Stuffs.RandomGenerator(new Guid().ToString(), new Guid().ToString());
            this.ctx = ctx.Module;
            Trace trace = new Trace(body, retType.RemoveModifiers().ElementType != ElementType.Void);
            var arraylocal = new Local(new SZArraySig(Method.Module.CorLibTypes.Int32));
            var fakeLocal = new Local(Method.Module.CorLibTypes.Int32);

            body.Variables.Add(arraylocal);
            body.Variables.Add(fakeLocal);

            body.InitLocals = true;
            body.MaxStack += 2;

            IPredicate predicate = new Predicate(ctx);
            predicate.Inititalize(body);

            foreach (InstrBlock block in GetAllBlocks(root))
            {
                LinkedList<Instruction[]> statements = SplitStatements(block, trace);
                if (Method.IsInstanceConstructor)
                {
                    var newStatement = new List<Instruction>();
                    while (statements.First != null)
                    {
                        newStatement.AddRange(statements.First.Value);
                        Instruction lastInstr = statements.First.Value.Last();
                        statements.RemoveFirst();
                        if (lastInstr.OpCode == OpCodes.Call && ((IMethod)lastInstr.Operand).Name == ".ctor")
                            break;
                    }
                    statements.AddFirst(newStatement.ToArray());
                }

                if (statements.Count < 3) continue;

                int i;

                var keyId = Enumerable.Range(0, statements.Count).ToArray();
                Shuffle(keyId);

                var key = new int[keyId.Length];
                for (i = 0; i < key.Length; i++)
                {
                    key[i] = keyId[i];
                }

                var statementKeys = new Dictionary<Instruction, int>();
                LinkedListNode<Instruction[]> current = statements.First;
                i = 0;
                while (current != null)
                {
                    if (i != 0)
                        statementKeys[current.Value[0]] = key[i];
                    i++;
                    current = current.Next;
                }

                var statementLast = new HashSet<Instruction>(statements.Select(st => st.Last()));

                Func<IList<Instruction>, bool> hasUnknownSource;
                hasUnknownSource = instrs => instrs.Any(instr =>
                {
                    if (trace.HasMultipleSources(instr.Offset))
                        return true;
                    if (trace.BrRefs.TryGetValue(instr.Offset, out List<Instruction> srcs))
                    {
                        // Target of switch => assume unknown
                        if (srcs.Any(src => src.Operand is Instruction[]))
                            return true;

                        // Not within current instruction block / targeted in first statement
                        if (srcs.Any(src => src.Offset <= statements.First.Value.Last().Offset ||
                                            src.Offset >= block.Instructions.Last().Offset))
                            return true;

                        // Disable flow obfuscation for blocks reached by jump instructions.
                        // Bug in #153 caused exactly this behaviour, expect for allowing wrong jump instructions
                        // There is another issue present here tracked here:
                        // https://github.com/mkaring/ConfuserEx/issues/162
                        // Until this issue is resolved, the ctrl flow obfuscation will be severely reduced.
                        if (srcs.Any())
                            return true;

                        // Not targeted by the last of statements
                        if (srcs.Any(src => !statementLast.Contains(src)))
                            return true;
                    }
                    return false;
                });

                var switchInstr = new Instruction(OpCodes.Switch);
                var jumpInstr = new Instruction(OpCodes.Nop);
                var switchHdr = new List<Instruction>
                {
                    Instruction.CreateLdcI4(predicate.GetSwitchKey(key[1])),
                    jumpInstr
                };
                predicate.EmitSwitchLoad(switchHdr);

                switchHdr.Add(Instruction.Create(OpCodes.Dup));
                switchHdr.Add(Instruction.Create(OpCodes.Stloc, fakeLocal));

                switchHdr.Add(switchInstr);
                switchHdr.Add(Instruction.Create(OpCodes.Br, statements.Last.Value[0])); // Jump to end of Switch.

                Shuffler.Instructions.Shuffler.Execute(switchHdr);

                var operands = new Instruction[statements.Count];
                current = statements.First;
                i = 0;
                while (current.Next != null)
                {
                    var newStatement = new List<Instruction>(current.Value);

                    if (i != 0)
                    {
                        var clone = newStatement.ToList(); // case begin instrs
                        newStatement.Clear();

                        // Case begin instrs encode
                        /////////////////////////////////////////////////////////////////////

                        // case num:
                        var nopgg = new Instruction(OpCodes.Nop);
                        newStatement.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                        newStatement.Add(Instruction.Create(OpCodes.Brtrue, nopgg));
                        newStatement.AddRange(clone);                   
                        newStatement.Add(nopgg); // else
                        //
                        //
                        // like this if
                        // because dude "if (true)" is sample if, so everytime is actinve if true, change if code
                        //
                        /////////////////////////////////////////////////////////////////////

                        // case end instrs

                        var unkSrc = hasUnknownSource(newStatement);
                        var converted = false;

                        if (newStatement.Last().IsBr()) // this is First if encoding
                        {
                            var target = (Instruction)newStatement.Last().Operand;
                            if (!trace.IsBranchTarget(newStatement.Last().Offset) &&
                                statementKeys.TryGetValue(target, out int brKey))
                            {
                                newStatement.RemoveAt(newStatement.Count - 1);

                                var targetKey = predicate.GetSwitchKey(brKey);
                                if (!unkSrc) // you can encode here
                                {
                                    newStatement.Add(Instruction.Create(OpCodes.Ldc_I4, targetKey));
                                }
                                else // "NEVER" ENCODE HERE!
                                {
                                    newStatement.Add(Instruction.Create(OpCodes.Ldc_I4, targetKey)); 
                                }

                                // "NEVER" ENCODE HERE!

                                newStatement.Add(Instruction.Create(OpCodes.Br, jumpInstr)); // Jump to Switch header.

                                operands[keyId[i]] = newStatement[0];
                                converted = true;
                            }
                        }
                        else if (newStatement.Last().IsConditionalBranch()) // this is for last if encoding
                        {
                            // Conditional

                            var target = (Instruction)newStatement.Last().Operand;
                            if (!trace.IsBranchTarget(newStatement.Last().Offset) &&
                                statementKeys.TryGetValue(target, out int brKey))
                            {
                                OpCode condBr = newStatement.Last().OpCode;
                                newStatement.RemoveAt(newStatement.Count - 1);

                                int nextKey = predicate.GetSwitchKey(key[i + 1]);
                                brKey = predicate.GetSwitchKey(brKey);

                                if (!unkSrc)
                                {
                                    // here is for encode (brKey, nextKey)
                                  
                                    // Sample
                                    brKey += 1;
                                    nextKey += 1;
                                }

                                Instruction brKeyInstr = Instruction.CreateLdcI4(brKey);
                                Instruction nextKeyInstr = Instruction.CreateLdcI4(nextKey);
                                Instruction pop = Instruction.Create(OpCodes.Nop);

                                newStatement.Add(Instruction.Create(condBr, brKeyInstr)); // unable if
                                newStatement.Add(nextKeyInstr);
                                newStatement.Add(Instruction.Create(OpCodes.Br, pop));
                                newStatement.Add(brKeyInstr);
                                newStatement.Add(pop);

                                if (!unkSrc)
                                {
                                    // here is for decode (brKey, nextKey) [add here decode instrs]
                                   
                                    // Sample
                                    newStatement.Add(Instruction.Create(OpCodes.Ldc_I4, 1));
                                    newStatement.Add(Instruction.Create(OpCodes.Add));
                                }

                                // "NEVER" ENCODE HERE!

                                newStatement.Add(Instruction.Create(OpCodes.Br, jumpInstr)); // Jump to Switch header.

                                operands[keyId[i]] = newStatement[0];
                                converted = true;
                            }
                        }

                        if (!converted) // this is normal encoding
                        {
                            var targetKey = predicate.GetSwitchKey(key[i + 1]);
                            if (!hasUnknownSource(newStatement)) // you can encode here
                            {
                                switch (random.NextInt32(0, 2))
                                {
                                    case 0:
                                        {
                                            var thisKey = key[i];
                                            var r = random.NextInt32();
                                            var tempLocal = new Local(Method.Module.CorLibTypes.Int32);
                                            var tempLocal2 = new Local(Method.Module.CorLibTypes.Int32);
                                            body.Variables.Add(tempLocal);
                                            newStatement.Add(Instruction.Create(OpCodes.Ldloc, fakeLocal));
                                            newStatement.Add(Instruction.Create(OpCodes.Ldc_I4, r));
                                            newStatement.Add(Instruction.Create(OpCodes.Add));
                                            newStatement.Add(Instruction.Create(OpCodes.Stloc, tempLocal));
                                            newStatement.Add(Instruction.Create(OpCodes.Ldloc, tempLocal));
                                            newStatement.Add(Instruction.Create(OpCodes.Ldc_I4, (thisKey + r) - targetKey));
                                            newStatement.Add(Instruction.Create(OpCodes.Sub));
                                            break;
                                        }
                                    case 1: // mutation 2 (replace here)
                                        {
                                            var thisKey = key[i];
                                            var tarray = GenerateArray();
                                            var r = tarray[tarray.Length - 1];
                                            var tempLocal = new Local(Method.Module.CorLibTypes.Int32);
                                            var tempLocal2 = new Local(Method.Module.CorLibTypes.Int32);
                                            body.Variables.Add(tempLocal);
                                            body.Variables.Add(tempLocal2);
                                            newStatement.Add(Instruction.Create(OpCodes.Ldloc, fakeLocal));
                                            newStatement.Add(Instruction.Create(OpCodes.Stloc, tempLocal2));
                                            InjectArray(Method, tarray, ref newStatement, arraylocal);
                                            newStatement.Add(Instruction.Create(OpCodes.Ldloc, tempLocal2));
                                            newStatement.Add(OpCodes.Ldloc_S.ToInstruction(arraylocal));
                                            newStatement.Add(OpCodes.Ldc_I4.ToInstruction(tarray.Length - 1));
                                            newStatement.Add(OpCodes.Ldelem_I4.ToInstruction());
                                            newStatement.Add(Instruction.Create(OpCodes.Add));
                                            newStatement.Add(Instruction.Create(OpCodes.Stloc, tempLocal));
                                            newStatement.Add(Instruction.Create(OpCodes.Ldloc, tempLocal));
                                            newStatement.Add(Instruction.Create(OpCodes.Ldc_I4, (thisKey + r) - targetKey));
                                            newStatement.Add(Instruction.Create(OpCodes.Sub));
                                            break;
                                        }
                                    default:
                                        newStatement.Add(Instruction.Create(OpCodes.Ldc_I4, targetKey));
                                        break;
                                }
                                Shuffler.Instructions.Shuffler.Execute(newStatement);
                                Shuffler.Instructions.Shuffler.Execute(switchHdr);
                            }
                            else // "NEVER" ENCODE HERE!
                            {
                                newStatement.Add(Instruction.Create(OpCodes.Ldc_I4, targetKey));
                            }

                            // "NEVER" ENCODE HERE!

                            newStatement.Add(Instruction.Create(OpCodes.Br, jumpInstr)); // Jump to Switch header.

                            operands[keyId[i]] = newStatement[0];
                        }
                    }
                    else
                        operands[keyId[i]] = switchHdr[0];

                    current.Value = newStatement.ToArray();
                    current = current.Next;
                    i++;
                }
                operands[keyId[i]] = current.Value[0];
                switchInstr.Operand = operands;

                Instruction[] first = statements.First.Value;
                statements.RemoveFirst();
                Instruction[] last = statements.Last.Value;
                statements.RemoveLast();

                List<Instruction[]> newStatements = statements.ToList();
                Shuffle(newStatements);

                block.Instructions.Clear();
                block.Instructions.AddRange(first);
                block.Instructions.AddRange(switchHdr);
                foreach (var statement in newStatements)
                    block.Instructions.AddRange(statement);
                block.Instructions.AddRange(last);
            }
        }
        static Random rnd = new Random();
        static int[] GenerateArray()
        {
            var array = new int[rnd.Next(3, 6)];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = rnd.Next(100, 500);
            }
            return array;
        }

        static void InjectArray(MethodDef method, int[] array, ref List<Instruction> toInject, Local local)
        {
            List<Instruction> lista = new List<Instruction>
                {
                    OpCodes.Ldc_I4.ToInstruction(array.Length),
                    OpCodes.Newarr.ToInstruction(method.Module.CorLibTypes.Int32),
                    OpCodes.Stloc_S.ToInstruction(local)
                };
            for (int i = 0; i < array.Length; i++)
            {
                if (i == 0)
                {
                    lista.Add(OpCodes.Ldloc_S.ToInstruction(local));
                    lista.Add(OpCodes.Ldc_I4.ToInstruction(i));
                    lista.Add(OpCodes.Ldc_I4.ToInstruction(array[i]));
                    lista.Add(OpCodes.Stelem_I4.ToInstruction());
                    lista.Add(OpCodes.Nop.ToInstruction());
                    continue;
                }
                int currentValue = array[i];
                lista.Add(OpCodes.Ldloc_S.ToInstruction(local));
                lista.Add(OpCodes.Ldc_I4.ToInstruction(i));
                lista.Add(OpCodes.Ldc_I4.ToInstruction(currentValue));
                int index = lista.Count - 1;
                for (int j = i - 1; j >= 0; j--)
                {
                    OpCode opcode = null;
                    switch (rnd.Next(0, 2))
                    {
                        case 0:
                            currentValue += array[j];
                            opcode = OpCodes.Sub;
                            break;
                        case 1:
                            currentValue -= array[j];
                            opcode = OpCodes.Add;
                            break;
                    }
                    lista.Add(OpCodes.Ldloc_S.ToInstruction(local));
                    lista.Add(OpCodes.Ldc_I4.ToInstruction(j));
                    lista.Add(OpCodes.Ldelem_I4.ToInstruction());
                    lista.Add(opcode.ToInstruction());
                }
                lista[index].OpCode = OpCodes.Ldc_I4;
                lista[index].Operand = currentValue;
                lista.Add(OpCodes.Stelem_I4.ToInstruction());
                lista.Add(OpCodes.Nop.ToInstruction());
            }
            for (int j = 0; j < lista.Count; j++)
                toInject.Add(lista[j]);
        }
        LinkedList<Instruction[]> SplitStatements(InstrBlock block, Trace trace)
        {
            var statements = new LinkedList<Instruction[]>();
            var currentStatement = new List<Instruction>();

            // Instructions that must be included in the ccurrent statement to ensure all outgoing
            // branches have stack = 0
            var requiredInstr = new HashSet<Instruction>();

            for (var i = 0; i < block.Instructions.Count; i++)
            {
                var instr = block.Instructions[i];
                currentStatement.Add(instr);

                var shouldSplit = i + 1 < block.Instructions.Count && trace.HasMultipleSources(block.Instructions[i + 1].Offset);
                switch (instr.OpCode.FlowControl)
                {
                    case FlowControl.Branch:
                    case FlowControl.Cond_Branch:
                    case FlowControl.Return:
                    case FlowControl.Throw:
                        shouldSplit = true;
                        if (trace.AfterStack[instr.Offset] != 0)
                        {
                            if (instr.Operand is Instruction targetInstr)
                                requiredInstr.Add(targetInstr);
                            else if (instr.Operand is Instruction[] targetInstrs)
                            {
                                foreach (var target in targetInstrs)
                                    requiredInstr.Add(target);
                            }
                        }
                        break;
                }

                requiredInstr.Remove(instr);

                if (instr.OpCode.OpCodeType != OpCodeType.Prefix && trace.AfterStack[instr.Offset] == 0 && requiredInstr.Count == 0/* && (shouldSplit || 90 > new Random().NextDouble()) && (i == 0 || block.Instructions[i - 1].OpCode.Code != Code.Tailcall)*/)
                {
                    statements.AddLast(currentStatement.ToArray());
                    currentStatement.Clear();
                }
            }

            if (currentStatement.Count > 0)
                statements.AddLast(currentStatement.ToArray());

            return statements;
        }
        public void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 1; i--)
            {
                int k = new Random().Next(i + 1);
                T tmp = list[k];
                list[k] = list[i];
                list[i] = tmp;
            }
        }
    }
}
