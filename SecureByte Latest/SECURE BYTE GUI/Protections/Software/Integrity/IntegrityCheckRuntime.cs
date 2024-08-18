using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Protections.Runtime
{
    internal static class IntegrityCheckRuntime
    {
        internal static void Initialize()
        {
            BinaryReader binaryReader = new BinaryReader(new StreamReader(typeof(IntegrityCheckRuntime).Assembly.Location).BaseStream);
            byte[] metin = binaryReader.ReadBytes(File.ReadAllBytes(typeof(IntegrityCheckRuntime).Assembly.Location).Length - 32);
            binaryReader.BaseStream.Position = binaryReader.BaseStream.Length - 32L;
            if (MD5(metin) != Encoding.ASCII.GetString(binaryReader.ReadBytes(32)))
            {
                Environment.Exit(0);
            }
        }
        internal static string MD5(byte[] metin)
        {
            metin = new MD5CryptoServiceProvider().ComputeHash(metin);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in metin)
            {
                stringBuilder.Append(b.ToString("x2").ToLower());
            }
            return stringBuilder.ToString();
        }
    }
}
