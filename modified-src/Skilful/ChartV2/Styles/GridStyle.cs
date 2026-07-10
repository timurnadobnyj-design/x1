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
    public class GridStyle
    {
        public Pen minorVerticalPen = null;
        public Pen minorHorizontalPen = null;
        public Pen majorVerticalPen = null;
        public Pen majorHorizontalPen = null;


        public Color verticalPenColor;
        public Color horizontalPenColor;
        public int verticalPenWidth = 1;
        public int horizontalPenWidth = 1;
        //длинна точки
        public int minH1 = 2;
        //расстояние между точками
        public int minH2 = 10;
        
        public int minV1 = 2;
        public int minV2 = 10;


        public int majH1 = 2;
        public int majH2 = 5;

        public int majV1 = 2;
        public int majV2 = 5;
        public bool IsVerticalVisible = true;
        public bool IsHorizontalVisible = true;

        public GridStyle()
        {
            verticalPenColor = Skilful.Sample.Grid_Color;
            horizontalPenColor = Skilful.Sample.Grid_Color;

            minorVerticalPen = new Pen(verticalPenColor, verticalPenWidth);

            minorHorizontalPen = new Pen(horizontalPenColor, horizontalPenWidth);
            majorHorizontalPen = new Pen(horizontalPenColor, horizontalPenWidth);

            minorVerticalPen.DashPattern = new float[2] { minV1, minV2 };
            minorHorizontalPen.DashPattern = new float[2] { minH1, minH2 };

            // majorVerticalPen.DashPattern = new float[2] { majV1, majV2 };
            majorHorizontalPen.DashPattern = new float[2] { majH1, majH2, };
        }
    }
}
