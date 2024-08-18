using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using ICore;
using Protections.Runtime;

namespace Protections.Software
{
    class IntegrityCheck
    {
        #region Integrity
        internal static string MD5(byte[] metin)
        {
            MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] array = md5CryptoServiceProvider.ComputeHash(metin);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in array)
            {
                stringBuilder.Append(b.ToString("x2").ToLower());
            }
            return stringBuilder.ToString();
        }
        private static void HashFile(object sender, ModuleWriterEventArgs writer)
        {
            if (writer.Event == ModuleWriterEvent.End)
            {
                StreamReader streamReader = new StreamReader(writer.Writer.DestinationStream);
                byte[] metin = new BinaryReader(streamReader.BaseStream)
                {
                    BaseStream =
                    {
                        Position = 0L
                    }
                }.ReadBytes((int)streamReader.BaseStream.Length);
                string s = MD5(metin);
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                writer.Writer.DestinationStream.Position = writer.Writer.DestinationStream.Length;
                writer.Writer.DestinationStream.Write(bytes, 0, bytes.Length);
            }
        }
        #endregion
        public static void Execute(Context context)
        {
            context.ModOpts.WriterEvent += HashFile;
            ModuleDefMD moduleDefMD = ModuleDefMD.Load(typeof(IntegrityCheckRuntime).Module);
            TypeDef typeDef = moduleDefMD.ResolveTypeDef(MDToken.ToRID(typeof(IntegrityCheckRuntime).MetadataToken));
            IEnumerable<IDnlibDef> defs = Helpers.Injection.InjectHelper.Inject(typeDef, context.Module.GlobalType, context.Module);
            MethodDef method2 = (MethodDef)defs.Single((IDnlibDef method) => method.Name == "Initialize");
            MethodDef entryPoint = context.Module.GlobalType.FindOrCreateStaticConstructor();
            entryPoint.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(method2));
            foreach (var mem in defs)
            {
                if (mem is MethodDef method)
                {
                    if (method.HasImplMap)
                        continue;
                    if (method.DeclaringType.IsDelegate)
                        continue;
                }
                Utils.MethodsRenamig(mem);
            }
        }
    }
}
