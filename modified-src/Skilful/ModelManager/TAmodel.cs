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

public static class GlobalMembersTAmodel
{

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//decimal Date2h(int date, int h);



//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
	//class bar;

    public static decimal round(decimal f, int k)
    {
        decimal ff = (decimal)Math.Pow(10, k);
        f = Math.Truncate(f * ff);
        return f / ff;
    }

    public static double round(double f, int k)
    {
        double ff = Math.Pow(10, k);
        f = Math.Truncate(f * ff);
        return f / ff;
    }

	public static int round(decimal d)
	{
		int g = (int) d;
		if(d>0)
		if(Math.Abs(d-g)>0.5m)
			return (g+1);
		else
			if(Math.Abs(d-g)>0.5m)
				return (g-1);
		return g;
	}
	//////////////////////////////////////////////////////////////////////
	// Construction/Destruction
	//////////////////////////////////////////////////////////////////////

    //количество баров среди которых данный бар должен быть экстремальным, чтобы стать 1-й точкой 
	public static int b_7=1;
    //количество баров среди которых данный бар должен быть экстремальным, чтобы стать 2-5-й точками 
    public static int b_3=1;
	// флаги отработки модели
    public static string[] msg ={"BrTL","Br5","TooFast","NewModel","Yes","Work","X13","X24"};
    // цвета моделей
    public static int[] cl = { 0xCC0033, 0xFF0000, 0x990033, 0xFF00FF, 0x00FA9A, 0x0000FF, 0xEE82EE, 0xFFFF00, 0x778899, 0xCD853F, 0x0000FF };

}



