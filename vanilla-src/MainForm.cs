//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Alex Kokomov(Loylick), Andrey Prokhorov(AVP), Andrey Zyablitsev (skat),
//                    Eugeniy Bazarov(obolon), Isabek Satybekov, Logvinenko Eugeniy (manuka), Pavel Kadomin (S_PASHKA) 
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
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Xml;
using Skilful.Properties;
using Microsoft.Win32;
using Skilful.Data;
using Skilful.Statistics;
using Skilful.QuotesManager;
using Skilful.ModelManager;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using ChartV2;
using ChartV2.Data;
using ChartV2.Styles;
//using StClientLib;




namespace Skilful
{
    public delegate void NewTickHandler(string SourceName, string SymbolName, DateTime time, double bid, double ask);
    public delegate void NewBarHandler(string SourceName, string SymbolName, TF frame/*, TBar bar, bool completed*/, int idx);
    public delegate void LibMsgHandler(string SourceName, string SymbolName, int code, string message);
    public delegate void RefreshHistoryHandler(string SourceName, string SymbolName);

    public partial class MainForm : Form
    {
        //public DataSourceLibraryLoader DataSource;

        private const int WM_MT4INIT = 0x0700;
        private const int WM_MT4HISTORY = 0x0701;
        private const int WM_MT4UPDATE = 0x0702;
        private const int WM_MT4TICK = 0x0704,
                          WM_MT4TICK_BID = 0x0705,
                          WM_MT4TICK_ASK = 0x0706,
                          WM_MT4PIPVAL = 0x707;
        private const int WM_MT4CONNECTIONLOST = 0x0703;
        public static MTproxy mt4 = new MTproxy();
        public static MT4messages MT4messagesForm = new MT4messages();

        public MDataManager DataManager;
        public static List<Working> workingCharts = new List<Working>();       
        public List<Save> Save_parameters = new List<Save>();
        public static event NewTickHandler NewTick;
        public static event NewBarHandler NewBar;
        public static event RefreshHistoryHandler RefreshHistory;
        public static bool ShowHypotheticalTarget = true;
        public static bool ShowIndexCheckedModel = true;
        public static bool ShowSourceName = true;
        public static bool EnableHistoryCache = false;
        public Sample smp;
        
        
        public MainForm()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            InitializeComponent();
            DataManager = new MDataManager(Tick2Chart, Bar2Chart, DataSource_Message, RefreshHistoryData);
            InitSymbolsTreeView();
            tabControl.Selected += new TabControlEventHandler(tabControl_Selected);
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            
        }

        //------------------------------------------------------//
        private int Get_Index_Current_workingChart()
        {
            int i = -1;
            if (ActiveMdiChild != null)
                foreach (Working w in workingCharts)
                    if (w.workingForm.Handle == this.ActiveMdiChild.Handle)
                        i = workingCharts.IndexOf(w);
            return i;
        }
        ChartV2.Control ActiveChart
        {
            get
            {
                return (ChartV2.Control)workingCharts[Get_Index_Current_workingChart()].ActiveChart;
            }
        }
        //----------------------------------------------------------------//
        public void Tick2Chart(string SourceName, string SymbolName, DateTime time, double bid, double ask)
        {
            if (NewTick != null) NewTick(SourceName, SymbolName, time, bid, ask);
        }
        public void Bar2Chart(string SourceName, string SymbolName, TF frame, int idx)
        {
            if (NewBar != null) NewBar(SourceName, SymbolName, frame, idx);
        }
        public void DataSource_Message(string SourceName, string SymbolName, int code, string text)
        {
            MessageBox.Show(text);
        }
        public void RefreshHistoryData(string SourceName, string SymbolName)
        {
            if (RefreshHistory != null) RefreshHistory(SourceName, SymbolName);
        }
        public void CreateChart(DataSourseModule DataSource, string ChartName, string SymbolName, TF tf, TreeNode sourceTreeNode, int digits)
        {
            Logging.Log("Opening " + ChartName + " start");
            MT4messagesForm.AddMessage("Opening " + ChartName + " start");
            Working workingChart = new Working();
            TSymbol Symbol = DataManager.GetSymbol(DataSource, SymbolName, EnableHistoryCache);
            if (Symbol == null)
            {
                MessageBox.Show("Can not get Symbol " + SymbolName);
                return;
            }
            if (digits < 100) Symbol.setPipValue(digits, null);
            // =========================================
            int actualTF = Math.Max((int)TSymbol.getTF(Symbol.customTF), (int)tf);
            if (tf == TF.custom) this.toolStripButtonCustomFrame.Text = actualTF.ToString();
            if (workingChart.Get_ManagedData(ChartName, SymbolName, Symbol.Frames[(int)ChartV2.ChartType.Candle, actualTF])) return;
            workingChart_init(workingChart);
            workingChart.workingForm.Tag = sourceTreeNode;
            workingChart.tf = tf;
            workingChart.actualTF = actualTF;
            workingChart.tn = sourceTreeNode;

            //реалтайм функции получения данных
            NewTick += new NewTickHandler(workingChart.NewTick);
            NewBar += new NewBarHandler(workingChart.NewBar);
            RefreshHistory += new RefreshHistoryHandler(workingChart.RefreshHistory);

            Logging.Log("Drawing " + ChartName + " finished");
            MT4messagesForm.AddMessage("Drawing " + ChartName + " finished");
        }
        //----------------------------------------------------------------//
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Открытие нового графика из контекстного меню главного окна

            openFileDialog.Filter = "ASCII files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Logging.Log("Opening " + openFileDialog.FileName + " start");
                MT4messagesForm.AddMessage("Opening " + openFileDialog.FileName + " start");
                FileOpen(openFileDialog.FileName);
            }
            Logging.Log("Drawing " + openFileDialog.FileName + " finished");
            MT4messagesForm.AddMessage("Drawing " + openFileDialog.FileName + " finished");
        }

        void FileOpen(string FileOpenName)
        {
            try
            {
                Working workingChart = new Working();
                if (workingChart.Get_Data_Off_Line(FileOpenName, Path.GetFileName(FileOpenName))) return;
                //workingChart_init(workingChart);
            }
            catch
            {
                MessageBox.Show(FileOpenName + " has unsatisfied file format.", "Skilful", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        //---+++++-----------------------------------------------//
        void workingChart_init(Working workingChart)
        {
            workingChart.Button_Search_Complete_ += new Search_Complete(workingChart_Button_Search_Complete_);
            workingChart.workingForm.FormClosed += new FormClosedEventHandler(chartClosed_FormClosed);
            workingChart.workingForm.MdiParent = this;
            workingChart.Plot_Chart();
            workingChart.workingForm.Show();
            workingChart.ActiveChart.Switch_charts_ += new ChartV2.Switch_charts(ActiveChart_Switch_charts_);
            if (workingChart.workingForm.WindowState == FormWindowState.Maximized)
            {
                workingChart.workingForm.WindowState = FormWindowState.Normal;
                workingChart.workingForm.WindowState = FormWindowState.Maximized;
            }

            Add_menuItem_menuWindow(workingChart.workingForm.Text, workingChart.workingForm.Handle);
            Add_TabItem_tabControl(workingChart.workingForm.Text, workingChart.workingForm.Handle);

            if (Skilful.Sample.ChartType_ == ChartType.Line) workingChart.SetChartType(ChartType.Line);
            workingCharts.Add(workingChart);

            Set_Switch_Search_models();
        }
        


        private void chartClosed_FormClosed(object sender, FormClosedEventArgs e)
        {
            int i;
            if (this.ActiveMdiChild != null)
            {
                i = Get_Index_Current_workingChart();
                if ((e.CloseReason == CloseReason.MdiFormClosing) & (!Save.save_))
                {
                    Save.save_ = true;
                    Save_Out(i);
                }
                Remove_menuItem_menuWindow(workingCharts[i].workingForm.Handle);
                Remove_TabItem_tabControl(workingCharts[i].workingForm.Handle);

                //if (workingCharts[i].ActiveChart.bgWorker != null && workingCharts[i].ActiveChart.bgWorker.IsBusy) workingCharts[i].Stop_Search();
                if (workingCharts[i].bgWorker != null && workingCharts[i].bgWorker.IsBusy) workingCharts[i].Stop_Search();
                TSymbol sym = workingCharts[i].Quotes.Symbol;
                sym.Clear();
                DataManager.RemoveSymbol(sym);
                sym = null;
                workingCharts.RemoveAt(i);

                (this.ActiveMdiChild).Dispose();
            }
        }



        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {

            if (this.ActiveMdiChild != null)
            {
                int i = Get_Index_Current_workingChart();
                if (i != -1)
                {
                    if (workingCharts[i].flagSerch)
                    {
                        Set_Switch_Search_Complete();

                        if (workingCharts[i].bgWorker != null && workingCharts[i].bgWorker.IsBusy)
                            Set_Switch_Stop_Search();
                    }
                    else
                        Set_Switch_Search_models();

                    tabControl.SelectTab(Convert.ToString(workingCharts[i].workingForm.Handle));
                    Set_Check_menuItem_menuWindow();
                }
            }


        }

        private void SearchModelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                int i = Get_Index_Current_workingChart();
                if (i != -1)
                {
                    if (workingCharts[i].flagSerch)
                    {
                        if (workingCharts[i].bgWorker != null && workingCharts[i].bgWorker.IsBusy)
                        {
                            Set_Switch_Search_models();
                            workingCharts[i].Stop_Search();
                        }
                    }
                    else
                    {
                        // Расчет моделей
                        Set_Switch_Stop_Search();
                        workingCharts[i].SearchModel2(true, true);
                    }
                }
            }

        }

        public void workingChart_Button_Search_Complete_()
        {
            int i = Get_Index_Current_workingChart();

            Set_Switch_Search_Complete();
            if (i != -1)
                if (!workingCharts[i].flagSerch)
                    Set_Switch_Search_models();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void MainForm_Shown(object sender, EventArgs e)
        {
            try
            {
                if (XMLConfig.Get("AgreedWithGPL").Length == 0) new Splash().ShowDialog();

                // Загрузка настроек
                // MT4 proxy init
                if (XMLConfig.Get("MT4Path").Length < 2) //необходимо ли проверять каждый раз поменялся ли МТ4 у клиента?
                //if (Properties.Settings.Default.MT4Path.Length < 2) //необходимо ли проверять каждый раз поменялся ли МТ4 у клиента?
                {
                    const string mt4registry = "Software\\MetaQuotes Software\\MetaTrader 4";
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(mt4registry);
                    if (key != null)
                    {
                        string mt4path = (string)key.GetValue("InstallPath");
                        if (mt4path != null) XMLConfig.Set("MT4Path", mt4path);
                    }
                }
                //DataManager.DataSource["MT4"].StoragePath = mt4.StoragePath();
                mt4.WriteWndHandle((int)this.Handle);
                mt4.mt4init();
                mt4.getDigits();

                checkWindowMenuDropChart = windowStripMenuItem.DropDownItems.Count - 1;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            // Заполняется меню File-->Sample--> и Remove Sample имеющимися samples 

               if (XMLConfig.Get("CurrentSample").Length != 0)
               {
                Sample.currentSample = XMLConfig.Get("CurrentSample");
                SetSample(Application.StartupPath + "\\" + Sample.currentSample + ".smp");
                }
                DirectoryInfo dir = new DirectoryInfo(Application.StartupPath);
                FileInfo[] filesSample = dir.GetFiles("*.smp");
                string SampleName;
                if (filesSample != null)
                    foreach (FileInfo fi in filesSample)
                    {
                        SampleName = fi.Name.Substring(0, fi.Name.LastIndexOf('.'));
                        if (SampleName != "user_current_Sample")
                        {
                            Add_menuItem_menuSample(SampleName, true);
                            Add_menuItem_menuSample(SampleName, false);
                        }
                    }
              
              //  if (Sample.currentSample != "Default") toolStripMenuDefault.Checked = false;
                      
            
           
           //--------------------------------------------
                                  
            string mbic = XMLConfig.Get("MaxBarInChart");
            if (mbic.Length != 0)
            {
                toolStripComboBoxMBiC.Text = mbic;
                ChartV2.Data.ViewPort.maxBarInChart = Convert.ToInt32(mbic);
            }
            else
            {
                ChartV2.Data.ViewPort.maxBarInChart = 150;
                toolStripComboBoxMBiC.Text = "150";
            }

             if (XMLConfig.Get("LoadWithNewSearch") == "False")
               toolStripMenuItemLNS.Checked = false;
            
            if (XMLConfig.Get("ShowHypotheticalTarget") == "False")
            { toolStripMenuItemSHT.Checked = false; ShowHypotheticalTarget = false; }

            if (XMLConfig.Get("ShowIndexCheckedModel") == "False")
            { toolStripMenuItemSICM.Checked = false; ShowIndexCheckedModel = false; }

            if (XMLConfig.Get("ShowSourceName") == "False")
            { toolStripMenuItemSDSN.Checked = false; ShowSourceName = false; }

            if (XMLConfig.Get("EnableHistoryCache") == "True")
                toolStripMenuItemEHC.Checked = EnableHistoryCache = true;

            if (XMLConfig.Get("SaveModelsToFiles") == "True")
                SaveModelstoFiles.Checked = true;

            if (XMLConfig.Get("LoadLastSession") == "False")
                toolStripMenuItemLLS.Checked = false;             
            else Load_In();

        }

        private void closeStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                int i = Get_Index_Current_workingChart();
                Remove_menuItem_menuWindow(workingCharts[i].workingForm.Handle);
                Remove_TabItem_tabControl(workingCharts[i].workingForm.Handle);
                if (workingCharts[i].bgWorker != null && workingCharts[i].bgWorker.IsBusy) workingCharts[i].Stop_Search();
                workingCharts.RemoveAt(i);
                (this.ActiveMdiChild).Dispose();
            }
        }

        private void closeAllStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("All Charts will be closed.", "Warning.", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel) return;
            int i = Get_Index_Current_workingChart();
            while (i != -1)
            {
                Remove_menuItem_menuWindow(workingCharts[i].workingForm.Handle);
                Remove_TabItem_tabControl(workingCharts[i].workingForm.Handle);
                if (workingCharts[i].bgWorker != null && workingCharts[i].bgWorker.IsBusy) workingCharts[i].Stop_Search();
                workingCharts.RemoveAt(i);
                (this.ActiveMdiChild).Dispose();
                i = Get_Index_Current_workingChart();
            }
        }


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }


        private void cascadetoolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);

        }
        private void verticaltoolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void horizontaltoolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void OptionsMnu_Click(object sender, EventArgs e)
        {
            //Form OptionsForm = new OptionsForm();
            //OptionsForm.ShowDialog();
        }
        private void Save_Out_SymbolTreeView()
        {
            //SymbolTreeView
            for (int i = 0; i < treeViewSymbols.Nodes.Count; i++)
            {
                Save save = new Save();
                save.SymbolTreeView_Node = (int)treeViewSymbols.Nodes[i].Tag + 100; //index, root level of treenode
                save.SymbolTreeView_Node_expanded = treeViewSymbols.Nodes[i].IsExpanded;
                save.SymbolTreeView_Width = toolStripContainerSymbolTree.Width;
                Save_parameters.Add(save);
            }
        }
        private void Save_Out(int i)
        {
            string NameDir = Application.StartupPath + "\\" + "TAmodels";
            if (SaveModelstoFiles.Checked)
            {
                if (Directory.Exists(NameDir)) Directory.Delete(NameDir, true);
                Directory.CreateDirectory(NameDir);
            } 
            for (int j = 0; j < workingCharts.Count; j++)
            {
                       bool treeViewSymbolsNodesExist = false;
                       foreach (TreeNode tn in treeViewSymbols.Nodes)
                           if (tn.Text == workingCharts[j].tn.Parent.Text)
                               for (int n = 0; n < tn.Nodes.Count; n++)
                                   if (tn.Nodes[n].Text == workingCharts[j].tn.Text)
                                   {
                                       treeViewSymbolsNodesExist = true;
                                       continue;
                                   }

                       if (!treeViewSymbolsNodesExist) continue;
                //  ToString().Substring(workingCharts[j].tn.ToString().IndexOf(':') +1)
               
                string NameFile = workingCharts[j].tn.Text + ".txt";
                FileStream fs = null;
                StreamWriter sw = null;
                Save save;

                if (SaveModelstoFiles.Checked)
                {
                    
                    fs = new FileStream(Application.StartupPath + "\\" + "TAmodels" + "\\" + NameFile, FileMode.Create);
                    sw = new StreamWriter(fs);
                    save = new Save(workingCharts[j], sw, SaveModelstoFiles.Checked);
                    sw.Close();
                    fs.Close();
                }
                else 
                  save = new Save(workingCharts[j], sw, SaveModelstoFiles.Checked);

                save.Current_workingChart = false;
                if (j == i) save.Current_workingChart = true;

                save.Current_Active_Chart_TF = workingCharts[j].tf;
                save.workingForm_Size = workingCharts[j].workingForm.Size;
                save.workingForm_Left = workingCharts[j].workingForm.Left;
                save.workingForm_Top = workingCharts[j].workingForm.Top;
                save.workingForm_WindowState = workingCharts[j].workingForm.WindowState;
                save.workingForm_flagSerch = workingCharts[j].flagSerch;
                if (workingCharts[j].tn != null)
                {
                    save.workingForm_Node_Text = workingCharts[j].tn.Text;
                    if (workingCharts[j].tn.Level > 0)
                        save.workingForm_Node_Parent_Text = workingCharts[j].tn.Parent.Text;
                }

                

                Save_parameters.Add(save);
            }
            
        }
        private void Load_In()
        {
            Load load = new Load();
            int k = 0;
            List<TModel> tm, hm;
            TModel tm_, hm_;
            List<THTangentLine> tht;
            if (File.Exists(Application.StartupPath + "\\" + "List.bin"))
              try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (Stream stream = File.Open(Application.StartupPath + "\\" + "List.bin", FileMode.Open, FileAccess.Read))
                    {
                        if (stream.Length == 0) return;
                        Save_parameters = (List<Save>)formatter.Deserialize(stream);
                        stream.Close();
                    }

                    ProgressBar pb = (ProgressBar)load.Controls[1];
                    pb.Minimum = pb.Maximum = 0;
                    for (int x = 0; x < Save_parameters.Count - treeViewSymbols.Nodes.Count; x++)
                        pb.Maximum = pb.Maximum + Save_parameters[x].Active_Chart_TF.Count;
                    pb.Value = 0;
                    load.TopMost = true;

                    if ((Save_parameters.Count - treeViewSymbols.Nodes.Count) != 0) load.Show();
                    pb.Refresh();  // ProgressBar
                    bool toolStripContainerSymbolTree_Expanded = false;

                    int BreakeSymbol = 0;
                    // ---- список имен не загруженных символов
                    string BreakeSymbol_String = "";

                    for (int i = 0; i < Save_parameters.Count; i++)
                    {

                        if (Save_parameters[i].SymbolTreeView_Node >= 100)
                        {
                            int idx = Save_parameters[i].SymbolTreeView_Node - 100;
                            foreach (TreeNode tn in treeViewSymbols.Nodes)
                                if ((int)tn.Tag == idx && Save_parameters[i].SymbolTreeView_Node_expanded)
                                    tn.Expand();
                            if (!toolStripContainerSymbolTree_Expanded)
                            {
                                toolStripContainerSymbolTree.Width = Save_parameters[i].SymbolTreeView_Width;
                                toolStripContainerSymbolTree_Expanded = true;
                                label1.Visible = toolStripContainerSymbolTree.Width == 3;
                                label2.Visible = !label1.Visible;
                            }
                            continue;
                        }

                        if (i > (Save_parameters.Count - treeViewSymbols.Nodes.Count)) break;

                        bool SymbolExist = false;
                        int CurrentCount = i - BreakeSymbol;
                        foreach (TreeNode tn in treeViewSymbols.Nodes)
                        {
                            if (tn.Text == Save_parameters[i].workingForm_Node_Parent_Text)
                            {

                                for (int j = 0; j < tn.Nodes.Count; j++)
                                {

                                    if (tn.Nodes[j].Text != Save_parameters[i].workingForm_Node_Text) continue;
                                    SymbolExist = true;
                                    ShowChart(Save_parameters[i].Current_Active_Chart_TF, tn.Nodes[j]);
                                    workingCharts[CurrentCount].workingForm.Left = Save_parameters[i].workingForm_Left;
                                    workingCharts[CurrentCount].workingForm.Top = Save_parameters[i].workingForm_Top;
                                    workingCharts[CurrentCount].workingForm.Size = Save_parameters[i].workingForm_Size;

                                    // ------------- собираем ММ
                                    if (Save_parameters[i].MM != null)
                                    {

                                        for (int x = 0; x < (int)ChartType.count; x++)
                                            for (int y = 0; y < (int)TF.count; y++)
                                                if (Save_parameters[i].MM[x, y].Models != null)
                                                {
                                                    tm = new List<TModel>();
                                                    hm = new List<TModel>();
                                                    tht = new List<THTangentLine>();
                                                    //TQuotes q = workingCharts[CurrentCount].Quotes.Symbol.Frames[x, y]; 

                                                    // загружаем список моделей tm
                                                    for (int b = 0; b < Save_parameters[i].MM[x, y].Models.Count; b++)
                                                    {
                                                        tm_ = new TModel();
                                                        tm_ = Save.get_to_Load_TAmodel(Save_parameters[i].MM[x, y].Models[b]);
                                                        tm_.Quotes = workingCharts[CurrentCount].Quotes.Symbol.Frames[x, y];
                                                        tm.Add(tm_);
                                                    }
                                                    //  загружаем список формирующихся моделей
                                                    for (int b = 0; b < Save_parameters[i].MM[x, y].HModels.Count; b++)
                                                    {
                                                        hm_ = new TModel();
                                                        hm_ = Save.get_to_Load_TAmodel(Save_parameters[i].MM[x, y].HModels[b]);
                                                        hm_.Quotes = workingCharts[CurrentCount].Quotes.Symbol.Frames[x, y];
                                                        hm.Add(hm_);
                                                    }
                                                    // загружаем список ГЦ
                                                    for (int b = 0; b < Save_parameters[i].MM[x, y].HTangentLines.Count; b++)
                                                        tht.Add(Save_parameters[i].MM[x, y].HTangentLines[b]);

                                                    workingCharts[CurrentCount].Quotes.Symbol.Frames[x, y].MM = new ModelsManager(tm, hm, tht);

                                                    // Дорасчет моделей на хвосте котировок
                                                    workingCharts[CurrentCount].Quotes.Symbol.Frames[x, y].MM.SeekModels(workingCharts[CurrentCount].Quotes.Symbol.Frames[x, y], Skilful.ModelManager.Options.Extremum1 * 3, workingCharts[CurrentCount].Quotes.Symbol.Frames[x, y].GetCount() - 1, workingCharts[CurrentCount].Quotes.Symbol.Frames[x, y].Decimals);

                                                }


                                    }


                                    // -------------



                                    for (int n = 0; n < Save_parameters[i].Active_Chart_TF.Count; n++)
                                    {
                                        Label l = (Label)load.Controls[0]; // Label
                                        l.Text = Save_parameters[i].workingForm_Node_Parent_Text + ": " + Save_parameters[i].workingForm_Node_Text + " " + Save_parameters[i].Active_Chart_TF[n].ToString();
                                        l.Refresh();
                                        pb.PerformStep();
                                        pb.Refresh();
                                        switchTFonActiveChart(Save_parameters[i].Active_Chart_TF[n]);
                                        ChartV2.Control c = ActiveChart;
                                        //       c.plot.style.backColor = Save_parameters[i].Chart_backColor[n];
                                        //     c.plot.grid.style.minorHorizontalPen = c.plot.grid.style.minorVerticalPen = new Pen(Save_parameters[i].Grid_Color[n]);
                                        c.plot.grid.style.minorVerticalPen.DashPattern = new float[2] { c.plot.grid.style.minV1, c.plot.grid.style.minV2 };
                                        c.plot.grid.style.minorHorizontalPen.DashPattern = new float[2] { c.plot.grid.style.minH1, c.plot.grid.style.minH2 };
                                        //     c.plot.grid.IsVisible = Save_parameters[i].Grid_IsVisible[n];
                                        //    c.leftAxis.style.magorTickFont = Save_parameters[i].AxisStyleRight[n];
                                        //     c.leftAxis.cursorLable.font = c.rightAxis.cursorLable.font = Sample.CursorLable_Left_Right;
                                        //     c.gridSplitter.IsGridVisible = Save_parameters[i].GridSplitter_IsGridVisible[n];

                                        if (Skilful.Sample.ChartType_ == ChartV2.ChartType.Line)
                                        {
                                            workingCharts[CurrentCount].SetChartType(ChartV2.ChartType.Line);
                                            ActiveChart.Switch_charts_ += new Switch_charts(ActiveChart_Switch_charts_);
                                        }

                                        try
                                        { //search models
                                            //  if (Save_parameters[i].workingForm_flagSerch.Count <= n) MessageBox.Show("(Save_parameters[i].workingForm_flagSerch.Count <= n)");
                                            if (Save_parameters[i].workingForm_flagSerch)
                                            {

                                                if (toolStripMenuItemLNS.Checked)
                                                {
                                                    Set_Switch_Stop_Search();
                                                    try
                                                    {
                                                        if (n == 0) workingCharts[CurrentCount].SearchModel2(true, false);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        MessageBox.Show("LoadIn::SearchModel2: " + e.Message);
                                                    }
                                                }
                                                else
                                                {

                                                    c.seriesList[0].info.MM = workingCharts[CurrentCount].Quotes.Symbol.Frames[(int)c.seriesList[0].legend.chartType, (int)c.seriesList[0].legend.frame].MM;
                                                    c.ModelList2 = c.seriesList[0].info.MM.Models;
                                                    c.HTList = c.seriesList[0].info.MM.HTangentLines;
                                                    workingCharts[CurrentCount].flagSerch = true;
                                                }


                                                workingChart_Button_Search_Complete_();

                                                // if (Save_parameters[i].Chart_selectedModelIndexes.Count <= n) MessageBox.Show("(Save_parameters[i].Chart_selectedModelIndexes.Count <= n)");
                                                //  if (Save_parameters[i].Chart_selectedModelIndexes[n][0] == -1) continue;
                                                if (Save_parameters[i].models_save[n][0].modelID != -1)
                                                {
                                                    int[] selectedModelIndexes = Get_selectedModelIndexes(Save_parameters[i].models_save[n], c);
                                                    c.c_smi(selectedModelIndexes);
                                                    for (int a = 0; a < selectedModelIndexes.Length
                                                                 && a < ActiveChart.plot.modelsToDrawList.Count; a++)
                                                    {
                                                        int d = selectedModelIndexes[a];

                                                        if (ActiveChart.plot.modelsToDrawList.Count <= d)
                                                            break;
                                                        //MessageBox.Show("(ActiveChart.plot.modelsToDrawList.Count <= n)");


                                                        ChartV2.Model model = ActiveChart.plot.modelsToDrawList[d];
                                                        model.IsSelected = true;
                                                        if (model.HTi >= 0) ActiveChart.plot.HTtoDrawList[model.HTi].SelectedCount++;

                                                        try
                                                        { //draw target label
                                                            if (model.type != ModelType.ATR)
                                                            {
                                                                if ((!model.ModelPoint[5].IsEmpty) && (model.ModelPoint[5].X != model.ModelPointReal[6].X))
                                                                {
                                                                    c.tempTargetLable = new ChartV2.Axis_Plot.CursorInfoLable();
                                                                    c.tempTargetLable.IsVisible = true;
                                                                    c.tempTargetLable.font = Skilful.Sample.ModelStyle_TargetLable_Font;
                                                                    c.tempTargetLable.backBrush = new SolidBrush(Skilful.Sample.ModelStyle_TargetLable_Color);
                                                                    c.rightAxis.IsTargetsVisible = true;
                                                                    c.tempTargetLable.Value = model.ModelPoint[5].Y;
                                                                    c.tempTargetLable.rect.Y = c.plot.seriesToDrawList[0].viewPort.PriceToPixels(c.tempTargetLable.Value);
                                                                    c.rightAxis.targetLable.Add(d, c.tempTargetLable);

                                                                }
                                                            }
                                                            else
                                                            {
                                                                c.tempTargetLable = new ChartV2.Axis_Plot.CursorInfoLable();
                                                                c.tempTargetLable.IsVisible = true;
                                                                c.tempTargetLable.font = Skilful.Sample.ModelStyle_TargetLable_Font;
                                                                c.tempTargetLable.backBrush = new SolidBrush(Skilful.Sample.ModelStyle_TargetLable_Color);
                                                                c.rightAxis.IsTargetsVisible = true;
                                                                c.tempTargetLable.Value = model.PointsForDrawing_1_3[1].Y;
                                                                c.rightAxis.targetLable.Add(d, c.tempTargetLable);

                                                            }
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            MessageBox.Show("LoadIn:://draw target label: " + e.Message);
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            MessageBox.Show("LoadIn:://search models: " + e.Message);
                                        }

                                        try
                                        { //draw user graphic elements


                                            if (Save_parameters[i].custom_drowing_tools[n][0].dt != DrawingTool.None)
                                            {
                                                c.plot.graphToolsToDrawList = new List<UserLine>();
                                                for (int dd = 0; dd < Save_parameters[i].custom_drowing_tools[n].Length; dd++)
                                                {
                                                    switch ((DrawingTool)Save_parameters[i].custom_drowing_tools[n][dd].dt)
                                                    {
                                                        case DrawingTool.HorizontalLine:
                                                            c.plot.graphToolsToDrawList.Add(new HorizontalLine(Save_parameters[i].custom_drowing_tools[n][dd].pf1, Save_parameters[i].custom_drowing_tools[n][dd].pf2, (new UserLineStyle(new Pen(Save_parameters[i].custom_drowing_tools[n][dd].penstr)))));
                                                            break;
                                                        case DrawingTool.VerticalLine:
                                                            c.plot.graphToolsToDrawList.Add(new VerticalLine(Save_parameters[i].custom_drowing_tools[n][dd].IntPt1, Save_parameters[i].custom_drowing_tools[n][dd].IntPt2, (new UserLineStyle(new Pen(Save_parameters[i].custom_drowing_tools[n][dd].penstr)))));
                                                            break;
                                                        case DrawingTool.FreeLine:
                                                            c.plot.graphToolsToDrawList.Add(new AngularLine(Save_parameters[i].custom_drowing_tools[n][dd].pf1, Save_parameters[i].custom_drowing_tools[n][dd].pf2, (new UserLineStyle(new Pen(Save_parameters[i].custom_drowing_tools[n][dd].penstr))), Save_parameters[i].custom_drowing_tools[n][dd].Ext));
                                                            break;
                                                        case DrawingTool.Cycles:
                                                            c.plot.graphToolsToDrawList.Add(new Cycles(Save_parameters[i].custom_drowing_tools[n][dd].pf1, Save_parameters[i].custom_drowing_tools[n][dd].pf2, (new UserLineStyle(new Pen(Save_parameters[i].custom_drowing_tools[n][dd].penstr)))));
                                                            break;
                                                        case DrawingTool.Circle:
                                                            c.plot.graphToolsToDrawList.Add(new Circle(Save_parameters[i].custom_drowing_tools[n][dd].pf1, Save_parameters[i].custom_drowing_tools[n][dd].pf2, Save_parameters[i].custom_drowing_tools[n][dd].pf3, Save_parameters[i].custom_drowing_tools[n][dd].pf4, new UserLineStyle(new Pen(Save_parameters[i].custom_drowing_tools[n][dd].penstr))));
                                                            break;
                                                        case DrawingTool.Arc:
                                                            c.plot.graphToolsToDrawList.Add(new Arch(Save_parameters[i].custom_drowing_tools[n][dd].pf1, Save_parameters[i].custom_drowing_tools[n][dd].pf2, Save_parameters[i].custom_drowing_tools[n][dd].pf3, Save_parameters[i].custom_drowing_tools[n][dd].pf4, new UserLineStyle(new Pen(Save_parameters[i].custom_drowing_tools[n][dd].penstr))));
                                                            break;
                                                    }

                                                }
                                                foreach (UserLine d in c.plot.graphToolsToDrawList)
                                                    d.IsVisible = true;
                                            }

                                        }
                                        catch (Exception e)
                                        {
                                            MessageBox.Show("LoadIn:://draw user graphic elements: " + e.Message);
                                        }
                                        c.ReDrawEveryThing(true);

                                    }
                                    Save_parameters[i].custom_drowing_tools.Clear();

                                    switchTFonActiveChart(Save_parameters[i].Current_Active_Chart_TF);

                                }
                            }
                        }


                        if (!SymbolExist)
                        {
                            BreakeSymbol_String = BreakeSymbol_String + Save_parameters[i].workingForm_Node_Parent_Text + ": " + Save_parameters[i].workingForm_Node_Text + ", ";
                            if (((Save_parameters.Count - treeViewSymbols.Nodes.Count) == 1)&& (i == 0)) { k = -1; break; }
                            BreakeSymbol++;
                            SymbolExist = false;
                        }
                        if ((Save_parameters[i].Current_workingChart) && (BreakeSymbol == 0)) k = i;

                    }

                    if (k >= workingCharts.Count) k = workingCharts.Count - 1;
                    if (k >= 0)
                    {
                        workingCharts[k].workingForm.Focus();
                        workingCharts[k].workingForm.Refresh();
                        workingCharts[k].workingForm.WindowState = Save_parameters[k].workingForm_WindowState;
                    }

                    load.TopMost = false;
                    load.MdiParent = this;
                    load.Dispose();

                    if (BreakeSymbol_String != "")                    
                        MessageBox.Show("Breake to load :" + BreakeSymbol_String.Substring(0, BreakeSymbol_String.Length - 2) + ".");
                    
                 }

                catch (Exception e)
                {
                    MessageBox.Show("Load_In exception: " + e.Message);
                }

        }

        private int[] Get_selectedModelIndexes(Model_Data_to_Save[] mds, ChartV2.Control c)
        {
            TQuotes quotes = c.seriesList[0].info;
            List<int> gi = new List<int>();
            int[] gi_;
           
            for (int i = 0; i < mds.Length; i++)
               for (int j = 0; j < quotes.MM.Models.Count; j++)
                     if (mds[i].Price_point_1 == quotes.MM.Models[j].Point1.Price)
                        if (mds[i].Price_point_2 == quotes.MM.Models[j].Point2.Price)
                            if (mds[i].Price_point_3 == quotes.MM.Models[j].Point3.Price)
                                if (mds[i].Price_point_4 == quotes.MM.Models[j].Point4.Price)
                                    if (mds[i].DateTime_point_4 == quotes.MM.Models[j].Point4.DT)
                                        gi.Add(j);
            gi_ = new int[gi.Count];
            for (int k = 0; k < mds.Length; k++)
                    gi_[k] = gi[k];
            return gi_;   
            
        }
       

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Properties.Settings.Default.Save();// вроде и без этой строчки сохраняет
            Save_Out_SymbolTreeView();
            // Если Хелп запущен, - закрываем его
            if (eventHelp_) help.Kill();
            XML_Save_Selected_Model();

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (Stream stream = File.Create(Application.StartupPath + "\\" + "List.bin"))
                {
                    formatter.Serialize(stream, Save_parameters);
                    stream.Close();
                }
            }
            catch (Exception s)
            {
                MessageBox.Show("Save to List.bin: " + s.Message);
            }
        }
        //--- models Save xml ------------------------------------//
        private void XML_Save_Selected_Model()
        {
            string Price_point1, Price_point2, Price_point3, Price_point4;
            if (Save_parameters.Count < 2) return;
            try
            {
                StreamWriter xmlf = new StreamWriter(Application.StartupPath + "\\" + "save_selected_models.xml");
                XmlTextWriter xml = new XmlTextWriter(xmlf);
                xml.Formatting = Formatting.Indented;
                xml.WriteStartDocument();
                xml.WriteStartElement("Save_selected_models");

                for (int i = 0; i < Save_parameters.Count - 1; i++)
                {
                    if (Save_parameters[i].Active_Chart_Name.Count == 0) continue;
                    xml.WriteStartElement(Save_parameters[i].Active_Chart_Name[0]);
                    for (int j = 0; j < Save_parameters[i].Active_Chart_TF.Count; j++)
                    {
                        if (Save_parameters[i].models_save[j][0].modelID == -1) continue;
                        xml.WriteStartElement(Save_parameters[i].Active_Chart_Name[j] + "_" + Save_parameters[i].Active_Chart_TF[j]);
                        for (int k = 0; k < Save_parameters[i].models_save[j].Length; k++)
                        {

                            if (Save_parameters[i].chart_Legeng_log[j])
                            {
                                Price_point1 = GlobalMembersTAmodel.round(Math.Pow(10, Save_parameters[i].models_save[j][k].Price_point_1), Save_parameters[i].chart_Symbol_decimals[j]).ToString();
                                Price_point2 = GlobalMembersTAmodel.round(Math.Pow(10, Save_parameters[i].models_save[j][k].Price_point_2), Save_parameters[i].chart_Symbol_decimals[j]).ToString();
                                Price_point3 = GlobalMembersTAmodel.round(Math.Pow(10, Save_parameters[i].models_save[j][k].Price_point_3), Save_parameters[i].chart_Symbol_decimals[j]).ToString();
                                Price_point4 = GlobalMembersTAmodel.round(Math.Pow(10, Save_parameters[i].models_save[j][k].Price_point_4), Save_parameters[i].chart_Symbol_decimals[j]).ToString();
                            }
                            else
                            {
                                Price_point1 = GlobalMembersTAmodel.round(Save_parameters[i].models_save[j][k].Price_point_1, Save_parameters[i].chart_Symbol_decimals[j]).ToString();
                                Price_point2 = GlobalMembersTAmodel.round(Save_parameters[i].models_save[j][k].Price_point_2, Save_parameters[i].chart_Symbol_decimals[j]).ToString();
                                Price_point3 = GlobalMembersTAmodel.round(Save_parameters[i].models_save[j][k].Price_point_3, Save_parameters[i].chart_Symbol_decimals[j]).ToString();
                                Price_point4 = GlobalMembersTAmodel.round(Save_parameters[i].models_save[j][k].Price_point_4, Save_parameters[i].chart_Symbol_decimals[j]).ToString();
                            }

                            xml.WriteStartElement("modelID" + "__" + Save_parameters[i].models_save[j][k].modelID.ToString());
                            xml.WriteStartElement("Model");
                            xml.WriteAttributeString("DateTime_point_1", " :" + Save_parameters[i].models_save[j][k].DateTime_point_1.ToString());
                            xml.WriteAttributeString("Price_point_1", " :" + Price_point1);
                            xml.WriteAttributeString("DateTime_point_2", " :" + Save_parameters[i].models_save[j][k].DateTime_point_2.ToString());
                            xml.WriteAttributeString("Price_point_2", " :" + Price_point2);
                            xml.WriteAttributeString("DateTime_point_3", " :" + Save_parameters[i].models_save[j][k].DateTime_point_3.ToString());
                            xml.WriteAttributeString("Price_point_3", " :" + Price_point3);
                            xml.WriteAttributeString("DateTime_point_4", " :" + Save_parameters[i].models_save[j][k].DateTime_point_4.ToString());
                            xml.WriteAttributeString("Price_point_4", " :" + Price_point4);
                            xml.WriteEndElement();
                            xml.WriteEndElement();
                        }
                        xml.WriteEndElement();

                    }
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Close();
                xmlf.Close();
            }
            catch (Exception s)
            {
                MessageBox.Show("Save to save_selected_models.xml: " + s.Message);
            }

        }

        // -------------------------------------------------------//

        //--- symbol list methods -----------------------------------------//
        public void ClearSymList()
        {
            SuspendLayout();
            treeViewSymbols.Nodes.Clear();
            ResumeLayout(false);
        }

        //--- fill treeNode by symbols ------------------------------------//
        public TreeNode AddSymList(int i, string ModuleName, string[] symlist)
        {
            SuspendLayout();
            TreeNode lastnode = null, tn = new TreeNode(ModuleName);
            tn.Tag = i; //index in Modulelist (top level node)
            foreach (string sym in symlist)
            {
                lastnode = new TreeNode(sym);
                tn.Nodes.Add(lastnode);
            }
            treeViewSymbols.Nodes.Add(tn);
            ResumeLayout(false);
            return lastnode;
        }

        //--- check for the symbol exist in the symbol`s tree
        bool SymbolExist(string ModuleName, string SymbolName)
        {
            //ModuleName += ":";
            foreach (TreeNode tn in treeViewSymbols.Nodes)
                if (tn.Text == ModuleName)
                    foreach (TreeNode sym in tn.Nodes)
                        if (sym.Text == SymbolName)
                            return true;
            return false;
        }

        //--- Refresh treeNode`s symbol list for given Module ---------------//
        public void RefreshSymList(string ModuleName)
        {
            if (!(DataManager.Config.ContainsKey(ModuleName))) return;
            DataSourseModule ds = DataManager.DataSource[ModuleName];
            if (ds == null) return;

            string[] symlist = ds.get_symbol_list(DataManager.Config[ModuleName]["prompt"]/*ds.prompt*/);

            TreeNode tn = null;
            foreach (TreeNode tn_ in treeViewSymbols.Nodes)
            {
                if (tn_.Text == ModuleName)
                {
                    tn = tn_;
                    break;
                }
            }
            if (tn == null)
            {
                AddSymList(DataManager.DataSource.name2index(ModuleName), ModuleName, symlist);
                toolStripContainerSymbolTree_expand(true);
                //treeViewSymbols.ExpandAll();
            }
            else
            {
                SuspendLayout();
                tn.Nodes.Clear();
                foreach (string sym in symlist)
                {
                    tn.Nodes.Add(new TreeNode(sym));
                }
                ResumeLayout(false);
            }
        }

        //----------------------------------------------------------------//
        string chartname(TreeNode node, TF tf)
        {
            if (ShowSourceName) return node.Parent.Text + ": " + getcoolname(node.Text) + " " + tf.ToString();
            return getcoolname(node.Text)+ " " + tf.ToString();
        }

        /// <summary>
        /// Убирает лишние символы из названия инструмента
        /// </summary>
        /// <param name="uglyname">название инструмента</param>
        /// <returns>Чистое и красивое название инструмента</returns>
        string getcoolname(string uglyname)
        {
            if (uglyname.IndexOfAny(new char[] { '#', '_' }, 0, 1) == 0)
            {
                uglyname = uglyname.Substring(1);
            }

            return uglyname;
        }
        
        //----------------------------------------------------------------//
        public bool ShowChart(string ChartName)
        {
            foreach (Working win in workingCharts)
            {
                if (win.workingForm.Text == ChartName)
                {
                    win.workingForm.Focus();
                    return true;
                }
            }
            return false;
        }

        //----------------------------------------------------------------//
        private void selectDataSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectDataSource_Form DataSourceForm = new SelectDataSource_Form(DataManager);
            this.AddOwnedForm(DataSourceForm);
            DataSourceForm.ShowDialog();
        }

        //----------------------------------------------------------------//
        public void toolStripContainerSymbolTree_expand(bool expand)
        {
            toolStripContainerSymbolTree.Width = expand ? 150 : 3;
            label1.Visible = expand;
            label2.Visible = !expand;
        }

        //----------------------------------------------------------------//
        bool enable_tree_expand = true;
        private void toolStripContainerSymbolTree_ContentPanel_Click(object sender, EventArgs e)
        {
            if (enable_tree_expand)
                toolStripContainerSymbolTree_expand(toolStripContainerSymbolTree.Width == 3);
            enable_tree_expand = true;
        }

        //----------------------------------------------------------------//
        private void label2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int new_width = toolStripContainerSymbolTree.Width + e.X;
                if (new_width > 3 && new_width < (this.Width - 6))
                    toolStripContainerSymbolTree.Width = new_width;
                enable_tree_expand = false;
            }
        }

        //----------------------------------------------------------------//
        void InitSymbolsTreeView()
        {
            for (int i = 0; i < DataManager.DataSource.Count(); i++)
            {
                string[] SymbolList = DataManager.GetSymbolList(i);

                Dictionary<string, string> param;
                if (DataManager.Config.TryGetValue(DataManager.DataSource[i].name, out param) && (param["checked"] == "1"))
                {
                    Array.Sort(SymbolList);
                    AddSymList(i, DataManager.DataSource[i].name, SymbolList);
                }
            }
            if (treeViewSymbols.Nodes.Count > 0)
            {
                toolStripContainerSymbolTree_expand(true);
                //treeViewSymbols.ExpandAll();
            }
        }
        //----------------------------------------------------------------//
        public TreeNode SelectedSymbolNode;
        private void treeViewSymbols_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            SelectedSymbolNode = e.Node;

            if (e.Button == MouseButtons.Right) e.Node.TreeView.SelectedNode = e.Node;
            else if (e.Button == MouseButtons.Left)
            {
                if (e.Node.Level == 1) //is Symbol list level
                    ShowChart(TF.m60, SelectedSymbolNode);
            }
        }
        //----------------------------------------------------------------//
        public void ShowChart(TF tf, TreeNode node)
        {
            if (node.Level == 0) return; //is root_element
            string ChartName = chartname(node, tf);
            string SymbolName = node.Text;
            DataSourseModule DataSource = DataManager.DataSource[(int)node.Parent.Tag]; //TreeView->Root_n                                                                                                                                                                                   ode->tag is DatasouerceModule index

            //check for existing window and focus to one
            if (!ShowChart(ChartName))
            {
                //or create new window
                //нюанс при работе с модулем МТ4 -- необходимо освежить входной файл цсв перед загрузкой
                if (DataSource.name == "MT4")
                {
                    int i = node.Tag == null ? 0 : (int)node.Tag; //nested level of tree nodes
                    if ((i & 1) == 0 && tf < TF.Day) //файл 60 был обновлен
                    {
                        mt4.mergeUpdatetoHistoryFile(SymbolName, 60);
                        i |= 1;
                    }
                    if ((i & 2) == 0 && tf >= TF.Day) //файл 1440 был обновлен
                    {
                        mt4.mergeUpdatetoHistoryFile(SymbolName, 1440);
                        i |= 2;
                    }
                    node.Tag = i; // флаги обновления входных файлов csv (60 1й бит и 1440 2й бит)
                }
                //в случае доступа к мт через мтпрокси, можно получить родные данные о символах
                int digits = (DataSource.name == "MT4" && mt4.Digits.ContainsKey(SymbolName)) ? mt4.Digits[SymbolName] : 100;
                CreateChart(DataSource, ChartName, SymbolName, tf, node, digits);
            }
        }
        //----------------------------------------------------------------//
        private void contextMenu_SymbolFrames_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "60": ShowChart(TF.m60, SelectedSymbolNode); break;
                case "240": ShowChart(TF.m240, SelectedSymbolNode); break;
                case "Daily": ShowChart(TF.Day, SelectedSymbolNode); break;
                case "Weekly": ShowChart(TF.Week, SelectedSymbolNode); break;
                case "Monthly": ShowChart(TF.Month, SelectedSymbolNode); break;
                case "Quarterly": ShowChart(TF.Quarter, SelectedSymbolNode); break;
                case "Yearly": ShowChart(TF.Year, SelectedSymbolNode); break;
                case "All Charts":
                    for (TF i = TF.m60; i < TF.count; i++)
                        ShowChart(i, SelectedSymbolNode);
                    break;
                default: ShowChart(TF.custom, SelectedSymbolNode); break; //текст меню, зависит от исходного фрейма
            }
        }
        //================================================================//
        void ActiveChart_Switch_charts_(string s, List<TF> lstr)
        {

            switch (s)
            {
                case "Close this":
                    if (lstr.Count == 1) return;
                    ChartV2.Control c = workingCharts[Get_Index_Current_workingChart()].ActiveChart;
                    int i = lstr.IndexOf(c.seriesList[0].legend.frame);
                    if (i == 0) switchTFonActiveChart(lstr[i + 1]);
                    else switchTFonActiveChart(lstr[i - 1]);
                    workingCharts[Get_Index_Current_workingChart()].workingForm.Controls.Remove(c);
                    break;
                case "m60": switchTFonActiveChart(TF.m60);
                    break;
                case "m240": switchTFonActiveChart(TF.m240);
                    break;
                case "Day": switchTFonActiveChart(TF.Day);
                    break;
                case "Week": switchTFonActiveChart(TF.Week);
                    break;
                case "Month": switchTFonActiveChart(TF.Month);
                    break;
                case "Quarter": switchTFonActiveChart(TF.Quarter);
                    break;
                case "Year": switchTFonActiveChart(TF.Year);
                    break;
                default: switchTFonActiveChart(TF.custom); break; //текст кнопки, зависит от исходного фрейма
            }
        }



        [DllImport("User32.dll")]
        public static extern int FindWindow(string classname, string windowname);
        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(int hwnd);
        Process help;
        bool eventHelp_ = false;
        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!eventHelp_)
            {
                try
                {
                    help = Process.Start(Application.StartupPath + "\\Skilful Help.chm");
                    help.EnableRaisingEvents = true;
                    help.Exited += new EventHandler(help_Exited);
                    eventHelp_ = true;
                }
                catch (Win32Exception ev)
                {
                    if (ev.NativeErrorCode == 2)
                        MessageBox.Show(ev.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else SetForegroundWindow(FindWindow(null, "Skilful Help"));
        }
        private void help_Exited(object sender, System.EventArgs e)
        {
            eventHelp_ = false;
        }

        /// <summary>
        /// Для обновления окон с барами по сигналу от эксперта
        /// </summary>
        /// <param name="m"></param>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            int msg = m.Msg & 0x70F;
            if (msg != 0)
                switch (msg)
                {
                    // WParam -- код символа
                    // LParam -- период в мтшном стиле
                    case WM_MT4UPDATE:
                        // WM_MT4UPDATE приходит при получении нового бара, при этом перезаписывается файл - последние 100-500 баров
                        mt4.mergeUpdatetoHistoryFile(m.WParam.ToInt32(), m.LParam.ToInt32());
                        DataSourseModule mt4mod = DataManager.DataSource["MT4"];
                        if (mt4mod != null)
                        {
                            string sym = mt4.getSymbolFromCode(m.WParam.ToInt32());
                            TF tf = TF.m60;
                            switch (m.LParam.ToInt32())
                            {
                                case 60: tf = TF.m60; break;
                                case 240: tf = TF.m240; break;
                                case 1440: tf = TF.Day; break;
                                case 10080: tf = TF.Week; break;
                                case 43200: tf = TF.Month; break;
                                default: if(m.LParam.ToInt32()>0 && m.LParam.ToInt32()< 52000) tf = TF.custom; break;
                            }
                            int lastPosition;
                            DateTime lastTime = DataManager.GetLastTime("MT4", sym, tf, out lastPosition);
                            mt4mod.get_bar(sym, tf, lastTime, lastPosition);
                        }
                        break;

                    // WM_MESSADGE -- старшие 16бит=0000 ибо иначе сообщения не проходят, 4бита=digits, младшие 12бит=код сообщения
                    // WParam -- дата+время в си стиле
                    // LParam -- старшие 12 бит=код символа, младшие 20=бид/аск. 
                    case WM_MT4TICK_BID:
                        DateTime DT = new DateTime(1970, 01, 01) + TimeSpan.FromSeconds((uint)m.WParam.ToInt32());
                        int digits = (m.Msg >> 12) - 7;
                        double bid = (m.LParam.ToInt32() & 0xfffff) * Math.Pow(10, -digits);
                        Tick2Chart("MT4", mt4.getSymbolFromCode(m.LParam.ToInt32() >> 20), DT, bid, 0);
                        break;

                    case WM_MT4TICK_ASK:
                        DT = new DateTime(1970, 01, 01) + TimeSpan.FromSeconds((uint)m.WParam.ToInt32());
                        digits = (m.Msg >> 12) - 7;
                        double ask = (m.LParam.ToInt32() & 0xfffff) * Math.Pow(10, -digits);
                        Tick2Chart("MT4", mt4.getSymbolFromCode(m.LParam.ToInt32() >> 20), DT, 0, ask);
                        break;


                    //case WM_MT4TICK:
                    //    //такая себе упаковка: параметры
                    //    // WM_MESSADGE -- старшие 12бит=код символа, 4бита=digits, младшие 16бит=код сообщения
                    //    // WParam -- дата+время в си стиле
                    //    // LParam -- старшие 8 бит=спред младшие 24=бид.
                    //    DateTime DT = new DateTime(1970, 01, 01) + TimeSpan.FromSeconds((uint)m.WParam.ToInt32());
                    //    int digits = (m.Msg >> 16) & 0xF;
                    //    double bid = (m.LParam.ToInt32() & 0xffffff) * Math.Pow(10, -digits);
                    //    double ask = bid - (m.LParam.ToInt32() >> 24) * Math.Pow(10, -digits);
                    //    NewTick("MT4", mt4.getSymbolFromCode(m.Msg >> 20), DT, bid, ask);
                    //    break;

                    case WM_MT4HISTORY:
                        Logging.Log("WM_MT4HISTORY:mt4.loadHistoryFile");
                        mt4.mt4init();
                        mt4.loadHistoryFile(m.WParam.ToInt32(), m.LParam.ToInt32());
                        //for WM_MT4HISTORY WParam contains symbol_code and pip_value
                        string symbol = mt4.getSymbolFromCode(m.WParam.ToInt32());
                        //check for symbol in symbol`s tree
                        if (!SymbolExist("MT4", symbol))
                        {
                            Logging.Log(" : RefreshSymList " + symbol);
                            RefreshSymList("MT4");
                        }
                        break;

                    case WM_MT4INIT:
                        mt4.mt4init();
                        MT4messagesForm.AddMessage("MT4 expert initialized.");
                        MT4messagesForm.Show();
                        break;

                    case WM_MT4CONNECTIONLOST:
                        MT4messagesForm.AddMessage("MT4 connection lost.");
                        MT4messagesForm.Show();
                        break;

                    case WM_MT4PIPVAL:
                        break;
                }
            m.Msg &= 0x0000FFFF;
            base.WndProc(ref m);
        }
        // ==== Работа с меню window (добавление/удаление) подменю при (инициализации/дезактивации) workingChart) ===

        // -- добавление в Меню window элемента открываемого графика

        ToolStripMenuItem windowMenuDropChart;
        int checkWindowMenuDropChart;

        private void Add_menuItem_menuWindow(string nameWorkingChart, IntPtr hahdle)
        {
            windowMenuDropChart = new ToolStripMenuItem();
            windowMenuDropChart.Checked = true;
            windowMenuDropChart.Name = Convert.ToString(hahdle);
            windowMenuDropChart.Text = nameWorkingChart;
            windowMenuDropChart.Click += new EventHandler(windowMenuDropChart_Click);

            if (workingCharts.Count > 0)

                ((ToolStripMenuItem)windowStripMenuItem.DropDownItems[checkWindowMenuDropChart]).Checked = false;


            windowStripMenuItem.DropDownItems.Add(windowMenuDropChart);
            checkWindowMenuDropChart = windowStripMenuItem.DropDownItems.Count - 1;

        }

        void windowMenuDropChart_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem i = (ToolStripMenuItem)sender;
            if (!i.Checked)
            {
                foreach (Working w in workingCharts)
                    if (Convert.ToString(w.workingForm.Handle) == i.Name)
                    {
                        ((ToolStripMenuItem)windowStripMenuItem.DropDownItems[checkWindowMenuDropChart]).Checked = false;
                        checkWindowMenuDropChart = windowStripMenuItem.DropDownItems.IndexOf(i);
                        ((ToolStripMenuItem)windowStripMenuItem.DropDownItems[checkWindowMenuDropChart]).Checked = true;
                        w.workingForm.Focus();
                        return;
                    }
            }
            else
                workingCharts[Get_Index_Current_workingChart()].workingForm.Focus();
        }

        // -- удаление из Меню window элемента закрываемого графика
        private void Remove_menuItem_menuWindow(IntPtr handle)
        {
            if (Get_Index_Current_workingChart() == -1) return;
            int j = windowStripMenuItem.DropDownItems.Count - workingCharts.Count;
            for (int i = j; windowStripMenuItem.DropDownItems.Count > i; i++)
                if (((ToolStripMenuItem)windowStripMenuItem.DropDownItems[i]).Name == Convert.ToString(handle))
                {
                    windowStripMenuItem.DropDownItems.RemoveAt(i);
                    return;
                }

        }
        // --- установка галочки в меню на выбранный график
        private void Set_Check_menuItem_menuWindow()
        {
            int k = Get_Index_Current_workingChart();
            if (k == -1) return;
            ToolStripItemCollection tc = windowStripMenuItem.DropDownItems;
            int j = windowStripMenuItem.DropDownItems.Count - workingCharts.Count;
            for (int i = j; windowStripMenuItem.DropDownItems.Count >= i; i++)
                if (((ToolStripMenuItem)tc[i]).Name == Convert.ToString(workingCharts[k].workingForm.Handle))
                {
                    if (windowStripMenuItem.DropDownItems.Count > checkWindowMenuDropChart)
                        ((ToolStripMenuItem)windowStripMenuItem.DropDownItems[checkWindowMenuDropChart]).Checked = false;
                    ((ToolStripMenuItem)tc[i]).Checked = true;
                    checkWindowMenuDropChart = i;
                    return;
                }
        }

        // ==== Работа с вкладками  (добавление/удаление) tabControl при (инициализации/дезактивации) workingChart) ===

        // -- добавление в tabControl элемента открываемого графика
        bool select_tabPage = true;
        private void Add_TabItem_tabControl(string nameWorkingChart, IntPtr hahdle)
        {
            TabPage tabControltabPageChart = new TabPage(nameWorkingChart);
            tabControltabPageChart.Name = Convert.ToString(hahdle);
            tabControl.Visible = true;
            tabControl.TabPages.Add(tabControltabPageChart);
            select_tabPage = true;
            tabControl.SelectTab(tabControltabPageChart);

        }



        // --  обработка выбранного TabPage
        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {

            if (tabControl.TabPages.Count == 0) return;
            if (!select_tabPage)
            {
                select_tabPage = true;
                return;
            }
            TabPage i = ((TabControl)sender).SelectedTab;
            foreach (Working w in workingCharts)
                if (Convert.ToString(w.workingForm.Handle) == i.Name)
                {
                    w.workingForm.Focus();
                    return;
                }

        }


        // -- удаление TabPage
        private void Remove_TabItem_tabControl(IntPtr handle)
        {
            if (tabControl.TabPages.Count == 1)
                tabControl.Visible = false;
            foreach (TabPage tp in tabControl.TabPages)
                if (tp.Name == Convert.ToString(handle))
                {
                    select_tabPage = false;
                    tabControl.TabPages.Remove(tp);
                    return;
                }

        }


        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string fileName in filenames)
                {
                    FileInfo fi = new FileInfo(fileName);
                    if ((fi.Attributes & (FileAttributes.Directory | FileAttributes.Device)) == 0 /*&& Path.GetExtension(fi.Name) == ".prn"*/)
                        FileOpen(fileName);
                }
            }

        }

        private void toolStripButtonHLine_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null) return;

            ActiveChart.HorizontalLine();
        }

        private void toolStripButtonVLine_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null) return;

            ActiveChart.VerticalLine();
        }

        private void toolStripButtonCircle_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null) return;

            ActiveChart.Circle();
        }

        private void toolStripButtonArrowC_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null) return;

            ActiveChart.Cursor_Arrow();
        }

        private void toolStripButtonArc_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null) return;

            ActiveChart.Arc();
        }

        private void toolStripButtonTLine_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null) return;

            ActiveChart.FreeLine();
        }
        private void toolStripButtonCycle_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null) return;

            ActiveChart.Cycles();
        }

        private void toolStripButtonCrossC_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null) return;

            ActiveChart.Cursor_Cross();
        }

        private void toolStripButtonCChart_Click(object sender, EventArgs e)
        {

            if (this.ActiveMdiChild == null) return;
            int j = Get_Index_Current_workingChart();
            ChartV2.ChartType ct = workingCharts[j].Quotes.chartType;
            TF tf = workingCharts[j].Quotes.tf;
            if (ct == ChartV2.ChartType.Candle) return;
            byte b = 0;
            for (int i = 0; i < workingCharts[j].workingForm.Controls.Count; i++)
            {
                if (workingCharts[j].workingForm.Controls[i].ToString() != "ChartV2.Control") continue;
                ChartV2.Control c = (ChartV2.Control)workingCharts[j].workingForm.Controls[i];
                if (c.seriesList[0].legend.frame == tf) b++;
            }
            workingCharts[j].SetChartType(ChartV2.ChartType.Candle);
            if (b < 2) ActiveChart.Switch_charts_ += new Switch_charts(ActiveChart_Switch_charts_);

        }



        private void toolStripButtonCLine_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null) return;
            int j = Get_Index_Current_workingChart();
            ChartV2.ChartType ct = workingCharts[Get_Index_Current_workingChart()].Quotes.chartType;
            TF tf = workingCharts[j].Quotes.tf;
            if (ct == ChartV2.ChartType.Line) return;
            byte b = 0;
            for (int i = 0; i < workingCharts[j].workingForm.Controls.Count; i++)
            {
                if (workingCharts[j].workingForm.Controls[i].ToString() != "ChartV2.Control") continue;
                ChartV2.Control c = (ChartV2.Control)workingCharts[j].workingForm.Controls[i];
                if (c.seriesList[0].legend.frame == tf) b++;
            }
            workingCharts[Get_Index_Current_workingChart()].SetChartType(ChartV2.ChartType.Line);
            if (b < 2) ActiveChart.Switch_charts_ += new Switch_charts(ActiveChart_Switch_charts_);
        }

        /// <summary>
        /// Переключение ТФ на активном графике
        /// </summary>
        /// <param name="new_frame">TF таймфрейм</param>
        void switchTFonActiveChart(TF new_frame)
        {
            if (this.ActiveMdiChild == null) return;
            int a = Get_Index_Current_workingChart();
            List<TF> ltf = new List<TF>();
            TF old_frame = workingCharts[a].Quotes.tf;
            if (old_frame == new_frame) return;
            if (TSymbol.getTF(workingCharts[a].Quotes.Symbol.customTF) > new_frame) return;
            
            //переключатель
            for (int i = 0; i < workingCharts[a].workingForm.Controls.Count; i++)
            {
                if (workingCharts[a].workingForm.Controls[i].ToString() != "ChartV2.Control") continue;
                ChartV2.Control c = (ChartV2.Control)workingCharts[a].workingForm.Controls[i];

                ltf.Add(c.seriesList[0].legend.frame);
            }
            int wc = Get_Index_Current_workingChart();
            workingCharts[wc].SetChartTF(new_frame);

            bool j = true;
            foreach (TF tf in ltf)
                if (tf == new_frame) j = false;
            if (j) workingCharts[wc].ActiveChart.Switch_charts_ += new ChartV2.Switch_charts(ActiveChart_Switch_charts_);
            //коррекция надписей на окне, в меню, в табах
            this.ActiveMdiChild.Text =
                this.tabControl.SelectedTab.Text =
                    windowStripMenuItem.DropDownItems[checkWindowMenuDropChart].Text =
                        this.ActiveMdiChild.Text.Replace(old_frame.ToString(), new_frame.ToString());
            //надпись на кнопке кастом в тулбаре
            this.toolStripButtonCustomFrame.Text = new_frame == TF.custom ? workingCharts[wc].actualTF.ToString() : "";
        }


        private void toolStripButtonCustomFrame_Click(object sender, EventArgs e)
        {
            switchTFonActiveChart(TF.custom);
        }
        private void toolStripButtonH1_Click(object sender, EventArgs e)
        {
            switchTFonActiveChart(TF.m60);
            //if (this.ActiveMdiChild == null) return;
            //ShowChart(TF.m60, (TreeNode)this.ActiveMdiChild.Tag);
        }
        private void toolStripButtonH4_Click(object sender, EventArgs e)
        {
            switchTFonActiveChart(TF.m240);
        }
        private void toolStripButtonD_Click(object sender, EventArgs e)
        {
            switchTFonActiveChart(TF.Day);
        }
        private void toolStripButtonW_Click(object sender, EventArgs e)
        {
            switchTFonActiveChart(TF.Week);
        }
        private void toolStripButtonM_Click(object sender, EventArgs e)
        {
            switchTFonActiveChart(TF.Month);
        }
        private void toolStripButtonQ_Click(object sender, EventArgs e)
        {
            switchTFonActiveChart(TF.Quarter);
        }
        private void toolStripButtonY_Click(object sender, EventArgs e)
        {
            switchTFonActiveChart(TF.Year);
        }

        private void toolStripButtonDSource_Click(object sender, EventArgs e)
        {
            selectDataSourceToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButtonSModels_Click(object sender, EventArgs e)
        {
            SearchModelsToolStripMenuItem_Click(sender, e);
        }

        // ----- установщики свойств меню (кнопки) Search
        private void Set_Switch_Search_models()
        {
            SearchModelsToolStripMenuItem.Enabled = true;
            SearchModelsToolStripMenuItem.Text = "Search models";
            toolStripButtonSModels.Enabled = true;
            toolStripButtonSModels.ToolTipText = "Search models";
            toolStripButtonSModels.Image = Resources.play1disabled;
            toolStripMenuItemClearModels.Enabled = true;
            toolStripMenuItem_Statistic.Enabled = true;
        }
        private void Set_Switch_Search_Complete()
        {
            SearchModelsToolStripMenuItem.Text = "Search Complete";
            SearchModelsToolStripMenuItem.Enabled = false;
            toolStripButtonSModels.Enabled = false;
            toolStripButtonSModels.ToolTipText = "Search Complete";
            toolStripButtonSModels.Image = Resources.play1disabled;
            toolStripMenuItemClearModels.Enabled = true;
            toolStripMenuItem_Statistic.Enabled = true;
        }
        private void Set_Switch_Stop_Search()
        {
            SearchModelsToolStripMenuItem.Enabled = true;
            SearchModelsToolStripMenuItem.Text = "Stop search";
            toolStripButtonSModels.Enabled = true;
            toolStripButtonSModels.ToolTipText = "Stop search";
            toolStripButtonSModels.Image = Resources.stop1disabled;
            toolStripMenuItemClearModels.Enabled = false;
            toolStripMenuItem_Statistic.Enabled = false;
        }


        //-------------------------------------------------------------

        /// <summary>
        /// mt4control menu item click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mT4ControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MT4control form = new MT4control(mt4);
            this.AddOwnedForm(form);
            form.ShowDialog();
            mt4.getDigits();
            mt4.mt4init();
            if (XMLConfig.Get("MT4Path").Length > 0)
                if (DataManager.SetNewDataSourceFilePath("MT4", XMLConfig.Get("MT4Path") + "\\experts\\files\\"))
                    RefreshSymList("MT4");
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Skilful.ModelManager.Options.VisibleSteps = Int32.Parse(((ToolStripTextBox)sender).Text);
                if (this.ActiveMdiChild == null) return;
                ChartV2.Control c;
                c = (ChartV2.Control)workingCharts[Get_Index_Current_workingChart()].workingForm.Controls[0];
                c.ReDrawEveryThing(false);

            }
            catch (FormatException)
            {
                MessageBox.Show("Введите числовое значение от 1 до 3!!!");
            }

        }

        //-------------------------------------------------------------
        private void removeAllToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = Get_Index_Current_workingChart();
            if (i == -1) return;
            ChartV2.Control c = (ChartV2.Control)workingCharts[i].ActiveChart;
            c.RemoveAllGraphicalTools();
            c.ReDrawEveryThing(false);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (MT4messagesForm.Visible == true) MT4messagesForm.Hide();
            else MT4messagesForm.Show();
        }

        private void toolStripButtonSkipScaling_Click(object sender, EventArgs e)
        {
            int i = Get_Index_Current_workingChart();
            if (i == -1) return;
            ChartV2.Control c = (ChartV2.Control)workingCharts[i].ActiveChart;
            c.skipScalingToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButtonRemoveAll_Click(object sender, EventArgs e)
        {
            removeAllToolsToolStripMenuItem_Click(sender, e);
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern long BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        Bitmap memoryImage;
        private Bitmap CaptureScreen(Object o, Size s, string ss)
        {

            MainForm f;
            ChartV2.Control c;
            Bitmap memoryImage;
            Graphics mygraphics, g;
            Image img;
            // -------- 
            int cW, cH, c_gW, c_gH;
            cW = cH = c_gW = c_gH = 0;
            // --------

            if (o is MainForm)
            {
                f = (MainForm)o;
                mygraphics = f.CreateGraphics();
                memoryImage = new Bitmap(f.Width, f.Height, mygraphics);
            }
            else
            {
                c = (ChartV2.Control)o;
                mygraphics = c.CreateGraphics();
                memoryImage = new Bitmap(c.Width, c.Height, mygraphics);
                // ----------------- 
                cW = c.Width; cH = c.Height;
                c_gW = c.gridSplitter.LeftAxisWidth + 1; c_gH = c.gridSplitter.TopAxisHeight + 1;

            }
            Graphics memoryGraphics = Graphics.FromImage(memoryImage);
            IntPtr dc1 = mygraphics.GetHdc();
            IntPtr dc2 = memoryGraphics.GetHdc();
            BitBlt(dc2, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height, dc1, 0, 0, 13369376);
            mygraphics.ReleaseHdc(dc1);
            memoryGraphics.ReleaseHdc(dc2);
            memoryGraphics.Dispose();
            if ((o is ChartV2.Control) && (ss == "Save Light Chart As Image"))
            {
                img = (Bitmap)memoryImage.Clone();
                Bitmap newbmp = new Bitmap(cW - c_gW, cH - c_gH);
                g = Graphics.FromImage(newbmp);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, -c_gW, -c_gH, cW, cH);
                return newbmp;
            }


            return memoryImage;
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = Get_Index_Current_workingChart();
            if (i == -1) return;
            Size s = new Size(0, 0);
            ChartV2.Control f = (ChartV2.Control)workingCharts[i].ActiveChart;
            f.Refresh();
            memoryImage = CaptureScreen(f, s, "");
            PrintDocument printDoc = new PrintDocument();
            printDialog1.Document = printDoc;
            if (printDialog1.ShowDialog() != DialogResult.Cancel)
            {
                if (printDialog1.PrintToFile)
                {
                    printDialog1.PrinterSettings.PrintFileName = Convert.ToString(Directory.GetCurrentDirectory()) + "\\" + workingCharts[i].workingForm.Text.Substring(workingCharts[i].workingForm.Text.IndexOf(':') + 1) + ".png";
                    if (File.Exists(printDialog1.PrinterSettings.PrintFileName))
                        if (MessageBox.Show("File " + workingCharts[i].workingForm.Text + " already exists.\n Rewrite?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                            return;
                    memoryImage.Save(printDialog1.PrinterSettings.PrintFileName, System.Drawing.Imaging.ImageFormat.Png);
                    return;
                }

                printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
                printDoc.Print();
            }
        }

        private void printDoc_PrintPage(System.Object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Size s = new Size();
            float x, y;
            int i = Get_Index_Current_workingChart();
            ChartV2.Control f = (ChartV2.Control)workingCharts[i].ActiveChart;
            f.Refresh();
            Font font = new Font("Arial", 10, FontStyle.Italic);
            y = font.GetHeight(e.Graphics);
            s.Height = e.MarginBounds.Height + e.MarginBounds.Y - (int)y;
            s.Width = e.MarginBounds.Width + e.MarginBounds.X;
            int H = f.Height;
            int W = f.Width;
            f.Dock = DockStyle.None;
            f.Height = H;
            f.Width = W;
            if (f.Width > s.Width) f.Width = s.Width;
            if (f.Height > s.Height) f.Height = s.Height;
            memoryImage = CaptureScreen(f, s, "");
            f.Height = H;
            f.Width = W;
            f.Dock = DockStyle.Fill;
            x = ((e.MarginBounds.Width + e.MarginBounds.X) - memoryImage.Width) / 2;
            e.Graphics.DrawString(workingCharts[i].workingForm.Text, font, Brushes.Black, x, 0);
            e.Graphics.DrawImage(memoryImage, x, y);
        }

        private void toolStripMenuItemSaveImage_Click(object sender, EventArgs e)
        {
            int i = Get_Index_Current_workingChart();
            if (i == -1) return;
            Size s = new Size(0, 0);
            saveFileDialogImage.FileName = workingCharts[i].workingForm.Text.Substring(workingCharts[i].workingForm.Text.IndexOf(':') + 1);
            saveFileDialogImage.Filter = "PNG (*.png)| *.png";
            if (XMLConfig.Get("Capture_FileSavePath").Length > 0) saveFileDialogImage.InitialDirectory = XMLConfig.Get("Capture_FileSavePath");

            ChartV2.Control f = (ChartV2.Control)workingCharts[i].ActiveChart;
            f.Refresh();
            memoryImage = CaptureScreen(f, s, sender.ToString());

            if (saveFileDialogImage.ShowDialog() == DialogResult.OK)
            {
                memoryImage.Save(saveFileDialogImage.FileName, System.Drawing.Imaging.ImageFormat.Png);
                XMLConfig.Set("Capture_FileSavePath", Path.GetDirectoryName(saveFileDialogImage.FileName));
            }

        }

        private void toolStripMenuItemSaveFImage_Click(object sender, EventArgs e)
        {
            int i = Get_Index_Current_workingChart();
            if (i == -1) return;
            Size s = new Size(0, 0);
            MainForm mf = this;
            mf.Refresh();
            memoryImage = CaptureScreen(mf, s, "");
            saveFileDialogImage.FileName = "Working Form " + workingCharts[i].workingForm.Text.Substring(workingCharts[i].workingForm.Text.IndexOf(':') + 1);
            saveFileDialogImage.Filter = "PNG (*.png)| *.png";
            if (XMLConfig.Get("Capture_FileSavePath").Length > 0) saveFileDialogImage.InitialDirectory = XMLConfig.Get("Capture_FileSavePath");
            if (saveFileDialogImage.ShowDialog() == DialogResult.OK)
            {
                memoryImage.Save(saveFileDialogImage.FileName, System.Drawing.Imaging.ImageFormat.Png);
                XMLConfig.Set("Capture_FileSavePath", saveFileDialogImage.FileName);
            }

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
        
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Save_parameters.Clear();
            XMLConfig.Set("LoadLastSession", toolStripMenuItemLLS.Checked.ToString());
            XMLConfig.Set("LoadWithNewSearch", toolStripMenuItemLNS.Checked.ToString());
            XMLConfig.Set("ShowHypotheticalTarget", toolStripMenuItemSHT.Checked.ToString());
            XMLConfig.Set("ShowIndexCheckedModel", toolStripMenuItemSICM.Checked.ToString());
            XMLConfig.Set("MaxBarInChart", toolStripComboBoxMBiC.Text);
            XMLConfig.Set("ShowSourceName", toolStripMenuItemSDSN.Checked.ToString());
            XMLConfig.Set("EnableHistoryCache", toolStripMenuItemEHC.Checked.ToString());
            XMLConfig.Set("SaveModelsToFiles", SaveModelstoFiles.Checked.ToString());

            if (Get_Index_Current_workingChart() != -1)
            {
                smp = new Sample(ActiveChart);
                string FileName = Application.StartupPath + '\\' + "user_current_Sample.smp";
                BinaryFormatter formatter = new BinaryFormatter();
                using (Stream stream = File.Create(FileName))
                {
                    formatter.Serialize(stream, smp);
                    stream.Close();
                }
                Sample.currentSample = "user_current_Sample";
            }
       
            
            XMLConfig.Set("CurrentSample", Sample.currentSample);
        }


        private void toolStripMenuItemMBiC_DropDownClosed(object sender, EventArgs e)
        {
            int i;
            try
            {
                i = Convert.ToInt32(toolStripComboBoxMBiC.Text);
                if ((i >= 100) & (i <= 1000) & (i != ChartV2.Data.ViewPort.maxBarInChart))
                {
                    ChartV2.Data.ViewPort.maxBarInChart = i;
                    ActiveChart.seriesList[0].viewPort.update();
                    ActiveChart.ReDrawEveryThing(true);
                }
                else toolStripComboBoxMBiC.Text = ChartV2.Data.ViewPort.maxBarInChart.ToString();
            }
            catch
            {
                toolStripComboBoxMBiC.Text = ChartV2.Data.ViewPort.maxBarInChart.ToString();
            }


        }


        private void toolStripComboBoxMBiC_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.toolsToolStripMenuItem.HideDropDown();
                toolStripMenuItemMBiC_DropDownClosed(sender, e);
            }

        }

        private void toolStripMenuItem_Statistic_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null) return;
            StatCalc stat = new StatCalc();
            stat.Describe(workingCharts[Get_Index_Current_workingChart()]);
        }


        private void toolStripMenuItemSaveLImage_Click(object sender, EventArgs e)
        {
            toolStripMenuItemSaveImage_Click(sender, e);
        }

        private void toolStripMenuItemSHT_CheckedChanged(object sender, EventArgs e)
        {
            int i = Get_Index_Current_workingChart();
            if (i == -1) return;
            ShowHypotheticalTarget = false;
            if (toolStripMenuItemSHT.Checked == true) ShowHypotheticalTarget = true;
            ChartV2.Control c = (ChartV2.Control)workingCharts[i].ActiveChart;
            c.ReDrawEveryThing(false);
        }

        private void toolStripMenuItemSICM_CheckedChanged(object sender, EventArgs e)
        {
            int i = Get_Index_Current_workingChart();
            if (i == -1) return;
            ShowIndexCheckedModel = false;
            if (toolStripMenuItemSICM.Checked == true) ShowIndexCheckedModel = true;
            ChartV2.Control c = (ChartV2.Control)workingCharts[i].ActiveChart;
            c.ReDrawEveryThing(false);
        }

        /// <summary>
        /// очистка списков моделей и ГЦ на всех фреймах для данного символа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_ClearModels_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                int i = Get_Index_Current_workingChart();
                if (i != -1)
                {
                    if (workingCharts[i].flagSerch)
                    {
                        workingCharts[i].ClearModels();
                        Set_Switch_Search_models();
                        workingCharts[i].flagSerch = false;
                    }
                }
            }
        }

        private void enableByTrendFiltrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelManager.Options.EnableFiltrationByTrend = ((ToolStripMenuItem)sender).Checked;
        }

        private void enableFanFiltrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelManager.Options.EnableFiltrationFan = ((ToolStripMenuItem)sender).Checked;
        }

        private void toolStripMenuItemSDSN_CheckedChanged(object sender, EventArgs e)
        {
            ShowSourceName = false;
            if (toolStripMenuItemSDSN.Checked == true) ShowSourceName = true;
        }

        private void toolStripMenuItemEHC_CheckedChanged(object sender, EventArgs e)
        {
            EnableHistoryCache = toolStripMenuItemEHC.Checked;
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripMenuSaveSample.Enabled = true;
            toolStripMenuDefault.Enabled = true;
            for (int i = 5; i < toolStripMenuSample.DropDownItems.Count; i++)
            {  ToolStripMenuItem t = (ToolStripMenuItem)toolStripMenuSample.DropDownItems[i];
                t.Enabled = true; }
            if (Get_Index_Current_workingChart() == -1)
            {
                toolStripMenuSaveSample.Enabled = false;
                toolStripMenuDefault.Enabled = false;
                for (int j = 5; j < toolStripMenuSample.DropDownItems.Count; j++)
                { ToolStripMenuItem t = (ToolStripMenuItem)toolStripMenuSample.DropDownItems[j];
                    t.Enabled = false;}
            }


        }
 //=========== Работа с меню File --> Sample =====================================
        ToolStripMenuItem sampleMenuDrop;
        readonly private int CountDropMenu = 5;
        private void Add_menuItem_menuSample(string nameSample, bool Save_Remove)
        {
            sampleMenuDrop = new ToolStripMenuItem();
            sampleMenuDrop.Name = sampleMenuDrop.Text =  nameSample;
            sampleMenuDrop.Click += new EventHandler(sampleMenuDrop_Click);
            if (Save_Remove)
            {             
                toolStripMenuSample.DropDownItems.Add(sampleMenuDrop);
                if (nameSample == Sample.currentSample) sampleMenuDrop.Checked = true;
            }
            else toolStripMenuRemoveSample.DropDownItems.Add(sampleMenuDrop);
         }

        void sampleMenuDrop_Click(object sender, EventArgs e)
        {
           ToolStripMenuItem tsm = (ToolStripMenuItem)sender;
            if (tsm.OwnerItem.Name == "toolStripMenuRemoveSample")
            {
                if (tsm.Name == Sample.currentSample)
                { MessageBox.Show("You can't remove a current samle. \n Please, switch sample and try again.", "Attention.", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                if (MessageBox.Show("Sample " + "'" + tsm.Name + "'" + " will be removed!", "Warning.", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel) return;
                {
                    toolStripMenuRemoveSample.DropDownItems.Remove(tsm);
                    for (int i = 0; i < toolStripMenuSample.DropDownItems.Count; i++)
                        if (toolStripMenuSample.DropDownItems[i] is ToolStripMenuItem)
                            if (toolStripMenuSample.DropDownItems[i].Name == tsm.Name)
                            {
                                toolStripMenuSample.DropDownItems.Remove(toolStripMenuSample.DropDownItems[i]);
                                try { File.Delete(Application.StartupPath + "\\" + tsm.Name + ".smp"); }
                                catch (Exception ex) { MessageBox.Show(ex.Message); }
                                return;
                            }
                }
            }
            if (tsm.OwnerItem.Name == "toolStripMenuSample")
                
                {
                    int i = Get_Index_Current_workingChart();
                    if (i == -1) return;
                    MenuSampleCheckedClear();
                    tsm.Checked = true;
                    toolStripMenuDefault.Checked = false;
                    Sample.currentSample = tsm.Name;
                    SetSample(Application.StartupPath + "\\" + Sample.currentSample + ".smp");                    
                }
        }
           
        private void MenuSampleCheckedClear ()
        {
            for (int j = CountDropMenu; j < toolStripMenuSample.DropDownItems.Count; j++)
                  ((ToolStripMenuItem)toolStripMenuSample.DropDownItems[j]).Checked = false; 
                        
                    
        }

        private void SetSample(string fileName)
        {

            if (File.Exists(fileName))
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (Stream stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                    {
                        if (stream.Length == 0) return;
                        if (Get_Index_Current_workingChart() != -1)
                        {
                            ChartV2.Control c = ActiveChart;
                            smp = new Sample((Sample)formatter.Deserialize(stream), c);
                            stream.Close();
                            RefreshSample(c);
                        }
                        else smp = new Sample((Sample)formatter.Deserialize(stream));
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show("Wrong format file: " + ee.Message);
                    toolStripMenuDefault.Checked = true;
                    Skilful.Sample.currentSample = "Default";

                }
            else toolStripMenuDefault.Checked = true;
        }

        private void toolStripMenuDefault_Click(object sender, EventArgs e)
        {
            int i = Get_Index_Current_workingChart();
            if (i == -1) return;
            
            ChartV2.Control c = ActiveChart;
            Sample.currentSample = "Default";
            Sample.DefaultSample(c);
            RefreshSample(c);
            MenuSampleCheckedClear();
            toolStripMenuDefault.Checked = true;
            workingCharts[i].SetChartType(ChartType.Candle);
         }

        private void RefreshSample(ChartV2.Control c)
        {
            c.ReDrawEveryThing(true);
        }


        private void toolStripMenuSaveSample_Click(object sender, EventArgs e)
        {
            
            string nameFile;
            saveFileDialogSample.FileName = "";
            saveFileDialogSample.InitialDirectory = Application.StartupPath;
            saveFileDialogSample.Filter = "SMP (*.smp)| *.smp";
            if (saveFileDialogSample.ShowDialog() == DialogResult.OK)
            {
                smp = new Sample(ActiveChart); 
                BinaryFormatter formatter = new BinaryFormatter();
                using (Stream stream = File.Create(saveFileDialogSample.FileName))
                {
                    formatter.Serialize(stream, smp);
                    stream.Close();
                }
                nameFile = saveFileDialogSample.FileName;
                nameFile = nameFile.Substring(nameFile.LastIndexOf((char)'\\') + 1,nameFile.LastIndexOf('.') - nameFile.LastIndexOf((char)'\\') - 1);
                MenuSampleCheckedClear();
                toolStripMenuDefault.Checked = false;  
                Sample.currentSample = nameFile;
                for (int i = 0; i < toolStripMenuRemoveSample.DropDownItems.Count; i++)
                    if (nameFile == ((ToolStripMenuItem)toolStripMenuRemoveSample.DropDownItems[i]).Name)
                        toolStripMenuRemoveSample.DropDownItems.RemoveAt(i);
                for (int j = CountDropMenu; j < toolStripMenuSample.DropDownItems.Count; j++)
                    if (nameFile == ((ToolStripMenuItem)toolStripMenuSample.DropDownItems[j]).Name)
                        toolStripMenuSample.DropDownItems.RemoveAt(j);      


                Add_menuItem_menuSample(nameFile, true);
                Add_menuItem_menuSample(nameFile, false);
                
            }
        }

        private void upDate_Click(object sender, EventArgs e)
        {
            toolStripMenuItem_ClearModels_Click(sender, e);
            SearchModelsToolStripMenuItem_Click(sender, e);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            QuotesPlayer QPform = new QuotesPlayer();
            this.AddOwnedForm(QPform);
            QPform.Show();
        }


        // ======== Методы работы с открытым интерфейсом SmartCOM http://www.itinvest.ru/software/smartcom/ через класс ITinvest
        
        /*ITinvest ITinv;

        private void toolMenuConnect_Click(object sender, EventArgs e)
        {
            if (ITinv != null)
                if (ITinv.SmartCOM.IsConnected())
                {
                    MessageBox.Show("Connect's, Ok");
                   return;
                }
            ITinvest_password ITinv_connect = new ITinvest_password();
            if (ITinv_connect.ShowDialog() != DialogResult.OK) return;
            if (ITinv == null)
                ITinv = new ITinvest();
            ITinv.Connect(ITinvest.ip, 8090, ITinvest.login, ITinvest.password);
           
          
              
        }

        private void toolMenuItemListSymbol_Click(object sender, EventArgs e)
        {
            if (ITinv != null)
                if (ITinv.SmartCOM.IsConnected())
                {
                    ITinv.Get_listing();
                }
                else
                {
                    MessageBox.Show("The servers disconnect, please, try connect.");
                }
        }
        
        private void toolMenuItemSaveFile_Click(object sender, EventArgs e)
        {
            if (ITinv != null)
                ITinv.Save_symbols_list_to_file();
        }

        private void toolMenuItemChooseSymbols_Click(object sender, EventArgs e)
        {
            if (ITinv == null) return;
            Symbol_list_ITinvest sli = new Symbol_list_ITinvest(ITinv);      
            if (sli.ShowDialog() == DialogResult.OK)
            {
                
            }

        }

        private void toolMenuItemDisconnect_Click(object sender, EventArgs e)
        {
            if (ITinv != null)
                ITinv.Disconnect();
        }

*/         

      //  ==========================================================================================================

    }
}
       
    
