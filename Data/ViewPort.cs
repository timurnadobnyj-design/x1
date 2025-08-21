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
using System.Drawing.Drawing2D;
using Skilful.ModelManager;

namespace ChartV2.Data
{
   public class ViewPort
    {
       //эти переменные нужны для отсекани отрисовки графика, когда
       //вьюпорт вышел за пределы Данных как таковых.
       public float absRealPriceMax;
       public float absRealPriceMin;
       private float currentViewPortPriceMax;
       private float currentViewPortPriceMin;
       static public int maxBarInChart; 

       //private int TimeRerfeshFinishedCounter = 0;
        

        private float maxTime;
        private float minTime;

       public float VPMaxTime
        {
            get { return maxTime; }
            set
            {
                maxTime = value;                
               // LookForVisibleMinMax(1);
                
            }
        }
        public float VPMinTime
        {
            get { return minTime; }
            set
            {
                minTime = value;  
               // LookForVisibleMinMax(1);
               
            }
        }

        private int height;
        private int width;
        public float varPrice;
        public float varTime;
       // temp vars for calculation
        private Point tempPoint=Point.Empty;
       private float tempFloat = 0F;
       public int stratTime = 0;
      public int finishTime = 0;

       private float margin = 0;
       //AutoScalseMarginsPoints
       private float visibleMaxPrice;
       public float VPMaxPrice
       {
           get { return currentViewPortPriceMax; }
           set 
           {
               
               currentViewPortPriceMax = value;
               margin = (currentViewPortPriceMax - visibleMaxPrice) / visibleMaxPrice;
              // currentViewPortPriceMax = visibleMaxPrice - margin * currentViewPortPriceMax;
           }
       }
       private float visibleMinPrice;
       public float VPMinPrice
       {
           get { return currentViewPortPriceMin; }
           set {

               currentViewPortPriceMin = value;
           }
       }

       private Data.Series linkedSerie;

        public ViewPort()
        { }
        public ViewPort(Data.Series ds)
        {
            linkedSerie = ds;
            update();
        }
        public void update()
        {
            Data.Series ds = linkedSerie;

            //задёт количество баров для первоначальной отрисовки
            if (ds.data.Count < maxBarInChart)
            {
                VPMaxTime = ds.data.Count + 2;
                VPMinTime = 0;
                stratTime = 0;
                finishTime = ds.data.Count;
            }
            else
            {
                VPMaxTime = ds.data.Count + 2; //2 бара - отступ от края
                VPMinTime = ds.data.Count - maxBarInChart;
                stratTime = ds.data.Count - maxBarInChart;
                finishTime = ds.data.Count;
            }

            LookForMinMax(ds);
            VPMaxPrice = visibleMaxPrice;
            VPMinPrice = visibleMinPrice;
        }

       ///// <summary>
       ///// Метод предназначенный для сдвига видимого участка графика на заданное количество баров
       ///// </summary>
       ///// <param name="ds">серия для сдвига</param>
       ///// <param name="strart">начало для отрисовки графика (номер первого видимого бара)</param>
       ///// <param name="finish">конец отрисовки видимой части графика (номер последнего видимого бара)</param>
       // public  static void ShiftViewPortByTime(Data.Series ds,int start, int finish)
       // {
       //     if (finish > ds.data.Count)
       //         finish = ds.data.Count;
       //     if (finish <=0)
       //         finish = 10; 

       //     if (start < 0)
       //         start = 0;
       //     if (start >= ds.data.Count)
       //         start = ds.data.Count - 10;
               
            
       //     ds.viewPort.VisibleMaxPrice = 0;
       //     ds.viewPort.VisibleMinPrice = float.PositiveInfinity;
            
           
       //         ds.viewPort.VisibleMinTime = start;
           
       //         ds.viewPort.VisibleMaxTime = finish;
            
       //     for (int i = start; i < finish; i++)
       //     {
       //         if (i > ds.viewPort.VisibleMinTime)
       //         {
       //             if (ds.viewPort.VisibleMaxPrice < (float)ds.data[i].High)
       //             {
       //                 ds.viewPort.VisibleMaxPrice = (float)ds.data[i].High;
       //                 //ds.viewPort.VisibleMaxPrice.X = i;
       //             }
       //             if (ds.viewPort.VisibleMinPrice > (float)ds.data[i].Low)
       //             {
       //                 ds.viewPort.VisibleMinPrice = (float)ds.data[i].Low;
       //               //  ds.viewPort.VisibleMinPrice.X = i;
       //             }
       //         }

       //     }
       //     ds.viewPort.varPrice = ds.viewPort.VisibleMaxPrice - ds.viewPort.VisibleMinPrice;
       // }
       
        public void AutoMarginLooking()
        {

            finishTime = (int)linkedSerie.viewPort.VPMaxTime+1;
            stratTime =(int) linkedSerie.viewPort.VPMinTime;

            if (finishTime > linkedSerie.data.Count)    finishTime = linkedSerie.data.Count;
            if (finishTime < 0) finishTime = 0;

            if (linkedSerie.viewPort.VPMinTime < 0)
              stratTime = 0;
            if (linkedSerie.viewPort.VPMinTime >= linkedSerie.data.Count)
                stratTime = linkedSerie.data.Count-1;
            
            
            linkedSerie.viewPort.visibleMaxPrice = float.NegativeInfinity;
            linkedSerie.viewPort.visibleMinPrice = absRealPriceMax;

            for (int i = stratTime; i < finishTime; i++)
            {
                if (i > linkedSerie.viewPort.VPMinTime)
                {
                    if (linkedSerie.viewPort.visibleMaxPrice < (float)linkedSerie.data[i].High)
                    {
                        linkedSerie.viewPort.visibleMaxPrice = (float)linkedSerie.data[i].High;
                       // linkedSerie.viewPort.VisibleMaxPrice.X = i;

                    }
                    if (linkedSerie.viewPort.visibleMinPrice > (float)linkedSerie.data[i].Low)
                    {
                        linkedSerie.viewPort.visibleMinPrice = (float)linkedSerie.data[i].Low;
                       // linkedSerie.viewPort.VisibleMinPrice.X = i;
                    }
                }

            }
            if (!float.IsInfinity(margin) && margin != 0 &&  !float.IsNaN(margin) && visibleMaxPrice != 0 && visibleMinPrice != 0)
            {
                linkedSerie.viewPort.VPMaxPrice = visibleMaxPrice + margin * visibleMaxPrice;
                linkedSerie.viewPort.VPMinPrice = visibleMinPrice - margin * visibleMinPrice;
            }
            else
            {
                linkedSerie.viewPort.VPMaxPrice = visibleMaxPrice;
                linkedSerie.viewPort.VPMinPrice = visibleMinPrice;
            }
            linkedSerie.viewPort.varPrice = linkedSerie.viewPort.VPMaxPrice - linkedSerie.viewPort.VPMinPrice;

        }
       
       public void LookForMinMax(Data.Series ds)
        {
            absRealPriceMax = float.NegativeInfinity;
            absRealPriceMin = float.PositiveInfinity;
            visibleMaxPrice = float.NegativeInfinity;
            visibleMinPrice = float.PositiveInfinity;
            //зададим отступ в 5 пунктов
            //double dig = (ds.data[0].High-Math.Truncate(ds.data[0].High))*100;
            double padding = 5 * (ds.info != null ? ds.info.Symbol.pip : 0.0005);
            //внутренний цикл для расчета масштаба(макс-мин) на участве графика, 
            //предназначенного для первоначальной отрисовки.
            for (int i = 1; i < ds.data.Count; i++)
            {
                double lastBarHigh = (double)ds.data[i].High,
                       lastBarLow = (double)ds.data[i].Low;
                if ((ds.info!=null? ds.info.log: ds.legend.log))
                {
                    lastBarHigh = Math.Log10(Math.Pow(10, lastBarHigh) + padding);
                    lastBarLow = Math.Log10(Math.Pow(10, lastBarLow) - padding);
                }
                else{
                    lastBarHigh = (double)ds.data[i].High + padding;
                    lastBarLow = (double)ds.data[i].Low - padding;
                }
                if (i > VPMinTime)
                {
                    if (visibleMaxPrice < lastBarHigh)
                    {
                        visibleMaxPrice = (float)lastBarHigh;
                       // VisibleMaxPrice.X = i;
                       
                    }
                    if (visibleMinPrice > lastBarLow)
                    {
                        visibleMinPrice = (float)lastBarLow;
                       // VisibleMinPrice.X = i;
                        
                    }
                }


                if (absRealPriceMax < (float)ds.data[i].High)
                    absRealPriceMax = (float)ds.data[i].High;

                if (absRealPriceMin > (float)ds.data[i].Low)
                    absRealPriceMin = (float)ds.data[i].Low;
            }

        }
       /// <summary>
       /// Эта функция для рачета разницы мин и макс значения в функциях трансформации
       /// </summary>
       /// <param name="bmp">Битмап с (возможно) новыми размерами</param>
        public void PrepareTransformFunctions(Bitmap bmp)
        {



            if (linkedSerie.viewPort.VPMaxTime >= linkedSerie.data.Count)
                finishTime = linkedSerie.data.Count;
            else
                finishTime = (int) linkedSerie.viewPort.VPMaxTime+1;


            if (linkedSerie.viewPort.VPMinTime < 2)
                stratTime = 2;
            else
                stratTime = (int) linkedSerie.viewPort.VPMinTime;
           
            width = bmp.Width;
            height = bmp.Height;

            varTime = (width) / (this.VPMaxTime - this.VPMinTime);
            varPrice = (height) / (this.VPMaxPrice - this.VPMinPrice);
          
        }
        /// <summary>
        /// Эта функция для рачета разницы мин и макс значения в функциях трансформации
        /// </summary>
        /// <param name="bmp">Битмап с (возможно) новыми размерами</param>
        public void PrepareTransformFunctionsAutoMargins(Bitmap bmp)
        { 
            if(linkedSerie.viewPort.VPMaxTime >= linkedSerie.data.Count)
                finishTime = linkedSerie.data.Count;
            else
                finishTime = (int) linkedSerie.viewPort.VPMaxTime+1;


            if (linkedSerie.viewPort.VPMinTime < 2)
                stratTime = 2;
            else
                stratTime = (int) linkedSerie.viewPort.VPMinTime;
           

            width = bmp.Width;
            height = bmp.Height;
            AutoMarginLooking();

            varTime = (width) / (this.VPMaxTime - this.VPMinTime);
            varPrice = (height) / (this.VPMaxPrice - this.VPMinPrice);


        }



        public float PixelToPrice(float pt)
        {
            tempFloat = (height - pt) / varPrice + this.VPMinPrice;
            return tempFloat;
        }
        public float PriceToPixels(float pt)
        {
            tempFloat = height - (pt - this.VPMinPrice) * varPrice;
            if (float.IsNaN(tempFloat) || float.IsInfinity(tempFloat))
                return 0;

            return tempFloat;

        }
        public float BarNumberToPixels(float pt)
        {
            tempFloat = (pt - this.VPMinTime) * varTime;
            return tempFloat;
        }
        public float PixelToBarNumber(float pt)
        {
            tempFloat = pt / varTime + this.VPMinTime;
            return tempFloat;

        }
        public Point TransformToPixels(PointF pt)
        {

            // ниже это дожно быть (width - 0) т.е. максимальный размах холста
            //полный расчет 
            //tempPoint.X = (int)(((pt.X  - this.minTime) / (this.maxTime - this.minTime))* (bmp.Width)) ;
            tempPoint.X = (int)((pt.X - this.VPMinTime) * varTime);
            if (tempPoint.X == int.MinValue)
                tempPoint.X = 0;

            // ниже это дожно быть (хейт - 0) т.е. максимальный размах холста
            tempPoint.Y = (int)(height - (pt.Y - this.VPMinPrice) * varPrice);
            if (tempPoint.Y == int.MinValue)
                tempPoint.Y = 0;

            return tempPoint;
        }
        public Border TransformToPixels(BorderF pt)
        {
            Border bord;
            bord.xa = (int)((pt.xa - this.VPMinTime) * varTime);
            if (bord.xa == int.MinValue) bord.xa = 0;

            bord.xb = (int)((pt.xb - this.VPMinTime) * varTime);
            if (bord.xb == int.MinValue) bord.xb = 0;

            bord.y = (int)(height - (pt.y - this.VPMinPrice) * varPrice);
            if (bord.y == int.MinValue)  bord.y = 0;

            return bord;
        }
    }
}
