using KeyAuth;
using System;
using System.Windows.Forms;

namespace SECURE_BYTE_GUI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Check_for_updates.updateChecker.checkforUpdates();
            Application.Run(new GUI());
        }
        public static api Auth = new api(
        name: "SecureByte",
        ownerid: "bA4iou97T1",
        secret: "6ab6ee572335bbf80ff052d8b794c7c2efce4e60e5e3ea22b100d8911c308094",
        version: "1.0"
        );
    }
}
