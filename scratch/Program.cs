using System;
using System.Collections.Generic;

namespace Pck
{
	class Program
	{
		static void Main(string[] args)
		{
			
			var text = "[a-z_A-Z]+(a|b|c)[a-z_A-Z0-9]*";
			Console.WriteLine(text);
			var ast = RegexExpression.Parse(text);
			var fa = ast.ToFA<string>();
			Console.WriteLine(ast.ToFA<string>().Reduce().FillClosure().Count);
			Console.WriteLine(ast);
			return;
		}
		static IList<RegexCharsetEntry> _ParseRanges(ParseContext pc)
		{
			pc.EnsureStarted();
			var result = new List<RegexCharsetEntry>();
			RegexCharsetEntry next = null;
			bool readDash = false;
			while (-1!=pc.Current && ']'!=pc.Current)
			{
				switch (pc.Current)
				{
					case '[': // char class 
						if (null!=next)
						{
							result.Add(next);
							if (readDash)
								result.Add(new RegexCharsetCharEntry('-'));
							result.Add(new RegexCharsetCharEntry('-'));
						}
						pc.Advance();
						pc.Expecting(':');
						pc.Advance();
						var l = pc.CaptureBuffer.Length;
						pc.TryReadUntil(':', false);
						var n = pc.GetCapture(l);
						pc.Advance();
						pc.Expecting(']');
						pc.Advance();
						result.Add(new RegexCharsetClassEntry(n));
						readDash = false;
						next = null;
						break;
					case '\\':
						pc.Advance();
						pc.Expecting();
						var ch = (char)_ParseEscape(pc);
						if (null==next)
						{
							next = new RegexCharsetCharEntry(ch);
						}
						else
						{
							if (readDash)
							{
								result.Add(new RegexCharsetRangeEntry(((RegexCharsetCharEntry)next).Value, ch));
								next = null;
								readDash = false;
							}
							else
							{
								result.Add(next);
								next = new RegexCharsetCharEntry(ch);
							}
						}
						break;
					case '-':
						pc.Advance();
						if (null == next)
						{
							next = new RegexCharsetCharEntry('-');
							readDash = false;
						}
						else
						{
							if(readDash)
								result.Add(next);
							readDash = true;
						}
						break;
					default:
						if (null == next)
						{
							next = new RegexCharsetCharEntry((char)pc.Current);
						}
						else
						{
							if (readDash)
							{
								result.Add(new RegexCharsetRangeEntry(((RegexCharsetCharEntry)next).Value, (char)pc.Current));
								next = null;
								readDash = false;
							}
							else
							{
								result.Add(next);
								next = new RegexCharsetCharEntry((char)pc.Current);
							}
						}
						pc.Advance();
						break;
				}
			}
			return result;
		}
		static byte _FromHexChar(char hex)
		{
			if (':' > hex && '/' < hex)
				return (byte)(hex - '0');
			if ('G' > hex && '@' < hex)
				return (byte)(hex - '7'); // 'A'-10
			if ('g' > hex && '`' < hex)
				return (byte)(hex - 'W'); // 'a'-10
			throw new ArgumentException("The value was not hex.", "hex");
		}
		static bool _IsHexChar(char hex)
		{
			return (':' > hex && '/' < hex) ||
				('G' > hex && '@' < hex) ||
				('g' > hex && '`' < hex);
		}
		static int _ParseEscape(ParseContext pc)
		{
			if ('\\' != pc.Current)
				return -1;
			if (-1 == pc.Advance())
				return -1;
			switch (pc.Current)
			{
				case 't':
					pc.Advance();
					return '\t';
				case 'n':
					pc.Advance();
					return '\n';
				case 'r':
					pc.Advance();
					return '\r';
				case 'x':
					if (-1 == pc.Advance() || !_IsHexChar((char)pc.Current))
						return 'x';
					byte b = _FromHexChar((char)pc.Current);
					if (-1 == pc.Advance() || !_IsHexChar((char)pc.Current))
						return unchecked((char)b);
					b <<= 4;
					b |= _FromHexChar((char)pc.Current);
					if (-1 == pc.Advance() || !_IsHexChar((char)pc.Current))
						return unchecked((char)b);
					b <<= 4;
					b |= _FromHexChar((char)pc.Current);
					if (-1 == pc.Advance() || !_IsHexChar((char)pc.Current))
						return unchecked((char)b);
					b <<= 4;
					b |= _FromHexChar((char)pc.Current);
					return unchecked((char)b);
				case 'u':
					if (-1 == pc.Advance())
						return 'u';
					ushort u = _FromHexChar((char)pc.Current);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked((char)u);
					u |= _FromHexChar((char)pc.Current);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked((char)u);
					u |= _FromHexChar((char)pc.Current);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked((char)u);
					u |= _FromHexChar((char)pc.Current);
					return unchecked((char)u);
				default:
					int i = pc.Current;
					pc.Advance();
					return (char)i;
			}
		}
	}
}
