//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Andrey Prokhorov(AVP)
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
    partial class OptionsForm
        {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
            {
            if (disposing && (components != null))
                {
                components.Dispose();
                }
            base.Dispose(disposing);
            }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
            {
                this.colorDialog = new System.Windows.Forms.ColorDialog();
                this.tabControl1 = new System.Windows.Forms.TabControl();
                this.ColorsPage = new System.Windows.Forms.TabPage();
                this.ModelLbl11 = new System.Windows.Forms.Label();
                this.ModelLbl10 = new System.Windows.Forms.Label();
                this.ModelLbl9 = new System.Windows.Forms.Label();
                this.ModelLbl8 = new System.Windows.Forms.Label();
                this.ModelLbl7 = new System.Windows.Forms.Label();
                this.ModelLbl6 = new System.Windows.Forms.Label();
                this.ModelLbl5 = new System.Windows.Forms.Label();
                this.ModelLbl4 = new System.Windows.Forms.Label();
                this.ModelLbl3 = new System.Windows.Forms.Label();
                this.ModelLbl2 = new System.Windows.Forms.Label();
                this.ModelLbl1 = new System.Windows.Forms.Label();
                this.RulesPage = new System.Windows.Forms.TabPage();
                this.panel2 = new System.Windows.Forms.Panel();
                this.RulesBox = new System.Windows.Forms.CheckedListBox();
                this.label7 = new System.Windows.Forms.Label();
                this.panel1 = new System.Windows.Forms.Panel();
                this.MDBSize2 = new System.Windows.Forms.NumericUpDown();
                this.MDBSize1 = new System.Windows.Forms.NumericUpDown();
                this.label10 = new System.Windows.Forms.Label();
                this.label9 = new System.Windows.Forms.Label();
                this.label8 = new System.Windows.Forms.Label();
                this.ExtremumSize5 = new System.Windows.Forms.NumericUpDown();
                this.label6 = new System.Windows.Forms.Label();
                this.ExtremumSize4 = new System.Windows.Forms.NumericUpDown();
                this.label5 = new System.Windows.Forms.Label();
                this.ExtremumSize3 = new System.Windows.Forms.NumericUpDown();
                this.label4 = new System.Windows.Forms.Label();
                this.ExtremumSize2 = new System.Windows.Forms.NumericUpDown();
                this.label3 = new System.Windows.Forms.Label();
                this.ExtremumSize1 = new System.Windows.Forms.NumericUpDown();
                this.label2 = new System.Windows.Forms.Label();
                this.label1 = new System.Windows.Forms.Label();
                this.tabControl1.SuspendLayout();
                this.ColorsPage.SuspendLayout();
                this.RulesPage.SuspendLayout();
                this.panel2.SuspendLayout();
                this.panel1.SuspendLayout();
                ((System.ComponentModel.ISupportInitialize)(this.MDBSize2)).BeginInit();
                ((System.ComponentModel.ISupportInitialize)(this.MDBSize1)).BeginInit();
                ((System.ComponentModel.ISupportInitialize)(this.ExtremumSize5)).BeginInit();
                ((System.ComponentModel.ISupportInitialize)(this.ExtremumSize4)).BeginInit();
                ((System.ComponentModel.ISupportInitialize)(this.ExtremumSize3)).BeginInit();
                ((System.ComponentModel.ISupportInitialize)(this.ExtremumSize2)).BeginInit();
                ((System.ComponentModel.ISupportInitialize)(this.ExtremumSize1)).BeginInit();
                this.SuspendLayout();
                // 
                // tabControl1
                // 
                this.tabControl1.Controls.Add(this.ColorsPage);
                this.tabControl1.Controls.Add(this.RulesPage);
                this.tabControl1.Location = new System.Drawing.Point(-3, 0);
                this.tabControl1.Name = "tabControl1";
                this.tabControl1.SelectedIndex = 0;
                this.tabControl1.Size = new System.Drawing.Size(339, 292);
                this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
                this.tabControl1.TabIndex = 0;
                // 
                // ColorsPage
                // 
                this.ColorsPage.Controls.Add(this.ModelLbl11);
                this.ColorsPage.Controls.Add(this.ModelLbl10);
                this.ColorsPage.Controls.Add(this.ModelLbl9);
                this.ColorsPage.Controls.Add(this.ModelLbl8);
                this.ColorsPage.Controls.Add(this.ModelLbl7);
                this.ColorsPage.Controls.Add(this.ModelLbl6);
                this.ColorsPage.Controls.Add(this.ModelLbl5);
                this.ColorsPage.Controls.Add(this.ModelLbl4);
                this.ColorsPage.Controls.Add(this.ModelLbl3);
                this.ColorsPage.Controls.Add(this.ModelLbl2);
                this.ColorsPage.Controls.Add(this.ModelLbl1);
                this.ColorsPage.Location = new System.Drawing.Point(4, 22);
                this.ColorsPage.Name = "ColorsPage";
                this.ColorsPage.Padding = new System.Windows.Forms.Padding(3);
                this.ColorsPage.Size = new System.Drawing.Size(331, 266);
                this.ColorsPage.TabIndex = 0;
                this.ColorsPage.Text = "Colors";
                this.ColorsPage.UseVisualStyleBackColor = true;
                // 
                // ModelLbl11
                // 
                this.ModelLbl11.AutoSize = true;
                this.ModelLbl11.Location = new System.Drawing.Point(12, 241);
                this.ModelLbl11.Name = "ModelLbl11";
                this.ModelLbl11.Size = new System.Drawing.Size(51, 13);
                this.ModelLbl11.TabIndex = 10;
                this.ModelLbl11.Text = "Model 11";
                this.ModelLbl11.Click += new System.EventHandler(this.ModelLbl1_Click);
                // 
                // ModelLbl10
                // 
                this.ModelLbl10.AutoSize = true;
                this.ModelLbl10.Location = new System.Drawing.Point(11, 216);
                this.ModelLbl10.Name = "ModelLbl10";
                this.ModelLbl10.Size = new System.Drawing.Size(51, 13);
                this.ModelLbl10.TabIndex = 9;
                this.ModelLbl10.Text = "Model 10";
                this.ModelLbl10.Click += new System.EventHandler(this.ModelLbl1_Click);
                // 
                // ModelLbl9
                // 
                this.ModelLbl9.AutoSize = true;
                this.ModelLbl9.Location = new System.Drawing.Point(11, 194);
                this.ModelLbl9.Name = "ModelLbl9";
                this.ModelLbl9.Size = new System.Drawing.Size(45, 13);
                this.ModelLbl9.TabIndex = 8;
                this.ModelLbl9.Text = "Model 9";
                this.ModelLbl9.Click += new System.EventHandler(this.ModelLbl1_Click);
                // 
                // ModelLbl8
                // 
                this.ModelLbl8.AutoSize = true;
                this.ModelLbl8.Location = new System.Drawing.Point(11, 171);
                this.ModelLbl8.Name = "ModelLbl8";
                this.ModelLbl8.Size = new System.Drawing.Size(45, 13);
                this.ModelLbl8.TabIndex = 7;
                this.ModelLbl8.Text = "Model 8";
                this.ModelLbl8.Click += new System.EventHandler(this.ModelLbl1_Click);
                // 
                // ModelLbl7
                // 
                this.ModelLbl7.AutoSize = true;
                this.ModelLbl7.Location = new System.Drawing.Point(11, 149);
                this.ModelLbl7.Name = "ModelLbl7";
                this.ModelLbl7.Size = new System.Drawing.Size(45, 13);
                this.ModelLbl7.TabIndex = 6;
                this.ModelLbl7.Text = "Model 7";
                this.ModelLbl7.Click += new System.EventHandler(this.ModelLbl1_Click);
                // 
                // ModelLbl6
                // 
                this.ModelLbl6.AutoSize = true;
                this.ModelLbl6.Location = new System.Drawing.Point(11, 126);
                this.ModelLbl6.Name = "ModelLbl6";
                this.ModelLbl6.Size = new System.Drawing.Size(45, 13);
                this.ModelLbl6.TabIndex = 5;
                this.ModelLbl6.Text = "Model 6";
                this.ModelLbl6.Click += new System.EventHandler(this.ModelLbl1_Click);
                // 
                // ModelLbl5
                // 
                this.ModelLbl5.AutoSize = true;
                this.ModelLbl5.Location = new System.Drawing.Point(11, 104);
                this.ModelLbl5.Name = "ModelLbl5";
                this.ModelLbl5.Size = new System.Drawing.Size(45, 13);
                this.ModelLbl5.TabIndex = 4;
                this.ModelLbl5.Text = "Model 5";
                this.ModelLbl5.Click += new System.EventHandler(this.ModelLbl1_Click);
                // 
                // ModelLbl4
                // 
                this.ModelLbl4.AutoSize = true;
                this.ModelLbl4.Location = new System.Drawing.Point(11, 81);
                this.ModelLbl4.Name = "ModelLbl4";
                this.ModelLbl4.Size = new System.Drawing.Size(45, 13);
                this.ModelLbl4.TabIndex = 3;
                this.ModelLbl4.Text = "Model 4";
                this.ModelLbl4.Click += new System.EventHandler(this.ModelLbl1_Click);
                // 
                // ModelLbl3
                // 
                this.ModelLbl3.AutoSize = true;
                this.ModelLbl3.Location = new System.Drawing.Point(11, 56);
                this.ModelLbl3.Name = "ModelLbl3";
                this.ModelLbl3.Size = new System.Drawing.Size(45, 13);
                this.ModelLbl3.TabIndex = 2;
                this.ModelLbl3.Text = "Model 3";
                this.ModelLbl3.Click += new System.EventHandler(this.ModelLbl1_Click);
                // 
                // ModelLbl2
                // 
                this.ModelLbl2.AutoSize = true;
                this.ModelLbl2.Location = new System.Drawing.Point(11, 29);
                this.ModelLbl2.Name = "ModelLbl2";
                this.ModelLbl2.Size = new System.Drawing.Size(45, 13);
                this.ModelLbl2.TabIndex = 1;
                this.ModelLbl2.Text = "Model 2";
                this.ModelLbl2.Click += new System.EventHandler(this.ModelLbl1_Click);
                // 
                // ModelLbl1
                // 
                this.ModelLbl1.AutoSize = true;
                this.ModelLbl1.Location = new System.Drawing.Point(12, 7);
                this.ModelLbl1.Name = "ModelLbl1";
                this.ModelLbl1.Size = new System.Drawing.Size(45, 13);
                this.ModelLbl1.TabIndex = 0;
                this.ModelLbl1.Text = "Model 1";
                this.ModelLbl1.Click += new System.EventHandler(this.ModelLbl1_Click);
                // 
                // RulesPage
                // 
                this.RulesPage.Controls.Add(this.panel2);
                this.RulesPage.Controls.Add(this.panel1);
                this.RulesPage.Location = new System.Drawing.Point(4, 22);
                this.RulesPage.Name = "RulesPage";
                this.RulesPage.Padding = new System.Windows.Forms.Padding(3);
                this.RulesPage.Size = new System.Drawing.Size(331, 266);
                this.RulesPage.TabIndex = 1;
                this.RulesPage.Text = "Rules";
                this.RulesPage.UseVisualStyleBackColor = true;
                // 
                // panel2
                // 
                this.panel2.Controls.Add(this.RulesBox);
                this.panel2.Controls.Add(this.label7);
                this.panel2.Location = new System.Drawing.Point(3, 129);
                this.panel2.Name = "panel2";
                this.panel2.Size = new System.Drawing.Size(325, 134);
                this.panel2.TabIndex = 1;
                // 
                // RulesBox
                // 
                this.RulesBox.CheckOnClick = true;
                this.RulesBox.FormattingEnabled = true;
                this.RulesBox.Items.AddRange(new object[] {
            "Extra points on trend line",
            "Extra points on target line",
            "Intersection of 1st and 3rd bar bodies for Atraction model",
            "Intersection of 2st and 4rd bar bodies for Atraction model",
            "Intersection of 2st and 5rd bar bodies",
            "Point 3 is not the global extremum",
            "Point 5 within the 4th point bar",
            "Point 3\' out of the base",
            "Target line and the 1st point bar intersection",
            "Superposition of model levels",
            "Point 3 is not the global extremun between p2 and p3",
            "Point 3 is not the global extremum for Atraction model",
            "Intersection of 2nd and 5th bar bodies for Atraction model",
            "Point 5 within the base"});
                this.RulesBox.Location = new System.Drawing.Point(15, 22);
                this.RulesBox.Name = "RulesBox";
                this.RulesBox.Size = new System.Drawing.Size(306, 109);
                this.RulesBox.TabIndex = 1;
                // 
                // label7
                // 
                this.label7.AutoSize = true;
                this.label7.Location = new System.Drawing.Point(15, 5);
                this.label7.Name = "label7";
                this.label7.Size = new System.Drawing.Size(75, 13);
                this.label7.TabIndex = 0;
                this.label7.Text = "Checked rules";
                // 
                // panel1
                // 
                this.panel1.Controls.Add(this.MDBSize2);
                this.panel1.Controls.Add(this.MDBSize1);
                this.panel1.Controls.Add(this.label10);
                this.panel1.Controls.Add(this.label9);
                this.panel1.Controls.Add(this.label8);
                this.panel1.Controls.Add(this.ExtremumSize5);
                this.panel1.Controls.Add(this.label6);
                this.panel1.Controls.Add(this.ExtremumSize4);
                this.panel1.Controls.Add(this.label5);
                this.panel1.Controls.Add(this.ExtremumSize3);
                this.panel1.Controls.Add(this.label4);
                this.panel1.Controls.Add(this.ExtremumSize2);
                this.panel1.Controls.Add(this.label3);
                this.panel1.Controls.Add(this.ExtremumSize1);
                this.panel1.Controls.Add(this.label2);
                this.panel1.Controls.Add(this.label1);
                this.panel1.Location = new System.Drawing.Point(3, 3);
                this.panel1.Name = "panel1";
                this.panel1.Size = new System.Drawing.Size(328, 124);
                this.panel1.TabIndex = 0;
                // 
                // MDBSize2
                // 
                this.MDBSize2.Location = new System.Drawing.Point(262, 38);
                this.MDBSize2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
                this.MDBSize2.Name = "MDBSize2";
                this.MDBSize2.Size = new System.Drawing.Size(57, 20);
                this.MDBSize2.TabIndex = 16;
                this.MDBSize2.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
                // 
                // MDBSize1
                // 
                this.MDBSize1.Location = new System.Drawing.Point(262, 19);
                this.MDBSize1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
                this.MDBSize1.Name = "MDBSize1";
                this.MDBSize1.Size = new System.Drawing.Size(57, 20);
                this.MDBSize1.TabIndex = 15;
                this.MDBSize1.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
                // 
                // label10
                // 
                this.label10.AutoSize = true;
                this.label10.Location = new System.Drawing.Point(159, 38);
                this.label10.Name = "label10";
                this.label10.Size = new System.Drawing.Size(97, 13);
                this.label10.TabIndex = 14;
                this.label10.Text = "Atraction->Balance";
                // 
                // label9
                // 
                this.label9.AutoSize = true;
                this.label9.Location = new System.Drawing.Point(159, 21);
                this.label9.Name = "label9";
                this.label9.Size = new System.Drawing.Size(104, 13);
                this.label9.TabIndex = 13;
                this.label9.Text = "Expansion->Balance";
                // 
                // label8
                // 
                this.label8.AutoSize = true;
                this.label8.Location = new System.Drawing.Point(179, 3);
                this.label8.Name = "label8";
                this.label8.Size = new System.Drawing.Size(106, 13);
                this.label8.TabIndex = 12;
                this.label8.Text = "Transformation coefs";
                // 
                // ExtremumSize5
                // 
                this.ExtremumSize5.Location = new System.Drawing.Point(53, 100);
                this.ExtremumSize5.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
                this.ExtremumSize5.Name = "ExtremumSize5";
                this.ExtremumSize5.Size = new System.Drawing.Size(57, 20);
                this.ExtremumSize5.TabIndex = 11;
                this.ExtremumSize5.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
                // 
                // label6
                // 
                this.label6.Location = new System.Drawing.Point(12, 102);
                this.label6.Name = "label6";
                this.label6.Size = new System.Drawing.Size(98, 18);
                this.label6.TabIndex = 10;
                this.label6.Text = "Point 5";
                // 
                // ExtremumSize4
                // 
                this.ExtremumSize4.Location = new System.Drawing.Point(53, 79);
                this.ExtremumSize4.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
                this.ExtremumSize4.Name = "ExtremumSize4";
                this.ExtremumSize4.Size = new System.Drawing.Size(57, 20);
                this.ExtremumSize4.TabIndex = 9;
                this.ExtremumSize4.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
                // 
                // label5
                // 
                this.label5.Location = new System.Drawing.Point(12, 81);
                this.label5.Name = "label5";
                this.label5.Size = new System.Drawing.Size(98, 18);
                this.label5.TabIndex = 8;
                this.label5.Text = "Point 4";
                // 
                // ExtremumSize3
                // 
                this.ExtremumSize3.Location = new System.Drawing.Point(53, 58);
                this.ExtremumSize3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
                this.ExtremumSize3.Name = "ExtremumSize3";
                this.ExtremumSize3.Size = new System.Drawing.Size(57, 20);
                this.ExtremumSize3.TabIndex = 7;
                this.ExtremumSize3.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
                // 
                // label4
                // 
                this.label4.Location = new System.Drawing.Point(12, 60);
                this.label4.Name = "label4";
                this.label4.Size = new System.Drawing.Size(98, 18);
                this.label4.TabIndex = 6;
                this.label4.Text = "Point 3";
                // 
                // ExtremumSize2
                // 
                this.ExtremumSize2.Location = new System.Drawing.Point(53, 37);
                this.ExtremumSize2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
                this.ExtremumSize2.Name = "ExtremumSize2";
                this.ExtremumSize2.Size = new System.Drawing.Size(57, 20);
                this.ExtremumSize2.TabIndex = 5;
                this.ExtremumSize2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
                // 
                // label3
                // 
                this.label3.Location = new System.Drawing.Point(12, 39);
                this.label3.Name = "label3";
                this.label3.Size = new System.Drawing.Size(98, 18);
                this.label3.TabIndex = 4;
                this.label3.Text = "Point 2";
                // 
                // ExtremumSize1
                // 
                this.ExtremumSize1.Location = new System.Drawing.Point(53, 19);
                this.ExtremumSize1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
                this.ExtremumSize1.Name = "ExtremumSize1";
                this.ExtremumSize1.Size = new System.Drawing.Size(57, 20);
                this.ExtremumSize1.TabIndex = 3;
                this.ExtremumSize1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
                // 
                // label2
                // 
                this.label2.Location = new System.Drawing.Point(12, 21);
                this.label2.Name = "label2";
                this.label2.Size = new System.Drawing.Size(98, 18);
                this.label2.TabIndex = 1;
                this.label2.Text = "Point 1";
                // 
                // label1
                // 
                this.label1.AutoSize = true;
                this.label1.Location = new System.Drawing.Point(9, 4);
                this.label1.Name = "label1";
                this.label1.Size = new System.Drawing.Size(125, 13);
                this.label1.TabIndex = 0;
                this.label1.Text = "Extremum sizes for points";
                // 
                // OptionsForm
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(337, 293);
                this.Controls.Add(this.tabControl1);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "OptionsForm";
                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                this.Text = "Options";
                this.Shown += new System.EventHandler(this.OptionsForm_Shown);
                this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OptionsForm_FormClosing);
                this.tabControl1.ResumeLayout(false);
                this.ColorsPage.ResumeLayout(false);
                this.ColorsPage.PerformLayout();
                this.RulesPage.ResumeLayout(false);
                this.panel2.ResumeLayout(false);
                this.panel2.PerformLayout();
                this.panel1.ResumeLayout(false);
                this.panel1.PerformLayout();
                ((System.ComponentModel.ISupportInitialize)(this.MDBSize2)).EndInit();
                ((System.ComponentModel.ISupportInitialize)(this.MDBSize1)).EndInit();
                ((System.ComponentModel.ISupportInitialize)(this.ExtremumSize5)).EndInit();
                ((System.ComponentModel.ISupportInitialize)(this.ExtremumSize4)).EndInit();
                ((System.ComponentModel.ISupportInitialize)(this.ExtremumSize3)).EndInit();
                ((System.ComponentModel.ISupportInitialize)(this.ExtremumSize2)).EndInit();
                ((System.ComponentModel.ISupportInitialize)(this.ExtremumSize1)).EndInit();
                this.ResumeLayout(false);

            }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage ColorsPage;
        private System.Windows.Forms.TabPage RulesPage;
        private System.Windows.Forms.Label ModelLbl10;
        private System.Windows.Forms.Label ModelLbl9;
        private System.Windows.Forms.Label ModelLbl8;
        private System.Windows.Forms.Label ModelLbl7;
        private System.Windows.Forms.Label ModelLbl6;
        private System.Windows.Forms.Label ModelLbl5;
        private System.Windows.Forms.Label ModelLbl4;
        private System.Windows.Forms.Label ModelLbl3;
        private System.Windows.Forms.Label ModelLbl2;
        private System.Windows.Forms.Label ModelLbl1;
        private System.Windows.Forms.Label ModelLbl11;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown ExtremumSize4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown ExtremumSize3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown ExtremumSize2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown ExtremumSize1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown ExtremumSize5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckedListBox RulesBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown MDBSize2;
        private System.Windows.Forms.NumericUpDown MDBSize1;
        private System.Windows.Forms.Label label10;
        }
    }