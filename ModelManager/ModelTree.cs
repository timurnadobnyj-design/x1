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
using ChartV2;
using Skilful.QuotesManager;
using System.Windows.Forms;

namespace Skilful.ModelManager
{
    class Stick //класс описываает один уровень -- один план
    {
        Node[] stick;
        public uint[] points;  //это список всех точек моделей данного уровня, включая их пересечения (см. ModelManager::CountPointsByStatus)
        public List<TModel> mlist;

        public Node this [int i]
        {
            get
            {
                return stick[i];
            }
            set
            {
                stick[i] = value;
            }
        }
        public Stick(TQuotes q)
        {
            mlist = q.MM.Models;
            points = q.MM.points;
            stick = new Node[mlist.Count];
            for(int i =0; i<stick.Length; i++) stick[i] = new Node();
        }
        public int length
        {
            get
            {
                return mlist.Count;
            }
        }
        public int lastIndex
        {
            get
            {
                return mlist.Count - 1;
            }
        }
    }

    public class ModelTree
    {
        //текущий узел
        public Node cr{
            get{ return map[l][i]; }
        }
        //
        int l=0, i;       //указатели l = level по вертикали; i = index по горизонтали
        int rootFrame;       //TF верхнего яруса
        TSymbol symbol; 
        Stick[] map;
        int level(TF frame)
        {
            return rootFrame - (int)frame;
        }
        TF  frame(int level)
        {
            return (TF)(rootFrame - level);
        }
        //
        int[] parentID;   //список счетчиков моделей старших планов где т1 ~ т1 текущей модели
        void initParentID()
        {
            for (int i = 0; i < parentID.Length; i++) parentID[i] = 0;
        }

        //
        public TModel GetModel(TF tf, int id)
        {
            Stick stick = map[level(tf)];
            return stick.mlist[id];
        }

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="rootFrame">TF верхнего яруса</param>
        /// <param name="cnt">количество фреймов в дереве</param>
        /// <param name="sym">объект TSymbol</param>
        public ModelTree(int rootFrame, int cnt, TSymbol sym) //int cnt - количество фреймов в дереве
        {
            symbol = sym;
            map = new Stick[cnt];
            this.rootFrame = rootFrame;
            parentID = new int[cnt];
        }

        Node next()
        {
            return i < map[l].lastIndex ? map[l][i+1] : (Node)null;
        }
        Node prev()
        {
            return i > 0 ? map[l][i-1] : (Node)null;
        }
        Node parent()
        {
            return l > 0 ? map[l-1][ map[l][i].parent ] : (Node)null;
        }
        Node child()
        {
            return l < (map.Length-1) ? map[l+1][ map[l][i].child ] : (Node)null;
        }

        /// <summary>
        /// возвращает id родительской модели если аналогичная модель существует на старшем плане
        /// parentID = ее индекс в массиве моделей старшего плана
        /// </summary>
        /// <param name="m">искомая модель на младшем плане</param>
        /// <param name="parentID">индекс последней из проверенных моделей старшего плана</param>
        /// <returns>id родительской модели если модель существует на старшем плане, false если нет или при остсутствии старшего плана</returns>
        public int SearchParentModel(TModel m)
        {
            try
            {
                //if (m.Quotes.tf == TF.m240 && (int)m.Point1.Bar == 812)
                //{
                //    int a = 1;
                //}

                int l = level(m.Quotes.tf + 1);
                if (l >= 0)
                {
                    for (int p = 0; p <= l; p++)
                    {
                        TF tf = frame(p);
                        Stick stick = map[p];  //ветка узлов старшего плана
                        int pID = parentID[p];
                        if ((pID >= 0) && (stick != null) && (pID < stick.length))
                        {
                            while (pID < stick.length)
                            {
                                if (m.Point1.Bar < stick[pID].pointsOnChildTF[l, 1]
                                 || m.Point1.Bar == stick[pID].pointsOnChildTF[l, 1]
                                 && m.Point4.Bar < stick[pID].pointsOnChildTF[l, 4]) break;

                                if (m.Point1.Bar == stick[pID].pointsOnChildTF[l, 1]
                                 && m.Point2.Bar == stick[pID].pointsOnChildTF[l, 2]
                                 && m.Point3.Bar == stick[pID].pointsOnChildTF[l, 3]
                                 && m.Point4.Bar == stick[pID].pointsOnChildTF[l, 4])
                                {
                                    m.TimeFrame = tf;
                                    m.IDonBaseTF = pID;
                                    return pID;
                                }

                                pID++;
                            }
                            parentID[p] = pID > 0 ? --pID : 0;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("SearchParentModel exception:" + m.Quotes.Symbol.Name + m.Quotes.tf + e.Message);
            }

            return -1;
        }

        /*
         * 
         * при инициализации нам нужно - 
         * ^1. проинитить точки на младшем фрейме
         * 2. определить родительскую модель (индекс)
         * 2.а определить точку родительской модели
         * 2.б определить точку глобальной модели
         * 3. определить глобальную модель (индекс), если данная модель является глобальной то индекс равен индексу данной модели
         * 4. определить индекс следующей глобальной модели
         *  (глобальной моделью являетя модель находящаяся за пределами линий тренда и целей предыдущих моделей данного плана)
         * 5. сохранить свой индекс в родительском ноде в качестве чилда
         * ^6. проверить существование модели на старшем фрейме
         * 
         */

        public int nextGlobalModel(int srcID, TF tf)
        {
            return -1;
        }

        /// <summary>
        /// model является глобальной для тренда от данной т1 на этом же плане
        /// </summary>
        /// <param name="m"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public bool isGlobalTrendModel(TModel m, uint[] points)
        {
            ///return stick.mlist[i].Stat & MStat.GlobalTrend;
            ///
            // модель является последней(по 3 и 4') в веере от этой т1
            // &&
            // данная точка 1 не пересекается с точками других моделей кроме 6
            // &&
            // также точка не должна быть свободно плавающей моделью по тренду на данном плане,
            // но такие модели должны фильтроваться менеджером моделей,
            // либо менеджер может отметить их например в Model.Stat (напр установить MStat.GlobalTrend)
            return (m.Stat & (uint)MStat.GlobalP1) != 0
                && (points[(int)m.Point1.Bar] & 0x011110) == 0
                ;

        }

        /// <summary>
        /// поиск предыдущей глобальной модели на данном плане
        /// </summary>
        /// <param name="srcID"></param>
        /// <param name="tf"></param>
        /// <returns></returns>
        public int prevGlobalModel(int srcID, TF tf)
        {
            if (srcID <= 0) return -1;

            Stick stick = map[level(tf)];
            TModel msrc = stick.mlist[srcID];
            int srcP1 = msrc.Point1.iBar;
            TDirection currentDir = stick.mlist[srcID].ModelDir;
            for (int i = srcID-1; i >= 0; i--)
            {
                TModel m = stick.mlist[i];
                if (
                    m.TimeFrame == tf     //модель принадлежит данному плану
                    &&
                    m.Point1.Bar < srcP1) //т1 испытуемой левее текущей т1
                {


                    ///if(stick.mlist[i].Stat & MStat.GlobalTrend) return i;
                    //if (
                    //    isGlobalTrendModel(stick.mlist[i], stick.points)
                    //    //&& stick.mlist[i].ModelDir != currentDir  //должна быть непременно противоположно направлена
                    //    ) return i;

                    //если модель является коррекционной либо по тренду (т1==т2|3|4) то предыдущая модель - не глобальная а ближайшая к данной
                    if ((stick.points[msrc.Point1.iBar] & 0x001110) != 0 && (m.Stat & (uint)MStat.GlobalTrend) != 0) return i;

                    //если модель является разворотной (т1==т6) то предыдущая та, с 6 которой она пересеклась
                    if ((stick.points[msrc.Point1.iBar] & (1 << 6)) != 0 && (m.Stat & (uint)MStat.GlobalTrend) != 0 && msrc.Point1.Bar == m.Point6.Bar) return i;
                    
                    //в остальных случаях берем предыдущую глобальную модель
                    if ((stick.points[msrc.Point1.iBar] & 0x111110) == 0 && (m.Stat & (uint)MStat.GlobalP1) != 0 && (m.Stat & (uint)MStat.GlobalTrend) != 0) return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 1. установка индексов точек соответствующих точкам моделей на данном плане для массива quotes младшего фрейма
        /// 2. инициализация узлов дерева моделей на данном плане
        /// </summary>
        /// <param name="tf">текущий Фрейм</param>
        /// <param name="ctype">тип графика, обычно ChartType.Candle</param>
        public void initFrame(TQuotes q)
        {
            try
            {
                initParentID();
                int lcur = level(q.tf);
                map[lcur] = new Stick(q);

                //if (q.tf == TF.Day)
                //{
                //    int a = 15;
                //}


                if (q.tf > 0/*TF.m60*/)
                {
                  TQuotes cq = symbol.Frames[(int)q.chartType, (int)q.tf - 1]; //child quotes
                    
                  if(cq != null && cq.GetCount()>0)
                    for (int l = 0; l <= lcur; l++)//перебор фреймов от корневого(старшего) до текущего
                    {
                        Stick stick = map[l];
                        //1. установка индексов точек соответствующих точкам моделей на данном плане для массива quotes младшего фрейма
                        int mi = 0; //индекс дочернего quotes сq, указывает на т1 последней индексируемой модели

                        //перебор моделей
                        //foreach(TModel m in stick.mlist)
                        for (i = 0; i < stick.length; i++)
                        {
                            TModel m = stick.mlist[i];
                            Node node = stick[i];// = new Node(m);

                            //if (m.Quotes.tf == TF.Day && (int)m.Point1.Bar == 2093)
                            //{
                            //    int a = 1234;
                            //}

                            int ci = mi;    //индекс дочернего quotes сq, указывает на текущую точку индексируемой модели
                            
                            //перебор точек моделей
                            int cnt = m.Point6.Bar == m.Point4.Bar ? 4 : 6; //включая СТ и НР точек может и 8
                            for (int pi = 1; pi <= cnt; pi++)
                            {
                                //позиционирование индекса дочернего массива по времени точки
                                //int idx = mpoint(m, pi);         //индекс точки
                                int idx = l == lcur ? mpoint(m, pi) : mpointonchild(i, pi, l, lcur - 1);

                                //TODO сопоставить корректно месяцы и недели -- установил что края недель могут выступать за края месяца, при этом точнсть расчетов не пострадает
                                while (cq[ci].DT < q[idx].DT) ci++;
                                while (ci > 0 && cq[ci].DT > q[idx].DT) ci--; //на случай перелета

                                if (pi == 1) mi = ci;              //сдвинуть указатель на т1 индексируемой модели

                                //поиск экстремума на дочернем массиве котировок
                                double prc = cq[ci].Open;          //тут годится любая цена данной свечи Low<prc<High
                                DateTime nextDT = q.lastindex > idx ? q[idx + 1].DT : Gen.incTime(q[idx].DT, q.tf);
                                while (ci < cq.GetCount() && cq[ci].DT < nextDT)
                                {
                                    if ((m.ModelDir == TDirection.dtUp && (pi % 2) == 0) || (m.ModelDir == TDirection.dtDn && (pi % 2) != 0))
                                    {
                                        if (prc < cq[ci].High)
                                        {
                                            prc = cq[ci].High;
                                            node.pointsOnChildTF[lcur, pi] = ci;
                                        }
                                    }
                                    else
                                    {
                                        if (prc > cq[ci].Low)
                                        {
                                            prc = cq[ci].Low;
                                            node.pointsOnChildTF[lcur, pi] = ci;
                                        }
                                    }
                                    ci++;
                                }
                            }
                        }
                    }

                    //проверка вложенности моделей
                    //для каждой модели проверяются
                    //for(active_models) //это модели пересекающие текущий бар
                    //for(p1..lifetime)
                    //for(m_cur_id..m_last_in_id) //это все модели 1-е точки которых лежат в диапазоне



                    //идентификация моделей по тренду или от начала тренда для своего Плана или коррекционных.
                    //1. изначально все модели лежат правее текущей точки
                    //List<mChannel> mactive = new List<mChannel>();
                    //Stick stick = map[lcur];
                    //for (int qi = 0, mi = 0; qi < q.GetCount(); qi++)
                    //{
                    //    while (stick.mlist[mi].Point1.iBar < qi) mi++;

                    //    //3. лайфтафм активной модели равен текущей точке -- конец цикла для данной модели
                    //    foreach (mChannel m in mactive)
                    //        if (m.LifeTime < qi) mactive.Remove(m);
                    //        //иначе продвигаем уровни линий тренда и целей на текущий бар => //4. для активных моделей вычисление уровней ЛЦ и ЛТ
                    //        else m.incpoint();
                        
                    //    //2. затем часть моделей включаются в список активных 
                    //    // -- 1 точка равно текущей. -- начало цикла для данных моделей
                    //    while (stick.mlist[mi].Point1.iBar == qi)
                    //    {
                    //        TModel mc = stick.mlist[mi];
                    //        if (mc.TimeFrame == q.tf)        //учитываем только модели текущего плана
                    //        {
                    //            //5. собссна проверка вложенности для активных моделей
                    //            foreach (mChannel m in mactive)
                    //            {
                    //                if(m.model.Point1.iBar == mc.Point1.iBar) break;
                    //                if (m.resistence >= mc.Point1.Price && m.support <= mc.Point1.Price) //допускается касание линий, this.t1==owner.t3 for example
                    //                {
                    //                    if (m.model.ModelDir == mc.ModelDir)    //модель по тренду если сонаправлена и расположена в любом месте от т1 до 1-4х4
                    //                        mc.isBytrendOfID = m.model.ModelID;
                    //                    else if (mc.Point1.Bar < m.model.Point6 || m.model.BreakTrendLine.Bar == 0) //модель коррекционная если расположена до появления т6 либо т6 не зафиксирована пробоем трендовой
                    //                        mc.isCorrectionOfID = m.model.ModelID;
                    //                }
                    //            }

                    //            mactive.Add(new mChannel(stick.mlist[mi]));
                    //        }
                    //        mi++;
                    //    }
                    //}
                    ////////////////////////////////////////////////////////////
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("initFrame exception:"+q.Symbol.Name+q.tf+e.Message);
            }
        }

        public int mpoint(TModel m, int i) // доступ к индексам точек модели (только для чтения)
        {
            switch (i)
            {
                case 1: return (int)m.Point1.Bar;
                case 2: return (int)m.Point2.Bar;
                case 3: return (int)m.Point3.Bar;
                case 4: return (int)m.Point4.Bar;
                case 5: return (int)m.Point5.Bar;
                case 6: return (int)m.Point6.Bar;
            }
            return -1;
        }

        public int mpointonchild(int imodel, int ipoint, int lparent, int lchild) // доступ к индексам точек модели (только для чтения)
        {
            return map[lparent][imodel].pointsOnChildTF[lchild, ipoint];
        }

        //в данной версии за следующую глобальную принимается ближайшая новая т1, причем модель берется последняя из списка с данной т1
        //в TODO -- нужно искать по списку точек
        //public Node NextGlobalNode(Node cr)
        //{
        //    while ((points[(int)cr.model.Point1.Bar] & 0x0FFFF0) != 0 && cr != null) cr = cr.next; //бар включает точки кроме 1 и 6
        //    return cr==null ? null : cr.model.Point1.Bar;
        //}
    }

    public class Node
    {
        public  int parent, //индекс родительской модели (старшего плана)
                    owner,  //индекс модели данного плана для которой данная модель является вложенной или коррекционной
                    child,
                    id;


        public int ParentPoint,//номер точки родительской модели от которой берет начало данная модель
            ParentRange;//если нет точного совпадения с точкой модели, то в диапазоне после какой точки появилась 1-я данной

        public int[,] pointsOnChildTF = new int[(int)TF.count, 8]; //всего 8 точек(макс) от СТ=0 до НР=7
        public TF baseTF;
        public int baseID;
    }

    /// <summary>
    /// вспомогательный класс лна базе TModel
    /// для быстрого доступа к уровням поддержки/сопротивления (ЛЦ/ЛТ в соответствии с направлением модели)
    /// + другие вспомогательллные функции
    /// </summary>
    class mChannel
    {
        public TModel model;
        public double trendK { get { return model.TrendLine.Angle; } }
        public double targetK { get { return model.TargetLineCorrected.Angle; } }
        public int LifeTime { get { return (int)model.MaxProcessBar; } }
        
        public double trendPoint, targetPoint, resistence, support;
        
        public mChannel(TModel m)
        {
            model = m;
            trendPoint = m.Point1.Price;//   m.ModelDir == TDirection.dtUp ? q[(int)m.Point1.Bar].Low : q[(int)m.Point1.Bar].High;
            targetPoint = Common.CalcLineValue(m.TargetLineCorrected, m.Point1.Bar);
        }

        public void incpoint()
        {
            trendPoint += trendK;
            targetPoint += targetK;
            if (model.ModelDir == TDirection.dtUp)
            {
                resistence = targetPoint;
                support = trendPoint;
            }
            else
            {
                resistence = trendPoint;
                support = targetPoint;
            }
        }
    }
}
/*
 * всего понадобится по ветке stick на каждый фрейм
 * + по массиву точек с указанием пересечений для данного фрейма
 * + массив индексов моделей (и их точек) данного фрейма на фрейме меньшем
*/
