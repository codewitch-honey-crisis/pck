using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class Lalr1DebugParser : Lalr1Parser
	{
		CfgDocument _cfg;
		CfgLalr1ParseTable _parseTable;
		Token _token;
		Token _errorToken;
		IEnumerator<Token> _tokenEnum;
		HashSet<string> _hidden;
		HashSet<string> _collapsed;
		Dictionary<string, string> _substitute;
		Dictionary<string, int> _symbolIds;
		LRNodeType _nodeType;
		Stack<int> _stack;
		string[] _ruleDef;
		public Lalr1DebugParser(CfgDocument cfg, IEnumerable<Token> tokenizer, CfgLalr1ParseTable parseTable = null)
		{
			_cfg = cfg;
			_PopulateAttrs();
			_stack = new Stack<int>();
			_tokenEnum = tokenizer.GetEnumerator();
			_parseTable = parseTable ?? cfg.ToLalr1ParseTable();
			_nodeType = LRNodeType.Initial;
		}
		public override KeyValuePair<string, object>[] GetAttributeSet(int symbolId)
		{
			var syms = _cfg.FillSymbols();
			if (0 < symbolId || syms.Count <= symbolId)
				return null;
			var attrs = _cfg.AttributeSets[syms[symbolId]];
			var result = new KeyValuePair<string, object>[attrs.Count];
			using (var e = attrs.GetEnumerator())
			{
				for (var i = 0; i < result.Length; i++)
				{
					e.MoveNext();
					var attr = e.Current;
					result[i] = new KeyValuePair<string, object>(attr.Name, attr.Value);
				}
			}
			return result;
		}
		public override object GetAttribute(string name, object @default = null)
		{
			var s = Symbol;
			if (null != s)
				return _cfg.GetAttribute(s, name, @default);
			return @default;
		}
		public override int Line {
			get {
				switch (_nodeType)
				{

					case LRNodeType.Error:
						return _errorToken.Line;
					default:
						return _token.Line;
				}
			}
		}
		public override int Column {
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
		public override long Position {
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
		public override LRNodeType NodeType {
			get { return _nodeType; }
		}
		public override string Symbol {
			get {
				switch (_nodeType)
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
		public override int SymbolId => (null == Symbol) ? -1 : _symbolIds[Symbol];
		public override string Value {
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
		public override string[] RuleDefinition {
			get {
				if(LRNodeType.Reduce==_nodeType)
					return _ruleDef;
				return null;
			}
		}
		public override int[] RuleDefinitionIds {
			get {
				if (LRNodeType.Reduce == _nodeType)
				{
					var result = new int[_ruleDef.Length];
					for (var i = 0; i < _ruleDef.Length; i++)
						result[i] = _symbolIds[_ruleDef[i]];
					return result;
				}
				return null;
			}
		}
		public override bool IsHidden { get { return _IsHidden(Symbol); } }
		public override bool IsCollapsed { get { return _IsCollapsed(Symbol); } }
		public override string Substitute {
			get {
				var result = "";
				if(_substitute.TryGetValue(Symbol, out result))
					return result;
				return null;
			}
		}
		public override int SubstituteId => (null == Substitute) ? -1 : _symbolIds[Substitute];
		
		
		void _PopulateAttrs()
		{
			var syms = _cfg.FillSymbols();
			_symbolIds = new Dictionary<string, int>();
			for (int ic = syms.Count, i = 0; i < ic; ++i)
				_symbolIds.Add(syms[i], i);

			_hidden = new HashSet<string>();
			_collapsed = new HashSet<string>();
			_substitute = new Dictionary<string, string>();
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
				i = attrsym.Value.IndexOf("substitute");
				if (-1 < i)
				{
					var substitute= attrsym.Value[i].Value as string;
					if(!string.IsNullOrEmpty(substitute) && _cfg.IsSymbol(substitute) && substitute!=attrsym.Key)
						_substitute.Add(attrsym.Key,substitute);
				}
			}
		}
			
		public override bool Read()
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
			else if ("#EOS" == _tokenEnum.Current.Symbol && LRNodeType.Error == _nodeType)
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
		public override void Close()
		{
			if (null != _tokenEnum)
			{
				_tokenEnum.Dispose();
				_tokenEnum = null;
			}
			_stack.Clear();
		}
		public override void Restart()
		{
			if (null == _tokenEnum) throw new ObjectDisposedException(GetType().Name);
			_tokenEnum.Reset();
			_stack.Clear();
			_nodeType = LRNodeType.Initial;
		}
		public override void Restart(IEnumerable<Token> tokenizer)
		{
			Close();
			if (null != tokenizer)
				_tokenEnum = tokenizer.GetEnumerator();
		}
		void _Panic()
		{
			// This is primitive. Should see if the Dragon Book has something better
			_nodeType = LRNodeType.Error;
			var state = _stack.Peek();
			var d = _parseTable[state];
			(int RuleOrStateId, string Left, string[] Right) e;
			_errorToken.Symbol = "#ERROR";
			_errorToken.SymbolId = _symbolIds[_errorToken.Symbol];
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
	}
}
