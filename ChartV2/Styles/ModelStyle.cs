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
    public class ModelStyle
    {
        public enum ModelType { MP, MDR, MR };
        public static int[,,] cl = {
            {{0xFF, 0x80, 0x00, 0x40}, {0x90, 0x80, 0x00, 0x40}}, //solid, faded
            {{0xFF, 0xFF, 0x00, 0x20}, {0x90, 0xFF, 0x00, 0x20}}, //solid, faded
            {{0xFF, 0xFF, 0x77, 0x00}, {0xA0, 0xFF, 0x64, 0x00}},
            {{0xFF, 0xFF, 0xB0, 0x00}, {0xC0, 0xE0, 0x90, 0x00}},
            {{0xFF, 0x00, 0xFF, 0x00}, {0x90, 0x00, 0xFF, 0x00}},
            {{0xFF, 0x00, 0x00, 0xFF}, {0x90, 0x00, 0x00, 0xFF}},
            {{0xFF, 0x66, 0x00, 0xAA}, {0x90, 0x66, 0x00, 0xAA}},
            {{0xFF, 0xcc, 0x00, 0xcc}, {0xC0, 0xcc, 0x00, 0xcc}}
        };
        public static Pen fadedMainLinesPen;
        public static Pen selectedMainLinesPen;
        public static Pen fadedMainDashedLinesPen;
        public static Pen fadedMainDashDotLinesPen;
        public static Pen selectedMainDashedLinesPen;
        public static Pen selectedMainDashDotLinesPen;


        public static Pen fadedAdditionalLinesPen;
        public static Pen selectedAdditionalLinesPen;
        public static Font mainLablesFont = Skilful.Sample.ModelStyle_mainLablesFont;   // new Font(FontFamily.GenericSansSerif, 10);
        public static Font additionalLablesFont = Skilful.Sample.ModelStyle_additionalLablesFont; // new Font(FontFamily.GenericSansSerif, 7);
        public static Brush mainLablesFontBrush = Brushes.Black;
        public static Brush lablesBrush = Brushes.WhiteSmoke;
        

        public static RectangleF rect ;
        public static RectangleF addRect ;
        public int step;
       
        //public Pen trendLinePen;
        //public Pen selectionPen;
        //public Pen pen;
        //public Pen addLinesPen;
        //public Pen addSelectedLinesPen;
        //public Brush fontBrush;
        //public Font font;
        //public Font addFont;
        //public RectangleF rect;
        //public RectangleF addRect;
        //public Brush rectFillBrush;


       
        /// <summary>
        /// Конструктор с одноцветными линиями
        /// </summary>
        /// <param name="pen">Ручка для отрисовки линий модели.</param>
        public ModelStyle(int StepOfModel)
        {

            step = StepOfModel;
            Styles.ModelStyle.lablesBrush = new SolidBrush(Skilful.Sample.ModelStyle_TargetLable_Color);
            Styles.ModelStyle.fadedMainLinesPen = new Pen(Color.FromArgb(100, Color.FromArgb(GlobalMembersTAmodel.cl[0 % 11])), 1F);
            Styles.ModelStyle.fadedMainLinesPen.Width = Skilful.Sample.ModelStyle_fadedMainLinesPen_Width;

            Styles.ModelStyle.fadedAdditionalLinesPen = new Pen(Color.FromArgb(100, Color.FromArgb(GlobalMembersTAmodel.cl[0 % 11])), 0.5F);
            Styles.ModelStyle.fadedAdditionalLinesPen.Width = Skilful.Sample.ModelStyle_fadedAdditionalLinesPen_Width;
          

            Styles.ModelStyle.fadedMainDashedLinesPen = new Pen(Color.FromArgb(100, Color.Red.R, Color.Red.G, Color.Red.B), 1F);
            Styles.ModelStyle.fadedMainDashedLinesPen.Width = Skilful.Sample.ModelStyle_fadedMainDashedLinesPen_Width;

            Styles.ModelStyle.fadedMainDashedLinesPen.DashPattern = new float[2] { 10, 4 };
            Styles.ModelStyle.fadedMainDashDotLinesPen = new Pen(Color.FromArgb(100, Color.Red.R, Color.Red.G, Color.Red.B), 1F);
            Styles.ModelStyle.fadedMainDashDotLinesPen.Width = Skilful.Sample.ModelStyle_fadedMainDashDotLinesPen_Width;

          Styles.ModelStyle.fadedMainDashDotLinesPen.DashPattern = new float[4] { 10, 4, 2 ,4 };
         //   Styles.ModelStyle.fadedMainDashDotLinesPen.DashPattern = new float[2] { 2, 2 };


            Styles.ModelStyle.selectedMainLinesPen = new Pen(Color.FromArgb(255, Color.FromArgb(GlobalMembersTAmodel.cl[0 % 11])), 3F);
            Styles.ModelStyle.selectedMainLinesPen.Width = Skilful.Sample.ModelStyle_selectedMainLinesPen_Width;

            Styles.ModelStyle.selectedAdditionalLinesPen = new Pen(Color.FromArgb(100, Color.FromArgb(GlobalMembersTAmodel.cl[0 % 11])), 1F);
            Styles.ModelStyle.selectedAdditionalLinesPen.Width = Skilful.Sample.ModelStyle_selectedAdditionalLinesPen_Width;

            Styles.ModelStyle.selectedMainDashedLinesPen = new Pen(Color.FromArgb(100, Color.Red.R, Color.Red.G, Color.Red.B), 2F);
            Styles.ModelStyle.selectedMainDashedLinesPen.Width = Skilful.Sample.ModelStyle_selectedMainDashedLinesPen_Width;

            Styles.ModelStyle.selectedMainDashedLinesPen.DashPattern = new float[2] { 10, 4 };
            Styles.ModelStyle.selectedMainDashDotLinesPen = new Pen(Color.FromArgb(100, Color.Red.R, Color.Red.G, Color.Red.B), 2F);
            Styles.ModelStyle.selectedMainDashDotLinesPen.Width = Skilful.Sample.ModelStyle_selectedMainDashDotLinesPen_Width;
           Styles.ModelStyle.selectedMainDashDotLinesPen.DashPattern = new float[4] { 10, 4, 2, 4 };
          //  Styles.ModelStyle.selectedMainDashDotLinesPen.DashPattern = new float[2] { 2, 2 };

        }
       
    }
}
