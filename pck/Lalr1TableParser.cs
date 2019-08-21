using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class Lalr1TableParser : Lalr1Parser
	{
		int[][][] _parseTable;
		Token _token;
		Token _errorToken;
		string[] _symbols;
		int[] _nodeFlags;
		int[] _substitutions;
		ITokenizer _tokenizer;
		IEnumerator<Token> _tokenEnum;
		LRNodeType _nodeType;
		Stack<int> _stack;
		int[] _ruleDef;
		int _eosId;
		int _errorId;
		KeyValuePair<string, object>[][] _attributeSets;

		public Lalr1TableParser(int[][][] parseTable, string[] symbols,int[] nodeFlags, int[] substitutions,KeyValuePair<string, object>[][] attributeSets,ITokenizer tokenizer)
		{
			_parseTable = parseTable;
			_symbols = symbols;
			_nodeFlags = nodeFlags;
			_substitutions = substitutions;
			_attributeSets = attributeSets;
			
			_eosId = Array.IndexOf(symbols, "#EOS");
			_errorId = Array.IndexOf(symbols, "#ERROR");
			if (0 > _eosId || 0 > _errorId)
				throw new ArgumentException("Error in symbol table", "symbols");

			_stack = new Stack<int>();
			_tokenizer = tokenizer;
			if(null!=tokenizer)
				_tokenEnum = tokenizer.GetEnumerator();
			_nodeType = LRNodeType.Initial;

		}
		public override KeyValuePair<string, object>[] GetAttributeSet(int symbolId)
		{
			if (0 < symbolId || _attributeSets.Length <= symbolId)
				return null;
			return _attributeSets[symbolId];
		}
		public override object GetAttribute(string name, object @default = null)
		{
			var s = SymbolId;
			if (null != _attributeSets && -1 < s)
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
		public override int SymbolId {
			get {
				switch (_nodeType)
				{
					case LRNodeType.Shift:
						return _token.SymbolId;
					case LRNodeType.Reduce:
						return _ruleDef[0];
					case LRNodeType.Error:
						return _errorToken.SymbolId;
				}
				return -1;
			}
		}
		public override string Symbol => (-1 < SymbolId) ? _symbols[SymbolId] : null;
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
				if (LRNodeType.Reduce == _nodeType)
				{
					var result = new string[_ruleDef.Length];
					for (var i = 0; i < _ruleDef.Length; i++)
						result[i] = _symbols[_ruleDef[i]];
					return result;
				}
				return null;
			}
		}
		public override int[] RuleDefinitionIds {
			get {
				if (LRNodeType.Reduce == _nodeType)
					return _ruleDef;
				return null;
			}
		}
		public override bool IsHidden { get { return _IsHidden(SymbolId); } }
		public override bool IsCollapsed { get { return _IsCollapsed(SymbolId); } }
		public override string Substitute => (-1 < SubstituteId) ? _symbols[SubstituteId] : null;
		public override int SubstituteId {
			get {
				var s = SymbolId;
				if (0 > s) return s;
				return _substitutions[s];
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
				_stack.Clear();
				return true;
			}
			else if (LRNodeType.EndDocument == _nodeType)
				return false;
			else if (LRNodeType.Error == _nodeType)
			{
				_nodeType = LRNodeType.EndDocument;
				_stack.Clear();
				return true;
			}
			if (LRNodeType.Error != _nodeType)
			{
				if (!ShowHidden)
				{
					while (_IsHidden(_tokenEnum.Current.SymbolId))
						_tokenEnum.MoveNext();
				}
				else if (_IsHidden(_tokenEnum.Current.SymbolId))
				{
					_token = _tokenEnum.Current;
					_nodeType = LRNodeType.Shift;
					_tokenEnum.MoveNext();
					return true;
				}
			}
			if (0 < _stack.Count)
			{
				var entry = _parseTable[_stack.Peek()];
				//(int RuleOrStateId, int Left, int[] Right) trns;
				if (_errorId == _tokenEnum.Current.SymbolId)
				{
					_Panic();
					return true;
				}
				int[] trns = entry[_tokenEnum.Current.SymbolId];
				if (null == trns)
				{
					_Panic();
					return true;
				}
				if (1 == trns.Length) // shift or accept
				{
					if (-1 != trns[0]) // shift
					{
						_nodeType = LRNodeType.Shift;
						_token = _tokenEnum.Current;
						_tokenEnum.MoveNext();
						_stack.Push(trns[0]);
						return true;
					}
					else
					{ // accept 
					  //throw if _tok is not $ (end)
						if (_eosId != _tokenEnum.Current.SymbolId)
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
					_ruleDef = new int[trns.Length - 1];
					for (var i = 1; i < trns.Length; i++)
						_ruleDef[i - 1] = trns[i];
					for (int i = 2; i < trns.Length; ++i)
						_stack.Pop();

					// There is a new number at the top of the stack. 
					// This number is our temporary state. Get the symbol 
					// from the left-hand side of the rule #. Treat it as 
					// the next input token in the GOTO table (and place 
					// the matching state at the top of the set stack).
					_stack.Push(_parseTable[_stack.Peek()][trns[1]][0]);
					_nodeType = LRNodeType.Reduce;
					return true;
				}
			} else
			{
				// if we already encountered an error
				// return false in this case, since the
				// stack is empty there's nothing to do
				var cont = LRNodeType.Error != _nodeType;
				_Panic();
				return cont;
			}

		}
		public override void Close()
		{
			if (null != _tokenEnum)
			{
				_tokenEnum.Dispose();
				_tokenEnum = null;
			}
			_nodeType = LRNodeType.EndDocument;
			_stack.Clear();
		}
		public override void Restart()
		{
			if (null == _tokenEnum) throw new ObjectDisposedException(GetType().Name);
			_tokenEnum.Reset();
			_stack.Clear();
			_nodeType = LRNodeType.Initial;
		}
		public override void Restart(ITokenizer tokenizer)
		{
			Close();
			_tokenizer = null;
			if (null != tokenizer)
			{
				_tokenizer = tokenizer;
				_tokenEnum = tokenizer.GetEnumerator();
				_nodeType = LRNodeType.Initial;
			}
		}
		public override void Restart(IEnumerable<char> input)
		{
			Close();
			_tokenizer.Restart(input);
			_tokenEnum = _tokenizer.GetEnumerator();
			_nodeType = LRNodeType.Initial;
		}
		void _Panic()
		{
			// This is primitive. Should see if the Dragon Book has something better
			_nodeType = LRNodeType.Error;
			int[] e;
			_errorToken.Symbol = "#ERROR";
			_errorToken.SymbolId = _errorId;
			_errorToken.Value = _tokenEnum.Current.Value;
			_errorToken.Line = _tokenEnum.Current.Line;
			_errorToken.Column = _tokenEnum.Current.Column;
			_errorToken.Position = _tokenEnum.Current.Position;
			var s = _tokenEnum.Current.SymbolId;
			if (0 == _stack.Count)
				return;
			var state = _stack.Peek();
			var d = _parseTable[state];
			if (_errorId!=s && null!=(e=d[s]) && _eosId != s)
			{
				_errorToken.Value += _tokenEnum.Current.Value;
				while (_tokenEnum.MoveNext() && _eosId != (s = _tokenEnum.Current.SymbolId))
					if (null!=(e=d[s]))
						_errorToken.Value += _tokenEnum.Current.Value;
					else
						break;
			} else 
			{
				//_errorToken.Value += _tokenEnum.Current.Value;
				_tokenEnum.MoveNext();
			}
		}

		bool _IsHidden(int symbolId)
		{
			if (0 > symbolId)
				return false;
			return 2 == (_nodeFlags[symbolId] & 2);
		}
		bool _IsCollapsed(int symbolId)
		{
			if (0 > symbolId)
				return false;
			return 1 == (_nodeFlags[symbolId] & 1);
		}
	}
}
