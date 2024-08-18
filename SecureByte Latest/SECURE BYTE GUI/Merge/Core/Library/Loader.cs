using System;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.IO.Compression;

namespace EmbedLibrary.Core.Library
{
    internal class Loader
    {
        public static void Load()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static byte[] Decompress(byte[] data)
        {
            try
            {
                using (MemoryStream decompressedStream = new MemoryStream())
                {
                    using (MemoryStream compressStream = new MemoryStream(data))
                    {
                        using (DeflateStream deflateStream = new DeflateStream(compressStream, CompressionMode.Decompress))
                        {
                            deflateStream.CopyTo(decompressedStream);
                        }
                    }
                    return decompressedStream.ToArray();
                }
            }
            catch
            {
                return null;
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);

            var assembly = ReadExistingAssembly(assemblyName);
            if (assembly != null)
                return assembly;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Embedded.{assemblyName.Name}.dll"))
            {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                byte[] unzip = Decompress(data);
                return Assembly.Load(unzip);
            }
        }

        private static Assembly ReadExistingAssembly(AssemblyName name)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssemblyName assemblyName = assembly.GetName();
                if (string.Equals(assemblyName.Name, name.Name, StringComparison.InvariantCultureIgnoreCase) && string.Equals(CultureToString(assemblyName.CultureInfo), CultureToString(name.CultureInfo), StringComparison.InvariantCultureIgnoreCase))
                    return assembly;
            }

            return null;
        }

        private static string CultureToString(CultureInfo culture)
        {
            if (culture == null)
                return string.Empty;

            return culture.Name;
        }
    }
}
