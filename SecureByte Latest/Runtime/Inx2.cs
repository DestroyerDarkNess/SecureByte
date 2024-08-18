using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;

namespace ConversionBack
{
    [SecuritySafeCritical]
    public class Dyn2
    {
        [System.Reflection.ObfuscationAttribute(Feature = "Virtualization", Exclude = false)]
        public static object ___(MethodBase callingMethod, object[] parameters, int ID, byte[] bytes)
        {
            MethodBody methodBody = callingMethod.GetMethodBody();//get calling methods body 
            BinaryReader binaryReader = new BinaryReader(new MemoryStream(new Cryptographer("أً").Decrypt(bytes)));//cast byte[] Position
            var methodParameters = callingMethod.GetParameters();//get its parameters
            var allLocals = new List<LocalBuilder>();
            var _allExceptionHandlerses = new List<ExceptionHandlerClass>();
            Type[] parametersArray;
            int? nID2 = 0;
            int i0 = (int)nID2;
            int start = i0;
            if (callingMethod.IsStatic)//check if the method is static or not
                parametersArray = new Type[methodParameters.Length];//if method is static set the parameters to the amount in calling method
            else
            {
                parametersArray = new Type[methodParameters.Length + 1];//if its not static this means there is an additional hidden parameter (this.) this is always used as the first parameter so we need to account for this
                parametersArray[0] = callingMethod.DeclaringType;
                start = 1;
            }
            for (var i = i0; i < methodParameters.Length; i++)
            {
                var parameterInfo = methodParameters[i];
                parametersArray[start + i] = parameterInfo.ParameterType;//set parameter types
            }
            DynamicMethod dynamicMethod = new DynamicMethod(Xor.Xoring("أً"), callingMethod.MemberType == MemberTypes.Constructor ? null : ((MethodInfo)callingMethod).ReturnParameter.ParameterType, parametersArray, Class.callingModule, true);//create the dynamic method
            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();//get ilgenerator
            var locs = methodBody.LocalVariables;
            var locals = new Type[locs.Count];
            foreach (var localVariableInfo in locs)
                allLocals.Add(ilGenerator.DeclareLocal(localVariableInfo.LocalType));//declare the local for use of stloc,ldloc/ldloca
            var exceptionHandlersCount = binaryReader.ReadInt32();//read amount of exception handlers
           VM.processExceptionHandler(binaryReader, exceptionHandlersCount, callingMethod, _allExceptionHandlerses);//convert exception handlers
            var sortedExceptionHandlers = VM.fixAndSortExceptionHandlers(_allExceptionHandlerses);//we need to sort the exception handlers incase there is multiple handlers that start at the same instruction
            var instructionCount = binaryReader.ReadInt32();//read the amount of instructions
            var _allLabelsDictionary = new Dictionary<int, Label>();
            for (var u = 0; u < instructionCount; u++)
            {
                var label = ilGenerator.DefineLabel();//we need to label each instruction to use with branches
                _allLabelsDictionary.Add(u, label);
            }
            for (var i = 0; i < instructionCount; i++)
            {
                VM.checkAndSetExceptionHandler(sortedExceptionHandlers, i, ilGenerator);//we check the instruction against our exception handlers to determine if we need to start/end any handlers
                var opcode = binaryReader.ReadInt16();//read opcode short this will relate to the correct opcode
                OpCode opc;
                if (opcode >= 0 && opcode < Class.oneByteOpCodes.Length)
                    opc = Class.oneByteOpCodes[opcode];//we check against one byte opcodes
                else
                {
                    var b2 = (byte)(opcode | 0xFE00);
                    opc = Class.twoByteOpCodes[b2];//check against two byte opcodes
                }
                ilGenerator.MarkLabel(_allLabelsDictionary[i]);//we now need to mark the label in the ilgenerator
                var operandType = binaryReader.ReadByte();//we get the operand type
                VM.HandleOpType(operandType, opc, ilGenerator, binaryReader, _allLabelsDictionary, allLocals);//we process the instruction with ilgenerator
            }
            lock (VM.locker)//we lock threads here to prevent exceptions of item already exists
            {
                if (!VM.cache.ContainsKey(ID))
                {
                    VM.cache.Add(ID, dynamicMethod);//add to cache if first time creating method
                }
            }
            return dynamicMethod.Invoke(null, parameters);//invoke the dynamic method which is the users original method and return the result
        }
    }
}
