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
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Skilful.QuotesManager;


namespace Skilful.Data
{
    public class DataSourceLibraryLoader
    {
        /// <summary>
        /// список датасурс модулей длл,
        /// объекты каждого модуля создаются при инициализации программы и живут все время до зывершения работы
        /// сам DataManager создан в MainForm (MainForm->DataManager->DataSource->DSModules[])
        /// </summary>
        List<DataSourseModule> DSModules;
        public int Count()
        {
            return DSModules != null ? DSModules.Count() : 0;
        }
        public DataSourceLibraryLoader()
        {
            DirectoryInfo di;
            FileInfo[] filelist;

            try
            {
                //di = new DirectoryInfo(Application.StartupPath + "\\DataSourceLibraries");
                di = new DirectoryInfo(Application.StartupPath);
                if (di.Exists)
                {
                    filelist = di.GetFiles("*.dll");

                    DSModules = new List<DataSourseModule>(filelist.Length);
                    for (int i = 0; i < filelist.Length; i++)
                    {
                        try
                        {
                            //файл библиотеки содержит обязательный public class DataSourceClass с единственным дефолным конструктором
                            Type[] types = Assembly.LoadFrom(filelist[i].FullName).GetTypes();
                            Type DataSourceObject = null;
                            for (int j = 0; j < types.Length; j++)
                            {
                                if (types[j].Name == "DataSourceClass")
                                    DataSourceObject = types[j];
                            }
                            if (DataSourceObject == null) continue;
                            
                            //создание объекта public класса из Dll 
                            Object instance = DataSourceObject.GetConstructors()[0].Invoke(null); 

                            DataSourseModule dsm = new DataSourseModule();

                            dsm.SetMethod(dm.SetTickHandler, DataSourceObject.GetMethod("SetTickHandler"));
                            dsm.SetMethod(dm.RemTickHandler, DataSourceObject.GetMethod("RemTickHandler"));
                            dsm.SetMethod(dm.import, DataSourceObject.GetMethod("import"));
                            dsm.SetMethod(dm.init, DataSourceObject.GetMethod("init"));
                            dsm.SetMethod(dm.get_symbol_list, DataSourceObject.GetMethod("get_symbol_list"));
                            dsm.SetMethod(dm.get_symbol_count, DataSourceObject.GetMethod("get_symbol_count"));
                            dsm.SetMethod(dm.get_history_length, DataSourceObject.GetMethod("get_history_length"));
                            dsm.SetMethod(dm.get_tick, DataSourceObject.GetMethod("get_tick"));
                            dsm.SetMethod(dm.get_bar, DataSourceObject.GetMethod("get_bar"));
                            dsm.SetMethod(dm.get_pip_value, DataSourceObject.GetMethod("get_pip_value"));
                            dsm.SetMethod(dm.storagePath, DataSourceObject.GetMethod("storagePath"));
                            try { dsm.SetMethod(dm.ShowDialog, DataSourceObject.GetMethod("ShowDialog")); }
                            catch { }
                            try { dsm.SetMethod(dm.GetWorkingDir, DataSourceObject.GetMethod("GetWorkingDir")); }
                            catch { }
                            try { dsm.SetMethod(dm.SetWorkingDir, DataSourceObject.GetMethod("SetWorkingDir")); }
                            catch { }
                            if (!dsm.validate_lib()) continue;

                            dsm.inst = instance;
                            DSModules.Add(dsm);
                            dsm.init();
                        }
                        catch /*(FileLoadException e)*/
                        {
                            //не удалось загрузить, считаем что данная dll не является DataSourceModule
                            continue;
                        }
                    }
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            } 
        }
        //индексатор, возвращает объект DataSourseModule из List по его индексу
        public DataSourseModule this[int i]
        {
            get
            {
                return DSModules[i];
            }
        }
        //индексатор 2, возвращает объект DataSourseModule из List по его имени
        public DataSourseModule this[string name]
        {
            get
            {
                foreach (DataSourseModule mod in DSModules)
                    if (mod.name == name)
                        return mod;
                return null;
            }
        }
        //возвращает индекс модуля в массиве DSModules по имени модуля
        public int name2index(string name)
        {
            for (int i = 0; i < DSModules.Count; i++)
            {
                if (DSModules[i].name == name) return i;
            }
            return -1;
        }
    }
}
