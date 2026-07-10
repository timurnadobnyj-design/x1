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
            strcpy("Rumus", module_name);
            strcpy("Select Rumus text file .txt", prompt);
            //useing this string as file mask
            strcpy("Rumus text files (*.txt)|*.txt|All files|*.*", description);
            login_type[0] = (int)LoginType.SelectFile;
        }
        //преобразование строки содержащей значение типа double в число double
        static double a2f(string s)
        {
            string[] dbl = s.Split(',', '.');
            double val = int.Parse(dbl[0]);
            if (dbl.Length == 2) val += int.Parse(dbl[1]) / Math.Pow(10, dbl[1].Length);
            return val;
        }
        public enum colname { time, open, high, low, close, count };
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
                    Console.WriteLine("rumustxt.SetTickHandler: can not load Skilful.exe");
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
            catch {
                Console.WriteLine("rumustext.import: can not load" + filename);
                return; 
            }

            sr.ReadLine(); //ignore first line with symbol name

            string s = sr.ReadLine();
            //rumus format
            if (s.IndexOf("TIME") >= 0 &&
                s.IndexOf("OPEN") >= 0    // check for existings of all needed header values
            && s.IndexOf("HIGH") >= 0
            && s.IndexOf("LOW") >= 0
            && s.IndexOf("CLOSE") >= 0
            && s.IndexOf("\t") >= 0) // check for column delimiter "\t"
            {
                //date parse
                int time = 0, open = 0, high = 0, low = 0, close = 0;
                //get hash of column indexes by headers
                string[] cols = s.Split('\t',';');
                for (int i = 0; i < cols.Length; i++)
                {
                    switch (cols[i])
                    {
                        case "TIME": time = i; break;
                        case "OPEN": open = i; break;
                        case "HIGH": high = i; break;
                        case "LOW": low = i; break;
                        case "CLOSE": close = i; break;
                    }
                }
                int qi = 0, cnt = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    //проверка переполнения
                    if ((qi + 5) > quotes.Length) break;
                    //проверка диапазона
                    if (shift > 0 && cnt >= shift) continue;
                    if (len > 0 && len + shift > cnt) break;
                    cnt++;
                    //сборка бара
                    cols = line.Split('\t');
                    if (cols.Length < 6) continue;

                    //date
                    int year = int.Parse(cols[time].Substring(6, 2));
                    if (year < 100) year += year >= 70 ? 1900 : 2000;
                    int month = int.Parse(cols[time].Substring(3, 2));
                    int day = int.Parse(cols[time].Substring(0, 2));
                    //time
                    int hour = int.Parse(cols[time].Substring(9, 2)) * 60 * 60;
                    int minute = int.Parse(cols[time].Substring(12, 2)) * 60;
                    int second = int.Parse(cols[time].Substring(15, 2));
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
            StreamReader sr;
            try { sr = File.OpenText(filename); }
            catch {
                Console.WriteLine("rumustext.get_symbol_list: can not load" + filename);
                return;
            }

            //symbol list в данном случае это единственный символ
            symbols[0] = sr.ReadLine();

            string s = sr.ReadLine();
            //rumus format
            if (s.IndexOf("TIME") >= 0 &&
                s.IndexOf("OPEN") >= 0    // check for existings of all needed header values
            && s.IndexOf("HIGH") >= 0
            && s.IndexOf("LOW") >= 0
            && s.IndexOf("CLOSE") >= 0
            && s.IndexOf("\t") >= 0) // check for column delimiter "\t"
            {
                int tb=0;
                period[0]=0x7FFFFFFF;
                while ((s = sr.ReadLine()) != null)
                {
                    len[0]++;
                    //time in minutes
                    int ta = int.Parse(s.Substring(s.IndexOf(':') - 2, 2)) * 60 + int.Parse(s.Substring(s.IndexOf(':') + 1, 2));
                    if (period[0] > (ta - tb)) period[0] = ta - tb;
                    tb = ta;
                }
            }
            sr.Close();
        }
        public static void get_symbol_count(string file_name, int[] count)
        {
            filename = file_name;
            count[0] = 1;
        }
        public static void get_tick(string sym, double tick)
        {
        }
        public void get_bar(string sym, int period, DateTime lastTime,int lastPosition)
        {
        }
    }
}
