using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class Lalr1DebugParser2 : Lalr1Parser
	{
		CfgDocument _cfg;
		IDictionary<string, string> _substitutions;
		HashSet<string> _hidden;
		HashSet<string> _collapsed;
		ITokenizer _tokenizer;
		CfgLalr1ParseTable _parseTable;
		Token _token;
		string[] _ruleDef;
		IEnumerator<Token> _tokenEnum;
		Stack<int> _stack;
		LRNodeType _nodeType;
		public Lalr1DebugParser2(CfgDocument cfg,ITokenizer tokenizer,CfgLalr1ParseTable parseTable=null)
		{
			_cfg = cfg;
			_parseTable = parseTable ?? cfg.ToLalr1ParseTable();
			_stack = new Stack<int>();
			_Populate();
			Restart(tokenizer);
		}
		void _Populate()
		{
			_substitutions = new Dictionary<string, string>();
			_hidden = new HashSet<string>();
			_collapsed = new HashSet<string>();
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
					var substitute = attrsym.Value[i].Value as string;
					if (!string.IsNullOrEmpty(substitute) && _cfg.IsSymbol(substitute) && substitute != attrsym.Key)
						_substitutions.Add(attrsym.Key, substitute);
				}
			}
		}
		public override LRNodeType NodeType => _nodeType;
		

		public override string Value {
			get {
				switch (NodeType)
				{
					case LRNodeType.Shift:
					case LRNodeType.Error:
						return _token.Value;
				}
				return null;
			}
		}
		public override string Symbol {
			get {
				switch(NodeType)
				{
					case LRNodeType.Error:
					case LRNodeType.Shift:
						return _token.Symbol;
					case LRNodeType.Reduce:
						return _ruleDef[0];
				}
				return null;
			}
		}
		public override int SymbolId => (null != Symbol) ? _cfg.GetIdOfSymbol(Symbol) : -1;

		public override string Substitute {
			get {
				var s = Symbol;
				if (null == s) return s;
				string result;
				if (!_substitutions.TryGetValue(s, out result))
					return s;
				return result;
			}
		}
		public override int SubstituteId => (null != Substitute) ? _cfg.GetIdOfSymbol(Substitute) : -1;

		public override object GetAttribute(string name, object @default = null)
		{
			var s = Symbol;
			if (null == s) return @default;
			return _cfg.GetAttribute(s, name, @default);
		}
		public override KeyValuePair<string, object>[] GetAttributeSet(int symbolId)
		{
			if (0 > symbolId) return null;
			var s = _cfg.GetSymbolOfId(symbolId);
			if (null == s) return null;
			CfgAttributeList attrs;
			if (!_cfg.AttributeSets.TryGetValue(s, out attrs))
				return null;
			var result = new KeyValuePair<string, object>[attrs.Count];
			for(var i = 0;i<result.Length;i++)
				result[i] = new KeyValuePair<string, object>(attrs[i].Name, attrs[i].Value);
			return result;
		}
		public override string[] RuleDefinition => (LRNodeType.Reduce == NodeType) ? _ruleDef : null;

		public override int[] RuleDefinitionIds {
			get {
				if(LRNodeType.Reduce==NodeType)
				{
					var result = new int[_ruleDef.Length];
					for (var i = 0; i < _ruleDef.Length; i++)
						result[i] = _cfg.GetIdOfSymbol(_ruleDef[i]);
					return result;
				}
				return null;
			}
		}
		public override int Line => _token.Line;
		public override int Column => _token.Column;
		public override long Position => _token.Position;

		public override bool IsHidden => _hidden.Contains(Symbol);
		public override bool IsCollapsed => _collapsed.Contains(Symbol);

		public override void Close()
		{
			if(null!=_tokenEnum)
				_tokenEnum.Dispose();
			_tokenEnum = null;
			_stack.Clear();
			_nodeType = LRNodeType.EndDocument;
		}
		public override void Restart()
		{
			if (null == _tokenEnum)
				throw new ObjectDisposedException(GetType().Name);
			_tokenEnum.Reset();
			_stack.Clear();
			_nodeType = LRNodeType.Initial;
		}
		public override void Restart(IEnumerable<char> input)
		{
			if (null == _tokenEnum)
				throw new ObjectDisposedException(GetType().Name);
			Close();
			_tokenizer.Restart(input);
			_tokenEnum = _tokenizer.GetEnumerator();
			_nodeType = LRNodeType.Initial;
		}
		public override void Restart(ITokenizer tokenizer)
		{
			Close();
			if (null != tokenizer)
			{
				_tokenizer = tokenizer;
				_tokenEnum = _tokenizer.GetEnumerator();
				_nodeType = LRNodeType.Initial;
			}
		}
		public override bool Read()
		{
			switch(NodeType)
			{
				case LRNodeType.Error:
					_stack.Clear();
					_nodeType = LRNodeType.EndDocument;
					return false;
				case LRNodeType.Accept:
					_stack.Clear();
					_nodeType = LRNodeType.EndDocument;
					return true;
				case LRNodeType.EndDocument:
					return false;
				case LRNodeType.Initial:
					// push the initial state, advance the token enumerator
					_stack.Push(0);
					// the enumerator should always return at least #EOS
					if (!_tokenEnum.MoveNext())
						throw new Exception("Error in ITokenizer implementation.");
					break;
				
			}
			if (!ShowHidden)
			{
				while (_hidden.Contains(_tokenEnum.Current.Symbol))
					_tokenEnum.MoveNext();
			} else
			{
				if(_hidden.Contains(_tokenEnum.Current.Symbol))
				{
					_token = _tokenEnum.Current;
					_tokenEnum.MoveNext();
					_nodeType = LRNodeType.Shift;
					return true;
				}
			}
			
			(int RuleOrStateId, string Left, string[] Right) trns;
			if(!_parseTable[_stack.Peek()].TryGetValue(_tokenEnum.Current.Symbol,out trns))
			{
				_Panic();
				return true;
			}
			if (null == trns.Right) // shift or accept
			{
				if (-1 != trns.RuleOrStateId) // shift
				{
					_ruleDef = null;
					_token=_tokenEnum.Current;
					_tokenEnum.MoveNext();
					_stack.Push(trns.RuleOrStateId);
					_nodeType = LRNodeType.Shift;
					return true;
				}
				else
				{ // accept 
					_ruleDef = null;
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
			if(0==_stack.Count)
				throw new Exception("Parse error");
			var sb = new StringBuilder();
			var l = _tokenEnum.Current.Line;
			var c = _tokenEnum.Current.Column;
			var p = _tokenEnum.Current.Position;
			var moved = false;
			while (!_IsMatch(_tokenEnum.Current.Symbol) && "#EOS" != _tokenEnum.Current.Symbol)
			{
				moved = true;
				sb.Append(_tokenEnum.Current.Value);
				_tokenEnum.MoveNext();
			}
			if (moved)
			{
				var et = new Token();
				et.Symbol = "#ERROR";
				et.SymbolId = _cfg.GetIdOfSymbol(et.Symbol);
				et.Line = l;
				et.Column = c;
				et.Position = p;
				et.Value = sb.ToString();
				_token = et;
				_nodeType = LRNodeType.Error;
			}

			
		}
		bool _IsMatch(string sym)
		{
			return _parseTable[_stack.Peek()].ContainsKey(sym);
		}
	}
}
