using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.Inject;
using dnlib.Load;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnlib.Extensions
{
    public static class Extension
    {
        public static AssemblyContext GetAssemblyContext(this ModuleDefMD module)
        {
            return new AssemblyContext(module);
        }
    }
}
