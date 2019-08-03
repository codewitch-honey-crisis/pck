using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	class RegexCharsetExpression : RegexExpression, IEquatable<RegexCharsetExpression>
	{
		public IList<RegexCharsetEntry> Entries { get; } = new List<RegexCharsetEntry>();
		public RegexCharsetExpression(IEnumerable<RegexCharsetEntry> entries,bool hasNegatedRanges=false)
		{
			foreach (var entry in entries)
				Entries.Add(entry);
			HasNegatedRanges = hasNegatedRanges;
		}
		public RegexCharsetExpression() { }
		public override CharFA<TAccept> ToFA<TAccept>(TAccept accept)
		{
			var ranges = new List<CharRange>();
			for(int ic=Entries.Count,i=0;i<ic;++i)
			{
				var entry = Entries[i];
				var crc = entry as RegexCharsetCharEntry;
				if(null!=crc)
					ranges.Add(new CharRange(crc.Value,crc.Value));
				var crr = entry as RegexCharsetRangeEntry;
				if (null != crr)
					ranges.Add(new CharRange(crr.First, crr.Last));
				var crcl = entry as RegexCharsetClassEntry;
				if (null != crcl)
					ranges.AddRange(CharFA<TAccept>.CharacterClasses[crcl.Name]);
			}
			if (HasNegatedRanges)
				return CharFA<TAccept>.Set(CharRange.NotRanges(ranges), accept);
			return CharFA<TAccept>.Set(ranges,accept);
		}
		public bool HasNegatedRanges { get; set; } = false;
		public override bool IsSingleElement => true;
		protected internal override void AppendTo(StringBuilder sb)
		{
			sb.Append('[');
			if (HasNegatedRanges)
				sb.Append('^');
			for (int ic = Entries.Count, i = 0; i < ic; ++i)
				sb.Append(Entries[i]);
			sb.Append(']');
		}

		protected override RegexExpression CloneImpl()
			=> Clone();
		public RegexCharsetExpression Clone()
		{
			return new RegexCharsetExpression(Entries, HasNegatedRanges);
		}
		#region Value semantics
		public bool Equals(RegexCharsetExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			if(HasNegatedRanges==rhs.HasNegatedRanges && rhs.Entries.Count==Entries.Count)
			{
				for (int ic = Entries.Count, i = 0; i < ic; ++i)
					if (Entries[i] != rhs.Entries[i])
						return false;
				return true;
			}
			return false;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexCharsetExpression);

		public override int GetHashCode()
		{
			var result = HasNegatedRanges.GetHashCode();
			for (int ic = Entries.Count, i = 0; i < ic; ++i)
				result ^= Entries[i].GetHashCode();
			return result;	
		}

		public static bool operator ==(RegexCharsetExpression lhs, RegexCharsetExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(RegexCharsetExpression lhs, RegexCharsetExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
