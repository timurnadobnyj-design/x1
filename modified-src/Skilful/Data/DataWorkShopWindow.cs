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
using Skilful.QuotesManager;

namespace ChartV2
{
    public partial class DataWorkShopWindow : Form
    {
        private int rowsCount = 0;
        public string decimalSeparator = ".";
        public NumberFormatInfo numInfo = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
        public Data.Series ds;
        private char ColsDel;
        private char RowsDel;
        private string[] parsedText;



        public DataWorkShopWindow(int rowcount, string str)
        {
            InitializeComponent();
            rowsCount = rowcount;
            richTextBox1.Text = str;
            ColsSeparatorBox.Items.Add((object)',');
            ColsSeparatorBox.Items.Add((object)'.');
            //стараемся найти сколько строк в файле
            if (rowsCount > NumberOfRows.Maximum)
            {
                //на случай того что строк будет больше, чем вмещает int32
                MessageBox.Show("rowsCount > numericUpDown3.Maximum   This Might result in Cutted data");
                NumberOfRows.Value = NumberOfRows.Maximum;
            }
            else
                NumberOfRows.Value = rowsCount;



            NumberOfRows2.Text = rowsCount.ToString();

            //стараемся найти сколько колонок в файле
            int indexOfnewLine = richTextBox1.Text.IndexOf((char)Keys.LineFeed);
            int indexOfnewCol = 0;
            int i = 0;
            while (indexOfnewCol < indexOfnewLine)
            {
                indexOfnewCol = richTextBox1.Text.IndexOf((char)ColsSeparatorBox.Items[0], indexOfnewCol + 1, indexOfnewLine);
                i++;
                if ((indexOfnewCol >= indexOfnewLine)||(indexOfnewCol==-1))
                    break;
            }
            NumberOfCols.Value = i;




            splitContainer1.SplitterDistance = 10;

            numInfo.NumberDecimalSeparator = decimalSeparator;


            ColorPickerBox.Items.Add(Color.Green);
            ColorPickerBox.Items.Add(Color.Red);
            ColorPickerBox.Items.Add(Color.Blue);
            ColorPickerBox.SelectedItem = ColorPickerBox.Items[0];

        }

        private void ArrangeToGrid_Click(object sender, EventArgs e)
        {
            FieldOrderBox.Enabled = true;
            
            RefreshDelimiters();
            char[] dels = new char[] { ColsDel, RowsDel, (char)Keys.LineFeed };
            parsedText = richTextBox1.Text.Split(dels);

            dataGrid.ColumnCount = (int)NumberOfCols.Value;
            dataGrid.RowCount = 5;

            int res = 0;
            float resFloat = 0F;
            string temp = "";

            for (int j = 0; j < dataGrid.RowCount; j++)
            {
                for (int i = 0; i < dataGrid.ColumnCount; i++)
                {
                    temp = parsedText[j * (dataGrid.ColumnCount) + i];

                    if (!(Int32.TryParse(temp, out res)))
                    {
                        if (!(System.Single.TryParse(temp, NumberStyles.Float, numInfo, out resFloat)))
                            goto stringtLable;
                        goto floatLable;
                    }
                    dataGrid.Rows[j].Cells[i].Value = res;
                    continue;
                floatLable:
                    dataGrid.Rows[j].Cells[i].Value = resFloat;
                    continue;
                stringtLable:
                    dataGrid.Rows[j].Cells[i].Value = temp;
                    continue;
                }
            }

        }

        private void ToHeadersButton_Click(object sender, EventArgs e)
        {

            if (dataGrid.RowCount > 0)
            {
                for (int i = 0; i < dataGrid.ColumnCount; i++)
                {
                    try
                    {
                        //dataGrid.Columns[i].HeaderText = (string)dataGrid.Rows[0].Cells[i].Value + dataGrid.Rows[0].Cells[i].Value.GetType().ToString();
                        dataGrid.Columns[i].HeaderText = dataGrid.Rows[2].Cells[i].Value.GetType().ToString();
                        dataGrid.Rows[0].Cells[i].Dispose();
                        // throw new Exception("Well, some promlems while casting this cell #"+ i.ToString()+"   !!!");
                        int ii = richTextBox1.Text.IndexOf('\n');


                        richTextBox1.Text = richTextBox1.Text.Remove(0, ii + 1);
                        NumberOfRows2.Text = (--rowsCount).ToString();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }

                dataGrid.Rows.RemoveAt(0);
            }

        }

        private void GoChartButton_Click(object sender, EventArgs e)
        {
        //    //  MessageBox.Show("Please, check if you casted needed cells to Int32s!!! \n"+
        //    // "Going to chart " + ((int)numericUpDown1.Value-1).ToString()+" colomn.");
            try
            {

                if (dataGrid.ColumnCount == 0)
                {
                    MessageBox.Show("No Data was provided to chart!!! Fatality!!!");
                    this.DialogResult = DialogResult.None;
                    //goto endOfFunction;
                    return;
                }


                RefreshDelimiters();
                char[] dels = new char[] { ColsDel, RowsDel, (char)Keys.LineFeed };
                parsedText = richTextBox1.Text.Split(dels);

                object[] tempObjArray = new object[FieldOrder.Items.Count];
                FieldOrder.Items.CopyTo(tempObjArray, 0);
                ds = new Data.Series(tempObjArray, new Legend(textBox1.Text,(Color)ColorPickerBox.SelectedItem, ChartType.Candle, TF.custom));


                //ArrayList tempList = null;
                int tempInt = 0;
                string tempString = "";
                DateTime T = new DateTime();
                DateTime D = new DateTime();
                for (int j = 0; j < rowsCount; j++)
                {
                    TBar B = new TBar();

                    foreach (object ob in tempObjArray)
                    {
                        switch (ob.ToString())
                        {
                            case "Close":
                                tempInt = FieldOrder.Items.IndexOf("Close");

                                tempString = parsedText[j * (FieldOrder.Items.Count) + tempInt];
                                //ds.close.Add(Single.Parse(tempString));
                                B.Close = Single.Parse(tempString);
                                break;
                            case "Open":

                                tempInt = FieldOrder.Items.IndexOf("Open");
                                tempString = parsedText[j * (FieldOrder.Items.Count) + tempInt];
                                // ds.open.Add(Single.Parse(tempString));
                                B.Open = Single.Parse(tempString);
                                break;
                            case "High":
                                tempInt = FieldOrder.Items.IndexOf("High");
                                tempString = parsedText[j * (FieldOrder.Items.Count) + tempInt];
                                //    ds.high.Add(Single.Parse(tempString));
                                B.High = Single.Parse(tempString);

                                break;
                            case "Low":
                                tempInt = FieldOrder.Items.IndexOf("Low");
                                tempString = parsedText[j * (FieldOrder.Items.Count) + tempInt];
                                //    ds.low.Add(Single.Parse(tempString));
                                B.Low = Single.Parse(tempString);
                                break;
                            case "Volume":
                                //    tempInt = FieldOrder.Items.IndexOf("Volume");
                                //    for (int j = 0; j < dataGrid.RowCount; j++)
                                //        ds.volume.Add((int)dataGrid.Rows[j].Cells[tempInt].Value);

                                break;
                            case "OI":
                                /*tempInt = FieldOrder.Items.IndexOf("OI");
                                 for (int j = 0; j < rowsCount; j++)
                                     ds.oi.Add((int)dataGrid.Rows[j].Cells[tempInt].Value);
                                 */
                                break;

                            case "Time":
                                tempInt = FieldOrder.Items.IndexOf("Time");
                                tempString = parsedText[j * (FieldOrder.Items.Count) + tempInt];
                                
                                T = DateTime.ParseExact(tempString, TimeFormatBox.Text, null);
                                
                                //ds.time.Add(int.Parse(tempString));

                                break;
                            case "Date":
                                tempInt = FieldOrder.Items.IndexOf("Date");
                                tempString = parsedText[j * (FieldOrder.Items.Count) + tempInt];

                                D = DateTime.ParseExact(tempString, DateFormatBox.Text, CultureInfo.InvariantCulture,DateTimeStyles.AllowWhiteSpaces);
                                
                                //ds.date.Add(int.Parse(tempString));

                                break;
                            default:
                                MessageBox.Show("Данное поле нужно было удалить: " + ob.ToString());
                                break;
                        }
                    }
                    if (tempObjArray.Length == 2)
                    {
                        B.Open = B.Close;
                        B.High = B.Close;
                        B.Low = B.Close;
                    }
                    B.DT = D.Add(T-T.Date);
                    B.Bar = j;
                    ds.data.Add(B);

                }
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Очевидно Вы ошиблись с количеством колонок, которые нужно преобразовать.");
                this.DialogResult = DialogResult.None;
                ds = null;

                return;
            }
            //ds.viewPort = new Data.ViewPort(ds);
            
}

        public int SaveConverted(string file)
        {
            try
            {
                StreamWriter sw = new StreamWriter(file);

                for (int i = 0; i < ds.data.Count; i++)
                {
                    sw.WriteLine(ds.data[i].DT.ToString("yyyy.MM.dd,") + ds.data[i].DT.ToString("HH:mm,") +
                        ds.data[i].Open.ToString("0.0000") +","+
                        ds.data[i].High.ToString("0.0000") +","+
                        ds.data[i].Low.ToString("0.0000") +","+
                        ds.data[i].Close.ToString("0.0000"));
                }

                sw.Close();
            }
            catch
            {
                MessageBox.Show("Failed to write " + file, "Skilful", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return 0;
            }

            return 1;

        }

        private void ShowTextButton_Click(object sender, EventArgs e)
        {

            switch (ShowTextButton.Checked.ToString())
            {
                case "True":
                    //splitContainer1.Panel1.Hide();
                    splitContainer1.SplitterDistance = 10;
                    ShowTextButton.Checked = false;

                    break;


                case "False":
                    // splitContainer1.Panel1.Show();
                    splitContainer1.SplitterDistance = 100;
                    ShowTextButton.Checked = true;
                    break;
            }

        }

        private void DecimalSepBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (DecimalSepBox.SelectedItem.ToString() != "space")
            {
                numInfo.NumberDecimalSeparator = DecimalSepBox.SelectedItem.ToString();
            }
            else if (DecimalSepBox.SelectedItem.ToString() == "space")
            {
                numInfo.NumberDecimalSeparator = " ";
            }

        }


        private void MoveDownButton_Click(object sender, EventArgs e)
        {
            object obj = FieldOrder.SelectedItem;
            int selection = FieldOrder.SelectedIndex + 1;
            if (obj.ToString() == "...")
            {
                FieldOrder.Items[FieldOrder.SelectedIndex] = "..";
                obj = FieldOrder.SelectedItem;
                if (selection < FieldOrder.Items.Count)
                {
                    FieldOrder.Items.Remove(obj);
                    FieldOrder.Items.Insert(selection, "...");
                    FieldOrder.SelectedIndex = selection;
                }
            }
            else
            {



                if (selection < FieldOrder.Items.Count)
                {
                    FieldOrder.Items.Remove(obj);
                    FieldOrder.Items.Insert(selection, obj);
                    FieldOrder.SelectedIndex = selection;
                }
            }
        }



        private void MoveUpButton_Click(object sender, EventArgs e)
        {
            if (FieldOrder.SelectedItem != null)
            {
                object obj = FieldOrder.SelectedItem;
                int selection = FieldOrder.SelectedIndex - 1;
                if (obj.ToString() == "...")
                {
                    FieldOrder.Items[FieldOrder.SelectedIndex] = "..";
                    obj = FieldOrder.SelectedItem;
                    if (selection >= 0)
                    {
                        FieldOrder.Items.Remove(obj);
                        FieldOrder.Items.Insert(selection, "...");
                        FieldOrder.SelectedIndex = selection;
                    }
                }
                else
                {
                    if (selection >= 0)
                    {
                        FieldOrder.Items.Remove(obj);
                        FieldOrder.Items.Insert(selection, obj);
                        FieldOrder.SelectedIndex = selection;
                    }
                }
            }
        }

        private void DeleteFieldButton_Click(object sender, EventArgs e)
        {
            if (FieldOrder.SelectedIndex > -1)
            {
                DataGridViewColumnCollection cc = new DataGridViewColumnCollection(dataGrid);
                cc = dataGrid.Columns; //.RemoveAt(FieldOrder.SelectedIndex);
                cc.RemoveAt(FieldOrder.SelectedIndex);
                FieldOrder.Items.Remove(FieldOrder.SelectedItem);
                FieldOrder_Click(null, null);
            }
        }

        private void AddIoButton_Click(object sender, EventArgs e)
        {

            if (!FieldOrder.Items.Contains("OI"))
                FieldOrder.Items.Add("OI");

        }

        private void AddFilterField_Click(object sender, EventArgs e)
        {
            FieldOrder.Items.Insert(0, "...");
        }

        private void OpenTemplateButton_Click(object sender, EventArgs e)
        {


            FieldOrder.Items.Clear();
            openTemplatesDialog.InitialDirectory = "c:\\";
            openTemplatesDialog.Filter = "ASCII files (*.txt)|*.txt|All files|*.*";
            if (openTemplatesDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader streamReader = new StreamReader(openTemplatesDialog.FileName);

                while (streamReader.EndOfStream == false)
                {
                    FieldOrder.Items.Add(streamReader.ReadLine());
                }
                streamReader.Dispose();
            }


        }

        private void SaveButton_Click(object sender, EventArgs e)
        {

            saveFileDialog1.Filter = "ASCII files (*.txt)|*.txt|All files|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                for (int j = 0; j < dataGrid.RowCount; j++)
                {
                    for (int i = 0; i < dataGrid.ColumnCount; i++)
                    {

                        if (i == dataGrid.ColumnCount - 1)
                        {
                            sw.WriteLine(dataGrid.Rows[j].Cells[i].Value.ToString());
                        }
                        else
                        {
                            sw.Write(dataGrid.Rows[j].Cells[i].Value.ToString() + ",");
                        }

                    }
                } sw.Close();
            }
        }

        private void ColorPickerBox_SelectedValueChanged(object sender, EventArgs e)
        {

            ColorPickerBox.BackColor = (Color)ColorPickerBox.SelectedItem;
        }

        private void FieldOrder_Click(object sender, EventArgs e)
        {
            if (dataGrid.ColumnCount == FieldOrder.Items.Count)
                groupBox2.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (FieldOrder.SelectedIndex > -1)
            {
                FieldOrder.Items.Remove(FieldOrder.SelectedItem);
            }
            FieldOrder_Click(null, null);
        }

        private void RefreshDelimiters()
        {
            switch (RowsSeparatorBox.SelectedText)
            {
                case "Enter":
                    RowsDel = (char)Keys.Enter;
                    break;

                default:
                    RowsDel = (char)Keys.Enter;
                    break;
            }


            switch (ColsSeparatorBox.SelectedText)
            {
                case ",":
                    ColsDel = ',';
                    break;

                default:
                    ColsDel = ',';
                    break;
            }
        }

        private void FieldOrder_ControlAdded(object sender, ControlEventArgs e)
        {
            if (dataGrid.ColumnCount == FieldOrder.Items.Count)
            {
                groupBox2.Enabled = true;
                //MessageBox.Show("Количество колонок для преобразования не совпадает с количеством колонок в таблице!");
            }
        }


    }
}
