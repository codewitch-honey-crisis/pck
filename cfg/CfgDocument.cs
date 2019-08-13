using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pck
{
	public class CfgDocument : IEquatable<CfgDocument>, ICloneable
	{
		HashSet<string> _ntCache = null;
		HashSet<string> _tCache = null;
		HashSet<string> _sCache = null;

		public void ClearCache()
		{
			_ntCache = null;
			_tCache = null;
			_sCache = null;
		}
		public void RebuildCache()
		{
			_ntCache = new HashSet<string>(EnumNonTerminals());
			_tCache = new HashSet<string>(EnumTerminals());
			_sCache = new HashSet<string>(EnumSymbols());
		}
		public IDictionary<string, CfgAttributeList> AttributeSets { get; } = new ListDictionary<string, CfgAttributeList>();
		public IList<CfgRule> Rules { get; } = new List<CfgRule>();

		/// <summary>
		/// The start symbol. If not set, the first non-terminal is used.
		/// </summary>
		public string StartSymbol {
			get {
				foreach (var sattr in AttributeSets)
				{
					var i = sattr.Value.IndexOf("start");
					if (-1 < i && (sattr.Value[i].Value is bool) && (bool)sattr.Value[i].Value)
						return sattr.Key;
				}
				if (0 < Rules.Count)
					return Rules[0].Left;
				return null;
			}
			set {
				foreach (var sattr in AttributeSets)
				{
					var i = sattr.Value.IndexOf("start");
					if (-1 < i && (sattr.Value[i].Value is bool) && (bool)sattr.Value[i].Value)
						sattr.Value.RemoveAt(i);
				}
				if (null != value)
				{
					if (!IsSymbol(value))
						throw new KeyNotFoundException("The specified symbol does not exist");
					foreach(var sattrs in AttributeSets)
					{
						var i =sattrs.Value.IndexOf("start");
						if(-1<i && (sattrs.Value[i].Value is bool) && (bool)sattrs.Value[i].Value)
							sattrs.Value.RemoveAt(i);
						if(sattrs.Key==value)
						{
							i = sattrs.Value.IndexOf("start");
							if (-1 < i)
								sattrs.Value[i].Value=true;
							else 
								sattrs.Value.Add(new CfgAttribute("start", true));
						}
					}
				}
			}
		}
		public void InternSymbols()
		{
			var syms = FillSymbols();
			for(int ic=syms.Count,i=0;i<ic;++i)
				string.Intern(syms[i]);
		}
		public bool IsDirectlyLeftRecursive {
			get {
				for (int ic = Rules.Count, i = 0; i < ic; ++i)
					if (Rules[i].IsDirectlyLeftRecursive)
						return true;
				return false;
			}
		}
		#region Symbols
		public IEnumerable<string> EnumNonTerminals()
		{
			var seen = new HashSet<string>();
			var ic = Rules.Count;
			for(var i=0;i<ic;++i)
			{
				var s = Rules[i].Left;
				if (seen.Add(s))
					yield return s;
			}
		}
		public IList<string> FillNonTerminals(IList<string> result=null)
		{
			if (null == result)
				result = new List<string>();
			var ic = Rules.Count;
			for (var i = 0; i < ic; ++i)
			{
				var s = Rules[i].Left;
				if (!result.Contains(s))
					result.Add(s);
			}
			return result;
		}
		public IEnumerable<string> EnumSymbols()
		{
			var seen = new HashSet<string>();
			var ic = Rules.Count;
			for (var i = 0; i < ic; ++i)
			{
				var s = Rules[i].Left;
				if (seen.Add(s))
					yield return s;
			}
			for (var i = 0; i < ic; ++i)
			{
				var right = Rules[i].Right;
				for(int jc=right.Count,j=0;j<jc;++j)
				{
					var s = right[j];
					if (seen.Add(s))
						yield return s;
				}
			}
			foreach (var attrs in AttributeSets)
				if (seen.Add(attrs.Key))
					yield return attrs.Key;

			yield return "#EOS";
			yield return "#ERROR";
		}
		public IList<string> FillSymbols(IList<string> result = null)
		{
			if (null == result)
				result = new List<string>();
			var seen = new HashSet<string>();
			var ic = Rules.Count;
			for (var i = 0; i < ic; ++i)
			{
				var s = Rules[i].Left;
				if (seen.Add(s))
					if (!result.Contains(s))
						result.Add(s);
			}
			for (var i = 0; i < ic; ++i)
			{
				var right = Rules[i].Right;
				for (int jc = right.Count, j = 0; j < jc; ++j)
				{
					var s = right[j];
					if (seen.Add(s))
						if (!result.Contains(s))
							result.Add(s);
				}
			}
			foreach(var attrs in AttributeSets)
				if(seen.Add(attrs.Key))
					if (!result.Contains(attrs.Key))
						result.Add(attrs.Key);
				
			if (!result.Contains("#EOS"))
				result.Add("#EOS");
			if (!result.Contains("#ERROR"))
				result.Add("#ERROR");
			return result;
		}
		public IEnumerable<string> EnumTerminals()
		{
			var seen = new HashSet<string>();
			var ic = Rules.Count;
			for (var i = 0; i < ic; ++i)
				seen.Add(Rules[i].Left);
			for (var i = 0; i < ic; ++i)
			{
				var right = Rules[i].Right;
				for (int jc = right.Count, j = 0; j < jc; ++j)
				{
					var s = right[j];
					if (seen.Add(s))
						yield return s;
				}
			}
			foreach (var attrs in AttributeSets)
				if (seen.Add(attrs.Key))
					yield return attrs.Key;
			yield return "#EOS";
			yield return "#ERROR";
		}
		public IList<string> FillTerminals(IList<string> result = null)
		{
			if (null == result)
				result = new List<string>();
			var seen = new HashSet<string>();
			var ic = Rules.Count;
			for (var i = 0; i < ic; ++i)
				seen.Add(Rules[i].Left);
			for (var i = 0; i < ic; ++i)
			{
				var right = Rules[i].Right;
				for (int jc = right.Count, j = 0; j < jc; ++j)
				{
					var s = right[j];
					if (seen.Add(s))
						if (!result.Contains(s))
							result.Add(s);
				}
			}
			foreach (var attrs in AttributeSets)
				if (seen.Add(attrs.Key))
					if (!result.Contains(attrs.Key))
						result.Add(attrs.Key);

			if (!result.Contains("#EOS"))
				result.Add("#EOS");
			if (!result.Contains("#ERROR"))
				result.Add("#ERROR");
			return result;
		}
		public int GetIdOfSymbol(string symbol)
		{
			var i = 0;
			foreach (var sym in EnumSymbols())
			{
				if (sym == symbol)
					return i;
				++i;
			}
			return -1;
		}
		public string GetSymbolOfId(int id)
		{
			var i = 0;
			foreach (var sym in EnumSymbols())
			{
				if (id == i)
					return sym;
				++i;
			}
			return null;
		}
		#endregion

		#region First/Follows/Predict
		IDictionary<string, ICollection<string>> _FillFirstsNT(IDictionary<string, ICollection<string>> result = null)
		{
			if (null == result)
				result = new Dictionary<string, ICollection<string>>();
			// first add the terminals to the result
			foreach (var t in EnumTerminals())
			{
				var l = new List<string>();
				l.Add(t);
				result.Add(t, l);
			}
			// now for each rule, find every first right hand side and add it to the rule's left non-terminal result
			for (int ic = Rules.Count, i = 0; i < ic; ++i)
			{
				var rule = Rules[i];
				ICollection<string> col;
				if (!result.TryGetValue(rule.Left, out col))
				{
					col = new HashSet<string>();
					result.Add(rule.Left, col);
				}
				if (!rule.IsNil)
				{
					var e = rule.Right[0];
					if (!col.Contains(e))
						col.Add(e);
				}
				else
				{
					// when it's nil, we represent that with a null
					if (!col.Contains(null))
						col.Add(null);
				}
			}

			return result;
		}
		/// <summary>
		/// Computes the predict table, which contains a collection of terminals and associated rules for each non-terminal.
		/// The terminals represent the terminals that will first appear in the non-terminal.
		/// </summary>
		/// <param name="result">The predict table</param>
		/// <returns>The result</returns>
		public IDictionary<string, ICollection<string>> FillFirsts(IDictionary<string, ICollection<string>> result = null)
		{
			if (null == result)
				result = new Dictionary<string, ICollection<string>>();
			_FillFirstsNT(result);
			// finally, for each non-terminal N we still have in the firsts, resolve FIRSTS(N)
			var done = false;
			while (!done)
			{
				done = true;
				foreach (var kvp in result)
				{
					foreach (var item in new List<string>(kvp.Value))
					{
						if (IsNonTerminal(item))
						{
							done = false;
							kvp.Value.Remove(item);
							foreach (var f in result[item])
								kvp.Value.Add(f);
						}
					}
				}
			}

			return result;
		}
		IDictionary<string, ICollection<(CfgRule Rule, string Symbol)>> _FillPredictNT(IDictionary<string, ICollection<(CfgRule Rule, string Symbol)>> result = null)
		{
			if (null == result)
				result = new Dictionary<string, ICollection<(CfgRule Rule, string Symbol)>>();
			// first add the terminals to the result
			foreach (var t in EnumTerminals())
			{
				var l = new List<(CfgRule Rule, string Symbol)>();
				l.Add((null, t));
				result.Add(t, l);
			}
			// now for each rule, find every first right hand side and add it to the rule's left non-terminal result
			for (int ic = Rules.Count, i = 0; i < ic; ++i)
			{
				var rule = Rules[i];
				ICollection<(CfgRule Rule, string Symbol)> col;
				if (!result.TryGetValue(rule.Left, out col))
				{
					col = new HashSet<(CfgRule Rule, string Symbol)>();
					result.Add(rule.Left, col);
				}
				if (!rule.IsNil)
				{
					var e = (rule, rule.Right[0]);
					if (!col.Contains(e))
						col.Add(e);
				}
				else
				{
					// when it's nil, we represent that with a null
					(CfgRule Rule, string Symbol) e = (rule, null);
					if (!col.Contains(e))
						col.Add(e);
				}
			}
			return result;
		}
		/// <summary>
		/// Computes the predict table, which contains a collection of terminals and associated rules for each non-terminal.
		/// The terminals represent the terminals that will first appear in the non-terminal.
		/// </summary>
		/// <param name="result">The predict table</param>
		/// <returns>The result</returns>
		IDictionary<string, ICollection<(CfgRule Rule, string Symbol)>> _FillPredict2(IDictionary<string, ICollection<(CfgRule Rule, string Symbol)>> result = null)
		{
			if (null == result)
				result = new Dictionary<string, ICollection<(CfgRule Rule, string Symbol)>>();
			_FillPredictNT(result);
			try
			{


				// finally, for each non-terminal N we still have in the firsts, resolve FIRSTS(N)
				var done = false;
				while (!done)
				{
					done = true;
					foreach (var kvp in result)
					{
						foreach (var item in new List<(CfgRule Rule, string Symbol)>(kvp.Value))
						{
							if (IsNonTerminal(item.Symbol))
							{
								done = false;
								kvp.Value.Remove(item);
								foreach (var f in result[item.Symbol])
									kvp.Value.Add((item.Rule, f.Symbol));
							}
						}
					}
				}
			}
			catch (InvalidOperationException)
			{
				throw new CfgException("This operation cannot be performed because the grammar is left recursive.");
			}
			return result;
		}
		public IDictionary<string, ICollection<(CfgRule Rule, string Symbol)>> FillPredict(IDictionary<string, ICollection<(CfgRule Rule, string Symbol)>> result = null)
		{
			if (null == result)
				result = new Dictionary<string, ICollection<(CfgRule Rule, string Symbol)>>();
			var predictNT = _FillPredictNT();
			
			// finally, for each non-terminal N we still have in the firsts, resolve FIRSTS(N)
			foreach (var kvp in predictNT)
			{
				var col = new HashSet<(CfgRule Rule, string Symbol)>();
				foreach (var item in kvp.Value)
				{
					var res = new List<string>();
					_ResolvePredict(item.Symbol, res, predictNT, new HashSet<string>());
					foreach (var r in res)
						col.Add((item.Rule, r));
					
				}
				result.Add(kvp.Key, col);
			}
			return result;
		}

		void _ResolvePredict(string symbol,ICollection<string> result,IDictionary<string, ICollection<(CfgRule Rule, string Symbol)>> predictNT,HashSet<string> seen)
		{
			if (seen.Add(symbol))
			{
				if (null != symbol)
				{
					foreach (var p in predictNT[symbol])
					{
						if (!IsNonTerminal(p.Symbol))
						{
							if (!result.Contains(p.Symbol))
								result.Add(p.Symbol);
						}
						else
						{
							_ResolvePredict(p.Symbol, result, predictNT, seen);
						}
					}
				}
				else if (!result.Contains(null))
					result.Add(null);
			}
		}
		public IDictionary<string, ICollection<string>> FillFollows(IDictionary<string, ICollection<string>> result = null)
		{
			if (null == result)
				result = new Dictionary<string, ICollection<string>>();

			var	followsNT = new Dictionary<string, ICollection<string>>();

			// we'll need the predict table
			//Console.Error.WriteLine("Computing predict...");
			var predict = FillPredict();
			//Console.Error.WriteLine("Done!");
			var ss = StartSymbol;
			for (int ic = Rules.Count, i = -1; i < ic; ++i)
			{
				// here we augment the grammar by inserting START' -> START #EOS as the first rule.
				var rule = (-1 < i) ? Rules[i] : new CfgRule(GetAugmentedStartId(ss), ss, "#EOS");
				ICollection<string> col;

				// traverse the rule looking for symbols that follow non-terminals
				if (!rule.IsNil)
				{
					var jc = rule.Right.Count;
					for (var j = 1; j < jc; ++j)
					{
						var r = rule.Right[j];
						var target = rule.Right[j - 1];
						if (IsNonTerminal(target))
						{
							if (!followsNT.TryGetValue(target, out col))
							{
								col = new HashSet<string>();
								followsNT.Add(target, col);
							}
							foreach (var f in predict[r])
							{
								if (null != f.Symbol)
								{
									if (!col.Contains(f.Symbol))
										col.Add(f.Symbol);
								}
								else
								{
									if (!col.Contains(f.Rule.Left))
										col.Add(f.Rule.Left);
								}
							}
						}
					}

					var rr = rule.Right[jc - 1];
					if (IsNonTerminal(rr))
					{
						if (!followsNT.TryGetValue(rr, out col))
						{
							col = new HashSet<string>();
							followsNT.Add(rr, col);
						}
						if (!col.Contains(rule.Left))
							col.Add(rule.Left);
					}
				}
				else // rule is nil
				{
					// what follows is the rule's left nonterminal itself
					if (!followsNT.TryGetValue(rule.Left, out col))
					{
						col = new HashSet<string>();
						followsNT.Add(rule.Left, col);
					}

					if (!col.Contains(rule.Left))
						col.Add(rule.Left);
				}
			}
			//Console.Error.WriteLine("Resolving follows...");
			// below we look for any non-terminals in the follows result and replace them
			// with their follows, so for example if N appeared, N would be replaced with 
			// the result of FOLLOW(N)
			
			foreach (var nt in EnumNonTerminals())
			{
				var col = new HashSet<string>();
				var res = new List<string>();
				_ResolveFollows(nt, col, followsNT, new HashSet<string>());
				result.Add(nt, col);
			}
			//Console.Error.WriteLine("Done!");
			return result;
	
		}
		void _ResolveFollows(string symbol, ICollection<string> result, IDictionary<string, ICollection<string>> followsNT, HashSet<string> seen)
		{
			if (seen.Add(symbol))
			{
				if (IsNonTerminal(symbol))
				{
					foreach (var f in followsNT[symbol])
					{
						if (!IsNonTerminal(f))
						{
							if (!result.Contains(f))
								result.Add(f);
						}
						else
							_ResolveFollows(f, result, followsNT, seen);

					}
				}
			}
		}
		public string GetAugmentedStartId(string s)
		{
			var i = 2;
			var ss = string.Concat(s, "start");
			while (IsSymbol(ss))
			{
				ss = string.Concat(s, "start", i.ToString());
				++i;
			}
			return ss;
		}
		#endregion
		public IList<CfgRule> FillNonTerminalRules(string symbol, IList<CfgRule> result = null)
		{
			if (null == result)
				result = new List<CfgRule>();
			for (int ic = Rules.Count, i = 0; i < ic; ++i)
			{
				var rule = Rules[i];
				if (rule.Left == symbol)
					result.Add(rule);
			}
			return result;
		}
		public IList<CfgRule> FillReferencesToSymbol(string symbol, IList<CfgRule> result = null)
		{
			if (null == result)
				result = new List<CfgRule>();
			for (int ic = Rules.Count, i = 0; i < ic; ++i)
			{
				var rule = Rules[i];
				if (rule.Right.Contains(symbol))
					if (!result.Contains(rule))
						result.Add(rule);
			}
			return result;
		}
		public IList<string> FillClosure(string symbol, IList<string> result = null)
		{
			if (null == result)
				result = new List<string>();
			else if (result.Contains(symbol))
				return result;
			var rules = FillNonTerminalRules(symbol);
			if (0 != rules.Count) // non-terminal
			{
				if (!result.Contains(symbol))
					result.Add(symbol);
				for (int ic = rules.Count, i = 0; i < ic; ++i)
				{
					var rule = rules[i];
					for (int jc = rule.Right.Count, j = 0; j < jc; ++j)
						FillClosure(rule.Right[j], result);
				}
			}
			else if (IsSymbol(symbol))
			{
				// make sure this is a terminal
				if (!result.Contains(symbol))
					result.Add(symbol);
			}
			return result;
		}
		public object GetAttribute(string symbol,string name, object @default = null)
		{
			CfgAttributeList l;
			if(AttributeSets.TryGetValue(symbol,out l))
			{
				var i = l.IndexOf(name);
				if(-1<i)
					return l[i].Value;
			}
			return @default;
		}
		public bool IsNonTerminal(string symbol)
		{
			if (null != _ntCache) return _ntCache.Contains(symbol);
			for(int ic=Rules.Count,i=0;i<ic;++i)
				if (Rules[i].Left == symbol)
					return true;
			return false;
		}
		public bool IsSymbol(string symbol)
		{
			if (null != _sCache) return _sCache.Contains(symbol);
			for (int ic = Rules.Count, i = 0; i < ic; ++i)
			{
				var rule = Rules[i];
				if (rule.Left == symbol)
					return true;
				for(int jc=rule.Right.Count,j=0;j<jc;++j)
				{
					if (symbol == rule.Right[j])
						return true;
				}
			}
			return false;
		}
		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach(var attrSet in AttributeSets)
			{
				if (0 < attrSet.Value.Count)
				{
					sb.Append(string.Concat(attrSet.Key, ":"));
					var delim = "";
					for (int jc = attrSet.Value.Count, j = 0; j < jc; ++j)
					{
						sb.Append(string.Concat(delim, attrSet.Value[j].ToString()));
						delim = ", ";
					}
					sb.AppendLine();
				}
			}
			for (int ic=Rules.Count,i=0;i<ic;++i)
				sb.AppendLine(Rules[i].ToString());
			return sb.ToString();
		}

		public CfgDocument Clone()
		{
			var result = new CfgDocument();
			foreach (var attrs in AttributeSets)
			{
				var d = new CfgAttributeList();
				result.AttributeSets.Add(attrs.Key, d);
				foreach (var attr in attrs.Value)
					d.Add(attr.Clone());
			}
			var ic = Rules.Count;
			for (var i = 0; i < ic; ++i)
				result.Rules.Add(Rules[i].Clone());
			return result;
		}
		object ICloneable.Clone()
			=> Clone();

		public static CfgDocument Parse(IEnumerable<char> @string)
			=> Parse(ParseContext.Create(@string));
		public static CfgDocument ReadFrom(TextReader reader)
			=> Parse(ParseContext.CreateFrom(reader));
		public static CfgDocument ReadFrom(string filename)
		{
			using (var sr = File.OpenText(filename))
				return ReadFrom(sr);
		}
		internal static CfgDocument Parse(ParseContext pc)
		{
			var result = new CfgDocument();
			while(-1!=pc.Current)
			{
				var line = pc.Line;
				var column = pc.Column;
				var position = pc.Position;
				//CfgNode.SkipCommentsAndWhitespace(pc);
				while ('\n' == pc.Current)
				{
					pc.Advance();
					CfgNode.SkipCommentsAndWhitespace(pc);
				}
				var id = CfgNode.ParseIdentifier(pc);
				CfgNode.SkipCommentsAndWhitespace(pc);
				
				pc.Expecting(':', '-', '=');
				if (':' == pc.Current) // attribute set
				{
					pc.Advance();
					var d = new CfgAttributeList();
					while (-1 != pc.Current && '\n' != pc.Current)
					{
						var attr = CfgAttribute.Parse(pc);
						d.Add(attr);

						CfgNode.SkipCommentsAndWhitespace(pc);
						pc.Expecting('\n', ',', -1);
						if (',' == pc.Current)
							pc.Advance();
					}
					result.AttributeSets.Add(id, d);
					CfgNode.SkipCommentsAndWhitespace(pc);
				} else if ('-' == pc.Current)
				{
					pc.Advance();
					pc.Expecting('>');
					pc.Advance();
					CfgNode.SkipCommentsAndWhitespace(pc);
					var rule = new CfgRule(id);
					rule.SetLocation(line, column, position);
					while (-1 != pc.Current && '\n' != pc.Current)
					{
						id = CfgNode.ParseIdentifier(pc);
						rule.Right.Add(id);
						CfgNode.SkipCommentsAndWhitespace(pc);
					}
					result.Rules.Add(rule);
				} else if ('=' == pc.Current)
				{
					pc.TrySkipUntil('\n', true);
				}
				if ('\n' == pc.Current)
					pc.Advance();
				CfgNode.SkipCommentsAndWhitespace(pc);
				
			}
			return result;
		}
		#region Value semantics
		public bool Equals(CfgDocument rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			else if (ReferenceEquals(rhs, null)) return false;
			if (AttributeSets.Count != rhs.AttributeSets.Count)
				return false;
			foreach (var attrs in AttributeSets)
			{
				CfgAttributeList d;
				if (!rhs.AttributeSets.TryGetValue(attrs.Key, out d))
				{
					if (d.Count != attrs.Value.Count)
						return false;
					foreach (var attr in attrs.Value)
					{
						var i = d.IndexOf(attr.Name);
						if (0>i || !Equals(d[i].Value, attr.Value))
							return false;
					}
				}
			}
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			var lc = Rules.Count;
			var rc = rhs.Rules.Count;
			if (lc != rc) return false;
			for (var i = 0; i < lc; ++i)
				if (Rules[i] != rhs.Rules[i])
					return false;
			return true;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as CfgDocument);

		public override int GetHashCode()
		{
			var result = 0;
			foreach (var attrs in AttributeSets)
			{
				if(null!=attrs.Key)
					result ^= attrs.Key.GetHashCode();
				foreach (var attr in attrs.Value)
				{
					if(null!=attr.Name)
						result ^= attr.Name.GetHashCode();
					if (null != attr.Value)
						result ^= attr.Value.GetHashCode();
				}
			}
			for (int ic = Rules.Count, i = 0; i < ic; ++i)
				if (null != Rules[i])
					result ^= Rules[i].GetHashCode();

			return result;
		}
		public static bool operator ==(CfgDocument lhs, CfgDocument rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(CfgDocument lhs, CfgDocument rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
