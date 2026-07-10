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
        const string ModuleName = "Dukascopy";
        static MethodInfo libmsgreceiver;
        static StreamWriter sw = null;
        static bool dbg = false;

        //загрузка информации о модуле
        public static void init(char[] module_name, char[] prompt, char[] description, int[] login_type)
        {
            strcpy(ModuleName, module_name);
            strcpy("Please enter URL, Login and Password", prompt);
            
            //using this string as file mask
            strcpy("Server Address=https://www.dukascopy.com/client/demo/jclient/jforex.jnlp, Login=, Passwrd=", description);
            login_type[0] = (int)LoginType.UserDefined;

            //функция передачи сообщений в вызывающую программу
            if (libmsgreceiver != null)
                libmsgreceiver = Assembly.LoadFrom("Skilful.exe").GetType("Skilful.QuotesManager.MDataManager").GetMethod("LibMsgReciever");

            //true to append to existing or create new, false to overwrite existing or create new
            if (dbg)
            {
                sw = new StreamWriter("dukas.log");
                sw.AutoFlush = true; //будет сразу в файл сбрасывать, не буферизуя
            }
        }

        //преобразование строки содержащей значение типа double в число double
        static double a2f(string s)
        {
            string[] dbl = s.Split(',',';','.');
            double val= int.Parse(dbl[0]);
            if (dbl[1].Length > 4) dbl[1] = dbl[1].Substring(0, 4);
            if (dbl.Length == 2) val += int.Parse(dbl[1]) / Math.Pow(10, dbl[1].Length);
            return val;
        }
        public enum colname { ticker, date, time, open, high, low, close, count };
        public enum Q { T, O, H, L, C };
        const int T = (int)Q.T, O = (int)Q.O, H = (int)Q.H, L = (int)Q.L, C = (int)Q.C;
        
        //путь по умолчанию для локальной БД
        static string StoragePath = "Dukascopy";
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
        /// /////////////////////////////
        MethodInfo ticksreceiver, barsreceiver;
        BackgroundWorker bglistener;
        Dictionary<string, double[]> quotes = new Dictionary<string, double[]>();
        Dictionary<string, int> gi = new Dictionary<string, int>(); //global int i=0; foreach SYMBOL+TF
        Dictionary<string, int> history_filled = new Dictionary<string, int>(); //0=no; 1=filled BID or ASK; 2=filled all
        Dictionary<string, bool> subscribed_symbols = new Dictionary<string, bool>();

        bool connected;

        System.Diagnostics.Process JFClient_lst;
        static string folder_path;
        //frcvr rcvr;
        void onConnect()
        {
            connected = true;
        }
        void onStop()
        {
            //bglistener.CancelAsync();
            if (JFClient_lst != null)
            {
                JFClient_lst.Dispose();
                JFClient_lst = null;
            }
            bglistener = null;
            connected = false;
            //subscribed_symbols.Clear();
        }
        public void SetTickHandler(string sym)
        {
            if (JFClient_lst != null && JFClient_lst.HasExited)
            {
                // SendMessage(JFClient_lst);
                onStop(); //JFClient_lst.Dispose(); JFClient_lst = null; bglistener = null; connected = false;
            }
            if (bglistener == null) //bglistener не активирован либо остановлен
            {
                try
                {
                    ticksreceiver = Assembly.LoadFrom("Skilful.exe").GetType("Skilful.QuotesManager.MDataManager").GetMethod("RecieveNewTick");
                    barsreceiver = Assembly.LoadFrom("Skilful.exe").GetType("Skilful.QuotesManager.MDataManager").GetMethod("RecieveNewBar");
                    bglistener = new BackgroundWorker();
                    bglistener.WorkerSupportsCancellation = true;
                    bglistener.DoWork += ticks_listener;
                    bglistener.RunWorkerCompleted += ticks_listener_completed;
                    bglistener.RunWorkerAsync(sym);
                    subscribed_symbols[sym] = true;
                    if (subscribed_symbols.Count > 1)
                    {
                        for (int cnt = 100; !connected && (cnt > 0); cnt--) Thread.Sleep(100); //wait 10 seconds
                        if (connected)
                          foreach (string key in subscribed_symbols.Keys)
                            if (key != sym)
                                JFClient_lst.StandardInput.WriteLine("add_symbol " + sym);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("dukascopy.SetTickHandler: can not load Skilful.exe");
                    SendMessage(e.Message + "(SetTickHandler)");
                }
            }
            else //bglistener уже запущен, подпишемся на новый инструмент
            {
                int cnt = 100; //wait 10 seconds
                while (!connected && (cnt-- > 0) && JFClient_lst != null && !JFClient_lst.HasExited) Thread.Sleep(100);
                try
                {
                    if (JFClient_lst == null || JFClient_lst.HasExited || !connected)
                    {
                        SendMessage(JFClient_lst);
                        onStop();  //JFClient_lst.Dispose(); JFClient_lst = null; bglistener = null; connected = false;
                        return;
                    }

                    JFClient_lst.StandardInput.WriteLine("add_symbol " + sym);
                    subscribed_symbols[sym] = true;
                    if(dbg)sw.WriteLine("Send: add_symbol " + sym);
                }
                catch (Exception e)
                {
                    SendMessage(e.Message + "(SetTickHandler)2");
                }
            }
        }

        //-------------------------------------//
        void ticks_listener(object sender, DoWorkEventArgs e)
        {
            if (ticksreceiver != null)
            {
                string[] param = folder_path.Split(',');
                char delim = '=';
                string server_url = param[0].Substring(param[0].IndexOf(delim) + 1);
                string login = param[1].Substring(param[1].IndexOf(delim) + 1);
                string password = param[2].Substring(param[2].IndexOf(delim) + 1);

                string sym = (string)e.Argument;
                JFClient_lst = new System.Diagnostics.Process();
                JFClient_lst.StartInfo.FileName = "java";
                JFClient_lst.StartInfo.Arguments = "-jar JFClient.jar " + "listen " + server_url + " " + login + " " + password + " " + sym + " 0 0 0 0";
//                JFClient_lst.StartInfo.Arguments = "-jar JFClient0.jar " + "listen " + server_url + " " + login + " " + password + " " + sym + " " + tf2ms((TF)current_frame) + " " + 1000 + " 0 0";
                JFClient_lst.StartInfo.UseShellExecute = false;
                JFClient_lst.StartInfo.RedirectStandardOutput = true;
                JFClient_lst.StartInfo.RedirectStandardInput = true;
                //JFClient_lst.StartInfo.RedirectStandardError = true;
                JFClient_lst.StartInfo.CreateNoWindow = true;
                try
                {
                    JFClient_lst.Start(); if (dbg) sw.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}: {1}", DateTime.Now, "java -jar JFClient.jar " + "listen " + server_url + " " + login + " " + password + " " + sym + " " + tf2ms(TF.m60) + " 0 0 0"));
                }
                catch(Exception ex)
                {
                    SendMessage("Can`t execute JFClient_lst.Start(" + JFClient_lst.StartInfo.FileName + " " + JFClient_lst.StartInfo.Arguments+")" + ex.Message);
                    JFClient_lst = null;
                    return;
                }

                if (JFClient_lst != null && JFClient_lst.HasExited)
                {
                    //SendMessage(JFClient_lst);
                    JFClient_lst.Dispose();
                    JFClient_lst = null;
                    return;
                }

                string input, log = "ticks_listener";
                while ((input = JFClient_lst.StandardOutput.ReadLine()) != null)
                {
                    if (dbg) sw.WriteLine("Rcv: " + input);
                    //if (input.IndexOf("no protocol:") >= 0)
                    //{
                    //    SendMessage(input);
                    //    return; //network not found
                    //}
                    if (input.IndexOf("&on") != 0)
                    {
                        log += "\n" + input;
                        continue;
                    }


                    string[] vals = input.Split(' ');
                    if (vals[0] == "&onTick:")
                    {
                        DateTime time = new DateTime(1970, 01, 01) + TimeSpan.FromMilliseconds(double.Parse(vals[1]));
                        string symbol = vals[2];
                        double bid = a2f(vals[3]);
                        double ask = a2f(vals[4]);
                        object[] args = { ModuleName, symbol, time, bid, ask };
                        ticksreceiver.Invoke(null, args);
                    }
                    else if (vals[0] == "&onBar:")
                    {
                        double ttime = double.Parse(vals[1]) / 1000;
                        string symbol = vals[2];
                        TF tf = ms2tf(vals[3]);
                        double[] bar = { ttime, a2f(vals[4]), a2f(vals[5]), a2f(vals[6]), a2f(vals[7]) };
                        object[] args = { ModuleName, symbol, tf, bar, true };
                        barsreceiver.Invoke(null, args);
                    }
                    //else if (vals[0] == "&onData:") //import data array
                    //{
                    //    string sym_tf = vals[2] + ms2tf(vals[3]).ToString();
                    //    if ((gi[sym_tf] + 5) <= quotes[sym_tf].Length)
                    //    {
                    //        quotes[sym_tf][gi[sym_tf]++] = double.Parse(vals[1]) / 1000; //c-style t_time
                    //        quotes[sym_tf][gi[sym_tf]++] = a2f(vals[4]);
                    //        quotes[sym_tf][gi[sym_tf]++] = a2f(vals[5]);
                    //        quotes[sym_tf][gi[sym_tf]++] = a2f(vals[6]);
                    //        quotes[sym_tf][gi[sym_tf]++] = a2f(vals[7]);
                    //    }
                    //}
                    else if (vals[0] == "&onData:") //import data array // BID+ASK
                    {
                        string sym_tf = vals[2] + (int)ms2tf(vals[3]) + vals[9]; //symbol.name + enum TF + BID/ASK
                        if ((gi[sym_tf] + 5) <= quotes[sym_tf].Length)
                        {
                            quotes[sym_tf][gi[sym_tf]++] = double.Parse(vals[1]) / 1000; //c-style t_time
                            quotes[sym_tf][gi[sym_tf]++] = a2f(vals[4]);
                            quotes[sym_tf][gi[sym_tf]++] = a2f(vals[5]);
                            quotes[sym_tf][gi[sym_tf]++] = a2f(vals[6]);
                            quotes[sym_tf][gi[sym_tf]++] = a2f(vals[7]);
                        }
                    }
                    else if (vals[0] == "&onProgress:") //import data array
                    {
                        if (vals[1] == "allDataLoaded")
                        {
                            string symbol = vals[4];
                            TF tf = ms2tf(vals[5]);
                            string sym_tf = symbol + (int)tf;
                            if (++history_filled[sym_tf] == 2)
                            {
                                //count
                                history_filled[sym_tf] = int.Parse(vals[3]) / 2 * 5;
                                if(vals[6]=="async"){
                                    double[] bar = new double[5];
                                    for (int i = 0; i < quotes[sym_tf + "BID"].Length; i += 5)
                                    {
                                        if(isEmptyCandle(quotes[sym_tf + "BID"], quotes[sym_tf + "ASK"], i)) continue;

                                        bar[T] = quotes[sym_tf + "BID"][i];
                                        bar[O] = (quotes[sym_tf + "ASK"][i + O] + quotes[sym_tf + "BID"][i + O]) / 2;
                                        bar[H] = quotes[sym_tf + "ASK"][i + H];
                                        bar[L] = quotes[sym_tf + "BID"][i + L];
                                        bar[C] = (quotes[sym_tf + "ASK"][i + C] + quotes[sym_tf + "BID"][i + C]) / 2;

                                        object[] args = { ModuleName, symbol, tf, bar, true };
                                        barsreceiver.Invoke(null, args);
                                    }
                                    //Array.Resize(ref quotes[sym_tf + "ASK"], 0);
                                    this.quotes.Remove(sym_tf + "ASK");
                                    this.quotes.Remove(sym_tf + "BID");
                                }
                            }
                        }
                    }
                    else if (vals[0] == "&onConnect:")
                    {
                        connected = true;
                    }
                    else if (vals[0] == "&onDisconnect:")
                    {
                        connected = false;
                    }
                    else if (vals[0] == "java.net.UnknownHostException:") // possiible bad internet connetcion or host address
                    {
                        SendMessage(vals[0] + "\nPossiible bad internet connetcion or host address: " + vals[1]);
                    }
                    else if (vals[0] == "&onConnectionFailed:")
                    {
                        SendMessage("Connection Faild,\n please checkup your connection,\n login info, or try later");
                    }
                    else if (vals[0] == "JFHistory::onStop")
                    {
                    }

                }
              // if(dbg) SendMessage("End of input stream -- while ((input = JFClient_lst.StandardOutput.ReadLine()) != null)");
                /////JFClient_lst.BeginOutputReadLine();
                if (JFClient_lst.HasExited && JFClient_lst.ExitCode != 0 && log.IndexOf("\n") > 0)
                {
                    //No Connection
                    if (log.IndexOf("java.net.UnknownHostException:") > 0 && log.IndexOf("(Unknown Source)") > 0 && JFClient_lst.ExitCode == 2)
                    {
                        if (dbg) sw.WriteLine("No internet connection");
                        //SendMessage("No internet connection");
                    }
                    else
                    {
                        log += "\n JFClient exited with code " + JFClient_lst.ExitCode;
                        SendMessage(log);
                    }
                }
                else if (JFClient_lst.HasExited && JFClient_lst.ExitCode != 0) SendMessage(JFClient_lst.ExitCode);
            }
        }
        bool isEmptyCandle(double[] BID, double[] ASK, int i)
        {
           return (BID[i + O] == BID[i + H]
                && BID[i + O] == BID[i + C]
                && BID[i + O] == BID[i + L]
                && ASK[i + O] == ASK[i + H]
                && ASK[i + O] == ASK[i + C]
                && ASK[i + O] == ASK[i + L]);
        }
        public void RemTickHandler(string sym)
        {
            if (bglistener != null && JFClient_lst != null && !JFClient_lst.HasExited)
            {
                JFClient_lst.StandardInput.WriteLine("remove_symbol " + sym);
                if (dbg) sw.WriteLine("Send: remove_symbol " + sym);
            }
        }
        //-------------------------------------//
        void ticks_listener_completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (bglistener != null)
            {
                bglistener.Dispose();
                bglistener = null;
            }
        }

        //-------------------------------------//
        const long m60  = 60 * 60 * 1000,
                   m240 = 240 * 60 * 1000,
                   day  = 24 * 60 * 60 * 1000,
                   week = 7 * 24 * 60 * 60 * 1000,
                   month= 30L * 24 * 60 * 60 * 1000,
                   year = 365240L * 24 * 60 * 60;

        //-------------------------------------//
        TF ms2tf(string msec)
        {
            switch (long.Parse(msec))
            {
                case m60: return TF.m60;
                case m240: return TF.m240;
                case day: return TF.Day;
                case week: return TF.Week;
                case year: return TF.Year;
                default: return TF.custom;
            }
        }

        //-------------------------------------//
        long tf2ms(TF tf)
        {
            switch (tf)
            {
                case TF.m60: return m60;
                case TF.m240: return m240;
                case TF.Day: return day;
                case TF.Week: return week;
                case TF.Year: return year;
                default: return -1;
            }
        }

        //-------------------------------------//
        double s2dtime(string stime)
        {
            DateTime cdate = DateTime.Parse(stime);
            DateTime nuldate = new DateTime(1970, 01, 01);
            TimeSpan ts = cdate - nuldate;
            return ts.TotalSeconds;
        }

        int taillen(long starttime, TF tf)
        {
            DateTime nuldate = new DateTime(1970, 01, 01);
            TimeSpan ts = DateTime.Now - nuldate;
            return (int)((ts.TotalMilliseconds - starttime) / tf2ms(tf));
        }

        //-------------------------------------//
        void SendMessage(System.Diagnostics.Process JFClient)
        {
            string line, text = "Connection failed (with exit code " + JFClient.ExitCode + ")";
            while ((line = JFClient.StandardOutput.ReadLine()) != null) text += "\n" + line;
            SendMessage(text);
        }

        //-------------------------------------//
        void SendMessage(string message)
        {
            if (dbg) sw.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}: {1}", DateTime.Now, "SM: " + message));
            if (libmsgreceiver != null)
            {
                string symbol = null;
                int code = 0;
                object[] args = { ModuleName, symbol, code, message };
                libmsgreceiver.Invoke(null, args);
            }
        }

        //-------------------------------------//
        void SendMessage(int code)
        {
            if (libmsgreceiver != null)
            {
                string message = null;
                switch (code)
                {
                    case 0: return;
                    case 1: //"Failed to connect Dukascopy servers"
                        message = "Failed to connect Dukascopy servers\nPlease check your network connetctions";
                        break;
                    case 2: //"Failed to login"
                        message = "Login failed, please check your login informations";
                        break;
                }
                SendMessage(message);
            }
        }

        //-------------------------------------//
        static Dictionary<string, string> last_time = new Dictionary<string, string>();
        static Dictionary<string, string> last_tick = new Dictionary<string, string>();
        enum TF{
            custom,
            m60, m240, Day, Week, Month, Quarter, Year, //набор базовых фреймов
            count,                                      //указатель ёмкости для массивов
            AllSeparate, AllSingle,                     //наряду с базовыми флаги выбора чарта для одного симовола или набора в одном или в семи разных окнах
            custom_                                     //все неперечисленные тут периоды рассматриваем как определенные пользователем
        };

        //-------------------------------------//
        long Now()
        {
            DateTime nuldate = new DateTime(1970, 01, 01);
            TimeSpan ts = DateTime.Now - nuldate;
            return (long)ts.TotalMilliseconds;
        }

        //-------------------------------------//
        const int DEFAULT_HISTORY_LENGTH =200;
        int current_frame;
        public void import(string symbol_name, int tf, int shift, int[] _len, double[] quotes)
        {
            int len = _len[0];
            string[] param = folder_path.Split(',');
            char delim = '=';
            string server_url = param[0].Substring(param[0].IndexOf(delim)+1);
            string login = param[1].Substring(param[1].IndexOf(delim)+1);
            string password = param[2].Substring(param[2].IndexOf(delim)+1);
            string log="import " + symbol_name;
            string sym_tf = symbol_name + tf.ToString();
            gi[sym_tf + "ASK"] = gi[sym_tf + "BID"] = 0;
            history_filled[sym_tf] = 0;
            //shift can be used as a startdate or shift from begin of available hostory data
            long startdate = 0;
            int quotes_length = 0;
            string mode = quotes.Length == 0 ? "async" : "sync";
            if (shift > 10000000)
            {
                startdate = shift * 1000L;
                shift = 0;
                mode = "async";

                quotes_length = 5 * (taillen(startdate, (TF)tf) + 10); //длинна хвоста для докачки +10 баров ну случай слабой связи
                if(quotes.Length > 0) Array.Resize(ref quotes, 0);
            }

            if (startdate == 0 && len == 0) len = DEFAULT_HISTORY_LENGTH;
            if (mode == "async")
            {
                this.quotes[sym_tf + "BID"] = new double[quotes_length];
            }
            else
            {
                this.quotes[sym_tf + "BID"] = quotes;
                quotes_length = quotes.Length;
            }
            this.quotes[sym_tf + "ASK"] = new double[quotes_length];


            if (JFClient_lst == null || JFClient_lst.HasExited)
            {
                current_frame = tf;
                SetTickHandler(symbol_name);
            }
            
            int cnt = 50; //wait 5 secoonds
            while (!connected && (cnt-- > 0)) Thread.Sleep(100);
            if (!connected)
            {
                if (dbg) sw.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}: {1}", DateTime.Now, "import: !connected"));
                //return;
            }

            if (JFClient_lst == null)
            {
                //SendMessage("import: JFClient_lst == null");
                if (dbg) sw.WriteLine("import: JFClient_lst == null");
                return;
            }
            if (JFClient_lst.HasExited)
            {
                //SendMessage("import: JFClient_lst.HasExited, return");
                //SendMessage(JFClient_lst);
                if (dbg) sw.WriteLine("import: JFClient_lst.HasExited, return");
                return;
            }

            //request the history//
            string request = "get_history "
                + symbol_name + " "
                + tf2ms((TF)tf) + " "
                + len +" "
                + startdate + " "
                + Now();

            try
            {
                JFClient_lst.StandardInput.WriteLine(request);
                if (dbg) sw.WriteLine("Send: " + request);
            }
            catch (Exception e)
            {
                SendMessage("can`t send get_history: JFClient_lst.StandardInput.WriteLine(request);" + e.Message);
            }
            
            //wait for load
            if (mode == "sync"){
                for (int i = 0; i < 6000; i++) //wait 30 seconds
                    if (history_filled[sym_tf] < 2 && !JFClient_lst.HasExited) Thread.Sleep(100); //needs for use local cash
                    else break;
                ///////////////////////
                if (history_filled[sym_tf] >= 2)
                {
                    int j = 0;
                    for (int i = 0; i < quotes.Length/*history_filled[sym_tf]*/; i += 5)
                    {
                        if (isEmptyCandle(quotes, this.quotes[sym_tf + "ASK"], i)) continue;
                        quotes[j + T] = quotes[i + T];
                        quotes[j + O] = (this.quotes[sym_tf + "ASK"][i + O] + quotes[i + O]) / 2;
                        quotes[j + H] = this.quotes[sym_tf + "ASK"][i + H];
                        quotes[j + L] = quotes[i + L];
                        quotes[j + C] = (this.quotes[sym_tf + "ASK"][i + C] + quotes[i + C]) / 2;
                        j+=5;
                    }
                    //Array.Resize(ref quotes, j); //совершенно без толку т.к. изменяется лишь атрибут длинны для данного указателя
                    _len[0] = j/5;
                }
                this.quotes.Remove(sym_tf + "ASK");
            }

            if (JFClient_lst.HasExited && JFClient_lst.ExitCode != 0 && log.IndexOf("\n") > 0)
            {
                log += "\n JFClient exited with code " + JFClient_lst.ExitCode;
                SendMessage(log);
            }
            else if (JFClient_lst.HasExited && JFClient_lst.ExitCode != 0) SendMessage(JFClient_lst.ExitCode);
        }
        /////////////////////////////////////////////////////////////////////////////

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
            int i = 0;
            symbols[i++] = "AUDJPY";
            symbols[i++] = "AUDNZD";
            symbols[i++] = "AUDUSD";
            symbols[i++] = "CADJPY";
            symbols[i++] = "CHFJPY";
            symbols[i++] = "EURAUD";
            symbols[i++] = "EURCAD";
            symbols[i++] = "EURCHF";
            symbols[i++] = "EURDKK";
            symbols[i++] = "EURGBP";
            symbols[i++] = "EURHKD";
            symbols[i++] = "EURJPY";
            symbols[i++] = "EURNOK";
            symbols[i++] = "EURSEK";
            symbols[i++] = "EURUSD";
            symbols[i++] = "GBPCHF";
            symbols[i++] = "GBPJPY";
            symbols[i++] = "GBPUSD";
            symbols[i++] = "NZDUSD";
            symbols[i++] = "USDCAD";
            symbols[i++] = "USDCHF";
            symbols[i++] = "USDDKK";
            symbols[i++] = "USDHKD";
            symbols[i++] = "USDJPY";
            symbols[i++] = "USDMXN";
            symbols[i++] = "USDNOK";
            symbols[i++] = "USDSEK";
            symbols[i++] = "USDSGD";
            symbols[i++] = "USDTRY";

            int _period = 60; // int.Parse(param[1].Substring(param[1].IndexOf(delim) + 1));
            for (i = 0; i < symbols.Length; i++)
            {
                len[i] = 0; //100 is random number
                period[i] = _period;
            }
        }
        public static void get_history_length(string symbol, int period, int[] len)
        {
            if (len[0] <= 0) len[0] = DEFAULT_HISTORY_LENGTH;
        }

        const int forexsymbolcount = 29;
        //get count of existing symbols
        public static void get_symbol_count(string folderpath, int[] count)
        {
            folder_path = folderpath;
            count[0] = forexsymbolcount;
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
            if (JFClient_lst != null && !JFClient_lst.HasExited)
            {
                JFClient_lst.Kill();
                JFClient_lst.WaitForExit();
            }
            if (bglistener != null) bglistener.CancelAsync();
            if (dbg) try { sw.Close(); } catch { }
        }
    }
}
