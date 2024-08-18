using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;

namespace ICore
{
    public static class Utils
    {
        public static Random rnd = new Random();
        public static byte[] RandomByteArr(int size)
        {
            var result = new byte[size];
            rnd.NextBytes(result);
            return result;
        }
        public static Code GetCode(bool supported = false)
        {
            var codes = new Code[] { Code.Add, Code.And, Code.Xor, Code.Sub, Code.Or };
            if (supported)
                codes = new Code[] { Code.Add, Code.Sub, Code.Xor };
            return codes[rnd.Next(0, codes.Length)];
        }
        public static FieldDefUser CreateField(FieldSig sig)
        {
            return new FieldDefUser(GenerateString(), sig, FieldAttributes.Public | FieldAttributes.Static);
        }
        public static MethodDefUser CreateMethod(ModuleDefMD mod)
        {
            var method = new MethodDefUser(GenerateString(), MethodSig.CreateStatic(mod.CorLibTypes.Void),
                MethodImplAttributes.IL | MethodImplAttributes.Managed,
                MethodAttributes.Public | MethodAttributes.Static)
            {
                Body = new CilBody()
            };
            method.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
            mod.GlobalType.Methods.Add(method);
            return method;
        }
        public static string Xoring(string inputString)
        {
            char xorKey = 'ع';
            string outputString = "";
            int len = inputString.Length;
            for (int i = 0; i < len; i++)
            {
                outputString += char.ToString((char)(inputString[i] ^ xorKey));
            }
            return outputString;
        }
        public static void EnsureNoInlining(MethodDef method)
        {
            method.ImplAttributes &= ~MethodImplAttributes.AggressiveInlining;
            method.ImplAttributes |= MethodImplAttributes.NoInlining;
        }
        public static string GenerateString()
        {
            return Xoring(Safe.GenerateRandomString());
        }
        public static int RandomTinyInt32() => rnd.Next(2, 25);
        public static int RandomSmallInt32() => rnd.Next(15, 40);
        public static int RandomInt32() => rnd.Next(100, 300);
        public static int RandomBigInt32() => int.MaxValue;
        public static bool RandomBoolean() => Convert.ToBoolean(rnd.Next(0, 2));
    }
}