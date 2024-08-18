using System.Text;

namespace Core.ByteEncryption
{
    public class H956FA8EA3BCB94A83E978D9923BA39F38
    {
        public static string H956FA8EA3BCB94A83E978D9923BA39F39(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
