//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Logvinenko Eugeniy (manuka)
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


namespace Skilful.Data
{
    partial class DataWorkShopWindow
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
            this.SelectDirDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.label5 = new System.Windows.Forms.Label();
            this.Extension = new System.Windows.Forms.RichTextBox();
            this.SelectDirBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.DirectoryName = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.DoneBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.DecimalDelimiter = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.FieldDelimiter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ExampleData = new System.Windows.Forms.ListView();
            this.SelectField = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tickerMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.periodMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.dateMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.DateFormatMnu = new System.Windows.Forms.ToolStripTextBox();
            this.timeMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.TimeFormatMnu = new System.Windows.Forms.ToolStripTextBox();
            this.openMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.highMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.lowMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.EnableWatching = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SelectField.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(633, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Suffix";
            // 
            // Extension
            // 
            this.Extension.Location = new System.Drawing.Point(672, 16);
            this.Extension.Name = "Extension";
            this.Extension.Size = new System.Drawing.Size(71, 21);
            this.Extension.TabIndex = 33;
            this.Extension.Text = ".TXT";
            this.Extension.TextChanged += new System.EventHandler(this.Extension_TextChanged);
            // 
            // SelectDirBtn
            // 
            this.SelectDirBtn.Location = new System.Drawing.Point(573, 14);
            this.SelectDirBtn.Name = "SelectDirBtn";
            this.SelectDirBtn.Size = new System.Drawing.Size(54, 23);
            this.SelectDirBtn.TabIndex = 32;
            this.SelectDirBtn.Text = "Select";
            this.SelectDirBtn.UseVisualStyleBackColor = true;
            this.SelectDirBtn.Click += new System.EventHandler(this.SelectDirBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Directory";
            // 
            // DirectoryName
            // 
            this.DirectoryName.Location = new System.Drawing.Point(80, 16);
            this.DirectoryName.Name = "DirectoryName";
            this.DirectoryName.Size = new System.Drawing.Size(487, 21);
            this.DirectoryName.TabIndex = 30;
            this.DirectoryName.Text = "";
            this.DirectoryName.WordWrap = false;
            this.DirectoryName.TextChanged += new System.EventHandler(this.DirectoryName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(279, 240);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 42;
            this.label1.Text = "Example";
            // 
            // DoneBtn
            // 
            this.DoneBtn.Location = new System.Drawing.Point(664, 271);
            this.DoneBtn.Name = "DoneBtn";
            this.DoneBtn.Size = new System.Drawing.Size(75, 23);
            this.DoneBtn.TabIndex = 44;
            this.DoneBtn.Text = "Done!";
            this.DoneBtn.UseVisualStyleBackColor = true;
            this.DoneBtn.Click += new System.EventHandler(this.DoneBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.DecimalDelimiter);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.FieldDelimiter);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(15, 43);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(204, 44);
            this.groupBox1.TabIndex = 49;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Delimiters";
            // 
            // DecimalDelimiter
            // 
            this.DecimalDelimiter.Location = new System.Drawing.Point(145, 18);
            this.DecimalDelimiter.Name = "DecimalDelimiter";
            this.DecimalDelimiter.Size = new System.Drawing.Size(18, 20);
            this.DecimalDelimiter.TabIndex = 3;
            this.DecimalDelimiter.Text = ".";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(92, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Decimal";
            // 
            // FieldDelimiter
            // 
            this.FieldDelimiter.Location = new System.Drawing.Point(59, 18);
            this.FieldDelimiter.Name = "FieldDelimiter";
            this.FieldDelimiter.Size = new System.Drawing.Size(18, 20);
            this.FieldDelimiter.TabIndex = 1;
            this.FieldDelimiter.Text = ",";
            this.FieldDelimiter.TextChanged += new System.EventHandler(this.FieldDelimiter_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Columns";
            // 
            // ExampleData
            // 
            this.ExampleData.GridLines = true;
            this.ExampleData.Location = new System.Drawing.Point(15, 107);
            this.ExampleData.MultiSelect = false;
            this.ExampleData.Name = "ExampleData";
            this.ExampleData.Size = new System.Drawing.Size(520, 187);
            this.ExampleData.TabIndex = 52;
            this.ExampleData.UseCompatibleStateImageBehavior = false;
            this.ExampleData.View = System.Windows.Forms.View.Details;
            this.ExampleData.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ExampleData_ColumnClick);
            // 
            // SelectField
            // 
            this.SelectField.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tickerMnu,
            this.periodMnu,
            this.dateMnu,
            this.timeMnu,
            this.openMnu,
            this.highMnu,
            this.lowMnu,
            this.closeMnu});
            this.SelectField.Name = "SelectField";
            this.SelectField.Size = new System.Drawing.Size(116, 180);
            this.SelectField.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.SelectField_ItemClicked);
            // 
            // tickerMnu
            // 
            this.tickerMnu.Name = "tickerMnu";
            this.tickerMnu.Size = new System.Drawing.Size(115, 22);
            this.tickerMnu.Text = "Ticker";
            // 
            // periodMnu
            // 
            this.periodMnu.Name = "periodMnu";
            this.periodMnu.Size = new System.Drawing.Size(115, 22);
            this.periodMnu.Text = "Period";
            // 
            // dateMnu
            // 
            this.dateMnu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DateFormatMnu});
            this.dateMnu.Name = "dateMnu";
            this.dateMnu.Size = new System.Drawing.Size(115, 22);
            this.dateMnu.Text = "Date";
            this.dateMnu.Click += new System.EventHandler(this.dateMnu_Click);
            // 
            // DateFormatMnu
            // 
            this.DateFormatMnu.Name = "DateFormatMnu";
            this.DateFormatMnu.Size = new System.Drawing.Size(100, 21);
            this.DateFormatMnu.Text = "yyyyMMdd";
            // 
            // timeMnu
            // 
            this.timeMnu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TimeFormatMnu});
            this.timeMnu.Name = "timeMnu";
            this.timeMnu.Size = new System.Drawing.Size(115, 22);
            this.timeMnu.Text = "Time";
            this.timeMnu.Click += new System.EventHandler(this.timeMnu_Click);
            // 
            // TimeFormatMnu
            // 
            this.TimeFormatMnu.Name = "TimeFormatMnu";
            this.TimeFormatMnu.Size = new System.Drawing.Size(100, 21);
            this.TimeFormatMnu.Text = "HH:mm";
            // 
            // openMnu
            // 
            this.openMnu.Name = "openMnu";
            this.openMnu.Size = new System.Drawing.Size(115, 22);
            this.openMnu.Text = "Open";
            // 
            // highMnu
            // 
            this.highMnu.Name = "highMnu";
            this.highMnu.Size = new System.Drawing.Size(115, 22);
            this.highMnu.Text = "High";
            // 
            // lowMnu
            // 
            this.lowMnu.Name = "lowMnu";
            this.lowMnu.Size = new System.Drawing.Size(115, 22);
            this.lowMnu.Text = "Low";
            // 
            // closeMnu
            // 
            this.closeMnu.Name = "closeMnu";
            this.closeMnu.Size = new System.Drawing.Size(115, 22);
            this.closeMnu.Text = "Close";
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(541, 175);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(201, 36);
            this.label16.TabIndex = 53;
            this.label16.Text = "Click on the title of a column and select a field from the list";
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(541, 222);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(198, 31);
            this.label17.TabIndex = 54;
            this.label17.Text = "You must define DATE, OPEN, HIGH and CLOSE fields, at least.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.EnableWatching);
            this.groupBox2.Location = new System.Drawing.Point(251, 43);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(284, 44);
            this.groupBox2.TabIndex = 55;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Directory watcher";
            // 
            // EnableWatching
            // 
            this.EnableWatching.AutoSize = true;
            this.EnableWatching.Location = new System.Drawing.Point(7, 18);
            this.EnableWatching.Name = "EnableWatching";
            this.EnableWatching.Size = new System.Drawing.Size(105, 17);
            this.EnableWatching.TabIndex = 0;
            this.EnableWatching.Text = "Enable watching";
            this.EnableWatching.UseVisualStyleBackColor = true;
            // 
            // DataWorkShopWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 306);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ExampleData);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.DoneBtn);
            this.Controls.Add(this.Extension);
            this.Controls.Add(this.SelectDirBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.DirectoryName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DataWorkShopWindow";
            this.Tag = "0";
            this.Text = "Data Preparation Window";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.SelectField.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog SelectDirDialog;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox Extension;
        private System.Windows.Forms.Button SelectDirBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox DirectoryName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button DoneBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox FieldDelimiter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox DecimalDelimiter;
        private System.Windows.Forms.ListView ExampleData;
        private System.Windows.Forms.ContextMenuStrip SelectField;
        private System.Windows.Forms.ToolStripMenuItem periodMnu;
        private System.Windows.Forms.ToolStripMenuItem dateMnu;
        private System.Windows.Forms.ToolStripMenuItem timeMnu;
        private System.Windows.Forms.ToolStripMenuItem openMnu;
        private System.Windows.Forms.ToolStripMenuItem highMnu;
        private System.Windows.Forms.ToolStripMenuItem lowMnu;
        private System.Windows.Forms.ToolStripMenuItem closeMnu;
        private System.Windows.Forms.ToolStripMenuItem tickerMnu;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ToolStripTextBox DateFormatMnu;
        private System.Windows.Forms.ToolStripTextBox TimeFormatMnu;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox EnableWatching;
    }
}

