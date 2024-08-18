using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SECURE_BYTE_GUI
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private void Form1_Load(object sender, EventArgs e)
        {
            Global_Checker.globalChecker.ints[0] = 1;
            Opacity = 0.98;
            guna2ShadowForm1.SetShadowForm(this);
            password.PasswordChar = '●';
            welcomeLabel.Text = "Welcome back " + Environment.UserName + " , Login to continue";
            noAcc.Cursor = Cursors.Hand;
            saveSession.Checked = true;
            if (File.Exists(appdata + @"\SecureByte.txt"))
            {
                string[] data = File.ReadAllLines(appdata + @"\SecureByte.txt");
                if (data != null)
                {
                    username.Text = data[0];
                    password.Text = data[1];
                }
            }
        }
        private void exitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(-1);
        }
        private void minButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private void password_TextChanged(object sender, EventArgs e)
        {
            if (password.Text != null)
                showPassword.Visible = true;
            if (password.Text.Length == 0)
                showPassword.Visible = false;
        }
        bool showhidepass = false;
        private void showPassword_Click(object sender, EventArgs e)
        {
            if (showhidepass)
            {
                showhidepass = false;
                password.PasswordChar = '●';
                showPassword.Image = Properties.Resources.eye_20px;
            }
            else
            {
                showhidepass = true;
                password.PasswordChar = '\0';
                showPassword.Image = Properties.Resources.hide_20px;
            }
        }
        private void loginButton_Click(object sender, EventArgs e)
        {
            try
            {
                Program.Auth.init();
            }
            catch
            {
                customMessage.msg = "Error, Please check your internet connection !";
                new customMessage().ShowDialog();
                return;
            }
            validImage.Visible = true;
            validLabel.Visible = true;
            validProgressbar.Visible = true;
            proceed.Start();
        }
        private void proceed_Tick(object sender, EventArgs e)
        {
            loginButton.Enabled = false;
            validProgressbar.Increment(6);
            if (validProgressbar.Value == validProgressbar.Maximum)
            {
                proceed.Stop();
                validImage.Visible = false;
                validLabel.Visible = false;
                validProgressbar.Visible = false;
                validProgressbar.Value = 0;
                loginButton.Enabled = true;
                Program.Auth.login(username.Text, password.Text);
                if (Program.Auth.response.success)
                {
                    if (saveSession.Checked)
                    {
                        StreamWriter streamWriter = new StreamWriter(appdata + @"\SecureByte.txt");
                        streamWriter.WriteLine(username.Text);
                        streamWriter.WriteLine(password.Text);
                        streamWriter.Flush();
                        streamWriter.Dispose();
                    }
                    Hide();
                    new GUI().ShowDialog();
                    Close();
                    return;
                }
                else
                {
                    customMessage.msg = "Error, Please check your username & password !";
                    new customMessage().ShowDialog();
                }
            }
        }
        private void guna2Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Inx.Ui.Movement.Move(this, e, false);
        }
        private void discordButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.com/invite/cAT83z5gCS");
        }
    }
}
