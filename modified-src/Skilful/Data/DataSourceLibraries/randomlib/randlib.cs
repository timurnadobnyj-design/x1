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
//using System.Windows;
using System.Reflection;
using System.Threading;
using System.ComponentModel;
using Microsoft.Win32;

namespace Skilful.Data
{
    enum LoginType { SelectFile, SelectFolder, SetIPLoginPassword, UserDefined }
    //public delegate void frcvr(string SourceName, string SymbolName, DateTime time, double tick);
    public class DataSourceClass
    {
        const string ModuleName = "Random quotes generator";
        //загрузка информации о модуле3
        public static void init(char[] module_name, char[] prompt, char[] description, int[] login_type)
        {
            strcpy(ModuleName, module_name);
            strcpy("set preferred length and period", prompt);
            //using this string as file mask
            strcpy("Initial bars number=50000", description);
            login_type[0] = (int)LoginType.UserDefined;
        }
        //преобразование строки содержащей значение типа double в число double
        static double a2f(string s)
        {
            string[] dbl = s.Split(',',';','.');
            double val= int.Parse(dbl[0]);
            if( dbl.Length==2 ) val+= int.Parse(dbl[1]) / Math.Pow(10, dbl[1].Length);
            return val;
        }
        public enum colname { ticker, date, time, open, high, low, close, count };
        public enum Q { T, O, H, L, C };

        /// /////////////////////////////
        MethodInfo ticksreceiver, barsreceiver;
        BackgroundWorker bglistener;
        System.Diagnostics.Process rtserver_imp, rtserver_lst;
        static string folder_path;
        //frcvr rcvr;
        public void SetTickHandler(string sym/*frcvr rcvr*/)
        {
            if (bglistener == null)
            {
                try
                {
                    ticksreceiver = Assembly.LoadFrom("Skilful.exe").GetType("Skilful.QuotesManager.MDataManager").GetMethod("RecieveNewTick");
                    barsreceiver = Assembly.LoadFrom("Skilful.exe").GetType("Skilful.QuotesManager.MDataManager").GetMethod("RecieveNewBar");
                    bglistener = new BackgroundWorker();
                    bglistener.WorkerSupportsCancellation = true;
                    bglistener.DoWork+= ticks_listener;
                    bglistener.RunWorkerAsync(sym);
                }
                catch (Exception e)
                {
                    Console.WriteLine("randlib.SetTickHandler: can not load Skilful.exe");
                    Console.WriteLine(e.Message);
                }
            }
        }
        public void ticks_listener(object sender, DoWorkEventArgs e)
        {
            if (ticksreceiver != null)
            {
                string sym = (string)e.Argument;
                rtserver_lst = new System.Diagnostics.Process();
                rtserver_lst.StartInfo.FileName = "rtserv.exe";
                rtserver_lst.StartInfo.Arguments = "listen " + sym + " " + last_time[sym] + " " + last_tick[sym];
                rtserver_lst.StartInfo.UseShellExecute = false;
                rtserver_lst.StartInfo.RedirectStandardOutput = true;
                //rtserver_lst.StartInfo.RedirectStandardInput = true;
                //rtserver_lst.StartInfo.RedirectStandardError = true;
                rtserver_lst.StartInfo.CreateNoWindow = true;
                bool res = rtserver_lst.Start();
                

                string input;
                while ((input = rtserver_lst.StandardOutput.ReadLine()) != null)
                {
                    string[] vals = input.Split(';');
                    if (vals[0] == "tick")
                    {
                        DateTime time = DateTime.Parse(vals[2]);
                        double bid = a2f(vals[3]);
                        double ask = a2f(vals[4]);
                        object[] args = { ModuleName, vals[1], time, bid, ask };
                        ticksreceiver.Invoke(null, args);
                    }
                    else if (vals[0] == "bar")
                    {
                        DateTime cdate = DateTime.Parse(vals[3]);
                        DateTime nuldate = new DateTime(1970, 01, 01);
                        TimeSpan ts = cdate - nuldate;
                        double ttime = ts.TotalSeconds;
                        TF tf = (TF)int.Parse(vals[2]);
                        double[] bar = {ttime, a2f(vals[4]), a2f(vals[5]), a2f(vals[6]), a2f(vals[7])};
                        object[] args = { ModuleName, vals[1], tf, bar, true};
                        barsreceiver.Invoke(null, args);
                    }
                }
                /////rtserver_lst.BeginOutputReadLine();
            }
        }
        static Dictionary<string, string> last_time = new Dictionary<string, string>();
        static Dictionary<string, string> last_tick = new Dictionary<string, string>();
        enum TF{
            custom, m60, m240, Day, Week, Month, Quarter, Year, //набор базовых фреймов
        };
        public void import(string symbol_name, int tf, int shift, int[] _len, double[] quotes)
        {
            int len = _len[0];
            if (rtserver_imp == null || rtserver_imp.HasExited)
            {
                rtserver_imp = new System.Diagnostics.Process();
                rtserver_imp.StartInfo.FileName = "rtserv.exe";
                rtserver_imp.StartInfo.Arguments = "get_history " + symbol_name + " " + (int)TF.m60 + " " + (len>0?len:(quotes.Length/5));
                rtserver_imp.StartInfo.UseShellExecute = false;
                rtserver_imp.StartInfo.RedirectStandardOutput = true;
                rtserver_imp.StartInfo.RedirectStandardInput = true;
                rtserver_imp.StartInfo.CreateNoWindow = true;
                bool res = rtserver_imp.Start();
            }
            
            //rtserver_imp.StandardInput.WriteLine("get_history " + symbol_name + " " + (int)TF.m60 + " " + (len>0?len:(quotes.Length/5)));

            const int time = 0, open = 1, high = 2, low = 3, close = 4;
            int qi = 0, cnt = 0;
            string line;
            string[] cols = new string[5];
            try
            {
                while ((line = rtserver_imp.StandardOutput.ReadLine()) != null)
                {
                    //проверка переполнения
                    if ((qi + 4) > quotes.Length) break;
                    //проверка диапазона
                    if (shift > 0 && cnt >= shift) continue;
                    if (len > 0 && len + shift > cnt) break;
                    cnt++;
                    //сборка бара
                    cols = line.Split(' ');
                    if (cols.Length != 5) continue;

                    DateTime cdate = DateTime.Parse(cols[time]);
                    DateTime nuldate = new DateTime(1970, 01, 01);
                    TimeSpan ts = cdate - nuldate;

                    //quotes[qi++] = ts.TotalSeconds;

                    quotes[qi++] = (DateTime.Parse(cols[time]) - new DateTime(1970, 01, 01)).TotalSeconds; //c-style t_time

                    quotes[qi++] = a2f(cols[open]);
                    quotes[qi++] = a2f(cols[high]);
                    quotes[qi++] = a2f(cols[low]);
                    quotes[qi++] = a2f(cols[close]);
                }
                last_time.Add(symbol_name, cols[time]);
                last_tick.Add(symbol_name, cols[close]);
            }
            catch (Exception e)
            {
                if (rtserver_imp != null && !rtserver_imp.HasExited)
                {
                    rtserver_imp.Kill();
                    rtserver_imp.WaitForExit();
                }
                Console.WriteLine(e.Message);
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
            symbols[0] = "ARand12345.6";
            symbols[1] = "BRand12345.";
            symbols[2] = "CRand1234.5";
            symbols[3] = "DRand123.45";
            symbols[4] = "ERand12.345";
            symbols[5] = "FRand1.2345";
            symbols[6] = "GRand.12345";
            //fill period[] and len[]
            string[] param = folder_path.Split(',');
            char delim='=';
            if (param[0].IndexOf('=') == -1) delim = ':';
            int _len = int.Parse(param[0].Substring(param[0].IndexOf(delim) + 1));
            int _period = 60; // int.Parse(param[1].Substring(param[1].IndexOf('=') + 1));
            for (int i = 0; i < 7; i++)
            {
                len[i] = _len;
                period[i] = _period;
            }
        }
        //get count of existing symbols
        public static void get_symbol_count(string folderpath, int[] count)
        {
            folder_path = folderpath;
            count[0] = 7;
        }
        //следующие 2 метода могут вызываться менеджером данных с определенным интервалом
        //double[] tick == tick[0]=bid, tick[1]=ask, tick[2]=time(day.miliseconds)
        public void get_tick(string sym, double[] tick)
        {
        }
        //вход: если bar[0] не пустой - то должен хранить время последнего полученного бара
        //если пустой то фонкция возвращает последний известный бар.
        public void get_bar(string sym, int period, DateTime lastTime, int lastPosition)
        {
        }
        ~DataSourceClass()
        {
            if (rtserver_imp != null && !rtserver_imp.HasExited)
            {
                rtserver_imp.Kill();
                rtserver_imp.WaitForExit();
            }
            if (rtserver_lst != null && !rtserver_lst.HasExited)
            {
                rtserver_lst.Kill();
                rtserver_lst.WaitForExit();
            }
            if (bglistener != null) bglistener.CancelAsync();
        }
    }
}
