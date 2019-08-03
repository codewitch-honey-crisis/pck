using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class RegexLiteralExpression : RegexExpression, IEquatable<RegexLiteralExpression>
	{
		public override bool IsSingleElement => true;
		public char Value { get; set; } = default(char);
		public static RegexExpression CreateString(string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			RegexExpression result = new RegexLiteralExpression(value[0]);
			for (var i = 1; i < value.Length; i++)
				result = new RegexConcatExpression(result, new RegexLiteralExpression(value[i]));
			return result;
		}
		public RegexLiteralExpression(char value) { Value = value; }
		public RegexLiteralExpression() { }
		public override CharFA<TAccept> ToFA<TAccept>(TAccept accept)
			=>CharFA<TAccept>.Literal(new char[] { Value }, accept);
		
		protected internal override void AppendTo(StringBuilder builder)
			=>AppendEscapedChar(Value,builder);
		protected override RegexExpression CloneImpl()
			=> Clone();
		public RegexLiteralExpression Clone()
		{
			return new RegexLiteralExpression(Value);
		}

		#region Value semantics
		public bool Equals(RegexLiteralExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Value == rhs.Value;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexLiteralExpression);

		public override int GetHashCode()
			=> Value.GetHashCode();

		public static bool operator ==(RegexLiteralExpression lhs, RegexLiteralExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(RegexLiteralExpression lhs, RegexLiteralExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion

	}
}
