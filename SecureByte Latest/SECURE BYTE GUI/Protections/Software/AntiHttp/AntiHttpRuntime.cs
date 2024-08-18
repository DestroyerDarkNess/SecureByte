using System.Net;
using System.Security;

namespace Protections.Runtime
{
    internal static class AntiHttpRuntime
    {
        internal static void Initialize()
        {
            WebRequest.DefaultWebProxy = new WebProxy();
        }
    }
}
