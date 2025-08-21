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
        const string ModuleName = "MT4";
        //загрузка информации о модуле
        public static void init(char[] module_name, char[] prompt, char[] description, int[] login_type)
        {
            strcpy(ModuleName, module_name);
            strcpy("Select MT4 csv-output folder", prompt);
            //using this string as file mask
            strcpy("MT4 text files (*60.csv; *60.prn)|*_60_history.prn;*60.prn;*60.csv|MT4 text files (*60.csv)|*60.csv|MT4 text files (*60.prn)|*60.prn", description);
            login_type[0] = (int) LoginType.SelectFolder;
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
        enum TF {custim, m60, m240, Day, Week, Month, Quarter, Year };
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
        static string StoragePath = "";
        public static void storagePath(char[] path)
        {
            if (path[0] == '\0') //get
            {
                strcpy(StoragePath, path);
            }
            else //set
            {
                StoragePath = new string(path, 0, path.Length);
            }
        }

        static string suffix = new string(' ', 60), folder_path;
        /// /////////////////////////////
        MethodInfo ticksreceiver, barsreceiver;
        BackgroundWorker bglistener = null;
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
                    Console.WriteLine("mt4txt.SetTickHandler: can not load Skilful.exe");
                    Console.WriteLine(e.Message);
                }
            }
        }
        /// /////////////////////////////

        /// /////////////////////////////
        public static void import(string symbol_name, int period, int shift, int[] _len, double[] quotes)
        {
            int len = _len[0];
            string filepath = folder_path + "\\" + symbol_name + tf2str(period) + suffix.Substring(suffix.IndexOf("."));
            int startdate = 0;
            if(shift > 10000000)
            {
                startdate = shift;
                shift=0;
            }

            StreamReader sr;
            try { sr = File.OpenText(filepath); }
            catch {
                Console.WriteLine("mt4txt.import: can not load"+filepath);
                return;
            }

            if (suffix.IndexOf(".prn") > 0)
            {

                string s = sr.ReadLine(); //ignore first line with headers
                if(s==null) return;

                //forexite daily format
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
                        int hour = int.Parse(cols[time].Substring(0, 2)) * 60 * 60;
                        int minute = int.Parse(cols[time].Substring(2, 2)) * 60;
                        int second = int.Parse(cols[time].Substring(4, 2));
                        DateTime cdate = new DateTime(year, month, day);
                        DateTime nuldate = new DateTime(1970, 01, 01);
                        TimeSpan ts = cdate - nuldate;
                        int ttime = ts.Days * 24 * 60 * 60 + hour + minute + second;
                        //if (startdate > ttime) continue;

                        quotes[qi++] = ttime;
                        quotes[qi++] = a2f(cols[open]);
                        quotes[qi++] = a2f(cols[high]);
                        quotes[qi++] = a2f(cols[low]);
                        quotes[qi++] = a2f(cols[close]);
                    }
                }
                sr.Close();
            }
            else if (suffix.IndexOf(".csv") > 0)
            {
                //.csv format have not header line
                //read first line
                int i_year = 0;
                int i_year_format = 4; //long
                int i_day = 8;
                int i_month = 5;
                int date = 0, time = 1, open = 2, high = 3, low = 4, close = 5;
                int qi = 0, cnt = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    //проверка переполнения выходного массива
                    if ((qi + 5) > quotes.Length) break;
                    //проверка диапазона
                    if (shift > 0 && shift > cnt++) continue;
                    if (len > 0 && (len + shift) < cnt) break;
                    ///посдедняя строка в файлах метатрейдера содержит как правило неполный бар,
                    ///а такие бары идут фтопку, для этого нужно раскомментировать следующую строку
                    ///if (sr.Peek() == -1) break;

                    //сборка бара
                    string[] cols = line.Split(',');
                    if (cols.Length < 6) continue;

                    //date
                    int year = int.Parse(cols[date].Substring(i_year, i_year_format));
                    int month = int.Parse(cols[date].Substring(i_month, 2));
                    int day = int.Parse(cols[date].Substring(i_day, 2));
                    //time
                    int hour = int.Parse(cols[time].Substring(0, 2)) * 60 * 60;
                    int minute = int.Parse(cols[time].Substring(3, 2)) * 60;
                    DateTime cdate = new DateTime(year, month, day);
                    DateTime nuldate = new DateTime(1970, 01, 01);
                    TimeSpan ts = cdate - nuldate;
                    int ttime = ts.Days * 24 * 60 * 60 + hour + minute;
                    if (startdate > ttime) continue;

                    quotes[qi++] = ttime;
                    quotes[qi++] = a2f(cols[open]);
                    quotes[qi++] = a2f(cols[high]);
                    quotes[qi++] = a2f(cols[low]);
                    quotes[qi++] = a2f(cols[close]);
                }
                sr.Close();
                sr.Dispose();
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
            //folder_path = folder_path.Substring(0, folder_path.LastIndexOf("\\"));

            try
            {
                DirectoryInfo di = new DirectoryInfo(folder_path);
                if (di.Exists)
                {
                    //by default reads only hourly quotes
                    FileInfo[] filelist = di.GetFiles("?*" + suffix);
                    
                    for (int i = 0, j=0; i<filelist.Length; i++ )
                    {
                        string filename = filelist[i].Name;
                        //get symbol name
                        symbols[j] = filename.Substring(0, filename.ToLower().IndexOf(suffix));
                        //get symbol`s data length
                        StreamReader sr;
                        try {
                            sr = File.OpenText(filelist[i].FullName);
                            len[j] = suffix.IndexOf(".csv") >= 0 ? 0 : -1;//one first line is extra, ///also one last line is extra -- it`s not full bar
                            string s;
                            while ((s = sr.ReadLine()) != null) if (s.Length > 0) len[j]++;
                            //set symbol frame
                            period[j++] = 60;
                            sr.Close();
                        }
                        catch(Exception e) {
                            Console.WriteLine("mt4txt.get_symbol_list: can not load" + filelist[i].FullName);
                                Console.WriteLine("The process failed: {0}", e.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            for (int i = 0; i < len.Length; i++) if (len[i] < 0) len[i] = 0;
        }

        //get list of symbol name, his period and length of quote_arrays for each symbol
        public static void get_history_length(string symbol, int period, int[] len)
        {
            string filepath = folder_path + "\\" + symbol + tf2str(period) + suffix.Substring(suffix.IndexOf("."));
            len[0] = 0;
            if (File.Exists(filepath))
            {
                try
                {
                    StreamReader sr = File.OpenText(filepath);
                    
                    len[0] = suffix.IndexOf(".csv")>=0 ? 0 : -1;//one first line is extra///, also one last line is extra -- it`s not full bar
                    string s;
                    while ((s = sr.ReadLine()) != null) if (s.Length > 0) len[0]++;
                    sr.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("mt4txt.get_history_length: can not load" + filepath);
                    Console.WriteLine("The process failed: {0}", e.ToString());
                }
            }
            if (len[0] < 0) len[0] = 0;
        }

        //get count of existing symbols
        public static void get_symbol_count(string folderpath, int[] count/*, int period*/)
        {
            folder_path = folderpath;
            string tf = "60";
            count[0] = 0;
            try
            {
                DirectoryInfo di = new DirectoryInfo(folder_path);
                if (di.Exists)
                //{
                    //by default reads only hourly quotes
                    suffix = tf + (string)".csv";
                    count[0] = di.GetFiles("?*" + suffix).Length;
                    if (count[0] == 0)
                    {
                        suffix = "_" + tf + "_history.prn";
                        count[0] = di.GetFiles("?*" + suffix).Length;
                    }
                    if (count[0] == 0){
                        suffix = tf+".prn";
                        count[0] = di.GetFiles("?*" + suffix).Length;
                    }
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            } 
        }

        public void get_tick(string sym, double tick)
        {
        }
        public void get_bar(string sym, int frame, DateTime lastTime, int lastPosition)
        {
            if (barsreceiver != null)
            {
                string filepath = folder_path + "\\" + sym + tf2i(frame) + suffix.Substring(suffix.IndexOf("."));

                StreamReader sr;
                try { sr = File.OpenText(filepath); }
                catch {
                    Console.WriteLine("mt4txt.get_bar: can not load " + filepath);
                    return;
                }
                string dateformat = suffix.IndexOf(".csv") > 0 ? /*csv*/"yyyy.MM.dd,HH:mm" : /*prn*/"yyyyMMdd,HHmmss";

                string line = sr.ReadLine(); //ignore first line with headers
                //skiping already existing data
                ////////for (int i = 1; i < lastPosition-1; i++) sr.ReadLine();//неа, так низзя

                while ((line = sr.ReadLine()) != null)
                {
                    bool completed = true;
                    if (sr.Peek() == -1)
                    {
                        completed = false;
                        //break;//ignore last bar
                    }

                    string[] cols = line.Split(',');
                    if (cols.Length < 6) continue;
                    DateTime ctime = DateTime.ParseExact(cols[0] + "," + cols[1], dateformat, null);
                    if(ctime < lastTime) continue; //[ > | >= ] -- последний бар может быть не полным, поэтому тоже должен быть переписан

                    TimeSpan ts = ctime - new DateTime(1970, 01, 01);
                    double[] bar = {ts.TotalSeconds, a2f(cols[2]), a2f(cols[3]), a2f(cols[4]), a2f(cols[5])};
                    object[] args = { ModuleName, sym, frame, bar, completed };
                    barsreceiver.Invoke(null, args);
                }
            }
        }

        public void get_pip_value(string sym, double[] pip)
        {
            // pip[0] = 0; //set value only if it`s known
        }
    }
}
