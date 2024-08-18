using Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;

namespace ConversionBack
{
    public class VM : Dyn
    {
        public static DynamicMethod value;
        public static object locker = new object();
        public static Dictionary<int, DynamicMethod> cache = new Dictionary<int, DynamicMethod>();
        public static EBytes eBytes = new EBytes("Class");
        [System.Reflection.ObfuscationAttribute(Feature = "Virtualization", Exclude = false)]
        public static void Run(string str)
        {
            if (Assembly.GetExecutingAssembly() == Assembly.GetCallingAssembly())
            {
                Run(str);
            }
        }
        public static void HandleOpType(int opType, OpCode opcode, ILGenerator ilGenerator, BinaryReader binaryReader, Dictionary<int, Label> _allLabelsDictionary, List<LocalBuilder> allLocals)
        {
            switch (opType)//we switch on operand type
            {
                case 0:
                    InlineNoneEmitter(ilGenerator, opcode, binaryReader);
                    break;
                case 1:
                    InlineMethodEmitter(ilGenerator, opcode, binaryReader);
                    break;
                case 2:
                    InlineStringEmitter(ilGenerator, opcode, binaryReader);
                    break;
                case 3:
                    InlineIEmitter(ilGenerator, opcode, binaryReader);
                    break;

                case 5:
                    InlineFieldEmitter(ilGenerator, opcode, binaryReader);
                    break;
                case 6:
                    InlineTypeEmitter(ilGenerator, opcode, binaryReader);
                    break;
                case 7:
                    ShortInlineBrTargetEmitter(ilGenerator, opcode, binaryReader, _allLabelsDictionary);
                    break;
                case 8:
                    ShortInlineIEmitter(ilGenerator, opcode, binaryReader);
                    break;
                case 9:
                    InlineSwitchEmitter(ilGenerator, opcode, binaryReader, _allLabelsDictionary);
                    break;
                case 10:
                    InlineBrTargetEmitter(ilGenerator, opcode, binaryReader, _allLabelsDictionary);
                    break;
                case 11:
                    InlineTokEmitter(ilGenerator, opcode, binaryReader);
                    break;
                case 12:
                case 4:
                    InlineVarEmitter(ilGenerator, opcode, binaryReader, allLocals);
                    break;
                case 13:
                    ShortInlineREmitter(ilGenerator, opcode, binaryReader);
                    break;
                case 14:
                    InlineREmitter(ilGenerator, opcode, binaryReader);
                    break;
                case 15:
                    InlineI8Emitter(ilGenerator, opcode, binaryReader);
                    break;
                default:
                    throw new Exception("Operand Type Unknown " + opType);
            }
        }
        /// <summary>
        /// this operand type does nothing it is for opcodes that have no operands
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="opcode"></param>
        /// <param name="binaryReader"></param>
        private static void InlineNoneEmitter(ILGenerator ilGenerator, OpCode opcode, BinaryReader binaryReader)
        {
            ilGenerator.Emit(opcode);
        }

        /// <summary>
        /// this is for calling of methods where it will resolve the metadata token that relates to the method
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="opcode"></param>
        /// <param name="binaryReader"></param>
        private static void InlineMethodEmitter(ILGenerator ilGenerator, OpCode opcode, BinaryReader binaryReader)
        {
            var mdtoken = binaryReader.ReadInt32();
            var resolvedMethodBase = Class.callingModule.ResolveMethod(mdtoken);
            if (resolvedMethodBase is MethodInfo)
                ilGenerator.Emit(opcode, (MethodInfo)resolvedMethodBase);
            else if (resolvedMethodBase is ConstructorInfo)
                ilGenerator.Emit(opcode, (ConstructorInfo)resolvedMethodBase);
            else
                throw new Exception("Check resolvedMethodBase Type");
        }
        /// <summary>
        /// This is for operands that handle variables and parameters we need to emit the label that it relates to which we defined earlier
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="opcode"></param>
        /// <param name="binaryReader"></param>
        /// <param name="allLocals"></param>
        private static void InlineVarEmitter(ILGenerator ilGenerator, OpCode opcode, BinaryReader binaryReader, List<LocalBuilder> allLocals)
        {
            var index = binaryReader.ReadInt32();
            var parOrloc = binaryReader.ReadByte();
            if (parOrloc == 0)
            {
                var label = allLocals[index];
                ilGenerator.Emit(opcode, label);
            }
            else
            {
                ilGenerator.Emit(opcode, index);
            }

        }

        /// <summary>
        /// read the string from the byte[] and emit the opcode with this string
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="opcode"></param>
        /// <param name="binaryReader"></param>
        private static void InlineStringEmitter(ILGenerator ilGenerator, OpCode opcode, BinaryReader binaryReader)
        {
            var readString = binaryReader.ReadString();
            ilGenerator.Emit(opcode, readString);
        }
        private static void InlineIEmitter(ILGenerator ilGenerator, OpCode opcode, BinaryReader binaryReader)
        {
            var readInt32 = binaryReader.ReadInt32();

            ilGenerator.Emit(opcode, readInt32);
        }

        private static void InlineFieldEmitter(ILGenerator ilGenerator, OpCode opcode, BinaryReader binaryReader)
        {
            int mdtoken = binaryReader.ReadInt32();
            FieldInfo fieldInfo = Class.callingModule.ResolveField(mdtoken);
            ilGenerator.Emit(opcode, fieldInfo);
        }

        private static void InlineTypeEmitter(ILGenerator ilGenerator, OpCode opcode, BinaryReader binaryReader)
        {
            int mdtoken = binaryReader.ReadInt32();
            Type type = Class.callingModule.ResolveType(mdtoken);
            ilGenerator.Emit(opcode, type);
        }

        private static void ShortInlineBrTargetEmitter(ILGenerator ilGenerator, OpCode opcode, BinaryReader binaryReader, Dictionary<int, Label> _allLabelsDictionary)
        {
            int index = binaryReader.ReadInt32();
            var location = _allLabelsDictionary[index];
            ilGenerator.Emit(opcode, location);
        }

        private static void ShortInlineIEmitter(ILGenerator ilGenerator, OpCode opCode, BinaryReader binaryReader)
        {
            byte b = binaryReader.ReadByte();
            ilGenerator.Emit(opCode, b);
        }
        private static void ShortInlineREmitter(ILGenerator ilGenerator, OpCode opCode, BinaryReader binaryReader)
        {
            var value = binaryReader.ReadBytes(4);
            var myFloat = BitConverter.ToSingle(value, 0);
            ilGenerator.Emit(opCode, myFloat);
        }
        private static void InlineREmitter(ILGenerator ilGenerator, OpCode opCode, BinaryReader binaryReader)
        {
            var value = binaryReader.ReadDouble();

            ilGenerator.Emit(opCode, value);
        }
        private static void InlineI8Emitter(ILGenerator ilGenerator, OpCode opCode, BinaryReader binaryReader)
        {
            var value = binaryReader.ReadInt64();

            ilGenerator.Emit(opCode, value);
        }

        private static void InlineSwitchEmitter(ILGenerator ilGenerator, OpCode opCode, BinaryReader binaryReader, Dictionary<int, Label> _allLabelsDictionary)
        {
            int count = binaryReader.ReadInt32();
            Label[] allLabels = new Label[count];
            for (int i = 0; i < count; i++)
            {
                allLabels[i] = _allLabelsDictionary[binaryReader.ReadInt32()];

            }
            ilGenerator.Emit(opCode, allLabels);
        }
        private static void InlineBrTargetEmitter(ILGenerator ilGenerator, OpCode opcode, BinaryReader binaryReader, Dictionary<int, Label> _allLabelsDictionary)
        {
            int index = binaryReader.ReadInt32();
            var location = _allLabelsDictionary[index];
            ilGenerator.Emit(opcode, location);
        }

        private static void InlineTokEmitter(ILGenerator ilGenerator, OpCode opcode, BinaryReader binaryReader)
        {
            int mdtoken = binaryReader.ReadInt32();
            byte type = binaryReader.ReadByte();
            if (type == 0)
            {
                var fieldinfo = Class.callingModule.ResolveField(mdtoken);
                ilGenerator.Emit(opcode, fieldinfo);
            }
            else if (type == 1)
            {
                var typeInfo = Class.callingModule.ResolveType(mdtoken);
                ilGenerator.Emit(opcode, typeInfo);
            }
            else if (type == 2)
            {
                var methodinfo = Class.callingModule.ResolveMethod(mdtoken);
                if (methodinfo is MethodInfo)
                    ilGenerator.Emit(opcode, (MethodInfo)methodinfo);
                else if (methodinfo is ConstructorInfo)
                    ilGenerator.Emit(opcode, (ConstructorInfo)methodinfo);
            }
        }

        public static void checkAndSetExceptionHandler(List<FixedExceptionHandlersClass> sorted, int i, ILGenerator ilGenerator)
        {
            foreach (var allExceptionHandlerse in sorted)
                if (allExceptionHandlerse.HandlerType == 1)
                {
                    if (allExceptionHandlerse.TryStart == i)
                    {
                        ilGenerator.BeginExceptionBlock();

                    }
                    if (allExceptionHandlerse.HandlerEnd == i)
                    {
                        ilGenerator.EndExceptionBlock();
                    }
                    if (allExceptionHandlerse.HandlerStart.Contains(i))
                    {
                        var indes = allExceptionHandlerse.HandlerStart.IndexOf(i);
                        ilGenerator.BeginCatchBlock(allExceptionHandlerse.CatchType[indes]);
                    }
                }
                else if (allExceptionHandlerse.HandlerType == 5)
                {
                    if (allExceptionHandlerse.TryStart == i)
                        ilGenerator.BeginExceptionBlock();
                    else if (allExceptionHandlerse.HandlerEnd == i)
                        ilGenerator.EndExceptionBlock();
                    else if (allExceptionHandlerse.TryEnd == i)
                        ilGenerator.BeginFinallyBlock();
                }
        }

        public static void processExceptionHandler(BinaryReader bin, int count, MethodBase method, List<ExceptionHandlerClass> _allExceptionHandlerses)
        {
            for (var i = 0; i < count; i++)
            {
                //catchType
                var expExceptionHandlers = new ExceptionHandlerClass();
                var catchTypeMdToken = bin.ReadInt32();
                if (catchTypeMdToken == -1)
                {
                    expExceptionHandlers.CatchType = null;
                }
                else
                {
                    var catchType = method.Module.ResolveType(catchTypeMdToken);
                    expExceptionHandlers.CatchType = catchType;
                }

                //filterStart
                var filterStartIndex = bin.ReadInt32();
                expExceptionHandlers.FilterStart = filterStartIndex;
                //handlerEnd
                var handlerEnd = bin.ReadInt32();
                expExceptionHandlers.HandlerEnd = handlerEnd;
                //handlerStart
                var handlerStart = bin.ReadInt32();
                expExceptionHandlers.HandlerStart = handlerStart;
                //handlerType
                var handlerType = bin.ReadByte();
                switch (handlerType)
                {
                    case 1:
                        expExceptionHandlers.HandlerType = 1;
                        break;
                    case 2:
                        expExceptionHandlers.HandlerType = 2;
                        break;
                    case 3:
                        expExceptionHandlers.HandlerType = 3;
                        break;
                    case 4:
                        expExceptionHandlers.HandlerType = 4;
                        break;
                    case 5:
                        expExceptionHandlers.HandlerType = 5;
                        break;
                    default:
                        throw new Exception("Out of Range");
                }
                //tryEnd 
                var tryEnd = bin.ReadInt32();
                expExceptionHandlers.TryEnd = tryEnd;
                //tryStart
                var tryStart = bin.ReadInt32();
                expExceptionHandlers.TryStart = tryStart;
                _allExceptionHandlerses.Add(expExceptionHandlers);
            }
        }

        public static List<FixedExceptionHandlersClass> fixAndSortExceptionHandlers(List<ExceptionHandlerClass> expHandlers)
        {
            var multiExp = new List<ExceptionHandlerClass>();
            var exceptionDictionary = new Dictionary<ExceptionHandlerClass, int>();
            foreach (var handler in expHandlers)
                if (handler.HandlerType == 5)
                {
                    exceptionDictionary.Add(handler, handler.TryStart);
                }
                else
                {
                    if (exceptionDictionary.ContainsValue(handler.TryStart))
                        if (handler.CatchType != null)
                            exceptionDictionary.Add(handler, handler.TryStart);
                        else
                            multiExp.Add(handler);

                    else
                        exceptionDictionary.Add(handler, handler.TryStart);
                }
            var sorted = new List<FixedExceptionHandlersClass>();
            foreach (var keyValuePair in exceptionDictionary)
            {
                if (keyValuePair.Key.HandlerType == 5)
                {
                    var fixedExceptionHandlers = new FixedExceptionHandlersClass();
                    fixedExceptionHandlers.TryStart = keyValuePair.Key.TryStart;
                    fixedExceptionHandlers.TryEnd = keyValuePair.Key.TryEnd;
                    fixedExceptionHandlers.FilterStart = keyValuePair.Key.FilterStart;
                    fixedExceptionHandlers.HandlerEnd = keyValuePair.Key.HandlerEnd;

                    fixedExceptionHandlers.HandlerType = keyValuePair.Key.HandlerType;

                    fixedExceptionHandlers.HandlerStart.Add(keyValuePair.Key.HandlerStart);
                    fixedExceptionHandlers.CatchType.Add(keyValuePair.Key.CatchType);

                    sorted.Add(fixedExceptionHandlers);
                    continue;
                }
                var rrr = WhereAlternate(multiExp, keyValuePair.Value);
                if (rrr.Count == 0)
                {
                    var fixedExceptionHandlers = new FixedExceptionHandlersClass();
                    fixedExceptionHandlers.TryStart = keyValuePair.Key.TryStart;
                    fixedExceptionHandlers.TryEnd = keyValuePair.Key.TryEnd;
                    fixedExceptionHandlers.FilterStart = keyValuePair.Key.FilterStart;
                    fixedExceptionHandlers.HandlerEnd = keyValuePair.Key.HandlerEnd;

                    fixedExceptionHandlers.HandlerType = keyValuePair.Key.HandlerType;

                    fixedExceptionHandlers.HandlerStart.Add(keyValuePair.Key.HandlerStart);
                    fixedExceptionHandlers.CatchType.Add(keyValuePair.Key.CatchType);

                    sorted.Add(fixedExceptionHandlers);
                }
                else
                {
                    var fixedExceptionHandlers = new FixedExceptionHandlersClass();
                    fixedExceptionHandlers.TryStart = keyValuePair.Key.TryStart;
                    fixedExceptionHandlers.TryEnd = keyValuePair.Key.TryEnd;
                    fixedExceptionHandlers.FilterStart = keyValuePair.Key.FilterStart;
                    fixedExceptionHandlers.HandlerEnd = rrr[rrr.Count - 1].HandlerEnd;

                    fixedExceptionHandlers.HandlerType = keyValuePair.Key.HandlerType;
                    fixedExceptionHandlers.HandlerStart.Add(keyValuePair.Key.HandlerStart);
                    fixedExceptionHandlers.CatchType.Add(keyValuePair.Key.CatchType);
                    foreach (var exceptionHandlerse in rrr)
                    {
                        fixedExceptionHandlers.HandlerStart.Add(exceptionHandlerse.HandlerStart);
                        fixedExceptionHandlers.CatchType.Add(exceptionHandlerse.CatchType);
                    }
                    sorted.Add(fixedExceptionHandlers);
                }
            }
            return sorted;
        }

        public static List<ExceptionHandlerClass> WhereAlternate(List<ExceptionHandlerClass> exp, int val)
        {
            var returnList = new List<ExceptionHandlerClass>();
            //    var rrr = MultiExp.Where(i => i.tryStart == keyValuePair.Value && i.HandlerType != 5);
            foreach (var handlers2 in exp)
                if (handlers2.TryStart == val && handlers2.HandlerType != 5)
                    returnList.Add(handlers2);
            return returnList;
        }
        public static byte[] byteArrayGrabber(byte[] bytes, int skip, int take)
        {
            byte[] newBarray = new byte[take];
            int y = 0;
            for (int i = 0; i < take; i++, y++)
            {
                byte curByte = bytes[skip + i];
                newBarray[y] = curByte;
            }

            return newBarray;

        }
        private static byte[] DecryptBytes(
           SymmetricAlgorithm alg,
           byte[] message)
        {
            if (message == null || message.Length == 0)
                return message;

            if (alg == null)
                throw new ArgumentNullException("alg is null");

            using (var stream = new MemoryStream())
            using (var decryptor = alg.CreateDecryptor())
            using (var encrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
            {
                encrypt.Write(message, 0, message.Length);
                encrypt.FlushFinalBlock();
                return stream.ToArray();
            }
        }
        public static byte[] Decrypt(byte[] key, byte[] message)
        {
            using (var rijndael = new RijndaelManaged())
            {
                rijndael.Key = key;
                rijndael.IV = key;
                return DecryptBytes(rijndael, message);
            }
        }
    }
}
