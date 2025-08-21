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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Xml;


namespace Skilful.Data
{
    public partial class DataWorkShopWindow : Form
    {
        static string SettingsFile = "_format.xml";
        public string decimalSeparator = ".";
        public NumberFormatInfo numInfo = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
        int TickerIndex = -1;
        int PeriodIndex = -1;
        int DateIndex = -1;
        int TimeIndex = -1;
        int OpenIndex = -1;
        int HighIndex = -1;
        int LowIndex = -1;
        int CloseIndex = -1;


        public DataWorkShopWindow(string str)
        {
            InitializeComponent();
            DirectoryName.Text = str;
        }

        public void ReadSettings(string DirectoryName)     //! Чтение настроек из файла _format.xml
        {
            if (File.Exists(DirectoryName+"\\"+SettingsFile))
            {
                XmlDocument OptionsXml = new XmlDocument();
                OptionsXml.Load(DirectoryName+"\\"+SettingsFile);
                // Выделение нода с фильтром для файлов
                XmlNode CommonNode = OptionsXml.SelectSingleNode("/formats/common");
                if (CommonNode != null)
                {   // Нод с фильтром файлов найден
                    Extension.Text = CommonNode.Attributes.GetNamedItem("filefilter").Value;
                    EnableWatching.Checked = false;
                    if ((CommonNode.Attributes.GetNamedItem("enablewatching") != null) && (CommonNode.Attributes.GetNamedItem("enablewatching").Value == "true"))
                        EnableWatching.Checked = true;
                }
                // Выделение узла для разделителей
                XmlNode DelimitersNode = OptionsXml.SelectSingleNode("/formats/delimitors");
                if (DelimitersNode != null)
                {   // Нод с разделителями
                    FieldDelimiter.Text = DelimitersNode.Attributes.GetNamedItem("fields").Value;
                    DecimalDelimiter.Text = DelimitersNode.Attributes.GetNamedItem("decimal").Value;
                    //DateDelimiter.Text = DelimitersNode.Attributes.GetNamedItem("date").Value;
                    //TimeDelimiter.Text = DelimitersNode.Attributes.GetNamedItem("time").Value;
                }
                // Выделение нода с индексами полей
                XmlNode FieldsIndexesNode = OptionsXml.SelectSingleNode("/formats/fieldindexes");
                if (FieldsIndexesNode != null)
                {   // Нод с настройкам размеров экстремумов найден
                    TickerIndex = Convert.ToInt32(FieldsIndexesNode.Attributes.GetNamedItem("ticker").Value);
                    PeriodIndex = Convert.ToInt32(FieldsIndexesNode.Attributes.GetNamedItem("period").Value);
                    DateIndex = Convert.ToInt32(FieldsIndexesNode.Attributes.GetNamedItem("date").Value);
                    TimeIndex = Convert.ToInt32(FieldsIndexesNode.Attributes.GetNamedItem("time").Value);
                    OpenIndex = Convert.ToInt32(FieldsIndexesNode.Attributes.GetNamedItem("open").Value);
                    HighIndex = Convert.ToInt32(FieldsIndexesNode.Attributes.GetNamedItem("high").Value);
                    LowIndex = Convert.ToInt32(FieldsIndexesNode.Attributes.GetNamedItem("low").Value);
                    CloseIndex = Convert.ToInt32(FieldsIndexesNode.Attributes.GetNamedItem("close").Value);
                }
                // Выделение нода с форматами даты и времени
                XmlNode DateTimeFormatsNode = OptionsXml.SelectSingleNode("/formats/datetimeformats");
                if (DateTimeFormatsNode != null)
                {   // Нод с форматами даты и времени найден
                    DateFormatMnu.Text = DateTimeFormatsNode.Attributes.GetNamedItem("date").Value; 
                    TimeFormatMnu.Text = DateTimeFormatsNode.Attributes.GetNamedItem("time").Value;
                }
                FillExampleData();
            }

        }
        public void WriteSettings(string DirectoryName)
        {
            // создание XML документа
            XmlDocument OptionsXml = new XmlDocument();
            // создание заголовка
            XmlDeclaration OptionsDecl = OptionsXml.CreateXmlDeclaration("1.0", "utf-8", null);
            OptionsXml.AppendChild(OptionsDecl);
            // создание корневого узла
            XmlElement RootNode = OptionsXml.CreateElement("formats");
            OptionsXml.AppendChild(RootNode);
            // создание узла для фильтра файлов и включения монитора каталога
            XmlElement CommonNode = OptionsXml.CreateElement("common");
            RootNode.AppendChild(CommonNode);
            CommonNode.SetAttribute("filefilter",Extension.Text);
            if (EnableWatching.Checked)
                CommonNode.SetAttribute("enablewatching", "true");
            else
                CommonNode.SetAttribute("enablewatching", "false");
            // создание узла для разделителей
            XmlElement DelimitorsNode = OptionsXml.CreateElement("delimitors");
            RootNode.AppendChild(DelimitorsNode);
            DelimitorsNode.SetAttribute("fields",FieldDelimiter.Text);
            DelimitorsNode.SetAttribute("decimal", DecimalDelimiter.Text);
            //DelimitorsNode.SetAttribute("date", DateDelimiter.Text);
            //DelimitorsNode.SetAttribute("time", TimeDelimiter.Text);
            // создание узла индексов полей
            XmlElement IndexesNode = OptionsXml.CreateElement("fieldindexes");
            RootNode.AppendChild(IndexesNode);
            IndexesNode.SetAttribute("ticker",Convert.ToString(TickerIndex));
            IndexesNode.SetAttribute("period",Convert.ToString(PeriodIndex));
            IndexesNode.SetAttribute("date",Convert.ToString(DateIndex));
            IndexesNode.SetAttribute("time",Convert.ToString(TimeIndex));
            IndexesNode.SetAttribute("open",Convert.ToString(OpenIndex));
            IndexesNode.SetAttribute("high",Convert.ToString(HighIndex));
            IndexesNode.SetAttribute("low",Convert.ToString(LowIndex));
            IndexesNode.SetAttribute("close",Convert.ToString(CloseIndex));
            // Создание узла для форматов даты и времени
            XmlElement DateTimeFormatsNode = OptionsXml.CreateElement("datetimeformats");
            RootNode.AppendChild(DateTimeFormatsNode);
            DateTimeFormatsNode.SetAttribute("date", DateFormatMnu.Text);
            DateTimeFormatsNode.SetAttribute("time", TimeFormatMnu.Text);

            // Сохранение документа в файл
            OptionsXml.Save(DirectoryName + "\\" + SettingsFile);
        }

        // Доступ к полю с названием каталога
        public void SetDirectory(string Dir)
        {
            DirectoryName.Text = Dir;
        }
        public string GetDirectory()
        {
            return DirectoryName.Text;
        }
        // Доступ к полю с расширением
        public string GetExtension()
        {
            return Extension.Text.ToUpper();
        }

        // Доступ к разделителю колонок
        public char GetFieldDelimiter()
        {
            return FieldDelimiter.Text[0];
        }
        // Доступ к полю с индексом для тикера
        public int GetTickerIndex()
        {
            return TickerIndex;
        }
        // Доступ к полю с индексом для периода
        public int GetPeriodIndex()
        {
            return PeriodIndex;
        }
        // Доступ к полю с индексом для даты
        public int GetDateIndex()
        {
            return DateIndex;
        }
        // Доступ к полю с индексом для времени
        public int GetTimeIndex()
        {
            return TimeIndex;
        }
        // Доступ к полю с индексом для open
        public int GetOpenIndex()
        {
            return OpenIndex;
        }
        // Доступ к полю с индексом для High
        public int GetHighIndex()
        {
            return HighIndex;
        }
        // Доступ к полю с индексом для Low
        public int GetLowIndex()
        {
            return LowIndex;
        }
        // Доступ к полю с индексом для тикера
        public int GetCloseIndex()
        {
            return CloseIndex;
        }

        // Доступ к разделителю дробной части
        public char GetDecimalDelimiter()
        {
            return DecimalDelimiter.Text[0];
        }

        // Доступ форматам даты и времени
        public string GetDateFormat()
        {
            return DateFormatMnu.Text;
        }
        public string GetTimeFormat()
        {
            return TimeFormatMnu.Text;
        }

        // Доступ к признаку включения мониторинга
        public bool GetWatcherStatus()
        {
            return EnableWatching.Checked;
        }

        void FillExampleData()  //! Заполнение образца данных с привязкой к типам столбцов
        {
            // Показ образца содержимого файла
            try
            {
                ExampleData.Columns.Clear();
                ExampleData.Items.Clear();
                DirectoryInfo di = new DirectoryInfo(DirectoryName.Text);
                if (di.Exists)
                {
                    FileInfo[] filelist = di.GetFiles("?*" + Extension.Text);
                    if (filelist.Length > 0)
                    {
                        string filename = DirectoryName.Text + "\\" + filelist[0].Name;
                        StreamReader sr;
                        // Открытие файла с данными
                        try { sr = File.OpenText(filename); }
                        catch { return; }

                        string line;
                        int i = 5;
                        while (((line = sr.ReadLine()) != null) && (i-- > 0))
                        {
                            string[] cols = line.Split(FieldDelimiter.Text[0]);
                            int ColWidth = ExampleData.Width / cols.Length;
                            if (ExampleData.Columns.Count == 0)
                            {
                                // Прочитана первая строка, разделение ее на столбцы
                                for (int c = 0; c < cols.Length; c++)
                                {
                                    // Добавление столбцов в грид, установка заголовкой
                                    string Header = "";
                                    if (TickerIndex == c) Header = "TICKER";
                                    if (PeriodIndex == c) Header = "PERIOD";
                                    if (DateIndex == c) Header = "DATE";
                                    if (TimeIndex == c) Header = "TIME";
                                    if (OpenIndex == c) Header = Header+(Header != "" ? ";OPEN" : "OPEN");
                                    if (HighIndex == c) Header = Header+(Header != "" ? ";HIGH" : "HIGH");
                                    if (LowIndex == c) Header = Header+(Header != "" ? ";LOW" : "LOW");
                                    if (CloseIndex == c) Header = Header+(Header != "" ? ";CLOSE" : "CLOSE");
                                    ExampleData.Columns.Add(Header, ColWidth, HorizontalAlignment.Left);
                                }
                            }
                            // Заполнение строк и столбцов
                            ExampleData.Items.Add(cols[0]);
                            for (int l = 1; l < cols.Length; l++)
                            {
                                ExampleData.Items[ExampleData.Items.Count-1].SubItems.Add(cols[l]);
                            }
                        }
                    }
                }
            }
            catch { };
        }

        private void SelectDirBtn_Click(object sender, EventArgs e)
        {
            SelectDirDialog.SelectedPath = DirectoryName.Text;
            if (SelectDirDialog.ShowDialog() == DialogResult.OK)
            {
                DirectoryName.Text = SelectDirDialog.SelectedPath;
            }
        }


        private void DirectoryName_TextChanged(object sender, EventArgs e)
        {
            ReadSettings(DirectoryName.Text);
            // Показ образца содержимого файла
            FillExampleData();
        }

        private void Extension_TextChanged(object sender, EventArgs e)
        {
            // Показ образца содержимого файла
            FillExampleData();
        }

        int CurCol = -1;
        private void ExampleData_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            tickerMnu.Checked = false;
            periodMnu.Checked = false;
            dateMnu.Checked = false;
            timeMnu.Checked = false;
            openMnu.Checked = false;
            highMnu.Checked = false;
            lowMnu.Checked = false;
            closeMnu.Checked = false;
            string[] cols = ExampleData.Columns[e.Column].Text.ToUpper().Split(';');
            for (int i = 0; i < cols.Length; i++)
            {
                switch (cols[i])
                {
                    case "TICKER": tickerMnu.Checked = true; break;
                    case "PERIOD": periodMnu.Checked = true; break;
                    case "DATE": dateMnu.Checked = true; break;
                    case "TIME": timeMnu.Checked = true; break;
                    case "OPEN": openMnu.Checked = true; break;
                    case "HIGH": highMnu.Checked = true; break;
                    case "LOW": lowMnu.Checked = true; break;
                    case "CLOSE": closeMnu.Checked = true; break;
                }
            }
            CurCol = e.Column;
            int ColWidth = ExampleData.Width / ExampleData.Columns.Count;
            SelectField.Show(ExampleData,ColWidth * e.Column, 0);
        }

        private void SelectField_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text.ToUpper())
            {
                case "TICKER": {
                                if (TickerIndex == CurCol)
                                    TickerIndex = -1;
                                else
                                {
                                    TickerIndex = CurCol;
                                    if (PeriodIndex == CurCol) PeriodIndex = -1;
                                    if (DateIndex == CurCol) DateIndex = -1;
                                    if (TimeIndex == CurCol) TimeIndex = -1;
                                    if (OpenIndex == CurCol) OpenIndex = -1;
                                    if (HighIndex == CurCol) HighIndex = -1;
                                    if (LowIndex == CurCol) LowIndex = -1;
                                    if (CloseIndex == CurCol) CloseIndex = -1;
                                }
                                break; 
                               }
                case "PERIOD": {
                                if (PeriodIndex == CurCol)
                                    PeriodIndex = -1;
                                else
                                {
                                    if (TickerIndex == CurCol) TickerIndex = -1;
                                    PeriodIndex = CurCol;
                                    if (DateIndex == CurCol) DateIndex = -1;
                                    if (TimeIndex == CurCol) TimeIndex = -1;
                                    if (OpenIndex == CurCol) OpenIndex = -1;
                                    if (HighIndex == CurCol) HighIndex = -1;
                                    if (LowIndex == CurCol) LowIndex = -1;
                                    if (CloseIndex == CurCol) CloseIndex = -1;
                                }
                                break; 
                               }
                case "DATE": {
                                if (DateIndex == CurCol)
                                    DateIndex = -1;
                                else
                                {
                                    if (TickerIndex == CurCol) TickerIndex = -1;
                                    if (PeriodIndex == CurCol) PeriodIndex = -1;
                                    DateIndex = CurCol;
                                    if (TimeIndex == CurCol) TimeIndex = -1;
                                    if (OpenIndex == CurCol) OpenIndex = -1;
                                    if (HighIndex == CurCol) HighIndex = -1;
                                    if (LowIndex == CurCol) LowIndex = -1;
                                    if (CloseIndex == CurCol) CloseIndex = -1;
                                }
                                break; 
                             }
                case "TIME": {
                                if (TimeIndex == CurCol)
                                    TimeIndex = -1;
                                else
                                {
                                    if (TickerIndex == CurCol) TickerIndex = -1;
                                    if (PeriodIndex == CurCol) PeriodIndex = -1;
                                    if (DateIndex == CurCol) DateIndex = -1;
                                    TimeIndex = CurCol;
                                    if (OpenIndex == CurCol) OpenIndex = -1;
                                    if (HighIndex == CurCol) HighIndex = -1;
                                    if (LowIndex == CurCol) LowIndex = -1;
                                    if (CloseIndex == CurCol) CloseIndex = -1;
                                }
                                break; 
                             }
                case "OPEN": {
                                if (OpenIndex == CurCol)
                                    OpenIndex = -1;
                                else
                                {
                                    if (TickerIndex == CurCol) TickerIndex = -1;
                                    if (PeriodIndex == CurCol) PeriodIndex = -1;
                                    if (DateIndex == CurCol) DateIndex = -1;
                                    if (TimeIndex == CurCol) TimeIndex = -1;
                                    OpenIndex = CurCol;
                                    //if (HighIndex == CurCol) HighIndex = -1;
                                    //if (LowIndex == CurCol) LowIndex = -1;
                                    //if (CloseIndex == CurCol) CloseIndex = -1;
                                }
                                break; 
                             }
                case "HIGH": {
                                if (HighIndex == CurCol)
                                    HighIndex = -1;
                                else
                                {
                                    if (TickerIndex == CurCol) TickerIndex = -1;
                                    if (PeriodIndex == CurCol) PeriodIndex = -1;
                                    if (DateIndex == CurCol) DateIndex = -1;
                                    if (TimeIndex == CurCol) TimeIndex = -1;
                                    //if (OpenIndex == CurCol) OpenIndex = -1;
                                    HighIndex = CurCol;
                                    //if (LowIndex == CurCol) LowIndex = -1;
                                    //if (CloseIndex == CurCol) CloseIndex = -1;
                                }
                                break; 
                             }
                case "LOW": {
                                if (LowIndex == CurCol)
                                    LowIndex = -1;
                                else
                                {
                                    if (TickerIndex == CurCol) TickerIndex = -1;
                                    if (PeriodIndex == CurCol) PeriodIndex = -1;
                                    if (DateIndex == CurCol) DateIndex = -1;
                                    if (TimeIndex == CurCol) TimeIndex = -1;
                                    //if (OpenIndex == CurCol) OpenIndex = -1;
                                    //if (HighIndex == CurCol) HighIndex = -1;
                                    LowIndex = CurCol;
                                    //if (CloseIndex == CurCol) CloseIndex = -1;
                                }
                                break; 
                            }
                case "CLOSE": {
                                if (CloseIndex == CurCol)
                                    CloseIndex = -1;
                                else 
                                {
                                    if (TickerIndex == CurCol) TickerIndex = -1;
                                    if (PeriodIndex == CurCol) PeriodIndex = -1;
                                    if (DateIndex == CurCol) DateIndex = -1;
                                    if (TimeIndex == CurCol) TimeIndex = -1;
                                    //if (OpenIndex == CurCol) OpenIndex = -1;
                                    //if (HighIndex == CurCol) HighIndex = -1;
                                    //if (LowIndex == CurCol) LowIndex = -1;
                                    CloseIndex = CurCol;
                                } 
                                break;
                              }
            }

            FillExampleData();
        }

        private void FieldDelimiter_TextChanged(object sender, EventArgs e)
        {
            // Показ образца содержимого файла
            FillExampleData();
        }

        private void DoneBtn_Click(object sender, EventArgs e)
        {
            // Проверка полноты определения полей
            if ((DateIndex == -1) || (OpenIndex == -1) || (HighIndex == -1) || (LowIndex == -1) || (CloseIndex == -1))
            {
                MessageBox.Show("There are not necessary columns in the file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //((System.Windows.Forms.Button)sender).DialogResult = DialogResult.None;
            }
            else
            {
                //((Button)sender).DialogResult = DialogResult.OK;
                ((Form)this).DialogResult = DialogResult.OK;
            }
        }

        private void dateMnu_Click(object sender, EventArgs e)
        {
            dateMnu.Checked = !dateMnu.Checked;
            if (dateMnu.Checked)
            {
                tickerMnu.Checked = false;
                periodMnu.Checked = false;
                timeMnu.Checked = false;
                openMnu.Checked = false;
                highMnu.Checked = false;
                lowMnu.Checked = false;
                closeMnu.Checked = false;
            }
        }

        private void timeMnu_Click(object sender, EventArgs e)
        {
            timeMnu.Checked = !timeMnu.Checked;
            if (timeMnu.Checked)
            {
                tickerMnu.Checked = false;
                periodMnu.Checked = false;
                dateMnu.Checked = false;
                openMnu.Checked = false;
                highMnu.Checked = false;
                lowMnu.Checked = false;
                closeMnu.Checked = false;
            }

        }

    }
}
