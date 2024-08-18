using SECURE_BYTE_GUI.Global_for_Obfuscation;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SECURE_BYTE_GUI
{
    public partial class customMessage : Form
    {
        public customMessage()
        {
            InitializeComponent();
        }
        private void customMessage_Load(object sender, EventArgs e)
        {
            Opacity = 0.98;
            guna2ShadowForm1.SetShadowForm(this);
        }
        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        public static string msg { get; set; }
        public static bool status { get; set; }
        private void customMessage_Shown(object sender, EventArgs e)
        {
            msgLabel.Text = msg;
            if (Global_for_links.globalLinks.type != "") okButton.Visible = true;
        }
        private void customMessage_MouseDown(object sender, MouseEventArgs e)
        {
            Inx.Ui.Movement.Move(this, e, false);
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            switch (Global_for_links.globalLinks.type)
            {
                case "Dir":
                    Process.Start(oGlobals.browseDir);
                    Close();
                    break;
                case "Paypal":
                    Process.Start("https://www.paypal.com/paypalme/Inx707");
                    Close();
                    break;
                case "Litecoin":
                    Clipboard.SetText("Le7UBT554mvNFb5oJt8npE68R9S9Zexec4");
                    Close();
                    break;
                case "Bitcoin":
                    Clipboard.SetText("1MTrX1sEXapi8hNCBhpB3Am5gYC5ykRmw");
                    Close();
                    break;
                case "Ethereum":
                    Clipboard.SetText("0xbe975b1be566a28fdad9ffb80b09ccb23b3007c4");
                    Close();
                    break;
                case "Tether":
                    Clipboard.SetText("TRKkkLQ6nE25qp4KLsAKMVj4qpjeBGhYzy");
                    Close();
                    break;
                case "Tron":
                    Clipboard.SetText("TRKkkLQ6nE25qp4KLsAKMVj4qpjeBGhYzy");
                    Close();
                    break;
            }
            Global_for_links.globalLinks.type = "";
        }
    }
}
