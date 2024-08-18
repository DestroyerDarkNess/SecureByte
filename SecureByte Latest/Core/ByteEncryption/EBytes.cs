using System.Text;

namespace Core.ByteEncryption
{
    public class EBytes
    {
        private byte[] Keys { get; set; }
        public EBytes(string password)
        {
            Keys = Encoding.ASCII.GetBytes(password);
        }
        public byte[] Encrypt(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ Keys[i % Keys.Length]);
            }
            return data;
        }
    }
}
