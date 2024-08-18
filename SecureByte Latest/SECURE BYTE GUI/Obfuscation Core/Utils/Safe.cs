using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ICore
{
    public static class Safe
    {
        private static readonly List<string> used_names = new List<string>();
        public static string GenerateRandomMD5()
        {
            string randomString_md5;
            do
            {
                string randomString = GenerateRandomLetters(Utils.RandomInt32());
                randomString_md5 = MD5Hash(randomString);
                if (char.IsDigit(randomString_md5[0]))
                {
                    char randomLetter = GetLetter();
                    randomString_md5 = randomString_md5.Replace(randomString_md5[0], randomLetter);
                }
            } while (CheckStringExists(randomString_md5));
            used_names.Add(randomString_md5);
            return randomString_md5;
        }
        public static string GenerateRandomLetters(int size)
        {
            var charSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var chars = charSet.ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(size);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
        public static string GenerateRandomTibetan(int size)
        {
            var charSet = "ཀཁགགྷངཅཆཇཉཊཋཌཌྷཎཏཐདདྷནཔཕབབྷམཙཚཛཛྷ/*ཝཞཟའཡརལཤཥསཧཨཀྵཪཫཬ༠༡༢༣༤༥༦༧༨༩༳༪༫༬༭༮༯༰༱༲༁༂༃༄༅༆༇༈༉༊་༌།༎༏༐༑༒༓ༀ*/";
            var chars = charSet.ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(size);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
        public static string GenerateRandomNumbers(int size)
        {
            var charSet = Protections.Strings.RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue).ToString();
            var chars = charSet.ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(size);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
        #region Utils
        private static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));
            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x"));
            }
            return hash.ToString();
        }
        private static char GetLetter()
        {
            System.Random rnd = new System.Random();
            int num = rnd.Next(0, 24);
            return (char)('A' + num);
        }
        private static bool CheckStringExists(string stringToCheck)
        {
            if (used_names.Contains(stringToCheck))
                return true;
            return false;
        }
        #endregion
    }
}