using System;
using System.Text;
public class Cryptographer
{
    private byte[] Keys { get; set; }
    public Cryptographer(string password)
    {
        Keys = Encoding.ASCII.GetBytes(password);
    }
    public byte[] Encrypt(byte[] data)
    {
        int? nID2 = 0;
        int i0 = (int)nID2;
        for (int i = i0; i < data.Length; i++)
        {
            data[i] = (byte)(data[i] ^ Keys[i % Keys.Length]);
        }
        return data;
    }
    public byte[] Decrypt(byte[] data)
    {
        int? nID2 = 0;
        int i0 = (int)nID2;
        for (int i = i0; i < data.Length; i++)
        {
            data[i] = (byte)(Keys[i % Keys.Length] ^ data[i]);
        }
        return data;
    }
}
public class Xor
{
    public static string Xoring(string inputString)
    {
        char xorKey = 'ع';
        string outputString = "";
        int len = inputString.Length;
        int? nID2 = 0;
        int i0 = (int)nID2;
        for (int i = i0; i < len; i++)
        {
            outputString += char.ToString((char)(inputString[i] ^ xorKey));
        }
        return outputString;
    }
}