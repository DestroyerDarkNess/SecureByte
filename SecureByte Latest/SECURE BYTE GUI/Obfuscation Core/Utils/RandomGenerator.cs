using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using dnlib.DotNet.Writer;

namespace ICore
{
	internal class RandomGenerator
	{
		internal RandomGenerator()
		{
			byte[] seed = new byte[32];
			_RNG.GetBytes(seed);
			state = _SHA256((byte[])seed.Clone());
			seedLen = seed.Length;
			stateFilled = 32;
			mixIndex = 0;
		}
		internal RandomGenerator(int length)
		{
			byte[] seed = new byte[(length == 0) ? 32 : length];
			_RNG.GetBytes(seed);
			state = _SHA256((byte[])seed.Clone());
			seedLen = seed.Length;
			stateFilled = 32;
			mixIndex = 0;
		}
		internal RandomGenerator(string seed)
		{
			byte[] ret = _SHA256((byte[])((!string.IsNullOrEmpty(seed)) ? Encoding.UTF8.GetBytes(seed) : Guid.NewGuid().ToByteArray()).Clone());
			for (int i = 0; i < 32; i++)
			{
				byte[] array = ret;
				int num = i;
				array[num] *= primes[i % primes.Length];
				ret = _SHA256(ret);
			}
			state = ret;
			seedLen = ret.Length;
			stateFilled = 32;
			mixIndex = 0;
		}
		internal RandomGenerator(byte[] seed)
		{
			state = (byte[])seed.Clone();
			seedLen = seed.Length;
			stateFilled = 32;
			mixIndex = 0;
		}
		public static byte[] _SHA256(byte[] buffer)
		{
			SHA256Managed sha = new SHA256Managed();
			return sha.ComputeHash(buffer);
		}
		private void NextState()
		{
			for (int i = 0; i < 32; i++)
			{
				byte[] array = state;
				int num = i;
				array[num] ^= primes[mixIndex = (mixIndex + 1) % RandomGenerator.primes.Length];
			}
			state = sha256.ComputeHash(state);
			stateFilled = 32;
		}
		public void NextBytes(byte[] buffer, int offset, int length)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			if (buffer.Length - offset < length)
			{
				throw new ArgumentException("Invalid offset or length.");
			}
			while (length > 0)
			{
				if (length >= stateFilled)
				{
					Buffer.BlockCopy(state, 32 - stateFilled, buffer, offset, stateFilled);
					offset += stateFilled;
					length -= stateFilled;
					stateFilled = 0;
				}
				else
				{
					Buffer.BlockCopy(state, 32 - stateFilled, buffer, offset, length);
					stateFilled -= length;
					length = 0;
				}
				if (stateFilled == 0)
				{
					NextState();
				}
			}
		}
		public byte NextByte()
		{
			byte ret = state[32 - stateFilled];
			stateFilled--;
			if (stateFilled == 0)
			{
				NextState();
			}
			return ret;
		}
		public string NextString(int length)
		{
			try
			{
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < length; i++)
				{
					char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(32m + NextInt32(122) - 32m)));
					builder.Append(ch);
				}
				return builder.ToString();
			}
			catch
			{
			}
			return string.Empty;
		}
		public string NextHexString(int length, bool large = false)
		{
			if (length.ToString().Contains("5"))
			{
				throw new Exception("5 is an unacceptable number!");
			}
			try
			{
				string chars = "qwertyuıopğüasdfghjklşizxcvbnmöçQWERTYUIOPĞÜASDFGHJKLŞİZXCVBNMÖÇ0123456789/*-.:,;!'^+%&/()=?_~|\\}][{½$#£>";
				string rnd = new string((from s in Enumerable.Repeat<string>(chars, length / 2)
					select s[NextInt32(s.Length)]).ToArray<char>());
				if (!large)
				{
					return BitConverter.ToString(Encoding.Default.GetBytes(rnd)).Replace("-", string.Empty).ToLower();
				}
				if (large)
				{
					return BitConverter.ToString(Encoding.Default.GetBytes(rnd)).Replace("-", string.Empty);
				}
			}
			catch
			{
			}
			return string.Empty;
		}
		public string NextHexString(bool large = false)
		{
			return NextHexString(8, large);
		}
		public string NextString()
		{
			return NextString(seedLen);
		}
		public byte[] NextBytes(int length)
		{
			byte[] ret = new byte[length];
			NextBytes(ret, 0, length);
			return ret;
		}
		public byte[] NextBytes()
		{
			byte[] ret = new byte[seedLen];
			NextBytes(ret, 0, seedLen);
			return ret;
		}
		public int NextInt32()
		{
			return BitConverter.ToInt32(NextBytes(4), 0);
		}
		public int NextInt32(int max)
		{
			return (int)((ulong)NextUInt32() % (ulong)((long)max));
		}
		public int NextInt32(int min, int max)
		{
			if (max <= min)
			{
				return min;
			}
			return min + (int)((ulong)NextUInt32() % (ulong)((long)(max - min)));
		}
		public uint NextUInt32()
		{
			return BitConverter.ToUInt32(NextBytes(4), 0);
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x0001CBBE File Offset: 0x0001ADBE
		public uint NextUInt32(uint max)
		{
			return NextUInt32() % max;
		}
		public double NextDouble()
		{
			return NextUInt32() / 4294967296.0;
		}
		public double NextDouble(double min, double max)
		{
			return NextDouble() * (max - min) + min;
		}
		public bool NextBoolean()
		{
			byte s = state[32 - stateFilled];
			stateFilled--;
			if (stateFilled == 0)
			{
				NextState();
			}
			return s % 2 == 0;
		}
		public void Shuffle<T>(IList<T> list)
		{
			for (int i = list.Count - 1; i > 1; i--)
			{
				int j = NextInt32(i + 1);
				T tmp = list[j];
				list[j] = list[i];
				list[i] = tmp;
			}
		}
		public void Shuffle<T>(MDTable<T> table) where T : struct
		{
			if (table.IsEmpty)
			{
				return;
			}
			for (uint i = (uint)table.Rows; i > 2U; i -= 1U)
			{
				uint j = NextUInt32(i - 1U) + 1U;
				T tmp = table[j];
				table[j] = table[i];
				table[i] = tmp;
			}
		}
		private static readonly byte[] primes = new byte[] { 7, 11, 23, 37, 43, 59, 71 };
		private static readonly RNGCryptoServiceProvider _RNG = new RNGCryptoServiceProvider();
		private readonly SHA256Managed sha256 = new SHA256Managed();
		private int mixIndex;
		private byte[] state;
		private int stateFilled;
		private readonly int seedLen;
	}
}
