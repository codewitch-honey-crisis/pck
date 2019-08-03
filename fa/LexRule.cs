using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class LexRule : LexNode, IEquatable<LexRule>, ICloneable
	{
		public LexRule(string left,RegexExpression right)
		{
			Left = left;
			Right = right;
		}
		public LexRule(string left, string right)
		{
			Left = left;
			Right = RegexExpression.Parse(right);
		}
		public LexRule() { }
		public string Left { get; set; } = null;
		public RegexExpression Right { get; set; } = null;

		public override string ToString()
		{
			return string.Concat(Left, "= \'", string.Concat(Right, "\'"));
		}
		public LexRule Clone()
		{
			return new LexRule(Left, Right);
		}
		object ICloneable.Clone()
			=> Clone();

		#region Value semantics
		public bool Equals(LexRule rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Left == rhs.Left && Right==rhs.Right;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as LexRule);

		public override int GetHashCode()
		{
			var result = 0;
			if (null != Left)
				result ^= Left.GetHashCode();
			if (null != Right)
				result ^= Right.GetHashCode();
			return result;
		}
		public static bool operator ==(LexRule lhs, LexRule rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(LexRule lhs, LexRule rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion

	}
}
