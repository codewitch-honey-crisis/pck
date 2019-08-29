using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class XbnfConcatExpression : XbnfBinaryExpression
	{
		public XbnfConcatExpression(XbnfExpression left, params XbnfExpression[] right)
		{
			Left = left;
			for(int i = 0;i<right.Length;i++)
			{
				var r = right[i];
				if (null == Right)
					Right = r;
				
				var c = new XbnfConcatExpression();
				c.Left = Left;
				c.Right = Right;
				Right = null;
				Left = c;
				
			}
		}
		public XbnfConcatExpression()
		{

		}
		protected override XbnfExpression CloneImpl()
			=>Clone();
		
		public new XbnfConcatExpression Clone()
		{
			var left = null != Left ? Left.Clone() : null;
			var right = null != Right ? Right.Clone() : null;
			return new XbnfConcatExpression(left, right);
		}
		public override string ToString()
		{
			if (null == Left)
				return null == Right ? "" : Right.ToString();
			else if (null == Right)
				return Left.ToString();
			var l = Left.ToString();
			var r = Right.ToString();
			var oe = Left as XbnfOrExpression;
			if (null != oe)
				l = string.Concat("( ", l, " )");
			oe = Right as XbnfOrExpression;
			if(null!=oe)
				r = string.Concat("( ", r, " )");
			return string.Concat(l, " ", r);
		}
		#region Value semantics
		public bool Equals(XbnfConcatExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Left == rhs.Left && Right == rhs.Right;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as XbnfConcatExpression);

		public override int GetHashCode()
		{
			var result = 0;
			if (null != Left)
				result ^= Left.GetHashCode();
			if (null != Right)
				result ^= Right.GetHashCode();

			return 0;
		}
		public static bool operator ==(XbnfConcatExpression lhs, XbnfConcatExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(XbnfConcatExpression lhs, XbnfConcatExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
