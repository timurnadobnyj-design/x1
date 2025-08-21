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
using System.Collections;
using Skilful;
using Skilful.QuotesManager;
using Skilful.ModelManager;

namespace ChartV2.Axis_Plot
{
    public abstract class DrawingFunctions
    {
        //tepm vars for calculation
        private PointF pt1;
        private PointF pt2;
        private PointF pt3;

        private PointF lablepoint;

        private RectangleF tempRect;
        private float radiusTemp;
        private double angleTemp;
        //for candels
        PointF openPx = PointF.Empty;
        PointF closePx = PointF.Empty;
        float highPx = 0;
        float lowPx = 0;
        //float tempMax = 0;
        //float tempMin = 0;

        public float barWidthInPixel;
        //
        public List<Data.Series> seriesToDrawList;
        public List<Data.UserLine> graphToolsToDrawList;
        public List<Model> modelsToDrawList;
        public List<HTargetLine> HTtoDrawList;
        public Axis_Plot.Axis axisForModelLables;
        public Axis_Plot.Axis topAxis;
        public Bitmap bmp;
        public Bitmap bmpCopyforCrossCursor;
        //private bool IsPt1InsideViewPort = true;
        internal Graphics g;
        public Grid grid;


        /// <summary>
        /// Изменяет размер Битмапа для отрисовки. 
        /// Должна использоваться при изменении размера клетки, отведенной для области графика.
        /// </summary>
        /// <param name="viewPortRect"> Прямоугольник из Контрола GRIDSPLITTER</param>
        internal void refreshDrawingSurfaces(Rectangle gridSplitterRectangle)
        {
            if (gridSplitterRectangle.Width > 0 && gridSplitterRectangle.Height > 0)
            {
                bmp = new Bitmap(gridSplitterRectangle.Width, gridSplitterRectangle.Height);
                g = Graphics.FromImage(bmp);

            }
            else
            {
                bmp = new Bitmap(10, 10);
                g = Graphics.FromImage(bmp);
            }
        }
        /// <summary>
        /// Функция отрисовки Грид(решетки) данные расчитываюстя в классах Axis
        /// </summary>
        internal void DrawGrid(int seriesIndex)
        {
            if (grid.style.IsHorizontalVisible)
            {
                
                for (int i = 0; i < grid.horizontalLevelsArray.Count; i++)
                {
                    
                    if (!float.IsInfinity(grid.horizontalLevelsArray[i])) 
                        g.DrawLine(grid.style.minorHorizontalPen, 0F, grid.horizontalLevelsArray[i], bmp.Width, grid.horizontalLevelsArray[i]);
                }
            }
            if (grid.style.IsVerticalVisible)
            {
                for (int i = 0; i < grid.verticalLevelsArray.Count; i++)
                {
                    if (!float.IsInfinity(grid.verticalLevelsArray[i])) 
                    g.DrawLine(grid.style.minorVerticalPen, grid.verticalLevelsArray[i], 0, grid.verticalLevelsArray[i], bmp.Height);
                }
            }
         //   g.SmoothingMode = g.SmoothingMode = legend.antialias ? System.Drawing.Drawing2D.SmoothingMode.AntiAlias : System.Drawing.Drawing2D.SmoothingMode.None;
        }
    


        internal void DrawLineChart(int i)
        {
            seriesToDrawList[i].viewPort.PrepareTransformFunctions(bmp);
            Legend legend = seriesToDrawList[i].legend;
            g.SmoothingMode = legend.antialias ? System.Drawing.Drawing2D.SmoothingMode.AntiAlias : System.Drawing.Drawing2D.SmoothingMode.None;

            if (seriesToDrawList[i].viewPort.VPMaxPrice >= seriesToDrawList[i].viewPort.absRealPriceMin
            && seriesToDrawList[i].viewPort.VPMinPrice <= seriesToDrawList[i].viewPort.absRealPriceMax)
            {
                int iMax = (int)Math.Min(seriesToDrawList[i].viewPort.VPMaxTime + 2, seriesToDrawList[i].data.Count);
                int iMin = (int)Math.Max(seriesToDrawList[i].viewPort.VPMinTime, 2);

                PointF[] pts = new PointF[10 + iMax - iMin];

                List<TBar> data = seriesToDrawList[i].data;
                for (int p = 10, j = iMin; j < iMax; j++, p++)
                {
                    pts[p] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(j, (float)data[j].Close));
                }
                if (legend.lineStyle.useShadow)
                {
                    //дополнительные точки для создания прямых углов на краях чарта
                    int u=0;
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMax - 1, (float)data[iMax - 1].Close));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMax - 1, (float)data[iMax - 1].Close));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMax - 1, seriesToDrawList[i].viewPort.VPMinPrice));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMax - 1, seriesToDrawList[i].viewPort.VPMinPrice));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMax - 2, seriesToDrawList[i].viewPort.VPMinPrice));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMin + 1, seriesToDrawList[i].viewPort.VPMinPrice));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMin, seriesToDrawList[i].viewPort.VPMinPrice));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMin, seriesToDrawList[i].viewPort.VPMinPrice));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMin, (float)data[iMin].Close));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMin, (float)data[iMin].Close));

                    pts[0].Y += 1;
                    pts[0].X += 1;
                    pts[1].Y += 2;
                    pts[1].X += 1;
                    
                    pts[8].Y += 1;
                    pts[8].X -= 1;
                    pts[9].Y += 1;
                    pts[9].X -= 1;
                    //g.FillPolygon(seriesToDrawList[i].legend.lineStyle.ShadowBrush, pts);
                    g.FillClosedCurve(legend.lineStyle.ShadowBrush, pts, System.Drawing.Drawing2D.FillMode.Winding, legend.lineStyle.smooth ? legend.lineStyle.tension : 0);
                }
                pts[0] = pts[1] = pts[2] = pts[3] = pts[4] = pts[5] = pts[6] = pts[7] = pts[8] = pts[9] = pts[10];
                g.DrawCurve(legend.lineStyle.pen, pts, legend.lineStyle.smooth ? legend.lineStyle.tension : 0);
            }
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            if (grid.IsVisible) DrawGrid(i);
        }

        internal void DrawLineChartWithAutoMargins(int i)
        {
            seriesToDrawList[i].viewPort.PrepareTransformFunctionsAutoMargins(bmp);
            Legend legend = seriesToDrawList[i].legend;
            g.SmoothingMode = legend.antialias ? System.Drawing.Drawing2D.SmoothingMode.AntiAlias : System.Drawing.Drawing2D.SmoothingMode.None;

            if (seriesToDrawList[i].viewPort.VPMaxPrice >= seriesToDrawList[i].viewPort.absRealPriceMin
             && seriesToDrawList[i].viewPort.VPMinPrice <= seriesToDrawList[i].viewPort.absRealPriceMax)
            {
                int iMin = (int)seriesToDrawList[i].viewPort.stratTime;
                int iMax = (int)seriesToDrawList[i].viewPort.finishTime;

                PointF[] pts = new PointF[10 + iMax - iMin];
                List<TBar> data = seriesToDrawList[i].data;
                for (int p = 10, j = iMin; j < iMax; j++, p++)
                {
                    pts[p] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(j, (float)data[j].Close));
                }
                if (legend.lineStyle.useShadow)
                {
                    //дополнительные точки для создания прямых углов на краях чарта
                    int u = 0;
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMax - 1, (float)data[iMax - 1].Close));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMax - 1, (float)data[iMax - 1].Close));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMax - 1, seriesToDrawList[i].viewPort.VPMinPrice));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMax - 1, seriesToDrawList[i].viewPort.VPMinPrice));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMax - 2, seriesToDrawList[i].viewPort.VPMinPrice));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMin + 1, seriesToDrawList[i].viewPort.VPMinPrice));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMin, seriesToDrawList[i].viewPort.VPMinPrice));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMin, seriesToDrawList[i].viewPort.VPMinPrice));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMin, (float)data[iMin].Close));
                    pts[u++] = seriesToDrawList[i].viewPort.TransformToPixels(new PointF(iMin, (float)data[iMin].Close));

                    pts[0].Y += 1;
                    pts[0].X += 1;
                    pts[1].Y += 2;
                    pts[1].X += 1;

                    pts[8].Y += 1;
                    pts[8].X -= 1;
                    pts[9].Y += 1;
                    pts[9].X -= 1;
                    //g.FillPolygon(seriesToDrawList[i].legend.lineStyle.ShadowBrush, pts);
                    g.FillClosedCurve(legend.lineStyle.ShadowBrush, pts, System.Drawing.Drawing2D.FillMode.Winding, legend.lineStyle.smooth ? legend.lineStyle.tension : 0);
                }

                pts[0] = pts[1] = pts[2] = pts[3] = pts[4] = pts[5] = pts[6] = pts[7] = pts[8] = pts[9] = pts[10];
                g.DrawCurve(legend.lineStyle.pen, pts, legend.lineStyle.smooth ? legend.lineStyle.tension : 0);

            }
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            if (grid.IsVisible) DrawGrid(i);
        }


        internal void DrawCandleChart(int seriesIndex)
        {
            if (grid.IsVisible)
                DrawGrid(seriesIndex);
            #region CandleStyle

            seriesToDrawList[seriesIndex].viewPort.PrepareTransformFunctions(bmp);
            

            if (seriesToDrawList[seriesIndex].viewPort.VPMaxPrice < seriesToDrawList[seriesIndex].viewPort.absRealPriceMin)
                goto passTheCicle;
            if (seriesToDrawList[seriesIndex].viewPort.VPMinPrice > seriesToDrawList[seriesIndex].viewPort.absRealPriceMax)
                goto passTheCicle;



             barWidthInPixel = seriesToDrawList[seriesIndex].viewPort.BarNumberToPixels(1) - seriesToDrawList[seriesIndex].viewPort.BarNumberToPixels(0);

            if (barWidthInPixel < 0)
                barWidthInPixel = 1;
            if (barWidthInPixel < 3)
            {
             // рисуем линии от ХАй до ЛОУ вместо свечей ибо не влезают в экран. 
                openPx = PointF.Empty;
                closePx = PointF.Empty;
                for (int i = seriesToDrawList[seriesIndex].viewPort.stratTime; i < seriesToDrawList[seriesIndex].viewPort.finishTime; i++)
                {
                   // Type type = list[seriesIndex].high[i].GetType();

                    highPx = seriesToDrawList[seriesIndex].viewPort.PriceToPixels((float)seriesToDrawList[seriesIndex].data[i].High);
                    lowPx = seriesToDrawList[seriesIndex].viewPort.PriceToPixels((float)seriesToDrawList[seriesIndex].data[i].Low);

                    openPx.Y = (float)seriesToDrawList[seriesIndex].data[i].Open;
                    openPx.X = i;
                    closePx.Y = (float)seriesToDrawList[seriesIndex].data[i].Close;
                    closePx.X = i;

                    openPx = seriesToDrawList[seriesIndex].viewPort.TransformToPixels(openPx);
                    closePx = seriesToDrawList[seriesIndex].viewPort.TransformToPixels(closePx);

                    if (closePx.Y - openPx.Y != 0)
                    {
                        if (closePx.Y - openPx.Y > 0)
                        {
                            g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.downBorderPen, openPx.X, highPx, openPx.X, lowPx);
                        }
                        else
                        {

                            g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, openPx.X, lowPx, openPx.X, highPx);
                        }
                    }
                    else
                    {
                        g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, openPx.X, lowPx, openPx.X, highPx);
                        if (lowPx - highPx == 0)
                            g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, openPx.X, lowPx, openPx.X, highPx + 1);

                    }
                }

            }
            else
            {
                // рисуем свечи полностью ибо влезают в экран
                barWidthInPixel /= 1.5F;
                openPx = PointF.Empty;
                closePx = PointF.Empty;
                for (int i = seriesToDrawList[seriesIndex].viewPort.stratTime; i < seriesToDrawList[seriesIndex].viewPort.finishTime; i++)
                {

                    //openPx = ds.TransformToPixels("Y", list[seriesIndex].open[i]);
                    //closePx = ds.TransformToPixels("Y", list[seriesIndex].close[i]);


                    openPx.Y = (float)seriesToDrawList[seriesIndex].data[i].Open;
                    openPx.X = i;
                    closePx.Y = (float)seriesToDrawList[seriesIndex].data[i].Close;
                    closePx.X = i;   
                    
                    highPx = seriesToDrawList[seriesIndex].viewPort.PriceToPixels((float)seriesToDrawList[seriesIndex].data[i].High);
                    lowPx = seriesToDrawList[seriesIndex].viewPort.PriceToPixels((float)seriesToDrawList[seriesIndex].data[i].Low);
                    
                    

                    if (closePx.Y - openPx.Y != 0)
                    {
                       
                        if (openPx.Y > closePx.Y)
                        {
                            openPx = seriesToDrawList[seriesIndex].viewPort.TransformToPixels(openPx);
                            closePx = seriesToDrawList[seriesIndex].viewPort.TransformToPixels(closePx);
                            if (closePx.Y == openPx.Y)
                                closePx.Y++;

                            g.FillRectangle(seriesToDrawList[seriesIndex].legend.candleStyle.donwFill, openPx.X - barWidthInPixel / 2, openPx.Y, barWidthInPixel, closePx.Y - openPx.Y);
                            g.DrawRectangle(seriesToDrawList[seriesIndex].legend.candleStyle.downBorderPen, openPx.X - barWidthInPixel / 2, openPx.Y, barWidthInPixel, closePx.Y - openPx.Y);

                            if (seriesToDrawList[seriesIndex].data[i].High != seriesToDrawList[seriesIndex].data[i].Close)
                                g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.downBorderPen, openPx.X, highPx, closePx.X, openPx.Y);
                            if (seriesToDrawList[seriesIndex].data[i].Low != seriesToDrawList[seriesIndex].data[i].Open)
                                g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.downBorderPen, closePx.X, lowPx, openPx.X, closePx.Y);
                        }
                        else 
                        {
                            openPx = seriesToDrawList[seriesIndex].viewPort.TransformToPixels(openPx);
                            closePx = seriesToDrawList[seriesIndex].viewPort.TransformToPixels(closePx);
                            if (closePx.Y == openPx.Y)
                                closePx.Y--;

                            g.FillRectangle(seriesToDrawList[seriesIndex].legend.candleStyle.upFill, openPx.X - barWidthInPixel / 2, closePx.Y, barWidthInPixel, openPx.Y - closePx.Y);
                            
                            g.DrawRectangle(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, openPx.X - barWidthInPixel / 2, closePx.Y, barWidthInPixel, openPx.Y - closePx.Y);

                            
                            if (seriesToDrawList[seriesIndex].data[i].High != seriesToDrawList[seriesIndex].data[i].Close)
                            g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, openPx.X, highPx, closePx.X, closePx.Y);
                            if (seriesToDrawList[seriesIndex].data[i].Low != seriesToDrawList[seriesIndex].data[i].Open)
                            g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, closePx.X, lowPx, openPx.X, openPx.Y);
                        }
                    }
                    else
                    {
                        openPx = seriesToDrawList[seriesIndex].viewPort.TransformToPixels(openPx);
                        g.DrawRectangle(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, openPx.X - barWidthInPixel / 2, openPx.Y, barWidthInPixel, 1);

                        g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, openPx.X, lowPx, openPx.X, highPx);
                    }


                    // g.DrawLine(ds.LineStyle.linePen, openPx,, p2);
                }
            }
           
        passTheCicle: ; 
           
            #endregion CandleStyle
            
        }
        internal void DrawCandleChartWithAutoMargins(int seriesIndex)
        {
            #region CandleStyle
            if (grid.IsVisible)
                DrawGrid(seriesIndex);
            seriesToDrawList[seriesIndex].viewPort.PrepareTransformFunctionsAutoMargins(bmp);
            if (seriesToDrawList[seriesIndex].viewPort.VPMaxPrice < seriesToDrawList[seriesIndex].viewPort.absRealPriceMin)
                goto passTheCicle;
            if (seriesToDrawList[seriesIndex].viewPort.VPMinPrice > seriesToDrawList[seriesIndex].viewPort.absRealPriceMax)
                goto passTheCicle;

           


            //if (seriesToDrawList[seriesIndex].viewPort.VisibleMaxTime > seriesToDrawList[seriesIndex].data.Count)
            //    tempMax = seriesToDrawList[seriesIndex].data.Count;
            //else
            //    tempMax = seriesToDrawList[seriesIndex].viewPort.VisibleMaxTime - 1;


            //if (seriesToDrawList[seriesIndex].viewPort.VisibleMinTime < 2)
            //    tempMin = 2;
            //else
            //    tempMin = seriesToDrawList[seriesIndex].viewPort.VisibleMinTime;

            barWidthInPixel = seriesToDrawList[seriesIndex].viewPort.BarNumberToPixels(1) - seriesToDrawList[seriesIndex].viewPort.BarNumberToPixels(0);

            if (barWidthInPixel < 0)
                barWidthInPixel = 1;
            if (barWidthInPixel < 3)
            {
                // рисуем линии от ХАй до ЛОУ вместо свечей ибо не влезают в экран. 
                openPx = PointF.Empty;
                closePx = PointF.Empty;
                for (int i = seriesToDrawList[seriesIndex].viewPort.stratTime; i < seriesToDrawList[seriesIndex].viewPort.finishTime; i++)
                {
                    // Type type = list[seriesIndex].high[i].GetType();

                    highPx = seriesToDrawList[seriesIndex].viewPort.PriceToPixels((float)seriesToDrawList[seriesIndex].data[i].High);
                    lowPx = seriesToDrawList[seriesIndex].viewPort.PriceToPixels((float)seriesToDrawList[seriesIndex].data[i].Low);

                    openPx.Y = (float)seriesToDrawList[seriesIndex].data[i].Open;
                    openPx.X = i;
                    closePx.Y = (float)seriesToDrawList[seriesIndex].data[i].Close;
                    closePx.X = i;

                    openPx = seriesToDrawList[seriesIndex].viewPort.TransformToPixels(openPx);
                    closePx = seriesToDrawList[seriesIndex].viewPort.TransformToPixels(closePx);

                    if (closePx.Y - openPx.Y != 0)
                    {
                        if (closePx.Y - openPx.Y > 0)
                        {
                            g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.downBorderPen, openPx.X, highPx, openPx.X, lowPx);
                        }
                        else
                        {

                            g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, openPx.X, lowPx, openPx.X, highPx);
                        }
                    }
                    else
                    {
                        g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, openPx.X, lowPx, openPx.X, highPx);
                        if (lowPx - highPx == 0)
                            g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, openPx.X, lowPx, openPx.X, highPx + 1);

                    }
                }

            }
            else
            {
                // рисуем свечи полностью ибо влезают в экран
                barWidthInPixel /= 1.5F;
                openPx = PointF.Empty;
                closePx = PointF.Empty;
                for (int i = seriesToDrawList[seriesIndex].viewPort.stratTime; i < seriesToDrawList[seriesIndex].viewPort.finishTime; i++)
                {

                    //openPx = ds.TransformToPixels("Y", list[seriesIndex].open[i]);
                    //closePx = ds.TransformToPixels("Y", list[seriesIndex].close[i]);


                    openPx.Y = (float)seriesToDrawList[seriesIndex].data[i].Open;
                    openPx.X = i;
                    closePx.Y = (float)seriesToDrawList[seriesIndex].data[i].Close;
                    closePx.X = i;

                    highPx = seriesToDrawList[seriesIndex].viewPort.PriceToPixels((float)seriesToDrawList[seriesIndex].data[i].High);
                    lowPx = seriesToDrawList[seriesIndex].viewPort.PriceToPixels((float)seriesToDrawList[seriesIndex].data[i].Low);



                    if (closePx.Y - openPx.Y != 0)
                    {

                        if (openPx.Y > closePx.Y)
                        {
                            openPx = seriesToDrawList[seriesIndex].viewPort.TransformToPixels(openPx);
                            closePx = seriesToDrawList[seriesIndex].viewPort.TransformToPixels(closePx);
                            if (closePx.Y == openPx.Y)
                                closePx.Y++;

                            g.FillRectangle(seriesToDrawList[seriesIndex].legend.candleStyle.donwFill, openPx.X - barWidthInPixel / 2, openPx.Y, barWidthInPixel, closePx.Y - openPx.Y);
                            g.DrawRectangle(seriesToDrawList[seriesIndex].legend.candleStyle.downBorderPen, openPx.X - barWidthInPixel / 2, openPx.Y, barWidthInPixel, closePx.Y - openPx.Y);

                            if (seriesToDrawList[seriesIndex].data[i].High != seriesToDrawList[seriesIndex].data[i].Close)
                                g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.downBorderPen, openPx.X, highPx, closePx.X, openPx.Y);
                            if (seriesToDrawList[seriesIndex].data[i].Low != seriesToDrawList[seriesIndex].data[i].Open)
                                g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.downBorderPen, closePx.X, lowPx, openPx.X, closePx.Y);
                        }
                        else
                        {
                            openPx = seriesToDrawList[seriesIndex].viewPort.TransformToPixels(openPx);
                            closePx = seriesToDrawList[seriesIndex].viewPort.TransformToPixels(closePx);
                            if (closePx.Y == openPx.Y)
                                closePx.Y--;

                            g.FillRectangle(seriesToDrawList[seriesIndex].legend.candleStyle.upFill, openPx.X - barWidthInPixel / 2, closePx.Y, barWidthInPixel, openPx.Y - closePx.Y);
                            g.DrawRectangle(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, openPx.X - barWidthInPixel / 2, closePx.Y, barWidthInPixel, openPx.Y - closePx.Y);

                            if (seriesToDrawList[seriesIndex].data[i].High != seriesToDrawList[seriesIndex].data[i].Close)
                                g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, openPx.X, highPx, closePx.X, closePx.Y);
                            if (seriesToDrawList[seriesIndex].data[i].Low != seriesToDrawList[seriesIndex].data[i].Open)
                                g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, closePx.X, lowPx, openPx.X, openPx.Y);
                        }
                    }
                    else
                    {
                        openPx = seriesToDrawList[seriesIndex].viewPort.TransformToPixels(openPx);
                        
                        g.DrawRectangle(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, openPx.X - barWidthInPixel / 2, openPx.Y, barWidthInPixel, 1);

                        g.DrawLine(seriesToDrawList[seriesIndex].legend.candleStyle.upBorderPen, openPx.X, lowPx, openPx.X, highPx);
                    }


                    // g.DrawLine(ds.LineStyle.linePen, openPx,, p2);
                }
            }

        passTheCicle: ;

            #endregion CandleStyle
            
        }
        
        //check Point values
        bool PtsValid(PointF[] p)
        {
            return ((!p[0].IsEmpty) && (!p[1].IsEmpty) && p[0].X > int.MinValue && p[1].X > int.MinValue);
        }
        List<string> logged = new List<string>();
        internal Pen selectPen(bool selected, int step, bool baseline)
        {
            int type = step + (baseline ? 0x0F00:0x00F0);
            if(selected) type |= 0xF000;
            switch (type)
            {
                case 0xFF01: return Styles.ModelStyle.selectedMainLinesPen;
                case 0xF0F1:
                case 0xF0F2:
                case 0xF0F4:
                    return Styles.ModelStyle.selectedAdditionalLinesPen;
                case 0xFF02: return Styles.ModelStyle.selectedMainDashedLinesPen;
                case 0xFF04: return Styles.ModelStyle.selectedMainDashDotLinesPen;
  
                case 0x0F01: return Styles.ModelStyle.fadedMainLinesPen;
                case 0x00F1: 
                case 0x00F2:
                case 0x00F4:
                    return Styles.ModelStyle.fadedAdditionalLinesPen;
                case 0x0F02: return Styles.ModelStyle.fadedMainDashedLinesPen;
                case 0x0F04: return Styles.ModelStyle.fadedMainDashDotLinesPen;

                default: return Styles.ModelStyle.fadedAdditionalLinesPen;
            }
        }

        internal void DrawHTLines()
        {
            pt1 = PointF.Empty;
            pt2 = PointF.Empty;
            pt3 = PointF.Empty;
            if (HTtoDrawList.Count == 0) return;
            float viewportMin = seriesToDrawList[0].viewPort.VPMinTime;
            float viewportMax = seriesToDrawList[0].viewPort.VPMaxTime;
            Color HTColor = Color.FromArgb(0x77909090);
            Styles.ModelStyle.selectedAdditionalLinesPen.Color = HTColor;
            Styles.ModelStyle.selectedMainLinesPen.Color = HTColor;
            Pen pen;

            g.SmoothingMode = seriesToDrawList[0].legend.antialias ? System.Drawing.Drawing2D.SmoothingMode.AntiAlias : System.Drawing.Drawing2D.SmoothingMode.None;

            for (int i = 0; i < HTtoDrawList.Count; i++)
            {
                if (HTtoDrawList[i].IsVisible)
                {
                    if (viewportMin > HTtoDrawList[i].PointsForDrawing_I_E[0].X ||
                        viewportMax < HTtoDrawList[i].PointsForDrawing_I_E[1].X)
                    {

                        //skipped++;
                        continue;
                    }
                    pen = HTtoDrawList[i].SelectedCount>0 ? Styles.ModelStyle.selectedMainLinesPen : Styles.ModelStyle.selectedAdditionalLinesPen;
                    // trend line
                    try
                    {
                        PointF[] pts = HTtoDrawList[i].PointsForDrawing_I_E;
                        if (PtsValid(pts))
                        {
                            pt1 = seriesToDrawList[0].viewPort.TransformToPixels(pts[0]);
                            pt2 = seriesToDrawList[0].viewPort.TransformToPixels(pts[1]);

                            //lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPoint[0]);
                            g.DrawLine(pen, pt1, pt2);
                        }
                        else if (!logged.Contains(i + " I_E"))
                        { //прверка чтобы не дублировать строки в лог бесконечно
                            logged.Add(i + " I_E");
                            Logging.Log("   Invalid Point in HT line (i=" + i + "), Points I,E pt1.X = " + pts[0].X + " && pt3.X = " + pts[1].X);
                            //MainForm.MT4messagesForm.AddMessage("   Invalid Point in HT line (i=" + i + "), Points I,E pt1.X = " + pt1.X + " && pt3.X = " + pt2.X);
                        }
                    }
                    catch
                    {
                        Logging.Log("   catch in drawing->// линия тренда, Индекс HT: " + i);
                        //MainForm.MT4messagesForm.AddMessage("   catch in drawing->// линия тренда, Индекс HT: " + i);
                    }
                    // hypothetical target line
                    try
                    {
                        PointF[] pts = HTtoDrawList[i].PointsForDrawing_I_F;
                        if (PtsValid(HTtoDrawList[i].PointsForDrawing_I_F))
                        {
                            pt1 = seriesToDrawList[0].viewPort.TransformToPixels(pts[0]);
                            pt2 = seriesToDrawList[0].viewPort.TransformToPixels(pts[1]);

                            //lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPoint[0]);
                            g.DrawLine(pen, pt1, pt2);
                        }
                        else if (!logged.Contains(i + " I_F"))
                        { //прверка чтобы не дублировать строки в лог бесконечно
                            logged.Add(i + " I_F");
                            Logging.Log("   Invalid Point in HT line (i=" + i + "), Points I,F pt1.X = " + pts[0].X + " && pt3.X = " + pts[1].X);
                            //MainForm.MT4messagesForm.AddMessage("   Invalid Point in HT line (i=" + i + "), Points I,F pt1.X = " + pt1.X + " && pt3.X = " + pt2.X);
                        }
                    }
                    catch
                    {
                        Logging.Log("   catch in drawing->// ГЦ линия, Индекс HT: " + i);
                        //MainForm.MT4messagesForm.AddMessage("   catch in drawing->// ГЦ линия, Индекс HT: " + i);
                    }
                }
            }
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
        }

        internal void DrawModels()
        {
            pt1 = PointF.Empty;
            pt2 = PointF.Empty;
            pt3 = PointF.Empty;
            if (modelsToDrawList.Count == 0) return;
            int ii=0;
            float viewportMin = seriesToDrawList[0].viewPort.VPMinTime;
            float viewportMax = seriesToDrawList[0].viewPort.VPMaxTime;

            //выбор цвета по ТФ
//            int tf = seriesToDrawList[0].legend.frame == Skilful.QuotesManager.TF.custom ? 0 : (int)seriesToDrawList[0].legend.frame;
            int[, ,] cl = Styles.ModelStyle.cl;

            //int tf = seriesToDrawList[0].legend.frame == Skilful.QuotesManager.TF.custom ? 0 : (int)seriesToDrawList[0].legend.frame;
            int tf = (int)seriesToDrawList[0].legend.frame;

            Color selectedColor = Color.FromArgb(cl[tf, 0, 0], cl[tf, 0, 1], cl[tf, 0, 2], cl[tf, 0, 3]);
            Color fadedColor = Color.FromArgb(cl[tf, 1, 0], cl[tf, 1, 1], cl[tf, 1, 2], cl[tf, 1, 3]);
            Styles.ModelStyle.selectedMainLinesPen.Color = selectedColor;
            Styles.ModelStyle.selectedAdditionalLinesPen.Color = selectedColor;
            Styles.ModelStyle.selectedMainDashedLinesPen.Color = selectedColor;
            Styles.ModelStyle.selectedMainDashDotLinesPen.Color = selectedColor;
    
            Styles.ModelStyle.fadedMainLinesPen.Color = fadedColor;
            Styles.ModelStyle.fadedAdditionalLinesPen.Color = fadedColor;
            Styles.ModelStyle.fadedMainDashedLinesPen.Color = fadedColor;
            Styles.ModelStyle.fadedMainDashDotLinesPen.Color = fadedColor;
            //for debug only
            bool show_drawing_time = false;
            DateTime tm= DateTime.Now;
            int skipped = 0;
            //for debug only
            g.SmoothingMode = seriesToDrawList[0].legend.antialias ? System.Drawing.Drawing2D.SmoothingMode.AntiAlias : System.Drawing.Drawing2D.SmoothingMode.None;




            for (int i = 0; i < modelsToDrawList.Count; i++)
            {
                if (modelsToDrawList[i].IsVisible)
                {
                    if (viewportMin > modelsToDrawList[i].PointsForDrawing_1_3[1].X ||
                        viewportMax < modelsToDrawList[i].PointsForDrawing_1_3[0].X)
                    {

                        skipped++;
                        continue;
                    }
                    // Вычисление цвета для ТФ данной модели
                    //tf = seriesToDrawList[0].legend.frame == Skilful.QuotesManager.TF.custom ? 0 : (int)modelsToDrawList[i].TimeFrame;
                    tf = (int)modelsToDrawList[i].TimeFrame;
                    selectedColor = Color.FromArgb(cl[tf, 0, 0], cl[tf, 0, 1], cl[tf, 0, 2], cl[tf, 0, 3]);
                    fadedColor = Color.FromArgb(cl[tf, 1, 0], cl[tf, 1, 1], cl[tf, 1, 2], cl[tf, 1, 3]);
                    Styles.ModelStyle.selectedMainLinesPen.Color = selectedColor;
                    Styles.ModelStyle.selectedAdditionalLinesPen.Color = selectedColor;
                    Styles.ModelStyle.selectedMainDashedLinesPen.Color = selectedColor;
                    Styles.ModelStyle.selectedMainDashDotLinesPen.Color = selectedColor;

                    Styles.ModelStyle.fadedMainLinesPen.Color = fadedColor;
                    Styles.ModelStyle.fadedAdditionalLinesPen.Color = fadedColor;
                    Styles.ModelStyle.fadedMainDashedLinesPen.Color = fadedColor;
                    Styles.ModelStyle.fadedMainDashDotLinesPen.Color = fadedColor;
                    // линия тренда
                    /////////////////////////////////////////////////////////////////////////////////////////


                    //if (modelsToDrawList[i].IsSelected )
                    //{
                    //     if (PtsValid(modelsToDrawList[i].PointsForDrawing_21_4))
                    //    {
                    //        pt1 = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].PointsForDrawing_21_4[0]);
                    //        pt2 = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].PointsForDrawing_21_4[1]);
                            
                    //    }
                    //     if (PtsValid(modelsToDrawList[i].PointsForDrawing_1_3))
                    //    {
                    //        pt1 = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].PointsForDrawing_1_3[0]);
                    //        pt2 = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].PointsForDrawing_1_3[1]);
                    //     }
                    //     PointF location = PointF.Empty;
                        
                    //    if (pt1.X > pt2.X)
                    //        location.X = pt2.X;
                    //    else
                    //        location.X = pt1.X;
                    //    if (pt1.Y > pt2.Y)
                    //        location.Y = pt2.Y;
                    //    else
                    //        location.Y = pt1.Y;

                    //    g.FillRectangle(new SolidBrush(Color.FromArgb(250, Color.White)), new RectangleF(location.X, location.Y, Math.Abs(pt1.X - pt2.X), Math.Abs(pt1.Y - pt2.Y)));

                    //}

                    try
                    {
                        PointF[] pts = modelsToDrawList[i].PointsForDrawing_1_3;
                        if (PtsValid(modelsToDrawList[i].PointsForDrawing_1_3))
                        {
                            pt1 = seriesToDrawList[0].viewPort.TransformToPixels(pts[0]);
                            pt2 = seriesToDrawList[0].viewPort.TransformToPixels(pts[1]);

                            //lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPoint[0]);
                            g.DrawLine(selectPen(modelsToDrawList[i].IsSelected, modelsToDrawList[i].style.step, true), pt1, pt2);
                        }
                        else if(!logged.Contains(i + " 1_3")){ //прверка чтобы не дублировать строки в лог бесконечно
                            logged.Add(i + " 1_3");
                            Logging.Log("   Invalid Point in model (i=" + i + "), Points 1,3 pt1.X = " + pts[0].X + " && pt3.X = " + pts[1].X);
                            //MainForm.MT4messagesForm.AddMessage("   Invalid Point in model (i=" + i + "), Points 1,3 pt1.X = " + pt1.X + " && pt3.X = " + pt2.X);
                        }
                    }
                    catch
                    {
                        Logging.Log("   catch in drawing->// линия тренда, Индекс модели: " + i);
                        //MainForm.MT4messagesForm.AddMessage("   catch in drawing->// линия тренда, Индекс модели: " + i);
                    }

                    //линия целей
                    /////////////////////////////////////////////////////////////////////////////////////////
                    try
                    {
                        PointF[] pts = modelsToDrawList[i].PointsForDrawing_21_4;
                        if (PtsValid(modelsToDrawList[i].PointsForDrawing_21_4))
                        {
                            pt1 = seriesToDrawList[0].viewPort.TransformToPixels(pts[0]);
                            pt2 = seriesToDrawList[0].viewPort.TransformToPixels(pts[1]);
                            g.DrawLine(selectPen(modelsToDrawList[i].IsSelected, modelsToDrawList[i].style.step, true), pt1, pt2);
                        }
                        else if (!logged.Contains(i + " 2_4"))
                        { //прверка чтобы не дублировать строки в лог бесконечно
                            logged.Add(i + " 2_4");
                            Logging.Log("   Invalid Point in model (i=" + i + "), Points 21,4 pt21.X = " + pts[0].X + " && pt4.X = " + pts[1].X);
                            //MainForm.MT4messagesForm.AddMessage("   Invalid Point in model (i=" + i + "), Points 21,4 pt21.X = " + pts[1].X + " && pt4.X = " + pts[2].X);
                        }
                    }
                    catch
                    {
                        Logging.Log("   catch in drawing->// линия целей, Индекс модели: " + i);
                        //MainForm.MT4messagesForm.AddMessage("   catch in drawing->// линия целей, Индекс модели: " + i);
                    }

                    if (modelsToDrawList[i].IsSelected)
                    {
                        //Модель притяжения в модели расширения
                        /////////////////////////////////////////////////////////////////////////////////////////
                        try
                        {
                            pt1 = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].PointsForDrawing_31_5[0]);
                            pt2 = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].PointsForDrawing_31_5[1]);
                            if (PtsValid(modelsToDrawList[i].PointsForDrawing_31_5))
                                //g.DrawLine(selectPen(modelsToDrawList[i].IsSelected, modelsToDrawList[i].style.step, false), pt1, pt2);
                                g.DrawLine(selectPen(true, 1, false), pt1, pt2);
                            //else ;
                            //                        Logging.Log("   Invalid Point in model (i=" + i + "), Points 31,5");
                        }
                        catch
                        {
                            Logging.Log("   catch in drawing->// линия притяжения, Индекс модели: " + i);
                            //MainForm.MT4messagesForm.AddMessage("   catch in drawing->// линия притяжения, Индекс модели: " + i);

                        }
                        //точки, уровни, цели
                        DrawSelectedModelLables(i);
                      if (Skilful.MainForm.ShowIndexCheckedModel)
                          g.DrawString("Model index: " + i.ToString(), SystemFonts.CaptionFont, Styles.ModelStyle.mainLablesFontBrush, new Point(0, ii++ * 15));
                    }
                }

            }
            g.SmoothingMode =  System.Drawing.Drawing2D.SmoothingMode.None;
            //for debug only
            if (show_drawing_time)
            {
                DateTime tm1 = DateTime.Now;
                TimeSpan ts = tm1 - tm;
                g.DrawString(skipped + " моделей из " + modelsToDrawList.Count + " пропущено, нарисовано " + (modelsToDrawList.Count - skipped) + " за " + ts.TotalMilliseconds + "мс", SystemFonts.CaptionFont, new SolidBrush(Styles.ModelStyle.selectedMainLinesPen.Color), new Point(0, ii++ * 15));
            }
            //for debug only
        }
        internal void DrawModelPointLabel(String Label, PointF LablePoint) //! Маркировка точки на графике
        {
            Styles.ModelStyle.rect.Location = LablePoint;
            RectangleF rect = Styles.ModelStyle.rect;
            if (Label.IndexOfAny(new Char[]{'2','4','6'})>=0) rect.X -= rect.Width;
            g.FillRectangle(Brushes.WhiteSmoke, rect);
            g.DrawRectangle(Styles.ModelStyle.fadedMainLinesPen, rect.X, rect.Y, rect.Width, rect.Height);

            //g.DrawString(Label, Styles.ModelStyle.mainLablesFont, Brushes.White, Styles.ModelStyle.rect.Location.X + 1, Styles.ModelStyle.rect.Location.Y - 1);
            g.DrawString(Label, Styles.ModelStyle.mainLablesFont, Styles.ModelStyle.mainLablesFontBrush, rect.Location);
        }

        //void DrawString(string Label, RectangleF rect)
        //{
        //    g.DrawString(Label, Styles.ModelStyle.mainLablesFont, Brushes.White, rect.Location.X + 1, rect.Location.Y - 1);
        //    g.DrawString(Label, Styles.ModelStyle.mainLablesFont, Styles.ModelStyle.mainLablesFontBrush, rect.Location);
        //}

        internal void DrawSelectedModelLables(int i)
        {
            float TextShiftEven = 0;  // Смещение текста по Y для четных точек
            float TextShiftOdd = 0;  // Смещение текста по Y для нечетных точек

            //if (barWidthInPixel > 10)
            //    fontSize = barWidthInPixel / 1.5F;
            //Styles.ModelStyle.mainLablesFont = new Font(FontFamily.GenericSansSerif, fontSize);
            //Styles.ModelStyle.additionalLablesFont = new Font(FontFamily.GenericSansSerif, fontSize / 1.5F);
       
            Styles.ModelStyle.rect = new RectangleF(PointF.Empty, g.MeasureString("0", Styles.ModelStyle.mainLablesFont));

            string price_format = seriesToDrawList[0].legend.price_format;

            switch (modelsToDrawList[i].type)
            {
                case ModelType.ATR:
                        lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].PointsForDrawing_1_3[1]);
                        // Вычисление смешения по Y для текстовых меток
                        if (modelsToDrawList[i].ModelPoint[1].Y > modelsToDrawList[i].ModelPoint[2].Y)
                        {
                            TextShiftEven = - Styles.ModelStyle.rect.Height;
                        }
                        g.DrawLine(Styles.ModelStyle.fadedMainLinesPen, lablepoint.X, lablepoint.Y, bmp.Width, lablepoint.Y);
                        lablepoint.Y = lablepoint.Y + TextShiftEven;

                        //axisForModelLables.targetLable.Value = (GlobalMembersTrend.log == true) ? (float)Math.Pow(10, modelsToDrawList[i].ModelPoint[5].Y) : modelsToDrawList[i].ModelPoint[5].Y;

                        Styles.ModelStyle.rect = new RectangleF(lablepoint, g.MeasureString("HP", Styles.ModelStyle.mainLablesFont));
                        g.FillRectangle(Brushes.WhiteSmoke, Styles.ModelStyle.rect);
                        g.DrawRectangle(Styles.ModelStyle.fadedMainLinesPen, Styles.ModelStyle.rect.X, Styles.ModelStyle.rect.Y, Styles.ModelStyle.rect.Width, Styles.ModelStyle.rect.Height);
                        g.DrawString("HP", Styles.ModelStyle.mainLablesFont, Styles.ModelStyle.mainLablesFontBrush, lablepoint);

                    goto case ModelType.MP;
                    
                case ModelType.MDR:
                case ModelType.MP:
                
                    //проверка на то есть ли СТ или нет. (не отработана система загона СТ в массив)
                    if (!modelsToDrawList[i].ModelPoint[0].IsEmpty)
                    {
                        Styles.ModelStyle.rect = new RectangleF(PointF.Empty, g.MeasureString("СТ", Styles.ModelStyle.mainLablesFont));
                        lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPoint[0]);
                        // Вычисление смешения по Y для текстовых меток
                        if (modelsToDrawList[i].ModelPoint[1].Y > modelsToDrawList[i].ModelPoint[2].Y)
                        {
                            TextShiftEven = -Styles.ModelStyle.rect.Height;
                        }
                        lablepoint.Y = lablepoint.Y + TextShiftEven;
                        float YY = seriesToDrawList[0].legend.log ? (float)Math.Pow(10, modelsToDrawList[i].ModelPoint[0].Y) : modelsToDrawList[i].ModelPoint[0].Y;
                        //
                        Styles.ModelStyle.rect.Location = lablepoint;
                        g.FillRectangle(Brushes.WhiteSmoke, Styles.ModelStyle.rect);
                        g.DrawRectangle(Styles.ModelStyle.fadedMainLinesPen, Styles.ModelStyle.rect.X, Styles.ModelStyle.rect.Y, Styles.ModelStyle.rect.Width, Styles.ModelStyle.rect.Height);
                        g.DrawString("CP  " + YY.ToString(price_format), Styles.ModelStyle.mainLablesFont, Styles.ModelStyle.mainLablesFontBrush, lablepoint);
                        //

                    }
                    ///1
                    //
                    //пересчет прямоугольника лейбла, чтобы не был такой здоровый как у СТ.

                    Styles.ModelStyle.rect = new RectangleF(PointF.Empty, g.MeasureString("0", Styles.ModelStyle.mainLablesFont));
                    Styles.ModelStyle.addRect = new RectangleF(PointF.Empty, g.MeasureString("0", Styles.ModelStyle.additionalLablesFont));

                    // Вычисление смещения по Y для текстовых меток
                    if (modelsToDrawList[i].ModelPoint[1].Y < modelsToDrawList[i].ModelPoint[2].Y)
                    {
                        TextShiftOdd = 0;
                        TextShiftEven = -Styles.ModelStyle.rect.Height;
                    }
                    else
                    {
                        TextShiftEven = 0;
                        TextShiftOdd = -Styles.ModelStyle.rect.Height;
                    }

                    lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPointReal[1]);
                    lablepoint.Y = lablepoint.Y + TextShiftOdd;

                    DrawModelPointLabel("1", lablepoint);
                    // Если т1 и т1' не совпадают, то обозначение обоих точек
                    if (modelsToDrawList[i].ModelPointReal[1].X != modelsToDrawList[i].ModelPoint[1].X)
                    {
                        lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPoint[1]);
                        lablepoint.Y = lablepoint.Y + TextShiftOdd;
                        DrawModelPointLabel("1'", lablepoint);
                    }

                    //
                    ///2
                    //
                    lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPointReal[2]);
                    lablepoint.Y = lablepoint.Y + TextShiftEven;

                    DrawModelPointLabel("2", lablepoint);
                    if (modelsToDrawList[i].ModelPoint[2].X != modelsToDrawList[i].ModelPointReal[2].X)
                    {
                        lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPoint[2]);
                        lablepoint.Y = lablepoint.Y + TextShiftEven;
                        DrawModelPointLabel("2'", lablepoint);
                    }

                    // 
                    ///3       
                    //
                    lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPointReal[3]);
                    lablepoint.Y = lablepoint.Y + TextShiftOdd;

                    DrawModelPointLabel("3", lablepoint);

                    //
                    ///4
                    //
                    lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPointReal[4]);
                    lablepoint.Y = lablepoint.Y + TextShiftEven;

                    DrawModelPointLabel("4", lablepoint);
                    // Если т4 и т4' не совпадают, то обозначение обоих точек
                    if (modelsToDrawList[i].ModelPoint[4].X != modelsToDrawList[i].ModelPointReal[4].X)
                    {
                        lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPoint[4]);
                        lablepoint.Y = lablepoint.Y + TextShiftEven;
                        DrawModelPointLabel("4'", lablepoint);
                    }
                    //                   
                    //проверка на наличие МР в моделях МДР МР и МП
                    if (modelsToDrawList[i].AttractionPoint[1].X > modelsToDrawList[i].ModelPointReal[3].X && modelsToDrawList[i].ModelPointReal[5].X != modelsToDrawList[i].ModelPointReal[4].X)
                    {
                        //проверка на различие точек 3 и 3'
                        if (modelsToDrawList[i].AttractionPoint[0].X > modelsToDrawList[i].ModelPointReal[3].X)
                        {
                            ///3'
                            //
                            lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].AttractionPoint[0]);
                            lablepoint.Y = lablepoint.Y + TextShiftOdd;

                            DrawModelPointLabel("3'", lablepoint);
                        }
                        ///5
                        //
                        lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].AttractionPoint[1]);
                        lablepoint.Y = lablepoint.Y + TextShiftOdd;

                        DrawModelPointLabel("5", lablepoint);
                    }
                    
                    ///6
                    //
                    if (!modelsToDrawList[i].ModelPointReal[6].IsEmpty)
                    {
                        lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPointReal[6]);
                        lablepoint.Y = lablepoint.Y + TextShiftEven;
//                        Styles.ModelStyle.rect.Location = lablepoint;

                        // Проверка на совпадение т4 и т6
                        if (modelsToDrawList[i].ModelPointReal[6].X == modelsToDrawList[i].ModelPointReal[4].X)
                        {
                            float width = Styles.ModelStyle.rect.Width;
                            Styles.ModelStyle.rect.Width += width;
                            DrawModelPointLabel("4=6", lablepoint);
                            Styles.ModelStyle.rect.Width -= width;
                        }
                        else
                        {
                            DrawModelPointLabel("6", lablepoint);
                        }
                    }

                    // Рисование HP's
                    if ((!modelsToDrawList[i].ModelPoint[5].IsEmpty) && (modelsToDrawList[i].ModelPoint[5].X!=modelsToDrawList[i].ModelPointReal[6].X))
                    {
           
                        //axisForModelLables.targetLable.borderPen = modelsToDrawList[i].style.targetLinePen;
                        
                        lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPoint[5]);
                        
                        if (modelsToDrawList[i].type != ModelType.ATR)
                        {
                            //уровень НР'
                            g.DrawLine(Styles.ModelStyle.fadedMainLinesPen, lablepoint.X, lablepoint.Y, bmp.Width, lablepoint.Y);
                           
                            lablepoint.Y = lablepoint.Y + TextShiftEven;

                            Styles.ModelStyle.rect.Location = lablepoint;
                            float width = Styles.ModelStyle.rect.Width;
                            Styles.ModelStyle.rect.Width += width;
                            g.FillRectangle(Brushes.WhiteSmoke, Styles.ModelStyle.rect);
                            g.DrawRectangle(Styles.ModelStyle.fadedMainLinesPen, Styles.ModelStyle.rect.X, Styles.ModelStyle.rect.Y, Styles.ModelStyle.rect.Width, Styles.ModelStyle.rect.Height);
                            g.DrawString("HP'", Styles.ModelStyle.mainLablesFont, Styles.ModelStyle.mainLablesFontBrush, lablepoint);
                            Styles.ModelStyle.rect.Width -= width;

                            //если 4!=4' то у нас есть 2 НР' 
                            if (modelsToDrawList[i].ModelPoint[4] != modelsToDrawList[i].ModelPointReal[4])
                            {
                                //целевая 2'_4
                                g.DrawLine(Styles.ModelStyle.selectedAdditionalLinesPen, 
                                    seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPoint[2]),
                                    seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPoint[5]));

                                //остутствие НР от 2-4', нужно дорисовать укороченную "трендовую" от 3-5 для 2-4
                                if (modelsToDrawList[i].PointsForDrawing_31_5[1].X == 0)
                                {
                                    g.DrawLine(Styles.ModelStyle.selectedAdditionalLinesPen,
                                        seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPoint[3]),
                                        seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPoint[5]));
                                }
                            }

                            if (!modelsToDrawList[i].ModelPointReal[7].IsEmpty)
                            {
                                lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPointReal[7]);
                                Styles.ModelStyle.rect = new RectangleF(lablepoint, g.MeasureString("6*", Styles.ModelStyle.additionalLablesFont));
                                g.FillRectangle(Brushes.WhiteSmoke, Styles.ModelStyle.rect);
                                g.DrawRectangle(Styles.ModelStyle.fadedMainLinesPen, Styles.ModelStyle.rect.X, Styles.ModelStyle.rect.Y, Styles.ModelStyle.rect.Width, Styles.ModelStyle.rect.Height);

                                g.DrawString("6*", Styles.ModelStyle.additionalLablesFont, Styles.ModelStyle.mainLablesFontBrush, lablepoint);
                            }

                            string[] HP_label = {"HP  ", "HP\"  ", "HP'  "};
                            for (int j=0; j < 3; j++)
                            {
                                if (!modelsToDrawList[i].FarPoint6[j].IsEmpty
                                  && modelsToDrawList[i].FarPoint6[j].X > modelsToDrawList[i].ModelPoint[5].X
                                  && (j==0 || modelsToDrawList[i].FarPoint6[j].X != modelsToDrawList[i].FarPoint6[j-1].X))
                                {
                                    float YY = seriesToDrawList[0].legend.log ? (float)Math.Pow(10, modelsToDrawList[i].FarPoint6[j].Y) : modelsToDrawList[i].FarPoint6[j].Y;
                                    PointF pt2 = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i]._211[j]);
                                    lablepoint = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].FarPoint6[j]);

                                    g.DrawLine(Styles.ModelStyle.selectedAdditionalLinesPen, pt2, lablepoint);
                                    g.DrawLine(Styles.ModelStyle.fadedMainLinesPen, lablepoint.X, lablepoint.Y, lablepoint.X + 150, lablepoint.Y);

                                    Styles.ModelStyle.rect = new RectangleF(lablepoint, g.MeasureString("HP", Styles.ModelStyle.additionalLablesFont));
                                    g.FillRectangle(Brushes.WhiteSmoke, Styles.ModelStyle.rect);
                                    g.DrawRectangle(Styles.ModelStyle.fadedMainLinesPen, Styles.ModelStyle.rect.X, Styles.ModelStyle.rect.Y, Styles.ModelStyle.rect.Width, Styles.ModelStyle.rect.Height);

                                    g.DrawString(HP_label[j] + YY.ToString(price_format), Styles.ModelStyle.additionalLablesFont, Styles.ModelStyle.mainLablesFontBrush, lablepoint);

                                }
                            }
                        }
                        else  //МПМП
                        {
                            if (lablepoint.X >= 0 && lablepoint.Y >= 0)
                            {
                                float YY = seriesToDrawList[0].legend.log ? (float)Math.Pow(10, modelsToDrawList[i].ModelPoint[5].Y) : modelsToDrawList[i].ModelPoint[5].Y;
                                g.DrawLine(Styles.ModelStyle.fadedMainLinesPen, lablepoint.X, lablepoint.Y, lablepoint.X + 100, lablepoint.Y);
                                Styles.ModelStyle.addRect.Location = lablepoint;
                                g.FillRectangle(Brushes.WhiteSmoke, Styles.ModelStyle.addRect);
                                g.DrawRectangle(Styles.ModelStyle.fadedMainLinesPen, Styles.ModelStyle.addRect.X, Styles.ModelStyle.addRect.Y, Styles.ModelStyle.addRect.Width, Styles.ModelStyle.addRect.Height);
                                g.DrawString("HP'  " + YY.ToString(price_format), Styles.ModelStyle.additionalLablesFont, Styles.ModelStyle.mainLablesFontBrush, lablepoint);
                            }
                           
                        }
                        
                               
                        
                      
                        
                    }

                    if ((!modelsToDrawList[i].ModelPointReal[6].IsEmpty) || (modelsToDrawList[i].type == ModelType.ATR))
                    {// рисование зеркальных точек модели
                        if (modelsToDrawList[i].type==ModelType.MP)
                        {
                            for (int k = 0; k < 6; k++)
                            {
                                if ((!modelsToDrawList[i].ModelPointReal[k].IsEmpty) && (modelsToDrawList[i].ModelPointReal[k].X != modelsToDrawList[i].ModelPointReal[6].X))
                                {

                                    float Y = seriesToDrawList[0].legend.log ? (float)Math.Pow(10, modelsToDrawList[i].ModelPointReal[k].Y) : modelsToDrawList[i].ModelPointReal[k].Y;
                                    pt1 = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPointReal[k]);
                                    PointF tmp = new PointF(2 * modelsToDrawList[i].ModelPointReal[6].X - modelsToDrawList[i].ModelPointReal[k].X, modelsToDrawList[i].ModelPointReal[k].Y);
                                    pt2 = seriesToDrawList[0].viewPort.TransformToPixels(tmp);
                                    
                                    if ((k == 1) && (modelsToDrawList[i].ModelPointReal[1].X != modelsToDrawList[i].ModelPoint[1].X))
                                    {
                                        Y = seriesToDrawList[0].legend.log ? (float)Math.Pow(10, modelsToDrawList[i].ModelPoint[k].Y) : modelsToDrawList[i].ModelPoint[k].Y;
                                        pt1 = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].ModelPoint[k]);
                                        tmp = new PointF(2 * modelsToDrawList[i].ModelPointReal[6].X - modelsToDrawList[i].ModelPoint[k].X, modelsToDrawList[i].ModelPoint[k].Y);
                                        pt2 = seriesToDrawList[0].viewPort.TransformToPixels(tmp);
                                    }
                                    g.DrawLine(Styles.ModelStyle.fadedMainLinesPen, pt1, pt2);
                                    
                                    g.DrawLine(Styles.ModelStyle.fadedMainLinesPen, pt2.X, pt2.Y + 20, pt2.X, pt2.Y - 20);
                                    g.DrawString(((k == 0) ? "CP" : k.ToString()) + "*", Styles.ModelStyle.mainLablesFont, Styles.ModelStyle.mainLablesFontBrush, pt2);
                                }
                            }
                        }
                        for (int k = 0; k < 2; k++)
                        {// рисование 100% 200%
                            //pt1 = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i]._100Percent[k].pt);
                            Border border = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i]._100Percent[k]);
                            if ((border.xb - border.xa) < 150) border.xa = Math.Max((border.xb - 150), (int)(seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].PointsForDrawing_1_3[0])).X);

                            g.DrawLine(Styles.ModelStyle.fadedMainLinesPen, border.xa, border.y, border.xb, border.y);

                            float Y = seriesToDrawList[0].legend.log ? (float)Math.Pow(10, modelsToDrawList[i]._100Percent[k].y) : modelsToDrawList[i]._100Percent[k].y;
                            g.DrawString(Y.ToString(price_format), Styles.ModelStyle.mainLablesFont, Styles.ModelStyle.mainLablesFontBrush, (float)border.xa, (float)border.y);
                            g.DrawString(((k + 1) * 100).ToString() + "% ", Styles.ModelStyle.mainLablesFont, Styles.ModelStyle.mainLablesFontBrush, border.xa, border.y - Styles.ModelStyle.mainLablesFont.GetHeight());

                        }                       

                    }

                    if (!modelsToDrawList[i].ModelPointReal[0].IsEmpty)
                    {// рисование СТ-4
                        for (int k = 0; k < 2; k++)
                        {
                            pt1 = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].CT_4[k]);
                            PointF tmp = new PointF(modelsToDrawList[i].CT_4[k].X, modelsToDrawList[i].ModelPointReal[6].Y);
                            pt2 = seriesToDrawList[0].viewPort.TransformToPixels(tmp);
                            g.DrawLine(Styles.ModelStyle.fadedMainLinesPen, pt1, pt2);
                            g.DrawString("CP-4 x "+(k+1).ToString(), Styles.ModelStyle.mainLablesFont, Styles.ModelStyle.mainLablesFontBrush, pt2);
                        }
                    }

                    for (int k = 0; k < 6; k++)
                    {// рисование целей модели
                        if (!modelsToDrawList[i].Targ[k].IsEmpty)
                        {
                            float Y = seriesToDrawList[0].legend.log ? (float)Math.Pow(10, modelsToDrawList[i].Targ[k].y) : modelsToDrawList[i].Targ[k].y;
                            Border border = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].Targ[k]);
                            PointF left = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].PointsForDrawing_1_3[0]);
                            PointF right = seriesToDrawList[0].viewPort.TransformToPixels(modelsToDrawList[i].PointsForDrawing_1_3[1]);
                            if (modelsToDrawList[i].Targ[k].xb == 0) border.xb = (int)right.X;
                            if (modelsToDrawList[i].Targ[k].xa == 0) border.xa = (int)left.X;
                            switch (k)
                            {
                                case 0:
                                    if ((border.xb - border.xa) < 40) border.xa = Math.Max(border.xb - 40, (int)(left.X));
                                    break;
                                default:
                                    if ((border.xb - border.xa) < 100) border.xa = Math.Max(border.xb - 100, (int)(left.X));
                                    break;
                            }
                            g.DrawLine(Styles.ModelStyle.fadedMainLinesPen, border.xa, border.y, border.xb, border.y);
                            g.DrawString(Y.ToString(price_format), Styles.ModelStyle.mainLablesFont, Styles.ModelStyle.mainLablesFontBrush, (float)border.xa, (float)border.y);
                            g.DrawString((k + 1).ToString() + " target ", Styles.ModelStyle.mainLablesFont, Styles.ModelStyle.mainLablesFontBrush, border.xa, border.y - Styles.ModelStyle.mainLablesFont.GetHeight());
                        }
                    }
                    //               


                    break;

            }
        }


        internal void DrawGraphicalTools()
        {
            System.Drawing.Drawing2D.SmoothingMode Antialias = seriesToDrawList[0].legend.antialias ? System.Drawing.Drawing2D.SmoothingMode.AntiAlias : System.Drawing.Drawing2D.SmoothingMode.None;

            for (int i = 0; i < graphToolsToDrawList.Count; i++)
            {
                if (!graphToolsToDrawList[i].IsVisible)
                    continue;

                if (Antialias != g.SmoothingMode)
                    g.SmoothingMode = Antialias;

                switch (graphToolsToDrawList[i].type)
                {
                    case DrawingTool.VerticalLine:
                        if (Antialias != System.Drawing.Drawing2D.SmoothingMode.None) 
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                        pt1.X =  seriesToDrawList[0].viewPort.BarNumberToPixels(graphToolsToDrawList[i].IntPt1.X);
                        pt1.Y =  seriesToDrawList[0].viewPort.PriceToPixels(seriesToDrawList[0].viewPort.VPMaxPrice);
                        pt2.X  =  seriesToDrawList[0].viewPort.BarNumberToPixels(graphToolsToDrawList[i].IntPt2.X);
                        pt2.Y = seriesToDrawList[0].viewPort.PriceToPixels(seriesToDrawList[0].viewPort.VPMinPrice);
                        if (float.IsInfinity(pt1.X) || float.IsInfinity(pt2.X))
                            break;
                        g.DrawLine(graphToolsToDrawList[i].style.pen, pt1,pt2);
                      // g.DrawString(seriesToDrawList[0].data[graphToolsToDrawList[i].IntPt1.X].DT.ToString("HH:mm"), SystemFonts.DefaultFont, Brushes.Black, pt1);
                      if (graphToolsToDrawList[i].IntPt1.X <0)
                          g.DrawString(seriesToDrawList[0].data[0].DT.ToString(topAxis.dateTimeFormat), SystemFonts.DefaultFont, Brushes.Black, pt1);
                      else if (graphToolsToDrawList[i].IntPt1.X>=seriesToDrawList[0].data.Count)
                          g.DrawString(seriesToDrawList[0].data[seriesToDrawList[0].data.Count-1].DT.ToString(topAxis.dateTimeFormat), SystemFonts.DefaultFont, Brushes.Black, pt1);
                      else
                          g.DrawString( seriesToDrawList[0].data[graphToolsToDrawList[i].IntPt1.X].DT.ToString(topAxis.dateTimeFormat), SystemFonts.DefaultFont, Brushes.Black, pt1);
                      
                       if (graphToolsToDrawList[i].IsSelected)
                        {
                            tempRect = new RectangleF(pt1.X-2, bmp.Height / 3, 4, 4);
                            g.DrawRectangle(graphToolsToDrawList[i].style.pen,tempRect.X,tempRect.Y,tempRect.Width,tempRect.Height);
                            tempRect.Location = new PointF(tempRect.X, bmp.Height - bmp.Height / 3F);
                                 g.DrawRectangle(graphToolsToDrawList[i].style.pen,tempRect.X,tempRect.Y,tempRect.Width,tempRect.Height);
                        }
                        break;
                    case DrawingTool.Cycles:
                        if (Antialias != System.Drawing.Drawing2D.SmoothingMode.None)
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                        pt1.X = seriesToDrawList[0].viewPort.BarNumberToPixels(graphToolsToDrawList[i].IntPt1.X);
                        pt1.Y = seriesToDrawList[0].viewPort.PriceToPixels(seriesToDrawList[0].viewPort.VPMaxPrice);
                        pt2.X = seriesToDrawList[0].viewPort.BarNumberToPixels(graphToolsToDrawList[i].IntPt2.X);
                        pt2.Y = seriesToDrawList[0].viewPort.PriceToPixels(seriesToDrawList[0].viewPort.VPMinPrice);
                        if (float.IsInfinity(pt1.X) || float.IsInfinity(pt2.X))
                            break;

                        float diff = (pt2.X - pt1.X);
                        int margin = bmp.Height - 20;
                        g.DrawLine(graphToolsToDrawList[i].style.pen, pt1.X, pt1.Y, pt1.X, pt2.Y);
                        g.DrawString("1", SystemFonts.CaptionFont, Brushes.Black, pt1.X + diff, margin);

                        g.DrawLine(graphToolsToDrawList[i].style.pen, pt1.X + diff, pt1.Y, pt1.X + diff, pt2.Y);
                        g.DrawString("2", SystemFonts.CaptionFont, Brushes.Black, pt1.X + diff * 2, margin);

                        g.DrawLine(graphToolsToDrawList[i].style.pen, pt1.X + diff * 2, pt1.Y, pt1.X + diff * 2, pt2.Y);
                        g.DrawString("3", SystemFonts.CaptionFont, Brushes.Black, pt1.X + diff * 3, margin);

                        g.DrawLine(graphToolsToDrawList[i].style.pen, pt1.X + diff * 3, pt1.Y, pt1.X + diff * 3, pt2.Y);
                        g.DrawString("4", SystemFonts.CaptionFont, Brushes.Black, pt1.X + diff * 4, margin);

                        g.DrawLine(graphToolsToDrawList[i].style.pen, pt1.X + diff * 4, pt1.Y, pt1.X + diff * 4, pt2.Y);
                        g.DrawString("5", SystemFonts.CaptionFont, Brushes.Black, pt1.X + diff * 5, margin);
                       
                        g.DrawLine(graphToolsToDrawList[i].style.pen, pt1.X + diff * 5, pt1.Y, pt1.X + diff * 5, pt2.Y);
                        g.DrawString("6", SystemFonts.CaptionFont, Brushes.Black, pt1.X + diff * 6, margin);
                       
                        g.DrawLine(graphToolsToDrawList[i].style.pen, pt1.X + diff * 6, pt1.Y, pt1.X + diff * 6, pt2.Y);
                        g.DrawString("7", SystemFonts.CaptionFont, Brushes.Black, pt1.X + diff * 7, margin);
                        
                        g.DrawLine(graphToolsToDrawList[i].style.pen, pt1.X + diff * 7, pt1.Y, pt1.X + diff * 7, pt2.Y);





                        if (graphToolsToDrawList[i].IsSelected)
                        {
                            tempRect = new RectangleF(pt1.X - 2, bmp.Height / 3, 4, 4);
                            g.DrawRectangle(graphToolsToDrawList[i].style.pen, tempRect.X, tempRect.Y, tempRect.Width, tempRect.Height);
                            tempRect.Location = new PointF(tempRect.X, bmp.Height - bmp.Height / 3F);
                            g.DrawRectangle(graphToolsToDrawList[i].style.pen, tempRect.X, tempRect.Y, tempRect.Width, tempRect.Height);
                        }
                        break;


                    case DrawingTool.HorizontalLine:
                        if (Antialias != System.Drawing.Drawing2D.SmoothingMode.None)
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                            
                            pt1.X = seriesToDrawList[0].viewPort.BarNumberToPixels(seriesToDrawList[0].viewPort.VPMinTime);
                            pt1.Y =  seriesToDrawList[0].viewPort.PriceToPixels(graphToolsToDrawList[i].pt1.Y);
                            pt2.X = seriesToDrawList[0].viewPort.BarNumberToPixels(seriesToDrawList[0].viewPort.VPMaxTime);
                            pt2.Y = seriesToDrawList[0].viewPort.PriceToPixels(graphToolsToDrawList[i].pt2.Y);
                            if (float.IsInfinity(pt1.Y) || float.IsInfinity(pt2.Y))
                                break;
                            g.DrawLine(graphToolsToDrawList[i].style.pen,pt1,pt2);
                            string price_label = (seriesToDrawList[0].legend.log ? (float)Math.Pow(10, graphToolsToDrawList[i].pt1.Y) : graphToolsToDrawList[i].pt1.Y).ToString(topAxis.price_format);
                            g.DrawString(price_label, SystemFonts.DefaultFont, Brushes.Black, pt1);
                            if (graphToolsToDrawList[i].IsSelected)
                            {
                                tempRect = new RectangleF( bmp.Width / 3, pt1.Y - 2, 4, 4);
                                g.DrawRectangle(graphToolsToDrawList[i].style.pen, tempRect.X, tempRect.Y, tempRect.Width, tempRect.Height);
                                tempRect.Location = new PointF( bmp.Width - bmp.Width / 3F,tempRect.Y);
                                g.DrawRectangle(graphToolsToDrawList[i].style.pen, tempRect.X, tempRect.Y, tempRect.Width, tempRect.Height);
                            }
                            break;
                    case DrawingTool.FreeLine:
                            pt1.X = seriesToDrawList[0].viewPort.BarNumberToPixels(graphToolsToDrawList[i].pt1.X);
                            pt1.Y = seriesToDrawList[0].viewPort.PriceToPixels(graphToolsToDrawList[i].pt1.Y);
                            pt2.X = seriesToDrawList[0].viewPort.BarNumberToPixels(graphToolsToDrawList[i].pt2.X);
                            pt2.Y = seriesToDrawList[0].viewPort.PriceToPixels(graphToolsToDrawList[i].pt2.Y);
                            float K = (pt2.Y - pt1.Y) / (pt2.X - pt1.X);
                            if (float.IsNaN(K) || float.IsInfinity(K) )
                                break;
                            g.DrawLine(graphToolsToDrawList[i].style.pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
                            switch (graphToolsToDrawList[i].style.extensionType)
                            {
                                case ChartV2.Styles.ExtensionType.BothRays:

                                    if (float.IsInfinity(K))
                                    {
                                        //если линия стала вертикальной.
                                        g.DrawLine(graphToolsToDrawList[i].style.pen,
                                        pt1.X,
                                        seriesToDrawList[0].viewPort.PriceToPixels(seriesToDrawList[0].viewPort.VPMaxPrice),
                                       pt2.X,
                                       seriesToDrawList[0].viewPort.PriceToPixels(seriesToDrawList[0].viewPort.VPMinPrice));

                                    }
                                    else
                                    {
                                        if (float.IsNaN(K))
                                            K = 0;
                                        g.DrawLine(graphToolsToDrawList[i].style.pen,
                                            seriesToDrawList[0].viewPort.BarNumberToPixels(seriesToDrawList[0].viewPort.VPMinTime),
                                            (seriesToDrawList[0].viewPort.BarNumberToPixels(seriesToDrawList[0].viewPort.VPMinTime) - pt1.X) * K + pt1.Y,
                                           seriesToDrawList[0].viewPort.BarNumberToPixels(seriesToDrawList[0].viewPort.VPMaxTime),
                                           (seriesToDrawList[0].viewPort.BarNumberToPixels(seriesToDrawList[0].viewPort.VPMaxTime) - pt2.X) * K + pt2.Y);

                                    }
                                    break;
                                case ChartV2.Styles.ExtensionType.LeftRay:
                                  //  g.DrawLine(graphToolsToDrawList[i].style.pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
                                    if (float.IsInfinity(K))
                                    {
                                        //если линия стала вертикальной.
                                        g.DrawLine(graphToolsToDrawList[i].style.pen,
                                        pt1.X,
                                        pt2.Y,
                                       pt2.X,
                                       seriesToDrawList[0].viewPort.PriceToPixels(seriesToDrawList[0].viewPort.VPMaxPrice));

                                    }
                                    else
                                    {
                                        if (float.IsNaN(K))
                                            K = 0;
                                        g.DrawLine(graphToolsToDrawList[i].style.pen,
                                            pt1.X,
                                           pt1.Y,
                                           seriesToDrawList[0].viewPort.BarNumberToPixels(seriesToDrawList[0].viewPort.VPMaxTime),
                                           (seriesToDrawList[0].viewPort.BarNumberToPixels(seriesToDrawList[0].viewPort.VPMaxTime) - pt2.X) * K + pt2.Y);

                                    }
                                    break;
                                case ChartV2.Styles.ExtensionType.RightRay:
                                   
                                    if (float.IsInfinity(K))
                                    {
                                        //если линия стала вертикальной.
                                        g.DrawLine(graphToolsToDrawList[i].style.pen, pt1.X, seriesToDrawList[0].viewPort.PriceToPixels(seriesToDrawList[0].viewPort.VPMinPrice), pt2.X,pt2.Y);

                                    }
                                    else
                                    {
                                        if (float.IsNaN(K))
                                            K = 0;
                                        g.DrawLine(graphToolsToDrawList[i].style.pen,
                                           seriesToDrawList[0].viewPort.BarNumberToPixels(seriesToDrawList[0].viewPort.VPMinTime),
                                           (seriesToDrawList[0].viewPort.BarNumberToPixels(seriesToDrawList[0].viewPort.VPMinTime) - pt1.X) * K + pt1.Y, pt2.X,
                                           pt2.Y);

                                    }
                                    break;

                                case ChartV2.Styles.ExtensionType.NoRays:
                                   // g.DrawLine(graphToolsToDrawList[i].style.pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
                                    break;
                            }
                            if (graphToolsToDrawList[i].IsSelected)
                            {
                                tempRect = new RectangleF(0, 0, 4, 4);
                                g.DrawRectangle(graphToolsToDrawList[i].style.pen, pt1.X-2, pt1.Y-2, tempRect.Width, tempRect.Height);
                                g.DrawRectangle(graphToolsToDrawList[i].style.pen, pt2.X - 2, pt2.Y - 2, tempRect.Width, tempRect.Height);
                            }
                            break;
                    case DrawingTool.Circle:
                        pt1 = seriesToDrawList[0].viewPort.TransformToPixels(graphToolsToDrawList[i].pt1);
                        pt2 = seriesToDrawList[0].viewPort.TransformToPixels(graphToolsToDrawList[i].pt2);
                        tempRect = new RectangleF(pt1.X,pt1.Y, pt2.X-pt1.X,pt2.Y-pt1.Y);
                        if (pt2.Y - pt1.Y <= 0 || pt2.X - pt1.X<=0)
                            break;
                        g.DrawEllipse(graphToolsToDrawList[i].style.pen, tempRect);
                        if (graphToolsToDrawList[i].IsSelected)
                        {
                            pt1 = seriesToDrawList[0].viewPort.TransformToPixels(graphToolsToDrawList[i].pt3);

                            g.DrawRectangle(graphToolsToDrawList[i].style.pen, new Rectangle((int)pt1.X - 2, (int)pt1.Y - 2, 4, 4));
                            pt1 = seriesToDrawList[0].viewPort.TransformToPixels(graphToolsToDrawList[i].pt4);
                            g.DrawRectangle(graphToolsToDrawList[i].style.pen, new Rectangle((int)pt1.X - 2, (int)pt1.Y - 2, 4, 4));
                       
                        }
                        pt1 = seriesToDrawList[0].viewPort.TransformToPixels(graphToolsToDrawList[i].pt3);
                        pt2 = seriesToDrawList[0].viewPort.TransformToPixels(graphToolsToDrawList[i].pt4);
                        g.DrawLine(Pens.Black, pt1, pt2);
                        tempRect.Width = tempRect.Width / 2;
                        tempRect.Height = tempRect.Height/2;
                        tempRect.Offset(tempRect.Width / 2,  tempRect.Height / 2);


                        radiusTemp = (float)Math.Sqrt(Math.Pow((pt1.Y - pt2.Y), 2) + Math.Pow((pt1.X - pt2.X), 2));

                        angleTemp = Math.Asin((pt2.Y - pt1.Y) / radiusTemp);
                        if (pt1.X > pt2.X)
                            angleTemp = Math.PI - angleTemp;

                        g.DrawArc(Pens.Gray, tempRect, (float)(angleTemp * 180 / Math.PI) - 10, 20);

                        break;
                    case DrawingTool.Arc:
                        pt1 = seriesToDrawList[0].viewPort.TransformToPixels(graphToolsToDrawList[i].pt1);
                        pt2 = seriesToDrawList[0].viewPort.TransformToPixels(graphToolsToDrawList[i].pt2);
                        tempRect = new RectangleF(pt1.X, pt1.Y, pt2.X - pt1.X, pt2.Y - pt1.Y);
                        if (pt2.Y - pt1.Y <= 0 || pt2.X - pt1.X <= 0)
                            break;
                        pt1 = seriesToDrawList[0].viewPort.TransformToPixels(graphToolsToDrawList[i].pt3);
                        pt2 = seriesToDrawList[0].viewPort.TransformToPixels(graphToolsToDrawList[i].pt4);
                        radiusTemp = (float)Math.Sqrt(Math.Pow((pt1.Y - pt2.Y), 2) + Math.Pow((pt1.X - pt2.X), 2));

                        angleTemp = Math.Asin((pt2.Y - pt1.Y) / radiusTemp);
                        if (pt1.X > pt2.X)
                            angleTemp = Math.PI - angleTemp;



                        g.DrawArc(graphToolsToDrawList[i].style.pen, tempRect, (float)(angleTemp * 180 / Math.PI), 60);
                        if (graphToolsToDrawList[i].IsSelected)
                        {
                         

                            g.DrawRectangle(graphToolsToDrawList[i].style.pen, new Rectangle((int)pt1.X - 2, (int)pt1.Y - 2, 4, 4));

                            g.DrawRectangle(graphToolsToDrawList[i].style.pen, new Rectangle((int)pt1.X - 2, (int)pt1.Y - 2, 4, 4));
                            
                        }

                        break;
                }
            }
            g.SmoothingMode =  System.Drawing.Drawing2D.SmoothingMode.None;

        }
    }
}
