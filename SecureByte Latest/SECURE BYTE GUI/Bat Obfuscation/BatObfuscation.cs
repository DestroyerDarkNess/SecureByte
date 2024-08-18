using System;
using System.IO;
using System.Text;

namespace BatObfuscation
{
    public static class Bat
    {
        static ProtoRandom protoRandom = new ProtoRandom(5);
        static string _alphaCharacters = "abcdefghijklmnopqrstuvwxyz";
        private static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }
        public static void Encrypt(string codeToObfuscate, string newpath)
        {
            StringBuilder obfuscatedCode = new StringBuilder();
            obfuscatedCode.Append("cls\r\n@echo off\r\ncls\r\n");
            string[] alphaCharactersDefinitions = new string[27];
            for (int i = 0; i < 26; i++)
            {
                alphaCharactersDefinitions[i] = protoRandom.GetRandomString(_alphaCharacters, 6);
                obfuscatedCode.Append($"set {alphaCharactersDefinitions[i]}={_alphaCharacters[i]}\r\n");
            }
            string theStr = "";
            for (int i = 0; i < codeToObfuscate.Length; i++)
            {
                char theCharacter = codeToObfuscate[i];
                bool exists = false;
                for (int j = 0; j < _alphaCharacters.Length; j++)
                {
                    if (theCharacter.Equals(_alphaCharacters[j]))
                    {
                        exists = true;
                        theStr = alphaCharactersDefinitions[j];
                    }
                }
                if (exists)
                {
                    obfuscatedCode.Append($"%{theStr}%");
                }
                else
                {
                    obfuscatedCode.Append(theCharacter);
                }
            }
            File.WriteAllBytes(newpath, Combine(new byte[4] { 0xFF, 0xFE, 0x0D, 0x0A }, Encoding.UTF8.GetBytes(obfuscatedCode.ToString())));
        }
    }
}