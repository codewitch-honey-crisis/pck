using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public enum CfgLL1ConflictKind
	{
		FirstFirst = 0,
		FirstFollows = 1
	}
	public sealed class CfgLL1Conflict : IEquatable<CfgLL1Conflict>, ICloneable
	{
		public CfgLL1Conflict(CfgLL1ConflictKind kind, CfgRule rule1, CfgRule rule2, string symbol)
		{
			Kind = kind;
			Rule1 = rule1 ?? throw new ArgumentNullException("rule1");
			Rule2 = rule2 ?? throw new ArgumentNullException("rule2");
			Symbol = symbol;
		}
		public CfgLL1ConflictKind Kind { get; }
		public CfgRule Rule1 { get; }
		public CfgRule Rule2 { get; }
		public string Symbol { get; }
		public bool Equals(CfgLL1Conflict rhs)
		{
			if (ReferenceEquals(this, rhs)) return true;
			if (ReferenceEquals(null, rhs)) return false;
			if (Equals(Kind, rhs.Kind) && Equals(Symbol, rhs.Symbol))
			{
				return (Equals(Rule1, rhs.Rule1) && Equals(Rule2, rhs.Rule2))
					|| (Equals(Rule1, rhs.Rule2) && Equals(Rule2, rhs.Rule1));

			}
			return false;
		}
		public override bool Equals(object obj)
			=> Equals(obj as CfgLL1Conflict);
		public override int GetHashCode()
		{
			var result = 0;
			result ^= Kind.GetHashCode();
			result ^= Rule1.GetHashCode();
			result ^= Rule2.GetHashCode();
			result ^= Symbol.GetHashCode();
			return result;
		}
		public static bool operator ==(CfgLL1Conflict lhs, CfgLL1Conflict rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(CfgLL1Conflict lhs, CfgLL1Conflict rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return false;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return true;
			return !lhs.Equals(rhs);
		}
		public CfgLL1Conflict Clone()
		{
			return new CfgLL1Conflict(Kind, Rule1, Rule2, Symbol);
		}
		object ICloneable.Clone() => Clone();
	}
}
