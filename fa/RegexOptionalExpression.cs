using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class RegexOptionalExpression : RegexUnaryExpression, IEquatable<RegexOptionalExpression>
	{
		public override bool IsSingleElement => true;
		public RegexOptionalExpression(RegexExpression expression) { Expression = expression; }
		public RegexOptionalExpression() { }
		public override CharFA<TAccept> ToFA<TAccept>(TAccept accept)
			=>null != Expression ? CharFA<TAccept>.Optional(Expression.ToFA(accept),accept) : null;
			
		protected internal override void AppendTo(StringBuilder builder)
		{
			if (null == Expression)
				builder.Append("()?");
			else
			{
				var ise = Expression.IsSingleElement;
				if (!ise)
					builder.Append('(');
				Expression.AppendTo(builder);
				if (!ise)
					builder.Append(")?");
				else
					builder.Append('?');
			}
		}
		protected override RegexExpression CloneImpl()
			=> Clone();
		public RegexOptionalExpression Clone()
		{
			return new RegexOptionalExpression(Expression);
		}
		#region Value semantics
		public bool Equals(RegexOptionalExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Equals(Expression, rhs.Expression);
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexOptionalExpression);

		public override int GetHashCode()
		{
			if (null != Expression)
				return Expression.GetHashCode();
			return 0;
		}

		public static bool operator ==(RegexOptionalExpression lhs, RegexOptionalExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(RegexOptionalExpression lhs, RegexOptionalExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
