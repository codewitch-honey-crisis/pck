﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public static class Lalr1
	{
		public static Lalr1ParseTable ToLalr1ParseTable(this CfgDocument cfg)
		{
			var start = cfg.GetAugmentedStartId(cfg.StartSymbol);
			var pda = _ToLrfa(cfg);
			var trnsCfg = _ToLRTransitionGrammar(cfg,pda);
			var closure = new List<FA<string, ICollection<LRItem>>>();
			var lalrclosure = new Lalr1ParseTable();

			var itemSets = new List<ICollection<LRItem>>();

			pda.FillClosure(closure);
			var i = 0;
			foreach (var p in closure)
			{
				itemSets.Add(p.AcceptSymbol);
				lalrclosure.Add(new Dictionary<string, (int RuleOrStateId, string Left, string[] Right)>());
				++i;
			}
			i = 0;
			foreach (var p in closure)
			{
				foreach (var trn in p.Transitions)
				{
					var idx = closure.IndexOf(trn.Value);
					lalrclosure[i].Add(
						trn.Key,
						(idx, null, null)
						);
				}
				foreach (var item in p.AcceptSymbol)
				{
					if (Equals(item.Rule.Left, start) && item.RightIndex == item.Rule.Right.Count)
					{
						lalrclosure[i].Add(
							"#EOS",
							(-1, null, null));
						break;
					}
				}
				++i;
			}
			// work on our reductions now
			var map = new Dictionary<CfgRule, ICollection<string>>(_TransitionMergeRuleComparer.Default);
			var follows = trnsCfg.FillFollows();
			var rtbl = new List<IDictionary<object, CfgRule>>();
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
			foreach (var me in map)
			{
				var rule = me.Key;
				var lr = _LrtSymbol.Parse(rule.Right[rule.Right.Count - 1]);
				var left = _LrtSymbol.Parse(rule.Left).Id;
				var right = new List<string>();
				foreach (var s in rule.Right)
					right.Add(_LrtSymbol.Parse(s).Id);
				var newRule = new CfgRule(left, right);
				if (!Equals(left, start))
					foreach (var f in me.Value)
					{
						// build the rule data
						var rr = new string[newRule.Right.Count];
						for (var ri = 0; ri < rr.Length; ri++)
							rr[ri] = newRule.Right[ri];

						var iid = _LrtSymbol.Parse(f).Id;
						(int RuleOrStateId, string Left, string[] Right) tuple;
						var rid = cfg.Rules.IndexOf(newRule);
						if (0 > rid)
							System.Diagnostics.Debugger.Break(); // couldn't find our rule

						// this gets rid of duplicate entries which seem to crop up in the table
						// TODO: I think this is a shift reduce conflict if it debugger-breaks
						if (lalrclosure[lr.To].TryGetValue(iid, out tuple))
						{
							if (rid != tuple.RuleOrStateId)
								System.Diagnostics.Debugger.Break(); // possible shift-reduce conflice?
						}
						else
						{
							lalrclosure[lr.To].Add(_LrtSymbol.Parse(f).Id,
								(rid, newRule.Left, rr));
						}
					}
			}
			return lalrclosure;
		}
		static ICollection<LRItem> _FillLRMove(this CfgDocument cfg,IEnumerable<LRItem> itemSet, object input, ICollection<LRItem> result = null)
		{
			if (null == result)
				result = new List<LRItem>();
			foreach (var item in itemSet)
			{
				var next = item.RightIndex < item.Rule.Right.Count ? item.Rule.Right[item.RightIndex] : null;
				if (item.RightIndex < item.Rule.Right.Count)
				{
					if (Equals(next, input))
					{
						var lri = new LRItem(item.Rule, item.RightIndex + 1);
						if (!result.Contains(lri))
							result.Add(lri);
					}
				}
			}
			_FillLRClosureInPlace(cfg,result);
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
		static void _FillLRClosureInPlace(CfgDocument cfg,ICollection<LRItem> result)
		{
			var done = false;
			while (!done)
			{
				done = true;
				var l = result.ToArray();
				for (var i = 0; i < l.Length; ++i)
				{
					var item = l[i];
					var next = item.RightIndex < item.Rule.Right.Count ? item.Rule.Right[item.RightIndex] : null;
					if (item.RightIndex < item.Rule.Right.Count)
					{
						if (cfg.EnumNonTerminals().Contains(next))
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
		static FA<string, ICollection<LRItem>> _ToLrfa(CfgDocument cfg)
		{
			// TODO: this takes a long time sometimes
			var map = new Dictionary<ICollection<LRItem>, FA<string, ICollection<LRItem>>>(_LRItemSetComparer.Default);
			// create an augmented grammar - add rule {start} -> [[StartId]] 
			var start = new CfgRule(cfg.GetAugmentedStartId(cfg.StartSymbol), new string[] { cfg.StartSymbol });
			var cl = new List<LRItem>();
			cl.Add(new LRItem(start, 0));
			_FillLRClosureInPlace(cfg,cl);
			var lrfa = new FA<string, ICollection<LRItem>>(true,cl);
			map.Add(cl, lrfa);
			var done = false;
			while (!done)
			{
				done = true;
				var arr = map.Keys.ToArray();
				foreach (var itemSet in new List<ICollection<LRItem>>(map.Keys))
				{
					foreach (var item in itemSet)
					{
						var next = item.RightIndex < item.Rule.Right.Count ? item.Rule.Right[item.RightIndex] : null;
						if (item.RightIndex < item.Rule.Right.Count)
						{
							var n = _FillLRMove(cfg,itemSet, next);
							if (!_ContainsItemSet(map.Keys, n))
							{
								done = false;
								var npda = new FA<string, ICollection<LRItem>>(true, n);
								map.Add(n, npda);
							}
							map[itemSet].Transitions[next] = map[n];
						}
					}
				}
			}


			return lrfa;
		}
		static CfgDocument _ToLRTransitionGrammar(CfgDocument cfg,FA<string, ICollection<LRItem>> pda = null)
		{
			var result = new CfgDocument();
			if (null == pda)
				pda = _ToLrfa(cfg);
			var closure = new List<FA<string, ICollection<LRItem>>>();
			var itemSets = new List<ICollection<LRItem>>();
			pda.FillClosure(closure);
			foreach (var p in closure)
				itemSets.Add(p.AcceptSymbol);

			_LrtSymbol start = null;
			foreach (var p in closure)
			{
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
							FA<string, ICollection<LRItem>> dsts;
							if (p.Transitions.ContainsKey(rule.Left))
							{
								dsts = p.Transitions[rule.Left] as FA<string, ICollection<LRItem>>;
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
								var pt = pc.Transitions[sym] as FA<string, ICollection<LRItem>>;
								int s2 = itemSets.IndexOf(pt.AcceptSymbol, _LRItemSetComparer.Default);
								_LrtSymbol n = new _LrtSymbol(s1, sym, s2);
								right.Add(n.ToString());
								pc = pt;
							}
							result.Rules.Add(new CfgRule(left.ToString(), right));
						}
					}
				}
			}
			result.StartSymbol = start.ToString();
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
				{
					var found = false;
					foreach (var yy in y)
					{
						if (xx.Equals(yy))
						{
							found = true;
							break;
						}
					}
					if (!found)
						return false;
				}
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
