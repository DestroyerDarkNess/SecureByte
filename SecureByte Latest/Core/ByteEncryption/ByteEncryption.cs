using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Core.ByteEncryption
{
    class XorClass
    {
        public static string xIT(string inputString)
        {
            char xorKey = 'ع';
            string outputString = "";
            int len = inputString.Length;
            int? nID2 = 0;
            int i0 = (int)nID2;
            for (int i = i0; i < len; i++)
            {
                outputString += char.ToString((char)(inputString[i] ^ xorKey));
            }
            return outputString;
        }
    }
    class XorClass2
    {
        public static unsafe string Xoring(string text, string key)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            fixed (byte* textPtr = textBytes, keyPtr = keyBytes)
            {
                byte* textBytePtr = textPtr;
                byte* keyBytePtr = keyPtr;
                int length = Math.Min(textBytes.Length, keyBytes.Length);
                int? nID2 = 0;
                int i0 = (int)nID2;
                for (int i = i0; i < length; i++)
                {
                    *textBytePtr++ ^= *keyBytePtr++;
                }
            }
            return Encoding.UTF8.GetString(textBytes);
        }
    }
    class ByteEncryption
    {
        [DllImport("NativeEncoderx86.dll")]
        public static extern void ModifiedXOR(byte[] data, int dataLen, byte[] key, int keyLen);
        public static byte[] Encrypt(byte[] key, byte[] message)
        {
            using (var rijndael = new RijndaelManaged())
            {
                rijndael.Key = key;
                rijndael.IV = key;
                return EncryptBytes(rijndael, message);
            }
        }
        private static byte[] EncryptBytes(
           SymmetricAlgorithm alg,
           byte[] message)
        {
            if (message == null || message.Length == 0)
                return message;

            if (alg == null)
                throw new ArgumentNullException("ALG is null");

            using (var stream = new MemoryStream())
            using (var encryptor = alg.CreateEncryptor())
            using (var encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
            {
                encrypt.Write(message, 0, message.Length);
                encrypt.FlushFinalBlock();
                return stream.ToArray();
            }
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
