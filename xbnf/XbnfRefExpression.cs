using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class XbnfRefExpression : XbnfExpression,IEquatable<XbnfRefExpression>
	{
		public override bool IsTerminal => false;
		public string Symbol { get; set; } = null;
		public XbnfRefExpression(string symbol) { Symbol = symbol; }
		public XbnfRefExpression() { }
		protected override XbnfExpression CloneImpl()
			=> Clone();
		public new XbnfRefExpression Clone()
		{
			return new XbnfRefExpression(Symbol);
		}
		public override string ToString()
		{
			if (null == Symbol) return "";
			return Symbol;
		}

		#region Value semantics
		public bool Equals(XbnfRefExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs,null)) return false;
			return Symbol == rhs.Symbol;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as XbnfRefExpression);
		
		public override int GetHashCode()
		{
			if (null != Symbol)
				return Symbol.GetHashCode();
			return 0;
		}
		public static bool operator==(XbnfRefExpression lhs,XbnfRefExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(XbnfRefExpression lhs, XbnfRefExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion

	}
}
