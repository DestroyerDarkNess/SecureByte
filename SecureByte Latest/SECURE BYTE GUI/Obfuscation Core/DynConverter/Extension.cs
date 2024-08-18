using dnlib.DotNet;
using System.IO;

namespace Helpers.DynConverter
{
    public static class Extension
    {
        public static void ConvertToBytes(this BinaryWriter writer, MethodDef method) => new Converter(method, writer).ConvertToBytes();
    }
}
