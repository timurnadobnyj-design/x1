//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Alex Kokomov(Loylick)
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
using Skilful.ModelManager;


namespace ChartV2
{
    public enum ModelType { MP, ATR, MDR };

    public class HTargetLine
    {
        public PointF[] PointsForDrawing_I_F = new PointF[2];
        public PointF[] PointsForDrawing_I_E = new PointF[2];
        public int SelectedCount = 0;  //количество выделенных моделей к которым прикреплена данная ГЦ
        public bool IsVisible = true;
        public HTargetLine(Skilful.ModelManager.THTangentLine TH)
        {

            Skilful.ModelManager.TLine TargetLine = Skilful.ModelManager.Common.CalcLine(TH.PointI, TH.PointF);
            PointsForDrawing_I_F[0].X = (float) (7*(TH.PointF.Bar - TH.PointI.Bar) + TH.PointF.Bar);
            PointsForDrawing_I_F[0].Y = (float) Skilful.ModelManager.Common.CalcLineValue(TargetLine, PointsForDrawing_I_F[0].X);
            PointsForDrawing_I_F[1] = TH.PointI.pt;
            PointsForDrawing_I_E[0] = TH.PointE.pt;
            PointsForDrawing_I_E[1] = TH.PointI.pt;


        }
    }

    public class Model
    {
        //индекс ГЦ соответствующей данной модели
        public int HTi = -1;

        public Skilful.QuotesManager.TF TimeFrame;
        //Точки для уже протянутых на нужную длинну линий. т.е. 
        //Данные точки не относятся к реальному положению вещей
        public PointF[] PointsForDrawing_1_3 = new PointF[2];
        public PointF[] PointsForDrawing_2_4 = new PointF[2];
        public PointF[] PointsForDrawing_21_4 = new PointF[2];
        public PointF[] PointsForDrawing_211_4 = new PointF[2];
        public PointF[] PointsForDrawing_3_5 = new PointF[2];
        public PointF[] PointsForDrawing_31_5 = new PointF[2];
        public PointF[] FarPoint6 = new PointF[3];  //!< Дальние HP, через т2 и т2''

        public static int? lastSelectedModelIndex = null;
        public static List<int> selectedModelIndexes = new List<int>();
        //четыре временных переменных для функции
        //выделения моделей (чтобы не запрашивать память на каждую модель)
        private int X1;
        private int X2;
        private int Y1;
        private int Y2;


        public PointF[] CT_4 = new PointF[3];
        public int LifeTime;
        public BorderF[] _100Percent = new BorderF[2];
        //public LineF[] _100Percent = new PointF[2];
        //public PointF CT1;
        //public PointF _1;
        //public PointF _2;
        
        public PointF[] _211 = new PointF[3];
        //public PointF _3;
        public PointF _31;
        //public PointF _4;
        //public PointF _5;
        public PointF _6;
        //public PointF _61;


        public PointF[] ModelPointReal = new PointF[8]; //!< Точки экстремумы
        public PointF[] ModelPoint = new PointF[6];     //!< Точки для построения линий
        public PointF[] AttractionPoint = new PointF[2];
        public BorderF[] Targ = new BorderF[6];
   //     public PointF[] DrawingAttractionPoint = new PointF[2];
        public Styles.ModelStyle style;
        public bool IsSelected = false;
        public bool IsVisible = true;


        public ModelType type;
 

        public Model(Skilful.ModelManager.TModel mod, int color)
        {
            if (((Skilful.ModelManager.Options.VisibleSteps & mod.Step) == 0) ||
                (mod.Step < 0))IsVisible = false;
            else IsVisible = true;

            Skilful.ModelManager.TPoint LastPoint = new Skilful.ModelManager.TPoint();
            LifeTime = Convert.ToInt32(mod.MaxProcessBar);
            TimeFrame = mod.TimeFrame;
            switch (mod.ModelType)
            {
                case Skilful.ModelManager.Common.TModelType.ModelOfExpansion:
                    {
                        color = GlobalMembersTAmodel.cl[0];
                        // Начало построения от СТ
                        PointsForDrawing_1_3[0] = mod.PointSP2.pt;
                        PointsForDrawing_21_4[0] = mod.PointSP2.pt;
//                        LastPoint.X = Math.Max(mod.BreakTrendLine.X, mod.Quotes.GetBarByIndex(mod.Quotes.GetCount() - 1).X);
                        ModelPoint[4] = mod.Point4.pt;
                        CT_4[0] = new PointF((float)(2 * ModelPoint[4].X - mod.PointSP2.Bar), mod.PointSP2.pt.Y);
                        CT_4[1] = new PointF((float)(3 * ModelPoint[4].X - 2 * mod.PointSP2.Bar), mod.PointSP2.pt.Y);
                        CT_4[2] = new PointF((float)(5 * ModelPoint[4].X - 4 * mod.PointSP2.Bar), mod.PointSP2.pt.Y);

                        if (mod.Point61.Bar > mod.Point41.Bar)
                        {
                            LastPoint.Price = Skilful.ModelManager.Common.CalcLineValue(mod.TargetLineTangent, CT_4[2].X);
                            LastPoint.Bar = CT_4[2].X;
                            if ((mod.Point64.Bar > mod.Point41.Bar)
                                && (mod.Point64.Bar <= CT_4[2].X)) FarPoint6[2] = mod.Point64.pt;
                            if ((mod.Point63.Bar > mod.Point41.Bar)
                                && (mod.Point63.Bar <= CT_4[2].X)
                                && ((mod.ErrorCode & 0x00020000) == 0)) FarPoint6[1] = mod.Point63.pt;
                            if ((mod.Point62.Bar > mod.Point41.Bar)
                                &&(mod.Point62.Bar <= CT_4[2].X) 
                                &&((mod.ErrorCode & 0x00010000) == 0)) FarPoint6[0] = mod.Point62.pt;
                            ModelPointReal[7] = mod.Point65.pt;
                        }
                        else
                        {
                            LastPoint.Price = Skilful.ModelManager.Common.CalcLineValue(mod.TargetLineTangent, CT_4[2].X);
                            LastPoint.Bar = CT_4[2].X;
                        }
                        PointsForDrawing_21_4[1] = LastPoint.pt;
                        if(mod.Point6.Bar > mod.Point41.Bar)
                            ModelPointReal[6] = mod.Point6.pt;
                        else ModelPointReal[6] = mod.Point4.pt;
                        ModelPoint[0] = mod.PointSP2.pt;
                        ModelPoint[1] = mod.Point11.pt;
                        ModelPoint[2] = mod.Point23.pt;
                        ModelPoint[3] = mod.Point32.pt;
                        ModelPoint[4] = mod.Point41.pt;

                        
                        ModelPointReal[0] = mod.PointSP2.pt;
                        ModelPointReal[1] = mod.Point1.pt;
                        ModelPointReal[2] = mod.Point2.pt;
                        _211[0] = mod.Point2.pt;
                        _211[1] = mod.Point22.pt;
                        _211[2] = mod.Point21.pt; //для НР' если 4!=4'
                        ModelPointReal[3] = mod.Point3.pt;
                        _31 = mod.Point31.pt;
                        ModelPointReal[4] = mod.Point4.pt;
                        PointsForDrawing_1_3[1].X = (float)Math.Max(mod.BreakTrendLine.Bar, CT_4[2].X);
                        PointsForDrawing_1_3[1].Y = (float) Skilful.ModelManager.Common.CalcLineValue(mod.TrendLineTangent, PointsForDrawing_1_3[1].X);
                        //PointsForDrawing_1_3[1] = LastPoint.pt;

                        if ((mod.Point61.Bar <= mod.Point41.Bar) || (mod.Point6.Bar == mod.Point4.Bar))
                        {
                            _100Percent[0] = new BorderF(ModelPointReal[6].X, LifeTime, (float)(2 * ModelPointReal[6].Y - ModelPointReal[1].Y));
                            _100Percent[1] = new BorderF(ModelPointReal[6].X, LifeTime, (float)(3 * ModelPointReal[6].Y - 2 * ModelPointReal[1].Y));
                        }
                        else
                        {
                            //if (mod.HPBreakOut)
                            
//                                _100Percent[0] = new PointF((float)mod.Point61.Bar, (float)(2 * mod.Point61.Price - ModelPointReal[1].Y));
//                                _100Percent[1] = new PointF((float)mod.Point61.Bar, (float)(3 * mod.Point61.Price - 2 * ModelPointReal[1].Y));

                            if ((mod.CurrentPoint == 99) && (!mod.HPreached))
                            {
                                _100Percent[0] = new BorderF((float)mod.Point6.Bar, LifeTime, (float)(2 * mod.Point6.Price - ModelPointReal[1].Y));
                                _100Percent[1] = new BorderF((float)mod.Point6.Bar, LifeTime, (float)(3 * mod.Point6.Price - 2 * ModelPointReal[1].Y));
                            }
                            else
                            {
                                _100Percent[0] = new BorderF((float)mod.Point61Pct100.Bar, LifeTime, (float)mod.Point61Pct100.Price);
                                _100Percent[1] = new BorderF((float)mod.Point61Pct200.Bar, LifeTime, (float)mod.Point61Pct200.Price);
                            }
                                
                            
                        }
                        Targ[0] = mod.Target1.pt;
                        Targ[1] = mod.Target2.pt;
                        Targ[2] = mod.Target3.pt;
/*                      if (!mod.BreakTargetLineFirst.pt.IsEmpty)
                        {
                            Targ[3] = mod.Target4.pt;
                            Targ[4] = mod.Target5.pt;
                            Targ[5] = mod.Target6.pt;
                        }*/
                       type = ModelType.MP;
                       break;
                      
                    }
                case Skilful.ModelManager.Common.TModelType.ModelOfAttraction:
                    {
                        color = GlobalMembersTAmodel.cl[1];
                        PointsForDrawing_1_3[0] = mod.Point11.pt;
                        PointsForDrawing_21_4[0] = mod.Point23.pt;
                        // Конец построения по точке пересечения ЛТ и ЛЦ
                        LastPoint = mod.PointSP2;
                        PointsForDrawing_1_3[1] = LastPoint.pt;
                        PointsForDrawing_21_4[1] = LastPoint.pt;

                       // ModelPoint[0] = LastPoint.pt;
                        ModelPoint[1] = mod.Point11.pt;
                        ModelPoint[2] = mod.Point23.pt;
                        ModelPoint[3] = mod.Point32.pt;
                        ModelPoint[4] = mod.Point41.pt;
                        //ModelPoint[5] = LastPoint.pt;
     
                        //CT = PointF.Empty;
                        ModelPointReal[0] = PointF.Empty;
                        ModelPointReal[1] = mod.Point1.pt;
                        ModelPointReal[2] = mod.Point21.pt;
                        _100Percent[0] = new BorderF(LastPoint.pt.X, LifeTime, (float)(2 * LastPoint.pt.Y - ModelPointReal[1].Y));
                        _100Percent[1] = new BorderF(LastPoint.pt.X, LifeTime, (float)(3 * LastPoint.pt.Y - 2 * ModelPointReal[1].Y)); 

                        
                        _211[0] = PointF.Empty;
                        _211[1] = PointF.Empty;
                        _211[2] = PointF.Empty;
                        ModelPointReal[3] = mod.Point3.pt;
                        _31 = PointF.Empty;
                        ModelPointReal[4] = mod.Point41.pt;
                        //ModelPointReal[6] = LastPoint.pt;
                        ModelPointReal[6] = PointF.Empty;
                        Targ[0] = mod.Target1.pt;
                        //Targ[1] = mod.Target2.pt;
                        //Targ[2] = mod.Target3.pt;
                        //Targ[3] = mod.Target4.pt;

                        type = ModelType.ATR;
                        break;
                    }
                case Skilful.ModelManager.Common.TModelType.ModelOfBalance:
                    {
                        color = GlobalMembersTAmodel.cl[2];
                        PointsForDrawing_1_3[0] = mod.Point11.pt;
                        PointsForDrawing_21_4[0] = mod.Point23.pt;
//                        LastPoint.X = Math.Max(mod.BreakTrendLine.X, mod.Quotes.GetBarByIndex(mod.Quotes.GetCount() - 1).X);
                        LastPoint.Bar = Math.Max(mod.BreakTrendLine.Bar, 2*mod.Point41.Bar-mod.Point11.Bar);
                        LastPoint.Price = Skilful.ModelManager.Common.CalcLineValue(mod.TrendLineTangent, LastPoint.Bar);
                        PointsForDrawing_1_3[1] = LastPoint.pt;
                        if (mod.Point61.Bar > mod.Point4.Bar)
                        {
                            LastPoint.Price = Skilful.ModelManager.Common.CalcLineValue(mod.TargetLineTangent, Math.Max(mod.Point61.Bar,LastPoint.Bar));
                            LastPoint.Bar = Math.Max(mod.Point61.Bar, LastPoint.Bar);
                        }
                        else
                        {
                            LastPoint.Price = Skilful.ModelManager.Common.CalcLineValue(mod.TargetLineTangent, PointsForDrawing_1_3[1].X);
                            LastPoint.Bar = PointsForDrawing_1_3[1].X;
                        }
                        PointsForDrawing_21_4[1] = LastPoint.pt;

                        if (mod.Point6.Bar > mod.Point41.Bar)
                            ModelPointReal[6] = mod.Point6.pt;
                        else ModelPointReal[6] = mod.Point4.pt;

                        ModelPoint[1] = mod.Point11.pt;
                        ModelPoint[2] = mod.Point23.pt;
                        ModelPoint[3] = mod.Point32.pt;
                        ModelPoint[4] = mod.Point41.pt;
                        //ModelPoint[5] = LastPoint.pt;
                        CT_4[2] = new PointF((float)(5 * ModelPoint[4].X - 4 * ModelPoint[1].X), ModelPoint[4].Y);
                        //CT = PointF.Empty;
                        ModelPointReal[0] = PointF.Empty;
                        ModelPointReal[1] = mod.Point1.pt;
                        ModelPointReal[2] = mod.Point2.pt;
          
                        if ((mod.Point61.Bar <= mod.Point41.Bar) || (mod.Point6.Bar == mod.Point4.Bar))
                        {
                            _100Percent[0] = new BorderF(ModelPointReal[6].X, LifeTime, (float)(2 * ModelPointReal[6].Y - ModelPointReal[1].Y));
                            _100Percent[1] = new BorderF(ModelPointReal[6].X, LifeTime, (float)(3 * ModelPointReal[6].Y - 2 * ModelPointReal[1].Y));
                        }
                        else
                        {
                            //if (mod.HPBreakOut)
                            {
                                _100Percent[0] = new BorderF((float)mod.Point61.Bar, LifeTime, (float)(2 * mod.Point61.Price - ModelPointReal[1].Y));
                                _100Percent[1] = new BorderF((float)mod.Point61.Bar, LifeTime, (float)(3 * mod.Point61.Price - 2 * ModelPointReal[1].Y));
                            }
                        }
                        _211[0] = PointF.Empty;
                        _211[1] = PointF.Empty;
                        _211[2] = PointF.Empty;
                        ModelPointReal[3] = mod.Point3.pt;
                        _31 = mod.Point31.pt;
                        ModelPointReal[4] = mod.Point4.pt;
                        Targ[0] = mod.Target1.pt;
                        Targ[1] = mod.Target2.pt;
                        Targ[2] = mod.Target3.pt;
                        Targ[3] = mod.Target4.pt;

                        type = ModelType.MDR;
                        break;
                    }
            }
            if (mod.Point6.Bar > mod.Point41.Bar)
            {   // Модель с 6-й точкой
                
                AttractionPoint[0] = mod.Point31.pt;
                AttractionPoint[1] = mod.Point5.pt;
                
                if (mod.ModelType == Skilful.ModelManager.Common.TModelType.ModelOfAttraction)
                {
                    PointsForDrawing_31_5[1] = mod.Point61.pt;
                    ModelPointReal[6] = PointF.Empty;
                    ModelPoint[5] = PointF.Empty;
                }
                else
                {
                    ModelPointReal[6] = mod.Point6.pt;
                    ModelPoint[5] = mod.Point6.pt;
                }
                ModelPointReal[5] = mod.Point5.pt;

                _6 = mod.Point6.pt;
                if (mod.Point61.Bar > mod.Point41.Bar+1)
                {
                    // Модель с МПМР
                    // ModelPointReal[6] = mod.Point62.pt;
                    //if (mod.ModelType == Skilful.ModelManager.Common.TModelType.ModelOfAttraction)
                    ModelPoint[5] = mod.Point61.pt;
                    PointsForDrawing_31_5[0] = mod.Point31.pt;
                    
                    if (mod.Point61.Bar < CT_4[2].X && mod.Point61.Bar > PointsForDrawing_31_5[1].X)
                        PointsForDrawing_31_5[1] = mod.Point61.pt;

                    if ((mod.Point64.Bar > mod.Point4.Bar + 1)
                        && (mod.Point64.Bar < CT_4[2].X)
                        && mod.Point64.Bar > PointsForDrawing_31_5[1].X)  PointsForDrawing_31_5[1] = mod.Point64.pt;

                    if ((mod.Point63.Bar > mod.Point4.Bar + 1)
                        && (mod.Point63.Bar < CT_4[2].X)
                        && mod.Point63.Bar > PointsForDrawing_31_5[1].X
                        && ((mod.ErrorCode & 0x00020000) == 0))           PointsForDrawing_31_5[1] = mod.Point63.pt;

                    if ((mod.Point62.Bar > mod.Point4.Bar + 1)
                        && (mod.Point62.Bar < CT_4[2].X)
                        && mod.Point62.Bar > PointsForDrawing_31_5[1].X
                        && ((mod.ErrorCode & 0x00010000) == 0))           PointsForDrawing_31_5[1] = mod.Point62.pt;
                }
            }
            else
            {
                
                AttractionPoint[0] = PointF.Empty;
                AttractionPoint[1] = PointF.Empty;
                ModelPointReal[5] = PointF.Empty;
                _6 = PointF.Empty;
                if (mod.ModelType != Skilful.ModelManager.Common.TModelType.ModelOfAttraction)
                    ModelPointReal[6] = ModelPointReal[4];
                PointsForDrawing_31_5[0] = PointF.Empty;
                PointsForDrawing_31_5[1] = PointF.Empty;
            }
           

            //this.style = new ChartV2.Styles.ModelStyle(Color.FromArgb(255,Color.FromArgb(color)),Color.FromArgb(100,Color.FromArgb(color)));
            this.style = new ChartV2.Styles.ModelStyle(mod.Step);
            this.HTi = mod.HTi;

        }

        public bool IsModelClicked(int X, int Y, Data.ViewPort viewPort)
        {
            X1 = (int) viewPort.BarNumberToPixels(PointsForDrawing_1_3[0].X);
            X2 = (int) viewPort.BarNumberToPixels(PointsForDrawing_1_3[1].X);
            Y1 = (int) viewPort.PriceToPixels(PointsForDrawing_1_3[0].Y);
            Y2 = (int) viewPort.PriceToPixels(PointsForDrawing_1_3[1].Y);
            //проверка на касание трендовой
            if ((X > X1) && (X < X2))
            {
                if (Math.Abs((Y - Y1) * (X2 - X1) - (X - X1) * (Y2 - Y1)) < ((Math.Abs(Y2 - Y1) + (X2 - X1)) * 2))
                    return true;
            }

            X1 = (int) viewPort.BarNumberToPixels(PointsForDrawing_21_4[0].X);
            X2 = (int) viewPort.BarNumberToPixels(PointsForDrawing_21_4[1].X);
            Y1 = (int) viewPort.PriceToPixels(PointsForDrawing_21_4[0].Y);
            Y2 = (int) viewPort.PriceToPixels(PointsForDrawing_21_4[1].Y);

            //проверка на касание целевой
            if ((X > X1) && (X < X2))
            {
                if (Math.Abs((Y - Y1) * (X2 - X1) - (X - X1) * (Y2 - Y1)) < ((Math.Abs(Y2 - Y1) + (X2 - X1)) * 2))
                    return true;
            }

            return false;
        }
    }
}

