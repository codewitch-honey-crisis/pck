using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

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
		class _InputEnumerable : IEnumerable<char>
		{
			Test _outer;
			public _InputEnumerable(Test outer) { _outer = outer; }

			public IEnumerator<char> GetEnumerator() { return new _InputEnumerator(_outer); }
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
		class _InputEnumerator : IEnumerator<char>
		{
			Test _outer;
			IEnumerator<char> _inner;
			public _InputEnumerator(Test outer) { _outer = outer; Reset(); }

			public char Current => _inner.Current;
			object IEnumerator.Current => _inner.Current;

			public void Dispose() { if (null != _inner) _inner.Dispose(); }

			public bool MoveNext()
				=> _inner.MoveNext();

			public void Reset()
			{
				if (null != _inner) _inner.Dispose();
				_inner = _outer.editor.Text.GetEnumerator();
			}
		}
		public IEnumerable<char> Input { get { return new _InputEnumerable(this); } }

		private void editor_TextChanged(object sender, EventArgs e)
		{
			parseTimer.Enabled = true;
		}

		private void parseTimer_Tick(object sender, EventArgs e)
		{
			parseTimer.Enabled = false;
			ParseNode pt=null;
			if (null != _ll1Parser)
			{
				_ll1Parser.Restart();
				_ll1Parser.ShowHidden = showHidden.Checked;
				while(LLNodeType.EndDocument!=_ll1Parser.NodeType)
				{
					pt = _ll1Parser.ParseSubtree(trimTree.Checked, transformTree.Checked);
					if (null == pt.Value)
						break;
				}
			} else if(null!=_lalr1Parser)
			{
				_lalr1Parser.Restart();
				_lalr1Parser.ShowHidden = showHidden.Checked;
				while (LRNodeType.EndDocument != _lalr1Parser.NodeType)
				{
					pt = _lalr1Parser.ParseReductions(trimTree.Checked, transformTree.Checked);
					if (null == pt.Value)
						break;
				}
			}
			parseTree.Nodes.Clear();
			if(null!=pt)
			{
				var treeNode = new TreeNode();
				if(null!=pt.Value)
				{
					treeNode.Text = string.Concat(pt.Symbol, ": ", pt.Value);
					treeNode.ImageIndex = ("#ERROR" == pt.Symbol) ? 2 : 1;
					treeNode.SelectedImageIndex = treeNode.ImageIndex;
					treeNode.Tag = pt;
					parseTree.Nodes.Add(treeNode);
				} else
				{
					treeNode.Text = pt.Symbol;
					treeNode.Tag = pt;
					foreach (var ptc in pt.Children)
					{
						_AddNodes(treeNode, ptc);
					}
					parseTree.Nodes.Add(treeNode);
				}
				treeNode.ExpandAll();
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
			}
			else
			{
				treeNode.Text = pt.Symbol;
				treeNode.Tag = pt;
				foreach (var ptc in pt.Children) {
					_AddNodes(treeNode, ptc);
				}
				node.Nodes.Add(treeNode);

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
	}
}
