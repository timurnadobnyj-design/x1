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

namespace ChartV2.Data
{

   // public enum LineType { Horizontal, Vertical, Angular, Circle, Arch };
    public abstract class UserLine
    {

        public float radius;
        public RectangleF rect;
       public PointF pt1;
       public PointF pt2;
       public PointF pt3;
       public PointF pt4;
       public PointF clickedPoint;
       public Point IntPt1;
       public Point IntPt2;
       public ChartV2.Styles.UserLineStyle style;
       public DrawingTool type;
        public bool IsSelected;
        public bool IsVisible = true;
        public static int magnetoActiveZone = 20;
        public static bool DoesParallelDisplacementNeeded;
        public static bool IsInEditingExistingLine;
        public static bool IsWaitingMouseClickToStoreLine;
        public static bool SkipSelection;
        public static bool MagnetoModeOn = true;

        public static UserLine selectedObj;
        public static Pen pen;
        public static int? selectedLineIndex ;

        public static void DrawItself(Graphics g, Point firstPoint, Point lastPoint, DrawingTool type)
        {
            if (pen == null)
                pen = Pens.Black;
            if (IsInEditingExistingLine)
            {
                if (selectedObj != null)
                    pen = selectedObj.style.pen;
            }


            switch (type)
            {
                case DrawingTool.VerticalLine:

                    g.DrawLine(pen, firstPoint, lastPoint);
                   
                    break;
               
                case DrawingTool.HorizontalLine:
                    g.DrawLine(pen, firstPoint, lastPoint);
                    break;
                
               
              
            }
        }
        public static void DrawItself(Graphics g, PointF firstPoint, PointF lastPoint, DrawingTool type)
        {
            if (pen == null)
                pen = Pens.Black;
            if (IsInEditingExistingLine)
            {
                if (selectedObj != null)
                    pen = selectedObj.style.pen;
            }

             switch (type)
             {
                 case DrawingTool.Circle:

                     float radius = (float)Math.Sqrt(Math.Pow((firstPoint.Y - lastPoint.Y), 2) + Math.Pow((firstPoint.X - lastPoint.X), 2));
                     RectangleF rect = new RectangleF(0, 0, radius * 2, radius * 2);
                     rect.Offset(firstPoint.X - radius, firstPoint.Y - radius);
                     g.DrawLine(Pens.Black, firstPoint, lastPoint);
                     g.DrawRectangle(Pens.Black, firstPoint.X - 2, firstPoint.Y - 2, 4,4);
                     g.DrawEllipse(pen, rect);
                     break;

                 case DrawingTool.FreeLine:
                     g.DrawLine(pen, firstPoint, lastPoint);
                     RectangleF rect2 = new RectangleF(0, 0, 4, 4);
                     rect2.Location = new PointF(firstPoint.X - 2, firstPoint.Y - 2);
                     g.DrawRectangle(Pens.Black, rect2.X, rect2.Y, rect2.Width, rect2.Height);
                     rect2.Location = new PointF(lastPoint.X - 2, lastPoint.Y - 2);
                     g.DrawRectangle(Pens.Black, rect2.X, rect2.Y, rect2.Width, rect2.Height);
                     break;
                 case DrawingTool.Arc:

                     float radius2 = (float)Math.Sqrt(Math.Pow((firstPoint.Y - lastPoint.Y), 2) + Math.Pow((firstPoint.X - lastPoint.X), 2));
                     if (radius2 == 0)
                         break;
                     RectangleF rect3 = new RectangleF(0, 0, radius2 * 2, radius2 * 2);
                     rect3.Offset(firstPoint.X - radius2, firstPoint.Y - radius2);
                     g.DrawLine(Pens.Black, firstPoint, lastPoint);
                     g.DrawRectangle(Pens.Black, firstPoint.X - 2, firstPoint.Y - 2,4,4);


                     double alpha = Math.Asin((lastPoint.Y - firstPoint.Y) / radius2);
                     if (firstPoint.X > lastPoint.X)
                         alpha = Math.PI - alpha;

                     g.DrawArc(pen, rect3, (float)(alpha * 180 / Math.PI), 60);
                     break;
                 
                 case DrawingTool.Cycles:
                     int diff = (int)lastPoint.X - (int)firstPoint.X;
                     //1
                     g.DrawLine(pen, firstPoint.X, 0, firstPoint.X, lastPoint.Y);
                     //2
                     g.DrawLine(pen, lastPoint.X, 0, lastPoint.X, lastPoint.Y);
                     //3
                     g.DrawLine(pen, lastPoint.X + diff, 0, lastPoint.X + diff, lastPoint.Y);
                     //4
                     g.DrawLine(pen, lastPoint.X + diff * 2, 0, lastPoint.X + diff * 2, lastPoint.Y);
                     //5
                     g.DrawLine(pen, lastPoint.X + diff * 3, 0, lastPoint.X + diff * 3, lastPoint.Y);
                     //6
                     g.DrawLine(pen, lastPoint.X + diff * 4, 0, lastPoint.X + diff * 4, lastPoint.Y);
                     //7
                     g.DrawLine(pen, lastPoint.X + diff * 5, 0, lastPoint.X + diff * 5, lastPoint.Y);

                     g.DrawLine(pen, lastPoint.X + diff * 6, 0, lastPoint.X + diff * 6, lastPoint.Y);
                     break;
             }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="IsEditClick">Этот клик нужен для выделения линии или для редактирования линии</param>
        /// <returns></returns>
        public bool IsLineClicked(float X, float Y,bool IsEditClick)
        {
            switch (this.type)
            {
                case DrawingTool.VerticalLine:
                    if (Math.Abs(X - IntPt1.X) <= 1)
                    {
                        if (IsEditClick)
                        selectedObj = this;
                        return true;
                    }
                    break;
                case DrawingTool.Cycles:
                    int diff = IntPt2.X - IntPt1.X;
                    for (int i = 0; i < 8; i++)
                    {
                        if (Math.Abs(X - (IntPt1.X + diff * i)) <= 1)
                        {
                            if (IsEditClick)
                            {
                                selectedObj = this;
                                clickedPoint = new PointF(X, Y);
                                DoesParallelDisplacementNeeded = true;
                            }
                            return true;
                        }
                    }

                    break;
                case DrawingTool.HorizontalLine:
                     if (Math.Abs((Y - pt1.Y) / pt1.Y*100) <= 0.01)
                     {
                         if (IsEditClick)
                             selectedObj = this;
                         return true;
                     }
                    break;
                case DrawingTool.Arc:
                    int radius3 = (int)Math.Sqrt(Math.Pow((Y - pt3.Y), 2) + Math.Pow((X - pt3.X), 2));
                    int radius4 = (int)Math.Sqrt(Math.Pow((pt4.Y - pt3.Y), 2) + Math.Pow((pt4.X - pt3.X), 2));
                    DoesParallelDisplacementNeeded = false;
                    if ((radius3< radius4 + radius4 / 4) && (radius3 > radius4 - radius4 / 4))
                    {
                        if (this.IsSelected)
                        {
                            //центральная точка. для переноса.
                            clickedPoint = new PointF(X, Y);
                            DoesParallelDisplacementNeeded = true;
                        }

                        if (IsEditClick)
                            selectedObj = this;
                        return true;
                    }
                    break;
                case DrawingTool.Circle:
                    
                    int radius1 = (int)Math.Sqrt(Math.Pow((Y - pt3.Y), 2) + Math.Pow((X - pt3.X), 2));
                    int radius2 = (int)Math.Sqrt(Math.Pow((pt4.Y - pt3.Y), 2) + Math.Pow((pt4.X - pt3.X), 2));
                    DoesParallelDisplacementNeeded = false;
                    if ((Y + 5 >= pt3.Y && Y - 5 <= pt3.Y) && (X + 5 >= pt3.X && X - 5 <= pt3.X))
                    {
                        if (this.IsSelected)
                        {
                            //щелчек на цетре круга?
                            if ((Y + 5 >= pt3.Y && Y - 5 <= pt3.Y) && (X + 5 >= pt3.X && X - 5 <= pt3.X))
                            {
                                //центральная точка. для переноса.
                                clickedPoint = new PointF(pt3.X, pt3.Y);
                                DoesParallelDisplacementNeeded = true;
                            }


                        }



                        if (IsEditClick)
                            selectedObj = this;
                        return true;
                    }
                    else
                    {
                        if (this.IsSelected)
                        {
                            if ((Y + 5 >= pt4.Y && Y - 5 <= pt4.Y) && (X + 5 >= pt4.X && X - 5 <= pt4.X))
                            {
                                //щелкнули точку на радиусе
                                clickedPoint = new PointF(pt4.X, pt4.Y);

                            }
                        }
                        if (IsEditClick)
                            selectedObj = this;
                        return true;
                    }
                    
                case DrawingTool.FreeLine:
                    if (Math.Abs((Y - pt1.Y) * (pt2.X - pt1.X) - (X - pt1.X) * (pt2.Y - pt1.Y)) < 0.05)
                    {
                        if (this.IsSelected)
                        {
                            //какая точка щелкнута.. и как теперь перетягивать линию?
                            if (Math.Abs(X - pt1.X) <= pt1.X / 100)
                            {
                                //щелкнули левую точку. значит будем крутить линию вокруг точки 2;
                                clickedPoint = pt2;
                                DoesParallelDisplacementNeeded = false;
                            }
                            else if (Math.Abs(X - pt2.X) <= pt2.X / 100)
                            {
                                //щелкнули правую точку
                                clickedPoint = pt1;
                                DoesParallelDisplacementNeeded = false;
                            }
                            else
                            {
                               
                                    //щелкнули центр значит нужен параллельный перенос
                                    clickedPoint = new PointF(X, Y);
                                    DoesParallelDisplacementNeeded = true;
                                 
                                

                            }
                        }
                        if (IsEditClick)
                            selectedObj = this;
                            return true;
                        
                    }
                       
                    break;


            }
            return false;
        }

        public static int round(float d)
        {
            int g = (int)d;
            if (d > 0)
                if (Math.Abs(d - g) > 0.5)
                    return (g + 1);
                else
                    if (Math.Abs(d - g) > 0.5)
                        return (g - 1);
            return g;
        }

        internal static PointF MagnetoPoint(PointF pt, Series seria)
        {
            
            
            //нахожу значение цены для первого бара и перевожу в пиксели.
            if (pt.X >= 0)
            {
                float Hpx;
                float Lpx;
                float old = pt.X;

                pt.X = round(seria.viewPort.PixelToBarNumber(pt.X));
                if (pt.X < 0)
                {
                    pt.X = old;
                    return pt; 
                }

                if (pt.X >= seria.data.Count)
                    pt.X = seria.data.Count - 1;
                Hpx = seria.viewPort.PriceToPixels((float)seria.data[(int)pt.X].High);
                Lpx = seria.viewPort.PriceToPixels((float)seria.data[(int)pt.X].Low);
                pt.X = seria.viewPort.BarNumberToPixels(pt.X);
               //ptY = seria.viewPort.PixelToPrice(pt.Y);


                if (pt.Y <= (Hpx + Lpx) / 2)
                {
                    if (Hpx <= pt.Y + magnetoActiveZone)
                        //прилипание к хаю
                        pt.Y = Hpx;

                }
                else
                {
                    if (Lpx >= pt.Y - magnetoActiveZone)
                        //прилипание к лоу
                        pt.Y = Lpx;

                }

                //pt.X = (int)seria.viewPort.PriceToPixels((float)seria.data[(int)seria.viewPort.PixelToBarNumber(pt.X)].High);

            }



            
            return pt;
        }
    }

    public class HorizontalLine:UserLine
    {
        public HorizontalLine(PointF pt1, PointF pt2, ChartV2.Styles.UserLineStyle style)
       {

            this.pt1 = pt1;
           this.pt2 = pt2;
           this.style = style;
           type = DrawingTool.HorizontalLine;
       }
    }
    public class VerticalLine : UserLine
    {
        public VerticalLine(PointF pt1, PointF pt2, ChartV2.Styles.UserLineStyle style)
        {
            this.IntPt1.X = (int)pt1.X;
            this.IntPt1.Y = (int)pt1.Y;
            this.IntPt2.X = (int)pt2.X;
            this.IntPt2.Y = (int)pt2.Y;
            this.style = style;
            type = DrawingTool.VerticalLine;
        }
    }
    public class AngularLine : UserLine
    {
        public AngularLine(PointF pt1, PointF pt2, ChartV2.Styles.UserLineStyle style)
        {
            this.pt1 = pt1;
            this.pt2 = pt2;
            //K
            this.pt3.X =  (pt2.Y - pt1.Y) / (pt2.X - pt1.X);

            this.style = style;
            style.extensionType = Styles.ExtensionType.BothRays;
            type = DrawingTool.FreeLine;
        }
        public AngularLine(PointF pt1, PointF pt2, ChartV2.Styles.UserLineStyle style, Styles.ExtensionType Ext)
        {
            this.pt1 = pt1;
            this.pt2 = pt2;
            //K
            this.pt3.X = (pt2.Y - pt1.Y) / (pt2.X - pt1.X);

            this.style = style;
            style.extensionType = Ext;
            type = DrawingTool.FreeLine;
        }
    }
    public class Circle : UserLine
    {
        public Circle(PointF pt1, PointF pt2,PointF center, PointF lastClick,  ChartV2.Styles.UserLineStyle style)
        {
            this.pt1 = pt1;
            this.pt2 = pt2;
            //точки для отрисовки адекватного радиуса уже после добавления в массив этой фигуры.
            this.pt3 = center;
            this.pt4 = lastClick;

            this.style = style;
            type = DrawingTool.Circle;
           
           // rect = new RectangleF(0, 0, (pt1.X - pt2.X) , (pt1.Y - pt2.Y) );
            //rect.Offset(pt1.X - rect.Width, pt1.Y - rect.Height);

        }
    }
    public class Arch : UserLine
    {
        public Arch(PointF pt1, PointF pt2, PointF center, PointF lastClick, ChartV2.Styles.UserLineStyle style)
        {
            this.pt1 = pt1;
            this.pt2 = pt2;
            //точки для отрисовки адекватного радиуса уже после добавления в массив этой фигуры.
            this.pt3 = center;
            this.pt4 = lastClick;

            this.style = style;
            type = DrawingTool.Arc;
        }
    }
    public class Cycles : UserLine
    {
        public Cycles(PointF pt1, PointF pt2, ChartV2.Styles.UserLineStyle style)
        {
            this.IntPt1.X = (int)pt1.X;
            this.IntPt1.Y = (int)pt1.Y;
            this.IntPt2.X = (int)pt2.X;
            this.IntPt2.Y = (int)pt2.Y;
            this.pt1 = pt1;
            this.pt2 = pt2;
            this.style = style;
            type = DrawingTool.Cycles;
        }
    }
}
