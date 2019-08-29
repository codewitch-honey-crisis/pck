using System;
using System.Collections.Generic;

namespace Pck
{
	public abstract class XbnfExpression : XbnfNode,ICloneable
	{
		public XbnfExpression Clone()
			=>CloneImpl();
		object ICloneable.Clone()
			=> CloneImpl();
		protected abstract XbnfExpression CloneImpl();
		public abstract bool IsTerminal {get;}
		internal static XbnfExpression Parse(ParseContext pc)
		{
			XbnfExpression current = null;
			XbnfExpression e;
			long position;
			int line;
			int column;
			pc.TrySkipCCommentsAndWhiteSpace();
			position = pc.Position; line = pc.Line; column = pc.Column;
			while (-1 != pc.Current && ']' != pc.Current && ')' != pc.Current && '}' != pc.Current && ';' != pc.Current)
			{
				pc.TrySkipCCommentsAndWhiteSpace();
				position = pc.Position; line = pc.Line; column = pc.Column;
				switch (pc.Current)
				{
					case '|':
						pc.Advance();
						position = pc.Position; line = pc.Line; column = pc.Column;
						current = new XbnfOrExpression(current, Parse(pc));
						current.SetLocation(line, column, position);
						break;
					case '(':
						pc.Advance();
						position = pc.Position; line = pc.Line; column = pc.Column;
						e = Parse(pc);
						current.SetLocation(line, column, position);
						pc.Expecting(')');
						pc.Advance();
						e.SetLocation(line, column, position);
						if (null == current)
							current = e;
						else
							current = new XbnfConcatExpression(current, e);

						break;
					case '[':
						pc.Advance();
						e = new XbnfOptionalExpression(Parse(pc));
						e.SetLocation(line, column, position);
						pc.TrySkipCCommentsAndWhiteSpace();
						pc.Expecting(']');
						pc.Advance();
						if (null == current)
							current = e;
						else
							current = new XbnfConcatExpression(current, e);

						break;
					case '{':
						pc.Advance();
						e = new XbnfRepeatExpression(Parse(pc));
						e.SetLocation(line, column, position);
						pc.TrySkipCCommentsAndWhiteSpace();
						pc.Expecting('}');
						pc.Advance();
						if ('+' == pc.Current)
						{
							pc.Advance();
							((XbnfRepeatExpression)e).IsOptional = false;
						}
						if (null == current)
							current = e;
						else
							current = new XbnfConcatExpression(current, e);

						break;

					case '\"':
						e = new XbnfLiteralExpression(pc.ParseJsonString());
						if (null == current)
							current = e;
						else
							current = new XbnfConcatExpression(current, e);
						e.SetLocation(line, column, position);
						break;

					case '\'':
						pc.Advance();
						pc.ClearCapture();
						pc.TryReadUntil('\'', '\\', false);
						pc.Expecting('\'');
						pc.Advance();
						e = new XbnfRegexExpression(pc.GetCapture());
						if (null == current)
							current = e;
						else
							current = new XbnfConcatExpression(current, e);
						e.SetLocation(line, column, position);
						break;
					case ';':
					case ']':
					case ')':
					case '}':
						return current;

					default:
						e = new XbnfRefExpression(ParseIdentifier(pc));
						if (null == current)
							current = e;
						else
							current = new XbnfConcatExpression(current, e);
						e.SetLocation(line, column, position);
						break;
				}
			}
			pc.TrySkipCCommentsAndWhiteSpace();
			return current;
		}
	}
}
