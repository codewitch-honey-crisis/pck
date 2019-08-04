using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class DebugLalrParser
	{
		CfgDocument _cfg;
		IEnumerator<Token> _tokenEnum;
		LalrParseTable _parseTable;
		HashSet<string> _hiddenTerminals;
		Stack<int> _stack = new Stack<int>();

		Lalr1ParserNodeType _nodeType;
		int _ruleId;
		string[] _ruleDef;

		string _symbol;
		Token _token;
		//Token _errorToken;
		public DebugLalrParser(CfgDocument cfg, IEnumerable<Token> tokenizer, LalrParseTable parseTable = null)
		{
			_cfg = cfg;
			_PopulateHiddenTerminals();
			_parseTable = parseTable ?? _cfg.ToLalrParseTable();
			Restart(tokenizer);

		}
		public string Symbol {
			get {
				if (Lalr1ParserNodeType.Shift == _nodeType)
					return _token.Symbol;
				return null;
			}
		}
		public string Value { get { return _token.Value; } }
		void _PopulateHiddenTerminals()
		{

			_hiddenTerminals = new HashSet<string>();
			
			foreach (var attrsym in _cfg.AttributeSets)
			{
				var i = attrsym.Value.IndexOf("hidden");
				if (-1<i)
				{
					var hidden = attrsym.Value[i].Value;
					if ((hidden is bool) && ((bool)hidden))
						_hiddenTerminals.Add(attrsym.Key);
				}
			}
		}
		public void Restart()
		{
			_tokenEnum.Reset();
			_stack.Clear();
			_nodeType = Lalr1ParserNodeType.Initial;
		}
		public void Restart(IEnumerable<Token> tokenizer)
		{
			Close();
			_stack.Clear();
			_tokenEnum = tokenizer.GetEnumerator();
			_nodeType = Lalr1ParserNodeType.Initial;
		}
		public Lalr1ParserNodeType NodeType => _nodeType;
		public int RuleId {
			get {
				if (Lalr1ParserNodeType.Reduce == _nodeType)
					return _ruleId;
				return -1;
			}
		}
		public string Rule {
			get {
				if (Lalr1ParserNodeType.Reduce == _nodeType)
				{
					var sb = new StringBuilder();
					sb.Append(_ruleDef[0]);
					sb.Append(" ->");
					for (var i = 1; i < _ruleDef.Length; ++i)
					{
						sb.Append(' ');
						sb.Append(_ruleDef[i]);
					}
					return sb.ToString();
				}
				return null;
			}
		}
		public string[] RuleDefinition {
			get {
				if (Lalr1ParserNodeType.Reduce == _nodeType)
				{
					return _ruleDef;
				}
				return null;
			}
		}


		public void Close()
		{
			if (null != _tokenEnum)
				_tokenEnum.Dispose();
			_tokenEnum = null;
		}
		void _Panic()
		{
			throw new Exception("Parse error");
		}
		public bool Read()
		{
			if (Lalr1ParserNodeType.EndDocument == _nodeType)
				return false;
			if (0 == _stack.Count && Lalr1ParserNodeType.Initial == _nodeType)
			{
				// state 0 is our start state
				_stack.Push(0);
				
				while (_tokenEnum.MoveNext() && _hiddenTerminals.Contains(_tokenEnum.Current.Symbol)) ;
			}
			else if ("#EOS" == _tokenEnum.Current.Symbol && 0 == _stack.Count)
			{
				_nodeType = Lalr1ParserNodeType.EndDocument;
				return true;
			}
			var entry = _parseTable[_stack.Peek()];
			(int RuleOrStateId, string Left, string[] Right) trns;
			if (entry.TryGetValue(_tokenEnum.Current.Symbol, out trns))
			{
				if (null == trns.Right) // shift or accept
				{
					if (-1 != trns.RuleOrStateId) // shift
					{
						_nodeType = Lalr1ParserNodeType.Shift;
						_token = _tokenEnum.Current;
						while (_tokenEnum.MoveNext() && _hiddenTerminals.Contains(_tokenEnum.Current.Symbol)) ;
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

						_nodeType = Lalr1ParserNodeType.Accept;
						_stack.Clear();
						return true;
					}
				}
				else // reduce
				{
					_ruleId = trns.RuleOrStateId;
					_ruleDef = new string[trns.Right.Length + 1];
					_ruleDef[0] = trns.Left;
					trns.Right.CopyTo(_ruleDef, 1);
					_symbol = trns.Left;
					for (int i = 0; i < trns.Right.Length; ++i)
						if (null != trns.Right[i])
							_stack.Pop();

					// There is a new number at the top of the stack. 
					// This number is our temporary state. Get the symbol 
					// from the left-hand side of the rule #. Treat it as 
					// the next input token in the GOTO table (and place 
					// the matching state at the top of the set stack).
					_stack.Push(_parseTable[_stack.Peek()][trns.Left].RuleOrStateId);
					_nodeType = Lalr1ParserNodeType.Reduce;
					return true;
				}
			}
			else
			{
				// parse error
				_Panic();
				return true;
			}

		}
		public ParseNode ParseReductions()
		{
			ParseNode p;
			var rs = new Stack<ParseNode>();
			while (Read())
			{
				switch (NodeType)
				{
					case Lalr1ParserNodeType.Shift:
						p = new ParseNode();
						//p.SetLocationInfo(Line, Column, Position);
						p.Symbol = Symbol;
						p.Value = Value;
						rs.Push(p);
						break;
					case Lalr1ParserNodeType.Reduce:
						var d = new List<ParseNode>();
						p = new ParseNode();
						p.Symbol = RuleDefinition[0];
						for (var i = 1; i < RuleDefinition.Length; ++i)
							p.Children.Insert(0, rs.Pop());
						rs.Push(p);

						break;

				}
			}
			return rs.Peek();
		}
	}
}
