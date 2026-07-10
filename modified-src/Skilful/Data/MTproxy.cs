//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Isabek Satybekov
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
using System.IO;
using Skilful;
using System.Threading;

namespace Skilful.Data
{
    public class MTproxy
    {
        private string _sklfile = string.Empty;
        private const string _sklfilename = "toget.skl";
        private DateTime lastupdate = DateTime.Today.AddDays(-10.0);
        private string file_delimiter = "'";

        public int wndHandle;
        public List<MTSymbol> mtSymbols = new List<MTSymbol>();
        public int MaxCode = -1;
        StreamWriter sw;
        public bool dbg = false;

        public Dictionary<string, int> Digits = new Dictionary<string, int>();

        public MTproxy()
        {
            if (XMLConfig.Get("MT4Path").Length > 0)
                _sklfile = XMLConfig.Get("MT4Path") + "\\experts\\files\\" + _sklfilename;
            //продумать если mt4path не установлен
        }

        public void WriteWndHandle(int hWnd)
        {
            if (hWnd != 0) wndHandle = hWnd;
            if (XMLConfig.Get("MT4Path").Length >= 2)
            {
                try
                {
                    string path = XMLConfig.Get("MT4Path");
                    sw = new StreamWriter(XMLConfig.Get("MT4Path") + "\\experts\\files\\hWnd.txt");
                    sw.WriteLine(wndHandle.ToString());
                    sw.Dispose();
                }
                catch (Exception e)
                {
                    if(sw != null) sw.Dispose();
                    MessageBox.Show(e.Message);
                }
            }
        }

        public void getDigits()
        {
            if (XMLConfig.Get("MT4SymbolsPath").Length == 0)
            {
                if (XMLConfig.Get("symbolsPath").Length == 0) return;
                XMLConfig.Set("MT4SymbolsPath", XMLConfig.Get("symbolsPath"));
            }
            //if(Properties.Settings.Default.symbolsPath.Length==0) return;
            int symbolOffset = 116;
            int symbolRecordLength = 128;
            int symbolLength = 12;
            FileStream fStream = new FileStream(XMLConfig.Get("MT4SymbolsPath"), FileMode.Open, FileAccess.Read);
            BinaryReader fReader = new BinaryReader(fStream);
            int symbolsNumber = (((int)fStream.Length) - 4) / symbolRecordLength;
            fReader.ReadBytes(4);
            for (int i = 0; i < symbolsNumber; i++)
            {
                char[] sym = fReader.ReadChars(symbolLength);
                byte[] digits = fReader.ReadBytes(symbolOffset);
                string symbol = "";
                for (int j = 0; sym[j] != 0 && j < sym.Length; j++) symbol += sym[j];
                int digit = digits[0];
                //проверим является ли инструмент валютной парой, и если да, то подкорректируем точность цены до 2-х/4-х знаков.
                //признак того что это валюта = формат символа АААБББ + digit == 3 || digit == 5
                if (symbol.Length == 6 && digit == 3 || digit == 5)
                {
                    bool superdigit = true;
                    for (int c = 0; c < 6; c++) if (sym[c] < 'A' || sym[c] > 'Z') superdigit = false;
                    if (superdigit) digit--;
                }
                Digits[symbol] = digit;
            }
            fReader.Close();
            fStream.Close();
        }

        public void mergeUpdatetoHistoryFile(int symbolcode, int period)
        {
            mergeUpdatetoHistoryFile(getSymbolFromCode(symbolcode), period);
        }

        public void mergeUpdatetoHistoryFile(string symbol, int period)
        {
            if(dbg) Logging.Log("mergeUpdatetoHistoryFile: " + symbol + " " + period);
            List<string> history = new List<string>();
            List<string> update = new List<string>();
            List<string> full = new List<string>();
            string _ext = ".csv";
            string _dir = Path.GetDirectoryName(_sklfile);
            string fullfile = string.Empty;
            string lastDateTime = string.Empty;
            int symbolcode = getCodeFromSymbol(symbol);
            
            //MessageBox.Show(symbol + " - " + period);
            
            if (Directory.Exists(_dir))
            {
                string _history = _dir + "\\" + symbol + file_delimiter + period + file_delimiter + "h" + file_delimiter + symbolcode.ToString();
                string _update = _dir + "\\" + symbol + file_delimiter + period + file_delimiter + "u" + file_delimiter + symbolcode.ToString();
                
                if (File.Exists(_history))
                {
                    history = this.loadFile(_history);
                }
                if (File.Exists(_update))
                {
                    update = this.loadFile(_update);
                    full = merge2files(history, update);
                }
                else
                {
                    full = history;
                }
                fullfile = _dir + "\\" + symbol + period + _ext;
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        sw = new StreamWriter(fullfile);
                        foreach (string str in full)
                            sw.WriteLine(str);
                        sw.Dispose();
                        break;
                    }
                    catch (Exception e)
                    {
                        sw.Dispose();
                        if (dbg) Logging.Log("mergeUpdatetoHistoryFile: Cannot write " + fullfile + "\n" + e.Message);
                        //  MessageBox.Show("mergeUpdatetoHistoryFile: Cannot write " + fullfile + "\n" + e.ToString());
                    }
                    Thread.Sleep(100);
                }
            }
        }

        public void mt4init()
        {
           if (XMLConfig.Get("MT4Path").Length == 0) return;
            _sklfile = XMLConfig.Get("MT4Path") + "\\experts\\files\\" + _sklfilename;
            mtSymbols = loadSkl(_sklfile);
            MaxCode = -1;
            foreach (MTSymbol symbol in mtSymbols)
            {
                if ((MaxCode) < (symbol.Code))
                {
                    MaxCode = symbol.Code;
                }
            }
        }

        private List<string> loadFile(string fileName)
        {
            if (dbg) Logging.Log("loadFile: " + fileName);

            List<string> prn = new List<string>();
            string line = string.Empty;
            int i;
            for (i = 0; i < 15; i++)
            {
                try
                {
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs);
                    while ((line = sr.ReadLine()) != null)
                    {
                        prn.Add(line);  
                        
                    }
                    sr.Close();
                    fs.Close();
                    break;
                }
                catch(Exception e)
                {
                    if (dbg) Logging.Log("Cannot read " + fileName + "\n" + e.Message);
                    Thread.Sleep(100);
                }
            }

            if (i == 15)
            {
                if (dbg) Logging.Log("LoadFile:" + fileName + " can't be read");
                MessageBox.Show("LoadFile:" + fileName + " can't be read", "Skilful", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            

            return prn;
        }

        private List<string> merge2files(List<string> history, List<string> update)
        {
            int firstline = 0;
            if (update.Count > 0)
            {
                string[] cols = update[firstline].Split(',');
                if (cols.Length >= 2)
                {
                    string updateFirstDateTime = cols[0] + cols[1];
                    int index = -1;
                    string temp = string.Empty;

                    for (int i = history.Count - 1; i >= firstline; i--)
                    {
                        temp = history[i].Split(',')[0] + history[i].Split(',')[1];
                        if (temp == updateFirstDateTime)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index >= 0)
                    {
                        history.RemoveRange(index, history.Count - index);
                    }

                    history.AddRange(update.GetRange(firstline, update.Count - firstline));
                }
            }
            return history;
        }

        public void loadHistoryFile(int symbolcode, int period)
        {
            if (dbg) Logging.Log("loadHistoryFile: " + symbolcode + " " + period);

            List<string> history = new List<string>();
            List<string> update = new List<string>();
            List<string> full = new List<string>();
            string symbol = string.Empty;
            string _ext = ".csv";
            string _dir = Path.GetDirectoryName(_sklfile);
            string fullfile = string.Empty;
            symbol = getSymbolFromCode(_dir, symbolcode);
            
            if (Directory.Exists(_dir))
            {
                string _history = _dir + "\\" + symbol + file_delimiter + period + file_delimiter + "h" + file_delimiter + symbolcode.ToString();
                //MessageBox.Show(_history);
                
                if (File.Exists(_history))
                {
                    history = this.loadFile(_history);
                    full = history;
                }
                fullfile = _dir + "\\" + symbol + period + _ext;

                try
                {
                    sw = new StreamWriter(fullfile);
                    foreach (string str in full)
                        sw.WriteLine(str);
                    sw.Dispose();
                }
                catch (Exception e)
                {
                    sw.Dispose(); 
                    if (dbg) Logging.Log("loadHistoryFile: Cannot write " + fullfile + "\n" + e.Message);
                    MessageBox.Show("loadHistoryFile: Cannot write " + fullfile + "\n" + e.Message);
                }

            }
            
        }

        public string getSymbolFromCode(int code)
        {
            foreach (MTSymbol sym in mtSymbols)
            {
                if (sym.Code == code) return sym.Symbol;
            }
            return null;
        }
        public int getCodeFromSymbol(string name)
        {
            foreach (MTSymbol sym in mtSymbols)
            {
                if (sym.Symbol == name) return sym.Code;
            }
            return 0;
        }

	    public string getSymbolFromCode(string filepath, int code)
        {
            string[] splitted;
            string line;
            string symbol = string.Empty;

            if (Directory.Exists(filepath))
            {
                List<string> files = Directory.GetFiles(filepath).ToList();
                foreach (string file in files)
                {
                    line = file;
                    if (line.Contains(file_delimiter))
                    {
                        line = Path.GetFileNameWithoutExtension(file);
                        splitted = line.Split(file_delimiter.ToCharArray()[0]);
                        if (splitted.Count() == 4 && splitted[2] == "h" && splitted[3] == code.ToString())
                        {
                            symbol = splitted[0];
                            break;
                        }
                    }
                }
            }
            return symbol;
        }

        public bool doesContains(List<MTSymbol> slist, string symbol)
        {
            foreach (MTSymbol mtsymbol in slist)
            {
                if (mtsymbol.Symbol == symbol) return true;
            }
            return false;
        }

        private List<MTSymbol> loadSkl(string fileName)
        {
            List<MTSymbol> mts = new List<MTSymbol>();
            MTSymbol temp = new MTSymbol();
            string line = string.Empty;
            string symbol = string.Empty;
            int code = -1;

            if (File.Exists(fileName) == false)
            {
                try
                {
                    sw = new StreamWriter(fileName);
                    sw.WriteLine("EURUSD|1");
                    sw.Close();
                }
                catch (Exception e)
                {
                    if (sw != null) sw.Dispose();
                    if (dbg) Logging.Log("loadSkl: Cannot write " + fileName + "\n" + e.Message);
                    MessageBox.Show("loadSkl: Cannot write " + fileName + "\n" + e.Message);
                }
            }

            using (StreamReader sr = new StreamReader(fileName))
            {

                while ((line = sr.ReadLine()) != null)
                {
                    symbol = line.Split('|')[0];
                    code = int.Parse(line.Split('|')[1]);
                    //MessageBox.Show(symbol + " = " + code);
                    temp = new MTSymbol(symbol, DateTime.Today.AddYears(-10), code);
                    mts.Add(temp);
                }
            }

            return mts;
        }
        //---------------------------------------------------------------//
        public string StoragePath()
        {
            string mt4path = XMLConfig.Get("MT4Path");
            return mt4path.Length > 0 ? mt4path.Substring(XMLConfig.Get("MT4Path").LastIndexOf(Path.DirectorySeparatorChar) + 1) + Path.DirectorySeparatorChar + XMLConfig.Get("MT4account") : mt4path;
        }
    }

    public class MTSymbol
    {
        private string _symbol;
        private DateTime _lastDateTime;
        private int _code;

        public string Symbol
        {
            get
            {
                return _symbol;
            }
        }
        
        public int Code
        {
            get
            {
                return _code;
            }
        }

        public MTSymbol(string symbol, DateTime datetime, int code)
        {
            this._symbol = symbol;
            this._lastDateTime = datetime;
            this._code = code;
        }

        public MTSymbol()
        {
            this._symbol = "";
            this._lastDateTime = DateTime.Today.AddYears(-10);
            this._code = -1;
        }

    }
    
}
