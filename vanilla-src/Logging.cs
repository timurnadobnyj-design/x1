//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Pavel Kadomin (S_PASHKA)
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
using System.IO;


namespace Skilful
{
    public class Logging
    {
        static StreamWriter sw = null;
        
        //true to append to existing or create new, false to overwrite existing or create new
        public static void Start(string fileName, bool append)
        {
            sw = new StreamWriter(fileName, append);
            sw.AutoFlush = true; //будет сразу в файл сбрасывать, не буферизуя
        }

        public static void Log(string description)
        {
            DateTime dtNow = DateTime.Now;
            sw.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}: {1}", dtNow, description));
        }

        public static void Stop()
        {
            sw.Close();
        }
    }
}
