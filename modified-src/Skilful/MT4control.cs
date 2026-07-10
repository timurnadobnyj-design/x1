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
using Skilful.Data;
using Skilful;
using System.IO;
using System.Threading;


namespace Skilful
{
    public partial class MT4control : Form
    {
        public MT4control(MTproxy mtproxy)
        {
            InitializeComponent();
            this.mtproxy = mtproxy;
        }
        MTproxy mtproxy;

        /// <summary>
        /// delimiter to split symbol and it's uid
        /// </summary>
        private const string delimiter = "|";
        
        private void buttonOK_Click(object sender, EventArgs e)
        {
            //будем считать что список инструментов обновлен по нажатию этой кнопки,
            //при этом оставшиеся файлы от старого списка использоваться больше никогда не будут (они имеют уникальные номера)
            //поэтому их можно смело удалять, и для стерильности так мы и сделаем;)

            string folder_path = XMLConfig.Get("MT4Path") + "\\experts\\files\\";
            DirectoryInfo di = new DirectoryInfo(folder_path);
            FileInfo[] filelist = di.GetFiles("*'?'?");
            foreach (FileInfo fi in filelist)
            {
                bool save=false;
                string symbolname = fi.Name.Split('\'')[0];
                foreach(string item in checkedListBox1.CheckedItems){
                    if(item == symbolname){
                        save=true;
                        break;
                    }
                }
                if (!save)
                {
                    fi.Delete();
                    FileInfo[] csv = di.GetFiles(symbolname+"*.csv");
                    foreach (FileInfo f in csv) f.Delete();
                }
            }
           
            this.Close();
        }

        /// <summary>
        /// On load, get MT4path, get accounts, select first account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MT4control_Load(object sender, EventArgs e)
        {
            checkedListBox1.CheckOnClick = true;
            folderBrowserDialog1.Description = "Choose path to your MT4 terminal";
            folderBrowserDialog1.SelectedPath = XMLConfig.Get("MT4Path");
            folderBrowserDialog1.ShowNewFolderButton = false;
            textBox1.Text = XMLConfig.Get("MT4Path");
            if (Directory.Exists(textBox1.Text))
            {
                getAccounts();
                textBox2.Text = "";
                mtproxy.mt4init();
                setList();
            }
            else
            {
                buttonOK.Enabled = false;
                textBox2.Text = "Choose correct MT4 path";
            }

        }

        private void button_mt4path_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                XMLConfig.Set("MT4Path", textBox1.Text);
                if (Directory.Exists(textBox1.Text))
                {
                    mtproxy.WriteWndHandle(0);
                    getAccounts();
                    textBox2.Text = "";
                    mtproxy.mt4init();
                    setList();
                    buttonOK.Enabled = true;
                }
                else
                {
                    buttonOK.Enabled = false;
                    textBox2.Text = "Choose correct MT4 path";
                }
            }
        }

        /// <summary>
        /// Get accounts from MT4path
        /// </summary>
        /// <param name="mt4path">path of MT4 terminal</param>
        private void getAccounts()
        {
            string mt4path = XMLConfig.Get("MT4Path") + "\\config\\";
            if (Directory.Exists(mt4path) == false)
            {
                listBox1.Items.Clear();
                checkedListBox1.Items.Clear();
                textBox2.Text = "check mt4 path";
                return;
            }
            string[] accounts = Directory.GetFiles(mt4path, "*.srv");
            listBox1.Items.Clear();
            foreach (string account in accounts)
                listBox1.Items.Add(Path.GetFileNameWithoutExtension(account));
            int idx = listBox1.FindString(XMLConfig.Get("MT4account"));
            if (idx < 0) idx = 0;
            if (listBox1.Items.Count>0) listBox1.SetSelected(idx, true);
        }

        /// <summary>
        /// On the end of editing MT4path - check if there is such directory exists,
        /// if yes - get accounts and so on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text != XMLConfig.Get("MT4Path"))
            {
                //textBox1.Text = "new directory";
                XMLConfig.Set("MT4Path",  textBox1.Text);
                if (Directory.Exists(XMLConfig.Get("MT4Path")))
                {
                    mtproxy.WriteWndHandle(0);
                    getAccounts();
                    textBox2.Text = "";
                    mtproxy.mt4init();
                    setList();
                    buttonOK.Enabled = true;
                }
                else
                {
                    buttonOK.Enabled = false;
                    textBox2.Text = "Choose correct MT4 path";
                }
            }

        }

        /// <summary>
        /// On change of selected account - get new instruments from newly selected account,
        /// select first instrument
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            XMLConfig.Set("MT4account", listBox1.SelectedItem.ToString());
            checkedListBox1.Items.Clear();
            string symbolsPath = XMLConfig.Get("MT4Path") + "\\history\\"
                                 + (string)listBox1.SelectedItem + "\\symbols.sel";
            if (File.Exists(symbolsPath) == true)
            {
                (this.Owner as MainForm).DataManager.DataSource["MT4"].StoragePath = mtproxy.StoragePath();
                
                if((this.Owner as MainForm).DataManager.Config.ContainsKey("MT4"))
                    (this.Owner as MainForm).DataManager.Config["MT4"]["storagePath"] = mtproxy.StoragePath();
                
                XMLConfig.Set("MT4SymbolsPath", symbolsPath);
                List<string> symbols = getSymbolList(symbolsPath);
                foreach (string symbol in symbols)
                {
                    checkedListBox1.Items.Add(symbol);

                }
                textBox2.Text = "";
                mtproxy.mt4init();
                setList();
            }
            else
            {
                textBox2.Text = "wrong account";
            }

        }

        /// <summary>
        /// Get symbols from MT4
        /// </summary>
        /// <param name="filePath">filename of MT4 symbols.sel</param>
        /// <returns>list of symbols</returns>
        private List<string> getSymbolList(string filePath)
        {
            int symbolOffset = 116;
            int symbolRecordLength = 128;
            int symbolLength = 12;
            List<string> symbols = new List<string>();
            FileStream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader fReader = new BinaryReader(fStream);
            int symbolsNumber = (((int)fStream.Length) - 4) / symbolRecordLength;
            fReader.ReadBytes(4);
            char[] test;
            string symbol;
            for (int i = 0; i < symbolsNumber; i++)
            {
                test = fReader.ReadChars(symbolLength);
                fReader.ReadBytes(symbolOffset);
                symbol = "";
                foreach (char c in test)
                {
                    if (c == '\x00') break;
                    symbol = symbol + c.ToString();
                }
                symbols.Add(symbol);
            }

            if (fReader != null) fReader.Close();
            if (fStream != null) fStream.Close();

            return symbols;
        }

        private void setList()
        {
            foreach (MTSymbol symbol in mtproxy.mtSymbols)
            {
                if (checkedListBox1.Items.Contains(symbol.Symbol) == true)
                {
                    checkedListBox1.SetItemChecked(checkedListBox1.Items.IndexOf(symbol.Symbol), true);
                }
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> mt4list = new List<string>();
            foreach (MTSymbol symbol in mtproxy.mtSymbols)
            {
                mt4list.Add(symbol.Symbol);
                //MessageBox.Show(symbol.Symbol + " - " + symbol.Code);
            }
            if (mt4list.Contains(checkedListBox1.SelectedItem.ToString()) == false)
            {
                addSymbol(checkedListBox1.SelectedItem.ToString());
            }
            if (mt4list.Contains(checkedListBox1.SelectedItem.ToString()) == true)
            {
                delSymbol(checkedListBox1.SelectedItem.ToString());
                ((MainForm)this.Owner).RefreshSymList("MT4");
            }
        }

        /// <summary>
        /// Add symbol to toget.skl, MT4expert would start getting quotes on this symbol
        /// </summary>
        /// <param name="symbol">symbol/instrument</param>
        private void addSymbol(string addsymbol)
        {
            string filePath = XMLConfig.Get("MT4Path");
            filePath += "\\experts\\files\\toget.skl";
            mtproxy.MaxCode++;
            //MessageBox.Show(mtproxy.MaxCode.ToString());
            FileStream fStream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            StreamWriter fWriter = new StreamWriter(fStream);
            fWriter.WriteLine(addsymbol + delimiter + mtproxy.MaxCode.ToString());
            fWriter.Flush();
            fWriter.Close();
            fStream.Close();
            mtproxy.mt4init();
        }

        /// <summary>
        /// Deletes symbol from toget.skl, MT4expert would stop getting quotes on this symbol
        /// </summary>
        /// <param name="symbol">symbol/instrument</param>
        private void delSymbol(string delsymbol)
        {
            List<string> symbols = new List<string>();
            string filePath = XMLConfig.Get("MT4Path");
            filePath += "\\experts\\files\\toget.skl";

            FileStream fStream = new FileStream(filePath, FileMode.Truncate, FileAccess.Write);
            StreamWriter fWriter = new StreamWriter(fStream);

            foreach (MTSymbol temp in mtproxy.mtSymbols)
                if (temp.Symbol != delsymbol)   
                    fWriter.WriteLine(temp.Symbol + delimiter + temp.Code);

            fWriter.Flush();
            fWriter.Close();
            fStream.Close();


            string folder_path = XMLConfig.Get("MT4Path") + "\\experts\\files\\";
            DirectoryInfo di = new DirectoryInfo(folder_path);
            for (int i = 0; i < 15; i++)
            {
                try
                {
                    FileInfo[] filelist = di.GetFiles(delsymbol + "*'?'?"); foreach (FileInfo fi in filelist) fi.Delete();
                    filelist = di.GetFiles(delsymbol + "*.csv"); foreach (FileInfo fi in filelist) fi.Delete();
                }
                catch (Exception e)
                {
                    if (mtproxy.dbg) Logging.Log("Cannot delete " + delsymbol + "(*'?'?/*.csv)\n" + e.Message);
                    Thread.Sleep(100);
                }
            }
            mtproxy.mt4init();
        }
    }
}
