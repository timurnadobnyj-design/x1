//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "..."
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
using System.IO;
using System.Threading;

namespace Skilful
{
    public partial class QuotesPlayer : Form
    {
        string SourceFile;
        string OutputFile;
        string QuotesString;
        int QuotesCount;
        int QC;
        StreamReader srr;
        StreamWriter sw;

        
        public QuotesPlayer()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SourceFile = openFileDialog1.FileName;
                textBox1.Text = SourceFile;
                if (File.Exists(openFileDialog1.FileName))
                {
                    try
                    {
                        StreamReader sr = File.OpenText(openFileDialog1.FileName);
                        QuotesCount = 0;
                        while ((QuotesString = sr.ReadLine()) != null) QuotesCount++;
                        //Console.WriteLine(QuotesCount.ToString());
                        sr.Close();
                        srr = new StreamReader(openFileDialog1.FileName);
                        QC = QuotesCount;
                        label8.Text = "0 of " + QuotesCount.ToString();
                    }
                    catch (Exception f)
                    {
                        Console.WriteLine("Can't load " + openFileDialog1.FileName);
                        Console.WriteLine("The process failed: {0}", f.ToString());
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                    OutputFile = saveFileDialog1.FileName; 
                    fs.Close();
                    textBox2.Text = OutputFile;
                    sw = new StreamWriter(saveFileDialog1.FileName);
                }
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled=true;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (QuotesCount >= 0)
            {
                try
                {
                    QuotesString = srr.ReadLine();
                    if (QuotesString != null)
                    {
                        sw.WriteLine(QuotesString);
                        //Console.WriteLine(QuotesString);
                        sw.Flush();
                        QuotesCount--;
                        label8.Text = (QC - QuotesCount).ToString() + " of " + QC.ToString();
                    }
                }
                catch (Exception f)
                {
                    Console.WriteLine("The process failed: {0}", f.ToString());
                }
            }
            else {
                sw.Close();
                srr.Close();
                timer1.Enabled = false; }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            timer1.Interval = trackBar1.Value;
            label5.Text = trackBar1.Value.ToString();
        }
    }
}
