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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GridSplitter
{
    enum ResizingWhat { LeftAxis, RightAxis, TopAxis, BottomAxis, None }
    public partial class GridSplitter : UserControl
    {
        public bool IsGridVisible = Skilful.Sample.GridSplitter_IsGridVisible;
        public bool IsInitialResize = true;
        public bool IsUserAllowedToResizeAxises = true;
        private bool IsLineGrabbed = false;
        public bool IsLeftAxisHidden = false;
        public bool IsRightAxisHidden = false;
        public bool IsTopAxisHidden = false;
        private bool IsUserScaling = false;

        public bool IsBottomAxisHidden = false;


        public int minimumAxisSize = 5;
        public int? clickedCellIndex = null;

        private Point lastMouseClick;
        // private Point mouseDragDelta;

        private ResizingWhat resizingWhichAxis = ResizingWhat.None;
        public Pen GridPen
        {
            get { return GridLine.gridPen; }
            set { GridLine.gridPen = value; }
        }
        private int leftAxisWidth;
        private int rightAxisWidth;
        private int topAxisHeight;
        private int bottomAxisHeight;
        public int LeftAxisWidth
        {
            get { return leftAxisWidth; }
            set 
            { 
            leftAxisWidth = value;
            resizingWhichAxis = ResizingWhat.LeftAxis;
            SetLinesPosition();
            SetCellSizes();
            resizingWhichAxis = ResizingWhat.None;
            OnResizingLeftAxis();
            }
        }
        public int RightAxisWidth
        {
            get { return rightAxisWidth; }
            set 
            {
                rightAxisWidth = value;
                resizingWhichAxis = ResizingWhat.RightAxis;
                SetLinesPosition();
                SetCellSizes();
                resizingWhichAxis = ResizingWhat.None;
                OnResizingRightAxis();
            }
        }
        public int TopAxisHeight
        {
            get { return topAxisHeight; }
            set 
            { 
                topAxisHeight = value;
                resizingWhichAxis = ResizingWhat.TopAxis;
                SetLinesPosition();
                SetCellSizes();
                resizingWhichAxis = ResizingWhat.None;
                OnResizingTopAxis();
            }
        }
        public int BottomAxisHeight
        {
            get { return bottomAxisHeight; }
            set 
            {
                bottomAxisHeight = value;
                resizingWhichAxis = ResizingWhat.BottomAxis;
                SetLinesPosition();
                SetCellSizes();
                resizingWhichAxis = ResizingWhat.None;
                OnResizingBottomAxis();
            }
        }

        private GridLine leftLine;
        private GridLine rightLine;
        private GridLine topLine;
        private GridLine bottomLine;

        private Point mouseDragDelta = Point.Empty;
        public Rectangle[] cell = new Rectangle[9];
        private int cellMargin = 1;


        public GridSplitter()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);


            InitializeComponent();
        }
        public GridSplitter(int leftW, int rightW, int topH, int bottomH)
        {

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);


            InitializeCellsAndRects();
            InitializeComponent();

            leftAxisWidth = leftW;
            rightAxisWidth = rightW;
            bottomAxisHeight = bottomH;
            topAxisHeight = topH;
            SetLinesPosition();
            SetCellSizes();
            Invalidate();

        }
        private void InitializeCellsAndRects()
        {
            leftLine = new GridLine();
            rightLine = new GridLine();
            topLine = new GridLine();
            bottomLine = new GridLine();

            cell.Initialize();
        }
        private void SetLinesPosition()
        {
            switch (resizingWhichAxis)
            {
                case ResizingWhat.LeftAxis:
                    leftLine.pt1.X = leftAxisWidth;
                    leftLine.pt1.Y = 0;
                    leftLine.pt2.X = leftAxisWidth;
                    leftLine.pt2.Y = this.Height;
                    leftLine.UptadeWithGrabbingZone();
                    break;

                case ResizingWhat.RightAxis:
                    rightLine.pt1.X = this.Width - rightAxisWidth;
                    rightLine.pt1.Y = 0;
                    rightLine.pt2.X = this.Width - rightAxisWidth;
                    rightLine.pt2.Y = this.Height;
                    rightLine.UptadeWithGrabbingZone();
                    break;
                case ResizingWhat.TopAxis:
                    topLine.pt1.X = 0;
                    topLine.pt1.Y = topAxisHeight;
                    topLine.pt2.X = this.Width;
                    topLine.pt2.Y = topAxisHeight;
                    topLine.UptadeWithGrabbingZone();
                    break;

                case ResizingWhat.BottomAxis:
                    bottomLine.pt1.X = 0;
                    bottomLine.pt1.Y = this.Height - bottomAxisHeight;
                    bottomLine.pt2.X = this.Width;
                    bottomLine.pt2.Y = this.Height - bottomAxisHeight;
                    bottomLine.UptadeWithGrabbingZone();
                    break;

                default:
                    leftLine.pt1.X = leftAxisWidth;
                    leftLine.pt1.Y = 0;
                    leftLine.pt2.X = leftAxisWidth;
                    leftLine.pt2.Y = this.Height;
                    leftLine.UptadeWithGrabbingZone();

                    rightLine.pt1.X = this.Width - rightAxisWidth;
                    rightLine.pt1.Y = 0;
                    rightLine.pt2.X = this.Width - rightAxisWidth;
                    rightLine.pt2.Y = this.Height;
                    rightLine.UptadeWithGrabbingZone();

                    topLine.pt1.X = 0;
                    topLine.pt1.Y = topAxisHeight;
                    topLine.pt2.X = this.Width;
                    topLine.pt2.Y = topAxisHeight;
                    topLine.UptadeWithGrabbingZone();

                    bottomLine.pt1.X = 0;
                    bottomLine.pt1.Y = this.Height - bottomAxisHeight;
                    bottomLine.pt2.X = this.Width;
                    bottomLine.pt2.Y = this.Height - bottomAxisHeight;
                    bottomLine.UptadeWithGrabbingZone();
                    break;
            }

        }
        private void SetCellSizes()
        {
            switch (resizingWhichAxis)
            {
                case ResizingWhat.LeftAxis:

                    for (int i = 0; i < 3; i++)
                    {
                        cell[i].Width = leftLine.pt1.X;
                        cell[i + 3].X = leftLine.pt1.X + cellMargin;
                        cell[i + 3].Width = rightLine.pt1.X - leftLine.pt1.X - cellMargin;
                    }
                    break;

                case ResizingWhat.RightAxis:
                    for (int i = 3; i < 6; i++)
                    {
                        cell[i].Width = rightLine.pt1.X - leftLine.pt1.X - cellMargin;
                        cell[i + 3].X = rightLine.pt1.X + cellMargin;
                        cell[i + 3].Width = this.Width - rightLine.pt1.X;
                    }
                    break;
                case ResizingWhat.TopAxis:
                    for (int i = 0; i < 9; i += 3)
                    {
                        cell[i].Height = topLine.pt1.Y;
                        cell[i + 1].Y = topLine.pt1.Y + cellMargin;
                        cell[i + 1].Height = bottomLine.pt1.Y - topLine.pt1.Y - cellMargin;
                    }
                    break;
                case ResizingWhat.BottomAxis:
                    for (int i = 1; i < 9; i += 3)
                    {
                        cell[i].Height = bottomLine.pt1.Y - topLine.pt1.Y - cellMargin;
                        cell[i + 1].Y = bottomLine.pt1.Y + cellMargin;
                        cell[i + 1].Height = this.Height - bottomLine.pt1.Y;
                    }
                    break;
                default:
                    cell[0].X = 0;
                    cell[0].Y = 0;
                    cell[0].Width = leftAxisWidth;
                    cell[0].Height = topAxisHeight;

                    cell[1].X = 0;
                    cell[1].Y = topAxisHeight;
                    cell[1].Width = leftAxisWidth;
                    cell[1].Height = this.Height - bottomAxisHeight - topAxisHeight;

                    cell[2].X = 0;
                    cell[2].Y = this.Height - bottomAxisHeight;
                    cell[2].Width = leftAxisWidth;
                    cell[2].Height = bottomAxisHeight;


                    cell[3].X = leftAxisWidth;
                    cell[3].Y = 0;
                    cell[3].Width = this.Width - leftAxisWidth - rightAxisWidth;
                    cell[3].Height = topAxisHeight;

                    cell[4].X = leftAxisWidth;
                    cell[4].Y = topAxisHeight;
                    cell[4].Width = this.Width - leftAxisWidth - rightAxisWidth;
                    cell[4].Height = this.Height - bottomAxisHeight - topAxisHeight;

                    cell[5].X = leftAxisWidth;
                    cell[5].Y = this.Height - bottomAxisHeight;
                    cell[5].Width = this.Width - leftAxisWidth - rightAxisWidth;
                    cell[5].Height = bottomAxisHeight;

                    cell[6].X = this.Width - rightAxisWidth;
                    cell[6].Y = 0;
                    cell[6].Width = rightAxisWidth;
                    cell[6].Height = topAxisHeight;

                    cell[7].X = this.Width - rightAxisWidth;
                    cell[7].Y = topAxisHeight;
                    cell[7].Width = rightAxisWidth;
                    cell[7].Height = this.Height - bottomAxisHeight - topAxisHeight;

                    cell[8].X = this.Width - rightAxisWidth;
                    cell[8].Y = this.Height - bottomAxisHeight;
                    cell[8].Width = rightAxisWidth;
                    cell[8].Height = bottomAxisHeight;
                    break;

            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //e.Graphics.FillRectangle(Brushes.AliceBlue, cell[0]);
            //e.Graphics.FillRectangle(Brushes.AntiqueWhite, cell[1]);
            //e.Graphics.FillRectangle(Brushes.Aqua, cell[2]);
            //e.Graphics.FillRectangle(Brushes.Aquamarine, cell[3]);
            //e.Graphics.FillRectangle(Brushes.Azure, cell[4]);
            //e.Graphics.FillRectangle(Brushes.Beige, cell[5]);
            //e.Graphics.FillRectangle(Brushes.Bisque, cell[6]);
            //e.Graphics.FillRectangle(Brushes.BlanchedAlmond, cell[7]);
            //e.Graphics.FillRectangle(Brushes.Brown, cell[8]);
            //if (clickedCellIndex != null)
            //{
            //    e.Graphics.FillRectangle(Brushes.Black, cell[(int)clickedCellIndex]);
            //}

            if (IsGridVisible)
            {
                e.Graphics.DrawLine(GridLine.gridPen, leftLine.pt1, leftLine.pt2);
                e.Graphics.DrawLine(GridLine.gridPen, rightLine.pt1, rightLine.pt2);
                e.Graphics.DrawLine(GridLine.gridPen, topLine.pt1, topLine.pt2);
                e.Graphics.DrawLine(GridLine.gridPen, bottomLine.pt1, bottomLine.pt2);
                //  e.Graphics.DrawRectangle(Pens.Red, new Rectangle(Point.Empty, new Size(this.Width - 1, this.Height - 1)));
            }


            //for (int i = 0; i < cell.Length; i++)
            //{
            //    e.Graphics.DrawString(i.ToString(), SystemFonts.CaptionFont, Brushes.Black, cell[i].Location);
            //}



        }

        private void GridSplitter_Resize(object sender, EventArgs e)
        {
            SetLinesPosition();
            SetCellSizes();
            Refresh();
        }

        private void GridSplitter_MouseMove(object sender, MouseEventArgs e)
        {


            if (!IsLineGrabbed)
            {
                if ((e.X < leftLine.xPlusGrabZone && e.X > leftLine.xMinusGrabzone) || (e.X > rightLine.xMinusGrabzone && e.X < rightLine.xPlusGrabZone))
                {
                    Cursor = Cursors.SizeWE;
                }
                else if ((e.Y < topLine.yPlusGrabZone && e.Y > topLine.yMinusGrabZone) || (e.Y > bottomLine.yMinusGrabZone && e.Y < bottomLine.yPlusGrabZone))
                {
                    Cursor = Cursors.SizeNS;
                }
                else
                {

                    Cursor = Cursors.Arrow;
                    if (IsUserScaling)
                    {

                        mouseDragDelta.X = e.X - lastMouseClick.X;
                        mouseDragDelta.Y = e.Y - lastMouseClick.Y;

                        lastMouseClick = e.Location;


                        if (mouseDragDelta.X != 0)
                            mouseArgs.mouseDragDelta.X = cell[4].Width / (mouseDragDelta.X);
                        else
                            mouseArgs.mouseDragDelta.X = 0;

                        if (mouseDragDelta.Y != 0)
                            mouseArgs.mouseDragDelta.Y = cell[4].Height / (mouseDragDelta.Y);
                        else
                            mouseArgs.mouseDragDelta.Y = 0;

                        OnDragging(clickedCellIndex, mouseArgs);
                        this.Refresh();

                    }
                }
            }
            else if (IsUserAllowedToResizeAxises)
            {

                switch (resizingWhichAxis)
                {
                    case ResizingWhat.LeftAxis:
                        leftAxisWidth += e.X - lastMouseClick.X;
                        if (leftAxisWidth < minimumAxisSize)
                        {
                            leftAxisWidth = minimumAxisSize;
                            IsLeftAxisHidden = true;
                        }
                        else if (leftAxisWidth > this.Width - rightAxisWidth - minimumAxisSize)
                        {
                            IsLeftAxisHidden = false;
                            leftAxisWidth = this.Width - rightAxisWidth - minimumAxisSize;
                        }
                        OnResizingLeftAxis();
                        lastMouseClick = e.Location;
                        SetLinesPosition();
                        SetCellSizes();
                        break;

                    case ResizingWhat.RightAxis:
                        rightAxisWidth -= e.X - lastMouseClick.X;
                        if (rightAxisWidth < minimumAxisSize)
                        {
                            rightAxisWidth = minimumAxisSize;
                            IsRightAxisHidden = true;
                        }
                        else if (rightAxisWidth > this.Width - leftAxisWidth - minimumAxisSize)
                        {
                            IsRightAxisHidden = false;
                            rightAxisWidth = this.Width - leftAxisWidth - minimumAxisSize;
                        }
                        lastMouseClick = e.Location;
                        OnResizingRightAxis();
                        SetLinesPosition();
                        SetCellSizes();
                        break;

                    case ResizingWhat.TopAxis:
                        topAxisHeight += e.Y - lastMouseClick.Y;
                        if (topAxisHeight < minimumAxisSize)
                        {
                            topAxisHeight = minimumAxisSize;
                            IsTopAxisHidden = true;
                        }
                        else if (topAxisHeight > this.Height - bottomAxisHeight - minimumAxisSize)
                        {
                            topAxisHeight = this.Height - bottomAxisHeight - minimumAxisSize;
                            IsTopAxisHidden = false;
                        }

                        lastMouseClick = e.Location;
                        OnResizingTopAxis();
                        SetLinesPosition();
                        SetCellSizes();
                        break;

                    case ResizingWhat.BottomAxis:
                        bottomAxisHeight -= e.Y - lastMouseClick.Y;
                        if (bottomAxisHeight < minimumAxisSize)
                        {
                            bottomAxisHeight = minimumAxisSize;
                            IsBottomAxisHidden = true;
                        }
                        else if (bottomAxisHeight > this.Height - topAxisHeight - minimumAxisSize)
                        {
                            bottomAxisHeight = this.Height - topAxisHeight - minimumAxisSize;
                            IsBottomAxisHidden = false;
                        }

                        lastMouseClick = e.Location;
                        OnResizingBottomAxis();
                        SetLinesPosition();
                        SetCellSizes();
                        break;
                }
                Refresh();
            }


        }

        private void GridSplitter_MouseDown(object sender, MouseEventArgs e)
        {
            lastMouseClick = e.Location;
            GetClickedCellIndex(lastMouseClick);
            if (!IsLineGrabbed)
            {

                if (Cursor == Cursors.SizeWE)
                {
                    if (e.X <= leftLine.xPlusGrabZone)
                    {
                        resizingWhichAxis = ResizingWhat.LeftAxis;
                        IsLineGrabbed = true;
                    }
                    else if (e.X >= rightLine.xMinusGrabzone)
                    {
                        resizingWhichAxis = ResizingWhat.RightAxis;
                        IsLineGrabbed = true;
                    }

                }

                else if (Cursor == Cursors.SizeNS)
                {
                    if (e.Y <= topLine.yPlusGrabZone)
                    {
                        resizingWhichAxis = ResizingWhat.TopAxis;
                        IsLineGrabbed = true;
                    }
                    else if (e.Y >= bottomLine.yMinusGrabZone)
                    {
                        resizingWhichAxis = ResizingWhat.BottomAxis;
                        IsLineGrabbed = true;
                    }
                }

                else
                {
                    IsUserScaling = true;
                    IsLineGrabbed = false;

                }
            }
        }

        private void GridSplitter_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsLineGrabbed)
            {
                IsLineGrabbed = false;
                Cursor = Cursors.Arrow;
                resizingWhichAxis = ResizingWhat.None;
            }
            IsUserScaling = false;

        }

        public void GetClickedCellIndex(Point e)
        {
            clickedCellIndex = null;
            if (e.X > leftLine.xPlusGrabZone && e.X < rightLine.xMinusGrabzone)
            {
                //щелчок по центральнной части контрола
                if (e.Y < bottomLine.yMinusGrabZone && e.Y > topLine.yPlusGrabZone)
                {
                    //центр
                    clickedCellIndex = 4;
                }
                else
                    if (e.Y > bottomLine.yPlusGrabZone)
                    {
                        //нижняя ось
                        clickedCellIndex = 5;
                    }
                    else
                        if (e.Y < topLine.yMinusGrabZone)
                        {
                            //верхняя ось
                            clickedCellIndex = 3;
                         
                        }
            }
            else
                if (e.X > rightLine.xPlusGrabZone && e.Y < bottomLine.yMinusGrabZone && e.Y > topLine.yPlusGrabZone)
                {
                    //праввая ось
                    clickedCellIndex = 7;
                }
                else
                    if (e.X < leftLine.xMinusGrabzone && e.Y < bottomLine.yMinusGrabZone && e.Y > topLine.yPlusGrabZone)
                    {
                        //левая ось
                        clickedCellIndex = 1;
                    }

        }



        #region Events
        // /////////////////////////////////////////////////////

        //public delegate void MouseDelegat(object sender, MouseDraggingEventArgs e);
        public delegate void MouseDelegat(object sender, MouseDraggingEventArgs e);
        public class MouseDraggingEventArgs : EventArgs
        {
            public PointF mouseDragDelta;
            //конструктор поумолчаинию
            public MouseDraggingEventArgs()
            {
            }

        }

        private MouseDraggingEventArgs mouseArgs = new MouseDraggingEventArgs();


        public event MouseDelegat ResizingLeftAxis;
        public void OnResizingLeftAxis()
        {
            if (ResizingLeftAxis != null)
                ResizingLeftAxis(null, null);
        }


        public event MouseDelegat ResizingRightAxis;
        public void OnResizingRightAxis()
        {
            if (ResizingRightAxis != null)
                ResizingRightAxis(null, null);
        }

        public event MouseDelegat ResizingBottomAxis;
        public void OnResizingBottomAxis()
        {
            if (ResizingBottomAxis != null)
                ResizingBottomAxis(null, null);
        }

        public event MouseDelegat ResizingTopAxis;
        public void OnResizingTopAxis()
        {
            if (ResizingTopAxis != null)
                ResizingTopAxis(null, null);
        }

        public event MouseDelegat DraggingEvent;
        public void OnDragging(int? indexOfCell, MouseDraggingEventArgs e)
        {
            if (DraggingEvent != null)
                DraggingEvent((object)indexOfCell, e);
        }




        //public delegate void EventHandler(object sender, EventArgs e);
        //public event EventHandler Resized;
        //public void OnResized(object sender, EventArgs e)
        //{
        //    if (Resized != null)
        //        Resized(null, e);
        //}


        //public event MouseDelegat;
        //public void FireDraggingPlot(int? indexOfCell, MouseDraggingEventArgs e)
        //{
        //    if (DraggingPlot != null)
        //        DraggingPlot((object)indexOfCell, e);
        //}
        #endregion

    }
    public class GridLine
    {
        public static Pen gridPen = Pens.Black;
        public int xPlusGrabZone;
        public int xMinusGrabzone;
        public int yPlusGrabZone;
        public int yMinusGrabZone;
        public Point pt1;

        public Point pt2;
        public static int grabZone = 5;

        public GridLine()
        {
            pt1 = Point.Empty;
            pt2 = Point.Empty;
        }

        public GridLine(Point point1, Point point2)
        {
            pt1 = point1;
            pt2 = point2;
        }
        public void UptadeWithGrabbingZone()
        {
            xPlusGrabZone = pt1.X + grabZone;
            xMinusGrabzone = pt1.X - grabZone;
            yPlusGrabZone = pt1.Y + grabZone;
            yMinusGrabZone = pt1.Y - grabZone;
        }
    }

}
