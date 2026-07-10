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
    public class AxisStyle
    {
        //Visuals
        public  Brush backBrush;
        public Color backColor;
        public Pen borderPen;
        public Pen magorTickPen;
        public Pen minorTickPen;
        public Font magorTickFont;
        public Font minorTickFont;
        public Brush magorFontBrush;
        public Brush minorFontBrush;
        public Color magorTickColor;
        public Color minorTickColor;
        

        /// <summary>
        /// Конструктор стиля оси по умолчанию.
        /// </summary>
        public AxisStyle()
        {
            backColor = Color.WhiteSmoke;
            magorTickFont = new Font(FontFamily.GenericSansSerif,10,FontStyle.Regular);
            minorTickPen = new Pen(Color.Gray,1);
            magorTickPen = new Pen(Color.Black,2);
            magorFontBrush = Brushes.Black;
            
        }
        
        public AxisStyle(Color background, Brush fontBrush,FontFamily ff, int fontSize, FontStyle fs)
        {
            backColor = background;
            magorTickFont = new Font(ff, fontSize, fs);
            minorTickPen = new Pen(Color.Gray, 1);
            magorTickPen = new Pen(Color.Black, 2);
            magorFontBrush = fontBrush;

        }
    }
}
