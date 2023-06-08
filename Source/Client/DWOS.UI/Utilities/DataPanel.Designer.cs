using System;
using DWOS.Shared;
namespace DWOS.UI
{
	partial class DataPanel
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
			try
			{
				NLog.LogManager.GetLogger(About.ApplicationName).Debug("Disposing of data panel: " + this.Name);
				OnDispose();

				if(disposing && (components != null))
				{
					foreach(var c in components.Components)
					{
						try
						{
							if(c is IDisposable)
								((IDisposable)c).Dispose();
						}
						catch(Exception exc)
						{
							string errorMsg = "Error disposing of component: " + c.ToString();
							NLog.LogManager.GetLogger(About.ApplicationName).Error(exc, errorMsg);
						}
					}
				}

				base.Dispose(disposing);
			}
			catch(Exception exc)
			{
				string errorMsg = "Error disposing of data panel: " + this.Name;
				NLog.LogManager.GetLogger(About.ApplicationName).Error(exc, errorMsg);
			}
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("", Infragistics.Win.ToolTipImage.Default, "Item Locked", Infragistics.Win.DefaultableBoolean.Default);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataPanel));
            this.grpData = new Infragistics.Win.Misc.UltraGroupBox();
            this.picLockImage = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.tipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.bsData = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).BeginInit();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).BeginInit();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.ContentPadding.Left = 5;
            this.grpData.ContentPadding.Right = 5;
            this.grpData.ContentPadding.Top = 5;
            this.grpData.Controls.Add(this.picLockImage);
            this.grpData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpData.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.TopOnBorder;
            this.grpData.Location = new System.Drawing.Point(3, 3);
            this.grpData.Name = "grpData";
            this.grpData.Size = new System.Drawing.Size(446, 559);
            this.grpData.TabIndex = 0;
            this.grpData.Text = "Data";
            // 
            // picLockImage
            // 
            this.picLockImage.Anchor = System.Windows.Forms.AnchorStyles.None;
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.picLockImage.Appearance = appearance1;
            this.picLockImage.AutoSize = true;
            this.picLockImage.BackColor = System.Drawing.Color.Transparent;
            this.picLockImage.BorderShadowColor = System.Drawing.Color.Empty;
            this.picLockImage.DefaultImage = global::DWOS.UI.Properties.Resources.Padlock_16;
            this.picLockImage.ImageTransparentColor = System.Drawing.Color.White;
            this.picLockImage.Location = new System.Drawing.Point(422, -2);
            this.picLockImage.Name = "picLockImage";
            this.picLockImage.Size = new System.Drawing.Size(16, 16);
            this.picLockImage.TabIndex = 0;
            ultraToolTipInfo1.ToolTipTextFormatted = resources.GetString("ultraToolTipInfo1.ToolTipTextFormatted");
            ultraToolTipInfo1.ToolTipTitle = "Item Locked";
            this.tipManager.SetUltraToolTip(this.picLockImage, ultraToolTipInfo1);
            this.picLockImage.UseAppStyling = false;
            // 
            // tipManager
            // 
            this.tipManager.ContainingControl = this;
            // 
            // DataPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.grpData);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DataPanel";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(452, 565);
            this.Load += new System.EventHandler(this.DataPanel_Load);
            this.Resize += new System.EventHandler(this.DataPanel_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.grpData)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsData)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		protected Infragistics.Win.Misc.UltraGroupBox grpData;
		protected System.Windows.Forms.BindingSource bsData;
		protected Infragistics.Win.UltraWinToolTip.UltraToolTipManager tipManager;
		protected Infragistics.Win.UltraWinEditors.UltraPictureBox picLockImage;

	}
}
