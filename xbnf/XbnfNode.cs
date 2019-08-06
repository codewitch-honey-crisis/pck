using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public abstract class XbnfNode
	{
		public int Line { get; set; } = 1;
		public int Column { get; set; } = 1;
		public long Position { get; set; } = 0L;
		public void SetLocation(int line,int column,long position)
		{
			Line = line;
			Column = column;
			Position = position;
		}
		internal static void SkipCommentsAndWhitespace(ParseContext pc)
		{

		}
		internal static string ParseIdentifier(ParseContext pc)
		{
			var l = pc.CaptureBuffer.Length;
			pc.TryReadUntil(false, '(', ')', '[', ']', '{', '}', '<', '>',',',':', ';','-', '=', '|','/', '\'', '\"', ' ', '\t', '\r', '\n', '\f', '\v');
			return pc.GetCapture(l);
		}
		public static string Escape(string str)
		{
			if (string.IsNullOrEmpty(str))
				return str;
			var sb = new StringBuilder();
			for(var i = 0;i<str.Length;++i)
				Escape(str[i], sb);
			return sb.ToString();
		}
		internal static void Escape(char ch, StringBuilder builder)
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

	}
}
