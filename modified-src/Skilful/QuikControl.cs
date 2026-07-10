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
using System.IO;
using System.Threading;

namespace Skilful
{
        
    public partial class QuikControl : Form
    {
        string LoadHistoryDir;
        string LoadQuotesDir;

        public QuikControl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //changing the quotes directory
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string LoadQuotesDir = folderBrowserDialog1.SelectedPath;
                linkLabel1.Text = LoadQuotesDir;
                XMLConfig.Set("QuikQuotesDir", LoadQuotesDir);
                Properties.Settings.Default.Save();
                LoadQuotesFilesList(LoadQuotesDir);
                fileSystemWatcher1.Path = LoadQuotesDir;
            }
        }

        private void QuikControl_Shown(object sender, EventArgs e)
         {  
            //LoadQuotesDir = XMLConfig.Get("QuikQuotesDir");
            //LoadHistoryDir = XMLConfig.Get("QuikHistoryDir");
            //linkLabel1.Text = LoadQuotesDir;
            //linkLabel2.Text = LoadHistoryDir;                
         }

        //loading the list of available quotes
        public void LoadQuotesFilesList(string dir)
         {
             int foundTF = 0;
             int foundTicker = 0;
             String line;
             String lineTF;
             String lineTicker;
                DirectoryInfo di = new DirectoryInfo(dir);
                FileInfo[] fi = di.GetFiles();
                listBox2.Items.Clear();
                listBox1.Items.Clear();
                listBox3.Items.Clear();
                foreach (FileInfo fiTemp in fi)
                {
                    using (StreamReader sr = new StreamReader(dir +"\\"+ fiTemp.Name))
                    {
                      if ((line = sr.ReadLine()) != null)
                        {
                            foundTF = line.IndexOf(",");
                            if (foundTF!=-1)
                            {
                            lineTF = line.Substring(foundTF+1,1);
                            if (lineTF == "5") //check the base timeframe
                            {
                                foundTicker = line.IndexOf(" ");
                                lineTicker = line.Substring(0, foundTicker); //get the ticker
                                listBox2.Items.Add(fiTemp.Name);
                                listBox1.Items.Add(lineTicker);
                                listBox3.Items.Add("<file not found>");
                            }
                            }
                        }
                      sr.Close();
                    }
                }
            }

      

         private void listBox1_Click(object sender, EventArgs e)
         {
             listBox2.SelectedIndex = listBox1.SelectedIndex;
             listBox3.SelectedIndex = listBox1.SelectedIndex;

         }

         private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
         {
             listBox1.SelectedIndex = listBox2.SelectedIndex;
             listBox3.SelectedIndex = listBox2.SelectedIndex;
         }

         private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
         {
             listBox2.SelectedIndex = listBox3.SelectedIndex;
             listBox1.SelectedIndex = listBox3.SelectedIndex;

         }

        //change the history directory
         private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
         {
             if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
             {
                 string LoadHistoryDir = folderBrowserDialog1.SelectedPath;
                 linkLabel2.Text = LoadHistoryDir;
                 XMLConfig.Set(".QuikHistoryDir", LoadHistoryDir);
                 //Properties.Settings.Default.Save();
                 listBox3.Items.Clear();
                 LoadHistoryFilesList(LoadHistoryDir);
             }
         }

         //loading the list of available history files
         public void LoadHistoryFilesList(string Histdir)
         {
             ///int HistFoundTicker = 0;
             int i =0;
             ///String HistLine;
             ///String HistLineTicker;
             DirectoryInfo Histdi = new DirectoryInfo(Histdir);
             FileInfo[] Histfi = Histdi.GetFiles();
             foreach (FileInfo HistfiTemp in Histfi)
             {
                 for (i = 0; i < listBox2.Items.Count; i++)
                 {
                     //the quotes and history files assigns by the filename, that must be equal to the ticker name
                     if (listBox2.Items[i].ToString().ToLower() == HistfiTemp.Name.ToLower())
                     {
                         listBox3.Items[i] = HistfiTemp.Name;
                     }                     
                 }           
             }
            
         }

         private void fileSystemWatcher1_Changed_1(object sender, FileSystemEventArgs e)
         {
             //in case of changes update the history file
             if (CompareQuotes(e.FullPath) == true) 
             {
                 //Console.WriteLine("True");
                 if (File.Exists(LoadHistoryDir + "\\" + Path.GetFileName(e.FullPath))) UpdateHistoryQuotes(e.FullPath);
             }
         }

         //check for the quotes changes
         private bool CompareQuotes(string FileName)
         {
            String QuotesLine = "";
            String HistoryLine = "";
            int i;
           
            for (i = 0; i < 15; i++)
            {
                try
                {
                    //get the last (up to date) quotes line from the history file
                    StreamReader HistoryFile = new StreamReader(LoadHistoryDir + "\\" + Path.GetFileName(FileName));
                    while (HistoryFile.EndOfStream != true) HistoryLine = HistoryFile.ReadLine();

                    //get the first (up to date) quotes line from the quotes file
                    StreamReader QuotesFile = new StreamReader(FileName);
                    QuotesLine = QuotesFile.ReadLine();
                    HistoryFile.Close();
                    QuotesFile.Close();
                    break;
                }
                catch
                { Thread.Sleep(200); }
            }

            //get the content of the last quote in the history file
            string[] HistQuotes = HistoryLine.Split(',');
            int HistDate = int.Parse(HistQuotes[2]);
            int HistTime = int.Parse(HistQuotes[3]);
            double HistHigh = double.Parse(HistQuotes[5].Replace('.',','));;
            double HistLow = double.Parse(HistQuotes[6].Replace('.',','));
            double HistClose = double.Parse(HistQuotes[7].Replace('.',','));

            //get the content of last quote in the quotes file
            string[] QuotesQuotes = QuotesLine.Split(',');
            int QuotesDate = int.Parse(QuotesQuotes[2]);
            int QuotesTime = int.Parse(QuotesQuotes[3]);
            double QuotesHigh = double.Parse(QuotesQuotes[5].Replace('.', ','));
            double QuotesLow = double.Parse(QuotesQuotes[6].Replace('.', ','));
            double QuotesClose = double.Parse(QuotesQuotes[7].Replace('.', ','));
            
            //check the changes in the quotes (date, time, high, low, close)
            if ((QuotesDate > HistDate) || (QuotesTime > HistTime) || (QuotesClose != HistClose) || (QuotesHigh != HistHigh) || (QuotesLow != HistLow)) 
            {
                return true;
            }
            else 
            {
                return false;
            }

                   
         }

         private void UpdateHistoryQuotes(string FileName)
         {
            String QuotesLine = "";
            String HistoryLine = "";
            String QuotesPrevLine = "";
            String HistoryPrevLine = "";
            int i;
            int z=0;
                    
            for (i = 0; i < 15; i++)
            {
                try
                {
                    //get the last quote from history file
                    StreamReader HistoryFile = new StreamReader(LoadHistoryDir + "\\" + Path.GetFileName(FileName));
                    while (HistoryFile.EndOfStream != true)
                    {
                        HistoryLine = HistoryFile.ReadLine();
                        z++;
                    }

                    //get the previous quote from history file
                    StreamReader HistoryPrevFile = new StreamReader(LoadHistoryDir + "\\" + Path.GetFileName(FileName));
                    z = z - 1;
                    for (i = 0; i < z; i++)
                    {
                        HistoryPrevLine = HistoryPrevFile.ReadLine();
                    }

                    //get the last and previous quotes from the quotes file
                    StreamReader QuotesFile = new StreamReader(FileName);
                    QuotesLine = QuotesFile.ReadLine();
                    QuotesPrevLine = QuotesFile.ReadLine();

                    HistoryFile.Close();
                    HistoryPrevFile.Close();
                    QuotesFile.Close();
                    break;
                }
                catch
                { Thread.Sleep(200); } //hush!!!
            }
             
             string[] HistQuotes = HistoryLine.Split(',');
             int HistDate = int.Parse(HistQuotes[2]);
             int HistTime = int.Parse(HistQuotes[3]);
             double HistHigh = double.Parse(HistQuotes[5].Replace('.', ',')); ;
             double HistLow = double.Parse(HistQuotes[6].Replace('.', ','));
             double HistClose = double.Parse(HistQuotes[7].Replace('.', ','));

             string[] HistPrevQuotes = HistoryPrevLine.Split(',');
             int HistPrevDate = int.Parse(HistPrevQuotes[2]);
             int HistPrevTime = int.Parse(HistPrevQuotes[3]);
             double HistPrevHigh = double.Parse(HistPrevQuotes[5].Replace('.', ',')); ;
             double HistPrevLow = double.Parse(HistPrevQuotes[6].Replace('.', ','));
             double HistPrevClose = double.Parse(HistPrevQuotes[7].Replace('.', ','));
             
             string[] QuotesQuotes = QuotesLine.Split(',');
             int QuotesDate = int.Parse(QuotesQuotes[2]);
             int QuotesTime = int.Parse(QuotesQuotes[3]);
             double QuotesHigh = double.Parse(QuotesQuotes[5].Replace('.', ','));
             double QuotesLow = double.Parse(QuotesQuotes[6].Replace('.', ','));
             double QuotesClose = double.Parse(QuotesQuotes[7].Replace('.', ','));

             string[] QuotesPrevQuotes = QuotesPrevLine.Split(',');
             int QuotesPrevDate = int.Parse(QuotesPrevQuotes[2]);
             int QuotesPrevTime = int.Parse(QuotesPrevQuotes[3]);
             double QuotesPrevHigh = double.Parse(QuotesPrevQuotes[5].Replace('.', ','));
             double QuotesPrevLow = double.Parse(QuotesPrevQuotes[6].Replace('.', ','));
             double QuotesPrevClose = double.Parse(QuotesPrevQuotes[7].Replace('.', ','));

             if (((QuotesDate == HistDate) && (QuotesTime == HistTime)) && ((QuotesClose != HistClose) || (QuotesHigh != HistHigh) || (QuotesLow != HistLow)))
             {
                 //update the current quote
             }

             if (((QuotesPrevDate == HistDate) && (QuotesPrevTime == HistTime)) && ((QuotesPrevClose != HistClose) || (QuotesPrevHigh != HistHigh) || (QuotesPrevLow != HistLow)))
             {
                 //there are new bar, update the previous and append the new bar
             }



         }
         
         //enable/disable quotes update
         private void checkBox1_CheckedChanged(object sender, EventArgs e)
         {
             if (checkBox1.Checked)
             {
                 LoadQuotesFilesList(LoadQuotesDir);
                 LoadHistoryFilesList(LoadHistoryDir);
                 fileSystemWatcher1.Path = LoadQuotesDir;
                 fileSystemWatcher1.NotifyFilter = NotifyFilters.LastWrite;
                 fileSystemWatcher1.Filter = "*.txt";
                 fileSystemWatcher1.EnableRaisingEvents = true;
             }
             else
             {
                     listBox1.Items.Clear();
                     listBox2.Items.Clear();
                     listBox3.Items.Clear();
                     fileSystemWatcher1.EnableRaisingEvents = false;
             
             }
         }

     


    }
}
