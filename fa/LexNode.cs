using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public abstract class LexNode
	{
		public int Line { get; set; } = 1;
		public int Column { get; set; } = 1;
		public long Position { get; set; } = 0L;

		public void SetLocation(int line, int column, long position)
		{
			Line = line;
			Column = column;
			Position = position;
		}
		internal static string ParseIdentifier(ParseContext pc)
		{
			var l = pc.CaptureBuffer.Length;
			pc.TryReadUntil(false, '(', ')', '[', ']', '{', '}', '<', '>', ',', ':', ';','-', '=', '|', '/', '\'', '\"', ' ', '\t', '\r', '\n', '\f', '\v');
			return pc.GetCapture(l);
		}
		static bool _SkipWhiteSpace(ParseContext pc)
		{
			pc.EnsureStarted();
			if (-1 == pc.Current || '\n'==pc.Current || !char.IsWhiteSpace((char)pc.Current))
				return false;
			while (-1 != pc.Advance() && '\n'!=pc.Current && char.IsWhiteSpace((char)pc.Current)) ;
			return true;
		}
		internal static void SkipCommentsAndWhitespace(ParseContext pc)
		{
			while (-1 != pc.Current)
				if (!_SkipWhiteSpace(pc) && !pc.TrySkipCLineComment())
					break;
			_SkipWhiteSpace(pc);
		}
	}
}
