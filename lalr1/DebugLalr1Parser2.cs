using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class DebugLalr1Parser2
	{
		CfgDocument _cfg;
		Lalr1ParseTable _parseTable;
		Token _token;
		IEnumerator<Token> _tokenEnum;
		HashSet<string> _hiddenTerminals;
		HashSet<string> _collapsed;
		LRNodeType _nodeType;
		Stack<int> _stack;
		string[] _ruleDef;

		public DebugLalr1Parser2(CfgDocument cfg, IEnumerable<Token> tokenizer, Lalr1ParseTable parseTable = null)
		{
			_cfg = cfg;
			_PopulateAttrs();
			//Restart(tokenizer);
			_stack = new Stack<int>();
			_tokenEnum = tokenizer.GetEnumerator();
			_parseTable = parseTable ?? cfg.ToLalr1ParseTable();
			_nodeType = LRNodeType.Initial;

		}
		public int Line => _token.Line;
		public int Column => _token.Column;
		public long Position => _token.Position;
		public LRNodeType NodeType {
			get { return _nodeType; }
		}
		public string Symbol {
			get {
				if (LRNodeType.Shift == _nodeType)
					return _token.Symbol;
				if (LRNodeType.Reduce == _nodeType)
					return _ruleDef[0];
				return null;
			}
		}
		public string Value {
			get {
				if(LRNodeType.Shift==_nodeType)
					return _token.Value;
				return null;
			}
		}
		public string[] RuleDefinition {
			get {
				if(LRNodeType.Reduce==_nodeType)
					return _ruleDef;
				return null;
			}
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
		public string Rule {
			get {
				if (LRNodeType.Reduce == _nodeType)
				{
					return _ToRuleString(_ruleDef);
				}
				return null;
			}
		}
		public bool IsHidden { get { return _IsHidden(Symbol); } }
		public ParseNode ParseReductions()
		{
			ParseNode p=null;
			var rs = new Stack<ParseNode>();
			while (Read())
			{
				switch (NodeType)
				{
					case LRNodeType.Shift:
						p = new ParseNode();
						p.SetLocation(Line, Column, Position);
						p.Symbol = Symbol;
						p.Value = Value;
						p.IsHidden = IsHidden;
						rs.Push(p);
						break;
					case LRNodeType.Reduce:
						var d = new List<ParseNode>();
						p = new ParseNode();
						p.Symbol = RuleDefinition[0];
						for (var i = 1; i < RuleDefinition.Length; ++i)
						{
							var pc = rs.Pop();
							p.Children.Insert(0, pc);
							if (_IsHidden(pc.Symbol))
								--i;
							
						}
						rs.Push(p);
						break;
					case LRNodeType.Accept:
						break;

				}
			}
			var result = rs.Pop();
			while (0 < rs.Count)
				result.Children.Insert(0, rs.Pop());
			return result;
		}
		void _PopulateAttrs()
		{
			_hiddenTerminals = new HashSet<string>();
			_collapsed = new HashSet<string>();
			foreach (var attrsym in _cfg.AttributeSets)
			{
				var i = attrsym.Value.IndexOf("hidden");
				if (-1 < i)
				{
					var hidden = attrsym.Value[i].Value;
					if ((hidden is bool) && ((bool)hidden))
						_hiddenTerminals.Add(attrsym.Key);
				}
				i = attrsym.Value.IndexOf("collapsed");
				if (-1 < i)
				{
					var collapsed = attrsym.Value[i].Value;
					if ((collapsed is bool) && ((bool)collapsed))
						_collapsed.Add(attrsym.Key);
				}
			}
		}
		public bool ShowHiddenTerminals { get; set; } = false;
		public bool Read()
		{ 
			if (_nodeType == LRNodeType.Initial)
			{
				_stack.Push(0); // push initial state
				if (!_tokenEnum.MoveNext())
					throw new Exception("Error in tokenizer implementation: Expecting #EOS token");
			}
			else if (LRNodeType.Accept == _nodeType)
			{
				_nodeType = LRNodeType.EndDocument;
				return true;
			}
			else if (LRNodeType.EndDocument == _nodeType)
				return false;
			if(!ShowHiddenTerminals)
			{
				while (_IsHidden(_tokenEnum.Current.Symbol))
					_tokenEnum.MoveNext();
			} else if(_IsHidden(_tokenEnum.Current.Symbol))
			{
				_token = _tokenEnum.Current;
				_nodeType = LRNodeType.Shift;
				_tokenEnum.MoveNext();
				return true;
			}
			var entry = _parseTable[_stack.Peek()];
			(int RuleOrStateId, string Left, string[] Right) trns;
			if (!entry.TryGetValue(_tokenEnum.Current.Symbol, out trns))
			{
				_Panic();
				return true;
			}
			if (null == trns.Right) // shift or accept
			{
				if (-1 != trns.RuleOrStateId) // shift
				{
					_nodeType = LRNodeType.Shift;
					_token = _tokenEnum.Current;
					_tokenEnum.MoveNext();
					_stack.Push(trns.RuleOrStateId);
					return true;
				}
				else
				{ // accept 
				  //throw if _tok is not $ (end)
					if ("#EOS" != _tokenEnum.Current.Symbol)
					{
						_Panic();
						return true;
					}

					_nodeType = LRNodeType.Accept;
					_stack.Clear();
					return true;
				}
			}
			else // reduce
			{
				_ruleDef = new string[trns.Right.Length + 1];
				_ruleDef[0] = trns.Left;
				trns.Right.CopyTo(_ruleDef, 1);
				for (int i = 0; i < trns.Right.Length; ++i)
					if (null != trns.Right[i])
						_stack.Pop();

				// There is a new number at the top of the stack. 
				// This number is our temporary state. Get the symbol 
				// from the left-hand side of the rule #. Treat it as 
				// the next input token in the GOTO table (and place 
				// the matching state at the top of the set stack).
				_stack.Push(_parseTable[_stack.Peek()][trns.Left].RuleOrStateId);
				_nodeType = LRNodeType.Reduce;
				return true;
			}

		}
		void _Panic()
		{
			throw new Exception("Parse error.");
		}
		bool _IsHidden(string symbol)
		{
			return _hiddenTerminals.Contains(symbol);
		}
	}
}
