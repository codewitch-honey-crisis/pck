using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class XbnfRepeatExpression : XbnfUnaryExpression, IEquatable<XbnfRepeatExpression>
	{
		public override bool IsTerminal => false;
		public bool IsOptional { get; set; } = true;
		public XbnfRepeatExpression(XbnfExpression expression, bool isOptional = true)
		{
			Expression = expression;
			IsOptional = isOptional;
		}
		public XbnfRepeatExpression() { }

		protected override XbnfExpression CloneImpl()
			=> Clone();
		public new XbnfRepeatExpression Clone()
		{
			return new XbnfRepeatExpression(null!=Expression?Expression.Clone():null,IsOptional);
		}
		public override string ToString()
		{
			if (null == Expression) return "";
			if(IsOptional)
				return string.Concat("{ ", Expression.ToString(), " }");
			else
				return string.Concat("{ ", Expression.ToString(), " }+");
		}

		#region Value semantics
		public bool Equals(XbnfRepeatExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return IsOptional==rhs.IsOptional && Expression == rhs.Expression;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as XbnfRepeatExpression);

		public override int GetHashCode()
		{
			if (null != Expression)
				return Expression.GetHashCode() ^ IsOptional.GetHashCode();
			return IsOptional.GetHashCode();
		}
		public static bool operator ==(XbnfRepeatExpression lhs, XbnfRepeatExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(XbnfRepeatExpression lhs, XbnfRepeatExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
