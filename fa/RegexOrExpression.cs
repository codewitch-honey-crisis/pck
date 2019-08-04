using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class RegexOrExpression : RegexBinaryExpression, IEquatable<RegexOrExpression>
	{
		public override bool IsSingleElement => false;
		public RegexOrExpression(RegexExpression left, params RegexExpression[] right)
		{
			Left = left;
			for (int i = 0; i < right.Length; i++)
			{
				var r = right[i];
				if (null == Right)
					Right = r;
				else
				{
					var c = new RegexOrExpression();
					c.Left = Left;
					c.Right = Right;
					Right = null;
					Left = c;
				}
			}
		}
		public RegexOrExpression() { }
		public override CharFA<TAccept> ToFA<TAccept>(TAccept accept)
		{
			var left = (null != Left) ? Left.ToFA(accept) : null;
			var right = (null != Right) ? Right.ToFA(accept) : null;
			return CharFA<TAccept>.Or(new CharFA<TAccept>[] { left, right }, accept);
		}
		protected internal override void AppendTo(StringBuilder sb)
		{
			if (null != Left)
				Left.AppendTo(sb);
			sb.Append('|');
			if (null != Right)
				Right.AppendTo(sb);
		}
		protected override RegexExpression CloneImpl()
			=> Clone();
		public RegexOrExpression Clone()
		{
			return new RegexOrExpression(Left, Right);
		}
		#region Value semantics
		public bool Equals(RegexOrExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return (Left == rhs.Left && Right == rhs.Right) ||
				(Left == rhs.Right && Right == rhs.Left);
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexOrExpression);

		public override int GetHashCode()
		{
			var result = 0;
			if (null != Left)
				result ^= Left.GetHashCode();
			if (null != Right)
				result ^= Right.GetHashCode();
			return result;
		}
		public static bool operator ==(RegexOrExpression lhs, RegexOrExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(RegexOrExpression lhs, RegexOrExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion

	}
}
