//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Eugeniy Bazarov(obolon)
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Skilful.Data;
using Skilful.QuotesManager;
using Skilful;
using System.IO;
using System.Threading;
using StClientLib;

namespace Skilful
{
    
    // Сервера, IT Invest
    // Основной 82.204.220.34
    // Дополнительный 213.247.232.236
    // Тестовый 89.175.35.229

    public  class ITinvest
    {
           
      public List<TBar> Quotes_List;
      private int qbar_, interval_;
      public int count_saves = 0,cod_ex = 0;
      private List<Symbol_List> symbols_;
      public StServer SmartCOM;
      public static string ip, login, password;
      public static int row_, nrows = 0;
      public string NameDir = Application.StartupPath + "\\" + "ITinvest";
      public string NameDir_Quotes = Application.StartupPath + "\\" + "ITinvest" + "\\" + "Quotes";
      public FileStream fs;
      public StreamWriter sw;
      public Load loadsimbol;
      public List<Symbol_List> symb_list_All, symb_list_MICEX, symb_list_RTS, symb_list_INDEX;
      private DateTime dt_;
     
      
       public ITinvest()
       {
           SmartCOM = new StServer();
           SmartCOM.Connected += new _IStClient_ConnectedEventHandler(SmartCOM_Connected);
           SmartCOM.AddSymbol +=new _IStClient_AddSymbolEventHandler(SmartCOM_AddSymbol);
           SmartCOM.AddBar += new _IStClient_AddBarEventHandler(SmartCOM_AddBar);
       }

       

       public void Connect(string ip, short port, string login, string password)
       {
           try
           {
               SmartCOM.connect(ip, 8090, login, password);
           }
           catch (Exception Error)
           {
               MessageBox.Show("Ошибка при подключении, " + Error.Message);
           }
       }

           
        
        public void Get_listing()
       {
           symb_list_All = new List<Symbol_List>();
           symb_list_MICEX = new List<Symbol_List>();
           symb_list_RTS = new List<Symbol_List>();
           symb_list_INDEX = new List<Symbol_List>();
           loadsimbol = new Load();
           loadsimbol.ControlBox = true;
           loadsimbol.FormBorderStyle = FormBorderStyle.FixedSingle;
           ProgressBar pb = (ProgressBar)loadsimbol.Controls["progressBar_Load"];
           pb.Minimum = 0;
           pb.Step = 1;
           loadsimbol.Show();
           try
           {
               SmartCOM.GetSymbols();
           }
           catch (Exception Error)
           {
               MessageBox.Show("Ошибка запроса списка символов, " + Error.Message);
           }
       }

       public void Save_symbols_list_to_file()
       {
           FileStream fs;
           StreamWriter sw;
           string NameDir_Symbol = NameDir + "\\" + "Symbols list";
           if (symb_list_All != null)
               if (symb_list_All.Count != 0)
               {
                   if (Directory.Exists(NameDir_Symbol)) Directory.Delete(NameDir_Symbol, true);
                   Directory.CreateDirectory(NameDir_Symbol);
                   try
                   {
                       fs = new FileStream(NameDir_Symbol + "\\" + "Symbols.txt", FileMode.Create);
                       sw = new StreamWriter(fs);
                       sw.WriteLine("<Наименование площадки>, <Код ЦБ из таблицы котировок SmartTrade (Тикер)> , <Название>, <Код типа из справочника SmartTrade>, <Лот>, <№>/<общее кол-во тикеров>.");
                       for (int i = 0; i < symb_list_All.Count; i++)
                       {

                           sw.WriteLine(symb_list_All[i].sec_exch_name + ": " + symb_list_All[i].symbol + ": " + symb_list_All[i].short_name + ", " + symb_list_All[i].type + ", " + symb_list_All[i].lot_size.ToString() + " : " + symb_list_All[i].row.ToString() + "/" + symb_list_All[i].nrows.ToString() + ".");

                       }
                       sw.Close();
                       fs.Close();
                   }
                   catch (Exception Error)
                   {
                       MessageBox.Show("Ошибка записи в файл Symbols.txt, " + Error.Message);
                   }
               }
       }
       public void Get_Data_Symbols(int interval, DateTime DT, int qbar)
       {
           if ((count_saves == 0)&&(cod_ex == 0))
                {
                 dt_ = DT;
                 qbar_ = qbar;
                 interval_ = interval;
                 symbols_ = symb_list_MICEX;
                }
            

               Quotes_List = new List<TBar>();
               if ((symbols_.Count > 1) && (count_saves < symbols_.Count))
                 {
                  if (symbols_[count_saves].checked_)
                   {
                       try
                       {
                           SmartCOM.GetBars(symbols_[count_saves].symbol, (StBarInterval)interval, DT, qbar);
                       }
                       catch (Exception Error)
                       {
                           MessageBox.Show("Ошибка при запросе данных баров, " + Error.Message);

                       }
                   }
                   else
                   {
                       count_saves++;
                       Get_Data_Symbols(interval_, dt_, qbar_);
                   }

               }
               else
               {
                   count_saves = 0;
                   if (cod_ex < 2)
                   {
                       cod_ex++;
                       if (cod_ex == 1) symbols_ = symb_list_RTS;
                       if (cod_ex == 2) symbols_ = symb_list_INDEX;
                       Get_Data_Symbols(interval_, dt_, qbar_);
                   }
                   else cod_ex = 0;
               }                         
       }

       private void Save_Quotes_to_File(List<TBar> Quotest_List, string symb, StBarInterval timefr)
       {
           FileStream fs;
           StreamWriter sw;
           try
           {
               if (Quotest_List == null) return;
               if (Quotest_List.Count == 0) return;
               if (!Directory.Exists(NameDir_Quotes)) Directory.CreateDirectory(NameDir_Quotes);
               fs = new FileStream(NameDir_Quotes + "\\" + symb + "_" + Get_Time_frime(timefr) + ".txt", FileMode.Create);
               sw = new StreamWriter(fs);
               sw.WriteLine("<DD/MM/YYYY>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>");
               for (int i = 0; i < Quotest_List.Count; i++)
                   sw.WriteLine(Convert_TBar_To_string(Quotest_List[i]));
               sw.Close();
               fs.Close();

               if (count_saves < symb_list_MICEX.Count - 1)
               {
                   count_saves++;
                   Get_Data_Symbols(interval_, dt_, qbar_);
               }
             
           }
           catch (Exception Error)
           {
               MessageBox.Show("Ошибка записи в файл  " + Error.Message);
               count_saves = 0;
               cod_ex = 0;
           }

       }

         
      public void Disconnect()
       {
          SmartCOM.disconnect();
       }

      public string Convert_TBar_To_string(TBar tb)
      {
          string dt = tb.DT.ToString(CultureInfo.CreateSpecificCulture("fr-FR"));
          string[] str = dt.Split(' ');
          str[1] = str[1].Remove(str[1].Length - 3);

          return str[0] + "," + str[1] + "," + tb.Open.ToString() + "," +
              tb.High.ToString() + "," + tb.Low.ToString() + "," + tb.Close.ToString();
      }

      

      public string Get_Time_frime(StBarInterval interv)
     {
         switch (interv)
         {
             case StBarInterval.StBarInterval_1Min: return "1 min"; break;
             case StBarInterval.StBarInterval_5Min: return "5 min"; break;
             case StBarInterval.StBarInterval_10Min: return "10 min"; break;
             case StBarInterval.StBarInterval_15Min: return "15 min"; break;
             case StBarInterval.StBarInterval_30Min: return "30 min"; break;
             case StBarInterval.StBarInterval_60Min: return "60 min"; break;
             case StBarInterval.StBarInterval_2Hour: return "120 min"; break;
             case StBarInterval.StBarInterval_4Hour: return "240 min"; break;
             case StBarInterval.StBarInterval_Day: return "Day"; break;
             case StBarInterval.StBarInterval_Week: return "Week"; break;
             case StBarInterval.StBarInterval_Month: return "Month"; break;
             case StBarInterval.StBarInterval_Quarter: return "Quarter"; break;
             case StBarInterval.StBarInterval_Year: return "Year"; break;
             default: return "";
         }
         
    }

     // ============== Обработчики событий SmartCOM
      void SmartCOM_Connected()
      {
          
          if (SmartCOM.IsConnected())
              {
                  Get_listing();
              }
              else
              {
                  MessageBox.Show("The servers disconnect, please, try connect.");
              }
          //---------------------------------------    
      }

      public void SmartCOM_AddSymbol(int row, int nrows, string symbol, string short_name, string long_name, string type, int decimals, int lot_size, double punkt, double step, string sec_ext_id, string sec_exch_name, DateTime expiry_date, double days_before_expiry)
       {
           ProgressBar pb = (ProgressBar)loadsimbol.Controls["progressBar_Load"];
           if (!SmartCOM.IsConnected())
           {
               pb.Minimum = 0;
               loadsimbol.Close();
               return;
           }
           symb_list_All.Add(new Symbol_List(row, nrows, symbol, short_name, long_name, type, decimals, lot_size, punkt, step, sec_ext_id, sec_exch_name, expiry_date, days_before_expiry));
          if (sec_exch_name == "EQ")
            symb_list_MICEX.Add(new Symbol_List(row, nrows, symbol, short_name, long_name, type, decimals, lot_size, punkt, step, sec_ext_id, sec_exch_name, expiry_date, days_before_expiry));
          if (sec_exch_name == "RTS_FUT") 
           symb_list_RTS.Add(new Symbol_List(row, nrows, symbol, short_name, long_name, type, decimals, lot_size, punkt, step, sec_ext_id, sec_exch_name, expiry_date, days_before_expiry));
          if (sec_exch_name == "RUSIDX")  
           symb_list_INDEX.Add(new Symbol_List(row, nrows, symbol, short_name, long_name, type, decimals, lot_size, punkt, step, sec_ext_id, sec_exch_name, expiry_date, days_before_expiry));


           Label lb = (Label)loadsimbol.Controls["label_Load"];
           lb.Text = "Load symbol: " + symbol;
           lb.Refresh();
           pb.Maximum = nrows;
           pb.PerformStep();
           pb.Refresh();
           if ((row + 1) == nrows)
           {
               pb.Minimum = 0;
               loadsimbol.Close();
               try
               {
                   MessageBox.Show("Load symbols: " + symb_list_All.Count.ToString() + ".  All: " + nrows.ToString() + "." + "\n" + "ММВБ: " + symb_list_MICEX.Count.ToString() + ".  PTC: " + symb_list_RTS.Count.ToString() + ".  Индексы: " + symb_list_INDEX.Count.ToString() + ".");
               }
               catch (Exception Error)
               {
                   
               }
           }
       }

    

      public void SmartCOM_AddBar(int row, int nrows, string symbol, StBarInterval interval, DateTime datetime, double open, double high, double low, double close, double volume)
       {
           
          Quotes_List.Add(new TBar(nrows, datetime, open, high, low, close));
          if ((row + 1) == nrows)
          {
             // string str = "Load bars: " + Quotes_List.Count.ToString() + ".  All: " + nrows.ToString() + ".\n" + "Save to file?";
              Save_Quotes_to_File(Quotes_List, symbol, interval);
          }
       }

      //================================
      
     
   }


    public class Symbol_List
    {
        public int row, nrows, decimals, lot_size;
        public double punkt, step, days_before_expiry;
        public string symbol, short_name, long_name, type, sec_ext_id, sec_exch_name;
        public DateTime expiry_date;
        public bool checked_ = false;
        public Symbol_List() { }
        public Symbol_List(int row_, int nrows_, string symbol_, string short_name_, string long_name_, string type_, int decimals_, int lot_size_, double punkt_, double step_, string sec_ext_id_, string sec_exch_name_, DateTime expiry_date_, double days_before_expiry_)
        {
            row = row_;
            nrows = nrows_;
            symbol = symbol_;
            short_name = short_name_;
            long_name = long_name_;
            type = type_;
            decimals = decimals_;
            lot_size = lot_size_;
            punkt = punkt_;
            step = step_;
            sec_ext_id = sec_ext_id_;
            sec_exch_name = sec_exch_name_;
            expiry_date = expiry_date_;
            days_before_expiry = days_before_expiry_;
        }
    }

}
