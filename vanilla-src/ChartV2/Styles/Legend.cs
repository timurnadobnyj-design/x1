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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ChartV2.Data;
using Skilful.QuotesManager;

namespace ChartV2
{
    public partial class Legend 
    {

        public Styles.Line lineStyle;
        public Styles.Bar barStyle;
        public Styles.Candle candleStyle;
        public ChartType chartType;
        public TF frame;
        public int customTF;
        public string symbol;
        public string price_format;
        public bool log;
        public double pip;
        public bool antialias = Skilful.Sample.LineChartStyle_Antialias;



        public Legend(string symbol, Color LinesColor, ChartType type, TF tf)
        {

            this.symbol = symbol;
         
            lineStyle = new Styles.Line(new Pen(LinesColor));
            candleStyle = new Styles.Candle();

            chartType = type;
            frame = tf;

        }
    
        public Legend(TQuotes quotes, Color LinesColor)
        {

            this.symbol = quotes.Symbol.Name;

            lineStyle = new Styles.Line(new Pen(LinesColor));
            candleStyle = new Styles.Candle();

            chartType = quotes.chartType;
            frame = quotes.tf;

        }
    }
}
