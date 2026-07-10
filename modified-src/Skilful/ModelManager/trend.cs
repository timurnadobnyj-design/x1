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
using System.IO;
using System.Globalization;
using System.Windows.Forms; 
using Skilful;
using Skilful.QuotesManager;


public static class GlobalMembersTrend
{


	//////////////////////////////////////////////////////////////////////
	// Construction/Destruction
	//////////////////////////////////////////////////////////////////////

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//extern decimal round(decimal f, int k);

	public static bool log = false;
	//public static int max_fr_diff =1;//максимальная разница во фреймах реперов модели
	//public static int min_fr_diff =-1;// минимальная разница во фреймах реперов модели
	public static int chkbars = 3;// флаг проверки минимального условия того что экстремум может быть репером
	public static int st_pos =0;
	public static int updat =0;
	public static int end_pos;
    //public Trend CurrentTrend;

    //функция создания индекса бара по входным параметрам
	public static uint makeIndex(int frame, int framerepers,int trenddir)
	{
		//             0-4,      0-3    ,    0-3        ,    0-2     
		//                      1-2bit  ,    7-8bit,       9-17bit        

		return ((uint)framerepers<<6)+((uint)trenddir<<(8+frame *2));
	}


	public static int sign(decimal a)
	{
		if(a ==0)
			return 0;
		if(a>0)
			return 1;
		else
			return 2;
	}



	//decimal dF=0.0083;

	public static readonly int[] days_in_month = {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};

	/*public static int DOY(int y, int m, int d)
	{
		int day = 0;
		for(int i =0; i<m-1; i++)
		{
			day += days_in_month[i];
		}
		day += d;
		if((m>2) && !(y%4) && ((y%100) || !(y%400)))
			day++;
		return day;
	}
	public static decimal Date2h(int date, int h)
	{
		int day = date%100;
		int tmp = (date - day)/100;
		int month = tmp%100;
		int min = (h/100)%100;
		int yr = (tmp - month)/100;
		int suprem = (yr-1972)/4;
		int corr =0;
		if(((yr-1972)%4==0)&&(month<=2))
			corr =1;
		decimal H = ((yr-1970)*365+suprem-corr)*24+DOY(yr, month, day)*24 + (int)(h/10000)+min/60.;
		return H;
	}
    */


	//sunday
	//public static int Sun_date = 20040111;

	/*public static void N2Date(ref List<bar> map, int N, ref int date, ref int T)
	{
	int day0 = map[0].d[0]; //TF
	int t0 = map[0].d[1];
	decimal d0 =Date2h(day0, t0)/24.;
	decimal d_s = Date2h(Sun_date, 0)/24.;

	int day = day0%100;
	int tmp = (day0 - day)/100;
	int mon = tmp%100;
	int yr = (tmp - mon)/100;

	int dw = (7+((int)d_s - (int)d0)%7)%7;
	decimal dt = N *T/1440.;
	decimal D = (int)((dt+dw)/5)*2 +dt;
	int yar = D/365.25;
	decimal month = (D - yar *365.25)+((yr+yar)%4)*0.25;

	decimal k = DOY(yr, mon, day) + month;
	if(k>365)
	{
		k-=365;
		yar = yr + yr +1;
	}
	mon =0;
	while(k>days_in_month[mon])
	{
		k-=days_in_month[mon++];
	}
	day = (int)k;
	int hour = (k-day)*24;
	int min = ((k - day)*24 - hour)*60;
	date = yar *10000 + (mon+1)*100 + day;
	T = hour *10000 + min *100;
	}

	public static int dataplace = 20;

	public static decimal scaleX =1;
	public static decimal scaleY;
	public static decimal K =1.05;
	public static int prev_pos =0;
}*/
// trend.cpp: implementation of the Trend class.
//



}

/// <summary>
/// Основной класс, содержащий всю информацию о тренде
/// </summary>
public class Trend
{
private int lim7;
private int TF; //фрейм в минутах, вроде не используется
private string ff = new string(new char[256]);// имя файла
private string sec = new string(new char[8]);// название инструмента
private string ext = new string(new char[4]);//расширение открытого файла данных
// массив баров тренда
//public List<TBar> map = new List<TBar>();
 // массив экстремумов на 4 фреймах
//public List<List<Mesh>> mesh = new List<List<Mesh>>();
// массив моделей
//public List<TAmodel> expan = new List<TAmodel>();
// номер посленего бара в тренде
public int end; //длинна массива котировок
    
    //private CPen grid = new CPen();
//private CPen axis = new CPen();
//private CPen red = new CPen();
//private CPen orange = new CPen();
//private CBrush backgr = new CBrush();
//private CFont fnt = new CFont();
/*private int x1;
private int y1;
private int x2;
private int y2;*/
private int jpy=0;// флаг является ли инструмент кроссом йены или йеной
public int Decimals
{
    get
    {
        return (jpy > 0) ? 2 : 4;
    }
}

	/*public void Shift(int dist)
	{
		if(dist>0)
		{
			if(GlobalMembersTrend.st_pos<(end-8/GlobalMembersTrend.scaleX))
			{
			  GlobalMembersTrend.st_pos+=8/GlobalMembersTrend.scaleX;
			}
		}
		else
		{
			if(GlobalMembersTrend.st_pos>8/GlobalMembersTrend.scaleX)
			{
				GlobalMembersTrend.st_pos-=8/GlobalMembersTrend.scaleX;
			}
			else
				GlobalMembersTrend.st_pos =0;
		}
	}*/

    public void Set37Bars(int bar3, int bar7)
	{
	 b3 =bar3;
	 b7 =bar7;
	 lim7 =2 *bar7+1;
	}
//	public CRect r = new CRect();
	public string secur;
	public int b3;
	public int b7;
	public int @out;
	public int frame;
    //массив определяющий размеры анализируемых фреймов.
	//public decimal[] dF = new decimal[4];


    //загрузка одного бара из TQuotes
    public TBar @get(TQuotes qts, int i)
    {
        return qts[i];
        //convert ttime to day(dat) and time(day) as (int)YYYYMMDD and (int)hhmmss
        /*chf[0] = qts[i].DT.Year * 10000 + qts[i].DT.Month * 100 + qts[i].DT.Day;
        chf[1] = qts[i].DT.Hour * 10000 + qts[i].DT.Minute * 100 + qts[i].DT.Second;
        chf[2] = (decimal)qts[i].Open;
        chf[3] = (decimal)qts[i].High;
        chf[4] = (decimal)qts[i].Low;
        chf[5] = (decimal)qts[i].Close;*/
    }

    //загрузка одной строки данных из файла
    public int @get(ref StreamReader f, ref decimal[] chf)
    {
        if (f != null)
        {
            string a = f.ReadLine();
            //string c = new string(new char[20]);
            //a = "";
            //fgets(a,69,f);


            if ((a != null) && (a.Length > 20))
            {
                //sbyte[] b = SimulateStringFunctions.StrTok(a, ",");
                string[] b = a.Split(',');
                //&&b[0].IndexOfAny(new char []{a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,s,t,u,v,w,x,y,z})


                if ((b != null))
                {
                    chf[0] = Convert.ToInt32(b[0]);
                    chf[1] = Convert.ToInt32(b[1]);
                    for (int i = 2; i < b.Length; i++)                   
                        if(GlobalMembersTrend.log==true)
                        chf[i] = (decimal)Math.Log10((double)GlobalMembersTAmodel.round(Decimal.Parse(b[i], CultureInfo.InvariantCulture),(jpy>0)?2:4));
                        else
                        chf[i] = GlobalMembersTAmodel.round(Decimal.Parse(b[i], CultureInfo.InvariantCulture), (jpy > 0) ? 2 : 4);
                    return 1;
                    //strcpy(c,b);
                    /*switch(ext[2])
                    {
                      case 'v':
                        c[0] =b[6];
                        c[1] =b[7];
                        c[2] =b[8];
                        c[3] =b[9];
                        c[4] =b[3];
                        c[5] =b[4];
                        c[6] =b[0];
                        c[7] =b[1];
                        c[8]='\0';
                        break;
                      default:
                          c = b;
                          break;
                    }
	
                    chf[0] = Convert.ToInt32(c);
                    b =SimulateStringFunctions.StrTok(0,",");
	
                    switch(ext[2])
                    {
                      case 'v':
                        c[0] =b[0];
                        c[1] =b[1];
                        c[2]=c[3];
                        c[3]=c[4];
                        c[4]='0';
                        c[5]='0';
                        c[6]='\0';
                        break;
                      default:
                          c = b;
                          break;
                    }
                    chf[1] = Convert.ToInt32(c);
                    int i =2;
                    while((i<6) && b)
                    {
                        b =SimulateStringFunctions.StrTok(0,",");
                        chf[i] = Convert.ToDouble(b);
                        chf[i] =GlobalMembersTrend.round(chf[i], 4);
                        if(jpy!=-1)
                            chf[i]/=100;
                        i++;
                    }
                    return 1;
                  }*/
                }
                else
                    return 0;
            }
            return 0;
        }
        return 0;
    }

    //функция вывода тренда в файл mode=1 - выдает список найденных моделей
    // mode=0  выдает список баров тренда с распечаткой индекса
   /* public void print(string ff, int mode)
    {
        StreamWriter f = new StreamWriter(ff);
        int i;
        switch (mode)
        {
            case 1:
             //   f.WriteLine("Expansion");
               // for (i = 0; i < expan.Count; i++)
                 //   expan[i].print(ref f, ref map);
                //		fprintf(f,"Attraction\n");
                //		for( i=0;i<atract.size();i++)
                //			atract[i].print(f,&map);
                break;
            case 0:
                //debug		int M[4],pos[4],ende[4];
                //		for( i=0;i<4;i++){
                //			M[i]= mesh[i].at(0).k;
                //			pos[i]=0;
                //			ende[i]=mesh[i].size();
                //		}
                //
                //		int msh,kk;
                //
                for (i = 0; i < (map.Count - 1); i++)
                {
                    //debug			msh=0;
                    //			for(kk=0;kk<4;kk++){
                    //				if(i==M[kk]){
                    //					msh=kk;
                    //					if(pos[kk]<(ende[kk]-1)){
                    //						pos[kk]++;
                    //						M[kk] =mesh[kk].at(pos[kk]).k;
                    //					}
                    //				}
                    //			}
                    int hl;
                    int b37;
                    int frep;
                    int trndir;
                    map[i].getStatus(out hl, out b37, out frep, out trndir);
                    decimal a1 = map[i].d[0];
                    decimal a2 = map[i].d[1];
                    decimal a3 = map[i].High;
                    decimal a4 = map[i].Low;
                    //string g = new string(new char[356]);
                    f.WriteLine("{0,8:f0} {1,6:f0} {2,5:f4} {3,5:f4} {4:D} {5:D} {6:D} {7:D} ", a1, a2, a3, a4, hl, b37, frep, trndir/*,msh);
                    //g = string.Format("{0,8:f0} {1,6:f0} {2,5:f4} {3,5:f4} {4:D} {5:D} {6:D} {7:D} ", a1, a2, a3, a4, hl, b37, frep, trndir);
                    //fprintf(f,"%s\n",g);
                }
                break;
             }
        f.Dispose();
    } */

    // конструктор 
    public Trend(string file, string security)
        //полный путь к файлу, название файла
	{
        GlobalMembersTrend.log = false;

        init(security);

        int l = file.Length;
        ff = file;
        ext = file.Substring(l - 3);
        
        // определение числа строк в файле
	    end =detect_length(file);
    
	//if(!map) printf("not enough memory allocating bar map");
//	if (mesh == null)
//		Console.Write("not enough memory allocating mesh");

	}

    //add by skat constructor and analizer for existing quotelist
    public Trend(string security)
    {
        init(security);
        ff = null;
        ext = null;
        end = 0;
    }
    void init(string security)
    {
        b3 = 1;
        b7 = 2;
        @out = 7;
        frame = 0;
        lim7 = 2 * b7 + 1;
        //dF[0] = 0.002m;
        //dF[1] = 0.005m;
        //dF[2] = 0.01m;
        //dF[3] = 0.02m;
        jpy = security.ToLower().IndexOf("jpy");
        // инициализация массива экстремумов
        /*for (int m = 0; m < 4; m++)
        {
            List<Mesh> MM = new List<Mesh>();
            mesh.Add(MM);
        }*/
    }


    // еще одна версия, для работы с TQuotes, предобработки тренда, индексирует бары и строит массив экстремумов mesh
    public int Analyser(TQuotes quotes, int i)
    {
        //if (map.Count > i) map.RemoveRange(i, map.Count - i);

        end = quotes.GetCount();
        //decimal[] d = new decimal[7];
        int j = i;
        while (j < end )
        {
            //@get(quotes, map.Count, ref );
            //map.Add(quotes[map.Count]);
            bar37Analyser(quotes, j);
            j++;
        }
        
        return 1;
    }
    //////////////// end of skat`s code ///////////////////////////////////////////////////////////


    // функция нахождения количества строк в файле и фрейма данных
	public int detect_length(string name)
	{
	
			int i = 0;
			//int F =0;
           StreamReader thic = new StreamReader(name);
			if (thic == null)
			{
					//cerr << "Cannot open file " << name;
                return 0; ;
			}
            string F=thic.ReadLine();
			 while(F!=null)
			 {
                 if(F.Length>10)
					 i++;
                 F = thic.ReadLine();	 
			 };
		thic.Dispose();

        return i;
	}

    // функция анализа бара на удовлетворение условиям "3" и "7"
    public void bar37Analyser(TQuotes Q, int k)
    {

        if ((k >= lim7) && (k < (end - b7)))
        {
            int ind = k - b7;
            double dh = Q[ind].High;
            double dl = Q[ind].Low;
            int numh = 0;
            int numl = 0;
            for (int i = 1; i <= b7; i++)
            {
                double dh1 = Q[ind + i].High;
                double dh2 = Q[ind - i].High;
                //if(dh1==dh) dh1=
                if ((dh1 <= dh) && (dh2 <= dh))
                    numh++;
                else
                    break;
            }
            if (numh >= b7)
                Q[ind].BarStatus |= ((2 << 4) + 2);
            else
            {
                if (numh >= b3)
                    Q[ind].BarStatus |= ((1 << 4) + 2);

            }
            for (int i = 1; i <= b7; i++)
            {
                double dh1 = Q[ind + i].Low;
                double dh2 = Q[ind - i].Low;
                if ((dh1 >= dl) && (dh2 >= dl))
                    numl++;
                else
                    break;
            }

            if (numl >= b7)
                Q[ind].BarStatus |= ((2 << 2) + 1);
            else
            {
                if (numl >= b3)
                    Q[ind].BarStatus |= ((1 << 2) + 1);

            }
        }
    }
        

    // основная функция предобработки тренда, индексирует бары и строит массив экстремумов mesh
 /*   public int Analyser(ref StreamReader fff)
	{
		StreamReader fil;
		if(fff == null)
			fil = new StreamReader(ff);
		else
			fil =fff;
	//debug       StreamWriter debugg = new StreamWriter("gebug.txt");
		if(fil ==null)
			return 0;
		decimal[] d = new decimal[7]; //,ph[8],pl[8];
		decimal[,] pl = new decimal[2, 4];
		decimal[,] ph = new decimal[2, 4];
		int[,] Nh = new int[2, 4];
		int[,] Nl = new int[2, 4];
        fil.ReadLine();  //skip the first string
		//@get(ref fil, ref d); 
	
	
		@get(ref fil, ref d);
		int i;
		int k =0;
		int[] F = new int[4];
	
	//	for( i=0;i<7;i++) map[k].d[i]=d[i];
	//	map[k].i=0;
		TBar B = new TBar(d, 0);
		map.Add(B);
		//B.~new bar();
		for(i =0;i<4;i++)
		{
			Nh[0, i] =Nh[1, i] =0;
			Nl[0, i] =Nl[1, i] =0;
			ph[0, i] =d[3];
			ph[1, i] =0;
			if(dF[i]>0)
				frame++;
			pl[0, i] =d[4];
			pl[1, i] =0;
			F[i] =0;
		Mesh a = new Mesh();
		a.b_be = 0;
		a.before = 0;
		a.k = 0;
		mesh[i].Add(a);
		}
	
		//int dd =0;
		while (@get(ref fil, ref d) != 0)
		{
			k++;
			TBar BB = new TBar(d, k);
	//		for(i=0;i<7;i++) map[k].d[i]=d[i];
	//		map[k].i=k;
			map.Add(BB);
			bar37Analyser(k);
			//BB.~new bar();
	//		if((++dd)%1000==0) printf("%ld\n",dd);
			for(int f =0;f<frame;f++)
			{
				if(d[3]>ph[0, f])
				{
					ph[0, f] =d[3];
					Nh[0, f] =k;
	
				}
				else
				{
					if(((ph[0, f]-d[4])>dF[f])&&(F[f]<=0))
					{
					map[Nh[0, f]].BarStatus -= (map[Nh[0, f]].BarStatus &192);
					map[Nh[0, f]].BarStatus|= GlobalMembersTrend.makeIndex(f, f, GlobalMembersTrend.sign(ph[0, f]-ph[1, f]));
					//map[Nh[0][f]].r[f] = Nh[0][f]-Nh[1][f];
					Mesh a = new Mesh();
					a.b_be = Nh[0, f]- Nl[1, f];
					int dat = (int) map[Nl[1, f]].d[0];
					int day = (int) map[Nl[1, f]].d[1];
					a.before = map[Nh[0, f]].High - map[Nl[1, f]].Low;
					a.k = Nh[0, f];
					mesh[f][mesh[f].Count-1].b_af = a.b_be;
                    mesh[f][mesh[f].Count-1].after = a.before;
					mesh[f].Add(a);
					Nh[1, f] =Nh[0, f];
					ph[1, f] =ph[0, f];
					if(Nl[0, f] ==Nh[0, f])
						F[f] =0;
						else
							F[f] =1;
					pl[0, f] =d[4];
					Nl[0, f] =k;
					}
				}
				if(d[4]<pl[0, f])
				{
					pl[0, f] =d[4];
					Nl[0, f] =k;
				}
				else
				{
					if((d[3]-pl[0, f])>dF[f]&&(F[f]>=0))
					{
					map[Nl[0, f]].BarStatus-=(map[Nl[0, f]].BarStatus &192);
					map[Nl[0, f]].BarStatus|= GlobalMembersTrend.makeIndex(f, f, GlobalMembersTrend.sign(pl[0, f]-pl[1, f]));
					map[Nl[0, f]].r[f] = Nl[0, f]-Nl[1, f];
					Mesh a = new Mesh();
					a.b_be = Nl[0, f]- Nh[1, f];
					int dat = (int) map[Nh[1, f]].d[0];
					int day = (int) map[Nh[1, f]].d[1];
					a.before = -map[Nl[0, f]].Low + map[Nh[1, f]].High;
					a.k = Nl[0, f];
					mesh[f][mesh[f].Count-1].b_af = a.b_be;
					mesh[f][mesh[f].Count-1].after = a.before;
					mesh[f].Add(a);
					Nl[1, f] =Nl[0, f];
					pl[1, f] =pl[0, f];
					if(Nl[0, f] ==Nh[0, f])
						F[f] =0;
						else
							F[f] =-1;
	
	//C++ TO C# CONVERTER NOTE: Beginning of line comments are not maintained by C++ to C# Converter
	 ph[0][f]=d[3];
					ph[0, f] =d[3];
					Nh[0, f] =k;
					}
				}
                //debug                    debugg.WriteLine("{0,3:d} {1,2:d} {2,6:f4} {3,6:f4} {4,6:f4} {5,6:f4} {6,3:d} {7,3:d} {8,3:d} {9,3:d}   ",k,F[f],
				//		ph[0,f],ph[1,f],pl[0,f],pl[1,f],Nh[0,f],Nh[1,f],Nl[0,f],Nl[1,f]);
			} //for f //map->Calc();
	//debug				fprintf(debugg,"\n");
			} //while k
	
		fil.Dispose();
	//debug       debugg.Dispose();
		end = map.Count;
		GlobalMembersTrend.updat =1;
		return 1;
	
	}
    */
    
    // функция поиска моделей на глубине фрейма f
  
/*	public virtual void Dispose()
	{
	//delete [] map;
	mesh = null;
	}*/
}



	
