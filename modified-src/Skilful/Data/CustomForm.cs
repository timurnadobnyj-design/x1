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


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Skilful.Data
{
    public partial class CustomForm : Form
    {
        Label[] label;
        TextBox[] textbox;
        public CustomForm()
        {
            InitializeComponent();
        }
        public string Settings
        {
            get {
                string @out="";
                for(int i=0; i<label.Length; i++)
                {
                    if (i > 0) @out += ",";
                    @out += label[i].Text + "=" + textbox[i].Text;
                }
                return @out;
            }
            set {
                string[] lines = value.Split(',');
                label = new Label[lines.Length];
                textbox = new TextBox[lines.Length];

                //this.groupBox1 = new System.Windows.Forms.GroupBox();
                groupBox1.SuspendLayout();
                SuspendLayout();

                char delim = '=';
                if (lines[0].IndexOf('=') == -1) delim = ':';

                for (int i = 0; i < lines.Length; i++)
                {
                    string[] val = lines[i].Split(delim);
                    label[i] = new Label();
                    textbox[i] = new TextBox();
                    // 
                    // label_i
                    // 
                    label[i].AutoSize = true;
                    label[i].Location = new System.Drawing.Point(20, 38 + i * 30);
                    label[i].Name = "label_"+i;
                    label[i].Size = new System.Drawing.Size(35, 13);
                    label[i].TabIndex = i*2;
                    label[i].Text = val[0];
                    // 
                    // textBox_i
                    // 
                    textbox[i].Location = new System.Drawing.Point(195, 38 + i * 30);
                    textbox[i].Name = "textBox_"+i;
                    textbox[i].Size = new System.Drawing.Size(165, 20);
                    textbox[i].TabIndex = i*2+1;
                    textbox[i].Text = val[1];
                    groupBox1.Controls.Add(label[i]);
                    groupBox1.Controls.Add(textbox[i]);
                }
                groupBox1.ResumeLayout(false);
                groupBox1.PerformLayout();
                ResumeLayout(false);
            }
        }
    }
}
