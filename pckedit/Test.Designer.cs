namespace Pck
{
	partial class Test
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Test));
			this.parseTimer = new System.Windows.Forms.Timer(this.components);
			this.treeImageList = new System.Windows.Forms.ImageList(this.components);
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.parseTree = new System.Windows.Forms.TreeView();
			this.showHidden = new System.Windows.Forms.CheckBox();
			this.transformTree = new System.Windows.Forms.CheckBox();
			this.trimTree = new System.Windows.Forms.CheckBox();
			this.editor = new ICSharpCode.TextEditor.TextEditorControl();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// parseTimer
			// 
			this.parseTimer.Tick += new System.EventHandler(this.parseTimer_Tick);
			// 
			// treeImageList
			// 
			this.treeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeImageList.ImageStream")));
			this.treeImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.treeImageList.Images.SetKeyName(0, "ASCube_16xLG.png");
			this.treeImageList.Images.SetKeyName(1, "action_create_16xLG.png");
			this.treeImageList.Images.SetKeyName(2, "breakpoint_Off_16xLG.png");
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.editor);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.parseTree);
			this.splitContainer1.Panel2.Controls.Add(this.showHidden);
			this.splitContainer1.Panel2.Controls.Add(this.transformTree);
			this.splitContainer1.Panel2.Controls.Add(this.trimTree);
			this.splitContainer1.Size = new System.Drawing.Size(682, 453);
			this.splitContainer1.SplitterDistance = 450;
			this.splitContainer1.TabIndex = 4;
			// 
			// parseTree
			// 
			this.parseTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.parseTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.parseTree.ImageIndex = 0;
			this.parseTree.ImageList = this.treeImageList;
			this.parseTree.Location = new System.Drawing.Point(0, 63);
			this.parseTree.Name = "parseTree";
			this.parseTree.PathSeparator = "/";
			this.parseTree.SelectedImageIndex = 0;
			this.parseTree.Size = new System.Drawing.Size(228, 390);
			this.parseTree.TabIndex = 6;
			// 
			// showHidden
			// 
			this.showHidden.AutoSize = true;
			this.showHidden.BackColor = System.Drawing.SystemColors.Window;
			this.showHidden.Dock = System.Windows.Forms.DockStyle.Top;
			this.showHidden.ForeColor = System.Drawing.SystemColors.WindowText;
			this.showHidden.Location = new System.Drawing.Point(0, 42);
			this.showHidden.Name = "showHidden";
			this.showHidden.Size = new System.Drawing.Size(228, 21);
			this.showHidden.TabIndex = 9;
			this.showHidden.Text = "Show Hidden";
			this.showHidden.UseVisualStyleBackColor = false;
			this.showHidden.CheckedChanged += new System.EventHandler(this.showHidden_CheckedChanged);
			// 
			// transformTree
			// 
			this.transformTree.AutoSize = true;
			this.transformTree.BackColor = System.Drawing.SystemColors.Window;
			this.transformTree.Checked = true;
			this.transformTree.CheckState = System.Windows.Forms.CheckState.Checked;
			this.transformTree.Dock = System.Windows.Forms.DockStyle.Top;
			this.transformTree.ForeColor = System.Drawing.SystemColors.WindowText;
			this.transformTree.Location = new System.Drawing.Point(0, 21);
			this.transformTree.Name = "transformTree";
			this.transformTree.Size = new System.Drawing.Size(228, 21);
			this.transformTree.TabIndex = 8;
			this.transformTree.Text = "Transform Tree";
			this.transformTree.UseVisualStyleBackColor = false;
			this.transformTree.CheckedChanged += new System.EventHandler(this.transformTree_CheckedChanged);
			// 
			// trimTree
			// 
			this.trimTree.AutoSize = true;
			this.trimTree.BackColor = System.Drawing.SystemColors.Window;
			this.trimTree.Dock = System.Windows.Forms.DockStyle.Top;
			this.trimTree.ForeColor = System.Drawing.SystemColors.WindowText;
			this.trimTree.Location = new System.Drawing.Point(0, 0);
			this.trimTree.Name = "trimTree";
			this.trimTree.Size = new System.Drawing.Size(228, 21);
			this.trimTree.TabIndex = 7;
			this.trimTree.Text = "Trim Tree";
			this.trimTree.UseVisualStyleBackColor = false;
			this.trimTree.CheckedChanged += new System.EventHandler(this.trimTree_CheckedChanged);
			// 
			// editor
			// 
			this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editor.IsReadOnly = false;
			this.editor.Location = new System.Drawing.Point(0, 0);
			this.editor.Name = "editor";
			this.editor.Size = new System.Drawing.Size(450, 453);
			this.editor.TabIndex = 2;
			this.editor.TextChanged += new System.EventHandler(this.editor_TextChanged);
			// 
			// Test
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(682, 453);
			this.Controls.Add(this.splitContainer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Test";
			this.Text = "Test";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.ImageList treeImageList;
		private System.Windows.Forms.Timer parseTimer;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private ICSharpCode.TextEditor.TextEditorControl editor;
		private System.Windows.Forms.TreeView parseTree;
		private System.Windows.Forms.CheckBox showHidden;
		private System.Windows.Forms.CheckBox transformTree;
		private System.Windows.Forms.CheckBox trimTree;
	}
}