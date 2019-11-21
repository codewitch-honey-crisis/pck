using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pck
{
	using Lrfa = FA<string, ICollection<Lalr1.LRItem>>;
	public static class Lalr1
	{
		public static Lalr1Parser ToLalr1Parser(this CfgDocument cfg,ITokenizer tokenizer,IProgress<CfgLalr1Progress> progress=null)
		{
			Lalr1Parser parser;
			var res = TryToLalr1Parser(cfg, out parser,tokenizer,progress);
			CfgException.ThrowIfErrors(res);
			return parser;
		}
		public static IList<CfgMessage> TryToLalr1Parser(this CfgDocument cfg, out Lalr1Parser parser,ITokenizer tokenizer = null,IProgress<CfgLalr1Progress> progress=null)
		{
			CfgLalr1ParseTable parseTable;
			var result = TryToLalr1ParseTable(cfg, progress, out parseTable);
			var syms = new List<string>();
			cfg.FillSymbols(syms);
			var nodeFlags = new int[syms.Count];
			for (var i = 0; i < nodeFlags.Length; i++)
			{
				var o = cfg.GetAttribute(syms[i], "hidden", false);
				if (o is bool && (bool)o)
					nodeFlags[i] |= 2;
				o = cfg.GetAttribute(syms[i], "collapsed", false);
				if (o is bool && (bool)o)
					nodeFlags[i] |= 1;
			}
			var substitutions = new int[syms.Count];
			for (var i = 0; i < substitutions.Length; i++)
			{
				var s = cfg.GetAttribute(syms[i], "substitute", null) as string;
				if (!string.IsNullOrEmpty(s) && cfg.IsSymbol(s) && s != syms[i])
					substitutions[i] = cfg.GetIdOfSymbol(s);
				else
					substitutions[i] = -1;
			}
			var attrSets = new ParseAttribute[syms.Count][];
			for (var i = 0; i < attrSets.Length; i++)
			{
				CfgAttributeList attrs;
				if (cfg.AttributeSets.TryGetValue(syms[i], out attrs))
				{
					attrSets[i] = new ParseAttribute[attrs.Count];
					var j = 0;
					foreach (var attr in attrs)
					{
						attrSets[i][j] = new ParseAttribute(attr.Name, attr.Value);
						++j;
					}
				}
				else
					attrSets[i] = null;
			}
			var ss = cfg.GetIdOfSymbol(cfg.StartSymbol);
			var ntc = cfg.FillNonTerminals().Count;
			parser = new Lalr1TableParser(parseTable.ToArray(syms), syms.ToArray(), nodeFlags, substitutions, attrSets, tokenizer);
			return result;

		}
		public static CfgLalr1ParseTable ToLalr1ParseTable(this CfgDocument cfg,IProgress<CfgLalr1Progress> progress=null)
		{
			CfgLalr1ParseTable result = null;
			var msgs = TryToLalr1ParseTable(cfg, progress, out result);
			CfgException.ThrowIfErrors(msgs);
			return result;
		}
		public static IList<CfgMessage> TryToLalr1ParseTable(this CfgDocument cfg, out CfgLalr1ParseTable parseTable)
			=> TryToLalr1ParseTable(cfg, null, out parseTable);
		public static IList<CfgMessage> TryToLalr1ParseTable(this CfgDocument cfg, IProgress<CfgLalr1Progress> progress,out CfgLalr1ParseTable parseTable)
		{
			var result = new List<CfgMessage>();
			var start = cfg.GetAugmentedStartId(cfg.StartSymbol);
			var lrfa = _ToLrfa(cfg,progress);
			var trnsCfg = _ToLRTransitionGrammar(cfg,lrfa,progress);
			trnsCfg.RebuildCache();
			var closure = new List<Lrfa>();
			parseTable = new CfgLalr1ParseTable();

			var itemSets = new List<ICollection<LRItem>>();

			lrfa.FillClosure(closure);
			var i = 0;
			foreach (var p in closure)
			{

				itemSets.Add(p.AcceptSymbol);
				parseTable.Add(new Dictionary<string, (int RuleOrStateId, string Left, string[] Right)>());
				++i;
			}
			i = 0;
			foreach (var p in closure)
			{
				foreach (var trn in p.Transitions)
				{
					var idx = closure.IndexOf(trn.Value);
					parseTable[i].Add(
						trn.Key,
						(idx, null, null)
						);
				}
				foreach (var item in p.AcceptSymbol)
				{
					if (Equals(item.Rule.Left, start) && item.RightIndex == item.Rule.Right.Count)
					{
						parseTable[i].Add(
							"#EOS",
							(-1, null, null));
						break;
					}
				}
				++i;
			}
			var follows = trnsCfg.FillFollows();
			// work on our reductions now
			var map = new Dictionary<CfgRule, ICollection<string>>(_TransitionMergeRuleComparer.Default);
			foreach (var rule in trnsCfg.Rules)
			{
				ICollection<string> f;
				if (!map.TryGetValue(rule, out f))
					map.Add(rule, follows[rule.Left]);
				else
					foreach (var o in follows[rule.Left])
						if (!f.Contains(o))
							f.Add(o);
			}
			var j = 0;
			foreach (var mapEntry in map)
			{
				if (null != progress)
					progress.Report(new CfgLalr1Progress(CfgLalr1Status.ComputingReductions, j));
				var rule = mapEntry.Key;
				var lr = _LrtSymbol.Parse(rule.Right[rule.Right.Count - 1]);
				var left = _LrtSymbol.Parse(rule.Left).Id;
				var right = new List<string>();
				foreach (var s in rule.Right)
					right.Add(_LrtSymbol.Parse(s).Id);
				var newRule = new CfgRule(left, right);
				if (!Equals(left, start))
					foreach (var f in mapEntry.Value)
					{
						// build the rule data
						var rr = new string[newRule.Right.Count];
						for (var ri = 0; ri < rr.Length; ri++)
							rr[ri] = newRule.Right[ri];

						var iid = _LrtSymbol.Parse(f).Id;
						(int RuleOrStateId, string Left, string[] Right) tuple;
						var rid = cfg.Rules.IndexOf(newRule);

						var newTuple = (RuleOrStateId: rid, Left: newRule.Left, Right: rr);
						// this gets rid of duplicate entries which crop up in the table
						if (!parseTable[lr.To].TryGetValue(iid, out tuple))
						{
							parseTable[lr.To].Add(_LrtSymbol.Parse(f).Id,
								newTuple);
						} else
						{
							// TODO: Verify this - may need the dragon book
							if(null==tuple.Right)
							{
								var nr = cfg.Rules[rid];
								var msg = new CfgMessage(ErrorLevel.Warning, -1, string.Format("Shift-Reduce conflict on rule {0}, token {1}", nr, iid), nr.Line, nr.Column, nr.Position,cfg.Filename);
								if(!result.Contains(msg))
									result.Add(msg);
							} else
							{
								if (rid != newTuple.RuleOrStateId)
								{
									var nr = cfg.Rules[rid];
									var msg = new CfgMessage(ErrorLevel.Error, -1, string.Format("Reduce-Reduce conflict on rule {0}, token {1}", nr, iid), nr.Line, nr.Column, nr.Position,cfg.Filename);
									if (!result.Contains(msg))
										result.Add(msg);
								}
							}
						}
					}
				++j;
			}
			return result;
		}
		static ICollection<LRItem> _FillLRMove(this CfgDocument cfg,IEnumerable<LRItem> itemSet, object input,IProgress<CfgLalr1Progress> progress,ICollection<LRItem> result = null)
		{
			if (null == result)
				result = new HashSet<LRItem>();
			int i = 0;
			foreach (var item in itemSet)
			{
				if (null != progress)
					progress.Report(new CfgLalr1Progress(CfgLalr1Status.ComputingMove, i));
				var next = item.RightIndex < item.Rule.Right.Count ? item.Rule.Right[item.RightIndex] : null;
				if (item.RightIndex < item.Rule.Right.Count)
				{
					if (Equals(next, input))
					{
						var lri = new LRItem(item.Rule, item.RightIndex + 1);
						result.Add(lri);
					}
				}
				++i;
			}
			_FillLRClosureInPlace(cfg, progress,result);
			return result;
		}
		static bool _ContainsItemSet(IEnumerable<ICollection<LRItem>> sets, ICollection<LRItem> set)
		{
			foreach (var lris in sets)
			{
				if (lris.Count == set.Count)
				{
					var found = true;
					foreach (var l in lris)
					{
						if (!set.Contains(l))
						{
							found = false;
							break;
						}
					}
					if (found)
					{
						return true;
					}
				}
			}
			return false;
		}
		static void _FillLRClosureInPlace(CfgDocument cfg,IProgress<CfgLalr1Progress> progress, ICollection<LRItem> result)
		{
			var done = false;
			while (!done)
			{
				done = true;
				var l = result.ToArray();
				for (var i = 0; i < l.Length; i++)
				{
					if(null!=progress)
						progress.Report(new CfgLalr1Progress(CfgLalr1Status.ComputingClosure, i));
					var item = l[i];
					var next = item.RightIndex < item.Rule.Right.Count ? item.Rule.Right[item.RightIndex] : null;
					if (item.RightIndex < item.Rule.Right.Count)
					{
						if (cfg.IsNonTerminal(next))
						{
							for (int jc = cfg.Rules.Count, j = 0; j < jc; ++j)
							{
								var r = cfg.Rules[j];
								if (r.Left == next)
								{
									var lri = new LRItem(r, 0);
									if (!result.Contains(lri))
									{
										done = false;
										result.Add(lri);
									}
								}
							}
						}
					}
				}
			}
		}
		static Lrfa _ToLrfa(CfgDocument cfg,IProgress<CfgLalr1Progress> progress)
		{
			if(null!=progress)
				progress.Report(new CfgLalr1Progress(CfgLalr1Status.ComputingStates, 0));
			// TODO: this takes a long time sometimes
			var map = new Dictionary<ICollection<LRItem>, Lrfa>(_LRItemSetComparer.Default);
			// create an augmented grammar - add rule {start} -> [[StartId]] 
			var start = new CfgRule(cfg.GetAugmentedStartId(cfg.StartSymbol), new string[] { cfg.StartSymbol });
			var cl = new HashSet<LRItem>();
			cl.Add(new LRItem(start, 0));
			_FillLRClosureInPlace(cfg,progress,cl);
			var lrfa = new Lrfa(true,cl);
			var items = cl.Count;
			map.Add(cl, lrfa);
			var done = false;
			var oc = 0;
			while (!done)
			{
				done = true;
				var arr = map.Keys.ToArray();
				for(var i = 0;i<arr.Length;++i)
				{
					var itemSet = arr[i];
					foreach (var item in itemSet)
					{
						var next = item.RightIndex < item.Rule.Right.Count ? item.Rule.Right[item.RightIndex] : null;
						if (item.RightIndex < item.Rule.Right.Count)
						{
							var n = _FillLRMove(cfg,itemSet, next,progress);
							if (!_ContainsItemSet(map.Keys, n))
							{
								// Epstein didn't kill himself
								done = false;
								var npda = new Lrfa(true, n);
								map.Add(n, npda);
								items += n.Count;
								if(null!=progress)
									progress.Report(new CfgLalr1Progress(CfgLalr1Status.ComputingConfigurations, items));
							}
							map[itemSet].Transitions[next] = map[n];	
						}
					}
				}
				if(!done)
				{
					oc = map.Count;
					if(null!=progress)
						progress.Report(new CfgLalr1Progress(CfgLalr1Status.ComputingStates, oc));
				}
			}
			return lrfa;
		}
		static CfgDocument _ToLRTransitionGrammar(CfgDocument cfg,Lrfa lrfa, IProgress<CfgLalr1Progress> progress)
		{
			var result = new CfgDocument();
			var closure = new List<Lrfa>();
			var itemSets = new List<ICollection<LRItem>>();
			lrfa.FillClosure(closure);
			foreach (var p in closure)
				itemSets.Add(p.AcceptSymbol);

			_LrtSymbol start = null;
			int j = 0;
			foreach (var p in closure)
			{
				if (null != progress)
					progress.Report(new CfgLalr1Progress(CfgLalr1Status.CreatingLookaheadGrammar, j));

				int si = itemSets.IndexOf(p.AcceptSymbol, _LRItemSetComparer.Default);

				foreach (var item in p.AcceptSymbol)
				{
					if (0 == item.RightIndex)
					{
						var next = item.RightIndex < item.Rule.Right.Count ? item.Rule.Right[item.RightIndex] : null;
						var rule = item.Rule;
						if (item.RightIndex < item.Rule.Right.Count)
						{
							int dst = -1;
							Lrfa dsts;
							if (p.Transitions.ContainsKey(rule.Left))
							{
								dsts = p.Transitions[rule.Left];
								dst = itemSets.IndexOf(dsts.AcceptSymbol, _LRItemSetComparer.Default);
							}

							_LrtSymbol left = new _LrtSymbol(si, rule.Left, dst);
							if (null == start)
								start = left;
							var right = new List<string>();
							var pc = p;
							foreach (var sym in rule.Right)
							{
								int s1 = itemSets.IndexOf(pc.AcceptSymbol, _LRItemSetComparer.Default);
								var pt = pc.Transitions[sym];
								int s2 = itemSets.IndexOf(pt.AcceptSymbol, _LRItemSetComparer.Default);
								_LrtSymbol n = new _LrtSymbol(s1, sym, s2);
								right.Add(n.ToString());
								pc = pt;
							}
							result.Rules.Add(new CfgRule(left.ToString(), right));
						}
					}
				}
				++j;
			}
			result.StartSymbol = start.ToString();
			return result;
		}
		static IList<CfgMessage> _TryValidateRulesLalr1(CfgDocument cfg, IList<CfgMessage> result)
		{
			if (null == result)
				result = new List<CfgMessage>();
			var ic = cfg.Rules.Count;
			if (0 == ic)
				result.Add(new CfgMessage(ErrorLevel.Error, -1, "Grammar has no rules", 0, 0, 0, cfg.Filename));

			var dups = new HashSet<CfgRule>();
			for (var i = 0; i < ic; ++i)
			{
				var rule = cfg.Rules[i];
				if (rule.Left.IsNullOrEmpty())
					result.Add(new CfgMessage(ErrorLevel.Error, -1, string.Concat("Rule has empty left hand side:", rule.ToString()), rule.Line, rule.Column, rule.Position,cfg.Filename));
				else if ("#ERROR" == rule.Left || "#EOS" == rule.Left)
					result.Add(new CfgMessage(ErrorLevel.Error, -1, string.Concat("Rule has reserved terminal on left hand side: ", rule.ToString()), rule.Line, rule.Column, rule.Position, cfg.Filename));
				for (int jc = rule.Right.Count, j = 0; j > jc; ++j)
					if (rule.Right[j].IsNullOrEmpty())
						result.Add(new CfgMessage(ErrorLevel.Error, -1, string.Concat("Rule has empty symbols on the right hand side:", rule.ToString()), rule.Line, rule.Column, rule.Position, cfg.Filename));
					else if ("#ERROR" == rule.Right[j] || "#EOS" == rule.Right[j])
						result.Add(new CfgMessage(ErrorLevel.Error, -1, string.Concat("Rule has reserved terminal on right hand side:", rule.ToString()), rule.Line, rule.Column, rule.Position, cfg.Filename));

				for (var j = 0; j < ic; ++j)
					if (i != j && cfg.Rules[j] == rule && dups.Add(rule))
						result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat("Duplicate rule:", rule.ToString()), rule.Line, rule.Column, rule.Position, cfg.Filename));

			}
			var closure = cfg.FillClosure(cfg.StartSymbol);
			var syms = cfg.FillSymbols();
			ic = syms.Count;
			for (var i = 0; i < ic; ++i)
			{
				var sym = syms[i];
				if (!closure.Contains(sym))
				{
					var found = false;
					if (!cfg.IsNonTerminal(sym))
						if ("#EOS" == sym || "#ERROR" == sym || (bool)cfg.GetAttribute(sym, "hidden", false))
							found = true;
					if (!found) {
						var rl = cfg.FillNonTerminalRules(sym);
						if (0 < rl.Count)
						{
							var r = rl[0];
							result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat("Unreachable symbol \"", sym, "\""), r.Line, r.Column, r.Position, cfg.Filename));
						}
					}
				}
			}
			// checking for conflicts here is way too time prohibitive for LALR(1) so we skip it
			return result;
		}
		public static IList<CfgMessage> TryValidateLalr1(this CfgDocument cfg, IList<CfgMessage> result = null)
		{
			if (null == result)
				result = new List<CfgMessage>();
			_TryValidateAttributesLalr1(cfg, result);
			_TryValidateRulesLalr1(cfg, result);
			return result;
		}
		static IList<CfgMessage> _TryValidateAttributesLalr1(CfgDocument cfg, IList<CfgMessage> result)
		{
			if (null == result)
				result = new List<CfgMessage>();
			string start = null;
			foreach (var attrs in cfg.AttributeSets)
			{
				if (!cfg.IsSymbol(attrs.Key))
				{
					// hidden rules should never be in the grammar
					// so warnings about them not being in the grammar
					// are suppressed.
					var i = attrs.Value.IndexOf("hidden");
					if (!(-1 < i && attrs.Value[i].Value is bool && ((bool)attrs.Value[i].Value)))
						result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat("Attributes declared on a symbol \"", attrs.Key, "\" that is not in the grammar"), attrs.Value[0].Line, attrs.Value[0].Column, attrs.Value[0].Position, cfg.Filename));
				}
				foreach (var attr in attrs.Value)
				{
					string s;
					var p = string.Concat("On \"", attrs.Key, "\": ");
					switch (attr.Name)
					{
						case "start":
							if (!(attr.Value is bool))
								result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat(p, "start attribute expects a bool value and will be ignored"), attr.Line, attr.Column, attr.Position, cfg.Filename));
							if (null != start)
								result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat(p, "start attribute was already specified on \"", start, "\" and this declaration will be ignored"), attr.Line, attr.Column, attr.Position, cfg.Filename));
							else
								start = attrs.Key;
							continue;
						case "hidden":
							if (!(attr.Value is bool))
								result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat(p, "hidden attribute expects a bool value and will be ignored"), attr.Line, attr.Column, attr.Position, cfg.Filename));
							continue;
						case "terminal":
							if (!(attr.Value is bool))
								result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat(p, "terminal attribute expects a bool value and will be ignored"), attr.Line, attr.Column, attr.Position, cfg.Filename));
							continue;
						case "collapsed":
							if (!(attr.Value is bool))
								result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat(p, "collapse attribute expects a bool value and will be ignored"), attr.Line, attr.Column, attr.Position, cfg.Filename));
							continue;
						case "substitute":
							s = attr.Value as string;
							if (!(attr.Value is string))
								result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat(p, "substitute attribute expects a string value and will be ignored"), attr.Line, attr.Column, attr.Position, cfg.Filename));
							else if (string.IsNullOrEmpty(s))
								result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat(p, "substitute attribute expects a non-empty string value and will be ignored"), attr.Line, attr.Column, attr.Position, cfg.Filename));
							else if (!cfg.IsSymbol(s))
								result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat(p, "substitute attribute expects a symbol reference and will be ignored"), attr.Line, attr.Column, attr.Position, cfg.Filename));
							continue;
						case "blockEnd":
							if (cfg.IsNonTerminal(attrs.Key))
								result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat(p, "blockEnd attribute cannot be specified on a non-terminal and will be ignored"), attr.Line, attr.Column, attr.Position, cfg.Filename));
							else
							{
								s = attr.Value as string;
								if (!(attr.Value is string))
									result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat(p, "blockEnd attribute expects a string value and will be ignored"), attr.Line, attr.Column, attr.Position, cfg.Filename));
								else if (string.IsNullOrEmpty(s))
									result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat(p, "blockEnd attribute expects a non-empty string value and will be ignored"), attr.Line, attr.Column, attr.Position, cfg.Filename));
							}
							continue;
						case "followsConflict":
							s = attr.Value as string;
							switch (s)
							{
								case "error":
								case "first":
								case "last":
									break;
								default:
									result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat(p, "followsError attribute expects \"error\", \"first\", or \"last\" and will revert to \"error\"."), attr.Line, attr.Column, attr.Position, cfg.Filename));
									break;
							}
							continue;
					}
					result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat(p, "Unknown attribute \"", attr.Name, "\" will be ignored"), attr.Line, attr.Column, attr.Position, cfg.Filename));
				}

			}
			if (null == start)
				result.Add(new CfgMessage(ErrorLevel.Warning, -1, string.Concat("start attribute was not specified and the first non-terminal in the grammar (\"", cfg.StartSymbol, "\") will be used"), 0, 0, 0, cfg.Filename));
			return result;
		}
		public struct LRItem : IEquatable<LRItem>, ICloneable
		{
			public LRItem(CfgRule rule, int rightIndex)
			{
				Rule = rule;
				RightIndex = rightIndex;
			}
			public CfgRule Rule { get; set; }
			public int RightIndex { get; set; }

			public override string ToString()
			{
				var sb = new StringBuilder();
				sb.Append(Rule.Left ?? "");
				sb.Append(" ->");
				var ic = Rule.Right.Count;
				for (var i = 0; i < ic; ++i)
				{
					if (i == RightIndex)
						sb.Append(" .");
					else
						sb.Append(" ");
					sb.Append(Rule.Right[i]);
				}
				if (ic == RightIndex)
					sb.Append(".");
				return sb.ToString();
			}
			public bool Equals(LRItem rhs)
			{
				return Equals(Rule, rhs.Rule) && RightIndex == rhs.RightIndex;
			}
			public override bool Equals(object obj)
			{
				if (obj is LRItem)
					return Equals((LRItem)obj);
				return false;
			}
			public override int GetHashCode()
			{
				var result = RightIndex;
				if (null != Rule)
					result ^= Rule.GetHashCode();
				return result;
			}
			public static bool operator ==(LRItem lhs, LRItem rhs)
			{
				return lhs.Equals(rhs);
			}
			public static bool operator !=(LRItem lhs, LRItem rhs)
			{
				return !lhs.Equals(rhs);
			}
			public LRItem Clone()
			{
				return new LRItem(Rule, RightIndex);
			}
			object ICloneable.Clone() => Clone();
		}
		class _LRItemSetComparer : IEqualityComparer<ICollection<LRItem>>
		{
			public static readonly _LRItemSetComparer Default = new _LRItemSetComparer();
			public bool Equals(ICollection<LRItem> x, ICollection<LRItem> y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (null == x || null == y) return false;
				if (x.Count != y.Count)
					return false;
				foreach (var xx in x)
					if (!y.Contains(xx))
						return false;
				
				return true;
			}

			public int GetHashCode(ICollection<LRItem> obj)
			{
				var result = 0;
				if (null == obj) return result;
				foreach (var lri in obj)
					result ^= lri.GetHashCode();
				return result;
			}
		}
	
		sealed class _LrtSymbol : IEquatable<_LrtSymbol>
		{
			public int From;
			public string Id;
			public int To;
			public _LrtSymbol(int from, string id, int to)
			{
				From = from;
				Id = id;
				To = to;
			}
			public static _LrtSymbol Parse(string str)
			{
				var sa = str.Split('|');
				if (1 == sa.Length)
				{
					return new _LrtSymbol(-1, str, -1);
				}
				return new _LrtSymbol(int.Parse(sa[0]), string.Join("|", sa, 1, sa.Length - 2), int.Parse(sa[sa.Length - 1]));
			}
			public override int GetHashCode()
			{
				return From ^ Id.GetHashCode() ^ To;
			}
			public override string ToString()
			{
				return string.Concat(From, "|", Id, "|", To);
			}
			public bool Equals(_LrtSymbol obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				return From == obj.From && To == obj.To && Equals(Id, obj.Id);
			}
			public override bool Equals(object obj)
			{
				if (ReferenceEquals(this, obj)) return true;
				var rhs = obj as _LrtSymbol;
				if (ReferenceEquals(null, rhs)) return false;
				return From == rhs.From && To == rhs.To && Equals(Id, rhs.Id);
			}
			public static bool operator ==(_LrtSymbol lhs, _LrtSymbol rhs)
			{
				if (ReferenceEquals(lhs, rhs)) return true;
				if (ReferenceEquals(null, rhs) ||
					ReferenceEquals(null, lhs))
					return false;

				return lhs.From == rhs.From && lhs.To == rhs.To && Equals(lhs.Id, rhs.Id);
			}
			public static bool operator !=(_LrtSymbol lhs, _LrtSymbol rhs)
			{
				if (ReferenceEquals(lhs, rhs)) return false;
				if (ReferenceEquals(null, rhs) ||
					ReferenceEquals(null, lhs))
					return true;

				return !(lhs.From == rhs.From && lhs.To == rhs.To && Equals(lhs.Id, rhs.Id));
			}
		}
		class _TransitionMergeRuleComparer : IEqualityComparer<CfgRule>
		{
			public static readonly _TransitionMergeRuleComparer Default = new _TransitionMergeRuleComparer();
			public bool Equals(CfgRule x, CfgRule y)
			{
				if (ReferenceEquals(x, y))
					return true;
				if (!_SymEq(_LrtSymbol.Parse(x.Left), _LrtSymbol.Parse(y.Left)))
					return false;
				var c = x.Right.Count;
				if (y.Right.Count != c) return false;

				if (0 == c) return true;
				var ll = _LrtSymbol.Parse(x.Right[c - 1]);
				var lr = _LrtSymbol.Parse(y.Right[c - 1]);
				if (!_SymEq(ll, lr)) return false;
				if (ll.To != lr.To) return false;

				for (int i = 0; i < c - 1; ++i)
					if (!_SymEq(_LrtSymbol.Parse(x.Right[i]), _LrtSymbol.Parse(y.Right[i])))
						return false;
				return true;
			}
			static bool _SymEq(_LrtSymbol x, _LrtSymbol y)
			{
				if (ReferenceEquals(x, y)) return true;
				else if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
					return false;
				var lhs = x;
				if (ReferenceEquals(lhs, null)) return false;
				var rhs = y;
				if (ReferenceEquals(rhs, null)) return false;

				return Equals(lhs.Id, rhs.Id);
			}
			public int GetHashCode(CfgRule obj)
			{
				var lr = _LrtSymbol.Parse(obj.Left);
				var result = lr.Id.GetHashCode();
				foreach (var sym in obj.Right)
				{
					lr = _LrtSymbol.Parse(sym);
					if (null != lr)
						result ^= (null != lr.Id) ? lr.Id.GetHashCode() : 0;
				}
				if (null != lr)
					result ^= lr.To;
				return result;
			}
		}
	}
}
