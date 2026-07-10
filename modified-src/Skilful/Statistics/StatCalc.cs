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
using System.Linq;
using System.Text;
using Skilful.Data;
using Skilful.QuotesManager;
using Skilful.ModelManager;
using ChartV2;

using System.Reflection;
using System.Threading;
using System.ComponentModel;

using System.Windows.Forms;

using Excel = Microsoft.Office.Interop.Excel;
//using Office = Microsoft.Office.Core; // это может понадобиться для работы с диалогами

namespace Skilful.Statistics
{
    // ===================== CSV-бэкенд вместо Excel-COM =====================
    // describe() строил статистику через Excel-COM (Workbooks.Add/Cells/SaveAs). Под Wine COM-
    // автоматизация Excel не работает (Workbooks.Add → 0x800706F4 RPC_X_NULL_REF_POINTER), а сам
    // автор отметил CSV-набор опциональным выводом. Классы ниже повторяют ТОЛЬКО ту поверхность
    // Excel-API, что зовёт describe(): Cells[r,c]=v, get_Range, Columns/Rows/Hyperlinks, Name, Next,
    // SaveAs. Заполнение статистики (tbl.Cells[r,c]=...) НЕ меняется — меняется лишь тип «доски».
    // SaveAs пишет по одному CSV-файлу на лист: <путь-без-.xls>_<ИмяЛиста>.csv. Формат как в datasets/
    // (эталонные цены): разделитель ',', десятичная '.', CRLF, UTF-8 без BOM.
    class CsvApp
    {
        public CsvBook Book = new CsvBook();
        public CsvBook Workbooks { get { return Book; } }
        public CsvBook Worksheets { get { return Book; } }
        public CsvSheet ActiveSheet { get { return Book.First(); } }
        public bool Interactive { get; set; }
        public bool EnableEvents { get; set; }
        public bool ScreenUpdating { get; set; }
        public bool UserControl { get; set; }
        public bool Visible { get; set; }
    }

    class CsvBook
    {
        public System.Collections.Generic.List<CsvSheet> Sheets = new System.Collections.Generic.List<CsvSheet>();
        public CsvSheet First() { while (Sheets.Count < 1) Sheets.Add(new CsvSheet(this, Sheets.Count)); return Sheets[0]; }
        public CsvSheet Add(params object[] a) { CsvSheet s = new CsvSheet(this, Sheets.Count); Sheets.Add(s); return s; }
        public CsvSheet NextOf(CsvSheet s)
        {
            int ni = s.Index + 1;
            while (Sheets.Count <= ni) Sheets.Add(new CsvSheet(this, Sheets.Count));
            return Sheets[ni];
        }
    }

    class CsvSheet
    {
        readonly CsvBook book;
        public int Index;
        public string Name = "";
        readonly System.Collections.Generic.Dictionary<long, object> cells = new System.Collections.Generic.Dictionary<long, object>();
        int maxRow, maxCol;
        public CsvSheet(CsvBook b, int i) { book = b; Index = i; }
        public CsvCells Cells { get { return new CsvCells(this); } }
        public CsvSheet Next { get { return book.NextOf(this); } }
        public CsvStub Columns { get { return new CsvStub(); } }
        public CsvStub Rows { get { return new CsvStub(); } }
        public CsvLinks Hyperlinks { get { return new CsvLinks(); } }
        public CsvRange get_Range(object a, object b)
        {
            CsvCellRef c = a as CsvCellRef;
            return (c != null) ? new CsvRange(c.Sheet, c.R, c.C) : new CsvRange(this, 1, 1);
        }
        public void Put(int r, int c, object v)
        {
            cells[((long)r << 20) + c] = v;
            if (r > maxRow) maxRow = r;
            if (c > maxCol) maxCol = c;
        }
        public void SaveAs(string path, params object[] ignore)
        {
            string bas = path.EndsWith(".xls") ? path.Substring(0, path.Length - 4) : path;
            System.Text.StringBuilder list = new System.Text.StringBuilder();
            foreach (CsvSheet s in book.Sheets)
            {
                string nm = (s.Name.Length > 0) ? s.Name : ("Sheet" + (s.Index + 1));
                string fn = bas + "_" + nm + ".csv";
                s.WriteCsv(fn);
                list.Append(fn).Append("\r\n");
            }
            try { System.Windows.Forms.MessageBox.Show("Statistics saved (CSV):\r\n" + list); }
            catch { }
        }
        void WriteCsv(string fn)
        {
            // Формат как в datasets/ (эталонные ценовые CSV): разделитель ',', десятичная '.' (Invariant),
            // CRLF, UTF-8 БЕЗ BOM (канон — ASCII-без-BOM; Cyrillic-метки требуют UTF-8). Текстовые поля
            // с запятой/кавычкой/переводом строки заключаются в кавычки (валидный CSV; цены этого не требуют).
            System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.InvariantCulture;
            System.Text.Encoding utf8NoBom = new System.Text.UTF8Encoding(false);
            using (System.IO.StreamWriter w = new System.IO.StreamWriter(fn, false, utf8NoBom))
            {
                w.NewLine = "\r\n";
                for (int r = 1; r <= maxRow; r++)
                {
                    System.Text.StringBuilder line = new System.Text.StringBuilder();
                    for (int c = 1; c <= maxCol; c++)
                    {
                        if (c > 1) line.Append(',');
                        object v;
                        string cell = (cells.TryGetValue(((long)r << 20) + c, out v) && v != null)
                            ? System.Convert.ToString(v, ci) : "";
                        if (cell.IndexOf(',') >= 0 || cell.IndexOf('"') >= 0 || cell.IndexOf('\n') >= 0 || cell.IndexOf('\r') >= 0)
                            cell = "\"" + cell.Replace("\"", "\"\"") + "\"";
                        line.Append(cell);
                    }
                    w.WriteLine(line.ToString());
                }
            }
        }
    }

    class CsvCells
    {
        readonly CsvSheet s;
        public CsvCells(CsvSheet sh) { s = sh; }
        public object this[int r, int c]
        {
            get { return new CsvCellRef(s, r, c); }
            set { s.Put(r, c, value); }
        }
    }

    class CsvCellRef
    {
        public readonly CsvSheet Sheet; public readonly int R, C;
        public CsvCellRef(CsvSheet s, int r, int c) { Sheet = s; R = r; C = c; }
    }

    class CsvRange
    {
        public readonly CsvSheet Sheet; public readonly int R, C;
        public CsvFont Font = new CsvFont();
        public CsvRange(CsvSheet s, int r, int c) { Sheet = s; R = r; C = c; }
        public void Merge(bool across) { }
    }

    class CsvFont { public int Size { get; set; } }

    class CsvStub
    {
        public void AutoFit() { }
        public double ColumnWidth { get; set; }
        public string NumberFormatLocal { get; set; }
    }

    class CsvLinks
    {
        // Hyperlinks.Add(anchor, file, subAddress, screenTip, textToDisplay):
        // в CSV нет гиперссылок — сохраняем видимый ТЕКСТ ссылки (ModelID) в ячейку якоря.
        public void Add(object anchor, object targetFile, object subAddress, object screenTip, object textToDisplay)
        {
            CsvRange a = anchor as CsvRange;
            if (a != null) a.Sheet.Put(a.R, a.C, textToDisplay);
        }
    }

    class StatCalc
    {
        DateTime Begin, End; //Временной горизонт (даты начала и конца истории)
        TF TFBase; //исследуемые фреймы, где TFBase-текущий, TFChild-меньший, TFParent-старший, TFGrand-старший-старший

        int BarCnt, BarsVoid,    //Количество баров истории / количество баров истории не входящих в состав моделей
            ModelCnt,          //Количество моделей на истории суммарно и с разделением на:
            Ext1cnt, Ext2cnt, Ext3cnt, 
            Bal1cnt, Bal2cnt, Bal3cnt,
            Atr1cnt, Atr2cnt, Atr3cnt,
            Ext1cnt6, Ext2cnt6, Ext3cnt6,
            Bal1cnt6, Bal2cnt6, Bal3cnt6,
            Ext1cnt4, Ext2cnt4, Ext3cnt4,
            Bal1cnt4, Bal2cnt4, Bal3cnt4,
            m6step1, m6step2, m6step3,
            m4step1, m4step2, m4step3;
        Fstat flags = new Fstat();
        const int ext = 0, bal = 1, atr = 2;
        int[] modelrefs;


        public void progressChanged(object sender, ProgressChangedEventArgs e)
        {
            progress_display(e.ProgressPercentage);
        }

        public void describe_completed(object sender, RunWorkerCompletedEventArgs e)
        {
            progressbar_init(0);
        }

        BackgroundWorker bgw;
        //запустим статистику фоновым процессом
        public void Describe(Working working)//TQuotes quotes)
        {
            progressbar_init(110);

            bgw = new BackgroundWorker();
            bgw.WorkerReportsProgress = true;
            bgw.WorkerSupportsCancellation = true;
            bgw.DoWork += describe;
            bgw.ProgressChanged += progressChanged;
            bgw.RunWorkerCompleted += describe_completed;
            bgw.RunWorkerAsync(working);
            //describe(bgw, new DoWorkEventArgs(working));
            //describe_completed(null, null);
        }
        

        TQuotes quotes;
        string source_name, symbol_name;
        TF tf;

        Load load;
        ProgressBar pb;
        Label progress_label;
        double progress_step = 1, progress_count=0, progress_prev=0;
        void progress_display(int i)
        {
            if (progress_label != null)
            {
                if (i < 10)
                {
                    string text = "";
                    switch (i)
                    {
                        case -1: text = "Search models on all frames"; break;
                        case 0: text = "Prepare statistics"; break;
                        case 1: text = "General statistics"; break;
                        case 2: text = "page 1 - Models of expansion"; break;
                        case 3: text = "page 2 - Models of dynamic balance"; break;
                        case 4: text = "page 3 - Models of attraction"; break;
                        case 5: text = "Writing Excel file"; break;
                    }

                    progress_label.Text = text;
                    progress_label.Refresh();
                    pb.PerformStep();
                    pb.Refresh();
                }
                else
                {
                    progress_count += progress_step;
                    if (progress_prev < progress_count)
                    {
                        progress_prev = Math.Ceiling(progress_count);
                        pb.PerformStep();
                        pb.Refresh();
                    }
                }
            }
        }
        void progressbar_init(int step_count)
        {
            if (step_count > 0)
            {
                load = new Load();
                load.Text = "Create statistics table";
                pb = (ProgressBar)load.Controls[1];
                pb.Minimum = 0;
                pb.Maximum = step_count;
                pb.Value = 0;
                progress_label = (Label)load.Controls[0]; // Label
                load.TopMost = true;
                load.Show();
                pb.Refresh();  // ProgressBar
                progress_count=0;
                progress_prev=0;
            }
            else
            {
                load.Dispose();
            }
        }

        private void describe(object sender, DoWorkEventArgs e)
        {
            Working working = e.Argument as Working;
            quotes = working.Quotes;
            if (quotes == null) return;

            source_name = quotes.Symbol.moduleName;
            symbol_name = quotes.Symbol.Name;
            tf = quotes.tf;

            BackgroundWorker worker = sender as BackgroundWorker;
            worker.ReportProgress(-1);

            if (quotes.MM == null)
               // return;
                working.SearchModel2(true, false);

            worker.ReportProgress(0);

            List<Model> models = ModelsInit(quotes.MM.Models);
            List<TModel> tmodels = quotes.MM.Models;
            progress_step = 100.0 / tmodels.Count;

            points = quotes.MM.points;

            //сбор статистики по имеющимся фреймам и моделям
            //1.    Временной горизонт (даты начала и конца истории)
            Begin = quotes[0].DT;
            End = quotes[quotes.lastindex].DT;
            //2.	План
            TFBase = tf;
            //3.	Количество баров истории
            BarCnt = quotes.GetCount();
            //4.	Количество моделей на истории суммарно
            ModelCnt = tmodels.Count;
            //4.a подсчет количества моделей по проходам и типам моделей
            CountModelsByType(tmodels, TFBase);
            //CountPointsByStatus(tmodels, BarCnt);
            CountModelsByStatus(points);
            //6. количество баров истории не входящих в состав моделей
            BarsVoid = CountVoidBars(tmodels);
            //7. % пункта 6 к общему количеству баров истории.
            //DarsVoid/BarCnt*100

            CountPercentOf_4eq6(tmodels);
            // - Многоплановый контекст

            worker.ReportProgress(1);

            // Статистика пишется в набор CSV-файлов через мини-бэкенд CsvApp/CsvSheet (классы вверху файла).
            // Excel-COM убран: Wine не умеет COM-автоматизацию Excel (Workbooks.Add → 0x800706F4).
            // Заполнение таблицы ниже (tbl.Cells[r,c]=...) не изменилось — поменялся только бэкенд «доски».
            try
            {
                //создание экселевской книжки
                CsvSheet tbl;
                CsvApp xlApp = new CsvApp();
                xlApp.Workbooks.Add(Type.Missing);
                xlApp.Worksheets.Add(Type.Missing, Type.Missing, 1, Type.Missing);
                xlApp.Interactive = false;
                xlApp.EnableEvents = false;
                tbl = (CsvSheet)xlApp.ActiveSheet;
                
                //заполнение таблиц
                //перва страница == "General"
                tbl.Name = "General";
                int row = 1, col = 2;
                tbl.Cells[row, col++] = symbol_name;
                tbl.Cells[row, col++] = "  " + TFBase.ToString() + "  ";
                tbl.Cells[row, col++] = Begin.ToString("dd.MM.yyyy HH:mm:ss");
                tbl.Cells[row++, col] = End.ToString("dd.MM.yyyy HH:mm:ss");

                col = 1;
                tbl.Cells[row++, col] = "Количество баров истории: ";
                smallfont(row, col, tbl);
                tbl.Cells[row, col++] = "Total history bars: ";
                tbl.Cells[row++, col] = BarCnt;

                col = 1;
                tbl.Cells[row++, col] = "Общее количество моделей: ";
                smallfont(row, col, tbl);
                tbl.Cells[row, col++] = "Total model's count: ";
                tbl.Cells[row++, col] = ModelCnt;
                //подровняем колонки
                //tbl.Rows.AutoFit();

                col = 1;
                tbl.Cells[row++, col] = "Моделей от начала тренда: *";
                smallfont(row, col, tbl);
                tbl.Cells[row, col++] = "Models from trend source (trend beginning): ";
                tbl.Cells[row, col++] = modelBaseCnt;
                tbl.Cells[row++, col] = modelBaseCnt * 100 / ModelCnt + "%";
                //подровняем колонки
                //tbl.Rows.AutoFit();

                col = 1;
                tbl.Cells[row++, col] = "Коррекционных моделей: **";
                smallfont(row, col, tbl);
                tbl.Cells[row, col++] = "Models of correction: ";
                tbl.Cells[row, col++] = modelCorrectionCnt;
                tbl.Cells[row++, col] = modelCorrectionCnt * 100 / ModelCnt + "%";
                //подровняем колонки
                //tbl.Rows.AutoFit();

                col = 1;
                tbl.Cells[row++, col] = "Моделей по тренду: ***";
                smallfont(row, col, tbl);
                tbl.Cells[row, col++] = "Models by trend: ";
                tbl.Cells[row, col++] = modelByTrendCnt;
                tbl.Cells[row++, col] = modelByTrendCnt * 100 / ModelCnt + "%";
                //подровняем колонки
                //tbl.Rows.AutoFit();

                tbl.Columns.AutoFit();

                //CsvRange rng = tbl.get_Range(tbl.Cells[row - 3, col], tbl.Cells[row, col + 1]);
                //rng.AutoOutline();

                //tbl.Columns.NumberFormatLocal = "0.00";

               // tbl.Columns.NumberFormatLocal = "0,00";

                incrows(ref row, ref col, tbl);
                merge(row - 1, row + 1, col, col + 10, tbl);
                tbl.Cells[row++, col] = "Из них моделей первого прохода (% от общего количества; % от всех моделей первого прохода): ";
                smallfont(row, col, tbl);
                tbl.Cells[row++, col] = "Models of 1-st iteration of them (% of total models count; % of total models of first iteration count): ";
                tbl.Cells[row, col++] = "MExp - Models of expansion: ";
                tbl.Cells[row, col++] = Ext1cnt;
                tbl.Cells[row, col++] = Math.Round(((double)Ext1cnt / ModelCnt * 100)) + "%";
                tbl.Cells[row++, col] = Math.Round(((double)Ext1cnt / (Ext1cnt + Bal1cnt + Atr1cnt) * 100)) + "%";
                col = 1;
                tbl.Cells[row, col++] = "MDB - Models of dynamic balance: ";
                tbl.Cells[row, col++] = Bal1cnt;
                tbl.Cells[row, col++] = Math.Round(((double)Bal1cnt / ModelCnt * 100)) + "%";
                tbl.Cells[row++, col] = Math.Round(((double)Bal1cnt / (Ext1cnt + Bal1cnt + Atr1cnt) * 100)) + "%";
                col = 1;
                tbl.Cells[row, col++] = "MAttr - Models of attraction: ";
                tbl.Cells[row, col++] = Atr1cnt;
                tbl.Cells[row, col++] = Math.Round(((double)Atr1cnt / ModelCnt * 100)) + "%";
                tbl.Cells[row++, col] = Math.Round(((double)Atr1cnt / (Ext1cnt + Bal1cnt + Atr1cnt)) * 100) + "%";
                col = 1;
                tbl.Cells[row, col++] = "MExp with p.6 / MExp with p.6=p.4 (%): ";
                tbl.Cells[row, col++] = Math.Round(((double)Ext1cnt6 * 100 / Ext1cnt)) + "%";
                tbl.Cells[row++, col] = Math.Round(((double)Ext1cnt4 * 100 / Ext1cnt)) + "%";
                col = 1;
                tbl.Cells[row, col++] = "MDB with p.6 / MDB with p.6=p.4 (%): ";
                tbl.Cells[row, col++] = (Bal1cnt != 0 ? Math.Round(((double)Bal1cnt6 * 100 / Bal1cnt)) : 0) + "%";
                tbl.Cells[row++, col] = (Bal1cnt != 0 ? Math.Round(((double)Bal1cnt4 * 100 / Bal1cnt)) : 0) + "%";

                incrows(ref row, ref col, tbl);
                merge(row - 1, row + 1, col, col + 10, tbl);
                tbl.Cells[row++, col] = "моделей второго прохода (% от общего количества; % от всех моделей второго прохода): ";
                smallfont(row, col, tbl);
                tbl.Cells[row++, col] = "models of 2-nd iteration (% of total models count; % of total models of 2-nd iteration count): ";
                tbl.Cells[row, col++] = "MExp - Models of expansion: ";
                tbl.Cells[row, col++] = Ext2cnt;
                tbl.Cells[row, col++] = Math.Round(((double)Ext2cnt / ModelCnt * 100)) + "%";
                tbl.Cells[row++, col] = Math.Round(((double)Ext2cnt / (Ext2cnt + Bal2cnt + Atr2cnt) * 100)) + "%";
                col = 1;
                tbl.Cells[row, col++] = "MDB - Models of dynamic balance: ";
                tbl.Cells[row, col++] = Bal2cnt;
                tbl.Cells[row, col++] = Math.Round(((double)Bal2cnt / ModelCnt * 100)) + "%";
                tbl.Cells[row++, col] = Math.Round(((double)Bal2cnt / (Ext2cnt + Bal2cnt + Atr2cnt) * 100)) + "%";
                col = 1;
                tbl.Cells[row, col++] = "MAttr - Models of attraction: ";
                tbl.Cells[row, col++] = Atr2cnt;
                tbl.Cells[row, col++] = Math.Round(((double)Atr2cnt / ModelCnt * 100)) + "%";
                tbl.Cells[row++, col] = Math.Round(((double)Atr2cnt / (Ext2cnt + Bal2cnt + Atr2cnt) * 100)) + "%";
                col = 1;
                tbl.Cells[row, col++] = "MExp with p.6 / MExp with p.6=p.4 (%): ";
                tbl.Cells[row, col++] = Math.Round(((double)Ext2cnt6 * 100 / Ext2cnt)) + "%";
                tbl.Cells[row++, col] = Math.Round(((double)Ext2cnt4 * 100 / Ext2cnt)) + "%";
                col = 1;
                tbl.Cells[row, col++] = "MDB with p.6 / MDB with p.6=p.4 (%): ";
                tbl.Cells[row, col++] = (Bal2cnt != 0 ? Math.Round(((double)Bal2cnt6 * 100 / Bal2cnt)) : 0) + "%";
                tbl.Cells[row++, col] = (Bal2cnt != 0 ? Math.Round(((double)Bal2cnt4 * 100 / Bal2cnt)) : 0) + "%";

                incrows(ref row, ref col, tbl);
                merge(row - 1, row + 1, col, col + 10, tbl);
                tbl.Cells[row++, col] = "моделей третьего прохода (% от общего количества; % от всех моделей третьего прохода): ";
                smallfont(row, col, tbl);
                tbl.Cells[row++, col] = "models of 3-rd iteration (% of total models count; % of total models of 3-rd iteration count): ";
                tbl.Cells[row, col++] = "MExp - Models of expansion: ";
                tbl.Cells[row, col++] = Ext3cnt;
                tbl.Cells[row, col++] = Math.Round(((double)Ext3cnt / ModelCnt * 100)) + "%";
                tbl.Cells[row++, col] = Math.Round(((double)Ext3cnt / (Ext3cnt + Bal3cnt + Atr3cnt) * 100)) + "%";
                col = 1;
                tbl.Cells[row, col++] = "MDB - Models of dynamic balance: ";
                tbl.Cells[row, col++] = Bal3cnt;
                tbl.Cells[row, col++] = Math.Round(((double)Bal3cnt / ModelCnt * 100)) + "%";
                tbl.Cells[row++, col] = Math.Round(((double)Bal3cnt / (Ext3cnt + Bal3cnt + Atr3cnt) * 100)) + "%";
                col = 1;
                tbl.Cells[row, col++] = "MAttr - Models of attraction: ";
                tbl.Cells[row, col++] = Atr3cnt;
                tbl.Cells[row, col++] = Math.Round(((double)Atr3cnt / ModelCnt * 100)) + "%";
                tbl.Cells[row++, col] = Math.Round(((double)Atr3cnt / (Ext3cnt + Bal3cnt + Atr3cnt) * 100)) + "%";
                col = 1;
                tbl.Cells[row, col++] = "MExp with p.6 / MExp with p.6=p.4 (%): ";
                tbl.Cells[row, col++] = Math.Round(((double)Ext3cnt6 * 100 / Ext3cnt)) + "%";
                tbl.Cells[row++, col] = Math.Round(((double)Ext3cnt4 * 100 / Ext3cnt)) + "%";
                col = 1;
                tbl.Cells[row, col++] = "MDB with p.6 / MDB with p.6=p.4 (%): ";
                tbl.Cells[row, col++] = (Bal3cnt != 0 ? Math.Round(((double)Bal3cnt6 * 100 / Bal3cnt)) : 0) + "%";
                tbl.Cells[row++, col] = (Bal3cnt != 0 ? Math.Round(((double)Bal3cnt4 * 100 / Bal3cnt)) : 0) + "%";

                incrows(ref row, ref col, tbl);
                merge(row - 1, row + 1, col, col + 10, tbl);
                tbl.Cells[row++, col] = "количество баров истории не входящих в состав моделей (% к общему количестау баров): ";
                smallfont(row, col, tbl);
                tbl.Cells[row++, col] = "count of history bars not included in the models (% of the total number of  bars): ";
                tbl.Cells[row, col++] = "Free bars count";
                tbl.Cells[row, col++] = BarsVoid;
                tbl.Cells[row++, col] = Math.Round(((double)BarsVoid / BarCnt * 100)) + "%";

                incrows(ref row, ref col, tbl);
                tbl.Cells[row++, col] = "* Модели от начала тренда --  если т1 совпадает с т6(4=6) какой- либо модели; (Models from trend source, when p.1 is p.6(p.4 if p.6=p.4) of other model)";
                tbl.Cells[row++, col] = " --  если т1 не пересекается ни с одной из других моделей (- if p.1 doesn't cross with other models)";
                tbl.Cells[row++, col] = "** Коррекционные модели -- модели от точек 2, 4, базовых моделей (Models of correction - models from p.2, p.4 of models from trend source)";
                tbl.Cells[row++, col] = "*** Модели по тренду -- от т3 базовой модели (Models by trend - models from p.3 of models from trend source)";


                int id, ri;
                int[] stepi = { 0, 1, 2, 0, 3 }; //Step == 1|2|4

                worker.ReportProgress(2);
                try
                {
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //страница 2 -- модели расширения
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //tbl.Rows.AutoFit();
                    tbl = (CsvSheet)tbl.Next;
                    tbl.Name = "MExp";
                    row = 1; col = 1;
                    tbl.Cells[row, col++] = "МР";
                    smallfont(row, col, tbl);
                    merge(row, row, col, col + 4, tbl);
                    tbl.Cells[row++, col] = "Models of expansion";

                    //шапка таблицы
                    col = 1;
                    smallfont(row + 1, 1, row + 1, 30, tbl);
                    tbl.Cells[row, col++] = "ID: ";
                    tbl.Cells[row, col++] = "TF: ";
                    tbl.Cells[row + 1, col] = "Iteration: (base::current model)";
                    tbl.Cells[row, col++] = "Проход: (базовой::данной модели)";
                    tbl.Cells[row + 1, col] = "Status: *(T1=model from the source of trend (base model), T3=model by trend, Corr=correction model, T3Corr=by trend of correction model, 1*=model from the source of trend without previous model p.6 confirmation)";
                    tbl.Cells[row, col++] = "Статус: *(T1=базовая, T3=по тренду, Corr=коррекционная, T3Corr=по тренду коррекционной модели, 1*=базовая, но не подтверждена другой 6-й)";
                    tbl.Cells[row + 1, col] = "Direction: ";
                    tbl.Cells[row, col++] = "Направление: ";

                    //значения точек модели
                    merge(row, row, col, col + 8, tbl);
                    merge(row + 1, row + 1, col, col + 8, tbl);
                    tbl.Cells[row + 1, col] = "Model`s points";
                    tbl.Cells[row, col] = "Значения точек модели: ";
                    tbl.Cells[row + 2, col++] = "p1 Date Time";
                    tbl.Cells[row + 2, col++] = "CP";
                    tbl.Cells[row + 2, col++] = "1";
                    tbl.Cells[row + 2, col++] = "2";
                    tbl.Cells[row + 2, col++] = "3";
                    tbl.Cells[row + 2, col++] = "4";
                    tbl.Cells[row + 2, col++] = "5";
                    tbl.Cells[row + 2, col++] = "HP";
                    tbl.Cells[row + 2, col++] = "6";

                    //<контекст>//////////////////////////////////////////////////////////////////////////////////
                    int context_cols_count = col;
                    merge(row, row, col, col + 8, tbl);
                    merge(row + 1, row + 1, col, col + 8, tbl);
                    tbl.Cells[row + 1, col] = "Last 3 models";
                    tbl.Cells[row, col] = "3 последних модели (от начала тренда): ";

                    //проход каждой из 3-х предшествующих
                    tbl.Cells[row + 2, col++] = "m1Step";
                    tbl.Cells[row + 2, col++] = "m2Step";
                    tbl.Cells[row + 2, col++] = "m3Step";

                    //тип каждой из 3-х предшествующих
                    tbl.Cells[row + 2, col++] = "m1Type";
                    tbl.Cells[row + 2, col++] = "m2Type";
                    tbl.Cells[row + 2, col++] = "m3Type";

                    //полная характеристика по каждой из 3-х предшествующих 
                    tbl.Cells[row + 2, col++] = "m1ID";
                    tbl.Cells[row + 2, col++] = "m2ID";
                    tbl.Cells[row + 2, col++] = "m3ID";

                    merge(row, row, col, col + 2, tbl);
                    merge(row + 1, row + 1, col, col + 2, tbl);
                    tbl.Cells[row + 1, col] = "Ratio of total bars to pips quantity of last 3 models";
                    tbl.Cells[row, col] = "соотношение общего количества баров к общему количеству пунктов 3-х последних моделей: ";
                    //Общее количество баров для этих трех моделей
                    tbl.Cells[row + 2, col++] = "total bars count (Общее количество баров для этих трех моделей)";

                    //Сумма в пунктах всего движения описываемого тремя моделями
                    tbl.Cells[row + 2, col++] = "total pips count (Сумма в пунктах всего движения описываемого тремя моделями)";

                    //Соотношение пунктов 4 и 5
                    tbl.Cells[row + 2, col++] = "pips/bars ratio";

                    //7.	Скорость для каждой из трех моделей по отдельности. 
                    merge(row, row, col, col + 2, tbl);
                    merge(row + 1, row + 1, col, col + 2, tbl);
                    tbl.Cells[row + 1, col] = "Speed of last 3 models";
                    tbl.Cells[row, col] = "Скорость для каждой из 3 последних моделей: ";
                    tbl.Cells[row + 2, col++] = "m1";
                    tbl.Cells[row + 2, col++] = "m2";
                    tbl.Cells[row + 2, col++] = "m3";

                    //8.	Модель по тренду или от начала тренда для своего Плана или коррекционная.
                    merge(row, row, col, col + 1, tbl);
                    merge(row + 1, row + 1, col, col + 1, tbl);
                    tbl.Cells[row + 1, col] = "Model’s status: Base/By trend/(p.1=p.3)/Corr";
                    tbl.Cells[row, col] = "Статус модели: Base/By trend/(p.1=p.3)/Corr";
                    tbl.Cells[row + 2, col++] = "Status";

                    //9.	Если модель по тренду то: ее 1 точка равна или нет 3 точки
                    tbl.Cells[row + 2, col++] = "(p.1=p.3) of base";

                    //10.	Соотношение наклона трендовой к горизонтали модели к каждой из трех по отдельности трендовых моделей контекста.
                    merge(row, row, col, col + 2, tbl);
                    merge(row + 1, row + 1, col, col + 2, tbl);
                    tbl.Cells[row + 1, col] = "Ratio of tilt of trendline to a horizontal for each of three models of contents";
                    tbl.Cells[row, col] = "Соотношение наклона трендовой модели к горизонтали для каждой из трех по отдельности трендовых моделей контекста: ";
                    tbl.Cells[row + 2, col++] = "m1";
                    tbl.Cells[row + 2, col++] = "m2";
                    tbl.Cells[row + 2, col++] = "m3";

                    //11.	Соотношение наклона целевой к горизонтали модели к каждой из трех по отдельности целевой моделей контекста.
                    merge(row, row, col, col + 2, tbl);
                    merge(row + 1, row + 1, col, col + 2, tbl);
                    tbl.Cells[row + 1, col] = "Ratio of tilt of target line to horizontal of model for each of three models";
                    tbl.Cells[row, col] = "Соотношение наклона целевой к горизонтали модели к каждой из трех по отдельности целевой моделей контекста: ";
                    tbl.Cells[row + 2, col++] = "m1";
                    tbl.Cells[row + 2, col++] = "m2";
                    tbl.Cells[row + 2, col++] = "m3";

                    //12.	Соотношение скорости модели к каждой из трех скоростей моделей контекста по отдельности.
                    merge(row, row, col, col + 2, tbl);
                    merge(row + 1, row + 1, col, col + 2, tbl);
                    tbl.Cells[row + 1, col] = "Ratio of speed of model to speed of each of three models (by НР)";
                    tbl.Cells[row, col] = "Соотношение скорости модели к каждой из трех скоростей моделей контекста по отдельности (по НР): ";
                    tbl.Cells[row + 2, col++] = "m1";
                    tbl.Cells[row + 2, col++] = "m2";
                    tbl.Cells[row + 2, col++] = "m3";

                    //13.	Для каждой модели контекста указать: достигнута НР, 100%, 200%, 1, 2, 3 цель, в рамках (ст-4)*4.
                    //merge(row, row, col, col + 2, tbl);
                    //merge(row + 1, row + 1, col, col + 2, tbl);
                    //tbl.Cells[row + 1, col] = "Targets";
                    //tbl.Cells[row, col] = "Цели: ";
                    //tbl.Cells[row + 2, col++] = "m1";
                    //tbl.Cells[row + 2, col++] = "m2";
                    //tbl.Cells[row + 2, col++] = "m3";

                    context_cols_count = col - context_cols_count;
                    //</контекст>//////////////////////////////////////////////////////////////////////////////////

                    //Сила Модели
                    tbl.Cells[row + 1, col] = "Power of model";
                    tbl.Cells[row + 2, col] = "(p.1-p.3)/(CP–p.1)";
                    tbl.Cells[row, col++] = "Сила Модели";

                    //4.	Соотношение Базы к БЦ 1-3 к 4-5 и 1-2 к 4-5 в %
                    merge(row, row, col, col + 1, tbl);
                    merge(row + 1, row + 1, col, col + 1, tbl);
                    tbl.Cells[row + 1, col] = "Ratio of Base to Target Block";
                    tbl.Cells[row, col] = "Соотношение Базы к БЦ: ";
                    tbl.Cells[row + 2, col++] = "1-3/4-5";
                    tbl.Cells[row + 2, col++] = "1-2/4-5";

                    //5.	Соотношение Базы ко всей модели – 1-3 к 1-6 (не НР, а именно 6) и 1-2 к 1-6 в %
                    merge(row, row, col, col + 1, tbl);
                    merge(row + 1, row + 1, col, col + 1, tbl);
                    tbl.Cells[row + 1, col] = "Ratio of Base to whole model";
                    tbl.Cells[row, col] = "Соотношение Базы ко всей модели: ";
                    tbl.Cells[row + 2, col++] = "1-3/1-6";
                    tbl.Cells[row + 2, col++] = "1-2/1-6";

                    //6.	Соотношение БЦ ко всей модели – 4-5 к 1-6 по вертикали (не НР, а именно 6) и 4-5 к 1-6 по горизонтали в %
                    merge(row, row, col, col + 1, tbl);
                    merge(row + 1, row + 1, col, col + 1, tbl);
                    tbl.Cells[row + 1, col] = "Ratio of Target Block to whole model";
                    tbl.Cells[row, col] = "Соотношение БЦ ко всей модели: ";
                    tbl.Cells[row + 2, col++] = "4-5/1-6v";
                    tbl.Cells[row + 2, col++] = "4-5/1-6h";

                    //7.	Соотношение 2-5 к 1-6 в %
                    tbl.Cells[row + 2, col++] = "2-5/1-6";
                    //8.	Соотношение 1-2 к 3-4 в %
                    tbl.Cells[row + 2, col++] = "1-2/3-4";
                    //9.	Соотношение 3-4 к 1-6 в %
                    tbl.Cells[row + 2, col++] = "3-4/1-6";
                    //10.	Соотношение ст-1 к 4-5 в %
                    tbl.Cells[row + 2, col++] = "CP-1/4-5";

                    //11.	Соотношение (наклона целевой к вертикали) к (наклону трендовой к горизонтали)
                    tbl.Cells[row + 1, col] = "Ratio of tilt of Target Line to vertical to tilt of Trend Line to horizontal";
                    tbl.Cells[row, col++] = "Соотношение (наклона целевой к вертикали) к (наклону трендовой к горизонтали): ";

                    //12.	Достижение НР до пробития целевой и трендовой на промежутке (ст-4)*4
                    tbl.Cells[row + 1, col] = "Reaching of НР before target or trend line break on (CP-p.4)*4 distance";
                    tbl.Cells[row, col++] = "Достижение НР до пробития целевой и трендовой на промежутке (ст-4)*4: ";

                    //13.	Достижение 100% и 200% после пробития ЛЦ на промежутке (ст-4)*4
                    merge(row, row, col, col + 1, tbl);
                    merge(row + 1, row + 1, col, col + 1, tbl);
                    tbl.Cells[row + 1, col] = "Reaching of 100% and 200% after Target line break on (CP-p.4)*4 distance";
                    tbl.Cells[row, col] = "Достижение 100% и 200% после пробития ЛЦ на промежутке (ст-4)*4: ";
                    tbl.Cells[row + 2, col++] = "100%";
                    tbl.Cells[row + 2, col++] = "200%";

                    //14.	Достижение 100% и 200% после (повторного) пробития НР** на промежутке (ст-4)*4
                    merge(row, row, col, col + 1, tbl);
                    merge(row + 1, row + 1, col, col + 1, tbl);
                    tbl.Cells[row + 1, col] = "Reaching of 100% and 200% after HP break on (CP-p.4)*4 distance";
                    tbl.Cells[row, col] = "Достижение 100% и 200% после пробития HP** на промежутке (ст-4)*4: ";
                    tbl.Cells[row + 2, col++] = "100%";
                    tbl.Cells[row + 2, col++] = "200%";

                    //15.	Достижение 100% и 200% без повторного пробития НР*** на промежутке (ст-4)*4
                    merge(row, row, col, col + 1, tbl);
                    merge(row + 1, row + 1, col, col + 1, tbl);
                    tbl.Cells[row + 1, col] = "Reaching of 100% and 200% without recurring HP break on (CP-p.4)*4 distance";
                    tbl.Cells[row, col] = "Достижение 100% и 200% без повторного пробития HP на промежутке (ст-4)*4: ";
                    tbl.Cells[row + 2, col++] = "100%";
                    tbl.Cells[row + 2, col++] = "200%";

                    //*** (т.е. уровень НР достигнут с перелетом или пункт в пункт не важно и следующими барами цена идет дальше без образования противоположного абс. экстремума)

                    //16.	Достижение 100% и 200% после пробития ЛЦ и до пробития ЛТ на промежутке (ст-4)*4
                    merge(row, row, col, col + 1, tbl);
                    merge(row + 1, row + 1, col, col + 1, tbl);
                    tbl.Cells[row + 1, col] = "Reaching of 100% and 200% after Target Line break and before Trend Line break on (CP-p.4)*4 distance";
                    tbl.Cells[row, col] = "Достижение 100% и 200% после пробития ЛЦ и до пробития ЛТ на промежутке (ст-4)*4: ";
                    tbl.Cells[row + 2, col++] = "100%";
                    tbl.Cells[row + 2, col++] = "200%";

                    //17.	Достижение 100% и 200% после пробития HP и до пробития ЛТ на промежутке (ст-4)*4
                    merge(row, row, col, col + 1, tbl);
                    merge(row + 1, row + 1, col, col + 1, tbl);
                    tbl.Cells[row + 1, col] = "Reaching of 100% and 200% after HP level break and before Trend Line break on (CP-p.4)*4 distance";
                    tbl.Cells[row, col] = "Достижение 100% и 200% после пробития HP и до пробития ЛТ на промежутке (ст-4)*4: ";
                    tbl.Cells[row + 2, col++] = "100%";
                    tbl.Cells[row + 2, col++] = "200%";

                    //17.a	Достижение 100% и 200% (по отдельности) без повторного пробития НР и до пробития ЛТ на промежутке (ст-4)*4
                    merge(row, row, col, col + 1, tbl);
                    merge(row + 1, row + 1, col, col + 1, tbl);
                    tbl.Cells[row + 1, col] = "Reaching of 100% and 200% without recurring HP break and before Trend Line break on (CP-p.4)*4 distance";
                    tbl.Cells[row, col] = "Достижение 100% и 200% без повторного пробития HP и до пробития ЛТ на промежутке (ст-4)*4: ";
                    tbl.Cells[row + 2, col++] = "100%";
                    tbl.Cells[row + 2, col++] = "200%";

                    //18.	Достижение целей без пробития целевой на промежутке (ст-4)*4
                    merge(row, row, col, col + 2, tbl);
                    merge(row + 1, row + 1, col, col + 2, tbl);
                    tbl.Cells[row + 1, col] = "Reaching of targets without Target Line break on (CP-p.4)*4 distance";
                    tbl.Cells[row, col] = "Достижение целей без пробития целевой на промежутке (ст-4)*4: ";
                    tbl.Cells[row + 2, col++] = "1";
                    tbl.Cells[row + 2, col++] = "2";
                    tbl.Cells[row + 2, col++] = "3";

                    //19.	Достижение целей с пробитой целевой на промежутке (ст-4)*4
                    merge(row, row, col, col + 2, tbl);
                    merge(row + 1, row + 1, col, col + 2, tbl);
                    tbl.Cells[row + 1, col] = "Reaching of targets with Target Line break on (CP-p.4)*4 distance";
                    tbl.Cells[row, col] = "Достижение целей с пробитой целевой на промежутке (ст-4)*4: ";
                    tbl.Cells[row + 2, col++] = "1";
                    tbl.Cells[row + 2, col++] = "2";
                    tbl.Cells[row + 2, col++] = "3";

                    //20.	Достижение целей без пробития целевой, но с образованием 6, на промежутке (ст-4)*4
                    merge(row, row, col, col + 2, tbl);
                    merge(row + 1, row + 1, col, col + 2, tbl);
                    tbl.Cells[row + 1, col] = "Reaching of targets without Target Line break but with forming of p.6 on (CP-p.4)*4 distance";
                    tbl.Cells[row, col] = "Достижение целей без пробития целевой, но с образованием 6, на промежутке (ст-4)*4: ";
                    tbl.Cells[row + 2, col++] = "1";
                    tbl.Cells[row + 2, col++] = "2";
                    tbl.Cells[row + 2, col++] = "3";

                    //21.	Достижение целей без пробития целевой, и без образования 6 (т.е. 4=6), на промежутке (ст-4)*4
                    merge(row, row, col, col + 2, tbl);
                    merge(row + 1, row + 1, col, col + 2, tbl);
                    tbl.Cells[row + 1, col] = "Reaching of targets without Target Line break and without forming of p.6 (p.4=p.6) on (CP-p.4)*4 distance";
                    tbl.Cells[row, col] = "Достижение целей без пробития целевой, и без образования 6 (т.е. 4=6), на промежутке (ст-4)*4: ";
                    tbl.Cells[row + 2, col++] = "1";
                    tbl.Cells[row + 2, col++] = "2";
                    tbl.Cells[row + 2, col++] = "3";

                    //22.	Соотношение (6 - 1 цель по горизонтали) к 1-6
                    tbl.Cells[row + 1, col] = "Ratio of (p.6-1st target) to (p.1-p.6)";
                    tbl.Cells[row, col++] = "Соотношение (6 - 1 цель по горизонтали) к 1-6: ";

                    //23.	Соотношение (6 - 2 цель по горизонтали) к 1-6
                    tbl.Cells[row + 1, col] = "Ratio of (p.6-2nd target) to (p.1-p.6)";
                    tbl.Cells[row, col++] = "Соотношение (6 - 2 цель по горизонтали) к 1-6: ";

                    //24.	Соотношение (6 - 3 цель по горизонтали) к 1-6
                    tbl.Cells[row + 1, col] = "Ratio of (p.6-3rd target) to (p.1-p.6)";
                    tbl.Cells[row, col++] = "Соотношение (6 - 3 цель по горизонтали) к 1-6: ";

                    //27.	Доход до НР  в % к 1-НР
                    tbl.Cells[row + 1, col] = "Percent ratio of range of НР level exceeding to (p.1-НР)";
                    tbl.Cells[row, col++] = "Доход до НР  в % к 1-НР: ";

                    //28.	Недоход до НР в % к 1-НР
                    tbl.Cells[row + 1, col] = "Percent ratio of range of HP level unreaching to (p.1-НР)";
                    tbl.Cells[row, col++] = "Недоход до НР в % к 1-НР: ";

                    //* 29. время 1 цели - последнее касание трендовой / 6 - первое касание трендовой
                    tbl.Cells[row + 1, col] = "1-st target time (1-st target – last trend line touch)/(p.6 - 1-st trend line touch)";
                    tbl.Cells[row, col] = "Время 1 цели - 1 цель - последнее касание трендовой / 6 - первое касание трендовой";
                    tbl.Cells[row + 2, col++] = "1 target time";

                    //2ST. время достижения СТ при ходе от 6-й в единицах СТ- 4.
                    tbl.Cells[row + 1, col] = "Time of CP level reaching after reaching of HP level in (CP-p.4) units";
                    tbl.Cells[row, col] = "Время достижения СТ при ходе от 6-й в единицах СТ- 4";
                    tbl.Cells[row + 2, col++] = "6-CP";

                    //3ST. был ли откат от 6-й до 4-й
                    tbl.Cells[row + 1, col] = "Rollback from p.6 level to p.4 level";
                    tbl.Cells[row, col] = "Откат от 6-й до 4-й";
                    tbl.Cells[row + 2, col++] = "6-4 get";

                    //4ST. был ли пробит лоу/хай бара 4.
                    tbl.Cells[row + 1, col] = "Break the p.4 bar after HP reaching";
                    tbl.Cells[row, col] = "Пробой бара 4-й";
                    tbl.Cells[row + 2, col++] = "6-4 break";

                    //5ST. была ли пробита 5 точка.
                    tbl.Cells[row + 1, col] = "Break the p.5 level";
                    tbl.Cells[row, col] = "Пробой 5";
                    tbl.Cells[row + 2, col++] = "6-5 break";

                    //9ST. время 6-й в единицах Ст-4
                    tbl.Cells[row + 1, col] = "Time of p.6 reaching in (CP-p.4) units";
                    tbl.Cells[row, col] = "Время 6-й в единицах Ст-4(Время 6-й в единицах Ст-4 (Здесь 6 считается первый экстремум после пробития НР))";
                    tbl.Cells[row + 2, col++] = "6 time";

                    //перелет(+)/недолет(-) 6-й до СТ4х2 для сильных и СТ4х1 для слабых моделей
                    merge(row, row, col, col + 1, tbl);
                    merge(row + 1, row + 1, col, col + 1, tbl);
                    tbl.Cells[row + 1, col] = "ratio of p6 and CP4x2 for intense and CPx4x1 for low models";
                    tbl.Cells[row, col] = "перелет(+)/недолет(-) 6-й до СТ4х2 для сильных и СТ4х1 для слабых моделей";
                    tbl.Cells[row + 2, col++] = "CP4x1";
                    tbl.Cells[row + 2, col++] = "CP4x2";

                    //6ST. время пробития трендовой в единицах Ст-4.
                    tbl.Cells[row + 1, col] = "Time of trend line break in (CP-p.4) units";
                    tbl.Cells[row, col] = "время пробития трендовой в единицах Ст-4";
                    tbl.Cells[row + 2, col++] = "LT break";

                    //перелет(+)/недолет(-) пробития трендовой до СТ4х2 для сильных и СТ4х1 для слабых моделей
                    merge(row, row, col, col + 1, tbl);
                    merge(row + 1, row + 1, col, col + 1, tbl);
                    tbl.Cells[row + 1, col] = "ratio of p6 and CP4x2 for intense and CPx4x1 for low models";
                    tbl.Cells[row, col] = "перелет(+)/недолет(-) 6-й до СТ4х2 для сильных и СТ4х1 для слабых моделей";
                    tbl.Cells[row + 2, col++] = "CP4x1";
                    tbl.Cells[row + 2, col++] = "CP4x2";

                    //7ST. время пробития 6-й (если он был) в единицах СТ-4.
                    tbl.Cells[row + 1, col] = "Time of p.6 break in (CP-p.4) units";
                    tbl.Cells[row, col] = "время пробития 6-й (если он был) в единицах СТ-4";
                    tbl.Cells[row + 2, col++] = "6 break";

                    //8ST. площади треугольников 123,234,345,456
                    merge(row, row, col, col + 3, tbl);
                    merge(row + 1, row + 1, col, col + 3, tbl);
                    tbl.Cells[row + 1, col] = "Area of triangles pp.(1-2-3), (2-3-4), (3-4-5), (4-5-6)";
                    tbl.Cells[row, col] = "Площади треугольников 123,234,345,456: ";
                    tbl.Cells[row + 2, col++] = "123";
                    tbl.Cells[row + 2, col++] = "234";
                    tbl.Cells[row + 2, col++] = "345";
                    tbl.Cells[row + 2, col++] = "456";



                    id = -1;
                    row += 2;
                    ri = row + 1;
                    //int[] stepi = { 0, 1, 2, 0, 3 }; //Step == 1|2|4
                    foreach (Model m in models)
                    {
                        id++;
                        TModel tm = tmodels[id];
                        if (/*tm.TimeFrame != TFBase || */tm.ModelType != Common.TModelType.ModelOfExpansion) continue;
                        double pip = tm.ModelDir == TDirection.dtUp ? quotes.Symbol.pip : -quotes.Symbol.pip;
                        worker.ReportProgress(200);

                        flags.checkup(quotes, tm, m);
                        row++; col = 1;
                        //индекс модели (соответствует индексу на графике)
                        tbl.Cells[row, col++] = id;
                        //ТаймФрейм модели (соответствует Старшему из планов где модель существует)
                        tbl.Cells[row, col++] = tm.TimeFrame.ToString();

                        //номер прохода
                        try
                        {
                            TModel baseModel = tm.TimeFrame == TFBase ? null : quotes.Symbol.MTree.GetModel(tm.TimeFrame, tm.IDonBaseTF);
                            tbl.Cells[row, col++] = baseModel != null ? stepi[baseModel.Step].ToString() + "::" + stepi[tm.Step].ToString() : stepi[tm.Step].ToString();
                        }
                        catch (Exception ee)
                        {
                            tbl.Cells[row, col++] = ee.Message;
                        }

                        //switch (tm.Step)
                        //{
                        //    case 1: tbl.Cells[row, col++] = 1; break;
                        //    case 2: tbl.Cells[row, col++] = 2; break;
                        //    case 4: tbl.Cells[row, col++] = 3; break;
                        //}

                        //статус модели (базовая\по тренду\коррекция)
                        tbl.Cells[row, col++] = ModelStatus(tm);
                        //вверх/вниз
                        tbl.Cells[row, col++] = tm.ModelDir == TDirection.dtUp ? "Up" : "Down";

                        //значения точек модели

                        double val;
                        int decs = quotes.Symbol.Decimals;
                        tbl.Cells[row, col++] = tm.Point1.DT.ToString("dd.MM.yy HH:mm");
                        val = tm.PointSP2.Price; tbl.Cells[row, col++] = GlobalMembersTAmodel.round(val > -1 && quotes.log ? Math.Pow(10, val) : val, decs); //SP
                        val = tm.Point1.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;
                        val = tm.Point2.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;
                        val = tm.Point3.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;
                        val = tm.Point4.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;
                        val = tm.Point5.Price; if (val != 0) tbl.Cells[row, col] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val; col++;
                        val = tm.Point61.Price; if (val != 0) tbl.Cells[row, col] = GlobalMembersTAmodel.round(val > -1 && quotes.log ? Math.Pow(10, val) : val, decs); col++; //HP
                        val = tm.Point6.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;

                        //<контекст>//////////////////////////////////////////////////////////////////////////////////
                        //индексы трех последних моделей,
                            //эта часть только для моделей от глобальных 1-х точек
                            //if ((points[(int)tm.Point1.Bar] & 0x0FFFF0) == 0)
                            //{
                            int[] bfr = { -1, -1, -1 };
                            
                            //определение трех моделей противоположно направленных, полностью описывающих свой тренд
                            GetContextModelIndexes(id, quotes, ref bfr);

                            //проход каждой из 3-х предшествующих
                            //int[] stepi = { 0, 1, 2, 0, 3 }; //Step == 1|2|4
                            if (bfr[0] >= 0) tbl.Cells[row, col] = stepi[tmodels[bfr[0]].Step]; col++;
                            if (bfr[1] >= 0) tbl.Cells[row, col] = stepi[tmodels[bfr[1]].Step]; col++;
                            if (bfr[2] >= 0) tbl.Cells[row, col] = stepi[tmodels[bfr[2]].Step]; col++;

                            //тип каждой из 3-х предшествующих
                            tbl.Cells[row, col++] = ModelType2str(tmodels, bfr[0]);
                            tbl.Cells[row, col++] = ModelType2str(tmodels, bfr[1]);
                            tbl.Cells[row, col++] = ModelType2str(tmodels, bfr[2]);

                            //полная характеристика по каждой из 3-х предшествующих => здесь линки на их описание на соответствующей странице
                            string[] pagename = { "'MExp'", "'MDB'", "'MAttr'" };
                            for (int i = 0; i < 3; i++)
                            {
                                if (bfr[i] >= 0 && modelrefs.Length > tmodels[bfr[i]].ModelID)
                                    tbl.Hyperlinks.Add(
                                        tbl.get_Range(tbl.Cells[row, col], tbl.Cells[row, col]), //current cell
                                        "",                                                      //target file name
                                        pagename[ModelType2int(tmodels, bfr[i])] +
                                        "!A" + (modelrefs[tmodels[bfr[i]].ModelID] + ri),        //address of target cell
                                        "go to model description",                               //tooltips
                                        tmodels[bfr[i]].ModelID.ToString()                       //text
                                        );
                                col++;
                            }

                            //Общее количество баров для этих трех моделей
                            int allbars = barcount(bfr, tmodels, -1, true);
                            if (allbars != 0) tbl.Cells[row, col] = allbars; col++;

                            //Сумма в пунктах всего движения описываемого тремя моделями
                            int allpips = pipscount(bfr, tmodels, -1, true);
                            if (allpips != 0) tbl.Cells[row, col] = allpips; col++;

                            //Соотношение пунктов 4 и 5
                            if (allpips > 0) tbl.Cells[row, col] = Math.Round((double)allpips / allbars, 1);
                            col++;

                            //7.	Скорость для каждой из трех моделей по отдельности. 
                            setval(tbl, row, col++, modelSpeed(bfr, tmodels, 0, true));
                            setval(tbl, row, col++, modelSpeed(bfr, tmodels, 1, true));
                            setval(tbl, row, col++, modelSpeed(bfr, tmodels, 2, true));

                            //8.	Модель по тренду или от начала тренда для своего Плана или коррекционная.
                            tbl.Cells[row, col++] = ModelStatusContext(tmodels[id]);

                            //9.	Если модель по тренду то: ее 1 точка равна или нет 3 точки
                            tbl.Cells[row, col++] = ModelStatus_1eq3(tmodels[id]);

                            //10.	Соотношение наклона трендовой к горизонтали модели к каждой из трех по отдельности трендовых моделей контекста.
                            setval(tbl, row, col++, bfr[0] == -1 ? 0 : Math.Round(tm.TrendLine.Delta / tmodels[bfr[0]].TrendLine.Delta, 2));
                            setval(tbl, row, col++, bfr[1] == -1 ? 0 : Math.Round(tm.TrendLine.Delta / tmodels[bfr[1]].TrendLine.Delta, 2));
                            setval(tbl, row, col++, bfr[2] == -1 ? 0 : Math.Round(tm.TrendLine.Delta / tmodels[bfr[2]].TrendLine.Delta, 2));

                            //11.	Соотношение наклона целевой к горизонтали модели к каждой из трех по отдельности целевой моделей контекста.
                            setval(tbl, row, col++, bfr[0] == -1 ? 0 : Math.Round(tm.TargetLine.Delta / tmodels[bfr[0]].TargetLine.Delta, 2));
                            setval(tbl, row, col++, bfr[1] == -1 ? 0 : Math.Round(tm.TargetLine.Delta / tmodels[bfr[1]].TargetLine.Delta, 2));
                            setval(tbl, row, col++, bfr[2] == -1 ? 0 : Math.Round(tm.TargetLine.Delta / tmodels[bfr[2]].TargetLine.Delta, 2));

                            //12.	Соотношение скорости модели к каждой из трех скоростей моделей контекста по отдельности.
                            setval(tbl, row, col++, ModelSpeedRatio(tmodels, id, bfr[0], true));
                            setval(tbl, row, col++, ModelSpeedRatio(tmodels, id, bfr[1], true));
                            setval(tbl, row, col++, ModelSpeedRatio(tmodels, id, bfr[2], true));

                            //13.	Для каждой модели контекста указать: достигнута НР, 100%, 200%, 1, 2, 3 цель, в рамках (ст-4)*4.
                            //tbl.Cells[row, col++] = GetupTargets(tmodels[bfr[0]], models[bfr[0]]);
                            //tbl.Cells[row, col++] = GetupTargets(tmodels[bfr[1]], models[bfr[1]]);
                            //tbl.Cells[row, col++] = GetupTargets(tmodels[bfr[2]], models[bfr[2]]);

                            //}
                            //else
                            //{
                            //    col += context_cols_count;
                            //}
                        //</контекст>//////////////////////////////////////////////////////////////////////////////////


                        //сила модели
                        tbl.Cells[row, col++] = tm.power;
                        //4.	Соотношение Базы к БЦ 1-3 к 4-5 и 1-2 к 4-5 в %
                        if (tm.Point5.pt.X == 0)
                        {
                            //tbl.Cells[row, col++] = "n/a";
                            //tbl.Cells[row, col++] = "-/-";
                            col += 2;
                        }
                        else
                        {
                            tbl.Cells[row, col++] = Math.Round((tm.Point3.pt.X - tm.Point1.pt.X + 1) / (tm.Point5.pt.X - tm.Point4.pt.X + 1) * 100) + "%";
                            tbl.Cells[row, col++] = Math.Round(((tm.Point2.pt.Y - tm.Point1.pt.Y + pip) / pip) / ((tm.Point4.pt.Y - tm.Point5.pt.Y + pip) / pip) * 100) + "%";
                        }
                        //5.	Соотношение Базы ко всей модели – 1-3 к 1-6 (не НР, а именно 6) и 1-2 к 1-6 в %
                        if (tm.Point6.pt.X == 0)
                        {
                            //tbl.Cells[row, col++] = "n/a";
                            //tbl.Cells[row, col++] = "-/-";
                            col += 2;
                        }
                        else
                        {
                            tbl.Cells[row, col++] = Math.Round((tm.Point3.pt.X - tm.Point1.pt.X + 1) / (tm.Point6.pt.X - tm.Point1.pt.X + 1) * 100) + "%";
                            tbl.Cells[row, col++] = Math.Round(((tm.Point2.pt.Y - tm.Point1.pt.Y + pip) / pip) / ((tm.Point6.pt.Y - tm.Point1.pt.Y + pip) / pip) * 100) + "%";
                        }
                        //6.	Соотношение БЦ ко всей модели – 4-5 к 1-6 по вертикали (не НР, а именно 6) и 4-5 к 1-6 по горизонтали в %
                        if (tm.Point5.pt.X == 0)
                        {
                            //tbl.Cells[row, col++] = "n/a";
                            //tbl.Cells[row, col++] = "-/-";
                            col += 2;
                        }
                        else
                        {
                            tbl.Cells[row, col++] = Math.Round((tm.Point5.pt.X - tm.Point4.pt.X + 1) / (tm.Point6.pt.X - tm.Point1.pt.X + 1) * 100) + "%";
                            tbl.Cells[row, col++] = Math.Round(((tm.Point4.pt.Y - tm.Point5.pt.Y + pip) / pip) / ((tm.Point6.pt.Y - tm.Point1.pt.Y + pip) / pip) * 100) + "%";
                        }
                        //7.	Соотношение 2-5 к 1-6 в % (|)
                        if (tm.Point5.pt.X == 0)
                        {
                            //tbl.Cells[row, col++] = "n/a";
                            col++;
                        }
                        else
                        {
                            tbl.Cells[row, col++] = Math.Round(((tm.Point5.pt.Y - tm.Point2.pt.Y + pip) / pip) / ((tm.Point6.pt.Y - tm.Point1.pt.Y + pip) / pip) * 100) + "%";
                        }
                        //8.	Соотношение 1-2 к 3-4 в % (|)
                        tbl.Cells[row, col++] = Math.Round(((tm.Point1.pt.Y - tm.Point2.pt.Y + pip) / pip) / ((tm.Point3.pt.Y - tm.Point4.pt.Y + pip) / pip) * 100) + "%";
                        //9.	Соотношение 3-4 к 1-6 в % (|)
                        if (tm.Point6.pt.X == 0)
                        {
                            //tbl.Cells[row, col++] = "n/a";
                            col++;
                        }
                        else
                        {
                            tbl.Cells[row, col++] = Math.Round(((tm.Point3.pt.Y - tm.Point4.pt.Y + pip) / pip) / ((tm.Point1.pt.Y - tm.Point6.pt.Y + pip) / pip) * 100) + "%";
                        }
                        //10.	Соотношение ст-1 к 4-5 в % (-)
                        if (tm.Point5.pt.X == 0)
                        {
                            //tbl.Cells[row, col++] = "n/a";
                            col++;
                        }
                        else
                        {
                            tbl.Cells[row, col++] = Math.Round((tm.PointSP2.pt.X - tm.Point1.pt.X - 1) / (tm.Point4.pt.X - tm.Point5.pt.X - 1) * 100) + "%";
                        }
                        //11.	Соотношение (наклона целевой к вертикали) к (наклону трендовой к горизонтали)
                        tbl.Cells[row, col++] = (tm.Point11.pt.Y == tm.Point32.pt.Y ? 0 :
                            Math.Round(
                            ((tm.Point23.pt.X - tm.Point41.pt.X - 1) / ((tm.Point23.pt.Y - tm.Point41.pt.Y - pip) / pip)) /
                            (((tm.Point11.pt.Y - tm.Point32.pt.Y - pip) / pip) / (tm.Point11.pt.X - tm.Point32.pt.X - 1)),
                            2));

                        //12.	Достижение НР до пробития целевой и трендовой на промежутке (ст-4)*4
                        tbl.Cells[row, col++] = tm.HPGetInside ? "Yes" : tm.HPGetOutside ? "Yes*" : "No";

                        //13.	Достижение 100% и 200% после пробития ЛЦ на промежутке (ст-4)*4
                        setval(tbl, row, col++, flags.BreakTargetline_100pct == 1, "+");
                        setval(tbl, row, col++, flags.BreakTargetline_200pct == 1, "+");

                        //14.	Достижение 100% и 200% после пробития НР** на промежутке (ст-4)*4
                        setval(tbl, row, col++, flags.BreakHP_100pct == 1, "+");
                        setval(tbl, row, col++, flags.BreakHP_200pct == 1, "+");

                        //** (пробитие HP – повторный подход к уровню НР с предыдущим образованием противоположного 4 точке абс. экстремума (годного по алгоритму отбора экстремумов Алексея, который отрабатывает на старте алгоритма поиска моделей,  после первого достижения НР)

                        //15.	Достижение 100% и 200% без повторного пробития НР*** на промежутке (ст-4)*4
                        setval(tbl, row, col++, flags.ClearHP_100pct == 1, "+");
                        setval(tbl, row, col++, flags.ClearHP_200pct == 1, "+");

                        //16.	Достижение 100% и 200% после пробития ЛЦ и до пробития ЛТ на промежутке (ст-4)*4
                        setval(tbl, row, col++, flags.BreakTargetline_100pct_CleanLTrend == 1, "+");
                        setval(tbl, row, col++, flags.BreakTargetline_200pct_CleanLTrend == 1, "+");

                        //17.	Достижение 100% и 200% после пробития НР и до пробития ЛТ на промежутке (ст-4)*4
                        setval(tbl, row, col++, flags.BreakHP_100pct_CleanLTrend == 1, "+");
                        setval(tbl, row, col++, flags.BreakHP_200pct_CleanLTrend == 1, "+");

                        //17.a	Достижение 100% и 200% (по отдельности) без повторного пробития НР и до пробития ЛТ на промежутке (ст-4)*4
                        setval(tbl, row, col++, flags.ClearHP_100pct_CleanLTrend == 1, "+");
                        setval(tbl, row, col++, flags.ClearHP_200pct_CleanLTrend == 1, "+");

                        //18.	Достижение целей (3-х по отдельности) без пробития целевой на промежутке (ст-4)*4
                        setval(tbl, row, col++, flags.Target1_ClearTargetline == 1, "+");
                        setval(tbl, row, col++, flags.Target2_ClearTargetline == 1, "+");
                        setval(tbl, row, col++, flags.Target3_ClearTargetline == 1, "+");

                        //19.	Достижение целей (3-х по отдельности) с пробитой целевой на промежутке (ст-4)*4
                        setval(tbl, row, col++, flags.Target1_BreakTargetline == 1, "+");
                        setval(tbl, row, col++, flags.Target2_BreakTargetline == 1, "+");
                        setval(tbl, row, col++, flags.Target3_BreakTargetline == 1, "+");

                        //20.	Достижение целей (3-х по отдельности) без пробития целевой, но с образованием 6, на промежутке (ст-4)*4
                        setval(tbl, row, col++, flags.Target1_ClearTargetline_with6pts == 1, "+");
                        setval(tbl, row, col++, flags.Target2_ClearTargetline_with6pts == 1, "+");
                        setval(tbl, row, col++, flags.Target3_ClearTargetline_with6pts == 1, "+");

                        //21.	Достижение целей (3-х по отдельности) без пробития целевой, но без  образования 6 (т.е. 4=6), на промежутке (ст-4)*4
                        setval(tbl, row, col++, flags.Target1_ClearTargetline_with4pts == 1, "+");
                        setval(tbl, row, col++, flags.Target2_ClearTargetline_with4pts == 1, "+");
                        setval(tbl, row, col++, flags.Target3_ClearTargetline_with4pts == 1, "+");

                        //22.	Соотношение (6 - 1 цель по горизонтали) к 1-6
                        setval(tbl, row, col++, flags.P6_T1x1_6 == -1 ? 0 : flags.P6_T1x1_6);
                        //23.	Соотношение (6 - 2 цель по горизонтали) к 1-6
                        setval(tbl, row, col++, flags.P6_T2x1_6 == -1 ? 0 : flags.P6_T2x1_6);
                        //24.	Соотношение (6 - 3 цель по горизонтали) к 1-6
                        setval(tbl, row, col++, flags.P6_T3x1_6 == -1 ? 0 : flags.P6_T3x1_6);

                        //27.	Доход до НР  в % к 1-НР
                        //int НРx1_НР = Get_6xHPpercent(tmodels[id]);
                        tbl.Cells[row, col++] = flags.НРx1_НР != -99999 && flags.НРx1_НР >= 0 ? (flags.НРx1_НР + "%") : "";

                        //28.	Недоход до НР в % к 1-НР
                        tbl.Cells[row, col++] = flags.НРx1_НР != -99999 && flags.НРx1_НР < 0 ? (flags.НРx1_НР + "%") : "";

                        //* 29. 1 цель - последнее касание трендовой / 6 - первое касание трендовой
                        setval(tbl, row, col++, flags.TimeOfTarget1);


                        //L2. время достижения СТ при ходе от 6-й в единицах СТ- 4.
                        setval(tbl, row, col++, flags.TimeOf6ST);

                        //L3. был ли откат от 6-й до 4-й
                        setval(tbl, row, col++, flags.HP_recur4 != 0, "+");

                        //L4. был ли пробит лоу/хай бара 4.
                        setval(tbl, row, col++, flags.HP_break4 != 0, "+");

                        //L5. была ли пробита 5 точка.
                        setval(tbl, row, col++, flags.HP_break5 != 0, "+");

                        //L9. указать время 6-й в единицах Ст-4
                        setval(tbl, row, col++, flags.TimeOfHPGet);

                        //перелет(+)/недолет(-) 6-й до СТ4х1 для слабых моделей
                        setval(tbl, row, col++, tm.power <= 1 && flags.P6_CP4x1 != 0, flags.P6_CP4x1 + "%");
                        //перелет(+)/недолет(-) 6-й до СТ4х2 для сильных моделей
                        setval(tbl, row, col++, tm.power > 1 && flags.P6_CP4x2 != 0, flags.P6_CP4x2 + "%");

                        //L6. указать время пробития трендовой в единицах Ст-4.
                        setval(tbl, row, col++, flags.TimeOfTrendBreak);

                        //перелет(+)/недолет(-) пробития трендовой до СТ4х1 для слабых моделей
                        setval(tbl, row, col++, tm.power <= 1 && flags.TrendBreak_CP4x1 != 0, flags.TrendBreak_CP4x1 + "%");
                        //перелет(+)/недолет(-) пробития трендовой до СТ4х2 для сильных моделей
                        setval(tbl, row, col++, tm.power > 1 && flags.TrendBreak_CP4x2 != 0, flags.TrendBreak_CP4x2 + "%");

                        //L7. указать время пробития 6-й (если он был) в единицах СТ-4.
                        setval(tbl, row, col++, flags.TimeOfHPBreak);

                        //L8. площади треугольников 123,234,345,456
                        flags.CountTrianglesArea(tmodels[id], quotes);
                        if (flags.S123 != 0) tbl.Cells[row, col] = flags.S123; col++;
                        if (flags.S234 != 0) tbl.Cells[row, col] = flags.S234; col++;
                        if (flags.S345 != 0) tbl.Cells[row, col] = flags.S345; col++;
                        if (flags.S456 != 0) tbl.Cells[row, col] = flags.S456; col++;
                    }
                    //tbl.Columns.AutoFit();
                    tbl.Columns.ColumnWidth = 7;
                }
                catch (Exception ee)
                {
                    try{System.IO.File.AppendAllText(@"C:\sc_err.log","[inner] "+ee.ToString()+"\r\n\r\n");}catch{}
                    MessageBox.Show("StatCalc::describe: " + ee.Message);
                }

                worker.ReportProgress(3);
                try{
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //страница 3 -- МДР
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                tbl = (CsvSheet)tbl.Next;
                tbl.Name = "MDB";

                row = 1; col = 1;
                tbl.Cells[row, col++] = "МДР";
                smallfont(row, col, tbl);
                merge(row, row, col, col + 4, tbl);
                tbl.Cells[row++, col] = "Models of dynamic balance";

                //шапка таблицы
                col = 1;
                smallfont(row + 1, 1, row + 1, 30, tbl);
                tbl.Cells[row, col++] = "ID: ";
                tbl.Cells[row, col++] = "TF: ";
                tbl.Cells[row + 1, col] = "Iteration: (base::current model)";
                tbl.Cells[row, col++] = "Проход: (базовой::данной модели)";
                tbl.Cells[row + 1, col] = "Status: *(T1=model from the source of trend (base model), T3=model by trend, Corr=correction model, T3Corr=by trend of correction model, 1*=model from the source of trend without previous model p.6 confirmation)";
                tbl.Cells[row, col++] = "Статус: *(T1=базовая, T3=по тренду, Corr=коррекционная, T3Corr=по тренду коррекционной модели, 1*=базовая, но не подтверждена другой 6-й)";
                tbl.Cells[row + 1, col] = "Direction: ";
                tbl.Cells[row, col++] = "Направление: ";

                //значения точек модели
                merge(row, row, col, col + 7, tbl);
                merge(row + 1, row + 1, col, col + 7, tbl);
                tbl.Cells[row + 1, col] = "Model`s points";
                tbl.Cells[row, col] = "Значения точек модели: ";
                tbl.Cells[row + 2, col++] = "p1 Date Time";
                tbl.Cells[row + 2, col++] = "1";
                tbl.Cells[row + 2, col++] = "2";
                tbl.Cells[row + 2, col++] = "3";
                tbl.Cells[row + 2, col++] = "4";
                tbl.Cells[row + 2, col++] = "5";
                tbl.Cells[row + 2, col++] = "HP";
                tbl.Cells[row + 2, col++] = "6";

                //3.	Достижение 100% и 200% после пробития ЛЦ на промежутке (1-4)*4
                merge(row, row, col, col + 1, tbl);
                merge(row + 1, row + 1, col, col + 1, tbl);
                tbl.Cells[row + 1, col] = "Reaching of 100% and 200% after Target line break on (CP-p.4)*4 distance";
                tbl.Cells[row, col] = "Достижение 100% и 200% после пробития ЛЦ на промежутке (1-4)*4: ";
                tbl.Cells[row + 2, col++] = "100%";
                tbl.Cells[row + 2, col++] = "200%";

                //4.	Достижение 100% и 200% после (повторного) пробития НР** на промежутке (1-4)*4
                merge(row, row, col, col + 1, tbl);
                merge(row + 1, row + 1, col, col + 1, tbl);
                tbl.Cells[row + 1, col] = "Reaching of 100% and 200% after HP break on (CP-p.4)*4 distance";
                tbl.Cells[row, col] = "Достижение 100% и 200% после пробития HP** на промежутке (1-4)*4: ";
                tbl.Cells[row + 2, col++] = "100%";
                tbl.Cells[row + 2, col++] = "200%";

                //5.	Достижение 100% и 200% без повторного пробития НР*** на промежутке (1-4)*4
                merge(row, row, col, col + 1, tbl);
                merge(row + 1, row + 1, col, col + 1, tbl);
                tbl.Cells[row + 1, col] = "Reaching of 100% and 200% without recurring HP break on (CP-p.4)*4 distance";
                tbl.Cells[row, col] = "Достижение 100% и 200% без повторного пробития HP на промежутке (1-4)*4: ";
                tbl.Cells[row + 2, col++] = "100%";
                tbl.Cells[row + 2, col++] = "200%";

                //*** (т.е. уровень НР достигнут с перелетом или пункт в пункт не важно и следующими барами цена идет дальше без образования противоположного абс. экстремума)

                //6.	Достижение 100% и 200% после пробития ЛЦ и до пробития ЛТ на промежутке (1-4)*4
                merge(row, row, col, col + 1, tbl);
                merge(row + 1, row + 1, col, col + 1, tbl);
                tbl.Cells[row + 1, col] = "Reaching of 100% and 200% after Target Line break and before Trend Line break on (CP-p.4)*4 distance";
                tbl.Cells[row, col] = "Достижение 100% и 200% после пробития ЛЦ и до пробития ЛТ на промежутке (1-4)*4: ";
                tbl.Cells[row + 2, col++] = "100%";
                tbl.Cells[row + 2, col++] = "200%";

                //7.	Достижение 100% и 200% после пробития HP и до пробития ЛТ на промежутке (1-4)*4
                merge(row, row, col, col + 1, tbl);
                merge(row + 1, row + 1, col, col + 1, tbl);
                tbl.Cells[row + 1, col] = "Reaching of 100% and 200% after HP level break and before Trend Line break on (CP-p.4)*4 distance";
                tbl.Cells[row, col] = "Достижение 100% и 200% после пробития HP и до пробития ЛТ на промежутке (1-4)*4: ";
                tbl.Cells[row + 2, col++] = "100%";
                tbl.Cells[row + 2, col++] = "200%";

                //8.	Достижение целей без пробития целевой на промежутке (1-4)*4
                merge(row, row, col, col + 2, tbl);
                merge(row + 1, row + 1, col, col + 2, tbl);
                tbl.Cells[row + 1, col] = "Reaching of targets without Target Line break on (CP-p.4)*4 distance";
                tbl.Cells[row, col] = "Достижение целей без пробития целевой на промежутке (1-4)*4: ";
                tbl.Cells[row + 2, col++] = "1";
                tbl.Cells[row + 2, col++] = "2";
                tbl.Cells[row + 2, col++] = "3";

                //9.	Достижение целей с пробитой целевой на промежутке (1-4)*4
                merge(row, row, col, col + 2, tbl);
                merge(row + 1, row + 1, col, col + 2, tbl);
                tbl.Cells[row + 1, col] = "Reaching of targets with Target Line break on (CP-p.4)*4 distance";
                tbl.Cells[row, col] = "Достижение целей с пробитой целевой на промежутке (1-4)*4: ";
                tbl.Cells[row + 2, col++] = "1";
                tbl.Cells[row + 2, col++] = "2";
                tbl.Cells[row + 2, col++] = "3";

                //10.	Достижение целей без пробития целевой, но с образованием 6, на промежутке (1-4)*4
                merge(row, row, col, col + 2, tbl);
                merge(row + 1, row + 1, col, col + 2, tbl);
                tbl.Cells[row + 1, col] = "Reaching of targets without Target Line break but with forming of p.6 on (CP-p.4)*4 distance";
                tbl.Cells[row, col] = "Достижение целей без пробития целевой, но с образованием 6, на промежутке (1-4)*4: ";
                tbl.Cells[row + 2, col++] = "1";
                tbl.Cells[row + 2, col++] = "2";
                tbl.Cells[row + 2, col++] = "3";

                //11.	Достижение целей без пробития целевой, и без образования 6 (т.е. 4=6), на промежутке (1-4)*4
                merge(row, row, col, col + 2, tbl);
                merge(row + 1, row + 1, col, col + 2, tbl);
                tbl.Cells[row + 1, col] = "Reaching of targets without Target Line break and without forming of p.6 (p.4=p.6) on (CP-p.4)*4 distance";
                tbl.Cells[row, col] = "Достижение целей без пробития целевой, и без образования 6 (т.е. 4=6), на промежутке (ст-4)*4: ";
                tbl.Cells[row + 2, col++] = "1";
                tbl.Cells[row + 2, col++] = "2";
                tbl.Cells[row + 2, col++] = "3";

                //12.	Соотношение (6 - 1 цель по горизонтали) к 1-6
                tbl.Cells[row + 1, col] = "Ratio of (p.6-1st target) to (p.1-p.6)";
                tbl.Cells[row, col++] = "Соотношение (6 - 1 цель по горизонтали) к 1-6: ";

                //13.	Соотношение (6 - 2 цель по горизонтали) к 1-6
                tbl.Cells[row + 1, col] = "Ratio of (p.6-2nd target) to (p.1-p.6)";
                tbl.Cells[row, col++] = "Соотношение (6 - 2 цель по горизонтали) к 1-6: ";

                //14.	Соотношение (6 - 3 цель по горизонтали) к 1-6
                tbl.Cells[row + 1, col] = "Ratio of (p.6-3rd target) to (p.1-p.6)";
                tbl.Cells[row, col++] = "Соотношение (6 - 3 цель по горизонтали) к 1-6: ";

                id = -1;
                row += 2;
                foreach (Model m in models)
                {
                    id++;
                    TModel tm = tmodels[id];
                    if (tm.ModelType != Common.TModelType.ModelOfBalance) continue;
                    double pip = tm.ModelDir == TDirection.dtUp ? quotes.Symbol.pip : -quotes.Symbol.pip;
                    worker.ReportProgress(300);

                    flags.checkup(quotes, tm, m);
                    row++; col = 1;
                    //индекс модели (соотвтствует индексу на графике)
                    tbl.Cells[row, col++] = id;
                    //ТаймФрейм модели (соответствует Старшему из планов где модель существует)
                    tbl.Cells[row, col++] = tm.TimeFrame.ToString();
                    //номер прохода
                    try
                    {
                        TModel baseModel = tm.TimeFrame == TFBase ? null : quotes.Symbol.MTree.GetModel(tm.TimeFrame, tm.IDonBaseTF);
                        tbl.Cells[row, col++] = baseModel != null ? stepi[baseModel.Step].ToString() + "::" + stepi[tm.Step].ToString() : stepi[tm.Step].ToString();
                    }
                    catch(Exception ee)
                    {
                        tbl.Cells[row, col++] = ee.Message;
                    }
                    //switch (tm.Step)
                    //{
                    //    case 1: tbl.Cells[row, col++] = 1; break;
                    //    case 2: tbl.Cells[row, col++] = 2; break;
                    //    case 4: tbl.Cells[row, col++] = 3; break;
                    //}
                    //статус модели (базовая\по тренду\коррекция)
                    tbl.Cells[row, col++] = ModelStatus(tm);
                    //вверх/вниз
                    tbl.Cells[row, col++] = tm.ModelDir == TDirection.dtUp ? "Up" : "Down";

                    //значения точек модели
                    double val;
                    int decs = quotes.Symbol.Decimals;
                    tbl.Cells[row, col++] = tm.Point1.DT.ToString("dd.MM.yy HH:mm");
                    val = tm.Point1.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;
                    val = tm.Point2.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;
                    val = tm.Point3.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;
                    val = tm.Point4.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;
                    val = tm.Point5.Price; if (val != 0) tbl.Cells[row, col] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val; col++;
                    val = tm.Point61.Price; if (val != 0) tbl.Cells[row, col] = GlobalMembersTAmodel.round(val > -1 && quotes.log ? Math.Pow(10, val) : val, decs); col++; //HP
                    val = tm.Point6.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;

                    //3.	Достижение 100% и 200% после пробития ЛЦ на промежутке (1-4)*4
                    setval(tbl, row, col++, flags.BreakTargetline_100pct == 1, "+");
                    setval(tbl, row, col++, flags.BreakTargetline_200pct == 1, "+");

                    //4.	Достижение 100% и 200% после пробития НР** на промежутке (1-4)*4
                    setval(tbl, row, col++, flags.BreakHP_100pct == 1, "+");
                    setval(tbl, row, col++, flags.BreakHP_200pct == 1, "+");

                    //** (пробитие HP – повторный подход к уровню НР с предыдущим образованием противоположного 4 точке абс. экстремума (годного по алгоритму отбора экстремумов Алексея, который отрабатывает на старте алгоритма поиска моделей,  после первого достижения НР)

                    //5.	Достижение 100% и 200% без повторного пробития НР*** на промежутке (ст-4)*4
                    setval(tbl, row, col++, flags.ClearHP_100pct == 1, "+");
                    setval(tbl, row, col++, flags.ClearHP_200pct == 1, "+");

                    //6.	Достижение 100% и 200% после пробития ЛЦ и до пробития ЛТ на промежутке (ст-4)*4
                    setval(tbl, row, col++, flags.BreakTargetline_100pct_CleanLTrend == 1, "+");
                    setval(tbl, row, col++, flags.BreakTargetline_200pct_CleanLTrend == 1, "+");

                    //7.	Достижение 100% и 200% после пробития НР и до пробития ЛТ на промежутке (ст-4)*4
                    setval(tbl, row, col++, flags.BreakHP_100pct_CleanLTrend == 1, "+");
                    setval(tbl, row, col++, flags.BreakHP_200pct_CleanLTrend == 1, "+");

                    //8.	Достижение целей (3-х по отдельности) без пробития целевой на промежутке (ст-4)*4
                    setval(tbl, row, col++, flags.Target1_ClearTargetline == 1, "+");
                    setval(tbl, row, col++, flags.Target2_ClearTargetline == 1, "+");
                    setval(tbl, row, col++, flags.Target3_ClearTargetline == 1, "+");

                    //9.	Достижение целей (3-х по отдельности) с пробитой целевой на промежутке (ст-4)*4
                    setval(tbl, row, col++, flags.Target1_BreakTargetline == 1, "+");
                    setval(tbl, row, col++, flags.Target2_BreakTargetline == 1, "+");
                    setval(tbl, row, col++, flags.Target3_BreakTargetline == 1, "+");

                    //10.	Достижение целей (3-х по отдельности) без пробития целевой, но с образованием 6, на промежутке (ст-4)*4
                    setval(tbl, row, col++, flags.Target1_ClearTargetline_with6pts == 1, "+");
                    setval(tbl, row, col++, flags.Target2_ClearTargetline_with6pts == 1, "+");
                    setval(tbl, row, col++, flags.Target3_ClearTargetline_with6pts == 1, "+");

                    //11.	Достижение целей (3-х по отдельности) без пробития целевой, но без  образования 6 (т.е. 4=6), на промежутке (ст-4)*4
                    setval(tbl, row, col++, flags.Target1_ClearTargetline_with4pts == 1, "+");
                    setval(tbl, row, col++, flags.Target2_ClearTargetline_with4pts == 1, "+");
                    setval(tbl, row, col++, flags.Target3_ClearTargetline_with4pts == 1, "+");

                    //12.	Соотношение (6 - 1 цель по горизонтали) к 1-6
                    setval(tbl, row, col++, flags.P6_T1x1_6 > -1, flags.P6_T1x1_6);
                    //13.	Соотношение (6 - 2 цель по горизонтали) к 1-6
                    setval(tbl, row, col++, flags.P6_T2x1_6 > -1, flags.P6_T2x1_6);
                    //14.	Соотношение (6 - 3 цель по горизонтали) к 1-6
                    setval(tbl, row, col++, flags.P6_T3x1_6 > -1, flags.P6_T3x1_6);
                }
                //tbl.Columns.AutoFit();
                tbl.Columns.ColumnWidth = 7;
                }
                catch (Exception ee)
                {
                    try{System.IO.File.AppendAllText(@"C:\sc_err.log","[inner] "+ee.ToString()+"\r\n\r\n");}catch{}
                    MessageBox.Show("StatCalc::describe: " + ee.Message);
                }

                worker.ReportProgress(4);
                try{
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //страница 4 - МП
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                tbl = (CsvSheet)tbl.Next;
                tbl.Name = "MAttr";

                row = 1; col = 1;
                tbl.Cells[row, col++] = "МП";
                smallfont(row, col, tbl);
                merge(row, row, col, col + 4, tbl);
                tbl.Cells[row++, col] = "Models of attraction";

                //шапка таблицы
                col = 1;
                smallfont(row + 1, 1, row + 1, 30, tbl);
                tbl.Cells[row, col++] = "ID: ";
                tbl.Cells[row, col++] = "TF: ";
                tbl.Cells[row + 1, col] = "Iteration: (base::current model)";
                tbl.Cells[row, col++] = "Проход: (базовой::данной модели)";
                tbl.Cells[row + 1, col] = "Status: *(T1=model from the source of trend (base model), T3=model by trend, Corr=correction model, T3Corr=by trend of correction model, 1*=model from the source of trend without previous model p.6 confirmation)";
                tbl.Cells[row, col++] = "Статус: *(T1=базовая, T3=по тренду, Corr=коррекционная, T3Corr=по тренду коррекционной модели, 1*=базовая, но не подтверждена другой 6-й)";
                tbl.Cells[row + 1, col] = "Direction: ";
                tbl.Cells[row, col++] = "Направление: ";

                //значения точек модели
                merge(row, row, col, col + 5, tbl);
                merge(row + 1, row + 1, col, col + 5, tbl);
                tbl.Cells[row + 1, col] = "Model`s points";
                tbl.Cells[row, col] = "Значения точек модели: ";
                tbl.Cells[row + 2, col++] = "p1 Date Time";
                tbl.Cells[row + 2, col++] = "1";
                tbl.Cells[row + 2, col++] = "2";
                tbl.Cells[row + 2, col++] = "3";
                tbl.Cells[row + 2, col++] = "4";
                //tbl.Cells[row + 2, col++] = "5";
                tbl.Cells[row + 2, col++] = "HP";
                //tbl.Cells[row + 2, col++] = "6";

                //3,4,5.	Соотношение БЦ ко всей модели – 4-5 к 1-6 по вертикали (не НР, а именно 6) и 4-5 к 1-6 по горизонтали в %
                merge(row, row, col, col + 2, tbl);
                merge(row + 1, row + 1, col, col + 2, tbl);
                tbl.Cells[row + 1, col] = "Ratios of";
                tbl.Cells[row, col] = "Соотношения: ";
                tbl.Cells[row + 2, col++] = "1-2/3-4";
                tbl.Cells[row + 2, col++] = "1-3/2-4";
                tbl.Cells[row + 2, col++] = "1-4/4-HP";

                //6.	Достижение 100% и 200% до пробития 1 точки на участке (1-НР)*2
                merge(row, row, col, col + 1, tbl);
                merge(row + 1, row + 1, col, col + 1, tbl);
                tbl.Cells[row + 1, col] = "Reaching 100% and 200% before p.1 break on (p.1-НР)*2 distance";
                tbl.Cells[row, col] = "Достижение 100% и 200% до пробития 1 точки на участке (1-НР)*2: ";
                tbl.Cells[row + 2, col++] = "100%";
                tbl.Cells[row + 2, col++] = "200%";

                //7.	Достижение 100% и 200% после пробития 1 точки на участке (1-НР)*2
                merge(row, row, col, col + 1, tbl);
                merge(row + 1, row + 1, col, col + 1, tbl);
                tbl.Cells[row + 1, col] = "Reaching 100% and 200% after p.1 break on (p.1-НР)*2 distance";
                tbl.Cells[row, col] = "Достижение 100% и 200% после пробития 1 точки на участке (1-НР)*2: ";
                tbl.Cells[row + 2, col++] = "100%";
                tbl.Cells[row + 2, col++] = "200%";

                //8.	Доход до НР  в % к 1-НР
                tbl.Cells[row + 1, col] = "Percent ratio of range of HP level exceeding to (p.1-HP)";
                tbl.Cells[row, col++] = "Доход до НР  в % к 1-НР: ";

                //9.	Недоход до НР в % к 1-НР
                tbl.Cells[row + 1, col] = "Percent ratio of range of HP level unreaching to (p.1-НР)";
                tbl.Cells[row, col++] = "Недоход до НР в % к 1-НР: ";

                //10.	Движение от НР к 100% и 200% против движения к 1 цели в %
                merge(row, row, col, col + 2, tbl);
                merge(row + 1, row + 1, col, col + 2, tbl);
                tbl.Cells[row + 1, col] = "Percent of models reached 100%, 200%, 1-st target";
                tbl.Cells[row, col] = "Процентное отношение моделей достигших 100%, 200% или 1-й цели к общему количеству моделей";//"Движение от НР к 100% и 200% против движения к 1 цели в %: ";
                tbl.Cells[row + 2, col++] = "100%";
                tbl.Cells[row + 2, col++] = "200%";
                tbl.Cells[row + 2, col++] = "1-st target";

                id = -1;
                row += 2;
                foreach (Model m in models)
                {
                    id++;
                    TModel tm = tmodels[id];
                    if (/*tm.TimeFrame != TFBase || */tm.ModelType != Common.TModelType.ModelOfAttraction) continue;
                    double pip = tm.ModelDir == TDirection.dtUp ? quotes.Symbol.pip : -quotes.Symbol.pip;
                    worker.ReportProgress(400);


                    flags.checkup(quotes, tm, m);
                    row++; col = 1;
                    //индекс модели (соотвтствует индексу на графике)
                    tbl.Cells[row, col++] = id;
                    //ТаймФрейм модели (соответствует Старшему из планов где модель существует)
                    tbl.Cells[row, col++] = tm.TimeFrame.ToString();
                    //номер прохода
                    try
                    {
                        TModel baseModel = tm.TimeFrame == TFBase ? null : quotes.Symbol.MTree.GetModel(tm.TimeFrame, tm.IDonBaseTF);
                        tbl.Cells[row, col++] = baseModel != null ? stepi[baseModel.Step].ToString() + "::" + stepi[tm.Step].ToString() : stepi[tm.Step].ToString();
                    }
                    catch (Exception ee)
                    {
                        tbl.Cells[row, col++] = ee.Message;
                    }
                    //статус модели (базовая\по тренду\коррекция)
                    tbl.Cells[row, col++] = ModelStatus(tm);
                    //вверх/вниз
                    tbl.Cells[row, col++] = tm.ModelDir == TDirection.dtUp ? "Up" : "Down";

                    //значения точек модели
                    double val;
                    int decs = quotes.Symbol.Decimals;
                    tbl.Cells[row, col++] = tm.Point1.DT.ToString("dd.MM.yy HH:mm");
                    val = tm.Point1.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;
                    val = tm.Point2.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;
                    val = tm.Point3.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;
                    val = tm.Point4.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;
                    //val = tm.Point5.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;
                    val = tm.PointSP2.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val; //HP
                    //val = tm.Point6.Price; tbl.Cells[row, col++] = val > -1 && quotes.log ? GlobalMembersTAmodel.round(Math.Pow(10, val), decs) : val;

                    //3.	Соотношение 1-2 к 3-4
                    tbl.Cells[row, col++] = Math.Round(((tm.Point1.pt.Y - tm.Point2.pt.Y - pip) / pip) / ((tm.Point3.pt.Y - tm.Point4.pt.Y - pip) / pip) * 100) + "%";

                    //4.	Соотношение 1-3 к 2-4
                    tbl.Cells[row, col++] = Math.Round((tm.Point1.pt.X - tm.Point3.pt.X - 1) / (tm.Point2.pt.X - tm.Point4.pt.X - 1) * 100) + "%";

                    //5.	Соотношение 1-4 к 4-НР
                    tbl.Cells[row, col++] = Math.Round(((tm.Point1.pt.Y - tm.Point4.pt.Y - pip) / pip) / ((tm.Point4.pt.Y - tm.PointSP2.pt.Y - pip) / pip) * 100) + "%";

                    //6.	Достижение 100% и 200% до пробития 1 точки на участке (1-НР)*2
                    setval(tbl, row, col++, flags.ClearHP_100pct == 1, "+");
                    setval(tbl, row, col++, flags.ClearHP_200pct == 1, "+");

                    //7.	Достижение 100% и 200% после пробития 1 точки на участке (1-НР)*2
                    setval(tbl, row, col++, flags.BreakHP_100pct == 1, "+");
                    setval(tbl, row, col++, flags.BreakHP_200pct == 1, "+");

                    //8.	Доход до НР  в % к 1-НР
                    //int НРx1_НР = Get_6xHPpercent(tmodels[id]);
                    tbl.Cells[row, col++] = flags.НРx1_НР >= 0 ? (flags.НРx1_НР + "%") : "";

                    //9.	Недоход до НР в % к 1-НР
                    tbl.Cells[row, col++] = flags.НРx1_НР < 0 ? (flags.НРx1_НР + "%") : "";

                    //10.	Движение от НР к 100% и 200% (отдельно) против движения к 1 цели в %
                    tbl.Cells[row, col++] = flags.HP_continue != 0 ?
                        Math.Round(((flags.HP_continue - tm.PointSP2.Price + pip) / pip) / ((m._100Percent[0].y - tm.PointSP2.Price + pip) / pip) * 100) + "%" : "";
                    tbl.Cells[row, col++] = flags.HP_continue != 0 ?
                        Math.Round(((flags.HP_continue - tm.PointSP2.Price + pip) / pip) / ((m._100Percent[1].y - tm.PointSP2.Price + pip) / pip) * 100)  + "%" : "";
                    tbl.Cells[row, col++] = flags.HP_recoil != 0 ?
                        Math.Round(((flags.HP_recoil - tm.PointSP2.Price - pip) / pip) / ((tm.Target1.Price - tm.PointSP2.Price - pip) / pip) * 100) + "%" : "";
                }   
                //tbl.Columns.AutoFit();
                tbl.Columns.ColumnWidth = 7;
                }
                catch (Exception ee)
                {
                    try{System.IO.File.AppendAllText(@"C:\sc_err.log","[inner] "+ee.ToString()+"\r\n\r\n");}catch{}
                    MessageBox.Show("StatCalc::describe: " + ee.Message);
                }


                worker.ReportProgress(5);
                //tbl.Columns.AutoFit();
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                try
                {
                    //сохранение экселевского файла (опционально в виде набора файлов цсв)
                    tbl.SaveAs(Application.StartupPath + @"\" + source_name + "_" + symbol_name + tf.ToString() + ".xls");
                    tbl = null;
                    xlApp.Interactive = true;
                    xlApp.ScreenUpdating = true;
                    xlApp.UserControl = true;
                    //запуск экселя
                    xlApp.Visible = true;
                    //освобождение памяти и выпускание экселя в свободное плавание
                    
                    xlApp = null;
                }
                catch(Exception se)
                {
                    try{System.IO.File.AppendAllText(@"C:\sc_err.log","[saveas] "+se.ToString()+"\r\n\r\n");}catch{}
                    MessageBox.Show("StatCalc::describe: can not write file " + Application.StartupPath + @"\" + source_name + "_" + symbol_name + tf.ToString() + ".xls");
                }
            }
            catch (Exception ex)
            {
                try{System.IO.File.AppendAllText(@"C:\sc_err.log","[outer] "+ex.ToString()+"\r\n\r\n");}catch{}
                MessageBox.Show("StatCalc::describe: "+ex.Message);
            }
        }
        void incrows(ref int row, ref int col, CsvSheet tbl)
        {
            row++; col = 1;
        }
        void smallfont(int row, int col, CsvSheet tbl)
        {
            CsvRange rng = tbl.get_Range(tbl.Cells[row, col], tbl.Cells[row, col]);
            rng.Font.Size = 8;
        }
        void smallfont(int row, int col, int x1, int y1, CsvSheet tbl)
        {
            CsvRange rng = tbl.get_Range(tbl.Cells[row, col], tbl.Cells[x1, y1]);
            rng.Font.Size = 8;
        }
        //combine(row, row + 1, col, col, tbl);
        void merge(int row, int x1, int col, int y1, CsvSheet tbl)
        {
            tbl.get_Range(tbl.Cells[row, col], tbl.Cells[x1, y1]).Merge(true);
        }

        /*
        Блок статистики.
        1.	Временной горизонт (даты начала и конца истории) //
        2.	План
        3.	Количество баров истории
        4.	Количество моделей на истории суммарно и с разделением на:
        А. моделей первого прохода:
        - мр
        - мп
        - мдр
        Б. моделей второго прохода:
        - мр
        - мп
        - мдр
        В. Моделей третьего прохода:
        - мр
        - мп
        - мдр
        5. процентное соотношение моделей из пункта 4 (по разделениям) к общему количеству моделей. Например, мр первого прохода 30% от общего количества моделей и 60% от общего количества моделей первого прохода и т.д.
        6. количество баров истории не входящих в состав моделей. Это значит учет участков в которых есть бары не входящие в состав ни одной модели этого же плана. Размер модели равен: мр - (ст-4)*4 (у нас в алгоритме 4 точно?), мдр - (1-4)*4 и мп – (1-НР)*2 (вроде 2, проверить по алгоритму) 
        7. % пункта 6 к общему количеству баров истории.
        --.	Модели где 4=6 в % к общему количеству мр
        --.	Модели с 6 точками в % к общему количеству мр

        Для одной мр без контекста:
        Далее везде точки 1 и 2 (отрезок 1-2) - два противоположных экстремума, значит имеется ввиду вертикальный отрезок.
        А например 1-3 – горизонтальный. Но 4-5 может быть двойного смысла в зависимости от того, с чем идет сравнение. Например если соотношение 1-2 и 4-5 – то вертикальный отрезок 4-5 имеется ввиду, а если 1-3 и 4-5 то горизонтальный.
        1.	Какого прохода модель
        2.	Ап или даун
        3.	Сила модели
        4.	Соотношение Базы к БЦ 1-3 к 4-5 и 1-2 к 4-5 в %
        5.	Соотношение Базы ко всей модели – 1-3 к 1-6 (не НР, а именно 6) и 1-2 к 1-6 в %
        6.	Соотношение БЦ ко всей модели – 1-3 к 1-6 (не НР, а именно 6) и 1-2 к 1-6 в %
        7.	Соотношение 2-5 к 1-6 в %
        8.	Соотношение 1-2 к 3-4 в %
        9.	Соотношение 3-4 к 1-6 в %
        10.	Соотношение ст-1 к 4-5 в %
        11.	Соотношение (наклона целевой к вертикали) к (наклону трендовой к горизонтали) 
        12.	Достижение НР до пробития целевой и трендовой на промежутке (ст-4)*4
         * 
        13.	Достижение 100% и 200% (по отдельности) после пробития ЛЦ на промежутке (ст-4)*4
        14.	Достижение 100% и 200% (по отдельности) после пробития (пробитие – повторный подход к уровню НР с предыдущим образованием противоположного 4 точке абс. экстремума (годного по алгоритму отбора экстремумов Алексея, который отрабатывает на старте алгоритма поиска моделей,  после первого достижения НР) НР на промежутке (ст-4)*4
        15.	Достижение 100% и 200% (по отдельности) без повторного (т.е. уровень НР достигнут с перелетом или пункт в пункт не важно и следующими барами цена идет дальше без образования противоположного абс. экстремума) пробития  НР на промежутке (ст-4)*4
        16.	Достижение 100% и 200% (по отдельности) после пробития ЛЦ и до пробития ЛТ на промежутке (ст-4)*4
        17.	Достижение 100% и 200% (по отдельности) после пробития НР и до пробития ЛТ на промежутке (ст-4)*4
         * 
        18.	Достижение целей (3-х по отдельности) без пробития целевой на промежутке (ст-4)*4
        19.	Достижение целей (3-х по отдельности) с пробитой целевой до пробития трендовой на промежутке (ст-4)*4
        20.	Достижение целей (3-х по отдельности) без пробития целевой, но с образованием 6, на промежутке (ст-4)*4
        21.	Достижение целей (3-х по отдельности) без пробития целевой, но без  образования 6 (т.е. 4=6), на промежутке (ст-4)*4
        22.	Соотношение (6 - 1 цель по горизонтали) к 1-6
        23.	Соотношение (6 - 2 цель по горизонтали) к 1-6
        24.	Соотношение (6 - 3 цель по горизонтали) к 1-6
        27.	Доход до НР  в % к 1-НР
        28.	Недоход до НР в % к 1-НР
        * 29. 1 цель - последнее касание трендовой / 6 - первое касание трендовой
          
        Для одной мдр без контекста:
        1.	Какого прохода модель
        2.	Ап или даун
        3.	Достижение 100% и 200% (по отдельности) после пробития ЛЦ на промежутке (1-4)*4
        4.	Достижение 100% и 200% (по отдельности) после пробития (пробитие – повторный подход к уровню НР с предыдущим образованием противоположного 4 точке абс. экстремума (годного по алгоритму отбора экстремумов Алексея, который отрабатывает на старте алгоритма поиска моделей,  после первого достижения НР) НР на промежутке (1-4)*4
        5.	Достижение 100% и 200% (по отдельности) без повторного (т.е. уровень НР достигнут с перелетом или пункт в пункт не важно и следующими барами цена идет дальше без образования противоположного абс. экстремума) пробития  НР на промежутке (1-4)*4
        6.	Достижение 100% и 200% (по отдельности) после пробития ЛЦ и до пробития ЛТ на промежутке (1-4)*4
        7.	Достижение 100% и 200% (по отдельности) после пробития НР и до пробития ЛТ на промежутке (1-4)*4
        8.	Достижение целей (3-х по отдельности) без пробития целевой на промежутке (1-4)*4
        9.	Достижение целей (3-х по отдельности) с пробитой целевой до пробития трендовой на промежутке (1-4)*4
        10.	Достижение целей (3-х по отдельности) без пробития целевой, но с образованием 6, на промежутке (1-4)*4
        11.	Достижение целей (3-х по отдельности) без пробития целевой, но без  образования 6 (т.е. 4=6), на промежутке (1-4)*4
        12.	Соотношение (6 - 1 цель по горизонтали) к 1-6
        13.	Соотношение (6 - 2 цель по горизонтали) к 1-6
        14.	Соотношение (6 - 3 цель по горизонтали) к 1-6

        Для одной мп без контекста:
        1.	Какого прохода модель
        2.	Ап или даун
        3.	Соотношение 1-2 к 3-4
        4.	Соотношение 1-3 к 2-4
        5.	Соотношение 1-4 к 4-НР
        6.	Достижение 100% и 200% до пробития 1 точки на участке (1-НР)*2 (сверить с алгоритмом!)
        7.	Достижение 100% и 200% после пробития 1 точки на участке (1-НР)*2 (сверить с алгоритмом!)
        8.	Доход до НР  в % к 1-НР
        9.	Недоход до НР в % к 1-НР
        10.	Движение от НР к 100% и 200% (отдельно) против движения к 1 цели в %

        Контекст моделей должен предварять статистику отдельно взятой модели.

        Для всех моделей в контексте все вышеперечисленное, плюс:

        1^.	Перечислить к каким проходам относятся три последние противоположные друг другу модели общего с моделью для которой собирается статистика Плана,
         * полностью описывающие свои тренды. Например: 1. Третий проход, 2. Первый проход, 3. Второй проход.
         * (приоритет у моделей 1-го прохода, затем второй итд, она же одна и рисуется)
        2^.	Перечислить типы трех моделей. Например, 1. мр, 2. мр, 3. мдр.
        3^.	Дать по ним полную статистику аналогично как для отдельной модели вне контекста.
        4^.	Общее количество баров для этих трех моделей.  <<< три модели без текущей для которой определяется контекст.
         * Расчет здесь от 1 точки первой модели до 6 точки третьей модели. 6 точка это не НР напоминаю на всякий случай.
        5^.	Сумма в пунктах всего движения описываемого тремя моделями. Внимание: не от начала первой и до конца третьей,
         * а для каждой модели отдельно и сложить.
        6^.	Соотношение пунктов 4 и 5
         * 
        7^.	Скорость для каждой из трех моделей по отдельности. 
         * Считается:  количество пунктов от 1 до 6 делится на количество баров составляющих отрезок 1-6.
        8^.	Модель по тренду или от начала тренда для своего Плана или коррекционная.
         * Поясню: если 1 точка этой модели находится между целевой и трендовой другой модели
         * (ее целевая и трендовая длятся на протяжении (ст-4)*4 от своей ст) этого же Плана,
         * и эта модель сонаправлена большей, то модель считается по тренду.
         * 
        9^.	Если модель по тренду то: ее 1 точка равна 3 точки (по времени и значению) модели по тренду которой она расположена или нет.
        10^.	Соотношение наклона трендовой к горизонтали модели к каждой из трех по отдельности трендовых моделей контекста.
        11^.	Соотношение наклона целевой к горизонтали модели к каждой из трех по отдельности целевой моделей контекста.
        12^.	Соотношение скорости модели к каждой из трех скоростей моделей контекста по отдельности. 
         * Скорость модели прогнозная и считается – 1-НР по вертикали разделить на 1-НР по горизонтали. Т.е. пункты на бары.
         
        13.	Для каждой модели контекста указать: достигнута НР, 100%, 200%, 1, 2, 3 цель. Перечислить все что в рамках (ст-4)*4 достигнуто.

        Многоплановый контекст:
        1.	Указать в какую модель какого Плана входит анализируемая. 
        2.	Дать полную статистику модели старшего Плана аналогично вышеперечисленному.
        3.	Указать ее местоположение в рамках модели старшего Плана: от 1 точки, от 3 точки, по тренду, коррекционная.
        ======================
         *  выводить давайте в экселевский файл так, чтобы на каждом листе был ярлык с названием и
         *  1 лист - общая статистика,
         *  2. по мр и т.д.
[20.08.2010 23:29:44] ...: На первом листе еще нужно добавить пункты:
         * 1. моделей от начала тренда,
         * 2. коррекционных,
         * 3. моделей по тренду.
         * =======================
         *^ + время достижения 1 цели от последнего касания трендовой / к времени т6-пробой трендовой
         * -----------------------
[06.08.2010 18:31:10] loylick: L1. где расположена СТ по отношению к предыдущей модели данного плана и плaна старше.
L2. время достижения СТh при ходе от 6-й в единицах СТ- 4.
L3. был ли откат от 6-й до 4-й
L4. был ли пробит лоу/хай бара 4.
L5. была ли пробита 5 точка.
L6. указать время пробития трендовой в единицах Ст-4.
L7. указать время пробития 6-й (если он был) в единицах СТ-4.
L8. площади треугольников 123,234,345,456
L9. указать время 6-й в единицах Ст-4 (wait) //значить. где 6-я мы знаем. берешь расстояние 4-6 и делишь его на расстояние СТ-4

         * ...:  1. по отношению к трем ст последних трех моделей правильнее. Остальное все принимается.
         * 
         * [06.08.2010 22:32:25] loylick: 1. по отношению к трем ст последних трех моделей правильнее. -- моделей какого плана?
[06.08.2010 22:34:28] loylick: 1. где расположена СТ по отношению к предыдущей модели данного плана и пална старше.
         * -- тут понятно как позиционировацца. к примеру "в 2 барах паравей 4 точки". или на 20% между 4 и 5.
         * а как позиционировацца к трем предыдущим СТ?
[06.08.2010 22:35:39] ...: 1. по отношению к трем ст последних трех моделей правильнее. -- моделей какого плана?
         * общего с моделью для которой собирается статистика.
[06.08.2010 22:36:31] ...: по вертикали к последней и по горизонтали к первым двум.
        */

        /// <summary>
        /// копируем List<TModel> в List<Model>
        /// </summary>
        /// <param name="tmodels">исходный List<TModel></param>
        /// <returns>List<Model></returns>
        public List<Model> ModelsInit(List<TModel> tmodels)
        {
            List<Model> models = new List<Model>();
            int lostmodelcount = 0;
            foreach (TModel m in tmodels)
                if (m.CurrentPoint >= 4)
                    models.Add(new Model(m, 0));
                else
                    lostmodelcount++;
            return models;
        }

        void GetContextModelIndexes(int id, TQuotes quotes, ref int[] bfr)
        {
            List<TModel> tmodels = quotes.MM.Models;
            bfr[2] = quotes.Symbol.MTree.prevGlobalModel(id, quotes.tf);    //предыдущая
            if (bfr[2] >= 0 && tmodels[bfr[2]].ModelDir == tmodels[id].ModelDir)
            {
                bfr[1] = bfr[2];
                bfr[2] = -1;
            }
            else
                bfr[1] = quotes.Symbol.MTree.prevGlobalModel(bfr[2], quotes.tf);//предпредыдущая
            if (bfr[1] >= 0 && bfr[2] >= 0 && tmodels[bfr[1]].ModelDir == tmodels[bfr[2]].ModelDir)
            {
                bfr[0] = bfr[1];
                bfr[1] = -1;
            }
            else
                bfr[0] = quotes.Symbol.MTree.prevGlobalModel(bfr[1], quotes.tf);//первая из трех
            if (bfr[0] >= 0 && bfr[1] >= 0 && tmodels[bfr[0]].ModelDir == tmodels[bfr[1]].ModelDir)
            {
                bfr[0] = -1;
            }
        }


        string GetupTargets(TModel tm, Model m)
        {
          //  string output;
          //  flags.HP_get
            return "";
        }

        /// <summary>
        /// строковое представление типа модели
        /// </summary>
        /// <param name="tmodels"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        string ModelType2str(List<TModel> tmodels, int id)
        {
            if (id >= 0)
                switch (tmodels[id].ModelType)
                {
                    case Common.TModelType.ModelOfExpansion:  return "MExp";
                    case Common.TModelType.ModelOfBalance:    return "MDB";
                    case Common.TModelType.ModelOfAttraction: return "MAttr";
                }
            return "";
        }
        int ModelType2int(List<TModel> tmodels, int id)
        {
            if (id >= 0)
                switch (tmodels[id].ModelType)
                {
                    case Common.TModelType.ModelOfExpansion: return ext;
                    case Common.TModelType.ModelOfBalance: return bal;
                    case Common.TModelType.ModelOfAttraction: return atr;
                }
            return 0;
        }

        string ModelStatusContext(TModel m)
        {
            if ((m.Stat & (uint)MStat.GlobalP1) != 0) return "Base";
            if (m.isBytrendOfID>=0) return "ByTrend";
            if (m.isCorrectionOfID >= 0) return "Corr.";
            return "";
        }

        string ModelStatus_1eq3(TModel m)
        {
            if ((m.Stat & (uint)MStat.GlobalP1) == 0 && m.isBytrendOfID >= 0)
                return ((points[m.Point1.iBar] & 0x000F00) != 0) ? "1=3" : "1<>3";
            return "";
        }

        void setval(CsvSheet tbl, int row, int col, double val)
        {
            if (val != 0) tbl.Cells[row, col] = val;
        }

        void setval(CsvSheet tbl, int row, int col, bool condition_is_true, double val)
        {
            if (condition_is_true) tbl.Cells[row, col] = val;
        }

        void setval(CsvSheet tbl, int row, int col, bool condition_is_true, string val)
        {
            if (condition_is_true) tbl.Cells[row, col] = val;
        }
        
        /// <summary>
        /// подсчет количества моделей по проходам и типам моделей
        /// </summary>
        /// <param name="models"></param>
        void CountModelsByType(List<TModel> models, TF TFBase)
        {
            modelrefs = new int[models.Count];
            int extcnt=0, balcnt=0, atrcnt=0, i=0;
            foreach (TModel m in models)
            {
                if (m.TimeFrame != TFBase) { i++; continue; }

                //по типам
                switch (m.ModelType)
                {
                    case Common.TModelType.ModelOfAttraction:
                        switch (m.Step)
                        {
                            case 1: Atr1cnt++; break;
                            case 2: Atr2cnt++; break;
                            case 4: Atr3cnt++; break;
                        }
                        modelrefs[i++] = atrcnt++;
                        break;
                    case Common.TModelType.ModelOfBalance:
                        switch (m.Step)
                        {
                            case 1: Bal1cnt++; break;
                            case 2: Bal2cnt++; break;
                            case 4: Bal3cnt++; break;
                        }
                        modelrefs[i++] = balcnt++;
                        break;
                    case Common.TModelType.ModelOfExpansion:
                        switch (m.Step)
                        {
                            case 1: Ext1cnt++; break;
                            case 2: Ext2cnt++; break;
                            case 4: Ext3cnt++; break;
                        }
                        modelrefs[i++] = extcnt++;
                        break;
                }
                //по количеству точек
                if (Common.TModelType.ModelOfAttraction != m.ModelType)
                    if (m.Point4.pt.X == m.Point6.pt.X)
                        switch (m.Step)
                        {
                            case 1: m4step1++; break;
                            case 2: m4step2++; break;
                            case 4: m4step3++; break;
                        }
                    else
                        switch (m.Step)
                        {
                            case 1: m6step1++; break;
                            case 2: m6step2++; break;
                            case 4: m6step3++; break;
                        }
                if (Common.TModelType.ModelOfAttraction == m.ModelType)
                    if (m.Point4.pt.X == m.Point6.pt.X)
                        switch (m.Step)
                        {
                            case 1: Ext1cnt4++; break;
                            case 2: Ext2cnt4++; break;
                            case 4: Ext3cnt4++; break;
                        }
                    else
                        switch (m.Step)
                        {
                            case 1: Ext1cnt6++; break;
                            case 2: Ext2cnt6++; break;
                            case 4: Ext3cnt6++; break;
                        }
                else if (Common.TModelType.ModelOfBalance == m.ModelType)
                    if (m.Point4.pt.X == m.Point6.pt.X)
                        switch (m.Step)
                        {
                            case 1: Bal1cnt4++; break;
                            case 2: Bal2cnt4++; break;
                            case 4: Bal3cnt4++; break;
                        }
                    else
                        switch (m.Step)
                        {
                            case 1: Bal1cnt6++; break;
                            case 2: Bal2cnt6++; break;
                            case 4: Bal3cnt6++; break;
                        }
            }
        }

        /// <summary>
        /// Общее количество баров для трех моделей контекста
        /// </summary>
        /// <param name="bfr"></param>
        /// <param name="tmodels"></param>
        /// <returns></returns>
        int barcount(int[] bfr, List<TModel> tmodels, int n, bool byReal6)
        {
            if (n >= 0 && n < 3)
            {
                return bfr[n] != -1 ? ((byReal6 ? tmodels[bfr[n]].Point6.iBar : tmodels[bfr[n]].Point61.iBar) - tmodels[bfr[n]].Point1.iBar + 1) : 1;
            }
            int istart = bfr[0] != -1 ? bfr[0] : bfr[1] != -1 ? bfr[1] : bfr[2] != -1 ? bfr[2] : -1;
            if (istart >= 0)
            {
                int iend = bfr[2] != -1 ? bfr[2] : bfr[1] != -1 ? bfr[1] : bfr[0] != -1 ? bfr[0] : -1;
                int p6bar = byReal6 ? tmodels[iend].Point6.iBar : tmodels[iend].Point61.iBar;
                return (p6bar - tmodels[istart].Point1.iBar + 1);
            }
            return 0;
        }

        /// <summary>
        /// Сумма в пунктах всего движения описываемого тремя моделями
        /// </summary>
        /// <param name="bfr"></param>
        /// <param name="tmodels"></param>
        /// <returns></returns>
        int pipscount(int[] bfr, List<TModel> tmodels, int n, bool byReal6)
        {
            double cnt = 0;
            int decs = 0;
            int from = (n >= 0 && n < 3) ? n : 0;
            int to = (n >= 0 && n < 3) ? (n+1) : 3;
            for (int i = from; i < to; i++)
            {
                if (bfr[i] != -1)
                {
                    double pip = tmodels[bfr[i]].ModelDir == TDirection.dtUp ? quotes.Symbol.pip : -quotes.Symbol.pip;
                    if(tmodels[bfr[i]].Quotes.log)
                        cnt += Math.Abs(Math.Pow(10, byReal6 ? tmodels[bfr[i]].Point6.Price : tmodels[bfr[i]].Point61.Price) - Math.Pow(10, tmodels[bfr[i]].Point1.Price) + pip);
                    else
                        cnt += Math.Abs((byReal6 ? tmodels[bfr[i]].Point6.Price : tmodels[bfr[i]].Point61.Price) - tmodels[bfr[i]].Point1.Price + pip);
                    decs = tmodels[bfr[i]].Quotes.Symbol.Decimals;
                }
            }
            cnt *= Math.Pow(10, decs);
            return (int)cnt;
        }

        double modelSpeed(int[] bfr, List<TModel> tmodels, int n, bool byReal6)
        {
            //double speed = 
            return Math.Round((double)pipscount(bfr, tmodels, n, byReal6) / barcount(bfr, tmodels, n, byReal6), 1);
            //return speed == 0 ? "" : speed.ToString();
        }

        double modelSpeed(TModel m, bool byReal6)
        {
            double pipscount, barcount;
            double pip = m.ModelDir == TDirection.dtUp ? quotes.Symbol.pip : -quotes.Symbol.pip;
            if(m.Quotes.log)
                pipscount = Math.Abs(Math.Pow(10, byReal6 ? m.Point6.Price : m.Point61.Price) - Math.Pow(10, m.Point1.Price) + pip);
            else
                pipscount = Math.Abs((byReal6 ? m.Point6.Price : m.Point61.Price) - m.Point1.Price + pip);

            pipscount *= Math.Pow(10, m.Quotes.Symbol.Decimals);

            barcount = (byReal6 ? m.Point6.iBar : m.Point61.iBar) - m.Point1.iBar + 1;
                        
            return pipscount / barcount;

        //    double pipscount, barcount;
        //    double p6price = (byReal6 || m.Point61.Price == 0) ? m.Point6.Price : m.Point61.Price;
        //    if (m.Quotes.log)
        //        pipscount = Math.Abs(Math.Pow(10, p6price) - Math.Pow(10, m.Point1.Price));
        //    else
        //        pipscount = Math.Abs(p6price - m.Point1.Price);

        //    pipscount *= Math.Pow(10, m.Quotes.Decimals);

        //    barcount = ((byReal6 || m.Point61.Price == 0) ? m.Point6.iBar : m.Point61.iBar) - m.Point1.iBar;

        //    return pipscount / barcount;
        }

        double ModelSpeedRatio(List<TModel> tmodels, int m1i, int m2i, bool byReal6)
        {
            if (m2i == -1 || !byReal6 && (tmodels[m1i].Point61.Price == 0 || tmodels[m2i].Point61.Price == 0)) return 0;

            return Math.Round(modelSpeed(tmodels[m1i], false) / modelSpeed(tmodels[m2i], false), 2);
        }

        //Подсчет моделей от начала тренда, по тренду, коррекционных
        uint[] points;
        //void CountPointsByStatus(List<TModel> tmodels, int barcount)
        //{
        //    points = new uint[barcount];
        //    //каждый байт - флаг принадлежности к соответствующей точке - порядковый номер байта = номеру точки
        //    //при этом 1 = точка пересекается с точкой N(байта) модели
        //    //2(второй бит) = точка N принадлежит модели по тренду
        //    //4(третий бит) = точка N принадлежит коррекционной модели
        //    foreach (TModel m in tmodels)
        //    {
        //        points[(int)m.Point1.Bar] |= 0x000001;
        //        points[(int)m.Point2.Bar] |= 0x000010;
        //        points[(int)m.Point3.Bar] |= 0x000100;
        //        if (m.Point4.Bar != m.Point6.Bar)
        //            points[(int)m.Point4.Bar] |= 0x001000;
        //        //else учитывается 6 точка (6=4)
        //        points[(int)m.Point5.Bar] |= 0x010000;
        //        points[(int)m.Point6.Bar] |= 0x100000;
        //        //проверка флагов родительских модели(с которыми пересекается данная т1)
        //        if ((points[(int)m.Point1.Bar] & 0x001010) != 0)
        //        {
        //            points[(int)m.Point3.Bar] |= 0x000400; //родительская модель - коррекционная
        //            points[(int)m.Point5.Bar] |= 0x040000; //родительская модель - коррекционная
        //        }
        //    }
        //}

        //Подсчет моделей от начала тренда, по тренду, коррекционных
        int modelBaseCnt,
            modelByTrendCnt,
            modelCorrectionCnt;

        //    //каждый байт - флаг принадлежности к соответствующей точке - порядковый номер байта = номеру точки
        //    //при этом 1 = точка пересекается с точкой N(байта) модели
        //    //2(второй бит) = точка N принадлежит модели по тренду
        //    //4(третий бит) = точка N принадлежит коррекционной модели
        void CountModelsByStatus(uint[] points)
        {
            foreach (uint point in points) //каждый бит == флаг соответствующей точки, от 1 до 6
            {
                if ((point ^ 0x01) == 0) modelBaseCnt++;               //бар является только первой точкой, будем считать его началом тренда
                else if ((point & 0x01) != 0)
                {
                    if ((point & 0x100000) != 0) modelBaseCnt++;       //бар включает точки 1 и 6 --> начало тренда
                    if ((point & 0x041410) != 0) modelCorrectionCnt++; //бар включает точки 1 и 2|4 (3|5 коррекционной)--> коррекционная модель
                    if ((point & 0x000100) != 0) modelByTrendCnt++;    //бар принадлежит точкам 1 и 3 --> модель по тренду
                }
            }
        }

        //возвращает строку - список точек с которыми пересекается т1 данной модели
        string ModelStatus(TModel m)
        {
            uint point = points[(int)m.Point1.pt.X];
            if ((point ^ 0x01) == 0) return "1*"; //бар является только первой точкой, будем считать его началом тренда
            string st = "";
            if ((point & 0x100000) != 0) st += ",T1"; //бар включает точки 1 и 6 --> начало тренда
            if ((point & 0x000100) != 0) st += ",T3"; //бар принадлежит точкам 1 и 3 --> модель по тренду
            if ((point & 0x000400) != 0) st += "Corr"; //бар принадлежит точкам 1 и 3, но родительская модель коррекционная --> модель по тренду коррекционной модели
            if ((point & 0x010000) != 0) st += ",t5"; //бар принадлежит точкам 1 и 5 --> модель по тренду
            if ((point & 0x040000) != 0) st += "Corr"; //бар принадлежит точкам 1 и 3, но родительская модель коррекционная --> модель по тренду коррекционной модели
            if ((point & 0x001010) != 0) st += ",Corr"; //бар включает точки 1 и 2|4 --> коррекционная модель
            if (st.Length == 0) return "";
            return st.Substring(1);
        }

        //12-24
        //данные параметры можно считать для каждой модели по мере заполнения таблицы

        //Подсчет:
        //25.	Модели где 4=6 в % к общему количеству мр
        //26.	Модели с 6 точками в % к общему количеству мр
        public int MReq46percent,
                   MRpts6percent;
                   
        void CountPercentOf_4eq6(List<TModel> mlist)
        {
            int mr_cnt=0, eq46cnt=0, pts6cnt=0;
            foreach (TModel m in mlist)
            {
                if (m.TimeFrame != TFBase) continue;
                if (m.ModelType == Common.TModelType.ModelOfExpansion)
                {
                    mr_cnt++;
                    if (m.Point4.Bar == m.Point6.Bar)
                        eq46cnt++;
                    else 
                        pts6cnt++;
                }
            }
            MReq46percent = eq46cnt * 100 / mr_cnt;
            MRpts6percent = pts6cnt * 100 / mr_cnt;
        }


        //количество баров истории не входящих в состав моделей
        int CountVoidBars(List<TModel> models)
        {
            Range r = new Range(0, quotes.lastindex);
            List<Range> ranges = new List<Range>();
            ranges.Add(r);
            int i=0;
            foreach (TModel m in models)
            {
                if (m.TimeFrame != TFBase) continue;

                if (i < 0) i = 0;
                if (i >= ranges.Count) i = ranges.Count - 1;
                
                //get poits a, b for this model
                //int a = Convert.ToInt32(tm.Point1.pt.X);
                int a = m.Point1.iBar;
                int b = m.LifeTime;

                //clear ranges
                while (i > 0 && (a > ranges[i].b || a < ranges[i].a)) i--;
                clearRange(ranges, i, a, b);
                while (i < (ranges.Count - 1) && (b < ranges[i].a || b > ranges[i].b)) i++;
                clearRange(ranges, i, a, b);
            }
            i = 0;
            foreach (Range range in ranges) i += (range.b - range.a);
           
            return i;
        }
        //вспомогательная функция для подсчета баров истории не входящих в состав моделей
        void clearRange(List<Range> ranges, int i, int a, int b)
        {
            Range range = ranges[i];
            if (a > range.a && b < range.b) //split range
            {
                ranges.Insert(i + 1, new Range(b, range.b));
                range.b = a;
            }
            else if (a > range.a && a < range.b) //cut tail
                range.b = a;
            else if (b > range.a && b < range.b) //cut head
                range.a = b;
            else if (a <= range.a && b >= range.b) //remove range
                ranges.RemoveAt(i);
        }
    }

    /// <summary>
    /// вспомогательный класс для функции подсчета баров истории не входящих в состав моделей
    /// </summary>
    class Range
    {
        int[] r = { 0, 0 };
        public Range(int a, int b)
        {
            this.a = a;
            this.b = b;
        }
        public int a
        {
            get
            {
                return r[0];
            }
            set
            {
                r[0] = value;
            }
        }
        public int b
        {
            get
            {
                return r[1];
            }
            set
            {
                r[1] = value;
            }
        }
    }
}
                                