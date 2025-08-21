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
using Skilful.QuotesManager;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using ChartV2.Data;
using System.Linq;

namespace ChartV2
{
    public delegate void Button_Search_Complete_();
    public delegate List<TF> Active_Charts(ChartType ct);
    public delegate void Switch_charts(string s, List<TF> lstr);
    public delegate void RefreshPlot();
    public delegate void RefreshTopAxis();

    public partial class Control : UserControl
    {
        public event Active_Charts Active_Charts_;
        public event Switch_charts Switch_charts_;

        public GridSplitter.GridSplitter gridSplitter;
        private CursorType cursor = CursorType.Arrow;
        private DrawingTool drawingTool = DrawingTool.None;
        private DrawingTool DrawingTool
        {
            get { return drawingTool; }
            set 
            {
                if (!UserLine.IsInEditingExistingLine && plot.graphToolsToDrawList!=null)
                {
                    UserLine.selectedObj = null;
                    UserLine.selectedLineIndex = null;
                    UserLine.DoesParallelDisplacementNeeded = false;
                    for (int i = 0; i < plot.graphToolsToDrawList.Count; i++)
                    {
                        plot.graphToolsToDrawList[i].IsSelected = false;
                    }
                }
                drawingTool = value;
            }
        }

        public Axis_Plot.CursorInfoLable tempTargetLable;
        private PointF currentCursorLocation;
        private PointF lastCursorLocation;
        private PointF pt1;
        private PointF pt2;
        private RectangleF tempRect;
        private bool IsCrtlPressed;
        private Graphics drawingToolCanvas;

        private Image drawingToolBmp;
        Skilful.Working workingForm;
        public List<Data.Series> seriesList;
        public List<TBar> bar;
        //public BackgroundWorker bgWorker;
        //public bool flagSerch;

        //public void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    workingForm.SearchModel2(false, true);
        //}

        //public void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    if (workingForm.ActiveChart == this)
        //        workingForm._progressBar.Value = workingForm._progressBar.Maximum - e.ProgressPercentage;
        //    workingForm._progressBar.Visible = true;
        //}

        public void c_smi(int[] ModelIndexes)
        {
            for (int i = 0; i < ModelIndexes.Length; i++)
                Model.selectedModelIndexes.Add(ModelIndexes[i]);
            Model.lastSelectedModelIndex = ModelIndexes[ModelIndexes.Length - 1];
        }

        public List<Skilful.ModelManager.THTangentLine> HTList
        {
            set
            {
        
            if (plot.HTtoDrawList == null)
                plot.HTtoDrawList = new List<HTargetLine>();
                
                for (int i = 0; i < value.Count; i++)
                {

                    plot.HTtoDrawList.Add(new HTargetLine(value[i]));
                     
                    
                }
            }
        }


        public List<Skilful.ModelManager.TModel> ModelList2
        {
            //get { return plot.modelsToDrawList; } 
            set
            {
                if (plot.modelsToDrawList == null)
                    plot.modelsToDrawList = new List<Model>();
                int kk = 0;
                for (int i = 0; i < value.Count; i++)
                {
                    if (value[i].CurrentPoint > 3)
                    {
                        plot.modelsToDrawList.Add(new Model(value[i], GlobalMembersTAmodel.cl[kk % 11]));
                        kk++;
                    }
                }
            }
        }

        public bool ModelListExist
        {
            get
            {
                return plot.modelsToDrawList != null;
            }
        }

        public void ModelListClear()  //! Очистка списка моделей
        {
            if (plot.modelsToDrawList != null) plot.modelsToDrawList.Clear();
        }

        public Axis_Plot.Axis leftAxis;
        public Axis_Plot.Axis rightAxis;
        public Axis_Plot.Axis bottomAxis;
        public Axis_Plot.Axis topAxis;
        public Axis_Plot.Plot plot;
        public int dragSpeed = 2;
        //temp vars
        private float tempTimeVar;
        private float tempPriceVar;
       // int stat = 0;
        

      //  private System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();

       public Control( Size tempSize,Point tempPoint, Skilful.Working working)
      // public Control( )
        {
            workingForm = working;
          //  this.gridSplitter = new GridSplitter.GridSplitterV4(new Size(100, 100));
           this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
           this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
           this.SetStyle(ControlStyles.Selectable, true);

           InitializeComponent();

           this.Size = tempSize;
           this.Location = tempPoint;

           GridSplitterInitialization();

            plot = new Axis_Plot.Plot(gridSplitter.cell[4]);
            rightAxis = new Axis_Plot.RightAxisDinamic(gridSplitter.cell[7]);
            leftAxis = new Axis_Plot.LeftAxisDinamic(gridSplitter.cell[1]);
            bottomAxis = new Axis_Plot.BottomAxisStatic(gridSplitter.cell[5]);
            topAxis = new Axis_Plot.TopAxisAsInfoPanel(gridSplitter.cell[3], this);
           
            plot.grid = new Axis_Plot.Grid();
            rightAxis.grid = plot.grid;
            leftAxis.grid = plot.grid;
            bottomAxis.grid = plot.grid;
           
            

            //Serialization.CustomSerializer.Serialize("C:\\dd2.txt",plot);
          //  Axis_Plot.Plot plott = (Axis_Plot.Plot)Serialization.CustomSerializer.DeSerialize("C:\\dd2.txt");
        }

       private void GridSplitterInitialization()
       {
            
            this.gridSplitter = new GridSplitter.GridSplitter(20, 20, 20, 20);
            this.Controls.Add(this.gridSplitter);
            this.gridSplitter.BackColor = System.Drawing.Color.WhiteSmoke; // Skilful.Sample.GridSplitter_BackColor;
            this.gridSplitter.BottomAxisHeight = 20;
            this.gridSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridSplitter.LeftAxisWidth = 20;
            this.gridSplitter.Location = new System.Drawing.Point(0, 0);
            this.gridSplitter.Name = "gridSplitter";
            this.gridSplitter.RightAxisWidth = 20;
            this.gridSplitter.Size = new System.Drawing.Size(685, 355);
            this.gridSplitter.TabIndex = 0;
            this.gridSplitter.TopAxisHeight = 20;
            this.gridSplitter.ResizingBottomAxis += new GridSplitter.GridSplitter.MouseDelegat(this.gridSplitter_DraggingBottomAxis);
            this.gridSplitter.Paint += new System.Windows.Forms.PaintEventHandler(this.gridSplitter_Paint);
            this.gridSplitter.ResizingLeftAxis += new GridSplitter.GridSplitter.MouseDelegat(this.gridSplitter_DraggingLeftAxis);
            this.gridSplitter.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.gridSplitter_PreviewKeyDown);
            this.gridSplitter.ResizingRightAxis += new GridSplitter.GridSplitter.MouseDelegat(this.gridSplitter_DraggingRightAxis);
            this.gridSplitter.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridSplitter_MouseMove);
            this.gridSplitter.ResizingTopAxis += new GridSplitter.GridSplitter.MouseDelegat(this.gridSplitter_DraggingTopAxis);
            this.gridSplitter.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gridSplitter_MouseDoubleClick);
            this.gridSplitter.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridSplitter_KeyUp);
            this.gridSplitter.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gridSplitter_MouseClick);
            this.gridSplitter.DraggingEvent += new GridSplitter.GridSplitter.MouseDelegat(this.gridSplitter_DraggingNothing);
            this.gridSplitter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridSplitter_KeyDown);
       }

        public void SetTopAxisHeight(int height)
        {
            if(gridSplitter.TopAxisHeight != height)
                gridSplitter.TopAxisHeight = height;
        }


        public List<TF> str__;
        private void gridSplitter_MouseClick(object sender, MouseEventArgs e)
        {


          if (e.Button == MouseButtons.Right)
            {
                UserLine.IsWaitingMouseClickToStoreLine = false;
                if (DrawingTool == DrawingTool.None)
                {
                    switch (gridSplitter.clickedCellIndex)
                    {
                        case 3:
                            ContextMenuStrip contextChartsmenu = new ContextMenuStrip();
                            contextChartsmenu.Items.Add("Close this");
                            contextChartsmenu.Items[0].Click += new EventHandler(Control_Click);
                            contextChartsmenu.Items[0].Image = Skilful.Properties.Resources.gtk_close;
                            contextChartsmenu.Items.Add(new ToolStripSeparator());
                            str__ = Active_Charts_(this.seriesList[0].legend.chartType);
                            if (str__.Count == 1) contextChartsmenu.Items[0].Enabled = false;
                            for (int i = 0; i < str__.Count; i++)
                            {
                                contextChartsmenu.Items.Add(str__[i].ToString());
                                contextChartsmenu.Items[2+i].Click += new EventHandler(Control_Click);
                            }                          
                            
                            contextChartsmenu.Show(this, e.Location);                         
                            break;
                        case 4:
                            plotContextMenu.Show(this, e.Location);
                            if (seriesList != null)
                            {
                                for (int i = 0; i < plotContextMenu.Items.Count; i++)
                                {
                                    plotContextMenu.Items[i].Enabled = true;
                                }

                            }
                            if (seriesList == null)
                            {
                                for (int i = 0; i < plotContextMenu.Items.Count; i++)
                                {
                                    plotContextMenu.Items[i].Enabled = false;
                                }

                            }
                            break;

                    }
                }
                else
                {
                    DrawingTool = DrawingTool.None;

                }
            }
            else 
            if (e.Button == MouseButtons.Left)
            {
                if (e.X > (plot.bmp.Width + gridSplitter.cell[4].Location.X )|| e.Y > (plot.bmp.Height + gridSplitter.cell[4].Location.Y) || e.X < gridSplitter.cell[1].Width || e.Y < gridSplitter.cell[3].Height)
                {
                    //щелчок не попал в область гарфика. а зашел на область ОСИ Y.
                    int d = 0;
                    d++;
                }
                else
                {
                    //сохраняем линию в массиве
                    if (UserLine.IsWaitingMouseClickToStoreLine)
                    {
                        if (plot.graphToolsToDrawList == null)
                            plot.graphToolsToDrawList = new List<Data.UserLine>();


                        switch (DrawingTool)
                        {


                            case DrawingTool.VerticalLine:
                                if (UserLine.selectedObj != null)
                                {

                                    plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].IntPt1 = new Point((int)seriesList[0].viewPort.PixelToBarNumber(currentCursorLocation.X), 0);
                                    plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].IntPt2 = new Point((int)seriesList[0].viewPort.PixelToBarNumber(currentCursorLocation.X), 0);
                                    plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].IsVisible = true;
                                    UserLine.selectedObj = null;


                                }
                                else
                                {
                                    plot.graphToolsToDrawList.Add(new VerticalLine(new PointF(seriesList[0].viewPort.PixelToBarNumber(currentCursorLocation.X), 0), new PointF(seriesList[0].viewPort.PixelToBarNumber(currentCursorLocation.X), 0), new ChartV2.Styles.UserLineStyle(UserLine.pen)));
                                }
                                break;



                            case DrawingTool.HorizontalLine:
                                if (UserLine.selectedObj != null)
                                {
                                    plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt1 = new PointF(0, seriesList[0].viewPort.PixelToPrice(currentCursorLocation.Y));
                                    plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt2 = new PointF(0, seriesList[0].viewPort.PixelToPrice(currentCursorLocation.Y));
                                    plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].IsVisible = true;
                                    UserLine.selectedObj = null;
                                }
                                else
                                {
                                    plot.graphToolsToDrawList.Add(new HorizontalLine(new PointF(0, seriesList[0].viewPort.PixelToPrice(currentCursorLocation.Y)), new PointF(0, seriesList[0].viewPort.PixelToPrice(currentCursorLocation.Y)), new ChartV2.Styles.UserLineStyle(UserLine.pen)));

                                }
                                break;

                            case DrawingTool.Cycles:
                                if (UserLine.selectedObj != null)
                                {
                                    plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].IntPt1 = new Point((int)(UserLine.selectedObj.IntPt1.X - UserLine.selectedObj.clickedPoint.X + (int)seriesList[0].viewPort.PixelToBarNumber(currentCursorLocation.X + (int)plot.barWidthInPixel)), 0);
                                    plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].IntPt2 = new Point((int)(UserLine.selectedObj.IntPt2.X - UserLine.selectedObj.clickedPoint.X + (int)seriesList[0].viewPort.PixelToBarNumber(currentCursorLocation.X + (int)plot.barWidthInPixel)), 0);
                                    plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt1 = plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].IntPt1;
                                    plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt2 = plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].IntPt2;
                                    plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].IsVisible = true;
                                    UserLine.selectedObj = null;
                                }
                                else
                                {
                                    pt1.X = seriesList[0].viewPort.PixelToBarNumber(pt1.X + (int)plot.barWidthInPixel);
                                    pt2.X = seriesList[0].viewPort.PixelToBarNumber(pt2.X + (int)plot.barWidthInPixel);
                                    plot.graphToolsToDrawList.Add(new Cycles(pt1, pt2, new ChartV2.Styles.UserLineStyle(UserLine.pen)));
                                
                                }
                                break;

                            case DrawingTool.FreeLine:
                                if (UserLine.selectedObj != null)
                                {
                                    if (!UserLine.DoesParallelDisplacementNeeded)
                                    {
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt1 = new PointF(seriesList[0].viewPort.PixelToBarNumber(currentCursorLocation.X),
                                                                                                                    seriesList[0].viewPort.PixelToPrice(currentCursorLocation.Y));
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt2 = new PointF(seriesList[0].viewPort.PixelToBarNumber(lastCursorLocation.X),
                                                                                                                    seriesList[0].viewPort.PixelToPrice(lastCursorLocation.Y));
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].IsVisible = true;

                                    }
                                    else
                                    {
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt1 = new PointF(seriesList[0].viewPort.PixelToBarNumber(pt1.X - (lastCursorLocation.X - currentCursorLocation.X)),
                                               seriesList[0].viewPort.PixelToPrice(pt1.Y - (lastCursorLocation.Y - currentCursorLocation.Y)));
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt2 = new PointF(seriesList[0].viewPort.PixelToBarNumber(pt2.X - (lastCursorLocation.X - currentCursorLocation.X)),
                                                seriesList[0].viewPort.PixelToPrice(pt2.Y - (lastCursorLocation.Y - currentCursorLocation.Y)));
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].IsVisible = true;

                                        UserLine.DoesParallelDisplacementNeeded = false;
                                    }
                                    UserLine.selectedObj = null;
                                }
                                else
                                {
                                    //первичное сохранение в массив линий
                                    plot.graphToolsToDrawList.Add(new AngularLine(
                                       new PointF(seriesList[0].viewPort.PixelToBarNumber(currentCursorLocation.X),
                                           seriesList[0].viewPort.PixelToPrice(currentCursorLocation.Y)),
                                           new PointF(seriesList[0].viewPort.PixelToBarNumber(lastCursorLocation.X),
                                            seriesList[0].viewPort.PixelToPrice(lastCursorLocation.Y)), new ChartV2.Styles.UserLineStyle(UserLine.pen)));

                                }
                                break;

                            case DrawingTool.Arc:
                                if (UserLine.selectedObj != null)
                                {
                                    if (UserLine.DoesParallelDisplacementNeeded)
                                    {

                                        pt1 = seriesList[0].viewPort.TransformToPixels(UserLine.selectedObj.pt1);
                                        pt2 = seriesList[0].viewPort.TransformToPixels(UserLine.selectedObj.pt2);
                                        lastCursorLocation = seriesList[0].viewPort.TransformToPixels(UserLine.selectedObj.clickedPoint);
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt1 = new PointF(seriesList[0].viewPort.PixelToBarNumber(pt1.X - (lastCursorLocation.X - currentCursorLocation.X)), seriesList[0].viewPort.PixelToPrice(pt1.Y - (lastCursorLocation.Y - currentCursorLocation.Y)));
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt2 = new PointF(seriesList[0].viewPort.PixelToBarNumber(pt2.X - (lastCursorLocation.X - currentCursorLocation.X)), seriesList[0].viewPort.PixelToPrice(pt2.Y - (lastCursorLocation.Y - currentCursorLocation.Y)));



                                        pt1 = seriesList[0].viewPort.TransformToPixels(UserLine.selectedObj.pt3);
                                        pt2 = seriesList[0].viewPort.TransformToPixels(UserLine.selectedObj.pt4);
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt3 = new PointF(seriesList[0].viewPort.PixelToBarNumber(pt1.X - (lastCursorLocation.X - currentCursorLocation.X)), seriesList[0].viewPort.PixelToPrice(pt1.Y - (lastCursorLocation.Y - currentCursorLocation.Y)));
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt4 = new PointF(seriesList[0].viewPort.PixelToBarNumber(pt2.X - (lastCursorLocation.X - currentCursorLocation.X)), seriesList[0].viewPort.PixelToPrice(pt2.Y - (lastCursorLocation.Y - currentCursorLocation.Y)));
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].IsVisible = true;
                                        UserLine.DoesParallelDisplacementNeeded = false;
                                    }
                                    UserLine.selectedObj = null;
                                }
                                else
                                {
                                    int radius = (int)Math.Sqrt(Math.Pow((lastCursorLocation.Y - currentCursorLocation.Y), 2) + Math.Pow((lastCursorLocation.X - currentCursorLocation.X), 2));

                                    plot.graphToolsToDrawList.Add(new Arch(new PointF(seriesList[0].viewPort.PixelToBarNumber(lastCursorLocation.X - radius), seriesList[0].viewPort.PixelToPrice(lastCursorLocation.Y - radius)),
                                                      new PointF(seriesList[0].viewPort.PixelToBarNumber(lastCursorLocation.X + radius), seriesList[0].viewPort.PixelToPrice(lastCursorLocation.Y + radius)),
                                                      new PointF(seriesList[0].viewPort.PixelToBarNumber(lastCursorLocation.X), seriesList[0].viewPort.PixelToPrice(lastCursorLocation.Y)),
                                                      new PointF(seriesList[0].viewPort.PixelToBarNumber(currentCursorLocation.X), seriesList[0].viewPort.PixelToPrice(currentCursorLocation.Y)),
                                          new ChartV2.Styles.UserLineStyle(UserLine.pen)));

                                }
                                break;
                            case DrawingTool.Circle:

                                if (UserLine.selectedObj != null)
                                {

                                    if (UserLine.DoesParallelDisplacementNeeded)
                                    {

                                        pt1 = seriesList[0].viewPort.TransformToPixels(UserLine.selectedObj.pt1);
                                        pt2 = seriesList[0].viewPort.TransformToPixels(UserLine.selectedObj.pt2);
                                        lastCursorLocation = seriesList[0].viewPort.TransformToPixels(UserLine.selectedObj.clickedPoint);
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt1 = new PointF(seriesList[0].viewPort.PixelToBarNumber(pt1.X - (lastCursorLocation.X - currentCursorLocation.X)), seriesList[0].viewPort.PixelToPrice(pt1.Y - (lastCursorLocation.Y - currentCursorLocation.Y)));
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt2 = new PointF(seriesList[0].viewPort.PixelToBarNumber(pt2.X - (lastCursorLocation.X - currentCursorLocation.X)), seriesList[0].viewPort.PixelToPrice(pt2.Y - (lastCursorLocation.Y - currentCursorLocation.Y)));



                                        pt1 = seriesList[0].viewPort.TransformToPixels(UserLine.selectedObj.pt3);
                                        pt2 = seriesList[0].viewPort.TransformToPixels(UserLine.selectedObj.pt4);
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt3 = new PointF(seriesList[0].viewPort.PixelToBarNumber(pt1.X - (lastCursorLocation.X - currentCursorLocation.X)), seriesList[0].viewPort.PixelToPrice(pt1.Y - (lastCursorLocation.Y - currentCursorLocation.Y)));
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt4 = new PointF(seriesList[0].viewPort.PixelToBarNumber(pt2.X - (lastCursorLocation.X - currentCursorLocation.X)), seriesList[0].viewPort.PixelToPrice(pt2.Y - (lastCursorLocation.Y - currentCursorLocation.Y)));
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].IsVisible = true;
                                        UserLine.DoesParallelDisplacementNeeded = false;
                                    }
                                    else
                                    {
                                        //RectangleF tempRect = new RectangleF(pt1.X, pt1.Y,
                                        //                                        pt2.X - (lastCursorLocation.X - currentCursorLocation.X) - pt1.X,
                                        //                                        pt2.Y - (lastCursorLocation.Y - currentCursorLocation.Y) - pt1.Y);
                                        //drawingToolCanvas.DrawEllipse(Pens.Black, tempRect);



                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt1 = new PointF(seriesList[0].viewPort.PixelToBarNumber(pt1.X), seriesList[0].viewPort.PixelToPrice(pt1.Y));
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt2 = new PointF(seriesList[0].viewPort.PixelToBarNumber(pt2.X - (lastCursorLocation.X - currentCursorLocation.X)), seriesList[0].viewPort.PixelToPrice(pt2.Y - (lastCursorLocation.Y - currentCursorLocation.Y)));
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt3 = new PointF(seriesList[0].viewPort.PixelToBarNumber(tempRect.X + tempRect.Width / 2), seriesList[0].viewPort.PixelToPrice(tempRect.Y + tempRect.Height / 2));
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].pt4 = new PointF(seriesList[0].viewPort.PixelToBarNumber(currentCursorLocation.X), seriesList[0].viewPort.PixelToPrice(currentCursorLocation.Y));
                                        plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].IsVisible = true;

                                    }
                                    UserLine.selectedObj = null;
                                }
                                else
                                {
                                    int radius = (int)Math.Sqrt(Math.Pow((lastCursorLocation.Y - currentCursorLocation.Y), 2) + Math.Pow((lastCursorLocation.X - currentCursorLocation.X), 2));
                                    plot.graphToolsToDrawList.Add(new Circle(new PointF(seriesList[0].viewPort.PixelToBarNumber(lastCursorLocation.X - radius), seriesList[0].viewPort.PixelToPrice(lastCursorLocation.Y - radius)),
                                                                             new PointF(seriesList[0].viewPort.PixelToBarNumber(lastCursorLocation.X + radius), seriesList[0].viewPort.PixelToPrice(lastCursorLocation.Y + radius)),
                                                                             new PointF(seriesList[0].viewPort.PixelToBarNumber(lastCursorLocation.X), seriesList[0].viewPort.PixelToPrice(lastCursorLocation.Y)),
                                                                             new PointF(seriesList[0].viewPort.PixelToBarNumber(currentCursorLocation.X), seriesList[0].viewPort.PixelToPrice(currentCursorLocation.Y)),
                                                                 new ChartV2.Styles.UserLineStyle(UserLine.pen)));

                                }
                                break;

                        }
                        UserLine.IsWaitingMouseClickToStoreLine = false;

                        if (UserLine.IsInEditingExistingLine)
                        {
                            UserLine.IsInEditingExistingLine = false;
                            DrawingTool = DrawingTool.None;

                        }
                        plot.Draw();
                        UserLine.SkipSelection = true;

                    }
                    else
                    {
                        //сохраняем координаты первого клика для построения линии.
                        lastCursorLocation.X = e.X - gridSplitter.cell[1].Width;
                        lastCursorLocation.Y = e.Y - gridSplitter.cell[3].Height;
                        //включаем ожидание КЛика т.к. первый только что был совершен.
                        if (DrawingTool != DrawingTool.None)
                        {
                            if (UserLine.MagnetoModeOn)
                                lastCursorLocation = UserLine.MagnetoPoint(lastCursorLocation, seriesList[0]);
                            UserLine.IsWaitingMouseClickToStoreLine = true;
                            UserLine.SkipSelection = true;
                        }
                    }
                }
            }


            //выделяем линии для ПЕРЕРИСОВКИ уже существующих
            if (plot.graphToolsToDrawList != null && plot.graphToolsToDrawList.Count != 0 && !UserLine.SkipSelection)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (IsCrtlPressed)
                    {
                        float Y = seriesList[0].viewPort.PixelToPrice(e.Location.Y - gridSplitter.cell[3].Height);
                        float X = seriesList[0].viewPort.PixelToBarNumber(e.Location.X - gridSplitter.cell[1].Width);
                        for (int i = 0; i < plot.graphToolsToDrawList.Count; i++)
                        {
                            if (plot.graphToolsToDrawList[i].IsSelected)
                            {

                                //проверяю поплал ли клив в уже выдленную линию, если да
                                //то начинаю перерисовывать ее по новым координатам
                                if (plot.graphToolsToDrawList[i].IsLineClicked(X, Y, true))
                                {
                                    UserLine.IsInEditingExistingLine = true;
                                    DrawingTool = plot.graphToolsToDrawList[i].type;


                                    //if (UserLine.DoesParallelDisplacementNeeded)
                                    //{
                                    //    //подготовка точке для параллельного переноса.
                                    //    pt1.X = (int)seriesList[0].viewPort.BarNumberToPixels(plot.graphToolsToDrawList[i].pt1.X);
                                    //    pt1.Y = (int)seriesList[0].viewPort.PriceToPixels(plot.graphToolsToDrawList[i].pt1.Y);
                                    //    pt2.X = (int)seriesList[0].viewPort.BarNumberToPixels(plot.graphToolsToDrawList[i].pt2.X);
                                    //    pt2.Y = (int)seriesList[0].viewPort.PriceToPixels(plot.graphToolsToDrawList[i].pt2.Y);
                                    //    lastCursorLocation.X = (int)seriesList[0].viewPort.BarNumberToPixels(plot.graphToolsToDrawList[i].clickedPoint.X);
                                    //    lastCursorLocation.Y = (int)seriesList[0].viewPort.PriceToPixels(plot.graphToolsToDrawList[i].clickedPoint.Y);
                                    //}
                                    //else
                                    //{
                                    pt1.X = (int)seriesList[0].viewPort.BarNumberToPixels(plot.graphToolsToDrawList[i].pt1.X);
                                    pt1.Y = (int)seriesList[0].viewPort.PriceToPixels(plot.graphToolsToDrawList[i].pt1.Y);
                                    pt2.X = (int)seriesList[0].viewPort.BarNumberToPixels(plot.graphToolsToDrawList[i].pt2.X);
                                    pt2.Y = (int)seriesList[0].viewPort.PriceToPixels(plot.graphToolsToDrawList[i].pt2.Y);
                                    lastCursorLocation.X = (int)seriesList[0].viewPort.BarNumberToPixels(plot.graphToolsToDrawList[i].clickedPoint.X);
                                    lastCursorLocation.Y = (int)seriesList[0].viewPort.PriceToPixels(plot.graphToolsToDrawList[i].clickedPoint.Y);
                                    //}

                                    plot.graphToolsToDrawList[i].IsVisible = false;
                                    UserLine.IsWaitingMouseClickToStoreLine = true;
                                }
                            }
                        }
                        plot.Draw();
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {

                    float Y = seriesList[0].viewPort.PixelToPrice(e.Location.Y - gridSplitter.cell[3].Height);
                    float X = seriesList[0].viewPort.PixelToBarNumber(e.Location.X - gridSplitter.cell[1].Width);
                    for (int i = 0; i < plot.graphToolsToDrawList.Count; i++)
                    {
                        if (plot.graphToolsToDrawList[i].IsSelected && plot.graphToolsToDrawList[i].IsLineClicked(X, Y, true))
                        {
                            if (plot.graphToolsToDrawList[i].type == DrawingTool.FreeLine)
                            {
                                UserToolContextMenu.Items["typeToolStripMenuItem1"].Visible = true;
                            }

                            else
                            {
                                UserToolContextMenu.Items["typeToolStripMenuItem1"].Visible = false;
                            }
                            UserToolContextMenu.Show(this,e.Location);
                            
                        }
                    }
                    
                }
            }
            UserLine.SkipSelection = false;
             
            
        }

        void Control_Click(object sender, EventArgs e)
        {
            Switch_charts_(sender.ToString(), str__);            
           
        }

 

     

     

        private void gridSplitter_Paint(object sender, PaintEventArgs e)
        {

            if (drawingToolBmp != null && (UserLine.IsWaitingMouseClickToStoreLine || cursor == CursorType.Cross || DrawingTool==DrawingTool.Cycles))
                e.Graphics.DrawImage(drawingToolBmp, gridSplitter.cell[4].Location);
            else
            e.Graphics.DrawImage(plot.bmp, gridSplitter.cell[4].Location);
            e.Graphics.DrawImage(rightAxis.bmp, gridSplitter.cell[7].Location);
            e.Graphics.DrawImage(leftAxis.bmp, gridSplitter.cell[1].Location);
            e.Graphics.FillRectangle(bottomAxis.style.backBrush, gridSplitter.cell[8]);
            e.Graphics.FillRectangle(bottomAxis.style.backBrush, gridSplitter.cell[2]);
            e.Graphics.DrawImage(bottomAxis.bmp, gridSplitter.cell[5].Location);
            e.Graphics.FillRectangle( topAxis.style.backBrush,gridSplitter.cell[0]);
            e.Graphics.FillRectangle(topAxis.style.backBrush, gridSplitter.cell[6]);
            e.Graphics.DrawImage(topAxis.bmp, gridSplitter.cell[3].Location);
         


        }

        private void gridSplitter_DraggingRightAxis(object sender, GridSplitter.GridSplitter.MouseDraggingEventArgs e)
        {
           
            ReDrawEveryThing(false);
            DrawingTool = DrawingTool.None;
        }

        private void gridSplitter_DraggingTopAxis(object sender, GridSplitter.GridSplitter.MouseDraggingEventArgs e)
        {
            ReDrawEveryThing(false);
            DrawingTool = DrawingTool.None;
        }

        private void gridSplitter_DraggingLeftAxis(object sender, GridSplitter.GridSplitter.MouseDraggingEventArgs e)
        {
           
            ReDrawEveryThing(false);
            DrawingTool = DrawingTool.None;
        }

        private void gridSplitter_DraggingBottomAxis(object sender, GridSplitter.GridSplitter.MouseDraggingEventArgs e)
        {

            ReDrawEveryThing(false);
            DrawingTool = DrawingTool.None;
        }

        private void gridSplitter_DraggingNothing(object sender, GridSplitter.GridSplitter.MouseDraggingEventArgs e)
        {
          
                if (seriesList != null)
                {
                    switch ((int?)sender)
                    {
                        //Сдвиг графика по всем осям.
                        case 4:                 
                                for (int i = 0; i < seriesList.Count; i++)
                                {
                                    if (e.mouseDragDelta.X != 0)
                                    {
                                        tempTimeVar = (seriesList[i].viewPort.VPMaxTime - seriesList[i].viewPort.VPMinTime) / (e.mouseDragDelta.X * dragSpeed);
                                        seriesList[i].viewPort.VPMinTime -= tempTimeVar;
                                        seriesList[i].viewPort.VPMaxTime -= tempTimeVar;
                                    }
                                    if (!plot.style.IsAutoMargin)
                                    {
                                        if (e.mouseDragDelta.Y != 0)
                                        {
                                            tempPriceVar = (seriesList[i].viewPort.VPMaxPrice - seriesList[i].viewPort.VPMinPrice) / (e.mouseDragDelta.Y * dragSpeed);
                                            seriesList[i].viewPort.VPMinPrice += tempPriceVar;
                                            seriesList[i].viewPort.VPMaxPrice += tempPriceVar;
                                        }
                                    }
                                }

                            break;

                        case 1:
                        case 7:
                            //Масштабирование по оси Y
                                for (int i = 0; i < seriesList.Count; i++)
                                {
                                    if (e.mouseDragDelta.Y != 0)
                                    {
                                        tempPriceVar = (seriesList[i].viewPort.VPMaxPrice - seriesList[i].viewPort.VPMinPrice) / (e.mouseDragDelta.Y * dragSpeed);
                                        seriesList[i].viewPort.VPMinPrice -= tempPriceVar;
                                        seriesList[i].viewPort.VPMaxPrice += tempPriceVar;
                                    }
                                }

                            break;

                        case 3:
                        case 5:
                           //Масштабирование по оси X
                                for (int i = 0; i < seriesList.Count; i++)
                                {
                                    if (e.mouseDragDelta.X != 0)
                                    {
                                        tempPriceVar = (seriesList[i].viewPort.VPMaxTime - seriesList[i].viewPort.VPMinTime) / (e.mouseDragDelta.X * dragSpeed);
                                        seriesList[i].viewPort.VPMinTime -= tempPriceVar;
                                        seriesList[i].viewPort.VPMaxTime += tempPriceVar;
                                    }
                                }

                            break;

                        default:
                            break;


                    }

                    ReDrawEveryThing(false);
                }
        }

        public void redrawTopAxis()
        {
            topAxis.refreshDrawingSurfaces(gridSplitter.cell[3]);
            topAxis.Draw(0);
            gridSplitter.Refresh();
        }
        public void redrawPlot()
        {
            plot.refreshDrawingSurfaces(gridSplitter.cell[4]);
            plot.Draw();
            gridSplitter.Refresh();
        }
        public void ReDrawEveryThing(bool IsFirstTimeDrawing)
        {
            if (IsFirstTimeDrawing)
            {
            plot.seriesToDrawList = seriesList;
            //заселяем данные для отображения шестой точки на оси.
            plot.axisForModelLables = rightAxis;
            plot.topAxis = topAxis;

            
            rightAxis.list = seriesList;
            leftAxis.list = seriesList;
            bottomAxis.list = seriesList;
            topAxis.list = seriesList;
            }

            plot.refreshDrawingSurfaces(gridSplitter.cell[4]);
            plot.Draw();

            rightAxis.refreshDrawingSurfaces(gridSplitter.cell[7]);   
            rightAxis.Draw(0);
                
            leftAxis.refreshDrawingSurfaces(gridSplitter.cell[1]);
            leftAxis.Draw(0);

            bottomAxis.refreshDrawingSurfaces(gridSplitter.cell[5]);
            bottomAxis.Draw(0);

            topAxis.refreshDrawingSurfaces(gridSplitter.cell[3]);
            topAxis.Draw(0);
            //измененеи размеров клеток ГридСплиттера для крректного первого показа. Чтобы влезли все лейблы в облать отрисовки.

            if (IsFirstTimeDrawing)
            {
               gridSplitter.LeftAxisWidth = (int)rightAxis.lableSize.Width + 10;
               gridSplitter.RightAxisWidth = (int)rightAxis.lableSize.Width + 10;
               gridSplitter.BottomAxisHeight =  (int)bottomAxis.lableSize.Height + 30;
               ReDrawEveryThing(false);
            }
            gridSplitter.Refresh();
            
        }

        private void Control_Resize(object sender, EventArgs e)
        {
          //нужно именно два вызова функции отрисовки. Ибо проблему поздней отрисовки
           // Grid не решить иначе.
            if (plot != null)
            {
                ReDrawEveryThing(false);
                ReDrawEveryThing(false);
            }
        }


        private void ChartPick_CheckStateChanged(object sender, EventArgs e)
        {
            if (sender.Equals(LineChartPick))
            {
               workingForm.SetChartType(ChartType.Line); 
            }
            else if (sender.Equals(CandleChartPick))
            {
                workingForm.SetChartType(ChartType.Candle); 
            }
        }
     

        private void gridSplitter_MouseMove(object sender, MouseEventArgs e)
        {
          
            currentCursorLocation.X = e.X - gridSplitter.cell[1].Width;
            currentCursorLocation.Y = e.Y - gridSplitter.cell[3].Height;
            topAxis.mouseCursorXcoordinate = (int)currentCursorLocation.X + (int)plot.barWidthInPixel;


            bottomAxis.cursorLable.Value = seriesList[0].viewPort.PixelToBarNumber(topAxis.mouseCursorXcoordinate);
          //  bottomAxis.cursorLable.rect.X = e.X - gridSplitter.cell[1].Width;
            bottomAxis.cursorLable.rect.X = currentCursorLocation.X;
            bottomAxis.cursorLable.rect.Y = 10;
            
            if (cursor == CursorType.Cross)
            {
                drawingToolBmp = (Image)plot.bmpCopyforCrossCursor.Clone();
                drawingToolCanvas = Graphics.FromImage(drawingToolBmp);
                drawingToolCanvas.DrawLine(Pens.Black, 0, currentCursorLocation.Y, plot.bmp.Width, currentCursorLocation.Y);
                drawingToolCanvas.DrawLine(Pens.Black, currentCursorLocation.X, 0, currentCursorLocation.X, plot.bmp.Height);
               // plot.bmp = (Bitmap)drawingToolBmp;
            }


            if (seriesList[0].legend.log)
                rightAxis.cursorLable.Value = (float)Math.Pow(10, seriesList[0].viewPort.PixelToPrice(currentCursorLocation.Y));
            else
                rightAxis.cursorLable.Value = seriesList[0].viewPort.PixelToPrice(currentCursorLocation.Y);
            rightAxis.cursorLable.rect.Y = currentCursorLocation.Y;
            rightAxis.cursorLable.rect.X =10;

            if (seriesList[0].legend.log)
                leftAxis.cursorLable.Value = (float)Math.Pow(10, seriesList[0].viewPort.PixelToPrice(currentCursorLocation.Y));
            else
                leftAxis.cursorLable.Value = seriesList[0].viewPort.PixelToPrice(currentCursorLocation.Y);
            leftAxis.cursorLable.rect.Y = currentCursorLocation.Y;

            bottomAxis.Draw(0);
            leftAxis.Draw(0);
            rightAxis.Draw(0);
            topAxis.Draw(0);
           //тянем линии от предыдущего клика до текущего положения курсора.
          //  
            {
                switch (DrawingTool)
                {
                    case DrawingTool.VerticalLine:
                        UserLine.IsWaitingMouseClickToStoreLine = true;
                        if (cursor != CursorType.Cross)
                        {
                            drawingToolBmp = (Image)plot.bmp.Clone();
                            drawingToolCanvas = Graphics.FromImage(drawingToolBmp);
                        }

                        pt1.X = (int)seriesList[0].viewPort.PixelToBarNumber(currentCursorLocation.X);
                        pt1.X = (int)seriesList[0].viewPort.BarNumberToPixels(pt1.X);
                        UserLine.DrawItself(drawingToolCanvas,
                            new Point((int)pt1.X, 0),
                            new Point((int)pt1.X, plot.bmp.Height),
                            DrawingTool.VerticalLine);

                        break;
                    case DrawingTool.HorizontalLine:
                        UserLine.IsWaitingMouseClickToStoreLine = true;
                        if (cursor != CursorType.Cross)
                        {
                            drawingToolBmp = (Image)plot.bmp.Clone();
                            drawingToolCanvas = Graphics.FromImage(drawingToolBmp);
                        }
                        UserLine.DrawItself(drawingToolCanvas, new Point(0, (int)currentCursorLocation.Y), new Point(plot.bmp.Width, (int)currentCursorLocation.Y), DrawingTool.HorizontalLine);

                        break;
                    case DrawingTool.Circle:

                        if (UserLine.IsWaitingMouseClickToStoreLine)
                        {
                            if (cursor != CursorType.Cross)
                            {
                                drawingToolBmp = (Image)plot.bmp.Clone();
                                drawingToolCanvas = Graphics.FromImage(drawingToolBmp);
                            }
                            if (UserLine.IsInEditingExistingLine)
                            {
                                if (UserLine.DoesParallelDisplacementNeeded)
                                {
                                    tempRect = new RectangleF(
                                        pt1.X - (lastCursorLocation.X - currentCursorLocation.X),
                                        pt1.Y - (lastCursorLocation.Y - currentCursorLocation.Y), pt2.X - pt1.X, pt2.Y - pt1.Y);
                                    drawingToolCanvas.DrawEllipse(Pens.Black, tempRect);
                                    //UserLine.DrawItself(drawingToolCanvas, lastCursorLocation, currentCursorLocation, DrawingTool.Circle);
                                }
                                else
                                {
                                    tempRect = new RectangleF(pt1.X, pt1.Y, 
                                        pt2.X - (lastCursorLocation.X - currentCursorLocation.X) - pt1.X, 
                                        pt2.Y - (lastCursorLocation.Y - currentCursorLocation.Y) - pt1.Y);
                                    drawingToolCanvas.DrawEllipse(Pens.Black, tempRect);
                                }
                            }
                            else
                            {
                                if (UserLine.MagnetoModeOn)
                                currentCursorLocation = UserLine.MagnetoPoint(currentCursorLocation, seriesList[0]);
                                UserLine.DrawItself(drawingToolCanvas, lastCursorLocation, currentCursorLocation, DrawingTool.Circle);
                            }
                            }
                        break;
                    case DrawingTool.FreeLine:

                        if (UserLine.IsWaitingMouseClickToStoreLine)
                        {
                            if (cursor != CursorType.Cross)
                            {
                                drawingToolBmp = (Image)plot.bmp.Clone();
                                drawingToolCanvas = Graphics.FromImage(drawingToolBmp);
                            }
                            if (UserLine.DoesParallelDisplacementNeeded)
                            {
                                UserLine.DrawItself(drawingToolCanvas,
                                    new PointF(pt1.X - (lastCursorLocation.X - currentCursorLocation.X),
                                    pt1.Y - (lastCursorLocation.Y - currentCursorLocation.Y)),
                                     new PointF(pt2.X - (lastCursorLocation.X - currentCursorLocation.X),
                                    pt2.Y - (lastCursorLocation.Y - currentCursorLocation.Y)),
                                    DrawingTool.FreeLine);
                            }
                            else
                            {
                                if (UserLine.MagnetoModeOn)
                                currentCursorLocation = UserLine.MagnetoPoint(currentCursorLocation, seriesList[0]);

                                UserLine.DrawItself(drawingToolCanvas, lastCursorLocation, currentCursorLocation, DrawingTool.FreeLine);
                            }
                        }
                        break;
                    case DrawingTool.Arc:
                        if (UserLine.IsWaitingMouseClickToStoreLine)
                        {

                            if (cursor != CursorType.Cross)
                            {
                                drawingToolBmp = (Image)plot.bmp.Clone();
                                drawingToolCanvas = Graphics.FromImage(drawingToolBmp);
                            }
                            if (UserLine.DoesParallelDisplacementNeeded)
                            {
                                tempRect = new RectangleF(
                                    pt1.X - (lastCursorLocation.X - currentCursorLocation.X),
                                    pt1.Y - (lastCursorLocation.Y - currentCursorLocation.Y), pt2.X - pt1.X, pt2.Y - pt1.Y);
                                drawingToolCanvas.DrawEllipse(Pens.Black, tempRect);
                                //UserLine.DrawItself(drawingToolCanvas, lastCursorLocation, currentCursorLocation, DrawingTool.Circle);
                            }
                            else
                            {
                                if (UserLine.MagnetoModeOn)
                                currentCursorLocation = UserLine.MagnetoPoint(currentCursorLocation, seriesList[0]);
                                UserLine.DrawItself(drawingToolCanvas, lastCursorLocation, currentCursorLocation, DrawingTool.Arc);
                            }
                        }
                        break;


                    case DrawingTool.Cycles:
                        if (UserLine.IsWaitingMouseClickToStoreLine)
                        {
                            //отрисовка циклов
                            if (cursor != CursorType.Cross)
                            {
                                drawingToolBmp = (Image)plot.bmp.Clone();
                                drawingToolCanvas = Graphics.FromImage(drawingToolBmp);
                            }
                            if (UserLine.DoesParallelDisplacementNeeded)
                            {

                                UserLine.DrawItself(drawingToolCanvas,
                                    new PointF((pt1.X - (lastCursorLocation.X - currentCursorLocation.X)),
                                    0),
                                     new PointF((pt2.X - (lastCursorLocation.X - currentCursorLocation.X)),
                                    plot.bmp.Height),
                                    DrawingTool.Cycles);
                            }
                            else
                            {
                                pt1.X = (int)seriesList[0].viewPort.PixelToBarNumber(lastCursorLocation.X);
                                pt1.X = (int)seriesList[0].viewPort.BarNumberToPixels(pt1.X);
                                pt1.Y = 0;
                                pt2.X = (int)seriesList[0].viewPort.PixelToBarNumber(currentCursorLocation.X);
                                pt2.X = (int)seriesList[0].viewPort.BarNumberToPixels(pt2.X);
                                pt2.Y = plot.bmp.Height;
                                UserLine.DrawItself(drawingToolCanvas,pt1,pt2,DrawingTool.Cycles);
                            }
                            
                        }
                        else
                        {
                            //отрисовка первой линии без циклов.
                            if (cursor != CursorType.Cross)
                            {
                                drawingToolBmp = (Image)plot.bmp.Clone();
                                drawingToolCanvas = Graphics.FromImage(drawingToolBmp);
                            }

                            UserLine.DrawItself(drawingToolCanvas,
                                new Point((int)(currentCursorLocation.X), 0),
                                new Point((int)(currentCursorLocation.X), plot.bmp.Height),
                                DrawingTool.VerticalLine);
                        }

                        break;

                    case DrawingTool.None:
                        UserLine.IsWaitingMouseClickToStoreLine = false;
                        break;
                }
            }


            this.Refresh();
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                plot.style.backColor = colorDialog1.Color;
                plot.Draw();
                gridSplitter.Refresh();
            }
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                //заменить надо на работу со свойствами, чтобы не переделывать Ручки, а только цвета. (так больше логики)
                plot.grid.style.minorVerticalPen = new Pen(colorDialog1.Color);
                plot.grid.style.minorHorizontalPen = plot.grid.style.minorVerticalPen;
                plot.grid.style.minorVerticalPen.DashPattern = new float[2] { plot.grid.style.minV1, plot.grid.style.minV2 };
                plot.grid.style.minorHorizontalPen.DashPattern = new float[2] { plot.grid.style.minH1, plot.grid.style.minH2 };
            }
            ReDrawEveryThing(false);
        }




        

        private void FindModelStripMenuItem_Click(object sender, EventArgs e)
        {
            if (seriesList != null)
            {
               // TAmodel model = new TAmodel();
                
            }
        }

        private void dynamicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rightAxis = new Axis_Plot.RightAxisDinamic(gridSplitter.cell[7]);
            rightAxis.list = seriesList;
            rightAxis.grid = plot.grid;
        }

        private void staticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rightAxis = new Axis_Plot.RightAxisStatic(gridSplitter.cell[7]);
            rightAxis.list = seriesList;
            rightAxis.grid = plot.grid;
        }

        private void arrowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cursor = CursorType.Arrow;
        }

        private void crossToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cursor = CursorType.Cross;
        }
        // ====================================================================
        public void Cursor_Arrow()
        {
            cursor = CursorType.Arrow;
        }
        public void Cursor_Cross()
        {
            cursor = CursorType.Cross;
        }
        // =====================================================================
        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridSplitter.IsGridVisible = false;
            gridSplitter.Refresh();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridSplitter.IsGridVisible = true;
            gridSplitter.Refresh();
        }

        private void hideToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            plot.IsModelsVisible = false;
            plot.IsHTVisible = false;
            ReDrawEveryThing(false);
        }

        private void showToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            plot.IsModelsVisible = true;
            plot.IsHTVisible = true;
            ReDrawEveryThing(false);
            
        }

        internal void skipScalingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            seriesList[0].viewPort = new ViewPort(seriesList[0]);
            ReDrawEveryThing(false);
        }

        private void gridSplitter_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:   
                    IsCrtlPressed = false;
                    UserLine.MagnetoModeOn = true;
                    break;

                case Keys.M:
                    plot.style.IsAutoMargin = !plot.style.IsAutoMargin;
                    break;

                case Keys.Delete:
                    if (plot.graphToolsToDrawList != null && plot.graphToolsToDrawList.Count != 0)
                    {
                        if (!UserLine.IsInEditingExistingLine && !UserLine.DoesParallelDisplacementNeeded && !UserLine.IsWaitingMouseClickToStoreLine)
                        {
                            if (UserLine.selectedLineIndex != null)
                            {
                                plot.graphToolsToDrawList.RemoveAt((int)UserLine.selectedLineIndex);
                                UserLine.selectedLineIndex = null;
                                UserLine.selectedObj = null;
                            }
                            //обнуляю флаг, чтобы при новой отрисовке круга или линии не взял старые коодинаты из буфера
                            UserLine.DoesParallelDisplacementNeeded = false;
                        }
                        plot.Draw();
                        gridSplitter.Refresh();
                    }
                    break;

                case Keys.N:
                    if (plot.modelsToDrawList != null)
                    {
                        for (int i = 0; i < plot.modelsToDrawList.Count; i++)
                        {
                            if (plot.modelsToDrawList[i].IsSelected && i != plot.modelsToDrawList.Count-2)
                            {
                                plot.modelsToDrawList[i].IsSelected = false; 
                                plot.modelsToDrawList[i + 1].IsSelected = true;
                                break;

                            }
                        }
                    }
                    plot.Draw();
                    break;
                case Keys.B:
                    if (plot.modelsToDrawList != null)
                    {
                        for (int i = 0; i < plot.modelsToDrawList.Count; i++)
                        {
                            if (plot.modelsToDrawList[i].IsSelected && i >= 1)
                            {
                                plot.modelsToDrawList[i].IsSelected = false;
                                plot.modelsToDrawList[i - 1].IsSelected = true;
                                break;

                            }
                        }
                    }
                    plot.Draw();
                    break;

                //Отменить все выделения
                case Keys.Escape:

                    if (plot.modelsToDrawList != null && plot.IsModelsVisible)
                    {
                        
                        Model.lastSelectedModelIndex = null;
                        for (int i = 0; i < plot.modelsToDrawList.Count; i++)
                        {
                            plot.modelsToDrawList[i].IsSelected = false;
                        }
                        for (int i = 0; i < plot.HTtoDrawList.Count; i++)
                        {
                            plot.HTtoDrawList[i].SelectedCount = 0;
                        }

                        plot.Draw();
                    }
                   
                    break;

                ////сдвинуть график вправо
                //case Keys.Left:
                //    tempPriceVar = (seriesList[0].viewPort.VPMaxTime - seriesList[0].viewPort.VPMinTime) / 3;
                //    if (seriesList[0].viewPort.VPMinTime + tempPriceVar < seriesList[0].data.Count - 1)
                //    {
                //        seriesList[0].viewPort.VPMinTime += tempPriceVar;
                //        seriesList[0].viewPort.VPMaxTime += tempPriceVar;
                //        ReDrawEveryThing(false);
                //    }

                //    break;
                ////сдвинуть график влево
                //case Keys.Right:
                //    tempPriceVar = (seriesList[0].viewPort.VPMaxTime - seriesList[0].viewPort.VPMinTime) / 3;
                //    if (seriesList[0].viewPort.VPMaxTime - tempPriceVar > 1)
                //    {
                //        seriesList[0].viewPort.VPMinTime -= tempPriceVar;
                //        seriesList[0].viewPort.VPMaxTime -= tempPriceVar;
                //        ReDrawEveryThing(false);
                //    }
                //    break;

                default:
                  
                    break;

            }
        }

        private void gridSplitter_MouseDoubleClick(object sender, MouseEventArgs e)
        {
             
            if (e.Button == MouseButtons.Left)
            {
                if (plot.modelsToDrawList != null && plot.IsModelsVisible)
                {
                    if (!IsCrtlPressed)
                    {
                        
                        Model.lastSelectedModelIndex = null;
                        for (int i = 0; i < plot.modelsToDrawList.Count; i++)
                        {
                            //обрезаю модели расположенные до видимого окна
                            if (plot.modelsToDrawList[i].PointsForDrawing_21_4[1].X < seriesList[0].viewPort.VPMinTime)
                                continue;
                            //обрезаю модели расположенные полсе видимого окна
                            if (Math.Min(plot.modelsToDrawList[i].PointsForDrawing_1_3[0].X, plot.modelsToDrawList[i].PointsForDrawing_1_3[1].X) > seriesList[0].viewPort.VPMaxTime)
                                continue;


                            if (plot.modelsToDrawList[i].IsModelClicked(e.Location.X - gridSplitter.cell[1].Width, e.Location.Y - gridSplitter.cell[3].Height, seriesList[0].viewPort))
                            {
                                //обнуляю флаги редактирвоания ручных построений. 
                                //чтобы избежать конфликта.
                                UserLine.IsInEditingExistingLine = false;
                                DrawingTool = DrawingTool.None;
                                rightAxis.IsTargetsVisible = true;
                                plot.modelsToDrawList[i].IsSelected = !plot.modelsToDrawList[i].IsSelected;
                                if (plot.modelsToDrawList[i].HTi >= 0)
                                    plot.HTtoDrawList[plot.modelsToDrawList[i].HTi].SelectedCount += plot.modelsToDrawList[i].IsSelected ? 1 : -1;
                                if (plot.modelsToDrawList[i].IsSelected)
                                {
                                    Model.selectedModelIndexes.Add(i);

                                    if (plot.modelsToDrawList[i].type != ModelType.ATR)
                                    {
                                        if ((!plot.modelsToDrawList[i].ModelPoint[5].IsEmpty) && (plot.modelsToDrawList[i].ModelPoint[5].X != plot.modelsToDrawList[i].ModelPointReal[6].X))
                                        {
                                            //цели МР и МДР
                                            tempTargetLable = new ChartV2.Axis_Plot.CursorInfoLable();
                                            tempTargetLable.Value =plot.modelsToDrawList[i].ModelPoint[5].Y;
                                            tempTargetLable.rect.Y = plot.seriesToDrawList[0].viewPort.PriceToPixels(tempTargetLable.Value);
                                            if (!rightAxis.targetLable.ContainsKey(i))
                                                rightAxis.targetLable.Add(i, tempTargetLable);
                                        }
                                    }
                                    else
                                    {
                                        //цели МП
                                        tempTargetLable = new ChartV2.Axis_Plot.CursorInfoLable();
                                        tempTargetLable.IsVisible = true;
                                        tempTargetLable.Value = plot.modelsToDrawList[i].PointsForDrawing_1_3[1].Y;                                
                                         
                                        if (!rightAxis.targetLable.ContainsKey(i))
                                            rightAxis.targetLable.Add(i, tempTargetLable);

                                    }
                                }
                                else
                                {
                                    Model.selectedModelIndexes.Remove(i);
                                    if (plot.modelsToDrawList[i].type != ModelType.ATR)
                                    {
                                        if ((!plot.modelsToDrawList[i].ModelPoint[5].IsEmpty) && (plot.modelsToDrawList[i].ModelPoint[5].X != plot.modelsToDrawList[i].ModelPointReal[6].X))
                                        {
                                            rightAxis.targetLable.Remove(i);
                                            rightAxis.targetLable.Remove(-i);
                                        }
                                    }
                                    else
                                    {
                                        rightAxis.targetLable.Remove(i);
                                        rightAxis.targetLable.Remove(-i);
                                    }
                                }

                               

                                Model.lastSelectedModelIndex = i;
                                break;
                            }

                        }

                }
                }
                if (plot.graphToolsToDrawList != null && DrawingTool==DrawingTool.None)
                {
                    if (IsCrtlPressed)
                    {
                        UserLine.selectedObj = null;
                        UserLine.selectedLineIndex = null;
                        float Y = seriesList[0].viewPort.PixelToPrice(e.Location.Y - gridSplitter.cell[3].Height);
                        float X = seriesList[0].viewPort.PixelToBarNumber(e.Location.X - gridSplitter.cell[1].Width);
                        for (int i = 0; i < plot.graphToolsToDrawList.Count; i++)
                        {
                            if (plot.graphToolsToDrawList[i].IsLineClicked(X, Y, false))
                            {
                                plot.graphToolsToDrawList[i].IsSelected = true;
                                UserLine.selectedLineIndex = i;
                                for (int ii = ++i; ii < plot.graphToolsToDrawList.Count; ii++)
                                {
                                    plot.graphToolsToDrawList[ii].IsSelected = false;
                                }
                                break;
                            }
                            else
                            {
                                plot.graphToolsToDrawList[i].IsSelected = false;
                            }
                        }
                    }
                
                }
                plot.Draw();
            }

        }

        private void verticalLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawingTool = DrawingTool.VerticalLine;

        }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawingTool = DrawingTool.Circle;
            UserLine.IsWaitingMouseClickToStoreLine = false;

        }

        private void horizontalLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawingTool = DrawingTool.HorizontalLine;

        }
        // ============================================================
        public void HorizontalLine()
        {
            DrawingTool = DrawingTool.HorizontalLine;

        }
        public void VerticalLine()
        {
            DrawingTool = DrawingTool.VerticalLine;


        }
        public void Circle()
        {
            DrawingTool = DrawingTool.Circle;
            UserLine.IsWaitingMouseClickToStoreLine = false;

        }
        public void FreeLine()
        {
            DrawingTool = DrawingTool.FreeLine;
            UserLine.IsWaitingMouseClickToStoreLine = false;

        }
        public void Arc()
        {
            DrawingTool = DrawingTool.Arc;
            UserLine.IsWaitingMouseClickToStoreLine = false;

        }
        public void Cycles()
        {
            DrawingTool = DrawingTool.Cycles;
            UserLine.IsWaitingMouseClickToStoreLine = false;

        }
        //  ===========================================================

        //protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        //{
        //    const int WM_KEYDOWN = 0x100;
        //    const int WM_KEYUP = 0x101;

        //    if (msg.Msg == WM_KEYDOWN)
        //        //if (msg.WParam.ToInt32() == (int)Keys.Left || msg.WParam.ToInt32() == (int)Keys.Right || msg.WParam.ToInt32() == (int)Keys.Up || msg.WParam.ToInt32() == (int)Keys.Down)
        //            gridSplitter_KeyDown(null, new KeyEventArgs(keyData));
        //    return true;
        //}

        private void gridSplitter_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    IsCrtlPressed = true;
                    UserLine.MagnetoModeOn = false;
                    break;

                //сдвинуть график вверх
                case Keys.Down:
                    if (!plot.style.IsAutoMargin)
                    {
                        tempPriceVar = (seriesList[0].viewPort.VPMaxPrice - seriesList[0].viewPort.VPMinPrice) / 20;
                        if (seriesList[0].viewPort.VPMaxPrice - tempPriceVar > seriesList[0].viewPort.absRealPriceMin - 0.02 * seriesList[0].viewPort.absRealPriceMin)
                        {
                            seriesList[0].viewPort.VPMaxPrice -= tempPriceVar;
                            seriesList[0].viewPort.VPMinPrice -= tempPriceVar;

                            ReDrawEveryThing(false);
                        }
                    }
                    break;
                //сдвинуть график вниз
                case Keys.Up:
                    if (!plot.style.IsAutoMargin)
                    {
                        tempPriceVar = (seriesList[0].viewPort.VPMaxPrice - seriesList[0].viewPort.VPMinPrice) / 20;

                        if (seriesList[0].viewPort.VPMinPrice + tempPriceVar < seriesList[0].viewPort.absRealPriceMax + 0.02 * seriesList[0].viewPort.absRealPriceMax)
                        {
                            seriesList[0].viewPort.VPMaxPrice += tempPriceVar;
                            seriesList[0].viewPort.VPMinPrice += tempPriceVar;

                            ReDrawEveryThing(false);
                        }
                    }
                    break;

                //пролистнуть график вправо
                case Keys.PageDown:
                    
                    tempPriceVar = (seriesList[0].viewPort.VPMaxTime - seriesList[0].viewPort.VPMinTime) / 2;
                    if (seriesList[0].viewPort.VPMinTime + tempPriceVar < seriesList[0].data.Count - 1)
                    {
                        seriesList[0].viewPort.VPMinTime += tempPriceVar;
                        seriesList[0].viewPort.VPMaxTime += tempPriceVar;
                        ReDrawEveryThing(false);
                    }
                   
                    break;
                //пролистнуть график в начало
                case Keys.Home:
                    float width = (seriesList[0].viewPort.VPMaxTime - seriesList[0].viewPort.VPMinTime);
                    if (seriesList[0].viewPort.VPMinTime + tempPriceVar < seriesList[0].data.Count - 1)
                    {
                        seriesList[0].viewPort.VPMinTime = - 10;
                        seriesList[0].viewPort.VPMaxTime = width - 10;
                        ReDrawEveryThing(false);
                    }

                    break;

                //пролистнуть график в конец
                case Keys.End:
                    width = (seriesList[0].viewPort.VPMaxTime - seriesList[0].viewPort.VPMinTime);
                    if (seriesList[0].viewPort.VPMinTime + tempPriceVar < seriesList[0].data.Count - 1)
                    {
                        seriesList[0].viewPort.VPMinTime = seriesList[0].data.Count + 10 - width;
                        seriesList[0].viewPort.VPMaxTime = seriesList[0].data.Count + 10;
                        ReDrawEveryThing(false);
                    }

                    break;
                //сместить выделение на следующую модель
                case Keys.Right:
                    if (e.Modifiers == Keys.Alt)
                    {
                        if (Model.lastSelectedModelIndex != null)
                        {

                            if (Model.lastSelectedModelIndex < plot.modelsToDrawList.Count - 1)
                            {
                                
                                plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].IsSelected = false;
                                Model.selectedModelIndexes.Remove((int)Model.lastSelectedModelIndex);
                                if (plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].HTi >= 0) 
                                    plot.HTtoDrawList[plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].HTi].SelectedCount --;

                                rightAxis.targetLable.Remove((int)Model.lastSelectedModelIndex);
                                rightAxis.targetLable.Remove(-(int)Model.lastSelectedModelIndex);
                                
                                plot.modelsToDrawList[(int)++Model.lastSelectedModelIndex].IsSelected = true;
                                Model.selectedModelIndexes.Add((int)Model.lastSelectedModelIndex);
                                if (plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].HTi >= 0) 
                                    plot.HTtoDrawList[plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].HTi].SelectedCount ++;

                                if (plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].type != ModelType.ATR)
                                {
                                    if ((!plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].ModelPoint[5].IsEmpty) && (plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].ModelPoint[5].X != plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].ModelPointReal[6].X))
                                    {
                                        //цели МР и МДР
                                        tempTargetLable = new ChartV2.Axis_Plot.CursorInfoLable();

                                        tempTargetLable.Value = plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].ModelPoint[5].Y;
                                        tempTargetLable.rect.Y = plot.seriesToDrawList[0].viewPort.PriceToPixels(tempTargetLable.Value);

                                        if (!rightAxis.targetLable.ContainsKey((int)Model.lastSelectedModelIndex))
                                            rightAxis.targetLable.Add((int)Model.lastSelectedModelIndex, tempTargetLable);


                                        tempTargetLable = new ChartV2.Axis_Plot.CursorInfoLable();
                                        tempTargetLable.Value = plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].FarPoint6[0].Y;
                                        tempTargetLable.rect.Y = plot.seriesToDrawList[0].viewPort.PriceToPixels(tempTargetLable.Value);
                                        if (!rightAxis.targetLable.ContainsKey(-(int)Model.lastSelectedModelIndex))
                                            rightAxis.targetLable.Add(-(int)Model.lastSelectedModelIndex, tempTargetLable);
                                    }
                                }
                                else
                                {
                                    //цели МП
                                    tempTargetLable = new ChartV2.Axis_Plot.CursorInfoLable();
                                    tempTargetLable.IsVisible = true;
                                    tempTargetLable.Value =
                                       plot.seriesToDrawList[0].legend.log ?
                                       (float)Math.Pow(10, plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].PointsForDrawing_1_3[1].Y) :
                                       plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].PointsForDrawing_1_3[1].Y;
                                    if (!rightAxis.targetLable.ContainsKey((int)Model.lastSelectedModelIndex))
                                        rightAxis.targetLable.Add((int)Model.lastSelectedModelIndex, tempTargetLable);

                                }
                                
                                
                            }
                        }
                    }
                    else
                    {
                        tempPriceVar = 10;
                        if (seriesList[0].viewPort.VPMinTime + tempPriceVar < seriesList[0].data.Count - 1)
                        {
                            seriesList[0].viewPort.VPMinTime += tempPriceVar;
                            seriesList[0].viewPort.VPMaxTime += tempPriceVar;
                            ReDrawEveryThing(false);
                        }
                    }
                    ReDrawEveryThing(false);
                    gridSplitter_MouseMove(null, new MouseEventArgs(MouseButtons.None, 0, (int)currentCursorLocation.X + gridSplitter.cell[1].Width, (int)currentCursorLocation.Y + gridSplitter.cell[3].Height, 0));
                    break;
                //пролистнуь график влево
                case Keys.PageUp:
                   
                    tempPriceVar = (seriesList[0].viewPort.VPMaxTime - seriesList[0].viewPort.VPMinTime) / 2;
                    if (seriesList[0].viewPort.VPMaxTime - tempPriceVar > 1)
                    {
                        seriesList[0].viewPort.VPMinTime -= tempPriceVar;
                        seriesList[0].viewPort.VPMaxTime -= tempPriceVar;
                        ReDrawEveryThing(false);
                    }
                    break;
                //сместить выделение на предыдущую модель
                case Keys.Left:
                    if (e.Modifiers == Keys.Alt)
                    {
                        if (Model.lastSelectedModelIndex != null)
                        {

                            if (Model.lastSelectedModelIndex >= 1)
                            {

                                plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].IsSelected = false;
                                Model.selectedModelIndexes.Remove((int)Model.lastSelectedModelIndex);
                                if (plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].HTi >= 0) plot.HTtoDrawList[plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].HTi].SelectedCount--;
                                {
                                    rightAxis.targetLable.Remove((int)Model.lastSelectedModelIndex);
                                    rightAxis.targetLable.Remove(-(int)Model.lastSelectedModelIndex);
                                }

                                plot.modelsToDrawList[(int)--Model.lastSelectedModelIndex].IsSelected = true;
                                if (plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].HTi >= 0) plot.HTtoDrawList[plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].HTi].SelectedCount++;
                                Model.selectedModelIndexes.Add((int)Model.lastSelectedModelIndex);

                                if (plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].type != ModelType.ATR)
                                {
                                    if ((!plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].ModelPoint[5].IsEmpty)
                                        && (plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].ModelPoint[5].X != plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].ModelPointReal[6].X))
                                    {
                                        //цели МР и МДР
                                        tempTargetLable = new ChartV2.Axis_Plot.CursorInfoLable();

                                        tempTargetLable.Value = plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].ModelPoint[5].Y;
                                        //tempTargetLable.rect.Y = plot.seriesToDrawList[0].viewPort.PriceToPixels(plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].ModelPoint[5].Y);
                                        tempTargetLable.rect.Y = plot.seriesToDrawList[0].viewPort.PriceToPixels(tempTargetLable.Value);
                                        if (!rightAxis.targetLable.ContainsKey((int)Model.lastSelectedModelIndex))
                                            rightAxis.targetLable.Add((int)Model.lastSelectedModelIndex, tempTargetLable);

                                        tempTargetLable = new ChartV2.Axis_Plot.CursorInfoLable();
                                        tempTargetLable.Value = plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].FarPoint6[0].Y;
                                        tempTargetLable.rect.Y = plot.seriesToDrawList[0].viewPort.PriceToPixels(tempTargetLable.Value);
                                        if (!rightAxis.targetLable.ContainsKey(-(int)Model.lastSelectedModelIndex))
                                            rightAxis.targetLable.Add(-(int)Model.lastSelectedModelIndex, tempTargetLable);
                                    }
                                }
                                else
                                {
                                    //цели МП
                                    tempTargetLable = new ChartV2.Axis_Plot.CursorInfoLable();
                                    tempTargetLable.IsVisible = true;
                                    tempTargetLable.Value =
                                       plot.modelsToDrawList[(int)Model.lastSelectedModelIndex].PointsForDrawing_1_3[1].Y;
                                    if (!rightAxis.targetLable.ContainsKey((int)Model.lastSelectedModelIndex))
                                        rightAxis.targetLable.Add((int)Model.lastSelectedModelIndex, tempTargetLable);

                                }
                            }
                        }
                    }
                    else
                    {
                        tempPriceVar = 10;
                        if (seriesList[0].viewPort.VPMaxTime - tempPriceVar > 1)
                        {
                            seriesList[0].viewPort.VPMinTime -= tempPriceVar;
                            seriesList[0].viewPort.VPMaxTime -= tempPriceVar;
                            ReDrawEveryThing(false);
                        }
                    }
                    ReDrawEveryThing(false);
                    gridSplitter_MouseMove(null, new MouseEventArgs(MouseButtons.None, 0, (int)currentCursorLocation.X + gridSplitter.cell[1].Width, (int)currentCursorLocation.Y + gridSplitter.cell[3].Height, 0));
                    break;
                //zoomIN
                case Keys.Oemplus:
                    tempPriceVar = (seriesList[0].viewPort.VPMaxPrice - seriesList[0].viewPort.VPMinPrice) / (dragSpeed * 10);
                    seriesList[0].viewPort.VPMinPrice += tempPriceVar;
                    seriesList[0].viewPort.VPMaxPrice -= tempPriceVar;
                    ReDrawEveryThing(false);
                    break;
                //ZoomOUT
                case Keys.OemMinus:
                    tempPriceVar = (seriesList[0].viewPort.VPMaxPrice - seriesList[0].viewPort.VPMinPrice) / (dragSpeed * 10);
                    seriesList[0].viewPort.VPMinPrice -= tempPriceVar;
                    seriesList[0].viewPort.VPMaxPrice += tempPriceVar;
                    ReDrawEveryThing(false);
                    break;
                //zoomIN
                case Keys.OemCloseBrackets:
                    tempPriceVar = (seriesList[0].viewPort.VPMaxTime - seriesList[0].viewPort.VPMinTime) / (dragSpeed * 10);
                    seriesList[0].viewPort.VPMinTime += tempPriceVar;
                    seriesList[0].viewPort.VPMaxTime -= tempPriceVar;
                    ReDrawEveryThing(false);
                    break;
                //ZoomOUT
                case Keys.OemOpenBrackets:
                    tempPriceVar = (seriesList[0].viewPort.VPMaxTime - seriesList[0].viewPort.VPMinTime) / (dragSpeed * 10);
                    seriesList[0].viewPort.VPMinTime -= tempPriceVar;
                    seriesList[0].viewPort.VPMaxTime += tempPriceVar;
                    ReDrawEveryThing(false);
                    break;

                default:

                    break;

            }
        }

        private void gridSplitter_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    e.IsInputKey = true;
                    break;
            }
        }

        private void colorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < plot.graphToolsToDrawList.Count; i++)
                {
                    if (plot.graphToolsToDrawList[i].IsSelected)
                    {
                        plot.graphToolsToDrawList[i].style.pen = new Pen(colorDialog1.Color, 1);
                        UserLine.pen = new Pen(colorDialog1.Color, 1); 
                    }
                }
                plot.Draw();
            }

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
                for (int i = 0; i < plot.graphToolsToDrawList.Count; i++)
                {
                    if (plot.graphToolsToDrawList[i].IsSelected)
                    {
                        plot.graphToolsToDrawList.RemoveAt(i);
                        //обнуляю флаг, чтобы при новой отрисовке круга или линии не взял старые коодинаты из буфера
                        UserLine.DoesParallelDisplacementNeeded = false;
                        UserLine.selectedObj = null;
                        UserLine.selectedLineIndex = null;
                        UserLine.IsInEditingExistingLine = false;
                    }
                    else
                    {
                        plot.graphToolsToDrawList[i].IsSelected = false;
                    }
                }
                plot.Draw();
            

        }

        private void freeLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawingTool = DrawingTool.FreeLine;
            UserLine.IsWaitingMouseClickToStoreLine = false;

        }

        private void arcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawingTool = DrawingTool.Arc;
            UserLine.IsWaitingMouseClickToStoreLine = false;

        }

        private void leftRayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].style.extensionType = Styles.ExtensionType.LeftRay;
            plot.Draw();
        }

        private void rightRayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].style.extensionType = Styles.ExtensionType.RightRay;
            plot.Draw();
        }

        private void bothRaysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].style.extensionType = Styles.ExtensionType.BothRays;
            plot.Draw();
        }

        private void noRaysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            plot.graphToolsToDrawList[(int)UserLine.selectedLineIndex].style.extensionType = Styles.ExtensionType.NoRays;
            plot.Draw();
        }

        private void cyclesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cycles();
        }



        internal void RemoveAllGraphicalTools()
        {
            if (plot.graphToolsToDrawList != null)
            {
                plot.graphToolsToDrawList.Clear();
                UserLine.selectedObj = null;
                UserLine.DoesParallelDisplacementNeeded = false;
                UserLine.selectedLineIndex = null;
                DrawingTool = DrawingTool.None;
                plot.Draw();
            }
        }

        private void hideToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < plot.modelsToDrawList.Count; i++)
            {
                if (!plot.modelsToDrawList[i].IsSelected)
                    plot.modelsToDrawList[i].IsVisible = false;
            }
            for (int i = 0; i < plot.HTtoDrawList.Count; i++)
            {
                if (plot.HTtoDrawList[i].SelectedCount == 0)
                    plot.HTtoDrawList[i].IsVisible = false;
            }
            plot.Draw();
        }

        private void showToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < plot.modelsToDrawList.Count; i++)
            {
                if (!plot.modelsToDrawList[i].IsSelected)
                    plot.modelsToDrawList[i].IsVisible = true;
            }
            for (int i = 0; i < plot.HTtoDrawList.Count; i++)
            {
                if (plot.HTtoDrawList[i].SelectedCount == 0)
                    plot.HTtoDrawList[i].IsVisible = true;
            }
            plot.Draw();
        }

        private void showHideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (plot != null && plot.grid != null)
            {
                plot.grid.IsVisible = !plot.grid.IsVisible;
                plot.Draw();
            }
        }

       
        private void showToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            gridSplitter.IsGridVisible = true;
            gridSplitter.Refresh();
        }

        private void hideToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            gridSplitter.IsGridVisible = false;
            gridSplitter.Refresh();
        }

        private void stylesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Skilful.Chart_Properties ms = new Skilful.Chart_Properties(leftAxis, rightAxis, topAxis, bottomAxis, this);
            
            //Skilful.AxisStyleWindow ms = new Skilful.AxisStyleWindow(leftAxis, rightAxis, topAxis, bottomAxis, this);
            
              
                if (ms.ShowDialog() == DialogResult.OK)
                {
                    plot.Draw();
                    
                }
            
        }

       
        private void ChartProperties_Click(object sender, EventArgs e)
        {
            Skilful.Chart_Properties ms = new Skilful.Chart_Properties(leftAxis, rightAxis, topAxis, bottomAxis, this);
            
            if (ms.ShowDialog() == DialogResult.OK)
            {
                ReDrawEveryThing(true);
            }
        }
 

       
    }
}
