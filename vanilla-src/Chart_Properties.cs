//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Eugeniy Bazarov (Obolon), Eugeniy Logvinenko (manuka)
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
using ChartV2.Styles;

namespace Skilful
{
    public partial class Chart_Properties : Form
    {
        private ChartV2.Legend legend;
        public ChartV2.Axis_Plot.Axis left;
        public ChartV2.Axis_Plot.Axis right;
        public ChartV2.Axis_Plot.Axis top;
        public ChartV2.Axis_Plot.Axis bottom;
        public ChartV2.Control c;


        public Chart_Properties(ChartV2.Axis_Plot.Axis left, ChartV2.Axis_Plot.Axis right, ChartV2.Axis_Plot.Axis top, ChartV2.Axis_Plot.Axis bottom, ChartV2.Control c)
        {
            this.legend = c.seriesList[0].legend;
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
            this.c = c;

            InitializeComponent();

            if (Skilful.Sample.Write_Skilful_Font.Size > 10)
                labelSkilful2010.Font = new Font(Skilful.Sample.Write_Skilful_Font.Name, 10);
            else
                labelSkilful2010.Font = Skilful.Sample.Write_Skilful_Font;

            labelSkilful2010.ForeColor = Skilful.Sample.Write_Skilful_Font_Color;

            target_label_Color.BackColor = Skilful.Sample.ModelStyle_TargetLable_Color;

            candleUpBorder.BackColor = Skilful.Sample.CandleUpBorder_Color;
            candleDownBorder.BackColor = Skilful.Sample.CandleDownBorder_Color;

            leftAxisBackColorButton.BackColor = Skilful.Sample.AxisStyleLeft_BackColor;
            rightAxisBackColorButton.BackColor = Skilful.Sample.AxisStyleRight_BackColor;
            topAxisBackColorButton.BackColor = Skilful.Sample.AxisStyleTop_BackColor;
            buttonAxisBackColorButton.BackColor = Skilful.Sample.AxisStyleBottom_BackColor;
            
            shadowColor.BackColor = Skilful.Sample.LineChartStyleShadow_Color;
            shadowCheckBox.Checked = Skilful.Sample.LineChartStyle_UseShadow;
            antialias.Checked = Skilful.Sample.LineChartStyle_Antialias;
            smooth.Checked = Skilful.Sample.LineChartStyle_Smooth;
            numericUpDown_smoothingTension.Value = (decimal)Skilful.Sample.LineChartStyle_Tension;
            lineNumericUpDown.Value = (decimal)Skilful.Sample.LineChartStylePen_Width;
            CandleBorderNumericUpDown.Value = (decimal)Skilful.Sample.CandleStyleBorderPen_Width;
            candleUpBody.BackColor = Skilful.Sample.CandleUpBodyFill_Color;
            candleDownBody.BackColor = Skilful.Sample.CandleDownBodyFill_Color;
            lineUp.BackColor = Skilful.Sample.LineChartStyle_Color;

            if (c.ModelListExist)
            {


                notSelectedLineNumUpDown.Value = (decimal)Skilful.Sample.ModelStyle_fadedMainLinesPen_Width;
                notSelectedLineDashedNumUpDown.Value = (decimal)Skilful.Sample.ModelStyle_fadedMainDashedLinesPen_Width;
                notSelectedDashDotLineNumUpDown.Value = (decimal)Skilful.Sample.ModelStyle_fadedMainDashDotLinesPen_Width;
                notSelectedAdditionalLineNumUpDown.Value = (decimal)Skilful.Sample.ModelStyle_fadedAdditionalLinesPen_Width;


                notSelectedLineButton.BackColor = ChartV2.Styles.ModelStyle.fadedMainLinesPen.Color;
                notSelectedDashedLineButton.BackColor = ChartV2.Styles.ModelStyle.fadedMainDashedLinesPen.Color;
                notSelectedDashDotLineButton.BackColor = ChartV2.Styles.ModelStyle.fadedMainDashDotLinesPen.Color;
                notSelectedAddLineButton.BackColor = ChartV2.Styles.ModelStyle.fadedAdditionalLinesPen.Color;


                selectedLineNumUpDo.Value = (decimal)Skilful.Sample.ModelStyle_selectedMainLinesPen_Width;
                selectedDashLineNumUpDo.Value = (decimal)Skilful.Sample.ModelStyle_selectedMainDashedLinesPen_Width;
                selectedDashDotLineNumUpDo.Value = (decimal)Skilful.Sample.ModelStyle_selectedMainDashDotLinesPen_Width;
                selectedAddLineNumUpDo.Value = (decimal)Skilful.Sample.ModelStyle_selectedAdditionalLinesPen_Width;

                selectedLineColor.BackColor = ChartV2.Styles.ModelStyle.selectedMainLinesPen.Color;
                selectedDashLineColor.BackColor = ChartV2.Styles.ModelStyle.selectedMainDashedLinesPen.Color;
                selectedDashDtLineColor.BackColor = ChartV2.Styles.ModelStyle.selectedMainDashDotLinesPen.Color;
                selectedAddLineColor.BackColor = ChartV2.Styles.ModelStyle.selectedAdditionalLinesPen.Color;
            }
            else tabControl.TabPages.Remove(Model_Style);
                

           
            
        }

        private void leftAxisFontButton_Click(object sender, EventArgs e)
        {
            fontDialog1 = new FontDialog();
            fontDialog1.ShowColor = true;
            fontDialog1.Font = Skilful.Sample.AxisStyleLeft_Font;
            fontDialog1.Color = Skilful.Sample.AxisStyleLeft_Font_Color;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                left.style.magorTickFont = fontDialog1.Font;
                left.style.magorFontBrush = new SolidBrush(fontDialog1.Color);
                Skilful.Sample.AxisStyleLeft_Font_Color = fontDialog1.Color;
                Skilful.Sample.AxisStyleLeft_Font = fontDialog1.Font;

            }
            fontDialog1.Dispose();
        }

        private void oKbutton_Click(object sender, EventArgs e)
        {
            this.Close();
            if (this != null)
                this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fontDialog1 = new FontDialog();
            fontDialog1.ShowColor = true;
            fontDialog1.Font = Skilful.Sample.CursorLable_Left_Font;
            fontDialog1.Color = Skilful.Sample.CursorLable_Left_Font_Color;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                left.cursorLable.font = fontDialog1.Font;
                left.cursorLable.fontBrush = new SolidBrush(fontDialog1.Color);
                Skilful.Sample.CursorLable_Left_Font_Color = fontDialog1.Color;
                Skilful.Sample.CursorLable_Left_Font = fontDialog1.Font;
            }
            fontDialog1.Dispose();
        }

        private void leftAxisBackColorButton_Click(object sender, EventArgs e)
        {

            colorDialog1 = new ColorDialog();
            colorDialog1.Color = Skilful.Sample.AxisStyleLeft_BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                left.style.backColor = colorDialog1.Color;
                left.style.backBrush = new SolidBrush(colorDialog1.Color);
                Skilful.Sample.AxisStyleLeft_BackColor = colorDialog1.Color;
               // c.gridSplitter.BackColor = colorDialog1.Color;
                 leftAxisBackColorButton.BackColor = colorDialog1.Color;

            }
            colorDialog1.Dispose();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            fontDialog1 = new FontDialog();
            fontDialog1.ShowColor = true;
            fontDialog1.Font = Skilful.Sample.AxisStyleRight_Font;
            fontDialog1.Color = Skilful.Sample.AxisStyleRight_Font_Color;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                right.style.magorTickFont = fontDialog1.Font;
                right.style.magorFontBrush = new SolidBrush(fontDialog1.Color);
                Skilful.Sample.AxisStyleRight_Font = fontDialog1.Font;
                Skilful.Sample.AxisStyleRight_Font_Color = fontDialog1.Color;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            fontDialog1 = new FontDialog();
            fontDialog1.ShowColor = true;
            fontDialog1.Font = Skilful.Sample.CursorLable_Right_Font;
            fontDialog1.Color = Skilful.Sample.CursorLable_Right_Font_Color;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                right.cursorLable.font = fontDialog1.Font;
                right.cursorLable.fontBrush = new SolidBrush(fontDialog1.Color);
                Skilful.Sample.CursorLable_Right_Font = fontDialog1.Font;
                Skilful.Sample.CursorLable_Right_Font_Color = fontDialog1.Color;
            }
            fontDialog1.Dispose();
        }

        private void rightAxisBackColorButton_Click(object sender, EventArgs e)
        {
            colorDialog1 = new ColorDialog();
            colorDialog1.Color = Skilful.Sample.AxisStyleRight_BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                right.style.backColor = colorDialog1.Color;
                right.style.backBrush = new SolidBrush(colorDialog1.Color);
                Skilful.Sample.AxisStyleRight_BackColor = colorDialog1.Color;
               // c.gridSplitter.BackColor = colorDialog1.Color;
                rightAxisBackColorButton.BackColor = colorDialog1.Color;

            }
            colorDialog1.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            fontDialog1 = new FontDialog();
            fontDialog1.ShowColor = true;
            fontDialog1.Font = Skilful.Sample.AxisStyleTop_Font;
            fontDialog1.Color = Skilful.Sample.AxisStyleTop_Font_Color;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                top.style.magorTickFont = fontDialog1.Font;
                top.style.magorFontBrush = new SolidBrush(fontDialog1.Color);
                Skilful.Sample.AxisStyleTop_Font_Color = fontDialog1.Color;
                Skilful.Sample.AxisStyleTop_Font = fontDialog1.Font;
            }
            fontDialog1.Dispose();
        }

        private void topAxisBackColorButton_Click(object sender, EventArgs e)
        {
            colorDialog1 = new ColorDialog();
            colorDialog1.Color = Skilful.Sample.AxisStyleTop_BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                top.style.backColor = colorDialog1.Color;
                top.style.backBrush = new SolidBrush(colorDialog1.Color);
                Skilful.Sample.AxisStyleTop_BackColor = colorDialog1.Color;
                c.gridSplitter.BackColor = colorDialog1.Color;
                topAxisBackColorButton.BackColor = colorDialog1.Color;
            }
            colorDialog1.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fontDialog1 = new FontDialog();
            fontDialog1.ShowColor = true;
            fontDialog1.Font = Skilful.Sample.AxisStyleBottom_Font;
            fontDialog1.Color = Skilful.Sample.AxisStyleBottom_Font_Color;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                bottom.style.magorTickFont = fontDialog1.Font;
                bottom.style.magorFontBrush = new SolidBrush(fontDialog1.Color);
                Skilful.Sample.AxisStyleBottom_Font_Color = fontDialog1.Color;
                Skilful.Sample.AxisStyleBottom_Font = fontDialog1.Font;
            }
            fontDialog1.Dispose();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            fontDialog1 = new FontDialog();
            fontDialog1.ShowColor = true;
            fontDialog1.Font = Skilful.Sample.CursorLable_Bottom_Font;
            fontDialog1.Color = Skilful.Sample.CursorLable_Bottom_Font_Color;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                bottom.cursorLable.font = fontDialog1.Font;
                bottom.cursorLable.fontBrush = new SolidBrush(fontDialog1.Color);
                Skilful.Sample.CursorLable_Bottom_Font_Color = fontDialog1.Color;
                Skilful.Sample.CursorLable_Bottom_Font = fontDialog1.Font;
            }
            fontDialog1.Dispose();
        }

        private void buttonAxisBackColorButton_Click(object sender, EventArgs e)
        {
            colorDialog1 = new ColorDialog();
            colorDialog1.Color = Skilful.Sample.AxisStyleBottom_BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                bottom.style.backColor = colorDialog1.Color;
                bottom.style.backBrush = new SolidBrush(colorDialog1.Color);
                Skilful.Sample.AxisStyleBottom_BackColor = colorDialog1.Color;
           //     c.gridSplitter.BackColor = colorDialog1.Color;
                buttonAxisBackColorButton.BackColor = colorDialog1.Color;

            }
            colorDialog1.Dispose();
        }

        private void candleUpBorder_Click(object sender, EventArgs e)
        {
            colorDialog1 = new ColorDialog();
            colorDialog1.Color = Skilful.Sample.CandleUpBorder_Color;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                candleUpBorder.BackColor = colorDialog1.Color;
                legend.candleStyle.upBorderPen = new Pen(colorDialog1.Color, legend.candleStyle.upBorderPen.Width);
                Skilful.Sample.CandleUpBorder_Color = colorDialog1.Color;
                candleUpBorder.BackColor = colorDialog1.Color;
            }
            colorDialog1.Dispose();
        }

        private void candleUpBody_Click(object sender, EventArgs e)
        {
            colorDialog1 = new ColorDialog();
            colorDialog1.Color = Skilful.Sample.CandleUpBodyFill_Color;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                candleUpBody.BackColor = colorDialog1.Color;
                legend.candleStyle.upFill = new SolidBrush(colorDialog1.Color);
                Skilful.Sample.CandleUpBodyFill_Color = colorDialog1.Color;
                candleUpBody.BackColor = colorDialog1.Color;
                
            }
            colorDialog1.Dispose();
        }

        private void candleDownBody_Click(object sender, EventArgs e)
        {
            colorDialog1 = new ColorDialog();
            colorDialog1.Color = Skilful.Sample.CandleDownBodyFill_Color;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                candleDownBody.BackColor = colorDialog1.Color;
                legend.candleStyle.donwFill = new SolidBrush(colorDialog1.Color);
                Skilful.Sample.CandleDownBodyFill_Color = colorDialog1.Color;
                candleDownBody.BackColor = colorDialog1.Color;
            }
            colorDialog1.Dispose();
        }

        private void CandleBorderNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            legend.candleStyle.upBorderPen = new Pen(legend.candleStyle.upBorderPen.Color, (int)CandleBorderNumericUpDown.Value);
            Skilful.Sample.CandleStyleBorderPen_Width = (float)CandleBorderNumericUpDown.Value;
        }

        private void lineUp_Click(object sender, EventArgs e)
        {
            colorDialog1 = new ColorDialog();
            colorDialog1.Color = Skilful.Sample.LineChartStyle_Color;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                lineUp.BackColor = colorDialog1.Color;
                legend.lineStyle.pen = new Pen(colorDialog1.Color, legend.lineStyle.pen.Width);
                Skilful.Sample.LineChartStyle_Color = colorDialog1.Color;
                lineUp.BackColor = colorDialog1.Color;
            }
            colorDialog1.Dispose();
        }

        private void lineNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            legend.lineStyle.pen = new Pen(legend.lineStyle.pen.Color, (int)lineNumericUpDown.Value);
            Skilful.Sample.LineChartStylePen_Width = (float)lineNumericUpDown.Value;

        }

        private void numericUpDown_smoothingTension_ValueChanged(object sender, EventArgs e)
        {
            Skilful.Sample.LineChartStyle_Tension = legend.lineStyle.tension = (float)numericUpDown_smoothingTension.Value;
            

        }

        private void shadowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
           Skilful.Sample.LineChartStyle_UseShadow = legend.lineStyle.useShadow = shadowCheckBox.Checked;

        }

        private void shadowColor_Click(object sender, EventArgs e)
        {
            colorDialog1 = new ColorDialog();
            colorDialog1.Color = Skilful.Sample.LineChartStyleShadow_Color;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                shadowColor.BackColor = colorDialog1.Color;
                legend.lineStyle.ShadowBrush = new SolidBrush(colorDialog1.Color);
                Skilful.Sample.LineChartStyleShadow_Color = colorDialog1.Color;
                shadowColor.BackColor = colorDialog1.Color;
            }
            colorDialog1.Dispose();
        }

        private void smooth_CheckedChanged(object sender, EventArgs e)
        {
          Skilful.Sample.LineChartStyle_Smooth = legend.lineStyle.smooth = smooth.Checked;
        }

        private void notSelectedLineNumUpDown_ValueChanged(object sender, EventArgs e)
        {
            ChartV2.Styles.ModelStyle.fadedMainLinesPen.Width = (float)notSelectedLineNumUpDown.Value;
         }

        private void notSelectedLineDashedNumUpDown_ValueChanged(object sender, EventArgs e)
        {
            ChartV2.Styles.ModelStyle.fadedMainDashedLinesPen.Width = (float)notSelectedLineDashedNumUpDown.Value;
        }

        private void notSelectedDashDotLineNumUpDown_ValueChanged(object sender, EventArgs e)
        {
            ChartV2.Styles.ModelStyle.fadedMainDashDotLinesPen.Width = (float)notSelectedDashDotLineNumUpDown.Value;
        }

        private void notSelectedAdditionalLineNumUpDown_ValueChanged(object sender, EventArgs e)
        {
            ChartV2.Styles.ModelStyle.fadedAdditionalLinesPen.Width = (float)notSelectedAdditionalLineNumUpDown.Value;
        }

        private void selectedLineNumUpDo_ValueChanged(object sender, EventArgs e)
        {
            ChartV2.Styles.ModelStyle.selectedMainLinesPen.Width = (float)selectedLineNumUpDo.Value;
        }

        private void selectedDashLineNumUpDo_ValueChanged(object sender, EventArgs e)
        {
            ChartV2.Styles.ModelStyle.selectedMainDashedLinesPen.Width = (float)selectedDashLineNumUpDo.Value;
        }

        private void selectedDashDotLineNumUpDo_ValueChanged(object sender, EventArgs e)
        {
            ChartV2.Styles.ModelStyle.selectedMainDashDotLinesPen.Width = (float)selectedDashDotLineNumUpDo.Value;
        }

        private void selectedAddLineNumUpDo_ValueChanged(object sender, EventArgs e)
        {
            ChartV2.Styles.ModelStyle.selectedAdditionalLinesPen.Width = (float)selectedAddLineNumUpDo.Value;
        }

        private void mainLableFontButton_Click(object sender, EventArgs e)
        {
            fontDialog1 = new FontDialog();
            fontDialog1.ShowColor = true;
            fontDialog1.Color = Skilful.Sample.ModelStyle_mainLablesFont_Color;
            fontDialog1.Font = Skilful.Sample.ModelStyle_mainLablesFont;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                ChartV2.Styles.ModelStyle.mainLablesFont = fontDialog1.Font;
                Skilful.Sample.ModelStyle_mainLablesFont = fontDialog1.Font;
                ChartV2.Styles.ModelStyle.mainLablesFontBrush = new SolidBrush(fontDialog1.Color);
                Skilful.Sample.ModelStyle_mainLablesFont_Color = fontDialog1.Color;
            }
            fontDialog1.Dispose();
        }

        private void additionalLableFontButton_Click(object sender, EventArgs e)
        {
            fontDialog1 = new FontDialog();
            fontDialog1.Font = Skilful.Sample.ModelStyle_additionalLablesFont;
            fontDialog1.ShowColor = false;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                Skilful.Sample.ModelStyle_additionalLablesFont = fontDialog1.Font;
                ChartV2.Styles.ModelStyle.additionalLablesFont = fontDialog1.Font;
            }
            fontDialog1.Dispose();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            fontDialog1 = new FontDialog();
            fontDialog1.ShowColor = true;
            fontDialog1.Color = Skilful.Sample.Write_Skilful_Font_Color;
            fontDialog1.Font = Skilful.Sample.Write_Skilful_Font;
            fontDialog1.MaxSize = 28;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {

                if (fontDialog1.Font.Size > 10)
                    labelSkilful2010.Font = new Font(fontDialog1.Font.Name, 10F);
                else
                    labelSkilful2010.Font = fontDialog1.Font;
                labelSkilful2010.ForeColor = fontDialog1.Color;
                Skilful.Sample.Write_Skilful_Font = fontDialog1.Font;
                Skilful.Sample.Write_Skilful_Font_Color = fontDialog1.Color;
            }
            fontDialog1.Dispose();

        }

        private void candleDownBorder_Click(object sender, EventArgs e)
        {
            colorDialog1 = new ColorDialog();
            colorDialog1.Color = Skilful.Sample.CandleDownBorder_Color;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                candleDownBorder.BackColor = colorDialog1.Color;
                legend.candleStyle.downBorderPen = new Pen(colorDialog1.Color, Skilful.Sample.CandleStyleBorderPen_Width);
                Skilful.Sample.CandleDownBorder_Color = colorDialog1.Color;
                candleDownBorder.BackColor = colorDialog1.Color;                
            }
            colorDialog1.Dispose();
        }

        private void target_label_Font_Click(object sender, EventArgs e)
        {
            fontDialog1 = new FontDialog();
            fontDialog1.ShowColor = true;
            fontDialog1.Color = Skilful.Sample.ModelStyle_TargetLable_Font_Color;
            fontDialog1.Font = Skilful.Sample.ModelStyle_TargetLable_Font;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                Skilful.Sample.ModelStyle_TargetLable_Font = fontDialog1.Font;
                Skilful.Sample.ModelStyle_TargetLable_Font_Color = fontDialog1.Color;

             //!!!!!!======== Убрать эти строчки после решения вопроса о автомасштабировании !!!! =====
             
                Skilful.Sample.AxisStyleRight_Font = right.style.magorTickFont =  fontDialog1.Font;
                Skilful.Sample.CursorLable_Right_Font = right.cursorLable.font = fontDialog1.Font;
             
             //!!!!!!======================================================================!!!! ========   
                
            }
            fontDialog1.Dispose();
        }

        private void target_label_Color_Click(object sender, EventArgs e)
        {
            colorDialog1 = new ColorDialog();
            colorDialog1.Color = Skilful.Sample.ModelStyle_TargetLable_Color;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Skilful.Sample.ModelStyle_TargetLable_Color = colorDialog1.Color;
                ModelStyle.lablesBrush = new SolidBrush(colorDialog1.Color);
                target_label_Color.BackColor = colorDialog1.Color;
            }
            colorDialog1.Dispose();

        }

        private void antialias_CheckedChanged(object sender, EventArgs e)
        {
            Skilful.Sample.LineChartStyle_Antialias = legend.antialias = antialias.Checked; 
        }

   
    }
}
