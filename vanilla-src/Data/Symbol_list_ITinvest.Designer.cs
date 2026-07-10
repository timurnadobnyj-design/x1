namespace Skilful.Data
{
    partial class Symbol_list_ITinvest
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
            this.groupBoxMICEX = new System.Windows.Forms.GroupBox();
            this.checkedListBoxMICEX = new System.Windows.Forms.CheckedListBox();
            this.groupBoxRTS = new System.Windows.Forms.GroupBox();
            this.checkedListBoxRTS = new System.Windows.Forms.CheckedListBox();
            this.groupBoxINDEX = new System.Windows.Forms.GroupBox();
            this.checkedListBoxINDEX = new System.Windows.Forms.CheckedListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxTimePeriod = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxQuantityBars = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBoxSaveorUpdate = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBoxMICEX.SuspendLayout();
            this.groupBoxRTS.SuspendLayout();
            this.groupBoxINDEX.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxMICEX
            // 
            this.groupBoxMICEX.Controls.Add(this.checkedListBoxMICEX);
            this.groupBoxMICEX.Location = new System.Drawing.Point(61, 12);
            this.groupBoxMICEX.Name = "groupBoxMICEX";
            this.groupBoxMICEX.Size = new System.Drawing.Size(242, 424);
            this.groupBoxMICEX.TabIndex = 0;
            this.groupBoxMICEX.TabStop = false;
            this.groupBoxMICEX.Text = "ММВБ";
            // 
            // checkedListBoxMICEX
            // 
            this.checkedListBoxMICEX.CheckOnClick = true;
            this.checkedListBoxMICEX.FormattingEnabled = true;
            this.checkedListBoxMICEX.Location = new System.Drawing.Point(19, 27);
            this.checkedListBoxMICEX.Name = "checkedListBoxMICEX";
            this.checkedListBoxMICEX.Size = new System.Drawing.Size(204, 379);
            this.checkedListBoxMICEX.Sorted = true;
            this.checkedListBoxMICEX.TabIndex = 0;
            // 
            // groupBoxRTS
            // 
            this.groupBoxRTS.Controls.Add(this.checkedListBoxRTS);
            this.groupBoxRTS.Location = new System.Drawing.Point(339, 12);
            this.groupBoxRTS.Name = "groupBoxRTS";
            this.groupBoxRTS.Size = new System.Drawing.Size(242, 424);
            this.groupBoxRTS.TabIndex = 1;
            this.groupBoxRTS.TabStop = false;
            this.groupBoxRTS.Text = "РТС";
            // 
            // checkedListBoxRTS
            // 
            this.checkedListBoxRTS.FormattingEnabled = true;
            this.checkedListBoxRTS.Location = new System.Drawing.Point(17, 27);
            this.checkedListBoxRTS.Name = "checkedListBoxRTS";
            this.checkedListBoxRTS.Size = new System.Drawing.Size(207, 379);
            this.checkedListBoxRTS.Sorted = true;
            this.checkedListBoxRTS.TabIndex = 0;
            // 
            // groupBoxINDEX
            // 
            this.groupBoxINDEX.Controls.Add(this.checkedListBoxINDEX);
            this.groupBoxINDEX.Location = new System.Drawing.Point(611, 12);
            this.groupBoxINDEX.Name = "groupBoxINDEX";
            this.groupBoxINDEX.Size = new System.Drawing.Size(243, 424);
            this.groupBoxINDEX.TabIndex = 2;
            this.groupBoxINDEX.TabStop = false;
            this.groupBoxINDEX.Text = "Индексы";
            // 
            // checkedListBoxINDEX
            // 
            this.checkedListBoxINDEX.FormattingEnabled = true;
            this.checkedListBoxINDEX.Location = new System.Drawing.Point(17, 27);
            this.checkedListBoxINDEX.Name = "checkedListBoxINDEX";
            this.checkedListBoxINDEX.Size = new System.Drawing.Size(209, 379);
            this.checkedListBoxINDEX.Sorted = true;
            this.checkedListBoxINDEX.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(628, 498);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(134, 27);
            this.button1.TabIndex = 3;
            this.button1.Text = "ОК";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.Location = new System.Drawing.Point(24, 458);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 442);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Choose start date:";
            // 
            // comboBoxTimePeriod
            // 
            this.comboBoxTimePeriod.FormattingEnabled = true;
            this.comboBoxTimePeriod.Items.AddRange(new object[] {
            "1 min",
            "5 min",
            "10 min",
            "15 min",
            "30 min",
            "60 min",
            "2 hour",
            "4 hour",
            "Day",
            "Week",
            "Month",
            "Quarter",
            "Year"});
            this.comboBoxTimePeriod.Location = new System.Drawing.Point(24, 504);
            this.comboBoxTimePeriod.MaxDropDownItems = 12;
            this.comboBoxTimePeriod.Name = "comboBoxTimePeriod";
            this.comboBoxTimePeriod.Size = new System.Drawing.Size(121, 21);
            this.comboBoxTimePeriod.TabIndex = 6;
            this.comboBoxTimePeriod.Text = "1 min";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 488);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Choose time perid:";
            // 
            // textBoxQuantityBars
            // 
            this.textBoxQuantityBars.Location = new System.Drawing.Point(24, 555);
            this.textBoxQuantityBars.Name = "textBoxQuantityBars";
            this.textBoxQuantityBars.Size = new System.Drawing.Size(100, 20);
            this.textBoxQuantityBars.TabIndex = 8;
            this.textBoxQuantityBars.Text = "1000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 539);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Quantity  bars\':";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 578);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(431, 26);
            this.label4.TabIndex = 10;
            this.label4.Text = "\'If the quantity positively, - gathering goes \"back\" on time to the past from the" +
                " named date;\r\n if it is negative, – that \"forward\".";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(727, 549);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 26);
            this.button2.TabIndex = 11;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkBoxSaveorUpdate
            // 
            this.checkBoxSaveorUpdate.AutoSize = true;
            this.checkBoxSaveorUpdate.Location = new System.Drawing.Point(297, 458);
            this.checkBoxSaveorUpdate.Name = "checkBoxSaveorUpdate";
            this.checkBoxSaveorUpdate.Size = new System.Drawing.Size(121, 17);
            this.checkBoxSaveorUpdate.TabIndex = 12;
            this.checkBoxSaveorUpdate.Text = "Save or  update  file";
            this.checkBoxSaveorUpdate.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(555, 551);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(110, 26);
            this.button3.TabIndex = 13;
            this.button3.Text = "Clear all checked";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Symbol_list_ITinvest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 609);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.checkBoxSaveorUpdate);
            this.Controls.Add(this.groupBoxMICEX);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxQuantityBars);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxTimePeriod);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBoxINDEX);
            this.Controls.Add(this.groupBoxRTS);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Symbol_list_ITinvest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Symbols ITinvest";
            this.groupBoxMICEX.ResumeLayout(false);
            this.groupBoxRTS.ResumeLayout(false);
            this.groupBoxINDEX.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxMICEX;
        private System.Windows.Forms.CheckedListBox checkedListBoxMICEX;
        private System.Windows.Forms.GroupBox groupBoxRTS;
        private System.Windows.Forms.CheckedListBox checkedListBoxRTS;
        private System.Windows.Forms.GroupBox groupBoxINDEX;
        private System.Windows.Forms.CheckedListBox checkedListBoxINDEX;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dateTimePicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxTimePeriod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxQuantityBars;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox checkBoxSaveorUpdate;
        private System.Windows.Forms.Button button3;
    }
}