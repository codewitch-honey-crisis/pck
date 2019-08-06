using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class CfgAttribute : CfgNode, IEquatable<CfgAttribute>, ICloneable
	{
		public CfgAttribute(string name, object value)
		{
			Name = name;
			Value = value;
		}
		public CfgAttribute() { }
		public string Name { get; set; } = null;
		public object Value { get; set; } = null;

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
						_EscAttrValChar(s[i], sb);
					sb.Append("\"");
				}
				else
					sb.Append(Value);
			}
			return sb.ToString();
		}
		public CfgAttribute Clone()
		{
			return new CfgAttribute(Name, Value);
		}
		object ICloneable.Clone()
			=> Clone();
		static void _EscAttrValChar(char ch, StringBuilder builder)
		{
			switch (ch)
			{
				case '\\':
					builder.Append('\\');
					builder.Append(ch);
					return;
				case '\t':
					builder.Append("\\t");
					return;
				case '\n':
					builder.Append("\\n");
					return;
				case '\r':
					builder.Append("\\r");
					return;
				case '\0':
					builder.Append("\\0");
					return;
				case '\f':
					builder.Append("\\f");
					return;
				case '\v':
					builder.Append("\\v");
					return;
				case '\b':
					builder.Append("\\b");
					return;
				default:
					if (!char.IsLetterOrDigit(ch) && !char.IsSeparator(ch) && !char.IsPunctuation(ch) && !char.IsSymbol(ch))
					{

						builder.Append("\\u");
						builder.Append(unchecked((ushort)ch).ToString("x4"));

					}
					else
						builder.Append(ch);
					break;
			}
			
		}
		static string _ParseAttrName(ParseContext pc)
		{
			var l = pc.CaptureBuffer.Length;
			pc.TryReadUntil(false, '(', ')', '[', ']', '{', '}', '<', '>', ',', ':', ';', '=', '|', '/', '\'', '\"', ' ', '\t', '\r', '\n', '\f', '\v');
			return pc.GetCapture(l);
		}
		internal static CfgAttribute Parse(ParseContext pc)
		{
			SkipCommentsAndWhitespace(pc);
			var attr = new CfgAttribute();
			attr.SetLocation(pc.Line, pc.Column, pc.Position);
			attr.Name = _ParseAttrName(pc);
			SkipCommentsAndWhitespace(pc);
			pc.Expecting(',', '=', ',', '>','\n');
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
		public bool Equals(CfgAttribute rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Name == rhs.Name && Equals(Value, rhs.Value);
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as CfgAttribute);

		public override int GetHashCode()
		{
			if (null != Value)
				return Value.GetHashCode();
			return 0;
		}
		public static bool operator ==(CfgAttribute lhs, CfgAttribute rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(CfgAttribute lhs, CfgAttribute rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion

	}
}
