using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynCore
{
    public static class Execute
    {
        public static MemoryStream Protect(ModuleDefMD assemblyData)
        {
            return Core.Protector.Protect(assemblyData);
        }

        public static byte[] Protect(byte[] assemblyData) { return Protect(ModuleDefMD.Load(assemblyData)).ToArray(); }
    }
}
