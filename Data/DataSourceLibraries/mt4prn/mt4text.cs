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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using System.ComponentModel;
using Microsoft.Win32;

namespace Skilful.Data
{
    enum LoginType { SelectFile, SelectFolder, SetIPLoginPassword, UserDefined }
    public class DataSourceClass
    {
        const string ModuleName = "CSV";
        static MethodInfo libmsgreceiver;
        //загрузка информации о модуле
        public static void init(char[] module_name, char[] prompt, char[] description, int[] login_type)
        {
            strcpy(ModuleName, module_name);
            strcpy("Select csv, prn, txt file", prompt);
        
            //useing this string as file mask
            strcpy("datasheet files (*.csv; *.prn; *.txt)|*.prn;*.csv;*.txt|MT4 text files (*.csv)|*.csv|mt4 import (*.prn)|*.prn", description);
            login_type[0] = (int) LoginType.SelectFile;

            //функция передачи сообщений в вызывающую программу
            if(libmsgreceiver != null)
               libmsgreceiver = Assembly.LoadFrom("Skilful.exe").GetType("Skilful.QuotesManager.MDataManager").GetMethod("LibMsgReciever");
        }
        //преобразование строки содержащей значение типа double в число double
        static double a2f(string s)
        {
            string[] dbl = s.Split('.');
            double val = int.Parse(dbl[0]);
            if (dbl.Length == 2) val += int.Parse(dbl[1]) / Math.Pow(10, dbl[1].Length);
            return val;
        }
        public enum colname { ticker, date, time, open, high, low, close, count };
        public enum Q { T, O, H, L, C };
        //enum TF { m1, m5, m15, m30, m60, m240, Day, Week, Month, Quarter, Year };
        enum TF {custom, m60, m240, Day, Week, Month, Quarter, Year };
        static string tf2str(int tf)
        {
            //return string.Format("{0:d}", tf2i(tf));
            return tf2i(tf).ToString();
        }
        static int tf2i(int tf)
        {
            switch ((TF)tf)
            {
                case TF.m60: return 60;
                case TF.m240: return 240;
                case TF.Day: return 1440;
                case TF.Week: return 10080;
                case TF.Month: return 43200;
                default: return 60;
            }
        }
        //путь по умолчанию для локальной БД
        //static string StoragePath = "";
        public static void storagePath(char[] path)
        {
            //отключено навсегда, поскольку не нужно для данного типа импорта
        }

        static string suffix = new string(' ', 60);
        /// /////////////////////////////
        MethodInfo ticksreceiver, barsreceiver;
        BackgroundWorker bglistener = null;
        //System.Diagnostics.Process rtserver;
        //frcvr rcvr;
        static string filename;

        public void SetTickHandler(string sym/*frcvr rcvr*/)

        {
            if (bglistener == null)
            {
                try
                {
                    ticksreceiver = Assembly.LoadFrom("Skilful.exe").GetType("Skilful.QuotesManager.MDataManager").GetMethod("RecieveNewTick");
                    barsreceiver = Assembly.LoadFrom("Skilful.exe").GetType("Skilful.QuotesManager.MDataManager").GetMethod("RecieveNewBar");
                }
                catch (Exception e)
                {
                    Console.WriteLine("mt4txt.SetTickHandler: can not load Skilful.exe");
                    Console.WriteLine(e.Message);
                }
            }
        }
        /// /////////////////////////////

        //-------------------------------------//
        void SendMessage(string message)
        {
            if (libmsgreceiver != null)
            {
                string symbol = null;
                int code = 0;
                object[] args = { ModuleName, symbol, code, message };
                libmsgreceiver.Invoke(null, args);
            }
        }

        /// /////////////////////////////
        public void import(string symbol_name, int period, int shift, int[] _len, double[] quotes)
        {
            int len = _len[0];
            string filepath = filename;
            int startdate = 0;
            if(shift > 10000000)
            {
                startdate = shift;
                shift=0;
            }

            StreamReader sr;
            try
            {
                sr = File.OpenText(filepath);

                if (suffix == ".prn" || suffix == ".txt")
                {

                    string s = sr.ReadLine(); //ignore first line with headers
                    if (s == null) return;

                    s = s.Replace("<DATE>", "<DTYYYYMMDD>");
                    if (s.IndexOf("<DT") >= 0    // check for existings of all needed header values
                    && s.IndexOf("<TIME>") >= 0
                    && s.IndexOf("<OPEN>") >= 0
                    && s.IndexOf("<HIGH>") >= 0
                    && s.IndexOf("<LOW>") >= 0
                    && s.IndexOf("<CLOSE>") >= 0
                    && s.IndexOf(">,<") >= 0) // check for column delimiter ","
                    {
                        //date parse
                        int start = s.IndexOf("<DT");
                        string adate = s.Substring(start, s.IndexOf(',', start) - start);
                        s = s.Replace(adate, "<DATE>");
                        //adate = adate.Substring(2); //get adate without "DT" prefix
                        //cols[colname.adate]
                        int i_year = adate.IndexOf("YYYY") - 3; //-3 == '<DT'
                        int i_year_format = 4; //long
                        if (i_year == -1)
                        {
                            i_year = adate.IndexOf("YY") - 3;
                            i_year_format = 2; //short
                        }
                        int i_day = adate.IndexOf("DD") - 3;
                        int i_month = adate.IndexOf("MM") - 3;
                        //default quotes can be without ticker specified
                        int date = 0, time = 0, open = 0, high = 0, low = 0, close = 0;
                        //get hash of column indexes by headers
                        string[] cols = s.Split(',');
                        for (int i = 0; i < cols.Length; i++)
                        {
                            switch (cols[i])
                            {
                                case "<DATE>": date = i; break;
                                case "<TIME>": time = i; break;
                                case "<OPEN>": open = i; break;
                                case "<HIGH>": high = i; break;
                                case "<LOW>": low = i; break;
                                case "<CLOSE>": close = i; break;
                            }
                        }
                        int qi = 0, cnt = 0;
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            //проверка переполнения выходного массива
                            if ((qi + 5) > quotes.Length) break;
                            //проверка диапазона
                            if (shift > 0 && shift > cnt++) continue;
                            if (len > 0 && len + shift < cnt) break;
                            ///посдедняя строка в файлах метатрейдера содержит как правило неполный бар,
                            ///а такие бары идут фтопку, для этого нужно раскомментировать следующую строку
                            ///if (sr.Peek() == -1) break;

                            //сборка бара
                            cols = line.Split(',');
                            if (cols.Length < 6) continue;

                            //date
                            int year = int.Parse(cols[date].Substring(i_year, i_year_format));
                            int month = int.Parse(cols[date].Substring(i_month, 2));
                            int day = int.Parse(cols[date].Substring(i_day, 2));
                            //time
                            int hour = 0, minute = 0, second = 0;
                            if (cols[time].Length == 6)
                            {
                                hour = int.Parse(cols[time].Substring(0, 2));
                                minute = int.Parse(cols[time].Substring(2, 2));
                                second = int.Parse(cols[time].Substring(4, 2));
                            }
                            DateTime cdate = new DateTime(year, month, day, hour, minute, second);
                            DateTime nuldate = new DateTime(0001, 01, 01);
                            TimeSpan ts = cdate - nuldate;

                            quotes[qi++] = ts.TotalDays;
                            quotes[qi++] = a2f(cols[open]);
                            quotes[qi++] = a2f(cols[high]);
                            quotes[qi++] = a2f(cols[low]);
                            quotes[qi++] = a2f(cols[close]);
                        }
                    }
                    sr.Close();
                }
                else if (suffix == ".csv")
                {
                    //.csv format have not header line
                    //read first line
                    int i_year = 0;
                    int i_year_format = 4; //long
                    int i_day = 8;
                    int i_month = 5;
                    int date = 0, time = 1, open = 2, high = 3, low = 4, close = 5;
                    int qi = 0, cnt = 0, fieldscount = 6;
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.IndexOf("Date,Open,High,Low,Close") == 0) //stooq.com.ua, finance.yahoo.com
                        {
                            time = -1;
                            open--;
                            high--;
                            low--; 
                            close--;
                            fieldscount = 5;
                            continue;
                        }
                        else if (line.IndexOf("Date,Close") == 0) //stooq.com.ua, finance.yahoo.com
                        {
                            time = -1;
                            open=high=low=close=1;
                            fieldscount = 2;
                            continue;
                        }

                        //проверка переполнения выходного массива
                        if ((qi + 5) > quotes.Length) break;
                        //проверка диапазона
                        if (shift > 0 && shift > cnt++) continue;
                        if (len > 0 && (len + shift) < cnt) break;
                        ///последняя строка в файлах метатрейдера содержит как правило неполный бар,
                        ///а такие бары идут фтопку, для этого нужно раскомментировать следующую строку
                        ///if (sr.Peek() == -1) break;

                        //сборка бара
                        string[] cols = line.Split(',');
                        if (cols.Length < fieldscount) continue;

                        //date
                        int year = int.Parse(cols[date].Substring(i_year, i_year_format));
                        int month = int.Parse(cols[date].Substring(i_month, 2));
                        int day = int.Parse(cols[date].Substring(i_day, 2));
                        //time
                        int hour=0, minute=0;
                        if (time >= 0 && cols[time].Length >= 5)
                        {
                            hour = int.Parse(cols[time].Substring(0, 2));
                            minute = int.Parse(cols[time].Substring(3, 2));
                        }
                        DateTime cdate = new DateTime(year, month, day, hour, minute, 0);
                        DateTime nuldate = new DateTime(0001, 01, 01);
                        TimeSpan ts = cdate - nuldate;

                        quotes[qi++] = ts.TotalDays;
                        quotes[qi++] = a2f(cols[open]);
                        quotes[qi++] = a2f(cols[high]);
                        quotes[qi++] = a2f(cols[low]);
                        quotes[qi++] = a2f(cols[close]);
                    }
                    sr.Close();
                    sr.Dispose();
                }
            }
            catch (Exception e)
            {
                SendMessage("error file reading " + filepath + ": " + e.Message);
            }
        }
        static void strcpy(string s, char[] d)
        {
            for (int i = 0; i < s.Length; i++)
            {
                d[i] = s[i];
            }
            d[s.Length] = '\0';
        }

        //get list of symbol name, his period and length of quote_arrays for each symbol
        public static void get_symbol_list(string[] symbols, int[] period, int[] len)
        {
            StreamReader sr;
            try { sr = File.OpenText(filename); }
            catch
            {
                Console.WriteLine("mt4text,CSV::.get_symbol_list: can not load" + filename);
                return;
            }

            //symbol list в данном случае это единственный символ
            int start = filename.LastIndexOf("\\")+1;
            int length = filename.LastIndexOf(".");
            symbols[0] = filename.Substring(start, (length > start ? length : filename.Length) - start);
            //
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                string[] tmp = s.Split('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
                int count = tmp.Length;
                if (count >= 10) //min length: 01-02-03,1,2,3,4
                {
                    len[0]++;
                    break;
                }
            }
            //int tb = 0;
            period[0] = 0x7FFFFFFF;
            while ((s = sr.ReadLine()) != null)
            {
                len[0]++;
                //time in minutes
                //int ta = int.Parse(s.Substring(s.IndexOf(':') - 2, 2)) * 60 + int.Parse(s.Substring(s.IndexOf(':') + 1, 2));
                //if (period[0] > (ta - tb)) period[0] = ta - tb;
                //tb = ta;
            }
            sr.Close();
        }

        public static void get_symbol_count(string file_name, int[] count)
        {
            filename = file_name;
            suffix = filename.Substring(filename.LastIndexOf('.'));
            count[0] = 1;
        }

        //get list of symbol name, his period and length of quote_arrays for each symbol
        public static void get_history_length(string symbol, int period, int[] len)
        {
            len[0] = 0;
            if (File.Exists(filename))
            {
                try
                {
                    StreamReader sr = File.OpenText(filename);

                    len[0] = 0;
                    string s;
                    while ((s = sr.ReadLine()) != null) if (s.Length > 0) len[0]++;
                    sr.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("mt4text.get_history_length: can not load" + filename);
                    Console.WriteLine("The process failed: {0}", e.ToString());
                }
            }
            if (len[0] < 0) len[0] = 0;
        }

    }
}
