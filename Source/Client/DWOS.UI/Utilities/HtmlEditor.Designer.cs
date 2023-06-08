using System.Windows.Forms;

namespace DWOS.UI.Utilities
{
    partial class HtmlEditor : UserControl
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.htmlPanel = new TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel();
            this.txtTemplate = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTemplate)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.htmlPanel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.txtTemplate);
            this.splitContainer.Size = new System.Drawing.Size(1097, 629);
            this.splitContainer.SplitterDistance = 313;
            this.splitContainer.TabIndex = 0;
            // 
            // htmlPanel
            // 
            this.htmlPanel.AutoScroll = true;
            this.htmlPanel.AutoScrollMinSize = new System.Drawing.Size(1097, 17);
            this.htmlPanel.BackColor = System.Drawing.SystemColors.Window;
            this.htmlPanel.BaseStylesheet = null;
            this.htmlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlPanel.Location = new System.Drawing.Point(0, 0);
            this.htmlPanel.Name = "htmlPanel";
            this.htmlPanel.Size = new System.Drawing.Size(1097, 313);
            this.htmlPanel.TabIndex = 175;
            this.htmlPanel.Text = "No Data Loaded";
            // 
            // txtTemplate
            // 
            this.txtTemplate.AcceptsReturn = true;
            this.txtTemplate.AcceptsTab = true;
            this.txtTemplate.AlwaysInEditMode = true;
            this.txtTemplate.CausesValidation = false;
            this.txtTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTemplate.Location = new System.Drawing.Point(0, 0);
            this.txtTemplate.Multiline = true;
            this.txtTemplate.Name = "txtTemplate";
            this.txtTemplate.Scrollbars = System.Windows.Forms.ScrollBars.Both;
            this.txtTemplate.Size = new System.Drawing.Size(1097, 312);
            this.txtTemplate.TabIndex = 50;
            this.txtTemplate.UseAppStyling = false;
            this.txtTemplate.ValueChanged += new System.EventHandler(this.txtTemplate_ValueChanged);
            // 
            // timer
            // 
            this.timer.Interval = 2000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // HtmlEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "HtmlEditor";
            this.Size = new System.Drawing.Size(1097, 629);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HtmlEditor_KeyUp);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtTemplate)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel htmlPanel;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTemplate;
        private System.Windows.Forms.Timer timer;
    }
}
