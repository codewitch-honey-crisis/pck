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

		Queue<Token> _tokens;
		string[] _ruleDef;
		LRNodeType _nodeType;
		IEnumerator<Token> _tokenEnum;
		Stack<int> _stack;

		public override LRNodeType NodeType {
			get {
				if(0<_tokens.Count)
				{
					var t = _tokens.Peek();
					return ("#ERROR" == t.Symbol) ? LRNodeType.Error : LRNodeType.Shift;
				}
				if(null!=_ruleDef)
					return LRNodeType.Reduce;
				if (0!=_stack.Count)
					return LRNodeType.EndDocument;
				return LRNodeType.Initial;
			}
		}

		public override string Value {
			get {
				switch (NodeType)
				{
					case LRNodeType.Shift:
					case LRNodeType.Error:
						return _tokens.Peek().Value;
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
						return _tokens.Peek().Symbol;
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
		public override int Line {
			get {
				if (0 < _tokens.Count)
					return _tokens.Peek().Line;
				return 1;
			}
		}
		public override int Column {
			get {
				if (0 < _tokens.Count)
					return _tokens.Peek().Column;
				return 1;
			}
		}
		public override long Position {
			get {
				if (0 < _tokens.Count)
					return _tokens.Peek().Position;
				return 0;
			}
		}
		public override bool IsHidden => _hidden.Contains(Symbol);
		public override bool IsCollapsed => _collapsed.Contains(Symbol);

		public override void Close()
		{
			if(null!=_tokenEnum)
				_tokenEnum.Dispose();
			_tokenEnum = null;
			_stack.Clear();
			_tokens.Clear();
		}
		public override void Restart()
		{
			if (null == _tokenEnum)
				throw new ObjectDisposedException(GetType().Name);
			_tokenEnum.Reset();
			_stack.Clear();
			_tokens.Clear();
		}
		public override void Restart(IEnumerable<char> input)
		{
			if (null == _tokenEnum)
				throw new ObjectDisposedException(GetType().Name);
			Close();
			_tokenizer.Restart(input);
			_tokenEnum = _tokenizer.GetEnumerator();

		}
		public override void Restart(ITokenizer tokenizer)
		{
			Close();
			if (null != _tokenizer)
			{
				_tokenizer = tokenizer;
				_tokenEnum = _tokenizer.GetEnumerator();
				_nodeType =LRNodeType.Initial;
			}
		}
		public override bool Read()
		{
			switch(NodeType)
			{
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
			if(!ShowHidden)
				while (IsHidden)
					_tokens.Dequeue();

			if (0<_tokens.Count)
			{

				return true;
			}
			return false;
		}
	}
}
