//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Andrey Prokhorov (AVP)
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

namespace Skilful.QuotesManager
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
    public class TBar// : Bar
    {
/*
 * public int Bar //! № бара в последовательности
        {
            get { return i; }
            set { i = value; }
        }
        public double X //! Для совместимости с TPoint
        {
            get { return Convert.ToDouble(i); }
        }
 */
        public DateTime DT;  //!< Дата/время бара
        public double Bar;   //!< № бара
        public double X;     //!< № бара на исходном ТФ с которого начинается этот бар
        public double Open;  //!< Цена открытия
        public double High;  //!< Цена макс
        public double Low;   //!< Цена мин
        public double Close; //!< Цена закрытия
       // public double High_X;//!< № бара на исходном ТФ с максимальной ценой
       // public double Low_X; //!< № бара на исходном ТФ с минимальной ценой
        public uint BarStatus;//!< Служебная переменная указывающая на каком фрейме и каким экстремумом является бар

        public TBar() { }
        public TBar(double Bar_, DateTime DT_, double Open_, double High_, double Low_, double Close_)
        {
            Bar = Bar_;
            DT = DT_;
            Open = Open_;
            High = High_;
            Low = Low_;
            Close = Close_;
        }
        
        public uint getStatus(out int hl, out int bar37, out int frep, out int trndir)
        {
           // if(BarStatus==0) return 0;
            //является ли бар high или low 
            hl = (int)(BarStatus & 3);
            //является ли бар баром "3"(для 2-5точек модели) или "7" (для 1-й точкимодели
            bar37 = (int)((BarStatus & 60) >> 2);
            //репером какого фрейма является бар
            frep = (int)((BarStatus & 192) >> 6);
            //направление тренда на фрейме, на котором бар является репером
            trndir = (int)((BarStatus & 261888) >> 8);
            return BarStatus;
        }

        public string whatExtremum()
        {
            int stat37 = (int)((BarStatus & 60) >> 2);
            int high37 = (stat37 & 12) >> 2;//флаг для бара хай является ли он баром "3" или "7"
            int low37 = (stat37 & 3);//флаг для бара лоу является ли он баром "3" или "7"
            if ((high37 > 0) && (low37 == 0)) return "Up";
            if ((low37 > 0) && (high37 == 0)) return "Dn";
            if ((high37 > 0) && (low37 > 0)) return "UpDn";
            return " ";
        }

    }
}
                                                                        