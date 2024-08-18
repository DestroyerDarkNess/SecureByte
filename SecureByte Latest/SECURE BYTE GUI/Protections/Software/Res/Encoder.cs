using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using ICore;
using Helpers.Mutations;

namespace Protections.Software
{
    public static class ResourcesEncoder
    {
        public static IList<MethodDef> Execute(Context context)
        {
            string mname = Utils.MethodsRenamig() + ".resources";
            int key = Utils.RandomTinyInt32();
            ModuleDefMD moduleDefMD = ModuleDefMD.Load(typeof(Runtime.ResRuntime).Module);
            TypeDef typeDef = moduleDefMD.ResolveTypeDef(MDToken.ToRID(typeof(Runtime.ResRuntime).MetadataToken));
            IEnumerable<IDnlibDef> source = Helpers.Injection.InjectHelper.Inject(typeDef, context.Module.GlobalType, context.Module);
            MethodDef init = (MethodDef)source.Single((IDnlibDef method) => method.Name == "Initialize");
            MethodDef ctor = context.Module.GlobalType.FindOrCreateStaticConstructor();
            var mutation = new MutationHelper("MutationClass");
            mutation.InjectKey<string>(init, 15, mname);
            mutation.InjectKey<int>(init, 0, key);
            //Helpers.Mutations.MutationHelper.InjectString(init, "h", mname);
            //Helpers.Mutations.MutationHelper.InjectKey(init, 10, key);
            ctor.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(init));
            foreach (var mem in source)
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
            string asmName = Utils.MethodsRenamig();
            var assembly = new AssemblyDefUser(asmName, new Version(0, 0));
            assembly.Modules.Add(new ModuleDefUser(asmName + ".dummy"));
            ModuleDef dlmodule = assembly.ManifestModule;
            assembly.ManifestModule.Kind = ModuleKind.Dll;
            var asmRef = new AssemblyRefUser(dlmodule.Assembly);
            for (int i = 0; i < context.Module.Resources.Count; i++)
            {
                if (context.Module.Resources[i] is EmbeddedResource)
                {
                    context.Module.Resources[i].Attributes = ManifestResourceAttributes.Private;
                    dlmodule.Resources.Add((context.Module.Resources[i] as EmbeddedResource));
                    context.Module.Resources.Add(new AssemblyLinkedResource(context.Module.Resources[i].Name, asmRef, context.Module.Resources[i].Attributes));
                    context.Module.Resources.RemoveAt(i);
                    i--;
                }
            }
            byte[] moduleBuff;
            using (var ms = new MemoryStream())
            {
                var options = new ModuleWriterOptions(dlmodule);
                options.MetadataOptions.Flags = MetadataFlags.PreserveAll;
                options.Cor20HeaderOptions.Flags = dnlib.DotNet.MD.ComImageFlags.ILOnly;
                options.ModuleKind = ModuleKind.Dll;
                dlmodule.Write(ms, options);
                var compressed = ICore.Compression.QuickLZ.CompressBytes(ms.ToArray());
                moduleBuff = Encrypt(compressed, key);
            }
            context.Module.Resources.Add(new EmbeddedResource(mname, moduleBuff, ManifestResourceAttributes.Private));
            var methods = new HashSet<MethodDef>
            {
                init
            };
            return methods.ToList();
        }        
        private static byte[] Encrypt(byte[] plainBytes, int key)
        {
            var aes = Rijndael.Create();
            aes.Key = SHA256.Create().ComputeHash(BitConverter.GetBytes(key));
            aes.IV = new byte[16];
            aes.Mode = CipherMode.CBC;
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return cipherBytes;
        }
    }
}
