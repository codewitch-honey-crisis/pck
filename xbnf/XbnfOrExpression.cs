using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class XbnfOrExpression : XbnfBinaryExpression, IEquatable<XbnfOrExpression>
	{
		public XbnfOrExpression(XbnfExpression left, params XbnfExpression[] right)
		{
			Left = left;
			for (int i = 0; i < right.Length; i++)
			{
				var r = right[i];
				if (null == Right)
					Right = r;
				else
				{
					var c = new XbnfOrExpression();
					c.Left = Left;
					c.Right = Right;
					Right = null;
					Left = c;
				}
			}
		}
		public XbnfOrExpression()
		{

		}
		protected override XbnfExpression CloneImpl()
			=> Clone();

		public new XbnfOrExpression Clone()
		{
			var left = null != Left ? Left.Clone() : null;
			var right = null != Right ? Right.Clone() : null;
			return new XbnfOrExpression(left, right);
		}
		public override string ToString()
		{
			if (null == Left)
				return null == Right ? " | " : string.Concat(" | ", Right.ToString());
			else if (null == Right)
				return string.Concat(Left.ToString(), " | ");
			return string.Concat(Left.ToString(), " | ", Right.ToString());
		}

		#region Value semantics
		public bool Equals(XbnfOrExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return (Left == rhs.Left && Right == rhs.Right) ||
				(Right == rhs.Left && Left == rhs.Right);
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as XbnfOrExpression);

		public override int GetHashCode()
		{
			var result = 0;
			if (null != Left)
				result ^= Left.GetHashCode();
			if (null != Right)
				result ^= Right.GetHashCode();

			return 0;
		}
		public static bool operator ==(XbnfOrExpression lhs, XbnfOrExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(XbnfOrExpression lhs, XbnfOrExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
