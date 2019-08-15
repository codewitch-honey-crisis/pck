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
			if (disposing && (components != null)) {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuFileNew = new System.Windows.Forms.ToolStripMenuItem();
			this.xbnfFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pckFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
			this.menuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.menuFileClose = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuEditCut = new System.Windows.Forms.ToolStripMenuItem();
			this.menuEditCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.menuEditPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.menuEditDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.menuEditFind = new System.Windows.Forms.ToolStripMenuItem();
			this.menuEditReplace = new System.Windows.Forms.ToolStripMenuItem();
			this.menuFindAgain = new System.Windows.Forms.ToolStripMenuItem();
			this.menuFindAgainReverse = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.menuToggleBookmark = new System.Windows.Forms.ToolStripMenuItem();
			this.menuGoToNextBookmark = new System.Windows.Forms.ToolStripMenuItem();
			this.menuGoToPrevBookmark = new System.Windows.Forms.ToolStripMenuItem();
			this.buildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createLL1ParserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createLalr1ParserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createFATokenizerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fATokenizerLL1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.createPCKSpecToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createFactoredPCKSpecToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSplitTextArea = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.menuShowSpacesTabs = new System.Windows.Forms.ToolStripMenuItem();
			this.menuShowNewlines = new System.Windows.Forms.ToolStripMenuItem();
			this.menuShowLineNumbers = new System.Windows.Forms.ToolStripMenuItem();
			this.menuHighlightCurrentRow = new System.Windows.Forms.ToolStripMenuItem();
			this.menuBracketMatchingStyle = new System.Windows.Forms.ToolStripMenuItem();
			this.menuEnableVirtualSpace = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.menuSetTabSize = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSetFont = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.codeLanguageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.vBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.fileTabs = new System.Windows.Forms.TabControl();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.fontDialog = new System.Windows.Forms.FontDialog();
			this.messages = new System.Windows.Forms.ListView();
			this.errorLevelColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.codeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.descriptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.fileColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lineColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.buildToolStripMenuItem,
            this.optionsToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
			this.menuStrip1.Size = new System.Drawing.Size(485, 28);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFileNew,
            this.menuFileOpen,
            this.menuFileSave,
            this.menuFileSaveAs,
            this.menuFileClose,
            this.toolStripSeparator1,
            this.menuFileExit});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// menuFileNew
			// 
			this.menuFileNew.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xbnfFileToolStripMenuItem,
            this.pckFileToolStripMenuItem});
			this.menuFileNew.Name = "menuFileNew";
			this.menuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.menuFileNew.Size = new System.Drawing.Size(216, 26);
			this.menuFileNew.Text = "&New";
			this.menuFileNew.Click += new System.EventHandler(this.menuFileNew_Click);
			// 
			// xbnfFileToolStripMenuItem
			// 
			this.xbnfFileToolStripMenuItem.Name = "xbnfFileToolStripMenuItem";
			this.xbnfFileToolStripMenuItem.Size = new System.Drawing.Size(142, 26);
			this.xbnfFileToolStripMenuItem.Text = "&Xbnf File";
			this.xbnfFileToolStripMenuItem.Click += new System.EventHandler(this.xbnfFileToolStripMenuItem_Click);
			// 
			// pckFileToolStripMenuItem
			// 
			this.pckFileToolStripMenuItem.Name = "pckFileToolStripMenuItem";
			this.pckFileToolStripMenuItem.Size = new System.Drawing.Size(142, 26);
			this.pckFileToolStripMenuItem.Text = "&Pck File";
			this.pckFileToolStripMenuItem.Click += new System.EventHandler(this.pckFileToolStripMenuItem_Click);
			// 
			// menuFileOpen
			// 
			this.menuFileOpen.Name = "menuFileOpen";
			this.menuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.menuFileOpen.Size = new System.Drawing.Size(216, 26);
			this.menuFileOpen.Text = "&Open...";
			this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
			// 
			// menuFileSave
			// 
			this.menuFileSave.Name = "menuFileSave";
			this.menuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.menuFileSave.Size = new System.Drawing.Size(216, 26);
			this.menuFileSave.Text = "&Save";
			this.menuFileSave.Click += new System.EventHandler(this.menuFileSave_Click);
			// 
			// menuFileSaveAs
			// 
			this.menuFileSaveAs.Name = "menuFileSaveAs";
			this.menuFileSaveAs.Size = new System.Drawing.Size(216, 26);
			this.menuFileSaveAs.Text = "Save as...";
			this.menuFileSaveAs.Click += new System.EventHandler(this.menuFileSaveAs_Click);
			// 
			// menuFileClose
			// 
			this.menuFileClose.Name = "menuFileClose";
			this.menuFileClose.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
			this.menuFileClose.Size = new System.Drawing.Size(216, 26);
			this.menuFileClose.Text = "&Close";
			this.menuFileClose.Click += new System.EventHandler(this.menuFileClose_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(213, 6);
			// 
			// menuFileExit
			// 
			this.menuFileExit.Name = "menuFileExit";
			this.menuFileExit.Size = new System.Drawing.Size(216, 26);
			this.menuFileExit.Text = "E&xit";
			this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEditCut,
            this.menuEditCopy,
            this.menuEditPaste,
            this.menuEditDelete,
            this.toolStripSeparator2,
            this.menuEditFind,
            this.menuEditReplace,
            this.menuFindAgain,
            this.menuFindAgainReverse,
            this.toolStripSeparator5,
            this.menuToggleBookmark,
            this.menuGoToNextBookmark,
            this.menuGoToPrevBookmark});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// menuEditCut
			// 
			this.menuEditCut.Name = "menuEditCut";
			this.menuEditCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.menuEditCut.Size = new System.Drawing.Size(316, 26);
			this.menuEditCut.Text = "Cu&t";
			this.menuEditCut.Click += new System.EventHandler(this.menuEditCut_Click);
			// 
			// menuEditCopy
			// 
			this.menuEditCopy.Name = "menuEditCopy";
			this.menuEditCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.menuEditCopy.Size = new System.Drawing.Size(316, 26);
			this.menuEditCopy.Text = "&Copy";
			this.menuEditCopy.Click += new System.EventHandler(this.menuEditCopy_Click);
			// 
			// menuEditPaste
			// 
			this.menuEditPaste.Name = "menuEditPaste";
			this.menuEditPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.menuEditPaste.Size = new System.Drawing.Size(316, 26);
			this.menuEditPaste.Text = "&Paste";
			this.menuEditPaste.Click += new System.EventHandler(this.menuEditPaste_Click);
			// 
			// menuEditDelete
			// 
			this.menuEditDelete.Name = "menuEditDelete";
			this.menuEditDelete.Size = new System.Drawing.Size(316, 26);
			this.menuEditDelete.Text = "&Delete";
			this.menuEditDelete.Click += new System.EventHandler(this.menuEditDelete_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(313, 6);
			// 
			// menuEditFind
			// 
			this.menuEditFind.Name = "menuEditFind";
			this.menuEditFind.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.menuEditFind.Size = new System.Drawing.Size(316, 26);
			this.menuEditFind.Text = "&Find...";
			this.menuEditFind.Click += new System.EventHandler(this.menuEditFind_Click);
			// 
			// menuEditReplace
			// 
			this.menuEditReplace.Name = "menuEditReplace";
			this.menuEditReplace.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
			this.menuEditReplace.Size = new System.Drawing.Size(316, 26);
			this.menuEditReplace.Text = "Find and &replace...";
			this.menuEditReplace.Click += new System.EventHandler(this.menuEditReplace_Click);
			// 
			// menuFindAgain
			// 
			this.menuFindAgain.Name = "menuFindAgain";
			this.menuFindAgain.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.menuFindAgain.Size = new System.Drawing.Size(316, 26);
			this.menuFindAgain.Text = "Find &again";
			this.menuFindAgain.Click += new System.EventHandler(this.menuFindAgain_Click);
			// 
			// menuFindAgainReverse
			// 
			this.menuFindAgainReverse.Name = "menuFindAgainReverse";
			this.menuFindAgainReverse.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
			this.menuFindAgainReverse.Size = new System.Drawing.Size(316, 26);
			this.menuFindAgainReverse.Text = "Find again (&reverse)";
			this.menuFindAgainReverse.Click += new System.EventHandler(this.menuFindAgainReverse_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(313, 6);
			// 
			// menuToggleBookmark
			// 
			this.menuToggleBookmark.Name = "menuToggleBookmark";
			this.menuToggleBookmark.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
			this.menuToggleBookmark.Size = new System.Drawing.Size(316, 26);
			this.menuToggleBookmark.Text = "Toggle bookmark";
			this.menuToggleBookmark.Click += new System.EventHandler(this.menuToggleBookmark_Click);
			// 
			// menuGoToNextBookmark
			// 
			this.menuGoToNextBookmark.Name = "menuGoToNextBookmark";
			this.menuGoToNextBookmark.ShortcutKeys = System.Windows.Forms.Keys.F2;
			this.menuGoToNextBookmark.Size = new System.Drawing.Size(316, 26);
			this.menuGoToNextBookmark.Text = "Go to next bookmark";
			this.menuGoToNextBookmark.Click += new System.EventHandler(this.menuGoToNextBookmark_Click);
			// 
			// menuGoToPrevBookmark
			// 
			this.menuGoToPrevBookmark.Name = "menuGoToPrevBookmark";
			this.menuGoToPrevBookmark.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
			this.menuGoToPrevBookmark.Size = new System.Drawing.Size(316, 26);
			this.menuGoToPrevBookmark.Text = "Go to previous bookmark";
			this.menuGoToPrevBookmark.Click += new System.EventHandler(this.menuGoToPrevBookmark_Click);
			// 
			// buildToolStripMenuItem
			// 
			this.buildToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createLL1ParserToolStripMenuItem,
            this.createLalr1ParserToolStripMenuItem,
            this.createFATokenizerToolStripMenuItem,
            this.fATokenizerLL1ToolStripMenuItem,
            this.toolStripMenuItem2,
            this.createPCKSpecToolStripMenuItem,
            this.createFactoredPCKSpecToolStripMenuItem});
			this.buildToolStripMenuItem.Name = "buildToolStripMenuItem";
			this.buildToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
			this.buildToolStripMenuItem.Text = "&Build";
			// 
			// createLL1ParserToolStripMenuItem
			// 
			this.createLL1ParserToolStripMenuItem.Name = "createLL1ParserToolStripMenuItem";
			this.createLL1ParserToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
			this.createLL1ParserToolStripMenuItem.Text = "&LL(1) Parser";
			this.createLL1ParserToolStripMenuItem.Click += new System.EventHandler(this.createLL1ParserToolStripMenuItem_Click);
			// 
			// createLalr1ParserToolStripMenuItem
			// 
			this.createLalr1ParserToolStripMenuItem.Name = "createLalr1ParserToolStripMenuItem";
			this.createLalr1ParserToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
			this.createLalr1ParserToolStripMenuItem.Text = "Lal&r(1) Parser";
			this.createLalr1ParserToolStripMenuItem.Click += new System.EventHandler(this.createLalr1ParserToolStripMenuItem_Click);
			// 
			// createFATokenizerToolStripMenuItem
			// 
			this.createFATokenizerToolStripMenuItem.Name = "createFATokenizerToolStripMenuItem";
			this.createFATokenizerToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
			this.createFATokenizerToolStripMenuItem.Text = "FA &Tokenizer";
			this.createFATokenizerToolStripMenuItem.Click += new System.EventHandler(this.createFATokenizerToolStripMenuItem_Click);
			// 
			// fATokenizerLL1ToolStripMenuItem
			// 
			this.fATokenizerLL1ToolStripMenuItem.Name = "fATokenizerLL1ToolStripMenuItem";
			this.fATokenizerLL1ToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
			this.fATokenizerLL1ToolStripMenuItem.Text = "FA Tokeni&zer / LL(1)";
			this.fATokenizerLL1ToolStripMenuItem.Click += new System.EventHandler(this.fATokenizerLL1ToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(213, 6);
			// 
			// createPCKSpecToolStripMenuItem
			// 
			this.createPCKSpecToolStripMenuItem.Name = "createPCKSpecToolStripMenuItem";
			this.createPCKSpecToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
			this.createPCKSpecToolStripMenuItem.Text = "&PCK spec";
			this.createPCKSpecToolStripMenuItem.Click += new System.EventHandler(this.createPCKSpecToolStripMenuItem_Click);
			// 
			// createFactoredPCKSpecToolStripMenuItem
			// 
			this.createFactoredPCKSpecToolStripMenuItem.Name = "createFactoredPCKSpecToolStripMenuItem";
			this.createFactoredPCKSpecToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
			this.createFactoredPCKSpecToolStripMenuItem.Text = "&Factored PCK spec";
			this.createFactoredPCKSpecToolStripMenuItem.Click += new System.EventHandler(this.createFactoredPCKSpecToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSplitTextArea,
            this.toolStripSeparator3,
            this.menuShowSpacesTabs,
            this.menuShowNewlines,
            this.menuShowLineNumbers,
            this.menuHighlightCurrentRow,
            this.menuBracketMatchingStyle,
            this.menuEnableVirtualSpace,
            this.toolStripSeparator4,
            this.menuSetTabSize,
            this.menuSetFont,
            this.toolStripMenuItem1,
            this.codeLanguageToolStripMenuItem});
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
			this.optionsToolStripMenuItem.Text = "&Options";
			// 
			// menuSplitTextArea
			// 
			this.menuSplitTextArea.Name = "menuSplitTextArea";
			this.menuSplitTextArea.Size = new System.Drawing.Size(403, 26);
			this.menuSplitTextArea.Text = "Split text area";
			this.menuSplitTextArea.Click += new System.EventHandler(this.menuSplitTextArea_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(400, 6);
			// 
			// menuShowSpacesTabs
			// 
			this.menuShowSpacesTabs.Name = "menuShowSpacesTabs";
			this.menuShowSpacesTabs.Size = new System.Drawing.Size(403, 26);
			this.menuShowSpacesTabs.Text = "Show spaces && tabs";
			this.menuShowSpacesTabs.Click += new System.EventHandler(this.menuShowSpaces_Click);
			// 
			// menuShowNewlines
			// 
			this.menuShowNewlines.Name = "menuShowNewlines";
			this.menuShowNewlines.Size = new System.Drawing.Size(403, 26);
			this.menuShowNewlines.Text = "Show newlines";
			this.menuShowNewlines.Click += new System.EventHandler(this.menuShowNewlines_Click);
			// 
			// menuShowLineNumbers
			// 
			this.menuShowLineNumbers.Name = "menuShowLineNumbers";
			this.menuShowLineNumbers.Size = new System.Drawing.Size(403, 26);
			this.menuShowLineNumbers.Text = "Show line numbers";
			this.menuShowLineNumbers.Click += new System.EventHandler(this.menuShowLineNumbers_Click);
			// 
			// menuHighlightCurrentRow
			// 
			this.menuHighlightCurrentRow.Name = "menuHighlightCurrentRow";
			this.menuHighlightCurrentRow.Size = new System.Drawing.Size(403, 26);
			this.menuHighlightCurrentRow.Text = "Highlight current row";
			this.menuHighlightCurrentRow.Click += new System.EventHandler(this.menuHighlightCurrentRow_Click);
			// 
			// menuBracketMatchingStyle
			// 
			this.menuBracketMatchingStyle.Name = "menuBracketMatchingStyle";
			this.menuBracketMatchingStyle.Size = new System.Drawing.Size(403, 26);
			this.menuBracketMatchingStyle.Text = "Highlight matching brackets when cursor is after";
			this.menuBracketMatchingStyle.Click += new System.EventHandler(this.menuBracketMatchingStyle_Click);
			// 
			// menuEnableVirtualSpace
			// 
			this.menuEnableVirtualSpace.Name = "menuEnableVirtualSpace";
			this.menuEnableVirtualSpace.Size = new System.Drawing.Size(403, 26);
			this.menuEnableVirtualSpace.Text = "Allow cursor past end-of-line";
			this.menuEnableVirtualSpace.Click += new System.EventHandler(this.menuEnableVirtualSpace_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(400, 6);
			// 
			// menuSetTabSize
			// 
			this.menuSetTabSize.Name = "menuSetTabSize";
			this.menuSetTabSize.Size = new System.Drawing.Size(403, 26);
			this.menuSetTabSize.Text = "Set tab size...";
			this.menuSetTabSize.Click += new System.EventHandler(this.menuSetTabSize_Click);
			// 
			// menuSetFont
			// 
			this.menuSetFont.Name = "menuSetFont";
			this.menuSetFont.Size = new System.Drawing.Size(403, 26);
			this.menuSetFont.Text = "Set font...";
			this.menuSetFont.Click += new System.EventHandler(this.menuSetFont_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(400, 6);
			// 
			// codeLanguageToolStripMenuItem
			// 
			this.codeLanguageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cToolStripMenuItem,
            this.vBToolStripMenuItem});
			this.codeLanguageToolStripMenuItem.Name = "codeLanguageToolStripMenuItem";
			this.codeLanguageToolStripMenuItem.Size = new System.Drawing.Size(403, 26);
			this.codeLanguageToolStripMenuItem.Text = "Code Language";
			// 
			// cToolStripMenuItem
			// 
			this.cToolStripMenuItem.Checked = true;
			this.cToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cToolStripMenuItem.Name = "cToolStripMenuItem";
			this.cToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
			this.cToolStripMenuItem.Text = "C#";
			this.cToolStripMenuItem.Click += new System.EventHandler(this.cToolStripMenuItem_Click);
			// 
			// vBToolStripMenuItem
			// 
			this.vBToolStripMenuItem.Name = "vBToolStripMenuItem";
			this.vBToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
			this.vBToolStripMenuItem.Text = "Visual Basic";
			this.vBToolStripMenuItem.Click += new System.EventHandler(this.vBToolStripMenuItem_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.Multiselect = true;
			// 
			// fileTabs
			// 
			this.fileTabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fileTabs.Location = new System.Drawing.Point(0, 28);
			this.fileTabs.Margin = new System.Windows.Forms.Padding(4);
			this.fileTabs.Name = "fileTabs";
			this.fileTabs.SelectedIndex = 0;
			this.fileTabs.Size = new System.Drawing.Size(485, 228);
			this.fileTabs.TabIndex = 3;
			this.fileTabs.TabStop = false;
			this.fileTabs.Selected += new System.Windows.Forms.TabControlEventHandler(this.fileTabs_Selected);
			// 
			// fontDialog
			// 
			this.fontDialog.AllowVerticalFonts = false;
			this.fontDialog.ShowEffects = false;
			// 
			// messages
			// 
			this.messages.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.messages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.errorLevelColumnHeader,
            this.codeColumnHeader,
            this.descriptionColumnHeader,
            this.fileColumnHeader,
            this.lineColumnHeader});
			this.messages.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.messages.FullRowSelect = true;
			this.messages.HideSelection = false;
			this.messages.Location = new System.Drawing.Point(0, 259);
			this.messages.MultiSelect = false;
			this.messages.Name = "messages";
			this.messages.Size = new System.Drawing.Size(485, 82);
			this.messages.SmallImageList = this.imageList1;
			this.messages.TabIndex = 4;
			this.messages.UseCompatibleStateImageBehavior = false;
			this.messages.View = System.Windows.Forms.View.Details;
			this.messages.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.messages_ItemSelectionChanged);
			// 
			// errorLevelColumnHeader
			// 
			this.errorLevelColumnHeader.Text = "";
			this.errorLevelColumnHeader.Width = 30;
			// 
			// codeColumnHeader
			// 
			this.codeColumnHeader.Text = "Code";
			// 
			// descriptionColumnHeader
			// 
			this.descriptionColumnHeader.Text = "Description";
			this.descriptionColumnHeader.Width = 270;
			// 
			// fileColumnHeader
			// 
			this.fileColumnHeader.Text = "File";
			// 
			// lineColumnHeader
			// 
			this.lineColumnHeader.Text = "Line";
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter1.Location = new System.Drawing.Point(0, 256);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(485, 3);
			this.splitter1.TabIndex = 5;
			this.splitter1.TabStop = false;
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "StatusAnnotations_Information_16xLG_color.png");
			this.imageList1.Images.SetKeyName(1, "StatusAnnotations_Warning_16xLG_color.png");
			this.imageList1.Images.SetKeyName(2, "StatusAnnotations_Critical_16xLG_color.png");
			// 
			// Main
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(485, 341);
			this.Controls.Add(this.fileTabs);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.messages);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "Main";
			this.Text = "Pck Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextEditor_FormClosing);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.TextEditorForm_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextEditorForm_DragEnter);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuFileOpen;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.ToolStripMenuItem menuFileSave;
		private System.Windows.Forms.TabControl fileTabs;
		private System.Windows.Forms.ToolStripMenuItem menuFileNew;
		private System.Windows.Forms.ToolStripMenuItem menuFileSaveAs;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem menuFileExit;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuEditCut;
		private System.Windows.Forms.ToolStripMenuItem menuEditCopy;
		private System.Windows.Forms.ToolStripMenuItem menuEditPaste;
		private System.Windows.Forms.ToolStripMenuItem menuEditDelete;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem menuEditFind;
		private System.Windows.Forms.ToolStripMenuItem menuEditReplace;
		private System.Windows.Forms.ToolStripMenuItem menuFileClose;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.ToolStripMenuItem menuFindAgain;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuSplitTextArea;
		private System.Windows.Forms.ToolStripMenuItem menuShowSpacesTabs;
		private System.Windows.Forms.ToolStripMenuItem menuShowNewlines;
		private System.Windows.Forms.ToolStripMenuItem menuHighlightCurrentRow;
		private System.Windows.Forms.ToolStripMenuItem menuBracketMatchingStyle;
		private System.Windows.Forms.ToolStripMenuItem menuEnableVirtualSpace;
		private System.Windows.Forms.ToolStripMenuItem menuShowLineNumbers;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem menuFindAgainReverse;
		private System.Windows.Forms.ToolStripMenuItem menuSetTabSize;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem menuSetFont;
		private System.Windows.Forms.FontDialog fontDialog;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem menuToggleBookmark;
		private System.Windows.Forms.ToolStripMenuItem menuGoToNextBookmark;
		private System.Windows.Forms.ToolStripMenuItem menuGoToPrevBookmark;
		private System.Windows.Forms.ToolStripMenuItem xbnfFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pckFileToolStripMenuItem;
		private System.Windows.Forms.ListView messages;
		private System.Windows.Forms.ColumnHeader errorLevelColumnHeader;
		private System.Windows.Forms.ColumnHeader codeColumnHeader;
		private System.Windows.Forms.ColumnHeader descriptionColumnHeader;
		private System.Windows.Forms.ColumnHeader fileColumnHeader;
		private System.Windows.Forms.ColumnHeader lineColumnHeader;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ToolStripMenuItem buildToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem createPCKSpecToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem createLL1ParserToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem createLalr1ParserToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem createFactoredPCKSpecToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem createFATokenizerToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem codeLanguageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem vBToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fATokenizerLL1ToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ImageList imageList1;
	}
}

