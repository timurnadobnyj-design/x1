//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Andrey Zyablitsev (skat)
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
using Skilful.Data;
using Skilful.QuotesManager;
using Skilful.ModelManager;
using ChartV2;

namespace Skilful.Statistics
{
    /// <summary>
    /// класс для хранения индексов баров соответствуюцих событиям необходимым для сбора статистики
    /// а также для различчных флагов
    /// </summary>
    public class Fstat
    {
        public
        int HP_get,         //Достижение НР
            HP_off,         //образованием противоположного 4 точке абс. экстремума после достижения НР
            HP_break,       //пробития НР (повторный подход к НР с предыдущим образованием противоположного 4 точке абс. экстремума)
            LTarget_break,  //пробития целевой
            LTrend_break,   //пробития трендовой
            LTrend_lastTouch,//последнее касание трендовой (до первого достижения 1 цели)
            HPx100_break,   //Достижение 100%
            HPx200_break,   //Достижение 200%
            Target1_get,    //Достижение 1-й цели
            Target2_get,    //Достижение 2-й цели
            Target3_get,    //Достижение 3-й цели
            Point1_get,
            НРx1_НР,
            ST4,
            ST4x2,
            ST_get,
            HP_recur4,       //L3. откат от 6-й до 4-й
            HP_break4,       //L4. пробой лоу/хай бара 4 на откате от 6.
            HP_break5;       //L3. пробой 5 на откате от 6.

        //12.	Достижение НР до пробития целевой и трендовой на промежутке (ст-4)*4
        public int ClearHP,
            //13.	Достижение 100% и 200% (по отдельности) после пробития ЛЦ на промежутке (ст-4)*4
                   BreakTargetline_100pct,
                   BreakTargetline_200pct,
            //14.	Достижение 100% и 200% (по отдельности) после пробития (пробитие – повторный подход к уровню НР с предыдущим образованием противоположного 4 точке абс. экстремума (годного по алгоритму отбора экстремумов Алексея, который отрабатывает на старте алгоритма поиска моделей,  после первого достижения НР) НР на промежутке (ст-4)*4
                   BreakHP_100pct,
                   BreakHP_200pct,
            //15.	Достижение 100% и 200% (по отдельности) без повторного (т.е. уровень НР достигнут с перелетом или пункт в пункт не важно и следующими барами цена идет дальше без образования противоположного абс. экстремума) пробития  НР на промежутке (ст-4)*4
                   ClearHP_100pct,
                   ClearHP_200pct,
            //16.	Достижение 100% и 200% (по отдельности) после пробития ЛЦ и до пробития ЛТ на промежутке (ст-4)*4
                   BreakTargetline_100pct_CleanLTrend,
                   BreakTargetline_200pct_CleanLTrend,
            //17.	Достижение 100% и 200% (по отдельности) после пробития НР и до пробития ЛТ на промежутке (ст-4)*4
                   BreakHP_100pct_CleanLTrend,
                   BreakHP_200pct_CleanLTrend,
            //17.a	Достижение 100% и 200% (по отдельности) без повторного пробития НР и до пробития ЛТ на промежутке (ст-4)*4
                   ClearHP_100pct_CleanLTrend,
                   ClearHP_200pct_CleanLTrend,
            //18.	Достижение целей (3-х по отдельности) без пробития целевой на промежутке (ст-4)*4
                   Target1_ClearTargetline,
                   Target2_ClearTargetline,
                   Target3_ClearTargetline,
            //19.	Достижение целей (3-х по отдельности) с пробитой целевой до пробития трендовой на промежутке (ст-4)*4
                   Target1_BreakTargetline,
                   Target2_BreakTargetline,
                   Target3_BreakTargetline,
            //20.	Достижение целей (3-х по отдельности) без пробития целевой, но с образованием 6, на промежутке (ст-4)*4
                   Target1_ClearTargetline_with6pts,
                   Target2_ClearTargetline_with6pts,
                   Target3_ClearTargetline_with6pts,
            //21.	Достижение целей (3-х по отдельности) без пробития целевой, но без  образования 6 (т.е. 4=6), на промежутке (ст-4)*4
                   Target1_ClearTargetline_with4pts,
                   Target2_ClearTargetline_with4pts,
                   Target3_ClearTargetline_with4pts;
        //22.	Соотношение (6 - 1 цель по горизонтали) к 1-6
        public double P6_T1x1_6,
            //23.	Соотношение (6 - 2 цель по горизонтали) к 1-6
                   P6_T2x1_6,
            //24.	Соотношение (6 - 3 цель по горизонтали) к 1-6
                   P6_T3x1_6,
            //хай/лоу бара пробившего НР
                   HP_price,
                   HP_extr_price, //хай/лоу бара пробившего НР
                   HP_continue,  //соответствует 6 реальной
                   HP_recoil,    //движение от НР к 1 цели в МП
                   TimeOfTarget1, //соотношение 6-Trend / Trend-aim1
                   TimeOf6ST,      //время в СТ4
                   TimeOfTrendBreak,
                   TimeOfHPBreak,
                   TimeOfHPGet,
                   P6_CP4x2,
                   P6_CP4x1,
                   TrendBreak_CP4x2,
                   TrendBreak_CP4x1,
                   S123, S234, S345, S456; //площади треугольников


        void cleanup(TDirection dir)
        {
            HP_get = HP_off = HP_break = LTarget_break = LTrend_break = LTrend_lastTouch =
                HPx100_break = HPx200_break = Target1_get = Target2_get = Target3_get = Point1_get = НРx1_НР =
                ST4 = ST4x2 = ST_get = HP_recur4 = HP_break4 = HP_break5 = 0;


            P6_T1x1_6 = P6_T2x1_6 = P6_T3x1_6 = P6_CP4x2 = P6_CP4x1 = TrendBreak_CP4x2 = TrendBreak_CP4x1 = 0;
            HP_price = dir == TDirection.dtUp ? 0 : 99999;
        }

        //L8. площади треугольников 123,234,345,456
        public void CountTrianglesArea(TModel tm, TQuotes q)
        {
            double pip = tm.ModelDir == TDirection.dtUp ? q.Symbol.pip : -q.Symbol.pip;
            double l12 = Math.Sqrt(Math.Pow(tm.Point2.Bar - tm.Point1.Bar, 2) + Math.Pow((tm.Point2.Price - tm.Point1.Price) / pip, 2));
            double l13 = Math.Sqrt(Math.Pow(tm.Point3.Bar - tm.Point1.Bar, 2) + Math.Pow((tm.Point3.Price - tm.Point1.Price) / pip, 2));
            double l23 = Math.Sqrt(Math.Pow(tm.Point3.Bar - tm.Point2.Bar, 2) + Math.Pow((tm.Point2.Price - tm.Point3.Price) / pip, 2));
            double l24 = Math.Sqrt(Math.Pow(tm.Point4.Bar - tm.Point2.Bar, 2) + Math.Pow((tm.Point4.Price - tm.Point2.Price) / pip, 2));
            double l34 = Math.Sqrt(Math.Pow(tm.Point4.Bar - tm.Point3.Bar, 2) + Math.Pow((tm.Point4.Price - tm.Point3.Price) / pip, 2));

            double p = (l12 + l13 + l23) / 2;
            S123 = Math.Round(Math.Sqrt(p * (p - l12) * (p - l13) * (p - l23)));
            p = (l23 + l24 + l34) / 2;
            S234 = Math.Round(Math.Sqrt(p * (p - l23) * (p - l24) * (p - l34)));
            S345 = S456 = 0;

            if (tm.Point5.Bar > tm.Point4.Bar)
            {
                double l35 = Math.Sqrt(Math.Pow(tm.Point5.Bar - tm.Point3.Bar, 2) + Math.Pow((tm.Point5.Price - tm.Point3.Price) / pip, 2));
                double l45 = Math.Sqrt(Math.Pow(tm.Point5.Bar - tm.Point4.Bar, 2) + Math.Pow((tm.Point4.Price - tm.Point5.Price) / pip, 2));
                p = (l34 + l35 + l45) / 2;
                S345 = Math.Round(Math.Sqrt(p * (p - l34) * (p - l35) * (p - l45)));

                if (tm.Point6.Bar > tm.Point4.Bar)
                {
                    double l46 = Math.Sqrt(Math.Pow(tm.Point6.Bar - tm.Point4.Bar, 2) + Math.Pow((tm.Point6.Price - tm.Point4.Price) / pip, 2));
                    double l56 = Math.Sqrt(Math.Pow(tm.Point6.Bar - tm.Point5.Bar, 2) + Math.Pow((tm.Point6.Price - tm.Point5.Price) / pip, 2));
                    p = (l45 + l46 + l56) / 2;
                    S456 = Math.Round(Math.Sqrt(p * (p - l45) * (p - l46) * (p - l56)));
                }
            }
        }

        //27.	Доход до НР  в % к 1-НР
        //28.	Недоход до НР в % к 1-НР
        void Calc_HP_OnOff_percent(TQuotes q, TModel tm)
        {
            int u;
            if (tm.ModelID == 540)
                u = tm.ModelID;   // use it for breakpoints
            //HP_price - это цена хай/лой бара достигшего НР
            double pip = tm.ModelDir == TDirection.dtUp ? q.Symbol.pip : -q.Symbol.pip;
            if (tm.ModelType == Common.TModelType.ModelOfAttraction)
            {
                НРx1_НР = (int)((HP_price - tm.PointSP2.Price + pip) / pip * 100 /
                               ((tm.PointSP2.Price - tm.Point1.Price + pip) / pip));
            }
            else
            {
                if (tm.Point61.Bar == 0)
                    НРx1_НР = -99999;
                else
                    НРx1_НР = (int)((HP_price - tm.Point61.Price + pip) / pip * 100 /
                                   ((tm.Point61.Price - tm.Point1.Price + pip) / pip));
            }
        }

        public void checkup(TQuotes q, TModel tm, Model md/*, List<TModel> tmodels, int[] points*/)
        {
            cleanup(tm.ModelDir);
            /*отслеживаю события:
             * Достижение НР
             * пробития целевой
             * пробития трендовой
             * Достижение 100% и 200%
             * пробития НР (повторный подход к НР с предыдущим образованием противоположного 4 точке абс. экстремума)
             * 4=6
             * Достижение целей (3-х по отдельности) #бара
             * Достижение Т1 (для МП)
            */
            double trendPoint = Common.CalcLineValue(tm.TrendLine, tm.Point41.Bar);
            //double targetPoint = Common.CalcLineValue(tm.TargetLineCorrected, tm.Point41.Bar);
            double targetPoint = tm.ModelDir == TDirection.dtUp ? q[(int)tm.Point41.Bar].High : q[(int)tm.Point41.Bar].Low;
            double trendK = tm.TrendLine.Angle;
            double targetK = tm.TargetLineCorrected.Angle;

            HP_price = tm.Point6.Price;
            HP_continue = tm.Point6.Price;
            HP_recoil = tm.PointSP2.Price;
            HP_extr_price = tm.Point61.Price;

            int u;
            if (tm.ModelID == 514)
                u = tm.ModelID;   // use it for breakpoints
            
            /// замена на данные TMolel /// 
            LTrend_break = tm.BreakTrendLineFirst.iBar;
            LTrend_lastTouch = tm.BreakTrendLine.iBar;

            if (tm.ModelDir == TDirection.dtUp)
                for (int i = (int)tm.Point41.Bar; i <= tm.MaxProcessBar && i < q.GetCount(); i++, trendPoint += trendK, targetPoint += targetK)
                {
                    //пробитие целевой
                    if (LTarget_break == 0 && q[i].High > targetPoint) LTarget_break = i;
                    //пробитие трендовой
                    /// замена на данные TMolel /// if (LTrend_break == 0 && q[i].Low < trendPoint) LTrend_break = i;
                    //Достижение 100%
                    //if (HPx100_break == 0 && q[i].High > tm.Point61Pct100.Price) HPx100_break = i;
                    if (HPx100_break == 0 && q[i].High > md._100Percent[0].y) HPx100_break = i;
                    //Достижение 200%
                    //if (HPx200_break == 0 && q[i].High > tm.Point61Pct200.Price) HPx200_break = i;
                    if (HPx200_break == 0 && q[i].High > md._100Percent[1].y) HPx200_break = i;
                    //Достижение 1-й цели
                    if (Target1_get == 0 && LTrend_break <= i && q[i].Low <= tm.Target1.Price && i >= tm.Target1.Bar) Target1_get = i;
                    //время 1-й цели
                    /// замена на данные TMolel /// if (Target1_get == 0 && LTrend_break != 0 && (q[i].Low <= trendPoint || (i < (q.GetCount() - 1) && q[i + 1].High < (trendPoint + targetK))) && q[i].High >= trendPoint) LTrend_lastTouch = i;
                    if (tm.ModelType != Common.TModelType.ModelOfAttraction)
                    {
                        //достижение НР
                        if (HP_get == 0)
                        {
                            if (tm.Point61.Price != 0 && q[i].High >= tm.Point61.Price && tm.Point6.Bar > tm.Point4.Bar)
                            //if (HP_get == 0 && q[i].High >= tm.Point61.Price)
                            {
                                HP_get = i;
                                HP_price = q[i].High;
                            }
                        }
                        //образованием противоположного 4 точке абс. экстремума после достижения НР
                        else if (HP_off == 0)
                        {
                            if (q[i].Low < tm.Point61.Price && q.IsExtremum(i, Options.Extremum1, TDirection.dtUnKnown) == TDirection.dtDn) HP_off = i;
                            //отслеживание экстемума НР
                            if (HP_extr_price < q[i].High) HP_extr_price = q[i].High;
                        }
                        //пробития НР (повторный подход к НР с предыдущим образованием противоположного 4 точке абс. экстремума)
                        else if (HP_break == 0)
                        {
                            if (q[i].High >= HP_extr_price) HP_break = i;
                        }
                        //Достижение 2-й цели
                        if (Target2_get == 0 && LTrend_break <= i && q[i].Low <= tm.Target2.Price && i >= tm.Target2.Bar) Target2_get = i;
                        //Достижение 3-й цели
                        if (Target3_get == 0 && Target2_get != 0 && q[i].Low <= tm.Target3.Price) Target3_get = i;
                    }
                    else
                    {
                        if (HP_get == 0 && q[i].High >= tm.PointSP2.Price)
                        {
                            HP_get = i;
                            HP_price = q[i].High;
                        }
                        if (HP_get != 0 && q[i].Low < HP_recoil)
                        {
                            HP_recoil = q[i].Low;
                        }
                        if (Point1_get == 0 && q[i].Low <= tm.Point1.Price) Point1_get = i;
                    }
                    if (tm.ModelType == Common.TModelType.ModelOfExpansion)
                    {
                        if (ST_get == 0 && q[i].Low <= tm.PointSP2.Price) ST_get = i;
                    }
                    if (tm.Point6.Bar > tm.Point4.Bar)
                    {
                        if (HP_recur4 == 0 && HP_get != 0 && q[i].Low <= tm.Point4.Price) HP_recur4 = i;
                        if (HP_break4 == 0 && HP_get != 0 && q[i].Low <= q[(int)tm.Point4.Bar].Low) HP_break4 = i;
                        if (HP_break5 == 0 && HP_get != 0 && q[i].Low <= tm.Point5.Price) HP_break5 = i;
                    }
                }
            else // (tm.ModelDir == TDirection.dtDn)
                for (int i = (int)tm.Point41.Bar; i <= tm.MaxProcessBar && i < q.GetCount(); i++, trendPoint += trendK, targetPoint += targetK)
                {
                    //пробитие целевой
                    if (LTarget_break == 0 && q[i].Low < targetPoint) LTarget_break = i;
                    //пробитие трендовой
                    /// замена на данные TMolel /// if (LTrend_break == 0 && q[i].High > trendPoint) LTrend_break = i;
                    //Достижение 100%
                    //if (HPx100_break == 0 && q[i].Low < tm.Point61Pct100.Price) HPx100_break = i;
                    if (HPx100_break == 0 && q[i].Low < md._100Percent[0].y) HPx100_break = i;
                    //Достижение 200%
                    //if (HPx200_break == 0 && q[i].Low < tm.Point61Pct200.Price) HPx200_break = i;
                    if (HPx200_break == 0 && q[i].Low < md._100Percent[1].y) HPx200_break = i;
                    //Достижение 1-й цели
                    if (Target1_get == 0 && LTrend_break <= i && q[i].High >= tm.Target1.Price && i >= tm.Target1.Bar) Target1_get = i;
                    //время 1-й цели
                    /// замена на данные TMolel /// if (Target1_get == 0 && LTrend_break != 0 && (q[i].High >= trendPoint || (i < (q.GetCount() - 1) && q[i + 1].Low > (trendPoint + targetK))) && q[i].Low <= trendPoint) LTrend_lastTouch = i;
                    if (tm.ModelType != Common.TModelType.ModelOfAttraction)
                    {
                        if (tm.ModelID == 15)
                            u = tm.ModelID;   // use it for breakpoints

                        //достижение НР
                        if (HP_get == 0)
                        {
                            if (tm.Point61.Price != 0 && q[i].Low <= tm.Point61.Price && tm.Point6.Bar > tm.Point4.Bar)
                            {
                                HP_get = i;
                                HP_price = q[i].Low;
                            }
                        }
                        //образованием противоположного 4 точке абс. экстремума после достижения НР
                        else if (HP_off == 0)
                        {
                            if (q[i].High > tm.Point61.Price && q.IsExtremum(i, Options.Extremum1, TDirection.dtUnKnown) == TDirection.dtUp) HP_off = i;
                            //отслеживание экстемума НР
                            if (HP_extr_price > q[i].Low) HP_extr_price = q[i].Low;
                        }
                        //пробития НР (повторный подход к НР с предыдущим образованием противоположного 4 точке абс. экстремума)
                        else if (HP_break == 0)
                        {
                            if (q[i].Low <= HP_extr_price) HP_break = i;
                        }
                        //Достижение 2-й цели
                        if (Target2_get == 0 && LTrend_break <= i && q[i].High >= tm.Target2.Price && i >= tm.Target2.Bar) Target2_get = i;
                        //Достижение 3-й цели
                        if (Target3_get == 0 && Target2_get != 0 && q[i].High >= tm.Target3.Price) Target3_get = i;
                    }
                    else
                    {
                        if (HP_get == 0 && q[i].Low <= tm.PointSP2.Price)
                        {
                            HP_get = i;
                            HP_price = q[i].Low;
                        }
                        if (HP_get != 0 && q[i].High > HP_recoil)
                        {
                            HP_recoil = q[i].High;
                        }
                        if (Point1_get == 0 && q[i].High >= tm.Point1.Price) Point1_get = i;
                    }
                    if (tm.ModelType == Common.TModelType.ModelOfExpansion)
                    {
                        if (ST_get == 0 && q[i].High >= tm.PointSP2.Price) ST_get = i;
                    }
                    if (tm.Point6.Bar > tm.Point4.Bar)
                    {
                        if (HP_recur4 == 0 && HP_get != 0 && q[i].High >= tm.Point4.Price) HP_recur4 = i;
                        if (HP_break4 == 0 && HP_get != 0 && q[i].High >= q[(int)tm.Point4.Bar].High) HP_break4 = i;
                        if (HP_break5 == 0 && HP_get != 0 && q[i].High >= tm.Point5.Price) HP_break5 = i;
                    }
                }
            if (HP_recoil == tm.PointSP2.Price) HP_recoil = 0;
            if (HP_get == 0) HP_continue = 0;

            if (tm.ModelType != Common.TModelType.ModelOfAttraction)
            {
                //12.	Достижение НР до пробития целевой и трендовой на промежутке (ст-4)*4
                this.ClearHP = (HP_get != 0 && (LTarget_break == 0 || HP_get <= LTarget_break) && (LTrend_break == 0 || HP_get <= LTrend_break)) ? 1 : 0;

                //13.	Достижение 100% и 200% (по отдельности) после пробития ЛЦ на промежутке (ст-4)*4
                this.BreakTargetline_100pct = (LTarget_break != 0 && HPx100_break > LTarget_break) ? 1 : 0;
                this.BreakTargetline_200pct = (LTarget_break != 0 && HPx200_break > LTarget_break) ? 1 : 0;
                
                //14.	Достижение 100% и 200% (по отдельности) после пробития (пробитие – повторный подход к уровню НР с предыдущим образованием противоположного 4 точке абс. экстремума (годного по алгоритму отбора экстремумов Алексея, который отрабатывает на старте алгоритма поиска моделей,  после первого достижения НР) НР на промежутке (ст-4)*4
                this.BreakHP_100pct = (HP_break != 0 && HPx100_break >= HP_off) ? 1 : 0;
                this.BreakHP_200pct = (HP_break != 0 && HPx200_break >= HP_off) ? 1 : 0;
                
                //15.	Достижение 100% и 200% (по отдельности) без повторного (т.е. уровень НР достигнут с перелетом или пункт в пункт не важно и следующими барами цена идет дальше без образования противоположного абс. экстремума) пробития  НР на промежутке (ст-4)*4
                //this.ClearHP_100pct = (HP_get != 0 && HPx100_break > HP_get && (HP_break == 0 || HP_break > HPx100_break)) ? 1 : 0;
                //this.ClearHP_200pct = (HP_get != 0 && HPx200_break > HP_get && (HP_break == 0 || HP_break > HPx200_break)) ? 1 : 0;
                this.ClearHP_100pct = (HPx100_break != 0 && (HP_off == 0 || HP_off > HPx100_break)) ? 1 : 0;
                this.ClearHP_200pct = (HPx200_break != 0 && (HP_off == 0 || HP_off > HPx200_break)) ? 1 : 0;
                
                //16.	Достижение 100% и 200% (по отдельности) после пробития ЛЦ и до пробития ЛТ на промежутке (ст-4)*4
                this.BreakTargetline_100pct_CleanLTrend = (LTarget_break != 0 && HPx100_break > LTarget_break && (LTrend_break == 0 || LTrend_break > HPx100_break)) ? 1 : 0;
                this.BreakTargetline_200pct_CleanLTrend = (LTarget_break != 0 && HPx200_break > LTarget_break && (LTrend_break == 0 || LTrend_break > HPx200_break)) ? 1 : 0;
                
                //17.	Достижение 100% и 200% (по отдельности) после пробития НР и до пробития ЛТ на промежутке (ст-4)*4
                this.BreakHP_100pct_CleanLTrend = (HP_off != 0 && HPx100_break >= HP_off && (LTrend_break == 0 || LTrend_break > HPx100_break)) ? 1 : 0;
                this.BreakHP_200pct_CleanLTrend = (HP_off != 0 && HPx200_break >= HP_off && (LTrend_break == 0 || LTrend_break > HPx200_break)) ? 1 : 0;
                
                //17.a	Достижение 100% и 200% (по отдельности) без повторного пробития НР и до пробития ЛТ на промежутке (ст-4)*4
                this.ClearHP_100pct_CleanLTrend = (HPx100_break != 0 && (LTrend_break == 0 || LTrend_break > HPx100_break)) ? 1 : 0;
                this.ClearHP_200pct_CleanLTrend = (HPx200_break != 0 && (LTrend_break == 0 || LTrend_break > HPx200_break)) ? 1 : 0;
                
                //18.	Достижение целей (3-х по отдельности) без пробития целевой на промежутке (ст-4)*4
                this.Target1_ClearTargetline = (Target1_get != 0 && (LTarget_break == 0 || LTarget_break > Target1_get)) ? 1 : 0;
                this.Target2_ClearTargetline = (Target2_get != 0 && (LTarget_break == 0 || LTarget_break > Target2_get)) ? 1 : 0;
                this.Target3_ClearTargetline = (Target3_get != 0 && (LTarget_break == 0 || LTarget_break > Target3_get)) ? 1 : 0;
                
                //19.	Достижение целей (3-х по отдельности) с пробитой целевой до пробития трендовой на промежутке (ст-4)*4
                this.Target1_BreakTargetline = (Target1_get != 0 && LTarget_break != 0 && LTarget_break < Target1_get) ? 1 : 0;
                this.Target2_BreakTargetline = (Target2_get != 0 && LTarget_break != 0 && LTarget_break < Target2_get) ? 1 : 0;
                this.Target3_BreakTargetline = (Target3_get != 0 && LTarget_break != 0 && LTarget_break < Target3_get) ? 1 : 0;
                
                //20.	Достижение целей (3-х по отдельности) без пробития целевой, но с образованием 6, на промежутке (ст-4)*4
                this.Target1_ClearTargetline_with6pts = (Target1_get != 0 && (LTarget_break == 0 || LTarget_break > Target1_get) && tm.Point6.Bar > tm.Point4.Bar) ? 1 : 0;
                this.Target2_ClearTargetline_with6pts = (Target2_get != 0 && (LTarget_break == 0 || LTarget_break > Target2_get) && tm.Point6.Bar > tm.Point4.Bar) ? 1 : 0;
                this.Target3_ClearTargetline_with6pts = (Target3_get != 0 && (LTarget_break == 0 || LTarget_break > Target3_get) && tm.Point6.Bar > tm.Point4.Bar) ? 1 : 0;
                
                //21.	Достижение целей (3-х по отдельности) без пробития целевой, но без  образования 6 (т.е. 4=6), на промежутке (ст-4)*4
                this.Target1_ClearTargetline_with4pts = (Target1_get != 0 && (LTarget_break == 0 || LTarget_break > Target1_get) && tm.Point6.Bar == tm.Point4.Bar) ? 1 : 0;
                this.Target2_ClearTargetline_with4pts = (Target2_get != 0 && (LTarget_break == 0 || LTarget_break > Target2_get) && tm.Point6.Bar == tm.Point4.Bar) ? 1 : 0;
                this.Target3_ClearTargetline_with4pts = (Target3_get != 0 && (LTarget_break == 0 || LTarget_break > Target3_get) && tm.Point6.Bar == tm.Point4.Bar) ? 1 : 0;
                
                //22.	Соотношение (6 - 1 цель по горизонтали) к 1-6
                this.P6_T1x1_6 = Target1_get == 0 ? -1 : Math.Round((tm.Point6.Bar - Target1_get - 1) / (tm.Point1.Bar - tm.Point6.Bar - 1), 2);
                
                //23.	Соотношение (6 - 2 цель по горизонтали) к 1-6
                this.P6_T2x1_6 = Target2_get == 0 ? -1 : Math.Round((tm.Point6.Bar - Target2_get - 1) / (tm.Point1.Bar - tm.Point6.Bar - 1), 2);
                
                //24.	Соотношение (6 - 3 цель по горизонтали) к 1-6
                this.P6_T3x1_6 = Target3_get == 0 ? -1 : Math.Round((tm.Point6.Bar - Target3_get - 1) / (tm.Point1.Bar - tm.Point6.Bar - 1), 2);
                
                //* 29. 1 цель - последнее касание трендовой / 6 - первое касание трендовой
                this.TimeOfTarget1 = Target1_get == 0 ? 0 : Math.Round((Target1_get - LTrend_lastTouch + 1) / (LTrend_break - tm.Point6.Bar + 1), 2);

                if (tm.ModelType == Common.TModelType.ModelOfExpansion)
                {
                    ST4 = (int)(tm.Point4.Bar - tm.PointSP2.Bar + 1);
                    ST4x2 = ST4*2;

                    //L2. время достижения СТ при ходе от 6-й в единицах СТ- 4.
                    this.TimeOf6ST = ST_get == 0 ? 0 : Math.Round((ST_get - tm.Point6.Bar + 1) / ST4, 2);
                
                    //L6. указать время пробития трендовой в единицах Ст-4.
                    this.TimeOfTrendBreak = LTrend_break == 0 ? 0 : Math.Round((LTrend_break - tm.Point4.Bar + 1) / ST4, 2);
                    
                    //ST9. указать время 6-й в единицах Ст-4
                    this.TimeOfHPGet = HP_get == 0 ? 0 : Math.Round((HP_get - tm.Point4.Bar + 1) / ST4, 2);
                    //время 6 по отношению т СТ4х1 для слабых и СТ4х2 для сильных моделей
                    this.P6_CP4x1 = tm.Point65.Bar == 0 ? 0 : Math.Round((tm.Point65.Bar - tm.PointSP2.Bar + 1) / ST4 * 100);
                    this.P6_CP4x2 = tm.Point65.Bar == 0 ? 0 : Math.Round((tm.Point65.Bar - tm.PointSP2.Bar + 1) / ST4x2 * 100);

                    this.TrendBreak_CP4x1 = LTrend_break == 0 ? 0 : Math.Round((LTrend_break - tm.PointSP2.Bar + 1) / ST4 * 100);
                    this.TrendBreak_CP4x2 = LTrend_break == 0 ? 0 : Math.Round((LTrend_break - tm.PointSP2.Bar + 1) / ST4x2 * 100);
                    
                    //L7. указать время пробития 6-й (если он был) в единицах СТ-4.
                    this.TimeOfHPBreak = HP_break == 0 ? 0 : Math.Round((HP_break - tm.Point4.Bar + 1) / ST4, 2);
                }
            }
            else
            {
                //6.	Достижение 100% и 200% до пробития 1 точки на участке (1-НР)*2 (сверить с алгоритмом!)
                this.ClearHP_100pct = (HPx100_break != 0 && (Point1_get == 0 || Point1_get > HPx100_break)) ? 1 : 0;
                this.ClearHP_200pct = (HPx200_break != 0 && (Point1_get == 0 || Point1_get > HPx200_break)) ? 1 : 0;

                //7.	Достижение 100% и 200% после пробития 1 точки на участке (1-НР)*2 (сверить с алгоритмом!)
                this.BreakHP_100pct = (Point1_get != 0 && HPx100_break > Point1_get) ? 1 : 0;
                this.BreakHP_200pct = (Point1_get != 0 && HPx200_break > Point1_get) ? 1 : 0;
            }
            Calc_HP_OnOff_percent(q, tm);
        }
    }
}
