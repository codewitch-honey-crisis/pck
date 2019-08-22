using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public abstract class Lalr1Parser : IDisposable
	{
		public bool ShowHidden { get; set; } = false;
		public abstract LRNodeType NodeType { get; }
		public abstract string Symbol { get; }
		public abstract int SymbolId { get; }
		public abstract string Value { get; }
		public abstract string[] RuleDefinition { get; }
		public abstract int[] RuleDefinitionIds { get; }
		public string Rule {
			get {
				if (LRNodeType.Reduce == NodeType)
					return _ToRuleString(RuleDefinition);
				return null;
			}
		}
		public abstract int Line { get; }
		public abstract int Column { get; }
		public abstract long Position { get; }
		public abstract bool IsHidden { get; }
		public abstract bool IsCollapsed { get; }
		public abstract string Substitute { get; }
		public abstract int SubstituteId { get; }
		public abstract KeyValuePair<string, object>[] GetAttributeSet(int symbolId);
		public abstract object GetAttribute(string name, object @default = null);
		public abstract bool Read();
		public abstract void Restart(ITokenizer tokenizer);
		public abstract void Restart(IEnumerable<char> input);
		public abstract void Restart();
		public abstract void Close();
		void IDisposable.Dispose()
			=> Close();
		public virtual ParseNode ParseReductions(bool trim = false, bool transform = true)
		{
			ParseNode p = null;
			var rs = new Stack<ParseNode>();
			while (Read())
			{
				switch (NodeType)
				{
					case LRNodeType.Shift:
						p = new ParseNode();
						p.SetLocation(Line, Column, Position);
						// this will get the original nodes attributes
						// in the case of a substitution. Still not sure
						// if that's preferred or not.
						p.AttributeSet = GetAttributeSet(SymbolId);
						var s = Substitute;
						if (null != s)
						{
							p.Symbol = s;
							p.SymbolId = SubstituteId;
							p.SubstituteFor = Symbol;
							p.SubstituteForId = SymbolId;
						}
						else
						{
							p.Symbol = Symbol;
							p.SymbolId = SymbolId;
							p.SubstituteForId = -1;
						}
						p.Value = Value;
						p.IsHidden = IsHidden;
						p.IsCollapsed = IsCollapsed;
						rs.Push(p);
						break;
					case LRNodeType.Reduce:
						if (!trim || 1/*2*/ < RuleDefinition.Length)
						{
							var d = new List<ParseNode>();
							p = new ParseNode();
							p.AttributeSet = GetAttributeSet(SymbolId);

							p.IsCollapsed = IsCollapsed;
							s = Substitute;
							if (null != s)
							{
								p.Symbol = s;
								p.SymbolId = SubstituteId;
								p.SubstituteFor = Symbol;
								p.SubstituteForId = SymbolId;
							}
							else
							{
								p.Symbol = Symbol;
								p.SymbolId = SymbolId;
								p.SubstituteForId = -1;
							}

							for (var i = 1; RuleDefinition.Length > i; i++)
							{
								var pc = rs.Pop();
								_AddChildren(pc, transform, p.Children);
								if ("#ERROR" == pc.Symbol)
									break;
								// don't count hidden terminals
								if (pc.IsHidden)
									--i;
							}
							rs.Push(p);
						}
						break;
					case LRNodeType.Accept:
						break;
					case LRNodeType.Error:
						p = new ParseNode();
						p.SetLocation(Line, Column, Position);
						p.Symbol = Symbol;
						p.SubstituteForId = -1;
						p.Value = Value;
						rs.Push(p);
						break;
				}
			}
			if (0 == rs.Count)
				return null;
			var result = rs.Pop();
			while ("#ERROR"!=result.Symbol && 0 < rs.Count)
				_AddChildren(rs.Pop(), transform, result.Children);
			return result;
		}
		void _AddChildren(ParseNode pc, bool transform, IList<ParseNode> result)
		{
			if (!transform)
			{
				result.Insert(0, pc);
				return;
			}
			if (pc.IsCollapsed)
			{
				if (null == pc.Value)
				{
					for (int ic = pc.Children.Count, i = ic - 1; 0 <= i; --i)
						_AddChildren(pc.Children[i], transform, result);
				}
			}
			else
				result.Insert(0, pc);
		}
		string _ToRuleString(string[] rule)
		{
			var sb = new StringBuilder();
			sb.Append(rule[0]);
			sb.Append(" ->");
			for (var i = 1; i < rule.Length; ++i)
			{
				sb.Append(' ');
				sb.Append(rule[i]);
			}
			return sb.ToString();
		}
	}

}
