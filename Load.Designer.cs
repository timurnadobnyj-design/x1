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
//---------------------------------------------------------------------------------------------------------------

namespace Skilful
{
    partial class Load
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Load));
            this.progressBar_Load = new System.Windows.Forms.ProgressBar();
            this.label_Load = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar_Load
            // 
            this.progressBar_Load.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar_Load.Location = new System.Drawing.Point(0, 33);
            this.progressBar_Load.Minimum = 1;
            this.progressBar_Load.Name = "progressBar_Load";
            this.progressBar_Load.Size = new System.Drawing.Size(284, 14);
            this.progressBar_Load.Step = 1;
            this.progressBar_Load.TabIndex = 0;
            this.progressBar_Load.Value = 1;
            // 
            // label_Load
            // 
            this.label_Load.AutoSize = true;
            this.label_Load.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_Load.Location = new System.Drawing.Point(60, 12);
            this.label_Load.Name = "label_Load";
            this.label_Load.Size = new System.Drawing.Size(0, 17);
            this.label_Load.TabIndex = 1;
            this.label_Load.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Load
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 47);
            this.ControlBox = false;
            this.Controls.Add(this.label_Load);
            this.Controls.Add(this.progressBar_Load);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Load";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar_Load;
        private System.Windows.Forms.Label label_Load;
    }
}