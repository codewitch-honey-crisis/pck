using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{

	/// <summary>
	/// An LL(1) parser implemented as a pull-style parser.
	/// </summary>
	/// <remarks>This interface is similar in use to <see cref="System.Xml.XmlReader"/></remarks>
	public class LL1TableParser : LL1Parser
	{
		int[][][] _parseTable;
		IEnumerator<Token> _tokenEnum;
		Token _errorToken;
		Stack<int> _stack;
		int[] _nodeFlags; // for hidden and collapsed
		string[] _symbolTable;
		KeyValuePair<string, object>[][] _attributeSets;
		int[] _initCfg;
		int _eosSymbolId;
		int _errorSymbolId;
		public override bool IsHidden => _IsHidden(SymbolId);
		public override bool IsCollapsed => _IsCollapsed(SymbolId);
		public override object GetAttribute(string name, object @default = null)
		{
			var s = SymbolId;
			if (null!=_attributeSets && -1 < s)
			{
				var attrs = _attributeSets[s];
				if (null != attrs)
				{
					for (var i = 0; i < attrs.Length; i++)
					{
						var attr = attrs[i];
						if (attr.Key == name)
							return attr.Value;
					}
				}
			}
			return @default;
		}
		/// <summary>
		/// Indicates the <see cref="LL1ParserNodeType"/> at the current position.
		/// </summary>
		public override LL1ParserNodeType NodeType {
			get {
				if (null != _errorToken.Symbol)
					return LL1ParserNodeType.Error;
				if (_stack.Count > 0)
				{
					var s = _stack.Peek();
					if (0>s)
						return LL1ParserNodeType.EndNonTerminal;
					if (s == _tokenEnum.Current.SymbolId)
						return LL1ParserNodeType.Terminal;
					return LL1ParserNodeType.NonTerminal;
				}
				try
				{
					if (_eosSymbolId== _tokenEnum.Current.SymbolId)
						return LL1ParserNodeType.EndDocument;
				}
				catch { }
				return LL1ParserNodeType.Initial;
			}
		}
		public override void Restart(IEnumerable<Token> tokenizer)
		{
			Close();
			if(null!=tokenizer)
				_tokenEnum = tokenizer.GetEnumerator();
		}
		public override void Restart()
		{
			if (null == _tokenEnum) throw new ObjectDisposedException(GetType().Name);
			_tokenEnum.Reset();
			_errorToken.Symbol = null;
			_stack.Clear();
		}
		/// <summary>
		/// Indicates the current symbol
		/// </summary>
		public override string Symbol {
			get {
				var id = SymbolId;
				if (-1 != id)
					return _symbolTable[id];
				return null;
			}
		}
		public override int SymbolId {
			get {
				if (null != _errorToken.Symbol)
					return _errorToken.SymbolId;
				if (_stack.Count > 0)
				{
					var s = _stack.Peek();
					if (0 > s)
						return ~s;
					return s;
				}
				return -1;
				
			}
		}
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
					case LL1ParserNodeType.Error:
						return _errorToken.Value;
					case LL1ParserNodeType.Terminal:
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
		public LL1TableParser(
			int[][][] parseTable,
			int[] initCfg,
			string[] symbolTable,
			int[] nodeFlags,
			KeyValuePair<string,object>[][] attributeSets,
			IEnumerable<Token> tokenizer)
		{
			_parseTable = parseTable;
			_initCfg = initCfg;
			_symbolTable = symbolTable;
			_nodeFlags = nodeFlags;
			_attributeSets = attributeSets;
			_stack = new Stack<int>();
			_errorToken.Symbol = null;
			_eosSymbolId = Array.IndexOf(_symbolTable,"#EOS");
			_errorSymbolId = Array.IndexOf(_symbolTable,"#ERROR");
			// we do actually handle this error since it's rough to track otherwise.
			if (0 > _eosSymbolId || 0 > _errorSymbolId)
				throw new ArgumentException("The symbol table is invalid.", "symbolTable");
			Restart(tokenizer);
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
			while ((!ShowCollapsed && result && _IsCollapsed(SymbolId)) || 
				(!ShowHidden && result && _IsHidden(SymbolId)))
				result = _ReadImpl();
			return result;
		}
		bool _IsHidden(int symbolId)
		{
			if (0 > symbolId)
				return false;
			return 2==(_nodeFlags[symbolId] & 2);
		}
		bool _IsCollapsed(int symbolId)
		{
			if (0 > symbolId)
				return false;
			return 1 == (_nodeFlags[symbolId] & 1);
		}
		bool _ReadImpl()
		{
			var n = NodeType;
			if (LL1ParserNodeType.Error == n && _eosSymbolId == _tokenEnum.Current.SymbolId)
			{
				_errorToken.Symbol = null;
				_stack.Clear();
				return true;
			}
			if (LL1ParserNodeType.Initial == n)
			{
				_stack.Push(_initCfg[0]);

				if (_tokenEnum.MoveNext() && _IsHidden(_tokenEnum.Current.SymbolId))
					_stack.Push(_tokenEnum.Current.SymbolId);

				return true;
			}
			_errorToken.Symbol = null; // clear the error status
			if (0 < _stack.Count)
			{
				var sid = _stack.Peek();
				if (0>sid)
				{
					_stack.Pop();
					return true;
				}
				if (sid == _tokenEnum.Current.SymbolId) // terminal
				{
					_stack.Pop();
					// lex the next token
					if (_tokenEnum.MoveNext() && _IsHidden(_tokenEnum.Current.SymbolId))
						_stack.Push(_tokenEnum.Current.SymbolId);
					return true;
				}
				// non-terminal
				var ntc = _initCfg[1];
				if(0>sid||ntc<=sid)
				{
					_Panic();
					return true;
				}
				var row = _parseTable[sid];
				if(null==row)
				{
					_Panic();
					return true;
				}
				var tid = _tokenEnum.Current.SymbolId - ntc;
				if(0>tid||row.Length<=tid||null==row[tid])
				{
					_Panic();
					return true;
				}
				_stack.Pop();
				var rule = row[tid];
				// store the end non-terminal marker for later
				_stack.Push(~sid);

				// push the rule's derivation onto the stack in reverse order
				for (var i = rule.Length - 1; 0 <= i; --i)
					_stack.Push(rule[i]);

				return true;
					
			}
			// last symbol must be the end of the input stream or there's a problem
			if (_eosSymbolId != _tokenEnum.Current.SymbolId)
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
			_errorToken.SymbolId = _errorSymbolId;
			_errorToken.Value = "";
			_errorToken.Column = _tokenEnum.Current.Column;
			_errorToken.Line = _tokenEnum.Current.Line;
			_errorToken.Position = _tokenEnum.Current.Position;
			int s = _stack.Peek();
			int[][] row;
			if(-1<s && s<_initCfg[1] && null!=(row=_parseTable[s]))
			{
				_errorToken.Value += _tokenEnum.Current.Value;
				while (-1<(s= _tokenEnum.Current.SymbolId - _initCfg[1]) && 
					s<row.Length && 
					null==row[s] &&
					_eosSymbolId!=_tokenEnum.Current.SymbolId &&
					_tokenEnum.MoveNext())
				{
					s = _tokenEnum.Current.SymbolId - _initCfg[1];
					if (0 > s || row.Length <= s || null == row[s])
					{
						_errorToken.Value += _tokenEnum.Current.Value;
					}
				}
				if (_stack.Contains(_tokenEnum.Current.SymbolId))
				{
					// TODO: not even 100% sure this works, but it passed the tests so far
					_errorToken.Value += _tokenEnum.Current.Value;
				}
			}
			else
			{
				do
				{
					s = _tokenEnum.Current.SymbolId;
					Console.Error.WriteLine(_tokenEnum.Current.Value);
					_errorToken.Value += _tokenEnum.Current.Value;
					if (!_tokenEnum.MoveNext())
						break;

				} while (_eosSymbolId != s && !_stack.Contains(s));

			}
			while (_stack.Contains((s = _tokenEnum.Current.SymbolId)) && _stack.Peek() != s)
				_stack.Pop();
			//Console.Error.WriteLine("ERROR: " + _errorToken.Value);
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
