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
            strcpy("MS.prn", module_name);
            strcpy("Select MetaStock .prn files Directory", prompt);
            //useing this string as file mask
            strcpy("MetaStock prn files (*.prn)|*.prn|All files|*.*", description);
            login_type[0] = (int) LoginType.SelectDir;
        }
        //преобразование строки содержащей значение типа double в число double
        static double a2f(string s)
        {
            if(s.Length==0) return 0;

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
        static string folder_path;
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
            try { sr = File.OpenText(folder_path + "\\" + symbol_name + ".prn"); }
            catch { return; }

            string s = sr.ReadLine(); //ignore first line with headers

            //forexite daily format
            if (s.IndexOf("<TICKER>") >= 0
            && s.IndexOf("<DATE") >= 0    // check for existings of all needed header values
            //&& s.IndexOf("<TIME>") >= 0
            && s.IndexOf("<OPEN>") >= 0
            && s.IndexOf("<HIGH>") >= 0
            && s.IndexOf("<LOW>") >= 0
            && s.IndexOf("<CLOSE>") >= 0
            && s.IndexOf(">,<") >= 0) // check for column delimiter ","
            {
                //date parse
                ////int start = s.IndexOf("<DT");
                ////string adate = s.Substring(start, s.IndexOf(',', start) - start);
                ////s = s.Replace(adate, "<DATE>");
                //adate = adate.Substring(2); //get adate without "DT" prefix
                //cols[colname.adate]
                int i_year = 0;
                int i_year_format = 4; //long
                ////if (i_year == -1)
                ////{
                ////    i_year = adate.IndexOf("YY") - 3;
                ////    i_year_format = 2; //short
                ////}
                int i_day = 6;
                int i_month = 4;
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
                if (open == 0 && high == 0 && low == 0 && close > 0)
                    open = high = low = close;

                if (open == 0) open = high;
                //bool symbol_found = false;
                int qi = 0, cnt = 0;
                string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //if (line.IndexOf(symbol_name) > -1) // в этой версии предполагается что каждый инструмент в отдельном файле
                        // и имя файла может отличаться от имени инструмента
                        //{
                        if (line.IndexOf(",,,,,") > -1) continue; //нехватка данных в строке
                        //symbol_found = true;
                        //проверка переполнения
                        if ((qi + 5) > quotes.Length) break;
                        //проверка диапазона
                        if (shift > 0 && shift > cnt++) continue;
                        if (len > 0 && (len + shift) < cnt) break;
                        cnt++;
                        //сборка бара
                        cols = line.Split(',');
                        if (cols.Length < 6) continue;
                        //если в данных отлько клоуз и ОНЛ пропущены
                        if (line.IndexOf(",,") > -1 
                            && cols[open].Length == 0
                            && cols[high].Length == 0
                            && cols[low].Length == 0
                            && cols[close].Length > 0)
                        {
                            open = high = low = close;
                        }

                        int ttime=0;
                        //date
                        int year = int.Parse(cols[date].Substring(i_year, i_year_format));
                        int month = int.Parse(cols[date].Substring(i_month, 2));
                        int day = int.Parse(cols[date].Substring(i_day, 2));
                        //time
                        int hour = 0, minute = 0, second = 0;
                        if (time > 0)
                        {
                            hour = int.Parse(cols[time].Substring(0, 2)) * 60 * 60;
                            minute = int.Parse(cols[time].Substring(2, 2)) * 60;
                            second = int.Parse(cols[time].Substring(4, 2));
                        }
                        DateTime cdate = new DateTime(year, month, day);
                        DateTime nuldate = new DateTime(1970, 01, 01);
                        TimeSpan ts = cdate - nuldate;
                        ttime = ts.Days * 24 * 60 * 60 + hour + minute + second;

                            quotes[qi++] = ttime;
                        quotes[qi++] = a2f(cols[open]);
                        quotes[qi++] = a2f(cols[high]);
                        quotes[qi++] = a2f(cols[low]);
                        quotes[qi++] = a2f(cols[close]);

                        //издержки возможных ошибок в источнике данных
                        if(quotes[qi-4]==0 && quotes[qi-1]>0) quotes[qi-4] = quotes[qi-1];
                        if(quotes[qi-3]==0 && quotes[qi-1]>0) quotes[qi-3] = quotes[qi-1];
                        if(quotes[qi-2]==0 && quotes[qi-1]>0) quotes[qi-2] = quotes[qi-1];
                        if(quotes[qi-1]==0 && quotes[qi-4]>0) quotes[qi-1] = quotes[qi-4];

                        //}
                        //else if (symbol_found)
                        //{
                        //    break;
                        //}
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

        //чтение и инициализация файлов
        public static void get_symbol_list(string[] symbols, int[] period, int[] len)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(folder_path);
                if (di.Exists)
                {
                    //by default reads only hourly quotes
                    FileInfo[] filelist = di.GetFiles("?*.prn");

                    for (int i = 0, j = 0; i < filelist.Length; i++)
                    {
                        string filename = filelist[i].Name;
                        //get symbol`s data length
                        StreamReader sr;
                        try
                        {
                            sr = File.OpenText(filelist[i].FullName);
                            len[j] = 0;
                            string s;
                            if ((s = sr.ReadLine()) != null && s.IndexOf("<TICKER>,") == 0)
                            {
                                while ((s = sr.ReadLine()) != null)
                                    if (s.Length > 0 && s.IndexOf(",,,,,") == -1)
                                        len[j]++;
                                //get symbol name
                                symbols[j] = filename.Substring(0, filename.ToLower().IndexOf(".prn"));
                                //set symbol frame
                                period[j++] = 60;
                            }
                            sr.Close();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("metastok.prn.get_symbol_list: can not load" + filelist[i].FullName);
                            Console.WriteLine("The process failed: {0}", e.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            for (int i = 0;  i < len.Length; i++) if (len[i] < 0) len[i] = 0;
        }

        public static void get_symbol_count(string folderpath, int[] count)
        {
            folder_path = folderpath;
            count[0] = 0;
            try
            {
                DirectoryInfo di = new DirectoryInfo(folder_path);
                if (di.Exists)
                {
                    count[0] = di.GetFiles("?*.prn").Length;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        public void get_pip_value(string sym, double[] pip)
        {
            StreamReader sr;
            try { sr = File.OpenText(folder_path + "\\" + sym + ".prn"); }
            catch { return; }

            string s = sr.ReadLine(); //ignore first line with headers
            string[] cols = s.Split(',');
            int close = 0;
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i] == "<CLOSE>")
                {
                    close = i; 
                    break;
                }
            }
            int digits=0;
            for (int i = 0; i < 30 && (s = sr.ReadLine()) != null;)
            {
                cols = s.Split(',');
                if (cols[close].Length == 0) continue;
                cols = cols[close].Split('.');
                if (cols.Length == 2 && cols[1].Length > digits) digits = cols[1].Length;
                //i++;
            }
            sr.Close();

            pip[0] = Math.Pow(10, -digits);
        }
    }
}
