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
using System.Globalization;
using Microsoft.Win32;

namespace Skilful.Data
{
    enum LoginType { SelectFile, SelectDir, SetIPLoginPassword, UserDefined };
    public class DataSourceClass
    {
        static string ModuleName = "TextFiles"; //!< Название модуля
        static string ExtName = ".txt";         //!< Расширение для списка файлов
        static string RowSeparator = "";        //!< Разделитель для строк
        static char FieldSeparator = ',';       //!< Разделитель для полей
        static char DecimalSeparator = '.';     //!< Разделитель между целой и дробной частями
        //static char DateSeparator = '-';       //!< Разделитель элементов в поле дата
        //static char TimeSeparator = ':';       //!< Разделитель элементов в поле время
        static string FormatDate = "yyyy-MM-dd";  //!< формат даты
        static string FormatTime = "HH:mm:ss";    //!< формат времени
        static int FieldTickerIndex = -1;               //!< № поля TICKER в строке котировок
        static int FieldPeriodIndex = -1;               //!< № поля PERIOD в строке котировок
        static int FieldDateIndex = -1;                 //!< № поля DATE в строке котировок
        static int FieldTimeIndex = -1;                 //!< № поля TIME в строке котировок
        static int FieldOpenIndex = -1;                 //!< № поля OPEN в строке котировок
        static int FieldHighIndex = -1;                 //!< № поля HIGH в строке котировок
        static int FieldLowIndex = -1;                  //!< № поля LOW в строке котировок
        static int FieldCloseIndex = -1;                //!< № поля CLOSE в строке котировок
        static System.IO.FileSystemWatcher DirWatcher;  //!< Мониторинг каталога
        System.Diagnostics.Process MQ_imp;
        static MethodInfo RefreshHistory, barsreceiver, GetLastDateTime;

        //загрузка информации о модуле
        public static void init(char[] module_name, char[] prompt, char[] description, int[] login_type)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            ModuleName = path.Substring(path.LastIndexOf('\\')+1, path.IndexOf('.') - path.LastIndexOf('\\')-1);
            strcpy(ModuleName, module_name);
            strcpy("Options for text files", prompt);
            //useing this string as file mask
            strcpy("All files (*.*)|All files|*.*", description);
            login_type[0] = (int) LoginType.UserDefined;
            // Настройка мониторинга каталога
            DirWatcher = new System.IO.FileSystemWatcher();
            //DirWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size;
            DirWatcher.Changed += new FileSystemEventHandler(OnChangeDir);
            //DirWatcher.Created += new FileSystemEventHandler(OnChangeDir);
            DirWatcher.Renamed += new RenamedEventHandler(OnChangeDir);
            // Настройка связи со Скилфулом
            barsreceiver = Assembly.LoadFrom(path.Substring(0,path.LastIndexOf('\\')+1)+"Skilful.exe").GetType("Skilful.QuotesManager.MDataManager").GetMethod("RecieveNewBar");
            RefreshHistory = Assembly.LoadFrom(path.Substring(0, path.LastIndexOf('\\')+1) + "Skilful.exe").GetType("Skilful.QuotesManager.MDataManager").GetMethod("RefreshHistoryData");
            GetLastDateTime = Assembly.LoadFrom(path.Substring(0, path.LastIndexOf('\\')+1) + "Skilful.exe").GetType("Skilful.QuotesManager.MDataManager").GetMethod("GetLastDateTime");
        }

        //преобразование строки содержащей значение типа double в число double
        static double a2f(string s)
        {
            string[] dbl = s.Split(DecimalSeparator);
            double val = int.Parse(dbl[0]);
            if (dbl.Length == 2) val += int.Parse(dbl[1]) / Math.Pow(10, dbl[1].Length);
            return val;
        }

        enum TF { custom, m60, m240, Day, Week, Month, Quarter, Year };
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
        /// /////////////////////////////
        BackgroundWorker bglistener = null;
        static string folder_path;
        public static void GetWorkingDir(char[] path)
        {
            strcpy(folder_path,path);
        }
        public static void SetWorkingDir(char[] path)
        {
            folder_path = Convert.ToString(path); 
        }
        //System.Diagnostics.Process rtserver;
        //frcvr rcvr;
        public void SetTickHandler(string sym/*frcvr rcvr*/)
        {
        }
        /// /////////////////////////////
        public static void import(string symbol_name, int tf, int shift, int[] _len, double[] quotes)
        {
            //if (tf != 0) return;
            int len = _len[0];
            // Вычисление индекса самого последнего поля в строке
            int CountFields = Math.Max(FieldTickerIndex,
                                       Math.Max(FieldPeriodIndex,
                                                Math.Max(FieldDateIndex,
                                                     Math.Max(FieldTimeIndex,
                                                          Math.Max(FieldOpenIndex,
                                                               Math.Max(FieldHighIndex,
                                                                    Math.Max(FieldLowIndex, FieldCloseIndex)))))))+1;
                                                                             
            StreamReader sr;
            try { sr = File.OpenText(folder_path + "\\" + symbol_name + ExtName); }
            catch { return; }

            int qi = 0, cnt = 0;
            string line;
            try
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if ((qi + 5) > quotes.Length) break;
                    //проверка диапазона
                    if (shift > 0 && shift > cnt++) continue;
                    if (len > 0 && (len + shift) < cnt) break;
                    cnt++;
                    //сборка бара
                    string[] cols = line.Split(FieldSeparator);
                    if (cols.Length < CountFields) continue;
                    //проверка первого символа, если не цифра то строка пропускается
                    char firstchar = cols[FieldDateIndex].ToCharArray()[0];
                    if (firstchar < '0' || firstchar > '9') continue;

                    DateTime cdate = DateTime.ParseExact(cols[FieldDateIndex], FormatDate, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
                    DateTime ctime = new DateTime(1970, 01, 01); 

                    //time
                    if (FieldTimeIndex >= 0)
                    {
                        ctime = DateTime.ParseExact(cols[FieldTimeIndex], FormatTime, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
                    }
                    //DateTime cdate = new DateTime(year, month, day);
                    DateTime nuldate = new DateTime(1970, 01, 01);
                    TimeSpan ts = cdate - nuldate;
                    int ttime = ts.Days * 24 * 60 * 60 + ctime.Hour*60*60 + ctime.Minute*60 + ctime.Second;
                    quotes[qi] = ttime;
                    quotes[qi+1] = a2f(cols[FieldOpenIndex]);
                    quotes[qi+2] = a2f(cols[FieldHighIndex]);
                    quotes[qi+3] = a2f(cols[FieldLowIndex]);
                    quotes[qi+4] = a2f(cols[FieldCloseIndex]);
                    qi = qi + 5;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            sr.Close();
            sr.Dispose();
        }

        static void strcpy(string s, char[] d)
        {
            if (s == null) d[0] = '\0';
            else
            {
                for (int i = 0; i < s.Length; i++)
                {
                    d[i] = s[i];
                }
                d[s.Length] = '\0';
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

        //чтение и инициализация файлов
        public static void get_symbol_list(string[] symbols, int[] period, int[] len)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(folder_path);
                if (di.Exists)
                {
                    //каталог для локального кэша данных
                    StoragePath = folder_path.Substring(folder_path.LastIndexOf("\\"));

                    FileInfo[] filelist = di.GetFiles("?*" + ExtName);
                    string s;
                    for (int i = 0, j = 0; i < filelist.Length; i++)
                    {
                        string filename = filelist[i].Name;
                        StreamReader sr;
                        if (FieldTickerIndex == -1)
                        {
                            // Поле TICKER отсутствует в строке котировок
                            // Название символа определяется по имени файла
                            symbols[i] = filename.Substring(0, filename.ToLower().IndexOf(ExtName.ToLower()));
                            period[i] = 0x7FFFFFFF;
                            //period[i] = 60;
                            // Определение кол-ва котировок
                            //sr = File.OpenText(filelist[i].FullName);
                            //s = sr.ReadLine();
                            len[j] = 0;
                            //while ((s = sr.ReadLine()) != null)
                            //{
                            //    len[j]++;
                            //}
                            //sr.Close();
                            //sr.Dispose();
                            j++;
                        }
                        else
                        {
                            //get symbol`s data length
                            try
                            {
                                sr = File.OpenText(filelist[i].FullName);
                                len[j] = 0;
                                string sa = "";
                                string sb = "";
                                string[] cols;
                                // Пропуск первой строки
                                s = sr.ReadLine();
                                while ((s = sr.ReadLine()) != null)
                                {
                                    cols = s.Split(FieldSeparator);
                                    sb = cols[FieldTickerIndex];
                                    if (sa == "") sa = sb;
                                    if (s.Length > 0 && s.IndexOf(FieldSeparator) != -1)
                                            len[j]++;
                                    //get symbol name
                                    if (sb != sa)
                                    {
                                        symbols[j] = sb;
                                        //set symbol frame
                                        period[j++] = 60;
                                        sa = sb;
                                    }
                                }
                                symbols[j] = sb;
                                //period[j] = 60;
                                period[j] = 0x7FFFFFFF;
                                sr.Close();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("TextFiles" + ExtName + ".get_symbol_list: can not load" + filelist[i].FullName);
                                Console.WriteLine("The process failed: {0}", e.ToString());
                            }
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
            // Чтение настроек формата в каталоге
            DataWorkShopWindow DataSourceForm = new DataWorkShopWindow("");
            DataSourceForm.SetDirectory(folder_path);
            DataSourceForm.ReadSettings(folder_path);
            ExtName = DataSourceForm.GetExtension();
            FieldSeparator = DataSourceForm.GetFieldDelimiter();
            DecimalSeparator = DataSourceForm.GetDecimalDelimiter();
            FieldTickerIndex = DataSourceForm.GetTickerIndex();
            FieldPeriodIndex = DataSourceForm.GetPeriodIndex();
            FieldDateIndex = DataSourceForm.GetDateIndex();
            FieldTimeIndex = DataSourceForm.GetTimeIndex();
            FieldOpenIndex = DataSourceForm.GetOpenIndex();
            FieldHighIndex = DataSourceForm.GetHighIndex();
            FieldLowIndex = DataSourceForm.GetLowIndex();
            FieldCloseIndex = DataSourceForm.GetCloseIndex();
            FormatDate = DataSourceForm.GetDateFormat();
            FormatTime = DataSourceForm.GetTimeFormat();
            // Настройка мониторинга каталога
            try
            {
                DirWatcher.Path = folder_path;
                DirWatcher.Filter = "?*" + ExtName;
                DirWatcher.EnableRaisingEvents = DataSourceForm.GetWatcherStatus();
            }
            catch
            {
                
            }

            count[0] = 0;
            try
            {
                DirectoryInfo di = new DirectoryInfo(folder_path);
                if (di.Exists)
                {
                    count[0] = di.GetFiles("?*"+ExtName).Length;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        //get list of symbol name, his period and length of quote_arrays for each symbol
        public static void get_history_length(string symbol, int period, int[] len)
        {
            if (period != 0) return;
            string filepath = folder_path + "\\" + symbol + ExtName;
            len[0] = 0;
            if (File.Exists(filepath))
            {
                try
                {
                    StreamReader sr = File.OpenText(filepath);

                    len[0] = 0;
                    string s;
                    while ((s = sr.ReadLine()) != null) if (s.Length > 0) len[0]++;
                    sr.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("TextFiles.get_history_length: can not load" + filepath);
                    Console.WriteLine("The process failed: {0}", e.ToString());
                }
            }
            if (len[0] < 0) len[0] = 0;
        }



        public void get_pip_value(string sym, double[] pip)
        {
            StreamReader sr;
            string[] cols;

            try { sr = File.OpenText(folder_path + "\\" + sym + ExtName); }
            catch { return; }
            for (int i=0; i<5; i++) sr.ReadLine(); //ignore first 5 lines wich can be headers

            string s;
            int digits=0;
            for (int i = 0; i < 30 && (s = sr.ReadLine()) != null; i++)
            {
                cols = s.Split(FieldSeparator);
                cols = cols[FieldOpenIndex].Split(FieldSeparator, DecimalSeparator);
                if (cols.Length == 2 && cols[1].Length > digits) digits = cols[1].Length;
            }
            sr.Close();

            pip[0] = Math.Pow(10, -digits);
        }

        public void ShowDialog()    //! Диалог для настроек
        {
            string s="";
            DataWorkShopWindow DataSourceForm = new DataWorkShopWindow(s);
            DataSourceForm.SetDirectory(folder_path);
            DataSourceForm.ReadSettings(folder_path);
            if (DataSourceForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folder_path = DataSourceForm.GetDirectory();
                DataSourceForm.WriteSettings(folder_path);
                ExtName = DataSourceForm.GetExtension();
                FieldSeparator = DataSourceForm.GetFieldDelimiter();
                DecimalSeparator = DataSourceForm.GetDecimalDelimiter();
                FieldTickerIndex = DataSourceForm.GetTickerIndex();
                FieldPeriodIndex = DataSourceForm.GetPeriodIndex();
                FieldDateIndex = DataSourceForm.GetDateIndex();
                FieldTimeIndex = DataSourceForm.GetTimeIndex();
                FieldOpenIndex = DataSourceForm.GetOpenIndex();
                FieldHighIndex = DataSourceForm.GetHighIndex();
                FieldLowIndex = DataSourceForm.GetLowIndex();
                FieldCloseIndex = DataSourceForm.GetCloseIndex();
                //DateSeparator = DataSourceForm.GetDateDelimiter();
                //TimeSeparator = DataSourceForm.GetTimeDelimiter();
                FormatDate = DataSourceForm.GetDateFormat();
                FormatTime = DataSourceForm.GetTimeFormat();
                // Настройка мониторинга каталога
                DirWatcher.Filter = "?*" + ExtName;
            }
        }

        static bool IsProcess = false;
        private static void OnChangeDir(object source, FileSystemEventArgs e)   //! обработка события "изменение файла"
        {
            DateTime LastDateTime;

            string Symbol = e.Name.Substring(0, e.Name.ToLower().IndexOf(ExtName.ToLower()));

            if (IsProcess) return;
            IsProcess = true;
            Thread.Sleep(1000);
            // Получение № последнего бара
            object[] argsGLT = { ModuleName, Symbol, TF.custom, (int)0 };
            LastDateTime = (DateTime) GetLastDateTime.Invoke(null, argsGLT);

            int[] _len = {0};
            get_history_length(Symbol, (int)TF.custom,_len);
            if (_len[0] >= (int)argsGLT[3])
            {
                double[] quotes = new double[(_len[0] - (int)argsGLT[3]) * 5];
                import(Symbol, (int)TF.custom, (int)argsGLT[3], _len, quotes);

                for (int i = 0; i < quotes.Length; i = i + 5)
                {
                    double[] bar = { quotes[i], quotes[i + 1], quotes[i + 2], quotes[i + 3], quotes[i + 4] };
                    object[] args = { ModuleName, Symbol, TF.custom, bar, true };
                    barsreceiver.Invoke(null, args);

                }
            }
            IsProcess = false;
            //object[] args = { ModuleName, Symbol };
            //RefreshHistory.Invoke(null, args);
        }
    }
}
