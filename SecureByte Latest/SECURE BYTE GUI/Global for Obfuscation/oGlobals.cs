using ICore;

namespace SECURE_BYTE_GUI.Global_for_Obfuscation
{
    public static class oGlobals
    {
        public static Context ctx = null;
        public static bool dynCctor = false;
        public static bool Atamper = false;
        public static bool AtamperWithAdump = false;
        public static bool InvalidMD = false;
        public static bool Integrity = false;
        public static bool Virt = false;
        //
        public static string FPath;
        public static string browseDir = "";
        public static string browseDir2 = "";
        //
        public static string x86RT = "";
        public static string x64RT = "";
        //
        public static string asmPath = "";
        //
        public static bool excludeforAC = false;
        public static string excludeString = "";
    }
}
