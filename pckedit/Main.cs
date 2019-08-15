using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace Pck
{
	/// <summary>Main form for a multi-file text editor based on 
	/// ICSharpCode.TextEditor.TextEditorControl.</summary>
	public partial class Main : Form
	{
		public Main()
		{
			InitializeComponent();
			var smp = new FileSyntaxModeProvider(".");
			HighlightingManager.Manager.AddSyntaxModeFileProvider(smp);
			
		}

		#region Code related to File menu

		private void menuFileNew_Click(object sender, EventArgs e)
		{
			
		}

		/// <summary>This variable holds the settings (whether to show line numbers, 
		/// etc.) that all editor controls share.</summary>
		ITextEditorProperties _editorSettings;

		private TextEditorControl AddNewTextEditor(string title)
		{
			var tab = new TabPage(title);
			var editor = new TextEditorControl();
			editor.Dock = System.Windows.Forms.DockStyle.Fill;
			editor.IsReadOnly = false;
			editor.Document.DocumentChanged += 
				new DocumentEventHandler((sender, e) => { SetModifiedFlag(editor, true); });
			// When a tab page gets the focus, move the focus to the editor control
			// instead when it gets the Enter (focus) event. I use BeginInvoke 
			// because changing the focus directly in the Enter handler doesn't 
			// work.
			tab.Enter +=
				new EventHandler((sender, e) => { 
					var page = ((TabPage)sender);
					page.BeginInvoke(new Action<TabPage>(p => p.Controls[0].Focus()), page);
				});
			tab.Controls.Add(editor);
			fileTabs.Controls.Add(tab);

			if (_editorSettings == null) {
				_editorSettings = editor.TextEditorProperties;
				OnSettingsChanged();
			} else
				editor.TextEditorProperties = _editorSettings;
			return editor;
		}

		private void menuFileOpen_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
				// Try to open chosen file
				OpenFiles(openFileDialog.FileNames);
		}

		private void OpenFiles(string[] fns)
		{
			// Close default untitled document if it is still empty
			if (fileTabs.TabPages.Count == 1 
				&& ActiveEditor.Document.TextLength == 0
				&& string.IsNullOrEmpty(ActiveEditor.FileName))
				RemoveTextEditor(ActiveEditor);

			// Open file(s)
			foreach (string fn in fns)
			{
				var editor = AddNewTextEditor(Path.GetFileName(fn));
				try {
					editor.LoadFile(fn);
					// Modified flag is set during loading because the document 
					// "changes" (from nothing to something). So, clear it again.
					SetModifiedFlag(editor, false);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, ex.GetType().Name);
					RemoveTextEditor(editor);
					return;
				}
				
				
			}
		}

		private void menuFileClose_Click(object sender, EventArgs e)
		{
			var editor = ActiveEditor;
			if (null!=editor)
			{
				
				if (IsModified(editor))
				{
					var r = MessageBox.Show(string.Format("Save changes to {0}?", editor.Parent.Text.Substring(0,editor.Parent.Text.Length-1) ?? "new file"),
						"Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
					if (r == DialogResult.Cancel)
						return;
					else if (r == DialogResult.Yes)
						if (!DoSave(editor))
							return;
				}
				RemoveTextEditor(ActiveEditor);
			}
		}
		void _AddMessage(CfgMessage msg,string filename)
		{
			var lvi = new ListViewItem(
				new string[] {
					"",
					(-1 < msg.ErrorCode) ? msg.ErrorCode.ToString() : "",
					msg.Message,
					filename ?? "", (!string.IsNullOrEmpty(filename) && 0<msg.Line)? msg.Line.ToString() : "" }
				);
			lvi.Tag = msg.Column;
		
			
			switch(msg.ErrorLevel)
			{
				case CfgErrorLevel.Message:
					lvi.ImageIndex = 0;
					break;
				case CfgErrorLevel.Warning:
					lvi.ImageIndex = 1;
					break;
				case CfgErrorLevel.Error:
					lvi.ImageIndex = 2;
					break;
			}
			messages.Items.Add(lvi);
		}
		private void RemoveTextEditor(TextEditorControl editor)
		{
			((TabControl)editor.Parent.Parent).Controls.Remove(editor.Parent);
		}

		private void menuFileSave_Click(object sender, EventArgs e)
		{
			TextEditorControl editor = ActiveEditor;
			if (editor != null)
				DoSave(editor);
		}

		private bool DoSave(TextEditorControl editor)
		{
			if (string.IsNullOrEmpty(editor.FileName))
				return DoSaveAs(editor);
			else {
				try {
					editor.SaveFile(editor.FileName);
					SetModifiedFlag(editor, false);
					return true;
				} catch (Exception ex) {
					MessageBox.Show(ex.Message, ex.GetType().Name);
					return false;
				}
			}
		}

		private void menuFileSaveAs_Click(object sender, EventArgs e)
		{
			var editor = ActiveEditor;
			if (editor != null)
				DoSaveAs(editor);
		}

		private bool DoSaveAs(TextEditorControl editor)
		{
			saveFileDialog.FileName = editor.FileName;
			if (saveFileDialog.ShowDialog() == DialogResult.OK) {
				try {
					editor.SaveFile(saveFileDialog.FileName);
					editor.Parent.Text = Path.GetFileName(editor.FileName);
					SetModifiedFlag(editor, false);
										
					// The syntax highlighting strategy doesn't change
					// automatically, so do it manually.
					editor.Document.HighlightingStrategy =
						HighlightingStrategyFactory.CreateHighlightingStrategyForFile(editor.FileName);
					return true;
				} catch (Exception ex) {
					MessageBox.Show(ex.Message, ex.GetType().Name);
				}
			}
			return false;
		}

		private void menuFileExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}
		
		#endregion

		#region Code related to Edit menu

		/// <summary>Performs an action encapsulated in IEditAction.</summary>
		/// <remarks>
		/// There is an implementation of IEditAction for every action that 
		/// the user can invoke using a shortcut key (arrow keys, Ctrl+X, etc.)
		/// The editor control doesn't provide a public funciton to perform one
		/// of these actions directly, so I wrote DoEditAction() based on the
		/// code in TextArea.ExecuteDialogKey(). You can call ExecuteDialogKey
		/// directly, but it is more fragile because it takes a Keys value (e.g.
		/// Keys.Left) instead of the action to perform.
		/// <para/>
		/// Clipboard commands could also be done by calling methods in
		/// editor.ActiveTextAreaControl.TextArea.ClipboardHandler.
		/// </remarks>
		private void DoEditAction(TextEditorControl editor, ICSharpCode.TextEditor.Actions.IEditAction action)
		{
			if (editor != null && action != null) {
				var area = editor.ActiveTextAreaControl.TextArea;
				editor.BeginUpdate();
				try {
					lock (editor.Document) {
						action.Execute(area);
						if (area.SelectionManager.HasSomethingSelected && area.AutoClearSelection /*&& caretchanged*/) {
							if (area.Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal) {
								area.SelectionManager.ClearSelection();
							}
						}
					}
				} finally {
					editor.EndUpdate();
					area.Caret.UpdateCaretPosition();
				}
			}
		}

		private void menuEditCut_Click(object sender, EventArgs e)
		{
			if (HaveSelection())
				DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.Cut());
		}
		private void menuEditCopy_Click(object sender, EventArgs e)
		{
			if (HaveSelection())
				DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.Copy());
		}
		private void menuEditPaste_Click(object sender, EventArgs e)
		{
			DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.Paste());
		}
		private void menuEditDelete_Click(object sender, EventArgs e)
		{
			if (HaveSelection())
				DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.Delete());
		}

		private bool HaveSelection()
		{
			var editor = ActiveEditor;
			return editor != null &&
				editor.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected;
		}

		FindAndReplaceForm _findForm = new FindAndReplaceForm();

		private void menuEditFind_Click(object sender, EventArgs e)
		{
			TextEditorControl editor = ActiveEditor;
			if (editor == null) return;
			_findForm.ShowFor(editor, false);
		}

		private void menuEditReplace_Click(object sender, EventArgs e)
		{
			TextEditorControl editor = ActiveEditor;
			if (editor == null) return;
			_findForm.ShowFor(editor, true);
		}

		private void menuFindAgain_Click(object sender, EventArgs e)
		{
			_findForm.FindNext(true, false, 
				string.Format("Search text «{0}» not found.", _findForm.LookFor));
		}
		private void menuFindAgainReverse_Click(object sender, EventArgs e)
		{
			_findForm.FindNext(true, true, 
				string.Format("Search text «{0}» not found.", _findForm.LookFor));
		}

		private void menuToggleBookmark_Click(object sender, EventArgs e)
		{
			var editor = ActiveEditor;
			if (editor != null) {
				DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.ToggleBookmark());
				editor.IsIconBarVisible = editor.Document.BookmarkManager.Marks.Count > 0;
			}
		}

		private void menuGoToNextBookmark_Click(object sender, EventArgs e)
		{
			DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.GotoNextBookmark
				(bookmark => true));
		}

		private void menuGoToPrevBookmark_Click(object sender, EventArgs e)
		{
			DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.GotoPrevBookmark
				(bookmark => true));
		}

		#endregion

		#region Code related to Options menu

		/// <summary>Toggles whether the editor control is split in two parts.</summary>
		/// <remarks>Exercise for the reader: modify TextEditorControl and
		/// TextAreaControl so it shows a little "splitter stub" like you see in
		/// other apps, that allows the user to split the text editor by dragging
		/// it.</remarks>
		private void menuSplitTextArea_Click(object sender, EventArgs e)
		{
			TextEditorControl editor = ActiveEditor;
			if (editor == null) return;
			editor.Split();
		}

		/// <summary>Show current settings on the Options menu</summary>
		/// <remarks>We don't have to sync settings between the editors because 
		/// they all share the same DefaultTextEditorProperties object.</remarks>
		private void OnSettingsChanged()
		{
			menuShowSpacesTabs.Checked = _editorSettings.ShowSpaces;
			menuShowNewlines.Checked = _editorSettings.ShowEOLMarker;
			menuHighlightCurrentRow.Checked = _editorSettings.LineViewerStyle == LineViewerStyle.FullRow;
			menuBracketMatchingStyle.Checked = _editorSettings.BracketMatchingStyle == BracketMatchingStyle.After;
			menuEnableVirtualSpace.Checked = _editorSettings.AllowCaretBeyondEOL;
			menuShowLineNumbers.Checked = _editorSettings.ShowLineNumbers;
		}

		private void menuShowSpaces_Click(object sender, EventArgs e)
		{
			TextEditorControl editor = ActiveEditor;
			if (editor == null) return;
			editor.ShowSpaces = editor.ShowTabs = !editor.ShowSpaces;
			OnSettingsChanged();
		}
		private void menuShowNewlines_Click(object sender, EventArgs e)
		{
			TextEditorControl editor = ActiveEditor;
			if (editor == null) return;
			editor.ShowEOLMarkers = !editor.ShowEOLMarkers;
			OnSettingsChanged();
		}

		private void menuHighlightCurrentRow_Click(object sender, EventArgs e)
		{
			TextEditorControl editor = ActiveEditor;
			if (editor == null) return;
			editor.LineViewerStyle = editor.LineViewerStyle == LineViewerStyle.None 
				? LineViewerStyle.FullRow : LineViewerStyle.None;
			OnSettingsChanged();
		}

		private void menuBracketMatchingStyle_Click(object sender, EventArgs e)
		{
			TextEditorControl editor = ActiveEditor;
			if (editor == null) return;
			editor.BracketMatchingStyle = editor.BracketMatchingStyle == BracketMatchingStyle.After 
				? BracketMatchingStyle.Before : BracketMatchingStyle.After;
			OnSettingsChanged();
		}

		private void menuEnableVirtualSpace_Click(object sender, EventArgs e)
		{
			TextEditorControl editor = ActiveEditor;
			if (editor == null) return;
			editor.AllowCaretBeyondEOL = !editor.AllowCaretBeyondEOL;
			OnSettingsChanged();
		}

		private void menuShowLineNumbers_Click(object sender, EventArgs e)
		{
			TextEditorControl editor = ActiveEditor;
			if (editor == null) return;
			editor.ShowLineNumbers = !editor.ShowLineNumbers;
			OnSettingsChanged();
		}

		private void menuSetTabSize_Click(object sender, EventArgs e)
		{
			if (ActiveEditor != null) {
				string result = InputBox.Show("Specify the desired tab width.", "Tab size", _editorSettings.TabIndent.ToString());
				int value;
				if (result != null && int.TryParse(result, out value) && value.IsInRange(1, 32)) {
					ActiveEditor.TabIndent = value;
				}
			}
		}
		
		private void menuSetFont_Click(object sender, EventArgs e)
		{
			var editor = ActiveEditor;
			if (editor != null) {
				fontDialog.Font = editor.Font;
				if (fontDialog.ShowDialog(this) == DialogResult.OK) {
					editor.Font = fontDialog.Font;
					OnSettingsChanged();
				}
			}
		}

		#endregion

		#region Other stuff

		private void TextEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Ask user to save changes
			foreach (var editor in AllEditors)
			{
				if (IsModified(editor))
				{
					var r = MessageBox.Show(string.Format("Save changes to {0}?", editor.Parent.Text.Substring(0,editor.Parent.Text.Length-1) ?? "new file"),
						"Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
					if (r == DialogResult.Cancel)
						e.Cancel = true;
					else if (r == DialogResult.Yes)
						if (!DoSave(editor))
							e.Cancel = true;
				}
			}
		}

		/// <summary>Returns a list of all editor controls</summary>
		private IEnumerable<TextEditorControl> AllEditors
		{
			get {
				return from t in fileTabs.Controls.Cast<TabPage>()
					   from c in t.Controls.OfType<TextEditorControl>()
					   select c;
			}
		}
		
		/// <summary>Returns the currently displayed editor, or null if none are open</summary>
		private TextEditorControl ActiveEditor
		{
			get {
				if (fileTabs.TabPages.Count == 0) return null;
				return fileTabs.SelectedTab.Controls.OfType<TextEditorControl>().FirstOrDefault();
			}
		}
		
		/// <summary>Gets whether the file in the specified editor is modified.</summary>
		/// <remarks>TextEditorControl doesn't maintain its own internal modified 
		/// flag, so we use the '*' shown after the file name to represent the 
		/// modified state.</remarks>
		private bool IsModified(TextEditorControl editor)
		{
			// TextEditorControl doesn't seem to contain its own 'modified' flag, so 
			// instead we'll treat the "*" on the filename as the modified flag.
			return editor.Parent.Text.EndsWith("*");
		}
		private void SetModifiedFlag(TextEditorControl editor, bool flag)
		{
			if (IsModified(editor) != flag)
			{
				var p = editor.Parent;
				if (IsModified(editor))
					p.Text = p.Text.Substring(0, p.Text.Length - 1);
				else
					p.Text += "*";
			}
		}

		/// <summary>We handle DragEnter and DragDrop so users can drop files on the editor.</summary>
		private void TextEditorForm_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}
		private void TextEditorForm_DragDrop(object sender, DragEventArgs e)
		{
			string[] list = e.Data.GetData(DataFormats.FileDrop) as string[];
			if (list != null)
				OpenFiles(list);
		}

		#endregion

		private void xbnfFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var editor = AddNewTextEditor("Untitled.xbnf");
			editor.Document.HighlightingStrategy =
						HighlightingStrategyFactory.CreateHighlightingStrategyForFile("Untitled.xbnf");
			_UpdateBuildMenu();
		}
		
		private void fileTabs_Selected(object sender, TabControlEventArgs e)
		{
			_UpdateBuildMenu();
		}
		void _UpdateBuildMenu()
		{
			if (null == ActiveEditor)
			{
				foreach (var item in buildToolStripMenuItem.DropDownItems)
				{
					var tsi = item as ToolStripMenuItem;
					if(null!=tsi)
						tsi.Enabled	= false;
				}
				return;
			}
			var title = ActiveEditor.Parent.Text;
			if (title.EndsWith("*"))
				title = title.Substring(0, title.Length - 1);
			switch (Path.GetExtension(title).ToLowerInvariant())
			{
				case ".pck":
					foreach (var item in buildToolStripMenuItem.DropDownItems)
					{
						var tsi = item as ToolStripMenuItem;
						if(null!=tsi)
							tsi.Enabled = true;
					}
					createPCKSpecToolStripMenuItem.Enabled = false;
					break;

				case ".xbnf":
					foreach (var item in buildToolStripMenuItem.DropDownItems)
					{
						var tsi = item as ToolStripMenuItem;
						if(null!=tsi)
							tsi.Enabled = true;
					}
					break;
				default:
					foreach (var item in buildToolStripMenuItem.DropDownItems)
					{
						var tsi = item as ToolStripMenuItem;
						if(null!=tsi)
							tsi.Enabled = false;
					}
					break;

			}
		}
		private void pckFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var editor = AddNewTextEditor("Untitled.pck");
			editor.Document.HighlightingStrategy =
						HighlightingStrategyFactory.CreateHighlightingStrategyForFile("Untitled.pck");
			_UpdateBuildMenu();
		}

		private void createPCKSpecToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var name = fileTabs.SelectedTab.Text;
			if (name.EndsWith("*"))
				name = name.Substring(0, name.Length - 1);
			name = Path.GetFileNameWithoutExtension(name);
			var sb = new StringBuilder();
			XbnfDocument xbnf = XbnfDocument.Parse(ActiveEditor.Text);

			XbnfToPckTransform.Transform(xbnf, new StringWriter(sb));
			var editor = AddNewTextEditor(string.Concat(name,".pck"));
			name = _GetUniqueFilename(name);
			editor.Document.HighlightingStrategy =
						HighlightingStrategyFactory.CreateHighlightingStrategyForFile("Untitled.pck");
			editor.Text = sb.ToString();
			SetModifiedFlag(editor, true);	
		}

		private void createFactoredPCKSpecToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var name = fileTabs.SelectedTab.Text;
			if (name.EndsWith("*"))
				name = name.Substring(0, name.Length - 1);
			var ext = Path.GetExtension(name).ToLowerInvariant();
			name = Path.GetFileNameWithoutExtension(name);
			messages.Items.Clear();
			var hasErrors = false;
			var input = ActiveEditor.Text;
			switch (ext)
			{
				case ".xbnf":
					var sb = new StringBuilder();
					XbnfToPckTransform.Transform(new StringReader(ActiveEditor.Text), new StringWriter(sb));
					input = sb.ToString();
					goto case ".pck";
				case ".pck":
					name = string.Concat(name , ".ll1");
					var cfg = CfgDocument.Parse(input);
					var lex = LexDocument.Parse(input);
					lex.AttributeSets.Clear(); // prevent attributes from being written twice
					foreach (var msg in cfg.FillValidateLL1())
					{
						if (CfgErrorLevel.Error == msg.ErrorLevel)
							hasErrors = true;
						var n = fileTabs.SelectedTab.Text;
						if (n.EndsWith("*"))
							n = n.Substring(0, n.Length - 1);
						_AddMessage(msg, (".pck" == ext) ? n : "");

					}
					if (!hasErrors)
					{
						foreach (var msg in cfg.PrepareLL1(false))
						{
							if (CfgErrorLevel.Error == msg.ErrorLevel)
								hasErrors = true;
							var n = fileTabs.SelectedTab.Text;
							if (n.EndsWith("*"))
								n = n.Substring(0, n.Length - 1);
							_AddMessage(msg, (".pck" == ext) ? n : "");

						}
					}
					if (!hasErrors)
					{
						input = string.Concat(cfg.ToString(), Environment.NewLine, lex.ToString());
						name = string.Concat(name, ".pck");
						name = _GetUniqueFilename(name);
						var editor = AddNewTextEditor(name);
						editor.Document.HighlightingStrategy =
									HighlightingStrategyFactory.CreateHighlightingStrategyForFile(name);
						editor.Text = input;
						SetModifiedFlag(editor, true);
					}
					break;
			}

		}
		string _GetUniqueFilename(string filename)
		{
			var names = new HashSet<string>();
			foreach (var editor in AllEditors)
			{
				var name = editor.Parent.Text;
				if (name.EndsWith("*"))
					name = name.Substring(0, name.Length - 1);
				names.Add(name);
			}
			var i = 2;
			var fn = filename;
			while (true)
			{
				if (names.Contains(fn))
				{
					var ext = Path.GetExtension(filename);
					fn = Path.GetFileNameWithoutExtension(filename);
					fn = string.Concat(fn, i.ToString(), ext);
				}
				else
					return fn;
				++i;
			}
		}
		private void createLL1ParserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var name = fileTabs.SelectedTab.Text;
			if (name.EndsWith("*"))
				name = name.Substring(0, name.Length - 1);
			var ext = Path.GetExtension(name).ToLowerInvariant();
			name = Path.GetFileNameWithoutExtension(name);
			var input = ActiveEditor.Text;
			var sb = new StringBuilder();
			var hasErrors = false;
			messages.Items.Clear();
			switch (ext)
			{
				case ".xbnf":
					XbnfToPckTransform.Transform(new StringReader(ActiveEditor.Text), new StringWriter(sb));
					input = sb.ToString();
					goto case ".pck";
				case ".pck":
					var cfg = CfgDocument.Parse(input);
					foreach (var msg in cfg.FillValidateLL1())
					{
						if (CfgErrorLevel.Error == msg.ErrorLevel)
							hasErrors = true;
						var n = fileTabs.SelectedTab.Text;
						if (n.EndsWith("*"))
							n = n.Substring(0, n.Length - 1);
						_AddMessage(msg, (".pck" == ext) ? n : "");

					}
					if (!hasErrors)
					{
						foreach (var msg in cfg.PrepareLL1(false))
						{
							if (CfgErrorLevel.Error == msg.ErrorLevel)
								hasErrors = true;
							var n = fileTabs.SelectedTab.Text;
							if (n.EndsWith("*"))
								n = n.Substring(0, n.Length - 1);
							_AddMessage(msg, (".pck" == ext) ? n : "");

						}
					}
					if (!hasErrors)
					{
						string lang = null;
						if (cToolStripMenuItem.Checked)
						{
							lang = "cs";
							name = string.Concat(name, "Parser.cs");
						}
						else if (vBToolStripMenuItem.Checked)
						{
							lang = "vb";
							name = string.Concat(name, "Parser.vb");
						}
						sb.Clear();
						using (var sw = new StringWriter(sb))
							LL1ParserCodeGenerator.WriteClassTo(cfg, Path.GetFileNameWithoutExtension(name), null, lang, sw);
						input = sb.ToString();
						name = _GetUniqueFilename(name);
						var editor = AddNewTextEditor(name);
						editor.Document.HighlightingStrategy =
									HighlightingStrategyFactory.CreateHighlightingStrategyForFile(name);
						editor.Text = input;
						SetModifiedFlag(editor, true);
					}
					break;
			}
		}

		private void createLalr1ParserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var name = fileTabs.SelectedTab.Text;
			if (name.EndsWith("*"))
				name = name.Substring(0, name.Length - 1);
			var ext = Path.GetExtension(name).ToLowerInvariant();
			name = Path.GetFileNameWithoutExtension(name);
			var input = ActiveEditor.Text;
			var sb = new StringBuilder();
			messages.Items.Clear();
			var hasErrors = false;
			switch (ext)
			{
				case ".xbnf":
					XbnfToPckTransform.Transform(new StringReader(ActiveEditor.Text), new StringWriter(sb));
					input = sb.ToString();
					goto case ".pck";
				case ".pck":
					//if(".pck"==ext)
					var cfg = CfgDocument.Parse(input);
					foreach (var msg in cfg.FillValidateLalr1())
					{
						if (CfgErrorLevel.Error == msg.ErrorLevel)
							hasErrors = true;
						var n = fileTabs.SelectedTab.Text;
						if (n.EndsWith("*"))
							n = n.Substring(0, n.Length - 1);
						_AddMessage(msg, (".pck" == ext) ? n : "");

					}
					if (!hasErrors)
					{
						string lang = null;
						if (cToolStripMenuItem.Checked)
						{
							lang = "cs";
							name = string.Concat(name, "Parser.cs");
						}
						else if (vBToolStripMenuItem.Checked)
						{
							lang = "vb";
							name = string.Concat(name, "Parser.vb");
						}
						sb.Clear();
						using (var sw = new StringWriter(sb))
							Lalr1ParserCodeGenerator.WriteClassTo(cfg, Path.GetFileNameWithoutExtension(name), null, lang, sw);
						input = sb.ToString();
						name = _GetUniqueFilename(name);
						var editor = AddNewTextEditor(name);
						editor.Document.HighlightingStrategy =
									HighlightingStrategyFactory.CreateHighlightingStrategyForFile(name);
						editor.Text = input;
						SetModifiedFlag(editor, true);
					}
					break;
			}
		}

		private void createFATokenizerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var name = fileTabs.SelectedTab.Text;
			if (name.EndsWith("*"))
				name = name.Substring(0, name.Length - 1);
			var ext = Path.GetExtension(name).ToLowerInvariant();
			name = Path.GetFileNameWithoutExtension(name);
			var input = ActiveEditor.Text;
			var sb = new StringBuilder();
			switch (ext)
			{
				case ".xbnf":
					XbnfToPckTransform.Transform(new StringReader(ActiveEditor.Text), new StringWriter(sb));
					input = sb.ToString();
					goto case ".pck";
				case ".pck":
					//if(".pck"==ext)
					var cfg = CfgDocument.Parse(input);
					var lex = LexDocument.Parse(input);
					string lang = null;
					if (cToolStripMenuItem.Checked)
					{
						lang = "cs";
						name = string.Concat(name, "Tokenizer.cs");
					}
					else if (vBToolStripMenuItem.Checked)
					{
						lang = "vb";
						name = string.Concat(name, "Tokenizer.vb");
					}
					sb.Clear();
					using (var sw = new StringWriter(sb))
						TokenizerCodeGenerator.WriteClassTo(lex, cfg.FillSymbols(),Path.GetFileNameWithoutExtension(name), null, lang, sw);
					input = sb.ToString();
					var editor = AddNewTextEditor(name);
					editor.Document.HighlightingStrategy =
								HighlightingStrategyFactory.CreateHighlightingStrategyForFile(name);
					editor.Text = input;
					SetModifiedFlag(editor, true);
					break;
			}
		}

		private void fATokenizerLL1ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var name = fileTabs.SelectedTab.Text;
			if (name.EndsWith("*"))
				name = name.Substring(0, name.Length - 1);
			var ext = Path.GetExtension(name).ToLowerInvariant();
			name = Path.GetFileNameWithoutExtension(name);
			var input = ActiveEditor.Text;
			var sb = new StringBuilder();
			var hasErrors = false;
			messages.Items.Clear();
			switch (ext)
			{
				case ".xbnf":
					XbnfToPckTransform.Transform(new StringReader(ActiveEditor.Text), new StringWriter(sb));
					input = sb.ToString();
					goto case ".pck";
				case ".pck":
					//if(".pck"==ext)
					var cfg = CfgDocument.Parse(input);
					var lex = LexDocument.Parse(input);
					foreach (var msg in cfg.PrepareLL1(false))
					{
						if (CfgErrorLevel.Error == msg.ErrorLevel)
							hasErrors = true;
						var n = fileTabs.SelectedTab.Text;
						if (n.EndsWith("*"))
							n = n.Substring(0, n.Length - 1);
						_AddMessage(msg, (".pck" == ext) ? n : "");

					}
					if (!hasErrors)
					{
						string lang = null;
						if (cToolStripMenuItem.Checked)
						{
							lang = "cs";
							name = string.Concat(name, "Tokenizer.cs");
						}
						else if (vBToolStripMenuItem.Checked)
						{
							lang = "vb";
							name = string.Concat(name, "Tokenizer.vb");
						}
						sb.Clear();
						using (var sw = new StringWriter(sb))
							TokenizerCodeGenerator.WriteClassTo(lex, cfg.FillSymbols(), Path.GetFileNameWithoutExtension(name), null, lang, sw);
						input = sb.ToString();
						var editor = AddNewTextEditor(name);
						editor.Document.HighlightingStrategy =
									HighlightingStrategyFactory.CreateHighlightingStrategyForFile(name);
						editor.Text = input;
						SetModifiedFlag(editor, true);
					}
					break;
			}
		}

		private void messages_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if(e.IsSelected)
			{
				var l = e.Item.SubItems[4].Text;
				var f = e.Item.SubItems[3].Text;
				foreach(var item in fileTabs.TabPages)
				{
					var tab = item as TabPage;
					var fn = tab.Text;
					if (fn.EndsWith("*"))
						fn = fn.Substring(0, fn.Length - 1);
					if(fn==f)
					{
						int line;
						if (int.TryParse(l, out line))
						{
							var col = 0;
							if(e.Item.Tag is int)
							{
								col = ((int)e.Item.Tag)-1;
							}
							if (0 > col)
								col = 0;
							--line;
							if (0 > line) line = 0;
							fileTabs.SelectedTab = tab;
							ActiveEditor.ActiveTextAreaControl.Caret.Position = new TextLocation(col, line);
							ActiveEditor.ActiveTextAreaControl.ScrollToCaret();
						}
						break;
					}
				}
				if(!string.IsNullOrEmpty(l))
				{
					// int.Parse(l)
				}
			}
		}

		private void cToolStripMenuItem_Click(object sender, EventArgs e)
		{
			cToolStripMenuItem.Checked = true;
			vBToolStripMenuItem.Checked = false;
		}

		private void vBToolStripMenuItem_Click(object sender, EventArgs e)
		{

			cToolStripMenuItem.Checked = false;
			vBToolStripMenuItem.Checked = true;
		}
	}
}