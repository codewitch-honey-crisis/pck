using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class XbnfOptionalExpression : XbnfUnaryExpression,IEquatable<XbnfOptionalExpression>
	{
		public override bool IsTerminal => false;
		public XbnfOptionalExpression(XbnfExpression expression) { Expression = expression; }
		public XbnfOptionalExpression() { }
		protected override XbnfExpression CloneImpl()
			=> Clone();

		public new XbnfOptionalExpression Clone()
		{
			return new XbnfOptionalExpression(null != Expression ? Expression.Clone() : null);
		}
		public override string ToString()
		{
			if (null == Expression) return "";
			return string.Concat("[ ", Expression.ToString(), " ]");
		}

		#region Value semantics
		public bool Equals(XbnfOptionalExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Expression == rhs.Expression;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as XbnfOptionalExpression);

		public override int GetHashCode()
		{
			if (null != Expression)
				return Expression.GetHashCode();
			return 0;
		}
		public static bool operator ==(XbnfOptionalExpression lhs, XbnfOptionalExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(XbnfOptionalExpression lhs, XbnfOptionalExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
