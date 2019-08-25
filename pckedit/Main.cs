using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Pck
{
	/// <summary>Main form for a multi-file text editor based on 
	/// ICSharpCode.TextEditor.TextEditorControl.</summary>
	public partial class Main : Form
	{
		readonly SynchronizationContext _synchronizationContext;

		Mru _mru;
		public Main()
		{
			_synchronizationContext = SynchronizationContext.Current;
			InitializeComponent();
			_editorSettings = new DefaultTextEditorProperties();
			_GetSettings();
			OnSettingsChanged();
			var smp = new FileSyntaxModeProvider(".");
			HighlightingManager.Manager.AddSyntaxModeFileProvider(smp);
			_UpdateMenuContext();
			_mru = new Mru();
			_mru.Load();
			_UpdateMenuMru();
		}

		#region Code related to File menu

		/// <summary>This variable holds the settings (whether to show line numbers, 
		/// etc.) that all editor controls share.</summary>
		ITextEditorProperties _editorSettings;

		TextEditorControl _AddNewTextEditor(string title)
		{
			var tab = new TabPage(title);
			var editor = new TextEditorControl();
			editor.Dock = DockStyle.Fill;
			editor.IsReadOnly = false;
			editor.Document.DocumentChanged += 
				new DocumentEventHandler((sender, e) => { SetModifiedFlag(editor, true); });
			// When a tab page gets the focus, move the focus to the editor control
			// instead when it gets the Enter (focus) event. I use BeginInvoke 
			// because changing the focus directly in the Enter handler doesn't 
			// work.
			tab.Enter +=
				new EventHandler((sender, e) => { 
					var page = (TabPage)sender;
					page.BeginInvoke(new Action<TabPage>(p => p.Controls[0].Focus()), page);
				});
			tab.Controls.Add(editor);
			fileTabs.Controls.Add(tab);
			
			if (_editorSettings == null) {
				{
					_editorSettings = editor.TextEditorProperties;
					_GetSettings();
					OnSettingsChanged();
				}
			} else
				editor.TextEditorProperties = _editorSettings;
			editor.ContextMenuStrip = editorContextMenu;
			fileTabs.SelectedTab = tab;
			_UpdateMenuContext();
			return editor;
		}

		private void menuFileOpen_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
				// Try to open chosen file
				OpenFiles(openFileDialog.FileNames);
		}
		
		public void OpenFiles(string[] fns)
		{
			// Close default untitled document if it is still empty
			if (fileTabs.TabPages.Count == 1 
				&& ActiveEditor.Document.TextLength == 0
				&& string.IsNullOrEmpty(ActiveEditor.FileName))
				RemoveTextEditor(ActiveEditor);

			// Open file(s)
			foreach (string fn in fns)
			{
				var editor = _AddNewTextEditor(Path.GetFileName(fn));
				try {
					editor.LoadFile(fn);
					// Modified flag is set during loading because the document 
					// "changes" (from nothing to something). So, clear it again.
					SetModifiedFlag(editor, false);
					_mru.Add(fn);
					_UpdateMenuMru();
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
		void _AddMessage(CfgMessage msg)
		{
			var lvi = new ListViewItem(
				new string[] {
					"",
					(-1 < msg.ErrorCode) ? msg.ErrorCode.ToString() : "",
					msg.Message,
					msg.Filename ?? "", (!string.IsNullOrEmpty(msg.Filename) && 0<msg.Line)? msg.Line.ToString() : "" }
				);
			lvi.Tag = msg.Column;
		
			
			switch(msg.ErrorLevel)
			{
				case ErrorLevel.Message:
					lvi.ImageIndex = 0;
					break;
				case ErrorLevel.Warning:
					lvi.ImageIndex = 1;
					break;
				case ErrorLevel.Error:
					lvi.ImageIndex = 2;
					break;
			}

			messages.BeginInvoke(new Action<ListViewItem>(_AddLVItem), lvi);
		}
		private void _AddLVItem(ListViewItem lvi)
		{
			messages.Items.Add(lvi);
		}
		void _AddMessage(XbnfMessage msg)
		{
			var lvi = new ListViewItem(
				new string[] {
					"",
					(-1 < msg.ErrorCode) ? msg.ErrorCode.ToString() : "",
					msg.Message,
					msg.Filename ?? "", (!string.IsNullOrEmpty(msg.Filename) && 0<msg.Line)? msg.Line.ToString() : "" }
				);
			lvi.Tag = msg.Column;


			switch (msg.ErrorLevel)
			{
				case ErrorLevel.Message:
					lvi.ImageIndex = 0;
					break;
				case ErrorLevel.Warning:
					lvi.ImageIndex = 1;
					break;
				case ErrorLevel.Error:
					lvi.ImageIndex = 2;
					break;
			}
			messages.BeginInvoke(new Action<ListViewItem>(_AddLVItem), lvi);
		}
		void _AddMessage(ExpectingException ex, string filename)
		{
			var lvi = new ListViewItem(
				new string[] {
					"",
					"",
					ex.Message,
					filename ?? "", (!string.IsNullOrEmpty(filename) && 0<ex.Line)? ex.Line.ToString() : "" }
				);
			lvi.Tag = ex.Column;


			lvi.ImageIndex = 2;

			messages.BeginInvoke(new Action<ListViewItem>(_AddLVItem), lvi);
		}
		void _AddMessage(Exception ex, string filename)
		{
			var lvi = new ListViewItem(
				new string[] {
					"",
					"",
					ex.Message,
					filename ?? "",
					"" }
				);
			

			lvi.ImageIndex = 2;

			messages.BeginInvoke(new Action<ListViewItem>(_AddLVItem), lvi);
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
			var n = editor.FileName;
			if (string.IsNullOrEmpty(n))
				return DoSaveAs(editor);
			else {
				try {
					editor.SaveFile(n);
					SetModifiedFlag(editor, false);
					_mru.Add(n);
					_UpdateMenuMru();
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
		string _GetFilename(TextEditorControl editor)
		{
			if (null == editor || null==editor.Parent) return null;
			var p = editor.Parent.Text;
			if (string.IsNullOrEmpty(p))
				return p;
			if (p.EndsWith("*"))
				p = p.Substring(0, p.Length - 1);
			return p;
		}
		string _GetFilename(TabPage page)
		{
			if (null == page) return null;
			var p = page.Text;
			if (string.IsNullOrEmpty(p))
				return p;
			if (p.EndsWith("*"))
				p = p.Substring(0, p.Length - 1);
			return p;
		}
		private bool DoSaveAs(TextEditorControl editor)
		{
			var n = _GetFilename(editor);
			saveFileDialog.FileName = n;
			var ext = Path.GetExtension(n).ToLowerInvariant();
			var f = "";
			switch (ext)
			{
				case ".pck":
					f="Pck Spec Files (*.pck)|*.pck|";
					break;
				case ".xbnf":
					f = "XBNF Files (*.xbnf)|*.xbnf|";
					break;
				case ".cs":
					f = "C# Files (*.cs)|*.cs|";
					break;
				case ".vb":
					f = "Visual Basic Files (*.vb)|*.vb|";
					break;
				default:
					f = string.Concat(ext.Substring(1).ToUpperInvariant()," (*",ext);
					f = string.Concat(f, "|*", ext);
					f = string.Concat(f, "|");
					break;
			}
			f = string.Concat(f, "All Files (*.*)|*.*");
			saveFileDialog.Filter = f;
			if (saveFileDialog.ShowDialog() == DialogResult.OK) {
				try {
					editor.SaveFile(saveFileDialog.FileName);
					editor.Parent.Text = Path.GetFileName(editor.FileName);
					SetModifiedFlag(editor, false);
										
					// The syntax highlighting strategy doesn't change
					// automatically, so do it manually.
					editor.Document.HighlightingStrategy =
						HighlightingStrategyFactory.CreateHighlightingStrategyForFile(editor.FileName);
					_mru.Add(editor.FileName);
					_UpdateMenuMru();
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
		static readonly string _ExeName = Path.GetFileName(Assembly.GetExecutingAssembly().GetModules()[0].Name);

		/// <summary>Show current settings on the Options menu</summary>
		/// <remarks>We don't have to sync settings between the editors because 
		/// they all share the same DefaultTextEditorProperties object.</remarks>
		private void OnSettingsChanged()
		{
			if (null != _editorSettings)
			{
				menuShowSpacesTabs.Checked = _editorSettings.ShowSpaces;
				menuShowNewlines.Checked = _editorSettings.ShowEOLMarker;
				menuHighlightCurrentRow.Checked = _editorSettings.LineViewerStyle == LineViewerStyle.FullRow;
				menuBracketMatchingStyle.Checked = _editorSettings.BracketMatchingStyle == BracketMatchingStyle.After;
				menuEnableVirtualSpace.Checked = _editorSettings.AllowCaretBeyondEOL;
				menuShowLineNumbers.Checked = _editorSettings.ShowLineNumbers;
			}
			RegistryKey key = null;
			try
			{
				key = Registry.CurrentUser.OpenSubKey(string.Concat(@"Software\", Path.GetFileNameWithoutExtension(_ExeName)), true);
				if(null!=key)
				{
					if (null != _editorSettings)
					{
						key.SetValue("ShowSpacesAndTabs", _editorSettings.ShowSpaces ? 1 : 0);
						key.SetValue("ShowNewlines", _editorSettings.ShowEOLMarker ? 1 : 0);
						key.SetValue("HighlightCurrentRow", _editorSettings.LineViewerStyle == LineViewerStyle.FullRow ? 1 : 0);
						key.SetValue("HighlightMatchingBracketsAfter", _editorSettings.BracketMatchingStyle == BracketMatchingStyle.After ? 1 : 0);
						key.SetValue("EnableVirtualSpace", _editorSettings.AllowCaretBeyondEOL ? 1 : 0);
						key.SetValue("ShowLineNumbers", _editorSettings.ShowLineNumbers ? 1 : 0);
						key.SetValue("TabSize", _editorSettings.TabIndent);
						key.SetValue("Font", _editorSettings.Font.Name);
						key.SetValue("FontSize", _editorSettings.Font.Size.ToString());
					}
					var isVB = vbToolStripMenuItem.Checked;
					if (isVB)
					{
						key.SetValue("CodeLanguage", "vb");
					}
					else
						key.SetValue("CodeLanguage", "cs");
				}
			}
			finally
			{
				if (null != key)
					key.Close();
				key = null;
			}
		}
		void _GetSettings()
		{
			RegistryKey key = null;
			try
			{
				key = Registry.CurrentUser.OpenSubKey(string.Concat(@"Software\", Path.GetFileNameWithoutExtension(_ExeName)), false);
				if (null != key)
				{
					_editorSettings.ShowSpaces = 1 == (int)key.GetValue("ShowSpacesAndTabs", 0);
					_editorSettings.ShowEOLMarker = 1 == (int)key.GetValue("ShowNewlines", 0);
					_editorSettings.LineViewerStyle = 1 == (int)key.GetValue("HighlightCurrentRow", 0) ? LineViewerStyle.FullRow : LineViewerStyle.None;
					_editorSettings.BracketMatchingStyle = 1 == (int)key.GetValue("HighlightMatchingBracketsAfter", 1) ? BracketMatchingStyle.After : BracketMatchingStyle.Before;
					_editorSettings.AllowCaretBeyondEOL = 1 == (int)key.GetValue("EnableVirtualSpace", 0);
					_editorSettings.ShowLineNumbers = 1 == (int)key.GetValue("ShowLineNumbers", 1);
					_editorSettings.TabIndent = (int)key.GetValue("TabSize", 4);
					var f = (string)key.GetValue("Font", "Lucida Console");
					var fs = (string)key.GetValue("FontSize", "10");
					_editorSettings.Font = new Font(f, float.Parse(fs));
					var lang = (string)key.GetValue("CodeLanguage",null);
					if(!string.IsNullOrEmpty(lang) && "vb"==lang.ToLowerInvariant())
					{
						vbToolStripMenuItem.Checked = true;
						csToolStripMenuItem.Checked = false;
					} else
					{
						vbToolStripMenuItem.Checked = false;
						csToolStripMenuItem.Checked = true;
					}
				}
			}
			finally
			{
				if (null != key)
					key.Close();
				key = null;
			}
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
			if(!e.Cancel)
				_mru.Save();
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
			var editor = _AddNewTextEditor("Untitled.xbnf");
			editor.Document.HighlightingStrategy =
						HighlightingStrategyFactory.CreateHighlightingStrategyForFile("Untitled.xbnf");
			_UpdateMenuContext();
		}
		
		private void fileTabs_Selected(object sender, TabControlEventArgs e)
		{
			_UpdateMenuContext();
		}
		void _UpdateMenuMru()
		{
			if (null == _mru)
				return;
			foreach(var f in fileToolStripMenuItem.DropDownItems.ToArray())
			{
				var ts = f as ToolStripMenuItem;
				if(null!=ts)
				{
					var s = ts.Tag as string;
					if(!string.IsNullOrEmpty(s) && s.StartsWith("MRU:"))
						fileToolStripMenuItem.DropDownItems.Remove(ts);
				} else
				{
					var tsp = f as ToolStripSeparator;
					if (null != tsp && Equals("MRU", tsp.Tag))
						fileToolStripMenuItem.DropDownItems.Remove(tsp);
				}
			}
			if(0<_mru.Count)
			{
				var sep = new ToolStripSeparator();
				sep.Tag = "MRU";
				fileToolStripMenuItem.DropDownItems.Add(sep);
				foreach(var filepath in _mru)
				{
					var item = new ToolStripMenuItem();
					item.Tag = string.Concat("MRU:",filepath);
					item.Text = _ShortenPath(filepath,30);
					item.Click += new EventHandler(delegate {
						OpenFiles(new string[] { (item.Tag as string).Substring(4) });
					});
					fileToolStripMenuItem.DropDownItems.Add(item);
				}
			}
		}
		[DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
		static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

		static string _ShortenPath(string path, int length)
		{
			StringBuilder sb = new StringBuilder(521);
			PathCompactPathEx(sb, path, length, 0);
			return sb.ToString();
		}

		void _UpdateMenuContext()
		{
			
			if (null == ActiveEditor)
			{
				menuFileSave.Enabled = false;
				menuFileSaveAs.Enabled = false;
				buildToolStripMenuItem.Enabled = false;
				testToolStripMenuItem.Enabled = false;
				menuFileClose.Enabled = false;
				editToolStripMenuItem.Enabled = false;
				foreach (var item in buildToolStripMenuItem.DropDownItems)
				{
					var tsi = item as ToolStripMenuItem;
					if(null!=tsi)
						tsi.Enabled	= false;
				}
				foreach (var item in testToolStripMenuItem.DropDownItems)
				{
					var tsi = item as ToolStripMenuItem;
					if (null != tsi)
						tsi.Enabled = false;
				}
				return;
			} else
			{
				menuFileSave.Enabled = true;
				menuFileSaveAs.Enabled = true;
				buildToolStripMenuItem.Enabled = true;
				testToolStripMenuItem.Enabled = true;
				menuFileClose.Enabled = true;
				editToolStripMenuItem.Enabled = true;
			}
			var title = _GetFilename(ActiveEditor);
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
					foreach (var item in testToolStripMenuItem.DropDownItems)
					{
						var tsi = item as ToolStripMenuItem;
						if (null != tsi)
							tsi.Enabled = true;
					}
					break;

				case ".xbnf":
					foreach (var item in buildToolStripMenuItem.DropDownItems)
					{
						var tsi = item as ToolStripMenuItem;
						if(null!=tsi)
							tsi.Enabled = true;
					}
					foreach (var item in testToolStripMenuItem.DropDownItems)
					{
						var tsi = item as ToolStripMenuItem;
						if (null != tsi)
							tsi.Enabled = true;
					}
					break;

				default:
					buildToolStripMenuItem.Enabled = false;
					break;

			}
		}
		private void pckFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var editor = _AddNewTextEditor("Untitled.pck");
			editor.Document.HighlightingStrategy =
						HighlightingStrategyFactory.CreateHighlightingStrategyForFile("Untitled.pck");
			_UpdateMenuContext();
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
		string _XbnfToPck(string input,string fname=null, Progress form=null)
		{
			if (string.IsNullOrEmpty(input))
				return input;
			XbnfDocument xbnf = null;
			xbnf = XbnfDocument.Parse(input);

			try
			{
				xbnf.SetFilename(fname);
			}
			catch (ExpectingException expex)
			{
				_AddMessage(expex, fname);
				return null;
			}
			catch (Exception ex)
			{
				_AddMessage(ex, fname);
				return null;
			}
			var hasErrors = false;
			foreach(var msg in xbnf.TryValidate())
			{
				if (msg.ErrorLevel == ErrorLevel.Error)
					hasErrors = true;
				_AddMessage(msg);
			}
			if (hasErrors)
				return null;
			var sb = new StringBuilder();
			if(null!=form)
				form.WriteLog("Transforming XBNF to PCK...");
			using (var sw = new StringWriter(sb))
				XbnfToPckTransform.Transform(xbnf, sw);
			if (null != form)
				form.WriteLog(Environment.NewLine);

			return sb.ToString();

		}
		async void _HandleCreatePckSpec(object sender, EventArgs e)
		{
			var prog = new Progress();
			var fname = _GetFilename(fileTabs.SelectedTab);
			var ext = Path.GetExtension(fname).ToLowerInvariant();
			var name = Path.GetFileNameWithoutExtension(fname);
			var newName = _GetUniqueFilename(name+".pck"); 
			var factor = ReferenceEquals(sender, createFactoredPCKSpecToolStripMenuItem);
			messages.Items.Clear();
			var hasErrors = false;
			prog.Show(this);
			var input = ActiveEditor.Text;

			switch (ext)
			{
				case ".xbnf":
					await Task.Run(()=>{
						input = _XbnfToPck(input, fname, prog);
					});
					if (null == input)
						break;
					goto case ".pck";
				case ".pck":
					await Task.Run((Action)(() =>
					{
						if(factor)
							name = string.Concat(name, ".ll1");
						var cfg = CfgDocument.Parse(input);
						if (".pck" == ext)
							cfg.SetFilename(fname);
						else
							cfg.SetFilename(newName);
						var lex = LexDocument.Parse(input);
						lex.AttributeSets.Clear(); // prevent attributes from being written twice
						foreach (var msg in cfg.TryValidateLL1())
						{
							if (ErrorLevel.Error == msg.ErrorLevel)
								hasErrors = true;
							_AddMessage(msg);
						}
						if (!hasErrors && factor)
						{
							foreach (var msg in cfg.TryPrepareLL1(new _LL1Progress(prog)))
							{
								if (Pck.ErrorLevel.Error == msg.ErrorLevel)
									hasErrors = true;
								_AddMessage(msg);

							}
						}
						if (!hasErrors)
						{
							input = string.Concat(cfg.ToString(), Environment.NewLine, lex.ToString());
							name = string.Concat(name, ".pck");
						}
					}));
					if(!hasErrors)
					{
						name = newName;
						var editor = _AddNewTextEditor(name);
						editor.Document.HighlightingStrategy =
									HighlightingStrategyFactory.CreateHighlightingStrategyForFile(name);
						editor.Text = input;
						SetModifiedFlag(editor, true);
					}
					break;
			}
			prog.Close();

		}
		async void _HandleCreateLL1Parser(object sender, EventArgs e)
		{
			var fname = _GetFilename(fileTabs.SelectedTab);
			var ext = Path.GetExtension(fname).ToLowerInvariant();
			var name = Path.GetFileNameWithoutExtension(fname);
			var input = ActiveEditor.Text;
			var prog = new Progress();
			prog.Show(this);
			var hasErrors = false;
			var isVB = vbToolStripMenuItem.Checked;
			messages.Items.Clear();
			switch (ext)
			{
				case ".xbnf":
					await Task.Run(() =>
					{
						input = _XbnfToPck(input, fname, prog);
					});
					if (null == input)
						break;
					goto case ".pck";
				case ".pck":
					await Task.Run((Action)(() => { 
						var sb = new StringBuilder();
						var cfg = CfgDocument.Parse(input);
						if (ext == ".pck")
							cfg.SetFilename(fname);
						foreach (var msg in cfg.TryValidateLL1())
						{
							if (Pck.ErrorLevel.Error == msg.ErrorLevel)
								hasErrors = true;
							_AddMessage(msg);
						}
						if (!hasErrors)
						{
							foreach (var msg in cfg.TryPrepareLL1(new _LL1Progress(prog)))
							{
								if (Pck.ErrorLevel.Error == msg.ErrorLevel)
									hasErrors = true;
								_AddMessage(msg);
							}
						}
						if (!hasErrors)
						{
							string lang = null;
							if (!isVB)
							{
								lang = "cs";
								name = string.Concat(name, "Parser.cs");
							}
							else
							{
								lang = "vb";
								name = string.Concat(name, "Parser.vb");
							}
							sb.Clear();
							using (var sw = new StringWriter(sb))
								LL1ParserCodeGenerator.WriteClassTo(cfg, Path.GetFileNameWithoutExtension(name), null, lang, new _LL1Progress(prog), sw);
							input = sb.ToString();
						}
					}));
					if(!hasErrors)
					{ 
						name = _GetUniqueFilename(name);
						var editor = _AddNewTextEditor(name);
						editor.Document.HighlightingStrategy =
									HighlightingStrategyFactory.CreateHighlightingStrategyForFile(name);
						editor.Text = input;
						SetModifiedFlag(editor, true);	
					}
					break;
			}
			prog.Close();
		}

		async void _HandleCreateLalr1ParserClass(object sender, EventArgs e)
		{
			var fname = _GetFilename(fileTabs.SelectedTab);
			var ext = Path.GetExtension(fname).ToLowerInvariant();
			var name = Path.GetFileNameWithoutExtension(fname);
			var input = ActiveEditor.Text;
			var prog = new Progress();
			prog.Show(this);
			messages.Items.Clear();
			var isVB = vbToolStripMenuItem.Checked;
			var hasErrors = false;
			switch (ext)
			{
				case ".xbnf":
					input = _XbnfToPck(input, fname, prog);
					if (null == input)
						break;
					goto case ".pck";
				case ".pck":
					await Task.Run((Action)(()=>{ 
						var sb = new StringBuilder();
						var cfg = CfgDocument.Parse(input);
						if (ext == ".pck")
							cfg.SetFilename(fname);
						foreach (var msg in cfg.TryValidateLalr1())
						{
							if (ErrorLevel.Error == msg.ErrorLevel)
								hasErrors = true;
							_AddMessage(msg);

						}
						if (!hasErrors)
						{
							string lang = null;
							if (!isVB)
							{
								lang = "cs";
								name = string.Concat(name, "Parser.cs");
							}
							else
							{
								lang = "vb";
								name = string.Concat(name, "Parser.vb");
							}
							sb.Clear();
							using (var sw = new StringWriter(sb))
							{
								foreach (var msg in Lalr1ParserCodeGenerator.TryWriteClassTo(cfg, Path.GetFileNameWithoutExtension(name), null, lang, new _Lalr1Progress(prog), sw))
								{
									if (ErrorLevel.Error == msg.ErrorLevel)
										hasErrors = true;
									_AddMessage(msg);
								}
							}
							if (!hasErrors)
								input = sb.ToString();
						}
					}));
					if (!hasErrors)
					{
						name = _GetUniqueFilename(name);
						var editor = _AddNewTextEditor(name);
						editor.Document.HighlightingStrategy =
									HighlightingStrategyFactory.CreateHighlightingStrategyForFile(name);
						editor.Text = input;
						SetModifiedFlag(editor, true);
					}


					break;
			}
			prog.Close();
		}

		
		async void _HandleCreateTokenizerClass(object sender, EventArgs e)
		{
			var fname = _GetFilename(fileTabs.SelectedTab);
			var prog = new Progress();
			var ext = Path.GetExtension(fname).ToLowerInvariant();
			var name = Path.GetFileNameWithoutExtension(fname);
			var input = ActiveEditor.Text;
			var factor = ReferenceEquals(sender, fATokenizerLL1ToolStripMenuItem);
			var isVB = vbToolStripMenuItem.Checked;
			var hasErrors = false;
			messages.Items.Clear();
			prog.Show(this);
			switch (ext)
			{
				case ".xbnf":
					input = _XbnfToPck(input, fname, prog);
					if (null == input)
						break;
					goto case ".pck";
				case ".pck":
					await Task.Run(() => { 
						var sb = new StringBuilder();
						var cfg = CfgDocument.Parse(input);
						if (ext == ".pck")
							cfg.SetFilename(fname);
						var lex = LexDocument.Parse(input);
						if (factor)
						{
							foreach (var msg in cfg.TryPrepareLL1(new _LL1Progress(prog)))
							{
								if (ErrorLevel.Error == msg.ErrorLevel)
									hasErrors = true;
									_AddMessage(msg);

							}
							prog.WriteLog(Environment.NewLine);
						}
						if (!hasErrors)
						{
							string lang = null;
							if (!isVB)
							{
								lang = "cs";
								name = string.Concat(name, "Tokenizer.cs");
							}
							else 
							{
								lang = "vb";
								name = string.Concat(name, "Tokenizer.vb");
							}
							sb.Clear();
							using (var sw = new StringWriter(sb))
									TokenizerCodeGenerator.WriteClassTo(lex, cfg.FillSymbols(), Path.GetFileNameWithoutExtension(name), null, lang,new _FAProgress(prog),  sw);
							input = sb.ToString();
						}
					});
					if (!hasErrors)
					{
						name = _GetUniqueFilename(name);
						var editor = _AddNewTextEditor(name);
						editor.Document.HighlightingStrategy =
									HighlightingStrategyFactory.CreateHighlightingStrategyForFile(name);
						editor.Text = input;
						SetModifiedFlag(editor, true);
					}
					break;
			}
			prog.Close();
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
					var fn = _GetFilename(tab);
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
				
			}
		}

		private void csToolStripMenuItem_Click(object sender, EventArgs e)
		{
			csToolStripMenuItem.Checked = true;
			vbToolStripMenuItem.Checked = false;
			OnSettingsChanged();
		}

		private void vBToolStripMenuItem_Click(object sender, EventArgs e)
		{

			csToolStripMenuItem.Checked = false;
			vbToolStripMenuItem.Checked = true;
			OnSettingsChanged();
		}
		
	
		async void _HandleTestLL1Parser(object sender, EventArgs e)
		{
			var fname = _GetFilename(fileTabs.SelectedTab);
			var ext = Path.GetExtension(fname).ToLowerInvariant();
			var name = Path.GetFileNameWithoutExtension(fname);
			var input = ActiveEditor.Text;
			var prog = new Progress();
			prog.Show(this);
			var test = new Test();
			test.Text = string.Concat(test.Text, " - ", fname);
			var sb = new StringBuilder();
			var hasErrors = false;
			messages.Items.Clear();
			await Task.Run((Action)(() =>
			{
				switch (ext)
				{
					case ".xbnf":
						input = _XbnfToPck(input, fname, prog);
						if (null == input)
							break;
						goto case ".pck";
					case ".pck":
						var cfg = CfgDocument.Parse(input);
						if (ext == ".pck")
							cfg.SetFilename(fname);
						var lex = LexDocument.Parse(input);
						foreach (var msg in cfg.TryValidateLL1())
						{
							if (ErrorLevel.Error == msg.ErrorLevel)
								hasErrors = true;
							var n = fname;
							_AddMessage(msg);
						}
						if (!hasErrors)
						{
							foreach (var msg in cfg.TryPrepareLL1(new _LL1Progress(prog)))
							{
								if (ErrorLevel.Error == msg.ErrorLevel)
									hasErrors = true;
								var n = fname;
								_AddMessage(msg);
							}
						}
						if (!hasErrors)
						{
							var tokenizer = lex.ToTokenizer(null, cfg.EnumSymbols(), new _FAProgress(prog));
							LL1Parser parser;
							foreach (var msg in cfg.TryToLL1Parser(out parser, tokenizer, new _LL1Progress(prog)))
							{
								if (Pck.ErrorLevel.Error == msg.ErrorLevel)
									hasErrors = true;
								var n = fname;
								_AddMessage(msg);
							}
							if (!hasErrors)
							{
								test.SetParser(parser);
								BeginInvoke(new Action<Form>(test.Show), this);

							}
							else BeginInvoke(new Action(test.Close));

						}
						else BeginInvoke(new Action(test.Close));

						break;
				}
			}));
			prog.Close();
		}

		private async void _HandleTestLalr1Parser(object sender, EventArgs e)
		{
			var prog = new Progress();
			prog.Show(this);
			var fname = _GetFilename(fileTabs.SelectedTab);
			var ext = Path.GetExtension(fname).ToLowerInvariant();
			var name = Path.GetFileNameWithoutExtension(fname);
			var input = ActiveEditor.Text;
			var sb = new StringBuilder();
			var hasErrors = false;
			messages.Items.Clear();
			var test = new Test();
			test.Text = string.Concat(test.Text, " - ", fname);
			await Task.Run((Action)(() => {
				switch (ext)
				{
					case ".xbnf":
						input = _XbnfToPck(input, fname, prog);
						if (null == input)
							break;
						goto case ".pck";
					case ".pck":
						var cfg = CfgDocument.Parse(input);
						if (ext == ".pck")
							cfg.SetFilename(fname);
						var lex = LexDocument.Parse(input);
						foreach (var msg in cfg.TryValidateLalr1())
						{
							if (ErrorLevel.Error == msg.ErrorLevel)
								hasErrors = true;
							var n = fname;
							_AddMessage(msg);
						}
						if (!hasErrors)
						{
							var tokenizer = lex.ToTokenizer(null, cfg.EnumSymbols(), new _FAProgress(prog));
							Lalr1Parser parser;
							foreach (var msg in cfg.TryToLalr1Parser(out parser, tokenizer, new _Lalr1Progress(prog)))
							{
								if (ErrorLevel.Error == msg.ErrorLevel)
									hasErrors = true;
								var n = fname;
								_AddMessage(msg);
							}

							if (!hasErrors)
							{
								test.SetParser(parser);
								BeginInvoke(new Action<Form>(test.Show), this);
							}
							else BeginInvoke(new Action(test.Close));
						}
						else
							BeginInvoke(new Action(test.Close));

						break;
				}

			}));
			prog.Close();
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var ed = ActiveEditor;
			if(null!=ed)
				ed.Undo();
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var ed = ActiveEditor;
			if (null != ed)
				ed.Redo();
		}
	}
	#region _FAProgress
	class _FAProgress : IProgress<FAProgress>
	{
		Progress _form;
		FAStatus _oldStatus;
		public _FAProgress(Progress form)
		{
			_form = form;
		}
		public void Report(FAProgress value)
		{
			if(_oldStatus==value.Status)
			{
				_form.WriteLog(".");
			} else
			{
				if (FAStatus.Unknown != _oldStatus)
					_form.WriteLog(Environment.NewLine);
				switch(value.Status)
				{
					case FAStatus.DfaTransform:
						_form.WriteLog("Transforming NFA to DFA");
						break;
					case FAStatus.TrimDuplicates:
						_form.WriteLog("Trimming duplicate states");
						break;
				}
			}
			_oldStatus = value.Status;
		}
	}
	#endregion

	#region _Lalr1Progress
	class _Lalr1Progress : IProgress<CfgLalr1Progress>
	{
		Progress _form;
		CfgLalr1Status _oldStatus;
		public _Lalr1Progress(Progress form)
		{
			_form = form;
		}
		public void Report(CfgLalr1Progress value)
		{
			switch(value.Status)
			{
				case CfgLalr1Status.ComputingStates:
				case CfgLalr1Status.ComputingReductions:
				case CfgLalr1Status.CreatingLookaheadGrammar:
					if (_oldStatus==value.Status)
						_form.WriteLog(".");
					else
					{
						if (_oldStatus != CfgLalr1Status.Unknown)
						{
							_form.WriteLog(Environment.NewLine);

						}
						switch(value.Status)
						{
							case CfgLalr1Status.ComputingStates:
								_form.WriteLog("Computing LR states");
								break;
							case CfgLalr1Status.ComputingReductions:
								_form.WriteLog("Computing reductions");
								break;
							case CfgLalr1Status.CreatingLookaheadGrammar:
								_form.WriteLog("Creating lookahead grammar");
								break;
						}
						
					}
					_oldStatus = value.Status;
					break;
					
				
			}
		}
	}
	#endregion

	#region _LL1Progress
	class _LL1Progress : IProgress<CfgLL1Progress>
	{
		Progress _form;
		CfgLL1Status _oldStatus;
		public _LL1Progress(Progress form)
		{
			_form = form;
		}
		public void Report(CfgLL1Progress value)
		{
			if (_oldStatus == value.Status)
				_form.WriteLog(".");
			else
			{
				if (_oldStatus != CfgLL1Status.Unknown)
				{
					_form.WriteLog(Environment.NewLine);

				}
				switch (value.Status)
				{
					case CfgLL1Status.ComputingFollows:
						_form.WriteLog("Computing follows");
						break;
					case CfgLL1Status.ComputingPredicts:
						_form.WriteLog("Computing predicts");
						break;
					case CfgLL1Status.Factoring:
						_form.WriteLog("Factoring");
						break;
					case CfgLL1Status.CreatingParseTable:
						_form.WriteLog("Creating parse table");
						break;
				}

			}
			_oldStatus = value.Status;
		}
	}
	#endregion
}