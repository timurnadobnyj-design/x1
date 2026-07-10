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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using Skilful.Data;
using Skilful.QuotesManager;
using ChartV2;
using ChartV2.Data;
using System.Threading;


namespace Skilful
{
    public delegate void Search_Complete();
   
    public class Working
    {
        public Form workingForm;
        public ProgressBar _progressBar;
        public Button button_Searh_Stop;
        public ModelManager.ModelsManager MM;
        public Skilful.QuotesManager.TQuotes Quotes;
        public BackgroundWorker bgWorker;
        public event Search_Complete Button_Search_Complete_;
        public int SearchIteration;
        public bool flagSerch; // флаг поиска моделей, true поиск в данном workingChart-е был произведен
        double last_price;
        DateTime last_time;
        public TF tf;
        public int actualTF;
        public TreeNode tn;
        ChartV2.Control activeChart;
        public ChartV2.Control ActiveChart{get{return activeChart;}}

        public void Control_Button_Search_Complete_()
        {
            Button_Search_Complete_();
        }

        private Guid _guid = new Guid();
        private string _symbol = "";
        
        /// <summary>
        /// Return Guid
        /// </summary>
        public Guid Uid
        {
            get
            {
                return _guid;
            }
            set
            {
                _guid = value;
            }
        }

        /// <summary>
        /// Return Symbol
        /// </summary>
        public string Symbol
        {
            get
            {
                return _symbol;
            }
            set
            {
                _symbol = value;
            }
        }

        public Working()
        {
            //--- создаем форму в которой все и будет
            workingForm = new Form();
            // Bitmap cbitmap = (Bitmap)Skilful.Properties.Resources.chart;
            //IntPtr hIcon = cbitmap.GetHicon();
            // workingForm.Icon = Icon.FromHandle(hIcon);
            workingForm.Icon = Skilful.Properties.Resources.chart_ico;

            Add_ChartV2Control_to_workingForm();

            // ---- добавляем в форму Прогресс бар
            _progressBar = new ProgressBar();
            _progressBar.Dock = DockStyle.Bottom;
            _progressBar.Visible = false;
            _progressBar.Value = 0;
            _progressBar.Maximum = 100;
            _progressBar.Step = 10;
            workingForm.Controls.Add(_progressBar);
            workingForm.Size = new Size(600, 300);
            workingForm.WindowState = FormWindowState.Maximized;
            //-- флаг поиска моделей, true поиск в данном workingChart-е был произведен
            flagSerch = false;
        }


        List<TF> activeChart_Active_Charts_(ChartType ct)
        {
            List<TF> ltf = new List<TF>();
            Form f = workingForm;
            
            for (int k = 0; k < f.Controls.Count; k++)
            {
                if (f.Controls[k].ToString() != "ChartV2.Control") continue;
                ChartV2.Control c = (ChartV2.Control)f.Controls[k];
                if (c.seriesList[0].legend.chartType == ct) ltf.Add(c.seriesList[0].legend.frame);

            }
            return ltf;
        }
        // // // // // // // // // // // // // // // // // // // // // // // // // // // // //
        public void RefreshTicksInfo(int counter)
        {
            //перерисовка
            if(counter == 0)
            if (activeChart.InvokeRequired)
            {
                activeChart.Invoke(new RefreshPlot(() => activeChart.redrawPlot()));
                activeChart.Invoke(new RefreshTopAxis(() => activeChart.redrawTopAxis()));
            }
            else
            {
                activeChart.redrawPlot();
                activeChart.redrawTopAxis();
            }
        }
        public void RefreshChart(int counter)
        {
            //перерисовка
            if (counter == 0)
            if (activeChart.InvokeRequired)
                activeChart.Invoke(new RefreshPlot(() => activeChart.redrawPlot()));
            else
                activeChart.redrawPlot();
        }
        public void NewTick(string SourceName, string SymbolName, DateTime time, double bid, double ask)
        {
            if (Quotes.Symbol.moduleName == SourceName && Quotes.Symbol.Name == SymbolName)
            {
                //нюанс при работе с модулем МТ4
                // - каждый тик приходит в два шага 1=бид, 2=аск
                if (SourceName == "MT4")
                {
                    if (last_time != null && time == last_time)
                    {
                        if (ask == 0 && bid != 0)
                            ask = last_price;
                        else if (bid == 0 && ask != 0)
                            bid = last_price;
                    }
                    else if (bid == 0 || ask == 0)
                    {
                        last_time = time;
                        last_price = bid != 0 ? bid : ask;
                        return;
                    }
                }
                //передача тика чарту
                for (int i = workingForm.Controls.Count-1; i >= 0; i--)
                {
                    ChartV2.Control control = null;
                    Series sr = null;

                    if (workingForm.Controls[i].ToString() != "ChartV2.Control") continue;
                    control = (ChartV2.Control)workingForm.Controls[i];
                    if (control.seriesList == null) continue;

                    sr = control.seriesList[0];
         
                    //вывод информации о тиках
                    sr.NewTick(time, bid, ask);

                    TQuotes quotes = null;
                    quotes = sr.info;
                    if (sr.viewPort.VPMaxTime >= (quotes.GetCount() - 2))
                    {
                        double padding = 5 * quotes.Symbol.pip;
                        double lastBarHigh = sr.data[sr.data.Count - 1].High,
                               lastBarLow = sr.data[sr.data.Count - 1].Low;
                        if (quotes.log)
                        {
                            lastBarHigh = Math.Log10(Math.Pow(10, lastBarHigh) + padding);
                            lastBarLow = Math.Log10(Math.Pow(10, lastBarLow) - padding);
                        }
                        else
                        {
                            lastBarHigh += padding;
                            lastBarLow -= padding;
                        }

                        if (sr.viewPort.VPMaxPrice < lastBarHigh) sr.viewPort.VPMaxPrice = (float)lastBarHigh;
                        if (sr.viewPort.VPMinPrice > lastBarLow) sr.viewPort.VPMinPrice = (float)lastBarLow;

                        RefreshTicksInfo(i);
                    }
                }
            }
        }
        /// <summary>
        /// событие инициируется при добавлении нового бара через датасурс
        /// </summary>
        /// <param name="SourceName"></param>
        /// <param name="SymbolName"></param>
        /// <param name="frame"></param>
        /// <param name="bar"></param>
        public void NewBar(string SourceName, string SymbolName, TF frame, int idx)
        {
            //проверка адреса назначения бара (тот ли это чарт)
            if (Quotes.Symbol.moduleName == SourceName && Quotes.Symbol.Name == SymbolName)
            {
                for (int i = workingForm.Controls.Count - 1; i >= 0; i--)
                {
                    if (workingForm.Controls[i].ToString() != "ChartV2.Control") continue;
                    ChartV2.Control control = workingForm.Controls[i] as ChartV2.Control;
                    if (control.seriesList == null) continue;
                    TQuotes quotes = control.seriesList[0].info;

                    if (quotes.tf == frame)
                    {
                        bool shifting = quotes.lastindex >= quotes.Quotes.Count;

                        //появился ли новый бар
                        //обновление индeксатора
                        quotes.trend.Analyser(quotes, idx);
                        //int j = quotes.lastindex - quotes.trend.b7 * 2 - 1;
                        //if(j>=0) quotes[j].BarStatus = quotes.trend.map[j].BarStatus;

                        //Обновление моделей
                        ///if(Quotes.MM!=null)
                        ////    Quotes.MM.SeekModels(Quotes, i, i, 4, ModelManager.Options.CheckedErrors, null, null);

                        //сдвиг графика влево на один бар
                        if (shifting && control.seriesList[0].viewPort.VPMaxTime >= (quotes.GetCount() - 2))
                        {
                            control.seriesList[0].viewPort.VPMaxTime++;
                            control.seriesList[0].viewPort.VPMinTime++;
                        }
                        //сжатие графика по вертикали
                        if (control.seriesList[0].data.Count > 2)
                        {
                            TBar prev = control.seriesList[0].data[control.seriesList[0].data.Count - 2];
                            ViewPort VP = control.seriesList[0].viewPort;

                            double padding = 5 * quotes.Symbol.pip;
                            double lastBarHigh = quotes[quotes.lastindex].High,
                                   lastBarLow = quotes[quotes.lastindex].Low;
                            if (quotes.log)
                            {
                                lastBarHigh = Math.Log10(Math.Pow(10, lastBarHigh) + padding);
                                lastBarLow = Math.Log10(Math.Pow(10, lastBarLow) - padding);
                            }
                            else
                            {
                                lastBarHigh += padding;
                                lastBarLow -= padding;
                            }

                            if (VP.VPMaxPrice < lastBarHigh && VP.VPMaxPrice > prev.High) VP.VPMaxPrice = (float)lastBarHigh;
                            if (VP.VPMinPrice > lastBarLow && VP.VPMinPrice < prev.Low) VP.VPMinPrice = (float)lastBarLow;
                        }
                        //обновление чарта
                        //control.flagSerch = false;
                      //  flagSerch = false;
                        RefreshChart(i);
                    }
                }
            }
        }

        public void RefreshHistory(string SourceName, string SymbolName)
        {
            if (Quotes.Symbol.moduleName == SourceName && Quotes.Symbol.Name == SymbolName)
            {
                for (int i = workingForm.Controls.Count - 1; i >= 0; i--)
                {
                    if (workingForm.Controls[i].ToString() != "ChartV2.Control") continue;
                    ChartV2.Control control = workingForm.Controls[i] as ChartV2.Control;
                    if (control.seriesList == null) continue;
                    TQuotes quotes = control.seriesList[0].info;

                    quotes.trend.Analyser(quotes, 0);
                    //for (int j = 0; j < quotes.GetCount(); j++) quotes[j].BarStatus = quotes.trend.map[j].BarStatus;

                    control.seriesList[0].viewPort.update();
                    control.ReDrawEveryThing(true);
                }
            }
        }
        //------- загрузка данных из класса TQuotes в объект Quotes.trend класса Trend
        public bool Get_ManagedData(string ChartName, string SymbolName, TQuotes quotes)
        {
            if (quotes == null) return true;
            Logging.Log("   Preparing data started");
            //MainForm.MT4messagesForm.AddMessage("   Preparing data started");
            workingForm.Text = ChartName;
            Quotes = quotes;
            
            //инициализация массива экстремумов
            if (Quotes.trend == null)
            {
                Quotes.trend = new Trend(SymbolName);
                Quotes.trend.Analyser(Quotes, 0);

                //for (int i = 0; i < Quotes.GetCount(); i++) Quotes[i].BarStatus = Quotes.trend.map[i].BarStatus;
            }
            else 
            if (Quotes.MM != null)
            {
                activeChart.ModelList2 = Quotes.MM.Models;  //ахтунг, это не дублирование а инициализация специальных массивов для рисования на графике
                activeChart.HTList = Quotes.MM.HTangentLines;
                //activeChart.flagSerch = true;
                //Button_Search_Complete_();
            }

            
            Logging.Log("   Preparing data ended");
            //MainForm.MT4messagesForm.AddMessage("   Preparing data ended");
            return false;
        }
        //------- загрузка данных из файла в объект Quotes.trend класса Trend
        public bool Get_Data_Off_Line(string nameFile, string pathFile)
        {
            Logging.Log("   Preparing data started");
            //MainForm.MT4messagesForm.AddMessage("   Preparing data started");
            StreamReader sr = new StreamReader(nameFile);
            // ---- получение имени инструмента из имени файла
            workingForm.Text = nameFile.Substring(nameFile.LastIndexOf((char)'\\') + 1,
                        nameFile.IndexOf('.') - nameFile.LastIndexOf((char)'\\') - 1);

            

            string ResultString = sr.ReadToEnd();
            sr.Dispose();
            
            Quotes = new TQuotes();
            Trend CurrentTrend = Quotes.trend = new Trend(nameFile, pathFile);
            if (CurrentTrend == null)
            {
                Logging.Log("Get_Data_Off_Line: Unable to create class Trend instance\n");
                //MainForm.MT4messagesForm.AddMessage("Get_Data_Off_Line: Unable to create class Trend instance\n");
                return false;
            }
            int rowsCount = CurrentTrend.end;

            // вызов окна выбора формата файла
            DataWorkShopWindow window = new DataWorkShopWindow(rowsCount, ResultString);
            if (window != null)
            {
                if (window.ShowDialog() == DialogResult.OK)
                {
                    //window.ShopsDs.SetMinMax();
                    //window.ShopsDs.CalculateScale(PlotAreaPictureBox.ClientRectangle, "XY");
                    //dc.Add(window.ShopsDs);

                    window.Close();
                    if (window.ds.data != null)
                    {
                        //Quotes.Quotes = window.ds.data;
                        window.SaveConverted("conv_" + workingForm.Text+".csv");
                    }
                    else
                    {
                        Logging.Log("   Loading data failed");
                        return false;
                    }
                    // window.ds.bmpForMap = plot.DrawSeriesNavigationMap(window.ds);
                    //seriesList.Add(window.ds);


                    window.Dispose();
                }
            }

           // if (CurrentTrend.Analyser(Quotes,0) != 1) return true;
            // Перенос данных из trend в  TQuotes
            /*for (int i = 0; i < CurrentTrend.map.Count; i++)
            {
                TBar NewBar = new TBar();
                NewBar.DT = CurrentTrend.map[i].DT;
                NewBar.Bar = CurrentTrend.map[i].Bar;
                NewBar.X = CurrentTrend.map[i].X;
                NewBar.Open = CurrentTrend.map[i].Open;
                NewBar.High = CurrentTrend.map[i].High;
                NewBar.Low = CurrentTrend.map[i].Low;
                NewBar.Close = CurrentTrend.map[i].Close;
                NewBar.BarStatus = CurrentTrend.map[i].BarStatus;
                Quotes.Add(NewBar);
            }*/
            //Quotes.calcFrame();
            Logging.Log("   Preparing data ended");
            //MainForm.MT4messagesForm.AddMessage("   Preparing data ended");
            return false;
        }

        void SetActiveChart(ChartV2.Control chart)
        {
            for (int i = 0; i < workingForm.Controls.Count; i++)
            {
                if (workingForm.Controls[i].ToString() != "ChartV2.Control") continue;
                workingForm.Controls[i].Visible = false;
                workingForm.Controls[i].Enabled = false;
            }
            chart.Visible = true;
            chart.Enabled = true;
            activeChart = chart;
        }
        //------- Переключение между фреймами
        public void SetChartTF(TF frame)
        {
            SwitchChart(activeChart.seriesList[0].legend.chartType, frame);
        }
        //------- Переключение между стилями чарта{Candle|Line|X0}//(отображение свечей или баров - функция самого чарта)
        public void SetChartType(ChartType ctype)
        {
            if (!_progressBar.Visible) //в процессе нельзя т.к. возможен конфликт с доступом к прогрессбару
                SwitchChart(ctype, activeChart.seriesList[0].legend.frame);
        }
        //возвращает заданный чарт
        ChartV2.Control GetChart(ChartType ctype, TF frame)
        {
            for (int i = 0; i < workingForm.Controls.Count; i++)
            {
                if (workingForm.Controls[i].ToString() != "ChartV2.Control") continue;
                ChartV2.Control chart = (ChartV2.Control)workingForm.Controls[i];
                if (chart.seriesList[0].legend.chartType == ctype
                 && (chart.seriesList[0].legend.frame == frame
                 ///|| frame == TF.custom
                 ))// ^^^ тут временная затычка, надо будет разобраться позже
                {
                    return chart;
                }
            }
            return null;
        }
        //------- Переключение между фреймами и стилями чарта, при необходимости создается новый чарт
        public void SwitchChart(ChartType ctype, TF frame)
        {
            
            if (Quotes.Symbol == null) return;
            if (TSymbol.getTF(Quotes.Symbol.customTF) > frame) return;
            tf = frame;
            //Get quotes corresponding charttype
            Quotes = Quotes.SwitchTo(frame, ctype);
            
            //Select existing chart//
            ChartV2.Control chart = GetChart(ctype, frame);
            if (chart != null)
            {
                SetActiveChart(chart);
                //-- флаг поиска моделей, true поиск в данном workingChart-е был произведен

                if (chart.plot.modelsToDrawList == null) flagSerch = false;
                else flagSerch = true;
                    
               
                    
                    // flagSerch = chart.ModelListExist;
                //activeChart.flagSerch = chart.ModelListExist;
                if (flagSerch) Button_Search_Complete_();
                //if (!activeChart.flagSerch) Button_Search_Complete_();
                return;
            }

            //Or create new one//
            //from constructor Working()
            // -----добавляем в форму Контрол, в котором будет рисоватся график и модели
            activeChart.Visible = activeChart.Enabled = false;
            Add_ChartV2Control_to_workingForm();
            //from Get_ManagedData()
            //------- загрузка данных из класса TQuotes в объект Quotes.trend класса Trend
            //инициализация массива экстремумов
            if (Quotes.trend == null)
            {
                Quotes.trend = new Trend(Quotes.Symbol.Name);
                Quotes.trend.Analyser(Quotes, 0);

                if (Quotes.MM != null)
                {
                    activeChart.ModelList2 = Quotes.MM.Models;  //ахтунг, это не дублирование а инициализация специальных массивов для рисования на графике
                    activeChart.HTList = Quotes.MM.HTangentLines;
                }
                 

                //for (int i = 0; i < Quotes.GetCount(); i++) Quotes[i].BarStatus = Quotes.trend.map[i].BarStatus;
            }
            else if (Quotes.MM != null)
            {
                activeChart.ModelList2 = Quotes.MM.Models;  //ахтунг, это не дублирование а инициализация специальных массивов для рисования на графике
                activeChart.HTList = Quotes.MM.HTangentLines;
            }
            
            //from Plot_Chart()
            //------- рисование графика 
            activeChart.seriesList = new List<ChartV2.Data.Series>();
            activeChart.seriesList.Add(new Series(Quotes, Color.MediumSpringGreen));
            activeChart.seriesList[0].viewPort = new ViewPort(activeChart.seriesList[0]);
            activeChart.ReDrawEveryThing(true);

            //-- флаг поиска моделей, true поиск в данном workingChart-е был произведен
            // flagSerch = Quotes.MM == null ? false : true;
            
            //activeChart.flagSerch = Quotes.MM == null ? false : true;
            Button_Search_Complete_();
           
        }
        //------- рисование графика 
        public void Plot_Chart()
        {
            ChartV2.Control chartControl = (ChartV2.Control)workingForm.Controls[0];
            if (chartControl.seriesList == null)
            {
                chartControl.seriesList = new List<ChartV2.Data.Series>();
                char[] digits  = {'0','1','2','3','4','5','6','7','8','9'};
                if (Quotes.Symbol == null)
                    chartControl.seriesList.Add(new ChartV2.Data.Series(Quotes.Quotes, new ChartV2.Legend(workingForm.Text, Color.Black, Skilful.Sample.ChartType_, TF.custom)));
                else
                {
                    while (Quotes.Symbol.Frames[(int)Quotes.chartType, (int)Quotes.tf].GetCount() == 0)
                           Quotes = Quotes.Symbol.Frames[(int)Quotes.chartType, (int)Quotes.tf + 1];
                    
                    chartControl.seriesList.Add(new Series(Quotes, Color.LightGoldenrodYellow));
                }
                chartControl.seriesList[0].viewPort = new ChartV2.Data.ViewPort(chartControl.seriesList[0]);
            }         
           chartControl.ReDrawEveryThing(true); 
        }
              
        public void Stop_Search()
        {
            flagSerch = false;
            //activeChart.flagSerch = false;

            if (bgWorker != null && bgWorker.IsBusy) bgWorker.CancelAsync();
            //if (activeChart.bgWorker != null && activeChart.bgWorker.IsBusy) activeChart.bgWorker.CancelAsync();           
            
             _progressBar.Value = 0;
             _progressBar.Visible = false;
        }
        // -----поиск моделей 

        //public void SearchModel()   //! Поиск моделей и заполнение Quotes.MM
        //{
        //    activeChart.flagSerch = true;
        //    _progressBar.Value = 0;
        //    _progressBar.Visible = true;

        //    MM = Quotes.MM = new Skilful.ModelManager.ModelsManager();
        //    MM.Models.Clear();
        //    // ----- создаем поток в которм будет выполнятся поиск моделей 
        //    activeChart.bgWorker = new BackgroundWorker();
        //    activeChart.bgWorker.WorkerReportsProgress = true;
        //    activeChart.bgWorker.WorkerSupportsCancellation = true;
        //    activeChart.bgWorker.DoWork += _backgroundWorker_DoWork;
        //    activeChart.bgWorker.ProgressChanged += activeChart._backgroundWorker_ProgressChanged;
        //    activeChart.bgWorker.RunWorkerCompleted += activeChart._backgroundWorker_RunWorkerCompleted;
        //    activeChart.Button_Search_Complete__ += new Button_Search_Complete_(Control_Button_Search_Complete_);
        //    // Поиск иоделей и разиещение их в ModelManager.Models. 
        //    activeChart.bgWorker.RunWorkerAsync(-1);
        //}

        /// <summary>
        /// поиск моделей последовательно на всех фреймах
        /// </summary>
        /// <param name="iteration">индекс очередного фрейма в Frames[,], он же (int)TF</param>
        public void SearchModel2(bool first_iteration, bool search_background)   //! Поиск моделей и заполнение Quotes.MM
        {
            _progressBar.Value = 0;
            if (first_iteration)
            {
                SearchIteration = Quotes.Symbol == null ? 0 : Quotes.Symbol.Frames.GetLength(1) - 1;
                //activeChart.flagSerch = true;
                flagSerch = true;
                _progressBar.Visible = true;

                // ----- создаем поток в которм будет выполнятся поиск моделей 
                if (search_background)
                {
                    bgWorker = new BackgroundWorker();
                    bgWorker.WorkerReportsProgress = true;
                    bgWorker.WorkerSupportsCancellation = true;
                    bgWorker.DoWork += _backgroundWorker_DoWork;
                    bgWorker.ProgressChanged += _backgroundWorker_ProgressChanged;
                    bgWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
                    //activeChart.bgWorker = new BackgroundWorker();
                    //activeChart.bgWorker.WorkerReportsProgress = true;
                    //activeChart.bgWorker.WorkerSupportsCancellation = true;
                    //activeChart.bgWorker.DoWork += _backgroundWorker_DoWork;
                    //activeChart.bgWorker.ProgressChanged += activeChart._backgroundWorker_ProgressChanged;
                    //activeChart.bgWorker.RunWorkerCompleted += activeChart._backgroundWorker_RunWorkerCompleted;
                }
            }
            else //не первая итерация, соответственно модели для данного фрейма посчитаны и можно проинитить данные для отрисовки на графике
            {
                ChartV2.Control chart = GetChart(Quotes.chartType, Quotes.Symbol == null ? TF.custom : (TF)SearchIteration);
                if (chart != null)
                {
                    TQuotes q = Quotes.Symbol == null ? Quotes : Quotes.Symbol.Frames[(int)Quotes.chartType, SearchIteration]; //получил котировки для текущего фрейма
                    chart.ModelList2 = q.MM.Models;
                    chart.HTList = q.MM.HTangentLines;

                    if (chart == activeChart) chart.ReDrawEveryThing(true);
                }
                SearchIteration--;
            }

            //запуск поиска DoWork
            if (SearchIteration >= 0)
            {
                TQuotes q = Quotes.Symbol == null ? Quotes : Quotes.Symbol.Frames[(int)Quotes.chartType, SearchIteration]; //получил котировки для текущего фрейма
                
                // Поиск моделей и размещение их в ModelManager.Models. 
                if (search_background)
                    bgWorker.RunWorkerAsync(q);
                    //activeChart.bgWorker.RunWorkerAsync(q);
                else
                    _backgroundWorker_DoWork(null, new DoWorkEventArgs(q));

                //рекурсивный вызов для всех фреймов [SearchIteration--] ! только в этом потоке, при запуске в отдельном потоке этот вызов находится в _backgroundWorker_RunWorkerCompleted
                if (!search_background) SearchModel2(false, false);
            }
            else //конец цикла
            {
                //total completed
                if (_progressBar.Visible)
                {
                    _progressBar.Visible = false;
                    Button_Search_Complete_();
                }
            }
        }

        public void LoadModels(string FileName) //! Чтение списка моделей из файла
        {
            MM = Quotes.MM = new ModelManager.ModelsManager();
            // Загрузка моделей из файла
            if (MM.LoadFromFile(Quotes, FileName))
            {
                activeChart.ModelList2 = MM.Models;
                activeChart.HTList = MM.HTangentLines;
                activeChart.ReDrawEveryThing(true);
                Button_Search_Complete_();
            }
            else
            {
                MessageBox.Show("File is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            TQuotes q = e.Argument as TQuotes;

            //инициализация экстремумов
            if (q.trend == null)
            {
                q.trend = new Trend(q.Symbol.Name);
                q.trend.Analyser(q, 0);
                //for (int i = 0; i < q.GetCount(); i++) q[i].BarStatus = q.trend.map[i].BarStatus;
            }
            //поиск моделей
            q.MM = new Skilful.ModelManager.ModelsManager();
            switch (ModelManager.Options.SeekMode)
            {
                case 1: // Поиск последовательностей моделей
                    {
                        q.MM.SeekModelsSeq(q, Skilful.ModelManager.Options.Extremum1 * 3, q.GetCount() - 1, q.Decimals, worker, e);
                        // q.MM.SeekModelsSeq(q, 2517, q.GetCount() - 1, 4, worker, e);
                        break;
                    }
                case 2: // Поиск моделей от каждого экстремума
                    {
                        q.MM.SeekModels(q, Skilful.ModelManager.Options.Extremum1 * 3, q.GetCount() - 1, q.Decimals, worker, e);
                        //if (q.tf == TF.m60)
                        //    q.MM.SeekModels(q, 5094, q.GetCount() - 1, q.Decimals, worker, e);
                        break;
                    }
            }

            //инициализация моделей в дереве и их точек на младшем фрейме
            if(q.Symbol != null) q.Symbol.MTree.initFrame(q);
        }

        public void ClearModels()
        {
            for (int i = workingForm.Controls.Count - 1; i >= 0; i--)
            {
                if (workingForm.Controls[i].ToString() != "ChartV2.Control") continue;
                ChartV2.Control control = workingForm.Controls[i] as ChartV2.Control;
                if (control.seriesList == null) continue;
               
                TQuotes quotes = control.seriesList[0].info;

                if (control.plot.modelsToDrawList != null)
                {
                    quotes.MM.Models.Clear();
                    quotes.MM.HTangentLines.Clear();
                    
                    if (control.plot.HTtoDrawList != null)
                        control.plot.HTtoDrawList.Clear();
                    control.plot.modelsToDrawList.Clear();
                    control.rightAxis.targetLable.Clear();
                    control.ReDrawEveryThing(true);
                }
            }
        }
        public void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _progressBar.Value = _progressBar.Maximum - e.ProgressPercentage;
            _progressBar.Visible = true;
        }
        public void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SearchModel2(false, true);
        }

        public void Add_ChartV2Control_to_workingForm()
        {
            // -----добавляем в форму Контрол, в котором будет рисоватся график и модели
            activeChart = new ChartV2.Control(workingForm.Size, new Point(0, 0), this);
            activeChart.Active_Charts_ += new Active_Charts(activeChart_Active_Charts_);
            
            activeChart.rightAxis.style.magorFontBrush = new SolidBrush(Skilful.Sample.AxisStyleRight_Font_Color);
            activeChart.leftAxis.style.magorFontBrush = new SolidBrush(Skilful.Sample.AxisStyleLeft_Font_Color);
            activeChart.topAxis.style.magorFontBrush = new SolidBrush(Skilful.Sample.AxisStyleTop_Font_Color);
            activeChart.bottomAxis.style.magorFontBrush = new SolidBrush(Skilful.Sample.AxisStyleBottom_Font_Color);

            activeChart.rightAxis.cursorLable.fontBrush = new SolidBrush(Skilful.Sample.CursorLable_Right_Font_Color);
            activeChart.leftAxis.cursorLable.fontBrush = new SolidBrush(Skilful.Sample.CursorLable_Left_Font_Color);
            activeChart.bottomAxis.cursorLable.fontBrush = new SolidBrush(Skilful.Sample.CursorLable_Bottom_Font_Color);

            activeChart.rightAxis.style.backColor = Skilful.Sample.AxisStyleRight_BackColor;
            activeChart.rightAxis.style.backBrush = new SolidBrush(Skilful.Sample.AxisStyleRight_BackColor);
            activeChart.leftAxis.style.backColor = Skilful.Sample.AxisStyleLeft_BackColor;
            activeChart.leftAxis.style.backBrush = new SolidBrush(Skilful.Sample.AxisStyleLeft_BackColor);
            activeChart.topAxis.style.backColor = Skilful.Sample.AxisStyleTop_BackColor;
            activeChart.topAxis.style.backBrush = new SolidBrush(Skilful.Sample.AxisStyleTop_BackColor);
            activeChart.bottomAxis.style.backColor = Skilful.Sample.AxisStyleBottom_BackColor;
            activeChart.bottomAxis.style.backBrush = new SolidBrush(Skilful.Sample.AxisStyleBottom_BackColor);
            
            workingForm.Controls.Add(activeChart);
            activeChart.Dock = DockStyle.Fill;

        }

    }
}