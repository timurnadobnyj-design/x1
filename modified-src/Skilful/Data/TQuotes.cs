//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Andrey Prokhorov (AVP), Andrey Zyablitsev (skat)
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
using System.Windows.Forms;
using Skilful.Data;
using ChartV2;

namespace Skilful.QuotesManager
{
    public enum TDirection : int //!< Типы экстремумов
    {
        dtDn,         //!< Экстремум вниз
        dtUp,         //!< Экстремум вверх
        dtUnKnown,    //!< Бар не является экстремумом
        dtDnEqual,    //!< Бар входит в состав экстремума вниз, состоящего из баров с одинаковыми Low
        dtUpEqual,     //!< Бар входит в состав экстремума вверх, состоящего из баров с одинаковыми High
        dtUpDn      //!< Бар является экстремумом как вверх так и вниз 
    }

    public static class Gen
    {
        /// <summary>
        /// увеличиваем дату-время на указанный интервал и позиционирование на начало фрейма
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static DateTime incTime(DateTime dt, TF tf)
        {
            switch (tf)
            {
                case TF.custom: dt = dt.AddHours(1); break;
                case TF.m60: dt = dt.AddHours(1); break;
                case TF.m240: dt = dt.AddHours(4); break;
                case TF.Day: dt = dt.AddDays(1); break;
                case TF.Week: dt = dt.AddDays(7); break;
                case TF.Month: dt = dt.AddMonths(1); break;
                case TF.Quarter: dt = dt.AddMonths(3); break;
                case TF.Year: dt = dt.AddYears(1); break;
            }
            //if (sign == 1 && dt.DayOfWeek == DayOfWeek.Saturday) dt = dt.AddDays(2 * sign);
            //if (sign ==-1 && dt.DayOfWeek == DayOfWeek.Sunday) dt = dt.AddDays(2 * sign);
            return dt;
        }
        /// <summary>
        /// увеличиваем дату-время на указанный интервал и позиционирование на начало фрейма
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static DateTime incTimeFloor(DateTime dt, TF tf)
        {
            switch (tf)
            {
                case TF.m60: dt = dt.AddHours(1).AddMinutes(-dt.Minute); break;
                case TF.m240: dt = dt.AddHours(4 - dt.Hour%4).AddMinutes(-dt.Minute); break;
                case TF.Day: dt = dt.AddDays(1).AddHours(-dt.Hour).AddMinutes(-dt.Minute); break;
                case TF.Week: dt = dt.AddDays(7-(int)dt.DayOfWeek).AddHours(-dt.Hour).AddMinutes(-dt.Minute); break;
                case TF.Month: dt = dt.AddMonths(1).AddDays(1-dt.Day).AddHours(-dt.Hour).AddMinutes(-dt.Minute); break;
                case TF.Quarter: dt = dt.AddMonths(3 - (dt.Month-1) % 3 ).AddDays(1-dt.Day).AddHours(-dt.Hour).AddMinutes(-dt.Minute); break;
                case TF.Year: dt = dt.AddYears(1).AddMonths(1-dt.Month).AddDays(1-dt.Day).AddHours(-dt.Hour).AddMinutes(-dt.Minute); break;
            }
            return dt;
        }
        public static DateTime decTime(DateTime dt, TF tf)
        {
            switch (tf)
            {
                case TF.m60: dt = dt.AddHours(-1); break;
                case TF.m240: dt = dt.AddHours(-4); break;
                case TF.Day: dt = dt.AddDays(-1); break;
                case TF.Week: dt = dt.AddDays(-7); break;
                case TF.Month: dt = dt.AddMonths(-1); break;
                case TF.Quarter: dt = dt.AddMonths(-3); break;
                case TF.Year: dt = dt.AddYears(-1); break;
            }
            //if (sign == 1 && dt.DayOfWeek == DayOfWeek.Saturday) dt = dt.AddDays(2 * sign);
            //if (sign ==-1 && dt.DayOfWeek == DayOfWeek.Sunday) dt = dt.AddDays(2 * sign);
            return dt;
        }
        public static int tf2i(int tf)
        {
            switch ((TF)tf)
            {
                case TF.m60: return 60;
                case TF.m240: return 240;
                case TF.Day: return 1440;
                case TF.Week: return 10080;
                case TF.Month: return 43200;
                case TF.Quarter: return 129600;
                case TF.Year: return 525600;
                default: return 60;
            }
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
    ///<summary>
    /// Класс хранит массив которивок для заданного фрейма
    /// также массивы моделей, протоформ и некоторые дополнительные данные
    ///</summary>
    public class TQuotes
    {
        private TSymbol symbol; //Указатель на класс владелец для получния данных по символу
        TF frame;
        public int customTF;
        public TF tf{
            get { return frame; }
            set { frame = value; }
        }
        ChartType ctype;
        public ChartType chartType{
            get { return ctype; }
        }
        public bool log{
            get{
                if (symbol == null) return GlobalMembersTrend.log;
                switch (symbol.priceScale)
                {
                    case PriceScaleType.mixed:     return (frame >= TF.Day);
                    case PriceScaleType.linear:    return false;
                    case PriceScaleType.log10:     return true;
                    default: return false;
                }
            }
        }
        public int Decimals 
        {
            get
            {
                return (symbol == null) ? trend.Decimals : Symbol.Decimals;
            }
        }
        public TSymbol Symbol
        {
            get { return symbol; }
        }
        public int lastindex
        {
            get { return Quotes.Count-1; }
        }
        public List<TBar> Quotes = new List<TBar>();      //!< массив котировок
        public Trend trend;                               //!< доступ к массивам mesh, map
        public ModelManager.ModelsManager MM = null;             //!< доступ к массиву моделей  

        public TQuotes(TSymbol sym, TF tf, ChartType type, int cnt)    //! Конструктор
        {
            symbol = sym;
            frame = tf;
            ctype = type;
            Quotes.Capacity = cnt;
        }
        public TQuotes(TSymbol sym, TF tf, ChartType type)    //! Конструктор
        {
            symbol = sym;
            ctype = type;
            frame = tf;
            
        }
        public TQuotes()    //! Конструктор
        {
            symbol = null;
            frame = TF.m60;
            ctype = ChartType.Candle;
        }
        public TBar this[int i]  //! Получение бара по его индексу
        {
            get{
                return Quotes[i];
            }
            set{
                Quotes[i] = value;
            }
        }
        public void Add(TBar bar)  //! Добавление бара в массив
        {
            Quotes.Add(bar);
        }
        public TQuotes SwitchTo(TF frame)
        {
            return SwitchTo(frame, ctype);
        }
        public TQuotes SwitchTo(ChartType ctype)
        {
            return SwitchTo(frame, ctype);
        }
        public TQuotes SwitchTo(TF frame, ChartType ctype)
        {
            return symbol.GetQuotes(frame, ctype);
        }

        public int calcFrame()
        {
            if ((Quotes != null) && (Quotes.Count > 1))
            {
                TimeSpan fr = Quotes[1].DT - Quotes[0].DT;
                for (int i = 1; i < 7 && i < (Quotes.Count-1); i++)
                {
                    TimeSpan t = Quotes[i + 1].DT - Quotes[i].DT;
                    if (t < fr) fr = t;
                }
                customTF = (int) fr.TotalMinutes;
                return customTF;
            }
            return -1;
        }


        public virtual TBar GetBarByIndex(int Index)  //! Получение бара по его индексу
        {
            /* тут проверка диапазона...
             * на самом деле программа как бы не имеет права на подобного рода ошибки
             * и их возникновение может говорить об ошибках в алгоритмах
             * поэтому как минимум на время отладки эти проверки диапазона здесь лучше отключить
             * 
            if ((Index >= 0) && (Index < Quotes.Count))
            {
                return Quotes[Index];
            }
            return new TBar(); // вернем пустой бар в случае ошибки
             */
            return Quotes[Index];
        }

        public virtual void SetBarByIndex(TBar Bar, int Index)  //! Добавление/изменение бара в массив
        {
            if (Index >= GetCount())
            {
                Quotes.Add(Bar);
            }
            else
            {
                Quotes[Index] = Bar;
            }
        }

        public int TimeSpanToIndex(TimeSpan TS)
        {
            int weekEnds = TS.Days % 7 +1;
            int h =TS.Hours-48*weekEnds;
            switch(tf)
            {   
                case TF.custom:
                    if (customTF != 0)
                       return (int)(h * 60 / customTF);
                    MessageBox.Show("TQuotes: Ahtung! customTF=0", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                  
                    return 0;
                case TF.m60:
                case TF.m240:
                case TF.Day:
                   return (int)( h/TSymbol.TFtoHours[(int)tf]);
                case TF.Week:
                case TF.Month:
                case TF.Quarter:
                case TF.Year:
                   return (int) (TS.Hours/TSymbol.TFtoHours[(int)tf]);
                default: 
                    return 0;
            }
        }

             
        
        public virtual TBar GetBarByDT(DateTime DT) //! Получение бара по дате/времени
        {
            int B;
            int E;
            int C;
            TimeSpan DDT = DT - Quotes[0].DT;
            DateTime DTC;
            if (GetCount() == 0)
            {
                return null;
            }
            else
            {
/*  Что то здесь не то с TimeSpanToIndex. Тупое половинное деление надежнее :)
                B = 0;
                E = GetCount();
                C = TimeSpanToIndex(DDT);
                DTC = Quotes[C].DT;
                while ((DTC != DT) && ((E - B) > 1))
                {
                    if (DTC > DT)
                    {
                        E = C;
                        DDT = DTC - DT;
                        C -= TimeSpanToIndex(DDT);
                    }
                    if (DTC < DT)
                    {
                        B = C;
                        DDT = DT - DTC;
                        C += TimeSpanToIndex(DDT);
                    }
                    //C = (E + B) / 2;
                    DTC = Quotes[C].DT;
                }
                if (DTC != DT)
                    return null;
                else
                    return Quotes[C];
*/
                B = 0;
                E = GetCount();
                C = (E + B) / 2;
                DTC = Quotes[C].DT;
                while ((DTC != DT) && ((E - B) > 1))
                {
                    if (DTC > DT) E = C;
                    if (DTC < DT) B = C;
                    C = (E + B) / 2;
                    DTC = Quotes[C].DT;
                }
                if (DTC != DT)
                    return null;
                else
                    return Quotes[C];
            }
        }

        public virtual int GetCount()  //! Получение кол-ва баров в массиве исходных котировок
        {
            return Quotes.Count;
        }

        public virtual void SetCount(int Count)  //! Обрезка кол-ва баров в массиве исходных котировок
        {
            Quotes.RemoveRange(Count, Quotes.Count - Count);
        }
        public void Resize(int cnt)
        {
            Quotes.Clear();
            Quotes.Capacity = cnt;
        }
        private void CheckHigh(ref TDirection RHigh, TBar BarI, TBar BarCurrent) //! Используется в IsExtremum
        {
            if ((RHigh == TDirection.dtUp) || (RHigh == TDirection.dtUpEqual) || (RHigh == TDirection.dtUnKnown))
            {   // экстремум неизвестно какой или, возможно, Ап
                if (BarI.High == BarCurrent.High)
                {   // возможно Ап с равными Хаями
                    if (RHigh == TDirection.dtUnKnown) RHigh = TDirection.dtUpEqual;
                }
                else
                {
                    if (BarI.High > BarCurrent.High)
                    {   // Точно не Ап
                        RHigh = TDirection.dtDn;
                    }
                    else
                    {   // возможно Ап
                        if (RHigh == TDirection.dtUnKnown) RHigh = TDirection.dtUp;
                    }
                }
            }
        }
        void CheckLow(ref TDirection RLow, TBar BarI, TBar BarCurrent)  //! Используется в ISExtremum
        {
            if ((RLow == TDirection.dtDn) || (RLow == TDirection.dtDnEqual) || (RLow == TDirection.dtUnKnown))
            {   // экстремум неизвестно какой или, возможно, Даун
                if (BarI.Low == BarCurrent.Low)
                {   // возможно Даун с равными Хаями
                    if (RLow == TDirection.dtUnKnown) RLow = TDirection.dtDnEqual;
                }
                else
                {
                    if (BarI.Low < BarCurrent.Low)
                    {   // Точно не Даун
                        RLow = TDirection.dtUp;
                    }
                    else
                    {   // возможно Даун
                        if (RLow == TDirection.dtUnKnown) RLow = TDirection.dtDn;
                    }
                }
            }
        }
        public virtual TDirection IsExtremum(int Bar, int ASize, TDirection ADir)  //! Проверка бара на экстремальность
        {
           if (ASize <= GlobalMembersTAmodel.b_7 )
            {
                int hl;
                int bar37;
                int frep;
                int trndir;
                Quotes[Bar].getStatus(out hl,out bar37,out frep,out trndir);
                int high37 = (bar37 & 12) >> 2;//флаг для бара хай является ли он баром "3" или "7"
                int low37 = (bar37 & 3);//флаг для бара лоу является ли он баром "3" или "7"
                if ((high37 > 0) && (low37 == 0)) return TDirection.dtUp;
                if ((low37 > 0) && (high37 == 0)) return TDirection.dtDn;
                if ((high37 > 0) && (low37 > 0))
                {
                    if (ADir == TDirection.dtUp) return TDirection.dtUp;
                    if (ADir == TDirection.dtDn) return TDirection.dtDn;
                    return TDirection.dtUpDn;
                }
                 

                return TDirection.dtUnKnown;
            }
            else 
            {
                int F;
                int L;
                int I;
                int Size;
                TBar BarCurrent;
                TBar BarI;
                TDirection R = TDirection.dtUnKnown;
                TDirection RLow = TDirection.dtUnKnown;
                TDirection RHigh = TDirection.dtUnKnown;
                Size = ASize;
                    F = Bar - Size;
                BarCurrent = GetBarByIndex(Bar);
                while ((R == TDirection.dtUnKnown) && (F >= 0) && ((Size <= 7) || (Size <= ASize)))
                {
                    // Просмотр баров от Bar влево до Bar-Size
                    F = Math.Max(Bar - Size, 0);
                    I = Bar - 1;
                    while (I >= F)
                    {
                        BarI = GetBarByIndex(I);
                        // Проверка Хая
                        CheckHigh(ref RHigh, BarI, BarCurrent);
                        // Проверка Лоу
                        CheckLow(ref RLow, BarI, BarCurrent);
                        I--;
                    }
                    if (((RHigh == TDirection.dtUp) || (RHigh == TDirection.dtUpEqual)) && ((RLow == TDirection.dtUp) || (RLow == TDirection.dtUpEqual))) // Направление экстремума по Хаям и Лоу Вверх
                        R = RHigh;
                    else
                        if (((RLow == TDirection.dtDn) || (RLow == TDirection.dtDnEqual)) && ((RHigh == TDirection.dtDn) || (RHigh == TDirection.dtDnEqual))) // Направление экстремума по Лоу и Хаям Вниз
                            R = RLow;
                        else // Направление экстремума по Хаям и Лоу разное, решение невозможно
                            R = TDirection.dtUnKnown;
                    Size++;
                    F = Bar - Size;
                }
                // Просмотр баров от Bar вправо до Bar+Size
                Size = ASize;
                L = Bar + Size;
                L = Math.Min(Bar + Size, Quotes.Count - 1);
                I = Bar + 1;
                while (I <= L)
                {
                    BarI = GetBarByIndex(I);
                    // Проверка Хая
                    CheckHigh(ref RHigh, BarI, BarCurrent);
                    // Проверка Лоу
                    CheckLow(ref RLow, BarI, BarCurrent);
                    I++;
                }
                if (((RHigh == TDirection.dtUp) || (RHigh == TDirection.dtUpEqual)) &&
                   ((RLow == TDirection.dtUp) || (RLow == TDirection.dtUpEqual)) &&
                   ((R == TDirection.dtUp) || (R == TDirection.dtUpEqual) || (R == TDirection.dtUnKnown)))// Направление экстремума по Хаям и Лоу Вверх
                {
                    if ((RHigh == TDirection.dtUpEqual) && (R == TDirection.dtUpEqual))
                        R = TDirection.dtUpEqual;
                    else
                        R = TDirection.dtUp;
                }
                else
                    if (((RLow == TDirection.dtDn) || (RLow == TDirection.dtDnEqual)) &&
                       ((RHigh == TDirection.dtDn) || (RHigh == TDirection.dtDnEqual)) &&
                       ((R == TDirection.dtDn) || (R == TDirection.dtDnEqual) || (R == TDirection.dtUnKnown)))// Направление экстремума по Лоу и Хаям Вниз
                    {
                        if ((RLow == TDirection.dtDnEqual) && (R == TDirection.dtDnEqual))
                            R = TDirection.dtDnEqual;
                        else
                            R = TDirection.dtDn;
                    }
                    else // Направление экстремума по Хаям и Лоу разное, решение невозможно
                        R = TDirection.dtUnKnown;
                return R;
            }
        }

        public double GetPriceFromValue(double Value)  //! Расчет цены по заданному значению с учетом логарифмирования и кол-ва знаков после запятой
        {
            double R;
            // Проверка необходимости логарифмирования
            //if ((frame == TF.Day) || (frame == TF.Week) || (frame == TF.Month) || (frame == TF.Quarter) || (frame == TF.Year))
            if (log)
            {
                R = Math.Pow(10, Value);
            }
            else
            {
                R = Value;
            }
            // Округление до нужного числа знаков
            double Decimals = Symbol == null ? trend.Decimals : Symbol.Decimals;
            // GlobalMembersTAmodel.round(R, Decimals);
            R = Math.Truncate(Math.Pow(10, Decimals) * R);
            R = R / Math.Pow(10, Decimals);
            return R;
        }
    }
}
                                                                        