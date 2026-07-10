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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Skilful;
using Skilful.QuotesManager;

namespace Skilful.Data
{
    public partial class SelectDataSource_Form : Form
    {
        MDataManager DataManager;
        DataSourceLibraryLoader DataSource;
        Dictionary<string, Dictionary<string, string>> Config;// = new Dictionary<string, Dictionary<string, string>>();
        int justNowRead = -1;
        TreeNode newnode;
        public SelectDataSource_Form(MDataManager DataManager_Object)
        {
            DataManager = DataManager_Object;
            DataSource = DataManager.DataSource;
            Config = DataManager.Config;
            InitializeComponent();
            InitDataSourceModuleList();
        }
        //---------------------------------------------------------------//
        private void checkBoxes_Click(object sender, EventArgs e)
        {

        }
        //---------------------------------------------------------------//
        private void linkLabels_FileOpen_Click(object sender, EventArgs e)
        {
            LinkLabel llabel = (LinkLabel)sender;
            int i = (int)llabel.Tag;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = SDS_description[i];
            justNowRead = -1;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                llabel.Text = openFileDialog.FileName;
                checkBoxes[i].Checked = true;
                storagePath[i] = DataSource[i].StoragePath;
                justNowRead = i;
            }
        }
        //---------------------------------------------------------------//
        private void linkLabels_FolderOpen_Click(object sender, EventArgs e)
        {
            LinkLabel llabel = (LinkLabel)sender;
            int i = (int)llabel.Tag;
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = llabel.Text.IndexOf(":\\") == 1 ? llabel.Text : Application.StartupPath;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                llabel.Text = folderBrowserDialog.SelectedPath;
                checkBoxes[i].Checked = true;
                storagePath[i] = DataSource[i].StoragePath;
            }
        }
        //---------------------------------------------------------------//
        private void linkLabels_LoginForm_Click(object sender, EventArgs e)
        {
        }
        //---------------------------------------------------------------//
        private void linkLabels_CustomForm_Click(object sender, EventArgs e)
        {
            LinkLabel llabel = (LinkLabel)sender;
            int i = (int)llabel.Tag;
            if (DataSource[i].method[11] != null)
            {
                string llabelold = llabel.Text;
                DataSource[i].ShowDialog();
                llabel.Text = DataSource[i].WorkingDir;
                if (llabel.Text == "") llabel.Text = llabelold;
            }
            else
            {
                CustomForm SettingsDialog = new CustomForm();

                char delim = '=';
                if (llabel.Text.IndexOf('=') == -1) delim = ':';
                SettingsDialog.Settings = llabel.Text.IndexOf(delim) > 0 ? llabel.Text : SDS_description[i];
                //SettingsDialog.Settings = llabel.Text.IndexOf(':') > 0 ? llabel.Text : SDS_description[i];

                if (SettingsDialog.ShowDialog() == DialogResult.OK)
                {
                    llabel.Text = SettingsDialog.Settings;
                    checkBoxes[i].Checked = true;
                    storagePath[i] = DataSource[i].StoragePath;
                }
            }
        }
        //---------------------------------------------------------------//
        private void buttonDone_Click(object sender, EventArgs e)
        {
            //---------------------------------------------------------------//
            //Init Symbol`s TreeView
            TreeView SymbolTreeView = ((MainForm)this.Owner).treeViewSymbols;
            //Skilful.Data.SelectSymbol_Form SymbolTreeView = ((MainForm)this.Owner).SymbolTreeView;
            //.Nodes.Count
            bool IsClearTree = SymbolTreeView.Nodes.Count == 0;
            //get selected datasource module and load his symbols
            ((MainForm)this.Owner).ClearSymList();
            for (int i = 0; i < checkBoxes.Length; i++)
            {
                if (checkBoxes[i].Checked)
                {
                    //linkLabels[i].Text now must contain a [Filename|Foldername|LoginDataString|OrSelf userdifened initialize string]
                    string[] SymbolList = DataSource[i].get_symbol_list(linkLabels[i].Text);
                    newnode = ((MainForm)this.Owner).AddSymList(i, checkBoxes[i].Text.Substring(0, checkBoxes[i].Text.Length - 1), SymbolList);
                    //если выбран единственный файл, то его можно открывать сразу же автоматом
                    if (i == justNowRead && SymbolList.Length == 1)
                    {
                        ((MainForm)this.Owner).SelectedSymbolNode = SymbolTreeView.SelectedNode = newnode;
                        ((MainForm)this.Owner).ShowChart(TF.m60, SymbolTreeView.SelectedNode);
                    }
                }
            }
            ((MainForm)this.Owner).toolStripContainerSymbolTree_expand(true);
            SymbolTreeView.ExpandAll();

            //---------------------------------------------------------------//
            //save settings
            for (int i = 0; i < checkBoxes.Length; i++)
            {
                Dictionary<string, string> param;
                if (Config.TryGetValue(checkBoxes[i].Name, out param))
                {
                    param["prompt"] = linkLabels[i].Text;
                    param["checked"] = checkBoxes[i].Checked ? "1" : "0";
                    param["storagePath"] = storagePath[i];
                }
                else
                {
                    param = new Dictionary<string, string>();
                    Config[checkBoxes[i].Name] = param;
                    param["prompt"] = linkLabels[i].Text;
                    param["checked"] = checkBoxes[i].Checked ? "1" : "0";
                    param["storagePath"] = storagePath[i];
                }
            }
            DataManager.Config_Save();
            //////////////////

            this.Close();
        }
        //---------------------------------------------------------------//
    }
}
