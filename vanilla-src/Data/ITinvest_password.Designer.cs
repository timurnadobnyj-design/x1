namespace Skilful.Data
{
    partial class ITinvest_password
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
            this.IPServerGroupBox = new System.Windows.Forms.GroupBox();
            this.IPTextBox = new System.Windows.Forms.ComboBox();
            this.PasswordGroupBox = new System.Windows.Forms.GroupBox();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.LoginGroupBox = new System.Windows.Forms.GroupBox();
            this.LoginTextBox = new System.Windows.Forms.TextBox();
            this.button_OK = new System.Windows.Forms.Button();
            this.IPServerGroupBox.SuspendLayout();
            this.PasswordGroupBox.SuspendLayout();
            this.LoginGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // IPServerGroupBox
            // 
            this.IPServerGroupBox.Controls.Add(this.IPTextBox);
            this.IPServerGroupBox.Location = new System.Drawing.Point(56, 14);
            this.IPServerGroupBox.Name = "IPServerGroupBox";
            this.IPServerGroupBox.Size = new System.Drawing.Size(109, 39);
            this.IPServerGroupBox.TabIndex = 17;
            this.IPServerGroupBox.TabStop = false;
            this.IPServerGroupBox.Text = "IP Server";
            // 
            // IPTextBox
            // 
            this.IPTextBox.FormattingEnabled = true;
            this.IPTextBox.Items.AddRange(new object[] {
            "89.175.35.229",
            "82.204.220.34",
            "213.247.232.236"});
            this.IPTextBox.Location = new System.Drawing.Point(3, 16);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new System.Drawing.Size(103, 21);
            this.IPTextBox.TabIndex = 0;
            this.IPTextBox.Text = "89.175.35.229";
            // 
            // PasswordGroupBox
            // 
            this.PasswordGroupBox.Controls.Add(this.PasswordTextBox);
            this.PasswordGroupBox.Location = new System.Drawing.Point(56, 104);
            this.PasswordGroupBox.Name = "PasswordGroupBox";
            this.PasswordGroupBox.Size = new System.Drawing.Size(109, 39);
            this.PasswordGroupBox.TabIndex = 16;
            this.PasswordGroupBox.TabStop = false;
            this.PasswordGroupBox.Text = "Password";
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PasswordTextBox.Location = new System.Drawing.Point(3, 16);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.PasswordChar = '*';
            this.PasswordTextBox.Size = new System.Drawing.Size(103, 20);
            this.PasswordTextBox.TabIndex = 5;
            // 
            // LoginGroupBox
            // 
            this.LoginGroupBox.Controls.Add(this.LoginTextBox);
            this.LoginGroupBox.Location = new System.Drawing.Point(56, 59);
            this.LoginGroupBox.Name = "LoginGroupBox";
            this.LoginGroupBox.Size = new System.Drawing.Size(109, 39);
            this.LoginGroupBox.TabIndex = 15;
            this.LoginGroupBox.TabStop = false;
            this.LoginGroupBox.Text = "Login";
            // 
            // LoginTextBox
            // 
            this.LoginTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoginTextBox.Location = new System.Drawing.Point(3, 16);
            this.LoginTextBox.Name = "LoginTextBox";
            this.LoginTextBox.Size = new System.Drawing.Size(103, 20);
            this.LoginTextBox.TabIndex = 5;
            // 
            // button_OK
            // 
            this.button_OK.Location = new System.Drawing.Point(73, 163);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(75, 23);
            this.button_OK.TabIndex = 18;
            this.button_OK.Text = "Connect";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // ITinvest_password
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 201);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.IPServerGroupBox);
            this.Controls.Add(this.PasswordGroupBox);
            this.Controls.Add(this.LoginGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ITinvest_password";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ITinvest";
            this.Shown += new System.EventHandler(this.ITinvest_password_Shown);
            this.IPServerGroupBox.ResumeLayout(false);
            this.PasswordGroupBox.ResumeLayout(false);
            this.PasswordGroupBox.PerformLayout();
            this.LoginGroupBox.ResumeLayout(false);
            this.LoginGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox IPServerGroupBox;
        private System.Windows.Forms.GroupBox PasswordGroupBox;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.GroupBox LoginGroupBox;
        private System.Windows.Forms.TextBox LoginTextBox;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.ComboBox IPTextBox;
    }
}