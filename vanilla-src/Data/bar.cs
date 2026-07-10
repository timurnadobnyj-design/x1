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

// bars.cs: implementation of the bar class.
//


public class Bar
{
    // Date-0, time - 1, open -2, high -3, low-4, close-5
    public decimal[] d = new decimal[6];
    //	decimal oc,h_l,ho;
    // r - расстояние в барах до предыдущего экстремума на четырех фреймах
    public int[] r = new int[4];
    //index показывает является ли бар экстремумом, на каком фрейме, 
    // направление тренда на этом фрейме более подробно см. makeIndex и getIndex
    public uint index;
    //номер бара 
    public int i;

    public DateTime DT
    {
        get
        {
            if (d[0] != 0)
            {
                int Y = Convert.ToInt32(d[0] / 10000);
                int M = Convert.ToInt32((d[0] - Y * 10000) / 100);
                int DD = Convert.ToInt32(d[0] % 100);
                int H = Convert.ToInt32(Math.Truncate(d[1] / 10000));
                int N = Convert.ToInt32((d[1] - H * 10000) / 100);
                int S = Convert.ToInt32(d[1] % 100);
                return new DateTime(Y, M, DD, H, N, S);
            }
            else
                return new DateTime();
        }
        set
        {
            d[0] = value.Year * 10000 + value.Month * 100 + value.Day;
            d[1] = value.Hour * 10000 + value.Minute * 100 + value.Second;
        }
    }
    public double Open  //! Цена открытия
    {
        get { return Convert.ToDouble(d[2]); }
       // set { }
    }
    public double High  //! Максимальная цена
    {
        get { return Convert.ToDouble(d[3]); }
       // set { }
    }
    public double Low   //! Минимальная цена
    {
        get { return Convert.ToDouble(d[4]); }
       // set { }
    }
    public double Close //! Цена закрытия
    {
        get { return Convert.ToDouble(d[5]); }
      //  set { }
    }


    //////////////////////////////////////////////////////////////////////
    // Construction/Destruction
    //////////////////////////////////////////////////////////////////////
    public Bar()
    {
        
        for (int k = 0; k < 6; k++)
            d[k] = 0;
        
    }
    public Bar(decimal[] dd, int ii)
    {
       // index = 0;
        for (int k = 0; k < 6; k++)
            d[k] = dd[k];
        i = ii;
    }
    public Bar(DateTime DT, double Open, double High, double Low, double Close, int BarNum)
    {
       // index = 0;
        d[0] = DT.Year * 10000 + DT.Month * 100 + DT.Day;
        d[1] = DT.Hour * 10000 + DT.Minute * 100 + DT.Second;
        d[2] = Convert.ToDecimal(Open);
        d[3] = Convert.ToDecimal(High);
        d[4] = Convert.ToDecimal(Low);
        d[5] = Convert.ToDecimal(Close);
        i = BarNum;
    }

    public virtual void Dispose()
    {
    }

    public void getIndex(out int hl, out int bar37, out int frep, out int trndir)
    {
        //является ли бар high или low
        hl = (int)(index & 3);
        //является ли бар баром "3"(для 2-5точек модели) или "7" (для 1-й точкимодели
        bar37 = (int)((index & 60) >> 2);
        //репером какого фрейма является бар
        frep = (int)((index & 192) >> 6);
        //направление тренда на фрейме, на котором бар является репером
        trndir = (int)((index & 261888) >> 8);
    }
   
    public string whatExtremum()
    {
        int stat37 = (int)((index & 60) >> 2);
        int high37 = (stat37 & 12) >> 2;//флаг для бара хай является ли он баром "3" или "7"
        int low37 = (stat37 & 3);//флаг для бара лоу является ли он баром "3" или "7"
        if ((high37 > 0) && (low37 == 0)) return "Up";
        if ((low37 > 0) && (high37 == 0)) return "Dn";
        if ((high37 > 0) && (low37 > 0)) return "UpDn";
        return " ";
    }
}

