using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public enum XbnfErrorLevel
	{
		Message = 0,
		Warning = 1,
		Error = 2
	}

	public sealed class XbnfMessage
	{
		public XbnfMessage(XbnfErrorLevel errorLevel, int errorCode, string message, int line, int column, long position)
		{
			ErrorLevel = errorLevel;
			ErrorCode = errorCode;
			Message = message;
			Line = line;
			Column = column;
			Position = position;
		}
		public XbnfErrorLevel ErrorLevel { get; private set; }
		public int ErrorCode { get; private set; }
		public string Message { get; private set; }
		public int Line { get; private set; }
		public int Column { get; private set; }
		public long Position { get; private set; }

		public override string ToString()
		{
			if (-1 == Position)
			{
				if (-1 != ErrorCode)
					return string.Format("{0}: {1} code {2}",
						ErrorLevel, Message, ErrorCode);
				return string.Format("{0}: {1}",
						ErrorLevel, Message);
			}
			else
			{
				if (-1 != ErrorCode)
					return string.Format("{0}: {1} code {2} at line {3}, column {4}, position {5}",
						ErrorLevel, Message, ErrorCode, Line, Column, Position);
				return string.Format("{0}: {1} at line {2}, column {3}, position {4}",
						ErrorLevel, Message, Line, Column, Position);
			}
		}
	}
}
