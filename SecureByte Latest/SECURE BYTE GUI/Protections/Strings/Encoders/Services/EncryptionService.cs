using System;
using System.Collections.Generic;
using System.Text;

namespace Protections.Strings
{
    public class EncryptionService
    {
        private readonly List<byte> _encryptedData;
        private readonly int _globalKey;
        private int _index;
        public EncryptionService()
        {
            _encryptedData = new List<byte>();
            _globalKey = RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);
            _encryptedData.AddRange(BitConverter.GetBytes(_globalKey));
            _index = 4;
        }
        public int Index => _index ^ _globalKey;
        public uint Length => (uint)_encryptedData.Count;
        public byte[] Data => _encryptedData.ToArray();
        public void Encrypt(object input)
        {
            if (_index <= 0)
                throw new ArgumentOutOfRangeException(nameof(_index));
            string x = Convert.ToString(input);
            byte[] data = Xoring(Encoding.UTF8.GetBytes(x), out uint key);
            byte[] temp = new byte[8];
            BitConverter.GetBytes(data.Length).CopyTo(temp, 0);
            BitConverter.GetBytes(key).CopyTo(temp, 4);
            _index = _encryptedData.Count;
            _encryptedData.AddRange(temp);
            _encryptedData.AddRange(data);
        }
        private byte[] Xoring(byte[] data, out uint key)
        {
            key = (uint)RNGCryptoServiceProviderRandom.GetNextInt32(int.MaxValue);      
            int n = data.Length - 1;
            for (int i = 0; i < n; i++, n--)
            {
                data[i] ^= data[n];
                data[n] ^= (byte)(data[i] ^ key);
                data[i] ^= data[n];
            }
            if (data.Length % 2 != 0)
                data[data.Length >> 1] ^= (byte)key; 
            return data;
        }
    }
}