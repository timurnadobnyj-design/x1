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


namespace ChartV2
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ShowTextButton = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.GoChartButton = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.ColorPickerBox = new System.Windows.Forms.ComboBox();
            this.SeriesPeriodTextBox = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.SeriesTick = new System.Windows.Forms.TextBox();
            this.NumberOfRows2 = new System.Windows.Forms.TextBox();
            this.FieldOrderBox = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SaveTemplateButton = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.OpenTemplateButton = new System.Windows.Forms.Button();
            this.AddFilterField = new System.Windows.Forms.Button();
            this.AddIoButton = new System.Windows.Forms.Button();
            this.MoveDownButton = new System.Windows.Forms.Button();
            this.MoveUpButton = new System.Windows.Forms.Button();
            this.DeleteFieldButton = new System.Windows.Forms.Button();
            this.FieldOrder = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.TimeFormatBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.DateFormatBox = new System.Windows.Forms.TextBox();
            this.DecimalSepBox = new System.Windows.Forms.ComboBox();
            this.ColsSeparatorBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.RowsSeparatorBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ToHeadersButton = new System.Windows.Forms.Button();
            this.ArrangeToGrid = new System.Windows.Forms.Button();
            this.NumberOfRows = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.NumberOfCols = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.openTemplatesDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.FieldOrderBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumberOfRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumberOfCols)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(2, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ShowTextButton);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.richTextBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(747, 473);
            this.splitContainer1.SplitterDistance = 28;
            this.splitContainer1.TabIndex = 1;
            // 
            // ShowTextButton
            // 
            this.ShowTextButton.AutoCheck = false;
            this.ShowTextButton.AutoSize = true;
            this.ShowTextButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.ShowTextButton.Location = new System.Drawing.Point(634, 0);
            this.ShowTextButton.Name = "ShowTextButton";
            this.ShowTextButton.Size = new System.Drawing.Size(113, 28);
            this.ShowTextButton.TabIndex = 26;
            this.ShowTextButton.TabStop = true;
            this.ShowTextButton.Text = "Extend output field";
            this.ShowTextButton.UseVisualStyleBackColor = true;
            this.ShowTextButton.Click += new System.EventHandler(this.ShowTextButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "File output:";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(71, 4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(554, 22);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer2.Panel1.Controls.Add(this.NumberOfRows2);
            this.splitContainer2.Panel1.Controls.Add(this.FieldOrderBox);
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dataGrid);
            this.splitContainer2.Size = new System.Drawing.Size(747, 441);
            this.splitContainer2.SplitterDistance = 197;
            this.splitContainer2.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.GoChartButton);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.ColorPickerBox);
            this.groupBox2.Controls.Add(this.SeriesPeriodTextBox);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.SeriesTick);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(524, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(213, 180);
            this.groupBox2.TabIndex = 37;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // GoChartButton
            // 
            this.GoChartButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.GoChartButton.Location = new System.Drawing.Point(6, 110);
            this.GoChartButton.Name = "GoChartButton";
            this.GoChartButton.Size = new System.Drawing.Size(201, 63);
            this.GoChartButton.TabIndex = 13;
            this.GoChartButton.Text = "Go Chart";
            this.GoChartButton.UseVisualStyleBackColor = true;
            this.GoChartButton.Click += new System.EventHandler(this.GoChartButton_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 78);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 13);
            this.label13.TabIndex = 35;
            this.label13.Text = "Serie\'s period";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 15);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 13);
            this.label11.TabIndex = 30;
            this.label11.Text = "Series Pen";
            // 
            // ColorPickerBox
            // 
            this.ColorPickerBox.FormattingEnabled = true;
            this.ColorPickerBox.Location = new System.Drawing.Point(75, 12);
            this.ColorPickerBox.Name = "ColorPickerBox";
            this.ColorPickerBox.Size = new System.Drawing.Size(82, 21);
            this.ColorPickerBox.TabIndex = 31;
            this.ColorPickerBox.Text = "Color";
            this.ColorPickerBox.SelectedValueChanged += new System.EventHandler(this.ColorPickerBox_SelectedValueChanged);
            // 
            // SeriesPeriodTextBox
            // 
            this.SeriesPeriodTextBox.Location = new System.Drawing.Point(75, 76);
            this.SeriesPeriodTextBox.Name = "SeriesPeriodTextBox";
            this.SeriesPeriodTextBox.Size = new System.Drawing.Size(58, 20);
            this.SeriesPeriodTextBox.TabIndex = 36;
            this.SeriesPeriodTextBox.Text = "1m";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(75, 34);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(82, 20);
            this.textBox1.TabIndex = 32;
            this.textBox1.Text = "Tick";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Serie\'s tick";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 34);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 13);
            this.label10.TabIndex = 33;
            this.label10.Text = "Series Name";
            // 
            // SeriesTick
            // 
            this.SeriesTick.Location = new System.Drawing.Point(75, 55);
            this.SeriesTick.Name = "SeriesTick";
            this.SeriesTick.Size = new System.Drawing.Size(58, 20);
            this.SeriesTick.TabIndex = 34;
            this.SeriesTick.Text = "5";
            // 
            // NumberOfRows2
            // 
            this.NumberOfRows2.Location = new System.Drawing.Point(303, 165);
            this.NumberOfRows2.Name = "NumberOfRows2";
            this.NumberOfRows2.Size = new System.Drawing.Size(147, 20);
            this.NumberOfRows2.TabIndex = 33;
            this.NumberOfRows2.Text = "number of lines";
            // 
            // FieldOrderBox
            // 
            this.FieldOrderBox.Controls.Add(this.button1);
            this.FieldOrderBox.Controls.Add(this.SaveTemplateButton);
            this.FieldOrderBox.Controls.Add(this.label12);
            this.FieldOrderBox.Controls.Add(this.OpenTemplateButton);
            this.FieldOrderBox.Controls.Add(this.AddFilterField);
            this.FieldOrderBox.Controls.Add(this.AddIoButton);
            this.FieldOrderBox.Controls.Add(this.MoveDownButton);
            this.FieldOrderBox.Controls.Add(this.MoveUpButton);
            this.FieldOrderBox.Controls.Add(this.DeleteFieldButton);
            this.FieldOrderBox.Controls.Add(this.FieldOrder);
            this.FieldOrderBox.Enabled = false;
            this.FieldOrderBox.Location = new System.Drawing.Point(303, 0);
            this.FieldOrderBox.Name = "FieldOrderBox";
            this.FieldOrderBox.Size = new System.Drawing.Size(215, 160);
            this.FieldOrderBox.TabIndex = 25;
            this.FieldOrderBox.TabStop = false;
            this.FieldOrderBox.Text = "Field Order";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(73, 101);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(136, 23);
            this.button1.TabIndex = 34;
            this.button1.Text = "Delete only lable";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SaveTemplateButton
            // 
            this.SaveTemplateButton.Location = new System.Drawing.Point(73, 47);
            this.SaveTemplateButton.Name = "SaveTemplateButton";
            this.SaveTemplateButton.Size = new System.Drawing.Size(92, 22);
            this.SaveTemplateButton.TabIndex = 33;
            this.SaveTemplateButton.Text = "Save Template";
            this.SaveTemplateButton.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(111, 47);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(0, 13);
            this.label12.TabIndex = 32;
            // 
            // OpenTemplateButton
            // 
            this.OpenTemplateButton.Location = new System.Drawing.Point(73, 19);
            this.OpenTemplateButton.Name = "OpenTemplateButton";
            this.OpenTemplateButton.Size = new System.Drawing.Size(92, 22);
            this.OpenTemplateButton.TabIndex = 31;
            this.OpenTemplateButton.Text = "Open Template";
            this.OpenTemplateButton.UseVisualStyleBackColor = true;
            this.OpenTemplateButton.Click += new System.EventHandler(this.OpenTemplateButton_Click);
            // 
            // AddFilterField
            // 
            this.AddFilterField.Location = new System.Drawing.Point(162, 130);
            this.AddFilterField.Name = "AddFilterField";
            this.AddFilterField.Size = new System.Drawing.Size(48, 23);
            this.AddFilterField.TabIndex = 30;
            this.AddFilterField.Text = "Filter";
            this.AddFilterField.UseVisualStyleBackColor = true;
            this.AddFilterField.Click += new System.EventHandler(this.AddFilterField_Click);
            // 
            // AddIoButton
            // 
            this.AddIoButton.Location = new System.Drawing.Point(73, 131);
            this.AddIoButton.Name = "AddIoButton";
            this.AddIoButton.Size = new System.Drawing.Size(48, 23);
            this.AddIoButton.TabIndex = 29;
            this.AddIoButton.Text = "Add IO";
            this.AddIoButton.UseVisualStyleBackColor = true;
            this.AddIoButton.Click += new System.EventHandler(this.AddIoButton_Click);
            // 
            // MoveDownButton
            // 
            this.MoveDownButton.Location = new System.Drawing.Point(167, 18);
            this.MoveDownButton.Name = "MoveDownButton";
            this.MoveDownButton.Size = new System.Drawing.Size(46, 23);
            this.MoveDownButton.TabIndex = 27;
            this.MoveDownButton.Text = "Down";
            this.MoveDownButton.UseVisualStyleBackColor = true;
            this.MoveDownButton.Click += new System.EventHandler(this.MoveDownButton_Click);
            // 
            // MoveUpButton
            // 
            this.MoveUpButton.Location = new System.Drawing.Point(167, 46);
            this.MoveUpButton.Name = "MoveUpButton";
            this.MoveUpButton.Size = new System.Drawing.Size(46, 23);
            this.MoveUpButton.TabIndex = 26;
            this.MoveUpButton.Text = "Up";
            this.MoveUpButton.UseVisualStyleBackColor = true;
            this.MoveUpButton.Click += new System.EventHandler(this.MoveUpButton_Click);
            // 
            // DeleteFieldButton
            // 
            this.DeleteFieldButton.Location = new System.Drawing.Point(73, 75);
            this.DeleteFieldButton.Name = "DeleteFieldButton";
            this.DeleteFieldButton.Size = new System.Drawing.Size(136, 23);
            this.DeleteFieldButton.TabIndex = 25;
            this.DeleteFieldButton.Text = "Delete lable as column";
            this.DeleteFieldButton.UseVisualStyleBackColor = true;
            this.DeleteFieldButton.Click += new System.EventHandler(this.DeleteFieldButton_Click);
            // 
            // FieldOrder
            // 
            this.FieldOrder.FormattingEnabled = true;
            this.FieldOrder.Items.AddRange(new object[] {
            "Date",
            "Time",
            "Open",
            "High",
            "Low",
            "Close",
            "Volume"});
            this.FieldOrder.Location = new System.Drawing.Point(6, 19);
            this.FieldOrder.Name = "FieldOrder";
            this.FieldOrder.Size = new System.Drawing.Size(61, 134);
            this.FieldOrder.TabIndex = 24;
            this.FieldOrder.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.FieldOrder_ControlAdded);
            this.FieldOrder.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.FieldOrder_ControlAdded);
            this.FieldOrder.Click += new System.EventHandler(this.FieldOrder_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SaveButton);
            this.groupBox1.Controls.Add(this.TimeFormatBox);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.DateFormatBox);
            this.groupBox1.Controls.Add(this.DecimalSepBox);
            this.groupBox1.Controls.Add(this.ColsSeparatorBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.RowsSeparatorBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.ToHeadersButton);
            this.groupBox1.Controls.Add(this.ArrangeToGrid);
            this.groupBox1.Controls.Add(this.NumberOfRows);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.NumberOfCols);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(3, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(294, 185);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Separators for";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(163, 146);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(123, 23);
            this.SaveButton.TabIndex = 34;
            this.SaveButton.Text = "Save grid to .txt";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // TimeFormatBox
            // 
            this.TimeFormatBox.Location = new System.Drawing.Point(100, 147);
            this.TimeFormatBox.Name = "TimeFormatBox";
            this.TimeFormatBox.Size = new System.Drawing.Size(55, 20);
            this.TimeFormatBox.TabIndex = 31;
            this.TimeFormatBox.Text = "HHmmss";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 150);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 13);
            this.label9.TabIndex = 30;
            this.label9.Text = "Time Format";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 129);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 29;
            this.label8.Text = "Date Format";
            // 
            // DateFormatBox
            // 
            this.DateFormatBox.Location = new System.Drawing.Point(100, 125);
            this.DateFormatBox.Name = "DateFormatBox";
            this.DateFormatBox.Size = new System.Drawing.Size(55, 20);
            this.DateFormatBox.TabIndex = 28;
            this.DateFormatBox.Text = "yyyyMMdd";
            // 
            // DecimalSepBox
            // 
            this.DecimalSepBox.FormattingEnabled = true;
            this.DecimalSepBox.Items.AddRange(new object[] {
            ",",
            ".",
            "space"});
            this.DecimalSepBox.Location = new System.Drawing.Point(100, 102);
            this.DecimalSepBox.Name = "DecimalSepBox";
            this.DecimalSepBox.Size = new System.Drawing.Size(55, 21);
            this.DecimalSepBox.TabIndex = 27;
            this.DecimalSepBox.Text = ".";
            this.DecimalSepBox.SelectedValueChanged += new System.EventHandler(this.DecimalSepBox_SelectedValueChanged);
            // 
            // ColsSeparatorBox
            // 
            this.ColsSeparatorBox.FormattingEnabled = true;
            this.ColsSeparatorBox.Location = new System.Drawing.Point(100, 79);
            this.ColsSeparatorBox.Name = "ColsSeparatorBox";
            this.ColsSeparatorBox.Size = new System.Drawing.Size(55, 21);
            this.ColsSeparatorBox.TabIndex = 8;
            this.ColsSeparatorBox.Text = ",";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Decimal separator";
            // 
            // RowsSeparatorBox
            // 
            this.RowsSeparatorBox.FormattingEnabled = true;
            this.RowsSeparatorBox.Location = new System.Drawing.Point(100, 56);
            this.RowsSeparatorBox.Name = "RowsSeparatorBox";
            this.RowsSeparatorBox.Size = new System.Drawing.Size(55, 21);
            this.RowsSeparatorBox.TabIndex = 7;
            this.RowsSeparatorBox.Text = "Enter";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Number of Colums";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(86, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Number of Rows";
            // 
            // ToHeadersButton
            // 
            this.ToHeadersButton.Location = new System.Drawing.Point(163, 36);
            this.ToHeadersButton.Name = "ToHeadersButton";
            this.ToHeadersButton.Size = new System.Drawing.Size(125, 21);
            this.ToHeadersButton.TabIndex = 11;
            this.ToHeadersButton.Text = "first row to headers";
            this.ToHeadersButton.UseVisualStyleBackColor = true;
            this.ToHeadersButton.Click += new System.EventHandler(this.ToHeadersButton_Click);
            // 
            // ArrangeToGrid
            // 
            this.ArrangeToGrid.Location = new System.Drawing.Point(163, 13);
            this.ArrangeToGrid.Name = "ArrangeToGrid";
            this.ArrangeToGrid.Size = new System.Drawing.Size(125, 20);
            this.ArrangeToGrid.TabIndex = 7;
            this.ArrangeToGrid.Text = "Populate Grid";
            this.ArrangeToGrid.UseVisualStyleBackColor = true;
            this.ArrangeToGrid.Click += new System.EventHandler(this.ArrangeToGrid_Click);
            // 
            // NumberOfRows
            // 
            this.NumberOfRows.Location = new System.Drawing.Point(100, 34);
            this.NumberOfRows.Maximum = new decimal(new int[] {
            65000,
            0,
            0,
            0});
            this.NumberOfRows.Name = "NumberOfRows";
            this.NumberOfRows.Size = new System.Drawing.Size(55, 20);
            this.NumberOfRows.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Colums separator";
            // 
            // NumberOfCols
            // 
            this.NumberOfCols.Location = new System.Drawing.Point(105, 13);
            this.NumberOfCols.Name = "NumberOfCols";
            this.NumberOfCols.Size = new System.Drawing.Size(50, 20);
            this.NumberOfCols.TabIndex = 7;
            this.NumberOfCols.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Rows separator";
            // 
            // dataGrid
            // 
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid.Location = new System.Drawing.Point(0, 0);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.Size = new System.Drawing.Size(747, 240);
            this.dataGrid.TabIndex = 0;
            // 
            // openTemplatesDialog
            // 
            this.openTemplatesDialog.FileName = "template name";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.InitialDirectory = "C:\\";
            // 
            // DataWorkShopWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 478);
            this.Controls.Add(this.splitContainer1);
            this.Name = "DataWorkShopWindow";
            this.Tag = "0";
            this.Text = "Data Preparation Window";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.FieldOrderBox.ResumeLayout(false);
            this.FieldOrderBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumberOfRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumberOfCols)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        public System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button ArrangeToGrid;
        private System.Windows.Forms.ComboBox RowsSeparatorBox;
        private System.Windows.Forms.ComboBox ColsSeparatorBox;
        private System.Windows.Forms.NumericUpDown NumberOfRows;
        private System.Windows.Forms.NumericUpDown NumberOfCols;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button GoChartButton;
        private System.Windows.Forms.Button ToHeadersButton;
        private System.Windows.Forms.GroupBox FieldOrderBox;
        private System.Windows.Forms.ListBox FieldOrder;
        private System.Windows.Forms.RadioButton ShowTextButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button MoveDownButton;
        private System.Windows.Forms.Button MoveUpButton;
        private System.Windows.Forms.Button DeleteFieldButton;
        private System.Windows.Forms.ComboBox DecimalSepBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button AddFilterField;
        private System.Windows.Forms.Button AddIoButton;
        private System.Windows.Forms.Button OpenTemplateButton;
        private System.Windows.Forms.TextBox DateFormatBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TimeFormatBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox SeriesTick;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox ColorPickerBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button SaveTemplateButton;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.OpenFileDialog openTemplatesDialog;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox SeriesPeriodTextBox;
        private System.Windows.Forms.TextBox NumberOfRows2;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
    }
}

