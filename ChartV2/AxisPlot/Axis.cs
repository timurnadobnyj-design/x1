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
using Skilful.QuotesManager;

namespace ChartV2.Axis_Plot
{
    public abstract class Axis
    {
        public Styles.AxisStyle style;
        public List<Data.Series> list;

        public enum PeriodToDraw {years,months,weeks, days, hours, minutes, seconds, chartIsOutSideTheBonds }
        public PeriodToDraw period = PeriodToDraw.seconds;
        public DateTime start1;
        public int startTime;
        public DateTime end1;
        public TimeSpan res1;
        public float timeMin;
        public float timeMax;
        public int BarNumber;

        public string dateTimeFormat = "";

  
        //переменная для пеервода промежуточных значений цены в логшкалу.
        public float s;
        public string price_format;

        public SizeF lableSize;
        public float howManyCanFit;

        //Bools
        public bool ShouldDrawBorder;
        public bool IsVisible;

        public CursorInfoLable cursorLable;
        public Dictionary<int, CursorInfoLable> targetLable = new Dictionary<int, CursorInfoLable>();
        public bool IsTargetsVisible = false;
        public int barIndex = 0;

        //BMPs
        public Bitmap bmp;
        public Graphics g;
        public Grid grid;
        //temp vars
        public float tempCoord;
        public float start;
        public float finish;
        public static string tempString;
        //series index
        public int ind = 0;
        //для хранения коодинаты мыши
        public int mouseCursorXcoordinate;
        //Methods
        public abstract void Draw(int? seriesIndex);
        public abstract void refreshDrawingSurfaces(Rectangle gridSplitterRectangle);
        public int GetPrecisionOf(float N, bool CountOnlyZeros)
        {
            if (N == 0)
                return 0;
            //  System.Globalization.NumberFormatInfo nfi = new System.Globalization.CultureInfo("ru-RU").NumberFormat;
            //  nfi.NumberDecimalDigits = 10;


            string temp = Convert.ToString(N, null);
            if (temp.Contains("E"))
            {
                temp = temp.Substring(temp.IndexOf('E') + 2, 2);
                return int.Parse(temp) * (-1);
            }
            int pescision = 0;

            if (N < 1)
            {
                pescision = -1;
                temp = temp.Remove(0, 2);
                while (temp.StartsWith("0"))
                {
                    temp = temp.Remove(0, 1);
                    pescision--;
                }

            }
            else
            {
                if (temp.Contains(",") )
                {
                    while (!temp.StartsWith(","))
                    {

                        pescision++;
                        temp = temp.Remove(0, 1);
                    }
                }else if (temp.Contains("."))
                {
                    while (!temp.StartsWith("."))
                    {

                        pescision++;
                        temp = temp.Remove(0, 1);
                    }
                }
                else
                {
                    for (int i = 0; i < temp.Length; i++)
                    {

                        pescision++;

                    }
                }


            }
            return pescision;
        }
        public static string GetCheckedString(int time)
        {
            tempString = "";
            switch (time.ToString().Length)
            {
                case 1:
                    //время = 00:00:0X
                    if (time == 0)
                    {
                        tempString = "000000";
                        return tempString;
                    }
                    else
                        tempString = "00000";
                    break;
                case 2:
                    //время = 00:00:XX
                    tempString = "0000";
                    break;
                case 3:
                    //время = 00:0X:XX
                    tempString = "000";
                    break;
                case 4:
                    //время = 00:XX:0X
                    tempString = "00";
                    break;
                case 5:
                    //время = 0X:XX:0X
                    tempString = "0";
                    break;

            }
            return tempString + time.ToString();
        }
    }


    public class TopAxisAsInfoPanel : Axis
    {

        private DateTime dt;
        Control parentControl;
        public TopAxisAsInfoPanel(Rectangle rect, Control control)
        {
            parentControl = control;
            style =  new Styles.AxisStyle(Color.WhiteSmoke, Brushes.Black, FontFamily.GenericSansSerif, 9, FontStyle.Regular);
            style.magorTickFont = Skilful.Sample.AxisStyleTop_Font;
            refreshDrawingSurfaces(rect);
        }
        
        public override void refreshDrawingSurfaces(Rectangle gridSplitterRectangle)
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


        

        public override void Draw(int? seriesIndex)
        {

            if (list == null)
                return;
            if (list.Count == 0)
            {
                g.DrawString("No series Selected.", SystemFonts.DialogFont, Brushes.Beige, Point.Empty);
            }
            else
            {
                ind = (int)seriesIndex;
            }

            price_format = list[ind].legend.price_format;

 
            //just to check numbers height
            g.Clear(style.backColor);

            if (ShouldDrawBorder)
                g.DrawRectangle(style.borderPen, 0, 0, bmp.Width - 1, bmp.Height - 1);

            barIndex = (int)list[ind].viewPort.PixelToBarNumber(mouseCursorXcoordinate);
            
            //if (barIndex < 0)
            //    barIndex = 0;


            dt = list[ind].barIndex2Date(barIndex);
            
            dateTimeFormat = "dd.MM.yy" + " HH:mm";

            string str = " Bar:" + barIndex.ToString() + " Date: " + dt.ToString("dd.MM.yyyy") + " Time: " + dt.ToString("HH:mm") + " |";

            if (barIndex >= list[ind].data.Count)
                barIndex = list[ind].data.Count-1;
            if (barIndex < 0)
                barIndex = 0;

            if (list[ind].legend.log)
            {
                try
                {
                    str += " O:" + (Math.Pow(10, (double)list[ind].data[barIndex].Open)).ToString(price_format)
                        + " H:" + (Math.Pow(10, (double)list[ind].data[barIndex].High)).ToString(price_format)
                        + " L:" + (Math.Pow(10, (double)list[ind].data[barIndex].Low)).ToString(price_format)
                        + " C:" + (Math.Pow(10, (double)list[ind].data[barIndex].Close)).ToString(price_format);
                }
                catch { str += ""; }
            }
            else
            {
                try
                {
                    str += " O:" + list[ind].data[barIndex].Open.ToString(price_format)
                        + " H:" + list[ind].data[barIndex].High.ToString(price_format)
                        + " L:" + list[ind].data[barIndex].Low.ToString(price_format)
                        + " C:" + list[ind].data[barIndex].Close.ToString(price_format);
                }
                catch { str += ""; }
            }
            //--- удаляет #,_ из названия
            string uglyname = list[ind].legend.symbol;
            if (uglyname.IndexOfAny(new char[]{'#','_'},0,1) == 0)
            {
                uglyname = uglyname.Substring(1);
            }
            str += "  [" + uglyname;
            //---
            //str += "  [" + list[ind].legend.symbol;
            if (list[ind].legend.frame != TF.custom) str += ":" + list[ind].legend.frame;
            try
            {
                str += "]" + " " + list[ind].data[barIndex].whatExtremum();
            }
            catch { str += "]"; } 
            g.DrawString(str, style.magorTickFont, style.magorFontBrush, Point.Empty);

            //вывод реалтайм тиков
            float shift = g.MeasureString(str, style.magorTickFont).Width;
            //str = "VPMinTime " + list[ind].viewPort.VPMinTime + " VPMaxTime " + list[ind].viewPort.VPMaxTime + " ";
            str = (list[ind].lastBid > 0) ?
                "Date: " + list[ind].lastTime.ToString("dd.MM.yy ") + "Time: " + list[ind].lastTime.ToString("HH:mm:ss | ") +
                "Bid: " + list[ind].lastBid.ToString(price_format) + " Ask: " + list[ind].lastAsk.ToString(price_format) : "offline";

            SizeF rect = g.MeasureString(str, style.magorTickFont);
            parentControl.SetTopAxisHeight((int)((shift + rect.Width) < bmp.Width ? (rect.Height+2) : (rect.Height * 2)));
            g.DrawString(str, style.magorTickFont, style.magorFontBrush, bmp.Width - rect.Width, (shift + rect.Width) < bmp.Width ? 0 : rect.Height);
        }
    }


    public class BottomAxisDinamic : Axis
    {
        public bool shouldDrawAxisRectangle = true;


        private int del = 2;

        private int stringWidth = 0;
        private int strHeight = 0;
        private float TempCoordX = 0;
        private int lastIndex= 0;
        private int ii = 0;
        private string highFrecTime = "";
        private string lowFrecTime = "";
        private string lastHighFrecTime = "";
        private string lastLowFrecTime = "";
        

         
        public BottomAxisDinamic(Rectangle rect)
        {
            cursorLable = new CursorInfoLable();    
            style = new Styles.AxisStyle();
          //  style.magorTickFont = Skilful.Sample.AxisStyleBottom;
            cursorLable.font = style.magorTickFont;
            refreshDrawingSurfaces(rect);
        }
        public override void refreshDrawingSurfaces(Rectangle gridSplitterRectangle)
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
         
        public override void Draw(int? seriesIndex)
         {
                if (list == null)
                return;
           //  public void DrawXaxis(DataSeries ds)

                if (list == null)
                    return;
                if (list.Count == 0)
                {

                    g.DrawString("No series Selected.", SystemFonts.DialogFont, Brushes.Beige, Point.Empty);
                }
                else
                {
                    ind = (int)seriesIndex;
                }



                //just to check numbers height
                g.Clear(style.backColor);

                if (ShouldDrawBorder)
                    g.DrawRectangle(style.borderPen, 0, 0, bmp.Width - 1, bmp.Height - 1);
/////////////////////////////////////////////////////////////////////////
            tempString = ":10m";
            stringWidth = (int)g.MeasureString(tempString, style.magorTickFont).Width;
            howManyCanFit = bmp.Width / stringWidth;
            strHeight = (int)g.MeasureString(tempString, style.magorTickFont).Height;

            g.DrawLine(style.magorTickPen, 0, strHeight, bmp.Width, strHeight);
            g.DrawLine(style.magorTickPen, 0, strHeight * 2, bmp.Width, strHeight * 2);

                #region NoTimeArray

                timeMin = list[ind].viewPort.VPMinTime;
                timeMax = list[ind].viewPort.VPMaxTime;


                float pointsPerLable = (timeMax - timeMin) / howManyCanFit;


                TempCoordX = 0;

                if (timeMin < 0)
                    startTime = 0;
                else
                    startTime = (int)timeMin;
                grid.verticalLevelsArray.Clear();
                lastIndex = 0 - (int)pointsPerLable - 1;
                ii = 0;


                if (timeMax > list[ind].data.Count)
                {
                    if (startTime < list[ind].data.Count && list[ind].data.Count > 0)
                    {


                        //tempString = list[ind].data[startTime].d[0].ToString() + GetCheckedString((int)list[ind].data[startTime].d[1]);
                        //start1 = DateTime.ParseExact(tempString, "yyyyMMddHHmmss", null);
                        start1 = list[ind].data[startTime].DT;
                        //tempString = list[ind].data[list[ind].data.Count - 1].d[0].ToString() + GetCheckedString((int)list[ind].data[startTime].d[1]);
                        //end1 = DateTime.ParseExact(tempString, "yyyyMMddHHmmss", null);
                        end1 = list[ind].barIndex2Date((int)timeMax);
                        res1 = end1 - start1;
                        if (res1.Seconds > 1)
                            period = PeriodToDraw.seconds;
                        if (res1.Minutes > 2)
                            period = PeriodToDraw.minutes;
                        if (res1.Hours > 2)
                            period = PeriodToDraw.hours;
                        if (res1.Days > 0 || res1.Hours > 10)
                            period = PeriodToDraw.days;
                    }

                }
                if (timeMax <= list[ind].data.Count)
                {
                    if (startTime < list[ind].data.Count && ((int)timeMax - 1) > 0)
                    {



                        //tempString = list[ind].data[startTime].d[0].ToString() + GetCheckedString((int)list[ind].data[startTime].d[1]);
                        //start1 = DateTime.ParseExact(tempString, "yyyyMMddHHmmss", null);
                        start1 = list[ind].data[startTime].DT;

                        //tempString = list[ind].data[(int)timeMax - 1].d[0].ToString() + GetCheckedString((int)list[ind].data[startTime].d[1]);
                        //end1 = DateTime.ParseExact(tempString, "yyyyMMddHHmmss", null);
                        end1 = list[ind].data[(int)timeMax - 1].DT;

                        res1 = end1 - start1;
                        if (res1.Seconds > 1)
                            period = PeriodToDraw.seconds;
                        if (res1.Minutes > 2)
                            period = PeriodToDraw.minutes;
                        if (res1.Hours > 2)
                            period = PeriodToDraw.hours;
                        if (res1.Days > 0 || res1.Hours > 10)
                            period = PeriodToDraw.days;
                    }
                }


                switch (period)
                {
                    #region Seconds
                    case PeriodToDraw.seconds:
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        for (int i = startTime; ((i < timeMax) && (i < list[ind].data.Count)); i++)
                        {
                            //tempString = list[ind].data[i].d[1].ToString();
                            //highFrecTime = tempString.Substring(tempString.Length - 2, 2);
                            highFrecTime = list[ind].data[i].DT.Second.ToString();
                            //lowFrecTime = tempString.Substring(tempString.Length - 4, 2);
                            lowFrecTime = list[ind].data[i].DT.Minute.ToString();

                            if (lastHighFrecTime != highFrecTime)
                            {
                                if (i != 0)
                                    ii = i;
                                else
                                    goto skip;
                                while (list[ind].data[ii].DT.TimeOfDay == list[ind].data[i].DT.TimeOfDay)
                                {
                                    ii--;
                                    if (ii < 0)
                                        break;
                                }
                            skip:
                                if (ii - lastIndex > pointsPerLable)
                                {
                                    lastHighFrecTime = highFrecTime;
                                    lastIndex = i;
                                    TempCoordX = list[ind].viewPort.BarNumberToPixels(i);
////
                                    ////
                                    tempString = list[ind].data[ii].DT.TimeOfDay.ToString();
                                    g.DrawLine(style.magorTickPen, TempCoordX, strHeight, TempCoordX, 10);
                                    g.DrawString(":" + highFrecTime.ToString() + "s", style.magorTickFont, style.magorFontBrush, TempCoordX + stringWidth / 6, 0);
                                    if (lastLowFrecTime != lowFrecTime)
                                    {
                                        lastLowFrecTime = lowFrecTime;
                                        g.DrawLine(style.magorTickPen, TempCoordX, strHeight, TempCoordX, strHeight + 10);
                                        g.DrawString(":" + lowFrecTime.ToString() + "m", style.magorTickFont, style.magorFontBrush, TempCoordX + stringWidth / 6, strHeight);
                                    }
                                    if (grid.style.IsVerticalVisible)
                                        grid.verticalLevelsArray.Add(TempCoordX);
                                  
                                }
                            }
                        }

                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        break;
                #endregion Seconds
                  
                    #region Minutes
                    case PeriodToDraw.minutes:
                        for (int i = startTime; ((i < timeMax) && (i < list[ind].data.Count)); i++)
                        {
                            //tempString = list[ind].data[i].DT.TimeOfDay.ToString();
                            //highFrecTime = tempString.Substring(tempString.Length - 4, 2);
                            highFrecTime = list[ind].data[i].DT.Minute.ToString();


                            lowFrecTime = list[ind].data[i].DT.Hour.ToString();
                            

                            if (i - lastIndex > pointsPerLable)
                            {
                                lastIndex = i;
                                TempCoordX = list[ind].viewPort.BarNumberToPixels(i);
                                g.DrawLine(style.magorTickPen, TempCoordX, strHeight, TempCoordX, 10);
                                g.DrawString(":" + highFrecTime.ToString() + "m", style.magorTickFont, style.magorFontBrush, TempCoordX + stringWidth / 6, 0);
                                if (lastLowFrecTime != lowFrecTime)
                                {
                                    lastLowFrecTime = lowFrecTime;
                                    g.DrawLine(style.magorTickPen, TempCoordX, strHeight, TempCoordX, strHeight + 10);
                                    g.DrawString(":" + lowFrecTime.ToString() + "h", style.magorTickFont, style.magorFontBrush, TempCoordX + stringWidth / 6, strHeight);
                                }
                                if (grid.style.IsVerticalVisible)
                                    grid.verticalLevelsArray.Add(TempCoordX);
                            }
                        }
                        break;
                    #endregion Minutes

                    #region Hours
                    case PeriodToDraw.hours:
                        for (int i = startTime; ((i < timeMax) && (i < list[ind].data.Count)); i++)
                        {
                            //tempString = list[ind].data[i].d[0].ToString() + list[ind].data[i].d[1].ToString();
                            //highFrecTime = tempString.Substring(tempString.Length - 6, 2);
                            highFrecTime = list[ind].data[i].DT.Hour.ToString();
                            //lowFrecTime = tempString.Substring(tempString.Length - 8, 2);
                            lowFrecTime = list[ind].data[i].DT.Day.ToString();

                            if (lastHighFrecTime != highFrecTime)
                            {
                                lastHighFrecTime = highFrecTime;
                                if (i - lastIndex > pointsPerLable)
                                {
                                    lastIndex = i;
                                    TempCoordX = list[ind].viewPort.BarNumberToPixels(i);
                                    g.DrawLine(style.magorTickPen, TempCoordX, strHeight, TempCoordX, 10);
                                    g.DrawString(":" + highFrecTime.ToString() + "h", style.magorTickFont, style.magorFontBrush, TempCoordX + stringWidth / 6, 0);
                                    if (lastLowFrecTime != lowFrecTime)
                                    {
                                        lastLowFrecTime = lowFrecTime;
                                        g.DrawLine(style.magorTickPen, TempCoordX, strHeight, TempCoordX, strHeight + 10);
                                        g.DrawString(lowFrecTime.ToString() + "d", style.magorTickFont, style.magorFontBrush, TempCoordX + stringWidth / 6, strHeight);
                                    }
                                    if (grid.style.IsVerticalVisible)
                                        grid.verticalLevelsArray.Add(TempCoordX);
                                }
                            }
                        }
                        break;
                    #endregion Hours

                    #region Days
                    case PeriodToDraw.days:
                        for (int i = startTime; ((i < timeMax) && (i < list[ind].data.Count)); i++)
                        {
                            //tempString = list[ind].data[i].d[0].ToString() + list[ind].data[i].d[1].ToString();
                            //highFrecTime = tempString.Substring(tempString.Length - 6, 2);
                            highFrecTime = list[ind].data[i].DT.Hour.ToString();
                            //lowFrecTime = tempString.Substring(tempString.Length - 8, 2);
                            lowFrecTime = list[ind].data[i].DT.Day.ToString();

                            if (lastHighFrecTime != highFrecTime)
                            {
                                lastHighFrecTime = highFrecTime;
                                if (i - lastIndex > pointsPerLable)
                                {
                                    lastIndex = i;
                                    TempCoordX = list[ind].viewPort.BarNumberToPixels(i);
                                    g.DrawLine(style.magorTickPen, TempCoordX, strHeight, TempCoordX, 10);
                                    g.DrawString(":" + highFrecTime.ToString() + "h", style.magorTickFont, style.magorFontBrush, TempCoordX + stringWidth / 6, 0);
                                    if (lastLowFrecTime != lowFrecTime)
                                    {
                                        lastLowFrecTime = lowFrecTime;
                                        g.DrawLine(style.magorTickPen, TempCoordX, strHeight, TempCoordX, strHeight + 10);
                                        g.DrawString(lowFrecTime.ToString() + "d", style.magorTickFont, style.magorFontBrush, TempCoordX + stringWidth / 6, strHeight);
                                    }
                                    if (grid.style.IsVerticalVisible)
                                        grid.verticalLevelsArray.Add(TempCoordX);
                                }
                            }
                        }
                        break;
                    #endregion Days

                    case PeriodToDraw.chartIsOutSideTheBonds:
                        break;

                    default:
                        break;
                }



                if (list[ind].viewPort.VPMaxTime - list[ind].viewPort.VPMinTime > 0)
                {
                    del -= 2;
                    if (del > 0)
                        del = 2;
                }

                #endregion NoTimeArray

               

            }

         }
    public class BottomAxisStatic : Axis 
    {
        public BottomAxisStatic(Rectangle rect)
        {
            cursorLable = new CursorInfoLable();
            style = new Styles.AxisStyle();
            style.magorTickFont = Skilful.Sample.AxisStyleBottom_Font;
            cursorLable.font = Skilful.Sample.CursorLable_Bottom_Font;   //style.magorTickFont;
            refreshDrawingSurfaces(rect);
        }
        public override void refreshDrawingSurfaces(Rectangle gridSplitterRectangle)
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
      
        public override void Draw(int? seriesIndex)
         {
                if (list == null)
                return;
           //  public void DrawXaxis(DataSeries ds)

                if (list == null)
                    return;
                if (list.Count == 0)
                {

                    g.DrawString("No series Selected.", SystemFonts.DialogFont, Brushes.Beige, Point.Empty);
                }
                else
                {
                    ind = (int)seriesIndex;
                }



                //just to check numbers height
                g.Clear(style.backColor);

                if (ShouldDrawBorder)
                    g.DrawRectangle(style.borderPen, 0, 0, bmp.Width - 1, bmp.Height - 1);

                try
                {
                    lableSize.Width = g.MeasureString(list[ind].data[0].DT.Date.ToString(), style.magorTickFont).Width;// + list[ind].data[0].DT.TimeOfDay.ToString(), style.magorTickFont).Width;
                }
                catch
                {
                    lableSize.Width = g.MeasureString("0000-00-00", style.magorTickFont).Width;// + list[ind].data[0].DT.TimeOfDay.ToString(), style.magorTickFont).Width;
                }

                howManyCanFit = bmp.Width / lableSize.Width;

                grid.verticalLevelsArray.Clear();


                timeMin = list[ind].viewPort.VPMinTime;
                timeMax = list[ind].viewPort.VPMaxTime;
               

                if (timeMin < 0)
                    startTime = 0;
                else
                    startTime = (int)timeMin;


                if (timeMax > list[ind].data.Count)
                {
                    if (startTime < list[ind].data.Count && list[ind].data.Count > 0)
                    {


                        //tempString = list[ind].data[startTime].d[0].ToString() + GetCheckedString((int)list[ind].data[startTime].d[1]);
                        //start1 = DateTime.ParseExact(tempString, "yyyyMMddHHmmss", null);
                        start1 = list[ind].data[startTime].DT;

                        end1 = list[ind].barIndex2Date((int)timeMax);
                        res1 = end1 - start1;
                        if (res1.Seconds > 1)
                            period = PeriodToDraw.seconds;
                        if (res1.Minutes > 2)
                            period = PeriodToDraw.minutes;
                        if (res1.Hours > 2)
                            period = PeriodToDraw.hours;
                        if (res1.Days > 2)
                            period = PeriodToDraw.days;
                        if (res1.Days > 10)
                            period = PeriodToDraw.weeks;
                        if (res1.Days > 30)
                            period = PeriodToDraw.months;
                        if (res1.Days > 365)
                            period = PeriodToDraw.years;
                    }

                }
                if (timeMax <= list[ind].data.Count)
                {
                    if (startTime < list[ind].data.Count && ((int)timeMax - 1) > 0)
                    {



                        //tempString = list[ind].data[startTime].d[0].ToString() + GetCheckedString((int)list[ind].data[startTime].d[1]);
                        //start1 = DateTime.ParseExact(tempString, "yyyyMMddHHmmss", null);
                        start1 = list[ind].data[startTime].DT;

                        //tempString = list[ind].data[(int)timeMax - 1].d[0].ToString() + GetCheckedString((int)list[ind].data[(int)timeMax - 1].d[1]);
                        //end1 = DateTime.ParseExact(tempString, "yyyyMMddHHmmss", null);
                        end1 = list[ind].data[(int)timeMax - 1].DT;

                        TimeSpan span = end1.Subtract(start1);
                        res1 = end1 - start1;
                        if (res1.Seconds > 1)
                            period = PeriodToDraw.seconds;
                        if (res1.Minutes > 2)
                            period = PeriodToDraw.minutes;
                        if (res1.Hours > 2)
                            period = PeriodToDraw.hours;
                        if (res1.Days > 2)
                            period = PeriodToDraw.days;
                        if (res1.Days > 10)
                            period = PeriodToDraw.weeks;
                        if (res1.Days > 30)
                            period = PeriodToDraw.months;
                        if (res1.Days > 365)
                            period = PeriodToDraw.years;
                    }
                }




                int newMonth = 0;
                int newDay = 0;
                        for (float i = 0; i < howManyCanFit + 10; i++)
                        {
                            BarNumber = (int)list[ind].viewPort.PixelToBarNumber(i * lableSize.Width);
                            if (BarNumber < list[ind].data.Count && BarNumber > 0)
                            {
                                //tempString = list[ind].data[BarNumber].d[0].ToString() + GetCheckedString((int)list[ind].data[BarNumber].d[1]);
                                //start1 = DateTime.ParseExact(tempString, "yyyyMMddHHmmss", null);
                                start1 = list[ind].data[BarNumber].DT;
                                float temp = i * lableSize.Width ;
                                for (int ii = 0; ii < 5; ii++)
                                {
                                    g.DrawLine(style.magorTickPen, temp + ii * lableSize.Width / 5, 0, temp+ii * lableSize.Width / 5, 5);
                                }
                                g.DrawLine(style.magorTickPen, temp, 0, temp, 5);
                            }
                            else
                            {
                                start1 = list[ind].barIndex2Date(BarNumber);
                                
                                g.DrawLine(style.magorTickPen, i * lableSize.Width, 0, i * lableSize.Width, 5);
                            
                            }
                                switch (period)
                                {
                                    case PeriodToDraw.minutes:
                                        dateTimeFormat = "mm:ss";
                                       
                                        g.DrawString(start1.ToString("mm:ss"), style.magorTickFont, style.magorFontBrush, i * lableSize.Width, 5);
                                        if (newDay < start1.Day)
                                        {
                                            newDay = start1.Day;
                                            g.DrawString(start1.ToString("HH") + "h", style.magorTickFont, style.magorFontBrush, i * lableSize.Width, 15);
                                        }
                                        break;

                                    case PeriodToDraw.hours:

                                        //if (newDay < start1.Day)
                                        if (i == 0)
                                        {
                                            dateTimeFormat = "HH:mm";
                                            newDay = start1.Day;
                                            g.DrawString(start1.ToString("dd MMM yyyy"), style.magorTickFont, style.magorFontBrush, i * lableSize.Width, 5);
                                        }
                                        else g.DrawString(start1.ToString("HH:mm"), style.magorTickFont, style.magorFontBrush, i * lableSize.Width, 5);
                                        break;
                                    case PeriodToDraw.days:
                                        //if (newMonth < start1.Month)
                                        if (i == 0)
                                        {
                                            newMonth = start1.Month;
                                            g.DrawString(start1.ToString("dd MMM yyyy"), style.magorTickFont, style.magorFontBrush, i * lableSize.Width, 5);
                                        }
                                        else
                                        {
                                            dateTimeFormat = "dd MMM HH:mm";
                                            g.DrawString(start1.ToString("dd MMM HH:mm"), style.magorTickFont, style.magorFontBrush, i * lableSize.Width, 5);

                                        }
                                        break;
                                    case PeriodToDraw.weeks:

                                        if (i == 0)
                                        // if(newMonth < start1.Month)
                                        {
                                            newMonth = start1.Month;
                                            g.DrawString(start1.ToString("dd MMM yyyy"), style.magorTickFont, style.magorFontBrush, i * lableSize.Width, 5);
                                        }
                                        else g.DrawString(start1.ToString("dd MMM HH") + "h", style.magorTickFont, style.magorFontBrush, i * lableSize.Width, 5);
                                        break;
                                    case PeriodToDraw.months:

                                        //if (newMonth < start1.Month)
                                        if (i == 0)
                                        {
                                            newMonth = start1.Month;
                                            g.DrawString(start1.ToString("dd MMM yyyy"), style.magorTickFont, style.magorFontBrush, i * lableSize.Width, 5);
                                        }
                                        else g.DrawString(start1.ToString("dd MMM HH") + "h", style.magorTickFont, style.magorFontBrush, i * lableSize.Width, 5);
                                        break;
                                    case PeriodToDraw.years:
                                        g.DrawString(start1.ToString("yyyy MMM"), style.magorTickFont, style.magorFontBrush, i * lableSize.Width, 5);
                                        break;

                                }

                                if (grid.style.IsVerticalVisible)
                                    grid.verticalLevelsArray.Add(i * lableSize.Width);
                            
                            
                        }




                        if (cursorLable.IsVisible && cursorLable != null)
                        {
                            barIndex = (int)cursorLable.Value;
                            /*if (barIndex >= list[ind].data.Count)
                                barIndex = list[ind].data.Count - 1;*/
                            //if (barIndex < 0)
                            //    barIndex = 0;
                            cursorLable.rect.Size = g.MeasureString(list[ind].barIndex2Date(barIndex).ToString("HH:mm"), cursorLable.font);
                            cursorLable.rect.Offset(-cursorLable.rect.Width / 2, 0);
                            cursorLable.rect.Y = bmp.Height - 10 - cursorLable.rect.Height;
                            g.FillRectangle(cursorLable.backBrush, cursorLable.rect.X, cursorLable.rect.Y, cursorLable.rect.Width, cursorLable.rect.Height);
                            if (cursorLable.borderPen != null)
                                g.DrawRectangle(cursorLable.borderPen, cursorLable.rect.X, cursorLable.rect.Y, cursorLable.rect.Width, cursorLable.rect.Height);
                            g.DrawString(list[ind].barIndex2Date(barIndex).ToString("HH:mm"), cursorLable.font, cursorLable.fontBrush, cursorLable.rect.Location);

                        }

        }
            
    }

  
    public class LeftAxisDinamic : Axis
    {

        public LeftAxisDinamic(Rectangle rect)
        {
            cursorLable = new CursorInfoLable();
            style = new Styles.AxisStyle();
            style.magorTickFont = Skilful.Sample.AxisStyleLeft_Font;
            cursorLable.font = Skilful.Sample.CursorLable_Left_Font; // style.magorTickFont;
            refreshDrawingSurfaces(rect);

        }

        public override void refreshDrawingSurfaces(Rectangle gridSplitterRectangle)
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

        public override void Draw(int? seriesIndex)
        {
            if (list == null)
                return;
            if (list.Count == 0)
            {

                g.DrawString("No series Selected.", SystemFonts.DialogFont, Brushes.Beige, Point.Empty);
            }
            else
            {
                ind = (int)seriesIndex;
            }

            price_format = list[ind].legend.price_format;

            //just to check numbers height
            g.Clear(style.backColor);

            if (ShouldDrawBorder)
                g.DrawRectangle(style.borderPen, 0, 0, bmp.Width - 1, bmp.Height - 1);

            
            s = list[ind].viewPort.VPMaxPrice;
            if(list[ind].legend.log)
                s = (float)Math.Pow(10, s);
            
            lableSize = g.MeasureString(s.ToString(price_format), style.magorTickFont);

            howManyCanFit = bmp.Height / lableSize.Height;
            if (howManyCanFit > 10) howManyCanFit = 10;


            float diff = (list[ind].viewPort.VPMaxPrice - list[ind].viewPort.VPMinPrice);

            float skolkoLableNaPrice = diff / howManyCanFit;

            int pressicion = GetPrecisionOf(skolkoLableNaPrice, true);
            if (pressicion < 0)
                pressicion++;
            if (skolkoLableNaPrice < 5 * Math.Pow(10, pressicion - 1))
            {
                skolkoLableNaPrice = (float)Math.Pow(10, pressicion - 1);
            }
            else
            {
                skolkoLableNaPrice = 5 * (float)Math.Pow(10, pressicion - 1);

            }

            // еще раз проверяю чтобы небыло случайных ошибок в округлении этого флоат. а то часто бывает вместо 0.05. пишет . 0.0499997
            if (skolkoLableNaPrice < 1)
            {
                if (pressicion < 0)
                    skolkoLableNaPrice = (float)Math.Round(skolkoLableNaPrice, 1 - pressicion);
            }

            //увеличиваю делитель если слишком плотно итут лейблы
            if ((diff / skolkoLableNaPrice) > howManyCanFit)
            {
                tempString = skolkoLableNaPrice.ToString();
                if (tempString.Contains("5"))
                    skolkoLableNaPrice *= 2;
                if (tempString.Contains("1"))
                    skolkoLableNaPrice *= 5;
                // еще раз проверяю чтобы небыло случайных ошибок в округлении этого флоат. а то часто бывает вместо 0.05. пишет . 0.0499997
                if (skolkoLableNaPrice < 1)
                {
                    if (pressicion < 0)
                        skolkoLableNaPrice = (float)Math.Round(skolkoLableNaPrice, 1 - pressicion);
                }
            }


            start = list[ind].viewPort.VPMinPrice - list[ind].viewPort.VPMinPrice % skolkoLableNaPrice;
            finish = list[ind].viewPort.VPMaxPrice + skolkoLableNaPrice;
            //сокращение вычислений (выдрал из цикла)
            pressicion = 1 - pressicion;
            //проверка на необходимость округления. Для больших цифр тут могла возникнуть ошибка из-за отрицательного значения
            if (pressicion < 0)
                pressicion = 0;
            if (grid.style.IsHorizontalVisible)
                grid.horizontalLevelsArray.Clear();

           

     

            for (float i = start; i <= finish; i = (float)Math.Round(i + skolkoLableNaPrice, pressicion))
            {


                for (float ii = skolkoLableNaPrice / 5; ii <= skolkoLableNaPrice; ii += skolkoLableNaPrice / 5)
                {
                    tempCoord = list[ind].viewPort.PriceToPixels(ii + i);
                    g.DrawLine(style.minorTickPen, bmp.Width - 5, tempCoord, bmp.Width, tempCoord);
                }


                tempCoord = list[ind].viewPort.PriceToPixels(i);
                g.DrawLine(style.magorTickPen, bmp.Width - 10, tempCoord, bmp.Width, tempCoord);
                
                s = list[ind].legend.log ? (float)Math.Pow(10, i) : i;
                
                g.DrawString(s.ToString(price_format), style.magorTickFont, style.magorFontBrush, bmp.Width - lableSize.Width-10, tempCoord - lableSize.Height / 2);

                if (grid.style.IsHorizontalVisible)
                    grid.horizontalLevelsArray.Add(tempCoord);

            }
            if (cursorLable.IsVisible && cursorLable != null)
            {

                cursorLable.rect.Size = g.MeasureString(cursorLable.Value.ToString(price_format), cursorLable.font);
                cursorLable.rect.Offset(0, -cursorLable.rect.Height / 2);
                cursorLable.rect.X = bmp.Width - 10 - cursorLable.rect.Width;
                g.FillRectangle(cursorLable.backBrush, cursorLable.rect.X,cursorLable.rect.Y,cursorLable.rect.Width,cursorLable.rect.Height);
                if (cursorLable.borderPen != null)
                    g.DrawRectangle(cursorLable.borderPen, cursorLable.rect.X, cursorLable.rect.Y, cursorLable.rect.Width, cursorLable.rect.Height);
                
                g.DrawString(cursorLable.Value.ToString(price_format), cursorLable.font, cursorLable.fontBrush, cursorLable.rect.Location);

            }

        }

      
    }
    

    public class RightAxisDinamic : Axis
    {

        public RightAxisDinamic(Rectangle rect)
        {
            cursorLable = new CursorInfoLable();
            style = new Styles.AxisStyle();
            style.magorTickFont = Skilful.Sample.AxisStyleRight_Font;
            cursorLable.font = Skilful.Sample.CursorLable_Right_Font; //style.magorTickFont;

          

            refreshDrawingSurfaces(rect);

        }

        public override void refreshDrawingSurfaces(Rectangle gridSplitterRectangle)
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

        public override void Draw(int? seriesIndex)
        {

            if (list == null)
                return;
            if (list.Count == 0)
            {

                g.DrawString("No series Selected.", SystemFonts.DialogFont, Brushes.Beige, Point.Empty);
            }
            else
            {
                ind = (int)seriesIndex;
            }

            price_format = list[ind].legend.price_format;

            //just to check numbers height
            g.Clear(style.backColor);

            if (ShouldDrawBorder)
                g.DrawRectangle(style.borderPen, 0, 0, bmp.Width - 1, bmp.Height - 1);


            s = list[ind].viewPort.VPMaxPrice;
            if(list[ind].legend.log)
                s = (float)Math.Pow(10, s);

            lableSize = g.MeasureString(s.ToString(price_format), style.magorTickFont);

            howManyCanFit = bmp.Height / lableSize.Height;
            if (howManyCanFit > 10) howManyCanFit = 10;

            float diff = (list[ind].viewPort.VPMaxPrice - list[ind].viewPort.VPMinPrice);

            float skolkoLableNaPrice = diff / howManyCanFit;

            int pressicion = GetPrecisionOf(skolkoLableNaPrice, true);
            if (pressicion < 0)
                pressicion++;
            if (skolkoLableNaPrice < 5 * Math.Pow(10, pressicion - 1))
            {
                skolkoLableNaPrice = (float)Math.Pow(10, pressicion - 1);
            }
            else
            {
                skolkoLableNaPrice = 5 * (float)Math.Pow(10, pressicion - 1);

            }

            // еще раз проверяю чтобы небыло случайных ошибок в округлении этого флоат. а то часто бывает вместо 0.05. пишет . 0.0499997
            if (skolkoLableNaPrice < 1)
            {
                if (pressicion < 0)
                    skolkoLableNaPrice = (float)Math.Round(skolkoLableNaPrice, 1 - pressicion);
            }

            //увеличиваю делитель если слишком плотно итут лейблы
            if ((diff / skolkoLableNaPrice) > howManyCanFit)
            {
                tempString = skolkoLableNaPrice.ToString();
                if (tempString.Contains("5"))
                    skolkoLableNaPrice *= 2;
                if (tempString.Contains("1"))
                    skolkoLableNaPrice *= 5;
                // еще раз проверяю чтобы небыло случайных ошибок в округлении этого флоат. а то часто бывает вместо 0.05. пишет . 0.0499997
                if (skolkoLableNaPrice < 1)
                {
                    if (pressicion < 0)
                        skolkoLableNaPrice = (float)Math.Round(skolkoLableNaPrice, 1 - pressicion);
                }
            }


            start = list[ind].viewPort.VPMinPrice - list[ind].viewPort.VPMinPrice % skolkoLableNaPrice;
            finish = list[ind].viewPort.VPMaxPrice + skolkoLableNaPrice;
            //сокращение вычислений (выдрал из цикла)
            pressicion = 1 - pressicion;
            //проверка на необходимость округления. Для больших цифр тут могла возникнуть ошибка из-за отрицательного значения
            if (pressicion < 0)
                pressicion = 0;
            if (grid.style.IsHorizontalVisible)
                grid.horizontalLevelsArray.Clear();



            for (float i = start; i <= finish; i = (float)Math.Round(i + skolkoLableNaPrice, pressicion))
            {


                for (float ii = skolkoLableNaPrice / 5; ii <= skolkoLableNaPrice; ii += skolkoLableNaPrice / 5)
                {
                    tempCoord = list[ind].viewPort.PriceToPixels(ii + i);

                        g.DrawLine(style.minorTickPen, 0, tempCoord, 5, tempCoord);
                }


                tempCoord = list[ind].viewPort.PriceToPixels(i);
                g.DrawLine(style.magorTickPen, 0, tempCoord, 10, tempCoord);
                
                s = list[ind].legend.log ? (float)Math.Pow(10, i) : i;

                g.DrawString(s.ToString(price_format), style.magorTickFont, style.magorFontBrush, 10, tempCoord - lableSize.Height / 2);

                if (grid.style.IsHorizontalVisible)
                    grid.horizontalLevelsArray.Add(tempCoord);

            }


            if (cursorLable != null && cursorLable.IsVisible)
            {
                cursorLable.rect.Size = g.MeasureString(cursorLable.Value.ToString(price_format), cursorLable.font);
                cursorLable.rect.Offset(0, -cursorLable.rect.Height / 2);
                
                g.FillRectangle(cursorLable.backBrush, cursorLable.rect);
                if (cursorLable.borderPen != null)
                    g.DrawRectangle(cursorLable.borderPen, cursorLable.rect.X, cursorLable.rect.Y, cursorLable.rect.Width, cursorLable.rect.Height);
                g.DrawString(cursorLable.Value.ToString(price_format), cursorLable.font, cursorLable.fontBrush, cursorLable.rect.Location);

            }
           
            if (targetLable != null && IsTargetsVisible)
            {
                
                
                foreach (int i in targetLable.Keys)
                {
                    
                    targetLable[i].rect.Size = cursorLable.rect.Size;
                    
                   

                    targetLable[i].rect.Y = list[ind].viewPort.PriceToPixels(targetLable[i].Value);

                    targetLable[i].rect.Offset(0, -targetLable[i].rect.Height / 2);
                    targetLable[i].rect.X = 10;
                   
                    // ========== меняем шрифт и цвет лейба НР  ==========
                    targetLable[i].font = Skilful.Sample.ModelStyle_TargetLable_Font;
                    targetLable[i].fontBrush = new SolidBrush(Skilful.Sample.ModelStyle_TargetLable_Font_Color);
                    //=============================================
                    
                    g.FillRectangle(Styles.ModelStyle.lablesBrush, targetLable[i].rect);
                    if (targetLable[i].borderPen != null)
                        g.DrawRectangle(cursorLable.borderPen, 10, targetLable[i].rect.Y, targetLable[i].rect.Width, targetLable[i].rect.Height);

                                    
                    
                    g.DrawString( list[ind].legend.log ?
                                       Math.Pow(10, targetLable[i].Value).ToString(price_format) :
                                       targetLable[i].Value.ToString(price_format), targetLable[i].font, targetLable[i].fontBrush, targetLable[i].rect.X, targetLable[i].rect.Y);
                                                                                                                             //  targetLable[i].font.Height
                    
                }
                   
                
               
            }

        }


    }
    public class RightAxisStatic : Axis
    {
        public RightAxisStatic(Rectangle rect)
        {
            cursorLable = new CursorInfoLable();
            style = new Styles.AxisStyle();
            cursorLable.font = style.magorTickFont;
            refreshDrawingSurfaces(rect);

        }

        public override void refreshDrawingSurfaces(Rectangle gridSplitterRectangle)
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

        public override void Draw(int? seriesIndex)
        {

            if (list == null)
                return;
            if (list.Count == 0)
            {

                g.DrawString("No series Selected.", SystemFonts.DialogFont, Brushes.Beige, Point.Empty);
            }
            else
            {
                ind = (int)seriesIndex;
            }

            price_format = list[ind].legend.price_format;


            //just to check numbers height
            g.Clear(style.backColor);

            if (ShouldDrawBorder)
                g.DrawRectangle(style.borderPen, 0, 0, bmp.Width - 1, bmp.Height - 1);

            float lableHeight = g.MeasureString("1234", style.magorTickFont).Height;
            float temp = bmp.Height / lableHeight;




            if (grid.style.IsHorizontalVisible)
                grid.horizontalLevelsArray.Clear();

            for (float i = 0; i < temp*5; i++)
            {
                g.DrawLine(style.minorTickPen, 0, i * lableHeight/5, 5, i * lableHeight/5);
            }
            for (float i = 0; i < temp; i++)
            {
                // g.DrawLine(style.minorTickPen, 0, tempCoord, 5, tempCoord);
                g.DrawLine(style.magorTickPen, 0, i * lableHeight, 10, i * lableHeight);
               
                s = list[ind].viewPort.PixelToPrice(i * lableHeight);

                if (list[ind].legend.log) s = (float)Math.Pow(10, s);

                g.DrawString(s.ToString(price_format), style.magorTickFont, style.magorFontBrush, 10, i * lableHeight - lableHeight / 2);

                if (grid.style.IsHorizontalVisible)
                    grid.horizontalLevelsArray.Add(tempCoord);
            }


            if (cursorLable != null && cursorLable.IsVisible)
            {
                cursorLable.rect.Size = g.MeasureString(cursorLable.Value.ToString(price_format), cursorLable.font);
                cursorLable.rect.Offset(0, -cursorLable.rect.Height / 2);

                g.FillRectangle(cursorLable.backBrush, cursorLable.rect);
                if (cursorLable.borderPen != null)
                    g.DrawRectangle(cursorLable.borderPen, cursorLable.rect.X, cursorLable.rect.Y, cursorLable.rect.Width, cursorLable.rect.Height);
                g.DrawString(cursorLable.Value.ToString(price_format), cursorLable.font, cursorLable.fontBrush, cursorLable.rect.Location);

            }
            if (targetLable != null && IsTargetsVisible)
            {
                foreach (int i in targetLable.Keys)
                {
                    targetLable[i].rect.Size = cursorLable.rect.Size;

                    targetLable[i].rect.Y = list[ind].viewPort.PriceToPixels(targetLable[i].Value);

                    targetLable[i].rect.Offset(0, -targetLable[i].rect.Height / 2);
                    targetLable[i].rect.X = 10;
                    g.FillRectangle(Styles.ModelStyle.lablesBrush, targetLable[i].rect);
                    if (targetLable[i].borderPen != null)
                        g.DrawRectangle(cursorLable.borderPen, 10, targetLable[i].rect.Y, targetLable[i].rect.Width, targetLable[i].rect.Height);
                    g.DrawString(targetLable[i].Value.ToString(price_format), targetLable[i].font, targetLable[i].fontBrush, 10, targetLable[i].rect.Y);

                }
               
            }


        }
    }
}
