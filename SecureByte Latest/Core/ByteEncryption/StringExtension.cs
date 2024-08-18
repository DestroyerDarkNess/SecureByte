using System;

namespace Core.ByteEncryption
{
    public static class StringExtension
    {
        public static string Xor(this String input)
        {
            var result = "";
            var xorResult = xor(input);
            result += input[0];
            do
            {
                result += xorResult[0];
                xorResult = xor(xorResult);
            }
            while (xorResult.Length > 1);
            result += xorResult;
            return result;
        }
        private static string xor(string input)
        {
            var chars = input.ToCharArray();
            var result = new char[chars.Length - 1];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (char)(chars[i] ^ chars[i + 1]);
            }
            return new string(result);
        }
    }
}
