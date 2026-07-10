//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Andrey Prokhorov (AVP), Andrey Zyablitsev (skat)
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
using Skilful.Data;
using ChartV2;
using System.IO;

using System.Reflection;
using System.Threading;
using System.ComponentModel;


namespace Skilful.QuotesManager
{
    public enum Candle { T, O, H, L, C };
    [System.Serializable]
    public enum TF : int
    {
        custom,
        m60, m240, Day, Week, Month, Quarter, Year, //набор базовых фреймов
        count,                                      //указатель ёмкости для массивов
        AllSeparate, AllSingle,                     //наряду с базовыми флаги выбора чарта для одного симовола или набора в одном или в семи разных окнах
        custom_                                     //все неперечисленные тут периоды рассматриваем как определенные пользователем
    };
    public enum PriceScaleType { log10, linear, mixed }; //mixed -- линейный 60 и 240, логарифм от дней и выше
    
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
    public class TSymbol
    {
        public PriceScaleType priceScale;
        ChartType DefaultChartType = ChartType.Candle;
        DataSourseModule Module;
        public string moduleName
        {
            get { return Module.name; }
        }
        //public string SourceName;
        public static float[] TFtoHours = {0.017f, 1, 4, 24, 168, 730.5f, 2191.5f, 8766 };
        public int customTF;
        public string Name;            //!< Название символа
        public int Decimals = 0;       //!< Кол-во знаков после запятой
        public double pip = NotKnownDigitsValue;    //!< Единица именения цены
        public TQuotes[,] Frames;      //!< массивы котировок TQuotes[ChartType, TimeFrame]
        public ModelManager.ModelTree MTree;  //!< древовидная структура моделей
        //локальные константы
        const int H1 = (int)TF.m60, H4 = (int)TF.m240, D = (int)TF.Day, W = (int)TF.Week, M = (int)TF.Month, Q = (int)TF.Quarter, Y = (int)TF.Year, CF =(int)TF.custom;
        const int T = (int)Candle.T, O = (int)Candle.O, H = (int)Candle.H, L = (int)Candle.L, C = (int)Candle.C;
        const int candles = (int)ChartType.Candle, linear = (int)ChartType.Line, X0 = (int)ChartType.X0;
        const int NotKnownDigitsValue = 1000000;
        string storagePath = null, storeFileH1, storeFileD1;
        public static event RefreshHistoryHandler RefreshHistory;
        bool timeisdouble;
        double onehour;
        DateTime nuldate;
        bool EnableHistoryCache;

        //BackgroundWorker bgimporter;
        double[] quotes_H1=null, quotes_D1=null;

        /// <summary>
        /// получаем enum TF для фрейма заданного в минутах
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static TF getTF(int minutes)
        {
            if (minutes < 60) return TF.custom;
            switch (minutes)
            {
                case 60:
                    return TF.m60;
                case 240:
                    return TF.m240;
                case 1440:
                    return TF.Day;
                case 10080:
                    return TF.Week;
                default:
                    if (minutes >= 28 * 1440)
                    {
                        if (minutes <= 31 * 1440) return TF.Month;
                        if (Math.Abs(minutes - 131490) < 3 * 1440) return TF.Quarter;
                        if (Math.Abs(minutes - 525960) < 2880) return TF.Year;
                    }
                    return TF.custom;
            }
        }


        /// <summary>
        /// конструктор ;)
        /// </summary>
        /// <param name="module">входной модуль</param>
        /// <param name="symbol">имя символа</param>
        /// <param name="QMRefreshHistory">типа коллбек функция для обновления истории при получении новых тиков или баров</param>
        public TSymbol(DataSourseModule module, string symbol, RefreshHistoryHandler QMRefreshHistory, bool EnableHistoryCache)
        {
            RefreshHistory = QMRefreshHistory;
            //на самом деле эти параметры нужно будет получить из пользовательскиъ настроек
            //или обрабатывать события контексного меню
            //scale prices: as default all graphics will be in log10
            //priceScale = GlobalMembersTrend.log ? PriceScaleType.log10 : PriceScaleType.linear;
            priceScale = PriceScaleType.mixed;
            ///////////////////
            Module = module;
            
            //SourceName = module.name;
            Name = symbol;//.ToUpper();

            this.EnableHistoryCache = EnableHistoryCache;
            StoragePathInit();
            Frames = new TQuotes[(int)ChartType.count, (int)TF.count];
            if (Module.name == "CSV")
            {
                timeisdouble = true;
                onehour = new TimeSpan(TimeSpan.TicksPerHour).TotalDays;
                nuldate = new DateTime(0001, 01, 01);
            }
            else
            {
                timeisdouble = false;
                onehour = 3600;
                nuldate = new DateTime(1970, 01, 01);
            }
            fillFrames1(DefaultChartType);


            MTree = new ModelManager.ModelTree((int)TF.count - 1, (int)TF.count, this);
        }

        public void Clear()
        {
            //Module = null;
            MTree = null;
            for (int i=0; i<(int)ChartType.count; i++)
                for(int j=0; j<(int)TF.count; j++)
                {
                    if(Frames[i, j] !=null)
                    {
                        Frames[i, j].MM = null;
                        Frames[i, j].trend = null;
                        Frames[i, j].Quotes.Clear();
                        Frames[i, j].Quotes = null;
                        Frames[i, j] = null;
                    }
                }
        }

        //путь к локальной базе
        bool StoragePathInit()
        {
            if(EnableHistoryCache)
                storagePath = Module.StoragePath;
            if (storagePath != null && storagePath.Length > 0)
            {
                
                storagePath = Application.StartupPath + "\\storage\\" + Module.StoragePath;
                if (!Directory.Exists(storagePath)) Directory.CreateDirectory(storagePath);
                storeFileH1 = storagePath + '\\' + Name + '.' + (TFtoHours[H1] * 360000) + ".qts";
                storeFileD1 = storagePath + '\\' + Name + '.' + (TFtoHours[D] * 360000) + ".qts";
                return true;
            }
            return false;
        }

        //количество разрядов в дробноой части
        public void setPipValue(int digits, double[] quotes)
        {
            pip = 0;
            if (digits == NotKnownDigitsValue)
            {
                pip = Module.get_pip_value(Name);
                if (pip == 0)
                {
                    if (Name.IndexOf("Rand") >= 0)
                        Decimals = Name.Substring(Name.IndexOf('.')).Length - 1;
                    //заглушка по умолчанию на случай отсутствия метода get_pip_value() в данном модуле
                    else if (quotes != null && quotes.Length > 4)
                        Decimals = quotes[4] < 10 ? 4 : 2;
                }
                else
                {
                    if (pip < 1) Decimals = -Convert.ToInt32(Math.Log10(pip));
                }
            }
            else
                Decimals = digits;

            if (pip == 0) pip = Math.Pow(10, -Decimals);
        }

        /// <summary>
        /// возвращаем массив котировок для заданых фрейма и типа чарта
        /// при первом вызове данные импортируются из входного модуля
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="ctype"></param>
        /// <returns></returns>
        public TQuotes GetQuotes(TF tf, ChartType ctype)
        {
            if (Frames[(int)ctype, (int)tf] == null) fillFrames1(ctype);
            return Frames[(int)ctype, (int)tf];
        }

        //------------------------------------//
        //<< блок функций для локального хранения данных 
        //------------------------------------//
        //------------------------------------//
        //------------------------------------//
        //--- read and write historical data into local database ---//
        //упаковка данных для записи в файл
        double[] qts_unpack(uint[,] package)
        {
            int len = package.GetLength(0);
            double[] quotes = new double[len * 5];
            for (int i = 0, o = 0; i < len; i++, o+=5)
            {
                long dt=0;
                for (int j = 0; j < 4; j++)
                {
                    quotes[o + j + 1] = (package[i, j] & 0x00ffffff) * pip;
                    dt |= (package[i, j] & 0xff000000) >> (j * 8);
                }
                quotes[o] = (double)dt;
            }
            return quotes;
        }
        //распаковка данных прочитанных из локальой базы(файла)
        TQuotes qts_unpack(uint[,] package, TF tf, ChartType ctype)
        {
            int len = package.GetLength(0);
            TQuotes quotes = new TQuotes(this, tf, ctype, len);

            for (int i = 0; i < len; i++)
            {
                TBar bar = new TBar();
                switch (ctype)
                {
                    case ChartType.Candle:
                        if (priceScale == PriceScaleType.log10 || priceScale == PriceScaleType.mixed && tf >= TF.Day)
                        {
                            bar.Open = Math.Log10((double)(package[i, 0] & 0x00ffffff) * pip);
                            bar.High = Math.Log10((double)(package[i, 1] & 0x00ffffff) * pip);
                            bar.Low = Math.Log10((double)(package[i, 2] & 0x00ffffff) * pip);
                            bar.Close = Math.Log10((double)(package[i, 3] & 0x00ffffff) * pip);
                        }
                        else// if (priceScale == PriceScaleType.linear)
                        {
                            bar.Open = (double)(package[i, 0] & 0x00ffffff) * pip;
                            bar.High = (double)(package[i, 1] & 0x00ffffff) * pip;
                            bar.Low = (double)(package[i, 2] & 0x00ffffff) * pip;
                            bar.Close = (double)(package[i, 3] & 0x00ffffff) * pip;
                        }
                        break;
                    case ChartType.Line:
                        if (priceScale == PriceScaleType.log10 || priceScale == PriceScaleType.mixed && tf >= TF.Day)
                        {
                            bar.Open = bar.High = bar.Low = bar.Close = Math.Log10((double)(package[i, C-O] & 0x00ffffff) * pip);
                        }
                        else// if (priceScale == PriceScaleType.linear)
                        {
                            bar.Open = bar.High = bar.Low = bar.Close = (double)(package[i, C-O] & 0x00ffffff) * pip;
                        }
                        break;
                    case ChartType.X0:    //TODO...//
                        break;
                }
                long dt=0;
                for (int j = 0; j < 4; j++) dt |= package[i, j] & (0xff000000 >> (j * 8));
                bar.DT = new DateTime(1970, 01, 01) + TimeSpan.FromSeconds(dt);
                quotes.Add(bar);
            }
            return quotes;
        }
        uint[,] qts_pack(double[] quotes)
        {
            int len = quotes.Length/5;
            uint[,] package = new uint [len,4];

            for (int i = 0, o = 0; o < len; i += 5, o++)
            {
                for(int j=0; j<4; j++)
                {
                    package[o, j] = (uint)(quotes[i + j + 1] / pip) | (((uint)quotes[i]<<(j*8)) & 0xff000000);
                }
            }
            return package;
        }
        //существует ли локальный файл данных
        bool ExistStoredData(string fname)
        {
            return storagePath != null && File.Exists(fname);
        }

        /// <summary>
        /// импорт данных из входного модуля
        /// или чтение из локального файла, если такой существует, с дозаполнением данных из входного модуля
        /// </summary>
        /// <param name="quotes_h1"></param>
        /// <param name="quotes_d1"></param>
        void get_data(ref double[] quotes_h1, ref double[] quotes_d1)
        {
            if (StoragePathInit())
            {
                if (ExistStoredData(storeFileH1))
                {
                    uint start_time, end_time;
                    //readLocal
                    quotes_h1 = qts_unpack(qts_read(storeFileH1, out start_time, out end_time));
                    //importTail
                    Module.import(Name, TF.m60, (int)end_time, 0);
                    //quotes_h1.Concat(Module.import(Name, TF.m60, (int)end_time, 0));
                    //storeTail
                   /// qts_write(storeFileH1, qts_pack(quotes_h1));
                    if (ExistStoredData(storeFileD1))
                    {
                        //readLocal
                        quotes_d1 = qts_unpack(qts_read(storeFileD1, out start_time, out end_time));
                        //importTail
                        Module.import(Name, TF.Day, (int)end_time, 0);
                        //quotes_d1.Concat(Module.import(Name, TF.Day, (int)end_time, 0));
                        //storeTail
                       /// qts_write(storeFileD1, qts_pack(quotes_d1));
                    }
                }
                else
                {
                    quotes_h1 = Module.import(Name, TF.m60);                //1. попытка получить часовую историю
                    //if (quotes_h1 == null || quotes_h1.Length < 5) quotes_h1 = Module.import(Name); //2. если нет часовой - попытка получить то что дают
                    //else 
                    if (quotes_h1 != null)  quotes_d1 = Module.import(Name, TF.Day);           //4. запрос дневной истории если есть часовая

                    //количество разрядов в дробноой части
                    setPipValue(NotKnownDigitsValue, quotes_h1);
                    //store
                    if (quotes_h1 != null && quotes_h1.Length >= 5 && quotes_h1[0] > 0) qts_write(storeFileH1, qts_pack(quotes_h1));
                    if (quotes_d1 != null && quotes_d1.Length >= 5 && quotes_d1[0] > 0) qts_write(storeFileD1, qts_pack(quotes_d1));
                }
            }
            else
            {
                quotes_h1 = Module.import(Name, TF.m60);                //1. попытка получить часовую историю
                if (quotes_h1 == null || quotes_h1.Length < 5) quotes_h1 = Module.import(Name); //2. если нет часовой - попытка получить то что дают
                else quotes_d1 = Module.import(Name, TF.Day);           //4. запрос дневной истории если есть часовая
            }
        }

        // чтение данных из файла
        uint[,] qts_read(string fname)
        {
            uint start_time=0, end_time=0;
            return qts_read(fname, out start_time, out end_time);
        }
        //запись данных в файл
        int qts_write(string fname, uint[,] package)
        {
            TF tf = TF.custom;
            uint start_time=0, end_time=0;
            for (int j = 0; j < 4; j++)
            {
                start_time |= (package[0, j] & 0xff000000) >> (j * 8);
                end_time |= (package[package.GetLength(0) - 1, j] & 0xff000000) >> (j * 8);
            }
            return qts_write(fname, tf, package, start_time, end_time);
        }
        uint[,] qts_read(string fname, out uint start_time, out uint end_time)
        {
            uint[,] pckg=null;
            start_time = end_time = 0;
            if (File.Exists(fname))
            {
                // Create the reader for data.
                FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(fs);
                // Read data from Test.data.
                TF tf = (TF) r.ReadInt32();   //period as enum TF (хотя фрейм присутствует в имени файла)
                start_time = r.ReadUInt32();  //time of first bar in file
                end_time = r.ReadUInt32();    //time of last bar in file
                int len = r.ReadInt32();      //number of bars
                r.ReadBytes(16);              //reserved
                //read array
                pckg = new uint[len, 4];
                for (int i = 0; i < len; i++)
                    for (int j = 0; j < 4; j++)
                        pckg[i, j] = r.ReadUInt32();
                r.Close();
                fs.Close();
                //r.ReadBytes(len * 4);
            }
            return pckg;
        }
        int qts_write(string fname, TF tf, uint[,] package, uint start_time, uint end_time)
        {
            FileStream fs = new FileStream(fname, FileMode.OpenOrCreate);
            // Create the writer for data.
            BinaryWriter w = new BinaryWriter(fs);
            // Write data to Test.data.
            int i = 0, j = 0;
            try
            {
                w.Write((int)tf);              //period as enum TF (хотя фрейм присутствует в имени файла)
                w.Write(start_time);           //time of first bar in file
                w.Write(end_time);             //time of last bar in file
                w.Write(package.GetLength(0)); //number of bars
                w.Write(new byte[16]);         //reserved
                //write data array
                for (i=0; i < package.GetLength(0); i++)
                    for (j=0; j < package.GetLength(1); j++)
                        w.Write(package[i, j]);
                w.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                w.Close();
                fs.Dispose();
            }
            return i * package.GetLength(1) + j;
        }
        
        //импорт данных в отдельном потоке
        //void bgimporter_start(object sender, DoWorkEventArgs e)
        //{
        //    quotes_H1 = null;
        //    get_data(ref quotes_H1, ref quotes_D1);
        //}
        //void bgimporter_complete(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    if (quotes_H1 == null || quotes_H1.Length < 5) return;
        //    fillFrames(quotes_H1, quotes_D1, DefaultChartType);
        //    if (RefreshHistory != null) RefreshHistory(moduleName, Name);
        //}
        //void bgimport(object sender, DoWorkEventArgs e)
        //{
        //    quotes_H1 = null;
        //}
        void fillFrames1(ChartType ctype)
        {
            double[] quotes_h1={0,0,0,0,0}, quotes_d1 = null;

            if (moduleName != "Dukascopy")
                get_data(ref quotes_h1, ref quotes_d1);//get_data перевести в бэкграунд\
            else
            {
                //bgimporter = new BackgroundWorker();
                //bgimporter.WorkerSupportsCancellation = true;
                //bgimporter.DoWork += bgimporter_start;
                //bgimporter.RunWorkerCompleted += bgimporter_complete;
                //bgimporter.RunWorkerAsync(ctype);
            }
            
            if (quotes_h1 == null || quotes_h1.Length < 5) return;

            setPipValue(NotKnownDigitsValue, quotes_h1);

            fillFrames(quotes_h1, quotes_d1, ctype);
        }
        //------------------------------------//
        // блок функций для локального хранения данных >>
        //------------------------------------//
        //------------------------------------//
        //------------------------------------//

        /// <summary>
        /// чтение входных данных и вызов функции для заполнения массива Frames[][] для заданного типа графика
        /// </summary>
        /// <param name="ctype">типа графика</param>
        void fillFrames(ChartType ctype)
        {
            double[] quotes_d1 = null,
                     quotes_h1 = Module.import(Name, TF.m60);       //1. попытка получить часовую историю
            if (quotes_h1 == null) quotes_h1 = Module.import(Name); //2. если нет часовой - попытка получить то что дают
            else quotes_d1 = Module.import(Name, TF.Day);           //4. запрос дневной истории если есть часовая

            fillFrames(quotes_h1, quotes_d1, ctype);
        }

        /// <summary>
        /// логарифмирование бара
        /// </summary>
        /// <param name="bar"></param>
        /// <returns></returns>
        TBar log(TBar bar)
        {
            bar.Open = Math.Log10(bar.Open);
            bar.High = Math.Log10(bar.High);
            bar.Low = Math.Log10(bar.Low);
            bar.Close = Math.Log10(bar.Close);
            return bar;
        }

        /// <summary>
        /// преобразование исходного бара double[5] в целевой ТБар в цикле для всех видов чарта(свечи, линии...)
        /// новый бар добавляется в массив Frames
        /// данные логарифмируются при необходимости
        /// </summary>
        public TBar darr2TBar(TF frame, double[] quotes, int bar_idx, int i)
        {
            for (ChartType ctype = 0; ctype < ChartType.count; ctype++)
                if (Frames[(int)ctype, (int)frame] != null)
                    return darr2TBar(ctype, frame, quotes, bar_idx, i);
            return null;
        }
        
        /// <summary>
        /// преобразование исходного бара double[5] в целевой ТБар
        /// новый бар добавляется в массив Frames
        /// данные логарифмируются при необходимости
        /// </summary>
        /// <param name="ctype">тип чарта</param>
        /// <param name="frame">текущий фрейм</param>
        /// <param name="quotes">исходный массив</param>
        /// <param name="bar_idx">счетчик баров ТБар</param>
        /// <param name="i">счетчик для исходного массива квотес</param>
        /// <returns>преобразованный бар</returns>
        public TBar darr2TBar(ChartType ctype, TF frame, double[] quotes, int bar_idx, int i)
        {
            TBar b = new TBar();
            b.Bar = b.X = bar_idx >= 0 ? bar_idx : Frames[(int)ctype, (int)frame].GetCount();
            switch (ctype)
            {
                case ChartType.Candle:
                    if (priceScale == PriceScaleType.log10 || priceScale == PriceScaleType.mixed && frame>=TF.Day)
                    {
                        b.Open = Math.Log10((double)GlobalMembersTAmodel.round((decimal)quotes[i + O], Decimals));
                        b.High = Math.Log10((double)GlobalMembersTAmodel.round((decimal)quotes[i + H], Decimals));
                        b.Low = Math.Log10((double)GlobalMembersTAmodel.round((decimal)quotes[i + L], Decimals));
                        b.Close = Math.Log10((double)GlobalMembersTAmodel.round((decimal)quotes[i + C], Decimals));
                    }
                    else// if (priceScale == PriceScaleType.linear)
                    {
                        b.Open = (double)GlobalMembersTAmodel.round((decimal)quotes[i + O], Decimals);
                        b.High = (double)GlobalMembersTAmodel.round((decimal)quotes[i + H], Decimals);
                        b.Low = (double)GlobalMembersTAmodel.round((decimal)quotes[i + L], Decimals);
                        b.Close = (double)GlobalMembersTAmodel.round((decimal)quotes[i + C], Decimals);
                    }
                    goto gen;
                case ChartType.Line:
                    if (priceScale == PriceScaleType.log10 || priceScale == PriceScaleType.mixed && frame >= TF.Day)
                    {
                        b.Open = b.High = b.Low = b.Close = Math.Log10((double)GlobalMembersTAmodel.round((decimal)quotes[i + C], Decimals));
                    }
                    else// if (priceScale == PriceScaleType.linear)
                    {
                        b.Open = b.High = b.Low = b.Close = (double)GlobalMembersTAmodel.round((decimal)quotes[i + C], Decimals);
                    }
                gen:
                    //general code for both Candle and Linear ChartTypes
                    if(timeisdouble)
                        b.DT = nuldate + TimeSpan.FromDays(quotes[i + (int)Candle.T]);
                    else
                        b.DT = nuldate + TimeSpan.FromSeconds((uint)quotes[i + (int)Candle.T]);
                    
                    // bar_idx = -1: добавление единичного бара в конец списка
                    // bar_idx = -2: не включается в список
                    if (bar_idx >= -1)
                        Frames[(int)ctype, (int)frame].Add(b);
                    return b;
                case ChartType.X0:    //TODO...//
                    break;
            }
            return null;
        }
        // сигнал о получении нового бара онлайн
        public int NewBar(ChartType chartType, TF sourcetimeFrame, TF timeFrame, double[] quotes)
        {
            int ctype = (int)chartType, stf = (int)sourcetimeFrame, tf = (int)timeFrame;
            switch (chartType)
            {
                case ChartType.Candle:
                case ChartType.Line:
                    if (Frames[ctype, tf] != null)
                    {
                        TBar bar = darr2TBar(ChartType.Candle, timeFrame, quotes, -2, 0);
                        //откатили на предыдущий или текущий бар
                        int i = Frames[ctype, tf].lastindex;
                        while ((Frames[ctype, tf][i].DT > bar.DT) && (i > 0)) i--;
                        DateTime dt = Gen.incTime(Frames[ctype, tf][i].DT, timeFrame);
                        //новый бар
                        if (dt <= bar.DT)
                        {
                            while ((dt = Gen.incTime(dt, timeFrame)) <= bar.DT) ;
                            bar.DT = Gen.decTime(dt, timeFrame);
                            Frames[ctype, tf].Quotes.Insert(++i, bar);
                            return i;
                        }
                        //заполняем текущий бар
                        else if (dt > bar.DT)
                        {
                            TBar last = Frames[ctype, tf][i];

                            if (chartType == ChartType.Candle)
                            {
                                if (Frames[ctype, tf][i].DT == bar.DT) last.Open = bar.Open;//обновление Опен только если время oткрытия бара совпадает со временем открытия на данном фрейме
                                if (last.High < bar.High) last.High = bar.High;
                                if (last.Low > bar.Low) last.Low = bar.Low;
                                last.Close = bar.Close;
                            }else{ //if (chartType == ChartType.Linear)
                                last.Open = last.High = last.Low = last.Close = bar.Close;
                            }
                            
                            return i;
                        }
                    }
                    break;
                case ChartType.X0:
                    //TODO
                    break;
            }
            return -1;
        }

        /// <summary>
        /// заполнение котировок по всем фреймам для свечного графика
        /// </summary>
        /// <param name="quotes_h1"></param>
        /// <param name="quotes_d1"></param>
        /// <summary>

        void fillFrames(double[] quotes_h1, double[] quotes_d1, ChartType ctype)
        {
            int ct = (int)ctype;

            //берем исходную историю
            Frames[ct, CF] = new TQuotes(this, TF.custom, ctype, quotes_h1.Length / 5);
            for (int i = 0, j = 0; i < quotes_h1.Length / 5; i++, j += 5) //set real index of bar in double[] quotes_h1 array
            {
                darr2TBar(ctype, TF.custom, quotes_h1, i, j);
            }
            sortListTBar(Frames[ct, CF].Quotes);
            customTF = Frames[ct, CF].calcFrame();

            //m60
            fillFrame(CF, H1, ct, (60 > customTF && customTF > 0) ? Frames[ct, CF].GetCount() / (60 / customTF) : 0);

            //m240
            fillFrame(H1, H4, ct, Frames[ct, H1].GetCount() / 5);

            if (quotes_d1 != null && quotes_d1.Length >= 5) //(quotes_d1.Length >  quotes_h1.Length / 100) -- актуальный размер массива при котором имеет смысл переходить на дневную истории, иначе, доступная история дневок короче чем часовая
            {
                //берем дневную историю
                if (Frames[ct, D] == null) Frames[ct, D] = new TQuotes(this, TF.Day, ctype, quotes_d1.Length / 5);
                else Frames[ct, D].Resize(quotes_d1.Length / 5);

                for (int i = 0, j = 0; i < quotes_d1.Length / 5; i++, j += 5) //set real index of bar in double[] quotes_h1 array
                {
                    darr2TBar(ctype, TF.Day, quotes_d1, i, j);
                }
            }
            else
            {
                //получаем дневную историю из 4-х часовой
                fillFrame(H4, D, ct, Frames[ct, H1].GetCount() / 25);
            }
            //weekly
            fillFrame(D, W, ct, 16);

            //mongthly
            fillFrame(D, M, ct, 16);

            //quarterly
            fillFrame(M, Q, ct, 16);

            //yearly
            fillFrame(Q, Y, ct, 16);

            //очистка от дублирующих фреймов
            for(int i=(int)TF.custom; i<(int)TF.count; i++)
                if (customTF > Gen.tf2i(i)) Frames[ct, i].Quotes.Clear();
            //end
        }

        

        //fillFrame
        /// сборка старшего фрейма из младшего
        /// </summary>
        /// <param name="srcTF">идентификатор исходного фрейма</param>
        /// <param name="destTF">идентификатор целевого фрейма</param>
        void fillFrame(int srcTF, int destTF, int ctype, int destlen)
        {
            TQuotes qsrc = Frames[ctype, srcTF];
            bool doLg = destTF == (int)TF.Day && priceScale == PriceScaleType.mixed;

            ////на случай если исходный фрейм более часового - массив просто перемещается по списку вверх до тех пор пока не станет на свое место
            //if (customTF > Gen.tf2i(destTF))
            //{
            //    //если исходный фрейм >= месячного, то в месяца копируем не дни а недельки, те последовательно переходим
            //    if (srcTF == D && destTF == M && Frames[ctype, srcTF].GetCount() == 0 && Frames[ctype, W].GetCount() > 0)
            //    {
            //        srcTF = W;
            //        qsrc = Frames[ctype, srcTF];
            //    }
            //    //исходныйй массив переходит на один уровень вверх
            //    Frames[ctype, srcTF] = new TQuotes(this, (TF)srcTF, (ChartType)ctype, 0);
            //    if (!doLg)
            //    {
            //        qsrc.tf = (TF)destTF;
            //        Frames[ctype, destTF] = qsrc;
            //        return;
            //    }
            //}

            TQuotes qdest = new TQuotes(this, (TF)destTF, (ChartType)ctype, destlen);

            DateTime nextBarTime = new DateTime();// = Gen.incTimeFloor(b.DT, destTF);
            TBar bar = new TBar();
            switch ((ChartType)ctype)
            {
                case ChartType.Candle:
                    for (int i = 0, j = 0; i < qsrc.GetCount(); i++)
                    {
                        TBar b = qsrc[i];  //local alias of current bar

                        if (i == 0 || nextBarTime <= b.DT) //next bar completed
                        {
                            if (i > 0) qdest.Add(doLg ? log(bar) : bar); //store existing bar
                            bar = new TBar();
                            bar.Bar = bar.X = j++;
                            bar.Open = b.Open;
                            bar.High = b.High;
                            bar.Low = b.Low;
                            bar.Close = b.Close;
                            bar.DT = b.DT;
                            nextBarTime = Gen.incTimeFloor(b.DT, (TF)destTF);
                        }
                        else
                        {
                            if (bar.High < b.High) bar.High = b.High;
                            if (bar.Low > b.Low) bar.Low = b.Low;
                            bar.Close = b.Close;
                        }
                    }
                    break;
                case ChartType.Line:
                    for (int i = 0, j = 0; i < qsrc.GetCount(); i++)
                    {
                        if (i == 0 || nextBarTime <= qsrc[i].DT) //next bar completed
                        {
                            if (i > 0)
                            {
                                bar.Open = bar.High = bar.Low = bar.Close = qsrc[i - 1].Close;
                                qdest.Add(doLg ? log(bar) : bar); //store existing bar
                            }
                            bar = new TBar();
                            bar.Bar = bar.X = j++;
                            bar.DT = qsrc[i].DT;
                            bar.Open = bar.High = bar.Low = bar.Close = qsrc[i].Close;
                            nextBarTime = Gen.incTimeFloor(qsrc[i].DT, (TF)destTF);
                        }
                    }
                    break;
            }

            qdest.Add(doLg ? log(bar) : bar); //store last, possible none completed bar
            Frames[ctype, destTF] = qdest;
        }

        void sortListTBar(List<TBar> list)
        {
            if (list.Count > 2 && list[0].DT > list[2].DT)
            try
            {
                list.Sort(
                    delegate(TBar a, TBar b)
                    {
                        //return (a != null && b != null && a.DT != null && b.DT != null && a.DT > b.DT) ? 1 : -1;
                        if (a.DT > b.DT) return 1;
                        else
                            if (a.DT == b.DT) return 0;
                            else return -1;
                        //return a.DT > b.DT ? 1 : -1;
                    }
                );
                for(int i = 0; i< list.Count; i++) list[i].Bar = list[i].X = i;
            }
            catch (Exception e) {
                MessageBox.Show("sorting troubles: " + e.Message);
            }
        }
    }
}
