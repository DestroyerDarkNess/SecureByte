using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Helpers.Injection;
using ICore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Protections.Software
{
    public class dnSpyCrasher
    {
        public static void Crash(Context context)
        {
            foreach (TypeDef typeDef in context.Module.Assembly.ManifestModule.Types)
            {
                foreach (MethodDef methodDef in typeDef.Methods)
                {
                    if (methodDef == context.Module.GlobalType.FindStaticConstructor())
                    {
                        for (int i = 0; i < 100000; i++)
                        {
                            methodDef.Body.Instructions.Insert(i, new Instruction(OpCodes.Nop));
                        }
                    }
                }
            }
        }
    }
}
