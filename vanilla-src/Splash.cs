//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Pavel Kadomin (S_PASHKA)
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
using Skilful.Properties;

namespace Skilful
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
             System.Diagnostics.Process.Start(linkLabel1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Cannot start Web browser.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Splash_FormClosed(object sender, FormClosedEventArgs e)
        {
              if (checkBox1.Checked)
              XMLConfig.Set("AgreedWithGPL", "true");
              //Properties.Settings.Default.Save();
              timer1.Enabled = false;
            
        }
        
        int i = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            i++;
            label8.Text = (5-i).ToString();
            if (i == 5)
            {
                this.Close();
            }
        }

        private void Splash_Shown(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void Splash_MouseClick(object sender, MouseEventArgs e)
        {
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            timer1.Enabled = false;
            checkBox1.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            timer1.Enabled = false;
            checkBox1.Visible = true;
        }

          
        
    }
}
