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

namespace rtservice
{
    enum TF
    {
        custom, m60, m240, Day, Week, Month, Quarter, Year, //набор базовых фреймов
    };
    /////////////////////////////////////
    class rtserv
    {
        static void Main(string[] args)
        {
            if (args.Length == 4 && args[0] == "get_history")
            {
                string sym = args[1];
                TF tf = (TF)int.Parse(args[2]);
                int len = int.Parse(args[3]);

                generator G = new generator();
                G.generate_history2(sym, tf, len);
            }
            else if (args.Length == 4 && args[0] == "listen")
            {
                string sym = args[1];
                DateTime lasttime = DateTime.Parse(args[2].Substring(0, 10)+" "+args[2].Substring(11));
                double lasttick = a2f(args[3]);
     
                generator G = new generator();
                G.listen(sym, TF.m60, lasttime, lasttick);
            }
            //else
            //{
            //    generator G = new generator();
            //    G.generate_history2("uRand123.45", TF.m60, 50000);
            //    return;
            //}
        }
        static double a2f(string s)
        {
            string[] dbl = s.Split(',', '.');
            double val = int.Parse(dbl[0]);
            if (dbl.Length == 2) val += int.Parse(dbl[1]) / Math.Pow(10, dbl[1].Length);
            return val;
        }
    }
    /////////////////////////////////////////
    class generator
    {
        /////////////////////////////////////////
        public void generate_history(string sym, TF tf, int len)
        {
            string number = sym.Substring(sym.LastIndexOf("om") + 2);
            int pwr = number.IndexOf('.') - 1;                                //количество разрядов в целой части
            int digits = number.Substring(number.IndexOf('.') + 1).Length;    //количество разрядов в дробноой части
            int cnt = number.Length - 1;                                      //общее количество разрядов
            double point = Math.Pow(10, -digits);

            double o = 0, h, l, c;
            for (int i = 1; i <= cnt; i++) o = o * 10 + i;
            o *= point;
            Random randObj = new Random();
            DateTime time = new DateTime(2000, 01, 01);

            while (len-- > 0)
            {
                h = o + randObj.Next(25) * point;
                l = o - randObj.Next(25) * point;
                c = l + randObj.Next((int)((h - l) / point)) * point;
                Console.WriteLine(inctime(ref time, tf).ToString("yyyy.MM.dd/HH:mm:ss") + " " + o + " " + h + " " + l + " " + c);
                o = c + randObj.Next(-6, 7) * point;
            }
        }
        /////////////////////////////////////////
        static Dictionary<string, DateTime> last_time = new Dictionary<string, DateTime>();
        static Dictionary<string, double> last_tick = new Dictionary<string, double>();
        public void generate_history2(string sym, TF tf, int len)
        {
            string number = sym.Substring(sym.LastIndexOf("Rand") + 4);
            int pwr = number.IndexOf('.');                                    //количество разрядов в целой части
            int digits = number.Substring(number.IndexOf('.') + 1).Length;    //количество разрядов в дробноой части
            int cnt = number.Length - 1;                                      //общее количество разрядов
            double point = Math.Pow(10, -digits);
            double max, min; //допустимые границы диапазона

            double o = 0, h, l, c=0;
            for (int i = 1; i <= cnt; i++) o = o * 10 + i;//сгенерируем начальное число вида 12345
            o *= point;                                   //добавим плавающую точку
            Random randObj = new Random();
            DateTime time = new DateTime(2000, 01, 01);   //дата начала отсчета
            min = o / 5;
            max = o * 5;

            int w = cnt-2, f=digits;
            while (len-- > 0)
            {
                int vol = randObj.Next(10) + 10;
                h = l = c = o;
                for (int j = 0; j < vol; j++)
                {
                    c = next(randObj, min, max, c, point);
                    if (h < c) h = c;
                    if (l > c) l = c;
                }
                Console.WriteLine(inctime(ref time, tf).ToString("yyyy.MM.dd/HH:mm:ss")+" {0,"+w+":f"+f+"} {1,"+w+":f"+f+"} {2,"+w+":f"+f+"} {3,"+w+":f"+f+"}", o, h, l, c);
                //Console.WriteLine(inctime(ref time, tf).ToString("yyyy.MM.dd/HH:mm:ss") + " " + o + " " + h + " " + l + " " + c);
                o = next(randObj, min, max, c, point);
            }
            //сохраним данные для последующего включения генератора тиков
            last_time.Add(sym, time);
            last_tick.Add(sym, c);
        }
        double next(Random randObj, double min, double max, double current, double point)
        {
            //double c = current + randObj.Next(-7, 7) * point;
            double c = ((int)(current / point) + randObj.Next(-5, 6)) * point;
            if (c <= min) c += 14*point;
            else if (c >= max) c -= 14*point;
            return c;
        }
        /////////////////////////////////////////
        public void listen(string sym, TF tf, DateTime time, double initial_tick)
        {
            string number = sym.Substring(sym.LastIndexOf("Rand") + 4);
            char delim = number.IndexOf('.') >= 0 ? '.' : ',';
            int digits = number.Substring(number.IndexOf(delim) + 1).Length;    //количество разрядов в дробноой части
            int cnt = number.Length - 1;                                      //общее количество разрядов
            double point = Math.Pow(10, -digits);
            double max, min; //допустимые границы диапазона

            //Console.WriteLine("tick" + ";" + sym + ";" + time + ";" + initial_tick + ";" + initial_tick);

            double o = initial_tick, h, l, c;
            min = o / 5;
            max = o * 5;
            Random randObj = new Random();

            while (true)
            {
                int vol = randObj.Next(10) + 10;
                h = l = c = o;
                for (int j = 0; j < vol; j++)
                {
                    c = next(randObj, min, max, c, point);
                    if (h < c) h = c;
                    if (l > c) l = c;
                    //выдача реал тайм тиков // sym, time, bid, ask
                    Console.WriteLine("tick;" + sym + ";" + time.AddSeconds(j+1).ToString("yyyy.MM.dd HH:mm:ss") + ";" + c + ";" + (c + randObj.Next(1,3)*point));
                    //Sleep(rand());
                    System.Threading.Thread.Sleep(randObj.Next(100, 250));
                }
                Console.WriteLine("bar;" + sym + ";" + (int)tf + ";" + inctime(ref time, tf).ToString("yyyy.MM.dd HH:mm:ss") + ";" + o + ";" + h + ";" + l + ";" + c);
                o = next(randObj, min, max, c, point);
            }
        }
        /////////////////////////////////////////
        DateTime inctime(ref DateTime time, TF tf)
        {
            switch (tf)
            {
                case TF.m60: time = time.AddHours(1);
                    if (time.DayOfWeek == DayOfWeek.Saturday) time = time.AddDays(2);
                    break;
            }
            return time;
        }
    }
}
