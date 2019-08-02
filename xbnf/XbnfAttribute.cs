using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class XbnfAttribute : XbnfNode, IEquatable<XbnfAttribute>,ICloneable
	{
		public XbnfAttribute(string name,object value)
		{
			Name = name;
			Value = value;
		}
		public XbnfAttribute() { }
		public string Name { get; set; } = null;
		public object Value { get; set; } = null;
		public XbnfAttribute Clone()
		{
			return new XbnfAttribute(Name, Value);
		}
		object ICloneable.Clone()
			=> Clone();
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append(Name);
			if (!(Value is bool) || !(bool)Value)
			{
				sb.Append("= ");
				var s = Value as string;
				if (null != s)
				{
					sb.Append("\"");
					for (var i = 0; i < s.Length; i++)
						Escape(s[i], sb);
					sb.Append("\"");
				}
				else
					sb.Append(Value);
			}
			return sb.ToString();
		}

		internal static XbnfAttribute Parse(ParseContext pc)
		{
			pc.TrySkipCCommentsAndWhiteSpace();
			var attr = new XbnfAttribute();
			attr.SetLocationInfo(pc.Line, pc.Column, pc.Position);
			attr.Name=ParseIdentifier(pc);
			pc.TrySkipCCommentsAndWhiteSpace();
			pc.Expecting(',', '=', ',', '>');
			if ('=' == pc.Current)
			{
				pc.Advance();
				attr.Value = pc.ParseJsonValue();
			}
			else
				attr.Value = true;
			return attr;
		}

		#region Value semantics
		public bool Equals(XbnfAttribute rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Name==rhs.Name && Equals(Value ,rhs.Value);
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as XbnfAttribute);

		public override int GetHashCode()
		{
			if (null != Value)
				return Value.GetHashCode();
			return 0;
		}
		public static bool operator ==(XbnfAttribute lhs, XbnfAttribute rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(XbnfAttribute lhs, XbnfAttribute rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion

	}
}
