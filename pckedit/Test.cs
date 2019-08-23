using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;
namespace Pck
{
	public partial class Test : Form
	{
		LL1Parser _ll1Parser=null;
		Lalr1Parser _lalr1Parser=null;

		public Test()
		{
			InitializeComponent();
			SuspendLayout();

			this.Size = new Size(700, 500);
			ResumeLayout();

		}
		public void SetParser(LL1Parser parser) 
		{
			if (null != _ll1Parser)
				_ll1Parser.Close();
			_ll1Parser = parser;
			if(null!=_lalr1Parser)
				_lalr1Parser.Close();
			_lalr1Parser = null;
		}
		public void SetParser(Lalr1Parser parser) 
		{
			if (null != _lalr1Parser)
				_lalr1Parser.Close();
			_lalr1Parser = parser;
			if (null != _ll1Parser)
				_ll1Parser.Close();
			_ll1Parser = null;
		}
		
		private void editor_TextChanged(object sender, EventArgs e)
		{
			parseTimer.Enabled = true;
		}

		async void parseTimer_Tick(object sender, EventArgs e)
		{
			parseTimer.Enabled = false;
			ParseNode pt=null;
			if (null != _ll1Parser)
			{
				await Task.Run(() =>
				{
					_ll1Parser.Restart(editor.Text);
					_ll1Parser.ShowHidden = showHidden.Checked;
					while (LLNodeType.EndDocument != _ll1Parser.NodeType)
					{
						pt = _ll1Parser.ParseSubtree(trimTree.Checked, transformTree.Checked);
						if (null == pt.Value)
							break;
					}
				});
			} else if(null!=_lalr1Parser)
			{
				await Task.Run(() =>
				{
					_lalr1Parser.Restart(editor.Text);
					_lalr1Parser.ShowHidden = showHidden.Checked;
					var opt = pt;
					while (LRNodeType.EndDocument != _lalr1Parser.NodeType)
					{
						pt = _lalr1Parser.ParseReductions(trimTree.Checked, transformTree.Checked);
						if (null == pt || null == pt.Value)
							break;
						opt = pt;
					}
					if (null == pt)
						pt = opt;
				});
			}
			parseTree.Nodes.Clear();
			if(null!=pt)
			{
				await Task.Run(() =>
				{
					var treeNode = new TreeNode();
					if (null != pt.Value)
					{
						treeNode.Text = string.Concat(pt.Symbol, ": ", pt.Value);
						treeNode.ImageIndex = ("#ERROR" == pt.Symbol) ? 2 : 1;
						treeNode.SelectedImageIndex = treeNode.ImageIndex;
						treeNode.Tag = pt;
						BeginInvoke(new Func<TreeNode, int>(parseTree.Nodes.Add), treeNode);
						BeginInvoke(new Action(treeNode.Expand));
					}
					else
					{
						treeNode.Text = pt.Symbol;
						treeNode.Tag = pt;
						foreach (var ptc in pt.Children)
						{
							_AddNodes(treeNode, ptc);
						}
						BeginInvoke(new Func<TreeNode, int>(parseTree.Nodes.Add), treeNode);
						BeginInvoke(new Action(treeNode.Expand));
					}
				});
				
			}
		}
		void _AddNodes(TreeNode node,ParseNode pt)
		{
			var treeNode = new TreeNode();
			if (null != pt.Value)
			{
				treeNode.Text = string.Concat(pt.Symbol, ": ", pt.Value);
				treeNode.ImageIndex = ("#ERROR"==pt.Symbol)?2:1;
				treeNode.SelectedImageIndex = treeNode.ImageIndex;
				treeNode.Tag = pt;
				node.Nodes.Add(treeNode);
				treeNode.Expand();
			}
			else
			{
				treeNode.Text = pt.Symbol;
				treeNode.Tag = pt;
				foreach (var ptc in pt.Children) {
					_AddNodes(treeNode, ptc);
				}
				node.Nodes.Add(treeNode);
				treeNode.Expand();
			}
		}

		private void trimTree_CheckedChanged(object sender, EventArgs e)
		{
			parseTimer.Enabled = true;
		}

		private void transformTree_CheckedChanged(object sender, EventArgs e)
		{
			parseTimer.Enabled = true;
		}

		private void showHidden_CheckedChanged(object sender, EventArgs e)
		{
			parseTimer.Enabled = true;
		}

		private void parseTree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var pn = e.Node.Tag as ParseNode;
			if(null!=pn)
			{
				var start = editor.Document.OffsetToPosition((int)pn.Position);
				var sel = new DefaultSelection(
					editor.Document, 
					start, 
					editor.Document.OffsetToPosition(pn.Length + (int)pn.Position));
				editor.ActiveTextAreaControl.SelectionManager.SetSelection(sel);
				editor.ActiveTextAreaControl.Caret.Position = start;
			}

		}
	}
}
