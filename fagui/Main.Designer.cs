namespace Pck
{

	partial class Main
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
			this.components = new System.ComponentModel.Container();
			this.panel1 = new System.Windows.Forms.Panel();
			this.Status = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.Input = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.Regex = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.RenderTimer = new System.Windows.Forms.Timer(this.components);
			this.NfaGraph = new System.Windows.Forms.PictureBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.DfaGraph = new System.Windows.Forms.PictureBox();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NfaGraph)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.DfaGraph)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.Status);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.Input);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.Regex);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(654, 116);
			this.panel1.TabIndex = 0;
			// 
			// Status
			// 
			this.Status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Status.AutoEllipsis = true;
			this.Status.Location = new System.Drawing.Point(66, 68);
			this.Status.Name = "Status";
			this.Status.Size = new System.Drawing.Size(575, 41);
			this.Status.TabIndex = 5;
			this.Status.Text = "Waiting for Regex";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 66);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 17);
			this.label3.TabIndex = 4;
			this.label3.Text = "Status";
			// 
			// Input
			// 
			this.Input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Input.Location = new System.Drawing.Point(67, 39);
			this.Input.Name = "Input";
			this.Input.Size = new System.Drawing.Size(580, 22);
			this.Input.TabIndex = 3;
			this.Input.TextChanged += new System.EventHandler(this.Input_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(21, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "Input";
			// 
			// Regex
			// 
			this.Regex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Regex.Location = new System.Drawing.Point(67, 9);
			this.Regex.Name = "Regex";
			this.Regex.Size = new System.Drawing.Size(580, 22);
			this.Regex.TabIndex = 1;
			this.Regex.TextChanged += new System.EventHandler(this.Regex_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Regex";
			// 
			// panel2
			// 
			this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.panel2.Controls.Add(this.DfaGraph);
			this.panel2.Controls.Add(this.splitter1);
			this.panel2.Controls.Add(this.NfaGraph);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 116);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(654, 311);
			this.panel2.TabIndex = 1;
			// 
			// RenderTimer
			// 
			this.RenderTimer.Tick += new System.EventHandler(this.RenderTimer_Tick);
			// 
			// NfaGraph
			// 
			this.NfaGraph.BackColor = System.Drawing.Color.White;
			this.NfaGraph.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.NfaGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.NfaGraph.Dock = System.Windows.Forms.DockStyle.Top;
			this.NfaGraph.Location = new System.Drawing.Point(0, 0);
			this.NfaGraph.Name = "NfaGraph";
			this.NfaGraph.Size = new System.Drawing.Size(654, 154);
			this.NfaGraph.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.NfaGraph.TabIndex = 0;
			this.NfaGraph.TabStop = false;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 154);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(654, 3);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// DfaGraph
			// 
			this.DfaGraph.BackColor = System.Drawing.Color.White;
			this.DfaGraph.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.DfaGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.DfaGraph.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DfaGraph.Location = new System.Drawing.Point(0, 157);
			this.DfaGraph.Name = "DfaGraph";
			this.DfaGraph.Size = new System.Drawing.Size(654, 154);
			this.DfaGraph.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.DfaGraph.TabIndex = 2;
			this.DfaGraph.TabStop = false;
			// 
			// Main
			// 
			this.ClientSize = new System.Drawing.Size(654, 427);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "Main";
			this.Text = "FA Visualizer";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.NfaGraph)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.DfaGraph)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Timer RenderTimer;
		private System.Windows.Forms.TextBox Regex;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label Status;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox Input;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PictureBox DfaGraph;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.PictureBox NfaGraph;
	}
}

