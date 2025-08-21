//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Andrey Prokhorov(AVP)
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
using System.Linq;
using System.Text;
using System.Globalization;
using Skilful.QuotesManager;

namespace Skilful.ModelManager
{
    //*
    //Points codes
    //1  - т1
    //11 - т1 для касательной ЛТ
    //2  - т2
    //21 - т2'
    //22 - т2''
    //23 - т2 для касательной ЛЦ
    //3  - т3
    //31 - т3'
    //32 - т3 для касательной ЛТ
    //4  - т4
    //41 - т4 по касательной
    //5  - т5 глобальный экстремум
    //51 - т5'
    //52 - т5''
    //6  - т6 реальная
    //61 - т6 расч. по ЛЦ'
    //62 - т6 расч. по ЛЦ
    //63 - т6 самая дальняя
    //64 - т6 по касательным ЛТ и ЛЦ
    //10 - точка пробития ЛТ
    //*
    //*
    //IsModelCorrect
    //0  - модель корректна
    //1 - нарушено взаимное положение точек по цене
    //2 - лишние экстремумы на ЛТ
    //4 - лишние экстремумы на ЛЦ
    //8 - пересечение тел баров точек 1 и 3 (только ЧМП)
    //16 - пересечение тел баров точек 2 и 4 (только ЧМП)
    //32 - пересечение тел баров точек 2 и 5
    //64 - точка 3 не глобальный экстремум
    //128 - точка 5 за пределами бара точки 4
    //256 - точка 3' вне базы
    //512 - ЛЦ пересекает бар точки 1
    //1024 - наложение уровней модели
    //2048 - точка 3 не глобальный экстремум междй тт2 и 3
    //4096 - Точка 3 не глобальный экстремум для ЧМП
    //16384 - точка 5 внутри базы
    //*
    //
    //  TargetLine's codes
    //0 - через т4 и т2-экстремум
    //1 - через т4 и т2-по касательной
    //2 - через т5 и т3' - для МПМР
    //3 - через т4 и т2, дающую самую дальнюю т6
    //4 - через т4 по касательной и т2 по касательной
    //
    public class TModel
    {
        private TPoint[] PointsSP = new TPoint[3];
        private TPoint[] Points1 = new TPoint[2];
        private TPoint[] Points2 = new TPoint[4];
        private TPoint[] Points3 = new TPoint[3];
        private TPoint[] Points4 = new TPoint[3];
        private TPoint[] Points5 = new TPoint[3];
        private TPoint[] Points6 = new TPoint[9];
        private TLine[] TargetLines = new TLine[5];
        private TLine[] TrendLines = new TLine[2];
        //private TPoint[] Targets = new TPoint[6];
        private TBorder[] Targets = new TBorder[6];

        // Внутрение переменные
        private int FalsePoint3 = 0;
        public Double MaxProcessBar;  //!< Последний бар которым может обработать модель (СТ-4)*2 от 4 == время жизни модели
        public int LifeTime { get { return (int)MaxProcessBar; } }
        private TBar SV = new TBar();
        private TBar SV1 = new TBar();
        private TBar SV2 = new TBar();
        private TBar SV3 = new TBar();
        private TBar SV4 = new TBar();
        private TBar SV5 = new TBar();
        private Double SymPoint3High;
        private Double SymPoint3Low;

        public TQuotes Quotes = new TQuotes();
        public int ProcessedBar;    //!< № обработанного бара
        public int DecDigs;         //!< Кол-во знаков после запятой 
        public double pip;          //!< Точность
        public int CurrentPoint;    //!< № обрабатываемой точки
        public int ModelID;         //!< № модели
        public TDirection ModelDir; //!< Направление модели
        public Common.TModelType ModelType;    //!< Тип модели
        public int Step;            //!< Номер прохода на котором найдена модель
        public QuotesManager.TF TimeFrame;  //<! ТФ на котором найдена модель
        ///Anrey@Skat:
        public int IDonBaseTF;            //!< ID этой же модели на самом старшем фрейме, где модель была найдена
        public uint Stat;                 //!< флаги глобальности модели от т1 и для тренда на данном плане
        public bool TargetLineBreakOut;   //!< Признак пробития ЛЦ'
        public bool HPBreakOut;           //!< Признак пробития HP
        public bool HPGetInside,          //!< Признак пробития HP без пробития целевой и трендовой
                    HPGetOutside;         //!< Признак пробития HP с одновременным пробитием целевой или трендовой
        public bool HPreached=false;      //!< Признак достижения расчетной НР.
        public int isCorrectionOfID,      //индексы модели по отношению к которой данная модель является вложенной или по тренду
                   isBytrendOfID;  
        public int HTi = -1;              //индекс ГЦ соответствующей данной модели

        public TLine TrendLine      //!< Трендовая по экстремумам
        {
            get { return TrendLines[0]; }
            set { TrendLines[0] = value; }
        }

        public TLine TrendLineTangent   //!< Трендовая по касательным
        {
            get { return TrendLines[1]; }
            set { TrendLines[1] = value; }
        }

        public TPoint PointSP //! Точка пересечения ЛЦ и ЛТ
        {
            get { return PointsSP[0]; }
            set { PointsSP[0] = value; }
        }
        public TPoint PointSP1 //! Точка пересечени ЛЦ' и ЛТ
        {
            get { return PointsSP[1]; }
            set { PointsSP[1] = value; }
        }
        public TPoint PointSP2 //! Точка пересечени ЛЦ и ЛТ по касательным
        {
            get { return PointsSP[2]; }
            set { PointsSP[2] = value; }
        }

        public TPoint Point1  //!< Точка 1
        {
            get { return Points1[0]; }
            set { Points1[0] = value; }
        }

        public TPoint Point11  //!< Точка 1 для касательной ЛТ
        {
            get { return Points1[1]; }
            set { Points1[1] = value; }
        }

        public TPoint Point2    //! Точки 2 глобальный экстремум
        {
            get { return Points2[0]; }
            set { Points2[0] = value; }
        }
        public TPoint Point21   //! Точка 2 по касательной
        {
            get { return Points2[1]; }
            set { Points2[1] = value; }
        }
        public TPoint Point22   //! Точка 2 дающая самую дальнюю цель
        {
            get { return Points2[2]; }
            set { Points2[2] = value; }
        }
        public TPoint Point23   //! Точка 2 для ЛЦ по касательной
        {
            get { return Points2[3]; }
            set { Points2[3] = value; }
        }

        public TPoint Point3  //! точка 3
        {
            get { return Points3[0]; }
            set { Points3[0] = value; }

        }
        public TPoint Point31   //! Точка 3' для МПМР
        {
            get { return Points3[1]; }
            set { Points3[1] = value; }
        }

        public TPoint Point32   //! Точка 3 для ЛТ по касательной
        {
            get { return Points3[2]; }
            set { Points3[2] = value; }
        }

        public TPoint Point4    //! Точка 4
        {
            get { return Points4[0]; }
            set { Points4[0] = value; }
        }
        public TPoint Point41
        {
            get { return Points4[1]; }
            set { Points4[1] = value; }
        }
        public TPoint Point5    //! Точка 5
        {
            get { return Points5[0]; }
            set { Points5[0] = value; }
        }
        public TPoint Point6    //! Точка 6
        {
            get { return Points6[0]; }
            set { Points6[0] = value; }
        }
        public TPoint Point61    //! Точка 6расч по МПМР
        {
            get { return Points6[1]; }
            set { Points6[1] = value; }
        }
        public TPoint Point62    //! Точка 6расч по МП(через экстремумы)МР
        {
            get { return Points6[2]; }
            set { Points6[2] = value; }
        }

        public TPoint Point63    //! Точка 6расч самая дальняя
        {
            get { return Points6[3]; }
            set { Points6[3] = value; }
        }

        public TPoint Point64    //! Точка 6расч по МПкасательнойМР
        {
            get { return Points6[4]; }
            set { Points6[4] = value; }
        }
        public TPoint Point61Pct100 //! 100% от т6расч по МПМР 
        {
            get { return Points6[5]; }
            set { Points6[5] = value; }
        }

        public TPoint Point61Pct200 //! 200% от т6расч по МПМР 
        {
            get { return Points6[6]; }
            set { Points6[6] = value; }
        }

        public TPoint Point65 // реальная 6-я по откату от НР к 4-й
        {
            get { return Points6[7]; }
            set { Points6[7] = value; }
        }

        public TLine TargetLine //! Целевая линия через т2-глобальный экстремум
        {
            get { return TargetLines[0]; }
            set { TargetLines[0] = value; }
        }
        public TLine TargetLineCorrected    //! Целевая линия по касательной т2
        {
            get { return TargetLines[1]; }
            set { TargetLines[1] = value; }
        }
        public TLine TargetLineMA   //!   Целевая линия для МПМР(МДР)
        {
            get { return TargetLines[2]; }
            set { TargetLines[2] = value; }
        }
        public TLine TargetLineSteep    //! Целевая линия дающая самую дальнюю цель
        {
            get { return TargetLines[3]; }
            set { TargetLines[3] = value; }
        }
        public TLine TargetLineTangent    //! Целевая линия по касательной, охватывающей все движение
        {
            get { return TargetLines[4]; }
            set { TargetLines[4] = value; }
        }

        public TBorder Target1 //! Первая цель
        {
            get { return Targets[0]; }
            set { Targets[0] = value; }
        }
        public TBorder Target2   //! Вторая цель
        {
            get { return Targets[1]; }
            set { Targets[1] = value; }
        }
        public TBorder Target3   //! Третья цель
        {
            get { return Targets[2]; }
            set { Targets[2] = value; }
        }
        public TBorder Target4   //! Четвертая цель
        {
            get { return Targets[3]; }
            set { Targets[3] = value; }
        }
        public TBorder Target5   //! Пятая цель
        {
            get { return Targets[4]; }
            set { Targets[4] = value; }
        }
        public TBorder Target6   //! Шестая цель
        {
            get { return Targets[5]; }
            set { Targets[5] = value; }
        }


        public TPoint BreakTrendLine = new TPoint();
        private bool SearchLastBreakTrendLine = true;    //! Признак поиска последнего касания ЛТ после пробития
        public TPoint BreakTrendLineFirst = new TPoint();
        public TPoint BreakTargetLine;
        public bool IsAlive;
        public int ErrorCode;
//        public int NewModelFlag=0; 

        public double power
        {
            get // значения >1 модель сильная, <1 слабая, 2 ст=1, >2 ст расположена правее 1
            {
                return Math.Round((Point3.Bar - Point1.Bar) * 2 / (Point3.Bar - PointSP2.Bar), 2);
            }
        }

        public void InitModel(TQuotes AQuotes, int AModelID, int AModelParentID, int AModelParentPoint, int ADecDigs)
        {
            ModelID = AModelID;
            Quotes = AQuotes;
            ModelType = Common.TModelType.UnknownModel;
            Step = 1;
            SymPoint3High = -9999999;
            SymPoint3Low = 999999;
            ModelDir = TDirection.dtUnKnown;
            CurrentPoint = 0;
            ProcessedBar = 0;
            IsAlive = true;
//            DecDigs = ADecDigs;
            DecDigs = ADecDigs;
            pip = Math.Pow(10, -DecDigs);
            TimeFrame = Quotes.tf;
            ErrorCode = 0;
            TargetLineBreakOut = false;
            HPBreakOut = false;
            LastMin.Price = 999999;
            LastMax.Price = -999999;
            MaxProcessBar = Quotes.GetCount() - 1;
            for (int i = 0; i <= 2; i++)
            {
                Points2[i] = new TPoint();
                Points3[i] = new TPoint();
                Points4[i] = new TPoint();
                Points5[i] = new TPoint();
            }
            for (int i = 0; i <= 8; i++)
            {
                Points6[i] = new TPoint();
            }
            for (int i = 0; i <= 3; i++)
            {
                TargetLines[i] = new TLine();
            }
            for (int i = 0; i <= 5; i++)
            {
                Targets[i] = new TBorder();
            }
        }

        public TModel() { }

        public TModel(TQuotes AQuotes, int AModelID, int AModelParentID, int AModelParentPoint, int ADecDigs)
        {
            InitModel(AQuotes, AModelID, AModelParentID, AModelParentPoint, ADecDigs);
        }

        public TModel(TQuotes AQuotes, int AModelID, TModel MM, int ADecDigs, int PointNum)  //! Создание модели на основе существующей
        {
            ModelID = AModelID;
            Quotes = AQuotes;
            ModelType = Common.TModelType.UnknownModel;
            DecDigs = ADecDigs;
            pip = Math.Pow(10, -DecDigs);
            TimeFrame = Quotes.tf;
            Step = 1;
            SymPoint3High = -9999999;
            SymPoint3Low = 999999;
            ModelDir = MM.ModelDir;
            TargetLineBreakOut = false;
            HPBreakOut = false;
            MaxProcessBar = Quotes.GetCount() - 1;
//            First4Point = true;
//            Final4Point = false;
            for (int i = 0; i <= 2; i++)
            {
                Points2[i] = new TPoint();
                Points3[i] = new TPoint();
                Points4[i] = new TPoint();
                Points5[i] = new TPoint();
            }
            for (int i = 0; i <= 8; i++)
            {
                Points6[i] = new TPoint();
            }
            for (int i = 0; i <= 3; i++)
            {
                TargetLines[i] = new TLine();
            }
            for (int i = 0; i <= 5; i++)
            {
                Targets[i] = new TBorder();
            }
            switch (PointNum)
            {
                case 1:
                    CurrentPoint = 1;
                    ProcessedBar = MM.ProcessedBar - 1;
                    IsAlive = true;
                    ErrorCode = 0;
                    SetPointParams(MM.Point1.Bar, MM.Point1.DT, MM.Point1.Price, 1);
                    SetPointParams(MM.Point4.Bar, MM.Point4.DT, MM.Point4.Price, 22);
                    if (ModelDir == TDirection.dtUp)
                        LastMax = MM.Point4;
                    else
                        LastMin = MM.Point4;
                    break;
                case 2:
                    CurrentPoint = 1;
                    ProcessedBar = MM.ProcessedBar - 1;
                    IsAlive = true;
                    ErrorCode = 0;
                    SetPointParams(MM.Point1.Bar, MM.Point1.DT, MM.Point1.Price, 1);
                    SetPointParams(MM.Point6.Bar, MM.Point6.DT, MM.Point6.Price, 22);
                    if (ModelDir == TDirection.dtUp)
                        LastMax = MM.Point6;
                    else
                        LastMin = MM.Point6;
                    break;
                case 3:
                    CurrentPoint = 1;
                    ProcessedBar = MM.ProcessedBar - 1;
                    IsAlive = true;
                    ErrorCode = 0;
                    SetPointParams(MM.Point1.Bar, MM.Point1.DT, MM.Point1.Price, 1);
                    SetPointParams(MM.Point6.Bar, MM.Point6.DT, MM.Point6.Price, 2);
//                    SetPointParams(MM.Point3.Bar, MM.Point3.DT, MM.Point3.Price, 3);
                    if (ModelDir == TDirection.dtUp)
                        LastMax = MM.Point6;
                    else
                        LastMin = MM.Point6;
                    break;
                case 4:
                    CurrentPoint = 3;
                    ProcessedBar = MM.ProcessedBar - 1;
                    IsAlive = true;
                    ErrorCode = 0;
                    SetPointParams(MM.Point1.Bar, MM.Point1.DT, MM.Point1.Price, 1);
                    SetPointParams(MM.Point2.Bar, MM.Point2.DT, MM.Point2.Price, 2);
                    SetPointParams(MM.Point3.Bar, MM.Point3.DT, MM.Point3.Price, 3);
//                    SetPointParams(MM.Point4.Bar, MM.Point4.DT, MM.Point4.Price, 4);
                    if (ModelDir == TDirection.dtUp)
                        LastMax = MM.Point4;
                    else
                        LastMin = MM.Point4;
                    break;
                default: ;
                    break;

            }
        }

        public TModel(TBar Bar, TDirection AModel, TQuotes AQuotes, int AModelID, int AModelParentID, int AModelParentPoint, int ADecDigs)  //! Создание модели заданного направления
        {
            InitModel(AQuotes,AModelID,AModelParentID,AModelParentPoint,ADecDigs);
            CurrentPoint = 1;
            ProcessedBar = Convert.ToInt32(Bar.Bar);
            SymPoint3High = Quotes.GetBarByIndex(ProcessedBar).High;
            SymPoint3Low = Quotes.GetBarByIndex(ProcessedBar).Low;
            switch (AModel)
            {
                case TDirection.dtUp:
                {
                    ModelDir = TDirection.dtUp;
                    SetPointParams(ProcessedBar, Bar.DT, Bar.Low, 1);
                    SetPointParams(ProcessedBar, new DateTime(), -999999, 2);
                    SetPointParams(0, new DateTime(), 999999, 5);
                }
                break;
                case TDirection.dtDn:
                {
                    ModelDir = TDirection.dtDn;
                    SetPointParams(ProcessedBar, Bar.DT, Bar.High, 1);
                    SetPointParams(ProcessedBar, new DateTime(), 999999, 2);
                    SetPointParams(0, new DateTime(), -999999, 5);
                }
                break;
            }
        }

        private void ClearPoint(int PointNumber)    //! Очистка точки по ее номеру
        {
            switch (PointNumber)
            {
                case 1:
                    {
                        Points1[0].Bar = 0;
                        Points1[0].DT = new DateTime();
                        Points1[0].Price = 0;
                    }
                    break;
                case 2:
                    {
                        Points2[0].Bar = 0;
                        Points2[0].DT = new DateTime();
                        Points2[0].Price = 0;
                    }
                    break;
                case 21:
                    {
                        Points2[1].Bar = 0;
                        Points2[1].DT = new DateTime();
                        Points2[1].Price = 0;
                    }
                    break;
                case 22:
                    {
                        Points2[2].Bar = 0;
                        Points2[2].DT = new DateTime();
                        Points2[2].Price = 0;
                    }
                    break;
                case 3:
                    {
                        Points3[0].Bar = 0;
                        Points3[0].DT = new DateTime();
                        Points3[0].Price = 0;
                    }
                    break;
                case 31:
                    {
                        Points3[1].Bar = 0;
                        Points3[1].DT = new DateTime();
                        Points3[1].Price = 0;
                    }
                    break;
                case 4:
                    {
                        Points4[0].Bar = 0;
                        Points4[0].DT = new DateTime();
                        Points4[0].Price = 0;
                    }
                    break;
                case 41:
                    {
                        Points4[1].Bar = 0;
                        Points4[1].DT = new DateTime();
                        Points4[1].Price = 0;
                    }
                    break;
                case 42:
                    {
                        Points4[2].Bar = 0;
                        Points4[2].DT = new DateTime();
                        Points4[2].Price = 0;
                    }
                    break;
                case 5:
                    {
                        Points5[0].Bar = 0;
                        Points5[0].DT = new DateTime();
                        Points5[0].Price = 0;
                    }
                    break;
                case 6:
                    {
                        Points6[0].Bar = 0;
                        Points6[0].DT = new DateTime();
                        Points6[0].Price = 0;
                    }
                    break;
                case 61:
                    {
                        Points6[1].Bar = 0;
                        Points6[1].DT = new DateTime();
                        Points6[1].Price = 0;
                    }
                    break;
                case 62:
                    {
                        Points6[2].Bar = 0;
                        Points6[2].DT = new DateTime();
                        Points6[2].Price = 0;
                    }
                    break;
                case 63:
                    {
                        Points6[3].Bar = 0;
                        Points6[3].DT = new DateTime();
                        Points6[3].Price = 0;
                    }
                    break;
                case 64:
                    {
                        Points6[4].Bar = 0;
                        Points6[4].DT = new DateTime();
                        Points6[4].Price = 0;
                    }
                    break;
                case 10:
                    {
                        BreakTrendLine.Bar = 0;
                        BreakTrendLine.DT = new DateTime();
                        BreakTrendLine.Price = 0;
                    }
                    break;
            }
        }
        public void ClearModel()    //! Очистка всех точек модели
        {
            int I;

            CurrentPoint = 0;
//            ProcessedBar = 0;
            IsAlive = true;
            ErrorCode = 0;
            ModelType = Common.TModelType.UnknownModel;
            CurrentPoint = 0;
            LastMax.Bar = 0;
            LastMax.Price = -9999999;
            LastMin.Bar = 0;
            LastMin.Price = 9999999;
            FalsePoint3 = 0;
            ClearPoint(1);
            ClearPoint(2);
            ClearPoint(21);
            ClearPoint(22);
            ClearPoint(3);
            ClearPoint(31);
            ClearPoint(4);
            ClearPoint(5);
            ClearPoint(6);
            ClearPoint(61);
            ClearPoint(62);
            ClearPoint(10);
            ClearPoint(41);
            for (I = 0; I < TargetLines.Length; I++)
            {
                TargetLines[I].Angle = 0;
                TargetLines[I].Delta = 0;
            }
            TrendLines[0].Angle = 0;
            TrendLines[0].Delta = 0;
            TrendLines[1] = TrendLine;
            //            TFs.Length = 1;
        }

        private void ClearTargets() //! Очистка целей модели
        {
            int I;
            for (I = 0; I < Targets.Length; I++)
            {
                Targets[I].Price = 0;
                Targets[I].Bar = 0;
            }
        }

        public void SetPointParams(double Bar, DateTime BarDT, double BarPrice, int PointNumber)    //!  Запись значения точки 
        {
            switch (PointNumber)
            {
                case 1:
                    {
                        Points1[0].Bar = Bar;
                        Points1[0].DT = BarDT;
                        Points1[0].Price = BarPrice;
                    }
                    break;
                case 2:
                    { // т2 - экстремум
                        Points2[0].Bar = Bar;
                        Points2[0].DT = BarDT;
                        Points2[0].Price = BarPrice;
                    }
                    break;
                case 21:
                    { // т2 - по касательной
                        Points2[1].Bar = Bar;
                        Points2[1].DT = BarDT;
                        Points2[1].Price = BarPrice;
                    }
                    break;
                case 22:
                    { // т2, дающая самую дальнюю т6
                        Points2[2].Bar = Bar;
                        Points2[2].DT = BarDT;
                        Points2[2].Price = BarPrice;
                    }
                    break;
                case 3:
                    {
                        Points3[0].Bar = Bar;
                        Points3[0].DT = BarDT;
                        Points3[0].Price = BarPrice;
                        CalcTrendLine();
                    }
                    break;
                case 31:
                    { // Касательная для МПМР(МДР)
                        Points3[1].Bar = Bar;
                        Points3[1].DT = BarDT;
                        Points3[1].Price = BarPrice;
                    }
                    break;
                case 4:
                    {
                        Points4[0].Bar = Bar;
                        Points4[0].DT = BarDT;
                        Points4[0].Price = BarPrice;
                        CalcTargetLine();
                        CalcTargetCorrected();
                        CalcModelType(false);
                        CalcSP();
                        // Расчет № последнего бара который может обработать модель
                        if (ModelType == Common.TModelType.ModelOfExpansion)
                            MaxProcessBar = Math.Round(Point4.Bar) + Options.SP_P4x4 * (Math.Round(Point4.Bar) - Math.Round(PointSP2.Bar));
                        else
                            if (ModelType == Common.TModelType.ModelOfAttraction)
                                MaxProcessBar = Math.Round(PointSP1.Bar) + Options.HP_P1x2 * (Math.Round(PointSP1.Bar) - Math.Round(Point1.Bar));
                            else
                                MaxProcessBar = Math.Round(Point4.Bar) + Options.P1_P4x4 * (Math.Round(Point4.Bar) - Math.Round(Point1.Bar));
                    }
                    break;
                case 5:
                    {
                        Points5[0].Bar = Bar;
                        Points5[0].DT = BarDT;
                        Points5[0].Price = BarPrice;
                        if (Points5[0].Bar != 0)
                        {
                            CalcPoint3Corrected();
                            CalcPoint6(false);
                        }
                        else
                        {   // Сброс расчетных т6
                            for (int i = 1; i < Points6.Count(); i++)
                            {
                                Points6[i].Bar = 0;
                                Points6[i].DT = new DateTime();
                                Points6[i].Price = 0;
                            }
                        }
                    }
                    break;
                case 6:
                    {
                        Points6[0].Bar = Bar;
                        Points6[0].DT = BarDT;
                        Points6[0].Price = BarPrice;
                    }
                    break;
                case 61:
                    {
                        Points6[1].Bar = Bar;
                        Points6[1].DT = BarDT;
                        Points6[1].Price = BarPrice;
                    }
                    break;
                case 62:
                    {
                        Points6[2].Bar = Bar;
                        Points6[2].DT = BarDT;
                        Points6[2].Price = BarPrice;
                    }
                    break;
                case 63:
                    {
                        Points6[3].Bar = Bar;
                        Points6[3].DT = BarDT;
                        Points6[3].Price = BarPrice;
                    }
                    break;
                case 64:
                    {
                        Points6[4].Bar = Bar;
                        Points6[4].DT = BarDT;
                        Points6[4].Price = BarPrice;
                    }
                    break;
                case 10:
                    {
                        BreakTrendLine.Bar = Bar;
                        BreakTrendLine.DT = BarDT;
                        BreakTrendLine.Price = BarPrice;
                    }
                    break;
                case 11:
                    {
                        BreakTargetLine.Bar = Bar;
                        BreakTargetLine.DT = BarDT;
                        BreakTargetLine.Price = BarPrice; 
                    }
                    break;
                case 41:    // т4 по касательной
                    {
                        Points4[1].Bar = Bar;
                        Points4[1].DT = BarDT;
                        Points4[1].Price = BarPrice;
                    }
                    break;
            }
        }

        private void CalcModelType(bool Tangent)    //! Определение типа модели
        {
            TLine TgL;  // Линия Целей
            TLine TL;   // Линия Тренда
            TPoint P1;
            TPoint P4;
            TPoint PSP;
            TPoint PSP1;
            if (Tangent)
            {   // Глобальная модель
                TgL = new TLine(TargetLineTangent);
                TL = new TLine(TrendLineTangent);
                P1 = new TPoint(Point11);
                P4 = new TPoint(Point41);
            }
            else
            {   // Обычная модель
                TgL = new TLine(TargetLineCorrected);
                TL = new TLine(TrendLine);
                P1 = new TPoint(Point1);
                P4 = new TPoint(Point4);
            }
            if (Math.Abs(TgL.Angle) > Math.Abs(TL.Angle))
            {
                ModelType = Common.TModelType.ModelOfExpansion;
                if (!Tangent) CalcSP();
                if (Tangent)
                {   // Глобальная модель
                    PSP = new TPoint(PointSP);
                    PSP1 = new TPoint(PointSP1);
                }
                else
                {   // Обычная модель
                    PSP = new TPoint(PointSP);
                    PSP1 = new TPoint(PointSP1);
                }
                if (((P1.Bar - PSP1.Bar) >= (P4.Bar - P1.Bar) * Options.MBSize1) ||
                    (PSP1.Bar > PSP.Bar))
                    ModelType = Common.TModelType.ModelOfBalance;
            }
            else
                if (Math.Abs(TgL.Angle) < Math.Abs(TL.Angle))
                {
                    ModelType = Common.TModelType.ModelOfAttraction;
                    if (!Tangent) CalcSP();
                    if (Tangent)
                    {   // Глобальная модель
                        PSP = new TPoint(PointSP2);
                        PSP1 = new TPoint(PointSP2);
                    }
                    else
                    {   // Обычная модель
                        PSP = new TPoint(PointSP);
                        PSP1 = new TPoint(PointSP1);
                    }
                    if ((PSP1.Bar - P1.Bar) >= (P4.Bar - P1.Bar) * Options.MBSize2)
                        ModelType = Common.TModelType.ModelOfBalance;
                }
                else
                {
                    ModelType = Common.TModelType.ModelOfBalance;
                }
            // Для ЧМП перенос бара т2' на ближайший справа экстремум
            // Корректировка TargetLineCorrected
            if ((ModelType == Common.TModelType.ModelOfAttraction) || (ModelType == Common.TModelType.ModelOfBalance))
            {
                SearchNearestRightExtremum(ModelDir, ref Points2[1]);
                TargetLineCorrected = Common.CalcLine(Point21, Point4);
            }
        }

        private void CalcSP()  //! Расчет положения СТ
        {
            switch (ModelType)
            {
                case Common.TModelType.ModelOfAttraction:
                    PointSP = Common.CrossLines(TrendLine, TargetLine);
                    PointSP1 = Common.CrossLines(TrendLine, TargetLineCorrected);
                    PointSP2 = Common.CrossLines(TrendLineTangent, TargetLineTangent);
                    break;
                case Common.TModelType.ModelOfExpansion:
                    {
                        PointSP = Common.CrossLines(TrendLine, TargetLine);
                        PointSP1 = Common.CrossLines(TrendLine, TargetLineCorrected);
                        PointSP2 = PointSP1;
                    }
                    break;
                default:
                    PointSP = Common.CrossLines(TrendLine, TargetLine);
                    PointSP1 = Common.CrossLines(TrendLine, TargetLineCorrected);
                    PointSP2 = PointSP1;
                    break;
            }
            // Расчет, если возможно, даты/времени СТ
            for (int i = 0; i <= 2; i++)
            {
                if ((PointsSP[i].Bar > 1) &&
                    (PointsSP[i].Bar < Quotes.GetCount() - 1))
                {
                    PointsSP[i].DT = Quotes[Convert.ToInt32(PointsSP[i].Bar)].DT;
                }
            }
        }

        private void CalcTrendLine()  //! Расчет трендовой линии
        {
            TrendLine = Common.CalcLine(Point1, Point3);
        }

        private void CalcTargetLine()   //! Расчет целевых линий, через экстремум т2 и по касательной
        {
            TargetLines[0] = Common.CalcLine(Point2, Point4);
            TargetLines[2] = Common.CalcLine(Point22, Point4);
        }
        private void CalcTargetCorrected()  //! Вычисление скорректированной целевой линии и точки 2'
        {
            int I;
            int P1;
            // Проход по барам от точки 2 назад до точки 1 и в случае пересечения ценой
            // линии 2'-4 точка 2' переносится на этот бар
            Points2[1].Bar = Points2[0].Bar;
            Points2[1].DT = Points2[0].DT;
            Points2[1].Price = Points2[0].Price;
            Points2[1].Bar = Points2[0].Bar;
            TargetLines[1] = Common.CalcLine(Points2[1], Point4);
            TargetLines[3] = TargetLines[1];
//            if (ModelType != Common.TModelType.ModelOfAttraction)
            {
                switch (ModelDir)
                {
                    case TDirection.dtUp:
                        {
                            if (Quotes[Convert.ToInt32(Point1.Bar)].Open < Quotes[Convert.ToInt32(Point1.Bar)].Close)
                                P1 = Convert.ToInt32(Point1.Bar);
                            else
                                P1 = Convert.ToInt32(Point1.Bar)+1;
                            for (I = Convert.ToInt32(Point2.Bar); I >= P1; I--)
                            {
                                SV = Quotes.GetBarByIndex(I);
                                if (Common.CalcLineValue(TargetLines[1], SV.X) < SV.High)
                                {
                                    SetPointParams(I, SV.DT, SV.High, 21);
                                    TargetLines[1] = Common.CalcLine(Point21, Point4);
                                }
                            }
                        }
                        break;
                    case TDirection.dtDn:
                        {
                            if (Quotes[Convert.ToInt32(Point1.Bar)].Open > Quotes[Convert.ToInt32(Point1.Bar)].Close)
                                P1 = Convert.ToInt32(Point1.Bar);
                            else
                                P1 = Convert.ToInt32(Point1.Bar) + 1;
                            for (I = Convert.ToInt32(Point2.Bar); I >= P1; I--)
                            {
                                SV = Quotes.GetBarByIndex(I);
                                if (Common.CalcLineValue(TargetLines[1], SV.X) > SV.Low)
                                {
                                    SetPointParams(I, SV.DT, SV.Low, 21);
                                    TargetLines[1] = Common.CalcLine(Point21, Point4);
                                }
                            }
                        }
                        break;
                }
            }
            CalcTangentLines();
        }

        private void SearchNearestRightExtremum(TDirection Dir, ref TPoint Point)   //! Перенос Point на ближайший справа экстремум от Point с направлением Dir
        {
            TBar CB = Quotes.GetBarByIndex(Convert.ToInt32(Point.Bar));
            switch (Dir)
            {
                case TDirection.dtUp:
                    {
                        for (int i = Convert.ToInt32(Point.Bar); i < Quotes.GetCount(); i++)
                        {
                            SV = Quotes.GetBarByIndex(i);
                            if (SV.High > CB.High)
                                CB = SV;
                            else
                            {
                                Point.Bar = SV.Bar;
                                Point.DT = SV.DT;
                                Point.Price = SV.High;
                                break;
                            }

                        }
                    }
                    break;
                case TDirection.dtDn:
                    {
                        for (int i = Convert.ToInt32(Point.Bar); i < Quotes.GetCount(); i++)
                        {
                            SV = Quotes.GetBarByIndex(i);
                            if (SV.Low < CB.Low)
                                CB = SV;
                            else
                            {
                                Point.Bar = SV.Bar;
                                Point.DT = SV.DT;
                                Point.Price = SV.Low;
                                break;
                            }
                        }
                    }
                    break;
            }
        }

        //! Вычисление положения т3' для МПМР(МПМДР)
        //! Вычисление целевой линии дающей самую дальнюю цель
        private void CalcPoint3Corrected()
        {
            int I;
            if (Point5.Bar == 0)
            {
                Point31 = Point5;
                return;
            }
            TLine TL = new TLine();
            TPoint P = new TPoint();
            // Проход по барам от точки 3 вперед до точки 5 и в случае пересечения ценой
            // линии 3'-5 точка 3' переносится на этот бар
            Points3[1].DT = Points3[0].DT;
            Points3[1].Price = Points3[0].Price;
            Points3[1].Bar = Points3[0].Bar;
            TargetLines[2] = Common.CalcLine(Point31, Point5);
            switch (ModelDir)
            {
                case TDirection.dtUp:
                    {
                        // Проверка на т3'
                        for (I = Convert.ToInt32(Point3.Bar) + 1; I < Convert.ToInt32(Point5.Bar); I++)
                        {
                            SV = Quotes.GetBarByIndex(I);
                            if (Common.CalcLineValue(TargetLines[2], SV.X) > SV.Low)
                            {
                                SetPointParams(I, SV.DT, SV.Low, 31);
                                TargetLines[2] = Common.CalcLine(Point31, Point5);
                            }
                        }
                        // Проверка на т2''
                        for (I = Convert.ToInt32(Points2[0].Bar) + 1; I < Convert.ToInt32(Point3.Bar); I++)
                        {
                            SV = Quotes.GetBarByIndex(I);
                            if ((Quotes.IsExtremum(Convert.ToInt32(SV.Bar), Options.Extremum2, TDirection.dtUp) == TDirection.dtUp) ||
                               (Quotes.IsExtremum(Convert.ToInt32(SV.Bar), Options.Extremum2, TDirection.dtUp) == TDirection.dtUpEqual))
                            {
                                P.DT = SV.DT;
                                P.Bar = SV.X;
                                P.Price = SV.High;
                                TL = Common.CalcLine(P, Point4);
                                if (Common.CrossLines(TL, TargetLineMA).Bar >= Point4.Bar)
                                {
                                    SetPointParams(P.Bar, P.DT, P.Price, 22);
                                    TargetLines[3] = TL;
                                }
                                else
                                    break;
                            }
                        }
                    }
                    break;
                case TDirection.dtDn:
                    {
                        // Проверка на т3'
                        for (I = Convert.ToInt32(Point3.Bar) + 1; I < Convert.ToInt32(Point5.Bar); I++)
                        {
                            SV = Quotes.GetBarByIndex(I);
                            if (Common.CalcLineValue(TargetLines[2], SV.X) < SV.High)
                            {
                                SetPointParams(I, SV.DT, SV.High, 31);
                                TargetLines[2] = Common.CalcLine(Point31, Point5);
                            }
                        }
                        // Проверка на т2''
                        for (I = Convert.ToInt32(Points2[0].Bar) + 1; I < Convert.ToInt32(Point3.Bar); I++)
                        {
                            SV = Quotes.GetBarByIndex(I);
                            if ((Quotes.IsExtremum(Convert.ToInt32(SV.Bar), Options.Extremum2, TDirection.dtDn) == TDirection.dtDn) ||
                               (Quotes.IsExtremum(Convert.ToInt32(SV.Bar), Options.Extremum2, TDirection.dtDn) == TDirection.dtDnEqual))
                            {
                                P.DT = SV.DT;
                                P.Bar = SV.X;
                                P.Price = SV.Low;
                                TL = Common.CalcLine(P, Point4);
                                if (Common.CrossLines(TL, TargetLineMA).Bar >= Point4.Bar)
                                {
                                    SetPointParams(P.Bar, P.DT, P.Price, 22);
                                    TargetLines[3] = TL;
                                }
                                else
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        //!
        //! Проверка на пересечение ценой линии тренда модели
        //!   TRUE - есть пересечение
        //!   FALSE - нет пересечения
        //!
        private bool CheckTrendLineBreakOut(TBar Bar)
        {
            bool result;
            double Price;
            Price = Quotes.GetPriceFromValue(Common.CalcLineValue(TrendLine, Bar.X));
            if (ModelDir == TDirection.dtUp)
                if (Price > (Quotes.GetPriceFromValue(Bar.Low) + pip))
                    result = true;
                else
                    result = false;
            else
                if (Price < (Quotes.GetPriceFromValue(Bar.High) - pip))
                    result = true;
                else
                    result = false;
            return result;
        }

        private bool CheckTargetLineBreakOut(TBar Bar)
        {
            bool result;
            double Price;
            Price = Quotes.GetPriceFromValue(Common.CalcLineValue(TargetLineCorrected, Bar.X));
            if (ModelDir == TDirection.dtUp)
                if (Price < (Quotes.GetPriceFromValue(Bar.High) - pip))
                    result = true;
                else
                    result = false;
            else
                if (Price > (Quotes.GetPriceFromValue(Bar.Low) + pip))
                    result = true;
                else
                    result = false;
            return result;
        }

        private bool CheckHPBreakOut(TBar Bar)  //! Проверка пробития уровня HP
        {
            bool R = false;
            switch (ModelType)
            {
                case Common.TModelType.ModelOfBalance:
                case Common.TModelType.ModelOfExpansion:
                    {
                        if ((ModelDir == TDirection.dtUp) && (Bar.High >= Point61.Price))
                        {
                            R = true;
                        }
                        if ((ModelDir == TDirection.dtDn) && (Bar.Low <= Point61.Price))
                        {
                            R = true;
                        }
                    }
                    break;
                case Common.TModelType.ModelOfAttraction:
                    {
                        if ((ModelDir == TDirection.dtUp) && (Bar.High >= PointSP.Price))
                        {
                            R = true;
                        }
                        if ((ModelDir == TDirection.dtDn) && (Bar.Low <= PointSP.Price))
                        {
                            R = true;
                        }
                    }
                    break;
            }
            HPBreakOut = HPBreakOut | R;
            Stat |= (uint)MStat.getupHP;
            //! Проверка пробития уровня HP до пробития трендовой и целевой
            HPGetInside |= HPBreakOut & BreakTrendLine.Bar == 0 & !TargetLineBreakOut & Point61.Bar > Point4.Bar;
            //! Проверка пробития уровня HP тем же баром которым пробита целевая
            if (!HPGetInside && TargetLineBreakOut && BreakTargetLine.Bar == Bar.Bar && HPBreakOut && Point61.Bar > Point4.Bar)
            {
                if ((ModelDir == TDirection.dtUp && Point61.Price <= Common.CalcLineValue(TargetLine, Bar.X))
                 || (ModelDir == TDirection.dtDn && Point61.Price >= Common.CalcLineValue(TargetLine, Bar.X)))
                    HPGetInside = true;
                else
                    HPGetOutside = true;
            }
            return HPBreakOut;
        }
        private void CheckHPBreakOut2(TBar Bar)  //! Проверка пробития уровня HP тем же баром которым пробита трендовая
        {
            switch (ModelType)
            {
                case Common.TModelType.ModelOfBalance:
                case Common.TModelType.ModelOfExpansion:
                    if ((ModelDir == TDirection.dtUp && Bar.High >= Point61.Price && Point61.Bar > Point4.Bar)
                    || (ModelDir == TDirection.dtDn && Bar.Low <= Point61.Price))
                    {
                        HPGetOutside = true;
                        Stat |= (uint)MStat.getupHP;
                    }
                break;
                case Common.TModelType.ModelOfAttraction:
                    if((ModelDir == TDirection.dtUp && Bar.High >= PointSP.Price)
                    || (ModelDir == TDirection.dtDn && Bar.Low <= PointSP.Price))
                    {
                        HPGetOutside = true;
                        Stat |= (uint)MStat.getupHP;
                    }
                break;
            }
        }

        private bool CheckRelationTargetBlock2Base(int Part)    //! Проверка соотношения БЦ больше 1/Part Базы
        {
            // Проверка на существование БЦ, наличие т5 и БЦ > Базы
            if ((Point6.Bar <= Point4.Bar) || (Point5.Price == Point4.Price) ||
               (((Math.Abs(Point5.Price - Point4.Price) > Math.Abs(Point2.Price - Point1.Price)) ||
                 ((Point5.Bar - Point4.Bar) > (Point3.Bar - Point1.Bar))))) return true;

            // Проверка соотноошения БЦ к Базе
            if (((Part == 3) && (Part * Math.Abs(Point4.Price - Point5.Price) >=Math.Abs(Point2.Price - Point1.Price)) &&
                (Part * (Point5.Bar - Point4.Bar + 1)  >= (Point3.Bar - Point1.Bar + 1))) ||
                ((Part == 2) && ((Part * Math.Abs(Point4.Price - Point5.Price) >=Math.Abs(Point2.Price - Point1.Price)) ||
                (Part * (Point5.Bar - Point4.Bar + 1) >= (Point3.Bar - Point1.Bar + 1)))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private TPoint LastMax = new TPoint();
        private TPoint LastMin = new TPoint();
        void SetLastMax(TBar Bar)
        {
            if (LastMax.Price < Bar.High)
            {
                LastMax.Bar = Bar.Bar;
                LastMax.DT = Bar.DT;
                LastMax.Price = Bar.High;
            }
        }
        TPoint SeekMax(TQuotes Quotes, int StartBar, int EndBar)
        {
            TPoint LastMaxPoint = new TPoint();
            LastMaxPoint.Bar = Quotes[StartBar].Bar;
            LastMaxPoint.DT = Quotes[StartBar].DT;
            LastMaxPoint.Price = Quotes[StartBar].High;
            for (int i = StartBar; i <= EndBar; i++)
                if (LastMaxPoint.Price < Quotes[i].High)
                {
                    LastMaxPoint.Bar = Quotes[i].Bar;
                    LastMaxPoint.DT = Quotes[i].DT;
                    LastMaxPoint.Price = Quotes[i].High;
                }
            return LastMaxPoint;
        }

        void SetLastMin(TBar Bar)
        {
            if (LastMin.Price > Bar.Low)
            {
                LastMin.Bar = Bar.Bar;
                LastMin.DT = Bar.DT;
                LastMin.Price = Bar.Low;
            }
        }
        TPoint SeekMin(TQuotes Quotes, int StartBar, int EndBar)
        {
            TPoint LastMinPoint = new TPoint();
            LastMinPoint.Bar = Quotes[StartBar].Bar;
            LastMinPoint.DT = Quotes[StartBar].DT;
            LastMinPoint.Price = Quotes[StartBar].Low;
            for (int i = StartBar; i <= EndBar; i++)
                if (LastMinPoint.Price > Quotes[i].Low)
                {
                    LastMinPoint.Bar = Quotes[i].Bar;
                    LastMinPoint.DT = Quotes[i].DT;
                    LastMinPoint.Price = Quotes[i].Low;
                }
            return LastMinPoint;
        }

        int PreProcessModel(TBar Bar)   //! Проверка т1 модели и определение направления модели
        {
            ClearModel();
            ClearTargets();
            CurrentPoint = 1;
            LastMin.Price = 999999;
            LastMax.Price = -999999;
            SymPoint3High = Quotes.GetBarByIndex(ProcessedBar).High;
            SymPoint3Low = Quotes.GetBarByIndex(ProcessedBar).Low;
            switch (Quotes.IsExtremum(ProcessedBar, Options.Extremum1, TDirection.dtUnKnown))
            {
                case TDirection.dtDn:
                case TDirection.dtDnEqual:
                    {
                        ModelDir = TDirection.dtUp;
                        SetPointParams(ProcessedBar, Bar.DT, Bar.Low, 1);
                        SetPointParams(ProcessedBar, new DateTime(), -999999, 2);
                        SetPointParams(0, new DateTime(), 999999, 5);
                        return 0;
                    }
                case TDirection.dtUp:
                case TDirection.dtUpEqual:
                    {
                        ModelDir = TDirection.dtDn;
                        SetPointParams(ProcessedBar, Bar.DT, Bar.High, 1);
                        SetPointParams(ProcessedBar, new DateTime(), 999999, 2);
                        SetPointParams(0, new DateTime(), -999999, 5);
                        return 0;
                    }
                default:
                    return 1;
            }
        }

        int ProcessBarInner1(TBar Bar)  // Обработка ожидания т3
        {
            TDirection Dir;
            TDirection Dir1;
            int SB;
            int EB;
            switch (ModelDir)
            {
                case TDirection.dtUp:
                    {
                        // Проверка на пробой уровня т1
                        if ((Bar.Low < Point1.Price) || (Point1.Price > SymPoint3Low))
                        {
                            return 1;
                        }
                        Dir = Quotes.IsExtremum(Convert.ToInt32(Bar.Bar) - 1, Options.Extremum2, TDirection.dtUp);
                        if ((Dir == TDirection.dtUp) || (Dir == TDirection.dtUpEqual))
                            SetPointParams(Bar.Bar - 1, Quotes.GetBarByIndex(Convert.ToInt32(Bar.Bar) - 1).DT, Quotes.GetBarByIndex(Convert.ToInt32(Bar.Bar) - 1).High, 22);
//                        Dir = Quotes.IsExtremum(Convert.ToInt32(Bar.Bar), Options.Extremum3, TDirection.dtDn);
                        if ((Bar.Low <= Quotes.GetBarByIndex(Convert.ToInt32(Bar.Bar) - 1).Low) || (FalsePoint3 != 0))
                            Dir = TDirection.dtDn;
                        else
                            Dir = TDirection.dtUnKnown;
 //                       Dir1 = Quotes.IsExtremum(Convert.ToInt32(LastMax.Bar), Options.Extremum2, TDirection.dtUp);
                        Dir1 = TDirection.dtUp;
                        if (((Dir == TDirection.dtDn) || (Dir == TDirection.dtDnEqual) || (Dir == TDirection.dtUpDn) | (Dir == TDirection.dtUpDn)) &&
                            ((Dir1 == TDirection.dtUp) || (Dir1 == TDirection.dtUpEqual)))
                        {
                            if (FalsePoint3 == 0)
                            {   // Первое пробитие ЛТ
                                // Установка т3
                                SetPointParams(Bar.Bar, Bar.DT, Bar.Low, 3);
                                FalsePoint3++;
                                CurrentPoint = 3;
                                // Проверка цвета свечи т1 и установка диапазона для поиска т2
                                // т1 на белой свече входит в диапазон, на черной не входит в диапазон
                                // Проверка цвета свечи т3 и установка диапазона для поиска т2
                                // т3 на черной свече входит в диапазон, на бело свече не входит в диапазон
                                SV1 = Quotes.GetBarByIndex(Convert.ToInt32(Point1.Bar));
                                SV3 = Quotes.GetBarByIndex(Convert.ToInt32(Point3.Bar));
                                if (SV1.Open < SV1.Close) SB = Convert.ToInt32(Point1.Bar); else SB = Convert.ToInt32(Point1.Bar) + 1;
                                if (SV3.Open > SV3.Close) EB = Convert.ToInt32(Point3.Bar); else EB = Convert.ToInt32(Point3.Bar) - 1;
                                Points2[0] = SeekMax(Quotes, SB, EB);
                                // Если бар пробивает уровень т2 и свеча белая, то обработка т4
                                if ((Bar.Open < Bar.Close) && (Bar.High >= Point2.Price))
                                    ProcessBarInner3(Bar);
                            }
                            else
                                if (Common.CalcLineValue(TrendLine, Bar.X) > Bar.Low)
                                {
                                    // Установка т3
                                    SetPointParams(Bar.Bar, Bar.DT, Bar.Low, 3);
                                    CurrentPoint = 3;
                                    // Проверка цвета свечи т1 и установка диапазона для поиска т2
                                    // т1 на белой свече входит в диапазон, на черной не входит в диапазон
                                    // Проверка цвета свечи т3 и установка диапазона для поиска т2
                                    // т3 на черной свече входит в диапазон, на белой свече не входит в диапазон
                                    SV1 = Quotes.GetBarByIndex(Convert.ToInt32(Point1.Bar));
                                    SV3 = Quotes.GetBarByIndex(Convert.ToInt32(Point3.Bar));
                                    if (SV1.Open < SV1.Close) SB = Convert.ToInt32(Point1.Bar); else SB = Convert.ToInt32(Point1.Bar) + 1;
                                    if (SV3.Open > SV3.Close) EB = Convert.ToInt32(Point3.Bar); else EB = Convert.ToInt32(Point3.Bar) - 1;
                                    Points2[0] = SeekMax(Quotes, SB, EB);
                                    // Если бар пробивает уровень т2 и свеча белая, то обработка т4
                                    if ((Bar.Open < Bar.Close) && (Bar.High >= Point2.Price))
                                        ProcessBarInner3(Bar);
                                }
                        }
                        SetLastMax(Bar);
                    }
                    break;
                case TDirection.dtDn:
                    {
                        // Проверка на пробой уровня т1
                        if ((Bar.High > Point1.Price) || (Point1.Price < SymPoint3High))
                        {
                            return 1;
                        }
                        Dir = Quotes.IsExtremum(Convert.ToInt32(Bar.Bar) - 1, Options.Extremum2, TDirection.dtDn);
                        if ((Dir == TDirection.dtDn) || (Dir == TDirection.dtDnEqual))
                            SetPointParams(Convert.ToInt32(Bar.Bar) - 1, Quotes.GetBarByIndex(Convert.ToInt32(Bar.Bar) - 1).DT, Quotes.GetBarByIndex(Convert.ToInt32(Bar.Bar) - 1).Low, 22);
                        //Dir = Quotes.IsExtremum(Convert.ToInt32(Bar.Bar), Options.Extremum3, TDirection.dtUp);
                        if ((Bar.High >= Quotes.GetBarByIndex(Convert.ToInt32(Bar.Bar) - 1).High) || (FalsePoint3 != 0))
                            Dir = TDirection.dtUp;
                        else
                            Dir = TDirection.dtUnKnown;
                        //Dir1 = Quotes.IsExtremum(Convert.ToInt32(LastMin.Bar), Options.Extremum2, TDirection.dtDn);
                        Dir1 = TDirection.dtDn;
                        if (((Dir == TDirection.dtUp) || (Dir == TDirection.dtUpEqual) || (Dir == TDirection.dtUpDn) || (Dir == TDirection.dtUpDn))
                            && ((Dir1 == TDirection.dtDn) || (Dir1 == TDirection.dtDnEqual)))
                        {
                            if (FalsePoint3 == 0)
                            {   // Первое пробитие ЛТ
                                // Установка т3
                                SetPointParams(Bar.Bar, Bar.DT, Bar.High, 3);
                                CurrentPoint = 3;
                                FalsePoint3++;
                                // Проверка цвета свечи т1 и установка диапазона для поиска т2
                                // т1 на черной свече входит в диапазон, на белой не входит в диапазон
                                // Проверка цвета свечи т3 и установка диапазона для поиска т2
                                // т3 на белой свече входит в диапазон, на черной не входит в диапазон
                                SV1 = Quotes.GetBarByIndex(Convert.ToInt32(Point1.Bar));
                                SV3 = Quotes.GetBarByIndex(Convert.ToInt32(Point3.Bar));
                                if (SV1.Open > SV1.Close) SB = Convert.ToInt32(Point1.Bar); else SB = Convert.ToInt32(Point1.Bar) + 1;
                                if (SV3.Open < SV3.Close) EB = Convert.ToInt32(Point3.Bar); else EB = Convert.ToInt32(Point3.Bar) - 1;
                                Points2[0] = SeekMin(Quotes, SB, EB);
                                // Если бар пробивает уровень т2 и свеча черная, то обработка т4
                                if ((Bar.Open > Bar.Close) && (Bar.Low <= Point2.Price))
                                    ProcessBarInner3(Bar);
                            }
                            else
                                if (Common.CalcLineValue(TrendLine, Bar.X) < Bar.High)
                                {
                                    // Установка т3
                                    SetPointParams(Bar.Bar, Bar.DT, Bar.High, 3);
                                    CurrentPoint = 3;
                                    // Проверка цвета свечи т1 и установка диапазона для поиска т2
                                    // т1 на черной свече входит в диапазон, на белой не входит в диапазон
                                    // Проверка цвета свечи т3 и установка диапазона для поиска т2
                                    // т3 на белой свече входит в диапазон, на черной не входит в диапазон
                                    SV1 = Quotes.GetBarByIndex(Convert.ToInt32(Point1.Bar));
                                    SV3 = Quotes.GetBarByIndex(Convert.ToInt32(Point3.Bar));
                                    if (SV1.Open > SV1.Close) SB = Convert.ToInt32(Point1.Bar); else SB = Convert.ToInt32(Point1.Bar) + 1;
                                    if (SV3.Open < SV3.Close) EB = Convert.ToInt32(Point3.Bar); else EB = Convert.ToInt32(Point3.Bar) - 1;
                                    Points2[0] = SeekMin(Quotes, SB, EB);
                                    // Если бар пробивает уровень т2 и свеча черная, то обработка т4
                                    if ((Bar.Open > Bar.Close) && (Bar.Low <= Point2.Price))
                                        ProcessBarInner3(Bar);
                                }
                        }
                        SetLastMin(Bar);
                    }
                    break;
            }
            return 0;
        }
        int ProcessBarInner3(TBar Bar)  // Обработка ожидания т4
        {
            // Проверка цвета свечи 
            // Для Ап модели - белая свеча, вначале проверка пробития ЛТ, затем пробитие уровня т2
            //                 черная свеча, вначале проверка пробитие уровня т2, запем пробитие ЛТ
            // Для Даун модели - черная свеча, вначале проверка пробития ЛТ, затем пробитие уровня т2
            //                   белая свеча, вначале проверка пробития уровня т2, затем пробитие ЛТ
            switch (ModelDir)
            {
                case TDirection.dtUp:
                    {
                        // Проверка на пробой уровня т1
                        if (Bar.Low < Point1.Price)
                        {
                            return 1;
                        }
                        // Проверка цвета свечи
                        // Свеча белая, проверяется пробой ЛТ, затем пробой уровня т2
                        if (Bar.Open < Bar.Close)
                        {
                            // Проверка на пробой ЛТ, если да, то перенос т3 на этот бар
                            if (CheckTrendLineBreakOut(Bar) && (Bar.Bar != Point3.Bar))
                            {
                                CurrentPoint = 1;
                                ClearPoint(3);
                                ClearPoint(10);
                                ProcessBarInner1(Bar);
                                FalsePoint3++;
                            }
                        }
                        // Если в состоянии поиска т4, то проверка пробоя т2
                        if ((Bar.High >= LastMax.Price) && (CurrentPoint == 3))
                        {
                            // Установка т4
                            CurrentPoint = 4;
                            SetPointParams(Bar.Bar, Bar.DT, Bar.High, 4);
                            // Если свеча черная, то т5 на другом конце бара т4
                            if (Bar.Open > Bar.Close)
                                SetPointParams(Point4.Bar, Point4.DT, Quotes.GetBarByIndex(Convert.ToInt32(Point4.Bar)).Low, 5);
                            else
                                SetPointParams(Point4.Bar, Point4.DT, Point4.Price, 5);
                            SetPointParams(Bar.Bar, Bar.DT, Bar.High, 6);
                            // Проверка пробоя  ЛТ для черной свечи
                            if ((Bar.Open > Bar.Close) && (CheckTrendLineBreakOut(Bar)))
                                ProcessBarInner(Bar);
                        }
                    }
                    break;
                case TDirection.dtDn:
                    {
                        // Проверка на пробой уровня т1
                        if (Bar.High > Point1.Price) 
                        {
                            return 1;
                        }
                        // Проверка цвета свечи
                        // Если свеча черная, то проверка пробоя ЛТ, затем пробоя уровня т2
                        if (Bar.Open > Bar.Close)
                        {
                            // Проверка на пробой ЛТ, если да, то перенос т3 на этот бар
                            if (CheckTrendLineBreakOut(Bar) && (Bar.Bar != Point3.Bar))
                            {
                                CurrentPoint = 1;
                                ClearPoint(3);
                                ClearPoint(10);
                                ProcessBarInner1(Bar);
                                FalsePoint3++;
                            }
                        }
                        // Проверка на пробой уровня т2
                        if ((Bar.Low <= LastMin.Price) && (CurrentPoint == 3))
                        {
                            CurrentPoint = 4;
                            SetPointParams(Bar.Bar, Bar.DT, Bar.Low, 4);
                            // Если свеча белая, то т5 на другом конце бара т4
                            if (Bar.Open < Bar.Close)
                                SetPointParams(Point4.Bar, Point4.DT, Quotes.GetBarByIndex(Convert.ToInt32(Point4.Bar)).High, 5);
                            else
                                SetPointParams(Point4.Bar, Point4.DT, Point4.Price, 5);
                            SetPointParams(Bar.Bar, Bar.DT, Bar.Low, 6);
                            // Проверка пробоя  ЛТ для белой свечи
                            if ((Bar.Open < Bar.Close) && (CheckTrendLineBreakOut(Bar)))
                                ProcessBarInner(Bar);
                        }
                    }
                    break;
            }
            SetLastMin(Bar);
            SetLastMax(Bar);
            // Проверка на пробой ЛТ, если да, то перенос т3 на этот бар
            if (CheckTrendLineBreakOut(Bar) && (CurrentPoint == 3) && (Bar.Bar != Point3.Bar))
            {
                CurrentPoint = 1;
                ClearPoint(3);
                ClearPoint(10);
                ProcessBarInner1(Bar);
                FalsePoint3++;
            }
            return 0;
        }

        int ProcessBarInner(TBar Bar)
        {
            int result = -1;
            double TLPrice;
            TDirection Dir;
            TDirection Dir1;
            // Вычисление Лоу и Хая на промежутке от т1 влево на расстоянии Bar.Bar-Point1.Bar
            if (SymPoint3High < Quotes.GetBarByIndex(Convert.ToInt32(Math.Max(Point1.Bar - (Bar.Bar - Point1.Bar) * Options.SizePoint1, 0))).High)
                SymPoint3High = Quotes.GetBarByIndex(Convert.ToInt32(Math.Max(Point1.Bar - (Bar.Bar - Point1.Bar) * Options.SizePoint1, 0))).High;
            if (SymPoint3Low > Quotes.GetBarByIndex(Convert.ToInt32(Math.Max(Point1.Bar - (Bar.Bar - Point1.Bar) * Options.SizePoint1, 0))).Low)
                SymPoint3Low = Quotes.GetBarByIndex(Convert.ToInt32(Math.Max(Point1.Bar - (Bar.Bar - Point1.Bar) * Options.SizePoint1, 0))).Low;
            switch (CurrentPoint)
            {
                case 0:
                    {   // Обработка т1. Проверка бара на экстремальность и определение направления модели
                        result = PreProcessModel(Bar);
                    }
                    break;
                case 1:
                    {   // Поиск т3
                        result = ProcessBarInner1(Bar);
                    }
                    break;
                case 3:
                    {   // Поиск т4
                        result = ProcessBarInner3(Bar);
                    }
                    break;
                case 4:
                    {   // Поиск пробоя уровня т4 или ЛТ
                        if (CheckTrendLineBreakOut(Bar))
                        {
                            // Первый пробой ЛТ
                            if(BreakTrendLine.Bar == 0){
                                switch (ModelDir)
                                {
                                    case TDirection.dtUp:
                                        {
                                            if ((Quotes.IsExtremum(Convert.ToInt32(Bar.Bar), Options.Extremum4, TDirection.dtUp) == TDirection.dtUp) &&
                                                (Bar.High > LastMax.Price))
                                            {
                                                CurrentPoint = 4;
                                                SetPointParams(Bar.Bar, Bar.DT, Bar.High, 4);
                                                SetPointParams(Point4.Bar, Point4.DT, Point4.Price, 5);
                                                SetPointParams(Bar.Bar, Bar.DT, Bar.High, 6);
                                            }
                                        }
                                        break;
                                    case TDirection.dtDn:
                                        {
                                            if ((Quotes.IsExtremum(Convert.ToInt32(Bar.Bar), Options.Extremum4, TDirection.dtDn) == TDirection.dtDn)
                                                && (Bar.Low < LastMin.Price))
                                            {
                                                CurrentPoint = 4;
                                                SetPointParams(Bar.Bar, Bar.DT, Bar.Low, 4);
                                                SetPointParams(Point4.Bar, Point4.DT, Point4.Price, 5);
                                                SetPointParams(Bar.Bar, Bar.DT, Bar.Low, 6);
                                            }
                                        }
                                        break;
                                }
                               // SetPointParams(Bar.Bar, Bar.DT, Common.CalcLineValue(TrendLine, Bar.X), 10);
                               // BreakTrendLineFirst = BreakTrendLine;
                               // SearchLastBreakTrendLine = true;
                                CheckHPBreakOut2(Bar);
                            }
                       
                            // Установка точки пробоя ЛТ
                            {
                                SetPointParams(Bar.Bar, Bar.DT, Common.CalcLineValue(TrendLine, Bar.X), 10); //last tuch
                                BreakTrendLineFirst = BreakTrendLine; //first tuch
                                SearchLastBreakTrendLine = true;
                            }
                            CurrentPoint = 6;
                            if (Point6.Bar == Point4.Bar)
                            {
                                switch (ModelDir)
                                {// если нет 5-й так ее нет!!!
                                    case TDirection.dtUp:
                                        // SetPointParams(Point4.Bar, Point4.DT, Point4.X, Quotes.GetBarByIndex(Convert.ToInt32(Point4.Bar)).Low, 5);
                                        SetPointParams(0d, new System.DateTime(0), 0d, 5);
                                        break;
                                    case TDirection.dtDn:
                                        // SetPointParams(Point4.Bar, Point4.DT, Point4.X, Quotes.GetBarByIndex(Convert.ToInt32(Point4.Bar)).High, 5);
                                        SetPointParams(0d, new System.DateTime(0), 0d, 5);
                                        break;
                                }
                            }
                            //CalcPoint3Corrected(); нет тут никаких 3'
                            CalcTargets();
                            CurrentPoint = 99;
                        }
                        else
                        {   // Пробития трендовой нет
                            SV2 = Quotes.GetBarByIndex(Convert.ToInt32(Point2.Bar));
                            switch (ModelDir)
                            {
                                case TDirection.dtUp:
                                    {   // Вычисление точки 5
                                        if (Bar.High >= Point4.Price)
                                        {   
                                            // Подтверждение т4
                                            // Проверка цвета свечи пробивающей уровнеь т4
                                            if (/*(Bar.High == Point4.Price) && */(Bar.Open < Bar.Close))
                                                if (Point5.Price > Bar.Low)
                                                    SetPointParams(Bar.Bar, Bar.DT, Bar.Low, 5);
                                            // Пробитие уровня точки 4, Блок целей > 1/3 Базы для МР
                                            // Расстояние т1-т2 более чем в 2 раза больше расстояния т4-т5 по цене для МДР
                                            // Нет пересечения тел баров т2-т5 для ЧМП
                                            Dir = Quotes.IsExtremum(Convert.ToInt32(Point5.Bar), Options.Extremum5, TDirection.dtDn);
                                            Dir1 = Quotes.IsExtremum(Convert.ToInt32(Point4.Bar), Options.Extremum4, TDirection.dtUp);
                                            if ((Dir1 == TDirection.dtUpDn) || (Dir1 == TDirection.dtUpEqual)) Dir1 = TDirection.dtUp;
                                            SV5 = Quotes.GetBarByIndex(Convert.ToInt32(Point5.Bar));
                                            if ((Bar.Bar > Point4.Bar) &&
                                                (Dir1 == TDirection.dtUp) /*&&
//                                                 CheckRelationTargetBlock2Base() &&
                                                 ((2 * (Point4.Price - Point5.Price) >= (Point2.Price - Point1.Price)) || (ModelType != Common.TModelType.ModelOfBalance)) &&
                                                 ((ModelType != Common.TModelType.ModelOfAttraction) || (Math.Min(SV5.Close,SV5.Open) >= Math.Max(SV2.Close,SV2.Open)))*/) 
                                            {   //Точка 5 экстремум, переход на поиск точки 6)
                                                CurrentPoint = 5;
                                                if (!TargetLineBreakOut)
                                                    if (CheckTargetLineBreakOut(Bar))
                                                    {
                                                        TargetLineBreakOut = true;
                                                        SetPointParams(Bar.Bar, Bar.DT, Common.CalcLineValue(TargetLine, Bar.X), 11);
                                                    }
//                                                CalcPoint3Corrected();
                                                CalcPoint6(false);
                                                SetPointParams(Bar.Bar, Bar.DT, Bar.High, 6);
                                                CheckHPBreakOut(Bar);
                                            }
                                            else
                                            {   // Проверка на т4 из двух баров с равными лоу
//                                                if (!(Convert.ToInt32(Bar.Bar) - 1 == Convert.ToInt32(Point4.Bar)) &&
//                                                    (Bar.High == Point4.Price))
                                                {   // Возврат на поиск точки 4
                                                    CurrentPoint = 3;
                                                    ClearPoint(4);
                                                    ProcessBarInner(Bar);
                                                }
                                            }
                                        }
                                        else
                                            if (Point5.Price > Bar.Low)
                                            {
                                                SetPointParams(Bar.Bar, Bar.DT, Bar.Low, 5);
                                            }
                                    }
                                    break;
                                case TDirection.dtDn:
                                    {   // Вычисление точки 5
                                        if (Bar.Low <= Point4.Price)
                                        {   
                                            // Подтверждение т4
                                            // Проверка цвета свечи пробивающей уровнеь т4
                                            if (/*(Bar.Low == Point4.Price) && */(Bar.Open > Bar.Close))
                                                if (Point5.Price < Bar.High)
                                                    SetPointParams(Bar.Bar, Bar.DT, Bar.High, 5);
                                            // Пробитие уровня точки 4, Блок целей > 1/3 Базы для МР
                                            // Расстояние т1-т2 более чем в 2 раза больше расстояния т4-т5 по цене для МДР
                                            // Нет пересечения тел баров т2-т5 для ЧМП
                                            Dir = Quotes.IsExtremum(Convert.ToInt32(Point5.Bar), Options.Extremum5, TDirection.dtUp);
                                            Dir1 = Quotes.IsExtremum(Convert.ToInt32(Point4.Bar), Options.Extremum4, TDirection.dtDn);
                                            if ((Dir1 == TDirection.dtUpDn) || (Dir1 == TDirection.dtDnEqual)) Dir1 = TDirection.dtDn;
                                            SV5 = Quotes.GetBarByIndex(Convert.ToInt32(Point5.Bar));
                                            if ((Bar.Bar > Point4.Bar) &&
                                                (Dir1 == TDirection.dtDn) /*&&
//                                                 CheckRelationTargetBlock2Base() &&
                                                 ((2 * (Point5.Price - Point4.Price) >= (Point1.Price - Point2.Price)) || (ModelType != Common.TModelType.ModelOfBalance)) &&
                                                 ((ModelType != Common.TModelType.ModelOfAttraction) || (Math.Max(SV5.Close,SV5.Open) <= Math.Min(SV2.Close,SV2.Open)))*/) 
                                            {   //Точка 5 экстремум, переход на поиск точки 6
                                                CurrentPoint = 5;
                                                if (!TargetLineBreakOut)
                                                    if (CheckTargetLineBreakOut(Bar))
                                                    {
                                                        TargetLineBreakOut = true;
                                                        SetPointParams(Bar.Bar, Bar.DT, Common.CalcLineValue(TargetLine, Bar.X), 11);
                                                    }
                                                //                                                CalcPoint3Corrected();
                                                CalcPoint6(false);
                                                SetPointParams(Bar.Bar, Bar.DT, Bar.Low, 6);
                                                CheckHPBreakOut(Bar);
                                            }
                                            else
                                            {   // Проверка на т4 из двух баров с равными лоу
//                                                if (!(Convert.ToInt32(Bar.Bar - 1) == Convert.ToInt32(Point4.Bar)) &&
//                                                    (Bar.Low == Point4.Price))
                                                {
                                                    // Возврат на поиск точки 4
                                                    CurrentPoint = 3;
                                                    ClearPoint(4);
                                                    ProcessBarInner3(Bar);
                                                }
                                            }
                                        }
                                        else
                                            if (Point5.Price < Bar.High)
                                            {
                                                SetPointParams(Bar.Bar, Bar.DT, Bar.High, 5);
                                            }
                                    }
                                    break;
                            }
                            SetLastMin(Bar);
                            SetLastMax(Bar);
                        }
                        result = 0;
                    }
                    break;
                case 5:
                    {
                        if (!TargetLineBreakOut)
                            if (CheckTargetLineBreakOut(Bar))
                            {
                                TargetLineBreakOut = true;
                                SetPointParams(Bar.Bar, Bar.DT, Common.CalcLineValue(TargetLine, Bar.X), 11);
                            }
                    }
                    goto case 6;
                case 6:
                    {
                        CheckHPBreakOut(Bar);
                        // проверка на одинаковость расположения точек пересечения ЛЦ's и ЛТ относительно т3
                        if (ModelType == Common.TModelType.UnknownModel)
                            return 1;
                        // Проверка на время жизни модели
                        if (Bar.Bar > MaxProcessBar)
                        {
                            IsAlive = false;
                            result = 0;
                            break;
                        }
                        SetLastMin(Bar);
                        SetLastMax(Bar);
                        switch (ModelDir)
                        {
                            case TDirection.dtUp:
                                {
                                    if (Point6.Price < Bar.High)
                                    {
                                        SetPointParams(Bar.Bar, Bar.DT, Bar.High, 6);
                                    }
                                    //далее находим 6-ю фиксируемую по откату к 4й
                                    if ((Point4.Price >= Bar.Low) && (!HPreached))
                                    {
                                        if (Point61.Bar > 0)
                                        {
                                            if ((Point61.Price - Point6.Price) < 0.1 * (Point61.Price - Point1.Price))
                                            {
                                                Point65 = Point6;
                                                if((Point61.Price - Point6.Price)<=0) HPreached = true;
                                            }
                                        }
                                    }
                                }
                                break;
                            case TDirection.dtDn:
                                {
                                    if (Point6.Price > Bar.Low)
                                    {
                                        SetPointParams(Bar.Bar, Bar.DT, Bar.Low, 6);
                                    }
                                    //далее находим 6-ю фиксируемую по откату к 4й
                                    if ((Point4.Price <= Bar.High) && (!HPreached))
                                    {
                                        if (Point61.Bar > 0)
                                        {
                                            if ((Point6.Price - Point61.Price) < 0.1 * (Point1.Price - Point61.Price))
                                            {
                                                Point65 = Point6;
                                                if ((Point61.Price - Point6.Price) >= 0) HPreached = true;
                                            }
                                        }
                                    }

                                }
                                break;
                        }
                        if (CheckTrendLineBreakOut(Bar))
                        {
                            SetPointParams(Bar.Bar, Bar.DT, Common.CalcLineValue(TrendLine, Bar.X), 10);
                            BreakTrendLineFirst = BreakTrendLine;
                            SearchLastBreakTrendLine = true;
                            CurrentPoint = 6;
                            if (Point6.Bar == Point4.Bar)
                            {
//                                if (NewModelFlag == 0) NewModelFlag = 1;//рождение новой модели с той же 1й и 2й равной 4й текущей модели
                                Points5[0].Bar = Point4.Bar;
                                Points5[0].DT = Point4.DT;
                                switch (ModelDir)
                                {
                                    case TDirection.dtUp:
                                        Points5[0].Price = Quotes.GetBarByIndex(Convert.ToInt32(Point4.Bar)).Low;
                                        break;
                                    case TDirection.dtDn:
                                        Points5[0].Price = Quotes.GetBarByIndex(Convert.ToInt32(Point4.Bar)).High;
                                        break;
                                }
                            }
                            else
                            {
                                CalcPoint3Corrected();
                                CalcPoint6(false);
                                //рождение новой модели с той же 1й и 2й равной 6й текущей модели
//                                if (NewModelFlag == 0) NewModelFlag = 2;
                            }
                            CalcTargets();
                            CurrentPoint = 99;
                        }
                        if (CheckTargetLineBreakOut(Bar))
                        {
                            // Проверка 4-й на окончательность
                            if (((3 * (Point5.Price - Point4.Price) / (Point1.Price - Point2.Price) > 1))
                                || (3.0 * (Point5.Bar - Point4.Bar) / (Point3.Bar - Point1.Bar) > 1))
                            {
                            }
                            else
                            {
                                //добавить код для появления новой 4-й на той же базе
//                                NewModelFlag = 3;
                            }
                        }

                        result = 0;
                    }
                    break;
                case 99:
                    {
                        // Проверка на время жизни модели
                        result = 0;
                        if (Bar.Bar > MaxProcessBar)
                        {
                            if (ModelType == Common.TModelType.ModelOfAttraction)
                            {
                                switch (ModelDir)
                                {
                                    case TDirection.dtUp:
                                        {
                                            SetPointParams(LastMax.Bar, LastMax.DT, LastMax.Price, 6);
                                        }
                                        break;
                                    case TDirection.dtDn:
                                        {
                                            SetPointParams(LastMin.Bar, LastMin.DT, LastMin.Price, 6);
                                        }
                                        break;
                                }
                            }
                            IsAlive = false;
                            result = 0;
                            break;
                        }
                        switch (ModelType)
                        {
                            case Common.TModelType.ModelOfBalance:
                            case Common.TModelType.ModelOfExpansion:
                                {
                                    // обнаружение достижения первой цели или уровня т6
                                    SV = Quotes.GetBarByIndex(Convert.ToInt32(Bar.Bar - 1));
                                    TLPrice = Common.CalcLineValue(TrendLine, Bar.X);
                                    
                                    // остановка поиска последнего касания при достижении цены ЛТ уровня 6
                                    if ((ModelDir == TDirection.dtUp && TLPrice >= Point6.Price)
                                     || (ModelDir == TDirection.dtDn && TLPrice <= Point6.Price))
                                    {
                                        SearchLastBreakTrendLine = false;
                                        // и сброс последнего касания на точку пробоя
                                        BreakTrendLine = BreakTrendLineFirst;
                                    }
                                    
                                    // Проверка пробития ЛТ
                                    if (SearchLastBreakTrendLine &&
                                        ((TLPrice >= Bar.Low && TLPrice <= Bar.High) // Обработка пересечения ЛТ бара
                                        || ((TLPrice >= Math.Min(Bar.Low, SV.Low) && TLPrice <= Math.Max(Bar.High, SV.High))
                                        && TLPrice > Math.Min(Bar.High, SV.High) && TLPrice < Math.Max(Bar.Low, SV.Low))// Обработка гэпа
                                        || ((ModelDir == TDirection.dtUp && Bar.Low > TLPrice) || (ModelDir == TDirection.dtDn && Bar.High < TLPrice)))) // Отбой от ЛТ
                                    {
                                        SetPointParams(Bar.Bar, Bar.DT, Common.CalcLineValue(TrendLine, Bar.X), 10); // последнее касание ЛТ
                                        CalcTargets();
                                        result = 0;
                                    }
                                    else
                                        ////для фиксации последнего касания нам нужен _весомый_ экстремум противоположный 4,
                                        ////на данный момент взят первый экстремум противоположный 4,
                                        ////экстремальная свеча не касается линии тренда
                                        ////(вобщем недоработанное условие, вероятно нужно учитывать также соотношение экстремума с базой, и факт достижения цели до появления такогоэкстремума)
                                        //if ((ModelDir == TDirection.dtUp && Quotes.IsExtremum((int)Bar.Bar, 1, TDirection.dtDn) == TDirection.dtDn)
                                        // || (ModelDir == TDirection.dtDn && Quotes.IsExtremum((int)Bar.Bar, 1, TDirection.dtUp) == TDirection.dtUp))
                                    {
                                        SearchLastBreakTrendLine = false;
                                    }
                                }
                                break;
                            case Common.TModelType.ModelOfAttraction:
                                {
                                    SetLastMin(Bar);
                                    SetLastMax(Bar);
                                }
                                break;
                        }
                    }
                    break;
            }
            return result;
        }

        public int ProcessBar(TBar Bar) //! Основная процедура обработки баров
        //  ProcessedBar:=Bar.Bar;
        {
            int result;
            result = ProcessBarInner(Bar);
            ProcessedBar++;
            return result;
        }

        //!
        //! Расчет тт6
        //! Points6[1] - как точки пересечения ЛЦ' и ЛТ
        //! Points6[2] - как точки пересечения ЛЦ и ЛТ
        //! Points6[3] - как точки пересечения ЛЦ'' и ЛТ
        //! Points6[4] - как точки пересечения касательных ЛЦ и ЛТ
        //! 
        public void CalcPoint6(bool Tangent)    //! Расчет 6-х точек
        {
            if ((CurrentPoint < 5) || (Point5.Bar == 0)) return;
            // Расчет т3'
            if (Point31.Bar == 0) 
                CalcPoint3Corrected();
            if (ModelType == Common.TModelType.ModelOfAttraction)
            {
                Points6[1] = Common.CrossLines(TargetLineCorrected, TargetLineMA);
                Points6[4] = Common.CrossLines(TargetLine, TargetLineMA);
            }
            else
            {
                if (Tangent)
                    Points6[4] = Common.CrossLines(TargetLineTangent, TargetLineMA);
                else
                    Points6[4] = Common.CrossLines(TargetLineCorrected, TargetLineMA);

                Points6[1] = Common.CrossLines(TargetLineCorrected, TargetLineMA);
                if (Point61.Bar >= Point4.Bar)
                {
                    if (Tangent)
                    {
                        Point61Pct100 = new TPoint(Point64.Bar, Point64.DT, 2 * Point64.Price - Point1.Price);
                        Point61Pct200 = new TPoint(Point64.Bar, Point64.DT, 3 * Point64.Price - 2 * Point1.Price);
                    }
                    else
                    {
                        Point61Pct100 = new TPoint(Point61.Bar, Point61.DT, 2 * Point61.Price - Point1.Price);
                        Point61Pct200 = new TPoint(Point61.Bar, Point61.DT, 3 * Point61.Price - 2 * Point1.Price);
                    }
                }
                else
                {
                    Point61 = new TPoint();
                    Point61Pct100 = new TPoint();
                    Point61Pct200 = new TPoint();
                }
                // Проверка положения т6расч по через т2экстремум
                if (Point61.Bar > Point4.Bar)
                {
                    // Проверка на лишнии точки на ЛЦ через т2''
                }
            }
            // Расчет т2'' как точки дающей самую дальнюю т6
            CalcPoint6Far();

            // Расчет т6 через т2экстремум
            if (Point22.Bar >= Point2.Bar)
            {
                Point62 = Common.CrossLines(TargetLine, TargetLineMA);
            }
            else
            {
                Point62 = new TPoint();
            }
            // Проверка на сходимость линий
            if ((Point61.Bar - Point1.Bar) > Options.MBSize2 * (Point4.Bar - Point1.Bar))
            {
                for (int i=1; i <= 6; i++)
                {
                    Points6[i].Bar = 0;
                    Points6[i].DT = new DateTime();
                    Points6[i].Price = 0;
                }
            }
            // Вычисление, если возможно, даты/времени расчетных т6
            for (int i = 1; i <= 4; i++)
            {
                if ((Points6[i].Bar > 1) &&
                    (Points6[i].Bar < Quotes.GetCount() - 1))
                {
                    Points6[i].DT = Quotes[Convert.ToInt32(Points6[i].Bar)].DT;
                }
            }
        }

        //!
        //! Проверяется наличие лишних точек на ЛЦ через т2'
        //! Если есть лишние точки, то в качестве Point63 берется Point61 (при построении по касательным Point64)
        //! Если лишних точек  нет, то проверяются последущие экстремумы, пока непоявятся лишние тчки на ЛЦ''
        //!
        private void CalcPoint6Far()    //! Расчет дальней т6
        {
            TDirection Dir;
            TPoint P2 = new TPoint();
            TLine TgL = new TLine();
            int P3Bar;
            //  Исходная позиция: дальняя т6 совпадает с т6расч
            Point22 = Point21;
            TargetLineSteep = TargetLineCorrected;
            Point63 = Point61;
            // Определение входимости т3 в диапазон возможных экстремумов
            P3Bar = Convert.ToInt32(Point3.Bar);
            if (ModelDir == TDirection.dtUp)
            {
                if (Quotes.GetBarByIndex(P3Bar).Open < Quotes.GetBarByIndex(P3Bar).Close) P3Bar--;
            }
            else
            {
                if (Quotes.GetBarByIndex(P3Bar).Open > Quotes.GetBarByIndex(P3Bar).Close) P3Bar--;
            }
            // Проверка на лишние точки на ЛЦ'
            if (!AreExtraPoints(ModelDir, TargetLineCorrected, Convert.ToInt32(Point2.Bar), Convert.ToInt32(Point4.Bar)))
            {   //  Проверка последующих экстремумов от т2
                for (int i = Convert.ToInt32(Point21.Bar) + 1; i <= P3Bar; i++)
                {
                    // Проверка бара на экстремальность
                    Dir = Quotes.IsExtremum(i,1,TDirection.dtUnKnown);
                    if (Dir == TDirection.dtDnEqual) Dir = TDirection.dtDn;
                    if (Dir == TDirection.dtUpEqual) Dir = TDirection.dtUp;
                    if (Dir == TDirection.dtUpDn) Dir = ModelDir;
                    if (ModelDir == Dir)
                    {
                        P2.Bar = i;
                        P2.Price = Dir == TDirection.dtDn ? Quotes.GetBarByIndex(i).Low : Quotes.GetBarByIndex(i).High;
                        TgL = Common.CalcLine(P2,Point4);
                        if (!AreExtraPoints(ModelDir, TgL, i, Convert.ToInt32(Point4.Bar)))
                        {
                            Point22 = P2;
                            TargetLineSteep = TgL;
                            Point63 = Common.CrossLines(TargetLineSteep,TargetLineMA);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void CalcTargets()
        {
           // const int TgL = 6;
            switch (ModelType)
            {
                case Common.TModelType.ModelOfExpansion:
                    {
                        Targets[0].Price = BreakTrendLine.Price + BreakTrendLineFirst.Price - Point6.Price;
                        Targets[0].Bar = BreakTrendLine.Bar;// +TgL * (BreakTrendLine.Bar - Point1.Bar);
                        Targets[0].xb = BreakTrendLine.iBar + (BreakTrendLineFirst.iBar - Point6.iBar);
                        Targets[1].Price = Point1.Price;
                        Targets[1].Bar = BreakTrendLine.Bar;// +TgL * (BreakTrendLine.Bar - Point1.Bar);
                        Targets[1].xb = LifeTime;
                        Targets[2].Price = 2 * Point1.Price - Point4.Price;
                        Targets[2].Bar = BreakTrendLine.Bar;// +TgL * (BreakTrendLine.Bar - Point1.Bar);
                        Targets[2].xb = LifeTime;

/*                        if (!BreakTargetLineFirst.pt.IsEmpty)
                        {
                            Targets[3].Price = 2 * BreakTargetLineFirst.Price - Point3.Price;
                            Targets[3].Bar = BreakTargetLineFirst.Bar;

                            Targets[4].Price = 2 * BreakTargetLineFirst.Price - Point1.Price;
                            Targets[4].Bar = BreakTargetLineFirst.Bar;

                            Targets[5].Price = 2 * BreakTargetLineFirst.Price - PointSP2.Price;
                            Targets[5].Bar = BreakTargetLineFirst.Bar;
                        }*/
                        break;
                    }
                case Common.TModelType.ModelOfBalance:
                    {
                        Targets[0].Price = BreakTrendLine.Price + BreakTrendLineFirst.Price - Point6.Price;
                        Targets[0].Bar = BreakTrendLine.Bar;// +TgL * (BreakTrendLine.Bar - Point1.Bar);
                        Targets[0].xb = BreakTrendLine.iBar + (BreakTrendLineFirst.iBar - Point6.iBar);
                        Targets[1].Price = Point1.Price;
                        Targets[1].Bar = BreakTrendLine.Bar;// +TgL * (BreakTrendLine.Bar - Point1.Bar);
                        Targets[1].xb = LifeTime;
                        Targets[2].Price = 2 * Point1.Price - Point4.Price;
                        Targets[2].Bar = BreakTrendLine.Bar;// +TgL * (BreakTrendLine.Bar - Point1.Bar);
                        Targets[2].xb = LifeTime;
                        break;
                    }
                case Common.TModelType.ModelOfAttraction:
                    {
                        Targets[0].Price = 2 * Point1.Price - PointSP.Price;
                        Targets[0].Bar = PointSP.Bar;// +TgL * (PointSP.Bar - Point1.Bar);
                        Targets[0].xb = LifeTime;
                        Targets[1].Price = PointSP.Price;
                        Targets[1].Bar = PointSP.Bar;// +TgL * (PointSP.Bar - Point1.Bar);
                        Targets[1].xb = LifeTime;
                        break;
                    }
            }
        }

        private bool AreExtraPoints(TDirection Dir, TLine Line, int First, int Last)    //! Проверка на лишние точки на линии Line от First до Last
        {
            bool R = false;
            double Value;
            switch (Dir)
            {
                case TDirection.dtUp:
                    {
                        for (int i = First+1; i < Last; i++)
                        {
                            Value = Common.CalcLineValue(Line, i);
                            if (Value < Quotes.GetBarByIndex(i).High)
                            {
                                R = true;
                                break;
                            }
                        }
                    }
                    break;
                case TDirection.dtDn:
                    {
                        for (int i = First+1; i < Last; i++)
                        {
                            Value = Common.CalcLineValue(Line, i);
                            if (Value > Quotes.GetBarByIndex(i).Low)
                            {
                                R = true;
                                break;
                            }
                        }
                    }
                    break;
            }
            return R;
        }

        public int IsModelCorrect() //! Вычисление вектора ошибок построения модели
        {
            int I;
            int FB;
            int BP4;
            int BP5;
            int BP6;
            TDirection Dir4;
            Common.TModelType Tp1;
            Common.TModelType Tp2;

            ErrorCode = 0;
            SV1 = Quotes.GetBarByIndex(Convert.ToInt32(Point1.Bar));
            SV3 = Quotes.GetBarByIndex(Convert.ToInt32(Point3.Bar));
            SV2 = Quotes.GetBarByIndex(Convert.ToInt32(Point2.Bar));
            SV4 = Quotes.GetBarByIndex(Convert.ToInt32(Point4.Bar));
            SV5 = Quotes.GetBarByIndex(Convert.ToInt32(Point5.Bar));
            BP4 = Convert.ToInt32(Point4.Bar);
            BP5 = Convert.ToInt32(Point5.Bar);
            BP6 = Convert.ToInt32(Point6.Bar);

            // Инициализация точек для построения ЛЦ и ЛТ, охватывающих тренд
//            TargetLines[4] = TargetLineCorrected;
//            TrendLines[1] = TrendLine;
//            Points4[1] = Point4;    // т4 для касательной ЛЦ
            if (ModelType != Common.TModelType.ModelOfAttraction)
            {
                Points2[3] = Point21;   // т2 для касательной ЛЦ
                PointSP2 = PointSP1;    // СТ по касательным ЛЦ и ЛТ
            }
            else
            {
                Points2[3] = Point21;    // т2 как экстремум
                PointSP2 = PointSP1;     // СТ по ЛЦ и ЛТ через экстремумы
            }
//            Points1[1] = Point1;        // т1 для касательной ЛТ
//            Points3[2] = Point3;        // т3 для касательной ЛТ

            // Точка 5 внутри бара точки 4
            if ((Point5.Bar > Point4.Bar) &&
                (Point5.Price > SV4.Low) &&
                (Point5.Price < SV4.High))
            {
                ErrorCode = ErrorCode + 128;
            }
            // Точка 5 внутри базы
            if ((Point5.Bar > Point4.Bar) &&
                (Point5.Price >= Math.Min(Point2.Price, Point3.Price)) &&
                (Point5.Price <= Math.Max(Point2.Price, Point3.Price)))
            {
                ErrorCode = ErrorCode + 16384;
            }
            // Проверка на положение т2' относительно т3
            if (((ModelType == Common.TModelType.ModelOfExpansion) || (ModelType == Common.TModelType.ModelOfBalance)) && 
                ((ModelDir == TDirection.dtUp) && (Point21.Price < Point3.Price)) ||
                ((ModelDir == TDirection.dtDn) && (Point21.Price > Point3.Price)))
           {
                ErrorCode = ErrorCode | 0x00040000;
            }
            // Проверка на взаимное расположение СТ по экстремумам и СТ по касательной ЛЦ
            if (((PointSP.Bar > Point4.Bar) && (PointSP2.Bar < Point4.Bar)) ||
                ((PointSP.Bar < Point4.Bar) && (PointSP2.Bar > Point4.Bar)))
            {
                ErrorCode = ErrorCode | 1;
                return ErrorCode;
            }
            // Проверка на неизменность типа подели по касательным и через экстремумы
            CalcModelType(false); Tp1 = ModelType;
            CalcModelType(true); Tp2 = ModelType;
            if (Tp1 != Tp2)
            {
                ErrorCode = ErrorCode | 1;
                return ErrorCode;
            }
            // Проверка положения т5
            if ((Point4.Bar == Point5.Bar) && (Point4.Price == Point5.Price) && (Point4.Bar < Point6.Bar))
            {
                ErrorCode = ErrorCode + 0x00000001;
                return ErrorCode;
            }
            // Соотношение Базы к Блоку Целей 
            if (!CheckRelationTargetBlock2Base(3))
            {
                ErrorCode = ErrorCode | 0x00100000;
            }
            // Соотношение Базы к Блоку Целей 
            if (!CheckRelationTargetBlock2Base(2))
            {
                ErrorCode = ErrorCode | 0x00200000;
            }
            // Разбег точек по экстремумам и через касательные
            if ((Point4.Bar - Point41.Bar) >= Options.TangentAndExtremumDiff)
                ErrorCode = ErrorCode | 0x00001000;
            switch (ModelDir)
            {
                case TDirection.dtUp:
                {
                    Dir4 = Quotes.IsExtremum(Convert.ToInt32(Point4.Bar), Options.Extremum4, TDirection.dtUp);
                    if ((Dir4 == TDirection.dtUpDn) || (Dir4 == TDirection.dtUpEqual)) Dir4 = TDirection.dtUp;
                    // Проверка взаимного расположения точек, по барам и ценам
                    if ((CurrentPoint >= 4) &&
                       ((Point1.Price < Point2.Price) &&
                        (Point3.Price < Point2.Price) &&
                        (Point1.Price <= Point3.Price) &&
                        (Point4.Price >= Point2.Price) &&
                        ((Point5.Price <= Point4.Price) || (BP5 == 0) || (BP4 == BP6) || (ModelType == Common.TModelType.ModelOfAttraction))) &&
                        (((Point1.Bar < Point2.Bar)/* || ((Point1.Bar == Point2.Bar) && (SV1.Open < SV1.Close))*/) &&
                         ((Point2.Bar < Point3.Bar) || ((Point2.Bar == Point3.Bar) && (SV3.Open > SV3.Close))) &&
                         (Point3.Bar < BP4) && (Point31.Bar <= Point41.Bar) &&
                         (((BP4 < BP6) && (BP4 < BP5) && (BP5 <= BP6)) || (BP4 == BP6) || (BP5 == 0) || (ModelType == Common.TModelType.ModelOfAttraction))) &&
                          ((Dir4 == TDirection.dtUp)))
                    {
                        ErrorCode = ErrorCode + 0x00000000;
                    }
                    else
                    {
                        ErrorCode = ErrorCode + 0x00000001;
                        return ErrorCode;
                    }
                    // проверка цвета свечи т2 для определения диапазона проверки глобальности экстремума т3
                    if (SV2.Open < SV2.Close)
                        FB = Convert.ToInt32(SV2.Bar) + 1;
                    else
                        FB = Convert.ToInt32(SV2.Bar);
                    // Проверка на лишнии точки на ЛТ от т1 до т4
                    // Проверка т3 на глобальность экстремума
                    for (I = Convert.ToInt32(Point1.Bar) + 1; I < Convert.ToInt32(Point3.Bar); I++)
                    {
                        SV = Quotes.GetBarByIndex(I);
                        if (((Quotes.GetPriceFromValue(SV.Low)+pip) < Quotes.GetPriceFromValue(Common.CalcLineValue(TrendLine, SV.X))) &&
                            (I != Convert.ToInt32(Point3.Bar)) &&
                            ((ErrorCode & 0x00000002) == 0))
                        {   // Лишние точки на ЛТ
                            ErrorCode = ErrorCode | 0x00000002;
                        }
                        if ((I >= FB) && (SV.Low < Point3.Price))
                        {   // Глобальность экстемума т3
                            ErrorCode = ErrorCode | 0x00000040;
                        }
                    }
                    // Проверка на лишнии точки на ЛЦ's от т2' до т4
                    for (I = Convert.ToInt32(Point21.Bar) + 1; I < Convert.ToInt32(Point4.Bar); I++)
                    {
                        SV = Quotes.GetBarByIndex(I);
                        // на ЛЦ'
                        if (((Quotes.GetPriceFromValue(SV.High)-pip) > Quotes.GetPriceFromValue(Common.CalcLineValue(TargetLineCorrected, SV.X))))
                        {
                            ErrorCode = ErrorCode | 4;
                        }
                        // на ЛЦ
                        if ((I > Point2.Bar) && ((Quotes.GetPriceFromValue(SV.High)-pip) > Quotes.GetPriceFromValue(Common.CalcLineValue(TargetLine, SV.X))))
                        {
                            ErrorCode = ErrorCode | 0x00010000;
                        }
                        // на ЛЦ''
                        if ((I > Point22.Bar) && ((Quotes.GetPriceFromValue(SV.High)-pip) > Quotes.GetPriceFromValue(Common.CalcLineValue(TargetLineSteep, SV.X))))
                        {
                            ErrorCode = ErrorCode | 0x00020000;
                        }
                    }
                    // Проверка правил, зависящих от типа модели
                    switch (ModelType)
                    {
                        case Common.TModelType.ModelOfAttraction:
                        {
                            // Пересечение тел баров 1 и 3 точек
                            if (Math.Max(SV1.Open, SV1.Close) > Math.Min(SV3.Open, SV3.Close))
                            {
                                ErrorCode = ErrorCode | 8;
                            }
                            // Пересечение тел баров 2 и 4 точек
                            if (Math.Max(SV2.Open, SV2.Close) > Math.Min(SV4.Open, SV4.Close))
                            {
                                ErrorCode = ErrorCode | 16;
                            }
                            // Пересечение тел баров 2 и 5 точек
                            
                            if ((SV5.Bar == 0) && (BreakTrendLineFirst.Bar > 0))
                            {
                                SV5 = Quotes.GetBarByIndex(Convert.ToInt32(BreakTrendLineFirst.Bar)); 
                            }


                            if ((Math.Max(SV2.Open, SV2.Close) > Math.Min(SV5.Open, SV5.Close)) &&
                                (SV5.Bar > 0))
                            {
                                ErrorCode = ErrorCode | 0x00000020;
                            }

                            // Соотношение 1-2 к 3-4
                            if (((Point2.Price - Point1.Price) > Options.MACoef * (Point4.Price - Point3.Price))
                                 /*&& ((Point2.Bar - Point1.Bar) > Options.MACoef * (Point4.Bar - Point3.Bar))*/)
                            {
                                ErrorCode = ErrorCode | 0x00080000;
                            }
                            break;
                        }
                        case Common.TModelType.ModelOfExpansion:
                            goto case Common.TModelType.ModelOfBalance;
                        case Common.TModelType.ModelOfBalance:
                        {
                            // Точка 3' вне базы
                            if (((Point31.Bar < Point3.Bar) ||
                                (Point31.Bar > Point4.Bar) ||
                                (Point31.Price < Math.Min(Point2.Price, Point1.Price)) ||
                                (Point31.Price > Math.Max(Point2.Price, Point1.Price))) &&
                                (Point6.Bar > Point4.Bar))
                            {
                                ErrorCode = ErrorCode | 256;
                            }
                            // ЛЦ пересекает бар точки 1
                            if ((Common.CalcLineValue(TargetLineCorrected, Point1.Bar) >= SV1.Low) &&
                                (Common.CalcLineValue(TargetLineCorrected, Point1.Bar) <= SV1.High))
                            {
                                ErrorCode = ErrorCode | 512;
                            }
                            // Пересечение тел баров 2 и 5 точек
                            if ((Math.Max(SV2.Open, SV2.Close) > Math.Min(SV5.Open, SV5.Close)) &&
                                (Point6.Bar > Point4.Bar))
                            {
                                ErrorCode = ErrorCode | 32;
                            }
                            break;
                        }
                    }
                    break;
                }
                case TDirection.dtDn:
                {
                    Dir4 = Quotes.IsExtremum(Convert.ToInt32(Point4.Bar), Options.Extremum4, TDirection.dtDn);
                    if ((Dir4 == TDirection.dtUpDn) || (Dir4 == TDirection.dtDnEqual)) Dir4 = TDirection.dtDn;
                    // Проверка взаимного расположения точек
                    if ((CurrentPoint >= 4) &&
                        ((Point1.Price > Point2.Price) &&
                        (Point3.Price > Point2.Price) &&
                        (Point1.Price >= Point3.Price) &&
                        (Point4.Price <= Point2.Price) &&
                        ((Point5.Price >= Point4.Price) || (BP5 == 0) || (BP4 == BP6) || (ModelType == Common.TModelType.ModelOfAttraction))) &&
                        (((Point1.Bar < Point2.Bar)/* || ((Point1.Bar == Point2.Bar) && (SV1.Open > SV1.Close))*/) &&
                         ((Point2.Bar < Point3.Bar) || ((Point2.Bar == Point3.Bar) && (SV3.Open < SV3.Close))) &&
                        (Point3.Bar < BP4) && (Point31.Bar <= Point41.Bar) && 
                        (((BP4 < BP6) && (BP4 < BP5) && (BP5 <= BP6)) || (BP4 == BP6) || (BP5 == 0) || (ModelType == Common.TModelType.ModelOfAttraction))) &&
                        (Dir4 == TDirection.dtDn))
                    {
                        ErrorCode = ErrorCode + 0;
                    }
                    else
                    {
                        ErrorCode = ErrorCode | 1;
                        return ErrorCode;
                    }
                    // Проверка на лишнии точки на ЛТ от т1 до т4
                    // Проверка т3 на глобальность экстремума
                    // проверка цвета свечи т2 для определения диапазона проверки глобальности экстремума т3
                    if (SV2.Open > SV2.Close)
                        FB = Convert.ToInt32(SV2.Bar) + 1;
                    else
                        FB = Convert.ToInt32(SV2.Bar);
                    for (I = Convert.ToInt32(Point1.Bar) + 1; I < Convert.ToInt32(Point3.Bar); I++)
                    {
                        SV = Quotes.GetBarByIndex(I);
                        if (((Quotes.GetPriceFromValue(SV.High)-pip) > Quotes.GetPriceFromValue(Common.CalcLineValue(TrendLine, SV.X))) &&
                            (I != Convert.ToInt32(Point3.Bar)) &&
                            ((ErrorCode & 0x00000002) == 0))
                        {
                            ErrorCode = ErrorCode | 0x00000002;
                        }
                        if ((I >= FB) && (SV.High > Point3.Price))
                        {
                            ErrorCode = ErrorCode | 0x00000040;
                        }
                    }
                    // Проверка на лишнии точки на ЛЦ's от т2' до т4
                    for (I = Convert.ToInt32(Point21.Bar) + 1; I < Convert.ToInt32(Point4.Bar); I++)
                    {
                        SV = Quotes.GetBarByIndex(I);
                        if ((Quotes.GetPriceFromValue(SV.Low)+pip) < Quotes.GetPriceFromValue(Common.CalcLineValue(TargetLineCorrected, SV.X)))
                            {
                                ErrorCode = ErrorCode | 4;
                            }
                        if ((I > Point2.Bar) && ((Quotes.GetPriceFromValue(SV.Low)+pip) < Quotes.GetPriceFromValue(Common.CalcLineValue(TargetLine, SV.X))))
                        {
                            ErrorCode = ErrorCode | 0x00010000;
                        }
                        if ((I > Point22.Bar) && ((Quotes.GetPriceFromValue(SV.Low)+pip) < Quotes.GetPriceFromValue(Common.CalcLineValue(TargetLineSteep, SV.X))))
                        {
                            ErrorCode = ErrorCode | 0x00020000;
                        }
                    }
                    // Проверка правил, зависящих от типа модели
                    switch (ModelType)
                    {
                        case Common.TModelType.ModelOfAttraction:
                        {
                            // Пересечение тел баров 1 и 3 точек
                            if (Math.Min(SV1.Open, SV1.Close) < Math.Max(SV3.Open, SV3.Close))
                            {
                                ErrorCode = ErrorCode | 8;
                            }
                            // Пересечение тел баров 2 и 4 точек
                            if (Math.Min(SV2.Open, SV2.Close) < Math.Max(SV4.Open, SV4.Close))
                            {
                                ErrorCode = ErrorCode | 16;
                            }
                            // Пересечение тел баров 2 и 5 точек

                            if ((SV5.Bar == 0) && (BreakTrendLineFirst.Bar > 0))
                            {
                                SV5 = Quotes.GetBarByIndex(Convert.ToInt32(BreakTrendLineFirst.Bar));
                            }

                            if ((Math.Min(SV2.Open, SV2.Close) < Math.Max(SV5.Open, SV5.Close)) &&
                                (SV5.Bar > 0))
                            {
                                ErrorCode = ErrorCode | 0x00000020;
                            }
                            // Соотношение 1-2 к 3-4
                            if ((Point1.Price - Point2.Price) > Options.MACoef * (Point3.Price - Point4.Price))
                            {
                                ErrorCode = ErrorCode | 0x00080000;
                            }
                            break;
                        }
                        case Common.TModelType.ModelOfExpansion:
                            goto case Common.TModelType.ModelOfBalance;
                        case Common.TModelType.ModelOfBalance:
                        {
                            // Точка 3' вне базы
                            if (((Point31.Bar < Point3.Bar) ||
                                (Point31.Bar > Point4.Bar) ||
                                (Point31.Price < Math.Min(Point2.Price, Point1.Price)) ||
                                (Point31.Price > Math.Max(Point2.Price, Point1.Price))) && 
                                (Point6.Bar > Point4.Bar))
                            {
                                ErrorCode = ErrorCode | 256;
                            }
                            // ЛЦ пересекает бар точки 1
                            if ((Common.CalcLineValue(TargetLineCorrected, Point1.Bar) >= SV1.Low) &&
                                (Common.CalcLineValue(TargetLineCorrected, Point1.Bar) <= SV1.High))
                            {
                                ErrorCode = ErrorCode | 512;
                            }
                            // Пересечение тел баров 2 и 5 точек
                            if ((Math.Min(SV2.Open, SV2.Close) < Math.Max(SV5.Open, SV5.Close)) && (Point6.Bar > Point4.Bar))
                            {
                                ErrorCode = ErrorCode | 32;
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            // Вычисление СТ по ЛЦ и ЛТ по касательным
            if (ModelType != Common.TModelType.ModelOfAttraction)
            {
                PointSP2 = Common.CrossLines(TargetLineTangent, TrendLineTangent);
                if (Point6.Bar > Point4.Bar)
                    CalcPoint6(true);
//                    Point61 = Common.CrossLines(TargetLineTangent, TargetLineMA);
            }
            return ErrorCode;
        }

        public void CalcTangentLines()  //! Построение касательных ЛЦ и ЛТ, охватывающих все движение от т1 до т4
        {
            //Double DistMax;
            //Double Dist;
           // TLine Line1To6;
            int P1;
            // Инициализация точек для построения ЛЦ и ЛТ, охватывающих тренд
            TargetLines[4] = TargetLineCorrected;
            TrendLines[1] = TrendLine;
            Points4[1] = Point4;    // т4 для касательной ЛЦ
            P1 = Convert.ToInt32(Point1.Bar) + 1;
            if (ModelType != Common.TModelType.ModelOfAttraction)
            {
                Points2[3] = Point21;   // т2 для касательной ЛЦ
                PointSP2 = PointSP1;    // СТ по касательным ЛЦ и ЛТ
            }
            else
            {
                Points2[3] = Point21;   // т2 как экстремум
//                PointSP2 = PointSP1;    // СТ по ЛЦ и ЛТ через экстремумы
            }
            Points1[1] = Point1;    // т1 для касательной ЛТ
            Points3[2] = Point3;    // т3 для касательной ЛТ
            switch (ModelDir)
            {
                case TDirection.dtUp:
                {
                    for (int I = Convert.ToInt32(Point1.Bar) + 1; I < Convert.ToInt32(Point4.Bar); I++)
                    {
                        SV = Quotes.GetBarByIndex(I);
                        // Вычисление точек т1 и т3 для построения касательной ЛТ
                        if (SV.Low <= Common.CalcLineValue(TrendLineTangent, SV.X))
                        {
                            if (I > Point21.Bar)
                            {   // Вычисление т3
                                Points3[2].Bar = SV.Bar;
                                Points3[2].DT = SV.DT;
                                Points3[2].Price = SV.Low;
                            }
                            else
                            {   // Вычисление т1
                                Points1[1].Bar = SV.Bar;
                                Points1[1].DT = SV.DT;
                                Points1[1].Price = SV.Low;
                            }
                            TrendLines[1] = Common.CalcLine(Point11, Point32);
                        }
                    }
                    for (int I = Convert.ToInt32(Point21.Bar) + 1; I < Convert.ToInt32(Point4.Bar); I++)
                    {
                        SV = Quotes.GetBarByIndex(I);
                        // Определение т4 по касательной
                        if (SV.High >= Common.CalcLineValue(TargetLineTangent, SV.X))
                        {
                            Points4[1].Bar = SV.Bar;
                            Points4[1].DT = SV.DT;
                            Points4[1].Price = SV.High;
                            TargetLines[4] = Common.CalcLine(Point21, Point41);
                        }
                    }
                    // Определение т2 для касательной ЛЦ
                    if (Quotes[Convert.ToInt32(Point1.Bar)].Open < Quotes[Convert.ToInt32(Point1.Bar)].Close)
                        P1--;
                    for (int I = P1; I < Convert.ToInt32(Point21.Bar); I++)
                    {
                        SV = Quotes.GetBarByIndex(I);
                        if (SV.High >= Common.CalcLineValue(TargetLineTangent, SV.X))
                        {
                            Points2[3].Bar = SV.Bar;
                            Points2[3].DT = SV.DT;
                            Points2[3].Price = SV.High;
                            TargetLines[4] = Common.CalcLine(Point23, Point41);
                        }
                   }
                    break;
                }
                case TDirection.dtDn:
                {
//                    Line1To6 = Common.CalcLine(Point1,Point6);
//                    DistMax = Common.DistancePointToLine(Point2, Line1To6);
                    for (int I = Convert.ToInt32(Point1.Bar) + 1; I < Convert.ToInt32(Point4.Bar); I++)
                    {
                        SV = Quotes.GetBarByIndex(I);
                        // Вычисление точек т1 и т3 для построения касательной ЛТ
                        if (SV.High >= Common.CalcLineValue(TrendLineTangent, SV.X))
                        {
                            if (I > Point21.Bar)
                            {   // Вычисление т3
                                Points3[2].Bar = SV.Bar;
                                Points3[2].DT = SV.DT;
                                Points3[2].Price = SV.High;
                            }
                            else
                            {   // Вычисление т1
                                Points1[1].Bar = SV.Bar;
                                Points1[1].DT = SV.DT;
                                Points1[1].Price = SV.High;
                            }
                            TrendLines[1] = Common.CalcLine(Point11, Point32);
                        }
                    }
                    for (int I = Convert.ToInt32(Point21.Bar) + 1; I < Convert.ToInt32(Point4.Bar); I++)
                    {   
                        SV = Quotes.GetBarByIndex(I);
                        // Определение т4 по касательной
                        if (SV.Low <= Common.CalcLineValue(TargetLineTangent, SV.X))
                        {
                            Points4[1].Bar = SV.Bar;
                            Points4[1].DT = SV.DT;
                            Points4[1].Price = SV.Low;
                            TargetLines[4] = Common.CalcLine(Point21, Point41);
                        }
                    }
                    // Определение т2 для касательной ЛЦ
                    if (Quotes[Convert.ToInt32(Point1.Bar)].Open > Quotes[Convert.ToInt32(Point1.Bar)].Close)
                        P1--;
                    for (int I = P1; I < Convert.ToInt32(Point21.Bar); I++)
                    {
                        SV = Quotes.GetBarByIndex(I);
                        if (SV.Low <= Common.CalcLineValue(TargetLineTangent, SV.X))
                        {
                            Points2[3].Bar = SV.Bar;
                            Points2[3].DT = SV.DT;
                            Points2[3].Price = SV.Low;
                            TargetLines[4] = Common.CalcLine(Point23, Point41);
                        }
                    }
                }
                break;
            }
            // Вычисление СТ по ЛЦ и ЛТ по касательным
//            if (ModelType != Common.TModelType.ModelOfAttraction)
            {
                PointSP2 = Common.CrossLines(TargetLineTangent, TrendLineTangent);
                Point64 = Common.CrossLines(TargetLineTangent, TargetLineMA);
            }
            
            //  Переопределение типа модели
            CalcModelType(true);
            CalcPoint6(true);
            CalcTargets();
        }

        public override string ToString()    //! Сохранение модели в строке
        {
            return
                ModelID.ToString()+";"+
                CurrentPoint.ToString() + ";" +
                ProcessedBar.ToString() + ";" +
                Step.ToString() + ";" +
                ModelType.ToString() + ";" +
                ModelDir.ToString() + ";" +
                Point1.DT.ToString("dd.MM.yyyy HH:mm:ss") + ";" +
                Point1.Price.ToString() + ";" +
                Point2.DT.ToString("dd.MM.yyyy HH:mm:ss") + ";" +
                Point2.Price.ToString() + ";" +
                Point3.DT.ToString("dd.MM.yyyy HH:mm:ss") + ";" +
                Point3.Price.ToString() + ";" +
                Point4.DT.ToString("dd.MM.yyyy HH:mm:ss") + ";" +
                Point4.Price.ToString() + ";" +
                Point5.DT.ToString("dd.MM.yyyy HH:mm:ss") + ";" +
                Point5.Price.ToString() + ";" +
                Point6.DT.ToString("dd.MM.yyyy HH:mm:ss") + ";" +
                Point6.Price.ToString();
        }

        public string SaveModel()    //! Сохранение модели в строке
        {
            string S =
            Point1.ToString() + ";" + Point11.ToString() + ";" + Point2.ToString() + ";" +
            Point21.ToString() + ";" + Point22.ToString() + ";" + Point23.ToString() + ";" +
            Point3.ToString() + ";" + Point31.ToString() + ";" + Point32.ToString() + ";" +
            Point4.ToString() + ";" + Point41.ToString() + ";" + Point5.ToString() + ";" +
            Point6.ToString() + ";" + Point61.ToString() + ";" + Point62.ToString() + ";" +
            Point63.ToString() + ";" +Point64.ToString() + ";" + Point65.ToString() + ";" + 
            PointSP.ToString() + ";" +BreakTrendLineFirst.ToString() + ";" + BreakTrendLine.ToString() + ";" +
            CurrentPoint.ToString() + ";" + ProcessedBar.ToString() + ";" + Step.ToString() + ";" +
            ModelType.ToString() + ";" + ModelDir.ToString() + ";" + TimeFrame.ToString() + ";" +
            IDonBaseTF.ToString() + ";" + Stat.ToString() + ";" + TargetLineBreakOut.ToString() + ";" +
            HPBreakOut.ToString() + ";" + HPGetInside.ToString() + ";" + HPGetOutside.ToString() + ";" +
            HPreached.ToString() + ";" + HTi.ToString();
            return S;

        }


        public void FromString(string Str)  //! Загрузка модели из строки
        {
            TBar Bar;
            String[] PointsElements = Str.Split(';');
            ModelID = Convert.ToInt32(PointsElements[0]);
            CurrentPoint = Convert.ToInt32(PointsElements[1]);
            ProcessedBar = Convert.ToInt32(PointsElements[2]);
            Step = Convert.ToInt32(PointsElements[3]);
            if (Common.TModelType.ModelOfAttraction.ToString() == PointsElements[4]) ModelType = Common.TModelType.ModelOfAttraction;
            if (Common.TModelType.ModelOfBalance.ToString() == PointsElements[4]) ModelType = Common.TModelType.ModelOfBalance;
            if (Common.TModelType.ModelOfExpansion.ToString() == PointsElements[4]) ModelType = Common.TModelType.ModelOfExpansion;
            if (TDirection.dtDn.ToString() == PointsElements[5]) ModelDir = TDirection.dtDn;
            if (TDirection.dtUp.ToString() == PointsElements[5]) ModelDir = TDirection.dtUp;
            // Определение т1
            Points1[0].DT = DateTime.ParseExact(PointsElements[6], "dd.MM.yyyy HH:mm:ss", null);
            Bar = Quotes.GetBarByDT(Point1.DT);
            SetPointParams(Bar.Bar, Points1[0].DT, Convert.ToDouble(PointsElements[7]), 1);
            Points1[1] = Points1[0];
            // Определение т2
            Points2[0].DT = DateTime.ParseExact(PointsElements[8], "dd.MM.yyyy HH:mm:ss", null);
            Bar = Quotes.GetBarByDT(Point2.DT);
            SetPointParams(Bar.Bar, Points2[0].DT, Convert.ToDouble(PointsElements[9]), 2);
            Points2[1] = Points2[0];
            Points2[2] = Points2[0];
            Points2[3] = Points2[0];
            // Определение т3
            Points3[0].DT = DateTime.ParseExact(PointsElements[10], "dd.MM.yyyy HH:mm:ss", null);
            Bar = Quotes.GetBarByDT(Point3.DT);
            SetPointParams(Bar.Bar, Points3[0].DT, Convert.ToDouble(PointsElements[11]), 3);
            Points3[1] = Points3[0];
            Points3[2] = Points3[0];
            TrendLines[1] = TrendLine;
            // Определение т4
            Points4[0].DT = DateTime.ParseExact(PointsElements[12], "dd.MM.yyyy HH:mm:ss", null);
            Bar = Quotes.GetBarByDT(Point4.DT);
            SetPointParams(Bar.Bar, Points4[0].DT, Convert.ToDouble(PointsElements[13]), 4);
            Points4[1] = Points4[0];
            TargetLines[4] = TargetLineCorrected;
            // Определение т5
            Points5[0].DT = DateTime.ParseExact(PointsElements[14], "dd.MM.yyyy HH:mm:ss", null);
            Bar = Quotes.GetBarByDT(Point5.DT);
            if (Bar != null)
            {
                SetPointParams(Bar.Bar, Points5[0].DT, Convert.ToDouble(PointsElements[15]), 5);
                CalcPoint6(false);
            }
            // Определение т6
            Points6[0].DT = DateTime.ParseExact(PointsElements[16], "dd.MM.yyyy HH:mm:ss", null);
            Bar = Quotes.GetBarByDT(Point6.DT);
            SetPointParams(Bar.Bar, Points6[0].DT, Convert.ToDouble(PointsElements[17]), 6);
//            Points6[1] = Points6[0];
            Points6[2] = Points6[1];
        }
    }
    [System.Serializable]
    public class THTangentLine
    {
        TLine TangentLine;
        public int Step = 0;
        private TPoint[] Points = new TPoint[6];    //!< Список точек для построения ГЦ
        public TPoint PointJ
        {
            get { return Points[0]; }
            set { Points[0] = value; }
        }
        public TPoint PointF
        {
            get { return Points[1]; }
            set { Points[1] = value; }
        }
        public TPoint PointE
        {
            get { return Points[2]; }
            set { Points[2] = value; }
        }
        public TPoint PointK
        {
            get { return Points[3]; }
            set { Points[3] = value; }
        }
        public TPoint PointG
        {
            get { return Points[4]; }
            set { Points[4] = value; }
        }
        public TPoint PointI
        {
            get { return Points[5]; }
            set { Points[5] = value; }
        }

        public THTangentLine() { }
        public THTangentLine(TQuotes Quotes, TModel M1, TModel M2)  //!< Создание ГЦ для модели M2 от т4 M1
        {
            int SB;
            int EB;
            TBar SV;
            // Инициализация точек
            TangentLine = new TLine();
            PointJ = M2.Point1;
            PointE = M2.Point3;
            PointG = M1.Point4;
            // PointI
            PointI = new TPoint(PointG);
            // Если M1 без т6 и т1 M2 совпадает с т4 M1, то для точки I берется предыдущий бар т4 M1
            if ((M1.Point4.Bar == M1.Point6.Bar) && (M1.Point4.Bar == M2.Point1.Bar))
            {
                Points[5].Bar = PointI.Bar - 1; 
            }
            Points[5].Price = Common.CalcLineValue(M2.TrendLine, PointI.Bar);
            // Расчет PointF
            // Проверка цвета свечи т1 у M2 и установка первого бара для построения касательной к цене
            SV = Quotes.GetBarByIndex(Convert.ToInt32(M2.Point1.Bar));
            if (((SV.Open > SV.Close) && (M2.ModelDir == TDirection.dtDn)) ||
                ((SV.Open < SV.Close) && (M2.ModelDir == TDirection.dtUp)))
                SB = Convert.ToInt32(M2.Point1.Bar);
            else
                SB = Convert.ToInt32(M2.Point1.Bar) + 1;
            SV = Quotes.GetBarByIndex(SB);
            Points[1].Bar = SV.Bar;
            Points[1].DT = SV.DT;
            if (M2.ModelDir == TDirection.dtUp)
            {
                Points[1].Price = SV.High;
            }
            else
            {
                Points[1].Price = SV.Low;
            }
            TangentLine = Common.CalcLine(PointI, PointF);
            // Проверка цвета свечи т3 у M2 и установка последнего бара для построения касательной к цене
            SV = Quotes.GetBarByIndex(Convert.ToInt32(M2.Point3.Bar));
            if (((SV.Open > SV.Close) && (M2.ModelDir == TDirection.dtDn)) ||
                ((SV.Open < SV.Close) && (M2.ModelDir == TDirection.dtUp)))
                EB = Convert.ToInt32(M2.Point3.Bar)-1;
            else
                EB = Convert.ToInt32(M2.Point3.Bar);
            for (int i = SB+1; i <= EB; i++)
            {
                SV = Quotes.GetBarByIndex(i);
                if ((M2.ModelDir == TDirection.dtUp) && (SV.High > Common.CalcLineValue(TangentLine, SV.Bar)))
                {
                    Points[1].Bar = SV.Bar;
                    Points[1].DT = SV.DT;
                    Points[1].Price = SV.High;
                    TangentLine = Common.CalcLine(PointI, PointF);
                }
                if ((M2.ModelDir == TDirection.dtDn) && (SV.Low < Common.CalcLineValue(TangentLine, SV.Bar)))
                {
                    Points[1].Bar = SV.Bar;
                    Points[1].DT = SV.DT;
                    Points[1].Price = SV.Low;
                    TangentLine = Common.CalcLine(PointI, PointF);
                }
            }
        }

    }
}
        