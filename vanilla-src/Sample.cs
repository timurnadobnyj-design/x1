//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Eugeniy Bazarov(obolon)
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
//---------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ChartV2;
using ChartV2.Axis_Plot;
using ChartV2.Styles;
using GridSplitter;
using Skilful.Data;
using Skilful.QuotesManager;

namespace Skilful
{
 [System.Serializable]
  public class Sample
    {
     
     // ================== Блок констант =========================================================

     static readonly Font Default_Write_Skilful_Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
     static readonly Color Default_Write_Skilful_Font_Color = Color.DarkSlateGray;
       
     static readonly ChartType Default_ChartType = ChartType.Candle;

     static readonly Color Default_CandleUpBodyFill_Color = Color.White;
     static readonly Color Default_CandleDownBodyFill_Color = Color.Black;
     static readonly Color Default_CandleDownBorder_Color = Color.Black;
     static readonly Color Default_CandleUpBorder_Color = Color.Black;
     static readonly float Default_CandleStyleBorderPen_Width = 1F;
     
     static readonly Color Default_LineChartStyle_Color = Color.Black;
     static readonly float Default_LineChartStyle_Tension = 0.3F;
     static readonly float Default_LineChartStylePen_Width = 1F;
     static readonly bool  Default_LineChartStyle_Smooth = false;
     static readonly bool Default_LineChartStyle_Antialias = false;
     static readonly bool Default_LineChartStyle_UseShadow = true;
     static readonly Color Default_LineChartStyleShadow_Color = Color.LightYellow;

     static readonly Color Default_Chart_backColor = Color.White;
     static readonly Color Default_Grid_Color = Color.Gray;
     static readonly bool Default_GridSplitter_IsGridVisible = true;
     static readonly bool Default_Grid_IsVisible = true;
     
     static readonly Font Default_AxisStyleRight_Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
     static readonly Font Default_AxisStyleLeft_Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
     static readonly Font Default_AxisStyleTop_Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
     static readonly Font Default_AxisStyleBottom_Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);

     static readonly Color Default_AxisStyleRight_Font_Color = Color.Black;
     static readonly Color Default_AxisStyleLeft_Font_Color = Color.Black;
     static readonly Color Default_AxisStyleTop_Font_Color = Color.Black;
     static readonly Color Default_AxisStyleBottom_Font_Color = Color.Black;

     static readonly Color Default_AxisStyleRight_BackColor = Color.WhiteSmoke;
     static readonly Color Default_AxisStyleLeft_BackColor = Color.WhiteSmoke;
     static readonly Color Default_AxisStyleTop_BackColor = Color.WhiteSmoke;
     static readonly Color Default_AxisStyleBottom_BackColor = Color.WhiteSmoke;
   
     static readonly Font Default_CursorLable_Right_Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
     static readonly Font Default_CursorLable_Left_Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
     static readonly Font Default_CursorLable_Bottom_Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);

     static readonly Color Default_CursorLable_Right_Font_Color = Color.Black;
     static readonly Color Default_CursorLable_Left_Font_Color = Color.Black;
     static readonly Color Default_CursorLable_Bottom_Font_Color = Color.Black;

     static readonly Font Default_ModelStyle_TargetLable_Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
     static readonly Color Default_ModelStyle_TargetLable_Color = Color.FromArgb(255, Color.FromArgb(GlobalMembersTAmodel.cl[0 % 11]));
     static readonly Color Default_ModelStyle_TargetLable_Font_Color = Color.Black;
    
     static readonly Font Default_ModelStyle_mainLablesFont = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
     static readonly Color Default_ModelStyle_mainLablesFont_Color = Color.Black;
    
     static readonly Font Default_ModelStyle_additionalLablesFont = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);

     static readonly float Default_ModelStyle_fadedMainLinesPen_Width = 1;
     static readonly float Default_ModelStyle_fadedMainDashedLinesPen_Width = 1;
     static readonly float Default_ModelStyle_fadedMainDashDotLinesPen_Width = 1;
     static readonly float Default_ModelStyle_fadedAdditionalLinesPen_Width =  1;

     static readonly float Default_ModelStyle_selectedMainLinesPen_Width = 2;
     static readonly float Default_ModelStyle_selectedMainDashedLinesPen_Width = 2;
     static readonly float Default_ModelStyle_selectedMainDashDotLinesPen_Width = 2;
     static readonly float Default_ModelStyle_selectedAdditionalLinesPen_Width = 1;

   // =======================================================================

   // ===== Первоначальная инициализация  по дефолту ========================

     static public Font Write_Skilful_Font = Default_Write_Skilful_Font;
     static public Color Write_Skilful_Font_Color = Default_Write_Skilful_Font_Color;

     static public string currentSample = "Default";

     static public ChartType ChartType_ = Default_ChartType;

     static public Color CandleUpBodyFill_Color = Default_CandleUpBodyFill_Color;
     static public Color CandleDownBodyFill_Color = Default_CandleDownBodyFill_Color;
     static public Color CandleDownBorder_Color = Default_CandleDownBorder_Color;
     static public Color CandleUpBorder_Color = Default_CandleUpBorder_Color;
     static public float CandleStyleBorderPen_Width = Default_CandleStyleBorderPen_Width;

     static public Color LineChartStyle_Color = Default_LineChartStyle_Color;
     static public float LineChartStylePen_Width = Default_LineChartStylePen_Width;
     static public float LineChartStyle_Tension = Default_LineChartStyle_Tension;
     static public bool LineChartStyle_Smooth = Default_LineChartStyle_Smooth;
     static public bool LineChartStyle_Antialias = Default_LineChartStyle_Antialias;
     static public bool LineChartStyle_UseShadow = Default_LineChartStyle_UseShadow;
     static public Color LineChartStyleShadow_Color = Default_LineChartStyleShadow_Color;

     static public Color Chart_backColor = Default_Chart_backColor;
     static public Color Grid_Color = Default_Grid_Color;
     static public bool GridSplitter_IsGridVisible = Default_GridSplitter_IsGridVisible;
     static public bool Grid_IsVisible = Default_Grid_IsVisible;

     static public Font AxisStyleRight_Font = Default_AxisStyleRight_Font;
     static public Font AxisStyleLeft_Font = Default_AxisStyleLeft_Font;
     static public Font AxisStyleTop_Font = Default_AxisStyleTop_Font;
     static public Font AxisStyleBottom_Font = Default_AxisStyleBottom_Font;

     static public Color AxisStyleRight_Font_Color = Default_AxisStyleRight_Font_Color;
     static public Color AxisStyleLeft_Font_Color = Default_AxisStyleLeft_Font_Color;
     static public Color AxisStyleTop_Font_Color = Default_AxisStyleTop_Font_Color;
     static public Color AxisStyleBottom_Font_Color = Default_AxisStyleBottom_Font_Color;

     static public Color AxisStyleRight_BackColor = Default_AxisStyleRight_BackColor;
     static public Color AxisStyleLeft_BackColor = Default_AxisStyleLeft_BackColor;
     static public Color AxisStyleTop_BackColor = Default_AxisStyleTop_BackColor;
     static public Color AxisStyleBottom_BackColor = Default_AxisStyleBottom_BackColor;

     static public Font CursorLable_Right_Font = Default_CursorLable_Right_Font;
     static public Font CursorLable_Left_Font = Default_CursorLable_Left_Font;
     static public Font CursorLable_Bottom_Font = Default_CursorLable_Bottom_Font;

     static public Color CursorLable_Right_Font_Color = Default_CursorLable_Right_Font_Color;
     static public Color CursorLable_Left_Font_Color = Default_CursorLable_Left_Font_Color;
     static public Color CursorLable_Bottom_Font_Color = Default_CursorLable_Bottom_Font_Color;

     static public Font ModelStyle_TargetLable_Font = Default_ModelStyle_TargetLable_Font;
     static public Color ModelStyle_TargetLable_Font_Color = Default_ModelStyle_TargetLable_Font_Color;
     static public Color ModelStyle_TargetLable_Color = Default_ModelStyle_TargetLable_Color;

     static public Font ModelStyle_mainLablesFont = Default_ModelStyle_mainLablesFont;
     static public Color ModelStyle_mainLablesFont_Color = Default_ModelStyle_mainLablesFont_Color;
     static public Font ModelStyle_additionalLablesFont = Default_ModelStyle_additionalLablesFont;

     static public float ModelStyle_fadedMainLinesPen_Width = Default_ModelStyle_fadedMainLinesPen_Width;
     static public float ModelStyle_fadedMainDashedLinesPen_Width = Default_ModelStyle_fadedMainDashedLinesPen_Width;
     static public float ModelStyle_fadedMainDashDotLinesPen_Width = Default_ModelStyle_fadedMainDashDotLinesPen_Width;
     static public float ModelStyle_fadedAdditionalLinesPen_Width = Default_ModelStyle_fadedAdditionalLinesPen_Width;
     
     static public float ModelStyle_selectedMainLinesPen_Width = Default_ModelStyle_selectedMainLinesPen_Width;
     static public float ModelStyle_selectedMainDashedLinesPen_Width = Default_ModelStyle_selectedMainDashedLinesPen_Width;
     static public float ModelStyle_selectedMainDashDotLinesPen_Width = Default_ModelStyle_selectedMainDashDotLinesPen_Width;
     static public float ModelStyle_selectedAdditionalLinesPen_Width = Default_ModelStyle_selectedAdditionalLinesPen_Width;

     // ============================================================================================
     public Font _Write_Skilful_Font;
     public Color _Write_Skilful_Font_Color;     
     
     public ChartType _ChartType_;

     public Color _CandleUpBodyFill_Color;
     public Color _CandleDownBodyFill_Color;
     public Color _CandleDownBorder_Color;
     public Color _CandleUpBorder_Color;
     public float _CandleStyleBorderPen_Width;

     public Color _LineChartStyle_Color;
     public float _LineChartStylePen_Width;
     public float _LineChartStyle_Tension;
     public bool _LineChartStyle_Smooth;
     public bool _LineChartStyle_Antialias;
     public bool _LineChartStyle_UseShadow;
     public Color _LineChartStyleShadow_Color;

     public Color _Chart_backColor; 
     public Color _Grid_Color;
     public bool _GridSplitter_IsGridVisible;
     public bool _Grid_IsVisible;

     public Font _AxisStyleRight_Font;
     public Font _AxisStyleLeft_Font;
     public Font _AxisStyleTop_Font;
     public Font _AxisStyleBottom_Font;

     public Color _AxisStyleLeft_Font_Color;
     public Color _AxisStyleRight_Font_Color;
     public Color _AxisStyleTop_Font_Color;
     public Color _AxisStyleBottom_Font_Color;

     public Color _AxisStyleRight_BackColor;
     public Color _AxisStyleLeft_BackColor;
     public Color _AxisStyleTop_BackColor;
     public Color _AxisStyleBottom_BackColor;

     public Font _CursorLable_Right_Font;
     public Font _CursorLable_Left_Font;
     public Font _CursorLable_Bottom_Font;

     public Color _CursorLable_Right_Font_Color;
     public Color _CursorLable_Left_Font_Color;
     public Color _CursorLable_Bottom_Font_Color;

     public Font _ModelStyle_TargetLable_Font;
     public Color _ModelStyle_TargetLable_Font_Color;
     public Color _ModelStyle_TargetLable_Color;

     public Font _ModelStyle_mainLablesFont;
     public Color _ModelStyle_mainLablesFont_Color;
     public Font _ModelStyle_additionalLablesFont;

     public float _ModelStyle_fadedMainLinesPen_Width;
     public float _ModelStyle_fadedMainDashedLinesPen_Width;
     public float _ModelStyle_fadedMainDashDotLinesPen_Width;
     public float _ModelStyle_fadedAdditionalLinesPen_Width;

     public float _ModelStyle_selectedMainLinesPen_Width;
     public float _ModelStyle_selectedMainDashedLinesPen_Width;
     public float _ModelStyle_selectedMainDashDotLinesPen_Width;
     public float _ModelStyle_selectedAdditionalLinesPen_Width;
   

     // ------  Конструктор для первоначальной загрузки из файла
     public Sample(Sample _smp)
      {
          Write_Skilful_Font = _smp._Write_Skilful_Font;
          Write_Skilful_Font_Color = _smp._Write_Skilful_Font_Color;

          ChartType_ = _smp._ChartType_;          
          Chart_backColor = _smp._Chart_backColor;
          
          CandleUpBodyFill_Color = _smp._CandleUpBodyFill_Color;
          CandleDownBodyFill_Color = _smp._CandleDownBodyFill_Color;
          CandleDownBorder_Color = _smp._CandleDownBorder_Color;
          CandleUpBorder_Color = _smp._CandleUpBorder_Color;
          CandleStyleBorderPen_Width = _smp._CandleStyleBorderPen_Width;

          LineChartStyle_Color = _smp._LineChartStyle_Color;
          LineChartStylePen_Width = _smp._LineChartStylePen_Width;
          LineChartStyle_Tension = _smp._LineChartStyle_Tension;
          LineChartStyle_Smooth = _smp._LineChartStyle_Smooth;
          LineChartStyle_Antialias = _smp._LineChartStyle_Antialias;
          LineChartStyle_UseShadow = _smp._LineChartStyle_UseShadow;
          LineChartStyleShadow_Color = _smp._LineChartStyleShadow_Color;

          Grid_Color = _smp._Grid_Color;
          GridSplitter_IsGridVisible = _smp._GridSplitter_IsGridVisible;
          Grid_IsVisible = _smp._Grid_IsVisible;
          
          AxisStyleRight_Font = _smp._AxisStyleRight_Font;
          AxisStyleLeft_Font = _smp._AxisStyleLeft_Font;
          AxisStyleTop_Font = _smp._AxisStyleTop_Font;
          AxisStyleBottom_Font = _smp._AxisStyleBottom_Font;

          AxisStyleRight_Font_Color = _smp._AxisStyleRight_Font_Color; 
          AxisStyleLeft_Font_Color = _smp._AxisStyleLeft_Font_Color;
          AxisStyleTop_Font_Color = _smp._AxisStyleTop_Font_Color;
          AxisStyleBottom_Font_Color = _smp._AxisStyleBottom_Font_Color;

          AxisStyleRight_BackColor = _smp._AxisStyleRight_BackColor;
          AxisStyleLeft_BackColor = _smp._AxisStyleLeft_BackColor;
          AxisStyleTop_BackColor = _smp._AxisStyleTop_BackColor;
          AxisStyleBottom_BackColor = _smp._AxisStyleBottom_BackColor;
                  
          CursorLable_Right_Font = _smp._CursorLable_Right_Font;
          CursorLable_Left_Font = _smp._CursorLable_Left_Font;
          CursorLable_Bottom_Font = _smp._CursorLable_Bottom_Font;

          CursorLable_Right_Font_Color = _smp._CursorLable_Right_Font_Color;
          CursorLable_Left_Font_Color = _smp._CursorLable_Left_Font_Color;
          CursorLable_Bottom_Font_Color = _smp._CursorLable_Bottom_Font_Color;

          ModelStyle_TargetLable_Font = _smp._ModelStyle_TargetLable_Font;
          ModelStyle_TargetLable_Font_Color = _smp._ModelStyle_TargetLable_Font_Color;
          ModelStyle_TargetLable_Color = _smp._ModelStyle_TargetLable_Color;
          
          ModelStyle_mainLablesFont = _smp._ModelStyle_mainLablesFont;
          ModelStyle_mainLablesFont_Color = _smp._ModelStyle_mainLablesFont_Color;
          ModelStyle_additionalLablesFont = _smp._ModelStyle_additionalLablesFont; 
          
          ModelStyle_fadedMainLinesPen_Width = _smp._ModelStyle_fadedMainLinesPen_Width;
          ModelStyle_fadedMainDashedLinesPen_Width = _smp._ModelStyle_fadedMainDashedLinesPen_Width;
          ModelStyle_fadedMainDashDotLinesPen_Width = _smp._ModelStyle_fadedMainDashDotLinesPen_Width;
          ModelStyle_fadedAdditionalLinesPen_Width = _smp._ModelStyle_fadedAdditionalLinesPen_Width;
          
          ModelStyle_selectedMainLinesPen_Width = _smp._ModelStyle_selectedMainLinesPen_Width;
          ModelStyle_selectedMainDashedLinesPen_Width = _smp._ModelStyle_selectedMainDashedLinesPen_Width;
          ModelStyle_selectedMainDashDotLinesPen_Width = _smp._ModelStyle_selectedMainDashDotLinesPen_Width;
          ModelStyle_selectedAdditionalLinesPen_Width = _smp._ModelStyle_selectedAdditionalLinesPen_Width;

      }
     
    // ------- Конструктор для смены sample на текущем чарте и дальнейшего использования
     public Sample(Sample _smp, ChartV2.Control c) 
     {
         Write_Skilful_Font = _smp._Write_Skilful_Font;
         Write_Skilful_Font_Color = _smp._Write_Skilful_Font_Color;
         
         ChartType_ = c.seriesList[0].legend.chartType = _smp._ChartType_;
         
         Chart_backColor = c.plot.style.backColor = _smp._Chart_backColor;

         CandleUpBodyFill_Color = _smp._CandleUpBodyFill_Color;
         c.seriesList[0].legend.candleStyle.upFill = new SolidBrush(CandleUpBodyFill_Color);
         CandleDownBodyFill_Color = _smp._CandleDownBodyFill_Color;
         c.seriesList[0].legend.candleStyle.donwFill = new SolidBrush(CandleDownBodyFill_Color);         
         CandleStyleBorderPen_Width = c.seriesList[0].legend.candleStyle.upBorderPen.Width = _smp._CandleStyleBorderPen_Width;
         CandleUpBorder_Color = _smp._CandleUpBorder_Color;
         c.seriesList[0].legend.candleStyle.upBorderPen = new Pen(CandleUpBorder_Color, CandleStyleBorderPen_Width);
         CandleDownBorder_Color = _smp._CandleDownBorder_Color;
         c.seriesList[0].legend.candleStyle.downBorderPen = new Pen(CandleDownBorder_Color, CandleStyleBorderPen_Width);


         LineChartStylePen_Width = _smp._LineChartStylePen_Width;
         LineChartStyle_Color = _smp._LineChartStyle_Color;
         c.seriesList[0].legend.lineStyle.pen = new Pen(LineChartStyle_Color,LineChartStylePen_Width);
         LineChartStyle_Tension = c.seriesList[0].legend.lineStyle.tension = _smp._LineChartStyle_Tension;
         LineChartStyle_Smooth = c.seriesList[0].legend.lineStyle.smooth = _smp._LineChartStyle_Smooth;
         LineChartStyle_Antialias = c.seriesList[0].legend.antialias = _smp._LineChartStyle_Antialias;
         LineChartStyle_UseShadow = c.seriesList[0].legend.lineStyle.useShadow = _smp._LineChartStyle_UseShadow;
         LineChartStyleShadow_Color = _smp._LineChartStyleShadow_Color;
         c.seriesList[0].legend.lineStyle.ShadowBrush = new SolidBrush(LineChartStyleShadow_Color);

         Grid_Color = c.plot.grid.style.minorVerticalPen.Color = c.plot.grid.style.minorHorizontalPen.Color = _smp._Grid_Color;
         GridSplitter_IsGridVisible = c.gridSplitter.IsGridVisible = _smp._GridSplitter_IsGridVisible;
         Grid_IsVisible = c.plot.grid.IsVisible = _smp._Grid_IsVisible;
         
         AxisStyleRight_Font = c.rightAxis.style.magorTickFont = _smp._AxisStyleRight_Font;
         AxisStyleLeft_Font = c.leftAxis.style.magorTickFont = _smp._AxisStyleLeft_Font;
         AxisStyleTop_Font = c.topAxis.style.magorTickFont = _smp._AxisStyleTop_Font;
         AxisStyleBottom_Font = c.bottomAxis.style.magorTickFont = _smp._AxisStyleBottom_Font;

         AxisStyleRight_Font_Color = _smp._AxisStyleRight_Font_Color;
         c.rightAxis.style.magorFontBrush = new SolidBrush(AxisStyleRight_Font_Color);
         AxisStyleLeft_Font_Color = _smp._AxisStyleLeft_Font_Color;
         c.leftAxis.style.magorFontBrush = new SolidBrush(AxisStyleLeft_Font_Color);
         AxisStyleTop_Font_Color = _smp._AxisStyleTop_Font_Color;
         c.topAxis.style.magorFontBrush = new SolidBrush(AxisStyleTop_Font_Color);
         AxisStyleBottom_Font_Color = _smp._AxisStyleBottom_Font_Color;
         c.bottomAxis.style.magorFontBrush = new SolidBrush(AxisStyleBottom_Font_Color);

         AxisStyleRight_BackColor = c.rightAxis.style.backColor = _smp._AxisStyleRight_BackColor;
         c.rightAxis.style.backBrush = new SolidBrush(AxisStyleRight_BackColor);
         AxisStyleLeft_BackColor = c.leftAxis.style.backColor = _smp._AxisStyleLeft_BackColor;
         c.leftAxis.style.backBrush = new SolidBrush(AxisStyleLeft_BackColor);
         AxisStyleTop_BackColor = c.topAxis.style.backColor = _smp._AxisStyleTop_BackColor;
         c.topAxis.style.backBrush = new SolidBrush(AxisStyleTop_BackColor);
         AxisStyleBottom_BackColor = c.bottomAxis.style.backColor = _smp._AxisStyleBottom_BackColor;
         c.bottomAxis.style.backBrush = new SolidBrush(AxisStyleBottom_BackColor);

         CursorLable_Right_Font = c.rightAxis.cursorLable.font = _smp._CursorLable_Right_Font;
         CursorLable_Left_Font = c.leftAxis.cursorLable.font = _smp._CursorLable_Left_Font;
         CursorLable_Bottom_Font = c.bottomAxis.cursorLable.font = _smp._CursorLable_Bottom_Font;

         CursorLable_Right_Font_Color = _smp._CursorLable_Right_Font_Color;
         c.rightAxis.cursorLable.fontBrush = new SolidBrush(CursorLable_Right_Font_Color);
         CursorLable_Left_Font_Color = _smp._CursorLable_Left_Font_Color;
         c.leftAxis.cursorLable.fontBrush = new SolidBrush(CursorLable_Left_Font_Color);
         CursorLable_Bottom_Font_Color = _smp._CursorLable_Bottom_Font_Color;
         c.bottomAxis.cursorLable.fontBrush = new SolidBrush(CursorLable_Bottom_Font_Color);

         ModelStyle_TargetLable_Font = _smp._ModelStyle_TargetLable_Font;
         ModelStyle_TargetLable_Font_Color = _smp._ModelStyle_TargetLable_Font_Color;
         ModelStyle_TargetLable_Color = _smp._ModelStyle_TargetLable_Color;
         ModelStyle.lablesBrush = new SolidBrush(ModelStyle_TargetLable_Color);

         ModelStyle_mainLablesFont = ModelStyle.mainLablesFont = _smp._ModelStyle_mainLablesFont;
         ModelStyle_mainLablesFont_Color = _smp._ModelStyle_mainLablesFont_Color;
         ModelStyle.mainLablesFontBrush = new SolidBrush(ModelStyle_mainLablesFont_Color);
         ModelStyle_additionalLablesFont = ModelStyle.additionalLablesFont = _smp._ModelStyle_additionalLablesFont;

         if (ModelStyle.selectedMainLinesPen != null)
         {
           
             ModelStyle_fadedMainLinesPen_Width = ModelStyle.fadedMainLinesPen.Width = _smp._ModelStyle_fadedMainLinesPen_Width;
             ModelStyle_fadedMainDashedLinesPen_Width = ModelStyle.fadedMainDashedLinesPen.Width = _smp._ModelStyle_fadedMainDashedLinesPen_Width;
             ModelStyle_fadedMainDashDotLinesPen_Width = ModelStyle.fadedMainDashDotLinesPen.Width = _smp._ModelStyle_fadedMainDashDotLinesPen_Width;
             ModelStyle_fadedAdditionalLinesPen_Width = ModelStyle.fadedAdditionalLinesPen.Width = _smp._ModelStyle_fadedAdditionalLinesPen_Width;

             ModelStyle_selectedMainLinesPen_Width = ModelStyle.selectedMainLinesPen.Width = _smp._ModelStyle_selectedMainLinesPen_Width;
             ModelStyle_selectedMainDashedLinesPen_Width = ModelStyle.selectedMainDashedLinesPen.Width = _smp._ModelStyle_selectedMainDashedLinesPen_Width;
             ModelStyle_selectedMainDashDotLinesPen_Width = ModelStyle.selectedMainDashDotLinesPen.Width = _smp._ModelStyle_selectedMainDashDotLinesPen_Width;
             ModelStyle_selectedAdditionalLinesPen_Width = ModelStyle.selectedAdditionalLinesPen.Width = _smp._ModelStyle_selectedAdditionalLinesPen_Width;
         }

         
     }
     
     // ------ Конструктор для сохранения в файл и текущего использования
     public Sample(ChartV2.Control c)
     {
         _Write_Skilful_Font = Write_Skilful_Font;
         _Write_Skilful_Font_Color = Write_Skilful_Font_Color;
         
         ChartType_ = _ChartType_ = c.seriesList[0].legend.chartType;

         _CandleUpBodyFill_Color = CandleUpBodyFill_Color;
         _CandleDownBodyFill_Color = CandleDownBodyFill_Color;
         _CandleDownBorder_Color = CandleDownBorder_Color;
         _CandleUpBorder_Color = CandleUpBorder_Color;
         _CandleStyleBorderPen_Width = _CandleStyleBorderPen_Width = c.seriesList[0].legend.candleStyle.upBorderPen.Width;

         _LineChartStyle_Color = LineChartStyle_Color;
         LineChartStylePen_Width = _LineChartStylePen_Width = c.seriesList[0].legend.lineStyle.pen.Width;
         LineChartStyle_Tension = _LineChartStyle_Tension = c.seriesList[0].legend.lineStyle.tension;
         LineChartStyle_Smooth = _LineChartStyle_Smooth = c.seriesList[0].legend.lineStyle.smooth;
         LineChartStyle_Antialias = _LineChartStyle_Antialias = c.seriesList[0].legend.antialias;
         LineChartStyle_UseShadow = _LineChartStyle_UseShadow = c.seriesList[0].legend.lineStyle.useShadow;
         _LineChartStyleShadow_Color = LineChartStyleShadow_Color;

         Chart_backColor = _Chart_backColor = c.plot.style.backColor;
         Grid_Color = _Grid_Color = c.plot.grid.style.minorVerticalPen.Color;
         GridSplitter_IsGridVisible = _GridSplitter_IsGridVisible = c.gridSplitter.IsGridVisible;
         Grid_IsVisible = _Grid_IsVisible = c.plot.grid.IsVisible;
         
         AxisStyleRight_Font = _AxisStyleRight_Font = c.rightAxis.style.magorTickFont;
         AxisStyleLeft_Font = _AxisStyleLeft_Font = c.leftAxis.style.magorTickFont;
         AxisStyleTop_Font = _AxisStyleTop_Font = c.topAxis.style.magorTickFont;
         AxisStyleBottom_Font = _AxisStyleBottom_Font = c.bottomAxis.style.magorTickFont;

         _AxisStyleRight_Font_Color = AxisStyleRight_Font_Color;
         _AxisStyleLeft_Font_Color = AxisStyleLeft_Font_Color;
         _AxisStyleTop_Font_Color = AxisStyleTop_Font_Color;
         _AxisStyleBottom_Font_Color = AxisStyleBottom_Font_Color;

         AxisStyleRight_BackColor = _AxisStyleRight_BackColor = c.rightAxis.style.backColor;
         AxisStyleLeft_BackColor = _AxisStyleLeft_BackColor = c.leftAxis.style.backColor;
         AxisStyleTop_BackColor = _AxisStyleTop_BackColor = c.topAxis.style.backColor;
         AxisStyleBottom_BackColor = _AxisStyleBottom_BackColor = c.bottomAxis.style.backColor;
         
         CursorLable_Right_Font = _CursorLable_Right_Font = c.rightAxis.cursorLable.font;
         CursorLable_Left_Font = _CursorLable_Left_Font = c.leftAxis.cursorLable.font;
         CursorLable_Bottom_Font = _CursorLable_Bottom_Font = c.bottomAxis.cursorLable.font;

         _CursorLable_Right_Font_Color = CursorLable_Right_Font_Color;
         _CursorLable_Left_Font_Color = CursorLable_Left_Font_Color;
         _CursorLable_Bottom_Font_Color = CursorLable_Bottom_Font_Color;

         _ModelStyle_TargetLable_Color = ModelStyle_TargetLable_Color;
         _ModelStyle_TargetLable_Font = ModelStyle_TargetLable_Font;
         _ModelStyle_TargetLable_Font_Color = ModelStyle_TargetLable_Font_Color;

         ModelStyle_mainLablesFont = _ModelStyle_mainLablesFont = ModelStyle.mainLablesFont;
         _ModelStyle_mainLablesFont_Color = ModelStyle_mainLablesFont_Color;
         ModelStyle_additionalLablesFont = _ModelStyle_additionalLablesFont = ModelStyle.additionalLablesFont;
                 
        if (ModelStyle.selectedMainLinesPen != null)
         {

             ModelStyle_fadedMainLinesPen_Width = _ModelStyle_fadedMainLinesPen_Width = ModelStyle.fadedMainLinesPen.Width;
             ModelStyle_fadedMainDashedLinesPen_Width = _ModelStyle_fadedMainDashedLinesPen_Width = ModelStyle.fadedMainDashedLinesPen.Width;
             ModelStyle_fadedMainDashDotLinesPen_Width = _ModelStyle_fadedMainDashDotLinesPen_Width = ModelStyle.fadedMainDashDotLinesPen.Width;
             ModelStyle_fadedAdditionalLinesPen_Width = _ModelStyle_fadedAdditionalLinesPen_Width = ModelStyle.fadedAdditionalLinesPen.Width;

             ModelStyle_selectedMainLinesPen_Width = _ModelStyle_selectedMainLinesPen_Width = ModelStyle.selectedMainLinesPen.Width;
             ModelStyle_selectedMainDashedLinesPen_Width = _ModelStyle_selectedMainDashedLinesPen_Width = ModelStyle.selectedMainDashedLinesPen.Width;
             ModelStyle_selectedMainDashDotLinesPen_Width = _ModelStyle_selectedMainDashDotLinesPen_Width = ModelStyle.selectedMainDashDotLinesPen.Width;
             ModelStyle_selectedAdditionalLinesPen_Width = _ModelStyle_selectedAdditionalLinesPen_Width = ModelStyle.selectedAdditionalLinesPen.Width;
         }
         else
         {
             ModelStyle_fadedMainLinesPen_Width = _ModelStyle_fadedMainLinesPen_Width = Default_ModelStyle_fadedMainLinesPen_Width;
             ModelStyle_fadedMainDashedLinesPen_Width = _ModelStyle_fadedMainDashedLinesPen_Width = Default_ModelStyle_fadedMainDashedLinesPen_Width;
             ModelStyle_fadedMainDashDotLinesPen_Width = _ModelStyle_fadedMainDashDotLinesPen_Width = Default_ModelStyle_fadedMainDashDotLinesPen_Width;
             ModelStyle_fadedAdditionalLinesPen_Width = _ModelStyle_fadedAdditionalLinesPen_Width = Default_ModelStyle_fadedAdditionalLinesPen_Width;

             ModelStyle_selectedMainLinesPen_Width = _ModelStyle_selectedMainLinesPen_Width = Default_ModelStyle_selectedMainLinesPen_Width;
             ModelStyle_selectedMainDashedLinesPen_Width = _ModelStyle_selectedMainDashedLinesPen_Width = Default_ModelStyle_selectedMainDashedLinesPen_Width;
             ModelStyle_selectedMainDashDotLinesPen_Width = _ModelStyle_selectedMainDashDotLinesPen_Width = Default_ModelStyle_selectedMainDashDotLinesPen_Width;
             ModelStyle_selectedAdditionalLinesPen_Width = _ModelStyle_selectedAdditionalLinesPen_Width = Default_ModelStyle_selectedAdditionalLinesPen_Width;
         }
        

     }


// =========== Установка по дефолту ====================================
    static public void DefaultSample(ChartV2.Control c)
     {

      Write_Skilful_Font = Default_Write_Skilful_Font;
      Write_Skilful_Font_Color = Default_Write_Skilful_Font_Color;
        
      ChartType_ = Default_ChartType;

      CandleUpBodyFill_Color = Default_CandleUpBodyFill_Color;
      c.seriesList[0].legend.candleStyle.upFill = new SolidBrush(CandleUpBodyFill_Color);
      CandleDownBodyFill_Color = Default_CandleDownBodyFill_Color;
      c.seriesList[0].legend.candleStyle.donwFill = new SolidBrush(CandleDownBodyFill_Color);
      CandleStyleBorderPen_Width = c.seriesList[0].legend.candleStyle.upBorderPen.Width = Default_CandleStyleBorderPen_Width;
      CandleUpBorder_Color = Default_CandleUpBorder_Color;
      c.seriesList[0].legend.candleStyle.upBorderPen = new Pen(CandleUpBorder_Color, CandleStyleBorderPen_Width);
      CandleDownBorder_Color = Default_CandleDownBorder_Color;
      c.seriesList[0].legend.candleStyle.downBorderPen = new Pen(CandleDownBorder_Color, CandleStyleBorderPen_Width);


      LineChartStyle_Color = Default_LineChartStyle_Color;
      LineChartStylePen_Width = Default_LineChartStylePen_Width;
      c.seriesList[0].legend.lineStyle.pen = new Pen(LineChartStyle_Color,LineChartStylePen_Width);
      LineChartStyle_Tension = c.seriesList[0].legend.lineStyle.tension = Default_LineChartStyle_Tension;
      LineChartStyle_Smooth = c.seriesList[0].legend.lineStyle.smooth = Default_LineChartStyle_Smooth;
      LineChartStyle_Antialias = c.seriesList[0].legend.antialias = Default_LineChartStyle_Antialias;
      LineChartStyle_UseShadow = c.seriesList[0].legend.lineStyle.useShadow = Default_LineChartStyle_UseShadow;
      LineChartStyleShadow_Color = Default_LineChartStyleShadow_Color;
      c.seriesList[0].legend.lineStyle.ShadowBrush = new SolidBrush(LineChartStyleShadow_Color);

      Chart_backColor = c.plot.style.backColor = Default_Chart_backColor;
      Grid_Color = c.plot.grid.style.minorVerticalPen.Color = c.plot.grid.style.minorHorizontalPen.Color = Default_Grid_Color;
      GridSplitter_IsGridVisible = c.gridSplitter.IsGridVisible = Default_GridSplitter_IsGridVisible;
      Grid_IsVisible = c.plot.grid.IsVisible = Default_Grid_IsVisible;
      
      AxisStyleRight_Font = c.rightAxis.style.magorTickFont = Default_AxisStyleRight_Font;
      AxisStyleLeft_Font = c.leftAxis.style.magorTickFont = Default_AxisStyleLeft_Font;
      AxisStyleTop_Font = c.topAxis.style.magorTickFont = Default_AxisStyleTop_Font;
      AxisStyleBottom_Font = c.bottomAxis.style.magorTickFont = Default_AxisStyleBottom_Font;

      AxisStyleRight_Font_Color = Default_AxisStyleRight_Font_Color;
      c.rightAxis.style.magorFontBrush = new SolidBrush(Default_AxisStyleRight_Font_Color);
      AxisStyleLeft_Font_Color = Default_AxisStyleLeft_Font_Color;
      c.leftAxis.style.magorFontBrush = new SolidBrush(Default_AxisStyleLeft_Font_Color);
      AxisStyleTop_Font_Color = Default_AxisStyleTop_Font_Color;
      c.topAxis.style.magorFontBrush = new SolidBrush(AxisStyleTop_Font_Color);
      AxisStyleBottom_Font_Color = Default_AxisStyleBottom_Font_Color;
      c.bottomAxis.style.magorFontBrush = new SolidBrush(AxisStyleBottom_Font_Color);

      AxisStyleRight_BackColor = c.rightAxis.style.backColor = Default_AxisStyleRight_BackColor;
      c.rightAxis.style.backBrush = new SolidBrush(AxisStyleRight_BackColor);
      AxisStyleLeft_BackColor = c.leftAxis.style.backColor = Default_AxisStyleLeft_BackColor;
      c.leftAxis.style.backBrush = new SolidBrush(AxisStyleLeft_BackColor);
      AxisStyleTop_BackColor = c.topAxis.style.backColor = Default_AxisStyleTop_BackColor;
      c.topAxis.style.backBrush = new SolidBrush(AxisStyleTop_BackColor);
      AxisStyleBottom_BackColor = c.bottomAxis.style.backColor = Default_AxisStyleBottom_BackColor;
      c.bottomAxis.style.backBrush = new SolidBrush(AxisStyleBottom_BackColor);

      CursorLable_Left_Font = c.leftAxis.cursorLable.font = Default_CursorLable_Left_Font;
      CursorLable_Right_Font = c.rightAxis.cursorLable.font = Default_CursorLable_Right_Font;
      CursorLable_Bottom_Font = c.bottomAxis.cursorLable.font = Default_CursorLable_Bottom_Font;

      CursorLable_Right_Font_Color = Default_CursorLable_Right_Font_Color;
      c.rightAxis.cursorLable.fontBrush = new SolidBrush(CursorLable_Right_Font_Color);
      CursorLable_Left_Font_Color = Default_CursorLable_Left_Font_Color;
      c.leftAxis.cursorLable.fontBrush = new SolidBrush(CursorLable_Left_Font_Color);
      CursorLable_Bottom_Font_Color = Default_CursorLable_Bottom_Font_Color;
      c.bottomAxis.cursorLable.fontBrush = new SolidBrush(CursorLable_Bottom_Font_Color);

      ModelStyle_TargetLable_Font = Default_ModelStyle_TargetLable_Font;
      ModelStyle_TargetLable_Color = Default_ModelStyle_TargetLable_Color;
      ModelStyle.lablesBrush = new SolidBrush(Default_ModelStyle_TargetLable_Color);      
      ModelStyle_TargetLable_Font_Color = Default_ModelStyle_TargetLable_Font_Color;
          

      ModelStyle_mainLablesFont = ModelStyle.mainLablesFont = Default_ModelStyle_mainLablesFont;
      ModelStyle_mainLablesFont_Color = Default_ModelStyle_mainLablesFont_Color;
      ModelStyle.mainLablesFontBrush = new SolidBrush(ModelStyle_mainLablesFont_Color);
      ModelStyle_additionalLablesFont = ModelStyle.additionalLablesFont = Default_ModelStyle_additionalLablesFont;
  
      if (ModelStyle.selectedMainLinesPen != null)
      {
         
          ModelStyle_fadedMainLinesPen_Width = ModelStyle.fadedMainLinesPen.Width = Default_ModelStyle_fadedMainLinesPen_Width;
          ModelStyle_fadedMainDashedLinesPen_Width = ModelStyle.fadedMainDashedLinesPen.Width = Default_ModelStyle_fadedMainDashedLinesPen_Width;
          ModelStyle_fadedMainDashDotLinesPen_Width = ModelStyle.fadedMainDashDotLinesPen.Width = Default_ModelStyle_fadedMainDashDotLinesPen_Width;
          ModelStyle_fadedAdditionalLinesPen_Width = ModelStyle.fadedAdditionalLinesPen.Width = Default_ModelStyle_fadedAdditionalLinesPen_Width;
         
          ModelStyle_selectedMainLinesPen_Width = ModelStyle.selectedMainLinesPen.Width = Default_ModelStyle_selectedMainLinesPen_Width;
          ModelStyle_selectedMainDashedLinesPen_Width = ModelStyle.selectedMainDashedLinesPen.Width = Default_ModelStyle_selectedMainDashedLinesPen_Width;
          ModelStyle_selectedMainDashDotLinesPen_Width = ModelStyle.selectedMainDashDotLinesPen.Width = Default_ModelStyle_selectedMainDashDotLinesPen_Width;
          ModelStyle_selectedAdditionalLinesPen_Width = ModelStyle.selectedAdditionalLinesPen.Width = Default_ModelStyle_selectedAdditionalLinesPen_Width;
          
      }
    

      
    }

    }
}
