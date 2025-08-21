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
using System.Drawing.Drawing2D;
using System.Collections;
using System.Runtime.Serialization;
using ChartV2.Styles;

namespace ChartV2.Axis_Plot
{

    public class Plot : DrawingFunctions
    {

        //переменнаая создана, чтобы не создавать новый Инт при каждой отрисовки.
        //в целях экономии.
        private int seriesIndex = 0;
        public PlotStyle style;
        public bool IsModelsVisible = true;
        public bool IsHTVisible = true;
        public bool IsCopyRightVisible = true;
        public Font copyRightFont = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
        


        public Plot()
        {

        }

        public Plot(Rectangle gridSplitterRectangle)
        {
            refreshDrawingSurfaces(gridSplitterRectangle);
            style = new PlotStyle();
        }



        public void Draw()
        {
            if (seriesToDrawList != null)
            {
                //Рисуем все серии в list[]
                g.Clear(style.backColor);

                for (; seriesIndex < seriesToDrawList.Count; seriesIndex++)
                {


                    switch (seriesToDrawList[seriesIndex].legend.chartType)
                    {
                        case (ChartType.Line):

                            if (style.IsAutoMargin)
                                DrawLineChartWithAutoMargins(seriesIndex);
                            else
                                DrawLineChart(seriesIndex);
                            break;

                        case (ChartType.Candle):
                            if (style.IsAutoMargin)
                                DrawCandleChartWithAutoMargins(seriesIndex);
                            else
                                DrawCandleChart(seriesIndex);

                            break;
                    }
                }
                // обнуляю индекс серии т.к. увеличивал его для прохода по циклу!!!
                seriesIndex = 0;

                //использую seriesIndex для прохода по графическим моделям, чтобы не создавать лишних переменных
                // Рисуем графические инструменты: МОдели, Ганн, Фибо и т.д.
                if (modelsToDrawList != null)
                {
                   if (IsModelsVisible)
                        DrawModels();
                }

               if (HTtoDrawList != null)
                {
                    if ((IsHTVisible)&&(Skilful.MainForm.ShowHypotheticalTarget))
                       DrawHTLines();
                    
                }

                if (graphToolsToDrawList!=null)
                {
                    DrawGraphicalTools();
                }

                if (IsCopyRightVisible)
                {
                    
                   //Brush Write_Skilful_Brush = new SolidBrush(Skilful.Sample.Write_Skilful_Font_Color);
                    g.DrawString(" © Skilful " + DateTime.Now.Year.ToString(), Skilful.Sample.Write_Skilful_Font, new SolidBrush(Skilful.Sample.Write_Skilful_Font_Color), 0, bmp.Height - 10 - Skilful.Sample.Write_Skilful_Font.Size); //copyRightFont
                    
                }
            }

            bmpCopyforCrossCursor = bmp;
           
        }




    }
}
