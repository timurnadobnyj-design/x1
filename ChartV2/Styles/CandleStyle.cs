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

namespace ChartV2.Styles
{

   public class Candle
    {

        public Brush upFill;
        public Brush donwFill;
        public Pen upBorderPen;
        public Pen downBorderPen;
       
       /// <summary>
       /// Конструктор для Черно-Белых свечей. Классика.
       /// </summary>
       public Candle()
        {
            downBorderPen = new Pen(Skilful.Sample.CandleDownBorder_Color, Skilful.Sample.CandleStyleBorderPen_Width);   //Pens.Black;
            upBorderPen = new Pen(Skilful.Sample.CandleUpBorder_Color, Skilful.Sample.CandleStyleBorderPen_Width);   //Pens.Black;
            donwFill = new SolidBrush(Skilful.Sample.CandleDownBodyFill_Color);   //Brushes.Black;
            upFill = new SolidBrush(Skilful.Sample.CandleUpBodyFill_Color);    //Brushes.White;
           
        }
       
    }
}
