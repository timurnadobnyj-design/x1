//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Logvinenko Eugeniy (manuka)
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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;


namespace ChartV2.Serialization
{
   public  class CustomSerializer
    {
        public static void Serialize(string fileName, object objectToSerialize)
        {
            Stream stream;
            IFormatter formatter = new BinaryFormatter();

            try
            {
                stream = new FileStream(fileName, FileMode.Create);
            }
            catch (Exception ex) { throw ex; }

            formatter.Serialize(stream, objectToSerialize);
            stream.Close();
        }

        public static object DeSerialize(string fileName)
        {
            Stream stream;
            IFormatter formatter = new BinaryFormatter();

            formatter.Binder = new DeserializationBinder();
            try
            {
                stream = new FileStream(fileName, FileMode.Open);
            }
            catch (Exception ex) { throw ex; }

            object ob = null;
            try
            {
                while (stream.Position < stream.Length - 1)
                {
                    ob = formatter.Deserialize(stream);
                }
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                stream.Close();
                
            }
            return ob;
        }

       
    }
   sealed class DeserializationBinder : SerializationBinder
   {

       public override Type BindToType(string assemblyName, string typeName)
       {
           Type typeToDeserialize = null;

           string assName = Assembly.GetExecutingAssembly().FullName;
           string typeVer = "ver1";

           if (assemblyName == assName && typeName == typeVer)
           {

               typeVer = "ver2";
           }
           Axis_Plot.Plot p = new Axis_Plot.Plot();
           typeToDeserialize = Type.GetType(String.Format("{0},{1}}", p.GetType().ToString(), assemblyName));
           return typeToDeserialize;
           
           
           
           throw new NotImplementedException();
       }
   }
}
