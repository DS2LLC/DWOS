namespace DWOS.UI.Utilities
{
	partial class DigitalGuage
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
			if(disposing && (components != null))
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
			Infragistics.UltraGauge.Resources.SegmentedDigitalGauge segmentedDigitalGauge1 = new Infragistics.UltraGauge.Resources.SegmentedDigitalGauge();
			Infragistics.UltraGauge.Resources.SolidFillBrushElement solidFillBrushElement1 = new Infragistics.UltraGauge.Resources.SolidFillBrushElement();
			Infragistics.UltraGauge.Resources.SolidFillBrushElement solidFillBrushElement2 = new Infragistics.UltraGauge.Resources.SolidFillBrushElement();
			this.guageOrderCount = new Infragistics.Win.UltraWinGauge.UltraGauge();
			this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
			((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
			this.ultraGroupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// guageOrderCount
			// 
			this.guageOrderCount.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.guageOrderCount.BackColor = System.Drawing.Color.Transparent;
			solidFillBrushElement1.Color = System.Drawing.Color.Black;
			segmentedDigitalGauge1.BrushElements.Add(solidFillBrushElement1);
			segmentedDigitalGauge1.Digits = 6;
			segmentedDigitalGauge1.DigitSpacing = 3D;
			solidFillBrushElement2.Color = System.Drawing.Color.Red;
			segmentedDigitalGauge1.FontBrushElements.Add(solidFillBrushElement2);
			segmentedDigitalGauge1.Mode = Infragistics.UltraGauge.Resources.SegmentMode.FourteenSegment;
			segmentedDigitalGauge1.Text = "999999";
			this.guageOrderCount.Gauges.Add(segmentedDigitalGauge1);
			this.guageOrderCount.Location = new System.Drawing.Point(5, 23);
			this.guageOrderCount.Name = "guageOrderCount";
			this.guageOrderCount.Size = new System.Drawing.Size(170, 74);
			this.guageOrderCount.TabIndex = 8;
			// 
			// ultraGroupBox1
			// 
			this.ultraGroupBox1.CaptionAlignment = Infragistics.Win.Misc.GroupBoxCaptionAlignment.Center;
			this.ultraGroupBox1.Controls.Add(this.guageOrderCount);
			this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ultraGroupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ultraGroupBox1.HeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.Dotted;
			this.ultraGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOutsideBorder;
			this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
			this.ultraGroupBox1.Name = "ultraGroupBox1";
			this.ultraGroupBox1.Size = new System.Drawing.Size(181, 102);
			this.ultraGroupBox1.TabIndex = 9;
			this.ultraGroupBox1.Text = "Label";
			// 
			// DigitalGuage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ultraGroupBox1);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "DigitalGuage";
			this.Size = new System.Drawing.Size(181, 102);
			((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
			this.ultraGroupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinGauge.UltraGauge guageOrderCount;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
	}
}
