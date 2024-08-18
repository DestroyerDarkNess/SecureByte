using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ICore;
using System;
using System.Collections.Generic;

namespace Protections.Renamer
{
    public class hideNSpace
    {
        public static void Hide(Context context)
        {
            foreach (TypeDef typeDef in context.Module.Types)
            {
                typeDef.Namespace = "";
            }
        }
    }
}
