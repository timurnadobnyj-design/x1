using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Skilful.Data
{
   delegate bool save_to_file();
    public partial class Symbol_list_ITinvest : Form
    {
        public ITinvest ITinv;
        
        public Symbol_list_ITinvest(ITinvest ITinv_)
        {
            InitializeComponent();

            ITinv = ITinv_;
            
            checkedListBoxMICEX.CheckOnClick = true;
            checkedListBoxRTS.CheckOnClick = true;
            checkedListBoxINDEX.CheckOnClick = true;
            
            if (ITinv.symb_list_MICEX != null)
                for (int i = 0; i < ITinv.symb_list_MICEX.Count; i++)
                    checkedListBoxMICEX.Items.Add(ITinv.symb_list_MICEX[i].symbol + " :  " + ITinv.symb_list_MICEX[i].short_name, ITinv.symb_list_MICEX[i].checked_);
            if (ITinv.symb_list_RTS != null)
                for (int i = 0; i < ITinv.symb_list_RTS.Count; i++)
                    checkedListBoxRTS.Items.Add(ITinv.symb_list_RTS[i].symbol + " :  " + ITinv.symb_list_RTS[i].short_name, ITinv.symb_list_RTS[i].checked_);
            if (ITinv.symb_list_INDEX != null)
                for (int i = 0; i < ITinv.symb_list_INDEX.Count; i++)
                    checkedListBoxINDEX.Items.Add(ITinv.symb_list_INDEX[i].symbol + " :  " + ITinv.symb_list_INDEX[i].short_name, ITinv.symb_list_INDEX[i].checked_);
            
                     

        }
    

        private void button1_Click(object sender, EventArgs e)
        {

          Checked_symbols(ITinv.symb_list_MICEX, checkedListBoxMICEX);
          Checked_symbols(ITinv.symb_list_RTS, checkedListBoxRTS);
          Checked_symbols(ITinv.symb_list_INDEX, checkedListBoxINDEX);
          int index = comboBoxTimePeriod.SelectedIndex + 1;
          this.Close();  
          if (!checkBoxSaveorUpdate.Checked) return;
          try 
          {
              ITinv.cod_ex = ITinv.count_saves = 0;
              ITinv.Get_Data_Symbols(index, dateTimePicker.Value, Convert.ToInt32(textBoxQuantityBars.Text));
          }
          catch (Exception Error)
           {
             MessageBox.Show("Ошибка : " + Error.Message);
           }       


        }

        private void Save_date_to_file(List<Symbol_List> ITinv, DateTime DM, int quntBar)
        {


        }


        private void Checked_symbols(List<Symbol_List> sl, CheckedListBox cb)
        {
            CheckedListBox.CheckedItemCollection checked_symbols;
            string str;
            if (sl != null)
            {
                checked_symbols = cb.CheckedItems;
                for (int k = 0; k < sl.Count; k++) sl[k].checked_ = false;
                for (int i = 0; i < checked_symbols.Count; i++)
                {
                    str = checked_symbols[i].ToString();
                    for (int j = 0; j < sl.Count; j++)
                    {                   
                          if ((sl[j].symbol + " :  " + sl[j].short_name) == str)
                            sl[j].checked_ = true;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ITinv.symb_list_MICEX != null)
            {
                checkedListBoxMICEX.Items.Clear();
                for (int i = 0; i < ITinv.symb_list_MICEX.Count; i++)
                {
                    ITinv.symb_list_MICEX[i].checked_ = false;
                    checkedListBoxMICEX.Items.Add(ITinv.symb_list_MICEX[i].symbol + " :  " + ITinv.symb_list_MICEX[i].short_name, false);

                }
            }
            if (ITinv.symb_list_RTS != null)
            {
                checkedListBoxRTS.Items.Clear();
                for (int i = 0; i < ITinv.symb_list_RTS.Count; i++)
                {
                    ITinv.symb_list_RTS[i].checked_ = false;
                    checkedListBoxRTS.Items.Add(ITinv.symb_list_RTS[i].symbol + " :  " + ITinv.symb_list_RTS[i].short_name, false);

                }
            }
            if (ITinv.symb_list_INDEX != null)
            {
                checkedListBoxINDEX.Items.Clear();
                for (int i = 0; i < ITinv.symb_list_INDEX.Count; i++)
                {
                    ITinv.symb_list_RTS[i].checked_ = false;
                    checkedListBoxINDEX.Items.Add(ITinv.symb_list_INDEX[i].symbol + " :  " + ITinv.symb_list_INDEX[i].short_name, false);

                }
            }

               
        }



 
    }
}
