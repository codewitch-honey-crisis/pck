using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class DebugLalr1Parser
	{
		CfgDocument _cfg;
		Lalr1ParseTable _parseTable;
		Token _token;
		Token _errorToken;
		IEnumerator<Token> _tokenEnum;
		HashSet<string> _hidden;
		HashSet<string> _collapsed;
		HashSet<string> _nonTerminals;
		LRNodeType _nodeType;
		Stack<int> _stack;
		string[] _ruleDef;
		List<string> _collapsedRight;

		public DebugLalr1Parser(CfgDocument cfg, IEnumerable<Token> tokenizer, Lalr1ParseTable parseTable = null)
		{
			_cfg = cfg;
			_PopulateAttrs();
			//Restart(tokenizer);
			_stack = new Stack<int>();
			_tokenEnum = tokenizer.GetEnumerator();
			_parseTable = parseTable ?? cfg.ToLalr1ParseTable();
			_nodeType = LRNodeType.Initial;
			_collapsedRight = new List<string>();
		}
		public int Line {
			get {
				switch(_nodeType)
				{
				
					case LRNodeType.Error:
						return _errorToken.Line;
					default:
						return _token.Line;
				}
			}
		}
		public int Column {
			get {
				switch (_nodeType)
				{

					case LRNodeType.Error:
						return _errorToken.Column;
					default:
						return _token.Column;
				}
			}
		}
		public long Position {
			get {
				switch (_nodeType)
				{

					case LRNodeType.Error:
						return _errorToken.Position;
					default:
						return _token.Position;
				}

			}
		}
		public LRNodeType NodeType {
			get { return _nodeType; }
		}
		public string Symbol {
			get {
				switch(_nodeType)
				{
					case LRNodeType.Shift:
						return _token.Symbol;
					case LRNodeType.Reduce:
						return _ruleDef[0];
					case LRNodeType.Error:
						return _errorToken.Symbol;
				}
				return null;
			}
		}
		public string Value {
			get {
				switch (_nodeType)
				{

					case LRNodeType.Error:
						return _errorToken.Value;
					case LRNodeType.Shift:
						return _token.Value;
				}

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
		public bool IsCollapsed { get { return _IsCollapsed(Symbol); } }
		void _AddChildren(ParseNode pc, bool transform,IList<ParseNode> result)
		{
			if(transform && _IsCollapsed(pc.Symbol))
			{
				if (null == pc.Value)
				{
					for(int ic=pc.Children.Count,i=ic-1;0<=i;--i)
						_AddChildren(pc.Children[i], transform, result);
				}
			} else
				result.Insert(0,pc);
			
		}
		public ParseNode ParseReductions(bool trim = false, bool transform = true)
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
						p.IsCollapsed = IsCollapsed;
						rs.Push(p);
						break;
					case LRNodeType.Reduce:
						if (!trim || 2 < RuleDefinition.Length)
						{
							var d = new List<ParseNode>();
							p = new ParseNode();
							p.Symbol = RuleDefinition[0];
							for (var i = 1;RuleDefinition.Length>i; i++)
							{
								var pc = rs.Pop();
								_AddChildren(pc, transform, p.Children);
								// don't count hidden terminals
								if (_IsHidden(pc.Symbol))
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
						p.Value = Value;
						rs.Push(p);
						break;
				}
			}
			var result = rs.Pop();
			while (0 < rs.Count)
				_AddChildren(rs.Pop(), transform, result.Children);
			return result;
		}
		void _PopulateAttrs()
		{
			_hidden = new HashSet<string>();
			_collapsed = new HashSet<string>();
			_nonTerminals = new HashSet<string>(_cfg.EnumNonTerminals());
			foreach (var attrsym in _cfg.AttributeSets)
			{
				var i = attrsym.Value.IndexOf("hidden");
				if (-1 < i)
				{
					var hidden = attrsym.Value[i].Value;
					if ((hidden is bool) && ((bool)hidden))
						_hidden.Add(attrsym.Key);
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
		public bool ShowHidden { get; set; } = false;
			
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
			if (LRNodeType.Error != _nodeType)
			{
				if (!ShowHidden)
				{
					while (_IsHidden(_tokenEnum.Current.Symbol))
						_tokenEnum.MoveNext();
				}
				else if (_IsHidden(_tokenEnum.Current.Symbol))
				{
					_token = _tokenEnum.Current;
					_nodeType = LRNodeType.Shift;
					_tokenEnum.MoveNext();
					return true;
				}
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
			// This is primitive. Should see if the Dragon Book has something better
			_nodeType = LRNodeType.Error;
			var state = _stack.Peek();
			var d = _parseTable[state];
			(int RuleOrStateId, string Left, string[] Right) e;
			_errorToken.Symbol = "#ERROR";
			_errorToken.SymbolId = _cfg.GetIdOfSymbol(_errorToken.Symbol);
			_errorToken.Value = _tokenEnum.Current.Value;
			_errorToken.Line = _tokenEnum.Current.Line;
			_errorToken.Column = _tokenEnum.Current.Column;
			_errorToken.Position = _tokenEnum.Current.Position;
			var s = _tokenEnum.Current.Symbol;
			if (!d.TryGetValue(s, out e) && "#EOS"!=s)
			{
				_errorToken.Value += _tokenEnum.Current.Value;
				while (_tokenEnum.MoveNext() && "#EOS" != (s = _tokenEnum.Current.Symbol))
					if (!d.TryGetValue(s, out e))
						_errorToken.Value += _tokenEnum.Current.Value;
					else
						break;
				
			}
		}
		bool _IsHidden(string symbol)
		{
			return _hidden.Contains(symbol);
		}
		bool _IsCollapsed(string symbol)
		{
			return _collapsed.Contains(symbol);
		}
		bool _IsNonTerminal(string symbol)
		{
			return _nonTerminals.Contains(symbol);
		}
	}
}
