using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	/// <summary>
	/// Represents a node of a parse tree
	/// </summary>
	public sealed class ParseNode
	{
		int _line;
		int _column;
		long _position;
		public ParseAttribute[] AttributeSet { get; set; }

		public object GetAttribute(string name,object @default=null)
		{
			var attrs = AttributeSet;
			for(var i =0;i<attrs.Length;i++)
			{
				var attr = attrs[i];
				if (attr.Name == name)
					return attr.Value;
			}
			return @default;
		}
		/// <summary>
		/// Gets every descendent of this node and itself
		/// </summary>
		/// <param name="result">The collection to fill</param>
		/// <returns>The <paramref name="result"/> or a new collection, filled with the results</returns>
		public IList<ParseNode> FillDescendantsAndSelf(IList<ParseNode> result = null)
		{
			if (null == result) result = new List<ParseNode>();
			result.Add(this);
			var ic = Children.Count;
			for (var i = 0; i < ic; ++i)
				Children[i].FillDescendantsAndSelf(result);
			return result;
		}
		public void SetLocation(int line, int column, long position)
		{
			_line = line;
			_column = column;
			_position = position;
		}
		public int Line {
			get {
				if (null == Value)
				{
					if (0 < Children.Count)
						return Children[0].Line;
					return 0;
				}
				else
				{
					return _line;
				}
			}
		}
		public int Column {
			get {
				if (null == Value)
				{
					if (0 < Children.Count)
						return Children[0].Column;
					return 0;
				}
				else
				{
					return _column;
				}
			}
		}
		public long Position {
			get {
				if (null == Value)
				{
					if (0 < Children.Count)
						return Children[0].Position;
					return 0;
				}
				else
				{
					return _position;
				}
			}
		}

		public int Length {
			get {
				if (null == Value)
				{
					if (0 < Children.Count)
					{
						var c = Children.Count - 1;
						var p = Children[c].Position;
						var l = Children[c].Length;
						return (int)(p - Position) + l;
					}
					return 0;
				}
				else
					return Value.Length;
			}
		}
		public bool IsHidden { get; set; }
		public bool IsCollapsed { get; set; }
		public string SubstituteFor { get; set; }
		public int SubstituteForId { get; set; }

		public string Symbol { get; set; }
		public int SymbolId { get; set; }
		public string Value { get; set; }
		
		public List<ParseNode> Children { get; } = new List<ParseNode>();

		public override string ToString()
		{
			var sb = new StringBuilder();
			_AppendTreeTo(sb, this);
			return sb.ToString();
		}

		static void _AppendTreeTo(StringBuilder result, ParseNode node)
		{
			// adapted from https://stackoverflow.com/questions/1649027/how-do-i-print-out-a-tree-structure
			List<ParseNode> firstStack = new List<ParseNode>();
			firstStack.Add(node);

			List<List<ParseNode>> childListStack = new List<List<ParseNode>>();
			childListStack.Add(firstStack);

			while (childListStack.Count > 0)
			{
				List<ParseNode> childStack = childListStack[childListStack.Count - 1];

				if (childStack.Count == 0)
				{
					childListStack.RemoveAt(childListStack.Count - 1);
				}
				else
				{
					node = childStack[0];
					childStack.RemoveAt(0);

					string indent = "";
					for (int i = 0; i < childListStack.Count - 1; i++)
					{
						indent += (childListStack[i].Count > 0) ? "|  " : "   ";
					}
					var s = node.Symbol;
					if (node.IsCollapsed)
						s = string.Concat("{", s, "}");
					if (node.IsHidden)
						s = string.Concat("(", s, ")");
					result.Append(string.Concat(indent, "+- ", s, " ", node.Value ?? "").TrimEnd());
					result.AppendLine();// string.Concat(" at line ", node.Line, ", column ", node.Column, ", position ", node.Position, ", length of ", node.Length));
					if (node.Children.Count > 0)
					{
						childListStack.Add(new List<ParseNode>(node.Children));
					}
				}
			}
		}
	}
}
