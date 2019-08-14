using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{

	public abstract class LL1Parser : IDisposable
	{
		public bool ShowHidden { get; set; } = false;
		public bool ShowCollapsed { get; set; } = true;
		public abstract LLNodeType NodeType { get; }
		public abstract string Symbol { get; }
		public abstract int SymbolId { get; }
		public abstract string Value { get; }
		public abstract int Line { get;  }
		public abstract int Column { get; }
		public abstract long Position { get; }
		public abstract bool Read();
		public abstract void Restart();
		public abstract void Restart(IEnumerable<Token> tokenizer);
		public abstract void Close();
		public abstract bool IsHidden { get; }
		public abstract bool IsCollapsed { get; }
		public abstract string Substitute { get; }
		public abstract int SubstituteId { get; }
		public abstract object GetAttribute(string name, object @default = null);
		void IDisposable.Dispose() { Close(); }

		/// <summary>
		/// Parses the from the current position into a parse tree. This will read an entire sub-tree.
		/// </summary>
		/// <param name="trim">Remove non-terminal nodes that have no terminals and collapse nodes that have a single non-terminal child</param>
		/// <param name="transform">Apply transformations indicated in the grammar to the tree</param>
		/// <returns>A <see cref="ParseNode"/> representing the parse tree. The reader's cursor is advanced.</returns>
		public virtual ParseNode ParseSubtree(bool trim = false,bool transform=true)
		{
			var res = false;
			while ((res = Read()) && transform && IsCollapsed);
			if (!res) return null;
			var nn = NodeType;
			if (LLNodeType.EndNonTerminal == nn)
				return null;

			var result = new ParseNode();
			result.IsHidden = IsHidden;
			result.IsCollapsed = IsCollapsed;
			var s = Substitute;
			if (null != s)
			{
				result.Symbol = s;
				result.SymbolId = SubstituteId;
				result.SubstituteFor = Symbol;
				result.SubstituteForId = SymbolId;
			}
			else
			{
				result.SubstituteForId = -1;
				result.Symbol = Symbol;
				result.SymbolId = SymbolId;
			}
			//if ("expressionlisttail" == result.Symbol) System.Diagnostics.Debugger.Break();

			if (LLNodeType.NonTerminal == nn)
			{
				while (true)
				{
					var k = ParseSubtree(trim,transform);
					if (null != k)
					{
						if (null != k.Value)
							result.Children.Add(k);
						else
						{
							if (!trim)
							{
								if(0<k.Children.Count)
									result.Children.Add(k);
							}
							else
							{
								if (1 < k.Children.Count)
									result.Children.Add(k);
								else
								{
									if (0 < k.Children.Count)
									{
										if (null == k.Children[0].Value)
											result.Children.Add(k.Children[0]);
										else
											result.Children.Add(k);
									}
								}
							}
						}
					}
					else
						break;
				}
				return result;
			}
			else if (LLNodeType.Terminal == nn)
			{
				result.SetLocation(Line, Column, Position);
				result.Value = Value;
				return result;
			}
			else if (LLNodeType.Error == nn)
			{
				result.SetLocation(Line, Column, Position);
				result.Value = Value;
				return result;
			}
			return null;
		}
	}
}
