//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Isabek Satybekov
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
using Skilful;

namespace Skilful.Data
{
    public partial class MT4messages : Form
    {
        public MT4messages()
        {
            InitializeComponent();
        }

        public List<MT4Log> mt4logs = new List<MT4Log>();

        private void MT4messages_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void AddMessage(string message)
        {
            MT4Log temp = new MT4Log();
            DateTime tempdt = DateTime.Now;
            temp = new MT4Log(tempdt, message);
            mt4logs.Add(temp);
                this.listBox1.Items.Add(string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}: {1}", tempdt, message));
        }

        private void MT4messages_VisibleChanged(object sender, EventArgs e)
        {
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - this.Width, Screen.PrimaryScreen.WorkingArea.Bottom - this.Height);
            //this.AddMessage("Form is Shown/Hidden.");
        }


    }

    public class MT4Log
    {
        private DateTime _dt;
        private string _message;

        public DateTime DateTimeofMessage
        {
            get
            {
                return _dt;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }

        public MT4Log()
        {
            this._dt = DateTime.Now;
            this._message = string.Empty;
        }

        public MT4Log(DateTime dt, string message)
        {
            this._dt = dt;
            this._message = message;
        }
    }
}
