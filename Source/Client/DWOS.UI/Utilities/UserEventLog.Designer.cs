namespace DWOS.UI
{
	partial class UserEventLog
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.btnOK = new Infragistics.Win.Misc.UltraButton();
			this.txtEvent = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
			this.ultraLabel14 = new Infragistics.Win.Misc.UltraLabel();
			this.ultraLabel13 = new Infragistics.Win.Misc.UltraLabel();
			this.txtReason = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
			this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
			this.inboxControlStyler1 = new Infragistics.Win.AppStyling.Runtime.InboxControlStyler(this.components);
			this.txtUserName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
			((System.ComponentModel.ISupportInitialize)(this.txtEvent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtReason)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtUserName)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(246, 217);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(71, 23);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// txtEvent
			// 
			this.txtEvent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtEvent.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtEvent.Location = new System.Drawing.Point(110, 12);
			this.txtEvent.MaxLength = 50;
			this.txtEvent.Name = "txtEvent";
			this.txtEvent.ReadOnly = true;
			this.txtEvent.Size = new System.Drawing.Size(207, 22);
			this.txtEvent.TabIndex = 0;
			// 
			// ultraLabel14
			// 
			this.ultraLabel14.AutoSize = true;
			this.ultraLabel14.Location = new System.Drawing.Point(16, 16);
			this.ultraLabel14.Name = "ultraLabel14";
			this.ultraLabel14.Size = new System.Drawing.Size(42, 15);
			this.ultraLabel14.TabIndex = 72;
			this.ultraLabel14.Text = "Event:";
			// 
			// ultraLabel13
			// 
			this.ultraLabel13.AutoSize = true;
			this.ultraLabel13.Location = new System.Drawing.Point(16, 44);
			this.ultraLabel13.Name = "ultraLabel13";
			this.ultraLabel13.Size = new System.Drawing.Size(72, 15);
			this.ultraLabel13.TabIndex = 71;
			this.ultraLabel13.Text = "Entered By:";
			// 
			// txtReason
			// 
			this.txtReason.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtReason.Location = new System.Drawing.Point(110, 68);
			this.txtReason.MaxLength = 50;
			this.txtReason.Multiline = true;
			this.txtReason.Name = "txtReason";
			this.txtReason.Size = new System.Drawing.Size(207, 143);
			this.txtReason.TabIndex = 2;
			// 
			// ultraLabel1
			// 
			this.ultraLabel1.AutoSize = true;
			this.ultraLabel1.Location = new System.Drawing.Point(16, 72);
			this.ultraLabel1.Name = "ultraLabel1";
			this.ultraLabel1.Size = new System.Drawing.Size(51, 15);
			this.ultraLabel1.TabIndex = 74;
			this.ultraLabel1.Text = "Reason:";
			// 
			// txtUserName
			// 
			this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtUserName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtUserName.Location = new System.Drawing.Point(110, 40);
			this.txtUserName.MaxLength = 50;
			this.txtUserName.Name = "txtUserName";
			this.txtUserName.ReadOnly = true;
			this.txtUserName.Size = new System.Drawing.Size(207, 22);
			this.txtUserName.TabIndex = 1;
			// 
			// UserEventLog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(329, 252);
			this.Controls.Add(this.txtUserName);
			this.Controls.Add(this.txtReason);
			this.Controls.Add(this.ultraLabel1);
			this.Controls.Add(this.txtEvent);
			this.Controls.Add(this.ultraLabel14);
			this.Controls.Add(this.ultraLabel13);
			this.Controls.Add(this.btnOK);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "UserEventLog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.inboxControlStyler1.SetStyleSettings(this, new Infragistics.Win.AppStyling.Runtime.InboxControlStyleSettings(Infragistics.Win.DefaultableBoolean.Default));
			this.Text = "Event Log";
			this.Load += new System.EventHandler(this.UserEventLog_Load);
			((System.ComponentModel.ISupportInitialize)(this.txtEvent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtReason)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.inboxControlStyler1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtUserName)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.Misc.UltraButton btnOK;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtEvent;
		private Infragistics.Win.Misc.UltraLabel ultraLabel14;
		private Infragistics.Win.Misc.UltraLabel ultraLabel13;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtReason;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.AppStyling.Runtime.InboxControlStyler inboxControlStyler1;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtUserName;
	}
}