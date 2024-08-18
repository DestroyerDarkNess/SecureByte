using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.ByteEncryption
{
    public static class byteEncryption
    {
        public static byte[] Encrypt(byte[] data)
        {
            byte[] key = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF }; // Encryption key

            byte[] encryptedData = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                encryptedData[i] = (byte)(data[i] ^ key[i % key.Length]); // XOR encryption
            }

            return encryptedData;
        }
    }
}
