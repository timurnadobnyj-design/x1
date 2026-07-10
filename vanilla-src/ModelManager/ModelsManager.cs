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
using Skilful.QuotesManager;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Skilful.ModelManager
{
    public static class Options
    {
        public static int VisibleSteps = 7;
        public static int Extremum1 = 1;
        public static int Extremum2 = 1;
        public static int Extremum4 = 1;
        public static int Extremum5 = 1;
        public static int MBSize1 = 3;
        public static int MBSize2 = 7;
        public static double MACoef = 2;   //!< Максимально возможное соотношение т1-т2 к т3-т4 для ЧМП
        public static TModelError[] CheckedErrors0 = //!< Общие правила
        {
            new TModelError(Common.TModelType.ModelOfAttraction,
                            0x00040000    // Взаимное расположение т2' и т3
                            | 0x00000020  // Пересечение тел баров точек 2 и 5 для МП
                            | 0x00080000,  // Соотношение для ЧМП расстояний т1-т2 к т3-т4  
                            ""),
            new TModelError(Common.TModelType.ModelOfBalance,
                            0x00040000    // Взаимное расположение т2' и т3
                            | 0x00000020,  // Пересечение тел баров точек 2 и 5 для МДР
                            ""),
            new TModelError(Common.TModelType.ModelOfExpansion,
                            0x00040000    // Взаимное расположение т2' и т3
                            | 0x00000020,  // Пересечение тел баров точек 2 и 5 для МР
                            "")
        };
        public static TModelError[] CheckedErrors1 = //!< Правила для моделей 1-го прохода
        {
            new TModelError(Common.TModelType.ModelOfAttraction,
                            0x00000002     // Лишние точки на ЛТ
                            | 0x00000004   // Лишние точки на ЛЦ'
                            | 0x00000020   // Пересечение тел баров точек 2 и 5 для МП
                            | 0x00000040   // Точка 3 не глобальный экстремум
                            | 0x00100000,  // БЦ >= 1/3 Базы для МП
                            ""),
            new TModelError(Common.TModelType.ModelOfBalance,
                            0x00000002    // Лишние точки на ЛТ
                            | 0x00000004  // Лишние точки на ЛЦ'
                            | 0x00000020  // Пересечение тел баров точек 2 и 5 для МДР
                            | 0x00000040  // Точка 3 не глобальный экстремум
                            | 0x00100000,  // БЦ >= 1/3 Базы для МДР
                            ""),
            new TModelError(Common.TModelType.ModelOfExpansion,
                            0x00000002    // Лишние точки на ЛТ
                            | 0x00000004  // Лишние точки на ЛЦ'
                            | 0x00000020  // Пересечение тел баров точек 2 и 5 для МР
                            | 0x00000040  // Точка 3 не глобальный экстремум
                            | 0x00000080  // Точки 5 внутри бара точки 4
                            | 0x00100000,  // БЦ >= 1/3 Базы для МР
                            "")
        };
        public static TModelError[] CheckedErrors2 = //!< Проверка правил для моделей 2-го прохода
        {
            new TModelError(Common.TModelType.ModelOfAttraction,
                            0x00001000 |    // Разбег точек т4 и т4' < 3 баров
                            0x00100000 | // БЦ >= 1/3 Базы для МП
                            0x00200000,  // БЦ >= 1/2 Базы для МП
                            ""),
            new TModelError(Common.TModelType.ModelOfBalance,
                            0x00001000 |    // Разбег точек т4 и т4' < 3 баров
                            0x00200000,  // БЦ >= 1/2 Базы для МДР
                            ""),
            new TModelError(Common.TModelType.ModelOfExpansion,
                            0x00001000 | // Разбег точек т4 и т4' < 3 баров
                            0x00100000,  // БЦ > 1/3 Базы для МР
                            "")
        };
        public static TModelError[] CheckedErrors3 = //!< Проверка правил для моделей 3-го прохода
        {
            new TModelError(Common.TModelType.ModelOfAttraction,
                            0x00001000 |     // Разбег точек т4 и т4' < 3 баров
                            0x00200000,      // БЦ >= 1/2 Базы для МП
                            ""),
            new TModelError(Common.TModelType.ModelOfBalance,
                            0x00001000 |     // Разбег точек т4 и т4' < 3 баров
                            0x00200000,      // БЦ >= 1/2 Базы для МДР
                            ""),
            new TModelError(Common.TModelType.ModelOfExpansion,
                            0x00001000 |    // Разбег точек т4 и т4' < 3 баров
                            0x00100000,     // БЦ > 1/3 Базы для МP
                            "")
        };
        public static double SizePoint1 = 1;
        public static int TangentAndExtremumDiff = 3; //!< Максимально возможное расхождение между точкой модели как экстремум и по касательной в барах
        public static int SP_P4x4 = 4;  //!< Коэф. для расстояния СТ-т4 (МР) (4 +1)
        public static int HP_P1x2 = 2;  //!< Коэф. для расстояния HP-т1 (МП)
        public static int P1_P4x4 = 4;  //!< Коэф. для расстояния т1-т4 (МДР)
        public static bool EnableFiltrationByTrend = true; //!< Включение/Выключение фильтрации моделей по тренду
        public static bool EnableFiltrationByTF = false;     //!< Включение/Выключение фильтрации моделей по планам
        public static bool EnableFiltrationFan = true;      //!< Включение/выключение фильтрации веера моделей
        public static int SeekMode = 2; //! Режим поиска моделей: 1- последовательности; 2- от каждого экстремума
    }

    public class ModelsManager
    {
        public bool SeeksComplete = false;
        public List<TModel> Models,                 //!< Список моделей
                            HModels;                // список формирующихся моделей
        public List<THTangentLine> HTangentLines;   //!< Список гипотетических ЛЦ
        public uint[] points;
        
        public ModelsManager()  //! Конструктор
        {
            Models = new List<TModel>();
            HModels = new List<TModel>();
            HTangentLines = new List<THTangentLine>();
        }
        public ModelsManager(List<TModel> tamod, List<TModel> hmod, List<THTangentLine> htangentl)
        {
            Models = tamod;
            HModels = hmod;
            HTangentLines = htangentl;
            SeeksComplete = true;
        }
        
        public void SeekModels(TQuotes Quotes, int StartPoint, int LastPoint, int DecDigs, System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)          //!  Метод нахождения моделей от указанной точки до конца котировок
        {
            SeeksComplete = false;
            int X;
            List<TModel> ModelsP1;  // Список моделей построенных от одной т1
            int s = 1;
            TDirection Dir = TDirection.dtUnKnown;
            int PB = 0;             // Кол-во обработанных баров;
            int step = (int)((LastPoint - StartPoint) / 100f);
            int Percent;            // % обработанных баров
            int ModelCount1 = 0;         // Кол-во моделей на 1-м шаге

            DateTime DTStart = DateTime.Now;
            Logging.Log("Search started");
            //MainForm.MT4messagesForm.AddMessage("Searching started");
            if (step == 0) step = 1;
            // Определение реального участка котировок на котором должен происходить поиск моделей
            if (Models.Count < 3)
                X = StartPoint; // Расчет с самого начала заданного участка
            else
            {
                int mil = 0;
                int mi;
                // Поиск предпредпоследней глобальной модели
                for (mi = Models.Count - 1; (mi > 0) && (mil < 3); mi--)
                {
                    // Отсчет трех последних последних глобальных моделей
                    if ((mil < 3) && ((Models[mi].Stat & 0x10) != 0))
                    {
                        mil++;
                    }
                }
                // если кол-во глобальных моделей < 3, то перерасчет с самого начала, 
                // иначе от т1 предпредпоследней глобальной модели
                if (mil < 3)
                    X = StartPoint; // Расчет с самого начала заданного участка
                else
                    X = Convert.ToInt32(Models[mi].Point1.Bar);

            }
            //if (Models.Count < 3)
            //    X = StartPoint; // Расчет с самого начала заданного участка
            //else
            //{
            //    X = Convert.ToInt32(Models[Models.Count - 3].Point1.Bar);
            //}
            ModelsP1 = new List<TModel>();    // Создание массива моделей от т1
            ModelCount1 = 0;
            while (X <= LastPoint)
            //while (X <= StartPoint)
            {
                Dir = Quotes.IsExtremum(X, Options.Extremum1, TDirection.dtUnKnown);
                if ((((Dir == TDirection.dtDn) || (Dir == TDirection.dtDnEqual) || (Dir == TDirection.dtUpDn)) && (Quotes.GetBarByIndex(X).Low < Quotes.GetBarByIndex(X + 1).Low)) ||
                    (((Dir == TDirection.dtUp) || (Dir == TDirection.dtUpEqual) || (Dir == TDirection.dtUpDn)) && (Quotes.GetBarByIndex(X).High > Quotes.GetBarByIndex(X + 1).High)))
                {
                    s = SeekModel(Dir, Quotes, ref ModelsP1, X, LastPoint, DecDigs, Options.CheckedErrors0, Options.CheckedErrors1);
                    if (Options.EnableFiltrationFan)
                        FilterFanOfModel(ref ModelsP1, Options.CheckedErrors1, Options.CheckedErrors2, Options.CheckedErrors3);
                    // Перенос найденных моделей в общий список моделей
                    MoveModelsToList(ModelsP1, ref ModelCount1, ref s);
                    ModelsP1.Clear();
                }
                X++;
                PB++;
                if (worker!=null && (PB % step) == 0)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    Percent = LastPoint > StartPoint ? (int)(100 - 100 * PB / (LastPoint - StartPoint)) : 100;
                    if (Percent < 0) Percent = 0;
                    if (Percent > 100) Percent = 100;
                    worker.ReportProgress(Percent);

                }
            }
            DateTime DTEnd = DateTime.Now;
            Logging.Log("Searching ended. " + Models.Count.ToString() + " models found. Duration time=" + Convert.ToString(DTEnd - DTStart));
            //MainForm.MT4messagesForm.AddMessage("Searching ended. " + Models.Count.ToString() + " models found. Duration time=" + Convert.ToString(DTEnd - DTStart));
            Logging.Log("First step models. " + ModelCount1.ToString() + " models found.");
            //MainForm.MT4messagesForm.AddMessage("First step models. " + ModelCount1.ToString() + " models found.");

            // Второй шаг
//            ProcessStep_2(CheckedRules1);

            MoveModelsToList(HModels, ref ModelCount1, ref s);
            //Models.AddRange(HModels);

            if (Options.EnableFiltrationByTrend)
            {
                FilteringByTrend(Quotes);
            }

            // Построение ГЦ
            SeekHTgL2(Quotes);

            HideHipoteticModels();

            //определение статуса точек и их моделей (глобальные, по тренду, коррекционные)
            CountPointsByStatus(Models, Quotes);

            //MainForm.MT4messagesForm.AddMessage("Searching Hypothesize Tangent Lines ended. " + HTangentLines.Count.ToString() + " lines build.");
            Logging.Log("Processing ended. " + Models.Count.ToString() + " models");
            //MainForm.MT4messagesForm.AddMessage("Processing ended. " + Models.Count.ToString() + " models");
            SeeksComplete = true;
        }

        /*private void ProcessStep_2(TModelError[] CheckedRules1)   //! Обработка второго прохода
        {
            int ModelCount2;
            int X;
            // Если от т1 есть хотя бы одна модель, то построение глобальной модели по касательным
            ModelCount2 = 0;
            Logging.Log("Second step started. ");
            //MainForm.MT4messagesForm.AddMessage("Second step started. ");
            // Сортировка моделей по времени возникновения т1 и т4
            Models.Sort(delegate(TModel M11, TModel M21)
            {
                if (M11.Point1.DT < M21.Point1.DT) return -1;
                else
                    if (M11.Point1.DT > M21.Point1.DT) return 1;
                    else
                        if (M11.Point4.DT < M21.Point4.DT) return -1;
                        else
                            if (M11.Point4.DT > M21.Point4.DT) return 1;
                            else return 0;
            });
            if (Models.Count > 0)
            {
                TModel MFirst;
                TModel MFirstGood;
                TModel MLast;
                ModelCount2 = 0;
                int Point1Bar = Convert.ToInt32(Models[0].Point1.Bar);  // № бара т1
                int Point3Bar = Convert.ToInt32(Models[0].Point3.Bar);  // № бара т3
                MFirst = Models[0];
                if (!CheckRules(MFirst.ErrorCode, MFirst.ModelType, CheckedRules1))
                    MFirstGood = Models[0];
                else
                    MFirstGood = null;
                //                if (Models[0].ModelType == Common.TModelType.ModelOfExpansion)
                MLast = Models[0];
                //                else
                //                    MLast = null;
                Models[0].Step = -1;
                for (int i = 1; i < Models.Count; i++)
                {
                    if ((Point1Bar != Convert.ToInt32(Models[i].Point1.Bar)) ||
                        (Point3Bar != Convert.ToInt32(Models[i].Point3.Bar)))
                    {
                        OnePointFirst(ref MFirst, ref MFirstGood, ref MLast, ref ModelCount2);
                        // Инициализация нового набора моделей от новой т1
                        MFirst = Models[i];
                        if (!CheckRules(Models[i].ErrorCode, Models[i].ModelType, CheckedRules1))
                            MFirstGood = Models[i];
                        else
                            MFirstGood = null;
                        //                        if (Models[i].ModelType == Common.TModelType.ModelOfExpansion)
                        MLast = Models[i];
                        //                        else
                        //                            MLast = null;
                        Point1Bar = Convert.ToInt32(Models[i].Point1.Bar);
                        Point3Bar = Convert.ToInt32(Models[i].Point3.Bar);
                    }
                    else
                    {
                        if ((!CheckRules(Models[i].ErrorCode, Models[i].ModelType, CheckedRules1)) &&
                            (MFirstGood == null))
                            MFirstGood = Models[i];
                        //                        else
                        //                            MFirstGood = null;
                        if (Models[i].ModelType != Common.TModelType.ModelOfAttraction)
                            MLast = Models[i];
                    }
                }
                if (Point1Bar == Convert.ToInt32(Models[Models.Count - 1].Point1.Bar))
                    OnePointFirst(ref MFirst, ref MFirstGood, ref MLast, ref ModelCount2);
                Logging.Log("Second step ended. " + ModelCount2.ToString() + " models");
                //MainForm.MT4messagesForm.AddMessage("Second step ended. " + ModelCount2.ToString() + " models");
            }
            // Удаление "лишних" моделей, не вошедших ни в 1-й ни во 2-й проходы
            X = 0;
            while (X < Models.Count)
            {
                if (Models[X].Step == -1)
                {
                    Models.RemoveAt(X);
                }
                else
                    X++;
            }
        }*/

        public void SeekModelsSeq(TQuotes Quotes, int StartPoint, int LastPoint, int DecDigs, System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)          //!  Метод нахождения последовательных моделей от указанной точки
        {
            int X;
            List<TModel> ModelsP1;  // Список моделей построенных от одной т1
            int s = 1;
            TDirection Dir = TDirection.dtUnKnown;
            int PB = 0;             // Кол-во обработанных баров;
            int step = (int)((LastPoint - StartPoint) / 100f);
            int Percent;            // % обработанных баров
            int ModelCount1 = 0;    // Кол-во моделей на 1-м шаге
            //int ModelCount2 = 0;    // Кол-во моделей на 2-м шаге
            int LastProcessedBar;    // № последнего обработанного бара
            int CircleNum;          // № смещения для построения начала последовательности

            DateTime DTStart = DateTime.Now;
            Logging.Log("Searching started");
            //MainForm.MT4messagesForm.AddMessage("Searching started");
            if (step == 0) step = 1;
            X = StartPoint;
            ModelsP1 = new List<TModel>();    // Создание массива моделей от т1
            ModelCount1 = 0;
            LastProcessedBar = 0;
            CircleNum = 0;
            while ((LastProcessedBar < Quotes.GetCount() - 1) && (CircleNum < 3))
            {
                CircleNum++;
                // Поиск первого веера моделей для построения последовательности
                while ((X <= LastPoint) && (ModelsP1.Count == 0))
                {
                    Dir = Quotes.IsExtremum(X, Options.Extremum1, TDirection.dtUnKnown);
                    if ((((Dir == TDirection.dtDn) || (Dir == TDirection.dtDnEqual) || (Dir == TDirection.dtUpDn)) && (Quotes.GetBarByIndex(X).Low < Quotes.GetBarByIndex(X + 1).Low)) ||
                        (((Dir == TDirection.dtUp) || (Dir == TDirection.dtUpEqual) || (Dir == TDirection.dtUpDn)) && (Quotes.GetBarByIndex(X).High > Quotes.GetBarByIndex(X + 1).High)))
                    {
                        // Построение веера моделей от указаной т1
                        s = SeekModel(Dir, Quotes, ref ModelsP1, X, LastPoint, DecDigs, Options.CheckedErrors0, Options.CheckedErrors1);
//                        if (Options.EnableFiltrationFan)
//                            FilterFanOfModel(ref ModelsP1, Options.CheckedErrors1, Options.CheckedErrors2);
                    }
                    X++;
                    PB++;
                    if (worker != null && (PB % step) == 0)
                    {
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                        Percent = (int)(100 - 100 * PB / (LastPoint - StartPoint));
                        if (Percent < 0) Percent = 0;
                        if (Percent > 100) Percent = 100;
                        worker.ReportProgress(Percent);
                    }
                }
                // Перенос найденных моделей в общий список моделей
                MoveModelsToList(ModelsP1, ref ModelCount1, ref LastProcessedBar);

                // Построение моделей от реперных точек найденных
                int j = 0;
                while (j < Models.Count)
                {
                    // от т6
                    // Пропуск равных баров
                    X = Convert.ToInt32(Models[j].Point6.Bar);
                    if (Models[j].ModelDir == TDirection.dtUp)
                    {
                        while (Quotes.GetBarByIndex(X + 1).High == Models[j].Point6.Price) X++;
                    }
                    else
                    {
                        while (Quotes.GetBarByIndex(X + 1).Low == Models[j].Point6.Price) X++;
                    }
                    Dir = Quotes.IsExtremum(X, Options.Extremum1, TDirection.dtUnKnown);
                    if ((((Dir == TDirection.dtDn) || (Dir == TDirection.dtDnEqual) || (Dir == TDirection.dtUpDn)) && (Quotes.GetBarByIndex(X).Low < Quotes.GetBarByIndex(X + 1).Low)) ||
                        (((Dir == TDirection.dtUp) || (Dir == TDirection.dtUpEqual) || (Dir == TDirection.dtUpDn)) && (Quotes.GetBarByIndex(X).High > Quotes.GetBarByIndex(X + 1).High)))
                    {
                        // Построение веера моделей от указаной т1
                        s = SeekModel(Dir, Quotes, ref ModelsP1, X, LastPoint, DecDigs, Options.CheckedErrors0, Options.CheckedErrors1);
                        if (Options.EnableFiltrationFan)
                            FilterFanOfModel(ref ModelsP1, Options.CheckedErrors1, Options.CheckedErrors2, Options.CheckedErrors3);

                        // Перенос найденных моделей в общий список моделей
                        MoveModelsToList(ModelsP1, ref ModelCount1, ref LastProcessedBar);
                    }
                    // от т4
                    // Пропуск равных баров
                    X = Convert.ToInt32(Models[j].Point4.Bar);
                    if (Models[j].ModelDir == TDirection.dtUp)
                    {
                        while (Quotes.GetBarByIndex(X + 1).High == Models[j].Point4.Price) X++;
                    }
                    else
                    {
                        while (Quotes.GetBarByIndex(X + 1).Low == Models[j].Point4.Price) X++;
                    }
                    Dir = Quotes.IsExtremum(X, Options.Extremum1, TDirection.dtUnKnown);
                    if ((((Dir == TDirection.dtDn) || (Dir == TDirection.dtDnEqual) || (Dir == TDirection.dtUpDn)) && (Quotes.GetBarByIndex(X).Low < Quotes.GetBarByIndex(X + 1).Low)) ||
                        (((Dir == TDirection.dtUp) || (Dir == TDirection.dtUpEqual) || (Dir == TDirection.dtUpDn)) && (Quotes.GetBarByIndex(X).High > Quotes.GetBarByIndex(X + 1).High)))
                    {
                        // Построение веера моделей от указаной т1
                        s = SeekModel(Dir, Quotes, ref ModelsP1, X, LastPoint, DecDigs, Options.CheckedErrors0, Options.CheckedErrors1);
                        if (Options.EnableFiltrationFan)
                            FilterFanOfModel(ref ModelsP1, Options.CheckedErrors1, Options.CheckedErrors2, Options.CheckedErrors3);
                        // Перенос найденных моделей в общий список моделей
                        MoveModelsToList(ModelsP1, ref ModelCount1, ref LastProcessedBar);
                    }
                    // от т2
                    // Пропуск равных баров
                    X = Convert.ToInt32(Models[j].Point2.Bar);
                    if (Models[j].ModelDir == TDirection.dtUp)
                    {
                        while (Quotes.GetBarByIndex(X + 1).High == Models[j].Point2.Price) X++;
                    }
                    else
                    {
                        while (Quotes.GetBarByIndex(X + 1).Low == Models[j].Point2.Price) X++;
                    }
                    Dir = Quotes.IsExtremum(X, Options.Extremum1, TDirection.dtUnKnown);
                    if ((((Dir == TDirection.dtDn) || (Dir == TDirection.dtDnEqual) || (Dir == TDirection.dtUpDn)) && (Quotes.GetBarByIndex(X).Low < Quotes.GetBarByIndex(X + 1).Low)) ||
                        (((Dir == TDirection.dtUp) || (Dir == TDirection.dtUpEqual) || (Dir == TDirection.dtUpDn)) && (Quotes.GetBarByIndex(X).High > Quotes.GetBarByIndex(X + 1).High)))
                    {
                        // Построение веера моделей от указаной т1
                        s = SeekModel(Dir, Quotes, ref ModelsP1, X, LastPoint, DecDigs, Options.CheckedErrors0, Options.CheckedErrors1);
                        if (Options.EnableFiltrationFan)
                            FilterFanOfModel(ref ModelsP1, Options.CheckedErrors1, Options.CheckedErrors2, Options.CheckedErrors3);
                        // Перенос найденных моделей в общий список моделей
                        MoveModelsToList(ModelsP1, ref ModelCount1, ref LastProcessedBar);
                    }
                    j++;
                    PB = LastProcessedBar;
                    if (worker != null && (PB % step) == 0)
                    {
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                        Percent = (int)(100 - 100 * PB / (LastPoint - StartPoint));
                        if (Percent < 0) Percent = 0;
                        if (Percent > 100) Percent = 100;
                        worker.ReportProgress(Percent);
                    }
                }
                X = LastProcessedBar;
            }

            // Сортировка моделей по времени возникновения т1 и т4
            Models.Sort(delegate(TModel M11, TModel M21)
            {
                if (M11.Point1.DT < M21.Point1.DT) return -1;
                else
                    if (M11.Point1.DT > M21.Point1.DT) return 1;
                    else
                        if (M11.Point4.DT < M21.Point4.DT) return -1;
                        else
                            if (M11.Point4.DT > M21.Point4.DT) return 1;
                            else return 0;
            });

            DateTime DTEnd = DateTime.Now;
            Logging.Log("Searching ended. " + Models.Count.ToString() + " models found. Duration time=" + Convert.ToString(DTEnd - DTStart));
            Logging.Log("First step models. " + ModelCount1.ToString() + " models found.");

//            ProcessStep_2(CheckedRules1);

            // Построение ГЦ
            SeekHTgL2(Quotes);

            //определение статуса точек и их моделей (глобальные, по тренду, коррекционные
            CountPointsByStatus(Models, Quotes);

            Logging.Log("Processing ended. " + Models.Count.ToString() + " models");
        }

        private void MoveModelsToList(List<TModel> ModelsP1, ref int ModelCount1, ref int LastProcessedBar)    //! Перенос моделей из ModelsP1 в общий список моделей Models
        {
            int MIdx;
            foreach (TModel M in ModelsP1)
            {
                if (M.Step != -1)
                {  // Перенос моделей и ModelsP1 в Models
                    switch (M.ModelType)
                    {
                        case Common.TModelType.ModelOfAttraction:
                            MIdx = IsModelExistsInList(Models, M, 4);
                            if (MIdx == -1)
                            {
                                M.ModelID = Models.Count;
                                Models.Add(M);
                                if (M.Step == 1)
                                    ModelCount1++;
                            }
                            else
                            {
                                M.ModelID = MIdx;
                                Models[MIdx] = M;
                            }
                            break;
                        case Common.TModelType.ModelOfBalance:
                            MIdx = IsModelExistsInList(Models, M, 4);
                            if (MIdx == -1)
                            {
                                M.ModelID = Models.Count;
                                Models.Add(M);
                                if (M.Step == 1)
                                    ModelCount1++;
                            }
                            else
                            {
                                M.ModelID = MIdx;
                                Models[MIdx] = M;
                            }
                            break;
                        case Common.TModelType.ModelOfExpansion:
                            MIdx = IsModelExistsInList(Models, M, 4);
                            if (MIdx == -1)
                            {
                                M.ModelID = Models.Count;
                                Models.Add(M);
                                if (M.Step == 1)
                                    ModelCount1++;
                            }
                            else
                            {
                                M.ModelID = MIdx;
                                Models[MIdx] = M;
                            }
                            break;
                    }
                }
                if (LastProcessedBar < M.ProcessedBar)
                    LastProcessedBar = Convert.ToInt32(M.Point6.Bar + 1);
            }
            ModelsP1.Clear();
        }


        private void OnePointFirst(ref TModel MFirst, ref TModel MFirstGood, ref TModel MLast, ref int ModelCount2)   // Обработка множества моделей от т1
        {
            if ((MFirst == MFirstGood) && (MFirstGood == MLast))
            {   // Только одна модель от т1 и она правильная. 
                // Рисование MFirstGood
                if (MFirstGood != null) MFirst.Step = 1;
            }
            else
            {
                if ((MFirst == MFirstGood) && (MFirst != MLast))
                {   // Две и более моделей от т1. Первая модель правильная.
                    // Рисование MFirstGood и MLast по касательным
                    if (MFirst != null) MFirstGood.Step = 1;
                    if ((MLast != null) && (MLast.Step == -1) && (MLast.ModelType != Common.TModelType.ModelOfAttraction))
                    {
//                        MLast.CalcTangentLines();
//                        MLast.IsModelCorrect();
                        if (!CheckRules(MLast.ErrorCode, MLast.ModelType, Options.CheckedErrors0))
                        {
                            MLast.Step = 4;
                            ModelCount2++;
                        }
                        else
                        {
                            MLast.Step = -1;
                        }
                    }
                }
                else
                {
                    if ((MFirst != MFirstGood) && (MFirst == MLast))
                    {   // Есть одна, но неправильная
                        // Рисование MFirst по касательным
                        if ((MFirst != null) && (MFirst.Step == -1))
                        {
//                            MFirst.CalcTangentLines();
//                            MFirst.IsModelCorrect();
                            if (!CheckRules(MFirst.ErrorCode, MFirst.ModelType, Options.CheckedErrors3))
                            {
                                MFirst.Step = 2;
                                ModelCount2++;
                            }
                            else
                            {
                                MFirst.Step = -1;
                            }
                        }
                    }
                    else
                    {
                        if ((MFirst != MFirstGood) && (MFirst != MLast))
                        {   // Есть две или более моделей от т1. Нет правильных моделей
                            // Рисование MFirst как есть. Рисование MLast по касательным
                            if ((MFirst != null) && (MFirst.Step == -1))
                            {
                                MFirst.Step = 2;
                                ModelCount2++;
                            }
                            if ((MLast != null) && (MLast != MFirstGood) && (MLast.Step == -1) &&
                                (MLast.ModelType != Common.TModelType.ModelOfAttraction))
                            {
//                                MLast.CalcTangentLines();
//                                MLast.IsModelCorrect();
                                if (!CheckRules(MLast.ErrorCode, MLast.ModelType, Options.CheckedErrors0))
                                {
                                    MLast.Step = 4;
                                    ModelCount2++;
                                }
                                else
                                {
                                    MLast.Step = -1;
                                }
                            }
                            else
                                if ((MLast == MFirstGood) && (MFirstGood != null))
                                {
                                    MFirstGood.Step = 1;
                                }
                        }
                    }
                }
            }
        }

        private void FilterFanOfModel(ref List<TModel> ModelsP1, TModelError[] CheckedRules1, TModelError[] CheckedRules2, TModelError[] CheckedRules3) //! Фильтрация веера моделей
        {
            TModel MFirst;
            TModel MFirstGood;
            TModel MLast;
            int X = 0;
            if (ModelsP1.Count == 0 /*|| ModelsP1[0].CurrentPoint == 4*/) return;
            // Разноска моделей по проходам
            // Определение первой, первой правильной и глобальных моделей
            int Point1Bar = Convert.ToInt32(ModelsP1[0].Point1.Bar);  // № бара т1
            int Point3Bar = Convert.ToInt32(ModelsP1[0].Point3.Bar);  // № бара т3
            int Point4Bar = Convert.ToInt32(ModelsP1[0].Point41.Bar);  // № бара т4
            //ModelsP1[0].IsModelCorrect();
            ModelsP1[0].Step = -1;
            if (!CheckRules(ModelsP1[0].ErrorCode, ModelsP1[0].ModelType, CheckedRules2))
                MFirst = ModelsP1[0];
            else
                MFirst = null;
            if (!CheckRules(ModelsP1[0].ErrorCode, ModelsP1[0].ModelType, CheckedRules1))
            {
                MFirstGood = ModelsP1[0];
                MFirstGood.Step = 1;
            }
            else
                MFirstGood = null;
            if (!CheckRules(ModelsP1[0].ErrorCode, ModelsP1[0].ModelType, CheckedRules3))
                MLast = ModelsP1[0];
            else
                MLast = null;

            // Фильтрация веера моделей, могут остаться только две модели
            for (int i = 1; i < ModelsP1.Count; i++)
            {
                ///if(ModelsP1[i].CurrentPoint == 4) continue;
                //ModelsP1[i].IsModelCorrect();
                ModelsP1[i].Step = -1;
                if ((Point1Bar != Convert.ToInt32(ModelsP1[i].Point1.Bar)) ||
                    (Point3Bar != Convert.ToInt32(ModelsP1[i].Point3.Bar)))
                {
                    OnePointFirst(ref MFirst, ref MFirstGood, ref MLast, ref X);
                    // Инициализация нового набора моделей от новой Базы
                    if (!CheckRules(ModelsP1[i].ErrorCode, ModelsP1[i].ModelType, CheckedRules2))
                        MFirst = ModelsP1[i];
                    else
                        MFirst = null;
                    if (!CheckRules(ModelsP1[i].ErrorCode, ModelsP1[i].ModelType, CheckedRules1))
                    {
                        MFirstGood = ModelsP1[i];
                        MFirstGood.Step = 1;
                    }
                    else
                        MFirstGood = null;
                    if (!CheckRules(ModelsP1[i].ErrorCode, ModelsP1[i].ModelType, CheckedRules3))
                        MLast = ModelsP1[i];
                    else
                        MLast = null;
                    Point1Bar = Convert.ToInt32(ModelsP1[i].Point1.Bar);
                    Point3Bar = Convert.ToInt32(ModelsP1[i].Point3.Bar);
                    Point4Bar = Convert.ToInt32(ModelsP1[i].Point41.Bar);
                }
                else
                {
                    if ((!CheckRules(ModelsP1[i].ErrorCode, ModelsP1[i].ModelType, CheckedRules1)) &&
                        (MFirstGood == null))
                    {
                        MFirstGood = ModelsP1[i];
                        MFirstGood.Step = 1;
                    }
                    if (((Point4Bar < ModelsP1[i].Point41.Bar) || (MLast == null)) &&
                        (!CheckRules(ModelsP1[i].ErrorCode, ModelsP1[i].ModelType, CheckedRules3)))
                    {
                        MLast = ModelsP1[i];
                        Point4Bar = Convert.ToInt32(ModelsP1[i].Point41.Bar);
                    }
                }
            }
            if (Point1Bar == Convert.ToInt32(ModelsP1[ModelsP1.Count - 1].Point1.Bar))
                OnePointFirst(ref MFirst, ref MFirstGood, ref MLast, ref X);
        }

        /// <summary>
        /// сравнение моделей по размеру 1-6
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns>соотношение M1/M2, 1 если модели равны, 0 если не определен тип сравнения</returns>
        double ModelWidthCompare(TModel m1, TModel m2)
        {
            double Base1 = m1.Point6.Bar == 0 ? 1000000 : m1.Point6.Bar - m1.Point1.Bar +1;
            double Base2 = m2.Point6.Bar == 0 ? 1000000 : m2.Point6.Bar - m2.Point1.Bar +1;
            return Base1 / Base2;
        }

        /// <summary>
        /// идентификация моделей по тренду или от начала тренда для своего Плана или коррекционных.
        /// </summary>
        /// <param name="ModelsP1"></param>
        /// <param name="q"></param>
        void ModelsIntrendStatus(List<TModel> models, TQuotes q)
        {
            List<mChannel> mactive = new List<mChannel>();
            for (int qi = 0, mi = 0; qi < q.GetCount(); qi++)
            {
                //1. изначально все модели лежат правее текущей точки, переводим исследуемую точку на ближайшую т1
                while (mi < models.Count && models[mi].Point1.iBar < qi) mi++;

                //3. если время жизни активной модели < текущей точки -- конец цикла для данной модели
                for (int i = mactive.Count - 1; i >= 0; i--)
                    if (mactive[i].LifeTime < qi) mactive.RemoveAt(i);
                    //иначе продвигаем уровни линий тренда и целей на текущий бар => //4. для активных моделей вычисление уровней ЛЦ и ЛТ(resist|support)
                    else mactive[i].incpoint();

                try
                {
                    //2. затем часть моделей включаются в список активных 
                    // если 1 точка равна текущей -- начало цикла для данной модели     
                    while (mi < models.Count && models[mi].Point1.iBar == qi)
                    {
                        TModel mc = models[mi];  //mc -- испытуемая
                        if (mc.ModelID > 0)
                        {
                            mc.isBytrendOfID = mc.isCorrectionOfID = -1;
                            mc.Stat &= ~((uint)MStat.GlobalP1 & (uint)MStat.GlobalTrend); //очистка флагов GlobalTrend и GlobalP1
                            if (mc.TimeFrame == q.tf)        //учитываем только модели текущего плана
                            {
                                //5. проверка вложенности для активных моделей (глобальные выступают в роли активных m)
                                foreach (mChannel m in mactive)// m -- глобальная
                                {
                                    if (m.model.Point1.iBar >= mc.Point1.iBar) break;
                                    if (m.model.ModelType == Common.TModelType.ModelOfExpansion)
                                    {
                                        if (m.model.TimeFrame == q.tf  // фильтр только по отношению к моделям текущего плана
                                         && ModelWidthCompare(m.model, mc) < 3 // размер базовой модели в барах не более чем в 3 раза превышает размер текущей
                                         && m.resistence >= mc.Point1.Price && m.support <= mc.Point1.Price) //допускается касание линий, this.t1==owner.t3 for example
                                        {
                                            if (m.model.ModelDir == mc.ModelDir)    //модель по тренду если сонаправлена и расположена в любом месте от т1 до 1-4х4
                                            {
                                                mc.isBytrendOfID = m.model.ModelID;

                                                if (mc.Point1.Bar == m.model.Point3.Bar && (m.model.Stat & (uint)MStat.GlobalP1) != 0)
                                                    mc.Stat |= (uint)(mc.ModelType == Common.TModelType.ModelOfExpansion ? MStat.valid : MStat.invalid);
                                            }

                                            else if (mc.Point1.Bar < m.model.Point6.Bar
                                                  || mc.Point1.Bar < m.model.Point65.Bar
                                                //|| m.model.BreakTrendLine.Bar == 0
                                                ) //модель коррекционная если расположена до появления т6 либо т6 не зафиксирована пробоем трендовой
                                            {
                                                mc.isCorrectionOfID = m.model.ModelID;

                                                //if (mc.Point1.Bar == m.model.Point2.Bar
                                                //|| mc.Point1.Bar == m.model.Point4.Bar
                                                //|| mc.Point1.Bar == m.model.Point6.Bar) mc.Stat |= (uint)MStat.valid;
                                            }
                                            //else if (mc.Point1.Bar == m.model.Point6.Bar)
                                            //{
                                            //    mc.Stat |= (uint)MStat.GlobalP1;
                                            //    mc.Stat |= (uint)MStat.valid;
                                            //}
                                        }
                                        
                                        if (mc.Point1.Bar == m.model.Point2.Bar
                                         || mc.Point1.Bar == m.model.Point4.Bar
                                         || mc.Point1.Bar == m.model.Point6.Bar
                                         ||(mc.Point1.Bar == m.model.Point65.Bar && mc.ModelType != Common.TModelType.ModelOfAttraction)
                                            )
                                            //для МП проверка размерности коррекционная МП не может быть больше половины модели от которой построена
                                            if(mc.ModelType != Common.TModelType.ModelOfAttraction
                                            || ((m.model.Point6.Bar - m.model.PointSP1.Bar) / (mc.PointSP1.Bar - mc.Point1.Bar)) >= 2
                                            || m.model.Point6.Bar == 0)

                                                mc.Stat |= (uint)MStat.valid;
                                        
                                        if (mc.Point1.Bar == m.model.Point6.Bar
                                        ||  mc.Point1.Bar == m.model.Point65.Bar
                                            )
                                        {
                                            mc.Stat |= (uint)MStat.GlobalP1;
                                        }
                                    }
                                    else //от МП и МДР проверяем только пересечения точек 1 и 6 и только для МР
                                        if (mc.ModelType == Common.TModelType.ModelOfExpansion)
                                    {
                                        if (mc.Point1.Bar == m.model.Point6.Bar
                                        || mc.Point1.Bar == m.model.Point65.Bar)
                                        {
                                            mc.Stat |= (uint)MStat.GlobalP1;
                                            mc.Stat |= (uint)MStat.valid;
                                        }
                                        //else if (mc.Point1.Bar == m.model.Point2.Bar || mc.Point1.Bar == m.model.Point4.Bar)
                                        //{
                                        //    mc.Stat |= (uint)MStat.valid;
                                        //    mc.isCorrectionOfID = m.model.ModelID;
                                        //}
                                        //else if (mc.Point1.Bar == m.model.Point3.Bar && mc.ModelType == Common.TModelType.ModelOfExpansion)
                                        //{
                                        //    mc.Stat |= (uint)MStat.valid;
                                        //    mc.isBytrendOfID = m.model.ModelID;
                                        //}
                                    }
                                }
                                if (mc.isBytrendOfID == -1 && mc.isCorrectionOfID == -1 
                                    && mc.ModelType == Common.TModelType.ModelOfExpansion
                                    )
                                {
                                    //глбальная МР
                                    mc.Stat |= (uint)MStat.GlobalP1;
                                    mc.Stat |= (uint)MStat.valid;
                                }

                                if ((mc.Stat & (uint)MStat.valid) != 0)
                                {
                                    //теперь, после проверки, эта модель включается в список активных
                                    mactive.Add(new mChannel(mc));
                                }
                            }
                            else
                                mc.Stat |= (uint)MStat.valid; //модели старших планов уже отфильтрованы

                            if ((mc.Stat & (uint)MStat.valid) != 0)
                            {
                                //теперь, после проверки, эта модель включается в список активных
                                mactive.Add(new mChannel(mc));
                            }
                        }
                        mi++;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("идентификация моделей по тренду:" + e.Message);
                }
            }
        }
        /*
         * 1. модель вложена
         * 2. по тренду + т1==т3 ? valid = true
         * 
         *
         */

        private void FilteringByTrend(TQuotes q) //! Фильтрация моделей по тренду
        {
            //int X;
            //TModel M1;
            //TModel M2;
            Logging.Log("Filtering started. ");
            //MainForm.MT4messagesForm.AddMessage("Filtering started. ");
            List<TModel> ModelsP1 = new List<TModel>();
            ModelsP1.AddRange(Models);
            Models.Clear();
            // Сортировка моделей по времени возникновения т1 и т4
            ModelsP1.Sort(delegate(TModel M11, TModel M21)
            {
                if (M11.Point1.DT < M21.Point1.DT) return -1;
                else
                    if (M11.Point1.DT > M21.Point1.DT) return 1;
                    else
                        if (M11.Point4.Bar > 0 && M11.Point4.DT < M21.Point4.DT) return -1;
                        else
                            if (M11.Point4.Bar == 0 || M11.Point4.DT > M21.Point4.DT) return 1;
                            else return 0;
            });

            //проверка вложенности моделей из их связей, 
            ModelsIntrendStatus(ModelsP1, q);

            //теперь можно фильтровать по признакам isBytrendOfID, isCorrectionOfID, Stat
            foreach (TModel m in ModelsP1)
            {
                if (m.Step > 0 && (m.Stat & (uint)MStat.valid) != 0 && (m.Stat & (uint)MStat.invalid) == 0)
                {
                    if (Models.Count > 0 && Models[Models.Count - 1].Point1.Bar < m.Point1.Bar)//последняя из моделей, отсортированных по 1+4, от данной т1
                        m.Stat |= (uint)MStat.GlobalTrend;

                    m.ModelID = Models.Count;
                    Models.Add(m);
                }
            }

            Logging.Log("Filtering ended. ");
        }

        private int SeekLastModelByPoint4(double Point4Bar) //! Поиск предыдущей модели для заданной т4
        {
            int R = 0;
            return R;
        }

        private void SeekHTgL(TQuotes Quotes) //! Построение гипотетических целевых линий
        {
            TModel M11;
            TModel M21;
            List<TModel> ML = new List<TModel>();
            int Last;
            int Prev;
            Logging.Log("Searching Hypothesize Tangent Lines started. ");
            ML.AddRange(Models);
            ML.Sort(delegate(TModel M1, TModel M2)
            {
                if (M1.Point4.DT < M2.Point4.DT) return -1;
                else
                    if (M1.Point4.DT > M2.Point4.DT) return 1;
                    else
                        return 0;
            });
            // Поиск самой первой модели для последней т1
            Last = Models.Count - 1;
            while (Last > 0)
            {
                while ((Last > 1) && (Models[Last].Point1.Bar == Models[Last - 1].Point1.Bar))
                {
                    Last--;
                }
                // Поиск самой последней модели у которой т6=т1 последующей 
                Prev = Last - 1;
                while ((Prev > 0)/* && (ML[Prev].Point4.Bar > Models[Last].Point1.Bar)*/)
                {
                    if ((ML[Prev].Point4.Bar < Models[Last].Point1.Bar) &&
                        (ML[Prev].Point4.Bar < ML[Prev].Point6.Bar) &&
                        (ML[Prev].ModelDir != Models[Last].ModelDir))
                        break;
                    Prev--;
                }
                if (Prev < 0) break;
                M11 = ML[Prev];
                M21 = Models[Last];
                // Создание ГЦ
                THTangentLine HTgL = new THTangentLine(Quotes, M11, M21);
                if (M21.ModelDir != M11.ModelDir)
                    HTgL.Step = 1;
                else
                    HTgL.Step = 2;
                HTangentLines.Add(HTgL);
                //ассоциируем данную ГЦ с моделью для которой она построена
                Models[Last].HTi = HTangentLines.Count - 1;
                //привязка этой ГЦ к остальным моделям с такой же базой
                for (int i = Last;
                    i < Models.Count
                    && Models[i].Point1.Bar == Models[Last].Point1.Bar
                    && Models[i].Point3.Bar == Models[Last].Point3.Bar;
                    i++)
                    Models[i].HTi = HTangentLines.Count - 1;
                Last--;
            }

            /*            for (int i = 0; i < Models.Count; i++)
                        {
                            M11 = Models[i];
                            // Поиск первой следующей модели у которой Point1 позже Point4 обрабатываемой
                            for (int j = i+1; j < Models.Count; j++)
                            {
                                M21 = Models[j];
                                if (M21.Point1.Bar >= M11.Point4.Bar)
                                {
                                    // Создание ГЦ
                                    THTangentLine HTgL = new THTangentLine(Quotes, M11, M21);
                                    if (M21.ModelDir != M11.ModelDir)
                                        HTgL.Step = 1;
                                    else
                                        HTgL.Step = 2;
                                    HTangentLines.Add(HTgL);
                                    break;
                                }
                            }
                        }*/
            Logging.Log("Searching Hypothesize Tangent Lines ended. " + HTangentLines.Count.ToString() + " lines build.");
        }

        // версия построения ГЦ by skat
        private void SeekHTgL2(TQuotes Quotes) //! Построение гипотетических целевых линий
        {
            TModel M11;
            TModel M21;
            List<TModel> ML = new List<TModel>(),
                         models = Models;
            int Last;
            int Prev;
            Logging.Log("Searching Hypothesize Tangent Lines started. ");
            ML.AddRange(models);
            ML.Sort(delegate(TModel M1, TModel M2)
            {
                if (M1.Point4.DT < M2.Point4.DT) return -1;
                else
                    if (M1.Point4.DT > M2.Point4.DT) return 1;
                    else
                        return 0;
            });
            // Поиск самой первой модели для последней т1-т3
            Prev = Last = models.Count;
            while (Last > 0)
            {
                Last--;
                while ((Last > 0) && (models[Last].Point1.Bar == models[Last - 1].Point1.Bar)
                                  && (models[Last].Point3.Bar == models[Last - 1].Point3.Bar))  Last--;

                // Поиск самой последней модели у которой т6=т1 последующей 
                // AS: почему именно такие модели где т6=т1?
                // имхо нужна предыдущая т4 < т1 модели, для которой ищется ГЦ
                while ((Prev < ML.Count) && ML[Prev].Point4.Bar < models[Last].Point1.Bar) Prev++;
                while (Prev > 0)
                {
                    Prev--;
                    if (ML[Prev].Point4.Bar < models[Last].Point1.Bar
                  // && ML[Prev].Point4.Bar != ML[Prev].Point6.Bar
                  // && ML[Prev].TimeFrame == ML[Prev].TimeFrame     // возможно полезным будет включить фильтрацию по ТФ
                     && ML[Prev].ModelDir != models[Last].ModelDir
                    ){
                        M11 = ML[Prev];
                        M21 = models[Last];
                        // Создание ГЦ
                        THTangentLine HTgL = new THTangentLine(Quotes, M11, M21);
                        if (M21.ModelDir != M11.ModelDir)
                            HTgL.Step = 1;
                        else
                            HTgL.Step = 2;
                        HTangentLines.Add(HTgL);
                        //ассоциируем данную ГЦ с моделью для которой она построена
                        models[Last].HTi = HTangentLines.Count - 1;
                        //привязка этой ГЦ к остальным моделям с такой же базой
                        for (int i = Last;
                            i < models.Count
                            && models[i].Point1.Bar == models[Last].Point1.Bar
                            && models[i].Point3.Bar == models[Last].Point3.Bar;
                            i++)
                            models[i].HTi = HTangentLines.Count - 1;
                        break;
                    }
                }
                
                //также поиск ГЦ для формирующихся моделей
                //if (Last == 0 && models == Models)
                //{
                //    models = HModels;
                //    Last = HModels.Count;
                //}

            }

            Logging.Log("Searching Hypothesize Tangent Lines ended. " + HTangentLines.Count.ToString() + " lines build.");
        }

        void HideHipoteticModels()
        {
            foreach(TModel m in HModels) 
                Models.Remove(m);
        }

        public int SeekModel(TDirection ADir, TQuotes Quotes, ref List<TModel> ModelsP1, int StartBar, int LastBar, int DecDigs, TModelError[] CheckedRules0, TModelError[] CheckedRules1)
        //!      Поиск моделей от указанного бара(StartBar) из массива котировок(Quotes) до LastBar
        //!      Найденные модели в ModelsP
        {
            int X;
            int i;
            TBar Bar = new TBar();
            TModel M;
            TModel MM;

            bool P1IsAlive;         // Признак живизны :) т1
            bool TgLineBreakOut;    // Признак пробития ЛЦ'
            int CPPrev;             // Номер точки модели, обработанной на предыдущем баре
            TDirection Dir;
            X = StartBar;
            // Создание заготовок моделей
            Bar = Quotes.GetBarByIndex(X);
            switch (ADir)
            {
                case TDirection.dtDn:
                case TDirection.dtDnEqual:
                    {
                        if (Quotes.GetBarByIndex(X).Low < Quotes.GetBarByIndex(X + 1).Low)
                        {
                            M = new TModel(Bar, TDirection.dtUp, Quotes, -1, -1, -1, DecDigs);
                            M.ProcessedBar = X;
                            ModelsP1.Add(M);
                        }
                    }
                    break;
                case TDirection.dtUp:
                case TDirection.dtUpEqual:
                    {
                        if (Quotes.GetBarByIndex(X).High > Quotes.GetBarByIndex(X + 1).High)
                        {
                            M = new TModel(Bar, TDirection.dtDn, Quotes, -1, -1, -1, DecDigs);
                            M.ProcessedBar = X;
                            ModelsP1.Add(M);
                        }
                    }
                    break;
                case TDirection.dtUpDn:
                    {
                        if (Quotes.GetBarByIndex(X).High > Quotes.GetBarByIndex(X + 1).High)
                        {
                            M = new TModel(Bar, TDirection.dtDn, Quotes, -1, -1, -1, DecDigs);
                            M.ProcessedBar = X;
                            ModelsP1.Add(M);
                        }
                        if (Quotes.GetBarByIndex(X).Low < Quotes.GetBarByIndex(X + 1).Low)
                        {
                            M = new TModel(Bar, TDirection.dtUp, Quotes, -1, -1, -1, DecDigs);
                            M.ProcessedBar = X;
                            ModelsP1.Add(M);
                        }
                    }
                    break;
            }
            P1IsAlive = true;
            X++;
            while (P1IsAlive && (X <= LastBar))
            {
                P1IsAlive = false;
                Bar = Quotes.GetBarByIndex(X);
                i = 0;
                while (i < ModelsP1.Count)
                {   // Просмотр списка моделей
                    M = ModelsP1[i];
                    CPPrev = M.CurrentPoint;    // Получение состояния модели до обработки
                    TgLineBreakOut = M.TargetLineBreakOut;
                    // Передача бара на обработку модели
                    if (M.IsAlive && (M.ProcessBar(Bar) != 0))
                    {   // Обработка завершилась неудачно
                        M.IsAlive = false;
                    }
                    // Проверка на пробой уровня т2 и построение ГЦ
                    if ((CPPrev == 3) && (M.CurrentPoint == 4))
                    {
                        // Создание ГЦ
//                        THTangentLine HTgL = new THTangentLine(Quotes, M11, M21);
                    }
                    P1IsAlive = P1IsAlive || M.IsAlive;
//                    if (TgLineBreakOut != M.TargetLineBreakOut)
                    if ((CPPrev <= 4) && (M.CurrentPoint > 4) && (M.CurrentPoint < 99))
                    {   // Модель определила 4-ре точки
                        //M.IsModelCorrect();
                        MM = new TModel(Quotes, -1, M, DecDigs, 4);
                        if (IsModelExistsInList(ModelsP1, MM, 4) == -1)
                        {
//                            M.Step = -1;
                            ModelsP1.Add(MM);
                        }
                    }
                    if ((CPPrev < 99) && (M.CurrentPoint == 99))
                    {   // Произошло пробитие ЛТ модели 
                        // При пробое ЛТ корректность модели более не проверялась
                        // а мы ее возьмем да и проверим
                        //if (M.ModelType == Common.TModelType.ModelOfAttraction) M.IsModelCorrect();

                        // Проверка на экстремальность т1 до обрабатываемой точки
                        Dir = Quotes.IsExtremum(Convert.ToInt32(M.Point1.Bar), Convert.ToInt32((Bar.Bar - M.Point1.Bar) * Options.SizePoint1), TDirection.dtUnKnown);
                        if ((Dir == TDirection.dtDnEqual) || (Dir == TDirection.dtDn))
                            Dir = TDirection.dtUp;
                        else
                            if ((Dir == TDirection.dtUpEqual) || (Dir == TDirection.dtUp)) Dir = TDirection.dtDn;
//                        if (Dir == Quotes.IsExtremum(Convert.ToInt32(M.Point1.Bar), Options.Extremum1, TDirection.dtUnKnown))
                        if ((Dir == M.ModelDir) || (Dir == TDirection.dtUpDn))
                        {
                            // создание новой модели
                            MM = new TModel(Quotes, -1, M, DecDigs, 3);
                            if (IsModelExistsInList(ModelsP1, MM, 3) == -1)
                            {
                                ModelsP1.Add(MM);
//                                Logging.Log("Trend line breakout. New model added. "+Convert.ToString(Bar.DT));
                            }
                        }
                    }
                    i++;
                }
                X++;
            }

            // Очистка веера от неправильных и неполных моделей
            i = 0;
            while (i < ModelsP1.Count)
            {
                ModelsP1[i].IsModelCorrect();
                if ((CheckRules(ModelsP1[i].ErrorCode, ModelsP1[i].ModelType, Options.CheckedErrors0) && Options.EnableFiltrationFan)
                  || IsModelExistOnParentTF(ModelsP1[i])
                  || ModelsP1[i].CurrentPoint < 4)
                {
                    ModelsP1.RemoveAt(i);
                }
                //отдельно обрабатываем формирующиеся модели, для построения ГЦ
                else if (ModelsP1[i].CurrentPoint == 4)
                {
                    if (ModelsP1[i].CurrentPoint == 4) HModels.Add(ModelsP1[i]);
                    ModelsP1.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            return 0;
        }

        public bool CheckRules(int ModelErrorCode, Common.TModelType ModelType, TModelError[] CheckedRules) //! Проверка соответствия модели набору правил
        {
            bool CR = false;
            int MT = 0;
            switch (ModelType)
            {
                case Common.TModelType.ModelOfAttraction: { MT = 0; break; }
                case Common.TModelType.ModelOfBalance: { MT = 1; break; }
                case Common.TModelType.ModelOfExpansion: { MT = 2; break; }
            }
            CheckedRules[MT].ErrorCode = CheckedRules[MT].ErrorCode | 1;
            for (int i = 1; i < Common.ModelErrors.Count(); i++)
            {
                if (((ModelErrorCode & Common.ModelErrors[i].ErrorCode & CheckedRules[MT].ErrorCode) == Common.ModelErrors[i].ErrorCode) 
                    && ((ModelType == Common.ModelErrors[i].ModelType) 
                        || (Common.ModelErrors[i].ModelType == Common.TModelType.UnknownModel)))
                {
                    CR = true;
                    break;
                }
            }
            if (ModelType == Common.TModelType.UnknownModel) CR = true;
            return CR;
        }

        private bool AreModelsEquals(TModel M1, TModel M2, int Points)   //! Сравнение двух моделей на идентичность
        {
            if (((M1.Point1.Bar == M2.Point1.Bar) || (Points < 1)) &&
                ((M1.Point2.Bar == M2.Point2.Bar) || (Points < 2)) &&
                ((M1.Point3.Bar == M2.Point3.Bar) || (Points < 3)) &&
                ((M1.Point41.Bar == M2.Point41.Bar) || (Points < 4)) &&
                ((M1.Point5.Bar == M2.Point5.Bar) || (M1.Point4.Bar == M1.Point6.Bar) || (M1.ModelType == Common.TModelType.ModelOfAttraction) || (Points < 5)) &&
                ((M1.Point6.Bar == M2.Point6.Bar) || (M1.ModelType == Common.TModelType.ModelOfAttraction) || (Points < 6)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int IsModelExistsInList(List<TModel> ModelList, TModel Model, int Points)   //! Проверка на существование модели в списке, возвращает № модели в списке или -1 если модели в списке нет
        {
            int R = -1;
            int I;
            TModel MI;
            if (ModelList.Count > 0)
            {
                for (I = 0; I < ModelList.Count; I++)
                {
                    MI = ((TModel)ModelList[I]);
                    if (AreModelsEquals(Model, MI, Points))
                    {
                        R = I;
                        break;
                    }
                }
            }
            return R;
        }

        public void SaveToFile(string FileName) //! Сохранение моделей в файле
        {
            StreamWriter ff = new StreamWriter(FileName);
            foreach (TModel M in Models)
            {
                ff.WriteLine(M.ToString());
            }
            ff.Close();
        }

        public bool LoadFromFile(TQuotes Quotes, String FileName)  //! Загрузка моделей из файла
        {
            string Str;
            bool R = true;
            StreamReader ff = new StreamReader(FileName);
            try
            {
                while (!ff.EndOfStream)
                {
                    Str = ff.ReadLine();
                    TModel M = new TModel(Quotes, -1, -1, -1, 4);
                    M.FromString(Str);
                    Models.Add(M);
                }
            }
            catch
            {
                Logging.Log("Error reading file " + FileName + ".");
                //MainForm.MT4messagesForm.AddMessage("Error reading file " + FileName + ".");
                R = false;
            }
            ff.Close();
            return R;
        }

        internal List<TModel> CompareWith(List<TModel> MComp)
        {
            List<TModel> MRes = new List<TModel>();
            bool Found;
            // Поиск отсутствующих моделей в MMComp
            foreach (TModel M1 in Models)
            {
                Found = false;
                foreach (TModel M2 in MComp)
                {
                    if (AreModelsEquals(M1, M2, 6))
                    {
                        Found = true;
                        break;
                    }
                }
                if (!Found)
                {
                    M1.Step = 1;
                    MRes.Add(M1);
                }
            }
            foreach (TModel M1 in MComp)
            {
                Found = false;
                foreach (TModel M2 in Models)
                {
                    if (AreModelsEquals(M1, M2, 6))
                    {
                        Found = true;
                        break;
                    }
                }
                if (!Found)
                {
                    M1.Step = 2;
                    MRes.Add(M1);
                }
            }
            return MRes;
        }



        //  ===============================================================================================
        //   Для загрузки данных без мультипотока 
        // ================================================================================================
        public void SeekModelsSeq(TQuotes Quotes, int StartPoint, int LastPoint, int DecDigs)          //!  Метод нахождения последовательных моделей от указанной точки
        {
            SeekModelsSeq(Quotes, StartPoint, LastPoint, DecDigs, null, null);
        }
        public void SeekModels(TQuotes Quotes, int StartPoint, int LastPoint, int DecDigs)          //!  Метод нахождения моделей от указанной точки до конца котировок
        {
            SeekModels(Quotes, StartPoint, LastPoint, DecDigs, null, null);
        }

        /// <summary>
        /// подсчет точек моделей и их пересечений для статистики
        /// учитываются только точки текущего плана
        /// </summary>
        /// <param name="tmodels"></param>
        /// <param name="quotes"></param>
        void CountPointsByStatus(List<TModel> tmodels, TQuotes quotes)
        {
            points = new uint[quotes.GetCount()];
            //каждый знак - флаг принадлежности к соответствующей точке - порядковый номер знака(справа налево) = номеру точки 
            //при этом 1 = точка пересекается с точкой N(байта) модели
            //2(второй бит) = точка N принадлежит модели по тренду
            //4(третий бит) = точка N принадлежит коррекционной модели
            foreach (TModel m in tmodels)
            {
                //if (m.TimeFrame != quotes.tf) continue; //поскольку модели пересекаются на данном плане то и взаимосвязи их пускай учитываются. короче включил точки старших моделей в общий массив.
                points[(int)m.Point1.Bar] |= 0x000001;
                points[(int)m.Point2.Bar] |= 0x000010;
                points[(int)m.Point3.Bar] |= 0x000100;
                if (m.Point4.Bar != m.Point6.Bar)
                    points[(int)m.Point4.Bar] |= 0x001000; //иначе т4=6 и не может учитываться как коррекционная, поэтому будет пропущена
                //else учитывается 6 точка (6=4)
                points[(int)m.Point5.Bar] |= 0x010000;
                points[(int)m.Point6.Bar] |= 0x100000;
                //проверка флагов родительских модели(с которыми пересекается данная т1)
                if ((points[(int)m.Point1.Bar] & 0x001010) != 0)
                {
                    points[(int)m.Point3.Bar] |= 0x000400; //родительская модель - коррекционная
                    points[(int)m.Point5.Bar] |= 0x040000; //родительская модель - коррекционная
                }
            }
        }

        bool IsModelExistOnParentTF(TModel m)
        {
            if (m.Quotes.Symbol == null) return false;

            int parentModelID = m.Quotes.Symbol.MTree.SearchParentModel(m);

            return Options.EnableFiltrationByTF && parentModelID >= 0; 
        }
    }

    //=================================================================================================================
    //
    //  Описания общих классов 
    //
    [System.Serializable]
    public struct TPoint
    {
        public DateTime DT;
        public double Bar;
        public double Price;
        public PointF pt
        {
            get
            {
                PointF pt = new PointF((float)Bar, (float)Price);
                return pt;
            }
        }
        public TPoint(TPoint Point)
        {
            Bar = Point.Bar;
            DT = Point.DT;
            Price = Point.Price;
        }
        public TPoint(TBorder Point)
        {
            Bar = Point.Bar;
            DT = Point.DT;
            Price = Point.Price;
        }
        public TPoint(double ABar, DateTime ADT, double APrice)
        {
            Bar = ABar;
            DT = ADT;
            Price = APrice;
        }
        public int iBar
        {
            get
            {
                return (int)Bar;
            }
        }
        public override string ToString()
        {
            return Bar.ToString() + ";" + DT.ToString("dd.MM.yyyy HH:mm:ss") + ";" + Price.ToString();
        }
    }
    //аналогично TPoint +  вторая точка в горизонтальной проекции для получения горизонтального отрезка
    [System.Serializable]
    public struct TBorder
    {
        public DateTime DT;
        public double Bar
        {
            get { return xa; }
            set { xa = value; }
        }
        public double Price
        {
            get { return y; }
            set { y = value; }
        }
        public BorderF pt
        {
            get { return new BorderF(xa, xb==0?xa+100:xb, y); }
        }
        public int iBar
        {
            get { return (int)xa; }
        }
        public double xa, xb, y;
    }
    [System.Serializable]
    public struct BorderF
    {
        public float xa, xb, y;
        
        public BorderF(double a, double b, double price)
        {
            xa = (float)a;
            xb = (float)b;
            y = (float)price;
        }
        public bool IsEmpty
        {
            get { return y == 0; }
        }
        public PointF pt
        {
            get { return new PointF(xa, y); }
        }
    }
    public struct Border
    {
        public int xa, xb, y;
    }
    [System.Serializable]
    public struct TLine
    {
        public double Angle;
        public double Delta;
        public TLine(TLine Line)
        {
            Angle = Line.Angle;
            Delta = Line.Delta;
        }
    }
    public struct Common
    {
        public static TModelError[] ModelErrors = 
            /*ModelType*/                       /*ErrorCode*/  /*ErrorDesc*/
        { 
            /* 0 */new TModelError(TModelType.UnknownModel, 0x00000000, "Ошибок нет"),
            /* 1 */new TModelError(TModelType.UnknownModel, 0x00000001, "Нарушена последовательность точек"),
            /* 2 */new TModelError(TModelType.UnknownModel, 0x00000002, "Лишние точки на ЛТ"),
            /* 3 */new TModelError(TModelType.UnknownModel, 0x00000004, "Лишние точки на ЛЦ''"),
            /* 4 */new TModelError(TModelType.ModelOfAttraction, 0x00000008, "Пересечение тел баров точек 1 и 3"),
            /* 5 */new TModelError(TModelType.ModelOfAttraction, 0x00000010, "Пересечение тел баров точек 2 и 4"),

            /* 6 */new TModelError(TModelType.ModelOfAttraction, 0x00000020, "Пересечение тел баров точек 2 и 5 для МП"),
            /* 7 */new TModelError(TModelType.ModelOfBalance, 0x00000020, "Пересечение тел баров точек 2 и 5 для МДР"),
            /* 8 */new TModelError(TModelType.ModelOfExpansion, 0x00000020, "Пересечение тел баров точек 2 и 5 для МР"),

            /* 9 */new TModelError(TModelType.ModelOfAttraction, 0x00000040, "Точка 3 не глобальный экстремум для МП"),
            /* 10 */new TModelError(TModelType.ModelOfBalance, 0x00000040, "Точка 3 не глобальный экстремум для МДР"),
            /* 11 */new TModelError(TModelType.ModelOfExpansion, 0x00000040, "Точка 3 не глобальный экстремум для МР"),

            /* 12 */new TModelError(TModelType.ModelOfAttraction, 0x00000080, "Точка 5 внутри бара точки 4 для МП"),
            /* 13 */new TModelError(TModelType.ModelOfBalance, 0x00000080, "Точка 5 внутри бара точки 4 для МДР"),
            /* 14 */new TModelError(TModelType.ModelOfExpansion, 0x00000080, "Точка 5 внутри бара точки 4 для МР"),

            /* 15 */new TModelError(TModelType.UnknownModel, 0x00000100, "Точка 3'' вне базы"),
            /* 16 */new TModelError(TModelType.UnknownModel, 0x00000200, "Пересечение ЛЦ бара точки 1"),
            /* 17 */new TModelError(TModelType.UnknownModel, 0x00000400, "Наложение уровней модели"),
            /* 18 */new TModelError(TModelType.UnknownModel, 0x00000800, "Точка 3 не глобальный экстремум между т2 и т3"),

            /* 19 */new TModelError(TModelType.ModelOfAttraction, 0x00001000, "Разбег точек т4 и т4' <= 3 баров"),
            /* 20 */new TModelError(TModelType.ModelOfBalance, 0x00001000, "Разбег точек т4 и т4' <= 3 баров"),
            /* 21 */new TModelError(TModelType.ModelOfExpansion, 0x00001000, "Разбег точек т4 и т4' <= 3 баров"),

            /* 22 */new TModelError(TModelType.ModelOfAttraction, 0x00002000, "Разбег точек т3 и т3'' < 3 баров"),
            /* 23 */new TModelError(TModelType.ModelOfBalance, 0x00002000, "Разбег точек т3 и т3'' < 3 баров"),
            /* 24 */new TModelError(TModelType.ModelOfExpansion, 0x00002000, "Разбег точек т3 и т3'' < 3 баров"),

            /* 25 */new TModelError(TModelType.UnknownModel, 0x00004000, "Точка 5 внутри базы"),
            /* 26 */new TModelError(TModelType.UnknownModel, 0x00008000, "Точка 3 по касательной"),
            /* 27 */new TModelError(TModelType.UnknownModel, 0x00010000, "Лишние точки на ЛЦ через т2 глоб. экстремум"),
            /* 28 */new TModelError(TModelType.UnknownModel, 0x00020000, "Лишние точки на ЛЦ дающей самую дальнюю цель"),
            /* 29 */new TModelError(TModelType.UnknownModel, 0x00040000, "Нарушено взаимное расположение т2' и т3"),
            /* 30 */new TModelError(TModelType.ModelOfAttraction, 0x00080000, "Нарушено соотношение по цене для т1-т2 к т3-т4"),

            /* 31 */new TModelError(TModelType.ModelOfAttraction, 0x00100000, "БЦ < 1/3 Базы для МП"),
            /* 32 */new TModelError(TModelType.ModelOfBalance, 0x00100000, "БЦ < 1/3 Базы для МДР"),
            /* 33 */new TModelError(TModelType.ModelOfExpansion, 0x00100000, "БЦ < 1/3 Базы для МР"),

            /* 34 */new TModelError(TModelType.ModelOfAttraction, 0x00200000, "БЦ < 1/2 Базы для МП"),
            /* 35 */new TModelError(TModelType.ModelOfBalance, 0x00200000, "БЦ < 1/2 Базы для МДР"),
            /* 36 */new TModelError(TModelType.ModelOfExpansion, 0x00200000, "БЦ < 1/2 Базы для МР"),
        };


        /// <summary>
        /// Типы моделей
        /// </summary>

        [System.Serializable]
        public enum TModelType : int
        {
            ModelOfExpansion,   //!< Модель расширения
            ModelOfAttraction,  //!< Модель притяжения
            ModelOfBalance,     //!< Модель динамического равновесия
            UnknownModel        //!< Х.З. какая модель
        }

        //*****************************************************************************************
        //       Вычисление параметров прямой, проходящей через две точки
        //*****************************************************************************************
        public static TLine CalcLine(TPoint Point1, TPoint Point2)
        {
            TLine R = new TLine();
            if (Point2.Bar != Point1.Bar)
                R.Angle = (Point2.Price - Point1.Price) / (Point2.Bar - Point1.Bar);
            else
                R.Angle = 0;
            R.Delta = Point1.Price - R.Angle * Point1.Bar;
            return R;
        }
        //*****************************************************************************************
        //       Вычисление координаты Y линии по координате X
        //*****************************************************************************************
        public static double CalcLineValue(TLine Line, double X)
        {
            return X * Line.Angle + Line.Delta;
        }

        //*****************************************************************************************
        //       Вычисление координат точки пересечения двух прямых
        //*****************************************************************************************
        public static TPoint CrossLines(TLine Line1, TLine Line2)
        {
            TPoint R = new TPoint();
            if (Line1.Angle != Line2.Angle)
            {
                R.Bar = (Line2.Delta - Line1.Delta) / (Line1.Angle - Line2.Angle);
                //                R.Bar = R.Bar;
            }
            else
            {
                R.Bar = 0;
                //                R.Bar = 0;
            }
            R.Price = Line1.Angle * R.Bar + Line1.Delta;
            //            R.DT = 0;
            return R;
        }

        /// <summary>
        /// Вычисление расстояния от точки до прямой
        /// </summary>
        public static double DistancePointToLine(TPoint Point, TLine Line)
        {
            return Math.Abs(Line.Angle * Point.Bar - Point.Price + Line.Delta) / Math.Sqrt(Line.Angle * Line.Angle + 1);
        }

    }
    public class TModelError
    {
        public Common.TModelType ModelType;
        public int ErrorCode;
        public string ErrorDesc;
        public TModelError(Common.TModelType AModelType, int AErrorCode, string AErrorDesc)
        {
            ModelType = AModelType;
            ErrorCode = AErrorCode;
            ErrorDesc = AErrorDesc;
        }
    };

    ///AS: различные флаги для TModel.Stat
    [Flags]
    public enum MStat : uint {
        GlobalP1 = 0x01,         // 1 не пересекается с точками других моделей кроме 6
        GlobalTrend = 0x10,      // модель описывает глобальный тренд на своем плане (для данной т1 последняя в серии)
        getupHP = 0x400,         // признак доcтижения НР
        getup100percent = 0x100, // признак достижения 100%
        getup200percent = 0x200, // признак достижения 100%
        getup1target = 0x1000,   // признак достижения 1-й цели
        getup2target = 0x2000,   // признак достижения 2-й цели
        getup3target = 0x4000,   // признак достижения 3-й цели
        valid = 0x10000,         // модель прошла фильтрацию потренду
        invalid = 0x20000,       // модель не прошла фильтрацию потренду
    };

    //public struct LineF
    //{
    //    float xa, xb, y;
    //}
}

