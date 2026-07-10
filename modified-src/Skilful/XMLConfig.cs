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
using System.IO;
using System.Xml;
using System.Windows.Forms;


namespace Skilful
{
    public class XMLConfig
    {
        static string SettingsFilePath = Application.StartupPath + "\\Settings.xml";
        /// <summary>
        /// get value from local XML config file as inner_text of <xmlpath></xmlpath>
        /// </summary>
        /// <param name="xmlpath">Name of Config`s Section</param>
        /// <returns>text value</returns>
        public static string Get(string xmlpath)
        {
            string config = "";
            if (File.Exists(SettingsFilePath))
            {
                XmlDocument XMLDoc = new XmlDocument();
                XMLDoc.Load(SettingsFilePath);
                XmlNode DataSourceLibraries = XMLDoc.SelectSingleNode("/settings/" + xmlpath);
                if (DataSourceLibraries != null) config = DataSourceLibraries.InnerText;
            }
            return config;
        }
        /// <summary>
        /// set parameter in Local Config File and save it to disk
        /// </summary>
        /// <param name="xmlpath">Name of Config`s Section</param>
        /// <param name="value">text value</param>
        public static void Set(string xmlpath, string value)
        {
            XmlDocument XMLDoc = new XmlDocument();
            XmlNode node = null;
            if (File.Exists(SettingsFilePath))
            {
                XMLDoc.Load(SettingsFilePath);
                node = XMLDoc.SelectSingleNode("/settings/" + xmlpath);
                if(node != null) node.InnerText = value;
            }
            if (node == null)
            {
                XmlNode root = XMLDoc.SelectSingleNode("/settings");
                if (root == null)
                {
                    root = XMLDoc.CreateElement("settings");
                    XMLDoc.AppendChild(root);
                }
                XmlElement elt = XMLDoc.CreateElement(xmlpath);
                root.AppendChild(elt);
                elt.InnerText = value;
            }
            XMLDoc.Save(SettingsFilePath);
        }
    }
}
