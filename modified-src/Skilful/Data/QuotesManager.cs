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
using Skilful;
using Skilful.Data;
using ChartV2;
using System.Threading;
using System.Xml;
using System.IO;

namespace Skilful.QuotesManager
{
    //менеджер котировок
    public class MDataManager
    {
        public static event NewTickHandler Chart_NewTick;
        public static event NewBarHandler Chart_NewBar;
        public static event LibMsgHandler MainForm_MessageBox;
        public static event RefreshHistoryHandler RefreshHistory;

        //---------------------------------------------------------------//
        /// <summary>
        /// Обработка тиков реалтайм и передача их в графический модуль через обработчик события в маинформ
        /// </summary>
        /// <param name="SourceName">имя модуля длл</param>
        /// <param name="SymbolName"имя инструмента></param>
        /// <param name="time">дата и время</param>
        /// <param name="bid">бид</param>
        /// <param name="ask">аск</param>
        public static void RecieveNewTick(string SourceName, string SymbolName, DateTime time, double bid, double ask)
        {
            if (Chart_NewTick != null) Chart_NewTick(SourceName, SymbolName, time, bid, ask);
        }
        
        /// <summary>
        /// функция обработки баров реал тайм и передача бара в графический модуль через обработчик события в маинформ
        /// </summary>
        /// <param name="SourceName"></param>
        /// <param name="SymbolName"></param>
        /// <param name="frame"></param>
        /// <param name="bar"></param>
        public static void RecieveNewBar(string SourceName, string SymbolName, TF frame, double[] bar, bool completed)
        {
            if (frame >= TF.count) return; //фрейм не входит в набор стандартных
            TSymbol sym = getTSymbol(SourceName, SymbolName);
            if (sym != null && Chart_NewBar != null)
                for (ChartType ctype = 0; ctype < ChartType.count; ctype++)
                    for (TF tf = frame; tf < TF.count; tf++)
                    {
                        int idx = sym.NewBar(ctype, frame, tf, bar);
                        if(idx>=0)
                            Chart_NewBar(SourceName, SymbolName, tf, idx);
                    }
        }
        /// <summary>
        /// обработука сообщений от модулей датасурс
        /// </summary>
        /// <param name="SourceName"></param>
        /// <param name="SymbolName"></param>
        /// <param name="code"></param>
        /// <param name="text"></param>
        public static void LibMsgReciever(string SourceName, string SymbolName, int code, string text)
        {
            if (MainForm_MessageBox != null) MainForm_MessageBox(SourceName, SymbolName, code, text);
        }
        //---------------------------------------------------------------//

        public static void RefreshHistoryData(string SourceName, string SymbolName)
        {
            if (RefreshHistory != null) RefreshHistory(SourceName, SymbolName);
        }

        public DataSourceLibraryLoader DataSource; //SourceModules list

        //Hash<key,Hash<key,val>>Config -- SourceModuleName=>{key=>value,...}
        public Dictionary<string, Dictionary<string, string>> Config = new Dictionary<string, Dictionary<string, string>>();

        //base list of opened/calculated symbols
        static List<TSymbol> SymList = new List<TSymbol>();
        public MDataManager(NewTickHandler MainForm_Tick2Chart, NewBarHandler MainForm_Bar2Chart, LibMsgHandler MFMessageBox, RefreshHistoryHandler MFRefreshHistory)
        {
            DataSource = new DataSourceLibraryLoader();
            Config_Init();
            Chart_NewTick += MainForm_Tick2Chart;
            Chart_NewBar += MainForm_Bar2Chart;
            MainForm_MessageBox += MFMessageBox;
            RefreshHistory += MFRefreshHistory;
        }

        /// <summary>
        /// возвращает объект TSymbol ассоциированный с данными ModuleName и SymbolName
        /// </summary>
        /// <param name="ModuleName">
        /// имя модуля полученное при инициализации данной длл(функция init()) вызов происходит при инициализации SelectDataSource_Form
        /// SelectDataSource_Form::addModuleLayout::DataSource[i].init(out SDSModuleName, out SDSModulePrompt, out description, out login_type);
        /// </param>
        /// <param name="SymbolName">имя символа</param>
        /// <returns>объект TSymbol</returns>
        public TSymbol GetSymbol(DataSourseModule module, string SymbolName, bool EnableHistoryCache)
        {
            //get existing
            TSymbol sym = getTSymbol(module.name, SymbolName);
            if(sym!=null) return(sym);
            //or create new
            //version for importing data //also need version for local_stored + online data
            SymList.Add(new TSymbol(module, SymbolName, RefreshHistory, EnableHistoryCache));

            //subscribe for online data
            module.SetTickHandler(SymbolName);
            //
            //также существует функция module.RemTickHandler(SymbolName);
            //для отключения символа от реалтайм потока
            //
            return SymList.Last();
        }

        public void RemoveSymbol(TSymbol symbol)
        {
            SymList.Remove(symbol);
        }

        /// <summary>
        /// вспомогательная функция для поиска существующего объекта TSymbol
        /// </summary>
        /// <param name="ModuleName"></param>
        /// <param name="SymbolName"></param>
        /// <returns>существующий оьъект TSymbol или null</returns>
        static TSymbol getTSymbol(string ModuleName, string SymbolName)
        {
            foreach (TSymbol sym in SymList)
                if (sym.moduleName == ModuleName && sym.Name == SymbolName) return sym;
            return (TSymbol)null;
        }
        
        //---------------------------------------------------------------//
        /// <summary>
        /// чтение настроек - выбранные модули и пути - из пользовательского файла конфигурации
        /// настройки для всех модулей хранятся в виде хеша(массив[параметр=>значение,...]) преобразованного в одну строку
        /// </summary>
        public void Config_Init()
        {
            string config = XMLConfig.Get("DataSourceLibraries");
            if (config.Length == 0) return;
            //parse config string
            //string config = "modulename^checked>bool,prompt>string[;...]";
            char delim = config.IndexOf('|') > 0 ? '|' : ';';
            foreach (string a in config.Split(delim))
            {
                Dictionary<string, string> child = new Dictionary<string, string>();
                string[] b = a.Split('^');
                if (b.Length != 2) continue;
                char splitter = config.IndexOf('~') > 0 ? '~' : ',';
                foreach (string c in b[1].Split(splitter))
                {
                    string[] d = c.Split('>');
                    if (d.Length == 2) child.Add(d[0], d[1]);
                }
                Config.Add(b[0], child);
            }
        }
        //---------------------------------------------------------------//
        /// <summary>
        /// запись настроек датасурс - выбранные модули и пути - в пользовательский файл конфигурации
        /// функция вызывается при закрытии окна настроек датасурс
        /// </summary>
        public void Config_Save()
        {
            //join hash to string function
            string hash = "";
            foreach (string a in Config.Keys)
            {
                hash = hash.Length == 0 ? a : hash + "|" + a;
                string val = "";
                foreach (string b in Config[a].Keys)
                {
                    if (val.Length > 0) val += "~";
                    val += b + ">" + Config[a][b];
                }
                hash += "^" + val;
            }
            XMLConfig.Set("DataSourceLibraries", hash);
        }
        
        //---------------------------------------------------------------//
        //эта функция создана специально для  МТ4контрол. (Выдрана из Tools->SelectDataSource)
        //в остальных случаях путь устанавливается в диалоге Tools->SelectDataSource
        //
        // DataManager.SetNewDataSourceFilePath("MT4", "c:\\progra~1\\mt4\\experts\\files");
        //
        public bool SetNewDataSourceFilePath(string ModuleName, string new_filespath)
        {
            DataSourseModule mod = DataSource[ModuleName];
            if (mod != null)
            {
                mod.prompt = new_filespath;

                //save settings // 
                Dictionary<string, string> param;
                if (Config.TryGetValue(ModuleName, out param))
                {
                    param["prompt"] = new_filespath;
                    param["checked"] = "1";
                }
                else
                {
                    param = new Dictionary<string, string>();
                    param.Add("prompt", new_filespath);
                    param.Add("checked", "1");
                    Config.Add(ModuleName, param);
                }
                Config_Save();
                return true;
            }
            return false;
        }
        
        //---------------------------------------------------------------//
        /// <summary>
        /// получает список инструментов для модуля по индексу модуля в списке СелектДатаСурс
        /// </summary>
        /// <param name="i">индекс модуля датаСурс</param>
        /// <returns></returns>
        public string[] GetSymbolList(int i)
        {
            Dictionary<string, string> config ;
            string prompt = "", @checked = "";

            if (i >= 0 && i < DataSource.Count())
            {
                if (DataSource[i].name == null)
                    DataSource[i].name = "";
                    if (Config.TryGetValue(DataSource[i].name, out config))
                        if (config.TryGetValue("prompt", out prompt))
                            if (config.TryGetValue("checked", out @checked))
                                if (@checked == "1" && prompt != DataSource[i].prompt)
                                    return DataSource[i].get_symbol_list(prompt);
            }
            return new string[0];
        }
        
        //---------------------------------------------------------------//
        public DateTime GetLastTime(string SourceName, string SymbolName, TF frame,out int lastindex)
        {
            //DateTime tm;
            TSymbol sym = getTSymbol(SourceName, SymbolName);
            lastindex = 0;
            if (sym != null)
            {
                TQuotes qts = sym.Frames[(int)ChartType.Candle, (int)frame];
                if (qts != null)
                {
                    lastindex = qts.lastindex - 1;
                    return qts[qts.lastindex - 2].DT;
                }
                else
                {
                    lastindex = 0;
                    return DateTime.Now;
                }
            }
            return DateTime.Now;
        }

        public static DateTime GetLastDateTime(string SourceName, string SymbolName, TF frame, out int lastindex)
        {
            //DateTime tm;
            TSymbol sym = getTSymbol(SourceName, SymbolName);
            lastindex = 0;
            if (sym != null)
            {
                TQuotes qts = sym.Frames[(int)ChartType.Candle, (int)frame];
                if (qts != null)
                {
                    lastindex = qts.lastindex - 1;
                    return qts[qts.lastindex - 2].DT;
                }
                else
                {
                    lastindex = 0;
                    return DateTime.Now;
                }
            }
            return DateTime.Now;
        }

        //---------------------------------------------------------------//
        public void SetPipValue(string SourceName, string SymbolName, int digits)
        {
            TSymbol sym = getTSymbol(SourceName, SymbolName);
            if (sym != null)
                sym.setPipValue(digits, null);
        }
    }
}
                                                                        