//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Alex Kokomov(Loylick)
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
//
// mail: support@protoforma.com 
//---------------------------------------------------------------------------------------------------------------


namespace Skilful
{
    partial class Settings
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
            this.label1 = new System.Windows.Forms.Label();
            this.setFrame1 = new System.Windows.Forms.TextBox();
            this.setFrame2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.setFrame3 = new System.Windows.Forms.TextBox();
            this.setFrame4 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.setRepersMin = new System.Windows.Forms.TextBox();
            this.seRepersMax = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.set7b = new System.Windows.Forms.TextBox();
            this.set3b = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonSettingsOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(116, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "1";
            // 
            // setFrame1
            // 
            this.setFrame1.Location = new System.Drawing.Point(138, 35);
            this.setFrame1.Name = "setFrame1";
            this.setFrame1.Size = new System.Drawing.Size(38, 22);
            this.setFrame1.TabIndex = 1;
            this.setFrame1.Text = "20";
            // 
            // setFrame2
            // 
            this.setFrame2.Location = new System.Drawing.Point(227, 35);
            this.setFrame2.Name = "setFrame2";
            this.setFrame2.Size = new System.Drawing.Size(36, 22);
            this.setFrame2.TabIndex = 2;
            this.setFrame2.Text = "50";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(206, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "2";
            // 
            // setFrame3
            // 
            this.setFrame3.Location = new System.Drawing.Point(138, 82);
            this.setFrame3.Name = "setFrame3";
            this.setFrame3.Size = new System.Drawing.Size(38, 22);
            this.setFrame3.TabIndex = 4;
            this.setFrame3.Text = "100";
            // 
            // setFrame4
            // 
            this.setFrame4.Location = new System.Drawing.Point(228, 81);
            this.setFrame4.Name = "setFrame4";
            this.setFrame4.Size = new System.Drawing.Size(36, 22);
            this.setFrame4.TabIndex = 5;
            this.setFrame4.Text = "200";
            this.setFrame4.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(116, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(206, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "4";
            // 
            // setRepersMin
            // 
            this.setRepersMin.Location = new System.Drawing.Point(138, 125);
            this.setRepersMin.Name = "setRepersMin";
            this.setRepersMin.Size = new System.Drawing.Size(38, 22);
            this.setRepersMin.TabIndex = 8;
            this.setRepersMin.Text = "-1";
            this.setRepersMin.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // seRepersMax
            // 
            this.seRepersMax.Location = new System.Drawing.Point(228, 124);
            this.seRepersMax.Name = "seRepersMax";
            this.seRepersMax.Size = new System.Drawing.Size(35, 22);
            this.seRepersMax.TabIndex = 9;
            this.seRepersMax.Text = "1";
            this.seRepersMax.TextChanged += new System.EventHandler(this.textBox6_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(92, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "min";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(176, 129);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "max";
            // 
            // set7b
            // 
            this.set7b.Location = new System.Drawing.Point(43, 35);
            this.set7b.Name = "set7b";
            this.set7b.Size = new System.Drawing.Size(37, 22);
            this.set7b.TabIndex = 12;
            this.set7b.Text = "3";
            this.set7b.TextChanged += new System.EventHandler(this.textBox7_TextChanged);
            // 
            // set3b
            // 
            this.set3b.Location = new System.Drawing.Point(43, 81);
            this.set3b.Name = "set3b";
            this.set3b.Size = new System.Drawing.Size(37, 22);
            this.set3b.TabIndex = 13;
            this.set3b.Text = "1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 17);
            this.label7.TabIndex = 14;
            this.label7.Text = "7b";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 87);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 17);
            this.label8.TabIndex = 15;
            this.label8.Text = "3b";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(135, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(135, 17);
            this.label9.TabIndex = 16;
            this.label9.Text = "Mesh Frames points";
            // 
            // buttonSettingsOk
            // 
            this.buttonSettingsOk.Location = new System.Drawing.Point(101, 162);
            this.buttonSettingsOk.Name = "buttonSettingsOk";
            this.buttonSettingsOk.Size = new System.Drawing.Size(75, 23);
            this.buttonSettingsOk.TabIndex = 17;
            this.buttonSettingsOk.Text = "Ok";
            this.buttonSettingsOk.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(291, 210);
            this.Controls.Add(this.buttonSettingsOk);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.set3b);
            this.Controls.Add(this.set7b);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.seRepersMax);
            this.Controls.Add(this.setRepersMin);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.setFrame4);
            this.Controls.Add(this.setFrame3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.setFrame2);
            this.Controls.Add(this.setFrame1);
            this.Controls.Add(this.label1);
            this.Name = "Settings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox setFrame1;
        private System.Windows.Forms.TextBox setFrame2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox setFrame3;
        private System.Windows.Forms.TextBox setFrame4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox setRepersMin;
        private System.Windows.Forms.TextBox seRepersMax;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox set7b;
        private System.Windows.Forms.TextBox set3b;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonSettingsOk;
    }
}