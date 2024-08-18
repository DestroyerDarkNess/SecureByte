using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;

public class ProtoRandom
{
    public ProtoRandom(int complexity = 100)
    {
        this.complexity = complexity;
    }
    public int GetComplexity()
    {
        return this.complexity;
    }
    public void SetComplexity(int complexity)
    {
        this.complexity = complexity;
    }
    private BigInteger Modulus(BigInteger a, BigInteger b)
    {
        return (BigInteger.Abs(a * b) + a) % b;
    }
    private int GetBasicRandomInt32()
    {
        RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider();
        byte[] array = new byte[4];
        randomNumberGenerator.GetBytes(array);
        int num = BitConverter.ToInt32(array, 0);
        if (num < 0)
        {
            num *= -1;
        }
        return num;
    }
    private uint GetBasicRandomUInt32()
    {
        RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider();
        byte[] array = new byte[4];
        randomNumberGenerator.GetBytes(array);
        return BitConverter.ToUInt32(array, 0);
    }
    private long GetBasicRandomInt64()
    {
        RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider();
        byte[] array = new byte[8];
        randomNumberGenerator.GetBytes(array);
        long num = BitConverter.ToInt64(array, 0);
        if (num < 0L)
        {
            num *= -1L;
        }
        return num;
    }
    private ulong GetBasicRandomUInt64()
    {
        RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider();
        byte[] array = new byte[8];
        randomNumberGenerator.GetBytes(array);
        return BitConverter.ToUInt64(array, 0);
    }
    private short GetBasicRandomInt16()
    {
        RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider();
        byte[] array = new byte[2];
        randomNumberGenerator.GetBytes(array);
        short num = BitConverter.ToInt16(array, 0);
        if (num < 0)
        {
            num *= -1;
        }
        return num;
    }
    private ushort GetBasicRandomUInt16()
    {
        RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider();
        byte[] array = new byte[2];
        randomNumberGenerator.GetBytes(array);
        return BitConverter.ToUInt16(array, 0);
    }
    private double GetBasicRandomDouble()
    {
        RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider();
        byte[] array = new byte[8];
        randomNumberGenerator.GetBytes(array);
        double num = BitConverter.ToDouble(array, 0);
        if (num < 0.0)
        {
            num *= -1.0;
        }
        if (num.ToString().Contains("E"))
        {
            num = double.Parse(num.ToString().Split(new char[] { 'E' })[0]);
        }
        return num;
    }
    public byte[] GetRandomByteArray(int size)
    {
        RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider();
        byte[] array = new byte[size];
        randomNumberGenerator.GetBytes(array);
        return array;
    }
    public byte[] GetRandomBytes(int size)
    {
        RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider();
        byte[] array = new byte[size];
        randomNumberGenerator.GetBytes(array);
        return array;
    }
    public byte[] GetRandomByteArray(int min, int max)
    {
        RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider();
        byte[] array = new byte[this.GetRandomInt32(min, max)];
        randomNumberGenerator.GetBytes(array);
        return array;
    }
    public byte[] GetRandomBytes(int min, int max)
    {
        RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider();
        byte[] array = new byte[this.GetRandomInt32(min, max)];
        randomNumberGenerator.GetBytes(array);
        return array;
    }
    public int GetRandomInt32()
    {
        List<int[]> list = new List<int[]>();
        for (int i = 0; i < this.complexity; i++)
        {
            int[] array = new int[this.complexity];
            for (int j = 0; j < this.complexity; j++)
            {
                array[j] = this.GetBasicRandomInt32();
            }
            list.Add(array);
        }
        return list[this.GetBasicRandomInt32() % this.complexity][this.GetBasicRandomInt32() % this.complexity];
    }
    public int GetRandomInt32(int max)
    {
        return this.GetRandomInt32() % (max + 1);
    }
    public int GetRandomInt32(int min, int max)
    {
        return this.GetRandomInt32() % (max - min + 1) + min;
    }
    public bool GetRandomBoolean()
    {
        return this.GetRandomInt32(1) != 0;
    }
    public string GetRandomString(char[] chars, int length)
    {
        string text = "";
        for (int i = 0; i < length; i++)
        {
            text += chars[this.GetRandomInt32(0, chars.Length - 1)].ToString();
        }
        return text;
    }
    public string GetRandomString(string chars, int length)
    {
        string text = "";
        for (int i = 0; i < length; i++)
        {
            text += chars[this.GetRandomInt32(0, chars.Length - 1)].ToString();
        }
        return text;
    }
    public string GetRandomString(char[] chars, int min, int max)
    {
        return this.GetRandomString(chars, this.GetRandomInt32(min, max));
    }
    public string GetRandomString(string chars, int min, int max)
    {
        return this.GetRandomString(chars, this.GetRandomInt32(min, max));
    }
    public uint GetRandomUInt32()
    {
        List<uint[]> list = new List<uint[]>();
        for (int i = 0; i < this.complexity; i++)
        {
            uint[] array = new uint[this.complexity];
            for (int j = 0; j < this.complexity; j++)
            {
                array[j] = this.GetBasicRandomUInt32();
            }
            list.Add(array);
        }
        return list[this.GetBasicRandomInt32() % this.complexity][this.GetBasicRandomInt32() % this.complexity];
    }
    public uint GetRandomUInt32(uint max)
    {
        return uint.Parse(this.Modulus(BigInteger.Parse(this.GetRandomUInt32().ToString()), BigInteger.Parse((max + 1U).ToString())).ToString());
    }
    public uint GetRandomUInt32(uint min, uint max)
    {
        return uint.Parse(this.Modulus(BigInteger.Parse(this.GetRandomUInt32().ToString()), BigInteger.Parse((max - min + 1U).ToString())).ToString()) + min;
    }
    public short GetRandomInt16()
    {
        List<short[]> list = new List<short[]>();
        for (int i = 0; i < this.complexity; i++)
        {
            short[] array = new short[this.complexity];
            for (int j = 0; j < this.complexity; j++)
            {
                array[j] = this.GetBasicRandomInt16();
            }
            list.Add(array);
        }
        return list[this.GetBasicRandomInt32() % this.complexity][this.GetBasicRandomInt32() % this.complexity];
    }
    public short GetRandomInt16(short max)
    {
        return short.Parse(this.Modulus(BigInteger.Parse(this.GetRandomInt16().ToString()), BigInteger.Parse(((int)(max + 1)).ToString())).ToString());
    }
    public ushort GetRandomUInt16()
    {
        List<ushort[]> list = new List<ushort[]>();
        for (int i = 0; i < this.complexity; i++)
        {
            ushort[] array = new ushort[this.complexity];
            for (int j = 0; j < this.complexity; j++)
            {
                array[j] = this.GetBasicRandomUInt16();
            }
            list.Add(array);
        }
        return list[this.GetBasicRandomInt32() % this.complexity][this.GetBasicRandomInt32() % this.complexity];
    }
    public ushort GetRandomUInt16(ushort max)
    {
        return ushort.Parse(this.Modulus(BigInteger.Parse(this.GetRandomInt16().ToString()), BigInteger.Parse(((int)(max + 1)).ToString())).ToString());
    }
    public long GetRandomInt64()
    {
        List<long[]> list = new List<long[]>();
        for (int i = 0; i < this.complexity; i++)
        {
            long[] array = new long[this.complexity];
            for (int j = 0; j < this.complexity; j++)
            {
                array[j] = this.GetBasicRandomInt64();
            }
            list.Add(array);
        }
        return list[this.GetBasicRandomInt32() % this.complexity][this.GetBasicRandomInt32() % this.complexity];
    }
    public long GetRandomInt64(long max)
    {
        return this.GetRandomInt64() % (max + 1L);
    }
    public long GetRandomInt64(long min, long max)
    {
        return this.GetRandomInt64() % (max - min + 1L) + min;
    }
    public ulong GetRandomUInt64()
    {
        List<ulong[]> list = new List<ulong[]>();
        for (int i = 0; i < this.complexity; i++)
        {
            ulong[] array = new ulong[this.complexity];
            for (int j = 0; j < this.complexity; j++)
            {
                array[j] = this.GetBasicRandomUInt64();
            }
            list.Add(array);
        }
        return list[this.GetBasicRandomInt32() % this.complexity][this.GetBasicRandomInt32() % this.complexity];
    }
    public ulong GetRandomUInt64(ulong max)
    {
        return ulong.Parse(this.Modulus(BigInteger.Parse(this.GetRandomUInt64().ToString()), BigInteger.Parse((max + 1UL).ToString())).ToString());
    }
    public ulong GetRandomUInt64(ulong min, ulong max)
    {
        return ulong.Parse(this.Modulus(BigInteger.Parse(this.GetRandomUInt64().ToString()), BigInteger.Parse((max - min + 1UL).ToString())).ToString()) + min;
    }
    public double GetRandomDouble()
    {
        List<double[]> list = new List<double[]>();
        for (int i = 0; i < this.complexity; i++)
        {
            double[] array = new double[this.complexity];
            for (int j = 0; j < this.complexity; j++)
            {
                array[j] = this.GetBasicRandomDouble();
            }
            list.Add(array);
        }
        return list[this.GetBasicRandomInt32() % this.complexity][this.GetBasicRandomInt32() % this.complexity];
    }
    public double GetRandomDouble(double max)
    {
        return this.modulo(this.GetRandomDouble(), max + 1.0, 14.0);
    }
    public double GetRandomDouble(double min, double max)
    {
        return this.modulo(this.GetRandomDouble(), max - min + 1.0, 14.0) + min;
    }
    private double modulo(double a, double b, double num_sig_digits = 14.0)
    {
        if (b == Math.Floor(b))
        {
            return a % b;
        }
        double num = Math.Round(a / b);
        double num2 = Math.Abs(a - num * b);
        if (num2 < Math.Pow(10.0, -num_sig_digits))
        {
            return 0.0;
        }
        return num2 * (double)Math.Sign(a);
    }
    private int complexity;
}