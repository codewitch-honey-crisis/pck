namespace Pck
{
	partial class Progress
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
			this.StatusBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// StatusBox
			// 
			this.StatusBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.StatusBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.StatusBox.Location = new System.Drawing.Point(0, 0);
			this.StatusBox.Multiline = true;
			this.StatusBox.Name = "StatusBox";
			this.StatusBox.ReadOnly = true;
			this.StatusBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.StatusBox.Size = new System.Drawing.Size(485, 154);
			this.StatusBox.TabIndex = 0;
			// 
			// ProgressForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(485, 154);
			this.ControlBox = false;
			this.Controls.Add(this.StatusBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProgressForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Progress";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox StatusBox;
	}
}