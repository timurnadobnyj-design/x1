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
   public class Line
    {
       private Color color;
       public Color Color
       {
           get { return color; }
           set 
           {
               pen = new Pen(value);
               color = value;
           }
       }

        public Brush ShadowBrush;
        public bool useShadow;
        public Color upColor;
        public Color downColor;
        public Pen pen;
        public LineChartType type = LineChartType.OneColorForUpDown;

        public bool smooth = Skilful.Sample.LineChartStyle_Smooth;
        public float tension = Skilful.Sample.LineChartStyle_Tension;

        public Line(Pen pen)
        {
            color = Skilful.Sample.LineChartStyle_Color;  //pen.Color;
            this.pen = new Pen(Skilful.Sample.LineChartStyle_Color, Skilful.Sample.LineChartStylePen_Width);  //pen;
            ShadowBrush = new SolidBrush(Skilful.Sample.LineChartStyleShadow_Color);
            useShadow = Skilful.Sample.LineChartStyle_UseShadow;
        }
    }
}
