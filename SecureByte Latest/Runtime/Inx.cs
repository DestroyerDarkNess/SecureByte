using Runtime;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace ConversionBack
{
    [SecuritySafeCritical]
    public class Dyn : Dyn2
    {
        private static object obj(object o, object[] oo)
        {
            return VM.value.Invoke(o, oo);
        }
        private static byte[] dec(byte[] key, byte[] message)
        {
           return VM.Decrypt(VM.eBytes.Decrypt(key), message);
        }
        private static byte[] grab(byte[] bytes, int s, int i)
        {
            return VM.byteArrayGrabber(bytes, s, i);
        }
        private static byte[] dkey(string name)
        {
            return MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(name));
        }
        private static byte[] glba(MethodBase methodBase)
        {
            return methodBase.GetMethodBody().GetILAsByteArray();
        }
        [System.Reflection.ObfuscationAttribute(Feature = "Virtualization", Exclude = false)]
        public static object Run(string Class, object[] parameters)
        {
            MethodBase mbase = null;
            int? nID2 = 0;
            int i = (int)nID2;
            byte[] decrypted = { (byte)i };
            string DClass = Xor.Xoring(Class);
            string[] Arrays = DClass.Split(' ');
            int nID = IConverter.Convertion(Arrays[1]);
            if (VM.cache.TryGetValue(nID, out VM.value))
            {
                return obj(null, parameters);
            }
            else
            {
                var callingMethod = new StackTrace().GetFrame(1).GetMethod();
                int nPos = IConverter.Convertion(Arrays[i]);
                int nSize = IConverter.Convertion(Arrays[2]);
                var grabbedBytes = grab(ConversionBack.Class.byteArrayResource, nPos, nSize);
                var decryptionKey = dkey(callingMethod.Name);
                var ab = glba(callingMethod);
                ConversionBack.Class.bc(new Cryptographer("أً").Encrypt(grabbedBytes), new Cryptographer("أً").Encrypt(grabbedBytes).Length, new Cryptographer("أً").Encrypt(ab), new Cryptographer("أً").Encrypt(ab).Length);
                decrypted = dec(decryptionKey, grabbedBytes);
                string[] Arrays2 = DClass.Split(' ');
                nID2 = IConverter.Convertion(Arrays2[1]);
                mbase = callingMethod;
            }
            return ___(mbase, parameters, (int)nID2, new Cryptographer("أً").Encrypt(decrypted));
        }
    }
}
