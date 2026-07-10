//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Logvinenko Eugeniy (manuka)
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
using System.Drawing;
using System.Text;
using Skilful.QuotesManager;
using System.Windows.Forms;

namespace ChartV2.Data
{
   public class Series
    {
        public Legend legend;
        public List<TBar> data;
        //public TSymbol symbol;
        public TQuotes info;
        //время и цена в реальном времени
        public double lastBid, lastAsk;
        public DateTime lastTime;
        //DateTime inctime(DateTime time, TF tf)
        //{
        //    switch (tf)
        //    {
        //        case TF.m60: time = time.AddHours(1);   break;
        //        case TF.m240: time = time.AddHours(4);  break;
        //        case TF.Day: time = time.AddDays(1);    break;
        //        case TF.Week: time = time.AddDays(7);  break;
        //        case TF.Month: time = time.AddMonths(1);break;
        //        case TF.Quarter: time = time.AddMonths(3);break;
        //        case TF.Year: time = time.AddYears(1);  break;
        //    }
        //    if (time.DayOfWeek == DayOfWeek.Saturday) time = time.AddDays(2);
        //    return time;
        //}
        public int calcFrame()
        {
            if (data != null && data.Count > 1)
            {
                TimeSpan fr = data[1].DT - data[0].DT;
                for (int i = 1; i < 7 && i < (data.Count-1); i++)
                {
                    TimeSpan t = data[i + 1].DT - data[i].DT;
                    if (t < fr) fr = t;
                }
                return (int)fr.TotalMinutes;
            }
            return 0;
        }

        public DateTime barIndex2Date(int N)
        {
            DateTime last;
            int D = 0;
            float timeFactor;
            if(legend.frame==TF.custom) timeFactor= legend.customTF/60f;
            else timeFactor = TSymbol.TFtoHours[(int)legend.frame];
            if((legend.frame==TF.custom)&&(legend.customTF==0)) MessageBox.Show("Series: Ahtung! customTF=0", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (N < 0)
            {
                last = info.Quotes[0].DT;
                int dN = (int)(N * timeFactor);
                int dw;
                if ((int)legend.frame <= 2) dw = dN / 168; else dw = 0;
                D = dN - dw * 48;
            }
            else if (N < data.Count)
                //last = DateTime.ParseExact(data[N].d[0].ToString() + ChartV2.Axis_Plot.Axis.GetCheckedString((int)data[N].d[1]), "yyyyMMddHHmmss", null);
                last = data[N].DT;
            else
            {
                //last = DateTime.ParseExact(data[data.Count - 1].d[0].ToString() + ChartV2.Axis_Plot.Axis.GetCheckedString((int)data[data.Count - 1].d[1]), "yyyyMMddHHmmss", null);
                try { last = data[data.Count - 1].DT; }
                catch { last = new DateTime(); }

                int dN = (int)((N - data.Count) * timeFactor);
                int dw;
                if ((int)legend.frame <= 2) dw = dN / 168; else dw = 0;
                D = dw * 48 + dN;

            }
            return last.AddHours(D);
        }
       
        
        public void NewTick(DateTime time, double bid, double ask)
        {
            if (data.Count <= 1) return;
            lastBid = bid;
            lastAsk = ask;
            lastTime = time;
            if (legend.log)
            {
                bid = Math.Log10((double)GlobalMembersTAmodel.round((decimal)bid, info.Symbol.Decimals));
                ask = Math.Log10((double)GlobalMembersTAmodel.round((decimal)ask, info.Symbol.Decimals));
            }
            //заполнение текущего бара
            TBar bar = data[data.Count-1];
            DateTime nexttime = Gen.incTime(bar.DT, legend.frame);
            if (time < nexttime && time > bar.DT)
            {
                if (bid > bar.High) bar.High = bid;     //High
                else if (bid < bar.Low) bar.Low = bid; //Low
                bar.Close = bid;                         //Close
            }
        }


        public ViewPort viewPort;
        //public List<float> close;
        //public List<float> open;
        //public List<float> high;
        //public List<float> low;
        //public List<int> time;
        //public List<int> date;
        //public List<int> volume;
        //public List<int> oi;

       /// <summary>
       /// Старый конструктор. нужне для ВРЕМЕННОй совместимости. по переходу на класс БАР убить.
       /// </summary>
       /// <param name="selectedFields">выбранные колонки (названия сданных)</param>
       /// <param name="legend">Стили для отрисовки графиков</param>
        public Series(object[] selectedFields, Legend legend)
        {
            this.legend = legend;
            data = new List<TBar>();
            //foreach (object ob in selectedFields)
            //{
             //   switch (ob.ToString())
              //  {
            //        case "Close":
             //           close = new List<float>();
            //            break;
            //        case "Open":
            //            open = new List<float>();
            //            break;
            //        case "High":
            //            high = new List<float>();
            //            break;
            //        case "Low":
            //            low = new List<float>();
            //            break;
            //        case "Time":
            //            time = new List<int>();
            //            break;
            //        case "Date":
            //            date = new List<int>();
            //            break;
            //        case "Volume":
            //            volume = new List<int>();
            //            break;
            //        case "OI":
            //            oi = new List<int>();
            //            break;
            //        default:
            //            break;
            //    }
           // }
            



        }
        // Date-0, time - 1, open -2, high -3, low-4, close-5
        public Series(List<TBar> map, Legend legend)
        {

            this.legend = legend;
            data = map;
            int fr = calcFrame();
            if (this.legend.frame == TF.custom)
            {
                this.legend.frame = TSymbol.getTF(fr);
                if (this.legend.frame == TF.custom) this.legend.customTF = fr;
            }

            //info = new TQuotes();
            //~~~~~~~~~~~~~
            legend.log = GlobalMembersTrend.log;
            if (map.Count > 0)
            {
                int k = map.Count / 2;
                legend.price_format = map[k].High < (legend.log ? 1 : 10) ? "0.0000" : " 00.00";
                legend.pip = map[k].High < (legend.log ? 1 : 10) ? 0.0001 : 00.01;
            }
            else
            {
                legend.price_format = "0.0000";
                legend.pip = 0.0001;
            }

        }
        public Series(TQuotes qts, Color LinesColor)
        {
            info = qts;
            data = info.Quotes;
            legend = new ChartV2.Legend(qts, LinesColor);
            //~~~~~~~~~~~~~~~
            int fr = calcFrame();
            if (this.legend.frame == TF.custom)
            {
                this.legend.frame = TSymbol.getTF(fr);
                if (this.legend.frame == TF.custom) this.legend.customTF = fr;
            }

            legend.log = info.log;
            //List<TBar> map = data;
            //if (map.Count > 0)
            //{
            //    int k = map.Count / 2;
            //    legend.price_format = map[k].High < (legend.log ? 1 : 10) ? "0.0000" : " 00.00";
            //    legend.pip = map[k].High < (legend.log ? 1 : 10) ? 0.0001 : 00.01;
            //}
            //else
            //{
            //    legend.price_format = "0.0000";
            //    legend.pip = 0.0001;
            //}
            legend.price_format = "F0" + info.Symbol.Decimals;
            legend.pip = info.Symbol.pip;
        }
        ////<map2qts>
        //public Series(TQuotes quotes, Legend legend)
        //{
        //    this.legend = legend;
        //    symbol = quotes.Symbol;
        //}
        //</map2qts>

    }
}
