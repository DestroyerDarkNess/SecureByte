using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;

namespace Protections.Strings
{
    public class replaceObfuscator
    {
        private ModuleDefMD _module;
        private readonly Mode _mode;
        private readonly Random _random;
        public enum Mode
        {
            Simple,
            Homoglyph
        }
        public replaceObfuscator(ModuleDefMD module, Mode mode = Mode.Homoglyph)
        {
            _module = module;
            _mode = mode;
            _random = new Random(Guid.NewGuid().GetHashCode());
        }
        public void Execute()
        {
            var importer = new Importer(_module);
            foreach (var type in _module.GetTypes().Where(t => t.Methods.Count != 0))
            {
                if (type.IsGlobalModuleType)
                    continue;
                if (type.Namespace == "Costura")
                    continue;
                foreach (var method in type.Methods)
                {
                    if (method.Body == null)
                        continue;
                    if (!method.HasBody || !method.Body.HasInstructions)
                        continue;
                    method.Body.SimplifyMacros(method.Parameters);
                    method.Body.SimplifyBranches();
                    var instructions = method.Body.Instructions;
                    for (int i = 0; i < instructions.Count; i++)
                    {
                        if (instructions[i].OpCode != OpCodes.Ldstr)
                            continue;
                        if ((string)instructions[i].Operand == string.Empty)
                            continue;
                        instructions[i].Operand = ObfuscateString((string)instructions[i].Operand);
                        var implant = new List<Instruction>();
                        var replaceMethod = importer.Import(typeof(string).GetMethod("Replace", new[] { typeof(string), typeof(string) }) ?? throw new InvalidDataException());
                        if (_mode == Mode.Homoglyph)
                        {
                            string[] glyphs = { "а", "е", "і", "о", "с" };
                            string[] ordered = glyphs.OrderBy(c => _random.Next()).ToArray();
                            for (int j = 0; j < ordered.Length; j++)
                            {
                                implant.Add(new Instruction(OpCodes.Ldstr, ordered[j]));
                                implant.Add(new Instruction(OpCodes.Ldstr, ""));
                                if (j == 0)
                                {
                                    implant.Add(new Instruction(OpCodes.Call, replaceMethod));
                                    continue;
                                }
                                implant.Add(new Instruction(OpCodes.Callvirt, replaceMethod));
                            }
                        }
                        else
                        {
                            implant.Add(new Instruction(OpCodes.Ldstr, "\u2029"));
                            implant.Add(new Instruction(OpCodes.Ldstr, ""));
                            implant.Add(new Instruction(OpCodes.Call, replaceMethod));
                        }
                        foreach (Instruction instr in implant)
                        {
                            instructions.Insert(i + 1, instr);
                            i++;
                        }
                    }
                    method.Body.OptimizeMacros();
                }
            }
        }
        public void ExecuteFor(MethodDef m)
        {
            var importer = new Importer(_module);
            foreach (var type in _module.GetTypes().Where(t => t.Methods.Count != 0))
            {
                if (type.IsGlobalModuleType)
                    continue;
                if (type.Namespace == "Costura")
                    continue;
                foreach (var method in type.Methods)
                {
                    if (method == m)
                    {
                        if (method.Body == null)
                            continue;
                        if (!method.HasBody || !method.Body.HasInstructions)
                            continue;
                        method.Body.SimplifyMacros(method.Parameters);
                        method.Body.SimplifyBranches();
                        var instructions = method.Body.Instructions;
                        for (int i = 0; i < instructions.Count; i++)
                        {
                            if (instructions[i].OpCode != OpCodes.Ldstr)
                                continue;
                            if ((string)instructions[i].Operand == string.Empty)
                                continue;
                            instructions[i].Operand = ObfuscateString((string)instructions[i].Operand);
                            var implant = new List<Instruction>();
                            var replaceMethod = importer.Import(typeof(string).GetMethod("Replace", new[] { typeof(string), typeof(string) }) ?? throw new InvalidDataException());
                            if (_mode == Mode.Homoglyph)
                            {
                                string[] glyphs = { "а", "е", "і", "о", "с" };
                                string[] ordered = glyphs.OrderBy(c => _random.Next()).ToArray();
                                for (int j = 0; j < ordered.Length; j++)
                                {
                                    implant.Add(new Instruction(OpCodes.Ldstr, ordered[j]));
                                    implant.Add(new Instruction(OpCodes.Ldstr, ""));
                                    if (j == 0)
                                    {
                                        implant.Add(new Instruction(OpCodes.Call, replaceMethod));
                                        continue;
                                    }
                                    implant.Add(new Instruction(OpCodes.Callvirt, replaceMethod));
                                }
                            }
                            else
                            {
                                implant.Add(new Instruction(OpCodes.Ldstr, "\u2029"));
                                implant.Add(new Instruction(OpCodes.Ldstr, ""));
                                implant.Add(new Instruction(OpCodes.Call, replaceMethod));
                            }
                            foreach (Instruction instr in implant)
                            {
                                instructions.Insert(i + 1, instr);
                                i++;
                            }
                        }
                        method.Body.OptimizeMacros();
                    }
                }
            }
        }
        private string ObfuscateString(string input)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in input)
            {
                if (_random.Next(0, 1) == 0)
                {
                    result.Append(_mode == Mode.Homoglyph
                        ? new string(GetHomoglyph(c), 1)
                        : new string('\u2029', 1));
                    result.Append(c);
                }
                else
                {
                    result.Append(c);
                    result.Append(_mode == Mode.Homoglyph
                        ? new string(GetHomoglyph(c), 1)
                        : new string('\u2029', 1));
                }
            }
            return result.ToString();
        }
        private char GetHomoglyph(char input)
        {
            char[] glyphs = { 'а', 'е', 'і', 'о', 'с' };
            switch (input)
            {
                case 'a':
                    return glyphs[0];
                case 'e':
                    return glyphs[1];
                case 'i':
                    return glyphs[2];
                case 'o':
                    return glyphs[3];
                //case 'c':
                //    return glyphs[4];
                default:
                    return glyphs[_random.Next(glyphs.Length)];
            }
        }
    }
}
