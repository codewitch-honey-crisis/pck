using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public abstract class RegexCharsetEntry : ICloneable
	{
		protected abstract RegexCharsetEntry CloneImpl();
		object ICloneable.Clone() => CloneImpl();
	}
	public class RegexCharsetClassEntry : RegexCharsetEntry
	{
		public RegexCharsetClassEntry(string name)
		{
			Name = name;
		}
		public RegexCharsetClassEntry() { }

		public string Name { get; set; }
		public override string ToString()
		{
			return string.Concat("[:", Name, ":]");
		}
		protected override RegexCharsetEntry CloneImpl()
			=> Clone();
		public RegexCharsetClassEntry Clone()
		{
			return new RegexCharsetClassEntry(Name);
		}

		#region Value semantics
		public bool Equals(RegexCharsetClassEntry rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Name == rhs.Name;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexCharsetClassEntry);

		public override int GetHashCode()
		{
			if(string.IsNullOrEmpty(Name)) return Name.GetHashCode();
			return 0;
		}

		public static bool operator ==(RegexCharsetClassEntry lhs, RegexCharsetClassEntry rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(RegexCharsetClassEntry lhs, RegexCharsetClassEntry rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
	public class RegexCharsetCharEntry : RegexCharsetEntry,IEquatable<RegexCharsetCharEntry>
	{
		public RegexCharsetCharEntry(char value)
		{
			Value = value;
		}
		public RegexCharsetCharEntry() { }
		public char Value { get; set; }
		public override string ToString()
		{
			return RegexExpression.EscapeRangeChar(Value);
		}
		protected override RegexCharsetEntry CloneImpl()
			=> Clone();
		public RegexCharsetCharEntry Clone()
		{
			return new RegexCharsetCharEntry(Value);
		}

		#region Value semantics
		public bool Equals(RegexCharsetCharEntry rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Value == rhs.Value;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexCharsetCharEntry);

		public override int GetHashCode()
			=> Value.GetHashCode();

		public static bool operator ==(RegexCharsetCharEntry lhs, RegexCharsetCharEntry rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(RegexCharsetCharEntry lhs, RegexCharsetCharEntry rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
	public class RegexCharsetRangeEntry : RegexCharsetEntry
	{
		public RegexCharsetRangeEntry(char first, char last)
		{
			First = first;
			Last = last;
		}
		public RegexCharsetRangeEntry()
		{
		}
		public char First { get; set; }
		public char Last { get; set;  }
		protected override RegexCharsetEntry CloneImpl()
			=> Clone();
		public RegexCharsetRangeEntry Clone()
		{
			return new RegexCharsetRangeEntry(First,Last);
		}
		public override string ToString()
		{
			return string.Concat(RegexExpression.EscapeRangeChar(First), "-", RegexExpression.EscapeRangeChar(Last));
		}
		#region Value semantics
		public bool Equals(RegexCharsetRangeEntry rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return First == rhs.First && Last==rhs.Last;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexCharsetRangeEntry);

		public override int GetHashCode()
			=> First.GetHashCode() ^ Last.GetHashCode();

		public static bool operator ==(RegexCharsetRangeEntry lhs, RegexCharsetRangeEntry rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(RegexCharsetRangeEntry lhs, RegexCharsetRangeEntry rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
