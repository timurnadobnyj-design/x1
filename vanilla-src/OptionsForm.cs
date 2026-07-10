//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Andrey Prokhorov(AVP)
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Skilful
    {
    public partial class OptionsForm : Form
        {
            public OptionsForm()
            {
                InitializeComponent();
            }

            private void ModelLbl1_Click(object sender, EventArgs e)  //! Изменение цвета модели
            {
                colorDialog.Color = (sender as System.Windows.Forms.Label).ForeColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    // Изменение цвета для модели с номером ...
                    string s = (sender as System.Windows.Forms.Label).Name;
                    int i = Convert.ToInt32(Convert.ToInt32(s.Substring(8)))-1;
                    (sender as System.Windows.Forms.Label).ForeColor = colorDialog.Color;
                    GlobalMembersTAmodel.cl[i] = (sender as System.Windows.Forms.Label).ForeColor.ToArgb();
                }

            }

            private void OptionsForm_Shown(object sender, EventArgs e)
            {
                // Установка значений цветов из массива цветов
                ModelLbl1.ForeColor = Color.FromArgb(GlobalMembersTAmodel.cl[0]);
                ModelLbl2.ForeColor = Color.FromArgb(GlobalMembersTAmodel.cl[1]);
                ModelLbl3.ForeColor = Color.FromArgb(GlobalMembersTAmodel.cl[2]);
                ModelLbl4.ForeColor = Color.FromArgb(GlobalMembersTAmodel.cl[3]);
                ModelLbl5.ForeColor = Color.FromArgb(GlobalMembersTAmodel.cl[4]);
                ModelLbl6.ForeColor = Color.FromArgb(GlobalMembersTAmodel.cl[5]);
                ModelLbl7.ForeColor = Color.FromArgb(GlobalMembersTAmodel.cl[6]);
                ModelLbl8.ForeColor = Color.FromArgb(GlobalMembersTAmodel.cl[7]);
                ModelLbl9.ForeColor = Color.FromArgb(GlobalMembersTAmodel.cl[8]);
                ModelLbl10.ForeColor = Color.FromArgb(GlobalMembersTAmodel.cl[9]);
                ModelLbl11.ForeColor = Color.FromArgb(GlobalMembersTAmodel.cl[10]);

                // Установка параметров для поиска моделей
                // Размеры экстремумов
                ExtremumSize1.Value = ModelManager.Options.Extremum1;
                // Коэффициенты трансформации МР->МДР и ЧМП->МДР
                MDBSize1.Value = ModelManager.Options.MBSize1;
                MDBSize2.Value = ModelManager.Options.MBSize2;
                // Правила отфильтровывания моделей
/*                for (int i = 0; i < RulesBox.Items.Count; i++)
                {
                    if ((ModelManager.Options.CheckedErrors1 & ModelManager.Common.ModelErrors[i + 2].ErrorCode) == ModelManager.Common.ModelErrors[i + 2].ErrorCode)
                    {
                        RulesBox.SetItemChecked(i,true);
                    }
                }
*/            }


            private void OptionsForm_FormClosing(object sender, FormClosingEventArgs e)
            {
                ModelManager.Options.Extremum1 = Convert.ToInt32(ExtremumSize1.Value);

                ModelManager.Options.MBSize1 = Convert.ToInt32(MDBSize1.Value);
                ModelManager.Options.MBSize2 = Convert.ToInt32(MDBSize2.Value);

/*                ModelManager.Options.CheckedErrors1 = 0;
                for (int i = 0; i < RulesBox.CheckedIndices.Count; i++)
                {
                    ModelManager.Options.CheckedErrors1 = ModelManager.Options.CheckedErrors1 + ModelManager.Common.ModelErrors[RulesBox.CheckedIndices[i] + 2].ErrorCode;
                }
*/            }
        }
    }
