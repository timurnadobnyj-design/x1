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
    enum LoginType { SelectFile, SelectDir, SetIPLoginPassword, UserDefined }
    public class DataSourceClass
    {
        //загрузка информации о модуле
        public static void init(char[] module_name, char[] prompt, char[] description, int[] login_type)
        {
            strcpy("MS.txt", module_name);
            strcpy("Select MetaStock text file .txt", prompt);
            //useing this string as file mask
            strcpy("MetaStock text files (*.txt)|*.txt|All files|*.*", description);
            login_type[0] = (int) LoginType.SelectFile;
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
        /// /////////////////////////////
        MethodInfo ticksreceiver, barsreceiver;
        BackgroundWorker bglistener = null;
        static string filename;
        //System.Diagnostics.Process rtserver;
        //frcvr rcvr;
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
                    Console.WriteLine(e.Message);
                }
            }
        }
        /// /////////////////////////////
        public static void import(string symbol_name, int tf, int shift, int[] _len, double[] quotes)
        {
            int len = _len[0];
            //build List<TBar> for first ticker in list
            //                    fill_quotesi(iQuotes, filename, tickers[0]);
            StreamReader sr;
            try { sr = File.OpenText(filename); }
            catch { return; }

            string s = sr.ReadLine(); //ignore first line with headers

            //forexite daily format
            if (s.IndexOf("<TICKER>") >= 0
            && s.IndexOf("<DT") >= 0    // check for existings of all needed header values
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
                int ticker = -1, date = 0, time = 0, open = 0, high = 0, low = 0, close = 0;
                //get hash of column indexes by headers
                string[] cols = s.Split(',');
                for (int i = 0; i < cols.Length; i++)
                {
                    switch (cols[i])
                    {
                        case "<TICKER>": ticker = i; break;
                        case "<DATE>": date = i; break;
                        case "<TIME>": time = i; break;
                        case "<OPEN>": open = i; break;
                        case "<HIGH>": high = i; break;
                        case "<LOW>": low = i; break;
                        case "<CLOSE>": close = i; break;
                    }
                }
                bool symbol_found = false;
                int qi = 0, cnt = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.IndexOf(symbol_name) > -1)
                    {
                        if (line.IndexOf(",,") > -1) continue; //нехватка данных в строке
                        symbol_found = true;
                        //проверка переполнения
                        if ((qi + 5) > quotes.Length) break;
                        //проверка диапазона
                        cnt++;
                        if (shift > 0 && cnt <= shift) continue;
                        if (len > 0 && len + shift < cnt) break;
                        //сборка бара
                        cols = line.Split(',');
                        if (cols.Length < 6) continue;

                        //date
                        int year = int.Parse(cols[date].Substring(i_year, i_year_format));
                        int month = int.Parse(cols[date].Substring(i_month, 2));
                        int day = int.Parse(cols[date].Substring(i_day, 2));
                        //time
                        int hour =0, minute =0, second = 0;
                        if (time > 0)
                        {
                            hour = int.Parse(cols[time].Substring(0, 2)) * 60 * 60;
                            minute = int.Parse(cols[time].Substring(2, 2)) * 60;
                            second = int.Parse(cols[time].Substring(4, 2));
                        }
                        DateTime cdate = new DateTime(year, month, day);
                        DateTime nuldate = new DateTime(1970, 01, 01);
                        TimeSpan ts = cdate - nuldate;
                        int ttime = ts.Days * 24 * 60 * 60 + hour + minute + second;

                        quotes[qi++] = ttime;
                        quotes[qi++] = a2f(cols[open]);
                        quotes[qi++] = a2f(cols[high]);
                        quotes[qi++] = a2f(cols[low]);
                        quotes[qi++] = a2f(cols[close]);
                    }
                    else if (symbol_found)
                    {
                        break;
                    }
                }
            }
            sr.Close();
        }
        static void strcpy(string s, char[] d)
        {
            for (int i = 0; i < s.Length; i++)
            {
                d[i] = s[i];
            }
            d[s.Length] = '\0';
        }
        //чтение и инициализация файла
        public static void get_symbol_list(string[] symbols, int[] period, int[] len)
        {
            List<int> tickers_len = new List<int>();
            List<string> tickers = new List<string>();

            StreamReader sr;
            try { sr = File.OpenText(filename); }
            catch { return; }

            string s = sr.ReadLine();

            //MetaStock daily format
            if (s.IndexOf("<TICKER>") >= 0 &&
                s.IndexOf("<DT") >= 0    // check for existings of all needed header values
            && s.IndexOf("<TIME>") >= 0
            && s.IndexOf("<OPEN>") >= 0
            && s.IndexOf("<HIGH>") >= 0
            && s.IndexOf("<LOW>") >= 0
            && s.IndexOf("<CLOSE>") >= 0
            && s.IndexOf(">,<") >= 0) // check for column delimiter ","
            {
                string[] cols = s.Split(',');
                //get hash of column indexes by headers
                int ticker = -1, time = -1;
                for (int i = 0; i < cols.Length; i++)
                {
                    switch (cols[i])
                    {
                        case "<TICKER>": ticker = i; break;
                        case "<TIME>": time = i; break;
                    }
                }

                //get list of tickers
                if (ticker >= 0 && time >= 0)
                {
                    string sa, sb = "empty";
                    int ta, tb = 0;
                    int cnt = 0;
                    period[0] = 0x7FFFFFFF;
                    while ((sa = sr.ReadLine()) != null)
                    {
                        cols = sa.Split(',');
                        cnt++;
                        ta = int.Parse(cols[time]);
                        sa = cols[ticker];
                        if (sa != sb)
                        {
                            //get min period in seconds
                            if (period[0] > (ta - tb)) period[0] = ta - tb;
                            //get tickers count
                            if (sb != "empty") tickers_len.Add(cnt);
                            //get tickers name
                            tickers.Add(sa);
                            sb = sa;
                            tb = ta;
                            cnt = 0;
                        }
                    }
                    tickers_len.Add(cnt);
                }
                sr.Close();
            }
            for (int i = 0; i < tickers.Count; i++)
            {
                len[i] = tickers_len[i];
                symbols[i] = tickers[i];
            }
            period[0] /= 60; //second to minutes
        }
        public static void get_symbol_count(string file_name, int[] count)
        {
            filename = file_name;
            count[0] = 0;
            StreamReader sr;
            try{sr = File.OpenText(filename);}catch{return;}

            string s = sr.ReadLine();
            string[] cols = s.Split(',');
            //get hash of column indexes by headers
            //get list of tickers
            int ticker = -1;
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i] == "<TICKER>") ticker = i;
            }
            if (ticker >= 0)
            {
                string sa, sb = "empty";
                while ((sa = sr.ReadLine()) != null)
                {
                    sa = sa.Split(',')[ticker];
                    if (sa != sb)
                    {
                        sb = sa;
                        count[0]++;
                    }
                }
            }
            sr.Close();
        }
        public static void get_tick(string sym, double tick)
        {
        }
        public void get_bar(string sym, int period, DateTime lastTime, int lastPosition)
        {
            if (barsreceiver != null)
            {
                //string filepath = folder_path + "\\" + sym + period + suffix.Substring(suffix.IndexOf("."));

                //StreamReader sr;
                //try { sr = File.OpenText(filepath); }
                //catch { return; }
                //string dateformat = suffix.IndexOf(".csv") > 0 ? /*csv*/"yyyy.MM.dd,HH:mm" : /*prn*/"yyyyMMdd,HHmmss";

                //string s = sr.ReadLine(); //ignore first line with headers
                //string line;
                //while ((line = sr.ReadLine()) != null)
                //{
                //    string[] cols = line.Split(',');
                //    if (cols.Length < 6) continue;
                //    DateTime ctime = DateTime.ParseExact(cols[0] + "," + cols[1], dateformat, null);
                //    if (ctime <= lastTime) continue;

                //    DateTime nuldate = new DateTime(1970, 01, 01);
                //    TimeSpan ts = ctime - nuldate;
                //    int ttime = ts.Seconds;

                //    double bar = { ttime, a2f(cols[open]), a2f(cols[high]), a2f(cols[low]), a2f(cols[close]) };
                //    object[] args = { ModuleName, sym, frame, bar true};
                //    barsreceiver.Invoke(null, args);
                //}
            }
        }
    }
}
