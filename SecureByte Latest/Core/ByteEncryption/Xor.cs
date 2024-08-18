using System.Text;

namespace Core.ByteEncryption
{
    internal class Xor
    {
        public static string XorProcess(string text, string key)
        {
            var result = new StringBuilder();
            for (int c = 0; c < text.Length; c++)
                result.Append((char)(text[c] ^ (uint)key[c % key.Length]));
            return result.ToString();
        }
    }
}
