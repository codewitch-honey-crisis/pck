using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class XbnfLiteralExpression : XbnfExpression,IEquatable<XbnfLiteralExpression>
	{
		public override bool IsTerminal => true;
		public XbnfLiteralExpression(string value) { Value = value; }
		public XbnfLiteralExpression() { }
		public string Value { get; set; } = null;
		protected override XbnfExpression CloneImpl()
			=> Clone();
		public new XbnfLiteralExpression Clone()
		{
			return new XbnfLiteralExpression(Value);
		}
		public override string ToString()
		{
			if (null == Value) return "";
			var sb = new StringBuilder();
			sb.Append("\"");
			for (int i = 0; i < Value.Length; i++)
				Escape(Value[i], sb);
			sb.Append("\"");
			return sb.ToString();
		}
		

		#region Value semantics
		public bool Equals(XbnfLiteralExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Value == rhs.Value;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as XbnfLiteralExpression);

		public override int GetHashCode()
		{
			if (null != Value)
				return Value.GetHashCode();
			return 0;
		}
		public static bool operator ==(XbnfLiteralExpression lhs, XbnfLiteralExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(XbnfLiteralExpression lhs, XbnfLiteralExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion

	}
}
