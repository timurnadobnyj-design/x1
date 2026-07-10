//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Eugeniy Bazarov(obolon)
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

using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using Skilful.Data;
using Skilful.QuotesManager;
using Skilful.ModelManager;
using ChartV2;
using System.Collections.Generic;
using ChartV2.Styles;
using GridSplitter;
using System;
using System.Xml;


namespace Skilful
{
    [System.Serializable]
    public struct Grafic_elements
    {
        public DrawingTool dt;
        public PointF pf1, pf2, pf3, pf4;
        public ExtensionType Ext;
        public PointF IntPt1, IntPt2;
        public Color penstr;
    }
    [System.Serializable]
    public struct Model_Data_to_Save
    {
        public double Price_point_1, Price_point_2, Price_point_3, Price_point_4;
        public DateTime DateTime_point_1, DateTime_point_2, DateTime_point_3, DateTime_point_4;
        public int modelID;
    }
    [System.Serializable]
    public struct TA_Models
    {

        public TPoint BreakTargetLine;
        public TPoint BreakTrendLine;
        public TPoint BreakTrendLineFirst;
        public int CurrentPoint;
        public int DecDigs;
        public int ErrorCode;
        public bool HPBreakOut;
        public bool HPGetInside;
        public bool HPGetOutside;
        public bool HPreached;
        public int HTi;
        public int IDonBaseTF;
        public bool IsAlive;
        public int isBytrendOfID;
        public int isCorrectionOfID;
      //  public int LifeTime;
        public double MaxProcessBar;
        public  TDirection ModelDir;
        public int ModelID;
        public Common.TModelType ModelType;
        public double pip;
        //-------------------
        public TPoint Point1, Point11, Point2, Point21, Point22, Point23, Point3, Point31, Point32, Point4, Point41, Point5;
        public TPoint Point6, Point61, Point61Pct100, Point61Pct200, Point62, Point63, Point64, Point65, PointSP, PointSP1, PointSP2;
        //-----------------
      //  public double power;
        public int ProcessedBar;
        public uint Stat;
        public int Step;
        public TBorder Target1, Target2, Target3, Target4, Target5, Target6;
        public TLine TargetLine;
        public bool TargetLineBreakOut;
        public TLine TargetLineCorrected, TargetLineMA, TargetLineSteep, TargetLineTangent;
        public TF TimeFrame;
        public TLine TrendLine;
        public TLine TrendLineTangent;

    }
    [System.Serializable]
    public struct Model_Manager
    {
        public bool SeeksComplete;
        public List<TA_Models> Models,
                            HModels;
        public List<THTangentLine> HTangentLines;
    }


    [System.Serializable]
    public class Save
    {
         
        static public bool save_ = false;
        public Model_Manager[,] MM;
        public List<TF> Active_Chart_TF = new List<TF>();
        public List<string> Active_Chart_Name = new List<string>();
     //   public List<int[]> Chart_selectedModelIndexes = new List<int[]>();
        public List<Grafic_elements[]> custom_drowing_tools = new List<Grafic_elements[]>();
        public List<Model_Data_to_Save[]> models_save = new List<Model_Data_to_Save[]>();
        public bool workingForm_flagSerch;
        public List<bool> chart_Legeng_log = new List<bool>();
        public List<int> chart_Symbol_decimals = new List<int>();
        public bool Current_workingChart;
        public ChartType workingForm_ChartType;
        public Size workingForm_Size;
        public string workingForm_Text;
        public int workingForm_Left;
        public int workingForm_Top;
        public FormWindowState workingForm_WindowState;
        public TF workingForm_Active_Chart_TF, Current_Active_Chart_TF;
        public string workingForm_Node_Text, workingForm_Node_Parent_Text;
        public int SymbolTreeView_Width,
                    SymbolTreeView_Node;
        public bool SymbolTreeView_Node_expanded;
     
 
        public Save() { }
        public Save(Working w, StreamWriter stream, bool write)
        {

          
            Model_Data_to_Save mds = new Model_Data_to_Save();   
            List<Model_Data_to_Save> mdl = new List<Model_Data_to_Save>();
            Model_Data_to_Save[] md;

            Grafic_elements ge = new Grafic_elements();
            List<Grafic_elements> lse = new List<Grafic_elements>();
            Grafic_elements[] ge_;
                      
            bool flag_model_search = true;
            TQuotes quotes, w_quotes;
            w_quotes = w.Quotes;

            // ------------ сохранение имеющихся Model Manager -----------------------    
            try
            {
                if (w.flagSerch)
                {
                    MM = new Model_Manager[(int)ChartType.count, (int)TF.count];
                    for (int i = 0; i < (int)ChartType.count; i++)
                        for (int j = 0; j < (int)TF.count; j++)
                            if (w_quotes.Symbol.Frames[i, j] != null)
                              // if (w_quotes.Symbol.Frames[i, j].MM.Models.
                                MM[i, j] = Get_MM(w_quotes.Symbol.Frames[i, j].MM, stream, write);

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("MM: Error" + e.Message);
            }
           // else stream.WriteLine("Models don't search.");
            //--------------------------------------------------------------------------
           
            try
            {
            for (int k = 0; k < w.workingForm.Controls.Count; k++)
            {                
                
                if (w.workingForm.Controls[k].ToString() != "ChartV2.Control") continue;
                ChartV2.Control c = (ChartV2.Control)w.workingForm.Controls[k];               

                quotes = c.seriesList[0].info;                
               
                chart_Legeng_log.Add(c.seriesList[0].legend.log);
                chart_Symbol_decimals.Add(c.seriesList[0].info.Symbol.Decimals);                

                Active_Chart_TF.Add(c.seriesList[0].legend.frame);
                Active_Chart_Name.Add(c.seriesList[0].legend.symbol);
                if (c.plot.modelsToDrawList == null) flag_model_search = false;
                  else 
                    if (c.plot.modelsToDrawList.Count == 0) flag_model_search = false;
             //   workingForm_flagSerch.Add(w.flagSerch);

               // ---- сохранение пользовательских граф. инструментов -------

                if (c.plot.graphToolsToDrawList != null)
                   if (c.plot.graphToolsToDrawList.Count != 0)
                    for (int d = 0; d < c.plot.graphToolsToDrawList.Count; d++)
                        switch (c.plot.graphToolsToDrawList[d].type)
                        {
                            case DrawingTool.HorizontalLine:
                                ge.dt = DrawingTool.HorizontalLine;
                                ge.pf1 = c.plot.graphToolsToDrawList[d].pt1;
                                ge.pf2 = c.plot.graphToolsToDrawList[d].pt2;
                                ge.penstr = c.plot.graphToolsToDrawList[d].style.pen.Color;
                                lse.Add(ge);
                                break;
                            case DrawingTool.VerticalLine:
                                ge.dt = DrawingTool.VerticalLine;
                                ge.IntPt1 = c.plot.graphToolsToDrawList[d].IntPt1;
                                ge.IntPt2 = c.plot.graphToolsToDrawList[d].IntPt2;
                                ge.penstr = c.plot.graphToolsToDrawList[d].style.pen.Color;
                                lse.Add(ge);
                                break;
                            case DrawingTool.FreeLine:
                                ge.dt = DrawingTool.FreeLine;
                                ge.pf1 = c.plot.graphToolsToDrawList[d].pt1;
                                ge.pf2 = c.plot.graphToolsToDrawList[d].pt2;
                                ge.Ext = c.plot.graphToolsToDrawList[d].style.extensionType;
                                ge.penstr = c.plot.graphToolsToDrawList[d].style.pen.Color;
                                lse.Add(ge);
                                break;
                            case DrawingTool.Cycles:
                                ge.dt = DrawingTool.Cycles;
                                ge.pf1 = c.plot.graphToolsToDrawList[d].pt1;
                                ge.pf2 = c.plot.graphToolsToDrawList[d].pt2;
                                ge.penstr = c.plot.graphToolsToDrawList[d].style.pen.Color;
                                lse.Add(ge);
                                break;
                            case DrawingTool.Circle:
                                ge.dt = DrawingTool.Circle;
                                ge.pf1 = c.plot.graphToolsToDrawList[d].pt1;
                                ge.pf2 = c.plot.graphToolsToDrawList[d].pt2;
                                ge.pf3 = c.plot.graphToolsToDrawList[d].pt3;
                                ge.pf4 = c.plot.graphToolsToDrawList[d].pt4;
                                ge.penstr = c.plot.graphToolsToDrawList[d].style.pen.Color;
                                lse.Add(ge);
                                break;
                            case DrawingTool.Arc:
                                ge.dt = DrawingTool.Arc;
                                ge.pf1 = c.plot.graphToolsToDrawList[d].pt1;
                                ge.pf2 = c.plot.graphToolsToDrawList[d].pt2;
                                ge.pf3 = c.plot.graphToolsToDrawList[d].pt3;
                                ge.pf4 = c.plot.graphToolsToDrawList[d].pt4;
                                ge.penstr = c.plot.graphToolsToDrawList[d].style.pen.Color;
                                lse.Add(ge);
                                break;

                        }
                   else
                   {
                       ge.dt = DrawingTool.None;
                       lse.Add(ge);
                   }
                else
                {
                    ge.dt = DrawingTool.None;
                    lse.Add(ge);
                }

                ge_ = lse.ToArray();
                custom_drowing_tools.Add(ge_);
                lse.Clear();
         // ------------------------------------------------------

         // --- сохранение данных выделенных моделей -------------
     
                if (!flag_model_search)
                   {
                    mds.modelID = -1;
                    models_save.Add(new Model_Data_to_Save[1] { mds });
                    flag_model_search = true;
                    continue; 
                   }  
                              
                for (int b = 0; b < c.plot.modelsToDrawList.Count; b++)
                  
                    if (c.plot.modelsToDrawList[b].IsSelected)
                    {

                        for (int ii = 0; ii < quotes.MM.Models.Count; ii++)
                        {
                            if (quotes.MM.Models[ii].ModelID == b)
                            {
                                mds.modelID = quotes.MM.Models[ii].ModelID;
                                mds.Price_point_1 = quotes.MM.Models[ii].Point1.Price;
                                mds.Price_point_2 = quotes.MM.Models[ii].Point2.Price;
                                mds.Price_point_3 = quotes.MM.Models[ii].Point3.Price;
                                mds.Price_point_4 = quotes.MM.Models[ii].Point4.Price;

                                mds.DateTime_point_1 = quotes.MM.Models[ii].Point1.DT;
                                mds.DateTime_point_2 = quotes.MM.Models[ii].Point2.DT;
                                mds.DateTime_point_3 = quotes.MM.Models[ii].Point3.DT;
                                mds.DateTime_point_4 = quotes.MM.Models[ii].Point4.DT;

                                mdl.Add(mds);
                            }

                        }
                                       
                    }                           
                
                
                if (mdl.Count == 0)
                 { 
                    mds.modelID = -1;
                    models_save.Add(new Model_Data_to_Save[1] { mds });
                    continue; 
                  }

                md = mdl.ToArray();
                models_save.Add(md);
                mdl.Clear();    
                           
             }
            }
            catch (Exception e)
            {
                MessageBox.Show("Save class: Error" + e.Message);
            }

       }

        public Model_Manager Get_MM(ModelsManager MM, StreamWriter stream, bool write)
        {
            
            Model_Manager mm;
            List<TA_Models> tamodels = new List<TA_Models>();
            List<TA_Models>  hmodels_ = new List<TA_Models>();
            List<THTangentLine> htLines = new List<THTangentLine>();
            mm.SeeksComplete = true;
            for (int i = 0; i < MM.Models.Count; i++)
            {
                tamodels.Add(get_to_Save_TAmodel(MM.Models[i]));
                if (write) stream.WriteLine(MM.Models[i].SaveModel());
            }
            
            for (int i = 0; i < MM.HModels.Count; i++)
             hmodels_.Add(get_to_Save_TAmodel(MM.HModels[i]));

            mm.Models = tamodels;
            mm.HModels = hmodels_;
            mm.HTangentLines = MM.HTangentLines;     
            
            return mm;
        }

        public TA_Models get_to_Save_TAmodel(TModel tm)
        {
           
            TA_Models tmodels = new TA_Models();
            tmodels.BreakTargetLine = tm.BreakTargetLine;
            tmodels.BreakTrendLine = tm.BreakTrendLine;
            tmodels.BreakTrendLineFirst = tm.BreakTrendLineFirst;
            tmodels.CurrentPoint = tm.CurrentPoint;
            tmodels.DecDigs = tm.DecDigs;
            tmodels.ErrorCode = tm.ErrorCode;
            tmodels.HPBreakOut = tm.HPBreakOut;
            tmodels.HPGetInside = tm.HPGetInside;
            tmodels.HPGetOutside = tm.HPGetOutside;
            tmodels.HPreached = tm.HPreached;
            tmodels.HTi = tm.HTi;
            tmodels.IDonBaseTF = tm.IDonBaseTF;
            tmodels.IsAlive = tm.IsAlive;
            tmodels.isBytrendOfID = tm.isBytrendOfID;
            tmodels.isCorrectionOfID = tm.isCorrectionOfID;
          //  tmodels.LifeTime = tm.LifeTime;
            tmodels.MaxProcessBar = tm.MaxProcessBar;
            tmodels.ModelDir = tm.ModelDir;
            tmodels.ModelID = tm.ModelID;
            tmodels.ModelType = tm.ModelType;
            tmodels.pip = tm.pip;
            tmodels.Point1 = tm.Point1;
            tmodels.Point11 = tm.Point11;
            tmodels.Point2 = tm.Point2;
            tmodels.Point21 = tm.Point21;
            tmodels.Point22 = tm.Point22;
            tmodels.Point23 = tm.Point23;
            tmodels.Point3 = tm.Point3;
            tmodels.Point31 = tm.Point31;
            tmodels.Point32 = tm.Point32;
            tmodels.Point4 = tm.Point4;
            tmodels.Point41 = tm.Point41;
            tmodels.Point5 = tm.Point5;
            tmodels.Point6 = tm.Point6;
            tmodels.Point61 = tm.Point61;
            tmodels.Point61Pct100 = tm.Point61Pct100;
            tmodels.Point61Pct200 = tm.Point61Pct200;
            tmodels.Point62 = tm.Point62;
            tmodels.Point63 = tm.Point63;
            tmodels.Point64 = tm.Point64;
            tmodels.Point65 = tm.Point65;
            tmodels.PointSP = tm.PointSP;
            tmodels.PointSP1 = tm.PointSP1;
            tmodels.PointSP2 = tm.PointSP2;
          //  tmodels.power = tm.power;
            tmodels.ProcessedBar = tm.ProcessedBar;
            tmodels.Stat = tm.Stat;
            tmodels.Step = tm.Step;
            tmodels.Target1 = tm.Target1;
            tmodels.Target2 = tm.Target2;
            tmodels.Target3 = tm.Target3;
            tmodels.Target4 = tm.Target4;
            tmodels.Target5 = tm.Target5;
            tmodels.Target6 = tm.Target6;
            tmodels.TargetLine = tm.TargetLine;
            tmodels.TargetLineBreakOut = tm.TargetLineBreakOut;
            tmodels.TargetLineCorrected = tm.TargetLineCorrected;
            tmodels.TargetLineMA = tm.TargetLineMA;
            tmodels.TargetLineSteep = tm.TargetLineSteep;
            tmodels.TargetLineTangent = tm.TargetLineTangent;
            tmodels.TimeFrame = tm.TimeFrame;
            tmodels.TrendLine = tm.TrendLine;
            tmodels.TrendLineTangent = tm.TrendLineTangent;

            return tmodels;
           

        }
      public static TModel get_to_Load_TAmodel(TA_Models tmodels)
      {
          TModel tm = new TModel();
          tm.BreakTargetLine = tmodels.BreakTargetLine;
          tm.BreakTrendLine = tmodels.BreakTrendLine;
          tm.BreakTrendLineFirst = tmodels.BreakTrendLineFirst;
          tm.CurrentPoint = tmodels.CurrentPoint;
          tm.DecDigs = tmodels.DecDigs;
          tm.ErrorCode = tmodels.ErrorCode;
          tm.HPBreakOut = tmodels.HPBreakOut;
          tm.HPGetInside = tmodels.HPGetInside;
          tm.HPGetOutside = tmodels.HPGetOutside;
          tm.HPreached = tmodels.HPreached;
          tm.HTi = tmodels.HTi;
          tm.IDonBaseTF = tmodels.IDonBaseTF;
          tm.IsAlive = tmodels.IsAlive;
          tm.isBytrendOfID = tmodels.isBytrendOfID;
          tm.isCorrectionOfID = tmodels.isCorrectionOfID;
       //   tm.LifeTime = tmodels.LifeTime;
          tm.MaxProcessBar = tmodels.MaxProcessBar;
          tm.ModelDir = tmodels.ModelDir;
          tm.ModelID = tmodels.ModelID;
          tm.ModelType = tmodels.ModelType;
          tm.pip = tmodels.pip;
          tm.Point1 = tmodels.Point1;
          tm.Point11 = tmodels.Point11;
          tm.Point2 = tmodels.Point2;
          tm.Point21 = tmodels.Point21;
          tm.Point22 = tmodels.Point22;
          tm.Point23 = tmodels.Point23;
          tm.Point3 = tmodels.Point3;
          tm.Point31 = tmodels.Point31;
          tm.Point32 = tmodels.Point32;
          tm.Point4 = tmodels.Point4;
          tm.Point41 = tmodels.Point41;
          tm.Point5 = tmodels.Point5;
          tm.Point6 = tmodels.Point6;
          tm.Point61 = tmodels.Point61;
          tm.Point61Pct100 = tmodels.Point61Pct100;
          tm.Point61Pct200 = tmodels.Point61Pct200;
          tm.Point62 = tmodels.Point62;
          tm.Point63 = tmodels.Point63;
          tm.Point64 = tmodels.Point64;
          tm.Point65 = tmodels.Point65;
          tm.PointSP = tmodels.PointSP;
          tm.PointSP1 = tmodels.PointSP1;
          tm.PointSP2 = tmodels.PointSP2;
         // tm.power = tmodels.power;
          tm.ProcessedBar = tmodels.ProcessedBar;
          tm.Stat = tmodels.Stat;
          tm.Step = tmodels.Step;
          tm.Target1 = tmodels.Target1;
          tm.Target2 = tmodels.Target2;
          tm.Target3 = tmodels.Target3;
          tm.Target4 = tmodels.Target4;
          tm.Target5 = tmodels.Target5;
          tm.Target6 = tmodels.Target6;
          tm.TargetLine = tmodels.TargetLine;
          tm.TargetLineBreakOut = tmodels.TargetLineBreakOut;
          tm.TargetLineCorrected = tmodels.TargetLineCorrected;
          tm.TargetLineMA = tmodels.TargetLineMA;
          tm.TargetLineSteep = tmodels.TargetLineSteep;
          tm.TargetLineTangent = tmodels.TargetLineTangent;
          tm.TimeFrame = tmodels.TimeFrame;
          tm.TrendLine = tmodels.TrendLine;
          tm.TrendLineTangent = tmodels.TrendLineTangent;

          return tm;

      }

    }
}
