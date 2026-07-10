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



using System.Drawing;
namespace ChartV2
{
    partial class Control
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.plotContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.drawingToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.freeLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.horizontalLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verticalLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.circleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cyclesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skipScalingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chartStyleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LineChartPick = new System.Windows.Forms.ToolStripMenuItem();
            this.CandleChartPick = new System.Windows.Forms.ToolStripMenuItem();
            this.plotAreaSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cursorSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arrowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.crossToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.axisesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataLablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.axisBordersToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.hideToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.modelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.hideToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.unselectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.ChartProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.UserToolContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.colorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.typeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.leftRayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightRayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bothRaysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noRaysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plotContextMenu.SuspendLayout();
            this.UserToolContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // plotContextMenu
            // 
            this.plotContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.drawingToolsToolStripMenuItem,
            this.skipScalingToolStripMenuItem,
            this.chartStyleToolStripMenuItem,
            this.plotAreaSetupToolStripMenuItem,
            this.modelsToolStripMenuItem,
            this.ChartProperties});
            this.plotContextMenu.Name = "rightMouseContextMenu";
            this.plotContextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.plotContextMenu.ShowImageMargin = false;
            this.plotContextMenu.Size = new System.Drawing.Size(135, 158);
            // 
            // drawingToolsToolStripMenuItem
            // 
            this.drawingToolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.freeLineToolStripMenuItem,
            this.horizontalLineToolStripMenuItem,
            this.verticalLineToolStripMenuItem,
            this.circleToolStripMenuItem,
            this.arcToolStripMenuItem,
            this.cyclesToolStripMenuItem});
            this.drawingToolsToolStripMenuItem.Name = "drawingToolsToolStripMenuItem";
            this.drawingToolsToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.drawingToolsToolStripMenuItem.Text = "Drawing Tools";
            // 
            // freeLineToolStripMenuItem
            // 
            this.freeLineToolStripMenuItem.Name = "freeLineToolStripMenuItem";
            this.freeLineToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.freeLineToolStripMenuItem.Text = "Free Line";
            this.freeLineToolStripMenuItem.Click += new System.EventHandler(this.freeLineToolStripMenuItem_Click);
            // 
            // horizontalLineToolStripMenuItem
            // 
            this.horizontalLineToolStripMenuItem.Name = "horizontalLineToolStripMenuItem";
            this.horizontalLineToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.horizontalLineToolStripMenuItem.Text = "Horizontal Line";
            this.horizontalLineToolStripMenuItem.Click += new System.EventHandler(this.horizontalLineToolStripMenuItem_Click);
            // 
            // verticalLineToolStripMenuItem
            // 
            this.verticalLineToolStripMenuItem.Name = "verticalLineToolStripMenuItem";
            this.verticalLineToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.verticalLineToolStripMenuItem.Text = "Vertical Line";
            this.verticalLineToolStripMenuItem.Click += new System.EventHandler(this.verticalLineToolStripMenuItem_Click);
            // 
            // circleToolStripMenuItem
            // 
            this.circleToolStripMenuItem.Name = "circleToolStripMenuItem";
            this.circleToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.circleToolStripMenuItem.Text = "Circle";
            this.circleToolStripMenuItem.Click += new System.EventHandler(this.circleToolStripMenuItem_Click);
            // 
            // arcToolStripMenuItem
            // 
            this.arcToolStripMenuItem.Name = "arcToolStripMenuItem";
            this.arcToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.arcToolStripMenuItem.Text = "Arc";
            this.arcToolStripMenuItem.Click += new System.EventHandler(this.arcToolStripMenuItem_Click);
            // 
            // cyclesToolStripMenuItem
            // 
            this.cyclesToolStripMenuItem.Name = "cyclesToolStripMenuItem";
            this.cyclesToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.cyclesToolStripMenuItem.Text = "Cycles";
            this.cyclesToolStripMenuItem.Click += new System.EventHandler(this.cyclesToolStripMenuItem_Click);
            // 
            // skipScalingToolStripMenuItem
            // 
            this.skipScalingToolStripMenuItem.Name = "skipScalingToolStripMenuItem";
            this.skipScalingToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.skipScalingToolStripMenuItem.Text = "Skip scaling";
            this.skipScalingToolStripMenuItem.Click += new System.EventHandler(this.skipScalingToolStripMenuItem_Click);
            // 
            // chartStyleToolStripMenuItem
            // 
            this.chartStyleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LineChartPick,
            this.CandleChartPick});
            this.chartStyleToolStripMenuItem.Name = "chartStyleToolStripMenuItem";
            this.chartStyleToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.chartStyleToolStripMenuItem.Text = "Chart style";
            // 
            // LineChartPick
            // 
            this.LineChartPick.Name = "LineChartPick";
            this.LineChartPick.Size = new System.Drawing.Size(152, 22);
            this.LineChartPick.Text = "Line";
            this.LineChartPick.Click += new System.EventHandler(this.ChartPick_CheckStateChanged);
            // 
            // CandleChartPick
            // 
            this.CandleChartPick.CheckOnClick = true;
            this.CandleChartPick.Name = "CandleChartPick";
            this.CandleChartPick.Size = new System.Drawing.Size(152, 22);
            this.CandleChartPick.Text = "Candle";
            this.CandleChartPick.Click += new System.EventHandler(this.ChartPick_CheckStateChanged);
            // 
            // plotAreaSetupToolStripMenuItem
            // 
            this.plotAreaSetupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backgroundColorToolStripMenuItem,
            this.gridSetupToolStripMenuItem,
            this.cursorSetupToolStripMenuItem,
            this.axisesToolStripMenuItem});
            this.plotAreaSetupToolStripMenuItem.Name = "plotAreaSetupToolStripMenuItem";
            this.plotAreaSetupToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.plotAreaSetupToolStripMenuItem.Text = "Plot area setup";
            // 
            // backgroundColorToolStripMenuItem
            // 
            this.backgroundColorToolStripMenuItem.Name = "backgroundColorToolStripMenuItem";
            this.backgroundColorToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.backgroundColorToolStripMenuItem.Text = "Background Color";
            this.backgroundColorToolStripMenuItem.Click += new System.EventHandler(this.backgroundColorToolStripMenuItem_Click);
            // 
            // gridSetupToolStripMenuItem
            // 
            this.gridSetupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colorToolStripMenuItem,
            this.showHideToolStripMenuItem});
            this.gridSetupToolStripMenuItem.Name = "gridSetupToolStripMenuItem";
            this.gridSetupToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.gridSetupToolStripMenuItem.Text = "Grid setup";
            // 
            // colorToolStripMenuItem
            // 
            this.colorToolStripMenuItem.Name = "colorToolStripMenuItem";
            this.colorToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.colorToolStripMenuItem.Text = "Color";
            this.colorToolStripMenuItem.Click += new System.EventHandler(this.colorToolStripMenuItem_Click);
            // 
            // showHideToolStripMenuItem
            // 
            this.showHideToolStripMenuItem.Name = "showHideToolStripMenuItem";
            this.showHideToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.showHideToolStripMenuItem.Text = "Show\\Hide";
            this.showHideToolStripMenuItem.Click += new System.EventHandler(this.showHideToolStripMenuItem_Click);
            // 
            // cursorSetupToolStripMenuItem
            // 
            this.cursorSetupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arrowToolStripMenuItem,
            this.crossToolStripMenuItem});
            this.cursorSetupToolStripMenuItem.Name = "cursorSetupToolStripMenuItem";
            this.cursorSetupToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.cursorSetupToolStripMenuItem.Text = "Cursor setup";
            // 
            // arrowToolStripMenuItem
            // 
            this.arrowToolStripMenuItem.Name = "arrowToolStripMenuItem";
            this.arrowToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.arrowToolStripMenuItem.Text = "Arrow";
            this.arrowToolStripMenuItem.Click += new System.EventHandler(this.arrowToolStripMenuItem_Click);
            // 
            // crossToolStripMenuItem
            // 
            this.crossToolStripMenuItem.Name = "crossToolStripMenuItem";
            this.crossToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.crossToolStripMenuItem.Text = "Cross";
            this.crossToolStripMenuItem.Click += new System.EventHandler(this.crossToolStripMenuItem_Click);
            // 
            // axisesToolStripMenuItem
            // 
            this.axisesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataLablesToolStripMenuItem,
            this.axisBordersToolStripMenuItem1});
            this.axisesToolStripMenuItem.Name = "axisesToolStripMenuItem";
            this.axisesToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.axisesToolStripMenuItem.Text = "Axises";
            // 
            // dataLablesToolStripMenuItem
            // 
            this.dataLablesToolStripMenuItem.Name = "dataLablesToolStripMenuItem";
            this.dataLablesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.dataLablesToolStripMenuItem.Text = "DataLables";
            this.dataLablesToolStripMenuItem.Visible = false;
            // 
            // axisBordersToolStripMenuItem1
            // 
            this.axisBordersToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem1,
            this.hideToolStripMenuItem1});
            this.axisBordersToolStripMenuItem1.Name = "axisBordersToolStripMenuItem1";
            this.axisBordersToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.axisBordersToolStripMenuItem1.Text = "Axis Borders";
            // 
            // showToolStripMenuItem1
            // 
            this.showToolStripMenuItem1.Name = "showToolStripMenuItem1";
            this.showToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.showToolStripMenuItem1.Text = "Show";
            this.showToolStripMenuItem1.Click += new System.EventHandler(this.showToolStripMenuItem1_Click_1);
            // 
            // hideToolStripMenuItem1
            // 
            this.hideToolStripMenuItem1.Name = "hideToolStripMenuItem1";
            this.hideToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.hideToolStripMenuItem1.Text = "Hide";
            this.hideToolStripMenuItem1.Click += new System.EventHandler(this.hideToolStripMenuItem1_Click_1);
            // 
            // modelsToolStripMenuItem
            // 
            this.modelsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allToolStripMenuItem,
            this.unselectedToolStripMenuItem});
            this.modelsToolStripMenuItem.Name = "modelsToolStripMenuItem";
            this.modelsToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.modelsToolStripMenuItem.Text = "Models";
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem2,
            this.hideToolStripMenuItem2});
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.allToolStripMenuItem.Text = "All";
            // 
            // showToolStripMenuItem2
            // 
            this.showToolStripMenuItem2.Name = "showToolStripMenuItem2";
            this.showToolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.showToolStripMenuItem2.Text = "Show";
            this.showToolStripMenuItem2.Click += new System.EventHandler(this.showToolStripMenuItem1_Click);
            // 
            // hideToolStripMenuItem2
            // 
            this.hideToolStripMenuItem2.Name = "hideToolStripMenuItem2";
            this.hideToolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.hideToolStripMenuItem2.Text = "Hide";
            this.hideToolStripMenuItem2.Click += new System.EventHandler(this.hideToolStripMenuItem1_Click);
            // 
            // unselectedToolStripMenuItem
            // 
            this.unselectedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideToolStripMenuItem3,
            this.showToolStripMenuItem3});
            this.unselectedToolStripMenuItem.Name = "unselectedToolStripMenuItem";
            this.unselectedToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.unselectedToolStripMenuItem.Text = "Unselected";
            // 
            // hideToolStripMenuItem3
            // 
            this.hideToolStripMenuItem3.Name = "hideToolStripMenuItem3";
            this.hideToolStripMenuItem3.Size = new System.Drawing.Size(103, 22);
            this.hideToolStripMenuItem3.Text = "Hide";
            this.hideToolStripMenuItem3.Click += new System.EventHandler(this.hideToolStripMenuItem3_Click);
            // 
            // showToolStripMenuItem3
            // 
            this.showToolStripMenuItem3.Name = "showToolStripMenuItem3";
            this.showToolStripMenuItem3.Size = new System.Drawing.Size(103, 22);
            this.showToolStripMenuItem3.Text = "Show";
            this.showToolStripMenuItem3.Click += new System.EventHandler(this.showToolStripMenuItem3_Click);
            // 
            // ChartProperties
            // 
            this.ChartProperties.Name = "ChartProperties";
            this.ChartProperties.Size = new System.Drawing.Size(134, 22);
            this.ChartProperties.Text = "Chart Properties";
            this.ChartProperties.Click += new System.EventHandler(this.ChartProperties_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Checked = true;
            this.toolStripMenuItem8.CheckOnClick = true;
            this.toolStripMenuItem8.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(120, 22);
            this.toolStripMenuItem8.Text = "dynamic";
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.CheckOnClick = true;
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(120, 22);
            this.toolStripMenuItem9.Text = "Static";
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem8,
            this.toolStripMenuItem9});
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(127, 22);
            this.toolStripMenuItem7.Text = "Axis type";
            // 
            // UserToolContextMenu
            // 
            this.UserToolContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colorToolStripMenuItem1,
            this.typeToolStripMenuItem1,
            this.deleteToolStripMenuItem});
            this.UserToolContextMenu.Name = "UserToolContextMenu";
            this.UserToolContextMenu.Size = new System.Drawing.Size(108, 70);
            // 
            // colorToolStripMenuItem1
            // 
            this.colorToolStripMenuItem1.Name = "colorToolStripMenuItem1";
            this.colorToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.colorToolStripMenuItem1.Text = "Color";
            this.colorToolStripMenuItem1.Click += new System.EventHandler(this.colorToolStripMenuItem1_Click);
            // 
            // typeToolStripMenuItem1
            // 
            this.typeToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.leftRayToolStripMenuItem,
            this.rightRayToolStripMenuItem,
            this.bothRaysToolStripMenuItem,
            this.noRaysToolStripMenuItem});
            this.typeToolStripMenuItem1.Name = "typeToolStripMenuItem1";
            this.typeToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.typeToolStripMenuItem1.Text = "Type";
            this.typeToolStripMenuItem1.Visible = false;
            // 
            // leftRayToolStripMenuItem
            // 
            this.leftRayToolStripMenuItem.Name = "leftRayToolStripMenuItem";
            this.leftRayToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.leftRayToolStripMenuItem.Text = "Left ray";
            this.leftRayToolStripMenuItem.Click += new System.EventHandler(this.leftRayToolStripMenuItem_Click);
            // 
            // rightRayToolStripMenuItem
            // 
            this.rightRayToolStripMenuItem.Name = "rightRayToolStripMenuItem";
            this.rightRayToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.rightRayToolStripMenuItem.Text = "Right ray";
            this.rightRayToolStripMenuItem.Click += new System.EventHandler(this.rightRayToolStripMenuItem_Click);
            // 
            // bothRaysToolStripMenuItem
            // 
            this.bothRaysToolStripMenuItem.Name = "bothRaysToolStripMenuItem";
            this.bothRaysToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.bothRaysToolStripMenuItem.Text = "Both rays";
            this.bothRaysToolStripMenuItem.Click += new System.EventHandler(this.bothRaysToolStripMenuItem_Click);
            // 
            // noRaysToolStripMenuItem
            // 
            this.noRaysToolStripMenuItem.Name = "noRaysToolStripMenuItem";
            this.noRaysToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.noRaysToolStripMenuItem.Text = "No rays";
            this.noRaysToolStripMenuItem.Click += new System.EventHandler(this.noRaysToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ForeColor = System.Drawing.SystemColors.Desktop;
            this.Name = "Control";
            this.Size = new System.Drawing.Size(685, 355);
            this.Resize += new System.EventHandler(this.Control_Resize);
            this.plotContextMenu.ResumeLayout(false);
            this.UserToolContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip plotContextMenu;
        private System.Windows.Forms.ToolStripMenuItem chartStyleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LineChartPick;
        private System.Windows.Forms.ToolStripMenuItem CandleChartPick;
        private System.Windows.Forms.ToolStripMenuItem plotAreaSetupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundColorToolStripMenuItem;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.ToolStripMenuItem gridSetupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem cursorSetupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arrowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem crossToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skipScalingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawingToolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem horizontalLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verticalLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem circleToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip UserToolContextMenu;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem typeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem freeLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leftRayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rightRayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bothRaysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noRaysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cyclesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem unselectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem showHideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem axisesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataLablesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem axisBordersToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem ChartProperties;

    }
}
