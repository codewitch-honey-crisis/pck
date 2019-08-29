using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class RegexConcatExpression : RegexBinaryExpression, IEquatable<RegexConcatExpression>
	{
		public override bool IsSingleElement {
			get {
				if (null == Left)
					return null == Right ? false : Right.IsSingleElement;
				else if (null == Right)
					return Left.IsSingleElement;
				return false;
			}
		}
		public RegexConcatExpression(RegexExpression left, params RegexExpression[] right)
		{
			Left = left;
			for (int i = 0; i < right.Length; i++)
			{
				var r = right[i];
				if (null == Right)
					Right = r;
				
				var c = new RegexConcatExpression();
				c.Left = Left;
				c.Right = Right;
				Right = null;
				Left = c;
				
			}
		}
		public RegexConcatExpression() { }
		public override CharFA<TAccept> ToFA<TAccept>(TAccept accept)
		{
			if (null == Left)
				return (null != Right) ? Right.ToFA(accept) : null;
			else if (null == Right)
				return Left.ToFA(accept);
			return CharFA<TAccept>.Concat(new CharFA<TAccept>[] { Left.ToFA(accept), Right.ToFA(accept) }, accept);
		}
		protected override RegexExpression CloneImpl()
			=> Clone();
		public RegexConcatExpression Clone()
		{
			return new RegexConcatExpression(Left, Right);
		}
		#region Value semantics
		public bool Equals(RegexConcatExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Left == rhs.Left && Right == rhs.Right;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexConcatExpression);

		public override int GetHashCode()
		{
			var result = 0;
			if (null != Left)
				result ^= Left.GetHashCode();
			if (null != Right)
				result ^= Right.GetHashCode();
			return result;
		}
		public static bool operator ==(RegexConcatExpression lhs, RegexConcatExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(RegexConcatExpression lhs, RegexConcatExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
		protected internal override void AppendTo(StringBuilder sb)
		{
			if (null != Left)
			{
				var oe = Left as RegexOrExpression;
				if (null != oe)
					sb.Append('(');
				Left.AppendTo(sb);
				if (null != oe)
					sb.Append(')');
			}
			if (null != Right)
			{
				var oe = Right as RegexOrExpression;
				if (null != oe)
					sb.Append('(');
				Right.AppendTo(sb);
				if (null != oe)
					sb.Append(')');
			}
		}
	}
}
