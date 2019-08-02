using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class XbnfRegexExpression : XbnfExpression, IEquatable<XbnfRegexExpression>
	{
		public override bool IsTerminal => true;
		public XbnfRegexExpression(string value) { Value = value; }
		public XbnfRegexExpression() { }
		public string Value { get; set; } = null;
		protected override XbnfExpression CloneImpl()
			=> Clone();
		public new XbnfRegexExpression Clone()
		{
			return new XbnfRegexExpression(Value);
		}
		public override string ToString()
		{
			return string.Concat("\'", Value, "\'");
		}

		#region Value semantics
		public bool Equals(XbnfRegexExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Value == rhs.Value;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as XbnfRegexExpression);

		public override int GetHashCode()
		{
			if (null != Value)
				return Value.GetHashCode();
			return 0;
		}
		public static bool operator ==(XbnfRegexExpression lhs, XbnfRegexExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(XbnfRegexExpression lhs, XbnfRegexExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion

	}
}
