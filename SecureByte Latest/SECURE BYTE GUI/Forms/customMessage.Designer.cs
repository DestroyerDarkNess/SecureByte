namespace SECURE_BYTE_GUI
{
    partial class customMessage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(customMessage));
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.guna2PictureBox1 = new Guna.UI2.WinForms.Guna2PictureBox();
            this.guna2ShadowForm1 = new Guna.UI2.WinForms.Guna2ShadowForm(this.components);
            this.exitButton = new Guna.UI2.WinForms.Guna2ImageButton();
            this.msgLabel = new System.Windows.Forms.Label();
            this.okButton = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 20;
            this.guna2Elipse1.TargetControl = this;
            // 
            // guna2PictureBox1
            // 
            this.guna2PictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.guna2PictureBox1.FillColor = System.Drawing.Color.Transparent;
            this.guna2PictureBox1.Image = global::SECURE_BYTE_GUI.Properties.Resources.info_96px;
            this.guna2PictureBox1.ImageRotate = 0F;
            this.guna2PictureBox1.Location = new System.Drawing.Point(31, 42);
            this.guna2PictureBox1.Name = "guna2PictureBox1";
            this.guna2PictureBox1.Size = new System.Drawing.Size(96, 96);
            this.guna2PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.guna2PictureBox1.TabIndex = 1;
            this.guna2PictureBox1.TabStop = false;
            this.guna2PictureBox1.UseTransparentBackground = true;
            // 
            // guna2ShadowForm1
            // 
            this.guna2ShadowForm1.BorderRadius = 20;
            // 
            // exitButton
            // 
            this.exitButton.CheckedState.ImageSize = new System.Drawing.Size(64, 64);
            this.exitButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.exitButton.HoverState.ImageSize = new System.Drawing.Size(23, 23);
            this.exitButton.Image = global::SECURE_BYTE_GUI.Properties.Resources.multiply_24px;
            this.exitButton.ImageOffset = new System.Drawing.Point(0, 0);
            this.exitButton.ImageRotate = 0F;
            this.exitButton.ImageSize = new System.Drawing.Size(24, 24);
            this.exitButton.Location = new System.Drawing.Point(464, 12);
            this.exitButton.Name = "exitButton";
            this.exitButton.PressedState.ImageSize = new System.Drawing.Size(22, 22);
            this.exitButton.Size = new System.Drawing.Size(24, 24);
            this.exitButton.TabIndex = 7;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // msgLabel
            // 
            this.msgLabel.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.msgLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.msgLabel.Location = new System.Drawing.Point(133, 45);
            this.msgLabel.Name = "msgLabel";
            this.msgLabel.Size = new System.Drawing.Size(330, 90);
            this.msgLabel.TabIndex = 9;
            this.msgLabel.Text = "label1";
            this.msgLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.msgLabel.UseCompatibleTextRendering = true;
            // 
            // okButton
            // 
            this.okButton.Animated = true;
            this.okButton.BackColor = System.Drawing.Color.Transparent;
            this.okButton.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.okButton.BorderRadius = 4;
            this.okButton.BorderThickness = 1;
            this.okButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.okButton.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.okButton.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.okButton.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.okButton.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.okButton.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(6)))), ((int)(((byte)(7)))));
            this.okButton.Font = new System.Drawing.Font("Century Gothic", 8F);
            this.okButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.okButton.Location = new System.Drawing.Point(436, 144);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(52, 24);
            this.okButton.TabIndex = 37;
            this.okButton.Text = "Ok";
            this.okButton.UseTransparentBackground = true;
            this.okButton.Visible = false;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // customMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(6)))), ((int)(((byte)(7)))));
            this.ClientSize = new System.Drawing.Size(500, 180);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.msgLabel);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.guna2PictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 180);
            this.Name = "customMessage";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.customMessage_Load);
            this.Shown += new System.EventHandler(this.customMessage_Shown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.customMessage_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox1;
        private Guna.UI2.WinForms.Guna2ShadowForm guna2ShadowForm1;
        private Guna.UI2.WinForms.Guna2ImageButton exitButton;
        private System.Windows.Forms.Label msgLabel;
        private Guna.UI2.WinForms.Guna2Button okButton;
    }
}