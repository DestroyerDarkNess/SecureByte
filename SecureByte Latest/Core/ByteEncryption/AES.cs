using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Core.ByteEncryption
{
    public class ManagedAesSample
    {
        public static byte[] Encrypt(byte[] input)
        {
            PasswordDeriveBytes pdb =
              new PasswordDeriveBytes("hjiweykaksd", // Change this
              new byte[] { 0x43, 0x87, 0x23, 0x72 }); // Change this
            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms,
              aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }
        public static byte[] Decrypt(byte[] input)
        {
            PasswordDeriveBytes pdb =
              new PasswordDeriveBytes("hjiweykaksd", // Change this
              new byte[] { 0x43, 0x87, 0x23, 0x72 }); // Change this
            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms,
              aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }
    }
}