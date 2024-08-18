using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Inx.Ui
{
    class Movement
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public static void Move(Form form, MouseEventArgs e)
        {
            if (MouseButtons.Left == e.Button)
            {
                form.Opacity = 0.90;
                form.Cursor = Cursors.SizeAll;
                ReleaseCapture();
                SendMessage(form.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                form.Opacity = 0.98;
                form.Cursor = Cursors.Default;
            }     
        }
    }
}
