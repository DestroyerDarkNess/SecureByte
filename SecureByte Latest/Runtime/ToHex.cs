using System;
using System.Text;

namespace xxx
{
    public class H956FA8EA3BCB94A83E978D9923BA39F39
    {
        public static string H956FA8EA3BCB94A83E978D9923BA39F38(string S)
        {
            var bytes = new byte[S.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(S.Substring(i * 2, 2), 16);
            }
            return Encoding.Unicode.GetString(bytes);
        }
    }
}
