//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Andrey Zyablitsev (skat)
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
    partial class SelectDataSource_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectDataSource_Form));
            this.buttonDone = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonDone
            // 
            this.buttonDone.Location = new System.Drawing.Point(250, 150);
            this.buttonDone.Name = "buttonDone";
            this.buttonDone.Size = new System.Drawing.Size(59, 23);
            this.buttonDone.TabIndex = 0;
            this.buttonDone.Text = "Done";
            this.buttonDone.UseVisualStyleBackColor = true;
            this.buttonDone.Click += new System.EventHandler(this.buttonDone_Click);
            // 
            // SelectDataSource_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(565, 381);
            this.Controls.Add(this.buttonDone);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SelectDataSource_Form";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SelectDataSourceWindow";
            this.ResumeLayout(false);

        }

        #endregion

        //DSL_ block
        //DataSourceLibraryLoader DataSource;
        //System.Collections.Generic.Dictionary<string, Dictionary<string, string>> Config;
        int left = 12,
            top = 12,
            width = 541,
            height,
            labelwidth = 440,
            labelleft = 200,
            checkwidth = 200,
            checkleft = 18,
            lablehight = 42, //42 is nice height for 3 text lines
            lineheight = 50;

        System.Windows.Forms.GroupBox groupBox_moduleList;
        System.Windows.Forms.LinkLabel[] linkLabels;
        System.Windows.Forms.CheckBox[] checkBoxes;
        string[] SDS_description;
        string[] storagePath;
        void addModuleLayout(int i)
        {
            //init
            string SDSModuleName, SDSModulePrompt, description;
            LoginType login_type;
            DataSource[i].init(out SDSModuleName, out SDSModulePrompt, out description, out login_type);
            System.Collections.Generic.Dictionary<string, string> DSLModuleConfig;
            Config.TryGetValue(SDSModuleName, out DSLModuleConfig);

            storagePath[i] = DSLModuleConfig != null && DSLModuleConfig.ContainsKey("storagePath") ? DSLModuleConfig["storagePath"] : DataSource[i].StoragePath;

            this.checkBoxes[i] = new System.Windows.Forms.CheckBox();
            this.linkLabels[i] = new System.Windows.Forms.LinkLabel();
            this.groupBox_moduleList.Controls.Add(this.checkBoxes[i]);
            this.groupBox_moduleList.Controls.Add(this.linkLabels[i]);
            // 
            // checkBox i
            // 
            this.checkBoxes[i].AutoSize = true;
            this.checkBoxes[i].Location = new System.Drawing.Point(checkleft, 39 + lineheight * i);
            this.checkBoxes[i].Size = new System.Drawing.Size(checkwidth, 17);
            this.checkBoxes[i].TabIndex = i+1;
            this.checkBoxes[i].Name = SDSModuleName;
            this.checkBoxes[i].Text = SDSModuleName+":";
            this.checkBoxes[i].Tag = i;
            this.checkBoxes[i].UseVisualStyleBackColor = true;
            this.checkBoxes[i].Click += new System.EventHandler(this.checkBoxes_Click);
            this.checkBoxes[i].Checked = DSLModuleConfig != null ? DSLModuleConfig["checked"] != "0" : false;
            // 
            // linkLabel i
            // 
            this.linkLabels[i].AutoSize = true;
            this.linkLabels[i].Location = new System.Drawing.Point(labelleft, 41 + lineheight * i);
            this.linkLabels[i].MaximumSize = new System.Drawing.Size(labelwidth, lablehight);
            this.linkLabels[i].TabIndex = i + 4;
            this.linkLabels[i].TabStop = true;
            this.linkLabels[i].Name = "linkLabelName" + i;
            this.linkLabels[i].Text = DSLModuleConfig != null ? DSLModuleConfig["prompt"] : SDSModulePrompt;
            this.linkLabels[i].Tag = i;
            SDS_description[i] = description;
            switch(login_type){
                case LoginType.SelectFile:
                    this.linkLabels[i].Click += new System.EventHandler(this.linkLabels_FileOpen_Click);
                break;
                case LoginType.SelectDir:
                    this.linkLabels[i].Click += new System.EventHandler(this.linkLabels_FolderOpen_Click);
                break;
                case LoginType.SetIPLoginPassword:
                    this.linkLabels[i].Click += new System.EventHandler(this.linkLabels_LoginForm_Click);
                break;
                case LoginType.UserDefinedForm:
                    this.linkLabels[i].Click += new System.EventHandler(this.linkLabels_CustomForm_Click);
                break;
            }
        }
        void InitDataSourceModuleList()
        {
            //DataSource = ((MainForm)this.Owner)...;

            this.groupBox_moduleList = new System.Windows.Forms.GroupBox();
            this.checkBoxes = new System.Windows.Forms.CheckBox[DataSource.Count()];
            this.linkLabels = new System.Windows.Forms.LinkLabel[DataSource.Count()];
            SDS_description = new string[DataSource.Count()];
            storagePath = new string[DataSource.Count()];

            for (int i = 0; i < DataSource.Count(); i++)
            {
                addModuleLayout(i);
            }
            
            this.groupBox_moduleList.SuspendLayout();
            this.SuspendLayout();

            this.groupBox_moduleList.Location = new System.Drawing.Point(left, top);
            this.groupBox_moduleList.AutoSize = true;
            height = 50 + DataSource.Count() * 50;
            this.groupBox_moduleList.Size = new System.Drawing.Size(width, height);
            this.groupBox_moduleList.Name = "groupBox_SDS";
            this.groupBox_moduleList.Text = "Select Data Source";
            this.groupBox_moduleList.TabIndex = 4;
            this.groupBox_moduleList.TabStop = false;
            this.groupBox_moduleList.BackColor = System.Drawing.SystemColors.Window;
            this.groupBox_moduleList.ForeColor = System.Drawing.SystemColors.WindowText;

            this.buttonDone.Location = new System.Drawing.Point(15, top + height + 15);

            this.Controls.Add(this.groupBox_moduleList);
            this.groupBox_moduleList.ResumeLayout(false);
            this.groupBox_moduleList.PerformLayout();
            this.ResumeLayout(false);
        }
        //end of DSL_ block

        private System.Windows.Forms.Button buttonDone;
    }
}
