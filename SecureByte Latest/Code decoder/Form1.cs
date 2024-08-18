using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Code_decoder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Inx.Ui.Movement.Move(this, e);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Opacity = 0.98;
            guna2ShadowForm1.SetShadowForm(this);
        }
        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(guna2TextBox2.Text))
            {
                string value = guna2TextBox2.Text;
                string decoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(new string("ISB".Select((c, i) => (char)(c ^ value[i % value.Length])).ToArray())));
                Clipboard.SetText(decoded);
                guna2TextBox2.Text = "Decoded & copied to clipboard !";
            }
            else
            {
                guna2TextBox2.Text = "Where is the code !";
            }
        }
    }
}
