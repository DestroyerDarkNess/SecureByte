using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Helpers.Mutations
{
    public static class MutationHelper
    {
        const string mutationType = "MutationClass";
        static readonly Dictionary<string, int> field2index = new Dictionary<string, int> {
            { "KeyI0", 0 },
            { "KeyI1", 1 },
            { "KeyI2", 2 },
            { "KeyI3", 3 },
            { "KeyI4", 4 },
            { "KeyI5", 5 },
            { "KeyI6", 6 },
            { "KeyI7", 7 },
            { "KeyI8", 8 },
            { "KeyI9", 9 },
            { "KeyI10", 10 },
            { "KeyI11", 11 },
            { "KeyI12", 12 },
            { "KeyI13", 13 },
            { "KeyI14", 14 },
            { "KeyI15", 15 },
            { "KeyI16", 16}
        };
        public static void InjectKey(MethodDef method, int keyId, int key)
        {
            foreach (Instruction instr in method.Body.Instructions)
            {
                if (instr.OpCode == OpCodes.Ldsfld)
                {
                    var field = (IField)instr.Operand;
                    int _keyId;
                    if (field.DeclaringType.FullName == mutationType &&
                        field2index.TryGetValue(field.Name, out _keyId) &&
                        _keyId == keyId)
                    {
                        instr.OpCode = OpCodes.Ldc_I4;
                        instr.Operand = key;
                    }
                }
            }
        }
        public static void InjectKeys(MethodDef method, int[] keyIds, int[] keys)
        {
            foreach (Instruction instr in method.Body.Instructions)
            {
                if (instr.OpCode == OpCodes.Ldsfld)
                {
                    var field = (IField)instr.Operand;
                    int _keyIndex;
                    if (field.DeclaringType.FullName == mutationType &&
                        field2index.TryGetValue(field.Name, out _keyIndex) &&
                        (_keyIndex = Array.IndexOf(keyIds, _keyIndex)) != -1)
                    {
                        instr.OpCode = OpCodes.Ldc_I4;
                        instr.Operand = keys[_keyIndex];
                    }
                }
            }
        }
        const string Typer = "MutationClass";
        static readonly Dictionary<string, string> field3index = new Dictionary<string, string> {
            { "Str1",  "a"},
            { "Str2",  "b"},
            { "Str3",  "c"},
            { "Str4",  "d"},
            { "Str5",  "e"},
            { "Str6",  "f"},
            { "Str7",  "g"},
            { "Str8",  "h"},
            { "Str9",  "i"},
            { "Str10",  "j"},
            { "Str11",  "k"},
            { "Str12",  "l"},
            { "Str13",  "m"},
            { "Str14",  "n"},
            { "Str15",  "o"},
            { "Str16",  "p"}
        };
        public static void InjectString(MethodDef method, string keyId, string key)
        {
            foreach (Instruction instr in method.Body.Instructions)
            {
                if (instr.OpCode == OpCodes.Ldsfld)
                {
                    var field = (IField)instr.Operand;
                    string _keyId;
                    if (field.DeclaringType.FullName == Typer &&
                        field3index.TryGetValue(field.Name, out _keyId) &&
                        _keyId == keyId)
                    {
                        instr.OpCode = OpCodes.Ldstr;
                        instr.Operand = key;
                    }
                }
            }
        }
    }
}