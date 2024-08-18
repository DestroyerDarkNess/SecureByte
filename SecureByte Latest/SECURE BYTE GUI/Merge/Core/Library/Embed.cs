using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.Load;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace EmbedLibrary.Core.Library
{
    public class Embed
    {
        private static byte[] Compress(byte[] data)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
                    {
                        deflateStream.Write(data, 0, data.Length);
                    }
                    return memoryStream.ToArray();
                }
            }
            catch
            {
                return null;
            }
        }
        public static void Execute(AssemblyContext context, string[] librarys)
        {
            foreach (var path in librarys)
            {
                string name = Path.GetFileName(path);
                byte[] file = File.ReadAllBytes(path);
                byte[] zip = Compress(file);
                context.Resources.Add($"Embedded.{name}", zip);
            }
            var typeDef = context.GetTypeDef(typeof(Loader));
            var copyTypeDef = context.CopyTypeDef(typeDef, ICore.Utils.MethodsRenamig(), ICore.Utils.MethodsRenamig());
            context.Module.Types.Add(copyTypeDef);
            var method = (MethodDef)copyTypeDef.Methods.Single(m => m.Name == "Load");
            context.GlobalTypeStaticConstructor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, method));
            SECURE_BYTE_GUI.GUI._librarys.Clear();
        }
    }
}
