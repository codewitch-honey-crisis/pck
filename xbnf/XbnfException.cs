using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public sealed class XbnfException : Exception
	{
		public IList<XbnfMessage> Messages { get; }
		public XbnfException(string message, int errorCode = -1, int line = 0, int column = 0, long position = -1) :
			this(new XbnfMessage[] { new XbnfMessage(ErrorLevel.Error, errorCode, message, line, column, position) })
		{ }
		static string _FindMessage(IEnumerable<XbnfMessage> messages)
		{
			var l = new List<XbnfMessage>(messages);
			if (null == messages) return "";
			int c = 0;
			foreach (var m in l)
			{
				if (ErrorLevel.Error == m.ErrorLevel)
				{
					if (1 == l.Count)
						return m.ToString();
					return string.Concat(m, " (multiple messages)");
				}
				++c;
			}
			foreach (var m in messages)
				return m.ToString();
			return "";
		}
		public XbnfException(IEnumerable<XbnfMessage> messages) : base(_FindMessage(messages))
		{
			Messages = new List<XbnfMessage>(messages);
		}
		public static void ThrowIfErrors(IEnumerable<XbnfMessage> messages)
		{
			if (null == messages) return;
			foreach (var m in messages)
				if (ErrorLevel.Error == m.ErrorLevel)
					throw new XbnfException(messages);
		}
	}
}
