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
using System.Collections;
using System.Drawing;

namespace ChartV2.Axis_Plot
{
    public class Grid
    {

        public bool IsVisible = Skilful.Sample.Grid_IsVisible;
        public Styles.GridStyle style;
        public List<float> horizontalLevelsArray;
        public List<float> verticalLevelsArray;



        //public Grid(string pathToXml)
        //{

        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(pathToXml);
        //    XmlNode tempXmlNode = null;
        //    FieldInfo fi = null;
        //    // PropertyInfo pi = null;
        //    for (int i = 0; i < doc.DocumentElement.ChildNodes.Count; i++)
        //    {
        //        if (doc.DocumentElement.ChildNodes[i].Name == "Grid")
        //        {
        //            for (int ii = 0; ii < doc.DocumentElement.ChildNodes[i].ChildNodes.Count; ii++)
        //            {
        //                tempXmlNode = doc.DocumentElement.ChildNodes[i].ChildNodes[ii];
        //                fi = this.GetType().GetField(tempXmlNode.Name);
        //                if (fi.FieldType == typeof(Color))
        //                {

        //                    fi.SetValue(this, Color.FromArgb(
        //                    Int32.Parse(tempXmlNode.Attributes["A"].Value),
        //                    Int32.Parse(tempXmlNode.Attributes["R"].Value),
        //                    Int32.Parse(tempXmlNode.Attributes["G"].Value),
        //                    Int32.Parse(tempXmlNode.Attributes["B"].Value)));
        //                    continue;
        //                }
        //                else if (fi.FieldType == typeof(Font))
        //                {

        //                    fi.SetValue(this, new Font(tempXmlNode.Attributes["family"].Value, Int32.Parse(tempXmlNode.Attributes["em"].Value)));
        //                    continue;
        //                }
        //                else if (fi.FieldType == typeof(Boolean))
        //                {
        //                    fi.SetValue(this, bool.Parse(tempXmlNode.Attributes["value"].Value));
        //                    continue;
        //                }
        //                else
        //                {
        //                    fi.SetValue(this, Int32.Parse(tempXmlNode.Attributes["value"].Value));
        //                    continue;
        //                }

        //            }

        //        }
        //    }
        //    verticalPen = new Pen(verticalPenColor, verticalPenWidth);
        //    horizontalPen = new Pen(horizontalPenColor, horizontalPenWidth);
        //    verticalPen.DashPattern = new float[2] { v1, v2 };
        //    horizontalPen.DashPattern = new float[2] { h1, h2 };
        //    horizontalLevelsArray = new ArrayList();
        //    verticalLevelsArray = new ArrayList();



        //}
        public Grid()
        {
            style = new Styles.GridStyle();
            horizontalLevelsArray = new List<float>();
            verticalLevelsArray = new List<float>();
        }







    }
}
