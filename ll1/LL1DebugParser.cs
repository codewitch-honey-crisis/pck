using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{

	/// <summary>
	/// An LL(1) parser implemented as a pull-style parser. This one uses strings throughout to make it easier to examine while debugging.
	/// </summary>
	/// <remarks>This interface is similar in use to <see cref="System.Xml.XmlReader"/></remarks>
	public class LL1DebugParser : LL1Parser
	{
		CfgDocument _cfg;
		CfgLL1ParseTable _parseTable;
		ITokenizer _tokenizer;
		IEnumerator<Token> _tokenEnum;
		Token _errorToken;
		Stack<string> _stack;
		HashSet<string> _hidden;
		HashSet<string> _collapsed;
		Dictionary<string,string> _substitute;
		IDictionary<string, int> _symbolIds;
		public override bool IsHidden => _IsHidden(Symbol);
		public override bool IsCollapsed => _IsCollapsed(Symbol);
		public override string Substitute => _Substitute(Symbol);
		public override int SubstituteId => _symbolIds[_Substitute(Symbol)];
		/// <summary>
		/// Indicates the <see cref="LLNodeType"/> at the current position.
		/// </summary>
		public override LLNodeType NodeType {
			get {
				if (null != _errorToken.Symbol)
					return LLNodeType.Error;
				if (_stack.Count > 0)
				{
					var s = _stack.Peek();
					if (s.StartsWith("#END "))
						return LLNodeType.EndNonTerminal;
					if (s == _tokenEnum.Current.Symbol)
						return LLNodeType.Terminal;
					return LLNodeType.NonTerminal;
				}
				try
				{
					if ("#EOS" == _tokenEnum.Current.Symbol)
						return LLNodeType.EndDocument;
				}
				catch { }
				return LLNodeType.Initial;
			}
		}
		public override void Restart(ITokenizer tokenizer)
		{
			Close();
			_tokenizer = null;
			if (null != tokenizer)
			{
				_tokenizer = tokenizer;
				_tokenEnum = tokenizer.GetEnumerator();
			}
		}
		public override void Restart(IEnumerable<char> input)
		{
			Close();
			_tokenizer.Restart(input);
			_tokenEnum = _tokenizer.GetEnumerator();
		}
		public override void Restart()
		{
			if (null == _tokenEnum) throw new ObjectDisposedException(GetType().Name);
			_tokenEnum.Reset();
			_stack.Clear();
		}

		/// <summary>
		/// Indicates the current symbol
		/// </summary>
		public override string Symbol {
			get {
				if (null != _errorToken.Symbol)
					return _errorToken.Symbol;
				if (_stack.Count > 0)
				{
					var s = _stack.Peek();
					if (s.StartsWith("#END "))
						return s.Substring(5);
					return s;
				}
				return null;
			}
		}
		public override ParseAttribute[] GetAttributeSet(int symbolId)
		{
			var syms = _cfg.FillSymbols();
			if (0 < symbolId || syms.Count <= symbolId)
				return null;
			var attrs = _cfg.AttributeSets[syms[symbolId]];
			var result = new ParseAttribute[attrs.Count];
			using (var e = attrs.GetEnumerator()) {
				for (var i = 0; i < result.Length; i++)
				{
					e.MoveNext();
					var attr = e.Current;
					result[i] = new ParseAttribute(attr.Name,attr.Value);
				}
			}
			return result;
		}
		public override object GetAttribute(string name, object @default = null)
		{
			var s = Symbol;
			if (null == s) return @default;
			return _cfg.GetAttribute(s, name, @default);
		}
		public override int SymbolId => (null==Symbol)?-1:_symbolIds[Symbol];
		/// <summary>
		/// Indicates the current line
		/// </summary>
		public override int Line => (null == _errorToken.Symbol) ? _tokenEnum.Current.Line : _errorToken.Line;
		/// <summary>
		/// Indicates the current column
		/// </summary>
		public override int Column => (null == _errorToken.Symbol) ? _tokenEnum.Current.Column : _errorToken.Column;
		/// <summary>
		/// Indicates the current position
		/// </summary>
		public override long Position => (null == _errorToken.Symbol) ? _tokenEnum.Current.Position : _errorToken.Position;
		/// <summary>
		/// Indicates the current value
		/// </summary>
		public override string Value {
			get {
				switch (NodeType)
				{
					case LLNodeType.Error:
						return _errorToken.Value;
					case LLNodeType.Terminal:
						return _tokenEnum.Current.Value;
				}
				return null;
			}
		}
		/// <summary>
		/// Constructs a new instance of the parser
		/// </summary>
		/// <param name="parseTable">The parse table to use</param>
		/// <param name="tokenizer">The tokenizer to use </param>
		/// <param name="startSymbol">The start symbol</param>
		public LL1DebugParser(CfgDocument cfg,
			ITokenizer tokenizer)
		{
			_cfg = cfg;
			_PopulateAttrs();
			_parseTable = cfg.ToLL1ParseTable();
			_stack = new Stack<string>();
			_errorToken.Symbol = null;
			Restart(tokenizer);
		}
		void _PopulateAttrs()
		{
			var syms = _cfg.FillSymbols();
			_symbolIds = new Dictionary<string, int>();
			for (int ic = syms.Count, i = 0; i < ic; ++i)
				_symbolIds.Add(syms[i], i);

			_hidden = new HashSet<string>();
			_collapsed = new HashSet<string>();
			_substitute = new Dictionary<string, string>();
			foreach (var sattr in _cfg.AttributeSets)
			{
				// make sure "hidden" is only applied to terminals.
				var i = sattr.Value.IndexOf("hidden");
				if (!_cfg.IsNonTerminal(sattr.Key) && -1 < i && sattr.Value[i].Value is bool && (bool)sattr.Value[i].Value)
					_hidden.Add(sattr.Key);
				i = sattr.Value.IndexOf("collapsed");
				if (-1 < i && sattr.Value[i].Value is bool && (bool)sattr.Value[i].Value)
					_collapsed.Add(sattr.Key);
				i = sattr.Value.IndexOf("substitute");
				string s;
				if (-1 < i && !string.IsNullOrEmpty(s=sattr.Value[i].Value as string) && _cfg.IsSymbol(s))
					_substitute.Add(sattr.Key,s);

			}

		}

		/// <summary>
		/// Reads and parses the next node from the document
		/// </summary>
		/// <returns>True if there is more to read, otherwise false.</returns>
		public override bool Read()
		{
			var result = _ReadImpl();
			// this is a big part of the "magic" behind clean parse trees
			// all it does is skip "collapsed" and "hidden" nodes in the parse tree
			// meaning any symbol with a "collapsed" or "hidden" attribute
			while ((!ShowCollapsed && result && _IsCollapsed(Symbol)) || (!ShowHidden && result && _IsHidden(Symbol)))
				result = _ReadImpl();


			return result;
		}
		bool _IsHidden(string symbol)
		{
			return _hidden.Contains(symbol);
		}
		bool _IsCollapsed(string symbol)
		{
			return _collapsed.Contains(symbol);
		}
	
		string _Substitute(string symbol)
		{
			if (null == symbol) return null;
			string result;
			if (_substitute.TryGetValue(symbol, out result))
				return result;
			return null;
		}
		bool _ReadImpl()
		{
			var n = NodeType;
			if (LLNodeType.Error == n && "#EOS" == _tokenEnum.Current.Symbol)
			{
				_errorToken.Symbol = null;
				_stack.Clear();
				return true;
			}
			if (LLNodeType.Initial == n)
			{
				_stack.Push(_cfg.StartSymbol);

				if (_tokenEnum.MoveNext() && _IsHidden(_tokenEnum.Current.Symbol))
					_stack.Push(_tokenEnum.Current.Symbol);

				return true;
			}
			_errorToken.Symbol = null; // clear the error status
			if (0 < _stack.Count)
			{
				var sid = _stack.Peek();
				if (sid.StartsWith("#END "))
				{
					_stack.Pop();
					return true;
				}
				if (sid == _tokenEnum.Current.Symbol) // terminal
				{
					_stack.Pop();
					// lex the next token
					//_tokEnum.MoveNext();
					if (_tokenEnum.MoveNext() && _IsHidden(_tokenEnum.Current.Symbol))
						_stack.Push(_tokenEnum.Current.Symbol);


					//_stack.Pop();
					return true;
				}
				// non-terminal
				IDictionary<string, CfgLL1ParseTableEntry> d;

				if (_parseTable.TryGetValue(sid, out d))
				{
					CfgLL1ParseTableEntry re;
					if (d.TryGetValue(_tokenEnum.Current.Symbol, out re))
					{
						_stack.Pop();

						// store the end non-terminal marker for later
						_stack.Push(string.Concat("#END ", sid));

						// push the rule's derivation onto the stack in reverse order
						var ic = re.Rule.Right.Count;
						for (var i = ic - 1; 0 <= i; --i)
						{
							sid = re.Rule.Right[i];
							_stack.Push(sid);
						}
						return true;
					}
					_Panic();
					return true;
				}
				_Panic();
				return true;
			}
			// last symbol must be the end of the input stream or there's a problem
			if ("#EOS" != _tokenEnum.Current.Symbol)
			{
				_Panic();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Does panic-mode error recovery
		/// </summary>
		void _Panic()
		{

			// fill the error token
			_errorToken.Symbol = "#ERROR"; // turn on error reporting
			_errorToken.Value = "";
			_errorToken.Column = _tokenEnum.Current.Column;
			_errorToken.Line = _tokenEnum.Current.Line;
			_errorToken.Position = _tokenEnum.Current.Position;
			string s;
			IDictionary<string, CfgLL1ParseTableEntry> d;
			// check the parse table at the current row
			if (_parseTable.TryGetValue(_stack.Peek(), out d))
			{
				// append the current error text since we know it already doesn't match
				_errorToken.Value += _tokenEnum.Current.Value;
				// if we can move next and we don't have a match on the stack move and append
				while (!d.Keys.Contains(s = _tokenEnum.Current.Symbol)
					&& s != "#EOS" && _tokenEnum.MoveNext())
					if (!d.Keys.Contains(_tokenEnum.Current.Symbol))
						_errorToken.Value += _tokenEnum.Current.Value;

				// TODO: not even 100% sure this works, but it passed the tests so far
				if (_stack.Contains(_tokenEnum.Current.Symbol))
					_errorToken.Value += _tokenEnum.Current.Value;

			}
			else
			{
				do
				{
					s = _tokenEnum.Current.Symbol;
					_errorToken.Value += _tokenEnum.Current.Value;
					if (!_tokenEnum.MoveNext())
						break;

				} while ("#EOS" != s && !_stack.Contains(s));

			}
			while (_stack.Contains(s = _tokenEnum.Current.Symbol) && _stack.Peek() != s)
				_stack.Pop();
		}
		public override void Close()
		{
			if (null != _tokenEnum)
				_tokenEnum.Dispose();
			_stack.Clear();
			_errorToken.Symbol = null;
		}
	}
}
