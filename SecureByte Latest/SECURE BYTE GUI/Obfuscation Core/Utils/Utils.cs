using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Protections.Renamer;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ICore
{
    public static class Utils
    {
        public static System.Random rnd = new System.Random();
        public static int Complexity = 100;
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
            return new FieldDefUser(MethodsRenamig(), sig, FieldAttributes.FamANDAssem | FieldAttributes.Family | FieldAttributes.Static);
        }
        public static MethodDefUser CreateMethod(ModuleDef mod)
        {
            var method = new MethodDefUser(MethodsRenamig(), MethodSig.CreateStatic(mod.CorLibTypes.Void),
                MethodImplAttributes.IL | MethodImplAttributes.Managed,
                MethodAttributes.Public | MethodAttributes.Static)
            {
                Body = new CilBody()
            };
            method.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
            mod.GlobalType.Methods.Add(method);
            return method;
        }
        public static MethodDefUser CreateMethod(ModuleDef mod, int num, string content)
        {
            MethodDefUser mdefuser = null;
            for (int i = 0; i < num; i++)
            {
                mdefuser = new MethodDefUser(MethodsRenamig(), MethodSig.CreateStatic(mod.CorLibTypes.Void),
                MethodImplAttributes.IL | MethodImplAttributes.Managed,
                    MethodAttributes.Public | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig)
                {
                    Body = new CilBody()
                };
                mdefuser.Body.Instructions.Add(OpCodes.Ldstr.ToInstruction(content));
                mdefuser.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
                mod.GlobalType.Methods.Add(mdefuser);
            }
            return mdefuser;
        }
        public static int GetBasicRandomInt32()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] randomNumber = new byte[4];
            rng.GetBytes(randomNumber);
            int value = BitConverter.ToInt32(randomNumber, 0);
            if (value < 0)
            {
                value *= -1;
            }
            return value;
        }
        public static int GetRandomInt32(int min, int max)
        {
            return GetRandomInt32() % (max - min + 1) + min;
        }
        public static int GetRandomInt32()
        {
            List<int[]> arrays = new List<int[]>();
            for (int i = 0; i < Complexity; i++)
            {
                int[] values = new int[Complexity];

                for (int j = 0; j < Complexity; j++)
                {
                    values[j] = GetBasicRandomInt32();
                }
                arrays.Add(values);
            }
            return arrays[GetBasicRandomInt32() % Complexity][GetBasicRandomInt32() % Complexity];
        }     
        public static int RandomTinyInt32() => rnd.Next(2, 25);
        public static int RandomSmallInt32() => rnd.Next(15, 40);
        public static int RandomInt32() => rnd.Next(100, 300);
        public static int RandomInt322() => rnd.Next(10000, 100000);
        public static int RandomBigInt32() => rnd.Next();
        public static bool RandomBoolean() => Convert.ToBoolean(rnd.Next(0, 2));
        public static string Generation(Schemes schemes)
        {
            if (Protections.Renamer.GlobalName.custom)
                Protections.Renamer.GlobalName.scheme = "Custom";
            else
                Protections.Renamer.GlobalName.scheme = "Safe";
            string str = null;
            str = RNG.Generate(str, schemes);
            return str;
        }
        public static void MethodRenamig(MethodDef m)
        {
            switch (GlobalName.scheme)
            {
                case "Custom":
                    m.Name = Utils.Generation(Schemes.Custom);
                    break;
                case "Safe":
                    m.Name = Utils.Generation(Schemes.Safe);
                    break;
            }
        }
        public static void MethodRenamig(MethodDef[] methods)
        {
            foreach (var m in methods)
            {
                switch (GlobalName.scheme)
                {
                    case "Custom":
                        m.Name = Utils.Generation(Schemes.Custom);
                        break;
                    case "Safe":
                        m.Name = Utils.Generation(Schemes.Safe);
                        break;
                }
            }
        }
        public static void MethodsRenamig(IDnlibDef mem)
        {
            switch (GlobalName.scheme)
            {
                case "Custom":
                    mem.Name = Utils.Generation(Schemes.Custom);
                    break;
                case "Safe":
                    mem.Name = Utils.Generation(Schemes.Safe);
                    break;              
            }
        }
        public static string MethodsRenamig()
        {
            switch (GlobalName.scheme)
            {
                case "Custom":
                    return Generation(Schemes.Custom);
                case "Safe":
                    return Generation(Schemes.Safe);              
            }
            return string.Empty;
        }
        public static string randomPharaoh()
        {
            return Safe.GenerateRandomTibetan(rnd.Next(2, 24));
        }
    }
}
