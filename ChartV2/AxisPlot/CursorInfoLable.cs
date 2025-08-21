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

namespace ChartV2.Axis_Plot
{
   public class CursorInfoLable
    {
       public RectangleF rect;
       public Brush backBrush;
        public Pen borderPen;
        public Font font;
        public Brush fontBrush;
       private float vvalue;
        public float Value
        {
            get{
                NeedOffset = false;
                return vvalue;
                
            }
            set
            {
               NeedOffset = true;
               vvalue = value;
            }
        }
       private string text;
       public string Text
        {
            get{
                NeedOffset = false;
                return text;
                
            }
            set
            {
               NeedOffset = true;
               text = value;
            }
        }
       public bool NeedOffset;
        public int axisWidth;
        public int axisHeight;

        public bool IsVisible = true;
        
        public CursorInfoLable()
        {
            font = SystemFonts.CaptionFont; 
            fontBrush = Brushes.Black;
            backBrush = Brushes.WhiteSmoke;
            borderPen = Pens.Black;
            Text = "none";
        }
    }
}
